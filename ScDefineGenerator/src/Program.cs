using ExcelReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScDefineGenerator
{
	class Program
	{
		private const string DefineBookFileName = "mst/Define.xlsx";
		private const string TextBookFileName = "mst/Text.xlsx";
		private const string InitialTextBookFileName = "mst/InitialText.xlsx";
		private const string EnumBookFileName = "other/EnumDefine.xlsx";

		private const string TextMasterClassName = "Text";
		private const string InitialTextMasterClassName = "InitialText";

		private const string GeneratePath = "output/";

		static void Main(string[] args)
		{
			// Debug用（ローカルパス）と本番用が存在する、Generate.batを参照
			var path = args[0];

			// outputフォルダ生成
			Directory.CreateDirectory(GeneratePath);

			// 各種スクリプトファイルを生成
			GenerateDefine(path);
			GenerateTextMaster(path);
			GenerateInitialTextMaster(path);
			GenerateEnumTypes(path);
		}

		private static void GenerateDefine(string path)
		{
			var book = new XlsBook(path + DefineBookFileName);

			foreach (var sh in book.Sheets)
			{
				var defineSheet = new DefineSheet(sh, GeneratePath);

				defineSheet.GenerateClass();
			}
		}

		private static void GenerateTextMaster(string path)
		{
			var book = new XlsBook(path + TextBookFileName);

			var textBook = new TextBook(book, GeneratePath, TextMasterClassName);

			textBook.GenerateClass();
			textBook.GenerateTextMasterClass();
		}

		private static void GenerateInitialTextMaster(string path)
		{
			var book = new XlsBook(path + InitialTextBookFileName);

			var initialTextBook = new TextBook(book, GeneratePath, InitialTextMasterClassName);

			initialTextBook.GenerateClass();
			initialTextBook.GenerateInitialTextMasterClass();
		}

		private static void GenerateEnumTypes(string path)
		{
			var book = new XlsBook(path + EnumBookFileName);

			var enumBook = new EnumBook(book, GeneratePath);
			enumBook.GenerateEnums();
		}
	}
}
