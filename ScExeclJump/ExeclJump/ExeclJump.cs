using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.POIFS.FileSystem;

namespace ExeclJump
{
	public partial class ExeclJump : Form
	{
		readonly string _no_path = "...右上のボタンでパスを設定しましょう.......................................................................";
		readonly string _defineFileName = "Define.txt";

		Size _openSize = new Size(665, 277);
		Size _closeSize = new Size(665, 155);
		string nowExcelFilePath = "";

		public ExeclJump()
		{
			InitializeComponent();

			ConfigData cdata = ConfigSerializer.Load();
			lblPath.Text = "";

			if (cdata == null || string.IsNullOrEmpty(cdata.DefinePath) == true)
			{
				FindDefineFile();
				if (lblPath.Text == "")
				{
					lblPath.Text = _no_path;
				}
			}
			else
			{
				lblPath.Text = cdata.DefinePath.Trim();
			}
			this.Size = _closeSize;
		}

		private void ExeclJump_Load(object sender, EventArgs e)
		{

		}

		private void btnOpen_Click(object sender, EventArgs e)
		{
			if (CheckPath() == false) { return; }

			if (string.IsNullOrWhiteSpace(nowExcelFilePath) == true) { return; }

			System.Diagnostics.Process.Start(nowExcelFilePath);
		}

