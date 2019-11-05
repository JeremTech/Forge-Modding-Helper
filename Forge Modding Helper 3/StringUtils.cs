using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3
{
    public static class StringUtils
    {
        public static string deleteStartEndSpaces(this string text)
        {
            return text.Trim(' ');
        }

        public static string formatTextToLower(this string text)
        {
            string output = deleteStartEndSpaces(text);
            output = output.Replace(" ", "");
            output = output.ToLower();
            output = RemoveSpecialCharacters(output);

            return output;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ' || c == '-' || c == (char)13)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static bool isTextValid(this string text)
        {
            bool output = false;

            if(!String.IsNullOrEmpty(text))
            {
                if(!String.IsNullOrWhiteSpace(text))
                {
                    output = true;
                }
            }

            return output;
        }
    }
}
