using Alim.Utility;
using ExcelReader;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ScChronosYmlGenerator
{
    public class ActionSheet : XlsElement
	{
		private const string Namespace = "Sc.Multi.Web";

        private const string ActionPath = "action/";

        private const int DataStartCol = 1;

		private const int ActionRow = 4;
		private const int ActionNameCol = 1;
		private const int ActionSummaryCol = 26;

		private const string ReqestStartKey = "Request key";
		private const string ResponseStartKey = "Response key";
        private const string TransactionStartKey = "Transaction key";
        private const string CommonResponseStartKey = "CommonResponse";
        private const string BodyStartKey = "T_BODY";

        private const int HeaderFieldNameCol = 1;
        private const int FieldNameCol = 8;
        private const int RequestNameCol = 1;
        private const int FieldSummaryCol = 26;
		private const int FieldTypeCol = 38;
        private const int FieldActionId = 47;

        private const string SingleFieldType = "SINGLE";
		private const string ListFieldType = "LIST";

		private XlsSheet Sheet { get; set; }
		private string ActionName { get; set; }
		private string ActionSummary { get; set; }
        private int RequestHeaderStartRow { get; set; }
        private int RequestBodyStartRow { get; set; }
        private int ResponseHeaderStartRow { get; set; }
        private int ResponseBodyStartRow { get; set; }
        private int TransactionBodyStartRow { get; set; }
        private int CommonResponseBodyStartRow { get; set; }

        private int MaxRowCount { get; set; }

        private string Category { get; set; }

        private string Request { get; set; }

        private string RequestType { get; set; }

        private string RequestName { get; set; }

        private string ActionId { get; set; }

        public static string Dir { get; set; }

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="dir">ディレクトリ</param>
		public static void Initialize(string dir)
		{
            Dir = dir;

			Directory.CreateDirectory(Dir);
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
            TransactionBodyStartRow = GetTransactionBodyStartRow(sheet);
            CommonResponseBodyStartRow = GetCommonResponseBodyStartRow(sheet);
            MaxRowCount = sheet.RowCount;

            RequestName = Sheet.Fields[ActionRow][RequestNameCol].ToLower(); // 小文字に変換
            RequestType = Sheet.Fields[ActionRow][FieldTypeCol];
            ActionId = Sheet.Fields[ActionRow][FieldActionId];

        }

		/// <summary>
		/// クラスを生成
		/// </summary>
		public void GenerateClasses()
		{
            GenerateClass(Dir + ActionPath);
		}

        /// <summary>
        /// アクションYmlを生成
        /// </summary>
        private void GenerateClass(string directory)
		{
            var sb = new StringBuilder();
            var structureNameList = new List<string>();
            int endRow = 0;

            sb.AppendLine("swagger: '2.0'");
            sb.AppendLine("info:");
            sb.AppendLine("  description: sc-API");
            sb.AppendLine("  version: 0.0.1");
            sb.AppendLine("  title: Project-SC APIスキーマ");
            sb.AppendLine("paths:");
            sb.AppendLine("  '/" + RequestType + "/" + RequestName + "':");
            sb.AppendLine("    x-action-type: " + ActionId);
            sb.AppendLine("    post:");
            sb.AppendLine("      description: " + ActionSummary.Trim());

            // RequestKey欄の解析
            sb.AppendLine("      parameters:");
            sb.AppendLine("        - in: body");
            sb.AppendLine("          name: T_HEADER");
            sb.AppendLine("          schema:");
            sb.AppendLine("            type: object");
            sb.AppendLine("            properties:");

            // RequestKey Header
            var isFirst = false;
            for (int row = RequestHeaderStartRow; row < RequestBodyStartRow; row++)
            {
                if (isFirst == false)
                {
                    isFirst = true;
                    
                }
                var structureName = Sheet.Fields[row][HeaderFieldNameCol];
                var structureNameServer = StringUtility.SnakeToCamel(Sheet.Fields[row][HeaderFieldNameCol].ToLower()).Substring(1);
                sb.AppendLine("              " + structureName + ":");
                sb.AppendLine("                $ref: \"#/definitions/" + structureNameServer + "\"");

                structureNameList.Add(structureNameServer);
            }

            //sb.AppendLine("            properties:");
            //sb.AppendLine("              T_HEADER:");
            //sb.AppendLine("                $ref: \"#/definitions/Header\"");

            isFirst = false;
            for (int row = RequestBodyStartRow; row < ResponseBodyStartRow; row++)
            {
                if (isFirst == false)
                {
                    isFirst = true;
                    sb.AppendLine("              T_BODY:");
                    sb.AppendLine("                type: object");
                    sb.AppendLine("                properties:");
                }
                if (Sheet.Fields[row][FieldNameCol].Contains("可変※1")) { continue; }
                if (!Regex.IsMatch(Sheet.Fields[row][FieldNameCol], @"^[_0-9a-zA-Z]")) { continue; }
                var structureName = Sheet.Fields[row][FieldNameCol];
                var structureNameServer = StringUtility.SnakeToCamel(Sheet.Fields[row][FieldNameCol].ToLower()).Substring(1);
                var isFieldTypeList = Sheet.Fields[row][FieldTypeCol].Contains(ListFieldType);
                if (isFieldTypeList == true)
                {
                    sb.AppendLine("                  " + structureName + ":");
                    sb.AppendLine("                    type: array");
                    sb.AppendLine("                    items:");
                    sb.AppendLine("                      $ref: \"#/definitions/" + structureNameServer + "\"");
                }
                else
                {
                    sb.AppendLine("                  " + structureName + ":");
                    sb.AppendLine("                    $ref: \"#/definitions/" + structureNameServer + "\"");
                }


                structureNameList.Add(structureNameServer);
            }

            // ResponseKey欄の解析
            sb.AppendLine("      responses:");
            sb.AppendLine("        200:");
            sb.AppendLine("          description: ok");
            sb.AppendLine("          schema:");
            sb.AppendLine("            type: object");
            sb.AppendLine("            properties:");

            // ResponseKey Header
            isFirst = false;
            for (int row = ResponseHeaderStartRow; row < ResponseBodyStartRow; row++)
            {
                if (isFirst == false)
                {
                    isFirst = true;
                    
                }

                var structureName = Sheet.Fields[row][HeaderFieldNameCol];
                var structureNameServer = StringUtility.SnakeToCamel(Sheet.Fields[row][HeaderFieldNameCol].ToLower()).Substring(1);
                sb.AppendLine("              " + structureName + ":");
                sb.AppendLine("                $ref: \"#/definitions/" + structureNameServer + "\"");

                structureNameList.Add(structureNameServer);
            }

            //sb.AppendLine("              T_HEADER:");
            //sb.AppendLine("                $ref: \"#/definitions/Header\"");
            //sb.AppendLine("              T_MESSAGE:");
            //sb.AppendLine("                $ref: \"#/definitions/Message\"");

            isFirst = false;
            // TransactionResponseが存在しない場合がある
            if (TransactionBodyStartRow > 0)
            {
                endRow = TransactionBodyStartRow;
            }
            else
            {
                if (CommonResponseBodyStartRow > 0)
                {
                    endRow = CommonResponseBodyStartRow;
                }
                else
                {
                    endRow = MaxRowCount;
                }
            }
            for (int row = ResponseBodyStartRow; row < endRow; row++)
            {
                if (isFirst == false)
                {
                    isFirst = true;
                    sb.AppendLine("              T_BODY:");
                    sb.AppendLine("                type: object");
                    sb.AppendLine("                properties:");
                }

                if (Sheet.Fields[row][FieldNameCol].Contains("可変※1")) { continue; }
                if (!Regex.IsMatch(Sheet.Fields[row][FieldNameCol], @"^[_0-9a-zA-Z]")) { continue; }
                var structureName = Sheet.Fields[row][FieldNameCol];
                var structureNameServer = StringUtility.SnakeToCamel(Sheet.Fields[row][FieldNameCol].ToLower()).Substring(1);
                var isFieldTypeList = Sheet.Fields[row][FieldTypeCol].Contains(ListFieldType);
                if (isFieldTypeList == true)
                {
                    sb.AppendLine("                  " + structureName + ":");
                    sb.AppendLine("                    type: array");
                    sb.AppendLine("                    items:");
                    sb.AppendLine("                      $ref: \"#/definitions/" + structureNameServer + "\"");
                }
                else
                {
                    sb.AppendLine("                  " + structureName + ":");
                    sb.AppendLine("                    $ref: \"#/definitions/" + structureNameServer + "\"");
                }

                structureNameList.Add(structureNameServer);
            }


            // TransactionKey欄の解析
            if (TransactionBodyStartRow > 0)
            {
                for (int row = TransactionBodyStartRow; row < CommonResponseBodyStartRow; row++)
                {
                    if (Sheet.Fields[row][FieldNameCol].Contains("可変※1")) { continue; }
                    if (!Regex.IsMatch(Sheet.Fields[row][FieldNameCol], @"^[_0-9a-zA-Z]")) { continue; }
                }
            }


            // CommonResponse欄の解析
            if (CommonResponseBodyStartRow > 0)
            {
                for (int row = CommonResponseBodyStartRow; row < MaxRowCount; row++)
                {
                    if (Sheet.Fields[row][FieldNameCol].Contains("可変※1")) { continue; }
                    if (!Regex.IsMatch(Sheet.Fields[row][FieldNameCol], @"^[_0-9a-zA-Z]")) { continue; }
                }
            }

            // includeストラクチャ抽出
            IEnumerable<string> structureNameListOut = structureNameList.Distinct(); // 重複ストラクチャ削除
            sb.AppendLine("");

            sb.AppendLine("definitions:");
            foreach (string structureName in structureNameListOut)
            {
                sb.AppendLine("include: ../structure/" + structureName + ".yaml");
            }

            File.WriteAllText(directory + ActionName + ".yaml", sb.ToString());
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
                    RequestHeaderStartRow = row + 1;
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
                    ResponseHeaderStartRow = row + 1;

                }

				if (isStart && sheet.Fields[row][DataStartCol].Contains(BodyStartKey))
				{
					return row;
				}
			}

			return 0;
		}

        /// <summary>
        /// シートのトランザクションボディの開始行を取得
        /// </summary>
        /// <param name="sheet">シート</param>
        /// <returns>レスポンスの開始行</returns>
        private int GetTransactionBodyStartRow(XlsSheet sheet)
        {
            var isStart = false;

            for (int row = 0; row < sheet.RowCount; row++)
            {
                if (sheet.Fields[row][DataStartCol].Contains(TransactionStartKey))
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
        /// シートの共通レスポンスボディの開始行を取得
        /// </summary>
        /// <param name="sheet">シート</param>
        /// <returns>レスポンスの開始行</returns>
        private int GetCommonResponseBodyStartRow(XlsSheet sheet)
        {
            var isStart = false;

            for (int row = 0; row < sheet.RowCount; row++)
            {
                if (sheet.Fields[row][DataStartCol].Contains(CommonResponseStartKey))
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
