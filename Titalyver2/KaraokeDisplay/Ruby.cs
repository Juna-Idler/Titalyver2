
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;



namespace Titalyver2
{
    public class RubyString
    {
        public Unit this[int index] => Units[index];
        public Unit[] Units { get; }

        public RubyString(string text, string parent = "｜", string begin = "《", string end = "》", IEnumerable<RubyingWord> rubyings = null)
        {
            List<Unit> result = new();
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(begin) || string.IsNullOrEmpty(end))
            {
                result.Add(new(text));
            }
            else
            {
                string pattern = Regex.Escape(parent) + "(.+?)" + Regex.Escape(begin) + "(.+?)" + Regex.Escape(end);
                string target = text;
                do
                {
                    Match match = Regex.Match(target, pattern);
                    if (match.Success)
                    {
                        if (match.Index > 0)
                        {
                            result.Add(new(target[0..match.Index]));
                        }
                        result.Add(new(match.Groups[1].Value, match.Groups[2].Value));
                        target = target[(match.Index + match.Length)..];
                    }
                    else
                    {
                        result.Add(new(target));
                        break;
                    }
                } while (target.Length > 0);
            }
            
            if (rubyings != null)
            {
                string basestring = string.Join("", result.Select((u) => { return u.BaseText; }));
                foreach (RubyingWord rubying in rubyings)
                {
                    string serchtarget = basestring;
                    while (true)
                    {
                        int index = serchtarget.IndexOf(rubying.TargetText);
                        if (index < 0)
                        {
                            break;
                        }
                        serchtarget = serchtarget[(index + rubying.TargetText.Length)..];

                        int count = 0;
                        index += rubying.ParentOffset;
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (count > index)
                                break;
                            if (count + result[i].BaseText.Length <= index)
                            {
                                count += result[i].BaseText.Length;
                                continue;
                            }
                            if (result[i].HasRuby)
                                break;
                            if (index + rubying.ParentLength > count + result[i].BaseText.Length)
                                break;

                            int div1 = index - count;
                            int div2 = index - count + rubying.ParentLength;
                            string target = result[i].BaseText;

                            if (div1 > 0 && div2 < target.Length)
                            {
                                result.RemoveAt(i);
                                result.Insert(i, new(target[0..div1]));
                                result.Insert(i + 1, new(target[div1..div2], rubying.RubyText));
                                result.Insert(i + 2, new(target[div2..]));
                            }
                            else if (div1 == 0 && div2 < target.Length)
                            {
                                result.RemoveAt(i);
                                result.Insert(i, new(target[0..div2], rubying.RubyText));
                                result.Insert(i + 1, new(target[div2..]));
                            }
                            else if (div1 > 0 && div2 == target.Length)
                            {
                                result.RemoveAt(i);
                                result.Insert(i, new(target[0..div1]));
                                result.Insert(i + 1, new(target[div1..], rubying.RubyText));
                            }
                            else if (div1 == 0 && div2 == target.Length)
                            {
                                result[i] = new(target, rubying.RubyText);
                            }
                            break;
                        }
                    }
                }
            }

            Units = result.ToArray();
        }

        public struct Unit
        {
            public string BaseText { get; }
            public string RubyText { get; }
            public Unit(string baseText, string rubyText = null)
            {
                BaseText = baseText;
                RubyText = rubyText;
            }

            public bool HasRuby => RubyText != null;
            public bool NoRuby => RubyText == null;
        }


        public struct RubyingWord
        {
            public string TargetText { get; }
            public int ParentOffset { get; }
            public int ParentLength { get; }
            public string RubyText { get; }

            public RubyingWord(string target, string ruby, int offset = 0, int length = -1)
            {
                TargetText = target;
                RubyText = ruby;
                ParentOffset = offset;
                ParentLength = (length < 0 || length > TargetText.Length - ParentOffset) ? (TargetText.Length - ParentOffset) : length;
            }
        };

    }

}
