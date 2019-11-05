/// <summary>
/// ログクラス
/// Noboru Matsunaga
/// 2018/01/26
/// </summary>
using System.Text;
using System.IO;
using System;
using NPOI.SS.UserModel;

namespace ScMstSqlGenerator
{
    /// <summary>
    /// ログクラス
    /// </summary>
    public class Log
	{
        public const string BookPath   = "tmp/";

        public const string LogFile　  = "error.txt";

        public const string LogPath     = "logs/";

        public const string Duplicate   = "は重複しています。";

        public const string NotFound    = "は存在しません。";

		public const string KeyNotFound = "というマスタ定義Keyは存在しません。";

		public const string DefNotFound = "は定義名に存在しません。";

        public const string DelimNotUse = "はデリミタに使用できません。";

        public const string EnvNotFound = "は環境設定名に存在しません。";

        public const string AlreadyOpen = "は開かれている為,更新できません";

        public const string Illegal     = "は定義名に#が含まれていません。";

        public const string Config      = "を配置してください。処理は中止します。";

        public const string ReadOnly    = "は読み取り専用の為アクセスできません。";

		public const string NotSelect = "ファイルが選択されていません。ファイルを選択してください。";

		public static bool ErrorFlag = false;

        private static DateTime nowTime;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Log()
        {

            if (!Directory.Exists(LogPath)){ Directory.CreateDirectory(LogPath);File.Create(LogPath + LogFile).Close();}

        }

		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="value">値</param>
		/// <param name="erorrMsg">エラーメッセージ</param>
		/// <return>false</return>
		public static bool FileError(string value, string erorrMsg)
		{
            var sw = CreateStream();

            Console.WriteLine(nowTime + " 「" + value + "」" + erorrMsg);
            Console.WriteLine();

            if (sw == null) { return false; }

            //エラー内容を書き込む
            sw.WriteLine(nowTime + " 「" + value + "」" + erorrMsg);

            //閉じる
            sw.Close();

            return false;
        }

		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="key">key</param>
		/// <return>false</return>
		public static bool KeyError(string key, string erorrMsg)
		{
			var sw = CreateStream();

			Console.WriteLine(nowTime + " " + "「" + key + "」" + erorrMsg);
			Console.WriteLine();

			if (sw == null) { return false; }

			//エラー内容を書き込む
			sw.WriteLine(nowTime + " " + "「" + key + "」" + erorrMsg);

			//閉じる
			sw.Close();

			return false;
		}

		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="Sheet">シート名</param>
		/// <param name="value">値</param>
		/// <param name="row">行</param>
		/// <param name="col">列</param>
		/// <return>false</return>
		public static bool CellError(string bookName, ISheet Sheet, string value, int row, int col, string erorrMsg)
        {
            var sw = CreateStream();

            Console.WriteLine(nowTime + " " + bookName + " " + Sheet.SheetName + " " + (row+1) + "行目" + "-" + col + "列目の「" + value + "」" + erorrMsg);
            Console.WriteLine();

            if (sw == null) { return false; }

            //エラー内容を書き込む
            sw.WriteLine(nowTime + " " + bookName + " " + Sheet.SheetName + " " + (row+1) + "行目" + "-" + col + "列目の「" + value + "」" + erorrMsg);

            //閉じる
            sw.Close();

            return false;
        }



        /// <summary>
        /// 配置エラー（中断する）
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="value">値</param>
        /// <param name="erorrMsg">エラーメッセージ</param>
        /// <return>false</return>
        public static bool ConfigError(string path, string value, string erorrMsg)
        {

            var sw = CreateStream();

            Console.WriteLine(nowTime + " 「" + path + "」フォルダに「" + value + "」" + erorrMsg);
            Console.WriteLine();

            if (sw == null) { System.Environment.Exit(0); }

            //エラー内容を書き込む
            sw.WriteLine(nowTime + " 「" + path + "」フォルダに「" + value + "」" + erorrMsg);

            //閉じる
            sw.Close();

            System.Environment.Exit(0);

            return false;
        }

		/// <summary>
		/// セレクトエラー（中断する）
		/// </summary>
		/// <param name="erorrMsg">エラーメッセージ</param>
		/// <return>false</return>
		public static bool SelectError(string erorrMsg)
		{
			var sw = CreateStream();

			Console.WriteLine(erorrMsg);
			Console.WriteLine();

			if (sw == null) { System.Environment.Exit(0); }

			//エラー内容を書き込む
			sw.WriteLine(nowTime + " " + erorrMsg);

			//閉じる
			sw.Close();

			System.Environment.Exit(0);

			return false;
		}

		/// <summary>
		/// StreamWriter作成
		/// </summary>
		/// <return>StreamWriter</return>
		public static StreamWriter CreateStream()
        {

            nowTime = DateTime.Now;

            ErrorFlag = true;

            if (Validate.IsLogOpen(LogPath + LogFile)) { return null; }

            System.IO.StreamWriter sw = new System.IO.StreamWriter(LogPath + LogFile, true, Encoding.UTF8);

            return sw;
        }

    }
}
