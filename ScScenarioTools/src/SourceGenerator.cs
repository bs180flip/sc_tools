using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScScenarioCommon;

namespace ScScenarioTools
{
	public class SourceGenerator
	{
		/// <summary>
		/// ソースコード生成
		/// </summary>
		/// <param name="argTypeFile">ArgType用の引数情報CSVファイル</param>
		/// <param name="commandDir">各種コマンド用のCSV格納フォルダ</param>
		/// <param name="outputDir">出力先</param>
		/// <param name="isDumpConsole">コンソールに作業内容を出力するかどうか</param>
		public static int Generate(string argTypeFile, string commandDir, string outputDir, bool isDumpConsole)
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

			GenerateArgTypeSource(commandData, outputDir);
			GenerateCommandTypeSource(commandData, outputDir);
			GenerateCommandInfoDictSource(commandData, outputDir);

			return 0;
		}

		/// <summary>
		/// ArgType.csの生成
		/// </summary>
		/// <param name="commandData">コマンド情報</param>
		/// <param name="outputDir">出力先</param>
		private static void GenerateArgTypeSource(ScenarioCommandData commandData, string outputDir)
		{
			var argTypeDict = commandData.ArgTypeDict;

			var sb = new SourceBuilder();

			sb.Append("namespace Sc.Scenario");
			sb.Append("{");
			sb.Append("/// <summary>");
			sb.Append("/// 引数タイプ");
			sb.Append("/// </summary>");
			sb.Append("public enum ArgType");
			sb.Append("{");

			foreach (var argType in argTypeDict)
			{
				sb.Append("/// <summary>" + argType.Value.Desc + "</summary>");
				sb.Append("" + argType.Key + ",");
				sb.Append();
			}

			sb.Append("}");
			sb.Append("}");

			FileUtility.WriteToFile(outputDir + "ArgType.cs", sb.ToString());
		}

		/// <summary>
		/// CommandType.csの生成
		/// </summary>
		/// <param name="commandData">コマンド情報</param>
		/// <param name="outputDir">出力先</param>
		private static void GenerateCommandTypeSource(ScenarioCommandData commandData, string outputDir)
		{
			var commandInfoDict = commandData.CommandInfoDict;

			var sb = new SourceBuilder();

			sb.Append("namespace Sc.Scenario");
			sb.Append("{");
			sb.Append("/// <summary>");
			sb.Append("/// コマンドタイプ");
			sb.Append("/// </summary>");
			sb.Append("public enum CommandType : uint");
			sb.Append("{");

			sb.Append("/// <summary>無効</summary>");
			sb.Append("None = 0,");
			sb.Append();

			foreach (var commandInfo in commandInfoDict)
			{
				sb.Append("/// <summary>" + commandInfo.Value.Name + "</summary>");
				sb.Append(commandInfo.Value.TypeName + " = " + commandInfo.Value.Id.ToString() + ",");
				sb.Append();
			}

			sb.Append("}");
			sb.Append("}");

			FileUtility.WriteToFile(outputDir + "CommandType.cs", sb.ToString());
		}

		/// <summary>
		/// CommandInfoDict.csの生成
		/// </summary>
		/// <param name="commandData">コマンド情報</param>
		/// <param name="outputDir">出力先</param>
		private static void GenerateCommandInfoDictSource(ScenarioCommandData commandData, string outputDir)
		{
			var commandInfoDict = commandData.CommandInfoDict;

			var sb = new SourceBuilder();

			sb.Append("using System.Collections.Generic;");
			sb.Append();

			sb.Append("namespace Sc.Scenario");
			sb.Append("{");
			sb.Append("/// <summary>");
			sb.Append("/// コマンド情報辞書");
			sb.Append("/// </summary>");
			sb.Append("public class CommandInfoDict");
			sb.Append("{");

			sb.Append("/// <summary>コマンド情報リスト</summary>");
			sb.Append("private Dictionary<CommandType, CommandInfo> _commandInfoDict = new Dictionary<CommandType, CommandInfo>()");
			sb.Append("{");

			foreach (var commandInfo in commandInfoDict)
			{
				int totalArgCount = commandInfo.Value.ArgDict.Count;

				sb.Append("{");
				sb.Append("CommandType." + commandInfo.Value.TypeName + ",");
				sb.Append("new CommandInfo(");

				if (totalArgCount == 0)
				{
					sb.Append("\tCommandType." + commandInfo.Value.TypeName + ")");
				}
				else
				{
					sb.Append("\tCommandType." + commandInfo.Value.TypeName + ",");
				}

				int argCount = 0;
				foreach (var arg in commandInfo.Value.ArgDict)
				{
					string code = "\tnew ArgInfo(ArgType." + arg.Value.Type;

					if (!arg.Value.IsReplaceVariable)
					{
						code += ", isReplaceVariable: false";
					}

					argCount++;
					if (argCount == totalArgCount)
					{
						code += "))";
					}
					else
					{
						code += "),";
					}

					sb.Append(code);
				}

				sb.Append("},");
			}

			sb.Append("};");

			sb.Append();
			sb.Append("/// <summary>");
			sb.Append("/// インデクサー");
			sb.Append("/// </summary>");
			sb.Append("/// <param name=\"command\">コマンド</param>");
			sb.Append("/// <returns>コマンド情報</returns>");
			sb.Append("public CommandInfo this[CommandType command]");
			sb.Append("{");
			sb.Append("get");
			sb.Append("{");
			sb.Append("CommandInfo info = null;");
			sb.Append("if (_commandInfoDict.TryGetValue(command, out info))");
			sb.Append("{");
			sb.Append("return info;");
			sb.Append("}");
			sb.Append();
			sb.Append("return null;");
			sb.Append("}");
			sb.Append("}");

			sb.Append("}");
			sb.Append("}");

			FileUtility.WriteToFile(outputDir + "CommandInfoDict.cs", sb.ToString());
		}
	}
}
