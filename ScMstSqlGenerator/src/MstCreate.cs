using System.IO;
using System.Collections.Generic;
using System;
using NPOI.SS.UserModel;
using Alim.Utility;
using System.Text;

namespace ScMstSqlGenerator
{
    public class MstCreate
    {
        private const int StartRow = 1;
        private const int StartCol = 3;

        private const string MstFilePath = "../mst/";
        private const string DocumentsPath = "../tools/Sc-Documents-Tools/";
        private const string EnvFile = "origin/Env.txt";
        private const string MstTemp = "mst_excel_templete.xlsx";
        private const string ReadFileNames = "a5m2_COLUMNS.csv";

        private const string TableNameKey = "TABLE_NAME";
        private const string ColumnNameKey = "COLUMN_NAME";
        private const string LogicalNameKey = "LOGICAL_NAME";
        private const string DataTypeKey = "DATA_TYPE";

        //終わり列初期値
        private static int EndCol = 0;

        //幅ポイント
        private static int WidthPoint = 300;

        //読み込んだCSVの中身のリスト
        static Dictionary<string, string> MstList = new Dictionary<string, string>();

        //読み込んだEnv.txtの中身
        private static List<string> FilesList = new List<string>();


        public ISheet Sheet { get; set; }
        public static string Dir { get; set; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="dir">ディレクトリ</param>
        public static void Initialize(string dir)
        {
            Dir = dir;

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MstCreate(string path) { }

        /// <summary>
        /// マスタファイル生成
        /// </summary>
        public void Create()
        {
            Create(Dir);

        }

        /// <summary>
        /// マスタファイル生成
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        private bool Create(string directory)
        {

            Console.WriteLine("-----MstCreate Start-----");

            // テンプレートファイル存在確認
            if(!IsFileExist(DocumentsPath + MstTemp)){ return false;}

            // csvファイル存在確認
            if (!IsFileExist(DocumentsPath + ReadFileNames)) { return false; }

            var mstPath = directory + MstFilePath;

            Console.WriteLine("マスタファイル 作成開始");
            Console.WriteLine();

            var mstBook = CreateBook(DocumentsPath + MstTemp);

            var mstFile = "";

            //csvファイル読み込み
            ReadCsv(mstPath);

            var MstName = StringUtility.SnakeToPascal(MstList[TableNameKey + 1]);

            // 文字MSTを削除
            MstName = MstName.Remove(MstName.Length - 3);

            mstFile = mstPath + MstName + ".xlsx";

            var sheetSel = mstBook.GetSheetAt(0);

            //シート名設定
            mstBook.SetSheetName(0, MstName);

            //セル書き込み
            WriteCell(sheetSel);

            //幅調整
            SetWidth(sheetSel);

            // ファイルが存在しているか
            if (File.Exists(mstFile))
            {
                // ファイルのチェック
                if (IsFileCheck(mstFile)) { return false; }
            }

            //ファイル作成
            using (var fs = new FileStream(mstFile, FileMode.Create)) { mstBook.Write(fs); }
            Console.WriteLine("マスタファイル 「" + MstName + "」作成完了");
            Console.WriteLine();

            //Env.txt更新
            EnvUpdate(MstName);

            Console.WriteLine("-----MstCreate End-----");
            Console.WriteLine();

            return true;

        }

        /// <summary>
        /// bookインスタンス作成
        /// </summary>
        /// <param name="Sheet">シート</param>
        private IWorkbook CreateBook(string path)
        {

            var book = WorkbookFactory.Create(path);

            if (book == null) { return null; }

            return book;
        }


        /// <summary>
        /// マスタCSVファイル読み込み
        /// </summary>
        /// <param name="mstPath">パス</param>
        private void ReadCsv(string mstPath)
        {

            string line;
            string[] words;
            int counter = 0;
            char[] delimiterChars = { ',' };

            StreamReader file = new StreamReader(DocumentsPath + ReadFileNames, Encoding.GetEncoding("shift_jis"));

            while ((line = file.ReadLine()) != null)
            {

                words = line.Split(delimiterChars);

                if (counter == 0)
                {
                    counter++;
                    continue;

                }

                //必要な列の情報をListへ保存
                MstList.Add(TableNameKey + counter, words[2]);
                MstList.Add(ColumnNameKey + counter, words[3]);
                MstList.Add(LogicalNameKey + counter, words[4]);
                MstList.Add(DataTypeKey + counter, ChangeString(words[8]));

                counter++;
            }

            EndCol = counter;
        }



        /// <summary>
        /// 書き込み
        /// </summary>
        /// <param name="Sheet">シート</param>
        //セル設定(文字列用)
        public static void WriteCell(ISheet sheet)
        {
            var tableNameRow = StartRow;
            var colNameRow = StartRow + 1;
            var logicNameRow = StartRow + 5;
            var dataTypeRow = StartRow + 2;
            var col = StartCol;
            var counter = 1;
            var endCol = EndCol;

            while (col < endCol)
            {

                if (MstList.ContainsKey(TableNameKey + counter))
                {

                    sheet.GetRow(tableNameRow).GetCell(col).SetCellValue(MstList[TableNameKey + counter]);
                    sheet.GetRow(colNameRow).GetCell(col).SetCellValue(MstList[ColumnNameKey + counter]);
                    sheet.GetRow(logicNameRow).GetCell(col).SetCellValue(MstList[LogicalNameKey + counter]);
                    sheet.GetRow(dataTypeRow).GetCell(col).SetCellValue(MstList[DataTypeKey + counter]);

                    col++;
                    counter++;
                    continue;
                }

            }

            sheet.GetRow(tableNameRow).GetCell(col).SetCellValue("END");
            sheet.GetRow(colNameRow).GetCell(col).SetCellValue("END");
            sheet.GetRow(logicNameRow).GetCell(col).SetCellValue("END");
            sheet.GetRow(dataTypeRow).GetCell(col).SetCellValue("END");

        }


        /// <summary>
        /// 文字列変換
        /// </summary>
        /// <param name="value">文字列</param>
        //セル設定(文字列用)
        private static string ChangeString(string value)
        {
            var dataType = value.Remove(0, 1);

            if (dataType.Equals("INT")
             | dataType.Equals("BIGINT")
             | dataType.Equals("TINYINT")
             | dataType.Equals("DECIMAL")
             | dataType.Equals("FLOAT"))
            {
                dataType = "INT";
            }
            else
            {
                dataType = "STR";
            }

            return dataType;

        }

        /// <summary>
        /// 幅調整
        /// </summary>
        /// <param name="sheet">シート</param>
        //セル設定(文字列用)
        private static void SetWidth(ISheet sheet)
        {
            var col = 3;
            var count = 1;
            var endCol = EndCol - 1;

            while (col < endCol)
            {
                sheet.SetColumnWidth(col, MstList[TableNameKey + count].Length * WidthPoint);
                col++;
            }

        }


        /// <summary>
        /// ファイルをチェックする
        /// </summary>
        /// <param name="path">検証したいファイルへのフルパス</param>
        /// <returns>チェック結果</returns>
        private bool IsFileCheck(string path)
        {
            // 読み込み専用かどうか
            if ((File.GetAttributes(path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                Log.FileError(path, Log.ReadOnly);
                return true;
            }

            FileStream stream = null;

            try
            {
                stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            }
            catch (IOException)
            {
                Log.FileError(path, Log.AlreadyOpen);
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();

                }
            }

            return false;
        }

        /// <summary>
        /// 必要ファイル存在確認
        /// </summary>
        /// <param name="path">検証したいファイルへのフルパス</param>
        /// <returns>チェック結果</returns>
        private static bool IsFileExist(string path)
        {

            if (!File.Exists(path))
            {
                Log.FileError(path, Log.Config);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Envファイル更新
        /// </summary>
        /// <param name="path">更新したいファイル</param>
        private static bool　EnvUpdate(string MstName)
        {
            Console.WriteLine(EnvFile + "を更新しますか？y or n");
            Console.WriteLine();

            var keyInput = false;
            do
            {
                //キー入力を取得
                ConsoleKeyInfo c = Console.ReadKey(true);

                if (c.Key == ConsoleKey.Y)
                {

                    if (!IsFileExist(EnvFile)) { return false; }

                    ReadFile(EnvFile);

                    if(FilesList.Contains(MstName + ".xlsx"))
                    {
                       Log.FileError(MstName + ".xlsx", Log.Duplicate);
                       keyInput = true;
                       break;
                    }

                    //Env.txt更新
                    using (StreamWriter sw = File.AppendText(EnvFile)) { sw.WriteLine(MstName + ".xlsx"); }
                    Console.WriteLine("「" + EnvFile + "」を更新しました。");
                    Console.WriteLine();
                    keyInput = true;
                }

                if (c.Key == ConsoleKey.N){keyInput = true;}

            } while (keyInput == false);

            return true;
        }

        /// <summary>
        /// Envファイル読み込み
        /// </summary>
        /// <param name="path">読み込みファイル</param>
        /// <returns>true</returns>
        private static bool ReadFile(string path)
        {

            int counter = 0;
            string line;

            StreamReader file = new StreamReader(path);

            while ((line = file.ReadLine()) != null)
            {

                if (line.Contains("//")) { counter++; continue; }

                if (line.Length < 12) { counter++; continue; }

                FilesList.Add(line);
                counter++;
            }

            file.Close();

            return true;
        }

    }

}


