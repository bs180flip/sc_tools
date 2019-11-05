/// <summary>
/// SQLファイル作成クラス
/// Noboru Matsunaga 
/// 2018/01/19
/// </summary>
using System.IO;
using System.Text;
using System.Collections.Generic;
using ExcelReader;
using System;
using NPOI.SS.UserModel;
using NPOI.SS.Util;


namespace ScMstSqlGenerator
{

	public enum Environment
	{
		none,
		dev,
		chk,
		plan,
		stage,
		prod,
		valid,
		rev
	}


	public class SqlCreate : XlsElement
	{
		//出力チェックセル設定値
		private const int OutputCheckRow = 0;
		private const int OutputCheckCol = 0;
		private const string OutputCheckString = "escape";
		private const string OutputSelectString = "select";

		//行列番号
		private const int FieldStartRow = 2;
		private const int FieldNameCol = 3;
		private const int EnvRow = 5;
		private const int EnvCol = 1;
		private const int FieldTypeRow = 3;
		private const int DelimiterTypeRow = 4;
		private const int ValueStartRow = 7;
		private const int ValueStartCol = 3;
		private const int DefineCol = 2;
		private const int SelectCol = 0;

		// ブック名
		private string bookName = "";

		//文字タイプ
		private const string StrType = "STR";
		private const string IntType = "INT";
		private const string LongType = "LONG";

		//デリミタタイプ
		private const string Colon = ":";
		private const string Comma = ",";

		// 現在DATETIMEの代替ファンクション
		private const string CurrentDatetimeFunc = "alim_now()";

		private int row = 0;
		private int col = 0;

		//スキップフラグ
		private int skipFlag = 0;

		//セレクトフラグ
		private int selectFlag = 0;

		//定義のDefine
		Dictionary<string, string> DefineList = Program.DefineList;

		//結合されたセル情報（行）
		SortedList<int, int> rowMergeList = new SortedList<int, int>();

		//結合されたセル情報（列）
		SortedList<int, int> colMergeList = new SortedList<int, int>();

