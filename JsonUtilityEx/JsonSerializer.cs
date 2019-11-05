/// <summary>
/// JSON シリアライザー
/// Yuji Oshima
/// 2017/07/18
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Alim.Serialize
{
	/// <summary>
	/// JSON シリアライザー
	/// </summary>
	internal sealed class JsonSerializer
	{
		/// <summary>文字列入力</summary>
		private StringBuilder stringBuilder = null;

		/// <summary>
		/// プライベートコンストラクタ
		/// </summary>
		private JsonSerializer()
		{
			stringBuilder = new StringBuilder();
		}

		/// <summary>
		/// シリアライズ処理
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public static string Serialize(object obj)
		{
			var serializer = new JsonSerializer();

			serializer.SerializeObject(obj);

			return serializer.stringBuilder.ToString();
		}

		/// <summary>
		/// object型をJSON文字列に変換
		/// </summary>
		/// <param name="obj">object型</param>
		private void SerializeObject(object obj)
		{
			var type = obj.GetType();
			var flags = BindingFlags.Public | BindingFlags.Instance;
			var fields = new List<FieldInfo>(type.GetFields(flags));

			stringBuilder.Append("{");

			var i = 0;
			foreach (var field in fields)
			{
				if (i++ != 0) { stringBuilder.Append(","); }

				var fieldType = field.FieldType;
				var key = field.Name;
				SerializeString((object)key);
				stringBuilder.Append(":");

				var val = field.GetValue(obj);

				if (val == null)
				{
					stringBuilder.Append("null");
					continue;
				}

				// Dictionary型
				if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
				{
					SerializeDictionary(val);
				}
				// List型
				else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
				{
					SerializeList(val);
				}
				// 配列型
				else if (fieldType.IsArray)
				{
					SerializeArray(val);
				}
				// それ以外
				else
				{
					SerializeStandard(val);
				}
			}
			stringBuilder.Append("}");
		}

		/// <summary>
		/// Dictionary型をJSON文字列に変換
		/// </summary>
		/// <param name="obj">Dictionary型</param>
		/// <returns>JSON文字列</returns>
		private void SerializeDictionary(object obj)
		{
			var dict = (IDictionary)obj;

			stringBuilder.Append("{");
			int i = 0;
			foreach (var key in dict.Keys)
			{
				if (i++ != 0) { stringBuilder.Append(","); }
				SerializeString(key);
				stringBuilder.Append(":");
				SerializeStandard(dict[key]);
			}
			stringBuilder.Append("}");
		}

		/// <summary>
		/// List型をJSON文字列に変換
		/// </summary>
		/// <param name="obj">List型</param>
		/// <returns>JSON文字列</returns>
		private void SerializeList(object obj)
		{
			var list = (IList)obj;

			stringBuilder.Append("[");
			var i = 0;
			foreach (var element in list)
			{
				if (i++ != 0) { stringBuilder.Append(","); }
				SerializeStandard(element);
			}
			stringBuilder.Append("]");
		}

		/// <summary>
		/// 配列型をJSON文字列に変換
		/// </summary>
		/// <param name="obj">配列型</param>
		/// <returns>JSON文字列</returns>
		private void SerializeArray(object obj)
		{
			var array = (Array)obj;

			stringBuilder.Append("[");
			var i = 0;
			foreach (var element in array)
			{
				if (i++ != 0) { stringBuilder.Append(","); }
				SerializeStandard(element);
			}
			stringBuilder.Append("]");
		}

		/// <summary>
		/// 基本型をJSON文字列に変換
		/// </summary>
		/// <param name="obj">基本型</param>
		/// <returns>JSON文字列</returns>
		private void SerializeStandard(object obj)
		{
			var type = obj.GetType();

			// 数値型
			if (type.IsPrimitive)
			{
				SerializeNumber(obj);
			}
			// String型
			else if (type == typeof(string))
			{
				SerializeString(obj);
			}
			// クラス型
			else
			{
				SerializeObject(obj);
			}
		}

		/// <summary>
		/// 文字列型をJSON文字列に変換
		/// </summary>
		/// <param name="obj">文字列型</param>
		/// <returns>JSON文字列</returns>
		private void SerializeString(object obj)
		{
			stringBuilder.Append("\"" + obj.ToString() + "\"");
		}

		/// <summary>
		/// 数値型をJSON文字列に変換
		/// </summary>
		/// <param name="obj">数値型</param>
		/// <returns>JSON文字列</returns>
		private void SerializeNumber(object obj)
		{
			stringBuilder.Append(obj.ToString());
		}
	}
}
