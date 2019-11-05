using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ExcelReader;
using Alim.Utility;

namespace ScDefineGenerator
{
	public class TextBook : XlsElement
	{
		/// <summary>
		/// テキストデータ
		/// </summary>
		private class TextData
		{
			public string DefineKey { get; set; }
			public string Name { get; set; }

			public TextData(string defineKey, string name)
			{
				DefineKey = defineKey;
				Name = name;
			}
		}

		/// <summary> ネームスペース </summary>
		private const string Namespace = "Sc.Common";
		/// <summary> テンプレートディレクトリ </summary>
		private const string TemplateDir = "template/";
		/// <summary> テンプレートファイル名 </summary>
		private const string TemplateTextFileName = "TextMasterTemplate.cs.txt";
		/// <summary> テンプレートファイル名 </summary>
		private const string TemplateInitialFileName = "InitialTextMasterTemplate.cs.txt";
		/// <summary> 出力ファイルフォーマット </summary>
		private const string ExportFileFormat = "{0}Master.cs";

		/// <summary> Const 出力先ディレクトリ </summary>
		private const string ConstOutputDir = "Const/";
		/// <summary> Enum 出力先ディレクトリ </summary>
		private const string EnumOutputDir = "Enum/";
		/// <summary> MasterData 出力先ディレクトリ </summary>
		private const string MasterDataOutputDir = "MasterData/";

		/// <summary> スクリプト名置き換えキー </summary>
		private const string ScriptNameReplaceKey = "#SCRIPTNAME#";
		/// <summary> シート名置き換えキー </summary>
		private const string SheetNameReplaceKey = "#SHEETNAME#";
		/// <summary> ディクショナリ定義置き換えキー </summary>
		private const string DictionaryReplaceKey = "#DICTIONARY#";
		/// <summary> コンストラクタ置き換えキー </summary>
		private const string ConstructorReplaceKey = "#CONSTRUCTOR#";
		/// <summary> インデクサ置き換えキー </summary>
		private const string IndexerReplaceKey = "#INDEXER#";
		/// <summary> デシリアライズ後置き換えキー </summary>
		private const string AfterDeserializeReplaceKey = "#AFTERDESERIALIZE#";

		private const int DefineStartRow = 7;

		private const int CategoryKeyCol = 4;
		private const int DefineKeyCol = 3;
		private const int CommentCol = 5;

		private XlsBook Book { get; set; }
		private string Dir { get; set; }
		private string MasterClassName { get; set; }

		private List<XlsSheet> SheetList { get; set; }

