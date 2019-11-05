
namespace Sc.Scenario
{
	/// <summary>
	/// 引数タイプ
	/// </summary>
	public enum ArgType
	{
		/// <summary>無効</summary>
		None,

		/// <summary>真偽値</summary>
		Bool,

		/// <summary>8bit自然数値</summary>
		Byte,

		/// <summary>32bit整数値</summary>
		Int,

		/// <summary>64bit整数値</summary>
		Long,

		/// <summary>実数値</summary>
		Float,

		/// <summary>文字列</summary>
		String,

		/// <summary>2次元ベクトル</summary>
		Vector2,

		/// <summary>3次元ベクトル</summary>
		Vector3,

		/// <summary>矩形</summary>
		Rect,

		/// <summary>RGBカラー</summary>
		Color,
	}
}
