using System;
using System.Diagnostics;

namespace ScMstSqlGenerator
{

	public class ProcessExecute
	{
		public static string JsonCopy = @"..\tools\ScMstSqlGenerator\Copy_Json.bat ";
		
		public static string ConvHousingEventInfo = @"..\tools\ScExcelToJsonCell\Conv_HousingEventInfo.bat ";

		public static void ExternalBatExecute(string cmdName)
		{
			var cmd = SetCmd(cmdName);

			// プロセス起動情報の構築
			ProcessStartInfo startInfo = new ProcessStartInfo();

			// バッチファイルを起動
			startInfo.FileName = "cmd.exe";

			// コマンド処理実行後、コマンドウィンドウ終了
			startInfo.Arguments = "/c ";

			// コマンド処理であるバッチファイル
			startInfo.Arguments += cmd;

			// バッチファイルへの引数 
			startInfo.Arguments += "";

			// バッチファイルを別プロセスとして起動
			var proc = Process.Start(startInfo);

			// 上記バッチ処理が終了まで待機
			proc.WaitForExit();
		}

		private static string SetCmd(string cmdName)
		{
			switch (cmdName)
			{
			case Const.ExcelToJson:
				return ConvHousingEventInfo;
			case Const.JsonCopy:
				return JsonCopy;
			default:
				break;
			}

			return null;
		}
	}

}