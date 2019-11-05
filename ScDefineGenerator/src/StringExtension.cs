using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtension
{
	/// <summary>
	/// CSV 文字列を int 型のリストに変換
	/// </summary>
	public static List<int> ToIntList(this string self, char delimiter = ',')
	{
		var list = new List<int>();

		if (!string.IsNullOrEmpty(self))
		{
			var values = self.Split(delimiter);
			foreach (var v in values)
			{
				int result = 0;
				if (int.TryParse(v, out result))
				{
					list.Add(result);
				}
				else
				{
					list.Add(0);
				}
			}
		}

		return list;
	}

	/// <summary>
	/// CSV 文字列を long 型のリストに変換
	/// </summary>
	public static List<long> ToLongList(this string self, char delimiter = ',')
	{
		var list = new List<long>();

		if (!string.IsNullOrEmpty(self))
		{
			var values = self.Split(delimiter);
			foreach (var v in values)
			{
				long result = 0;
				if (long.TryParse(v, out result))
				{
					list.Add(result);
				}
				else
				{
					list.Add(0);
				}
			}
		}

		return list;
	}

	/// <summary>
	/// CSV 文字列を float 型のリストに変換
	/// </summary>
	public static List<float> ToFloatList(this string self, char delimiter = ',')
	{
		var list = new List<float>();

		if (!string.IsNullOrEmpty(self))
		{
			var values = self.Split(delimiter);
			foreach (var v in values)
			{
				list.Add(float.Parse(v, CultureInfo.InvariantCulture));
			}
		}

		return list;
	}

	/// <summary>
	/// CSV 文字列を string 型のリストに変換
	/// </summary>
	public static List<string> ToStringList(this string self, char delimiter = ',')
	{
		var list = new List<string>();

		if (!string.IsNullOrEmpty(self))
		{
			var values = self.Split(delimiter);
			foreach (var v in values)
			{
				list.Add(v);
			}
		}

		return list;
	}

	/// <summary>
	/// CSV 文字列を Dictionary<int, int> 型のリストに変換
	/// </summary>
	public static Dictionary<int, int> ToIntIntDict(this string self,
		char delimiter1 = ',', char delimiter2 = ':')
	{
		var results = new Dictionary<int, int>();

		if (string.IsNullOrEmpty(self)) { return results; }

		var list = self.Split(delimiter1);
		foreach (var n in list)
		{
			var cols = n.Split(delimiter2);
			if (cols.Length < 2) { continue; }

			var key = int.Parse(cols[0]);
			var val = int.Parse(cols[1]);
			results.Add(key, val);
		}

		return results;
	}

	/// <summary>
	/// CSV 文字列を Dictionary<int, long> 型のリストに変換
	/// </summary>
	public static Dictionary<int, long> ToIntLongDict(this string self,
		char delimiter1 = ',', char delimiter2 = ':')
	{
		var results = new Dictionary<int, long>();

		if (string.IsNullOrEmpty(self)) { return results; }

		var list = self.Split(delimiter1);
		foreach (var n in list)
		{
			var cols = n.Split(delimiter2);
			if (cols.Length < 2) { continue; }

			var key = int.Parse(cols[0]);
			var val = long.Parse(cols[1]);
			results.Add(key, val);
		}

		return results;
	}

	/// <summary>
	/// 16進数の文字列からバイト配列に変換
	/// </summary>
	/// <param name="str">16進数文字列</param>
	/// <returns>バイト配列</returns>
	public static byte[] HexStringToBytes(this string str)
	{
		str = (str.Length % 2) != 0 ? str + "0" : str;
		var length = str.Length / 2;
		var bytes = new byte[length];

		for (int i = 0; i < length; i++)
		{
			bytes[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
		}

		return bytes;
	}

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

	/// <summary>
	/// メッセージを分割長で分割
	/// </summary>
	/// <param name="self"> メッセージ </param>
	/// <param name="length"> 分割長 </param>
	/// <returns> 分割されたメッセージ </returns>
	public static string[] Divide(this string self, int length)
	{
		var list = new List<string>();

		while (self != null && self.Length != 0)
		{
			var index = Math.Min(length, self.Length);
			list.Add(self.Substring(0, index));
			self = self.Substring(index);
		}

		return list.ToArray();
	}

	/// <summary>
	/// 指定したキーワードが文字列の終端に存在している場合、削除する
	/// </summary>
	/// <param name="self">除去前の文字列</param>
	/// <param name="keyword">除去したいキーワード</param>
	public static string RemoveLastKeyword(this string self, string keyword)
	{
		var selfLen = self.Length;
		var keyLen = keyword.Length;
		var index = self.LastIndexOf(keyword);

		if (index != -1 && selfLen - index == keyLen)
		{
			return self.Remove(index, keyLen);
		}
		return self;
	}

	/// <summary>
	/// ピリオドでスプリットした結果の配列0番目を返す
	/// </summary>
	/// <param name="self">ファイル名</param>
	public static string RemoveExtension(this string self)
	{
		var fileName = self.Split('.');
		return fileName[0];
	}
}
