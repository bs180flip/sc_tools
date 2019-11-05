using System.Text;
using System.Text.RegularExpressions;

namespace ScWebYamlGenerator
{
    class StringUtility
    {
        /// <summary>
        /// 空文字ORNULL検定
        /// </summary>
        /// <param name="srcStr"></param>
        /// <returns></returns>
        public static bool IsNUllOrEmpty(string srcStr)
        {
            return srcStr.Length == 0 || srcStr == null;
        }

        public static string checkReservedWord(string srcStr)
        {
            switch (srcStr)
            {
                case "yes":
                case "no":
                    return "'" + srcStr + "'";
                default:
                    return srcStr;
            }
        }

        public static string Camel2Snake(string s)
        {
            var pattern = "[a-z][A-Z]";
            var rgx = new Regex(pattern);
            return rgx.Replace(s, m => m.Groups[0].Value[0] + "_" + m.Groups[0].Value[1]).ToLower();
        }
    }
}
