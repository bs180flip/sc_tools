using System;

namespace ScExcelToJsonCell
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 4)
			{
				Console.WriteLine("引数の数が足りません");
				return;
			}

			var srcBookPath = args[0];
			var dstBookPath = args[1];
			var keyColName = args[2];
			var dstColName = args[3];

			var isList = false;
			var isCreate = false;
			for (int i = 4; i < args.Length; i++)
			{
				if (args[i] == "-l")
				{
					isList = true;
				}
				else if (args[i] == "-c")
				{
					isCreate = true;
				}
			}

			var srcBook = new SrcBook(srcBookPath, keyColName, isList, isCreate);
			var dstBook = new DstBook(dstBookPath, srcBook.JsonDict, keyColName, dstColName);
		}
	}
}
