using System.Collections.Generic;

namespace ExcelReader
{
	/// <summary>
	/// JSONブック
	/// </summary>
	public class XlsJsonBook : XlsBookBase<XlsJsonSheet>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="xlsFileName">.xls ファイル名</param>
		public XlsJsonBook(string xlsFileName) : this(new XlsBook(xlsFileName))
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="book">xlsブックデータ</param>
		private XlsJsonBook(XlsBook book)
		{
			BookName = book.BookName;
			Sheets = new List<XlsJsonSheet>();

			foreach (var sheet in book.Sheets)
			{
				Sheets.Add(new XlsJsonSheet(sheet));
			}
		}

		/// <summary>
		/// .json ファイルを出力
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		public virtual void Export(string directory)
		{
			foreach (var sheet in Sheets)
			{
				sheet.ExportJson(directory);
			}
		}
	}
}
