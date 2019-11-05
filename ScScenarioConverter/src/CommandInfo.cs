using System.Collections.Generic;

namespace Sc.Scenario
{
	/// <summary>
	/// コマンド情報
	/// </summary>
	public class CommandInfo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="commandType">コマンドタイプ</param>
		/// <param name="commandDesc">コマンド説明</param>
		/// <param name="argInfos">引数情報リスト</param>
		public CommandInfo(CommandType commandType, string commandDesc, params ArgInfo[] argInfos)
		{
			CommandType = commandType;
			CommandDesc = commandDesc;
			ArgInfoList = new List<ArgInfo>(argInfos);
		}

		/// <summary>コマンドタイプ</summary>
		public CommandType CommandType { get; private set; }

		/// <summary>コマンド説明</summary>
		public string CommandDesc { get; private set; }

		/// <summary>引数情報リスト</summary>
		public List<ArgInfo> ArgInfoList { get; private set; }
	}
}
