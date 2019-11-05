/// <summary>
/// Mainクラス
/// Noboru Matsunaga 
/// 2018/01/12
/// </summary>
using System.IO;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using ExcelReader;

namespace ScMstSqlGenerator
{
    public class Program
    {
        private const string FileName = "Env.txt";
        private const string DefineFileName = "Define.txt";
        private const string DragListName = "DragList.txt";
        private const string DefineFilePath = "../origin/";
        private const string MstFolderName = "mst";
		private const string EnvSetOutputFilePath = "../EnvSetOutput.txt";
		private const string DelFilePath = "output/";

		//マスタ定数読み込みファイル
		private const string DocumentsPath = "../tools/Sc-Documents-Tools/";
		private const string MstConstFile  = "マスタファイル定数定義.xlsm";

		public static int CreateTextFlag = 9;
		public static int CreateFileFlag = 9;
		public static int ExecuteFlag = 9;

		public static List<string> Envtype = new List<string>();

        private static List<string> FilesList = new List<string>();

        private static List<string> DragList = new List<string>();

        public static Dictionary<string, string> DefineList = new Dictionary<string, string>();

		//マスタ定義辞書
		public static Dictionary<string, string> MstConstList = new Dictionary<string, string>();

		//最後のシート名
		public static string LastSheetName { get; set; }

		private static void Main(string[] args)
        {
			new Log();

			var path = args[0];

			if (args.Length > 1)
			{
				MstFileCreate(path);
			}
			else
			{
				Generate(path);
			}
		}

        /// <summary>
        /// Generate実行
        /// </summary>
        /// <param name="path"></param>
        private static bool Generate(string path)
        {

            Console.WriteLine("-----Generate Start-----");

            //ドラッグリスト読み込み
            ReadDragList(path);

            // Env読み込み
            ReadFile(path);

			// フラグが立っていた場合のみTextCreateを実行
			if (CreateTextFlag == 1) { TextCreate(path, DefineFilePath + DefineFileName, CreateTextFlag); }

			if (CreateTextFlag == 99) { TextCreate(path, EnvSetOutputFilePath, CreateTextFlag); return true; }

			// Json出力フラグが立っていた場合のみマスタファイル定数定義読込
			if (CreateFileFlag == 1 || CreateFileFlag == 2){ReadMstConst();}

			// フラグが立っていた場合のみ外部batの実行
			if (ExecuteFlag == 1) { ProcessExecute.ExternalBatExecute(Const.ExcelToJson); }

			// Define読み込み
			ReadDefine(path);

            Console.WriteLine("-----FILE作成処理開始-----");
            Console.WriteLine();

            // 複数の環境がある場合は複数環境分作成
            for (int x = 0; x < Envtype.Count; x++)
            {
                Console.WriteLine("出力環境:" + Envtype[x]);
                Console.WriteLine();

				// Json出力フラグが立っていた場合のみJsonフォルダ内削除
				if (CreateFileFlag == 1 || CreateFileFlag == 2)
				{
					var delPath = DelFilePath + Envtype[x] + Const.JsonDir;
					DirectoryInfo target = new DirectoryInfo(delPath);

					// Jsonフォルダ内のファイル消す
					foreach (FileInfo file in target.GetFiles())
					{
						file.Delete();
					}
				}

				//ファイルリスト読み込み
				foreach (string list in FilesList)
                {

                    if (!File.Exists(path + list))
                    {
                        Log.ConfigError(MstFolderName, list, Log.Config);
                    }

                    var book = WorkbookFactory.Create(path + list);

                    if (book == null) { continue; }

                    //シート数を取得
                    var sheetNum = book.NumberOfSheets;

					LastSheetName = book.GetSheetAt(sheetNum -1).SheetName;

					Console.WriteLine("読み込みファイル名:" + list);

					if (CreateFileFlag == 0 || CreateFileFlag == 2)
					{
						Validate.Initialize();

						SqlCreate.Initialize(path, Envtype[x]);
					}

					//シート数分SQL出力
					for (int i = Const.StartSheetNum; i < sheetNum; i++)
                    {
                        var sheetSel = book.GetSheetAt(i);

						if (CreateFileFlag == 0 || CreateFileFlag == 2)
						{
							var sqlCreate = new SqlCreate(list, sheetSel);

							sqlCreate.Generate();
						}
					}

					Console.WriteLine();

					if (CreateFileFlag == 1 || CreateFileFlag == 2)
					{
						Validate.Initialize();
						JsonCreate.Initialize(path, Envtype[x]);
					}

					//シート数分JSON出力
					for (int y = Const.StartSheetNum; y < sheetNum; y++)
					{
						var sheetSel = book.GetSheetAt(y);

						if (CreateFileFlag == 1 || CreateFileFlag == 2)
						{
							var jsonCreate = new JsonCreate(list, sheetSel);
							jsonCreate.Generate();
						}
					}

					Console.WriteLine();
                }
            }

            Console.WriteLine("-----FILE作成処理終了-----");
            Console.WriteLine("-----Generate End-----");
            Console.WriteLine();

			// フラグが立っていた場合のみ外部batの実行
			if (CreateFileFlag == 1) { ProcessExecute.ExternalBatExecute(Const.JsonCopy); }
			
			return true;
        }

