using System;
using System.IO;
using System.Text;
using System.Collections;
using ExcelReader;
using System.Collections.Generic;

namespace ScExcelToCsv
{
	public class ScriptBook : XlsElement
	{
		/// <summary>
		/// マクロ情報用クラス
		/// </summary>
		public class MacroInfo
		{
			/// <summary>マクロ名</summary>
			public string MacroName = "";

			/// <summary>マクロ開始点かどうか</summary>
			public bool IsBegin = false;

			/// <summary>マクロ終了点かどうか</summary>
			public bool IsEnd = false;
		}

		/// <summary>マクロのキーワード</summary>
		private const string MacroKeyword = "マクロ_";

		/// <summary>引数名のフォーマット</summary>
		private const string ArgNameFormat = "arg{0}";

		/// <summary>スクリプト終了キーワード</summary>
		private const string ScriptEndKeyword = "END";

		/// <summary>ブック</summary>
		private XlsBook _book = null;

		/// <summary>ディレクトリ</summary>
		private string _dir = null;

		/// <summary>補足情報用ファイルディレクトリ</summary>
		private string _infoDir = null;

		/// <summary>マクロ辞書</summary>
		private MacroDict _macroDict = null;

		/// <summary>定義名辞書</summary>
		private DefineDict _defineDict = null;

		/// <summary>スクリプト用定義名辞書</summary>
		private ScriptDefineDict _scriptDefineDict = null;

		/// <summary>
		///   マクロ情報
		///   [key  ] シート名
		///   [value] マクロ情報群
		///           [key  ] セルインデックス
		///           [value] マクロ情報
		/// </summary>
		private Dictionary<string, Dictionary<int, MacroInfo>> _macroInfoList = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="dir">csv出力ディレクトリ</param>
		/// <param name="defineDict">グローバル定義データ辞書</param>
		/// <param name="scriptDefineDict">スクリプト事の定義データ辞書</param>
		/// <param name="dir">補足ファイル用出力ディレクトリ</param>
		public ScriptBook(string filePath, string dir, DefineDict defineDict, ScriptDefineDict scriptDefineDict, string infoDir)
		{
			_book = new XlsBook(filePath);
			_dir = dir + _book.BookName + "/";
			_defineDict = defineDict;
			_scriptDefineDict = scriptDefineDict;
			_macroInfoList = new Dictionary<string, Dictionary<int, MacroInfo>>();
			_infoDir = infoDir;
		}

		/// <summary>
		/// 全出力
		/// </summary>
		public void ExportAll()
		{
			foreach (var sheet in _book.Sheets)
			{
				ExportCsv(sheet);

				if (_infoDir != null)
				{
					ExportInfoCsv(sheet);
				}
			}
		}

