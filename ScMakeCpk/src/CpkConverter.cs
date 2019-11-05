using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ScMakeCpk
{
	/// <summary>
	/// コンバートクラス
	/// </summary>
	class CpkConverter
	{
		/// <summary>対応拡張子一覧</summary>
		private HashSet<string> _extensionList = null;

		/// <summary>
		/// cpkにコンバート
		/// </summary>
		/// <param name="inputDir">パッケージ化する対象フォルダ</param>
		/// <param name="outputDir">cpk出力先</param>
		/// <param name="cpkmakecPath">cpkmakec.exeへのパス</param>
		/// <param name="configXmlPath">config.xmlへのパス</param>
		/// <param name="tempDir">テンポラリーフォルダ</param>
		public int Convert(string inputDir,
						   string outputDir,
						   string cpkmakecPath,
						   string configXmlPath,
						   string tempDir)
		{
			// config.xmlの読み込み
			{
				_extensionList = new HashSet<string>();

				var xml = XDocument.Load(configXmlPath);

				var root = xml.Element("ConfigData");
				if (root == null)
				{
					Console.WriteLine("XML: ConfigData読み込みエラー");
					return 1;
				}

				var extensions = root.Element("extensions");
				if (extensions == null)
				{
					Console.WriteLine("XML: extensions読み込みエラー");
					return 1;
				}

				foreach (var extension in extensions.Descendants("string"))
				{
					_extensionList.Add("." + extension.Value);
				}
			}

			ProcessStartInfo processInfo = new ProcessStartInfo();
			processInfo.FileName = cpkmakecPath;
			processInfo.CreateNoWindow = true;
			processInfo.UseShellExecute = false;

			// フォルダからcpk作成
			{
				var directories = Directory.GetDirectories(inputDir);
				foreach (var directory in directories)
				{
					Console.WriteLine(directory);

					var targetFileList = new List<string>();

					var files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories);
					foreach (var file in files)
					{
						var extension = Path.GetExtension(file);
						if (_extensionList.Contains(extension))
						{
							targetFileList.Add(file);
							Console.WriteLine("  " + Path.GetFileName(file));
						}
					}

					if (targetFileList.Count > 0)
					{
						var sb = new StringBuilder();

						foreach (var targetFile in targetFileList)
						{
							sb.AppendLine("\"" + targetFile + "\", , Compress");
						}

						// cpkmakec用のcsv作成
						var filename = Path.GetFileName(directory);
						var csvPath = tempDir + filename + ".csv";
						File.WriteAllText(csvPath, sb.ToString(), Encoding.UTF8);

						// cpkmakecでcpk作成
						RunCpkmakec(processInfo, csvPath, outputDir, filename);
					}
				}
			}

			// ファイルからcpk作成
			{
				var files = Directory.GetFiles(inputDir);
				foreach (var file in files)
				{
					var extension = Path.GetExtension(file);
					if (_extensionList.Contains(extension))
					{
						Console.WriteLine(file);

						// cpkmakec用のcsv作成
						var filename = Path.GetFileNameWithoutExtension(file);
						var csvPath = tempDir + filename + ".csv";
						File.WriteAllText(csvPath, "\"" + file + "\", , Compress", Encoding.UTF8);

						// cpkmakecでcpk作成
						RunCpkmakec(processInfo, csvPath, outputDir, filename);
					}
				}
			}

			return 0;
		}

		/// <summary>
		/// cpkmakec起動
		/// </summary>
		/// <param name="processInfo">外部アプリ実行情報</param>
		/// <param name="csvPath">cpkmakecに使用するcsvファイルへのパス</param>
		/// <param name="outputDir">cpk出力先</param>
		/// <param name="outputFilename">出力ファイル名</param>
		private void RunCpkmakec(ProcessStartInfo processInfo,
								 string csvPath,
								 string outputDir,
								 string outputFilename)
		{
			var argument = csvPath + " " + outputDir + outputFilename + ".cpk -align=8 -code=SJIS -mode=FILENAMEIDGROUP -nolocalinfo -rand -mask";
			processInfo.Arguments = argument;
			Process.Start(processInfo);
		}
	}
}
