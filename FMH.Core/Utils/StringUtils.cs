using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMH.Core.Utils
{
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
        /// Generate ModID from mod name
        /// </summary>
        /// <param name="_modName">Mod name</param>
        /// <returns>Generated ModID</returns>
        public static string CreateModIDFromModName(string _modName)
        {
            // Result
            string result = "";

            // Remove specials characters
            result = RemoveSpecialCharacters(_modName);

            // Remove uppercase
            result = result.ToLower();

            // Replace spaces with '_'
            result = result.Replace(' ', '_');

            // Return result
            return result;
        }

        /// <summary>
        /// Generate ModID from mod name
        /// </summary>
        /// <param name="_modName">Mod name</param>
        /// <returns>Generated ModID</returns>
        public static string CreateModGroupFromModIDAndAuthor(string _modID, string _modAuthor)
        {
            // Result
            string result = "net.";

            // Format and add mod author
            result += RemoveSpecialCharacters(_modAuthor.Split(',')[0].ToLower().Replace("_", ""));

            // Add ModID
            result += "." + _modID;

            // Return result
            return result;
        }

        /// <summary>
        /// Get string between two other string
        /// </summary>
        public static string getBetween(this string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        /// <summary>
        /// Format translation text for a correct displaying
        /// </summary>
        public static string FormateTranslationText(this string str)
        {
            string output = str;

            if (!string.IsNullOrWhiteSpace(output))
                output.Replace("\n", Environment.NewLine);

            return output;
        }

        /// <summary>
        /// Check if a string is a valid value
        /// </summary>
        /// <param name="text">Input string</param>
        /// <returns><code>true</code> if valid value, <code>false</code> else</returns>
        public static bool isTextValid(this string text)
        {
            bool output = false;

            if (!String.IsNullOrEmpty(text))
            {
                if (!String.IsNullOrWhiteSpace(text))
                {
                    output = true;
                }
            }

            return output;
        }
    }
}
