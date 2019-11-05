using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using ScScenarioCommon;
using ExcelReader;

namespace ScScenarioTools
{
	public class ScenarioConverter
	{
		/// <summary>
		/// セル情報クラス
		/// エラー時にセルの位置表示に使用
		/// マクロ機能でシートが結合されて本来のシート位置がわからなくなるため用意
		/// </summary>
		private class CellInfo
		{
			/// <summary>縦方向のカウント</summary>
			public int Row = 0;

			/// <summary>マクロが使用されているかどうか</summary>
			public bool IsInMacro = false;

			/// <summary>マクロ側の縦方向のカウント</summary>
			public int MacroRow = 0;

			/// <summary>使用されているマクロ名</summary>
			public string MacroName = "";
		}

		/// <summary>
		/// エラー情報クラス
		/// </summary>
		private class ErrorInfo
		{
			/// <summary>マクロ内で発生したかどうか</summary>
			public bool IsInMacro = false;

			/// <summary>対象ブックパス</summary>
			public string Path = "";

			/// <summary>対象ブック名</summary>
			public string BookName = "";

			/// <summary>対象シート名</summary>
			public string SheetName = "";

			/// <summary>セル番地</summary>
			public string CellPosition = "";

			/// <summary>メッセージ</summary>
			public string Message = "";
		}

		/// <summary>マクロのキーワード</summary>
		private const string MacroKeyword = "マクロ_";

		/// <summary>引数名のフォーマット</summary>
		private const string ArgNameFormat = "arg{0}";

		/// <summary>
		/// ブック内のエクセルのセル情報格納
		/// マクロと結合すると元のシート内の番地がわからなくなるので保存
		/// key=ブック名/シート名
		/// </summary>
		private Dictionary<string, List<CellInfo>> _cellInfoListDict = null;

		/// <summary>エラー一覧</summary>
		private List<ErrorInfo> _errorInfoList = null;

		/// <summary>
		/// 宣言した変数名格納
		/// 実際のゲーム側ではコマンドごとに変数の入れものが違っていますが
		/// 同スクリプトに同じ変数名の違う機能のものがあると混乱のもとなのでエラーとします
		/// </summary>
		private HashSet<string> _variableList = null;

		/// <summary>csvから読み込んだコマンドデータ</summary>
		private ScenarioCommandData _command = null;

		/// <summary>ゲーム全体の定義</summary>
		private GlobalDefineData _globalDefine = null;

		/// <summary>スクリプト側定義</summary>
		private LocalDefineData _localDefine = null;

		/// <summary>マクロ情報</summary>
		private MacroData _macro = null;

		/// <summary>処理中のブック名</summary>
		private string _currentBookPath = "";

