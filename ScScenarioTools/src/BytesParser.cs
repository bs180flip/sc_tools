using System;
using System.IO;
using System.Text;
using ScScenarioCommon;

namespace ScScenarioTools
{
	public class BytesParser
	{
		/// <summary>
		/// コンバートしたファイルを解析する
		/// </summary>
		/// <param name="bytesFile">コンバートしたファイル</param>
		/// <param name="argTypeFile">ArgType用の引数情報CSVファイル</param>
		/// <param name="commandDir">各種コマンド用のCSV格納フォルダ</param>
		public static int Parse(string bytesFile, string argTypeFile, string commandDir)
		{
			var ret = 0;

			var commandData = new ScenarioCommandData();
			commandData.IsDumpConsole = false;

			ret = commandData.ReadArgTypeCsv(argTypeFile);
			if (ret != 0)
			{
				return ret;
			}

			ret = commandData.ReadCommandCsv(commandDir);
			if (ret != 0)
			{
				return ret;
			}

			if (File.Exists(bytesFile))
			{
				var stream = new FileStream(bytesFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using (var reader = new BinaryReader(stream))
				{
					Console.OutputEncoding = new UTF8Encoding();
					System.Console.WriteLine("コマンド,引数1,引数2,引数3,引数4,引数5,引数6,引数7,引数8,");

					while (reader.BaseStream.Position != reader.BaseStream.Length)
					{
						var commandId = reader.ReadUInt32();
						var argSize = (int)reader.ReadInt16();
						var argBytes = reader.ReadBytes(argSize);

						ScenarioCommandData.CommandInfo commandInfo = null;
						foreach (var info in commandData.CommandInfoDict)
						{
							if (info.Value.Id == commandId)
							{
								commandInfo = info.Value;
								break;
							}
						}
						if (commandInfo == null)
						{
							continue;
						}

						System.Console.Write(commandInfo.Name + ",");

						var byteCount = 0;
						var argCount = 0;

						foreach (var argInfo in commandInfo.ArgDict)
						{
							// 型情報
							if (byteCount + sizeof(byte) >= argBytes.Length)
							{
								continue;
							}

							var argType = (int)argBytes[byteCount];
							byteCount++;

							foreach (var arg in commandData.ArgTypeDict)
							{
								if (argType == arg.Value.TypeValue)
								{
									if (arg.Key.Equals("Bool"))
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
									}
									else if (arg.Key.Equals("Byte"))
									{
										if (byteCount + sizeof(Byte) >= argBytes.Length) { continue; }

										var value = argBytes[byteCount];
										byteCount += sizeof(Byte);
										System.Console.Write(value);
									}
									else if (arg.Key.Equals("Int"))
									{
										if (byteCount + sizeof(Int32) >= argBytes.Length) { continue; }

										var value = BitConverter.ToInt32(argBytes, byteCount);
										byteCount += sizeof(Int32);
										System.Console.Write(value);
									}
									else if (arg.Key.Equals("Long"))
									{
										if (byteCount + sizeof(Int64) >= argBytes.Length) { continue; }

										var value = BitConverter.ToInt64(argBytes, byteCount);
										byteCount += sizeof(Int64);
										System.Console.Write(value);
									}
									else if (arg.Key.Equals("Float"))
									{
										if (byteCount + sizeof(Single) >= argBytes.Length) { continue; }

										var value = BitConverter.ToSingle(argBytes, byteCount);
										byteCount += sizeof(Single);
										System.Console.Write(value);
									}
									else if (arg.Key.Equals("String"))
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
									}
									else if (arg.Key.Equals("Vector2"))
									{
										if (byteCount + (sizeof(Single) * 2) >= argBytes.Length) { continue; }

										var xValue = BitConverter.ToSingle(argBytes, byteCount);
										byteCount += sizeof(Single);
										var yValue = BitConverter.ToSingle(argBytes, byteCount);
										byteCount += sizeof(Single);
										System.Console.Write(xValue + "|" + yValue);
									}
									else if (arg.Key.Equals("Vector3"))
									{
										if (byteCount + (sizeof(Single) * 3) >= argBytes.Length) { continue; }

										var xValue = BitConverter.ToSingle(argBytes, byteCount);
										byteCount += sizeof(Single);
										var yValue = BitConverter.ToSingle(argBytes, byteCount);
										byteCount += sizeof(Single);
										var zValue = BitConverter.ToSingle(argBytes, byteCount);
										byteCount += sizeof(Single);
										System.Console.Write(xValue + "|" + yValue + "|" + zValue);
									}
									else if (arg.Key.Equals("Rect"))
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
									}
									else if (arg.Key.Equals("Color"))
									{
										if (byteCount + (sizeof(Byte) * 3) >= argBytes.Length) { continue; }

										var rValue = argBytes[byteCount];
										byteCount += sizeof(Byte);
										var gValue = argBytes[byteCount];
										byteCount += sizeof(Byte);
										var bValue = argBytes[byteCount];
										byteCount += sizeof(Byte);
										System.Console.Write(rValue + "|" + gValue + "|" + bValue);
									}

									break;
								}
							}

							argCount++;
							if (argCount < ScenarioCommandData.MaxCommandArgCount)
							{
								System.Console.Write(",");
							}

							if (argBytes.Length == byteCount)
							{
								break;
							}
						}

						for (int i = argCount; i < ScenarioCommandData.MaxCommandArgCount; i++)
						{
							System.Console.Write(",");
						}

						System.Console.WriteLine("");
					}
				}
			}
			else
			{
				System.Console.WriteLine("ファイルオープンエラー path=" + bytesFile);
				return 1;
			}

			return 0;
		}
	}
}
