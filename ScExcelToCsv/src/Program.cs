using System;
using ExcelReader;

namespace ScExcelToCsv
{
	class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length < 5)
			{
				Console.WriteLine("引数が足りません");
				Console.WriteLine("第1引数：ファイル名");
				Console.WriteLine("第2引数：出力先ディレクトリ");
				Console.WriteLine("第3引数：マクロ設定エクセルディレクトリ");
				Console.WriteLine("第4引数：Defineファイルのパス");
				Console.WriteLine("第5引数：ScriptDefineファイルがあるディレクトリ");
				Console.WriteLine("第6引数：[オプション]補足用ファイル出力ディレクトリ(指定がなければ出力されません)");

				return;
			}
			var filePath = args[0];
			var directory = args[1];
			var macroDir = args[2];
			var definePath = args[3];
			var scriptDefinePath = args[4];
			var macroDict = new MacroDict(macroDir);
			var defineDict = new DefineDict(definePath);
			var scriptDefineDict = new ScriptDefineDict(scriptDefinePath);

			string infoDir = null;
			if (args.Length >= 6)
			{
				infoDir = args[5];
			}

			var targetBook = new ScriptBook(filePath, directory, defineDict, scriptDefineDict, infoDir);
			//マクロDictにマクロエクセルを読ませて、マクロ辞書を作る
			targetBook.ConvertMacros(macroDict);
			targetBook.ExportAll();
		}
	}
}
