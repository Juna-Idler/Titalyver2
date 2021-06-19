using System.Collections.Generic;

using System.Text.RegularExpressions;


namespace Titalyver2
{
    public class AtTagContainer
    {
        public string RubyParent { get; }
        public string RubyBegin { get; }
        public string RubyEnd { get; }
        public List<RubyString.RubyingWord> Ruby { get; } = new();

        public double Offset { get; }
        public Dictionary<string, string> OtherTags { get; } = new();

        public AtTagContainer(string text)
        {
            System.IO.StringReader sr = new(text);

            for (string line = sr.ReadLine(); line != null; line = sr.ReadLine())
            {
                Match match = Regex.Match(line, @"^@([^=]+)=(.*)$");
                if (match.Success)
                {
                    string name = match.Groups[1].Value.ToLower();
                    string value = match.Groups[2].Value;

                    if (name == "ruby")
                    {
                        Match m = Regex.Match(value, @"^\[([^\]]+)\]((\d+),(\d+))?\[([^\]]+)\]$");
                        if (m.Success)
                        {
                            Ruby.Add(new(m.Groups[1].Value, m.Groups[5].Value,
                                m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : 0,
                                m.Groups[4].Success ? int.Parse(m.Groups[4].Value) : 0));
                        }
                    }
                    else if (name == "ruby_parent")
                    {
                        RubyParent = value;
                    }
                    else if (name == "ruby_begin")
                    {
                        RubyBegin = value;
                    }
                    else if (name == "ruby_end")
                    {
                        RubyEnd = value;
                    }
                    else if (name == "ruby_set")
                    {
                        //ルビ指定にタグと紛らわしい[]を使わないとするなら一行で書ける @ruby_set=[｜][《][》]
                        Match m = Regex.Match(value, @"\[([^\]]+)\]\[([^\]]+)\]\[([^\]]+)\]$");
                        if (m.Success)
                        {
                            RubyParent = m.Groups[1].Value;
                            RubyBegin = m.Groups[2].Value;
                            RubyEnd = m.Groups[3].Value;
                        }
                    }
                    else if (name == "offset")
                    {
                        if (double.TryParse(value, out double result))
                        {
                            Offset = (result >= 10 || result <= -10) ? result / 1000 : result;
                        }
                    }
                    else
                    {
                        OtherTags.Add(name, value);
                    }
                }
                match = Regex.Match(line, @"^\[offset:([^\]]+)\]$");
                if (match.Success)
                {
                    //[]タグは知らんけど、[offset:ぐらいは読んでやってもいい
                    if (double.TryParse(match.Groups[1].Value, out double result))
                    {
                        Offset = (result >= 10 || result <= -10) ? result / 1000 : result;
                    }
                }
            }
        }

    }

}
