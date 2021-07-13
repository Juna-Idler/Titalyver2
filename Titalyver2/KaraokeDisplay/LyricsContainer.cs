using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

namespace Titalyver2
{

    public class LyricsContainer
    {
        public enum SyncMode { None = 0, Line = 1, Karaoke = 3 }

        public AtTagContainer AtTagContainer { get; }

        public Line[] Lines { get; }

        public SyncMode Sync { get; }

        public LyricsContainer(string lyrics)
        {
            AtTagContainer = new(lyrics);

            SyncMode sync = SyncMode.None;
            List<Line> lines = new();
            using StringReader sr = new(lyrics);
            for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
            {
                if (line.Length == 0 || line[0] == '@')
                    continue;
                Line l = new(line, AtTagContainer);
                if (l.StartTime >= 0)
                    lines.Add(l);
                sync |= l.Sync;
            }
            Sync = sync;
            lines.Add(new("[99:59.99]"));
            if (lines.First().StartTime != 0)
                lines.Insert(0, new Line("[00:00.00]"));
            Lines = lines.ToArray();
            for (int i = 0; i < Lines.Length - 1; i++)
            {
                if (Lines[i].EndTime < 0)
                {
                    Lines[i].EndTime = Lines[i].Words.Length > 0 ? Math.Max(Lines[i].Words[^1].EndTime, Lines[i + 1].StartTime) : Lines[i + 1].StartTime;
                }
            }
        }


        public class Line
        {
            public int StartTime { get; set; }
            public int EndTime { get; set; }

            public WordWithRuby[] Words { get; }

            public SyncMode Sync { get; }

            public string Text => string.Join("", Words.Select(w => w.Word.Text));
            public string PhoneticText => string.Join("", Words.Select(w => w.HasRuby ? w.Ruby.Text : w.Word.Text));

            public bool HasRuby => Words.Sum(w => Convert.ToInt32(w.HasRuby)) != 0;

            public struct WordWithRuby
            {
                public Word Word { get; }
                public Word Ruby { get; }
                public WordWithRuby(Word w, Word r = null)
                {
                    Word = w;
                    Ruby = r;
                    if (HasRuby)
                    {
                        if (Word.StartTimes[0] < 0)
                            Word.StartTimes[0] = Ruby.StartTimes[0];
                        else if (Ruby.StartTimes[0] < 0)
                            Ruby.StartTimes[0] = Word.StartTimes[0];
                        if (Word.EndTimes[^1] < 0)
                            Word.EndTimes[^1] = Ruby.EndTimes[^1];
                        else if (Ruby.EndTimes[^1] < 0)
                            Ruby.EndTimes[^1] = Word.EndTimes[^1];
                    }
                }
                public bool HasRuby => Ruby != null;
                public bool NoRuby => Ruby == null;
                public int StartTime
                {
                    get => NoRuby ? Word.StartTimes[0] : Math.Min(Word.StartTimes[0], Ruby.StartTimes[0]);
                    set
                    {
                        Word.StartTimes[0] = value;
                        if (HasRuby)
                            Ruby.StartTimes[0] = value;
                    }
                }

                public int EndTime
                {
                    get => NoRuby ? Word.EndTimes[^1] : Math.Max(Word.EndTimes[^1], Ruby.EndTimes[^1]);
                    set
                    {
                        Word.EndTimes[^1] = value;
                        if (HasRuby)
                            Ruby.EndTimes[^1] = value;
                    }
                }
                public void Complement()
                {
                    Word.Complement();
                    if (HasRuby)
                        Ruby.Complement();
                }

                public void GetFirstTime(out int count, out int time)
                {
                    if (HasRuby)
                        Ruby.GetFirstTime(out count, out time);
                    else
                        Word.GetFirstTime(out count, out time);
                }
                public void GetLastTime(out int count, out int time)
                {
                    if (HasRuby)
                        Ruby.GetLastTime(out count, out time);
                    else
                        Word.GetLastTime(out count, out time);
                }
            }


