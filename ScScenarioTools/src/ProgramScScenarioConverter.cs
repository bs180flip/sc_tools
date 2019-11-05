namespace ScScenarioTools
{
	/// <summary>
	/// CSVファイルからシナリオ用のソースコード生成ツール
	/// プログラム引数
	///   --book_file (ファイルパス)
	///     [必須]
	///     変換元のxlsxファイルパス
	///   --arg_type_file (ファイルパス)
	///     [必須]
	///     ArgType.cs作成用のCSVファイルパス
	///   --command_dir (フォルダパス)
	///     [必須]
	///     コマンド用のCSVファイルがあるフォルダパス
	///   --macro_dir (フォルダパス)
	///     [オプション]
	///     マクロ用xlsxがあるフォルダへのパス
	///   --global_define_file (ファイルパス)
	///     [オプション]
	///     定義内容が書かれたテキストファイルへのパス
	///   --output_csv_dir (フォルダパス)
	///     [オプション]
	///     CSV出力先指定(指定がない場合はexeと同じ場所に出力)
	///   --output_bin_dir (フォルダパス)
	///     [オプション]
	///     スクリプトデータ出力先指定(指定がない場合はexeと同じ場所に出力)
	///   --disable_dump_console
	///     [オプション]
	///     指定するとコンソールに作業中の出力を制限する
	///   --enable_error_xlsx
	///     [オプション]
	///     指定するとエラー内容のエクセルブックを出力する
	///   --output_error_xlsx_dir（フォルダパス）
	///     [オプション]
	///     --enable_error_xlsxをしたときに出力する先
	/// </summary>
    class Program
    {
		/// <summary>
		/// 引数オプション
		/// </summary>
		private enum ArgOption
		{
			/// <summary>初期値</summary>
			None,

			/// <summary>xlsxファイルパス</summary>
			BookFile,

			/// <summary>xlsxフォルダパス</summary>
			BookDir,

			/// <summary>引数情報csvファイルパス</summary>
			ArgTypeFile,

			/// <summary>コマンドフォルダパス</summary>
			CommandDir,

			/// <summary>マクロフォルダパス</summary>
			MacroDir,

			/// <summary>定義ファイルパス</summary>
			GlobalDefineFile,

			/// <summary>スクリプト用定義フォルダパス</summary>
			LocalDefineDir,

			/// <summary>Csv出力先</summary>
			OutputCsvDir,

			/// <summary>Bin出力先</summary>
			OutputBinDir,

			/// <summary>エラー用のxlsxを出力する先</summary>
			OutputErrorXlsxDir,
		}

		/// <summary>
		/// エントリポイント
		/// </summary>
        static int Main(string[] args)
        {
			string bookFile = "";
			string bookDir = "";
			string argTypeFile = "";
			string commandDir = "";
			string macroDir = "";
			string globalDefineFile = "";
			string localDefineDir = "";
			string outputCsvDir = "";
			string outputBinDir = "";
			string outputErrorXlsxDir = "";
			bool isDumpConsole = true;
			bool isEnableErrorXlsx = false;

			var argOption = ArgOption.None;

			foreach (var arg in args)
			{
				if (arg.Equals("--book_file"))
				{
					argOption = ArgOption.BookFile;
				}
				else if (arg.Equals("--book_dir"))
				{
					argOption = ArgOption.BookDir;
				}
				else if (arg.Equals("--arg_type_file"))
				{
					argOption = ArgOption.ArgTypeFile;
				}
				else if (arg.Equals("--command_dir"))
				{
					argOption = ArgOption.CommandDir;
				}
				else if (arg.Equals("--macro_dir"))
				{
					argOption = ArgOption.MacroDir;
				}
				else if (arg.Equals("--global_define_file"))
				{
					argOption = ArgOption.GlobalDefineFile;
				}
				else if (arg.Equals("--local_define_dir"))
				{
					argOption = ArgOption.LocalDefineDir;
				}
				else if (arg.Equals("--output_csv_dir"))
				{
					argOption = ArgOption.OutputCsvDir;
				}
				else if (arg.Equals("--output_bin_dir"))
				{
					argOption = ArgOption.OutputBinDir;
				}
				else if (arg.Equals("--output_error_xlsx_dir"))
				{
					argOption = ArgOption.OutputErrorXlsxDir;
				}
				else if (arg.Equals("--disable_dump_console"))
				{
					isDumpConsole = false;
				}
				else if (arg.Equals("--enable_error_xlsx"))
				{
					isEnableErrorXlsx = true;
				}
				else
				{
					switch (argOption)
					{
						case ArgOption.BookFile:
						{
							bookFile = arg;
							break;
						}
						case ArgOption.BookDir:
						{
							bookDir = arg;
							break;
						}
						case ArgOption.ArgTypeFile:
						{
							argTypeFile = arg;
							break;
						}
						case ArgOption.CommandDir:
						{
							commandDir = arg;
							break;
						}
						case ArgOption.MacroDir:
						{
							macroDir = arg;
							break;
						}
						case ArgOption.GlobalDefineFile:
						{
							globalDefineFile = arg;
							break;
						}
						case ArgOption.LocalDefineDir:
						{
							localDefineDir = arg;
							break;
						}
						case ArgOption.OutputCsvDir:
						{
							outputCsvDir = arg;
							break;
						}
						case ArgOption.OutputBinDir:
						{
							outputBinDir = arg;
							break;
						}
						case ArgOption.OutputErrorXlsxDir:
						{
							outputErrorXlsxDir = arg;
							break;
						}
						default:
						{
							System.Console.WriteLine("引数エラー: 引数が認識できません arg=" + arg);
							return 1;
						}
					}

					argOption = ArgOption.None;
				}
			}

			if (string.IsNullOrEmpty(bookFile) && string.IsNullOrEmpty(bookDir))
			{
				System.Console.WriteLine("引数エラー: --book_fileか--book_dirオプションが指定されていません");
				return 1;
			}

			if (string.IsNullOrEmpty(argTypeFile))
			{
				System.Console.WriteLine("引数エラー: --arg_info_fileオプションが指定されていません");
				return 1;
			}

			if (string.IsNullOrEmpty(commandDir))
			{
				System.Console.WriteLine("引数エラー: --command_dirオプションが指定されていません");
				return 1;
			}

			var converter = new ScenarioConverter();
			return converter.Convert(bookFile,
									 bookDir,
									 argTypeFile,
									 commandDir,
									 globalDefineFile,
									 localDefineDir,
									 macroDir,
									 outputCsvDir,
									 outputBinDir,
									 outputErrorXlsxDir,
									 isEnableErrorXlsx,
									 isDumpConsole);
        }
    }
}
