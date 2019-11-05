using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ExcelReader;

namespace ScScenarioTools
{
	/// <summary>
	/// スクリプト用のローカル定義データクラス
	/// </summary>
	public class LocalDefineData
	{
		/// <summary>ログ出力するかどうか</summary>
		private bool _isDumpConsole = true;
		public bool IsDumpConsole { set { _isDumpConsole = value; } }

		/// <summary>定義名の辞書</summary>
		private Dictionary<string, string> _defineDict = null;
		public Dictionary<string, string> DefineDict { get { return _defineDict; } }

		/// <summary>
		/// Excelデータ読み込み
		/// </summary>
		/// <param name="defineDir">定義ファイルのフォルダ</param>
		public int Read(string defineDir)
		{
			try
			{
				var scriptDefinSheet = new List<List<string>>();

				_defineDict = new Dictionary<string, string>();

				if (_isDumpConsole)
				{
					System.Console.WriteLine("[LocalDefineFiles]");
				}

				var files = Directory.EnumerateFiles(defineDir, "*.xlsx", SearchOption.AllDirectories);
				foreach (var file in files)
				{
					// Excelオープン中のロック用ファイルを対象外にする
					if (System.IO.Path.GetFileName(file).IndexOf("~") == 0)
					{
						continue;
					}

					if (_isDumpConsole)
					{
						System.Console.WriteLine("  " + file);
					}

					var xlsBook = new XlsBook(file);
					foreach (var sheet in xlsBook.Sheets)
					{
						for (int i = 1, iMax = sheet.Fields.Count; i < iMax; i++) // Countは縦  1行目（0）は除く
						{
							// 1行分
							var row = sheet.Fields[i];

							if (row.Count < 2)
							{
								continue;
							}
							else if (row[0].IndexOf("//") == 0)
							{
								// コメントを無視
								continue;
							}
							else if (row[0] == "END")
							{
								// 定義終了
								break;
							}

							if (_isDumpConsole)
							{
								System.Console.WriteLine("    " + row[0] + ": " + row[1]);
							}

							_defineDict.Add(row[0], row[1]);
						}
					}
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
				return 1;
			}

			return 0;
		}
	}
}
