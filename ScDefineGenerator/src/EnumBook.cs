using System.IO;
using System.Text;
using System.Collections.Generic;
using ExcelReader;
using Alim.Utility;

namespace ScDefineGenerator
{
	public class EnumBook : XlsElement
	{
		/// <summary>
		/// 列挙データ
		/// </summary>
		private class EnumData
		{
			public string TypeSummary { get; set; }
			public string EnumName { get; set; }
			public string EnumSummary { get; set; }
			public int EnumValue { get; set; }

			public EnumData(string typeSummary, string enumName, string enumSummary, int enumValue)
			{
				TypeSummary = typeSummary;
				EnumName = enumName;
				EnumSummary = enumSummary;
				EnumValue = enumValue;
			}
		}

		private const string Namespace = "Sc.Common";

        private const string YamlFileName = "application.common";

        private const string PhpFileName = "common";

        private const string YamlTab = "    ";

        private const string EnumPath = "Enum/";
        private const string YamlPath = "Yaml/";
        private const string PhpPath = "php/";
        private const string BeforeIOS = "ios";
		private const string AfterIOS = "iOS";

		private const string EnumOutputDir = "Enum/";
        private const string YamlOutputDir = "Yaml/";
        private const string PhpOutputDir = "Php/";

        private const int DataStartRow = 7;

		private const int TypeNameCol = 5;
		private const int TypeSummaryCol = 6;
		private const int EnumNameCol = 7;
		private const int EnumSummaryCol = 8;
		private const int EnumValueCol = 3;

		private XlsBook Book { get; set; }
		private string Dir { get; set; }

		private Dictionary<string, List<EnumData>> EnumTypeDict { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">シート</param>
		public EnumBook(XlsBook book, string dir)
		{
			Book = book;
			Dir = dir;

			Directory.CreateDirectory(Dir + EnumOutputDir);
            Directory.CreateDirectory(Dir + YamlOutputDir);
            Directory.CreateDirectory(Dir + PhpOutputDir);
        }

        /// <summary>
        /// クラスを生成
        /// </summary>
        public void GenerateEnums()
		{
			EnumTypeDict = new Dictionary<string, List<EnumData>>();

			foreach (var sheet in Book.Sheets)
			{
				InputEnumData(sheet);
			}

			GenerateAllEnum(Dir + EnumPath);
            GenerateAllYaml(Dir + YamlPath);
            GenerateAllPhp(Dir + PhpPath);
        }

		/// <summary>
		/// 列挙データを入力
		/// </summary>
		/// <param name="sheet">シート</param>
		private void InputEnumData(XlsSheet sheet)
		{
			var row = DataStartRow;

			while (true)
			{
				if (sheet.Fields.Count <= row) { break; }

				var typeName = sheet.Fields[row][TypeNameCol];

				if (string.IsNullOrEmpty(typeName))
				{
					row++;
					continue;
				}

				var typeSummary = sheet.Fields[row][TypeSummaryCol];
				var enumName = sheet.Fields[row][EnumNameCol];
				var enumSummary = sheet.Fields[row][EnumSummaryCol];
				var enumValue = int.Parse(sheet.Fields[row][EnumValueCol]);

				var enumData = new EnumData(typeSummary, enumName, enumSummary, enumValue);

				if (EnumTypeDict.ContainsKey(typeName))
				{
					EnumTypeDict[typeName].Add(enumData);
				}
				else
				{
					EnumTypeDict.Add(typeName, new List<EnumData>() { enumData });
				}

				row++;
			}
		}

		/// <summary>
		/// Enum ファイルを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private void GenerateAllEnum(string directory)
		{
			foreach (var keyValue in EnumTypeDict)
			{
				var typeName = keyValue.Key.SnakeToPascal();
				var enumDataList = keyValue.Value;

				if (enumDataList == null) { continue; }
				if (enumDataList.Count == 0) { continue; }

				var typeSummary = enumDataList[0].TypeSummary;

				var sb = new StringBuilder();

				sb.AppendLine();
				sb.AppendLine("namespace " + Namespace);
				sb.AppendLine("{");
				sb.AppendLine(Tab + "/// <summary>");
				sb.AppendLine(Tab + "/// " + typeSummary);
				sb.AppendLine(Tab + "/// </summary>");
				sb.AppendLine(Tab + "public enum " + typeName);
				sb.Append(Tab + "{");

				foreach (var enumData in enumDataList)
				{
					var enumName = enumData.EnumName;
					if (enumName.Contains(BeforeIOS))
					{
						enumName = AfterIOS;
					}
					else
					{
						enumName = StringUtility.SnakeToPascal(enumName);
					}
					var enumValue = enumData.EnumValue;
					var enumSummary = enumData.EnumSummary;

					sb.AppendLine();
					sb.AppendLine(Tab + Tab + "/// <summary>" + enumSummary + "</summary>");
					sb.AppendLine(Tab + Tab + enumName + " = " + enumValue + ",");
				}

				sb.AppendLine(Tab + "}");
				sb.AppendLine("}");

				File.WriteAllText(directory + typeName + ".cs", sb.ToString(), Encoding.UTF8);
			}
		}

        /// <summary>
		/// Enum ファイルを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private void GenerateAllYaml(string directory)
        {
            var sb = new StringBuilder();

            sb.AppendLine("####################################");
            sb.AppendLine("### アプリサーバー間共通定数定義 ###");
            sb.AppendLine("####################################");
            sb.AppendLine();

            foreach (var keyValue in EnumTypeDict)
            {
                var typeName = StringUtility.CheckReservedWord(keyValue.Key);
                var enumDataList = keyValue.Value;

                if (enumDataList == null) { continue; }
                if (enumDataList.Count == 0) { continue; }

                var typeSummary = enumDataList[0].TypeSummary;

                sb.AppendLine("### " + typeSummary);
                sb.AppendLine(typeName + ":");

                foreach (var enumData in enumDataList)
                {
                    var enumName = StringUtility.CheckReservedWord(enumData.EnumName);
                    var enumValue = enumData.EnumValue;
                    var enumSummary = enumData.EnumSummary;

                    sb.AppendLine(YamlTab + enumName + ": " + Tab + Tab + enumValue + Tab + Tab + "# " + enumSummary);
                }

                sb.AppendLine();
            }

            var utf8Encoding = new UTF8Encoding(false);
            File.WriteAllText(directory + YamlFileName + ".yaml", sb.ToString(), utf8Encoding);
        }

        /// <summary>
        /// Php ファイルを生成
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        private void GenerateAllPhp(string directory)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?php");
            sb.AppendLine("");
            sb.AppendLine("/**");
            sb.AppendLine(" * アプリサーバー間共通定数定義");
            sb.AppendLine("*/");
            sb.AppendLine();

            foreach (var keyValue in EnumTypeDict)
            {
                var typeName = StringUtility.CheckReservedWord(keyValue.Key);
                var enumDataList = keyValue.Value;

                if (enumDataList == null) { continue; }
                if (enumDataList.Count == 0) { continue; }

                var typeSummary = enumDataList[0].TypeSummary;

                sb.AppendLine("// " + typeSummary);

                foreach (var enumData in enumDataList)
                {
                    var enumName = enumData.EnumName;
                    var enumValue = enumData.EnumValue;
                    var enumSummary = enumData.EnumSummary;

                    var deffineName = (typeName + "_" + enumName).ToUpper();
                    sb.AppendLine("define('" + deffineName + "',\"" + enumValue + "\");" + " //" + enumSummary);
                }

                sb.AppendLine();
            }

            File.WriteAllText(directory + PhpFileName + ".php", sb.ToString());
        }
    }
}
