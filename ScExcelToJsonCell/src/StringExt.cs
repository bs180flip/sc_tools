using System.Text;
using System.Text.RegularExpressions;

namespace ScExcelToJsonCell
{
	public static class StringExt
	{
		/// <summary>
		/// キャメルケースをスネークケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>スネークケース</returns>
		public static string CamelToSnake(this string srcStr)
		{
			return Regex.Replace(srcStr, "([a-z])([A-Z])", "$1_$2").ToLower();
		}

		/// <summary>
		/// スネークケースをキャメルケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>キャメルケース</returns>
		public static string SnakeToCamel(this string srcStr)
		{
			return srcStr.SnakeToPascal().PascalToCamel();
		}

		/// <summary>
		/// パスカルケースをスネークケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>スネークケース</returns>
		public static string PascalToSnake(this string srcStr)
		{
			return Regex.Replace(srcStr, "([a-z])([A-Z])", "$1_$2").ToLower();
		}

		/// <summary>
		/// スネークケースをパスカルケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>パスカルケース</returns>
		public static string SnakeToPascal(this string srcStr)
		{
			var tmpStrs = srcStr.Split('_');
			var sb = new StringBuilder();
			foreach (var str in tmpStrs)
			{
				if (!string.IsNullOrEmpty(str))
				{
					sb.Append(char.ToUpper(str[0]) + (str.Length > 1 ? str.Substring(1) : ""));
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// キャメルケースをパスカルケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>パスカルケース</returns>
		public static string CamelToPascal(this string srcStr)
		{
			if (!string.IsNullOrEmpty(srcStr))
			{
				return char.ToUpper(srcStr[0]) + (srcStr.Length > 1 ? srcStr.Substring(1) : "");
			}

			return srcStr;
		}

		/// <summary>
		/// パスカルケースをキャメルケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>キャメルケース</returns>
		public static string PascalToCamel(this string srcStr)
		{
			if (!string.IsNullOrEmpty(srcStr))
			{
				return char.ToLower(srcStr[0]) + (srcStr.Length > 1 ? srcStr.Substring(1) : "");
			}

			return srcStr;
		}
	}
}
