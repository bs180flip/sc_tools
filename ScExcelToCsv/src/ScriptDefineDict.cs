using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ExcelReader;

namespace ScExcelToCsv
{
	public class ScriptDefineDict
	{
		/// <summary>定義名の辞書</summary>
		private Dictionary<string, string> _scriptDefineDict = null;

		/// <summary>
		/// インデクサー
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string this[string key]
		{
			get
			{
				if (_scriptDefineDict.TryGetValue(key, out string val))
				{
					return val;
				}
				return null;
			}
		}

		public ScriptDefineDict(string defineDir)
		{
			var scriptDefinSheet = new List<List<string>>();
			//var scriptDefinSheet = new List<List<string>>>();

			_scriptDefineDict = new Dictionary<string, string>();

			var fileNames = Directory.EnumerateFiles(defineDir, "*.xlsx", SearchOption.AllDirectories);
			foreach (var fileName in fileNames)
			{
				// Excelオープン中のロック用ファイルを対象外にする
				if (System.IO.Path.GetFileName(fileName).IndexOf("~") == 0)
				{
					continue;
				}

				var xlsBook = new XlsBook(fileName);
				foreach (var sheet in xlsBook.Sheets)
				{
					for (int i = 1, iMax = sheet.Fields.Count; i < iMax; i++)//Countは縦  1行目（0）は除く
					{
						var row = sheet.Fields[i];//1行分

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
							// 定義終了
							break;
						}

						_scriptDefineDict.Add(row[0], row[1]);
					}
				}
			}

			// Note: 出力制御のためいったんコメントアウト
			// foreach (var kvp in _scriptDefineDict)
			// {
			// 	Console.WriteLine(kvp.Key + " : " + kvp.Value);
			// }
		}
	}
}
