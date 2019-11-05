using System.IO;
using ExcelReader;

namespace ScWebInterfaceGenerator
{
	public class Program
	{
		private const string ActionBookFileName = "SC通信仕様書(アクション).xlsm";
		private const string StructureBookFileName = "SC通信仕様書(ストラクチャ).xlsm";
		private const string ConstSrcDir = "source/conf/";

		private static readonly string[] ConstFileNames = new string[]
		{
			"ActionUrlConst.cs",
			"ConnectKeyConst.cs",
			"MstFileConst.cs",
		};
		private const string GenerateDir = "output/";
		private const string ConstCopyDir = "Const/";

		private const string ActionSheetPrefix = "R_";
		private const string StructureSheetPrefix = "T_";

		private static void Main(string[] args)
		{
			var path = args[0];

			GenerateAction(path);
			GenerateStructure(path);
			CopyConstant(path);
		}

		private static void GenerateAction(string path)
		{
			var book = new XlsBook(path + ActionBookFileName);

			ActionSheet.Initialize(GenerateDir);

			foreach (var sh in book.Sheets)
			{
				if (sh.SheetName.Contains(ActionSheetPrefix))
				{
					var actionSheet = new ActionSheet(sh);
					actionSheet.GenerateClasses();
				}
			}
		}

		private static void GenerateStructure(string path)
		{
			var book = new XlsBook(path + StructureBookFileName);

			StructureSheet.Initialize(GenerateDir);

			foreach (var sh in book.Sheets)
			{
				if (sh.SheetName.Contains(StructureSheetPrefix))
				{
					var structureSheet = new StructureSheet(sh);

					structureSheet.GenerateClass();
				}
			}
		}

		private static void CopyConstant(string path)
		{
			var constCopyPath = GenerateDir + ConstCopyDir;
			Directory.CreateDirectory(constCopyPath);

			foreach (var cf in ConstFileNames)
			{
				File.Copy(path + ConstSrcDir + cf, constCopyPath + cf, true);
			}
		}
	}
}
