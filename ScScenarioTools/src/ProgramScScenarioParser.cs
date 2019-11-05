using System;

namespace ScScenarioTools
{
	/// <summary>
	/// コンバートされたファイルをパースしてcsvの内容を出力する
	/// プログラム引数
	///   --bytes_file (ファイルパス)
	///     [必須]
	///     コンバートされたファイル
	///   --arg_type_file (ファイルパス)
	///     [必須]
	///     ArgType.cs作成用のCSVファイルパス
	///   --command_dir (フォルダパス)
	///     [必須]
	///     コマンド用のCSVファイルがあるフォルダパス
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
			BytesFile,

			/// <summary>引数情報csvファイルパス</summary>
			ArgTypeFile,

			/// <summary>コマンドディレクトリ指定</summary>
			CommandDir,
		}

		/// <summary>
		/// エントリポイント
		/// </summary>
        static int Main(string[] args)
        {
			string bytesFile = "";
			string argTypeFile = "";
			string commandDir = "";

			var argOption = ArgOption.None;

			foreach (var arg in args)
			{
				if (arg.Equals("--bytes_file"))
				{
					argOption = ArgOption.BytesFile;
				}
				else if (arg.Equals("--arg_type_file"))
				{
					argOption = ArgOption.ArgTypeFile;
				}
				else if (arg.Equals("--command_dir"))
				{
					argOption = ArgOption.CommandDir;
				}
				else
				{
					switch (argOption)
					{
						case ArgOption.BytesFile:
						{
							bytesFile = arg;
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
						default:
						{
							System.Console.WriteLine("引数エラー: 引数が認識できません arg=" + arg);
							return 1;
						}
					}

					argOption = ArgOption.None;
				}
			}

			if (string.IsNullOrEmpty(bytesFile))
			{
				System.Console.WriteLine("引数エラー: --bytes_fileオプションが指定されていません");
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

			return BytesParser.Parse(bytesFile, argTypeFile, commandDir);
        }
    }
}