		private void btnPath_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Define Files|*.txt;*";

			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var result = Path.GetFileName(ofd.FileName);
				if (result == _defineFileName)
				{
					ConfigData cdata = ConfigSerializer.Load();
					cdata.DefinePath = ofd.FileName;
					lblPath.Text = cdata.DefinePath;
					cdata.UpdateFile();
				}
				else
				{
					MessageBox.Show(@"Error： Define.txt はｓｖｎの'02_マスタ管理\origin\'にいます...");
				}

			}
		}

		private void btnFind_Click(object sender, EventArgs e)
		{
			if (CheckPath() == false) { return; }

			string seach = txtName.Text.Trim();
			if (string.IsNullOrWhiteSpace(seach) == true) { return; }
			seach = seach.Replace("#", "");
			nowExcelFilePath = "";

			LoadData loadData = LoadDefineFile(lblPath.Text.Trim(), _defineFileName);
			if (loadData == null)
			{
				lblPath.Text = _no_path;
				this.Size = _closeSize;
				MessageBox.Show("Define.txtのパスを設定しましょう、右上のボタンで！");
				return;
			}

			lstPath.Items.Clear();

			foreach (var excel in loadData.AllExcel)
			{
				foreach (var sheet in excel.Value.Sheets)
				{
					foreach (var val in sheet.Defines)
					{
						if (val.DefineName == seach)
						{
							string excelFileName = excel.Value.ExcelFileName;
							loadData.FindExcel(excel.Value.ExcelFileName, ref nowExcelFilePath, ref excelFileName);
							string str = string.Format("{0}.xlsx => {1} : {2}", excelFileName, val.DefineName, val.Value);
							lstPath.Items.Add(str);
						}
					}
				}
			}

			if (lstPath.Items.Count == 0)
			{
				this.Size = _closeSize;
				MessageBox.Show("何も見つかりません。。。");
			}
			else
			{
				this.Size = _openSize;
			}
		}

		private void btnOpenDefineFolder_Click(object sender, EventArgs e)
		{
			if (lblPath.Text == "")
			{
				MessageBox.Show(_defineFileName + "のパス設定していない。。。");
				return;
			}

			System.Diagnostics.Process.Start(Directory.GetParent(lblPath.Text).FullName);
		}

		bool CheckPath()
		{
			if (lblPath.Text == null || lblPath.Text == "" || lblPath.Text == _no_path)
			{
				MessageBox.Show(_no_path);
				return false;

			}

			return true;
		}

		static LoadData LoadDefineFile(string path, string defineFileName)
		{
			try
			{
				var logFile = File.ReadAllLines(path);
				var loadData = new LoadData();

				loadData.DefineFileAllLines = new List<string>();
				foreach (var item in logFile)
				{
					string line = item.Trim();
					if (string.IsNullOrEmpty(line) == true) { continue; }

					loadData.DefineFileAllLines.Add(line);
				}

				if (loadData.DefineFileAllLines.Count <= 0)
				{
					return null;
				}

				string mstfolder = path.Replace(@"\origin\" + defineFileName, "");
				string[] files = Directory.GetFiles(mstfolder, "*.xlsx", SearchOption.AllDirectories);

				loadData.AllExcelPath = new Dictionary<string, string>();
				foreach (var f in files)
				{
					var name = Path.GetFileName(f).Replace(".xlsx", "");
					loadData.AllExcelPath[name] = f;
				}

				loadData.OnLoadOver();
				return loadData;
			}
			catch
			{
				return null;
			}
		}

		static bool IsNewSheet(string line)
		{
			if (line.Length > 2 && line[0] == '/' && line[1] == '/')
			{
				return true;
			}
			return false;
		}

		static bool GetName(string line, out string excelName, out string sheetName)
		{
			excelName = "";
			sheetName = "";
			string[] str = line.Split(':');
			if (str.Length != 2)
			{
				MessageBox.Show(string.Format("excel/sheet Name 解析Error。。。\n Line:{0}", line));
				return false;
			}

			excelName = str[0].Replace("//", "").Trim();
			sheetName = str[1].Trim();
			return true;
		}

		static bool GetDefineNameAndValue(string line, out string defineName, out string defineValue)
		{
			defineName = "";
			defineValue = "";
			string[] str = line.Split('|');
			if (str.Length != 2)
			{
				MessageBox.Show(string.Format("defineName/defineValue 解析Error。。。\n Line:{0}", line));
				return false;
			}

			defineName = str[0].Replace("#", "").Trim();
			defineValue = str[1].Replace("#", "").Trim();
			return true;
		}

		void FindDefineFile()
		{
			string exePath = Directory.GetCurrentDirectory();
			string defineFolder = exePath.Replace(@"tools\ScExcelJump", "");
			defineFolder += @"02_マスタ管理\origin\" + _defineFileName;
			if (File.Exists(defineFolder) == true)
			{
				ConfigData cdata = ConfigSerializer.Load();
				cdata.DefinePath = defineFolder;
				lblPath.Text = cdata.DefinePath;
				cdata.UpdateFile();
			}
		}

		class LoadData
		{
			public List<string> DefineFileAllLines = new List<string>();

			//key is excel name 
			public Dictionary<string, Excel> AllExcel = new Dictionary<string, Excel>();

			//excel name and full path
			public Dictionary<string, string> AllExcelPath = new Dictionary<string, string>();

			public void OnLoadOver()
			{
				Excel excel = null;
				string nowSheetName = "";

				for (int i = 0; i < DefineFileAllLines.Count; i++)
				{
					string line = DefineFileAllLines[i];

					if (IsNewSheet(line) == true)
					{
						string excelName, sheetName;
						if (GetName(line, out excelName, out sheetName) == false) { return; }
						nowSheetName = sheetName;

						if (AllExcel.ContainsKey(excelName) == false)
						{
							excel = new Excel(excelName);
							AllExcel.Add(excelName, excel);
						}
						else
						{
							excel = AllExcel[excelName];
						}
					}
					else
					{
						if (excel == null) { continue; }

						string defineName, defineValue;
						if (GetDefineNameAndValue(line, out defineName, out defineValue) == false) { return; }
						excel.Add(nowSheetName, defineName, defineValue);
					}
				}
			}

			public bool FindExcel(string defineExcelFileName, ref string nowExcelFilePath, ref string excelFileName)
			{
				string name = defineExcelFileName.Replace("_", "").Replace("MST", "").ToLower();

				foreach (var item in AllExcelPath)
				{
					string p = item.Key.ToLower();
					if (p == name)
					{
						nowExcelFilePath = item.Value;
						excelFileName = item.Key;

						return true;
					}
				}
				return false;
			}
		}

		///one excel file with all define in it
		class Excel
		{
			public string ExcelFileName = "";
			public List<Sheet> Sheets = new List<Sheet>();

			public Excel(string excelFileName)
			{
				ExcelFileName = excelFileName;
			}

			public void Add(string sheetName, string defineName, string defineValue)
			{
				foreach (var item in Sheets)
				{
					if (item.SheetName == sheetName)
					{
						item.Defines.Add(new NameValue(defineName, defineValue));
						return;
					}
				}

				Sheets.Add(new Sheet(sheetName, defineName, defineValue));
			}
		}

		class Sheet
		{
			public string SheetName = "";
			public List<NameValue> Defines = new List<NameValue>();

			public Sheet(string sheetName, string defineName, string defineValue)
			{
				SheetName = sheetName;
				Defines.Add(new NameValue(defineName, defineValue));
			}
		}

		class NameValue
		{
			public string DefineName = "";
			public string Value = "";

			public NameValue(string defineName, string value)
			{
				DefineName = defineName;
				Value = value;
			}
		}
	}
}
