using System.Text;

namespace ScScenarioTools
{
	/// <summary>
	/// ソースコード構築用のクラス
	/// インデントを適当に処理してくれる
	/// </summary>
	public class SourceBuilder
	{
		private StringBuilder _sb = null;
		private int _tabCount = 0;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SourceBuilder()
		{
			_sb = new StringBuilder();

			_sb.AppendLine("// このコードはツールによって自動生成されたものです");
			_sb.AppendLine();
		}

		/// <summary>
		/// コード追加
		/// </summary>
		/// <param name="data">書き込むデータ(nullの場合は改行のみ)</param>
		public void Append(string data = null)
		{
			if (data == null)
			{
				_sb.AppendLine();
			}
			else
			{
				var beginBracketCount = CountOf(data, "{");
				var endBracketCount = CountOf(data, "}");

				if (endBracketCount > 0)
				{
					_tabCount -= (endBracketCount - beginBracketCount);
				}

				for (int i = 0; i < _tabCount; i++)
				{
					_sb.Append("\t");
				}

				_sb.AppendLine(data);

				if (endBracketCount == 0)
				{
					_tabCount += beginBracketCount;
				}
			}
		}

		/// <summary>
		/// 文字列化
		/// </summary>
		public override string ToString()
		{
			return _sb.ToString();
		}

		/// <summary>
		/// 文字列に特定文字が含まれている数をカウント
		/// </summary>
		private int CountOf(string target, string str)
		{
			int count = 0;

			int index = target.IndexOf(str, 0);
			while (index != -1)
			{
				count++;
				index = target.IndexOf(str, index + str.Length);
			}

			return count;
		}
	}
}
