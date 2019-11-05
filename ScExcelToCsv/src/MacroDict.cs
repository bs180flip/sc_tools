using System.IO;
using System.Collections.Generic;
using ExcelReader;

namespace ScExcelToCsv
{
	public class MacroDict
	{
		private Dictionary<string, List<List<string>>> _macroDict;
		public List<List<string>> this[string name]
		{
			get
			{
				if (!_macroDict.ContainsKey(name))
				{
					return null;
				}
				return _macroDict[name]; 
			}
		}
		public MacroDict(string macroDir)
		{
			_macroDict = new Dictionary<string, List<List<string>>>();
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

				var xlsBook = new XlsBook(fileName);
				foreach(var sheet in xlsBook.Sheets)
				{
					var list = new List<List<string>>();

					for (int i = 1, iMax = sheet.Fields.Count; i < iMax; i++)//Countは縦
					{
						var row = sheet.Fields[i];

						if (row.Count == 0)
						{
							continue;
						}

						if (row[0].IndexOf("//") == 0)
						{
							// コメントを無視
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

						list.Add(column);
					}

					_macroDict.Add(sheet.SheetName, list);
				}
			}
		}
	}

}
