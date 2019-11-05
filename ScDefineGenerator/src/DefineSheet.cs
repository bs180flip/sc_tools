using System;
using System.IO;
using System.Text;
using ExcelReader;
using Alim.Utility;

namespace ScDefineGenerator
{
	public class DefineSheet : XlsElement
	{
		private const string Namespace = "Sc.Common";
		private const string ClassName = "DefineMstConst";
		private const string ClassSummary = "定数マスタ";
		private const string ConstOutputDir = "Const/";

		private const int DefineStartRow = 7;
		private const int DefineKeyCol = 3;
		private const int CommentCol = 4;
		private const int DefineValueCol = 5;

		private XlsSheet Sheet { get; set; }
		private string Dir { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">シート</param>
		public DefineSheet(XlsSheet sheet, string dir)
		{
			Sheet = sheet;
			Dir = dir;

			Directory.CreateDirectory(Dir + ConstOutputDir);
		}

		/// <summary>
		/// クラスを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		public void GenerateClass()
		{
			var sb = new StringBuilder();

			sb.AppendLine("/// <summary>");
			sb.AppendLine("/// " + ClassSummary);
			sb.AppendLine("/// </summary>");
			sb.AppendLine("public static class " + ClassName);
			sb.Append("{");

			var row = DefineStartRow;

			while (true)
			{
				if (string.IsNullOrEmpty(Sheet.Fields[row][DefineKeyCol]))
				{
					if (string.IsNullOrEmpty(Sheet.Fields[row][DefineValueCol]))
					{
						break;
					}
					row++;
					continue;
				}

				var defineKeyName = Sheet.Fields[row][DefineKeyCol];
				var variableName = StringUtility.SnakeToPascal(defineKeyName.ToLower());
				var summary = Sheet.Fields[row][CommentCol];

				sb.AppendLine();
				sb.AppendLine(Tab +"/// <summary>" + summary + "</summary>");
				sb.AppendLine(Tab +"internal const string " + variableName + " = \"" + defineKeyName + "\";");

				row++;
			}

			sb.AppendLine("}");

			File.WriteAllText(Dir + ConstOutputDir + ClassName + ".cs", sb.ToString(), Encoding.UTF8);
		}
	}
}
