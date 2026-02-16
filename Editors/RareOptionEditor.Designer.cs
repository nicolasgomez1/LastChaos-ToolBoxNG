using System.Windows.Forms;

namespace LastChaos_ToolBoxNG
{
	partial class RareOptionEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			btnReload = new Button();
			btnAddNew = new Button();
			MainList = new ListBox();
			groupBox1 = new GroupBox();
			cbTypeSelector = new ComboBox();
			label7 = new Label();
			groupBox3 = new GroupBox();
			lOptionProbPercentage9 = new Label();
			cbOptionLevel9 = new ComboBox();
			label40 = new Label();
			tbOptionProb9 = new TextBox();
			label41 = new Label();
			cbOptionID9 = new ComboBox();
			label42 = new Label();
			lOptionProbPercentage8 = new Label();
			cbOptionLevel8 = new ComboBox();
			label44 = new Label();
			tbOptionProb8 = new TextBox();
			label45 = new Label();
			cbOptionID8 = new ComboBox();
			label46 = new Label();
			lOptionProbPercentage7 = new Label();
			cbOptionLevel7 = new ComboBox();
			label32 = new Label();
			tbOptionProb7 = new TextBox();
			label33 = new Label();
			cbOptionID7 = new ComboBox();
			label34 = new Label();
			lOptionProbPercentage6 = new Label();
			cbOptionLevel6 = new ComboBox();
			label36 = new Label();
			tbOptionProb6 = new TextBox();
			label37 = new Label();
			cbOptionID6 = new ComboBox();
			label38 = new Label();
			lOptionProbPercentage5 = new Label();
			cbOptionLevel5 = new ComboBox();
			label24 = new Label();
			tbOptionProb5 = new TextBox();
			label25 = new Label();
			cbOptionID5 = new ComboBox();
			label26 = new Label();
			lOptionProbPercentage4 = new Label();
			cbOptionLevel4 = new ComboBox();
			label28 = new Label();
			tbOptionProb4 = new TextBox();
			label29 = new Label();
			cbOptionID4 = new ComboBox();
			label30 = new Label();
			lOptionProbPercentage3 = new Label();
			cbOptionLevel3 = new ComboBox();
			label16 = new Label();
			tbOptionProb3 = new TextBox();
			label17 = new Label();
			cbOptionID3 = new ComboBox();
			label18 = new Label();
			lOptionProbPercentage2 = new Label();
			cbOptionLevel2 = new ComboBox();
			label20 = new Label();
			tbOptionProb2 = new TextBox();
			label21 = new Label();
			cbOptionID2 = new ComboBox();
			label22 = new Label();
			lOptionProbPercentage1 = new Label();
			cbOptionLevel1 = new ComboBox();
			label6 = new Label();
			tbOptionProb1 = new TextBox();
			label12 = new Label();
			cbOptionID1 = new ComboBox();
			label15 = new Label();
			lOptionProbPercentage0 = new Label();
			cbOptionLevel0 = new ComboBox();
			label14 = new Label();
			tbOptionProb0 = new TextBox();
			label13 = new Label();
			cbOptionID0 = new ComboBox();
			label4 = new Label();
			groupBox5 = new GroupBox();
			tbAttack = new TextBox();
			tbResistance = new TextBox();
			label8 = new Label();
			label11 = new Label();
			label9 = new Label();
			tbMagicAttack = new TextBox();
			tbDefense = new TextBox();
			label10 = new Label();
			btnUpdate = new Button();
			cbGradeSelector = new ComboBox();
			label1 = new Label();
			label3 = new Label();
			groupBox2 = new GroupBox();
			cbNationSelector = new ComboBox();
			tbName = new TextBox();
			label2 = new Label();
			tbID = new TextBox();
			btnCopy = new Button();
			btnDelete = new Button();
			tbSearch = new TextBox();
			groupBox1.SuspendLayout();
			groupBox3.SuspendLayout();
			groupBox5.SuspendLayout();
			groupBox2.SuspendLayout();
			SuspendLayout();
			// 
			// btnReload
			// 
			btnReload.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnReload.BackColor = Color.FromArgb(40, 40, 40);
			btnReload.Enabled = false;
			btnReload.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnReload.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnReload.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnReload.FlatStyle = FlatStyle.Flat;
			btnReload.ForeColor = Color.FromArgb(208, 203, 148);
			btnReload.Location = new Point(13, 502);
			btnReload.Margin = new Padding(4, 3, 4, 3);
			btnReload.Name = "btnReload";
			btnReload.Size = new Size(70, 27);
			btnReload.TabIndex = 0;
			btnReload.Text = "Reload";
			btnReload.UseVisualStyleBackColor = false;
			btnReload.Click += btnReload_Click;
			// 
			// btnAddNew
			// 
			btnAddNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnAddNew.BackColor = Color.FromArgb(40, 40, 40);
			btnAddNew.Enabled = false;
			btnAddNew.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnAddNew.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnAddNew.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnAddNew.FlatStyle = FlatStyle.Flat;
			btnAddNew.ForeColor = Color.FromArgb(208, 203, 148);
			btnAddNew.Location = new Point(91, 502);
			btnAddNew.Margin = new Padding(4, 3, 4, 3);
			btnAddNew.Name = "btnAddNew";
			btnAddNew.Size = new Size(70, 27);
			btnAddNew.TabIndex = 0;
			btnAddNew.Text = "Add New";
			btnAddNew.UseVisualStyleBackColor = false;
			btnAddNew.Click += btnAddNew_Click;
			// 
			// MainList
			// 
			MainList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			MainList.BackColor = Color.FromArgb(28, 30, 31);
			MainList.BorderStyle = BorderStyle.FixedSingle;
			MainList.Enabled = false;
			MainList.ForeColor = Color.FromArgb(208, 203, 148);
			MainList.FormattingEnabled = true;
			MainList.Location = new Point(13, 41);
			MainList.Margin = new Padding(4, 3, 4, 3);
			MainList.Name = "MainList";
			MainList.Size = new Size(304, 452);
			MainList.TabIndex = 1;
			MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBox1.Controls.Add(cbTypeSelector);
			groupBox1.Controls.Add(label7);
			groupBox1.Controls.Add(groupBox3);
			groupBox1.Controls.Add(groupBox5);
			groupBox1.Controls.Add(btnUpdate);
			groupBox1.Controls.Add(cbGradeSelector);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(groupBox2);
			groupBox1.Controls.Add(tbID);
			groupBox1.FlatStyle = FlatStyle.Flat;
			groupBox1.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox1.ImeMode = ImeMode.On;
			groupBox1.Location = new Point(325, 12);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(686, 517);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Rare Option Data";
			// 
			// cbTypeSelector
			// 
			cbTypeSelector.AllowDrop = true;
			cbTypeSelector.BackColor = Color.FromArgb(28, 30, 31);
			cbTypeSelector.DropDownStyle = ComboBoxStyle.DropDownList;
			cbTypeSelector.FlatStyle = FlatStyle.Flat;
			cbTypeSelector.ForeColor = Color.FromArgb(208, 203, 148);
			cbTypeSelector.FormattingEnabled = true;
			cbTypeSelector.Location = new Point(48, 170);
			cbTypeSelector.Margin = new Padding(4, 3, 4, 3);
			cbTypeSelector.Name = "cbTypeSelector";
			cbTypeSelector.Size = new Size(192, 23);
			cbTypeSelector.TabIndex = 1038;
			cbTypeSelector.Visible = false;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.ForeColor = Color.FromArgb(208, 203, 148);
			label7.Location = new Point(8, 174);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new Size(32, 15);
			label7.TabIndex = 1039;
			label7.Text = "Type";
			label7.TextAlign = ContentAlignment.MiddleRight;
			label7.Visible = false;
			// 
			// groupBox3
			// 
			groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			groupBox3.Controls.Add(lOptionProbPercentage9);
			groupBox3.Controls.Add(cbOptionLevel9);
			groupBox3.Controls.Add(label40);
			groupBox3.Controls.Add(tbOptionProb9);
			groupBox3.Controls.Add(label41);
			groupBox3.Controls.Add(cbOptionID9);
			groupBox3.Controls.Add(label42);
			groupBox3.Controls.Add(lOptionProbPercentage8);
			groupBox3.Controls.Add(cbOptionLevel8);
			groupBox3.Controls.Add(label44);
			groupBox3.Controls.Add(tbOptionProb8);
			groupBox3.Controls.Add(label45);
			groupBox3.Controls.Add(cbOptionID8);
			groupBox3.Controls.Add(label46);
			groupBox3.Controls.Add(lOptionProbPercentage7);
			groupBox3.Controls.Add(cbOptionLevel7);
			groupBox3.Controls.Add(label32);
			groupBox3.Controls.Add(tbOptionProb7);
			groupBox3.Controls.Add(label33);
			groupBox3.Controls.Add(cbOptionID7);
			groupBox3.Controls.Add(label34);
			groupBox3.Controls.Add(lOptionProbPercentage6);
			groupBox3.Controls.Add(cbOptionLevel6);
			groupBox3.Controls.Add(label36);
			groupBox3.Controls.Add(tbOptionProb6);
			groupBox3.Controls.Add(label37);
			groupBox3.Controls.Add(cbOptionID6);
			groupBox3.Controls.Add(label38);
			groupBox3.Controls.Add(lOptionProbPercentage5);
			groupBox3.Controls.Add(cbOptionLevel5);
			groupBox3.Controls.Add(label24);
			groupBox3.Controls.Add(tbOptionProb5);
			groupBox3.Controls.Add(label25);
			groupBox3.Controls.Add(cbOptionID5);
			groupBox3.Controls.Add(label26);
			groupBox3.Controls.Add(lOptionProbPercentage4);
			groupBox3.Controls.Add(cbOptionLevel4);
			groupBox3.Controls.Add(label28);
			groupBox3.Controls.Add(tbOptionProb4);
			groupBox3.Controls.Add(label29);
			groupBox3.Controls.Add(cbOptionID4);
			groupBox3.Controls.Add(label30);
			groupBox3.Controls.Add(lOptionProbPercentage3);
			groupBox3.Controls.Add(cbOptionLevel3);
			groupBox3.Controls.Add(label16);
			groupBox3.Controls.Add(tbOptionProb3);
			groupBox3.Controls.Add(label17);
			groupBox3.Controls.Add(cbOptionID3);
			groupBox3.Controls.Add(label18);
			groupBox3.Controls.Add(lOptionProbPercentage2);
			groupBox3.Controls.Add(cbOptionLevel2);
			groupBox3.Controls.Add(label20);
			groupBox3.Controls.Add(tbOptionProb2);
			groupBox3.Controls.Add(label21);
			groupBox3.Controls.Add(cbOptionID2);
			groupBox3.Controls.Add(label22);
			groupBox3.Controls.Add(lOptionProbPercentage1);
			groupBox3.Controls.Add(cbOptionLevel1);
			groupBox3.Controls.Add(label6);
			groupBox3.Controls.Add(tbOptionProb1);
			groupBox3.Controls.Add(label12);
			groupBox3.Controls.Add(cbOptionID1);
			groupBox3.Controls.Add(label15);
			groupBox3.Controls.Add(lOptionProbPercentage0);
			groupBox3.Controls.Add(cbOptionLevel0);
			groupBox3.Controls.Add(label14);
			groupBox3.Controls.Add(tbOptionProb0);
			groupBox3.Controls.Add(label13);
			groupBox3.Controls.Add(cbOptionID0);
			groupBox3.Controls.Add(label4);
			groupBox3.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox3.Location = new Point(8, 199);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new Size(670, 312);
			groupBox3.TabIndex = 1049;
			groupBox3.TabStop = false;
			groupBox3.Text = "Options";
			// 
			// lOptionProbPercentage9
			// 
			lOptionProbPercentage9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage9.AutoSize = true;
			lOptionProbPercentage9.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage9.Location = new Point(627, 287);
			lOptionProbPercentage9.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage9.Name = "lOptionProbPercentage9";
			lOptionProbPercentage9.Size = new Size(12, 15);
			lOptionProbPercentage9.TabIndex = 1165;
			lOptionProbPercentage9.Text = "-";
			lOptionProbPercentage9.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel9
			// 
			cbOptionLevel9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel9.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel9.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel9.FlatStyle = FlatStyle.Flat;
			cbOptionLevel9.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel9.FormattingEnabled = true;
			cbOptionLevel9.Location = new Point(370, 283);
			cbOptionLevel9.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel9.Name = "cbOptionLevel9";
			cbOptionLevel9.Size = new Size(155, 23);
			cbOptionLevel9.TabIndex = 1164;
			cbOptionLevel9.SelectedIndexChanged += cbOptionLevel9_SelectedIndexChanged;
			// 
			// label40
			// 
			label40.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label40.AutoSize = true;
			label40.ForeColor = Color.FromArgb(208, 203, 148);
			label40.Location = new Point(533, 287);
			label40.Margin = new Padding(4, 0, 4, 0);
			label40.Name = "label40";
			label40.Size = new Size(32, 15);
			label40.TabIndex = 1163;
			label40.Text = "Prob";
			label40.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb9
			// 
			tbOptionProb9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb9.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb9.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb9.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb9.Location = new Point(573, 283);
			tbOptionProb9.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb9.Name = "tbOptionProb9";
			tbOptionProb9.Size = new Size(46, 23);
			tbOptionProb9.TabIndex = 1162;
			tbOptionProb9.TextAlign = HorizontalAlignment.Center;
			tbOptionProb9.TextChanged += tbOptionProb9_TextChanged;
			// 
			// label41
			// 
			label41.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label41.AutoSize = true;
			label41.ForeColor = Color.FromArgb(208, 203, 148);
			label41.Location = new Point(328, 287);
			label41.Margin = new Padding(4, 0, 4, 0);
			label41.Name = "label41";
			label41.Size = new Size(34, 15);
			label41.TabIndex = 1161;
			label41.Text = "Level";
			label41.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID9
			// 
			cbOptionID9.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID9.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID9.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID9.FlatStyle = FlatStyle.Flat;
			cbOptionID9.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID9.FormattingEnabled = true;
			cbOptionID9.Location = new Point(69, 283);
			cbOptionID9.Margin = new Padding(4, 3, 4, 3);
			cbOptionID9.Name = "cbOptionID9";
			cbOptionID9.Size = new Size(251, 23);
			cbOptionID9.TabIndex = 1160;
			cbOptionID9.SelectedIndexChanged += cbOptionID9_SelectedIndexChanged;
			// 
			// label42
			// 
			label42.AutoSize = true;
			label42.ForeColor = Color.FromArgb(208, 203, 148);
			label42.Location = new Point(8, 287);
			label42.Margin = new Padding(4, 0, 4, 0);
			label42.Name = "label42";
			label42.Size = new Size(53, 15);
			label42.TabIndex = 1159;
			label42.Text = "Option 9";
			label42.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage8
			// 
			lOptionProbPercentage8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage8.AutoSize = true;
			lOptionProbPercentage8.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage8.Location = new Point(627, 258);
			lOptionProbPercentage8.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage8.Name = "lOptionProbPercentage8";
			lOptionProbPercentage8.Size = new Size(12, 15);
			lOptionProbPercentage8.TabIndex = 1158;
			lOptionProbPercentage8.Text = "-";
			lOptionProbPercentage8.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel8
			// 
			cbOptionLevel8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel8.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel8.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel8.FlatStyle = FlatStyle.Flat;
			cbOptionLevel8.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel8.FormattingEnabled = true;
			cbOptionLevel8.Location = new Point(370, 254);
			cbOptionLevel8.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel8.Name = "cbOptionLevel8";
			cbOptionLevel8.Size = new Size(155, 23);
			cbOptionLevel8.TabIndex = 1157;
			cbOptionLevel8.SelectedIndexChanged += cbOptionLevel8_SelectedIndexChanged;
			// 
			// label44
			// 
			label44.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label44.AutoSize = true;
			label44.ForeColor = Color.FromArgb(208, 203, 148);
			label44.Location = new Point(533, 258);
			label44.Margin = new Padding(4, 0, 4, 0);
			label44.Name = "label44";
			label44.Size = new Size(32, 15);
			label44.TabIndex = 1156;
			label44.Text = "Prob";
			label44.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb8
			// 
			tbOptionProb8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb8.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb8.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb8.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb8.Location = new Point(573, 254);
			tbOptionProb8.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb8.Name = "tbOptionProb8";
			tbOptionProb8.Size = new Size(46, 23);
			tbOptionProb8.TabIndex = 1155;
			tbOptionProb8.TextAlign = HorizontalAlignment.Center;
			tbOptionProb8.TextChanged += tbOptionProb8_TextChanged;
			// 
			// label45
			// 
			label45.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label45.AutoSize = true;
			label45.ForeColor = Color.FromArgb(208, 203, 148);
			label45.Location = new Point(328, 258);
			label45.Margin = new Padding(4, 0, 4, 0);
			label45.Name = "label45";
			label45.Size = new Size(34, 15);
			label45.TabIndex = 1154;
			label45.Text = "Level";
			label45.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID8
			// 
			cbOptionID8.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID8.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID8.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID8.FlatStyle = FlatStyle.Flat;
			cbOptionID8.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID8.FormattingEnabled = true;
			cbOptionID8.Location = new Point(69, 254);
			cbOptionID8.Margin = new Padding(4, 3, 4, 3);
			cbOptionID8.Name = "cbOptionID8";
			cbOptionID8.Size = new Size(251, 23);
			cbOptionID8.TabIndex = 1153;
			cbOptionID8.SelectedIndexChanged += cbOptionID8_SelectedIndexChanged;
			// 
			// label46
			// 
			label46.AutoSize = true;
			label46.ForeColor = Color.FromArgb(208, 203, 148);
			label46.Location = new Point(8, 258);
			label46.Margin = new Padding(4, 0, 4, 0);
			label46.Name = "label46";
			label46.Size = new Size(53, 15);
			label46.TabIndex = 1152;
			label46.Text = "Option 8";
			label46.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage7
			// 
			lOptionProbPercentage7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage7.AutoSize = true;
			lOptionProbPercentage7.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage7.Location = new Point(627, 229);
			lOptionProbPercentage7.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage7.Name = "lOptionProbPercentage7";
			lOptionProbPercentage7.Size = new Size(12, 15);
			lOptionProbPercentage7.TabIndex = 1151;
			lOptionProbPercentage7.Text = "-";
			lOptionProbPercentage7.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel7
			// 
			cbOptionLevel7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel7.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel7.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel7.FlatStyle = FlatStyle.Flat;
			cbOptionLevel7.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel7.FormattingEnabled = true;
			cbOptionLevel7.Location = new Point(370, 225);
			cbOptionLevel7.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel7.Name = "cbOptionLevel7";
			cbOptionLevel7.Size = new Size(155, 23);
			cbOptionLevel7.TabIndex = 1150;
			cbOptionLevel7.SelectedIndexChanged += cbOptionLevel7_SelectedIndexChanged;
			// 
			// label32
			// 
			label32.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label32.AutoSize = true;
			label32.ForeColor = Color.FromArgb(208, 203, 148);
			label32.Location = new Point(533, 229);
			label32.Margin = new Padding(4, 0, 4, 0);
			label32.Name = "label32";
			label32.Size = new Size(32, 15);
			label32.TabIndex = 1149;
			label32.Text = "Prob";
			label32.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb7
			// 
			tbOptionProb7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb7.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb7.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb7.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb7.Location = new Point(573, 225);
			tbOptionProb7.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb7.Name = "tbOptionProb7";
			tbOptionProb7.Size = new Size(46, 23);
			tbOptionProb7.TabIndex = 1148;
			tbOptionProb7.TextAlign = HorizontalAlignment.Center;
			tbOptionProb7.TextChanged += tbOptionProb7_TextChanged;
			// 
			// label33
			// 
			label33.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label33.AutoSize = true;
			label33.ForeColor = Color.FromArgb(208, 203, 148);
			label33.Location = new Point(328, 229);
			label33.Margin = new Padding(4, 0, 4, 0);
			label33.Name = "label33";
			label33.Size = new Size(34, 15);
			label33.TabIndex = 1147;
			label33.Text = "Level";
			label33.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID7
			// 
			cbOptionID7.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID7.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID7.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID7.FlatStyle = FlatStyle.Flat;
			cbOptionID7.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID7.FormattingEnabled = true;
			cbOptionID7.Location = new Point(69, 225);
			cbOptionID7.Margin = new Padding(4, 3, 4, 3);
			cbOptionID7.Name = "cbOptionID7";
			cbOptionID7.Size = new Size(251, 23);
			cbOptionID7.TabIndex = 1146;
			cbOptionID7.SelectedIndexChanged += cbOptionID7_SelectedIndexChanged;
			// 
			// label34
			// 
			label34.AutoSize = true;
			label34.ForeColor = Color.FromArgb(208, 203, 148);
			label34.Location = new Point(8, 229);
			label34.Margin = new Padding(4, 0, 4, 0);
			label34.Name = "label34";
			label34.Size = new Size(53, 15);
			label34.TabIndex = 1145;
			label34.Text = "Option 7";
			label34.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage6
			// 
			lOptionProbPercentage6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage6.AutoSize = true;
			lOptionProbPercentage6.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage6.Location = new Point(627, 200);
			lOptionProbPercentage6.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage6.Name = "lOptionProbPercentage6";
			lOptionProbPercentage6.Size = new Size(12, 15);
			lOptionProbPercentage6.TabIndex = 1144;
			lOptionProbPercentage6.Text = "-";
			lOptionProbPercentage6.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel6
			// 
			cbOptionLevel6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel6.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel6.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel6.FlatStyle = FlatStyle.Flat;
			cbOptionLevel6.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel6.FormattingEnabled = true;
			cbOptionLevel6.Location = new Point(370, 196);
			cbOptionLevel6.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel6.Name = "cbOptionLevel6";
			cbOptionLevel6.Size = new Size(155, 23);
			cbOptionLevel6.TabIndex = 1143;
			cbOptionLevel6.SelectedIndexChanged += cbOptionLevel6_SelectedIndexChanged;
			// 
			// label36
			// 
			label36.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label36.AutoSize = true;
			label36.ForeColor = Color.FromArgb(208, 203, 148);
			label36.Location = new Point(533, 200);
			label36.Margin = new Padding(4, 0, 4, 0);
			label36.Name = "label36";
			label36.Size = new Size(32, 15);
			label36.TabIndex = 1142;
			label36.Text = "Prob";
			label36.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb6
			// 
			tbOptionProb6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb6.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb6.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb6.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb6.Location = new Point(573, 196);
			tbOptionProb6.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb6.Name = "tbOptionProb6";
			tbOptionProb6.Size = new Size(46, 23);
			tbOptionProb6.TabIndex = 1141;
			tbOptionProb6.TextAlign = HorizontalAlignment.Center;
			tbOptionProb6.TextChanged += tbOptionProb6_TextChanged;
			// 
			// label37
			// 
			label37.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label37.AutoSize = true;
			label37.ForeColor = Color.FromArgb(208, 203, 148);
			label37.Location = new Point(328, 200);
			label37.Margin = new Padding(4, 0, 4, 0);
			label37.Name = "label37";
			label37.Size = new Size(34, 15);
			label37.TabIndex = 1140;
			label37.Text = "Level";
			label37.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID6
			// 
			cbOptionID6.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID6.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID6.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID6.FlatStyle = FlatStyle.Flat;
			cbOptionID6.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID6.FormattingEnabled = true;
			cbOptionID6.Location = new Point(69, 196);
			cbOptionID6.Margin = new Padding(4, 3, 4, 3);
			cbOptionID6.Name = "cbOptionID6";
			cbOptionID6.Size = new Size(251, 23);
			cbOptionID6.TabIndex = 1139;
			cbOptionID6.SelectedIndexChanged += cbOptionID6_SelectedIndexChanged;
			// 
			// label38
			// 
			label38.AutoSize = true;
			label38.ForeColor = Color.FromArgb(208, 203, 148);
			label38.Location = new Point(8, 200);
			label38.Margin = new Padding(4, 0, 4, 0);
			label38.Name = "label38";
			label38.Size = new Size(53, 15);
			label38.TabIndex = 1138;
			label38.Text = "Option 6";
			label38.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage5
			// 
			lOptionProbPercentage5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage5.AutoSize = true;
			lOptionProbPercentage5.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage5.Location = new Point(627, 171);
			lOptionProbPercentage5.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage5.Name = "lOptionProbPercentage5";
			lOptionProbPercentage5.Size = new Size(12, 15);
			lOptionProbPercentage5.TabIndex = 1137;
			lOptionProbPercentage5.Text = "-";
			lOptionProbPercentage5.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel5
			// 
			cbOptionLevel5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel5.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel5.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel5.FlatStyle = FlatStyle.Flat;
			cbOptionLevel5.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel5.FormattingEnabled = true;
			cbOptionLevel5.Location = new Point(370, 167);
			cbOptionLevel5.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel5.Name = "cbOptionLevel5";
			cbOptionLevel5.Size = new Size(155, 23);
			cbOptionLevel5.TabIndex = 1136;
			cbOptionLevel5.SelectedIndexChanged += cbOptionLevel5_SelectedIndexChanged;
			// 
			// label24
			// 
			label24.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label24.AutoSize = true;
			label24.ForeColor = Color.FromArgb(208, 203, 148);
			label24.Location = new Point(533, 171);
			label24.Margin = new Padding(4, 0, 4, 0);
			label24.Name = "label24";
			label24.Size = new Size(32, 15);
			label24.TabIndex = 1135;
			label24.Text = "Prob";
			label24.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb5
			// 
			tbOptionProb5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb5.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb5.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb5.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb5.Location = new Point(573, 167);
			tbOptionProb5.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb5.Name = "tbOptionProb5";
			tbOptionProb5.Size = new Size(46, 23);
			tbOptionProb5.TabIndex = 1134;
			tbOptionProb5.TextAlign = HorizontalAlignment.Center;
			tbOptionProb5.TextChanged += tbOptionProb5_TextChanged;
			// 
			// label25
			// 
			label25.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label25.AutoSize = true;
			label25.ForeColor = Color.FromArgb(208, 203, 148);
			label25.Location = new Point(328, 171);
			label25.Margin = new Padding(4, 0, 4, 0);
			label25.Name = "label25";
			label25.Size = new Size(34, 15);
			label25.TabIndex = 1133;
			label25.Text = "Level";
			label25.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID5
			// 
			cbOptionID5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID5.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID5.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID5.FlatStyle = FlatStyle.Flat;
			cbOptionID5.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID5.FormattingEnabled = true;
			cbOptionID5.Location = new Point(69, 167);
			cbOptionID5.Margin = new Padding(4, 3, 4, 3);
			cbOptionID5.Name = "cbOptionID5";
			cbOptionID5.Size = new Size(251, 23);
			cbOptionID5.TabIndex = 1132;
			cbOptionID5.SelectedIndexChanged += cbOptionID5_SelectedIndexChanged;
			// 
			// label26
			// 
			label26.AutoSize = true;
			label26.ForeColor = Color.FromArgb(208, 203, 148);
			label26.Location = new Point(8, 171);
			label26.Margin = new Padding(4, 0, 4, 0);
			label26.Name = "label26";
			label26.Size = new Size(53, 15);
			label26.TabIndex = 1131;
			label26.Text = "Option 5";
			label26.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage4
			// 
			lOptionProbPercentage4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage4.AutoSize = true;
			lOptionProbPercentage4.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage4.Location = new Point(627, 142);
			lOptionProbPercentage4.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage4.Name = "lOptionProbPercentage4";
			lOptionProbPercentage4.Size = new Size(12, 15);
			lOptionProbPercentage4.TabIndex = 1130;
			lOptionProbPercentage4.Text = "-";
			lOptionProbPercentage4.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel4
			// 
			cbOptionLevel4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel4.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel4.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel4.FlatStyle = FlatStyle.Flat;
			cbOptionLevel4.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel4.FormattingEnabled = true;
			cbOptionLevel4.Location = new Point(370, 138);
			cbOptionLevel4.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel4.Name = "cbOptionLevel4";
			cbOptionLevel4.Size = new Size(155, 23);
			cbOptionLevel4.TabIndex = 1129;
			cbOptionLevel4.SelectedIndexChanged += cbOptionLevel4_SelectedIndexChanged;
			// 
			// label28
			// 
			label28.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label28.AutoSize = true;
			label28.ForeColor = Color.FromArgb(208, 203, 148);
			label28.Location = new Point(533, 142);
			label28.Margin = new Padding(4, 0, 4, 0);
			label28.Name = "label28";
			label28.Size = new Size(32, 15);
			label28.TabIndex = 1128;
			label28.Text = "Prob";
			label28.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb4
			// 
			tbOptionProb4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb4.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb4.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb4.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb4.Location = new Point(573, 138);
			tbOptionProb4.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb4.Name = "tbOptionProb4";
			tbOptionProb4.Size = new Size(46, 23);
			tbOptionProb4.TabIndex = 1127;
			tbOptionProb4.TextAlign = HorizontalAlignment.Center;
			tbOptionProb4.TextChanged += tbOptionProb4_TextChanged;
			// 
			// label29
			// 
			label29.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label29.AutoSize = true;
			label29.ForeColor = Color.FromArgb(208, 203, 148);
			label29.Location = new Point(328, 142);
			label29.Margin = new Padding(4, 0, 4, 0);
			label29.Name = "label29";
			label29.Size = new Size(34, 15);
			label29.TabIndex = 1126;
			label29.Text = "Level";
			label29.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID4
			// 
			cbOptionID4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID4.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID4.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID4.FlatStyle = FlatStyle.Flat;
			cbOptionID4.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID4.FormattingEnabled = true;
			cbOptionID4.Location = new Point(69, 138);
			cbOptionID4.Margin = new Padding(4, 3, 4, 3);
			cbOptionID4.Name = "cbOptionID4";
			cbOptionID4.Size = new Size(251, 23);
			cbOptionID4.TabIndex = 1125;
			cbOptionID4.SelectedIndexChanged += cbOptionID4_SelectedIndexChanged;
			// 
			// label30
			// 
			label30.AutoSize = true;
			label30.ForeColor = Color.FromArgb(208, 203, 148);
			label30.Location = new Point(8, 142);
			label30.Margin = new Padding(4, 0, 4, 0);
			label30.Name = "label30";
			label30.Size = new Size(53, 15);
			label30.TabIndex = 1124;
			label30.Text = "Option 4";
			label30.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage3
			// 
			lOptionProbPercentage3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage3.AutoSize = true;
			lOptionProbPercentage3.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage3.Location = new Point(627, 113);
			lOptionProbPercentage3.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage3.Name = "lOptionProbPercentage3";
			lOptionProbPercentage3.Size = new Size(12, 15);
			lOptionProbPercentage3.TabIndex = 1123;
			lOptionProbPercentage3.Text = "-";
			lOptionProbPercentage3.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel3
			// 
			cbOptionLevel3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel3.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel3.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel3.FlatStyle = FlatStyle.Flat;
			cbOptionLevel3.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel3.FormattingEnabled = true;
			cbOptionLevel3.Location = new Point(370, 109);
			cbOptionLevel3.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel3.Name = "cbOptionLevel3";
			cbOptionLevel3.Size = new Size(155, 23);
			cbOptionLevel3.TabIndex = 1122;
			cbOptionLevel3.SelectedIndexChanged += cbOptionLevel3_SelectedIndexChanged;
			// 
			// label16
			// 
			label16.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label16.AutoSize = true;
			label16.ForeColor = Color.FromArgb(208, 203, 148);
			label16.Location = new Point(533, 113);
			label16.Margin = new Padding(4, 0, 4, 0);
			label16.Name = "label16";
			label16.Size = new Size(32, 15);
			label16.TabIndex = 1121;
			label16.Text = "Prob";
			label16.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb3
			// 
			tbOptionProb3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb3.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb3.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb3.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb3.Location = new Point(573, 109);
			tbOptionProb3.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb3.Name = "tbOptionProb3";
			tbOptionProb3.Size = new Size(46, 23);
			tbOptionProb3.TabIndex = 1120;
			tbOptionProb3.TextAlign = HorizontalAlignment.Center;
			tbOptionProb3.TextChanged += tbOptionProb3_TextChanged;
			// 
			// label17
			// 
			label17.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label17.AutoSize = true;
			label17.ForeColor = Color.FromArgb(208, 203, 148);
			label17.Location = new Point(328, 113);
			label17.Margin = new Padding(4, 0, 4, 0);
			label17.Name = "label17";
			label17.Size = new Size(34, 15);
			label17.TabIndex = 1119;
			label17.Text = "Level";
			label17.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID3
			// 
			cbOptionID3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID3.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID3.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID3.FlatStyle = FlatStyle.Flat;
			cbOptionID3.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID3.FormattingEnabled = true;
			cbOptionID3.Location = new Point(69, 109);
			cbOptionID3.Margin = new Padding(4, 3, 4, 3);
			cbOptionID3.Name = "cbOptionID3";
			cbOptionID3.Size = new Size(251, 23);
			cbOptionID3.TabIndex = 1118;
			cbOptionID3.SelectedIndexChanged += cbOptionID3_SelectedIndexChanged;
			// 
			// label18
			// 
			label18.AutoSize = true;
			label18.ForeColor = Color.FromArgb(208, 203, 148);
			label18.Location = new Point(8, 113);
			label18.Margin = new Padding(4, 0, 4, 0);
			label18.Name = "label18";
			label18.Size = new Size(53, 15);
			label18.TabIndex = 1117;
			label18.Text = "Option 3";
			label18.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage2
			// 
			lOptionProbPercentage2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage2.AutoSize = true;
			lOptionProbPercentage2.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage2.Location = new Point(627, 84);
			lOptionProbPercentage2.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage2.Name = "lOptionProbPercentage2";
			lOptionProbPercentage2.Size = new Size(12, 15);
			lOptionProbPercentage2.TabIndex = 1116;
			lOptionProbPercentage2.Text = "-";
			lOptionProbPercentage2.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel2
			// 
			cbOptionLevel2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel2.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel2.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel2.FlatStyle = FlatStyle.Flat;
			cbOptionLevel2.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel2.FormattingEnabled = true;
			cbOptionLevel2.Location = new Point(370, 80);
			cbOptionLevel2.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel2.Name = "cbOptionLevel2";
			cbOptionLevel2.Size = new Size(155, 23);
			cbOptionLevel2.TabIndex = 1115;
			cbOptionLevel2.SelectedIndexChanged += cbOptionLevel2_SelectedIndexChanged;
			// 
			// label20
			// 
			label20.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label20.AutoSize = true;
			label20.ForeColor = Color.FromArgb(208, 203, 148);
			label20.Location = new Point(533, 84);
			label20.Margin = new Padding(4, 0, 4, 0);
			label20.Name = "label20";
			label20.Size = new Size(32, 15);
			label20.TabIndex = 1114;
			label20.Text = "Prob";
			label20.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb2
			// 
			tbOptionProb2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb2.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb2.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb2.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb2.Location = new Point(573, 80);
			tbOptionProb2.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb2.Name = "tbOptionProb2";
			tbOptionProb2.Size = new Size(46, 23);
			tbOptionProb2.TabIndex = 1113;
			tbOptionProb2.TextAlign = HorizontalAlignment.Center;
			tbOptionProb2.TextChanged += tbOptionProb2_TextChanged;
			// 
			// label21
			// 
			label21.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label21.AutoSize = true;
			label21.ForeColor = Color.FromArgb(208, 203, 148);
			label21.Location = new Point(328, 84);
			label21.Margin = new Padding(4, 0, 4, 0);
			label21.Name = "label21";
			label21.Size = new Size(34, 15);
			label21.TabIndex = 1112;
			label21.Text = "Level";
			label21.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID2
			// 
			cbOptionID2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID2.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID2.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID2.FlatStyle = FlatStyle.Flat;
			cbOptionID2.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID2.FormattingEnabled = true;
			cbOptionID2.Location = new Point(69, 80);
			cbOptionID2.Margin = new Padding(4, 3, 4, 3);
			cbOptionID2.Name = "cbOptionID2";
			cbOptionID2.Size = new Size(251, 23);
			cbOptionID2.TabIndex = 1111;
			cbOptionID2.SelectedIndexChanged += cbOptionID2_SelectedIndexChanged;
			// 
			// label22
			// 
			label22.AutoSize = true;
			label22.ForeColor = Color.FromArgb(208, 203, 148);
			label22.Location = new Point(8, 84);
			label22.Margin = new Padding(4, 0, 4, 0);
			label22.Name = "label22";
			label22.Size = new Size(53, 15);
			label22.TabIndex = 1110;
			label22.Text = "Option 2";
			label22.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage1
			// 
			lOptionProbPercentage1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage1.AutoSize = true;
			lOptionProbPercentage1.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage1.Location = new Point(627, 55);
			lOptionProbPercentage1.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage1.Name = "lOptionProbPercentage1";
			lOptionProbPercentage1.Size = new Size(12, 15);
			lOptionProbPercentage1.TabIndex = 1109;
			lOptionProbPercentage1.Text = "-";
			lOptionProbPercentage1.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel1
			// 
			cbOptionLevel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel1.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel1.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel1.FlatStyle = FlatStyle.Flat;
			cbOptionLevel1.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel1.FormattingEnabled = true;
			cbOptionLevel1.Location = new Point(370, 51);
			cbOptionLevel1.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel1.Name = "cbOptionLevel1";
			cbOptionLevel1.Size = new Size(155, 23);
			cbOptionLevel1.TabIndex = 1108;
			cbOptionLevel1.SelectedIndexChanged += cbOptionLevel1_SelectedIndexChanged;
			// 
			// label6
			// 
			label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label6.AutoSize = true;
			label6.ForeColor = Color.FromArgb(208, 203, 148);
			label6.Location = new Point(533, 55);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new Size(32, 15);
			label6.TabIndex = 1107;
			label6.Text = "Prob";
			label6.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb1
			// 
			tbOptionProb1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb1.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb1.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb1.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb1.Location = new Point(573, 51);
			tbOptionProb1.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb1.Name = "tbOptionProb1";
			tbOptionProb1.Size = new Size(46, 23);
			tbOptionProb1.TabIndex = 1106;
			tbOptionProb1.TextAlign = HorizontalAlignment.Center;
			tbOptionProb1.TextChanged += tbOptionProb1_TextChanged;
			// 
			// label12
			// 
			label12.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label12.AutoSize = true;
			label12.ForeColor = Color.FromArgb(208, 203, 148);
			label12.Location = new Point(328, 55);
			label12.Margin = new Padding(4, 0, 4, 0);
			label12.Name = "label12";
			label12.Size = new Size(34, 15);
			label12.TabIndex = 1105;
			label12.Text = "Level";
			label12.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID1
			// 
			cbOptionID1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID1.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID1.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID1.FlatStyle = FlatStyle.Flat;
			cbOptionID1.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID1.FormattingEnabled = true;
			cbOptionID1.Location = new Point(69, 51);
			cbOptionID1.Margin = new Padding(4, 3, 4, 3);
			cbOptionID1.Name = "cbOptionID1";
			cbOptionID1.Size = new Size(251, 23);
			cbOptionID1.TabIndex = 1104;
			cbOptionID1.SelectedIndexChanged += cbOptionID1_SelectedIndexChanged;
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.ForeColor = Color.FromArgb(208, 203, 148);
			label15.Location = new Point(8, 55);
			label15.Margin = new Padding(4, 0, 4, 0);
			label15.Name = "label15";
			label15.Size = new Size(53, 15);
			label15.TabIndex = 1103;
			label15.Text = "Option 1";
			label15.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lOptionProbPercentage0
			// 
			lOptionProbPercentage0.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lOptionProbPercentage0.AutoSize = true;
			lOptionProbPercentage0.ForeColor = Color.FromArgb(208, 203, 148);
			lOptionProbPercentage0.Location = new Point(627, 26);
			lOptionProbPercentage0.Margin = new Padding(4, 0, 4, 0);
			lOptionProbPercentage0.Name = "lOptionProbPercentage0";
			lOptionProbPercentage0.Size = new Size(12, 15);
			lOptionProbPercentage0.TabIndex = 1102;
			lOptionProbPercentage0.Text = "-";
			lOptionProbPercentage0.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// cbOptionLevel0
			// 
			cbOptionLevel0.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel0.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel0.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel0.FlatStyle = FlatStyle.Flat;
			cbOptionLevel0.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel0.FormattingEnabled = true;
			cbOptionLevel0.Location = new Point(370, 22);
			cbOptionLevel0.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel0.Name = "cbOptionLevel0";
			cbOptionLevel0.Size = new Size(155, 23);
			cbOptionLevel0.TabIndex = 1054;
			cbOptionLevel0.SelectedIndexChanged += cbOptionLevel0_SelectedIndexChanged;
			// 
			// label14
			// 
			label14.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label14.AutoSize = true;
			label14.ForeColor = Color.FromArgb(208, 203, 148);
			label14.Location = new Point(533, 26);
			label14.Margin = new Padding(4, 0, 4, 0);
			label14.Name = "label14";
			label14.Size = new Size(32, 15);
			label14.TabIndex = 1053;
			label14.Text = "Prob";
			label14.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbOptionProb0
			// 
			tbOptionProb0.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbOptionProb0.BackColor = Color.FromArgb(28, 30, 31);
			tbOptionProb0.BorderStyle = BorderStyle.FixedSingle;
			tbOptionProb0.ForeColor = Color.FromArgb(208, 203, 148);
			tbOptionProb0.Location = new Point(573, 22);
			tbOptionProb0.Margin = new Padding(4, 3, 4, 3);
			tbOptionProb0.Name = "tbOptionProb0";
			tbOptionProb0.Size = new Size(46, 23);
			tbOptionProb0.TabIndex = 1052;
			tbOptionProb0.TextAlign = HorizontalAlignment.Center;
			tbOptionProb0.TextChanged += tbOptionProb0_TextChanged;
			// 
			// label13
			// 
			label13.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label13.AutoSize = true;
			label13.ForeColor = Color.FromArgb(208, 203, 148);
			label13.Location = new Point(328, 26);
			label13.Margin = new Padding(4, 0, 4, 0);
			label13.Name = "label13";
			label13.Size = new Size(34, 15);
			label13.TabIndex = 1051;
			label13.Text = "Level";
			label13.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionID0
			// 
			cbOptionID0.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbOptionID0.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionID0.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionID0.FlatStyle = FlatStyle.Flat;
			cbOptionID0.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionID0.FormattingEnabled = true;
			cbOptionID0.Location = new Point(69, 22);
			cbOptionID0.Margin = new Padding(4, 3, 4, 3);
			cbOptionID0.Name = "cbOptionID0";
			cbOptionID0.Size = new Size(251, 23);
			cbOptionID0.TabIndex = 1050;
			cbOptionID0.SelectedIndexChanged += cbOptionID0_SelectedIndexChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.ForeColor = Color.FromArgb(208, 203, 148);
			label4.Location = new Point(8, 26);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(53, 15);
			label4.TabIndex = 1041;
			label4.Text = "Option 0";
			label4.TextAlign = ContentAlignment.MiddleRight;
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(tbAttack);
			groupBox5.Controls.Add(tbResistance);
			groupBox5.Controls.Add(label8);
			groupBox5.Controls.Add(label11);
			groupBox5.Controls.Add(label9);
			groupBox5.Controls.Add(tbMagicAttack);
			groupBox5.Controls.Add(tbDefense);
			groupBox5.Controls.Add(label10);
			groupBox5.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox5.Location = new Point(297, 55);
			groupBox5.Margin = new Padding(4, 3, 4, 3);
			groupBox5.Name = "groupBox5";
			groupBox5.Padding = new Padding(4, 3, 4, 3);
			groupBox5.Size = new Size(172, 138);
			groupBox5.TabIndex = 1048;
			groupBox5.TabStop = false;
			groupBox5.Text = "Statistics";
			// 
			// tbAttack
			// 
			tbAttack.BackColor = Color.FromArgb(28, 30, 31);
			tbAttack.BorderStyle = BorderStyle.FixedSingle;
			tbAttack.ForeColor = Color.FromArgb(208, 203, 148);
			tbAttack.Location = new Point(94, 22);
			tbAttack.Margin = new Padding(4, 3, 4, 3);
			tbAttack.Name = "tbAttack";
			tbAttack.Size = new Size(70, 23);
			tbAttack.TabIndex = 1040;
			tbAttack.TextAlign = HorizontalAlignment.Center;
			tbAttack.TextChanged += tbAttack_TextChanged;
			// 
			// tbResistance
			// 
			tbResistance.BackColor = Color.FromArgb(28, 30, 31);
			tbResistance.BorderStyle = BorderStyle.FixedSingle;
			tbResistance.ForeColor = Color.FromArgb(208, 203, 148);
			tbResistance.Location = new Point(94, 109);
			tbResistance.Margin = new Padding(4, 3, 4, 3);
			tbResistance.Name = "tbResistance";
			tbResistance.Size = new Size(70, 23);
			tbResistance.TabIndex = 1046;
			tbResistance.TextAlign = HorizontalAlignment.Center;
			tbResistance.TextChanged += tbResistance_TextChanged;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.ForeColor = Color.FromArgb(208, 203, 148);
			label8.Location = new Point(45, 26);
			label8.Margin = new Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new Size(41, 15);
			label8.TabIndex = 1041;
			label8.Text = "Attack";
			label8.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.ForeColor = Color.FromArgb(208, 203, 148);
			label11.Location = new Point(24, 113);
			label11.Margin = new Padding(4, 0, 4, 0);
			label11.Name = "label11";
			label11.Size = new Size(62, 15);
			label11.TabIndex = 1047;
			label11.Text = "Resistance";
			label11.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.ForeColor = Color.FromArgb(208, 203, 148);
			label9.Location = new Point(37, 55);
			label9.Margin = new Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new Size(49, 15);
			label9.TabIndex = 1043;
			label9.Text = "Defense";
			label9.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbMagicAttack
			// 
			tbMagicAttack.BackColor = Color.FromArgb(28, 30, 31);
			tbMagicAttack.BorderStyle = BorderStyle.FixedSingle;
			tbMagicAttack.ForeColor = Color.FromArgb(208, 203, 148);
			tbMagicAttack.Location = new Point(94, 80);
			tbMagicAttack.Margin = new Padding(4, 3, 4, 3);
			tbMagicAttack.Name = "tbMagicAttack";
			tbMagicAttack.Size = new Size(70, 23);
			tbMagicAttack.TabIndex = 1044;
			tbMagicAttack.TextAlign = HorizontalAlignment.Center;
			tbMagicAttack.TextChanged += tbMagicAttack_TextChanged;
			// 
			// tbDefense
			// 
			tbDefense.BackColor = Color.FromArgb(28, 30, 31);
			tbDefense.BorderStyle = BorderStyle.FixedSingle;
			tbDefense.ForeColor = Color.FromArgb(208, 203, 148);
			tbDefense.Location = new Point(94, 51);
			tbDefense.Margin = new Padding(4, 3, 4, 3);
			tbDefense.Name = "tbDefense";
			tbDefense.Size = new Size(70, 23);
			tbDefense.TabIndex = 1042;
			tbDefense.TextAlign = HorizontalAlignment.Center;
			tbDefense.TextChanged += tbDefense_TextChanged;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.ForeColor = Color.FromArgb(208, 203, 148);
			label10.Location = new Point(9, 84);
			label10.Margin = new Padding(4, 0, 4, 0);
			label10.Name = "label10";
			label10.Size = new Size(77, 15);
			label10.TabIndex = 1045;
			label10.Text = "Magic Attack";
			label10.TextAlign = ContentAlignment.MiddleRight;
			// 
			// btnUpdate
			// 
			btnUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			btnUpdate.BackColor = Color.FromArgb(40, 40, 40);
			btnUpdate.Enabled = false;
			btnUpdate.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnUpdate.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnUpdate.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnUpdate.FlatStyle = FlatStyle.Flat;
			btnUpdate.Location = new Point(7, 22);
			btnUpdate.Margin = new Padding(4, 3, 4, 3);
			btnUpdate.Name = "btnUpdate";
			btnUpdate.Size = new Size(671, 27);
			btnUpdate.TabIndex = 999;
			btnUpdate.Text = "Update";
			btnUpdate.UseVisualStyleBackColor = false;
			btnUpdate.Click += btnUpdate_Click;
			// 
			// cbGradeSelector
			// 
			cbGradeSelector.BackColor = Color.FromArgb(28, 30, 31);
			cbGradeSelector.DropDownStyle = ComboBoxStyle.DropDownList;
			cbGradeSelector.FlatStyle = FlatStyle.Flat;
			cbGradeSelector.ForeColor = Color.Black;
			cbGradeSelector.FormattingEnabled = true;
			cbGradeSelector.Location = new Point(145, 55);
			cbGradeSelector.Margin = new Padding(4, 3, 4, 3);
			cbGradeSelector.Name = "cbGradeSelector";
			cbGradeSelector.Size = new Size(144, 23);
			cbGradeSelector.TabIndex = 16;
			cbGradeSelector.SelectedIndexChanged += cbGradeSelector_SelectedIndexChanged;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.ForeColor = Color.FromArgb(208, 203, 148);
			label1.Location = new Point(8, 59);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(18, 15);
			label1.TabIndex = 1;
			label1.Text = "ID";
			label1.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.ForeColor = Color.FromArgb(208, 203, 148);
			label3.Location = new Point(99, 59);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(38, 15);
			label3.TabIndex = 1006;
			label3.Text = "Grade";
			label3.TextAlign = ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(cbNationSelector);
			groupBox2.Controls.Add(tbName);
			groupBox2.Controls.Add(label2);
			groupBox2.FlatStyle = FlatStyle.Flat;
			groupBox2.Location = new Point(8, 84);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(281, 80);
			groupBox2.TabIndex = 1000;
			groupBox2.TabStop = false;
			// 
			// cbNationSelector
			// 
			cbNationSelector.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			cbNationSelector.BackColor = Color.FromArgb(28, 30, 31);
			cbNationSelector.DropDownStyle = ComboBoxStyle.DropDownList;
			cbNationSelector.ForeColor = Color.FromArgb(208, 203, 148);
			cbNationSelector.FormattingEnabled = true;
			cbNationSelector.Location = new Point(8, 22);
			cbNationSelector.Margin = new Padding(4, 3, 4, 3);
			cbNationSelector.Name = "cbNationSelector";
			cbNationSelector.Size = new Size(265, 23);
			cbNationSelector.TabIndex = 11;
			cbNationSelector.SelectedIndexChanged += cbNationSelector_SelectedIndexChanged;
			// 
			// tbName
			// 
			tbName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbName.BackColor = Color.FromArgb(28, 30, 31);
			tbName.BorderStyle = BorderStyle.FixedSingle;
			tbName.ForeColor = Color.FromArgb(208, 203, 148);
			tbName.Location = new Point(52, 51);
			tbName.Margin = new Padding(4, 3, 4, 3);
			tbName.Name = "tbName";
			tbName.Size = new Size(221, 23);
			tbName.TabIndex = 12;
			tbName.TextChanged += tbName_TextChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.ForeColor = Color.FromArgb(208, 203, 148);
			label2.Location = new Point(8, 55);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(36, 15);
			label2.TabIndex = 14;
			label2.Text = "Prefix";
			label2.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbID
			// 
			tbID.BackColor = Color.FromArgb(28, 30, 31);
			tbID.BorderStyle = BorderStyle.FixedSingle;
			tbID.ForeColor = Color.FromArgb(208, 203, 148);
			tbID.Location = new Point(34, 55);
			tbID.Margin = new Padding(4, 3, 4, 3);
			tbID.Name = "tbID";
			tbID.ReadOnly = true;
			tbID.Size = new Size(57, 23);
			tbID.TabIndex = 1;
			tbID.TextAlign = HorizontalAlignment.Center;
			// 
			// btnCopy
			// 
			btnCopy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnCopy.BackColor = Color.FromArgb(40, 40, 40);
			btnCopy.Enabled = false;
			btnCopy.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnCopy.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnCopy.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnCopy.FlatStyle = FlatStyle.Flat;
			btnCopy.ForeColor = Color.FromArgb(208, 203, 148);
			btnCopy.Location = new Point(169, 502);
			btnCopy.Margin = new Padding(4, 3, 4, 3);
			btnCopy.Name = "btnCopy";
			btnCopy.Size = new Size(70, 27);
			btnCopy.TabIndex = 0;
			btnCopy.Text = "Copy";
			btnCopy.UseVisualStyleBackColor = false;
			btnCopy.Click += btnCopy_Click;
			// 
			// btnDelete
			// 
			btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnDelete.BackColor = Color.FromArgb(40, 40, 40);
			btnDelete.Enabled = false;
			btnDelete.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnDelete.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnDelete.FlatStyle = FlatStyle.Flat;
			btnDelete.ForeColor = Color.FromArgb(208, 203, 148);
			btnDelete.Location = new Point(247, 502);
			btnDelete.Margin = new Padding(4, 3, 4, 3);
			btnDelete.Name = "btnDelete";
			btnDelete.Size = new Size(70, 27);
			btnDelete.TabIndex = 0;
			btnDelete.Text = "Delete";
			btnDelete.UseVisualStyleBackColor = false;
			btnDelete.Click += btnDelete_Click;
			// 
			// tbSearch
			// 
			tbSearch.BackColor = Color.FromArgb(28, 30, 31);
			tbSearch.BorderStyle = BorderStyle.FixedSingle;
			tbSearch.ForeColor = Color.FromArgb(208, 203, 148);
			tbSearch.Location = new Point(13, 12);
			tbSearch.Margin = new Padding(4, 3, 4, 3);
			tbSearch.Name = "tbSearch";
			tbSearch.Size = new Size(304, 23);
			tbSearch.TabIndex = 0;
			tbSearch.TextChanged += tbSearch_TextChanged;
			tbSearch.KeyDown += tbSearch_KeyDown;
			// 
			// RareOptionEditor
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(40, 40, 40);
			ClientSize = new Size(1024, 541);
			Controls.Add(tbSearch);
			Controls.Add(btnDelete);
			Controls.Add(btnCopy);
			Controls.Add(MainList);
			Controls.Add(btnAddNew);
			Controls.Add(btnReload);
			Controls.Add(groupBox1);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = Properties.Resources.NG;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimumSize = new Size(1040, 580);
			Name = "RareOptionEditor";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Rare Option Editor";
			FormClosing += RareOptionEditor_FormClosing;
			Load += RareOptionEditor_LoadAsync;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			groupBox5.ResumeLayout(false);
			groupBox5.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnReload;
		private System.Windows.Forms.Button btnAddNew;
		private System.Windows.Forms.ListBox MainList;
		private System.Windows.Forms.GroupBox groupBox1;
		private TextBox tbID;
		private Button btnUpdate;
		private Button btnCopy;
		private Button btnDelete;
		private Label label1;
		private Label label2;
		private TextBox tbName;
		private ComboBox cbNationSelector;
		private GroupBox groupBox2;
		private Label label3;
		private TextBox tbSearch;
		private ComboBox cbGradeSelector;
		private ComboBox cbTypeSelector;
		private Label label7;
		private TextBox tbAttack;
		private Label label8;
		private TextBox tbDefense;
		private Label label9;
		private TextBox tbMagicAttack;
		private Label label10;
		private TextBox tbResistance;
		private Label label11;
		private GroupBox groupBox5;
		private GroupBox groupBox3;
		private Label label4;
		private ComboBox cbOptionID0;
		private Label label14;
		private TextBox tbOptionProb0;
		private Label label13;
		private ComboBox cbOptionLevel0;
		private Label lOptionProbPercentage0;
		private Label lOptionProbPercentage1;
		private ComboBox cbOptionLevel1;
		private Label label6;
		private TextBox tbOptionProb1;
		private Label label12;
		private ComboBox cbOptionID1;
		private Label label15;
		private Label lOptionProbPercentage9;
		private ComboBox cbOptionLevel9;
		private Label label40;
		private TextBox tbOptionProb9;
		private Label label41;
		private ComboBox cbOptionID9;
		private Label label42;
		private Label lOptionProbPercentage8;
		private ComboBox cbOptionLevel8;
		private Label label44;
		private TextBox tbOptionProb8;
		private Label label45;
		private ComboBox cbOptionID8;
		private Label label46;
		private Label lOptionProbPercentage7;
		private ComboBox cbOptionLevel7;
		private Label label32;
		private TextBox tbOptionProb7;
		private Label label33;
		private ComboBox cbOptionID7;
		private Label label34;
		private Label lOptionProbPercentage6;
		private ComboBox cbOptionLevel6;
		private Label label36;
		private TextBox tbOptionProb6;
		private Label label37;
		private ComboBox cbOptionID6;
		private Label label38;
		private Label lOptionProbPercentage5;
		private ComboBox cbOptionLevel5;
		private Label label24;
		private TextBox tbOptionProb5;
		private Label label25;
		private ComboBox cbOptionID5;
		private Label label26;
		private Label lOptionProbPercentage4;
		private ComboBox cbOptionLevel4;
		private Label label28;
		private TextBox tbOptionProb4;
		private Label label29;
		private ComboBox cbOptionID4;
		private Label label30;
		private Label lOptionProbPercentage3;
		private ComboBox cbOptionLevel3;
		private Label label16;
		private TextBox tbOptionProb3;
		private Label label17;
		private ComboBox cbOptionID3;
		private Label label18;
		private Label lOptionProbPercentage2;
		private ComboBox cbOptionLevel2;
		private Label label20;
		private TextBox tbOptionProb2;
		private Label label21;
		private ComboBox cbOptionID2;
		private Label label22;
	}
}