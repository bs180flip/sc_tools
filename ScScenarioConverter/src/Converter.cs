using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sc.Scenario;

namespace ScScenarioConverter
{
	/// <summary>
	/// CSVをシナリオバイナリに変換
	/// </summary>
	public class Converter
	{
		/// <summary>
		/// Csvの情報用クラス
		/// </summary>
		public class CsvInfo
		{
			/// <summary>セルの番地</summary>
			public int CellIndex { get { return _cellIndex; } }
			private int _cellIndex;

			/// <summary>マクロ名</summary>
			public string MacroName { get { return _macroName; } }
			private string _macroName;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="cellIndex">セル番号</param>
			/// <param name="macroName">マクロ名</param>
			public CsvInfo(int cellIndex, string macroName)
			{
				_cellIndex = cellIndex;
				_macroName = macroName;
			}
		}

		/// <summary>CSVの開始行</summary>
		private const int CSV_START_LINE = 1;

		/// <summary>セパレーター</summary>
		private const char Separator = ',';

		/// <summary>デリミター</summary>
		private const char Delimiter = '|';

		/// <summary>置換元改行コード</summary>
		private const string NewLineBefore = "\n";

		/// <summary>置換先改行コード</summary>
		private const string NewLineAfter = "\r\n";

		/// <summary>コマンド行</summary>
		private const int ColumnCommand = 0;

		/// <summary>引数行</summary>
		private const int ColumnArg = 1;

		/// <summary>CSVファイル名</summary>
		private string _csvFileName = "";

		/// <summary>変数の辞書</summary>
		private HashSet<string> _varHash = null;

		/// <summary>変数(Vector3型）の辞書</summary>
		private HashSet<string> _varVector3Hash = null;

		/// <summary>成功失敗</summary>
		private bool _isSuccess = false;
		public bool IsSuccess { get { return _isSuccess; } }

		/// <summary>エラーリスト</summary>
		private List<string> _errorList = new List<string>();
		public List<string> ErrorList { get { return _errorList; } }

		/// <summary>コマンド情報辞書</summary>
		private static CommandInfoDict _commandInfoDict = new CommandInfoDict();

		/// <summary>バイナリライター</summary>
		private BinaryWriter _binaryWriter = null;

		/// <summary>バイナリ拡張子</summary>
		private const string BinExt = ".bytes";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="csvFileName">CSVファイルパス</param>
		/// <param name="outputDir">出力先ディレクトリ</param>
		/// <param name="infoCsvFileName">補足ファイルパス(オプション)</param>
		public Converter(string csvFilePath, string outputDir, string infoCsvFilePath)
		{
			_varHash = new HashSet<string>();
			_varVector3Hash = new HashSet<string>();

			var binFileDir = Path.Combine(outputDir, Path.GetFileName(Path.GetDirectoryName(csvFilePath)));
			if (!Directory.Exists(binFileDir))
			{
				Directory.CreateDirectory(binFileDir);
			}

			_csvFileName = Path.GetFileName(csvFilePath);
			var binFileName = Path.GetFileNameWithoutExtension(csvFilePath) + BinExt;
			var binFilePath = Path.Combine(binFileDir, binFileName);

			// csvの補足情報の読み込み
			var csvInfoList = new List<CsvInfo>();
			if (infoCsvFilePath != null)
			{
				// 見出し用
				csvInfoList.Add(new CsvInfo(0, ""));

				var text = File.ReadAllText(infoCsvFilePath);
				var lines = text.Split(new string[] { Separator + NewLineAfter }, StringSplitOptions.None);

				for (int i = CSV_START_LINE, iMax = lines.Length; i < iMax; i++)
				{
					if (string.IsNullOrEmpty(lines[i])) { continue; }

					var values = new List<string>(lines[i].Split(Separator));
					if (values == null || values.Count < 2) { continue; }

					csvInfoList.Add(new CsvInfo(Int32.Parse(values[0]), values[1]));
				}
			}

			using (var fileStream = new FileStream(binFilePath, FileMode.Create))
			{
				_binaryWriter = new BinaryWriter(fileStream);

				var csvText = File.ReadAllText(csvFilePath);
				var csvLines = csvText.Split(new string[] { Separator + NewLineAfter }, StringSplitOptions.None);
				_isSuccess = ConvertAll(csvLines, csvInfoList);

				_binaryWriter.Close();
				fileStream.Close();
			}

			if (!_isSuccess)
			{
				File.Delete(binFilePath);
			}
		}

