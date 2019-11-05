using System;

namespace ScScenarioTools
{
	/// <summary>
	/// CSVファイルからシナリオ用のソースコード生成ツール
	/// プログラム引数
	///   --arg_type_file (ファイルパス)
	///     [必須]
	///     ArgType.cs作成用のCSVファイルパス
	///   --command_dir (フォルダパス)
	///     [必須]
	///     コマンド用のCSVファイルがあるフォルダパス
	///   --output_dir (フォルダパス)
	///     [オプション]
	///     出力先指定(指定がない場合はexeと同じ場所に出力)
	///   --disable_dump_console
	///     [オプション]
	///     指定するとコンソールに作業中の出力を制限する
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

			/// <summary>引数情報csvファイルパス</summary>
			ArgTypeFile,

			/// <summary>コマンドディレクトリ指定</summary>
			CommandDir,

			/// <summary>出力先</summary>
			OutputDir,
		}

		/// <summary>
		/// エントリポイント
		/// </summary>
        static int Main(string[] args)
        {
			string argTypeFile = "";
			string commandDir = "";
			string outputDir = "";
			bool isDumpConsole = true;

			var argOption = ArgOption.None;

			foreach (var arg in args)
			{
				if (arg.Equals("--arg_type_file"))
				{
					argOption = ArgOption.ArgTypeFile;
				}
				else if (arg.Equals("--command_dir"))
				{
					argOption = ArgOption.CommandDir;
				}
				else if (arg.Equals("--output_dir"))
				{
					argOption = ArgOption.OutputDir;
				}
				else if (arg.Equals("--disable_dump_console"))
				{
					isDumpConsole = false;
				}
				else
				{
					switch (argOption)
					{
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
						case ArgOption.OutputDir:
						{
							outputDir = arg;
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

			return SourceGenerator.Generate(argTypeFile, commandDir, outputDir, isDumpConsole);
        }
    }
}
