using System.Collections.Generic;
using System.IO;

namespace ExcelReader
{
	using NPOI.XSSF.UserModel;
	using NPOI.SS.UserModel;

	/// <summary>
	/// .xls ファイルのブックデータ基底クラス
	/// </summary>
	public abstract class XlsBookBase<T> : XlsElement
	{
		/// <summary>ブック名</summary>
		public string BookName { get; protected set; }

		/// <summary>シートのリスト</summary>
		public List<T> Sheets { get; protected set; }
	}

	/// <summary>
	/// .xls ファイルのブックデータ
	/// </summary>
	public class XlsBook : XlsBookBase<XlsSheet>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="xlsFileName">.xls ファイル名</param>
		public XlsBook(string xlsFileName)
		{
			// File.OpenReadだとオープン中のファイルを開けないのでFileStreamを使用する
			Stream stream = new FileStream(xlsFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var srcBook = WorkbookFactory.Create(stream);
			stream.Close();

			BookName = Path.GetFileNameWithoutExtension(xlsFileName);

			Create(srcBook);
		}

		/// <summary>
		/// ブックデータを生成
		/// </summary>
		/// <param name="book">ブック</param>
		private void Create(IWorkbook book)
		{
			Sheets = new List<XlsSheet>();

			for (int i = 0; i < book.NumberOfSheets; i++)
			{
				var sheet = book.GetSheetAt(i);

				// 保護されているシートは無視
				if (sheet.Protect) { continue; }

				Sheets.Add(new XlsSheet(sheet));
			}
		}
	}
}