		/// <summary>
		/// コンバート処理
		/// xlsxからbytesファイルに変換する
		/// bookFileとbookDirが両方指定された場合はbookFileが優先される
		/// </summary>
		/// <param name="bookFile">単体Excelブックファイル</param>
		/// <param name="bookDir">Excelブックファイル格納フォルダ</param>
		/// <param name="argTypeFile">引数情報csv</param>
		/// <param name="commandDir">コマンドcsv格納フォルダ</param>
		/// <param name="globalDefineFile">ゲーム全体定義ファイル</param>
		/// <param name="localDefineDir">スクリプト専用定義ファイル格納フォルダ</param>
		/// <param name="macroDir">マクロファイル格納フォルダ</param>
		/// <param name="outputCsvDir">csvファイル出力先</param>
		/// <param name="outputBinDir">bytesファイル出力先</param>
		/// <param name="outputErrorXlsxDir">エラー用エクセルブックの出力先</param>
		/// <param name="isEnableErrorXlsx">エラー用エクセルブックを出力するかどうか</param>
		/// <param name="isDumpConsole">コンソールに処理中の内容を表示するかどうか</param>
		public int Convert(string bookFile,
						   string bookDir,
						   string argTypeFile,
						   string commandDir,
						   string globalDefineFile,
						   string localDefineDir,
						   string macroDir,
						   string outputCsvDir,
						   string outputBinDir,
						   string outputErrorXlsxDir,
						   bool isEnableErrorXlsx,
						   bool isDumpConsole)
		{
			var ret = 0;

			_cellInfoListDict = new Dictionary<string, List<CellInfo>>();
			_errorInfoList = new List<ErrorInfo>();
			_variableList = new HashSet<string>();

			_command = new ScenarioCommandData();
			_globalDefine = new GlobalDefineData();
			_localDefine = new LocalDefineData();
			_macro = new MacroData();

			_command.IsDumpConsole = isDumpConsole;
			_globalDefine.IsDumpConsole = isDumpConsole;
			_localDefine.IsDumpConsole = isDumpConsole;
			_macro.IsDumpConsole = isDumpConsole;

			ret = _command.ReadArgTypeCsv(argTypeFile);
			if (ret != 0)
			{
				return ret;
			}

			ret = _command.ReadCommandCsv(commandDir);
			if (ret != 0)
			{
				return ret;
			}

			if (string.IsNullOrEmpty(globalDefineFile) == false)
			{
				ret = _globalDefine.Read(globalDefineFile);
				if (ret != 0)
				{
					return ret;
				}
			}

			if (string.IsNullOrEmpty(localDefineDir) == false)
			{
				ret = _localDefine.Read(localDefineDir);
				if (ret != 0)
				{
					return ret;
				}
			}

			if (string.IsNullOrEmpty(macroDir) == false)
			{
				ret = _macro.Read(macroDir);
				if (ret != 0)
				{
					return ret;
				}
			}

			if (isDumpConsole)
			{
				System.Console.WriteLine("[Convert]");
			}

			if (string.IsNullOrEmpty(bookFile) == false)
			{
				ret = ConvertToCsvAndBin(bookFile, outputCsvDir, outputBinDir, isDumpConsole);
				if (ret != 0)
				{
					ret = 1;
				}
			}
			else
			{
				int successCount = 0;

				var files = FileUtility.SearchFiles(bookDir, "*.xlsx", isSearchingSubDirectories: true);
				foreach (var file in files)
				{
					if (ConvertToCsvAndBin(file, outputCsvDir, outputBinDir, isDumpConsole) == 0)
					{
						successCount++;
					}
				}

				if (successCount < files.Count)
				{
					ret = 1;
				}
			}

			if (isEnableErrorXlsx && _errorInfoList.Count > 0)
			{
				WriteErrorXlsx(outputErrorXlsxDir);
			}

			return ret;
		}

		/// <summary>
		/// ブックからcsv、binファイルにコンバート
		/// </summary>
		/// <param name="path">xlsxファイルパス</param>
		/// <param name="outputCsvDir">csvファイル出力先</param>
		/// <param name="outputBinDir">binファイル出力先</param>
		/// <param name="isDumpConsole">処理中の内容をコンソールに出力するかどうか</param>
		private int ConvertToCsvAndBin(string path, string outputCsvDir, string outputBinDir, bool isDumpConsole)
		{
			int ret = 0;

			try
			{
				var book = new XlsBook(path);

				// ブック名のディレクトリを作成しその中に生成物を出力する
				outputCsvDir = outputCsvDir + book.BookName + "/";
				if (!Directory.Exists(outputCsvDir))
				{
					Directory.CreateDirectory(outputCsvDir);
				}

				_currentBookPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)) + "/";

				foreach (var sheet in book.Sheets)
				{
					if (CreateCellInfoList(book, sheet) != 0)
					{
						ret = 1;
						continue;
					}

					if (ConvertMacro(book, sheet) != 0)
					{
						ret = 1;
						continue;
					}

					if (isDumpConsole)
					{
						System.Console.WriteLine("  " + book.BookName + ".xlsx > " + sheet.SheetName);
					}

					if (ExportCsv(sheet, outputCsvDir) != 0)
					{
						ret = 1;
						continue;
					}

					if (ConvertToBin(outputCsvDir + sheet.SheetName + ".csv", outputBinDir, isDumpConsole) != 0)
					{
						ret = 1;
						continue;
					}
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
				System.Console.WriteLine(e.StackTrace);
				return 1;
			}