        /// <summary>
        /// Define読み込み
        /// </summary>
        /// <param name="path">パス</param>
        private static void ReadDefine(string path)
        {

            if (!File.Exists(path + DefineFilePath + DefineFileName))
            {
                Log.ConfigError(DefineFilePath, DefineFileName, Log.Config);
            }

            char[] delimiterChars = { '|' };

            StreamReader file = new StreamReader(path + DefineFilePath + DefineFileName);

            int counter = 0;
            string line;
            string[] words;

            while ((line = file.ReadLine()) != null)
            {

                if (line.Contains("//")) { counter++; continue; }

                if (line.Length == 0) { counter++; continue; }

                words = line.Split(delimiterChars);

                if (DefineList.ContainsKey(words[0]))
                {
                    Log.FileError(words[0], Log.Duplicate);
                }

                DefineList.Add(words[0], words[1]);
                counter++;
            }

            file.Close();
        }


        /// <summary>
        /// ドラッグリスト読み込みとファイルリスト生成
        /// </summary>
        /// <param name="path">パス</param>
        private static void ReadDragList(string path)
        {

            if (File.Exists(path + DragListName))
            {
                int counter = 0;
                string line;

                StreamReader file = new StreamReader(path + DragListName);

                while ((line = file.ReadLine()) != null)
                {

                    if (line.Contains("//")) { counter++; continue; }

                    if (line.Length == 0) { counter++; continue; }

                    DragList.Add(line);
                }

                file.Close();
            }
        }

        /// <summary>
        /// マスタリスト読み込みとファイルリスト生成
        /// </summary>
        /// <param name="path">パス</param>
        private static bool ReadFile(string path)
        {
            if (!File.Exists(path + FileName))
            {
                Log.ConfigError(MstFolderName, FileName, Log.Config);
            }

            int counter = 0;
            string line;

            StreamReader file = new StreamReader(path + FileName);

            while ((line = file.ReadLine()) != null)
            {
				if (counter == 17)
				{
					if (DragList.Count > 0)
					{
						FilesList = DragList;
						return true;
					}
				}

				if (line.Contains("//")) { counter++; continue; }

                if (line.Length == 0) { counter++; continue; }

                if (counter == 1)
                {
                    CreateTextFlag = Int32.Parse(line);
                    counter++;
                    Console.WriteLine("TEXT作成フラグ:" + CreateTextFlag);
                    Console.WriteLine();
                    continue;
                }

				if (counter == 4)
				{
					CreateFileFlag = Int32.Parse(line);
					counter++;
					Console.WriteLine("SQL・JSON作成フラグ:" + CreateFileFlag);
					Console.WriteLine();
					continue;
				}

				if (counter == 7)
				{
					ExecuteFlag = Int32.Parse(line);
					counter++;
					Console.WriteLine("ExcelToJson実行フラグ:" + ExecuteFlag);
					Console.WriteLine();
					continue;
				}

				if (9 < counter && counter < 16)
                {
                    Envtype.Add(line);
                    counter++;
                    continue;
                }

                FilesList.Add(line);
                counter++;
            }

			if(FilesList.Count == 0)
			{
				Log.SelectError(Log.NotSelect);
			}

			file.Close();

            return true;
        }

        /// <summary>
        /// Textファイル生成
        /// </summary>
        private static void TextCreate(string path, string createfile, int CreateTextFlag)
        {
			var FileCreate = new FileCreate(path + createfile);

            FileCreate.Initialize(path, CreateTextFlag);

            FileCreate.Generate();

        }

        /// <summary>
        /// マスタファイル生成
        /// </summary>
        private static void MstFileCreate(string path)
        {
            var mstCreate = new MstCreate(path);

            MstCreate.Initialize(path);

            mstCreate.Create();
        }

		/// <summary>
		/// マスタファイル定数定義.xlsmを読み込み
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		private static bool ReadMstConst()
		{
			var row      = 14;
			var constCol = 3;
			var keyCol = 4;

			var book = new XlsBook(DocumentsPath + MstConstFile);

			if (book == null) { return false; }

			foreach (var sh in book.Sheets)
			{
				while (true)
				{
					// カラム名NULLチェック
					if (string.IsNullOrEmpty(sh.Fields[row][constCol])) { break; }

					// マスタ定数辞書に追加
					MstConstList.Add(sh.Fields[row][constCol].ToString(), sh.Fields[row][keyCol].ToString());
					row++;
				}
			}

			return true;
		}
	}
}
