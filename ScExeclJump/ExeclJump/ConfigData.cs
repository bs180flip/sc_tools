using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ExeclJump
{
	[XmlRoot(ElementName = "config")]
	public class ConfigData
	{
		[XmlElement(ElementName = "define_path")]
		public string DefinePath { get; set; }

		public void UpdateFile()
		{
			ConfigSerializer.Save(this);
		}
	}
}
