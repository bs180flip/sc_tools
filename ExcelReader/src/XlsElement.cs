using System.Text;

namespace ExcelReader
{
	/// <summary>
	/// .xls の要素基底クラス
	/// </summary>
	public abstract class XlsElement
	{
		/// <summary>エンコーディング</summary>
		protected static readonly Encoding Encoding = Encoding.UTF8;
		/// <summary>クォーテーション</summary>
		protected const string Quote = "\"";
		/// <summary>タブ</summary>
		protected const string Tab = "\t";
		/// <summary>Excel ファイルの拡張子</summary>
		protected const string ExcelFileExt = "xlsx";
	}
}
