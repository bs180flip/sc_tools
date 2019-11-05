/// <summary>
/// ログクラス
/// Noboru Matsunaga 
/// 2018/01/29
/// </summary>
using System.IO;
using System.Text;
using System.Collections.Generic;
using ExcelReader;
using System;
using NPOI.SS.UserModel;

namespace ScMstSqlGenerator
{
    public class FileCreate : XlsElement
    {
        // 行列定義
        private const int EnvCol = 1;
        private const int DefineCol = 2;
        private const int IdCol = 3;
        private const int DefineStartRow = 7;
        private const int StartSheetNum = 0;
        private const int EnvSetRow = 5;
        private const int ColNameRow = 2;

        // パス定義
        private const string Extension = "*.xlsx";

		// Define専用ファイルパス
		private const string DefineFilePath = "../other/";

		// EnumDefine専用ファイル
		private const string EnumDefineFileName = "EnumDefine.xlsx";

		// プランナー用Define専用ファイル
		private const string ReplaceDefineFileName = "Replace_define_name.xlsx";
		
		// テーブル名
		private const int TableNameCol = 3;
        private const int TableNameRow = 1;

        // マスタ名
        private string MstName = "";

        // DefineDict
        Dictionary<string, string> DefineDict = new Dictionary<string, string>();

		//EnvList
		List<string> EnvList = new List<string>();

		private StringBuilder df = new StringBuilder();
		private StringBuilder inputStr = new StringBuilder();
		private StringBuilder sb;
		private StringBuilder tmp;

		private string txtName;

