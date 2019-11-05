using System;
using System.IO;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace ScExcelToJsonCell
{
	public class DstBook
	{
		/// <summary>フィールド名行</summary>
		private const int FieldNameRow = 2;

		/// <summary>フィールド型行</summary>
		private const int FieldTypeRow = 3;

		/// <summary>値開始行</summary>
		private const int ValueStartRow = 7;

		/// <summary>値開始列</summary>
		private const int ValueStartCol = 3;

		/// <summary>一時ディレクトリ</summary>
		private const string TempDir = "tmp/";

		/// <summary>JSON辞書</summary>
		private Dictionary<string, string> JsonDict { get; set; }

		/// <summary>キーカラム名</summary>
		private string KeyColName { get; set; }

		/// <summary>対象カラム名</summary>
		private string DstColName { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="jsonDict">JSON辞書</param>
		/// <param name="keyColName">キーカラム名</param>
		/// <param name="dstColName">対象カラム名</param>
		public DstBook(string filePath, Dictionary<string, string> jsonDict, string keyColName, string dstColName)
		{
			if ((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				Console.WriteLine("!!!" + filePath + "は読み取り専用です。Lockを取得してから実行して下さい。");
				Console.WriteLine("!!! 処理を中止します。");
				return;
			}

			JsonDict = jsonDict;
			KeyColName = keyColName;
			DstColName = dstColName;

			var dstFileName = Path.GetFileName(filePath);
			var dstFileDir = Path.GetDirectoryName(filePath);
			var tempFileDir = Path.Combine(dstFileDir, TempDir);
			var tempFilePath = Path.Combine(tempFileDir, dstFileName);

			Directory.CreateDirectory(tempFileDir);
			File.Copy(filePath, tempFilePath, true);
			Console.WriteLine("### " + filePath + "を一時フォルダにコピーしました。");

			var tempStream = new FileStream(tempFilePath, FileMode.Open);
			var book = WorkbookFactory.Create(tempStream);
			tempStream.Close();
			Console.WriteLine("### " + tempFilePath + "の内容を読み込みました。");

			for (int i = 0, iMax = book.NumberOfSheets; i < iMax; i++)
			{
				var sheet = book.GetSheetAt(i);
				WriteCell(sheet);
			}

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				book.Write(stream);
				Console.WriteLine("### " + filePath + "に書き込みました。");
			}

			File.Delete(tempFilePath);
			Console.WriteLine("### " + tempFilePath + "を削除しました。");
			Directory.Delete(tempFileDir);
			Console.WriteLine("### 一時フォルダを削除しました。");
		}

		/// <summary>
		/// セルに書き込む
		/// </summary>
		/// <param name="sheet">シート</param>
		private void WriteCell(ISheet sheet)
		{
			Console.WriteLine("### " + sheet.SheetName + "シートに書き込みを開始しました。");
			var endCol = sheet.GetRow(FieldNameRow).LastCellNum;

			int keyColIndex = -1;
			for (int col = ValueStartCol; col < endCol; col++)
			{
				if (sheet.GetRow(FieldNameRow).GetCell(col).StringCellValue == KeyColName)
				{
					keyColIndex = col;
				}
			}
			if (keyColIndex == -1) { return; }

			var dstColIndex = -1;
			for (int col = ValueStartCol; col < endCol; col++)
			{
				if (sheet.GetRow(FieldNameRow).GetCell(col).StringCellValue == DstColName)
				{
					dstColIndex = col;
				}
			}
			if (dstColIndex == -1) { return; }

			for (int row = ValueStartRow, rowMax = sheet.LastRowNum; row < rowMax; row++)
			{
				var rowData = sheet.GetRow(row);
				if (rowData == null) { continue; }

				var keyCell = rowData.GetCell(keyColIndex);
				if (keyCell == null) { continue; }

				var dstCell = rowData.GetCell(dstColIndex);
				if (dstCell == null) { continue; }

				var key = string.Empty;
				if (keyCell.CellType == CellType.Numeric)
				{
					key = keyCell.NumericCellValue.ToString();
				}
				else
				{
					key = keyCell.StringCellValue;
				}

				if (JsonDict.ContainsKey(key))
				{
					var val = JsonDict[key];
					dstCell.SetCellValue(val);
				}
			}
			Console.WriteLine("### " + sheet.SheetName + "シートに書き込みを完了しました。");
		}
	}
}
