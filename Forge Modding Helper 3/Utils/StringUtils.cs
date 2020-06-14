using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3
{
    // This class add useful string actions
    public static class StringUtils
    {
        /// <summary>
        /// Remove special character from a string
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Formatted string</returns>
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

        /// <summary>
        /// Check if a string is a valid value
        /// </summary>
        /// <param name="text">Input string</param>
        /// <returns><code>true</code> if valid value, <code>false</code> else</returns>
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
