using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ExcelReader;

namespace ScWebYamlGenerator
{
    public class ConstantSheet : XlsElement
    {
        private const int ConstantRow = 2;
        private const int ConstantNameCol = 1;
        private const int ConstantSummaryCol = 3;

        private const int ConstStartRow = 3;
        private const int ConstInitCol = 0;
        private const int ConstCommentCol = 4;

        private const string KeyValueSeparator = ":";
        private const string CommentPrefix = "### ";
        private const string YamlTab = "    ";

        private XlsSheet Sheet { get; set; }
        private string ConstantName { get; set; }
        private string ConstantSummary { get; set; }

        public static string Dir { get; set; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="dir">ディレクトリ</param>
        public static void Initialize(string dir)
        {
            Dir = dir;

            Directory.CreateDirectory(Dir);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sheet">シート</param>
        public ConstantSheet(XlsSheet sheet)
        {
            Sheet = sheet;
            ConstantName = Sheet.Fields[ConstantRow][ConstantNameCol];
            ConstantSummary = Sheet.Fields[ConstantRow][ConstantSummaryCol];
        }

        /// <summary>
        /// クラスを生成
        /// </summary>
        public void GenerateYamls()
        {
            GenerateConstantYaml(Dir);
        }

        /// <summary>
        /// アクションクラスを生成
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        private void GenerateConstantYaml(string directory)
        {
            var sb = new StringBuilder();

            sb.AppendLine("###############################");
            sb.AppendLine("### " + ConstantSummary + " ###");
            sb.AppendLine("###############################");
            sb.AppendLine();

            var row = ConstStartRow;
            var col = ConstInitCol;

            while(true)
            {
                if (Sheet.Fields.Count <= row) { break; }

                sb.AppendLine(CommentPrefix + Sheet.Fields[row][col]);
                col++;

                var tab = "";
                var fixStr = "";

                while (true)
                {
                    var checkCol = col + 1;

                    if (ConstCommentCol <= checkCol)
                    {
                        sb.AppendLine(fixStr + " " + StringUtility.checkReservedWord(Sheet.Fields[row][col]) + Tab + Tab + "# " + Sheet.Fields[row][ConstCommentCol]);
                        row++;
                        col--;
                        fixStr = "";
                        if (Sheet.Fields.Count - 1 >= row && StringUtility.IsNUllOrEmpty(Sheet.Fields[row][col]))
                        {
                            col = ConstInitCol;
                            sb.AppendLine();
                            break;
                        }
                        else if (Sheet.Fields.Count <= row)
                        {
                            break;
                        }
                    }
                    else
                    {
                        fixStr = tab + StringUtility.checkReservedWord(Sheet.Fields[row][col]) + KeyValueSeparator;
                        if (ConstCommentCol > checkCol + 1)
                        {
                            sb.AppendLine(fixStr);
                            fixStr = "";
                            tab += YamlTab;
                        }
                        col++;
                    }
                }

                row++;
            }

            var utf8Encoding = new UTF8Encoding(false);
            File.WriteAllText(directory + ConstantName + ".yaml", sb.ToString(), utf8Encoding);
        }
    }
}

