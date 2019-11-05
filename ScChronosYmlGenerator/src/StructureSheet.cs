using Alim.Utility;
using ExcelReader;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScChronosYmlGenerator
{
    public class StructureSheet : XlsElement
	{
		private const string Namespace = "Sc.Web";

		private const string StructurePath = "structure/";

        private const int StructureRow = 4;
		private const int StructureNameCol = 1;
		private const int StructureSummaryCol = 17;

		private const int FieldStartRow = 7;
		private const int FieldNameCol = 4;
		private const int FieldSummaryCol = 17;
        private const int FieldNoteCol = 42;

		private readonly int[] FieldTypeCols = new int[] { 33, 38, };

		private const string StrType = "STR";
		private const string IntType = "INT";
		private const string LongType = "LONG";
		private const string UTimeType = "UTIME";
        private const string UBINType = "BIN";


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
			ClassName = StringUtility.SnakeToPascal(Sheet.Fields[StructureRow][StructureNameCol].ToLower()).Substring(1); // 1文字目のTを除く
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
			var sb = new StringBuilder();

            sb.AppendLine("  " + ClassName + ":");
            sb.AppendLine("    type: object");
            sb.AppendLine("    description: " + ClassSummary);
            sb.AppendLine("    properties:");


            if (!string.IsNullOrEmpty(Sheet.Fields[FieldStartRow][FieldNameCol]))
            {
                var row = FieldStartRow;
                while (row < Sheet.RowCount)
                {
                    if (string.IsNullOrEmpty(Sheet.Fields[row][FieldNameCol])) { break; }

                    var fieldName = Sheet.Fields[row][FieldNameCol].Substring(2); // P_を抜いた文字列を利用する
                    var fieldType = GetFieldType(Sheet.Fields[row]);
                    var fieldFormat = GetFieldFormat(Sheet.Fields[row]);

                    var fieldSummary = Sheet.Fields[row][FieldSummaryCol];
                    var fieldNote = Sheet.Fields[row][FieldNoteCol];

                    sb.AppendLine("      " + fieldName + ":");
                    sb.AppendLine("        type: " + fieldType);
                    if (fieldFormat != "")
                    {
                        sb.AppendLine("        format: " + fieldFormat);
                    }
                    if (fieldSummary.Length > 0)
                    {
                        if (fieldNote.Length > 0)
                        {
                            sb.AppendLine("        description: |"); // 改行対応
                        }
                        else
                        {
                            sb.AppendLine("        description:");
                        }

                        sb.AppendLine("          " + fieldSummary.Trim());
                    }

                    if (fieldNote.Length > 0)
                    {
                        var note = System.Text.RegularExpressions.Regex.Replace(fieldNote, "[\r\n]", "\n            ");
                        sb.AppendLine("          " + note);
                    }
                    if (fieldNote.Equals("未設定時は省略"))
                    {
                        sb.AppendLine("        x-optional: true");
                    }


                    row++;
                }
            }

            File.WriteAllText(directory + ClassName + ".yaml", sb.ToString());

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
                        case LongType:
                        case UTimeType:
                            return "integer";
                    }
				}
			}
            // str, bin

			return "string";
		}

        /// <summary>
        /// 行データからフィールドのフォーマットを取得
        /// </summary>
        /// <param name="rowData">行データ</param>
        /// <returns>フィールドのタイプ</returns>
        private string GetFieldFormat(List<string> rowData)
        {
            foreach (var col in FieldTypeCols)
            {
                var fieldType = rowData[col];

                if (!string.IsNullOrEmpty(fieldType))
                {
                    switch (fieldType)
                    {
                        case LongType:
                            return "int64";
                        case UTimeType:
                            return "utime";
                        case UBINType:
                            return "binary";

                    }
                }
            }

            return "";
        }
    }
}
