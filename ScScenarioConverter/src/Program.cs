
namespace ScScenarioConverter
{
	class Program
	{
		/// <summary>
		/// エントリポイント
		///
		/// retval 0 正常終了
		/// retval 1 コンバートエラー
		/// </summary>
		private static int Main(string[] args)
		{
			if (args.Length >= 2)
			{
				// csv -> byte
				var csvFilePath = args[0];
				var outputDir = args[1];

				string infoCsvFilePath = null;
				if (args.Length >= 3)
				{
					infoCsvFilePath = args[2];
				}

				var converter = new Converter(csvFilePath, outputDir, infoCsvFilePath);
				if (!converter.IsSuccess)
				{
					return 1;
				}
			}
			else if (args.Length == 1)
			{
				if (args[0] == "/D")
				{
					Converter.ExportDocument();
				}
				else
				{
					// byte -> csv
					var binFilePath = args[0];

					var parser = new Parser(binFilePath);
					if (!parser.IsSuccess)
					{
						return 1;
					}
				}
			}
			else
			{
				System.Console.WriteLine("引数エラー");
				System.Console.WriteLine("");
				System.Console.WriteLine("[コンバート]");
				System.Console.WriteLine("ScScenarioConverter [CSVファイルパス] [出力パス] [補足情報ファイル(オプション)]");
				System.Console.WriteLine("");
				System.Console.WriteLine("[パース]");
				System.Console.WriteLine("ScScenarioConverter [バイナリファイルパス]");
				System.Console.WriteLine("");
				System.Console.WriteLine("[ドキュメント生成]");
				System.Console.WriteLine("ScScenarioConverter /D");

				return 1;
			}

			return 0;
		}
	}
}
