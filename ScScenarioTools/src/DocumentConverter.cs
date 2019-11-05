using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScScenarioCommon;

namespace ScScenarioTools
{
	public class DocumentConverter
	{
		/// <summary>
		/// ドキュメント化
		/// </summary>
		/// <param name="argTypeFile">ArgType用の引数情報CSVファイル</param>
		/// <param name="commandDir">各種コマンド用のCSV格納フォルダ</param>
		/// <param name="outputDir">出力先</param>
		/// <param name="isDumpConsole">コンソールに作業内容を出力するかどうか</param>
		public static int Convert(string argTypeFile, string commandDir, string outputDir, bool isDumpConsole)
		{
			var ret = 0;

			var commandData = new ScenarioCommandData();
			commandData.IsDumpConsole = isDumpConsole;

			ret = commandData.ReadArgTypeCsv(argTypeFile);
			if (ret != 0)
			{
				return ret;
			}

			ret = commandData.ReadCommandCsv(commandDir);
			if (ret != 0)
			{
				return ret;
			}

			GenerateHtmlDetail(commandData, outputDir);
			GenerateHtmlMenu(commandData, outputDir);

			return 0;
		}

		/// <summary>
		/// detail.html吐き出し
		/// </summary>
		/// <param name="commandData">コマンド情報</param>
		/// <param name="outputDir">出力先</param>
		private static void GenerateHtmlDetail(ScenarioCommandData commandData, string outputDir)
		{
			var commandInfoDict = commandData.CommandInfoDict;

			var sb = new StringBuilder();

			sb.AppendLine("<html>");

			sb.AppendLine("<script>");
			sb.AppendLine("function copyToClipboard(value) {");
			sb.AppendLine("var tempInput = document.createElement('input');");
			sb.AppendLine("tempInput.style = 'position: absolute; left: -1000px; top: -1000px';");
			sb.AppendLine("tempInput.value = value;");
			sb.AppendLine("document.body.appendChild(tempInput);");
			sb.AppendLine("tempInput.select();");
			sb.AppendLine("document.execCommand('copy');");
			sb.AppendLine("document.body.removeChild(tempInput);");
			sb.AppendLine("}");
			sb.AppendLine("</script>");

			sb.AppendLine("<body>");

			foreach (var commandInfo in commandInfoDict)
			{
				var copyValue = commandInfo.Key;
				if (commandInfo.Value.ArgDict.Count > 0)
				{
					copyValue += "\t";
				}

				var index = 0;
				foreach (var arg in commandInfo.Value.ArgDict)
				{
					copyValue += arg.Value.Desc;

					if (index < commandInfo.Value.ArgDict.Count - 1)
					{
						copyValue += "\t";
					}

					index++;
				}

				sb.Append("<h1 id=\"" + commandInfo.Value.TypeName + "\">" + commandInfo.Value.Name + "　");
				sb.AppendLine("<button onclick=\"copyToClipboard('" + copyValue + "')\">Excel形式でクリップボードにコピー</button></h1>");
				sb.AppendLine("<h2>説明: " + commandInfo.Value.Desc + "</h2>");

				var count = commandInfo.Value.ArgDict.Count;
				if (count == 0)
				{
					sb.AppendLine("<h3>引数なし</h3>");
				}
				else
				{
					int argNumber = 1;
					foreach (var arg in commandInfo.Value.ArgDict)
					{
						sb.Append("<h3>");
						sb.Append("引数" + argNumber + ": ");

						sb.Append("<br>　　　説明: " + arg.Value.Desc);
						sb.Append("<br>　　　　型: " + arg.Value.Type);

						if (arg.Value.Min != null || arg.Value.Max != null)
						{
							sb.Append("<br>　　　");

							if (arg.Value.Min != null)
							{
								sb.Append("範囲: 最小=" + arg.Value.Min + "　");
							}

							if (arg.Value.Max != null)
							{
								sb.Append("範囲: 最大=" + arg.Value.Max);
							}
						}

						if (!arg.Value.IsReplaceVariable)
						{
							sb.Append("<br>　変数置換: 不可");
						}

						if (arg.Value.IsOptional)
						{
							sb.Append("<br>　　　省略: 可");

							if (!string.IsNullOrEmpty(arg.Value.OptionalDefault))
							{
								sb.Append("　デフォルト値: " + arg.Value.OptionalDefault);
							}
						}

						argNumber++;
					}
				}

				sb.AppendLine("<hr>");
			}

			sb.AppendLine("</body>");
			sb.AppendLine("</html>");

			FileUtility.WriteToFile(outputDir + "detail.html", sb.ToString());
		}

		/// <summary>
		/// menu.html吐き出し
		/// </summary>
		/// <param name="commandData">コマンド情報</param>
		/// <param name="outputDir">出力先</param>
		private static void GenerateHtmlMenu(ScenarioCommandData commandData, string outputDir)
		{
			var commandInfoDict = commandData.CommandInfoDict;

			var categoryCommandDict = new SortedDictionary<string, List<ScenarioCommandData.CommandInfo>>();
			foreach (var commandInfo in commandInfoDict)
			{
				List<ScenarioCommandData.CommandInfo> commandList = null;
				if (categoryCommandDict.TryGetValue(commandInfo.Value.Category, out commandList))
				{
					commandList.Add(commandInfo.Value);
				}
				else
				{
					commandList = new List<ScenarioCommandData.CommandInfo>();
					commandList.Add(commandInfo.Value);

					categoryCommandDict.Add(commandInfo.Value.Category, commandList);
				}
			}

			var sb = new StringBuilder();

			sb.AppendLine("<html>");
			sb.AppendLine("<body>");
			sb.AppendLine("<h3>コマンド一覧</h3>");

			sb.AppendLine("<ul>");

			foreach (var commandList in categoryCommandDict)
			{
				sb.AppendLine("<li>" + commandList.Key);
				sb.AppendLine("<ul>");

				foreach (var commandInfo in commandList.Value)
				{
					sb.AppendLine("<li>");
					sb.AppendLine("  <a href=\"detail.html#" + commandInfo.TypeName + "\" target = \"detail\">" + commandInfo.Name + "</a>");
					sb.AppendLine("</li>");
				}

				sb.AppendLine("</ul>");
				sb.AppendLine("</li>");
			}

			sb.AppendLine("</ul>");

			sb.AppendLine("</body>");
			sb.AppendLine("</html>");

			FileUtility.WriteToFile(outputDir + "menu.html", sb.ToString());
		}
	}
}
