/// <summary>
/// ジェネレートユーティリティ
/// Yuji Oshima
/// 2016/07/18
/// </summary>
using System;

namespace Alim.Utility
{
	/// <summary>
	/// ジェネレートユーティリティ
	/// </summary>
	public static class GenerateUtility
	{
		/// <summary>
		/// 著者名を取得
		/// </summary>
		/// <returns> 著者名 </returns>
		public static string GetAuthorName()
		{
			var names = Environment.UserName.Split('.');
			var firstName = names.Length > 0 ? names[0].CamelToPascal() : "";
			var familyName = names.Length > 1 ? names[1].CamelToPascal() : "";
			return firstName + " " + familyName;
		}
	}
}