			return ret;
		}

		/// <summary>
		/// セル情報を作成する
		/// </summary>
		/// <param name="book">ブック</param>
		/// <param name="sheet">シート</param>
		private int CreateCellInfoList(XlsBook book, XlsSheet sheet)
		{
			var cellInfoList = new List<CellInfo>();

			for (int i = 0; i < sheet.Fields.Count; i++)
			{
				var cellInfo = new CellInfo();
				cellInfo.Row = i + 1;

				cellInfoList.Add(cellInfo);
			}

			_cellInfoListDict.Add(book.BookName + "/" + sheet.SheetName, cellInfoList);

			return 0;
		}

		/// <summary>
		/// sheet内のマクロをコンバート
		/// </summary>
		/// <param name="book">ブック</param>
		/// <param name="sheet">シート</param>
		private int ConvertMacro(XlsBook book, XlsSheet sheet)
		{
			List<CellInfo> cellInfoList = null;
			if (_cellInfoListDict.TryGetValue(book.BookName + "/" + sheet.SheetName, out cellInfoList) == false)
			{
				System.Console.WriteLine("    セル情報が見つかりません name=" + sheet.SheetName);
				return 1;
			}

			for (int i = 0; i < sheet.Fields.Count; i++)
			{
				var row = sheet.Fields[i];
				if (row.Count < 1) { continue; } // コメントや空行は飛ばす

				var args = new List<string>();

				if (row[0].IndexOf(MacroKeyword) == 0)
				{
					var macroName = row[0].Replace(MacroKeyword, "");
					for (int j = 1; j < row.Count; j++)
					{
						args.Add(row[j]);
					}

					var macroFields = GetMacroFields(macroName, args);
					if (macroFields == null)
					{
						System.Console.WriteLine("    マクロが見つかりません name=" + macroName);
						return 1;
					}

					// "マクロ"と出たフィールドを消す
					sheet.Fields.RemoveAt(i);
					cellInfoList.RemoveAt(i);

					// 消したところから対応したマクロ辞書のシートをその行数分追加する処理
					sheet.Fields.InsertRange(i, macroFields);

					int macroLineCount = 0;
					foreach (var macro in macroFields)
					{
						var cellInfo = new CellInfo();
						cellInfo.Row = i + 1;
						cellInfo.IsInMacro = true;
						cellInfo.MacroName = macroName;
						cellInfo.MacroRow = macroLineCount + 1 + 1; // 見出し分があるのでさらに+1

						cellInfoList.Insert(i + macroLineCount, cellInfo);

						macroLineCount++;
					}

					i--;
				}
			}

			return 0;
		}

		/// <summary>
		/// マクロ名を指定して引数を置き換えたマクロ情報を取得
		/// </summary>
		/// <param name="macroName">マクロ名</param>
		/// <param name="args">引数</param>
		/// <returns></returns>
		private List<List<string>> GetMacroFields(string macroName, List<string> args)
		{
			var macroValue = _macro[macroName];
			if (macroValue == null)
			{
				return null;
			}

			var macroFields = new List<List<string>>();

			foreach (var row in macroValue)
			{
				var macroRow = new List<string>();
				foreach (var cell in row)
				{
					macroRow.Add(cell);
				}
				macroFields.Add(macroRow);
			}

			for (int k = 0, kMax = macroFields.Count; k < kMax; k++)
			{
				var macroRow = macroFields[k];
				for (int m = 0, mMax = macroRow.Count; m < mMax; m++)
				{
					var cellString = macroRow[m];

					for (int n = 0; n < args.Count; n++)
					{
						var argName = string.Format(ArgNameFormat, n + 1);
						if (cellString == argName)
						{
							cellString = args[n];
						}
					}
					macroRow[m] = cellString;
				}
				macroFields[k] = macroRow;
			}

			return macroFields;
		}

