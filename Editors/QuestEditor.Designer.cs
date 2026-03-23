using System.Windows.Forms;

namespace LastChaos_ToolBoxNG
{
	partial class QuestEditor
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
			cbEnable = new CheckBox();
			cbJob = new ComboBox();
			label7 = new Label();
			groupBox5 = new GroupBox();
			tbMinLevel = new TextBox();
			tbResistance = new TextBox();
			label8 = new Label();
			label11 = new Label();
			label9 = new Label();
			tbMagicAttack = new TextBox();
			tbDefense = new TextBox();
			label10 = new Label();
			btnUpdate = new Button();
			cbType0 = new ComboBox();
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
			tbStartDescription = new TextBox();
			label19 = new Label();
			tbRewardDescription = new TextBox();
			label23 = new Label();
			tbConditionDescription = new TextBox();
			label27 = new Label();
			cbType1 = new ComboBox();
			label5 = new Label();
			cbType2 = new ComboBox();
			label31 = new Label();
			tbMaxLevel = new TextBox();
			label35 = new Label();
			tbNeededExperience = new TextBox();
			label39 = new Label();
			cbStartFrom = new ComboBox();
			label4 = new Label();
			groupBox1.SuspendLayout();
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
			btnReload.Location = new Point(13, 522);
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
			btnAddNew.Location = new Point(91, 522);
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
			MainList.Size = new Size(304, 467);
			MainList.TabIndex = 1;
			MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBox1.Controls.Add(cbStartFrom);
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(tbNeededExperience);
			groupBox1.Controls.Add(label39);
			groupBox1.Controls.Add(tbMaxLevel);
			groupBox1.Controls.Add(label35);
			groupBox1.Controls.Add(tbMinLevel);
			groupBox1.Controls.Add(cbType2);
			groupBox1.Controls.Add(label31);
			groupBox1.Controls.Add(label8);
			groupBox1.Controls.Add(cbType1);
			groupBox1.Controls.Add(label5);
			groupBox1.Controls.Add(cbEnable);
			groupBox1.Controls.Add(cbJob);
			groupBox1.Controls.Add(label7);
			groupBox1.Controls.Add(groupBox5);
			groupBox1.Controls.Add(btnUpdate);
			groupBox1.Controls.Add(cbType0);
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
			groupBox1.Size = new Size(1056, 537);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Quest Data";
			// 
			// cbEnable
			// 
			cbEnable.AutoSize = true;
			cbEnable.ForeColor = Color.FromArgb(208, 203, 148);
			cbEnable.Location = new Point(99, 57);
			cbEnable.Margin = new Padding(4, 3, 4, 3);
			cbEnable.Name = "cbEnable";
			cbEnable.Size = new Size(61, 19);
			cbEnable.TabIndex = 1050;
			cbEnable.Text = "Enable";
			cbEnable.UseVisualStyleBackColor = true;
			// 
			// cbJob
			// 
			cbJob.AllowDrop = true;
			cbJob.BackColor = Color.FromArgb(28, 30, 31);
			cbJob.DropDownStyle = ComboBoxStyle.DropDownList;
			cbJob.FlatStyle = FlatStyle.Flat;
			cbJob.ForeColor = Color.FromArgb(208, 203, 148);
			cbJob.FormattingEnabled = true;
			cbJob.Location = new Point(464, 55);
			cbJob.Margin = new Padding(4, 3, 4, 3);
			cbJob.Name = "cbJob";
			cbJob.Size = new Size(100, 23);
			cbJob.TabIndex = 1038;
			cbJob.Visible = false;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.ForeColor = Color.FromArgb(208, 203, 148);
			label7.Location = new Point(431, 59);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new Size(25, 15);
			label7.TabIndex = 1039;
			label7.Text = "Job";
			label7.TextAlign = ContentAlignment.MiddleRight;
			label7.Visible = false;
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(tbResistance);
			groupBox5.Controls.Add(label11);
			groupBox5.Controls.Add(label9);
			groupBox5.Controls.Add(tbMagicAttack);
			groupBox5.Controls.Add(tbDefense);
			groupBox5.Controls.Add(label10);
			groupBox5.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox5.Location = new Point(785, 335);
			groupBox5.Margin = new Padding(4, 3, 4, 3);
			groupBox5.Name = "groupBox5";
			groupBox5.Padding = new Padding(4, 3, 4, 3);
			groupBox5.Size = new Size(172, 138);
			groupBox5.TabIndex = 1048;
			groupBox5.TabStop = false;
			groupBox5.Text = "Statistics";
			// 
			// tbMinLevel
			// 
			tbMinLevel.BackColor = Color.FromArgb(28, 30, 31);
			tbMinLevel.BorderStyle = BorderStyle.FixedSingle;
			tbMinLevel.ForeColor = Color.FromArgb(208, 203, 148);
			tbMinLevel.Location = new Point(234, 55);
			tbMinLevel.Margin = new Padding(4, 3, 4, 3);
			tbMinLevel.Name = "tbMinLevel";
			tbMinLevel.Size = new Size(57, 23);
			tbMinLevel.TabIndex = 1040;
			tbMinLevel.TextAlign = HorizontalAlignment.Center;
			tbMinLevel.TextChanged += tbAttack_TextChanged;
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
			label8.Location = new Point(168, 59);
			label8.Margin = new Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new Size(58, 15);
			label8.TabIndex = 1041;
			label8.Text = "Min Level";
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
			btnUpdate.Size = new Size(1041, 27);
			btnUpdate.TabIndex = 999;
			btnUpdate.Text = "Update";
			btnUpdate.UseVisualStyleBackColor = false;
			btnUpdate.Click += btnUpdate_Click;
			// 
			// cbType0
			// 
			cbType0.BackColor = Color.FromArgb(28, 30, 31);
			cbType0.DropDownStyle = ComboBoxStyle.DropDownList;
			cbType0.FlatStyle = FlatStyle.Flat;
			cbType0.ForeColor = Color.Black;
			cbType0.FormattingEnabled = true;
			cbType0.Location = new Point(347, 84);
			cbType0.Margin = new Padding(4, 3, 4, 3);
			cbType0.Name = "cbType0";
			cbType0.Size = new Size(208, 23);
			cbType0.TabIndex = 16;
			cbType0.SelectedIndexChanged += cbGradeSelector_SelectedIndexChanged;
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
			label3.Location = new Point(298, 88);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(41, 15);
			label3.TabIndex = 1006;
			label3.Text = "Type 0";
			label3.TextAlign = ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(tbConditionDescription);
			groupBox2.Controls.Add(label27);
			groupBox2.Controls.Add(tbRewardDescription);
			groupBox2.Controls.Add(label23);
			groupBox2.Controls.Add(cbNationSelector);
			groupBox2.Controls.Add(tbName);
			groupBox2.Controls.Add(label2);
			groupBox2.Controls.Add(tbStartDescription);
			groupBox2.Controls.Add(label19);
			groupBox2.FlatStyle = FlatStyle.Flat;
			groupBox2.Location = new Point(8, 84);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(282, 341);
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
			cbNationSelector.Size = new Size(266, 23);
			cbNationSelector.TabIndex = 11;
			cbNationSelector.SelectedIndexChanged += cbNationSelector_SelectedIndexChanged;
			// 
			// tbName
			// 
			tbName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbName.BackColor = Color.FromArgb(28, 30, 31);
			tbName.BorderStyle = BorderStyle.FixedSingle;
			tbName.ForeColor = Color.FromArgb(208, 203, 148);
			tbName.Location = new Point(55, 51);
			tbName.Margin = new Padding(4, 3, 4, 3);
			tbName.Name = "tbName";
			tbName.Size = new Size(219, 23);
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
			label2.Size = new Size(39, 15);
			label2.TabIndex = 14;
			label2.Text = "Name";
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
			btnCopy.Location = new Point(169, 522);
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
			btnDelete.Location = new Point(247, 522);
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
			// tbStartDescription
			// 
			tbStartDescription.BackColor = Color.FromArgb(28, 30, 31);
			tbStartDescription.BorderStyle = BorderStyle.FixedSingle;
			tbStartDescription.ForeColor = Color.FromArgb(208, 203, 148);
			tbStartDescription.Location = new Point(8, 95);
			tbStartDescription.Margin = new Padding(4, 3, 4, 3);
			tbStartDescription.Multiline = true;
			tbStartDescription.Name = "tbStartDescription";
			tbStartDescription.ScrollBars = ScrollBars.Vertical;
			tbStartDescription.Size = new Size(266, 66);
			tbStartDescription.TabIndex = 13;
			// 
			// label19
			// 
			label19.AutoSize = true;
			label19.ForeColor = Color.FromArgb(208, 203, 148);
			label19.Location = new Point(8, 77);
			label19.Margin = new Padding(4, 0, 4, 0);
			label19.Name = "label19";
			label19.Size = new Size(94, 15);
			label19.TabIndex = 15;
			label19.Text = "Start Description";
			label19.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbRewardDescription
			// 
			tbRewardDescription.BackColor = Color.FromArgb(28, 30, 31);
			tbRewardDescription.BorderStyle = BorderStyle.FixedSingle;
			tbRewardDescription.ForeColor = Color.FromArgb(208, 203, 148);
			tbRewardDescription.Location = new Point(8, 182);
			tbRewardDescription.Margin = new Padding(4, 3, 4, 3);
			tbRewardDescription.Multiline = true;
			tbRewardDescription.Name = "tbRewardDescription";
			tbRewardDescription.ScrollBars = ScrollBars.Vertical;
			tbRewardDescription.Size = new Size(266, 66);
			tbRewardDescription.TabIndex = 16;
			// 
			// label23
			// 
			label23.AutoSize = true;
			label23.ForeColor = Color.FromArgb(208, 203, 148);
			label23.Location = new Point(8, 164);
			label23.Margin = new Padding(4, 0, 4, 0);
			label23.Name = "label23";
			label23.Size = new Size(109, 15);
			label23.TabIndex = 17;
			label23.Text = "Reward Description";
			label23.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbConditionDescription
			// 
			tbConditionDescription.BackColor = Color.FromArgb(28, 30, 31);
			tbConditionDescription.BorderStyle = BorderStyle.FixedSingle;
			tbConditionDescription.ForeColor = Color.FromArgb(208, 203, 148);
			tbConditionDescription.Location = new Point(8, 269);
			tbConditionDescription.Margin = new Padding(4, 3, 4, 3);
			tbConditionDescription.Multiline = true;
			tbConditionDescription.Name = "tbConditionDescription";
			tbConditionDescription.ScrollBars = ScrollBars.Vertical;
			tbConditionDescription.Size = new Size(266, 66);
			tbConditionDescription.TabIndex = 18;
			// 
			// label27
			// 
			label27.AutoSize = true;
			label27.ForeColor = Color.FromArgb(208, 203, 148);
			label27.Location = new Point(8, 251);
			label27.Margin = new Padding(4, 0, 4, 0);
			label27.Name = "label27";
			label27.Size = new Size(123, 15);
			label27.TabIndex = 19;
			label27.Text = "Condition Description";
			label27.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbType1
			// 
			cbType1.BackColor = Color.FromArgb(28, 30, 31);
			cbType1.DropDownStyle = ComboBoxStyle.DropDownList;
			cbType1.FlatStyle = FlatStyle.Flat;
			cbType1.ForeColor = Color.Black;
			cbType1.FormattingEnabled = true;
			cbType1.Location = new Point(347, 113);
			cbType1.Margin = new Padding(4, 3, 4, 3);
			cbType1.Name = "cbType1";
			cbType1.Size = new Size(208, 23);
			cbType1.TabIndex = 1051;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.ForeColor = Color.FromArgb(208, 203, 148);
			label5.Location = new Point(298, 117);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new Size(41, 15);
			label5.TabIndex = 1052;
			label5.Text = "Type 1";
			label5.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbType2
			// 
			cbType2.BackColor = Color.FromArgb(28, 30, 31);
			cbType2.DropDownStyle = ComboBoxStyle.DropDownList;
			cbType2.FlatStyle = FlatStyle.Flat;
			cbType2.ForeColor = Color.Black;
			cbType2.FormattingEnabled = true;
			cbType2.Location = new Point(347, 142);
			cbType2.Margin = new Padding(4, 3, 4, 3);
			cbType2.Name = "cbType2";
			cbType2.Size = new Size(208, 23);
			cbType2.TabIndex = 1053;
			// 
			// label31
			// 
			label31.AutoSize = true;
			label31.ForeColor = Color.FromArgb(208, 203, 148);
			label31.Location = new Point(298, 146);
			label31.Margin = new Padding(4, 0, 4, 0);
			label31.Name = "label31";
			label31.Size = new Size(41, 15);
			label31.TabIndex = 1054;
			label31.Text = "Type 2";
			label31.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbMaxLevel
			// 
			tbMaxLevel.BackColor = Color.FromArgb(28, 30, 31);
			tbMaxLevel.BorderStyle = BorderStyle.FixedSingle;
			tbMaxLevel.ForeColor = Color.FromArgb(208, 203, 148);
			tbMaxLevel.Location = new Point(366, 55);
			tbMaxLevel.Margin = new Padding(4, 3, 4, 3);
			tbMaxLevel.Name = "tbMaxLevel";
			tbMaxLevel.Size = new Size(57, 23);
			tbMaxLevel.TabIndex = 1055;
			tbMaxLevel.TextAlign = HorizontalAlignment.Center;
			// 
			// label35
			// 
			label35.AutoSize = true;
			label35.ForeColor = Color.FromArgb(208, 203, 148);
			label35.Location = new Point(299, 59);
			label35.Margin = new Padding(4, 0, 4, 0);
			label35.Name = "label35";
			label35.Size = new Size(59, 15);
			label35.TabIndex = 1056;
			label35.Text = "Max Level";
			label35.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbNeededExperience
			// 
			tbNeededExperience.BackColor = Color.FromArgb(28, 30, 31);
			tbNeededExperience.BorderStyle = BorderStyle.FixedSingle;
			tbNeededExperience.ForeColor = Color.FromArgb(208, 203, 148);
			tbNeededExperience.Location = new Point(687, 55);
			tbNeededExperience.Margin = new Padding(4, 3, 4, 3);
			tbNeededExperience.Name = "tbNeededExperience";
			tbNeededExperience.Size = new Size(80, 23);
			tbNeededExperience.TabIndex = 1057;
			tbNeededExperience.TextAlign = HorizontalAlignment.Center;
			// 
			// label39
			// 
			label39.AutoSize = true;
			label39.ForeColor = Color.FromArgb(208, 203, 148);
			label39.Location = new Point(572, 59);
			label39.Margin = new Padding(4, 0, 4, 0);
			label39.Name = "label39";
			label39.Size = new Size(107, 15);
			label39.TabIndex = 1058;
			label39.Text = "Needed Experience";
			label39.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbStartFrom
			// 
			cbStartFrom.AllowDrop = true;
			cbStartFrom.BackColor = Color.FromArgb(28, 30, 31);
			cbStartFrom.DropDownStyle = ComboBoxStyle.DropDownList;
			cbStartFrom.FlatStyle = FlatStyle.Flat;
			cbStartFrom.ForeColor = Color.FromArgb(208, 203, 148);
			cbStartFrom.FormattingEnabled = true;
			cbStartFrom.Location = new Point(366, 171);
			cbStartFrom.Margin = new Padding(4, 3, 4, 3);
			cbStartFrom.Name = "cbStartFrom";
			cbStartFrom.Size = new Size(80, 23);
			cbStartFrom.TabIndex = 1059;
			cbStartFrom.Visible = false;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.ForeColor = Color.FromArgb(208, 203, 148);
			label4.Location = new Point(298, 175);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(62, 15);
			label4.TabIndex = 1060;
			label4.Text = "Start From";
			label4.TextAlign = ContentAlignment.MiddleRight;
			label4.Visible = false;
			// 
			// QuestEditor
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(40, 40, 40);
			ClientSize = new Size(1394, 561);
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
			Name = "QuestEditor";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Quest Editor";
			FormClosing += RareOptionEditor_FormClosing;
			Load += RareOptionEditor_LoadAsync;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
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
		private ComboBox cbType0;
		private ComboBox cbJob;
		private Label label7;
		private TextBox tbMinLevel;
		private Label label8;
		private TextBox tbDefense;
		private Label label9;
		private TextBox tbMagicAttack;
		private Label label10;
		private TextBox tbResistance;
		private Label label11;
		private GroupBox groupBox5;
		private CheckBox cbEnable;
		private TextBox tbStartDescription;
		private Label label19;
		private TextBox tbRewardDescription;
		private Label label23;
		private TextBox tbConditionDescription;
		private Label label27;
		private ComboBox cbType1;
		private Label label5;
		private ComboBox cbType2;
		private Label label31;
		private TextBox tbMaxLevel;
		private Label label35;
		private TextBox tbNeededExperience;
		private Label label39;
		private ComboBox cbStartFrom;
		private Label label4;
	}
}