		/// <summary>
		/// CSV出力
		/// </summary>
		/// <param name="sheet">シート</param>
		private void ExportCsv(XlsSheet sheet)
		{
			var sb = new StringBuilder();

			if (!Directory.Exists(_dir))//ディレクトリ無かったら生成
			{
				Directory.CreateDirectory(_dir);
			}

			for (int i = 0, iMax = sheet.Fields.Count; i < iMax; i++)//Countは縦
			{
				var row = sheet.Fields[i];//1行分

				if (row.Count == 0)
				{
					continue;
				}

				if (row[0] == ScriptEndKeyword)
				{
					// スクリプト終了
					break;
				}
				else if (row[0].IndexOf("//") == 0)
				{
					// コメント
					continue;
				}

				for (int j = 0, jMax = row.Count; j < jMax; j++)//1行の中のセルを処理
				{
					if (!string.IsNullOrEmpty(row[j]) && row[j][0] == '#')
					{
						//全体のdefineより、scriptのdefineの方が優先されて上書きされる

						if (!string.IsNullOrEmpty(_defineDict[row[j]]))
						{
							row[j] = _defineDict[row[j]];
						}
						if (!string.IsNullOrEmpty(_scriptDefineDict[row[j]]))
						{
							row[j] = _scriptDefineDict[row[j]];
						}
						//row[j] = _defineDict[row[j]];
					}
					if (row[j].Contains("\n"))//セル内に改行があった場合
					{
						row[j] = "\"" + row[j] + "\"";//セルのテキストを""で囲む
					}

					sb.Append(row[j] + ",");//カンマでつないでいく
				}

				sb.AppendLine();//改行をするためにAppendLine()
			}

			File.WriteAllText(_dir + sheet.SheetName + ".csv", sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// 補足用ファイルの出力
		/// 作成しなくてもOKですがバイナリコンバート時にあるとエラー時の出力が詳細に出せるようになります
		/// </summary>
		/// <param name="sheet">シート</param>
		private void ExportInfoCsv(XlsSheet sheet)
		{
			// マクロ情報がある場合は取得する
			Dictionary<int, MacroInfo> macroInfo = null;
			if (_macroInfoList.ContainsKey(sheet.SheetName))
			{
				macroInfo = _macroInfoList[sheet.SheetName];
			}

			var sb = new StringBuilder();

			if (!Directory.Exists(_infoDir))//ディレクトリ無かったら生成
			{
				Directory.CreateDirectory(_infoDir);
			}

			int cellRowCount = 2; // 1行目は見出しなので2から開始

			for (int i = 0, iMax = sheet.Fields.Count; i < iMax; i++)//Countは縦
			{
				var row = sheet.Fields[i];//1行分

				if (row.Count == 0)
				{
					continue;
				}

				if (row[0] == ScriptEndKeyword)
				{
					// スクリプト終了
					break;
				}
				else if (row[0].IndexOf("//") == 0)
				{
					// コメント
					continue;
				}

				if (i == 0)
				{
					// ヘッダー
					sb.Append("セル行数,マクロ名,");
				}
				else
				{
					// セル行数
					sb.Append(cellRowCount);
					sb.Append(",");

					// マクロ
					if (macroInfo != null && macroInfo.ContainsKey(i))
					{
						sb.Append(macroInfo[i].MacroName);

						if (macroInfo[i].IsEnd)
						{
							cellRowCount++;
						}
					}
					else
					{
						cellRowCount++;
					}
					sb.Append(",");
				}

				sb.AppendLine();//改行をするためにAppendLine()
			}

			File.WriteAllText(_infoDir + "/" + sheet.SheetName + ".info.csv", sb.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// book内のマクロをすべてコンバートする
		/// </summary>
		/// <param name="macroDict">マクロ辞書</param>
		public void ConvertMacros(MacroDict macroDict)
		{
			_macroDict = macroDict;

			foreach (var sheet in _book.Sheets)
			{
				ConvertMacro(sheet);
			}

		}

		/// <summary>
		/// sheet内のマクロをコンバート
		/// </summary>
		/// <param name="sheet">コンバートを掛けたいシート</param>
		private void ConvertMacro(XlsSheet sheet)
		{
			var macroInfos = new Dictionary<int, MacroInfo>();

			for (int i = 0; i < sheet.Fields.Count; i++)//Countは縦
			{
				var row = sheet.Fields[i];//1行分
				var args = new List<string>();

				if (row[0].IndexOf(MacroKeyword) == 0)
				{
					var macroName = row[0].Replace(MacroKeyword, "");//マクロ名を取得  //ここから確認
					for (int j = 1; j < row.Count; j++)//マクロの引数を取得する処理
					{
						args.Add(row[j]);
					}

					var macroFields = GetMacroFields(macroName, args);

					sheet.Fields.RemoveAt(i);//"マクロ"と出たフィールドを消す
					sheet.Fields.InsertRange(i, macroFields);//消したところから対応したマクロ辞書のシートをその行数分
															 //追加する処理

					// マクロ情報の取得
					for (int k = 0; k < macroFields.Count; k++)
					{
						if (macroInfos.ContainsKey(k + i))
						{
							macroInfos.Remove(k + i);
						}

						var info = new MacroInfo();
						if (k == 0)
						{
							info.IsBegin = true;
						}
						if (k == macroFields.Count - 1)
						{
							info.IsEnd = true;
						}
						info.MacroName = macroName;

						macroInfos.Add(k + i, info);
					}

					i--;
				}
			}

			_macroInfoList.Add(sheet.SheetName, macroInfos);
		}

		/// <summary>
		/// マクロ名を指定して引数を置き換えたマクロ情報を取得
		/// </summary>
		/// <param name="macroName">マクロ名</param>
		/// <param name="args">引数</param>
		/// <returns></returns>
		private List<List<string>> GetMacroFields(string macroName, List<string> args)
		{
			var macroValue = _macroDict[macroName];

			var macroFields = new List<List<string>>();

			foreach (var row in macroValue)
			{
				var macroRow = new List<string>();
				foreach (var cell in row)
				{
					macroRow.Add(cell);
				}
				macroFields.Add(macroRow);
			}

			for (int k = 0, kMax = macroFields.Count; k < kMax; k++)//Countは縦
			{
				var macroRow = macroFields[k];//1行分
				for (int m = 0, mMax = macroRow.Count; m < mMax; m++)//1行の中のセルを処理
				{
					var cellString = macroRow[m];

					for (int n = 0; n < args.Count; n++)
					{
						var argName = string.Format(ArgNameFormat, n + 1);
						if (cellString == argName)
						{
							cellString = args[n];
						}
					}
					macroRow[m] = cellString;
				}
				macroFields[k] = macroRow;//追加
			}
			return macroFields;
		}
	}
}
