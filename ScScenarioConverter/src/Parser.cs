using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Sc.Scenario;

namespace ScScenarioConverter
{
	/// <summary>
	/// バイナリをCsvにパース
	/// </summary>
	public class Parser
	{
		/// <summary>最大引数の数</summary>
		private const int MAX_ARG_COUNT = 8;

		/// <summary>成功したかどうか</summary>
		public bool IsSuccess { get { return _isSuccess; } }
		private bool _isSuccess = false;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="binFileName">bytesファイルパス</param>
		public Parser(string binFilePath)
		{
			if (File.Exists(binFilePath))
			{
				var stream = new FileStream(binFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using (var reader = new BinaryReader(stream))
				{
					var commandInfoDict = new CommandInfoDict();

					Console.OutputEncoding = new UTF8Encoding();
					System.Console.WriteLine("コマンド,引数1,引数2,引数3,引数4,引数5,引数6,引数7,引数8,");

					while (reader.BaseStream.Position != reader.BaseStream.Length)
					{
						var command = (CommandType)reader.ReadInt16();
						var argSize = (int)reader.ReadInt16();
						var argBytes = reader.ReadBytes(argSize);

						var commandInfo = commandInfoDict[command];
						if (commandInfo == null)
						{
							continue;
						}

						var commandName = commandInfoDict.SearchNameFromType(command);
						if (commandName == "")
						{
							continue;
						}

						System.Console.Write(commandName + ",");

						var byteCount = 0;
						var argCount = 0;

						foreach (var argInfo in commandInfo.ArgInfoList)
						{
							// 型情報
							if (byteCount + sizeof(byte) >= argBytes.Length)
							{
								continue;
							}

							var argType = (ArgType)argBytes[byteCount];
							byteCount++;

							switch (argType)
							{
							case ArgType.Bool:
							{
								if (byteCount + sizeof(Boolean) >= argBytes.Length) { continue; }

								var value = BitConverter.ToBoolean(argBytes, byteCount);
								byteCount += sizeof(Boolean);
								if (value)
								{
									System.Console.Write("TRUE");
								}
								else
								{
									System.Console.Write("FALSE");
								}
								break;
							}
							case ArgType.Byte:
							{
								if (byteCount + sizeof(Byte) >= argBytes.Length) { continue; }

								var value = argBytes[byteCount];
								byteCount += sizeof(Byte);
								System.Console.Write(value);
								break;
							}
							case ArgType.Int:
							{
								if (byteCount + sizeof(Int32) >= argBytes.Length) { continue; }

								var value = BitConverter.ToInt32(argBytes, byteCount);
								byteCount += sizeof(Int32);
								System.Console.Write(value);
								break;
							}
							case ArgType.Long:
							{
								if (byteCount + sizeof(Int64) >= argBytes.Length) { continue; }

								var value = BitConverter.ToInt64(argBytes, byteCount);
								byteCount += sizeof(Int64);
								System.Console.Write(value);
								break;
							}
							case ArgType.Float:
							{
								if (byteCount + sizeof(Single) >= argBytes.Length) { continue; }

								var value = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								System.Console.Write(value);
								break;
							}
							case ArgType.String:
							{
								if (byteCount + sizeof(Int16) >= argBytes.Length) { continue; }

								var length = BitConverter.ToInt16(argBytes, byteCount);
								byteCount += sizeof(Int16);

								if (byteCount + length >= argBytes.Length) { continue; }

								var bytes = new byte[length];
								Array.Copy(argBytes, byteCount, bytes, 0, length);
								var value = Encoding.UTF8.GetString(bytes);
								byteCount += length;
								if (value.Contains("\n"))
								{
									System.Console.Write("\"" + value + "\"");
								}
								else
								{
									System.Console.Write(value);
								}
								break;
							}
							case ArgType.Vector2:
							{
								if (byteCount + (sizeof(Single) * 2) >= argBytes.Length) { continue; }

								var xValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								var yValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								System.Console.Write(xValue + "|" + yValue);
								break;
							}
							case ArgType.Vector3:
							{
								if (byteCount + (sizeof(Single) * 3) >= argBytes.Length) { continue; }

								var xValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								var yValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								var zValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								System.Console.Write(xValue + "|" + yValue + "|" + zValue);
								break;
							}
							case ArgType.Rect:
							{
								if (byteCount + (sizeof(Single) * 4) >= argBytes.Length) { continue; }

								var xValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								var yValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								var wValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								var hValue = BitConverter.ToSingle(argBytes, byteCount);
								byteCount += sizeof(Single);
								System.Console.Write(xValue + "|" + yValue + "|" + wValue + "|" + hValue);
								break;
							}
							case ArgType.Color:
							{
								if (byteCount + (sizeof(Byte) * 3) >= argBytes.Length) { continue; }

								var rValue = argBytes[byteCount];
								byteCount += sizeof(Byte);
								var gValue = argBytes[byteCount];
								byteCount += sizeof(Byte);
								var bValue = argBytes[byteCount];
								byteCount += sizeof(Byte);
								System.Console.Write(rValue + "|" + gValue + "|" + bValue);
								break;
							}
							default:
								break;
							}

							argCount++;
							if (argCount < MAX_ARG_COUNT)
							{
								System.Console.Write(",");
							}

							if (argBytes.Length == byteCount)
							{
								break;
							}
						}

						for (int i = argCount; i < MAX_ARG_COUNT; i++)
						{
							System.Console.Write(",");
						}

						System.Console.WriteLine("");
					}

					_isSuccess = true;
				}
			}
		}
	}
}
