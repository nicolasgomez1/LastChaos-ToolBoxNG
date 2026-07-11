using System.Windows.Forms;

namespace LastChaos_ToolBoxNG
{
	partial class TitleEditor
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
			panel1 = new Panel();
			lbTitleViewer = new AlphaLabel();
			groupBox6 = new GroupBox();
			cbOptionLevel4 = new ComboBox();
			cbOptionID0 = new ComboBox();
			label29 = new Label();
			label23 = new Label();
			cbOptionID4 = new ComboBox();
			label16 = new Label();
			label30 = new Label();
			cbOptionLevel0 = new ComboBox();
			cbOptionLevel3 = new ComboBox();
			label15 = new Label();
			label17 = new Label();
			cbOptionID1 = new ComboBox();
			cbOptionID3 = new ComboBox();
			label5 = new Label();
			label18 = new Label();
			cbOptionLevel1 = new ComboBox();
			cbOptionLevel2 = new ComboBox();
			label22 = new Label();
			label4 = new Label();
			cbOptionID2 = new ComboBox();
			lbFlag = new Label();
			btnItemFlag = new Button();
			label14 = new Label();
			btnClaimItem = new Button();
			groupBox4 = new GroupBox();
			lbTitleColorBlue = new Label();
			tbTitleColor = new TextBox();
			lbTitleColorGreen = new Label();
			label10 = new Label();
			lbTitleColorRed = new Label();
			tbTitleColorBlue = new TrackBar();
			lbTitleColorOpacity = new Label();
			label11 = new Label();
			tbTitleColorGreen = new TrackBar();
			label12 = new Label();
			tbTitleColorRed = new TrackBar();
			label13 = new Label();
			tbTitleColorOpacity = new TrackBar();
			groupBox2 = new GroupBox();
			lbBGColorBlue = new Label();
			lbBGColorGreen = new Label();
			lbBGColorRed = new Label();
			lbBGOpacity = new Label();
			tbBGColor = new TextBox();
			label9 = new Label();
			tbBGColorBlue = new TrackBar();
			label8 = new Label();
			tbBGColorGreen = new TrackBar();
			label7 = new Label();
			tbBGColorRed = new TrackBar();
			label2 = new Label();
			tbBGColorOpacity = new TrackBar();
			tbDuration = new TextBox();
			label6 = new Label();
			groupBox3 = new GroupBox();
			tbEffectDamage = new TextBox();
			tbEffectAttack = new TextBox();
			label21 = new Label();
			label20 = new Label();
			label19 = new Label();
			tbEffectName = new TextBox();
			cbEnable = new CheckBox();
			btnUpdate = new Button();
			cbCastleSelector = new ComboBox();
			label1 = new Label();
			label3 = new Label();
			tbID = new TextBox();
			btnCopy = new Button();
			btnDelete = new Button();
			tbSearch = new TextBox();
			groupBox1.SuspendLayout();
			panel1.SuspendLayout();
			groupBox6.SuspendLayout();
			groupBox4.SuspendLayout();
			((ISupportInitialize)tbTitleColorBlue).BeginInit();
			((ISupportInitialize)tbTitleColorGreen).BeginInit();
			((ISupportInitialize)tbTitleColorRed).BeginInit();
			((ISupportInitialize)tbTitleColorOpacity).BeginInit();
			groupBox2.SuspendLayout();
			((ISupportInitialize)tbBGColorBlue).BeginInit();
			((ISupportInitialize)tbBGColorGreen).BeginInit();
			((ISupportInitialize)tbBGColorRed).BeginInit();
			((ISupportInitialize)tbBGColorOpacity).BeginInit();
			groupBox3.SuspendLayout();
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
			btnReload.Location = new Point(13, 431);
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
			btnAddNew.Location = new Point(91, 431);
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
			MainList.Size = new Size(304, 377);
			MainList.TabIndex = 1;
			MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBox1.Controls.Add(panel1);
			groupBox1.Controls.Add(groupBox6);
			groupBox1.Controls.Add(lbFlag);
			groupBox1.Controls.Add(btnItemFlag);
			groupBox1.Controls.Add(label14);
			groupBox1.Controls.Add(btnClaimItem);
			groupBox1.Controls.Add(groupBox4);
			groupBox1.Controls.Add(groupBox2);
			groupBox1.Controls.Add(tbDuration);
			groupBox1.Controls.Add(label6);
			groupBox1.Controls.Add(groupBox3);
			groupBox1.Controls.Add(cbEnable);
			groupBox1.Controls.Add(btnUpdate);
			groupBox1.Controls.Add(cbCastleSelector);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(tbID);
			groupBox1.FlatStyle = FlatStyle.Flat;
			groupBox1.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox1.ImeMode = ImeMode.On;
			groupBox1.Location = new Point(325, 12);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(818, 446);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Title Data";
			// 
			// panel1
			// 
			panel1.BackgroundImage = Properties.Resources.TitleBackground;
			panel1.BackgroundImageLayout = ImageLayout.Center;
			panel1.Controls.Add(lbTitleViewer);
			panel1.Location = new Point(7, 233);
			panel1.Name = "panel1";
			panel1.Size = new Size(804, 34);
			panel1.TabIndex = 1069;
			// 
			// lbTitleViewer
			// 
			lbTitleViewer.BackColor = Color.Transparent;
			lbTitleViewer.BackColorAlpha = Color.FromArgb(128, 66, 66, 66);
			lbTitleViewer.Font = new Font("Corbel", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
			lbTitleViewer.ForeColorAlpha = Color.FromArgb(208, 203, 148);
			lbTitleViewer.Location = new Point(8, 8);
			lbTitleViewer.Margin = new Padding(3);
			lbTitleViewer.Name = "lbTitleViewer";
			lbTitleViewer.Size = new Size(788, 18);
			lbTitleViewer.TabIndex = 0;
			lbTitleViewer.Text = "-";
			lbTitleViewer.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// groupBox6
			// 
			groupBox6.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			groupBox6.Controls.Add(cbOptionLevel4);
			groupBox6.Controls.Add(cbOptionID0);
			groupBox6.Controls.Add(label29);
			groupBox6.Controls.Add(label23);
			groupBox6.Controls.Add(cbOptionID4);
			groupBox6.Controls.Add(label16);
			groupBox6.Controls.Add(label30);
			groupBox6.Controls.Add(cbOptionLevel0);
			groupBox6.Controls.Add(cbOptionLevel3);
			groupBox6.Controls.Add(label15);
			groupBox6.Controls.Add(label17);
			groupBox6.Controls.Add(cbOptionID1);
			groupBox6.Controls.Add(cbOptionID3);
			groupBox6.Controls.Add(label5);
			groupBox6.Controls.Add(label18);
			groupBox6.Controls.Add(cbOptionLevel1);
			groupBox6.Controls.Add(cbOptionLevel2);
			groupBox6.Controls.Add(label22);
			groupBox6.Controls.Add(label4);
			groupBox6.Controls.Add(cbOptionID2);
			groupBox6.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox6.Location = new Point(8, 273);
			groupBox6.Margin = new Padding(4, 3, 4, 3);
			groupBox6.Name = "groupBox6";
			groupBox6.Padding = new Padding(4, 3, 4, 3);
			groupBox6.Size = new Size(802, 167);
			groupBox6.TabIndex = 1055;
			groupBox6.TabStop = false;
			groupBox6.Text = "Options Data (raw values)";
			// 
			// cbOptionLevel4
			// 
			cbOptionLevel4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel4.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel4.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel4.FlatStyle = FlatStyle.Flat;
			cbOptionLevel4.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel4.FormattingEnabled = true;
			cbOptionLevel4.Location = new Point(639, 138);
			cbOptionLevel4.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel4.Name = "cbOptionLevel4";
			cbOptionLevel4.Size = new Size(155, 23);
			cbOptionLevel4.TabIndex = 1149;
			cbOptionLevel4.SelectedIndexChanged += cbOptionLevel4_SelectedIndexChanged;
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
			cbOptionID0.Size = new Size(520, 23);
			cbOptionID0.TabIndex = 1131;
			cbOptionID0.SelectedIndexChanged += cbOptionID0_SelectedIndexChanged;
			// 
			// label29
			// 
			label29.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label29.AutoSize = true;
			label29.ForeColor = Color.FromArgb(208, 203, 148);
			label29.Location = new Point(597, 142);
			label29.Margin = new Padding(4, 0, 4, 0);
			label29.Name = "label29";
			label29.Size = new Size(34, 15);
			label29.TabIndex = 1148;
			label29.Text = "Value";
			label29.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label23
			// 
			label23.AutoSize = true;
			label23.ForeColor = Color.FromArgb(208, 203, 148);
			label23.Location = new Point(8, 26);
			label23.Margin = new Padding(4, 0, 4, 0);
			label23.Name = "label23";
			label23.Size = new Size(53, 15);
			label23.TabIndex = 1130;
			label23.Text = "Option 0";
			label23.TextAlign = ContentAlignment.MiddleRight;
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
			cbOptionID4.Size = new Size(520, 23);
			cbOptionID4.TabIndex = 1147;
			cbOptionID4.SelectedIndexChanged += cbOptionID4_SelectedIndexChanged;
			// 
			// label16
			// 
			label16.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label16.AutoSize = true;
			label16.ForeColor = Color.FromArgb(208, 203, 148);
			label16.Location = new Point(597, 26);
			label16.Margin = new Padding(4, 0, 4, 0);
			label16.Name = "label16";
			label16.Size = new Size(34, 15);
			label16.TabIndex = 1132;
			label16.Text = "Value";
			label16.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label30
			// 
			label30.AutoSize = true;
			label30.ForeColor = Color.FromArgb(208, 203, 148);
			label30.Location = new Point(8, 142);
			label30.Margin = new Padding(4, 0, 4, 0);
			label30.Name = "label30";
			label30.Size = new Size(53, 15);
			label30.TabIndex = 1146;
			label30.Text = "Option 4";
			label30.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionLevel0
			// 
			cbOptionLevel0.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel0.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel0.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel0.FlatStyle = FlatStyle.Flat;
			cbOptionLevel0.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel0.FormattingEnabled = true;
			cbOptionLevel0.Location = new Point(639, 22);
			cbOptionLevel0.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel0.Name = "cbOptionLevel0";
			cbOptionLevel0.Size = new Size(155, 23);
			cbOptionLevel0.TabIndex = 1133;
			cbOptionLevel0.SelectedIndexChanged += cbOptionLevel0_SelectedIndexChanged;
			// 
			// cbOptionLevel3
			// 
			cbOptionLevel3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel3.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel3.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel3.FlatStyle = FlatStyle.Flat;
			cbOptionLevel3.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel3.FormattingEnabled = true;
			cbOptionLevel3.Location = new Point(639, 109);
			cbOptionLevel3.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel3.Name = "cbOptionLevel3";
			cbOptionLevel3.Size = new Size(155, 23);
			cbOptionLevel3.TabIndex = 1145;
			cbOptionLevel3.SelectedIndexChanged += cbOptionLevel3_SelectedIndexChanged;
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.ForeColor = Color.FromArgb(208, 203, 148);
			label15.Location = new Point(8, 55);
			label15.Margin = new Padding(4, 0, 4, 0);
			label15.Name = "label15";
			label15.Size = new Size(53, 15);
			label15.TabIndex = 1134;
			label15.Text = "Option 1";
			label15.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label17
			// 
			label17.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label17.AutoSize = true;
			label17.ForeColor = Color.FromArgb(208, 203, 148);
			label17.Location = new Point(597, 113);
			label17.Margin = new Padding(4, 0, 4, 0);
			label17.Name = "label17";
			label17.Size = new Size(34, 15);
			label17.TabIndex = 1144;
			label17.Text = "Value";
			label17.TextAlign = ContentAlignment.MiddleRight;
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
			cbOptionID1.Size = new Size(520, 23);
			cbOptionID1.TabIndex = 1135;
			cbOptionID1.SelectedIndexChanged += cbOptionID1_SelectedIndexChanged;
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
			cbOptionID3.Size = new Size(520, 23);
			cbOptionID3.TabIndex = 1143;
			cbOptionID3.SelectedIndexChanged += cbOptionID3_SelectedIndexChanged;
			// 
			// label5
			// 
			label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label5.AutoSize = true;
			label5.ForeColor = Color.FromArgb(208, 203, 148);
			label5.Location = new Point(597, 55);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new Size(34, 15);
			label5.TabIndex = 1136;
			label5.Text = "Value";
			label5.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label18
			// 
			label18.AutoSize = true;
			label18.ForeColor = Color.FromArgb(208, 203, 148);
			label18.Location = new Point(8, 113);
			label18.Margin = new Padding(4, 0, 4, 0);
			label18.Name = "label18";
			label18.Size = new Size(53, 15);
			label18.TabIndex = 1142;
			label18.Text = "Option 3";
			label18.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbOptionLevel1
			// 
			cbOptionLevel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel1.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel1.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel1.FlatStyle = FlatStyle.Flat;
			cbOptionLevel1.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel1.FormattingEnabled = true;
			cbOptionLevel1.Location = new Point(639, 51);
			cbOptionLevel1.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel1.Name = "cbOptionLevel1";
			cbOptionLevel1.Size = new Size(155, 23);
			cbOptionLevel1.TabIndex = 1137;
			cbOptionLevel1.SelectedIndexChanged += cbOptionLevel1_SelectedIndexChanged;
			// 
			// cbOptionLevel2
			// 
			cbOptionLevel2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbOptionLevel2.BackColor = Color.FromArgb(28, 30, 31);
			cbOptionLevel2.DropDownStyle = ComboBoxStyle.DropDownList;
			cbOptionLevel2.FlatStyle = FlatStyle.Flat;
			cbOptionLevel2.ForeColor = Color.FromArgb(208, 203, 148);
			cbOptionLevel2.FormattingEnabled = true;
			cbOptionLevel2.Location = new Point(639, 80);
			cbOptionLevel2.Margin = new Padding(4, 3, 4, 3);
			cbOptionLevel2.Name = "cbOptionLevel2";
			cbOptionLevel2.Size = new Size(155, 23);
			cbOptionLevel2.TabIndex = 1141;
			cbOptionLevel2.SelectedIndexChanged += cbOptionLevel2_SelectedIndexChanged;
			// 
			// label22
			// 
			label22.AutoSize = true;
			label22.ForeColor = Color.FromArgb(208, 203, 148);
			label22.Location = new Point(8, 84);
			label22.Margin = new Padding(4, 0, 4, 0);
			label22.Name = "label22";
			label22.Size = new Size(53, 15);
			label22.TabIndex = 1138;
			label22.Text = "Option 2";
			label22.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label4.AutoSize = true;
			label4.ForeColor = Color.FromArgb(208, 203, 148);
			label4.Location = new Point(597, 84);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(34, 15);
			label4.TabIndex = 1140;
			label4.Text = "Value";
			label4.TextAlign = ContentAlignment.MiddleRight;
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
			cbOptionID2.Size = new Size(520, 23);
			cbOptionID2.TabIndex = 1139;
			cbOptionID2.SelectedIndexChanged += cbOptionID2_SelectedIndexChanged;
			// 
			// lbFlag
			// 
			lbFlag.AutoSize = true;
			lbFlag.ForeColor = Color.FromArgb(208, 203, 148);
			lbFlag.Location = new Point(8, 204);
			lbFlag.Margin = new Padding(4, 0, 4, 0);
			lbFlag.Name = "lbFlag";
			lbFlag.Size = new Size(29, 15);
			lbFlag.TabIndex = 1068;
			lbFlag.Text = "Flag";
			lbFlag.TextAlign = ContentAlignment.MiddleRight;
			lbFlag.Visible = false;
			// 
			// btnItemFlag
			// 
			btnItemFlag.BackColor = Color.FromArgb(40, 40, 40);
			btnItemFlag.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnItemFlag.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnItemFlag.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnItemFlag.FlatStyle = FlatStyle.Flat;
			btnItemFlag.ForeColor = Color.FromArgb(208, 203, 148);
			btnItemFlag.Location = new Point(45, 198);
			btnItemFlag.Margin = new Padding(4, 3, 4, 3);
			btnItemFlag.Name = "btnItemFlag";
			btnItemFlag.Size = new Size(217, 27);
			btnItemFlag.TabIndex = 1067;
			btnItemFlag.UseVisualStyleBackColor = false;
			btnItemFlag.Visible = false;
			btnItemFlag.Click += btnItemFlag_Click;
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.ForeColor = Color.FromArgb(208, 203, 148);
			label14.Location = new Point(168, 59);
			label14.Margin = new Padding(4, 0, 4, 0);
			label14.Name = "label14";
			label14.Size = new Size(65, 15);
			label14.TabIndex = 1066;
			label14.Text = "Claim Item";
			label14.TextAlign = ContentAlignment.MiddleRight;
			// 
			// btnClaimItem
			// 
			btnClaimItem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			btnClaimItem.BackColor = Color.FromArgb(40, 40, 40);
			btnClaimItem.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnClaimItem.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnClaimItem.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnClaimItem.FlatStyle = FlatStyle.Flat;
			btnClaimItem.ForeColor = Color.FromArgb(208, 203, 148);
			btnClaimItem.ImageAlign = ContentAlignment.MiddleLeft;
			btnClaimItem.Location = new Point(241, 53);
			btnClaimItem.Margin = new Padding(4, 3, 4, 3);
			btnClaimItem.Name = "btnClaimItem";
			btnClaimItem.Size = new Size(222, 27);
			btnClaimItem.TabIndex = 1065;
			btnClaimItem.UseVisualStyleBackColor = false;
			btnClaimItem.Click += btnClaimItem_Click;
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(lbTitleColorBlue);
			groupBox4.Controls.Add(tbTitleColor);
			groupBox4.Controls.Add(lbTitleColorGreen);
			groupBox4.Controls.Add(label10);
			groupBox4.Controls.Add(lbTitleColorRed);
			groupBox4.Controls.Add(tbTitleColorBlue);
			groupBox4.Controls.Add(lbTitleColorOpacity);
			groupBox4.Controls.Add(label11);
			groupBox4.Controls.Add(tbTitleColorGreen);
			groupBox4.Controls.Add(label12);
			groupBox4.Controls.Add(tbTitleColorRed);
			groupBox4.Controls.Add(label13);
			groupBox4.Controls.Add(tbTitleColorOpacity);
			groupBox4.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox4.Location = new Point(544, 84);
			groupBox4.Margin = new Padding(4, 3, 4, 3);
			groupBox4.Name = "groupBox4";
			groupBox4.Padding = new Padding(4, 3, 4, 3);
			groupBox4.Size = new Size(266, 143);
			groupBox4.TabIndex = 1064;
			groupBox4.TabStop = false;
			groupBox4.Text = "Title Color";
			// 
			// lbTitleColorBlue
			// 
			lbTitleColorBlue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbTitleColorBlue.AutoSize = true;
			lbTitleColorBlue.ForeColor = Color.FromArgb(208, 203, 148);
			lbTitleColorBlue.Location = new Point(233, 88);
			lbTitleColorBlue.Margin = new Padding(4, 0, 4, 0);
			lbTitleColorBlue.Name = "lbTitleColorBlue";
			lbTitleColorBlue.Size = new Size(13, 15);
			lbTitleColorBlue.TabIndex = 1074;
			lbTitleColorBlue.Text = "0";
			lbTitleColorBlue.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbTitleColor
			// 
			tbTitleColor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbTitleColor.BackColor = Color.FromArgb(28, 30, 31);
			tbTitleColor.BorderStyle = BorderStyle.FixedSingle;
			tbTitleColor.ForeColor = Color.FromArgb(208, 203, 148);
			tbTitleColor.Location = new Point(8, 114);
			tbTitleColor.Margin = new Padding(4, 3, 4, 3);
			tbTitleColor.Name = "tbTitleColor";
			tbTitleColor.Size = new Size(250, 23);
			tbTitleColor.TabIndex = 1057;
			tbTitleColor.TextAlign = HorizontalAlignment.Center;
			tbTitleColor.TextChanged += tbBGOrTitleColor_TextChanged;
			// 
			// lbTitleColorGreen
			// 
			lbTitleColorGreen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbTitleColorGreen.AutoSize = true;
			lbTitleColorGreen.ForeColor = Color.FromArgb(208, 203, 148);
			lbTitleColorGreen.Location = new Point(233, 66);
			lbTitleColorGreen.Margin = new Padding(4, 0, 4, 0);
			lbTitleColorGreen.Name = "lbTitleColorGreen";
			lbTitleColorGreen.Size = new Size(13, 15);
			lbTitleColorGreen.TabIndex = 1073;
			lbTitleColorGreen.Text = "0";
			lbTitleColorGreen.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.ForeColor = Color.FromArgb(208, 203, 148);
			label10.Location = new Point(27, 88);
			label10.Margin = new Padding(4, 0, 4, 0);
			label10.Name = "label10";
			label10.Size = new Size(30, 15);
			label10.TabIndex = 1062;
			label10.Text = "Blue";
			label10.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lbTitleColorRed
			// 
			lbTitleColorRed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbTitleColorRed.AutoSize = true;
			lbTitleColorRed.ForeColor = Color.FromArgb(208, 203, 148);
			lbTitleColorRed.Location = new Point(233, 44);
			lbTitleColorRed.Margin = new Padding(4, 0, 4, 0);
			lbTitleColorRed.Name = "lbTitleColorRed";
			lbTitleColorRed.Size = new Size(13, 15);
			lbTitleColorRed.TabIndex = 1072;
			lbTitleColorRed.Text = "0";
			lbTitleColorRed.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbTitleColorBlue
			// 
			tbTitleColorBlue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbTitleColorBlue.BackColor = Color.FromArgb(40, 40, 40);
			tbTitleColorBlue.LargeChange = 1;
			tbTitleColorBlue.Location = new Point(63, 88);
			tbTitleColorBlue.Maximum = 255;
			tbTitleColorBlue.Name = "tbTitleColorBlue";
			tbTitleColorBlue.Size = new Size(163, 45);
			tbTitleColorBlue.TabIndex = 1063;
			tbTitleColorBlue.TickStyle = TickStyle.None;
			tbTitleColorBlue.Scroll += TrackBar_Scroll;
			// 
			// lbTitleColorOpacity
			// 
			lbTitleColorOpacity.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbTitleColorOpacity.AutoSize = true;
			lbTitleColorOpacity.ForeColor = Color.FromArgb(208, 203, 148);
			lbTitleColorOpacity.Location = new Point(233, 22);
			lbTitleColorOpacity.Margin = new Padding(4, 0, 4, 0);
			lbTitleColorOpacity.Name = "lbTitleColorOpacity";
			lbTitleColorOpacity.Size = new Size(13, 15);
			lbTitleColorOpacity.TabIndex = 1071;
			lbTitleColorOpacity.Text = "0";
			lbTitleColorOpacity.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.ForeColor = Color.FromArgb(208, 203, 148);
			label11.Location = new Point(19, 66);
			label11.Margin = new Padding(4, 0, 4, 0);
			label11.Name = "label11";
			label11.Size = new Size(38, 15);
			label11.TabIndex = 1060;
			label11.Text = "Green";
			label11.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbTitleColorGreen
			// 
			tbTitleColorGreen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbTitleColorGreen.LargeChange = 1;
			tbTitleColorGreen.Location = new Point(63, 66);
			tbTitleColorGreen.Maximum = 255;
			tbTitleColorGreen.Name = "tbTitleColorGreen";
			tbTitleColorGreen.Size = new Size(163, 45);
			tbTitleColorGreen.TabIndex = 1061;
			tbTitleColorGreen.TickStyle = TickStyle.None;
			tbTitleColorGreen.Scroll += TrackBar_Scroll;
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.ForeColor = Color.FromArgb(208, 203, 148);
			label12.Location = new Point(30, 44);
			label12.Margin = new Padding(4, 0, 4, 0);
			label12.Name = "label12";
			label12.Size = new Size(27, 15);
			label12.TabIndex = 1058;
			label12.Text = "Red";
			label12.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbTitleColorRed
			// 
			tbTitleColorRed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbTitleColorRed.LargeChange = 1;
			tbTitleColorRed.Location = new Point(63, 44);
			tbTitleColorRed.Maximum = 255;
			tbTitleColorRed.Name = "tbTitleColorRed";
			tbTitleColorRed.Size = new Size(163, 45);
			tbTitleColorRed.TabIndex = 1059;
			tbTitleColorRed.TickStyle = TickStyle.None;
			tbTitleColorRed.Scroll += TrackBar_Scroll;
			// 
			// label13
			// 
			label13.AutoSize = true;
			label13.ForeColor = Color.FromArgb(208, 203, 148);
			label13.Location = new Point(9, 22);
			label13.Margin = new Padding(4, 0, 4, 0);
			label13.Name = "label13";
			label13.Size = new Size(48, 15);
			label13.TabIndex = 1057;
			label13.Text = "Opacity";
			label13.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbTitleColorOpacity
			// 
			tbTitleColorOpacity.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbTitleColorOpacity.LargeChange = 1;
			tbTitleColorOpacity.Location = new Point(63, 22);
			tbTitleColorOpacity.Maximum = 255;
			tbTitleColorOpacity.Name = "tbTitleColorOpacity";
			tbTitleColorOpacity.Size = new Size(163, 45);
			tbTitleColorOpacity.TabIndex = 1057;
			tbTitleColorOpacity.TickStyle = TickStyle.None;
			tbTitleColorOpacity.Scroll += TrackBar_Scroll;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(lbBGColorBlue);
			groupBox2.Controls.Add(lbBGColorGreen);
			groupBox2.Controls.Add(lbBGColorRed);
			groupBox2.Controls.Add(lbBGOpacity);
			groupBox2.Controls.Add(tbBGColor);
			groupBox2.Controls.Add(label9);
			groupBox2.Controls.Add(tbBGColorBlue);
			groupBox2.Controls.Add(label8);
			groupBox2.Controls.Add(tbBGColorGreen);
			groupBox2.Controls.Add(label7);
			groupBox2.Controls.Add(tbBGColorRed);
			groupBox2.Controls.Add(label2);
			groupBox2.Controls.Add(tbBGColorOpacity);
			groupBox2.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox2.Location = new Point(270, 84);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(266, 143);
			groupBox2.TabIndex = 1055;
			groupBox2.TabStop = false;
			groupBox2.Text = "Background Color";
			// 
			// lbBGColorBlue
			// 
			lbBGColorBlue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbBGColorBlue.AutoSize = true;
			lbBGColorBlue.ForeColor = Color.FromArgb(208, 203, 148);
			lbBGColorBlue.Location = new Point(233, 88);
			lbBGColorBlue.Margin = new Padding(4, 0, 4, 0);
			lbBGColorBlue.Name = "lbBGColorBlue";
			lbBGColorBlue.Size = new Size(13, 15);
			lbBGColorBlue.TabIndex = 1070;
			lbBGColorBlue.Text = "0";
			lbBGColorBlue.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lbBGColorGreen
			// 
			lbBGColorGreen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbBGColorGreen.AutoSize = true;
			lbBGColorGreen.ForeColor = Color.FromArgb(208, 203, 148);
			lbBGColorGreen.Location = new Point(233, 66);
			lbBGColorGreen.Margin = new Padding(4, 0, 4, 0);
			lbBGColorGreen.Name = "lbBGColorGreen";
			lbBGColorGreen.Size = new Size(13, 15);
			lbBGColorGreen.TabIndex = 1069;
			lbBGColorGreen.Text = "0";
			lbBGColorGreen.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lbBGColorRed
			// 
			lbBGColorRed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbBGColorRed.AutoSize = true;
			lbBGColorRed.ForeColor = Color.FromArgb(208, 203, 148);
			lbBGColorRed.Location = new Point(233, 44);
			lbBGColorRed.Margin = new Padding(4, 0, 4, 0);
			lbBGColorRed.Name = "lbBGColorRed";
			lbBGColorRed.Size = new Size(13, 15);
			lbBGColorRed.TabIndex = 1068;
			lbBGColorRed.Text = "0";
			lbBGColorRed.TextAlign = ContentAlignment.MiddleRight;
			// 
			// lbBGOpacity
			// 
			lbBGOpacity.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbBGOpacity.AutoSize = true;
			lbBGOpacity.ForeColor = Color.FromArgb(208, 203, 148);
			lbBGOpacity.Location = new Point(233, 22);
			lbBGOpacity.Margin = new Padding(4, 0, 4, 0);
			lbBGOpacity.Name = "lbBGOpacity";
			lbBGOpacity.Size = new Size(13, 15);
			lbBGOpacity.TabIndex = 1067;
			lbBGOpacity.Text = "0";
			lbBGOpacity.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbBGColor
			// 
			tbBGColor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbBGColor.BackColor = Color.FromArgb(28, 30, 31);
			tbBGColor.BorderStyle = BorderStyle.FixedSingle;
			tbBGColor.ForeColor = Color.FromArgb(208, 203, 148);
			tbBGColor.Location = new Point(8, 114);
			tbBGColor.Margin = new Padding(4, 3, 4, 3);
			tbBGColor.Name = "tbBGColor";
			tbBGColor.Size = new Size(250, 23);
			tbBGColor.TabIndex = 1057;
			tbBGColor.TextAlign = HorizontalAlignment.Center;
			tbBGColor.TextChanged += tbBGOrTitleColor_TextChanged;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.ForeColor = Color.FromArgb(208, 203, 148);
			label9.Location = new Point(26, 88);
			label9.Margin = new Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new Size(30, 15);
			label9.TabIndex = 1062;
			label9.Text = "Blue";
			label9.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbBGColorBlue
			// 
			tbBGColorBlue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbBGColorBlue.BackColor = Color.FromArgb(40, 40, 40);
			tbBGColorBlue.LargeChange = 1;
			tbBGColorBlue.Location = new Point(63, 88);
			tbBGColorBlue.Maximum = 255;
			tbBGColorBlue.Name = "tbBGColorBlue";
			tbBGColorBlue.Size = new Size(163, 45);
			tbBGColorBlue.TabIndex = 1063;
			tbBGColorBlue.TickStyle = TickStyle.None;
			tbBGColorBlue.Scroll += TrackBar_Scroll;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.ForeColor = Color.FromArgb(208, 203, 148);
			label8.Location = new Point(18, 66);
			label8.Margin = new Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new Size(38, 15);
			label8.TabIndex = 1060;
			label8.Text = "Green";
			label8.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbBGColorGreen
			// 
			tbBGColorGreen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbBGColorGreen.LargeChange = 1;
			tbBGColorGreen.Location = new Point(63, 66);
			tbBGColorGreen.Maximum = 255;
			tbBGColorGreen.Name = "tbBGColorGreen";
			tbBGColorGreen.Size = new Size(163, 45);
			tbBGColorGreen.TabIndex = 1061;
			tbBGColorGreen.TickStyle = TickStyle.None;
			tbBGColorGreen.Scroll += TrackBar_Scroll;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.ForeColor = Color.FromArgb(208, 203, 148);
			label7.Location = new Point(29, 44);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new Size(27, 15);
			label7.TabIndex = 1058;
			label7.Text = "Red";
			label7.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbBGColorRed
			// 
			tbBGColorRed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbBGColorRed.LargeChange = 1;
			tbBGColorRed.Location = new Point(63, 44);
			tbBGColorRed.Maximum = 255;
			tbBGColorRed.Name = "tbBGColorRed";
			tbBGColorRed.Size = new Size(163, 45);
			tbBGColorRed.TabIndex = 1059;
			tbBGColorRed.TickStyle = TickStyle.None;
			tbBGColorRed.Scroll += TrackBar_Scroll;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.ForeColor = Color.FromArgb(208, 203, 148);
			label2.Location = new Point(8, 22);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(48, 15);
			label2.TabIndex = 1057;
			label2.Text = "Opacity";
			label2.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbBGColorOpacity
			// 
			tbBGColorOpacity.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbBGColorOpacity.LargeChange = 1;
			tbBGColorOpacity.Location = new Point(63, 22);
			tbBGColorOpacity.Maximum = 255;
			tbBGColorOpacity.Name = "tbBGColorOpacity";
			tbBGColorOpacity.Size = new Size(163, 45);
			tbBGColorOpacity.TabIndex = 1057;
			tbBGColorOpacity.TickStyle = TickStyle.None;
			tbBGColorOpacity.Scroll += TrackBar_Scroll;
			// 
			// tbDuration
			// 
			tbDuration.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			tbDuration.BackColor = Color.FromArgb(28, 30, 31);
			tbDuration.BorderStyle = BorderStyle.FixedSingle;
			tbDuration.ForeColor = Color.FromArgb(208, 203, 148);
			tbDuration.Location = new Point(532, 55);
			tbDuration.Margin = new Padding(4, 3, 4, 3);
			tbDuration.Name = "tbDuration";
			tbDuration.Size = new Size(30, 23);
			tbDuration.TabIndex = 1056;
			tbDuration.TextAlign = HorizontalAlignment.Center;
			tbDuration.TextChanged += tbDuration_TextChanged;
			// 
			// label6
			// 
			label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label6.AutoSize = true;
			label6.ForeColor = Color.FromArgb(208, 203, 148);
			label6.Location = new Point(471, 59);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new Size(53, 15);
			label6.TabIndex = 1055;
			label6.Text = "Duration";
			label6.TextAlign = ContentAlignment.MiddleRight;
			// 
			// groupBox3
			// 
			groupBox3.Controls.Add(tbEffectDamage);
			groupBox3.Controls.Add(tbEffectAttack);
			groupBox3.Controls.Add(label21);
			groupBox3.Controls.Add(label20);
			groupBox3.Controls.Add(label19);
			groupBox3.Controls.Add(tbEffectName);
			groupBox3.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox3.Location = new Point(8, 84);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new Size(254, 109);
			groupBox3.TabIndex = 1054;
			groupBox3.TabStop = false;
			groupBox3.Text = "Effects Data";
			// 
			// tbEffectDamage
			// 
			tbEffectDamage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbEffectDamage.BackColor = Color.FromArgb(28, 30, 31);
			tbEffectDamage.BorderStyle = BorderStyle.FixedSingle;
			tbEffectDamage.ForeColor = Color.FromArgb(208, 203, 148);
			tbEffectDamage.Location = new Point(67, 80);
			tbEffectDamage.Margin = new Padding(4, 3, 4, 3);
			tbEffectDamage.Name = "tbEffectDamage";
			tbEffectDamage.Size = new Size(179, 23);
			tbEffectDamage.TabIndex = 1011;
			tbEffectDamage.TextAlign = HorizontalAlignment.Center;
			tbEffectDamage.TextChanged += tbEffectDamage_TextChanged;
			// 
			// tbEffectAttack
			// 
			tbEffectAttack.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbEffectAttack.BackColor = Color.FromArgb(28, 30, 31);
			tbEffectAttack.BorderStyle = BorderStyle.FixedSingle;
			tbEffectAttack.ForeColor = Color.FromArgb(208, 203, 148);
			tbEffectAttack.Location = new Point(67, 51);
			tbEffectAttack.Margin = new Padding(4, 3, 4, 3);
			tbEffectAttack.Name = "tbEffectAttack";
			tbEffectAttack.Size = new Size(179, 23);
			tbEffectAttack.TabIndex = 1010;
			tbEffectAttack.TextAlign = HorizontalAlignment.Center;
			tbEffectAttack.TextChanged += tbEffectAttack_TextChanged;
			// 
			// label21
			// 
			label21.AutoSize = true;
			label21.ForeColor = Color.FromArgb(208, 203, 148);
			label21.Location = new Point(8, 84);
			label21.Margin = new Padding(4, 0, 4, 0);
			label21.Name = "label21";
			label21.Size = new Size(51, 15);
			label21.TabIndex = 1009;
			label21.Text = "Damage";
			label21.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label20
			// 
			label20.AutoSize = true;
			label20.ForeColor = Color.FromArgb(208, 203, 148);
			label20.Location = new Point(18, 55);
			label20.Margin = new Padding(4, 0, 4, 0);
			label20.Name = "label20";
			label20.Size = new Size(41, 15);
			label20.TabIndex = 1008;
			label20.Text = "Attack";
			label20.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label19
			// 
			label19.AutoSize = true;
			label19.ForeColor = Color.FromArgb(208, 203, 148);
			label19.Location = new Point(20, 26);
			label19.Margin = new Padding(4, 0, 4, 0);
			label19.Name = "label19";
			label19.Size = new Size(39, 15);
			label19.TabIndex = 1006;
			label19.Text = "Name";
			label19.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbEffectName
			// 
			tbEffectName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbEffectName.BackColor = Color.FromArgb(28, 30, 31);
			tbEffectName.BorderStyle = BorderStyle.FixedSingle;
			tbEffectName.ForeColor = Color.FromArgb(208, 203, 148);
			tbEffectName.Location = new Point(67, 22);
			tbEffectName.Margin = new Padding(4, 3, 4, 3);
			tbEffectName.Name = "tbEffectName";
			tbEffectName.Size = new Size(179, 23);
			tbEffectName.TabIndex = 1007;
			tbEffectName.TextAlign = HorizontalAlignment.Center;
			tbEffectName.TextChanged += tbEffectName_TextChanged;
			// 
			// cbEnable
			// 
			cbEnable.AutoSize = true;
			cbEnable.ForeColor = Color.FromArgb(208, 203, 148);
			cbEnable.Location = new Point(99, 57);
			cbEnable.Margin = new Padding(4, 3, 4, 3);
			cbEnable.Name = "cbEnable";
			cbEnable.Size = new Size(61, 19);
			cbEnable.TabIndex = 1049;
			cbEnable.Text = "Enable";
			cbEnable.UseVisualStyleBackColor = true;
			cbEnable.CheckedChanged += cbEnable_CheckedChanged;
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
			btnUpdate.Size = new Size(803, 27);
			btnUpdate.TabIndex = 999;
			btnUpdate.Text = "Update";
			btnUpdate.UseVisualStyleBackColor = false;
			btnUpdate.Click += btnUpdate_Click;
			// 
			// cbCastleSelector
			// 
			cbCastleSelector.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbCastleSelector.BackColor = Color.FromArgb(28, 30, 31);
			cbCastleSelector.DropDownStyle = ComboBoxStyle.DropDownList;
			cbCastleSelector.FlatStyle = FlatStyle.Flat;
			cbCastleSelector.ForeColor = Color.FromArgb(208, 203, 148);
			cbCastleSelector.FormattingEnabled = true;
			cbCastleSelector.Location = new Point(617, 55);
			cbCastleSelector.Margin = new Padding(4, 3, 4, 3);
			cbCastleSelector.Name = "cbCastleSelector";
			cbCastleSelector.Size = new Size(193, 23);
			cbCastleSelector.TabIndex = 16;
			cbCastleSelector.SelectedIndexChanged += cbGradeSelector_SelectedIndexChanged;
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
			label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label3.AutoSize = true;
			label3.ForeColor = Color.FromArgb(208, 203, 148);
			label3.Location = new Point(570, 59);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(39, 15);
			label3.TabIndex = 1006;
			label3.Text = "Castle";
			label3.TextAlign = ContentAlignment.MiddleRight;
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
			btnCopy.Location = new Point(169, 431);
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
			btnDelete.Location = new Point(247, 431);
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
			// TitleEditor
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(40, 40, 40);
			ClientSize = new Size(1156, 470);
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
			MinimumSize = new Size(1172, 509);
			Name = "TitleEditor";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Title Editor";
			FormClosing += TitleEditor_FormClosing;
			Load += TitleEditor_LoadAsync;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			panel1.ResumeLayout(false);
			groupBox6.ResumeLayout(false);
			groupBox6.PerformLayout();
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			((ISupportInitialize)tbTitleColorBlue).EndInit();
			((ISupportInitialize)tbTitleColorGreen).EndInit();
			((ISupportInitialize)tbTitleColorRed).EndInit();
			((ISupportInitialize)tbTitleColorOpacity).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((ISupportInitialize)tbBGColorBlue).EndInit();
			((ISupportInitialize)tbBGColorGreen).EndInit();
			((ISupportInitialize)tbBGColorRed).EndInit();
			((ISupportInitialize)tbBGColorOpacity).EndInit();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
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
		private GroupBox groupBox2;
		private Label label3;
		private TextBox tbSearch;
		private ComboBox cbCastleSelector;
		private CheckBox cbEnable;
		private GroupBox groupBox3;
		private TextBox tbEffectDamage;
		private TextBox tbEffectAttack;
		private Label label21;
		private Label label20;
		private Label label19;
		private TextBox tbEffectName;
		private TextBox tbDuration;
		private Label label6;
		private TrackBar tbBGColorOpacity;
		private Label label7;
		private TrackBar tbBGColorRed;
		private Label label2;
		private Label label8;
		private TrackBar tbBGColorGreen;
		private Label label9;
		private TrackBar tbBGColorBlue;
		private TextBox tbBGColor;
		private GroupBox groupBox4;
		private TextBox tbTitleColor;
		private Label label10;
		private TrackBar tbTitleColorBlue;
		private Label label11;
		private TrackBar tbTitleColorGreen;
		private Label label12;
		private TrackBar tbTitleColorRed;
		private Label label13;
		private TrackBar tbTitleColorOpacity;
		private Button btnClaimItem;
		private Label label14;
		private Label lbBGOpacity;
		private Label lbBGColorBlue;
		private Label lbBGColorGreen;
		private Label lbBGColorRed;
		private Label lbTitleColorBlue;
		private Label lbTitleColorGreen;
		private Label lbTitleColorRed;
		private Label lbTitleColorOpacity;
		private AlphaLabel lbTitleViewer;
		private Label lbFlag;
		private Button btnItemFlag;
		private GroupBox groupBox6;
		private ComboBox cbOptionLevel4;
		private ComboBox cbOptionID0;
		private Label label29;
		private Label label23;
		private ComboBox cbOptionID4;
		private Label label16;
		private Label label30;
		private ComboBox cbOptionLevel0;
		private ComboBox cbOptionLevel3;
		private Label label15;
		private Label label17;
		private ComboBox cbOptionID1;
		private ComboBox cbOptionID3;
		private Label label5;
		private Label label18;
		private ComboBox cbOptionLevel1;
		private ComboBox cbOptionLevel2;
		private Label label22;
		private Label label4;
		private ComboBox cbOptionID2;
		private Panel panel1;
	}
}