		/// <summary>
		/// 全体を変換
		/// </summary>
		/// <param name="csvLines">CSV全行</param>
		/// <param name="csvInfoList">補足情報データ一覧</param>
		private bool ConvertAll(string[] csvLines, List<CsvInfo> csvInfoList)
		{
			var defaultCsvInfo = new CsvInfo(cellIndex: -1, macroName: "");

			for (int i = CSV_START_LINE, iMax = csvLines.Length; i < iMax; i++)
			{
				ConvertLine(i, csvLines[i], csvInfoList.Count > i ? csvInfoList[i] : defaultCsvInfo);
			}

			Console.Write("> " + _csvFileName + ": ");
			if (_errorList.Count == 0)
			{
				Console.WriteLine("成功");

				return true;
			}
			else
			{
				Console.WriteLine("失敗");

				foreach (var err in _errorList)
				{
					Console.WriteLine(err);
				}

				return false;
			}
		}

		/// <summary>
		/// 1行分を変換
		/// </summary>
		/// <param name="csvLine">CSV1行</param>
		/// <param name="csvInfo">補足情報</param>
		private void ConvertLine(int line, string csvLine, CsvInfo csvInfo)
		{
			if (string.IsNullOrEmpty(csvLine)) { return; }

			var values = new List<string>(csvLine.Split(Separator));
			if (values == null || values.Count < 2) { return; }

			var commandName = values[0];
			var argList = values.GetRange(1, values.Count - 1);

			// 空行なので読み飛ばす
			if (string.IsNullOrEmpty(commandName)) { return; }

			// コメント行なので読み飛ばす
			if (commandName.IndexOf("//") == 0) { return; }

			var commandInfo = _commandInfoDict[commandName];
			if (commandInfo == null)
			{
				AddError(line, ColumnCommand, null, csvInfo, "コマンドが存在しません！ コマンド=" + commandName);
				return;
			}

			Convert(line, commandInfo, argList, csvInfo);
		}

