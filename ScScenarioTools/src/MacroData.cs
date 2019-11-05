using System;
using System.IO;
using System.Collections.Generic;
using ExcelReader;

namespace ScScenarioTools
{
	/// <summary>
	/// マクロデータ用クラス
	/// xlsx空からマクロデータを読み込んで格納する
	/// </summary>
	public class MacroData
	{
		/// <summary>
		/// マクロ情報クラス
		/// </summary>
		private class MacroInfo
		{
			/// <summary>マクロのパス</summary>
			public string Path = "";

			/// <summary>マクロブック名</summary>
			public string BookName = "";

			/// <summary>マクロデータ</summary>
			public List<List<string>> Data = new List<List<string>>();
		}

		/// <summary>ログ出力するかどうか</summary>
		private bool _isDumpConsole = true;
		public bool IsDumpConsole { set { _isDumpConsole = value; } }

		/// <summary>マクロデータ格納</summary>
		private Dictionary<string, MacroInfo> _macroDict = new Dictionary<string, MacroInfo>();

		/// <summary>
		/// マクロ名からマクロ取得
		/// </summary>
		public List<List<string>> this[string name]
		{
			get
			{
				if (!_macroDict.ContainsKey(name))
				{
					return null;
				}
				return _macroDict[name].Data;
			}
		}

		/// <summary>
		/// マクロ名からパス取得
		/// </summary>
		/// <param name="macroName">マクロ名</param>
		public string FindMacroPath(string macroName)
		{
			if (_macroDict.ContainsKey(macroName))
			{
				return _macroDict[macroName].Path;
			}

			return "";
		}

		/// <summary>
		/// マクロ名からブック名取得
		/// </summary>
		/// <param name="macroName">マクロ名</param>
		public string FindMacroBookName(string macroName)
		{
			if (_macroDict.ContainsKey(macroName))
			{
				return _macroDict[macroName].BookName;
			}

			return "";
		}

		/// <summary>
		/// Excelデータから読み込み
		/// </summary>
		/// <param name="macroDir">マクロの定義Excelファイルが格納されたフォルダ</param>
		public int Read(string macroDir)
		{
			if (_isDumpConsole)
			{
				System.Console.WriteLine("[MacroFiles]");
			}

			try
			{
				//マクロディレクトリから、マクロエクセルを読み込んで、辞書を作る処理
				//fileNameから、マクロ名を取り出し、辞書を作る
				var fileNames = Directory.EnumerateFiles(macroDir, "*.xlsx", SearchOption.AllDirectories);
				foreach(var fileName in fileNames)
				{
					// Excelオープン中のロック用ファイルを対象外にする
					if (System.IO.Path.GetFileName(fileName).IndexOf("~") == 0)
					{
						continue;
					}

					if (_isDumpConsole)
					{
						System.Console.WriteLine("  " + fileName);
					}

					var xlsBook = new XlsBook(fileName);
					foreach(var sheet in xlsBook.Sheets)
					{
						var macroInfo = new MacroInfo();
						macroInfo.Path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(fileName)) + "/";
						macroInfo.BookName = System.IO.Path.GetFileNameWithoutExtension(fileName);

						if (_isDumpConsole)
						{
							System.Console.WriteLine("    " + sheet.SheetName);
						}

						for (int i = 1, iMax = sheet.Fields.Count; i < iMax; i++)//Countは縦
						{
							var row = sheet.Fields[i];

							if (row.Count == 0)
							{
								macroInfo.Data.Add(new List<string>());
								continue;
							}

							if (row[0].IndexOf("//") == 0)
							{
								// コメントを無視
								macroInfo.Data.Add(new List<string>());
								continue;
							}
							else if (row[0] == "END")
							{
								// マクロ終了
								break;
							}

							var column = new List<string>();
							for (int j = 0, jMax = row.Count; j < jMax; j++)//1行の中のセルを処理
							{
								column.Add(row[j]);
							}

							macroInfo.Data.Add(column);
						}

						_macroDict.Add(sheet.SheetName, macroInfo);
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
