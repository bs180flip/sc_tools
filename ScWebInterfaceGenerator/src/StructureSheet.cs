using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ExcelReader;
using Alim.Utility;

namespace ScWebInterfaceGenerator
{
	public class StructureSheet : XlsElement
	{
		private const string Namespace = "Sc.Common";

		private const string StructurePath = "Structure/";

		private const int StructureRow = 4;
		private const int StructureNameCol = 1;
		private const int StructureSummaryCol = 17;

		private const int FieldStartRow = 7;
		private const int FieldNameCol = 4;
		private const int FieldSummaryCol = 17;

		private readonly int[] FieldTypeCols = new int[] { 33, 38, };

		private const string StrType = "STR";
		private const string IntType = "INT";
		private const string LongType = "LONG";
		private const string UTimeType = "UTIME";

		private XlsSheet Sheet { get; set; }
		private string ClassName { get; set; }
		private string ClassSummary { get; set; }

		public static string Dir { get; set; }

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="dir">ディレクトリ</param>
		public static void Initialize(string dir)
		{
			Dir = dir;

			Directory.CreateDirectory(Dir);
			Directory.CreateDirectory(Dir + StructurePath);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">シート</param>
		public StructureSheet(XlsSheet sheet)
		{
			Sheet = sheet;
			ClassName = StringUtility.SnakeToPascal(Sheet.Fields[StructureRow][StructureNameCol].ToLower());
			ClassSummary = Sheet.Fields[StructureRow][StructureSummaryCol];
		}

		/// <summary>
		/// クラスを生成
		/// </summary>
		public void GenerateClass()
		{
			var Generate = GenerateClass(Dir + StructurePath);
		}

		/// <summary>
		/// クラスを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private bool GenerateClass(string directory)
		{
			if (string.IsNullOrEmpty(Sheet.Fields[FieldStartRow][FieldNameCol])) { return false; }

			var sb = new StringBuilder();

			sb.AppendLine("using System;");
			sb.AppendLine();
			sb.AppendLine("namespace " + Namespace);
			sb.AppendLine("{");
			sb.AppendLine(Tab + "/// <summary>");
			sb.AppendLine(Tab + "/// " + ClassSummary);
			sb.AppendLine(Tab + "/// </summary>");
			sb.AppendLine(Tab + "[Serializable]");
			sb.AppendLine(Tab + "public class " + ClassName);
			sb.AppendLine(Tab + "{");

			var row = FieldStartRow;

			while (row < Sheet.RowCount)
			{

				if (string.IsNullOrEmpty(Sheet.Fields[row][FieldNameCol])) { break; }

				var fieldName = Sheet.Fields[row][FieldNameCol];
				var propertyName = StringUtility.SnakeToPascal(fieldName.ToLower()).Substring(1);
				var fieldSummary = Sheet.Fields[row][FieldSummaryCol];
				var fieldType = GetFieldType(Sheet.Fields[row]);

				if (row != FieldStartRow) { sb.AppendLine(); }

				sb.AppendLine(Tab + Tab + "/// <summary>" + fieldSummary + "</summary>");
				sb.AppendLine(Tab + Tab + "public " + fieldType + " " + propertyName + " { get { return " + fieldName + "; } set { " + fieldName + " = value; } }");
				sb.AppendLine(Tab + Tab + "public " + fieldType + " " + fieldName + ";");

				row++;
			}

			sb.AppendLine(Tab + "}");
			sb.AppendLine("}");

			File.WriteAllText(directory + ClassName + ".cs", sb.ToString(), Encoding.UTF8);

			return true;
		}

		/// <summary>
		/// 行データからフィールドのタイプを取得
		/// </summary>
		/// <param name="rowData">行データ</param>
		/// <returns>フィールドのタイプ</returns>
		private string GetFieldType(List<string> rowData)
		{
			foreach (var col in FieldTypeCols)
			{
				var fieldType = rowData[col];

				if (!string.IsNullOrEmpty(fieldType))
				{
					switch (fieldType)
					{
					case IntType:
						return "int";

					case LongType:
					case UTimeType:
						return "long";

					case StrType:
						return "string";
					}
				}
			}

			return "string";
		}
	}
}
