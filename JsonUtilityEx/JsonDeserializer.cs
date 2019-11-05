/// <summary>
/// JSON デシリアライザー
/// Yuji Oshima
/// 2017/07/18
/// </summary>

namespace Alim.Serialize
{
	/// <summary>
	/// JSON デシリアライザー
	/// </summary>
	internal sealed class JsonDeserializer
	{
		/// <summary>
		/// パース処理
		/// </summary>
		/// <param name="json">JSON テキスト</param>
		/// <returns></returns>
		public static T Deserialize<T>(string json) where T : new()
		{
			var hashtable = JsonParser.Parse(json);

			var obj = ObjectCreator.CreateObject<T>(hashtable);

			return obj;
		}
	}
}
