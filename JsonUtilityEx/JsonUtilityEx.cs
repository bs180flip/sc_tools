/// <summary>
/// JSONユーティリティ
/// Yuji Oshima
/// 2017/07/18
/// </summary>
namespace Alim.Serialize
{
	/// <summary>
	/// JSONユーティリティ
	/// </summary>
	public static class JsonUtilityEx
	{
		/// <summary>
		/// JSON文字列に変換
		/// </summary>
		/// <param name="obj">対象</param>
		/// <returns>JSON文字列</returns>
		public static string ToJson(object obj)
		{
			return JsonSerializer.Serialize(obj);
		}

		/// <summary>
		/// デシリアライズ
		/// </summary>
		/// <typeparam name="T">クラス型</typeparam>
		/// <param name="json">JSON形式テキスト</param>
		/// <returns>データが設定されたインスタンス</returns>
		public static T FromJson<T>(string json) where T : new()
		{
			return JsonDeserializer.Deserialize<T>(json);
		}
	}
}
