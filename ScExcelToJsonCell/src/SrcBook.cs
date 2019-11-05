using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ExcelReader;

namespace ScExcelToJsonCell
{
	public class SrcBook : XlsElement
	{
		/// <summary>出力先ディレクトリ</summary>
		private const string OutputDir = "output/";

		/// <summary>クラスファイルの拡張子</summary>
		private const string ClassExt = ".cs";

		/// <summary>クラス名行</summary>
		private const int ClassNameRow = 1;

		/// <summary>クラス名列</summary>
		private const int ClassNameCol = 3;

		/// <summary>フィールド名行</summary>
		private const int FieldNameRow = 2;

		/// <summary>フィールド型行</summary>
		private const int FieldTypeRow = 3;

		/// <summary>デリミタ行</summary>
		private const int DelimiterRow = 4;

		/// <summary>フィールドコメント行</summary>
		private const int FieldCommentRow = 6;

		/// <summary>値開始行</summary>
		private const int ValueStartRow = 7;

		/// <summary>値開始列</summary>
		private const int ValueStartCol = 3;

		/// <summary>string 型</summary>
		private const string StrType = "STR";

		/// <summary>int 型</summary>
		private const string IntType = "INT";

		/// <summary>long 型</summary>
		private const string LongType = "LONG";

		/// <summary>ブック</summary>
		private XlsBook Book { get; set; }

		/// <summary>ディレクトリ</summary>
		private string Dir { get; set; }

		/// <summary>キーカラム名</summary>
		private string KeyColName { get; set; }

		/// <summary>JSON辞書</summary>
		public Dictionary<string, string> JsonDict { get; private set; }

