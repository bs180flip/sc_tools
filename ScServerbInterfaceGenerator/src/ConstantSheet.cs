using System.IO;
using System.Text;
using ExcelReader;
using Alim.Utility;

namespace ScServerInterfaceGenerator
{
	public class ConstantSheet : XlsElement
	{
		private const string Namespace = "Sc.Multi.Web";

		private const string EnumPath = "Enum/";
		private const string BeforeIOS = "ios";
		private const string AfterIOS = "iOS";

		private const int ConstantRow = 2;
		private const int ConstantNameCol = 1;
		private const int ConstantSummaryCol = 3;

		private const int ConstStartRow = 3;
		private const int ConstInitCol = 0;

		private const int TypeSummaryCol = 0;
		private const int TypeNameCol = 1;
		private const int EnumNameCol = 2;
		private const int EnumValueCol = 3;
		private const int EnumSummaryCol = 4;

		private const int TypeStartRow = 3;

		private const string KeyValueSeparator = ":";
		private const string CommentPrefix = "### ";

		private XlsSheet Sheet { get; set; }
		private string ConstantName { get; set; }
		private string ConstantSummary { get; set; }

		public static string Dir { get; set; }

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="dir">ディレクトリ</param>
		public static void Initialize(string dir)
		{
			Dir = dir;

			Directory.CreateDirectory(Dir);
			Directory.CreateDirectory(Dir + EnumPath);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">シート</param>
		public ConstantSheet(XlsSheet sheet)
		{
			Sheet = sheet;
			ConstantName = Sheet.Fields[ConstantRow][ConstantNameCol];
			ConstantSummary = Sheet.Fields[ConstantRow][ConstantSummaryCol];
		}

		/// <summary>
		/// クラスを生成
		/// </summary>
		public void GenerateYamls()
		{
			GenerateConstantYaml(Dir);
		}

		/// <summary>
		/// 定数YAMLファイルを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private void GenerateConstantYaml(string directory)
		{
			var sb = new StringBuilder();

			sb.AppendLine("#######################");
			sb.AppendLine("###" + ConstantSummary + "###");
			sb.AppendLine("#######################");
			sb.AppendLine();

			var row = ConstStartRow;
			var col = ConstInitCol;

			while (true)
			{
				if (Sheet.Fields.Count <= row) { break; }

				sb.AppendLine(CommentPrefix + Sheet.Fields[row][col]);
				col++;

				var tab = "";
				var fixStr = "";

				while (true)
				{
					var checkCol = col + 1;

					if (Sheet.Fields[row].Count <= checkCol)
					{
						sb.AppendLine(fixStr + " " + StringUtility.CheckReservedWord(Sheet.Fields[row][col]));
						row++;
						col--;
						fixStr = "";
						if (Sheet.Fields.Count - 1 >= row && string.IsNullOrEmpty(Sheet.Fields[row][col]))
						{
							col = ConstInitCol;
							sb.AppendLine();
							break;
						}
						else if (Sheet.Fields.Count <= row)
						{
							break;
						}
					}
					else
					{
						fixStr = tab + StringUtility.CheckReservedWord(Sheet.Fields[row][col]) + KeyValueSeparator;
						if (Sheet.Fields[row].Count > checkCol + 1)
						{
							sb.AppendLine(fixStr);
							fixStr = "";
							tab += Tab;
						}
						col++;
					}
				}

				row++;
			}

			File.WriteAllText(directory + ConstantName + ".yaml", sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// クラスを生成
		/// </summary>
		public void GenerateEnums()
		{
			GenerateAllEnum(Dir + EnumPath);
		}

		/// <summary>
		/// Enum ファイルを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private void GenerateAllEnum(string directory)
		{
			var row = TypeStartRow;

			while (true)
			{
				if (Sheet.Fields.Count <= row) { break; }

				var sb = new StringBuilder();

				var typeSummary = Sheet.Fields[row][TypeSummaryCol];
				var typeName = StringUtility.SnakeToPascal(Sheet.Fields[row][TypeNameCol]);

				sb.AppendLine();
				sb.AppendLine("namespace " + Namespace);
				sb.AppendLine("{");
				sb.AppendLine(Tab + "/// <summary>");
				sb.AppendLine(Tab + "/// " + typeSummary);
				sb.AppendLine(Tab + "/// </summary>");
				sb.AppendLine(Tab + "public enum " + typeName);
				sb.AppendLine(Tab + "{");

				while (true)
				{
					var enumName = Sheet.Fields[row][EnumNameCol];
					if (enumName.Contains(BeforeIOS))
					{
						enumName = AfterIOS;
					}
					else
					{
						enumName = StringUtility.SnakeToPascal(enumName);
					}
					var enumValue = Sheet.Fields[row][EnumValueCol];
					var enumSummary = Sheet.Fields[row][EnumSummaryCol];

					sb.AppendLine(Tab + Tab + "/// <summary>" + enumSummary + "</summary>");
					sb.AppendLine(Tab + Tab + enumName + " = " + enumValue + ",");

					row++;

					if (string.IsNullOrEmpty(Sheet.Fields[row][EnumNameCol]))
					{
						break;
					}
					else
					{
						sb.AppendLine();
					}
				}

				sb.AppendLine(Tab + "}");
				sb.AppendLine("}");

				File.WriteAllText(directory + typeName + ".cs", sb.ToString(), Encoding.UTF8);

				row++;
			}
		}
	}
}

