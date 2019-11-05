using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ExcelReader;
using Alim.Utility;

namespace ScWebInterfaceGenerator
{
	public class ActionSheet : XlsElement
	{
		private const string Namespace = "Sc.Web";

		private const string RequestPath = "Request/";
		private const string ResponsePath = "Response/";
		private const string ActionPath = "Action/";

		private const int DataStartCol = 1;

		private const int ActionRow = 4;
		private const int ActionNameCol = 1;
		private const int ActionSummaryCol = 26;

		private const string ReqestStartKey = "Request key";
		private const string ResponseStartKey = "Response key";
		private const string BodyStartKey = "T_BODY";

		private const int FieldNameCol = 8;
		private const int FieldSummaryCol = 26;
		private const int FieldTypeCol = 38;

		private const string SingleFieldType = "SINGLE";
		private const string ListFieldType = "LIST";

		private XlsSheet Sheet { get; set; }
		private string ActionName { get; set; }
		private string ActionSummary { get; set; }
		private int RequestBodyStartRow { get; set; }
		private int ResponseBodyStartRow { get; set; }

		public static string Dir { get; set; }

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="dir">ディレクトリ</param>
		public static void Initialize(string dir)
		{
			Dir = dir;

			Directory.CreateDirectory(Dir);
			Directory.CreateDirectory(Dir + RequestPath);
			Directory.CreateDirectory(Dir + ResponsePath);
			Directory.CreateDirectory(Dir + ActionPath);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">シート</param>
		public ActionSheet(XlsSheet sheet)
		{
			Sheet = sheet;
			ActionName = StringUtility.SnakeToPascal(Sheet.Fields[ActionRow][ActionNameCol].ToLower());
			ActionSummary = Sheet.Fields[ActionRow][ActionSummaryCol];
			RequestBodyStartRow = GetRequestBodyStartRow(Sheet);
			ResponseBodyStartRow = GetResponseBodyStartRow(Sheet);
		}

		/// <summary>
		/// クラスを生成
		/// </summary>
		public void GenerateClasses()
		{
			GenerateRequestClass(Dir + RequestPath);
			GenerateResponseClass(Dir + ResponsePath);
			GenerateActionClass(Dir + ActionPath);
		}

		/// <summary>
		/// リクエストクラスを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private void GenerateRequestClass(string directory)
		{
			var sb = new StringBuilder();

			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("using Sc.Common;");
			sb.AppendLine();
			sb.AppendLine("namespace " + Namespace);
			sb.AppendLine("{");
			sb.AppendLine(Tab + "/// <summary>");
			sb.AppendLine(Tab + "/// " + ActionSummary + "リクエストキー");
			sb.AppendLine(Tab + "/// </summary>");
			sb.AppendLine(Tab + "[Serializable]");
			sb.AppendLine(Tab + "public class " + ActionName + "RequestKey : RequestKeyBase");
			sb.AppendLine(Tab + "{");

			var row = RequestBodyStartRow;

			while (true)
			{
				if (!Regex.IsMatch(Sheet.Fields[row][FieldNameCol], @"^[a-zA-Z]")) { break; }

				var fieldName = Sheet.Fields[row][FieldNameCol];
				var typeName = StringUtility.SnakeToPascal(fieldName.ToLower());
				var propertyName = typeName.Substring(1);
				var fieldSummary = Sheet.Fields[row][FieldSummaryCol];
				var isFieldTypeList = true;//Sheet.Fields[row][FieldTypeCol].Contains(ListFieldType);
				var fieldType = isFieldTypeList ? ("List<" + typeName + ">") : typeName;

				if (row != RequestBodyStartRow) { sb.AppendLine(); }

				sb.AppendLine(Tab + Tab + "/// <summary>" + fieldSummary + "</summary>");
				sb.AppendLine(Tab + Tab + "public " + fieldType + " " + propertyName + " { get { return " + fieldName + "; } set { " + fieldName + " = value; } }");
				sb.AppendLine(Tab + Tab + "[SerializeField]");
				sb.AppendLine(Tab + Tab + "private " + fieldType + " " + fieldName + ";");

				row++;
			}

			sb.AppendLine(Tab + "}");
			sb.AppendLine("}");

			File.WriteAllText(directory + ActionName + "Request.cs", sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// レスポンスクラスを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private void GenerateResponseClass(string directory)
		{
			var sb = new StringBuilder();

			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("using Sc.Common;");
			sb.AppendLine();
			sb.AppendLine("namespace " + Namespace);
			sb.AppendLine("{");
			sb.AppendLine(Tab + "/// <summary>");
			sb.AppendLine(Tab + "/// " + ActionSummary + "レスポンスキー");
			sb.AppendLine(Tab + "/// </summary>");
			sb.AppendLine(Tab + "[Serializable]");
			sb.AppendLine(Tab + "public class " + ActionName + "ResponseKey : ResponseKeyBase");
			sb.AppendLine(Tab + "{");

			var row = ResponseBodyStartRow;

			while (true)
			{
				if (!Regex.IsMatch(Sheet.Fields[row][FieldNameCol], @"^[a-zA-Z]")) { break; }

				var fieldName = Sheet.Fields[row][FieldNameCol];
				var typeName = StringUtility.SnakeToPascal(fieldName.ToLower());
				var jenericsName = StringUtility.DelUpdString(typeName);
				var propertyName = typeName.Substring(1);
				var fieldSummary = Sheet.Fields[row][FieldSummaryCol];
				var isFieldTypeList = true;//Sheet.Fields[row][FieldTypeCol].Contains(ListFieldType);
				var fieldType = isFieldTypeList ? ("List<" + jenericsName + ">") : jenericsName;

				if (row != ResponseBodyStartRow) { sb.AppendLine(); }

				sb.AppendLine(Tab + Tab + "/// <summary>" + fieldSummary + "</summary>");
				sb.AppendLine(Tab + Tab + "public " + fieldType + " " + propertyName + " { get { return " + fieldName + "; } set { " + fieldName + " = value; } }");
				sb.AppendLine(Tab + Tab + "[SerializeField]");
				sb.AppendLine(Tab + Tab + "private " + fieldType + " " + fieldName + ";");

				row++;
			}

			sb.AppendLine(Tab + "}");
			sb.AppendLine("}");

			File.WriteAllText(directory + ActionName + "Response.cs", sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// アクションクラスを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private void GenerateActionClass(string directory)
		{
			var sb = new StringBuilder();

			sb.AppendLine("using System;");
			sb.AppendLine("using Sc.Common;");
			sb.AppendLine();
			sb.AppendLine("namespace " + Namespace);
			sb.AppendLine("{");
			sb.AppendLine(Tab + "/// <summary>");
			sb.AppendLine(Tab + "/// " + ActionSummary + "アクション");
			sb.AppendLine(Tab + "/// </summary>");
			sb.AppendLine(Tab + "[Serializable]");
			sb.AppendLine(Tab + "public class " + ActionName + "Action : ActionBase<" + ActionName + "RequestKey, " + ActionName + "ResponseKey>");
			sb.AppendLine(Tab + "{");

			sb.AppendLine(Tab + Tab + "/// <summary>URL</summary>");
			sb.AppendLine(Tab + Tab + "protected override string Url { get { return ActionUrlConst.Url" + ActionName + "; } }");
			sb.AppendLine();
			sb.AppendLine(Tab + Tab + "/// <summary>暗号化キー</summary>");
			sb.AppendLine(Tab + Tab + "protected override string CipherKey { get { return ConnectKeyConst.Atk" + ActionName + "; } }");
			sb.AppendLine();
			sb.AppendLine(Tab + Tab + "/// <summary>");
			sb.AppendLine(Tab + Tab + "/// コンストラクタ");
			sb.AppendLine(Tab + Tab + "/// </summary>");
			sb.AppendLine(Tab + Tab + "public " + ActionName + "Action()");
			sb.AppendLine(Tab + Tab + "{");
			sb.AppendLine(Tab + Tab + Tab + "Request = new RequestBase<" + ActionName + "RequestKey>()");
			sb.AppendLine(Tab + Tab + Tab + "{");
			sb.AppendLine(Tab + Tab + Tab + Tab + "Header = new THeader()");
			sb.AppendLine(Tab + Tab + Tab + Tab + "{");
			sb.AppendLine(Tab + Tab + Tab + Tab + Tab + "ActionType = ConnectKeyConst.At" + ActionName);
			sb.AppendLine(Tab + Tab + Tab + Tab + "},");
			sb.AppendLine(Tab + Tab + Tab + Tab + "Body = new RequestBodyBase<" + ActionName + "RequestKey>()");
			sb.AppendLine(Tab + Tab + Tab + Tab + "{");
			sb.AppendLine(Tab + Tab + Tab + Tab + Tab + "Key = new " + ActionName + "RequestKey()");
			sb.AppendLine(Tab + Tab + Tab + Tab + "}");
			sb.AppendLine(Tab + Tab + Tab + "};");
			sb.AppendLine(Tab + Tab + "}");

			sb.AppendLine(Tab + "}");
			sb.AppendLine("}");

			File.WriteAllText(directory + ActionName + "Action.cs", sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// シートのリクエストボディの開始行を取得
		/// </summary>
		/// <param name="sheet">シート</param>
		/// <returns>リクエストの開始行</returns>
		private int GetRequestBodyStartRow(XlsSheet sheet)
		{
			var isStart = false;

			for (int row = 0; row < sheet.RowCount; row++)
			{
				if (sheet.Fields[row][DataStartCol].Contains(ReqestStartKey))
				{
					isStart = true;
				}

				if (isStart && sheet.Fields[row][DataStartCol].Contains(BodyStartKey))
				{
					return row;
				}
			}

			return 0;
		}

		/// <summary>
		/// シートのレスポンスボディの開始行を取得
		/// </summary>
		/// <param name="sheet">シート</param>
		/// <returns>レスポンスの開始行</returns>
		private int GetResponseBodyStartRow(XlsSheet sheet)
		{
			var isStart = false;

			for (int row = 0; row < sheet.RowCount; row++)
			{
				if (sheet.Fields[row][DataStartCol].Contains(ResponseStartKey))
				{
					isStart = true;
				}

				if (isStart && sheet.Fields[row][DataStartCol].Contains(BodyStartKey))
				{
					return row;
				}
			}

			return 0;
		}
	}
}