		/// <summary>
		/// 変換
		/// </summary>
		/// <param name="commandInfo">コマンド情報</param>
		/// <param name="argList">引数リスト</param>
		/// <param name="csvInfo">Csv情報</param>
		private void Convert(int line, CommandInfo commandInfo, List<string> argList, CsvInfo csvInfo)
		{
			var argCount = 0;
			var argsByteList = new List<byte>();

			// 引数の超過チェック
			for (var i = commandInfo.ArgInfoList.Count; i < argList.Count; i++)
			{
				if (argList[i] != "")
				{
					AddError(line, ColumnArg + i, commandInfo, csvInfo, "引数オーバー！");
				}
			}

			foreach (var argInfo in commandInfo.ArgInfoList)
			{
				var argType = argInfo.ArgType;
				var argString = argList[argCount];

				// #で始まる値がある場合は定数展開できてないのでエラーにする
				if (argString.IndexOf("#") == 0)
				{
					AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "定数が見つかりません！");
				}
				else if (_varHash.Contains(argString) || _varVector3Hash.Contains(argString))
				{
					// 変数リストにある文字列の場合はstringにして書き込み
					// スクリプト側で変数データを展開する
					argType = ArgType.String;
				}

				// 変数セット
				if (commandInfo.CommandType == CommandType.VarSet && argCount == 0)
				{
					if (_varHash.Contains(argString))
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "すでに定義されているInt変数です！");
					}
					else
					{
						_varHash.Add(argString);
					}
				}
				// Vector3変数セット
				else if (commandInfo.CommandType == CommandType.VarVector3Set && argCount == 0)
				{
					if (_varVector3Hash.Contains(argString))
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "すでに定義されているVector3変数です！");
					}
					else
					{
						_varVector3Hash.Add(argString);
					}
				}

				// 型情報を追加
				argsByteList.Add((byte)argType);

				switch (argType)
				{
				case ArgType.Bool:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "FALSE";
					}

					if (bool.TryParse(argString, out bool boolValue))
					{
						var bytes = BitConverter.GetBytes(boolValue);
						argsByteList.AddRange(bytes);
					}
					else
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "boolへの変換失敗！ TRUEかFALSEを使用して下さい！");
					}
					break;
				}
				case ArgType.Byte:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "0";
					}

					if (byte.TryParse(argString, out byte value))
					{
						var min = argInfo.Min;
						var max = argInfo.Max;
						if (!min.HasValue || !max.HasValue || (min <= value && value <= max))
						{
							argsByteList.Add(value);
						}
						else
						{
							AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "範囲外！ " + min + " ~ " + max + " の範囲を指定して下さい！");
						}
					}
					else
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "byteへの変換失敗！");
					}
					break;
				}
				case ArgType.Int:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "0";
					}

					if (int.TryParse(argString, out int value))
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
							AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "範囲外！ " + min + " ~ " + max + " の範囲を指定して下さい！");
						}
					}
					else
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "Intへの変換失敗！");
					}
					break;
				}
				case ArgType.Long:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "0";
					}

					if (long.TryParse(argString, out long value))
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
							AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "範囲外！ " + min + " ~ " + max + " の範囲を指定して下さい！");
						}
					}
					else
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "Longへの変換失敗！");
					}
					break;
				}
				case ArgType.Float:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "0";
					}

					if (float.TryParse(argString, out float value))
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
							AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "範囲外！ " + min + " ~ " + max + " の範囲を指定して下さい！");
						}
					}
					else
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "Floatへの変換失敗！");
					}
					break;
				}
				case ArgType.String:
				{
					if (argInfo.IsOptional == false && string.IsNullOrEmpty(argString))
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "文字列データが空になっています");
						break;
					}

					var value = argString.Replace("\"", "").Replace(NewLineBefore, NewLineAfter);
					var bytes = Encoding.UTF8.GetBytes(value);
					var length = BitConverter.GetBytes((short)bytes.Length);
					argsByteList.AddRange(length);
					argsByteList.AddRange(bytes);
					break;
				}
				case ArgType.Vector2:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "0|0";
					}

					var values = argString.Split(Delimiter);
					if (values == null || values.Length != 2)
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "要素数が不一致！ n|nの形で指定して下さい！");
						break;
					}

					if ((float.TryParse(values[0], out float xValue))
					 && (float.TryParse(values[1], out float yValue))
					)
					{
						var xBytes = BitConverter.GetBytes(xValue);
						argsByteList.AddRange(xBytes);

						var yBytes = BitConverter.GetBytes(yValue);
						argsByteList.AddRange(yBytes);
					}
					else
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "Vector2への変換失敗！");
					}
					break;
				}
				case ArgType.Vector3:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "0|0|0";
					}

					var values = argString.Split(Delimiter);
					if (values == null || values.Length != 3)
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "要素数が不一致！ n|n|nの形で指定して下さい！");
						break;
					}

					if ((float.TryParse(values[0], out float xValue))
					 && (float.TryParse(values[1], out float yValue))
					 && (float.TryParse(values[2], out float zValue))
					)
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
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "Vector3への変換失敗！");
					}
					break;
				}
				case ArgType.Rect:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "0|0|0|0";
					}

					var values = argString.Split(Delimiter);
					if (values == null || values.Length != 4)
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "要素数が不一致！ n|n|n|nの形で指定して下さい！");
						break;
					}

					if ((float.TryParse(values[0], out float xValue))
					 && (float.TryParse(values[1], out float yValue))
					 && (float.TryParse(values[2], out float wValue))
					 && (float.TryParse(values[3], out float hValue))
					)
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
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "Rectへの変換失敗！");
					}
					break;
				}
				case ArgType.Color:
				{
					if (argInfo.IsOptional && argString == "")
					{
						argString = "0|0|0";
					}

					var values = argString.Split(Delimiter);
					if (values == null || values.Length != 3)
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "要素数が不一致！ n|n|nの形で指定して下さい！");
						break;
					}

					if ((byte.TryParse(values[0], out byte rValue))
					 && (byte.TryParse(values[1], out byte gValue))
					 && (byte.TryParse(values[2], out byte bValue))
					)
					{
						argsByteList.Add(rValue);
						argsByteList.Add(gValue);
						argsByteList.Add(bValue);
					}
					else
					{
						AddError(line, ColumnArg + argCount, commandInfo, csvInfo, "Colorへの変換失敗！");
					}
					break;
				}
				default:
					break;
				}

				argCount++;
			}

			// コマンドタイプを書き込む
			var commandType = (short)commandInfo.CommandType;
			_binaryWriter.Write(commandType);

			// 引数長を書き込む
			var argsSize = (short)argsByteList.Count;
			_binaryWriter.Write(argsSize);

			// 引数情報を書き込む
			_binaryWriter.Write(argsByteList.ToArray());
		}

		/// <summary>
		/// エラー一覧にエラーを追加
		/// </summary>
		/// <param name="row">行</param>
		/// <param name="column">列</param>
		/// <param name="csvInfo">csv補足情報(Excel時のデータ)</param>
		/// <param name="message">表示メッセージ</param>
		private void AddError(int row, int column, CommandInfo commandInfo, CsvInfo csvInfo, string message)
		{
			var sb = new StringBuilder();

			sb.Append("[");

			// 補足情報がある場合はExcelでのセル位置も記載
			if (csvInfo.CellIndex >= 0)
			{
				sb.Append("Excel=");
				sb.Append((char)('A' + column));
				sb.Append(csvInfo.CellIndex);
				sb.Append("|");
			}

			// csv内の行数
			sb.Append("Csv=");
			sb.Append(column + 1);
			sb.Append(",");
			sb.Append(row + 1);
			sb.Append("]: ");

			if (csvInfo.MacroName != "")
			{
				sb.Append("マクロ名=" + csvInfo.MacroName);
				sb.Append(" ");
			}

			if (commandInfo != null)
			{
				sb.Append("コマンド=" + _commandInfoDict.SearchNameFromType(commandInfo.CommandType));
				sb.Append(" ");
			}

			_errorList.Add(sb.ToString() + message);
		}

		/// <summary>
		/// ドキュメントを出力
		/// </summary>
		public static void ExportDocument()
		{
			ExportMenu();
			ExportDetail();
		}

		/// <summary>
		/// 詳細を出力
		/// </summary>
		private static void ExportDetail()
		{
			var sb = new StringBuilder();

			sb.AppendLine("<html>");
			sb.AppendLine("<body>");

			foreach (var commandName in _commandInfoDict.Keys)
			{
				var commandInfo = _commandInfoDict[commandName];
				if (commandInfo == null) { continue; }
				var commandType = commandInfo.CommandType.ToString();

				sb.AppendLine("<h1 id=\"" + commandType + "\">" + commandName + "</h1>");
				sb.AppendLine("<h2>説明: " + commandInfo.CommandDesc + "</h2>");

				var count = commandInfo.ArgInfoList.Count;
				if (count == 0)
				{
					sb.AppendLine("<h3>引数なし</h3>");
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						var argInfo = commandInfo.ArgInfoList[i];
						sb.Append("<h3>");
						sb.Append("引数" + (i + 1) + ": ");

						sb.Append("<br>　説明: " + argInfo.ArgDesc);

						switch (argInfo.ArgType)
						{
						case ArgType.Bool:
						{
							sb.Append("<br>　　型: Bool");
							break;
						}
						case ArgType.Byte:
						{
							sb.Append("<br>　　型: Byte");
							break;
						}
						case ArgType.Int:
						{
							sb.Append("<br>　　型: Int");
							break;
						}
						case ArgType.Long:
						{
							sb.Append("<br>　　型: Long");
							break;
						}
						case ArgType.Float:
						{
							sb.Append("<br>　　型: Float");
							break;
						}
						case ArgType.String:
						{
							sb.Append("<br>　　型: String");
							break;
						}
						case ArgType.Vector2:
						{
							sb.Append("<br>　　型: Vector2 (n|n)");
							break;
						}
						case ArgType.Vector3:
						{
							sb.Append("<br>　　型: Vector3 (n|n|n)");
							break;
						}
						case ArgType.Rect:
						{
							sb.Append("<br>　　型: Rect (n|n|n|n)");
							break;
						}
						case ArgType.Color:
						{
							sb.Append("<br>　　型: Color (n|n|n)");
							break;
						}
						}

						if (argInfo.Min == null && argInfo.Max == null)
						{
						}
						else if (argInfo.Min == null)
						{
							sb.Append("<br>　範囲: 最大=" + argInfo.Max);
						}
						else if (argInfo.Max == null)
						{
							sb.Append("<br>　範囲: 最小=" + argInfo.Min);
						}
						else
						{
							sb.Append("<br>　範囲: 最小=" + argInfo.Min + " 最大=" + argInfo.Max);
						}

						if (argInfo.IsOptional)
						{
							sb.Append("<br>　省略: 可");
						}

						sb.AppendLine("</h3>");
					}
				}

				sb.AppendLine("<hr>");
			}

			sb.AppendLine("</body>");
			sb.AppendLine("</html>");

			File.WriteAllText("detail.html", sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// メニューを出力
		/// </summary>
		private static void ExportMenu()
		{
			var sb = new StringBuilder();

			sb.AppendLine("<html>");
			sb.AppendLine("<body>");
			sb.AppendLine("<h3>コマンド一覧</h3>");

			foreach (var commandName in _commandInfoDict.Keys)
			{
				var commandInfo = _commandInfoDict[commandName];
				if (commandInfo == null) { continue; }
				var commandType = commandInfo.CommandType.ToString();
				sb.AppendLine("<li>");
				sb.AppendLine("  <a href=\"detail.html#" + commandType + "\" target = \"detail\">" + commandName + "</a>");
				sb.AppendLine("</li>");
			}

			sb.AppendLine("</body>");
			sb.AppendLine("</html>");

			File.WriteAllText("menu.html", sb.ToString(), Encoding.UTF8);
		}
	}
}
