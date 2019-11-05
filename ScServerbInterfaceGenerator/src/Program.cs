using System.IO;
using ExcelReader;

namespace ScServerInterfaceGenerator
{
	public class Program
	{
		private const string ActionBookFileName = "SCマルチ通信仕様書(アクション).xlsm";
		private const string StructureBookFileName = "SCマルチ通信仕様書(ストラクチャ).xlsm";
		private const string ConstSrcDir = "source/server/conf/";

		private static readonly string[] ConstFileNames = new string[]
		{
            "MultiActionUrlConst.cs",
            "MultiConnectKeyConst.cs",
			//"MstFileConst.cs",
		};
		private const string GenerateDir = "output/Server/";
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
