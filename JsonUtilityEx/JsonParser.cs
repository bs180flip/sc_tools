/// <summary>
/// Json 解析クラス
/// Yuji Oshima
/// 2017/07/18
/// </summary>
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Alim.Serialize
{
	/// <summary>
	/// Json 解析クラス
	/// </summary>
	internal sealed class JsonParser : IDisposable
	{
		/// <summary>空白扱いにする記号(タブ, 改行, 復帰)</summary>
		private const string whiteSpace = " \t\n\r";
		/// <summary>ワードを区切る記号</summary>
		private const string wordBreak = " \t\n\r{}[],:\"";

		/// <summary>
		/// トークン
		/// </summary>
		private enum Token
		{
			None,           //
			ObjectOpen,     // {
			ObjectClose,    // }
			ArrayOpen,      // [
			ArrayClose,     // ]
			Colon,          // :
			Comma,          // ,
			String,         // "string"
			Number,         // 1234
			True,           // true
			False,          // false
			Null,           // null
		}

		/// <summary>文字列読み込み</summary>
		private StringReader stringReader = null;

		/// <summary>
		/// パース処理
		/// </summary>
		/// <param name="json">JSONテキスト</param>
		/// <returns>ハッシュテーブル</returns>
		public static Hashtable Parse(string json)
		{
			var parser = new JsonParser(json);

			return (Hashtable)parser.ParseValue();
		}

		/// <summary>
		/// プライベートコンストラクタ
		/// </summary>
		/// <param name="json">JSONテキスト</param>
		private JsonParser(string json)
		{
			stringReader = new StringReader(json);
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			stringReader.Dispose();
			stringReader = null;
		}

		/// <summary>
		/// 値型をパース
		/// </summary>
		/// <returns>値</returns>
		private object ParseValue()
		{
			var nextToken = NextToken;

			return ParseByToken(nextToken);
		}

		/// <summary>
		/// トークンごとにパース処理を分岐
		/// </summary>
		/// <param name="token">トークン</param>
		/// <returns>トークンごとの値</returns>
		private object ParseByToken(Token token)
		{
			switch (token)
			{
			case Token.String:
				return ParseString();

			case Token.Number:
				return ParseNumber();

			case Token.ObjectOpen:
				return ParseObject();

			case Token.ArrayOpen:
				return ParseArray();

			case Token.True:
				return true;

			case Token.False:
				return false;

			case Token.Null:
				return null;

			default:
				return null;
			}
		}

		/// <summary>
		/// オブジェクト型をパース
		/// </summary>
		/// <returns>オブジェクト</returns>
		private Hashtable ParseObject()
		{
			var table = new Hashtable();

			// ditch opening brace
			stringReader.Read();

			// {
			while (true)
			{
				switch (NextToken)
				{
				case Token.None:
					return null;

				case Token.Comma:
					continue;

				case Token.ObjectClose:
					return table;

				default:
					// name
					string name = ParseString();
					if (name == null)
					{
						return null;
					}

					// :
					if (NextToken != Token.Colon)
					{
						return null;
					}
					// ditch the colon
					stringReader.Read();

					// value
					table[name] = ParseValue();
					break;
				}
			}
		}

		/// <summary>
		/// 配列型をパース
		/// </summary>
		/// <returns>配列</returns>
		private object[] ParseArray()
		{
			var array = new List<object>();

			// ditch opening bracket
			stringReader.Read();

			// [
			var parsing = true;
			while (parsing)
			{
				Token nextToken = NextToken;

				switch (nextToken)
				{
				case Token.None:
					return null;

				case Token.Comma:
					continue;

				case Token.ArrayClose:
					parsing = false;
					break;

				default:
					object value = ParseByToken(nextToken);

					array.Add(value);
					break;
				}
			}

			return array.ToArray();
		}

		/// <summary>
		/// 文字列型をパース
		/// </summary>
		/// <returns>文字列</returns>
		private string ParseString()
		{
			var stringBuilder = new StringBuilder();
			char c;

			// ditch opening quote
			stringReader.Read();

			var isParsing = true;

			while (isParsing)
			{
				if (stringReader.Peek() == -1)
				{
					isParsing = false;
					break;
				}

				c = NextChar;
				switch (c)
				{
				case '"':
					isParsing = false;
					break;

				case '\\':
					if (stringReader.Peek() == -1)
					{
						isParsing = false;
						break;
					}

					c = NextChar;
					switch (c)
					{
					case '"':
					case '\\':
					case '/':
						stringBuilder.Append(c);
						break;

					case 'b':
						stringBuilder.Append('\b');
						break;

					case 'f':
						stringBuilder.Append('\f');
						break;

					case 'n':
						stringBuilder.Append('\n');
						break;

					case 'r':
						stringBuilder.Append('\r');
						break;

					case 't':
						stringBuilder.Append('\t');
						break;

					case 'u':
						var hex = new StringBuilder();

						for (int i = 0; i < 4; i++)
						{
							hex.Append(NextChar);
						}

						stringBuilder.Append((char)Convert.ToInt32(hex.ToString(), 16));
						break;
					}
					break;

				default:
					stringBuilder.Append(c);
					break;
				}
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// 数値型を解析
		/// </summary>
		/// <returns>数値</returns>
		private object ParseNumber()
		{
			// 次のワードを取得
			var number = NextWord;

			// '.' が見つからない場合は整数
			if (number.IndexOf('.') == -1)
			{
				long parsedInt;
				Int64.TryParse(number, out parsedInt);
				return parsedInt;
			}
			// それ以外は実数
			else
			{
				double parsedDouble;
				Double.TryParse(number, out parsedDouble);
				return parsedDouble;
			}
		}

		/// <summary>
		/// 空白扱いの文字を無視
		/// </summary>
		private void EatWhitespace()
		{
			while (whiteSpace.IndexOf(PeekChar) != -1)
			{
				stringReader.Read();

				if (stringReader.Peek() == -1)
				{
					break;
				}
			}
		}

		/// <summary>
		/// 現在の文字
		/// </summary>
		private char PeekChar
		{
			get { return Convert.ToChar(stringReader.Peek()); }
		}

		/// <summary>
		/// 次の文字
		/// </summary>
		private char NextChar
		{
			get { return Convert.ToChar(stringReader.Read()); }
		}

		/// <summary>
		/// 次のワード
		/// </summary>
		private string NextWord
		{
			get
			{
				var word = new StringBuilder();

				while (wordBreak.IndexOf(PeekChar) == -1)
				{
					word.Append(NextChar);

					if (stringReader.Peek() == -1)
					{
						break;
					}
				}

				return word.ToString();
			}
		}

		/// <summary>
		/// 次のトークン
		/// </summary>
		private Token NextToken
		{
			get
			{
				EatWhitespace();

				if (stringReader.Peek() == -1)
				{
					return Token.None;
				}

				char c = PeekChar;

				switch (c)
				{
				case '{':
					return Token.ObjectOpen;

				case '}':
					stringReader.Read();
					return Token.ObjectClose;

				case '[':
					return Token.ArrayOpen;

				case ']':
					stringReader.Read();
					return Token.ArrayClose;

				case ',':
					stringReader.Read();
					return Token.Comma;

				case '"':
					return Token.String;

				case ':':
					return Token.Colon;

				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
				case '+':
					return Token.Number;
				}

				var word = NextWord;

				switch (word)
				{
				case "false":
					return Token.False;

				case "true":
					return Token.True;

				case "null":
					return Token.Null;
				}

				return Token.None;
			}
		}
	}
}
