/// <summary>
/// オブジェクト生成クラス
/// Yuji Oshima
/// 2017/07/18
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Alim.Serialize
{
	/// <summary>
	/// オブジェクト生成クラス
	/// </summary>
	public sealed class ObjectCreator
	{
		/// <summary>
		/// オブジェクトを生成
		/// </summary>
		/// <typeparam name="T">タイプ</typeparam>
		/// <param name="hashtable">ハッシュテーブル</param>
		/// <returns>オブジェクト</returns>
		public static T CreateObject<T>(Hashtable hashtable) where T : new()
		{
			var creator = new ObjectCreator();
			return (T)creator.CreateObject(typeof(T), hashtable);
		}

		/// <summary>
		/// プライベートコンストラクタ
		/// </summary>
		private ObjectCreator()
		{

		}

		/// <summary>
		/// オブジェクトを生成
		/// </summary>
		/// <param name="type">タイプ</param>
		/// <param name="hashtable">ハッシュテーブル</param>
		/// <returns>オブジェクト</returns>
		private object CreateObject(Type type, Hashtable hashtable)
		{
			// デフォルトコンストラクタを取得
			var constructorInfo = type.GetConstructor(new Type[] { });
			// 戻り値のオブジェクトを生成
			var obj = constructorInfo.Invoke(new object[] { });

			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
			// ハッシュテーブルに登録された全フィールドに対して
			foreach (string key in hashtable.Keys)
			{
				var value = hashtable[key];

				if (value == null) { continue; }

				// フィールドの情報を取得
				var fieldInfo = type.GetField(key, flags);

				if (fieldInfo == null) { continue; }

				// フィールドの型を取得
				var fieldType = fieldInfo.FieldType;

				// Dictionary型
				if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
				{
					// フィールドに Dictionary のインスタンスを設定
					var dictObj = CreateDictionary(fieldType, value);
					fieldInfo.SetValue(obj, dictObj);
				}
				// List型
				else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
				{
					// フィールドに List のインスタンスを設定
					var listObj = CreateList(fieldType, value);
					fieldInfo.SetValue(obj, listObj);
				}
				// 配列型の時
				else if (fieldType.IsArray)
				{
					var arrayObj = CreateArray(fieldType, value);
					// フィールドに配列のインスタンスを設定
					fieldInfo.SetValue(obj, arrayObj);
				}
				else
				{
					// プリミティブ型かstring型なら
					if (fieldType.IsPrimitive || fieldType == typeof(string))
					{
						// Value に値が直接入っている
						fieldInfo.SetValue(obj, Convert.ChangeType(value, fieldType));
					}
					// クラス型なら
					else
					{
						// 内包オブジェクト生成
						var innerObj = CreateObject(fieldType, value as Hashtable);
						// フィールドにインスタンスを設定
						fieldInfo.SetValue(obj, innerObj);
					}
				}
			}

			return obj;
		}

		/// <summary>
		/// Dictionary オブジェクトを生成
		/// </summary>
		/// <param name="fieldType">フィールドタイプ</param>
		/// <param name="value">値</param>
		/// <returns>Dictionary オブジェクト</returns>
		private IDictionary CreateDictionary(Type fieldType, object value)
		{
			// value に Hashtable の形で入っている
			var hashtable = value as Hashtable;

			if (hashtable == null) { return null; }

			var keyType = fieldType.GetGenericArguments()[0];
			var valueType = fieldType.GetGenericArguments()[1];
			var constructedType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
			// Dictionary のインスタンスを生成
			var dictObj = Activator.CreateInstance(constructedType) as IDictionary;

			// 配列の要素数すべてに
			foreach (DictionaryEntry kvp in hashtable)
			{
				// プリミティブ型かstring型なら
				if (valueType.IsPrimitive || valueType == typeof(string))
				{
					var kvpKey = Convert.ChangeType(kvp.Key, keyType);
					var kvpValue = Convert.ChangeType(kvp.Value, valueType);
					dictObj.Add(kvpKey, kvpValue);
				}
				// クラス型なら
				else
				{
					var kvpKey = Convert.ChangeType(kvp.Key, keyType);
					var kvpValue = CreateObject(valueType, kvp.Value as Hashtable);
					dictObj.Add(kvpKey, kvpValue);
				}
			}

			return dictObj;
		}

		/// <summary>
		/// List オブジェクトを生成
		/// </summary>
		/// <param name="fieldType">フィールドタイプ</param>
		/// <param name="value">値</param>
		/// <returns>List オブジェクト</returns>
		private IList CreateList(Type fieldType, object value)
		{
			// value に object[] の形で入っている
			var itemList = value as object[];

			if (itemList == null) { return null; }

			// List の要素型を取得
			var elementType = fieldType.GetGenericArguments()[0];
			// List<> の型を生成
			var constructedType = typeof(List<>).MakeGenericType(elementType);
			// List<> のインスタンスを生成
			var listObj = Activator.CreateInstance(constructedType) as IList;

			foreach (var item in itemList)
			{
				// プリミティブ型かstring型なら
				if (elementType.IsPrimitive || elementType == typeof(string))
				{
					listObj.Add(Convert.ChangeType(item, elementType));
				}
				// クラス型なら
				else
				{
					var innerObj = CreateObject(elementType, item as Hashtable);
					listObj.Add(innerObj);
				}
			}

			return listObj;
		}

		/// <summary>
		/// 配列オブジェクトを生成
		/// </summary>
		/// <param name="fieldType">フィールドタイプ</param>
		/// <param name="value">値</param>
		/// <returns>配列オブジェクト</returns>
		private Array CreateArray(Type fieldType, object value)
		{
			// value に object[] の形で入っている
			var itemList = value as object[];

			if (itemList == null) { return null; }

			// 配列の要素型を取得
			var elementType = fieldType.GetElementType();
			// 配列のインスタンスを生成
			var arrayObj = Array.CreateInstance(elementType, itemList.Length);

			// 配列の要素数すべてに
			for (int i = 0; i < itemList.Length; i++)
			{
				// プリミティブ型かstring型なら
				if (elementType.IsPrimitive || elementType == typeof(string))
				{
					arrayObj.SetValue(Convert.ChangeType(itemList[i], elementType), i);
				}
				// クラス型なら
				else
				{
					var innerObj = CreateObject(elementType, itemList[i] as Hashtable);
					arrayObj.SetValue(innerObj, i);
				}
			}

			return arrayObj;
		}
	}
}
