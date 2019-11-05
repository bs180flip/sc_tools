/// <summary>
/// 文字列操作ユーティリティ
/// Yuji Oshima
/// 2016/07/18
/// </summary>
using System.Text;
using System.Text.RegularExpressions;

namespace Alim.Utility
{
	/// <summary>
	/// 文字列操作ユーティリティ
	/// </summary>
	public static class StringUtility
	{
		/// <summary>
		/// キャメルケースをスネークケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>スネークケース</returns>
		public static string CamelToSnake(string srcStr)
		{
			return Regex.Replace(srcStr, "([a-z])([A-Z])", "$1_$2").ToLower();
		}

		/// <summary>
		/// スネークケースをキャメルケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>キャメルケース</returns>
		public static string SnakeToCamel(string srcStr)
		{
			return PascalToCamel(SnakeToPascal(srcStr));
		}

		/// <summary>
		/// パスカルケースをスネークケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>スネークケース</returns>
		public static string PascalToSnake(string srcStr)
		{
			return Regex.Replace(srcStr, "([a-z])([A-Z])", "$1_$2").ToLower();
		}

		/// <summary>
		/// スネークケースをパスカルケースに変換
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>パスカルケース</returns>
		public static string SnakeToPascal(string srcStr)
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
		public static string CamelToPascal(string srcStr)
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
		public static string PascalToCamel(string srcStr)
		{
			if (!string.IsNullOrEmpty(srcStr))
			{
				return char.ToLower(srcStr[0]) + (srcStr.Length > 1 ? srcStr.Substring(1) : "");
			}

			return srcStr;
		}

		/// <summary>
		/// suffixのUpdを削除
		/// </summary>
		/// <param name="srcStr">変換元文字列</param>
		/// <returns>変換文字列</returns>
		public static string DelUpdString(string srcStr)
		{
			if (string.IsNullOrEmpty(srcStr))
			{
				return srcStr;
			}

			int len = srcStr.Length - 3;
			string suffix = srcStr.Substring(len);

			if (suffix == "Upd")
			{
				return srcStr.Replace("Upd", "");
			}

			return srcStr;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srcStr"></param>
		/// <returns></returns>
		public static string CheckReservedWord(string srcStr)
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
	}
}