		/// <summary>
		/// CSV出力
		/// </summary>
		/// <param name="sheet">シート</param>
		/// <param name="outputDir">出力先</param>
		private int ExportCsv(XlsSheet sheet, string outputDir)
		{
			var sb = new StringBuilder();

			string value = "";

			for (int i = 0, iMax = sheet.Fields.Count; i < iMax; i++)
			{
				var row = sheet.Fields[i];

				if (row.Count == 0)
				{
					sb.AppendLine();
					continue;
				}

				if (row[0] == "END")
				{
					// スクリプト終了
					break;
				}
				else if (row[0].IndexOf("//") == 0)
				{
					// コメント
					sb.AppendLine();
					continue;
				}

				for (int j = 0, jMax = row.Count; j < jMax; j++)
				{
					if (!string.IsNullOrEmpty(row[j]) && row[j][0] == '#')
					{
						// 定義データはローカルのものが優先
						if (_localDefine.DefineDict.TryGetValue(row[j], out value))
						{
							row[j] = value;
						}
						else if (_globalDefine.DefineDict.TryGetValue(row[j], out value))
						{
							row[j] = value;
						}
					}
					if (row[j].Contains("\n"))
					{
						// セルのテキストを""で囲む
						row[j] = "\"" + row[j] + "\"";
					}

					sb.Append(row[j] + ",");
				}

				sb.AppendLine();
			}

			File.WriteAllText(outputDir + sheet.SheetName + ".csv", sb.ToString(), Encoding.UTF8);

			return 0;
		}

