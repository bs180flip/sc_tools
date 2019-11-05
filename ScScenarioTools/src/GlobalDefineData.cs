using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ScScenarioTools
{
	/// <summary>
	/// 全体で定義されたDefine.txtのデータ用クラス
	/// ExcelデータではないのでLocal版と別クラス化
	/// </summary>
	public class GlobalDefineData
	{
		/// <summary>ログ出力するかどうか</summary>
		private bool _isDumpConsole = true;
		public bool IsDumpConsole { set { _isDumpConsole = value; } }

		/// <summary>定義名の辞書</summary>
		private Dictionary<string, string> _defineDict = null;
		public Dictionary<string, string> DefineDict { get { return _defineDict; } }

		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="defineFile">定義ファイルパス</param>
		public int Read(string defineFile)
		{
			try
			{
				if (_isDumpConsole)
				{
					System.Console.WriteLine("[GlobalDefineFile]");
					System.Console.WriteLine("  " + defineFile);
				}

				_defineDict = new Dictionary<string, string>();

				var defineData = File.ReadAllLines(defineFile, Encoding.UTF8);
				foreach (var line in defineData)
				{
					if (string.IsNullOrEmpty(line)) { continue; }
					if (line.Contains("//")) { continue; }

					var values = line.Split('|');
					if (values == null || values.Length < 2) { continue; }

					var defineName = values[0];
					if (defineName[0] != '#') { continue; }

					var id = values[1];

					_defineDict.Add(defineName, id);
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
