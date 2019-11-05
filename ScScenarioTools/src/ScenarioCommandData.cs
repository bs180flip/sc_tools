using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScScenarioCommon
{
	public class ScenarioCommandData
	{
		/// <summary>コマンド引数の最大値</summary>
		public const int MaxCommandArgCount = 8;

		/// <summary>
		/// 引数情報
		/// </summary>
		public class ArgType
		{
			/// <summary>型情報</summary>
			public string Type = "";

			/// <summary>型情報(整数値)</summary>
			public byte TypeValue = 0;

			/// <summary>説明</summary>
			public string Desc = "";
		}

		/// <summary>
		/// コマンドの引数クラス
		/// </summary>
		public class CommandArg
		{
			/// <summary>型情報</summary>
			public string Type = "";

			/// <summary>型情報(整数値)</summary>
			public byte TypeValue = 0;

			/// <summary>説明</summary>
			public string Desc = "";

			/// <summary>最小値(指定がない場合はnull)</summary>
			public float? Min = null;

			/// <summary>最大値(指定がない場合はnull)</summary>
			public float? Max = null;

			/// <summary>変数置換の対象とするか</summary>
			public bool IsReplaceVariable = true;

			/// <summary>記述を省略可能か</summary>
			public bool IsOptional = false;

			/// <summary>オプション指定時のデフォルト値</summary>
			public string OptionalDefault = "";

			/// <summary>変数扱いをするかどうか</summary>
			public bool IsVariableOn = false;

			/// <summary>変数扱いをやめるかどうか</summary>
			public bool IsVariableOff = false;

			/// <summary>変数のみ許可するかどうか</summary>
			public bool IsVariableOnly = false;
		}

		/// <summary>
		/// コマンドクラス
		/// </summary>
		public class CommandInfo
		{
			/// <summary>コマンド名</summary>
			public string Name = "";

			/// <summary>コマンドの説明</summary>
			public string Desc = "";

			/// <summary>ドキュメント用のカテゴリ</summary>
			public string Category = "その他";

			/// <summary>ユニークなタイプ名(コマンドcsvのファイル名)</summary>
			public string TypeName = "";

			/// <summary>ユニークなID(NameをCRC32したものを入れる)</summary>
			public uint Id = 0;

			/// <summary>
			/// 引数情報データ
			/// 引数番号でソートするためSortedDictionaryに設定
			///   key=引数番号 value=引数情報
			/// </summary>
			public SortedDictionary<int, CommandArg> ArgDict = new SortedDictionary<int, CommandArg>();
		}

		/// <summary>ログ出力するかどうか</summary>
		private bool _isDumpConsole = true;
		public bool IsDumpConsole { set { _isDumpConsole = value; } }

		/// <summary>
		/// 読み込んだ引数情報格納場所
		///   key=引数名(Enumにするタイプ名) value=引数タイプ
		/// </summary>
		private Dictionary<string, ArgType> _argTypeDict = null;
		public Dictionary<string, ArgType> ArgTypeDict { get { return _argTypeDict; } }

		/// <summary>
		/// 読み込んだコマンド情報格納場所
		///   key=コマンド名 value=コマンド情報
		/// </summary>
		private SortedDictionary<string, CommandInfo> _commandInfoDict = null;
		public SortedDictionary<string, CommandInfo> CommandInfoDict { get { return _commandInfoDict; } }

		/// <summary>
		/// ArgTypeのCSVからデータ読み込み
		/// </summary>
		/// <param name="argTypeFile">引数情報のCSVファイル</param>
		public int ReadArgTypeCsv(string argTypeFile)
		{
			_argTypeDict = new Dictionary<string, ArgType>();

			byte typeValue = 0;

			var csv = FileUtility.ReadCsv(argTypeFile);
			for (var i = 0; i < csv.Count; i++)
			{
				var line = csv[i];

				if (line.Count <= 2) { continue; }

				if (line[0].IndexOf("//") == 0) { continue; }

				var typeEnum = line[0];
				var type = line[1];
				var desc = line[2];

				if (_argTypeDict.ContainsKey(typeEnum))
				{
					System.Console.WriteLine("ArgType CSVエラー: タイプが重複しています typeEnum=" + typeEnum);
					return 1;
				}

				var argType = new ArgType();
				argType.Type = type;
				argType.TypeValue = typeValue;
				argType.Desc = desc.Replace("\"", "");

				_argTypeDict.Add(typeEnum, argType);

				typeValue++;
			}

			if (_isDumpConsole)
			{
				System.Console.WriteLine("[ArgTypeFile]");
				System.Console.WriteLine("  " + argTypeFile);

				foreach (var argType in _argTypeDict)
				{
					var value = argType.Value;
					System.Console.WriteLine("    enum=" + argType.Key + " type=" + value.Type);
				}
			}

			return 0;
		}

		/// <summary>
		/// コマンドCSVからデータの読み込みをする
		/// </summary>
		/// <param name="commandDir">コマンドCSVが格納されたフォルダ</param>
		public int ReadCommandCsv(string commandDir)
		{
			_commandInfoDict = new SortedDictionary<string, CommandInfo>();

			if (_isDumpConsole)
			{
				System.Console.WriteLine("[CommandFiles]");
			}

			var crc32 = new CRC32();

			var files = Directory.GetFiles(commandDir, "*.csv");
			foreach (var file in files)
			{
				var commandInfo = new CommandInfo();

				// ファイル名をユニークタイプ名にする
				commandInfo.TypeName = Path.GetFileNameWithoutExtension(file);

				// ファイル名からIDをCRC32で自動算出する
				{
					var bytes = Encoding.ASCII.GetBytes(commandInfo.TypeName);
					var strHex = BitConverter.ToString(crc32.ComputeHash(bytes)).Replace("-", "");
					commandInfo.Id = uint.Parse(strHex, System.Globalization.NumberStyles.HexNumber);
				}

				var csv = FileUtility.ReadCsv(file);
				foreach (var line in csv)
				{
					if (line.Count <= 1) { continue; }

					var key = line[0];
					var value = line[1];

					// 値が設定されてないものは無視する
					if (string.IsNullOrEmpty(value))
					{
						continue;
					}

					if (key.Equals("name"))
					{
						commandInfo.Name = value;
					}
					else if (key.Equals("desc"))
					{
						commandInfo.Desc = value.Replace("\"", "");
					}
					else if (key.Equals("category"))
					{
						commandInfo.Category = value;
					}
					else if (key.IndexOf("_") >= 1) // 引数は[引数番号_コマンド]の形になっている
					{
						var splits = key.Split('_');
						if (splits.Length < 2)
						{
							DumpError("Command CSVエラー: 引数指定エラー target=" + key);
							return 1;
						}

						var strArgNumber = splits[0];
						var strCommand = splits[1];

						int argNumber = 0;
						if (Int32.TryParse(strArgNumber, out argNumber))
						{
							if (argNumber < 1 || argNumber > MaxCommandArgCount)
							{
								DumpError("Command CSVエラー: 引数番号オーバー number=" + strArgNumber);
								return 1;
							}

							var argIndex = argNumber - 1;

							CommandArg commandArg = null;
							if (commandInfo.ArgDict.TryGetValue(argIndex, out commandArg) == false)
							{
								commandArg = new CommandArg();
								commandInfo.ArgDict.Add(argIndex, commandArg);
							}

							if (strCommand.Equals("type"))
							{
								// 引数タイプ情報がある場合は型が存在するかチェックする
								ArgType argType = null;
								if (_argTypeDict.TryGetValue(value, out argType) == false)
								{
									DumpError("Command CSVエラー: 存在しないタイプです type=" + value);
									return 1;
								}

								commandArg.Type = value;
								commandArg.TypeValue = argType.TypeValue;
							}
							else if (strCommand.Equals("desc"))
							{
								commandArg.Desc = value.Replace("\"", "");
							}
							else if (strCommand.Equals("min"))
							{
								float min = 0f;
								if (float.TryParse(value, out min))
								{
									commandArg.Min = min;
								}
								else
								{
									DumpError("Command CSVエラー: 実数変換に失敗 float=" + value);
									return 1;
								}
							}
							else if (strCommand.Equals("max"))
							{
								float max = 0f;
								if (float.TryParse(value, out max))
								{
									commandArg.Max = max;
								}
								else
								{
									DumpError("Command CSVエラー: 実数変換に失敗 float=" + value);
									return 1;
								}
							}
							else if (strCommand.Equals("replace-variable"))
							{
								if (value.ToLower().Equals("false"))
								{
									commandArg.IsReplaceVariable = false;
								}
							}
							else if (strCommand.Equals("optional"))
							{
								if (value.ToLower().Equals("true"))
								{
									commandArg.IsOptional = true;
								}
							}
							else if (strCommand.Equals("optional-default"))
							{
								commandArg.OptionalDefault = value;
							}
							else if (strCommand.Equals("variable-on"))
							{
								if (value.ToLower().Equals("true"))
								{
									commandArg.IsVariableOn = true;
								}
							}
							else if (strCommand.Equals("variable-off"))
							{
								if (value.ToLower().Equals("true"))
								{
									commandArg.IsVariableOff = true;
								}
							}
							else if (strCommand.Equals("variable-only"))
							{
								if (value.ToLower().Equals("true"))
								{
									commandArg.IsVariableOnly = true;
								}
							}
							else
							{
								DumpError("Command CSVエラー: 引数のキーが認識できません=" + strCommand);
								return 1;
							}
						}
						else
						{
							DumpError("Command CSVエラー: 引数番号が整数に変換できません number=" + argNumber);
							return 1;
						}
					}
					else
					{
						DumpError("Command CSVエラー: 認識できないキーです command=" + key);
						return 1;
					}
				}

				if (_commandInfoDict.ContainsKey(commandInfo.Name))
				{
					DumpError("コマンド名が重複しています command=" + commandInfo.Name);
					return 1;
				}

				if (_isDumpConsole)
				{
					System.Console.WriteLine("  " + commandInfo.Name + " " + file);
				}

				_commandInfoDict.Add(commandInfo.Name, commandInfo);
			}

			return 0;
		}

		/// <summary>
		/// エラー出力(コンソール)
		/// </summary>
		/// <param name="message">表示メッセージ</param>
		private void DumpError(string message)
		{
			System.Console.WriteLine("    " + message);
		}
	}
}
