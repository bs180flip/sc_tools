using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ExcelReader;

namespace ScWebYamlGenerator
{
    public class MstSheet : XlsElement
    {

        private const string KeyValueSeparator = ":";
        private const string CommentPrefix = "### ";
        private const string YamlTab = "    ";

        private const int ConstantAppendMstStartCol = 2;
        private const int ConstantAppendMstStartRow = 7;

        private const int ConstantAppendMstSummaryColFromEnd = 1;
        private const int ConstantAppendMstValueColFromEnd = 2;

        private XlsBook[] Msts { get; set; }
        private string ConstantName { get; set; }
        private string ConstantSummary { get; set; }

        public static string Dir { get; set; }

        private static int EndCol { get; set; }

        private StringBuilder sb { get; set; }

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
        public MstSheet(XlsBook[] books)
        {
            Msts = books;
            ConstantName = "application.common.mst";
            ConstantSummary = "";
            EndCol = ConstantAppendMstStartCol;

            sb = new StringBuilder();
        }

        /// <summary>
        /// 追加された参照マスタのyaml作成
        /// </summary>
        public void CreateYamlsFromMsts()
        {
            MakeConstantYamlFromMsts();
        }

        /// <summary>
        /// yamlの吐出し
        /// </summary>
        public void GenerateYamls()
        {
            var utf8Encoding = new UTF8Encoding(false);
            File.WriteAllText(Dir + ConstantName + ".yaml", sb.ToString(), utf8Encoding);
        }

        /// <summary>
        /// 追加された参照マスタのyaml作成
        /// </summary>
        /// <param name="directory">ディレクトリ</param>
        private void MakeConstantYamlFromMsts()
        {
            sb.AppendLine("###############################");
            sb.AppendLine("### マスタから抽出したタイプ定数 ###");
            sb.AppendLine("###############################");
            sb.AppendLine();

            foreach (var book in Msts)
            {
                foreach (var mstSheet in book.Sheets)
                {
                    // シートにあり判定なければ追加しない
                    if (!mstSheet.SheetName.Contains("タイプ定数あり")) { continue; }
                    
                    sb.AppendLine("###############################");
                    sb.AppendLine("### " + mstSheet.SheetName + "タイプ定数 ###");
                    sb.AppendLine("###############################");
                    sb.AppendLine();

                    var row = ConstantAppendMstStartRow;
                    var col = ConstantAppendMstStartCol;

                    while (true)
                    {
                        // 空文字か、rowの数で処理抜け
                        if (mstSheet.Fields.Count <= row || mstSheet.Fields[row][ConstantAppendMstStartCol] == "") { break; }

                        var checkStr = mstSheet.Fields[row][col];

                        sb.AppendLine(CommentPrefix + mstSheet.Fields[row][getEndCol(mstSheet) + ConstantAppendMstSummaryColFromEnd]);
                        sb.AppendLine(StringUtility.Camel2Snake(checkStr) + ":");

                        var index = 0;
                        col++;

                        while (true)
                        {
                            // カテゴリの名前が変わっていれば、break
                            if (checkStr != mstSheet.Fields[row][ConstantAppendMstStartCol]) { break; }
                         
                            // 値取得(なければ、暗黙index)
                            var value = mstSheet.Fields[row][getEndCol(mstSheet) + ConstantAppendMstValueColFromEnd];
                            if (value == "")
                            {
                                value = index.ToString();
                            }

                            sb.AppendLine(YamlTab + StringUtility.Camel2Snake(mstSheet.Fields[row][col]) + ": " + value);

                            index++;
                            row++;
                        }
                        sb.AppendLine();
                        col = ConstantAppendMstStartCol;
                    }
                }
                clearEndCol();
            }
        }
        /// <summary>
        /// シートの終了col取得
        /// </summary>
        /// <param name="sheet"></param>
        private int getEndCol(XlsSheet sheet)
        {
            // 設定済み
            if (EndCol > ConstantAppendMstStartCol) { return EndCol; }

            // end col調べ
            var endCol = ConstantAppendMstStartCol;
            while (true)
            {
                if (sheet.Fields[ConstantAppendMstStartRow - 1][endCol] == "END") { break; }
                endCol++;
            }
            EndCol = endCol;
            return EndCol;
        }

        /// <summary>
        /// 終了カラムのリセット
        /// </summary>
        private void clearEndCol()
        {
            EndCol = ConstantAppendMstStartCol;
        }
    }
}