            public Line(string textline, AtTagContainer atTag = null)
            {
                List<WordWithRuby> words = new();
                if (atTag != null)
                {
                    RubyString rs = new(textline, atTag.RubyParent ?? "｜", atTag.RubyBegin ?? "《", atTag.RubyEnd ?? "》", atTag.Ruby);

                    foreach (RubyString.Unit u in rs.Units)
                    {
                        words.Add(new WordWithRuby(new Word(u.BaseText), u.HasRuby ? new Word(u.RubyText) : null));
                    }
                    for (int i = 0; i < words.Count; i++)
                    {
                        if (words[i].Word.Chars.Length == 0)
                        {
                            if (i + 1 < words.Count)
                            {
                                if (words[i + 1].StartTime < 0)
                                {
                                    WordWithRuby tmp = words[i + 1];
                                    tmp.StartTime = words[i].StartTime;
                                    words[i + 1] = tmp;
                                }
                            }
                            if (i - 1 >= 0 && words[i - 1].EndTime < 0)
                            {
                                if (i == words.Count - 1 || (words[i + 1].StartTime >= 0))
                                {
                                    WordWithRuby tmp = words[i - 1];
                                    tmp.EndTime = words[i].EndTime;
                                    words[i - 1] = tmp;
                                }
                            }
                        }
                    }
                }
                else
                {
                    words.Add(new WordWithRuby(new Word(textline)));
                }
                _ = words.RemoveAll(w => { return w.Word.Chars.Length == 0; });

                Words = words.ToArray();

                TimeTagElement[] elements = TimeTagElement.Parse(textline);
                StartTime = (elements[0].StartTime >= 0) ? elements[0].StartTime : Words[0].StartTime;
                EndTime = (elements.Length > 1 && elements[^1].Text == "" && elements[^2].Text == "") ? elements[^1].StartTime : -1;
                if (elements.Length == 1 && elements[0].StartTime >= 0)
                    Sync = SyncMode.Line;
                else if (elements.Length <= 1)
                    Sync = SyncMode.None;
                else
                    Sync = SyncMode.Karaoke;
            }

            public bool FullComplement()
            {
                if (Complement())
                {
                    foreach (WordWithRuby w in Words)
                    {
                        bool r = w.Word.FullComplement();
                        if (r && w.HasRuby)
                            return w.Ruby.FullComplement();
                        return r;
                    }
                }
                return false;
            }

            public bool Complement()
            {
                if (StartTime < 0 || EndTime < 0)
                    return false;
                if (Words.Length == 0)
                    return true;
                if (Words[0].StartTime < 0)
                    Words[0].StartTime = StartTime;
                if (Words[^1].EndTime < 0)
                    Words[^1].EndTime = EndTime;

                foreach (WordWithRuby w in Words) { w.Complement(); }

                for (int i = 0; i < Words.Length - 1; i++)
                {
                    if (Words[i].EndTime < 0 && Words[i + 1].StartTime < 0)
                    {
                        Words[i].GetLastTime(out int prev_count, out int prev_time);
                        int next_count = 0;
                        int next_time = 0;
                        for (int j = i + 1; j < Words.Length; j++)
                        {
                            Words[j].GetFirstTime(out int nc, out int nt);
                            next_count += nc;
                            if (nt >= 0)
                            {
                                next_time = nt;
                                break;
                            }
                        }
                        int divtime = (prev_time * next_count + next_time * prev_count) / (prev_count + next_count);
                        Words[i].EndTime = Words[i + 1].StartTime = divtime;
                    }
                    else
                    {
                        if (Words[i + 1].StartTime < 0)
                            Words[i + 1].StartTime = Words[i].EndTime;
                        else if (Words[i].EndTime < 0)
                            Words[i].EndTime = Words[i + 1].StartTime;
                    }
                }
                return true;
            }

        }


        public class Word
        {
            public string[] Chars { get; }
            public int[] StartTimes { get; }
            public int[] EndTimes{ get; }

            public string Text => string.Join("", Chars);

