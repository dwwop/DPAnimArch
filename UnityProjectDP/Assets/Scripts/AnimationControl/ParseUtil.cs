using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class ParseUtil
    {
        public static String RemoveWhitespace(String String)
        {
            StringBuilder FilteredStringBuilder = new StringBuilder();
            foreach (char c in String)
            {
                if (!Char.IsWhiteSpace(c))
                {
                    FilteredStringBuilder.Append(c);
                }
            }
            return FilteredStringBuilder.ToString();
        }

        public static String SqueezeWhiteSpace(String String)
        {
            if (String.Length <= 0)
            {
                return String;
            }

            int FirstNonWSIndex = 0;
            while (FirstNonWSIndex < String.Length && Char.IsWhiteSpace(String[FirstNonWSIndex]))
            {
                FirstNonWSIndex++;
            }
            int LastNonWSIndex = String.Length - 1;
            while (LastNonWSIndex > 0 && Char.IsWhiteSpace(String[LastNonWSIndex]))
            {
                LastNonWSIndex--;
            }
            // This means that the string is only whitespce
            if (LastNonWSIndex == 0 && FirstNonWSIndex != 0)
            {
                return "";
            }
            String TrimmedString = String.Substring(FirstNonWSIndex, LastNonWSIndex - FirstNonWSIndex + 1);

            StringBuilder FilteredStringBuilder = new StringBuilder();
            int WhitespaceCount = 0;
            Boolean InString = false;
            foreach (char c in TrimmedString)
            {
                if (c == '"')
                {
                    InString = !InString;
                    FilteredStringBuilder.Append(c);
                    continue;
                }

                if (InString)
                {
                    FilteredStringBuilder.Append(c);
                    continue;
                }

                if (!Char.IsWhiteSpace(c))
                {
                    WhitespaceCount = 0;
                    FilteredStringBuilder.Append(c);
                }
                else
                {
                    if (WhitespaceCount == 0)
                    {
                        FilteredStringBuilder.Append(' ');
                    }
                    WhitespaceCount++;
                }
            }
            return FilteredStringBuilder.ToString();
        }
        public static String StripWhiteSpace(String String)
        {
            if (String.Length <= 0)
            {
                return String;
            }

            int FirstNonWSIndex = 0;
            while (FirstNonWSIndex < String.Length && Char.IsWhiteSpace(String[FirstNonWSIndex]))
            {
                FirstNonWSIndex++;
            }
            int LastNonWSIndex = String.Length - 1;
            while (LastNonWSIndex > 0 && Char.IsWhiteSpace(String[LastNonWSIndex]))
            {
                LastNonWSIndex--;
            }
            // This means that the string is only whitespce
            if (LastNonWSIndex == 0 && FirstNonWSIndex != 0)
            {
                return "";
            }
            String TrimmedString = String.Substring(FirstNonWSIndex, LastNonWSIndex - FirstNonWSIndex + 1);

            StringBuilder FilteredStringBuilder = new StringBuilder();
            Boolean InString = false;
            Boolean Denoting = false;
            foreach (char c in TrimmedString)
            {
                if (Denoting)
                {
                    Denoting = false;
                    FilteredStringBuilder.Append(c);
                    continue;
                }

                if (c == '\\')
                {
                    Denoting = true;
                    FilteredStringBuilder.Append(c);
                    continue;
                }

                if (c == '"')
                {
                    InString = !InString;
                    FilteredStringBuilder.Append(c);
                    continue;
                }

                if (InString)
                {
                    FilteredStringBuilder.Append(c);
                    continue;
                }

                if (!Char.IsWhiteSpace(c))
                {
                    FilteredStringBuilder.Append(c);
                    continue;
                }
            }
            return FilteredStringBuilder.ToString();
        }
    }
}
