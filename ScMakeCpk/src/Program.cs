using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScMakeCpk
{
    class Program
    {
		/// <summary>
		/// 引数オプション
		/// </summary>
		private enum ArgOption
		{
			/// <summary>初期値</summary>
			None,

			/// <summary>元データフォルダ</summary>
			InputDir,

			/// <summary>Cpk出力先</summary>
			OutputDir,

			/// <summary>cpkmakec.exeへのパス</summary>
			CpkmakecPath,

			/// <summary>config.xmlへのパス</summary>
			ConfigXmlPath,

			/// <summary>テンポラリーファイル用フォルダ</summary>
			TempDir,
		}

		/// <summary>
		/// エントリポイント
		/// </summary>
        static int Main(string[] args)
        {
			string inputDir = "";
			string outputDir = "";
			string cpkmakecPath = "";
			string configXmlPath = "";
			string tempDir = "";

			var argOption = ArgOption.None;

			foreach (var arg in args)
			{
				if (arg.Equals("--input_dir"))
				{
					argOption = ArgOption.InputDir;
				}
				else if (arg.Equals("--output_dir"))
				{
					argOption = ArgOption.OutputDir;
				}
				else if (arg.Equals("--cpkmakec_path"))
				{
					argOption = ArgOption.CpkmakecPath;
				}
				else if (arg.Equals("--config_xml_path"))
				{
					argOption = ArgOption.ConfigXmlPath;
				}
				else if (arg.Equals("--temp_dir"))
				{
					argOption = ArgOption.TempDir;
				}
				else
				{
					switch (argOption)
					{
						case ArgOption.InputDir:
						{
							inputDir = arg;
							break;
						}
						case ArgOption.OutputDir:
						{
							outputDir = arg;
							break;
						}
						case ArgOption.CpkmakecPath:
						{
							cpkmakecPath = arg;
							break;
						}
						case ArgOption.ConfigXmlPath:
						{
							configXmlPath = arg;
							break;
						}
						case ArgOption.TempDir:
						{
							tempDir = arg;
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

			if (string.IsNullOrEmpty(inputDir))
			{
				System.Console.WriteLine("引数エラー: --input_dirオプションが指定されていません");
				return 1;
			}

			if (string.IsNullOrEmpty(outputDir))
			{
				System.Console.WriteLine("引数エラー: --output_dirオプションが指定されていません");
				return 1;
			}

			if (string.IsNullOrEmpty(cpkmakecPath))
			{
				System.Console.WriteLine("引数エラー: --cpkmakec_pathオプションが指定されていません");
				return 1;
			}

			if (string.IsNullOrEmpty(configXmlPath))
			{
				System.Console.WriteLine("引数エラー: --config_xml_pathオプションが指定されていません");
				return 1;
			}

			var converter = new CpkConverter();
			return converter.Convert(inputDir, outputDir, cpkmakecPath, configXmlPath, tempDir);
        }
    }
}
