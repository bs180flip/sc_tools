
namespace Sc.Scenario
{
	/// <summary>
	/// 引数情報
	/// </summary>
	public class ArgInfo
	{
		/// <summary>引数タイプ</summary>
		public ArgType ArgType { get { return _argType; } }
		private ArgType _argType = ArgType.None;

		/// <summary>引数説明</summary>
		public string ArgDesc { get { return _argDesc; } }
		private string _argDesc = "";

		/// <summary>最小値</summary>
		public double? Min { get { return _min; } }
		private double? _min = null;

		/// <summary>最大値</summary>
		public double? Max { get { return _max; } }
		private double? _max = null;

		/// <summary>変数として置き換え可能かどうか</summary>
		public bool IsReplaceVariable { get { return _isReplaceVariable; } }
		private bool _isReplaceVariable = true;

		/// <summary>省略可能か</summary>
		public bool IsOptional { get { return _isOptional; } }
		private bool _isOptional = false;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="argType">引数タイプ</param>
		/// <param name="argDesc">引数説明</param>
		/// <param name="min">最小値</param>
		/// <param name="max">最大値</param>
		/// <param name="isReplaceVariable">変数として置き換え可能かどうか</param>
		/// <param name="isOptional">省略可能か</param>
		public ArgInfo(ArgType argType,
					   string argDesc,
					   double? min = null,
					   double? max = null,
					   bool isReplaceVariable = true,
					   bool isOptional = false)
		{
			_argType = argType;
			_argDesc = argDesc;
			_min = min;
			_max = max;
			_isReplaceVariable = isReplaceVariable;
			_isOptional = isOptional;
		}
	}
}
