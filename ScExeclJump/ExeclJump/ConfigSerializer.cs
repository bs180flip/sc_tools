using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ExeclJump
{
	public class ConfigSerializer
	{
		static readonly string _configFileName = "\\execlJump.config";

		static string GetFullPath()
		{
			string fullPath = Path.GetDirectoryName(Application.ExecutablePath) + _configFileName;
			return fullPath;
		}

		// ファイルに書き出すときに使う
		public static void Save(ConfigData data)
		{
			try
			{
				string savePath = GetFullPath();
				using (var sw = new StreamWriter(savePath, false, Encoding.UTF8))
				{
					var ns = new XmlSerializerNamespaces();
					ns.Add(string.Empty, string.Empty);

					new XmlSerializer(typeof(ConfigData)).Serialize(sw, data, ns);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		// ファイルを読み取るときに使う
		public static ConfigData Load()
		{
			ConfigData cdata = InternalLoad();

			if (cdata == null)
			{
				cdata = new ConfigData();
				Save(cdata);
			}

			return cdata;
		}

		// ファイルを読み取るときに使う
		static ConfigData InternalLoad()
		{
			string loadPath = GetFullPath();
			try
			{
				using (var sr = new StreamReader(loadPath))
				{
					return (ConfigData)new XmlSerializer(typeof(ConfigData)).Deserialize(sr);
				}
			}
			catch (Exception e)
			{
			//	MessageBox.Show(e.Message);
				return null;
			}
		}
	}
}

