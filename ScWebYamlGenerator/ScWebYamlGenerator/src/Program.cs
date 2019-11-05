using System.IO;
using ExcelReader;

namespace ScWebYamlGenerator
{
    public class Program
    {
        private const string ConstantBookFileName = "SC共通定数定義書.xlsm";

        private const string ConstantTextMstFileName = "Text.xlsx";

        private const string GeneratePath = "output/";

        private static void Main(string[] args)
        {
            var path = args[0];

            GenerateConstant(path);

            var mstPath = args[1];

            GenerateConstantFromMst(mstPath);
        }

        private static void GenerateConstant(string path)
        {
            var book = new XlsBook(path + ConstantBookFileName);

            ConstantSheet.Initialize(GeneratePath);

            foreach (var sh in book.Sheets)
            {
                var actionSheet = new ConstantSheet(sh);
                actionSheet.GenerateYamls();
            }
        }

        private static void GenerateConstantFromMst(string path)
        {
            XlsBook[] books = new XlsBook[] { new XlsBook(path + ConstantTextMstFileName) };

            MstSheet.Initialize(GeneratePath);

            var mstSheet = new MstSheet(books);
            mstSheet.CreateYamlsFromMsts();
            mstSheet.GenerateYamls();
        }
    }
}
