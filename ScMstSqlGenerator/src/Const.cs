/// <summary>
/// 定数
/// </summary>

public static class Const
{

    /// <summary> 出力ファイルパス </summary>
    internal const string GeneratePath = "output/";

    /// <summary> Excel拡張子 </summary>
    internal const string exelExtention = ".xlsx";

	/// <summary> sql拡張子 </summary>
	internal const string SqlExtention = ".sql";

	/// <summary> json拡張子 </summary>
	internal const string JsonExtention = ".json";

	/// <summary> sqlディレクトリ </summary>
	internal const string SqlDir = "/sql/";

	/// <summary> jsonディレクトリ </summary>
	internal const string JsonDir = "/json/";

	/// <summary> クラス名行番号 </summary>
	internal const int ClassNameRow = 1;

    /// <summary> クラス名列番号 </summary>
    internal const int ClassNameCol = 3;

    /// <summary> 開始シート </summary>
    internal const int StartSheetNum = 0;

	/// <summary> ExcelToJsonコマンド</summary>
	internal const string ExcelToJson= "ExcelToJson";

	/// <summary> コピーコマンド</summary>
	internal const string JsonCopy = "JsonCopy";
}