		private static ISheet Sheet { get; set; }
        public static string Dir { get; set; }
        public static string FilePath { get; set; }
        public static int CreateFlag { get; set; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="dir">ディレクトリ</param>
        public static void Initialize(string dir, int createFlag)
        {
            Dir = dir;
            CreateFlag = createFlag;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">パス</param>
		public FileCreate(string path)
        {
            FilePath = path;
			EnvList.Add("test");
			EnvList.Add("dev");
			EnvList.Add("chk");
			EnvList.Add("plan");
			EnvList.Add("rev");
			EnvList.Add("prod");
		}

		/// <summary>
		/// ファイル生成
		/// </summary>
		public void Generate()
        {
            Generate(Dir);
		}

		/// <summary>
		/// ファイルを生成
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		private bool Generate(string directory)
        {
			if (Validate.IsFileOpen(FilePath)) { return false; }

			// EnumDefineをテキスト作成用にコピー
			File.Copy(directory + DefineFilePath + EnumDefineFileName, directory + EnumDefineFileName ,true);

			// ReplaceDefineをテキスト作成用にコピー
			File.Copy(directory + DefineFilePath + ReplaceDefineFileName, directory + ReplaceDefineFileName, true);

			//directory以下のマスタをすべて取得する
			string[] files = System.IO.Directory.GetFiles(directory, Extension, System.IO.SearchOption.AllDirectories);

            if (CreateFlag == 1) { txtName = "Define.txt";  Console.WriteLine(txtName + " 作成開始");}
            if (CreateFlag == 99) { txtName = "EnvSetOutput.txt"; Console.WriteLine(txtName + " 作成開始"); }

            //ファイルリスト読み込み
            foreach (string file in files)
            {
                var book = WorkbookFactory.Create(file);

                if (book == null) { continue; }

				MstName = book.GetSheetAt(0).GetRow(Const.ClassNameRow).GetCell(Const.ClassNameCol).ToString();

				//シート数を取得
				var sheetNum = book.NumberOfSheets;

                if (CreateFlag == 1)
                {
                    //シート数分生成
                    for (int i = StartSheetNum; i < sheetNum; i++)
                    {
                        var sheetSel = book.GetSheetAt(i);
                        InputStream(MstName, sheetSel);
                    }

					if (df.Length != 0)
					{
						inputStr.Append(df);
						df = new StringBuilder();
					}

				}

                if (CreateFlag == 99)
                {
                    //シート数分生成
                    for (int i = StartSheetNum; i < sheetNum; i++)
                    {
                        var sheetSel = book.GetSheetAt(i);

                        InputStreamEnv(sheetSel);
                    }

					if(sb.Length != 0)
					{
						inputStr.AppendLine("【" + book.GetSheetAt(0).GetRow(Const.ClassNameRow).GetCell(Const.ClassNameCol).ToString() + "】");
						inputStr.Append(sb);
					}
				}
			}

            // 中身があれば
            if (inputStr.ToString().Length != 0)
            {
                File.Delete(FilePath);
                File.WriteAllText(FilePath, inputStr.ToString(), new UTF8Encoding(false));
                Console.WriteLine(txtName + " 作成完了");
            }

            return true;

        }


        /// <summary>
        /// ファイルを生成
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        private void InputStream(string mstName, ISheet Sheet)
        {
            int row = DefineStartRow;
			tmp = new StringBuilder();
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

                // レコードENDチェック
                if (string.Equals(Sheet.GetRow(row).GetCell(EnvCol).ToString(), "END")) { break; }

                //　定義がnullであれば次のレコードへ
                if (Sheet.GetRow(row).GetCell(DefineCol) == null) {row++; continue; }

                //　定義がBlankであれば次のレコードへ
                if (string.IsNullOrEmpty(Sheet.GetRow(row).GetCell(DefineCol).ToString())) { row++; continue; }

                //　定義名に#が含まれていない
                if (!Sheet.GetRow(row).GetCell(DefineCol).ToString().Contains("#")) {

                    Log.CellError(mstName, Sheet, Sheet.GetRow(row).GetCell(DefineCol).ToString(), row, DefineCol, Log.Illegal);
                    row++;
                    continue;
                }

                //　定義名が重複
                if (DefineDict.ContainsKey(Sheet.GetRow(row).GetCell(DefineCol).ToString()))
                {
                    Log.CellError(mstName, Sheet, Sheet.GetRow(row).GetCell(DefineCol).ToString(), row, DefineCol, Log.Duplicate);
                    row++;
                    continue;
                }

                //Switchマスタのみ特殊対応
                if (Sheet.GetRow(TableNameRow).GetCell(TableNameCol).ToString() == "SWITCH_MST")
                {
                    DefineDict.Add(Sheet.GetRow(row).GetCell(DefineCol).ToString(), Sheet.GetRow(row).GetCell(IdCol+ 1).ToString() + ":" + Sheet.GetRow(row).GetCell(IdCol + 2).ToString());

					tmp.AppendLine(Sheet.GetRow(row).GetCell(DefineCol).ToString() + "|" + Sheet.GetRow(row).GetCell(IdCol + 1).ToString() + ":" + Sheet.GetRow(row).GetCell(IdCol + 2).ToString());
                    row++;
                    continue;
                }

                DefineDict.Add(Sheet.GetRow(row).GetCell(DefineCol).ToString(), Sheet.GetRow(row).GetCell(IdCol).ToString());

				tmp.AppendLine(Sheet.GetRow(row).GetCell(DefineCol).ToString() + "|" + Sheet.GetRow(row).GetCell(IdCol).ToString());
                row++;
            }

			if (tmp.Length != 0)
			{
				df.AppendLine("//" + mstName + " : " + Sheet.SheetName);
				df.Append(tmp);
				df.AppendLine();
			}
        }

        /// <summary>
        /// 環境設定一覧ファイルを生成
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        private void InputStreamEnv(ISheet Sheet)
        {
            int endCol = 0;
            int col = IdCol;
			tmp = new StringBuilder(); 
			sb  = new StringBuilder();

			while (true)
            {
                // ENDカラムを取得
                if (string.Equals(Sheet.GetRow(TableNameRow).GetCell(col).ToString(), "END"))
                {
                    endCol = col;
                    col = IdCol;
                    break;
                }

                col++;
            }

            while (col < endCol)
            {
                // 環境設定CellのNULL（セルがない状態)チェック
                // セルがない場合だとBlankと区別ができない
                // 後の環境設定チェックでも利用する為セル設定
                if (Sheet.GetRow(EnvSetRow).GetCell(col) == null)
                {
                    //セルを作成
                    Sheet.GetRow(EnvSetRow).CreateCell(col);
                }

                //　nullであれば次のカラムへ
                if (Sheet.GetRow(EnvSetRow).GetCell(col) == null) { col++; continue; }

                //　Blankであれば次のカラムへ
                if (string.IsNullOrEmpty(Sheet.GetRow(EnvSetRow).GetCell(col).ToString())) { col++; continue; }

				if(EnvList.Contains(Sheet.GetRow(EnvSetRow).GetCell(col).ToString()))
				{ 
					tmp.AppendLine(Sheet.GetRow(ColNameRow).GetCell(col).ToString() + "/" + Sheet.GetRow(EnvSetRow).GetCell(col).ToString());
				}
				col++;
            }

			if(tmp.Length != 0)
			{
				sb.AppendLine("シート名:" + Sheet.SheetName);
				sb.Append(tmp);
				sb.AppendLine();
			}
		}
    }
}


