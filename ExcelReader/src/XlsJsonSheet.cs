using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ExcelReader
{
	/// <summary>
	/// フィールドの型
	/// </summary>
	public enum FieldType
	{
		@int,       // int
		@long,      // long
		@float,     // float
		@string,    // string
	}

	/// <summary>
	/// JSONシート
	/// </summary>
	public class XlsJsonSheet : HasFieldNameSheetBase
	{
		/// <summary>開始行番号</summary>
		private const int StartRowIndex = 3;
		/// <summary>コメント行番号</summary>
		private const int FieldCommentRowIndex = 0;
		/// <summary>タイプ行番号</summary>
		private const int FieldTypeRowIndex = 1;
		/// <summary>変数名行番号</summary>
		protected override int FieldNameRowIndex { get { return 2; } }

		/// <summary>フィールド</summary>
		public override List<List<string>> Fields { get; protected set; }

		/// <summary>コメントリスト</summary>
		public List<string> FieldComments { get; protected set; }
		/// <summary>タイプリスト</summary>
		public List<FieldType> FieldTypes { get; protected set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public XlsJsonSheet()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">xlsシート</param>
		public XlsJsonSheet(XlsSheet sheet)
		{
			SheetName = sheet.SheetName;
			ColumnCount = sheet.ColumnCount;
			RowCount = sheet.RowCount - StartRowIndex;
			FieldComments = sheet.Fields[FieldCommentRowIndex];

			FieldTypes = new List<FieldType>();
			var fieldTypeRow = sheet.Fields[FieldTypeRowIndex];
			for (int i = 0; i < fieldTypeRow.Count; i++)
			{
				var fieldType = (FieldType)Enum.Parse(typeof(FieldType), fieldTypeRow[i]);
				FieldTypes.Add(fieldType);
			}

			FieldNames = sheet.Fields[FieldNameRowIndex];

			Fields = new List<List<string>>();

			for (int i = StartRowIndex; i < sheet.RowCount; i++)
			{
				Fields.Add(sheet.Fields[i]);
			}
		}

		/// <summary>
		/// クラスファイルを出力
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		public void GenerateClass(string directory)
		{
			var className = SheetName;
			var filePath = directory + className + "Data.cs";

			using (var writer = new StreamWriter(filePath, false, Encoding))
			{
				// using
				writer.WriteLine("using System;");
				writer.WriteLine("using UnityEngine;");
				writer.WriteLine("");

				writer.WriteLine("namespace Scenario");
				// Beginning of namespace Scenario
				writer.WriteLine("{");

				writer.WriteLine(Tab + "[Serializable]");
				writer.WriteLine(Tab + "public class " + className + "Data");
				writer.WriteLine(Tab + "{");

				// メンバ変数
				for (int col = 0; col < ColumnCount; col++)
				{
					var fieldName = FieldNames[col];
					var propertyName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
					var typeName = FieldTypes[col].ToString();

					if (!string.IsNullOrEmpty(fieldName))
					{
						if (col != 0) { writer.WriteLine(); }

						writer.WriteLine(Tab + Tab + "/// <summary>" + fieldName + "</summary>");
						writer.WriteLine(Tab + Tab + "[SerializeField]");
						writer.WriteLine(Tab + Tab + "private " + typeName + " " + fieldName + ";");
						writer.WriteLine(Tab + Tab + "public " + typeName + " " + propertyName + " { get { return " + fieldName + "; } }");
					}
				}

				writer.WriteLine(Tab + "}");

				// End of namespace Scenario
				writer.WriteLine("}");

				writer.Close();
			}
		}

		/// <summary>
		/// 1行分の値を変換
		/// </summary>
		/// <param name="rowValue">1行分の値</param>
		protected virtual void ConvertRow(List<string> rowValue)
		{
		}

		/// <summary>
		/// JSON 文字列に変換
		/// </summary>
		/// <returns>JSON 文字列</returns>
		public string ToJson()
		{
			var sb = new StringBuilder();

			sb.Append("{");

			var listName = "dataList";
			sb.Append(Quote + listName + Quote + ":");
			// Start list
			sb.Append("[");

			for (int row = 0; row < RowCount; row++)
			{
				// Start row data
				sb.Append("{");

				var rowValue = Fields[row];

				ConvertRow(rowValue);

				for (int col = 0; col < ColumnCount; col++)
				{
					var cellValue = rowValue[col];
					var name = FieldNames[col];
					var type = FieldTypes[col];

					var val = "";

					if (string.IsNullOrEmpty(cellValue))
					{
						switch (type)
						{
						case FieldType.@int:
						case FieldType.@long:
						case FieldType.@float:
							val = "0";
							break;

						case FieldType.@string:
							val = Quote + Quote;
							break;
						}
					}
					else
					{
						switch (type)
						{
						case FieldType.@int:
							{
								int parseInt;
								if (int.TryParse(cellValue, out parseInt))
								{
									val = cellValue;
								}
								else
								{
									val = "0";
								}
							}
							break;

						case FieldType.@long:
							{
								long parseLong;
								if (long.TryParse(cellValue, out parseLong))
								{
									val = cellValue;
								}
								else
								{
									val = "0";
								}
							}
							break;

						case FieldType.@float:
							{
								float parseFloat;
								if (float.TryParse(cellValue, out parseFloat))
								{
									val = cellValue;
								}
								else
								{
									val = "0";
								}
							}
							break;

						case FieldType.@string:
							val = Quote + cellValue + Quote;
							break;
						}
					}

					// 改行コード -> '\n'エスケープ文字 変換
					val = val.Replace("\r", @"\n").Replace("\n", @"\n");

					sb.Append(Quote + name + Quote + ":" + val);

					if (col < ColumnCount - 1)
					{
						sb.Append(",");
					}
				}

				// End of row data.
				sb.Append("}");

				if (row < RowCount - 1)
				{
					sb.Append(",");
				}
			}

			// End of list
			sb.Append("]");
			sb.Append("}");

			return sb.ToString();
		}

		/// <summary>
		/// .json ファイルを出力
		/// </summary>
		/// <param name="directory">出力パス</param>
		public void ExportJson(string directory)
		{
			var filePath = directory + SheetName + ".json";

			File.WriteAllText(filePath, ToJson(), Encoding);
		}
	}
}
