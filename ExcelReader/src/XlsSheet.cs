using System.Collections.Generic;

namespace ExcelReader
{
	using NPOI.SS.UserModel;

	/// <summary>
	/// .xls ファイルのシートデータ基底クラス
	/// </summary>
	public abstract class XlsSheetBase : XlsElement
	{
		/// <summary>シート名</summary>
		public string SheetName { get; protected set; }

		/// <summary>列の総数</summary>
		public int ColumnCount { get; protected set; }

		/// <summary>行の総数</summary>
		public int RowCount { get; protected set; }

		/// <summary>フィールド</summary>
		public abstract List<List<string>> Fields { get; protected set; }
	}

	/// <summary>
	/// .xls ファイルのシートデータ
	/// </summary>
	public class XlsSheet : XlsSheetBase
	{
		/// <summary>フィールド</summary>
		public override List<List<string>> Fields { get; protected set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sheet">シート</param>
		public XlsSheet(ISheet sheet)
		{
			Create(sheet);
		}

		/// <summary>
		/// シートデータを生成
		/// </summary>
		/// <param name="sheet">シート</param>
		private void Create(ISheet sheet)
		{
			var rowCount = sheet.LastRowNum + 1;

			SheetName = sheet.SheetName;
			ColumnCount = 1;
			Fields = new List<List<string>>();

			for (var row = 0; row < rowCount; row++)
			{
				var rowData = sheet.GetRow(row);

				if (rowData == null || rowData.ZeroHeight) { continue; }

				if (ColumnCount < rowData.LastCellNum)
				{
					ColumnCount = rowData.LastCellNum;
				}
			}

			for (var row = 0; row < rowCount; row++)
			{
				var rowData = sheet.GetRow(row);

				if (rowData == null || rowData.ZeroHeight) { continue; }

				var rowValue = new List<string>();

				for (var col = 0; col < ColumnCount; col++)
				{
					var cellData = rowData.GetCell(col);
					var cellValue = "";

					if (cellData != null)
					{
						if (cellData.CellType == CellType.Numeric)
						{
							cellValue = cellData.NumericCellValue.ToString();
						}
						else
						{
							cellData.SetCellType(CellType.String);
							cellValue = cellData.StringCellValue;
						}
					}

					rowValue.Add(cellValue);
				}

				Fields.Add(rowValue);
			}

			RowCount = Fields.Count;
		}
	}


	/// <summary>
	/// 変数名列を持ったシートデータ
	/// </summary>
	public abstract class HasFieldNameSheetBase : XlsSheetBase
	{
		/// <summary>変数名行番号</summary>
		protected abstract int FieldNameRowIndex { get; }

		/// <summary>変数名リスト</summary>
		public List<string> FieldNames { get; protected set; }

		/// <summary>
		/// 指定のレコードの値の別のカラムの値を取得
		/// </summary>
		/// <param name="srcField">検索元の値</param>
		/// <param name="srcFieldName">検索元の変数名</param>
		/// <param name="dstFieldName">検索先の変数名</param>
		/// <returns>検索先の値</returns>
		public string GetFieldByOtherFieldName(string srcField, string srcFieldName, string dstFieldName)
		{
			int srcFieldNameIndex = FieldNames.IndexOf(srcFieldName);

			if (srcFieldNameIndex != -1)
			{
				var dstRow = Fields.Find(val => val[srcFieldNameIndex] == srcField);

				if (dstRow != null)
				{
					int dstFieldNameIndex = FieldNames.IndexOf(dstFieldName);

					if (dstFieldNameIndex != -1)
					{
						return dstRow[dstFieldNameIndex];
					}
				}
			}

			return null;
		}
	}
}