		private Dictionary<string, List<TextData>> CategoryDict { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">シート</param>
		public TextBook(XlsBook book, string dir, string masterClassName)
		{
			Book = book;
			Dir = dir;
			MasterClassName = masterClassName;

			SheetList = new List<XlsSheet>();

			foreach (var sheet in Book.Sheets)
			{
				SheetList.Add(sheet);
			}

			Directory.CreateDirectory(Dir + ConstOutputDir);
			Directory.CreateDirectory(Dir + EnumOutputDir);
			Directory.CreateDirectory(Dir + MasterDataOutputDir);
		}

		/// <summary>
		/// クラスを生成
		/// </summary>
		public void GenerateClass()
		{
			CategoryDict = new Dictionary<string, List<TextData>>();

			var textMstKeyName = MasterClassName + "MstKey";
			CategoryDict.Add(textMstKeyName, new List<TextData>());

			// 全シートからカテゴリーキー別に辞書へ登録
			foreach (var sheet in SheetList)
			{
				var row = DefineStartRow;

				while (true)
				{
					if (string.IsNullOrEmpty(sheet.Fields[row][DefineKeyCol])) { break; }

					var categoryKey = sheet.Fields[row][CategoryKeyCol];
					if (string.IsNullOrEmpty(categoryKey))
					{
						categoryKey = textMstKeyName;
					}

					var defineKeyName = sheet.Fields[row][DefineKeyCol];
					var commentName = sheet.Fields[row][CommentCol];
					var textData = new TextData(defineKeyName, commentName);

					if (CategoryDict.ContainsKey(categoryKey))
					{
						CategoryDict[categoryKey].Add(textData);
					}
					else
					{
						CategoryDict.Add(categoryKey, new List<TextData>() { textData });
					}

					row++;
				}
			}

			var sb = new StringBuilder();

			sb.AppendLine("namespace " + Namespace);
			sb.Append("{");

			// *TextMstKey の enum のみ生成
			var textDataList = CategoryDict[textMstKeyName];

			sb.AppendLine();
			sb.AppendLine(Tab + "public enum " + textMstKeyName);
			sb.Append(Tab + "{");

			foreach (var textData in textDataList)
			{
				sb.AppendLine();
				sb.AppendLine(Tab + Tab + "/// <summary>" + textData.Name + "</summary>");
				sb.AppendLine(Tab + Tab + textData.DefineKey + ",");
			}

			sb.AppendLine(Tab + "}");
			sb.AppendLine("}");

			File.WriteAllText(Dir + EnumOutputDir + textMstKeyName + ".cs", sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// マスターデータクラスを生成
		/// </summary>
		public void GenerateTextMasterClass()
		{
			var resourceFile = Path.Combine(TemplateDir, TemplateTextFileName);

			var encoding = new UTF8Encoding(true, false);
			var classScript = File.ReadAllText(resourceFile, encoding);
			classScript = classScript.Replace(ScriptNameReplaceKey, MasterClassName);

			var dictionary = GetPropertyDefinitionString();
			classScript = classScript.Replace(DictionaryReplaceKey, dictionary);

			var constructor = GetConstructorDefinitionString();
			classScript = classScript.Replace(ConstructorReplaceKey, constructor);

			var indexer = GetIndexerDefinitionString();
			classScript = classScript.Replace(IndexerReplaceKey, indexer);

			var afterDeserialize = GetAfterDeserialize();
			classScript = classScript.Replace(AfterDeserializeReplaceKey, afterDeserialize);

			var fileName = string.Format(ExportFileFormat, MasterClassName);
			var filePath = Path.Combine(Dir + MasterDataOutputDir, fileName);

			File.WriteAllText(filePath, classScript, encoding);
		}

		/// <summary>
		/// マスターデータクラスを生成
		/// </summary>
		public void GenerateInitialTextMasterClass()
		{
			var resourceFile = Path.Combine(TemplateDir, TemplateInitialFileName);

			var encoding = new UTF8Encoding(true, false);
			var classScript = File.ReadAllText(resourceFile, encoding);
			classScript = classScript.Replace(ScriptNameReplaceKey, MasterClassName);

			var dictionary = GetPropertyDefinitionString();
			classScript = classScript.Replace(DictionaryReplaceKey, dictionary);

			var constructor = GetConstructorDefinitionString();
			classScript = classScript.Replace(ConstructorReplaceKey, constructor);

			var indexer = GetIndexerDefinitionString();
			classScript = classScript.Replace(IndexerReplaceKey, indexer);

			var afterDeserialize = GetAfterDeserialize();
			classScript = classScript.Replace(AfterDeserializeReplaceKey, afterDeserialize);

			var fileName = string.Format(ExportFileFormat, MasterClassName);
			var filePath = Path.Combine(Dir + MasterDataOutputDir, fileName);

			File.WriteAllText(filePath, classScript, encoding);
		}

		/// <summary>
		/// プロパティ定義文字列を取得
		/// </summary>
		/// <returns>プロパティ定義文字列</returns>
		private string GetPropertyDefinitionString()
		{
			var sb = new StringBuilder();

			foreach (var categoryKey in CategoryDict.Keys)
			{
				sb.AppendLine(Tab + Tab + "private Dictionary<" + categoryKey + ", " + MasterClassName + "MasterData> " + categoryKey + "Dict { get; set; }");
			}

			return sb.ToString();
		}

		/// <summary>
		/// コンストラクタ定義文字列を取得
		/// </summary>
		/// <returns>コンストラクタ定義文字列</returns>
		private string GetConstructorDefinitionString()
		{
			var sb = new StringBuilder();

			foreach (var categoryKey in CategoryDict.Keys)
			{
				sb.AppendLine(Tab + Tab + Tab + categoryKey + "Dict = new Dictionary<" + categoryKey + ", " + MasterClassName + "MasterData>();");
			}

			return sb.ToString();
		}

		/// <summary>
		/// インデクサー定義文字列を取得
		/// </summary>
		/// <returns>インデクサー定義文字列</returns>
		private string GetIndexerDefinitionString()
		{
			var sb = new StringBuilder();

			foreach (var categoryKey in CategoryDict.Keys)
			{
				sb.AppendLine(Tab + Tab + "public string this[" + categoryKey + " key]");
				sb.AppendLine(Tab + Tab + "{");
				sb.AppendLine(Tab + Tab + Tab + "get");
				sb.AppendLine(Tab + Tab + Tab + "{");
				sb.AppendLine(Tab + Tab + Tab + Tab + MasterClassName + "MasterData data;");
				sb.AppendLine(Tab + Tab + Tab + Tab + "if (" + categoryKey + "Dict.TryGetValue(key, out data))");
				sb.AppendLine(Tab + Tab + Tab + Tab + "{");
				sb.AppendLine(Tab + Tab + Tab + Tab + Tab + "return data.TextValue;");
				sb.AppendLine(Tab + Tab + Tab + Tab + "}");
				sb.AppendLine(Tab + Tab + Tab + Tab + "return \"\";");
				sb.AppendLine(Tab + Tab + Tab + "}");
				sb.AppendLine(Tab + Tab + "}");
			}

			return sb.ToString();
		}

		/// <summary>
		/// デシリアライズ後処理の文字列を取得
		/// </summary>
		/// <returns>デシリアライズ後処理の文字列</returns>
		private string GetAfterDeserialize()
		{
			var sb = new StringBuilder();
			var fourTab = Tab + Tab + Tab + Tab;

			foreach (var categoryKey in CategoryDict.Keys)
			{
				sb.AppendLine(fourTab + "case \"" + categoryKey + "\":");
				sb.AppendLine(fourTab + "{");
				sb.AppendLine(fourTab + Tab + "if (Enum.IsDefined(typeof(" + categoryKey + "), data.TextDefineKey))");
				sb.AppendLine(fourTab + Tab + "{");
				sb.AppendLine(fourTab + Tab + Tab + "var defineKey = (" + categoryKey + ")Enum.Parse(typeof(" + categoryKey + "), data.TextDefineKey, true);");
				sb.AppendLine(fourTab + Tab + Tab + "if (" + categoryKey + "Dict.ContainsKey(defineKey))");
				sb.AppendLine(fourTab + Tab + Tab + "{");
				sb.AppendLine(fourTab + Tab + Tab + Tab + "Debug.LogWarningFormat(\"{0} TextDefineKey = {1} is already exist.\", GetType().Name, data.TextDefineKey);");
				sb.AppendLine(fourTab + Tab + Tab + "}");
				sb.AppendLine(fourTab + Tab + Tab + "else");
				sb.AppendLine(fourTab + Tab + Tab + "{");
				sb.AppendLine(fourTab + Tab + Tab + Tab + categoryKey + "Dict.Add(defineKey, data);");
				sb.AppendLine(fourTab + Tab + Tab + "}");
				sb.AppendLine(fourTab + Tab + "}");
				sb.AppendLine(fourTab + Tab + "else");
				sb.AppendLine(fourTab + Tab + "{");
				sb.AppendLine(fourTab + Tab + Tab + "Debug.LogWarningFormat(\"{0} TextDefineKey = {1} is not defined.\", GetType().Name, data.TextDefineKey);");
				sb.AppendLine(fourTab + Tab + "}");
				sb.AppendLine(fourTab + Tab + "break;");
				sb.AppendLine(fourTab + "}");
			}

			return sb.ToString();
		}
	}
}