		/// <summary>定義名辞書</summary>
		private DefineDict Define { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="keyColName">キーカラム名</param>
		/// <param name="isList">リストで出力するか</param>
		/// <param name="isCreate">格納クラスを出力するか</param>
		public SrcBook(string filePath, string keyColName, bool isList, bool isCreate)
		{
			Book = new XlsBook(filePath);
			Console.WriteLine("### " + filePath + "を読み込みました。");
			KeyColName = keyColName;
			JsonDict = new Dictionary<string, string>();

			var defineDir = Path.GetDirectoryName(filePath);
			Define = new DefineDict(defineDir);

			foreach (var sheet in Book.Sheets)
			{
				_CreateJsonDict(sheet, isList);

				if (isCreate)
				{
					_CreateClass(sheet, isList);
				}
			}
			Console.WriteLine("### JSON辞書を生成しました。");
		}

		/// <summary>
		/// Json辞書生成
		/// </summary>
		/// <param name="sheet">シート</param>
		/// <param name="isList">リストで出力するか</param>
		private void _CreateJsonDict(XlsSheet sheet, bool isList)
		{
			var keyIndex = sheet.Fields[FieldNameRow].FindIndex((item) => item == KeyColName);
			if (keyIndex == -1)
			{
				Console.WriteLine("!!! キー名と一致するカラムが存在しません");
			}

			if (isList)
			{
				var dict = new Dictionary<string, List<string>>();

				for (int row = ValueStartRow; row < sheet.RowCount; row++)
				{
					var key = sheet.Fields[row][keyIndex];
					if (string.IsNullOrEmpty(key)) { continue; }

					var json = _CreateJsonFromRecord(sheet, row);
					if (string.IsNullOrEmpty(json)) { continue; }

					if (!dict.ContainsKey(key))
					{
						dict[key] = new List<string>();
					}

					dict[key].Add(json);
				}

				foreach (var key in dict.Keys)
				{
					var sb = new StringBuilder();

					sb.Append("{\"dataList\":[");
					sb.Append(string.Join(",", dict[key]));
					sb.Append("]}");

					JsonDict.Add(key, sb.ToString());
				}
			}
			else
			{
				for (int row = ValueStartRow; row < sheet.RowCount; row++)
				{
					var key = sheet.Fields[row][keyIndex];
					if (string.IsNullOrEmpty(key)) { continue; }

					var json = _CreateJsonFromRecord(sheet, row);
					if (string.IsNullOrEmpty(json)) { continue; }

					JsonDict.Add(key, json);
				}
			}
		}

		/// <summary>
		/// 1レコードからJSONを生成
		/// </summary>
		/// <param name="sheet">シート</param>
		/// <param name="row">行</param>
		/// <param name="isList">リストで出力するか</param>
		/// <returns>JSON文字列</returns>
		private string _CreateJsonFromRecord(XlsSheet sheet, int row)
		{
			if (sheet.Fields[row][ValueStartCol].Contains("//")) { return null; }

			var sb = new StringBuilder();
			sb.Append("{");

			var endCol = sheet.Fields[FieldNameRow].FindIndex((item) => item == "END");
			var col = ValueStartCol;

			while (col < endCol)
			{
				var name = sheet.Fields[FieldNameRow][col];
				if (!string.IsNullOrEmpty(name))
				{
					if (col != ValueStartCol)
					{
						sb.Append(",");
					}

					// デリミタ
					var delimiter = sheet.Fields[DelimiterRow][col];

					var vsb = new StringBuilder();

					var type = sheet.Fields[FieldTypeRow][col];

					do
					{
						if (vsb.Length != 0)
						{
							vsb.Append(delimiter);
						}

						var val = sheet.Fields[row][col];
						// 定義名の変換
						if (!string.IsNullOrEmpty(val) && val[0] == '#')
						{
							val = Define[val];
						}
						vsb.Append(val);

						col++;
					}
					while (string.IsNullOrEmpty(sheet.Fields[FieldNameRow][col]));

					var value = vsb.ToString();

					switch (type)
					{
					case StrType:
						value = "\"" + value + "\"";
						break;
					}

					sb.Append("\"" + name + "\":" + value);
				}
			}

			sb.Append("}");

			return sb.ToString();
		}

		/// <summary>
		/// 格納クラスを生成
		/// </summary>
		/// <param name="sheet">シート</param>
		/// <param name="isList">リストで出力するか</param>
		private void _CreateClass(XlsSheet sheet, bool isList)
		{
			var className = sheet.Fields[ClassNameRow][ClassNameCol];
			className = className.ToLower().SnakeToPascal();

			var sb = new StringBuilder();

			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine();
			sb.AppendLine("namespace Sc.Common");
			sb.AppendLine("{");

			sb.Append(_CreateSingleClass(className, sheet));

			if (isList)
			{
				sb.AppendLine();
				sb.Append(_CreateListClass(className));
			}

			sb.AppendLine("}");

			File.WriteAllText(OutputDir + className + ClassExt, sb.ToString());
		}

		/// <summary>
		/// 格納クラスを生成
		/// </summary>
		/// <param name="sheet">シート</param>
		private string _CreateSingleClass(string className, XlsSheet sheet)
		{
			var sb = new StringBuilder();

			sb.AppendLine(Tab + "/// <summary>");
			sb.AppendLine(Tab + "/// " + sheet.SheetName);
			sb.AppendLine(Tab + "/// </summary>");
			sb.AppendLine(Tab + "public class " + className);
			sb.AppendLine(Tab + "{");

			var endCol = sheet.Fields[FieldNameRow].FindIndex((item) => item == "END");
			var col = ValueStartCol;

			while (col < endCol)
			{
				var name = sheet.Fields[FieldNameRow][col];
				if (!string.IsNullOrEmpty(name))
				{
					var type = sheet.Fields[FieldTypeRow][col];
					switch (type)
					{
					case StrType:
						type = "string";
						break;

					case IntType:
						type = "int";
						break;

					case LongType:
						type = "long";
						break;
					}
					var comment = sheet.Fields[FieldCommentRow][col];

					if (col != ValueStartCol)
					{
						sb.AppendLine();
					}

					var propertyName = name.ToLower().SnakeToPascal();
					var fieldName = name.ToLower().SnakeToCamel();

					sb.AppendLine(Tab + Tab + "/// <summary>" + comment + "</summary>");
					sb.AppendLine(Tab + Tab + "public " + type + " " + propertyName + " { get { return " + fieldName + "; } private set { " + fieldName + " = value; } }");
					sb.AppendLine(Tab + Tab + "[SerializeField]");
					sb.AppendLine(Tab + Tab + "private " + type + " " + fieldName + ";");
				}

				col++;
			}

			sb.AppendLine(Tab + "}");

			return sb.ToString();
		}

		/// <summary>
		/// リストクラスを生成
		/// </summary>
		/// <param name="className">クラス名</param>
		private string _CreateListClass(string className)
		{
			var sb = new StringBuilder();

			sb.AppendLine(Tab + "/// <summary>");
			sb.AppendLine(Tab + "/// " + className + "のリストを格納するクラス");
			sb.AppendLine(Tab + "/// </summary>");
			sb.AppendLine(Tab + "public class " + className + "List");
			sb.AppendLine(Tab + "{");

			sb.AppendLine(Tab + Tab + "/// <summary>データリスト</summary>");
			sb.AppendLine(Tab + Tab + "public List<" + className + "> DataList { get { return dataList; } private set { dataList = value; } }");
			sb.AppendLine(Tab + Tab + "[SerializeField]");
			sb.AppendLine(Tab + Tab + "private List<" + className + "> dataList;");

			sb.AppendLine(Tab + "}");

			return sb.ToString();
		}
	}
}
