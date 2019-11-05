/// <summary>
/// Validateクラス
/// Noboru Matsunaga
/// 2018/01/26
/// </summary>
using System.Text;
using System.IO;
using System;
using NPOI.SS.UserModel;

namespace ScMstSqlGenerator
{
    public class Validate
    {
		// ファイル削除数
		public static int fileDelCnt;

		// selectファイル削除数
		public static int selFileDelCnt;

        public static void Initialize()
        {
            fileDelCnt = 0;
            selFileDelCnt = 0;
        }

        /// <summary>
        /// ファイルが開いているかチェックする
        /// </summary>
        /// <param name="path">検証したいファイル</param>
        /// <returns>開いているかどうか</returns>
        public static bool IsFileOpen(string file)
        {
            FileStream stream = null;
            FileInfo fileInfo = new FileInfo(file);

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);

            }
            catch (IOException)
            {
                Log.FileError(file.ToString(), Log.AlreadyOpen);
                return true;
            }
            finally
            {
                if (stream != null) { stream.Close(); }
            }

            return false;
        }

        /// <summary>
        /// ログファイルが開いているかチェックする
        /// </summary>
        /// <param name="path">検証したいログファイル</param>
        /// <returns>開いているかどうか</returns>
        public static bool IsLogOpen(string file)
        {
            FileStream stream = null;
            FileInfo fileInfo = new FileInfo(file);

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);

            }
            catch (IOException)
            {
                Console.WriteLine(Log.LogPath + Log.LogFile +  Log.AlreadyOpen);

            }
            finally
            {
                if (stream != null) { stream.Close(); }

            }

            return false;
        }


        /// <summary>
        /// ファイル存在チェック
        /// </summary>
        /// <param name="book"></param>
        /// <param name="envType"></param>
        public static bool FileExistCheck(ISheet Sheet, string envType , string dir, string extention, int selectFlag = 0, string mstConstName ="")
        {
			var fileName = "";

			if (selectFlag == 0 && fileDelCnt == 0)
            {
				if (extention == ".sql")
				{
					fileName = envType + "_" + Sheet.GetRow(Const.ClassNameRow).GetCell(Const.ClassNameCol).ToString() + extention;
				}

				if (extention == ".json")
				{
					fileName = mstConstName + extention;
				}

				var file = Const.GeneratePath + envType + dir + fileName;

				// ファイルが存在する場合は削除処理
				if (File.Exists(file))
                {
					if (IsFileOpen(file)) { return true; }

                    File.Delete(file);
                }

				fileDelCnt += 1;

			}

			if (selectFlag == 1 && selFileDelCnt == 0)
            {
				var selectFileName = envType + "_" + "select" + "_" + Sheet.GetRow(Const.ClassNameRow).GetCell(Const.ClassNameCol).ToString() + extention;

				var file = Const.GeneratePath + envType + dir + selectFileName;

				// ファイルが存在する場合は削除処理
				if (File.Exists(file))
                {
                    if (IsFileOpen(file)) { return true; }

                    File.Delete(file);
                }

				selFileDelCnt += 1;

			}

			return false;

        }






    }
}
