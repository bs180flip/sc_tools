using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScScenarioCommon
{
	public class FileUtility
	{
		/// <summary>
		/// ファイル検索をして見つかったものをListで返す
		/// </summary>
		/// <param name="path">検索パス</param>
		/// <param name="pattern">検索パターン(*.csvとか)</param>
		/// <param name="isSearchingSubDirectories">サブディレクトリも検索するかどうか</param>
		public static List<string> SearchFiles(string dir, string pattern, bool isSearchingSubDirectories)
		{
			var files = new List<string>();

			var option = SearchOption.TopDirectoryOnly;
			if (isSearchingSubDirectories)
			{
				option = SearchOption.AllDirectories;
			}

			var searchFiles = Directory.EnumerateFiles(dir, pattern, option);
			foreach (var searchFile in searchFiles)
			{
				// Excel等開いた時のロックファイルは無視
				if (System.IO.Path.GetFileName(searchFile).IndexOf("~") == 0)
				{
					continue;
				}

				files.Add(searchFile);
			}

			return files;
		}

		/// <summary>
		/// CSVを読み込んでstring[][]の形で返却する
		/// 先頭データの行頭が//になっている場合は無視される
		/// 先頭データが空の場合は無視される
		/// エラー時はメッセージを出力してアプリを終了する
		/// </summary>
		/// <param name="path">csvパス</param>
		public static List<List<string>> ReadCsv(string path)
		{
			var list = new List<List<string>>();

			try
			{
				using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					// CSVはShift_JISで作成しているので変換する
					using (var stream = new StreamReader(fileStream, Encoding.GetEncoding(932)))
					{
						var text = stream.ReadToEnd();
						var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

						for (int i = 0; i < lines.Length; i++)
						{
							var values = new List<string>(lines[i].Split(','));

							var data = new List<string>();
							foreach (var value in values)
							{
								data.Add(value);
							}

							list.Add(data);
						}
					}
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
				Environment.Exit(1);
			}

			return list;
		}

		/// <summary>
		/// ファイルに書き出し
		/// 例外の場合はメッセージを出力してアプリを終了する
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="str">書き込むデータ</param>
		public static void WriteToFile(string path, string str)
		{
			try
			{
				File.WriteAllText(path, str, Encoding.UTF8);
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
				Environment.Exit(1);
			}
		}
	}
}
