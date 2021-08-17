using System;
using System.Collections.Generic;
using System.Globalization;


namespace Titalyver2
{
    public class LineBreakWord
    {
		public static string[] SepalateWord(string text)
        {
			if (text.Length == 0)
				return Array.Empty<string>();

			List<string> words = new();

			TextElementEnumerator erator = StringInfo.GetTextElementEnumerator(text);
			erator.MoveNext();
			string c1 = erator.GetTextElement();
			string link = c1;
			while (erator.MoveNext())
			{
				string c2 = erator.GetTextElement();
				if (IsLink(c1[^1],c2[0]))
                {
					c1 += c2;
                }
				else
                {
					words.Add(c1);
					c1 = c2;
                }
			}
			words.Add(c1);
			return words.ToArray();
		}

		public static string forcebreak = " \t\r\n　@*|/";

		public static string forcelink1 = "([{\"'<‘“（〔［｛〈《「『【";

		public static string forcelink2 = ",.;:)]}\"'>、。，．’”）〕］｝〉》」』】";

		public static string not_begin = "・：；？！ヽヾゝゞ〃々ー―～…‥っゃゅょッャュョぁぃぅぇぉァィゥェォ";

		public static string nextbreak1 = ")]};>!%&?";

		public static string numlink_nextbreak1 = ".,-:$\\";

		public static string prevbreak2 = "([{<";

		public static bool IsForceBreak(char c) { return forcebreak.Contains(c); }
		public static bool IsForceLink1(char c1) { return forcelink1.Contains(c1); }
		public static bool IsForceLink2(char c2) { return forcelink2.Contains(c2); }
		public static bool IsNotBegin(char c2) { return not_begin.Contains(c2); }
		public static bool IsNextBreak1(char c) { return nextbreak1.Contains(c); }
		public static bool IsNumLink_NextBreak1(char c2) { return numlink_nextbreak1.Contains(c2); }
		public static bool IsPrevBreak2(char c) { return prevbreak2.Contains(c); }

		public static bool IsLink(char c1, char c2)
		{
			if (IsForceBreak(c1) || IsForceBreak(c2))
				return false;

			if (IsForceLink1(c1) || IsForceLink2(c2))
				return true;

			//ASCII文字
			if (c1 <= 0x7F)
			{
				if (IsNextBreak1(c1))
					return false;
				if (IsPrevBreak2(c2))
					return false;
				if (IsNumLink_NextBreak1(c1))
				{
					if ('0' <= c2 && c2 <= '9')
						return true;
					else
						return false;
				}

				//その他のASCIIの連続は全部くっつける
				if (c2 <= 0x7F)
				{
					return true;
				}
				return false;
			}

			//その他の文字
			if (IsNotBegin(c2))
				return true;

			return false;
		}

	}
}
