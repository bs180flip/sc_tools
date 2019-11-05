namespace ExeclJump
{
	partial class ExeclJump
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnPath = new System.Windows.Forms.Button();
			this.lstPath = new System.Windows.Forms.ListBox();
			this.lblPath = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnFind = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.btnOpenDefineFolder = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(518, 144);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(119, 100);
			this.button1.TabIndex = 0;
			this.button1.Text = "open excel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.btnOpen_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(67, 95);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 12);
			this.label1.TabIndex = 1;
			// 
			// btnPath
			// 
			this.btnPath.Location = new System.Drawing.Point(518, 9);
			this.btnPath.Name = "btnPath";
			this.btnPath.Size = new System.Drawing.Size(121, 23);
			this.btnPath.TabIndex = 2;
			this.btnPath.Text = "Define.txtのパス設定";
			this.btnPath.UseVisualStyleBackColor = true;
			this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
			// 
			// lstPath
			// 
			this.lstPath.AllowDrop = true;
			this.lstPath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstPath.FormattingEnabled = true;
			this.lstPath.ItemHeight = 12;
			this.lstPath.Location = new System.Drawing.Point(12, 144);
			this.lstPath.Name = "lstPath";
			this.lstPath.Size = new System.Drawing.Size(500, 100);
			this.lstPath.TabIndex = 3;
			// 
			// lblPath
			// 
			this.lblPath.AutoSize = true;
			this.lblPath.Location = new System.Drawing.Point(138, 98);
			this.lblPath.Name = "lblPath";
			this.lblPath.Size = new System.Drawing.Size(317, 12);
			this.lblPath.TabIndex = 4;
			this.lblPath.Text = "...右上のボタンでパスを設定しましょう............................................................." +
    "..........";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(14, 65);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(295, 19);
			this.txtName.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("MS UI Gothic", 19F);
			this.label2.Location = new System.Drawing.Point(12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(351, 26);
			this.label2.TabIndex = 6;
			this.label2.Text = "定義名を定義したexcelを探す！";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 50);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 12);
			this.label3.TabIndex = 7;
			this.label3.Text = "定義名：";
			// 
			// btnFind
			// 
			this.btnFind.Location = new System.Drawing.Point(333, 63);
			this.btnFind.Name = "btnFind";
			this.btnFind.Size = new System.Drawing.Size(121, 23);
			this.btnFind.TabIndex = 8;
			this.btnFind.Text = "探す";
			this.btnFind.UseVisualStyleBackColor = true;
			this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(15, 129);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(39, 12);
			this.label4.TabIndex = 9;
			this.label4.Text = "Find!! :";
			// 
			// btnOpenDefineFolder
			// 
			this.btnOpenDefineFolder.BackColor = System.Drawing.SystemColors.Control;
			this.btnOpenDefineFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOpenDefineFolder.Location = new System.Drawing.Point(14, 90);
			this.btnOpenDefineFolder.Name = "btnOpenDefineFolder";
			this.btnOpenDefineFolder.Size = new System.Drawing.Size(118, 23);
			this.btnOpenDefineFolder.TabIndex = 10;
			this.btnOpenDefineFolder.Text = "Define.txt のパス：";
			this.btnOpenDefineFolder.UseVisualStyleBackColor = false;
			this.btnOpenDefineFolder.Click += new System.EventHandler(this.btnOpenDefineFolder_Click);
			// 
			// ExeclJump
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(649, 259);
			this.Controls.Add(this.btnOpenDefineFolder);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnFind);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.lblPath);
			this.Controls.Add(this.lstPath);
			this.Controls.Add(this.btnPath);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "ExeclJump";
			this.Text = "ExeclJump (ver 2019.04.25)";
			this.Load += new System.EventHandler(this.ExeclJump_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnPath;
		private System.Windows.Forms.Label lblPath;
		protected System.Windows.Forms.ListBox lstPath;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnFind;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnOpenDefineFolder;
	}
}

