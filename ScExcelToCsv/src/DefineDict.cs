using System.IO;
using System.Text;
using System.Collections.Generic;
using ExcelReader;

namespace ScExcelToCsv
{
	public class DefineDict
	{
		/// <summary>定義名の辞書</summary>
		private Dictionary<string, string> _defineDict = null;

		/// <summary>
		/// インデクサー
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string this[string key]
		{
			get
			{
				if (_defineDict.TryGetValue(key, out string val))
				{
					return val;
				}
				return null;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dir">ディレクトリ</param>
		public DefineDict(string definePath)
		{
			_defineDict = new Dictionary<string, string>();

			//var filePath = Path.Combine(dir, "../origin/Define.txt");
			//var defineData = File.ReadAllLines(filePath, Encoding.UTF8);
			var defineData = File.ReadAllLines(definePath, Encoding.UTF8);

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
	}
}
