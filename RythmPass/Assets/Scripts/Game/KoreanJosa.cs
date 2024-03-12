using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dev
{
    public class KoreanJosa
    {
		public string Replace(string text)
		{
			var sb = new StringBuilder(text.Length);
			var matchCollection = m_Regex.Matches(text);
			int num = 0;
			if (text.IsNullOrEmpty())
			{
				return "";
			}
			foreach (Match match in matchCollection)
			{
				var josaPair = m_Patterns[match.Value];
				sb.Append(text, num, match.Index - num);
				if (match.Index > 0)
				{
					char c = text[match.Index - 1];
					int num2 = 1;
					while (c == '>')
					{
						while (c != '<')
						{
							num2++;
							if (match.Index - num2 < 0)
							{
								return text;
							}
							c = text[match.Index - num2];
						}
						if (c == '<')
						{
							num2++;
							if (match.Index - num2 < 0)
							{
								return text;
							}
							c = text[match.Index - num2];
						}
					}
					while (c == ')')
					{
						while (c != '(')
						{
							num2++;
							if (match.Index - num2 < 0)
							{
								return text;
							}
							c = text[match.Index - num2];
						}
						if (c == '(')
						{
							num2++;
							if (match.Index - num2 < 0)
							{
								return text;
							}
							c = text[match.Index - num2];
						}
					}
					if ((HasJong(c) && match.Value != "(으)로") || (HasJongExceptRieul(c) && match.Value == "(으)로"))
					{
						sb.Append(josaPair.Item1);
					}
					else
					{
						sb.Append(josaPair.Item2);
					}
				}
				else
				{
					sb.Append(josaPair.Item1);
				}
				num = match.Index + match.Length;
			}
			sb.Append(text, num, text.Length - num);
			return sb.ToString();
		}

		bool HasJong(char inChar)
		{
			return inChar >= '가' && inChar <= '힣' && (inChar - '가') % '\u001c' > '\0';
		}

		bool HasJongExceptRieul(char inChar)
		{
			if (inChar >= '가' && inChar <= '힣')
			{
				int num = ((inChar - '가') % '\u001c');
				return num != 8 && num != 0;
			}
			return false;
		}

		readonly Regex m_Regex = new("\\(이\\)가|\\(와\\)과|\\(을\\)를|\\(은\\)는|\\(아\\)야|\\(이\\)야|\\(이\\)여|\\(으\\)로|\\(이\\)라");
		readonly Dictionary<string, (string, string)> m_Patterns = new()
		{
			{
				"(이)가",
				new("이", "가")
			},
			{
				"(와)과",
				new("과", "와")
			},
			{
				"(을)를",
				new("을", "를")
			},
			{
				"(은)는",
				new("은", "는")
			},
			{
				"(아)야",
				new("아", "야")
			},
			{
				"(이)야",
				new("이야", "야")
			},
			{
				"(이)여",
				new("이여", "여")
			},
			{
				"(으)로",
				new("으로", "로")
			},
			{
				"(이)라",
				new("이라", "라")
			}
		};
	}
}