		/// <summary>
		/// csvをバイナリファイルにコンバートして出力
		/// </summary>
		/// <param name="path">csvファイルパス</param>
		/// <param name="outputDir">出力先</param>
		/// <param name="isDumpConsole">処理中の内容をコンソールに出力するかどうか</param>
		private int ConvertToBin(string path, string outputDir, bool isDumpConsole)
		{
			int ret = 0;

			try
			{
				_variableList.Clear();

				var info = Directory.GetParent(path);

				var bookName = info.Name;
				var sheetName = Path.GetFileNameWithoutExtension(path);

				// ブック名のディレクトリを作成しその中に生成物を出力する
				outputDir = outputDir + bookName + "/";
				if (!Directory.Exists(outputDir))
				{
					Directory.CreateDirectory(outputDir);
				}

				var binFilePath = outputDir + Path.GetFileNameWithoutExtension(path) + ".bytes";
				using (var fileStream = new FileStream(binFilePath, FileMode.Create))
				{
					var binaryWriter = new BinaryWriter(fileStream);

					var csv = FileUtility.ReadCsv(path);
					for (int row = 0; row < csv.Count; row++)
					{
						// 見出しはスルー
						if (row == 0) { continue; }

						var line = csv[row];
						if (line.Count < 2) { continue; }

						var commandName = line[0];
						var argList = line.GetRange(1, line.Count - 1);

						ScenarioCommandData.CommandInfo commandInfo = null;
						if (_command.CommandInfoDict.TryGetValue(commandName, out commandInfo))
						{
							if (ParseCommand(binaryWriter, commandInfo, argList, bookName, sheetName, row) != 0)
							{
								ret = 1;
							}
						}
						else
						{
							DumpError(bookName, sheetName, row, 0, "定義されてないコマンド名です command=" + commandName);
							ret = 1;
						}
					}

					binaryWriter.Close();
					fileStream.Close();
				}

				if (ret != 0)
				{
					File.Delete(binFilePath);
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
				System.Console.WriteLine(e.StackTrace);
				return 1;
			}

			return ret;
		}

		/// <summary>
		/// コマンド解析
		/// ループ毎に行う部分をメソッド化
		/// </summary>
		/// <param name="binaryWriter">バイナリ書き出し用</param>
		/// <param name="commandInfo">コマンド情報</param>
		/// <param name="argList">引数情報</param>
		/// <param name="bookName">ブック名(エラー表示用)</param>
		/// <param name="sheetName">シート名(エラー表示用)</param>
		/// <param name="row">現在の縦方向のカウント(エラー表示用)</param>
		private int ParseCommand(BinaryWriter binaryWriter,
								 ScenarioCommandData.CommandInfo commandInfo,
								 List<string> argList,
								 string bookName,
								 string sheetName,
								 int row)
		{
			int currentErrorCount = _errorInfoList.Count;

			var argsByteList = new List<byte>();

			// 引数の超過チェック
			for (var column = commandInfo.ArgDict.Count; column < argList.Count; column++)
			{
				if (argList[column] != "")
				{
					DumpError(bookName, sheetName, row, column + 1, "引数オーバー");
				}
			}

			var argCount = 0;
			foreach (var argInfo in commandInfo.ArgDict.Values)
			{
				var argInfoType = argInfo.Type;
				var argString = argList[argCount];

				// #で始まる値がある場合は定数展開できてないのでエラーにする
				if (argString.IndexOf("#") == 0)
				{
					DumpError(bookName, sheetName, row, argCount + 1, "定数が見つかりません define=" + argString);
					continue;
				}

				if (argInfo.IsVariableOn && !string.IsNullOrEmpty(argString))
				{
					// 変数登録
					if (_variableList.Contains(argString))
					{
						DumpError(bookName, sheetName, row, argCount + 1, "変数名が重複しています 変数名=" + argString);
					}
					else
					{
						_variableList.Add(argString);
					}
				}
				else if (argInfo.IsVariableOff)
				{
					// 変数削除
					if (_variableList.Contains(argString))
					{
						_variableList.Remove(argString);
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "変数名が定義されていません 変数名=" + argString);
					}
				}
				else if (argInfo.IsVariableOnly)
				{
					// 変数のみ許可引数
					if (!_variableList.Contains(argString))
					{
						DumpError(bookName, sheetName, row, argCount + 1, "変数や定義が見つかりません value=" + argString);
					}
				}

				// 変数定義されている場合は文字列型に置き換え
				if (_variableList.Contains(argString))
				{
					argInfoType = "String";
				}

				ScenarioCommandData.ArgType argType = null;
				if (_command.ArgTypeDict.TryGetValue(argInfoType, out argType) == false)
				{
					DumpError(bookName, sheetName, row, argCount + 1, "引数タイプが見つかりません type=" + argInfoType);
					continue;
				}

				// 型情報を追加
				argsByteList.Add(argType.TypeValue);

				if (argInfoType.Equals("Bool"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "FALSE";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					bool value = false;
					if (bool.TryParse(argString, out value))
					{
						var bytes = BitConverter.GetBytes(value);
						argsByteList.AddRange(bytes);
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "boolへの変換失敗！ TRUEかFALSEを使用して下さい");
					}
				}
				else if (argInfoType.Equals("Byte"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "0";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					byte value = 0;
					if (byte.TryParse(argString, out value))
					{
						var min = argInfo.Min;
						var max = argInfo.Max;
						if (!min.HasValue || !max.HasValue || (min <= value && value <= max))
						{
							argsByteList.Add(value);
						}
						else
						{
							DumpError(bookName, sheetName, row, argCount + 1, "範囲外！ " + min + " ~ " + max + " の範囲を指定して下さい");
						}
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "byteへの変換失敗 data=" + argString);
					}
				}
				else if (argInfoType.Equals("Int"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "0";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					int value = 0;
					if (int.TryParse(argString, out value))
					{
						var min = argInfo.Min;
						var max = argInfo.Max;
						if (!min.HasValue || !max.HasValue || (min <= value && value <= max))
						{
							var bytes = BitConverter.GetBytes(value);
							argsByteList.AddRange(bytes);
						}
						else
						{
							DumpError(bookName, sheetName, row, argCount + 1, "範囲外！ " + min + " ~ " + max + " の範囲を指定して下さい");
						}
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "Intへの変換失敗 data=" + argString);
					}
				}
				else if (argInfoType.Equals("Long"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "0";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					long value = 0;
					if (long.TryParse(argString, out value))
					{
						var min = argInfo.Min;
						var max = argInfo.Max;
						if (!min.HasValue || !max.HasValue || (min <= value && value <= max))
						{
							var bytes = BitConverter.GetBytes(value);
							argsByteList.AddRange(bytes);
						}
						else
						{
							DumpError(bookName, sheetName, row, argCount + 1, "範囲外！ " + min + " ~ " + max + " の範囲を指定して下さい");
						}
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "Longへの変換失敗 data=" + argString);
					}
				}
				else if (argInfoType.Equals("Float"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "0";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					float value = 0f;
					if (float.TryParse(argString, out value))
					{
						var min = argInfo.Min;
						var max = argInfo.Max;
						if (!min.HasValue || !max.HasValue || (min <= value && value <= max))
						{
							var bytes = BitConverter.GetBytes(value);
							argsByteList.AddRange(bytes);
						}
						else
						{
							DumpError(bookName, sheetName, row, argCount + 1, "範囲外！ " + min + " ~ " + max + " の範囲を指定して下さい");
						}
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "Floatへの変換失敗 dat=" + argString);
					}
				}
				else if (argInfoType.Equals("String"))
				{
					if (string.IsNullOrEmpty(argString))
					{
						if (argInfo.IsOptional == false)
						{
							DumpError(bookName, sheetName, row, argCount + 1, "文字列データが空になっています");
							continue;
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					var value = argString.Replace("\"", "").Replace("\n", "\r\n");
					var bytes = Encoding.UTF8.GetBytes(value);
					var length = BitConverter.GetBytes((short)bytes.Length);
					argsByteList.AddRange(length);
					argsByteList.AddRange(bytes);
				}
				else if (argInfoType.Equals("Vector2"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "0|0";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					var values = argString.Split('|');
					if (values == null || values.Length != 2)
					{
						DumpError(bookName, sheetName, row, argCount + 1, "要素数が不一致！ n|nの形で指定して下さい");
						continue;
					}

					float xValue = 0;
					float yValue = 0;
					if ((float.TryParse(values[0], out xValue)) &&
						(float.TryParse(values[1], out yValue)))
					{
						var xBytes = BitConverter.GetBytes(xValue);
						argsByteList.AddRange(xBytes);

						var yBytes = BitConverter.GetBytes(yValue);
						argsByteList.AddRange(yBytes);
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1,"Vector2への変換失敗 data=" + argString);
					}
				}
				else if (argInfoType.Equals("Vector3"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "0|0|0";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					var values = argString.Split('|');
					if (values == null || values.Length != 3)
					{
						DumpError(bookName, sheetName, row, argCount + 1, "要素数が不一致！ n|n|nの形で指定して下さい");
						continue;
					}

					float xValue = 0;
					float yValue = 0;
					float zValue = 0;
					if ((float.TryParse(values[0], out xValue)) &&
						(float.TryParse(values[1], out yValue)) &&
						(float.TryParse(values[2], out zValue)))
					{
						var xBytes = BitConverter.GetBytes(xValue);
						argsByteList.AddRange(xBytes);

						var yBytes = BitConverter.GetBytes(yValue);
						argsByteList.AddRange(yBytes);

						var zBytes = BitConverter.GetBytes(zValue);
						argsByteList.AddRange(zBytes);
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "Vector3への変換失敗 data=" + argString);
					}
				}
				else if (argInfoType.Equals("Rect"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "0|0|0|0";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					var values = argString.Split('|');
					if (values == null || values.Length != 4)
					{
						DumpError(bookName, sheetName, row, argCount + 1, "要素数が不一致！ n|n|n|nの形で指定して下さい");
						continue;
					}

					float xValue = 0;
					float yValue = 0;
					float wValue = 0;
					float hValue = 0;
					if ((float.TryParse(values[0], out xValue)) &&
						(float.TryParse(values[1], out yValue)) &&
						(float.TryParse(values[2], out wValue)) &&
						(float.TryParse(values[3], out hValue)))
					{
						var xBytes = BitConverter.GetBytes(xValue);
						argsByteList.AddRange(xBytes);

						var yBytes = BitConverter.GetBytes(yValue);
						argsByteList.AddRange(yBytes);

						var wBytes = BitConverter.GetBytes(wValue);
						argsByteList.AddRange(wBytes);

						var hBytes = BitConverter.GetBytes(hValue);
						argsByteList.AddRange(hBytes);
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "Rectへの変換失敗 data=" + argString);
					}
				}
				else if (argInfoType.Equals("Color"))
				{
					if (argInfo.IsOptional && string.IsNullOrEmpty(argString))
					{
						if (string.IsNullOrEmpty(argInfo.OptionalDefault))
						{
							argString = "0|0|0";
						}
						else
						{
							argString = argInfo.OptionalDefault;
						}
					}

					var values = argString.Split('|');
					if (values == null || values.Length != 3)
					{
						DumpError(bookName, sheetName, row, argCount + 1, "要素数が不一致！ n|n|nの形で指定して下さい");
						continue;
					}

					byte rValue = 0;
					byte gValue = 0;
					byte bValue = 0;
					if ((byte.TryParse(values[0], out rValue)) &&
						(byte.TryParse(values[1], out gValue)) &&
						(byte.TryParse(values[2], out bValue)))
					{
						argsByteList.Add(rValue);
						argsByteList.Add(gValue);
						argsByteList.Add(bValue);
					}
					else
					{
						DumpError(bookName, sheetName, row, argCount + 1, "Colorへの変換失敗 data=" + argString);
					}
				}
				else
				{
					DumpError(bookName, sheetName, row, argCount + 1, "定義されてない引数タイプです type=" + argType.Type);
				}

				argCount++;
			}

			if (currentErrorCount == _errorInfoList.Count)
			{
				// コマンドタイプを書き込む
				binaryWriter.Write((uint)commandInfo.Id);

				// 引数長を書き込む
				binaryWriter.Write((short)argsByteList.Count);

				// 引数情報を書き込む
				binaryWriter.Write(argsByteList.ToArray());

				return 0;
			}

			return 1;
		}

		/// <summary>
		/// エラー出力
		/// 引数の内容からExcelのどの番地かも出力する
		/// </summary>
		/// <param name="bookName">ブック名</param>
		/// <param name="sheetName">シート名</param>
		/// <param name="row">縦方向のカウント</param>
		/// <param name="column">横方向のカウント</param>
		/// <param name="message">表示メッセージ</param>
		private void DumpError(string bookName, string sheetName, int row, int column, string message)
		{
			List<CellInfo> cellInfoList = null;
			if (_cellInfoListDict.TryGetValue(bookName + "/" + sheetName, out cellInfoList) == false)
			{
				System.Console.WriteLine("    " + message);
				return;
			}

			var cellInfo = cellInfoList[row];

			string commonMessage = "    ";
			string strCellPosition = "";

			if (cellInfo.IsInMacro)
			{
				// マクロの場合は呼び出し元をA列に固定する
				commonMessage += "A" + cellInfo.Row;
				commonMessage += " マクロ=" + cellInfo.MacroName + " ";
				strCellPosition += (char)('A' + column);
				strCellPosition += cellInfo.MacroRow;
			}
			else
			{
				strCellPosition += (char)('A' + column);
				strCellPosition += cellInfo.Row;
			}

			commonMessage += strCellPosition;

			commonMessage += ": ";

			System.Console.WriteLine(commonMessage + message);

			// エラー一覧に追加
			{
				var errorInfo = new ErrorInfo();
				errorInfo.CellPosition = strCellPosition;
				errorInfo.Message = message;

				if (cellInfo.IsInMacro)
				{
					errorInfo.IsInMacro = true;
					errorInfo.Path = _macro.FindMacroPath(cellInfo.MacroName);
					errorInfo.BookName = _macro.FindMacroBookName(cellInfo.MacroName);
					errorInfo.SheetName = cellInfo.MacroName;
				}
				else
				{
					errorInfo.Path = _currentBookPath;
					errorInfo.BookName = bookName;
					errorInfo.SheetName = sheetName;
				}

				_errorInfoList.Add(errorInfo);
			}
		}

		/// <summary>
		/// エラー用エクセルブック作成
		/// </summary>
		/// <param name="outputErrorXlsxDir">出力先</param>
		private void WriteErrorXlsx(string outputErrorXlsxDir)
		{
			IWorkbook book = new XSSFWorkbook();

			const int WordWidth = 256;

			var sheet = book.CreateSheet("エラーリスト");
			sheet.SetColumnWidth(0, WordWidth * 28);
			sheet.SetColumnWidth(1, WordWidth * 28);
			sheet.SetColumnWidth(2, WordWidth * 16);
			sheet.SetColumnWidth(3, WordWidth * 100);
			sheet.SetColumnWidth(4, WordWidth * 20);

			int row = 0;

			{
				var cellRow = sheet.CreateRow(row);
				var style = book.CreateCellStyle();
				style.Alignment = HorizontalAlignment.Center;

				ICell cell = null;

				cell = cellRow.CreateCell(0);
				cell.SetCellValue("ブック名");
				cell.CellStyle = style;

				cell = cellRow.CreateCell(1);
				cell.SetCellValue("シート名");
				cell.CellStyle = style;

				cell = cellRow.CreateCell(2);
				cell.SetCellValue("セル番地");
				cell.CellStyle = style;

				cell = cellRow.CreateCell(3);
				cell.SetCellValue("エラー内容");
				cell.CellStyle = style;

				cell = cellRow.CreateCell(4);
				cell.SetCellValue("リンク");
				cell.CellStyle = style;

				row++;
			}

			// 枠固定
			sheet.CreateFreezePane(1, row);

			foreach (var errorInfo in _errorInfoList)
			{
				var cellRow = sheet.CreateRow(row);

				var bookName = errorInfo.BookName + ".xlsx";

				cellRow.CreateCell(0).SetCellValue(bookName);
				cellRow.CreateCell(1).SetCellValue(errorInfo.SheetName);
				cellRow.CreateCell(2).SetCellValue(errorInfo.CellPosition);
				cellRow.CreateCell(3).SetCellValue(errorInfo.Message);

				// リンク
				{
					var path = errorInfo.Path;
					var pos = errorInfo.Path.IndexOf(':');
					if (pos >= 0)
					{
						path = path.Substring(pos);
					}

					var hyperLink = book.GetCreationHelper().CreateHyperlink(HyperlinkType.Unknown);
					hyperLink.Address = path + bookName + "#'" + errorInfo.SheetName + "'!" + errorInfo.CellPosition;

					var style = book.CreateCellStyle();
					style.Alignment = HorizontalAlignment.Center;

					var cell = cellRow.CreateCell(4);
					cell.SetCellValue("ブックにジャンプ");
					cell.Hyperlink = hyperLink;
					cell.CellStyle = style;
				}

				row++;
			}

			var filename = "error_list_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";
			using (var fs = new FileStream(outputErrorXlsxDir + filename, FileMode.Create))
			{
				book.Write(fs);
			}
		}
	}
}