		private static ISheet Sheet { get; set; }
		private string ClassName { get; set; }
		public static string Dir { get; set; }
		public static string EnvType { get; set; }
		public static string PrefixEnvtype { get; set; }

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="dir">ディレクトリ</param>
		/// <param name="envType">環境タイプ</param>
		public static void Initialize(string dir, string envType)
		{
			Dir = Const.GeneratePath;

			EnvType = envType;

			if (!string.IsNullOrEmpty(EnvType))
			{
				Dir += EnvType + Const.SqlDir;
				PrefixEnvtype = EnvType + "_";
			};

			Directory.CreateDirectory(Dir);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">シート</param>
		public SqlCreate(string createBookName, ISheet sheetSel)
		{
			var index = createBookName.Length - Const.exelExtention.Length;
			bookName = createBookName.Substring(0, index);

			Sheet = sheetSel;

			ClassName = Sheet.GetRow(Const.ClassNameRow).GetCell(Const.ClassNameCol).ToString();

			Log.ErrorFlag = false;

		}

		/// <summary>
		/// SQL生成
		/// </summary>
		public void Generate()
		{
			Generate(Dir);

		}

		/// <summary>
		/// Dictionary生成
		/// </summary>
		public static readonly Dictionary<string, Environment> EnvironmentLabel = new Dictionary<string, Environment>
		{
			{ "dev",   Environment.dev },
			{ "chk",   Environment.chk },
			{ "plan",  Environment.plan },
			{ "stage", Environment.stage },
			{ "prod",  Environment.prod },
			{ "valid", Environment.valid },
			{ "rev",   Environment.rev }

		};

		/// <summary>
		/// SQLを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private bool Generate(string directory)
		{
			var cellCheck = false;

			// セルの内容をチェック
			// ここでは1行目1列目の出力フラグをチェック
			if (Sheet.GetRow(OutputCheckRow).GetCell(OutputCheckCol) != null) { cellCheck = IsCellCheck(Sheet.GetRow(OutputCheckRow).GetCell(OutputCheckCol)) ? true : false; }

			// escapeが設定されていれば出力処理終了
			if (cellCheck) { return false; }

			var sb = new StringBuilder();
			var sql = new StringBuilder();
			var del = new StringBuilder();
			del.AppendLine("DELETE FROM " + ClassName + ";");

			var fileName = directory + PrefixEnvtype +  ClassName + Const.SqlExtention;
			var fieldName = "";
			var fieldType = "";
			var parmName = "";
			var mergeColLen = 0;
			var mergeRowLen = 1;
			var endCol = 0;
			var emptyFlag = 0;
			string mergeFieldName = "";
			string ins = "INSERT INTO " + ClassName + " (";
			string parm = "";

			row = FieldStartRow;
			col = FieldNameCol;

			//重複定義チェック
			CheckDuplicate(ValueStartRow);

			//結合セル情報取得
			GetMergeCellInfo();

			//レコードカラム名読み込み処理開始
			//Insert共通部作成
			while (true)
			{
				// カラム名NULLチェック(結合セル対応）
				if (string.IsNullOrEmpty(Sheet.GetRow(row).GetCell(col).ToString())) { col++; continue; }

				//パラメータ名（カラム名）とENDカラムを取得
				if (string.Equals(Sheet.GetRow(row).GetCell(col).ToString(), "END"))
				{
					// insert共通部生成完了
					parm = ins + sb.ToString() + "CREATEDATE,UPDATEDATE)" + " VALUES (";
					// ENDカラム設定
					endCol = col;
					col = FieldNameCol;
					sb.Length = 0;
					break;
				}

				// カラム環境設定チェック
				if (checkEnv(Sheet.GetRow(EnvRow).GetCell(col).ToString(), EnvRow, col)) { col++; continue; }

				parmName = Sheet.GetRow(row).GetCell(col).ToString();

				sb.Append("`" + parmName + "`");

				if (col != FieldNameCol | skipFlag == 0) { sb.Append(","); }

				skipFlag = 0;

				col++;

			}

			// 行数をValue部へ
			row = ValueStartRow;

			// 選択出力チェック
			while (true)
			{
				IsNullCreate(row, SelectCol);

				IsNullCreate(row, col);

				// レコードENDチェック
				if (string.Equals(Sheet.GetRow(row).GetCell(EnvCol).ToString(), "END")) { break; }

				if (string.Equals(Sheet.GetRow(row).GetCell(SelectCol).ToString(), OutputSelectString))
				{
					selectFlag = 1;
					break;
				}

				row++;
				continue;
			}

			Validate.FileExistCheck(Sheet, EnvType, Const.SqlDir, Const.SqlExtention, selectFlag);

			// 行数をValue部へ
			row = ValueStartRow;

			// Value行読み込み処理開始
			// Endまで処理継続
			while (true)
			{
				// レコードENDチェック
				if (string.Equals(Sheet.GetRow(row).GetCell(EnvCol).ToString(), "END")) { break; }

				// 選択出力の場合
				if (selectFlag == 1)
				{

					IsNullCreate(row, SelectCol);

					if (!string.Equals(Sheet.GetRow(row).GetCell(SelectCol).ToString(), OutputSelectString)) { row++; continue; }
				}

				// レコード環境設定チェック
				if (checkEnv(Sheet.GetRow(row).GetCell(EnvCol).ToString(), row, col)) { row++; continue; }

				// コメントアウトチェック
				if (checkCommentOut(Sheet.GetRow(row).GetCell(FieldNameCol).ToString())) { row++; continue; }

				//　ID列がnullであれば次のレコードへ
				if (string.IsNullOrEmpty(Sheet.GetRow(row).GetCell(FieldNameCol).ToString())) { row++; continue; }

				//Value部列読み込み開始
				while (col < endCol)
				{
					// カラム環境設定チェック
					if (checkEnv(Sheet.GetRow(EnvRow).GetCell(col).ToString(), EnvRow, col))
					{
						// 結合カラム分カラム数増やしcontinue
						if (colMergeList.ContainsKey(col))
						{
							mergeColLen = colMergeList[col];
							col = col + mergeColLen;
							continue;
						}
						col++;
						continue;
					}

					IsNullCreate(row, col);

					var delimiterType = Sheet.GetRow(DelimiterTypeRow).GetCell(col).ToString();

					//DELIMITER設定時処理
					//存在する場合は後述のリテラル(fieldType)は無視
					if (!string.IsNullOrEmpty(delimiterType))
					{
						switch (delimiterType)
						{
						case Comma:

							if (!colMergeList.ContainsKey(col)) { break; }

							mergeColLen = colMergeList[col];

							for (int i = 0; i < mergeColLen; i++)
							{
								IsNullCreate(row, col);

								if (string.IsNullOrEmpty(Sheet.GetRow(row).GetCell(col).ToString())) { col++; continue; }

								if (i != 0) { mergeFieldName += ","; }

								mergeFieldName += GetChangeFieldName(Sheet.GetRow(row).GetCell(col).ToString());

								col++;

							}

							mergeColLen = 0;

							col--;

							break;

						case Colon:

							if (Sheet.GetRow(row).GetCell(col).IsMergedCell)
							{
								//対象のセルが結合セルであれば1行しか読まないように設定
								mergeRowLen = 1;

							}
							else
							{
								//対象のセルが結合セルでなければ結合セル情報を取得して設定
								//結合セル情報がない場合は1行しか読まないよう設定
								mergeRowLen = rowMergeList.ContainsKey(row) ? rowMergeList[row] : 1;

							}

							mergeColLen = colMergeList.ContainsKey(col) ? colMergeList[col] : 1;

							var startCol = col;
							var startRow = row;

							for (int y = 0; y < mergeRowLen; y++)
							{
								if (y != 0) { mergeFieldName += ","; }

								for (int x = 0; x < mergeColLen; x++)
								{
									IsNullCreate(row, col);

									if (string.IsNullOrEmpty(Sheet.GetRow(row).GetCell(col).ToString())) { emptyFlag = 0; col++; continue; }

									if (x != 0) { mergeFieldName += ":"; }

									mergeFieldName += GetChangeFieldName(Sheet.GetRow(row).GetCell(col).ToString());
									emptyFlag = 1;

									col++;
								}

								//最後にコロンがついてしまう場合にコロン削除
								if (emptyFlag == 0 && mergeFieldName.EndsWith(",")) { mergeFieldName = mergeFieldName.Remove(mergeFieldName.Length - 1); }

								if (row < (startRow + mergeRowLen - 1))
								{
									col = startCol;
									row++;
								}
								else
								{
									row = startRow;
								}

							}

							mergeColLen = 0;

							mergeRowLen = 1;

							col--;

							break;

						default:

							Log.CellError(bookName, Sheet, Sheet.GetRow(DelimiterTypeRow).GetCell(col).ToString(), DelimiterTypeRow, col, Log.DelimNotUse);

							break;

						}

						fieldName = "'" + mergeFieldName + "'";
						mergeFieldName = "";

					}
					else
					{
						//DELIMITER設定がない場合の処理
						//リテラル(fieldType)により処理を分ける
						fieldType = GetFieldType(Sheet.GetRow(FieldTypeRow).GetCell(col).ToString());

						// Null設定
						if (string.IsNullOrEmpty(Sheet.GetRow(row).GetCell(col).ToString()))
						{
							if (fieldType == "string")
							{

								fieldName = "'" + Sheet.GetRow(row).GetCell(col).ToString() + "'";

							}
							else
							{

								fieldName = "NULL";
							}
						}
						else
						{

							if (fieldType == "string")
							{

								fieldName = "'" + GetChangeFieldName(Sheet.GetRow(row).GetCell(col).ToString()) + "'";

							}
							else
							{

								fieldName = GetChangeFieldName(Sheet.GetRow(row).GetCell(col).ToString()); ;
							}
						}
					}

					sb.Append(fieldName);

					if (col != FieldNameCol | skipFlag == 0) { sb.Append(","); }

					skipFlag = 0;
					fieldName = "";
					col++;

				}

				sql.AppendLine(parm + sb.ToString() + CurrentDatetimeFunc + "," + CurrentDatetimeFunc + ");");

				//列とStringBuilderno初期化して次の行へ
				col = FieldNameCol;
				sb.Length = 0;
				row++;
			}

			if (Log.ErrorFlag) { return false; }

			// 中身がない場合
			if (sql.ToString().Length == 0) { return false; }

			if (selectFlag == 1)
			{
				fileName = directory + PrefixEnvtype + OutputSelectString + "_" + ClassName + Const.SqlExtention;
			}

			// ファイルが存在しているか
			if (File.Exists(fileName))
			{
				//ファイルが開かれているか
				if (Validate.IsFileOpen(fileName)) { return false; }

				File.AppendAllText(fileName, sql.ToString(), new UTF8Encoding(false));
				Console.WriteLine("SQL出力シート名:" + Sheet.SheetName);

				return true;
			}

			if (selectFlag == 0)
			{

				File.WriteAllText(fileName, del.ToString() + sql.ToString(), new UTF8Encoding(false));
			}
			else
			{
				File.WriteAllText(fileName, sql.ToString(), new UTF8Encoding(false));
			}

			Console.WriteLine("SQL出力シート名:" + Sheet.SheetName);

			return true;

		}

		/// <summary>
		/// 結合されたセル情報取得
		/// </summary>
		private void GetMergeCellInfo()
		{

			//結合セル数を取得
			int size = Sheet.NumMergedRegions;

			for (int i = 0; i < size; i++)
			{
				CellRangeAddress range = Sheet.GetMergedRegion(i);

				int numCel = range.NumberOfCells;
				int firstRow = range.FirstRow;
				int firstColumn = range.FirstColumn;
				int lastRow = range.LastRow;
				int lastColumn = range.LastColumn;

				//2行目のカラム名のセル結合情報をリスト化(結合開始列,結合セル数）
				if (firstRow == 2) { colMergeList.Add(firstColumn, numCel); }

				//1列目のセル結合情報をリスト化(結合開始行,結合セル数）
				if (firstColumn == 0) { rowMergeList.Add(firstRow, numCel); }

			}

		}

		/// <summary>
		/// 重複している要素を抽出
		/// </summary>
		/// <param name="defineStartRow"></param>
		private void CheckDuplicate(int defineStartRow)
		{
			var row = defineStartRow;
			List<string> list = new List<string>();

			while (true)
			{

				// 環境設定CellのNULL（セルがない状態)チェック
				// セルがない場合だとBlankと区別ができない
				// 後の環境設定チェックでも利用する為セル設定
				if (Sheet.GetRow(row).GetCell(EnvCol) == null)
				{
					//セルを作成
					Sheet.GetRow(row).CreateCell(EnvCol);
				}

				// 環境設定列のENDチェック
				if (string.Equals(Sheet.GetRow(row).GetCell(EnvCol).ToString(), "END")) { break; }

				// 定義名CellのNULLチェック（セルがない状態)
				if (Sheet.GetRow(row).GetCell(DefineCol) == null) { row++; continue; }

				// 定義名CellのBlankチェック
				if (string.IsNullOrEmpty(Sheet.GetRow(row).GetCell(DefineCol).ToString())) { row++; continue; }

				// 定義名重複チェック
				if (list.Contains(Sheet.GetRow(row).GetCell(DefineCol).ToString()))
				{
					Log.CellError(bookName, Sheet, Sheet.GetRow(row).GetCell(DefineCol).ToString(), row, DefineCol, Log.Duplicate);
				}

				list.Add(Sheet.GetRow(row).GetCell(DefineCol).ToString());

				row++;
			}
		}

		/// <summary>
		/// データからフィールドのタイプを取得
		/// </summary>
		/// <param name="value">データ</param>
		/// <returns>フィールドのタイプ</returns>
		private string GetFieldType(string value)
		{
			var fieldType = value;

			if (!string.IsNullOrEmpty(fieldType))
			{
				switch (fieldType)
				{
				case IntType:
					return "int";

				case LongType:
					return "long";

				case StrType:
					return "string";
				}
			}

			return "string";
		}

		/// <summary>
		/// コメントアウト行かどうかチェックする
		/// </summary>
		/// <param name="value">データ</param>
		/// <returns>bool</returns
		private bool checkCommentOut(string value)
		{
			var ret = false;

			if (value.Contains("//") | value.Contains("/*")) { ret = true; }

			return ret;
		}

		/// <summary>
		/// 環境設定チェック(文字チェック)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns>bool</returns>
		private bool checkEnv(string value, int row, int col)
		{
			skipFlag = 0;

			bool envCheck = false;
			string newValue = "";

			// 空白セルは出力
			if (string.IsNullOrEmpty(value)) { return false; }

			// testは出力しない
			if (value.Contains("test")) { skipFlag = 1; return true; }

			string[] envTypeArray = value.Split(',');

			foreach (string envType in envTypeArray)
			{
				// 含まれていればフラグをtrueに
				if (envType.Contains(EnvType))
				{
					envCheck = true;
					newValue = envType;
				}
			}

			// フラグがtrueでなければ出力しない
			if (!envCheck) { skipFlag = 1; return true; }

			Environment env;

			EnvironmentLabel.TryGetValue(newValue, out env);

			// 存在しない環境設定名
			if (env == Environment.none)
			{
				Log.CellError(bookName, Sheet, newValue, row, col, Log.EnvNotFound);
			}

			return false;
		}

		/// <summary>
		/// 変換対象の文字列があれば文字列をIDに変換
		/// </summary>
		/// <param name="value">データ</param>
		/// <returns>フィールドのタイプ</returns>
		private string GetChangeFieldName(string value)
		{

			string changeFieldName = "";

			if (!value.StartsWith("#"))
			{
				changeFieldName = value;

				return changeFieldName;
			}

			if (DefineList.ContainsKey(value))
			{
				changeFieldName = DefineList[value];
			}
			else
			{
				Log.CellError(bookName, Sheet, value, row, col, Log.DefNotFound);
			}

			return changeFieldName;
		}

		/// <summary>
		/// 特定cellの値をチェックする
		/// </summary>
		/// <param name="cell">cell情報</param>
		/// <returns>bool</returns>
		private bool IsCellCheck(ICell cell)
		{
			if (string.IsNullOrEmpty(cell.ToString())) { return false; }

			if (cell.ToString() == OutputCheckString) { return true; }

			return false;
		}

		/// <summary>
		/// セルなしをチェックし、セルを生成
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		private void IsNullCreate(int row, int col)
		{
			// CellのNULL（セルがない状態)チェック
			// セルがない場合だとBlankと区別ができない為、セル作成
			if (Sheet.GetRow(row).GetCell(col) == null) { Sheet.GetRow(row).CreateCell(col); }
		}

	}

}