            public Word(string text)
            {
                TimeTagElement[] elements = TimeTagElement.Parse(text);

                int text_length = 0;
                int grapheme_length = 0;
                foreach (TimeTagElement e in elements)
                {
                    text_length += e.Text.Length;
                    grapheme_length += new StringInfo(e.Text).LengthInTextElements;
                }
                if (text_length == 0)
                {
                    Chars = Array.Empty<string>();
                    StartTimes = new int[] { elements[0].StartTime };
                    EndTimes = new int[] { elements[^1].StartTime };
                    return;
                }

                Chars = new string[grapheme_length];
                List<int> starts = Enumerable.Repeat(-1,grapheme_length + 2).ToList();
                List<int> ends = Enumerable.Repeat(-1, grapheme_length + 2).ToList();

                int text_pos = 0;
                for (int i = 0; i < elements.Length; i++)
                {
                    TimeTagElement e = elements[i];
                    if (e.Text.Length > 0)
                        starts[text_pos + 1] = e.StartTime;
                    else if (ends[text_pos] < 0)
                        ends[text_pos] = e.StartTime;

                    TextElementEnumerator erator = StringInfo.GetTextElementEnumerator(e.Text);
                    while (erator.MoveNext())
                    {
                        Chars[text_pos++] = erator.GetTextElement();
                    }
                }
                starts.RemoveAt(0); starts.RemoveAt(starts.Count - 1);
                ends.RemoveAt(0); ends.RemoveAt(ends.Count - 1);

                StartTimes = starts.ToArray();
                EndTimes = ends.ToArray();
            }
            public void Complement()
            {
                for (int i = 0; i < Chars.Length - 1; i++)
                {
                    if (EndTimes[i] < 0 && StartTimes[i + 1] >= 0)
                    {
                        EndTimes[i] = StartTimes[i + 1];
                        continue;
                    }
                    if (EndTimes[i] >= 0 && StartTimes[i + 1] < 0)
                        StartTimes[i + 1] = EndTimes[i];
                }
            }
            public bool FullComplement()
            {
                if (StartTimes[0] < 0 || EndTimes[^1] < 0)
                    return false;

                for (int i = 0; i < Chars.Length - 1; i++)
                {
                    if (EndTimes[i] < 0 && StartTimes[i + 1] < 0)
                    {
                        int prev_count = 1;
                        int prev_time = StartTimes[i];
                        int next_count = 1;
                        int next_time = EndTimes[^1];
                        for (int j = i + 2; j < Chars.Length; j++)
                        {
                            if (StartTimes[j] >= 0)
                            {
                                next_time = StartTimes[j];
                                break;
                            }
                            next_count++;
                        }
                        int divtime = (prev_time * next_count + next_time * prev_count) / (prev_count + next_count);
                        EndTimes[i] = StartTimes[i + 1] = divtime;
                    }
                    else
                    {
                        if (StartTimes[i + 1] < 0)
                            StartTimes[i + 1] = EndTimes[i];
                        else if (EndTimes[i] < 0)
                            EndTimes[i] = StartTimes[i + 1];
                    }
                }
                return true;
            }
            public void GetFirstTime(out int count ,out int time)
            {
                for (int i = 0; i < Chars.Length; i++)
                {
                    if (StartTimes[i] >= 0)
                    {
                        count = i;
                        time = StartTimes[i];
                        return;
                    }
                }
                count = Chars.Length;
                time = EndTimes[^1];
            }
            public void GetLastTime(out int count, out int time)
            {
                for (int i = Chars.Length - 1; i >= 0; i--)
                {
                    if (EndTimes[i] >= 0)
                    {
                        count = Chars.Length - i - 1;
                        time = EndTimes[i];
                        return;
                    }
                }
                count = Chars.Length;
                time = StartTimes[0];
            }


        }

    }
    public class TimeTagElement
    {
        public string Text { get; }
        public int StartTime { get; set; }

        public TimeTagElement(string timetagtext)
        {
            Match match = Regex.Match(timetagtext, @"^\[(\d+):(\d+)[:.](\d+)\](.*)$");
            if (match.Success)
            {
                double second = double.Parse(match.Groups[2].Value + '.' + match.Groups[3].Value);
                StartTime = int.Parse(match.Groups[1].Value) * 60000 + (int)(second * 1000);
                Text = match.Groups[4].Value;
            }
            else
            {
                StartTime = -1;
                Text = timetagtext;
            }
        }
        public const int MaxTime = (99 * 60 + 59) * 1000 + 990;

        public static TimeTagElement[] Parse(string text)
        {
            Match head = Regex.Match(text, @"^\[(\d+):(\d+)[:.](\d+)\]");
            string timetagtext = head.Success ? text : ("[00:00.00]" + text);
            MatchCollection mc = Regex.Matches(timetagtext, @"\[\d+:\d+[:.]\d+\].*?((?=\[\d+:\d+[:.]\d+\])|$)");
            TimeTagElement[] elements = mc.Select(m => new TimeTagElement(m.Value)).ToArray();
            if (!head.Success)
                elements[0].StartTime = -1;
            return elements;
        }
    }

}
