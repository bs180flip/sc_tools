using ExcelReader;
using System.IO;
using System;

namespace ScChronosYmlGenerator
{
    public class Program
	{
		private const string ActionBookFileName = "SC通信仕様書(アクション).xlsm";
		private const string StructureBookFileName = "SC通信仕様書(ストラクチャ).xlsm";

        private const string MultiActionBookFileName = "SCマルチ通信仕様書(アクション).xlsm";
        private const string MultiStructureBookFileName = "SCマルチ通信仕様書(ストラクチャ).xlsm";

		private const string GenerateDir = "output/";
        private const string MultiGenerateDir = "output/multi/";

        private const string ActionSheetPrefix = "R_";
		private const string StructureSheetPrefix = "T_";

		private static void Main(string[] args)
		{
			var path = args[0];

            // gme
            CreateStructureYml(path, false);
            GenerateAction(path, false);

            // multi
            CreateStructureYml(path + "SCマルチ通信仕様書/", true);
            GenerateAction(path + "SCマルチ通信仕様書/", true);
        }

        private static void CreateStructureYml(string path, bool isMulti)
        {
            var fileName = StructureBookFileName;
            StructureSheet.Initialize(GenerateDir);
            if (isMulti)
            {
                fileName = MultiStructureBookFileName;
                StructureSheet.Initialize(MultiGenerateDir);
            }
            var book = new XlsBook(path + fileName);
            foreach (var sh in book.Sheets)
            {
                if (sh.SheetName.Contains(StructureSheetPrefix))
                {
                    var structureSheet = new StructureSheet(sh);
                    structureSheet.GenerateClass();
                }
            }
        }

		private static void GenerateAction(string path, bool isMulti)
		{
            var fileName = ActionBookFileName;
            ActionSheet.Initialize(GenerateDir);
            if (isMulti)
            {
                fileName = MultiActionBookFileName;
                ActionSheet.Initialize(MultiGenerateDir);
            }
            var book = new XlsBook(path + fileName);

			foreach (var sh in book.Sheets)
			{
				if (sh.SheetName.Contains(ActionSheetPrefix))
				{
					var actionSheet = new ActionSheet(sh);
                    actionSheet.GenerateClasses();
				}
			}
		}
	}
}
