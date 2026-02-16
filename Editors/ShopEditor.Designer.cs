using System.Windows.Forms;

namespace LastChaos_ToolBoxNG
{
    partial class ShopEditor
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
			DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
			btnReload = new Button();
			btnAddNew = new Button();
			MainList = new ListBox();
			groupBox1 = new GroupBox();
			groupBox3 = new GroupBox();
			gridSaleItems = new DataGridView();
			itemIcon = new DataGridViewImageColumn();
			item = new DataGridViewTextBoxColumn();
			itemPlus = new DataGridViewTextBoxColumn();
			nation = new DataGridViewTextBoxColumn();
			btnNPCID = new Button();
			groupBox2 = new GroupBox();
			tbYLayer = new TextBox();
			label9 = new Label();
			tbPosR = new TextBox();
			label8 = new Label();
			tbPosH = new TextBox();
			label7 = new Label();
			tbPosZ = new TextBox();
			label6 = new Label();
			tbPosX = new TextBox();
			cbZoneNumSelector = new ComboBox();
			label4 = new Label();
			label5 = new Label();
			label3 = new Label();
			tbBuyRate = new TextBox();
			label2 = new Label();
			tbSellRate = new TextBox();
			lRequiredQuest = new Label();
			btnRequiredQuest = new Button();
			btnUpdate = new Button();
			label1 = new Label();
			btnCopy = new Button();
			btnDelete = new Button();
			tbSearch = new TextBox();
			cbRenderDialog = new CheckBox();
			groupBox1.SuspendLayout();
			groupBox3.SuspendLayout();
			((ISupportInitialize)gridSaleItems).BeginInit();
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
			btnReload.Location = new Point(13, 481);
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
			btnAddNew.Location = new Point(91, 481);
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
			MainList.Size = new Size(304, 437);
			MainList.TabIndex = 1;
			MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBox1.Controls.Add(groupBox3);
			groupBox1.Controls.Add(btnNPCID);
			groupBox1.Controls.Add(groupBox2);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(tbBuyRate);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(tbSellRate);
			groupBox1.Controls.Add(lRequiredQuest);
			groupBox1.Controls.Add(btnRequiredQuest);
			groupBox1.Controls.Add(btnUpdate);
			groupBox1.Controls.Add(label1);
			groupBox1.FlatStyle = FlatStyle.Flat;
			groupBox1.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox1.Location = new Point(325, 12);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(650, 521);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Shop Data";
			// 
			// groupBox3
			// 
			groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBox3.Controls.Add(gridSaleItems);
			groupBox3.FlatStyle = FlatStyle.Flat;
			groupBox3.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox3.Location = new Point(7, 170);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new Size(636, 345);
			groupBox3.TabIndex = 1060;
			groupBox3.TabStop = false;
			groupBox3.Text = "Sale Items";
			// 
			// gridSaleItems
			// 
			gridSaleItems.AllowUserToAddRows = false;
			gridSaleItems.AllowUserToDeleteRows = false;
			gridSaleItems.AllowUserToResizeRows = false;
			gridSaleItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gridSaleItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			gridSaleItems.BackgroundColor = Color.FromArgb(28, 30, 31);
			gridSaleItems.BorderStyle = BorderStyle.None;
			gridSaleItems.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.BackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle4.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle4.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
			gridSaleItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
			gridSaleItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			gridSaleItems.Columns.AddRange(new DataGridViewColumn[] { itemIcon, item, itemPlus, nation });
			dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.BackColor = Color.FromArgb(40, 40, 40);
			dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle5.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle5.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
			gridSaleItems.DefaultCellStyle = dataGridViewCellStyle5;
			gridSaleItems.EditMode = DataGridViewEditMode.EditOnEnter;
			gridSaleItems.EnableHeadersVisualStyles = false;
			gridSaleItems.GridColor = Color.FromArgb(91, 85, 76);
			gridSaleItems.Location = new Point(8, 22);
			gridSaleItems.Margin = new Padding(4, 3, 4, 3);
			gridSaleItems.MultiSelect = false;
			gridSaleItems.Name = "gridSaleItems";
			gridSaleItems.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle6.BackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle6.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle6.WrapMode = DataGridViewTriState.True;
			gridSaleItems.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
			gridSaleItems.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
			gridSaleItems.ScrollBars = ScrollBars.Vertical;
			gridSaleItems.Size = new Size(620, 317);
			gridSaleItems.TabIndex = 0;
			gridSaleItems.CellMouseClick += gridSaleItems_CellMouseClick;
			gridSaleItems.CellValueChanged += gridSaleItems_CellValueChanged;
			// 
			// itemIcon
			// 
			itemIcon.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
			itemIcon.Frozen = true;
			itemIcon.HeaderText = "";
			itemIcon.Name = "itemIcon";
			itemIcon.Resizable = DataGridViewTriState.False;
			itemIcon.Width = 25;
			// 
			// item
			// 
			item.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			item.HeaderText = "Item";
			item.Name = "item";
			item.ReadOnly = true;
			item.Resizable = DataGridViewTriState.True;
			item.SortMode = DataGridViewColumnSortMode.NotSortable;
			// 
			// itemPlus
			// 
			itemPlus.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			itemPlus.HeaderText = "Plus";
			itemPlus.Name = "itemPlus";
			itemPlus.SortMode = DataGridViewColumnSortMode.NotSortable;
			itemPlus.Visible = false;
			// 
			// nation
			// 
			nation.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			nation.HeaderText = "Block Nations";
			nation.Name = "nation";
			nation.ReadOnly = true;
			nation.Resizable = DataGridViewTriState.True;
			nation.SortMode = DataGridViewColumnSortMode.NotSortable;
			nation.Width = 78;
			// 
			// btnNPCID
			// 
			btnNPCID.BackColor = Color.FromArgb(40, 40, 40);
			btnNPCID.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnNPCID.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnNPCID.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnNPCID.FlatStyle = FlatStyle.Flat;
			btnNPCID.ForeColor = Color.FromArgb(208, 203, 148);
			btnNPCID.ImageAlign = ContentAlignment.MiddleLeft;
			btnNPCID.Location = new Point(47, 55);
			btnNPCID.Margin = new Padding(4, 3, 4, 3);
			btnNPCID.Name = "btnNPCID";
			btnNPCID.Size = new Size(307, 27);
			btnNPCID.TabIndex = 1059;
			btnNPCID.UseVisualStyleBackColor = false;
			btnNPCID.Click += btnNPCID_Click;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(tbYLayer);
			groupBox2.Controls.Add(label9);
			groupBox2.Controls.Add(tbPosR);
			groupBox2.Controls.Add(label8);
			groupBox2.Controls.Add(tbPosH);
			groupBox2.Controls.Add(label7);
			groupBox2.Controls.Add(tbPosZ);
			groupBox2.Controls.Add(label6);
			groupBox2.Controls.Add(tbPosX);
			groupBox2.Controls.Add(cbZoneNumSelector);
			groupBox2.Controls.Add(label4);
			groupBox2.Controls.Add(label5);
			groupBox2.FlatStyle = FlatStyle.Flat;
			groupBox2.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox2.Location = new Point(362, 55);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(280, 109);
			groupBox2.TabIndex = 1058;
			groupBox2.TabStop = false;
			groupBox2.Text = "Position";
			// 
			// tbYLayer
			// 
			tbYLayer.BackColor = Color.FromArgb(28, 30, 31);
			tbYLayer.BorderStyle = BorderStyle.FixedSingle;
			tbYLayer.ForeColor = Color.FromArgb(208, 203, 148);
			tbYLayer.Location = new Point(177, 80);
			tbYLayer.Margin = new Padding(4, 3, 4, 3);
			tbYLayer.Name = "tbYLayer";
			tbYLayer.Size = new Size(48, 23);
			tbYLayer.TabIndex = 1065;
			tbYLayer.TextAlign = HorizontalAlignment.Center;
			tbYLayer.TextChanged += tbYLayer_TextChanged;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.ForeColor = Color.FromArgb(208, 203, 148);
			label9.Location = new Point(124, 84);
			label9.Margin = new Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new Size(45, 15);
			label9.TabIndex = 1064;
			label9.Text = "Y Layer";
			label9.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbPosR
			// 
			tbPosR.BackColor = Color.FromArgb(28, 30, 31);
			tbPosR.BorderStyle = BorderStyle.FixedSingle;
			tbPosR.ForeColor = Color.FromArgb(208, 203, 148);
			tbPosR.Location = new Point(68, 80);
			tbPosR.Margin = new Padding(4, 3, 4, 3);
			tbPosR.Name = "tbPosR";
			tbPosR.Size = new Size(48, 23);
			tbPosR.TabIndex = 1063;
			tbPosR.TextAlign = HorizontalAlignment.Center;
			tbPosR.TextChanged += tbPosR_TextChanged;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.ForeColor = Color.FromArgb(208, 203, 148);
			label8.Location = new Point(8, 84);
			label8.Margin = new Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new Size(52, 15);
			label8.TabIndex = 1062;
			label8.Text = "Rotation";
			label8.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbPosH
			// 
			tbPosH.BackColor = Color.FromArgb(28, 30, 31);
			tbPosH.BorderStyle = BorderStyle.FixedSingle;
			tbPosH.ForeColor = Color.FromArgb(208, 203, 148);
			tbPosH.Location = new Point(212, 51);
			tbPosH.Margin = new Padding(4, 3, 4, 3);
			tbPosH.Name = "tbPosH";
			tbPosH.Size = new Size(60, 23);
			tbPosH.TabIndex = 1061;
			tbPosH.TextAlign = HorizontalAlignment.Center;
			tbPosH.TextChanged += tbPosH_TextChanged;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.ForeColor = Color.FromArgb(208, 203, 148);
			label7.Location = new Point(188, 55);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new Size(16, 15);
			label7.TabIndex = 1060;
			label7.Text = "H";
			label7.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbPosZ
			// 
			tbPosZ.BackColor = Color.FromArgb(28, 30, 31);
			tbPosZ.BorderStyle = BorderStyle.FixedSingle;
			tbPosZ.ForeColor = Color.FromArgb(208, 203, 148);
			tbPosZ.Location = new Point(120, 51);
			tbPosZ.Margin = new Padding(4, 3, 4, 3);
			tbPosZ.Name = "tbPosZ";
			tbPosZ.Size = new Size(60, 23);
			tbPosZ.TabIndex = 1059;
			tbPosZ.TextAlign = HorizontalAlignment.Center;
			tbPosZ.TextChanged += tbPosZ_TextChanged;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.ForeColor = Color.FromArgb(208, 203, 148);
			label6.Location = new Point(98, 55);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new Size(14, 15);
			label6.TabIndex = 1058;
			label6.Text = "Z";
			label6.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbPosX
			// 
			tbPosX.BackColor = Color.FromArgb(28, 30, 31);
			tbPosX.BorderStyle = BorderStyle.FixedSingle;
			tbPosX.ForeColor = Color.FromArgb(208, 203, 148);
			tbPosX.Location = new Point(30, 51);
			tbPosX.Margin = new Padding(4, 3, 4, 3);
			tbPosX.Name = "tbPosX";
			tbPosX.Size = new Size(60, 23);
			tbPosX.TabIndex = 1057;
			tbPosX.TextAlign = HorizontalAlignment.Center;
			tbPosX.TextChanged += tbPosX_TextChanged;
			// 
			// cbZoneNumSelector
			// 
			cbZoneNumSelector.BackColor = Color.FromArgb(28, 30, 31);
			cbZoneNumSelector.DropDownStyle = ComboBoxStyle.DropDownList;
			cbZoneNumSelector.ForeColor = Color.FromArgb(208, 203, 148);
			cbZoneNumSelector.FormattingEnabled = true;
			cbZoneNumSelector.Location = new Point(50, 22);
			cbZoneNumSelector.Margin = new Padding(4, 3, 4, 3);
			cbZoneNumSelector.Name = "cbZoneNumSelector";
			cbZoneNumSelector.Size = new Size(222, 23);
			cbZoneNumSelector.TabIndex = 1044;
			cbZoneNumSelector.SelectedIndexChanged += cbZoneNumSelector_SelectedIndexChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.ForeColor = Color.FromArgb(208, 203, 148);
			label4.Location = new Point(8, 55);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(14, 15);
			label4.TabIndex = 1056;
			label4.Text = "X";
			label4.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.ForeColor = Color.FromArgb(208, 203, 148);
			label5.Location = new Point(8, 26);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new Size(34, 15);
			label5.TabIndex = 1045;
			label5.Text = "Zone";
			label5.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.ForeColor = Color.FromArgb(208, 203, 148);
			label3.Location = new Point(184, 92);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(127, 15);
			label3.TabIndex = 1054;
			label3.Text = "Buy Price Multiplicator";
			label3.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbBuyRate
			// 
			tbBuyRate.BackColor = Color.FromArgb(28, 30, 31);
			tbBuyRate.BorderStyle = BorderStyle.FixedSingle;
			tbBuyRate.ForeColor = Color.FromArgb(208, 203, 148);
			tbBuyRate.Location = new Point(319, 88);
			tbBuyRate.Margin = new Padding(4, 3, 4, 3);
			tbBuyRate.Name = "tbBuyRate";
			tbBuyRate.Size = new Size(35, 23);
			tbBuyRate.TabIndex = 1055;
			tbBuyRate.TextAlign = HorizontalAlignment.Center;
			tbBuyRate.TextChanged += tbBuyRate_TextChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.ForeColor = Color.FromArgb(208, 203, 148);
			label2.Location = new Point(8, 92);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(125, 15);
			label2.TabIndex = 1052;
			label2.Text = "Sell Price Multiplicator";
			label2.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbSellRate
			// 
			tbSellRate.BackColor = Color.FromArgb(28, 30, 31);
			tbSellRate.BorderStyle = BorderStyle.FixedSingle;
			tbSellRate.ForeColor = Color.FromArgb(208, 203, 148);
			tbSellRate.Location = new Point(141, 88);
			tbSellRate.Margin = new Padding(4, 3, 4, 3);
			tbSellRate.Name = "tbSellRate";
			tbSellRate.Size = new Size(35, 23);
			tbSellRate.TabIndex = 1053;
			tbSellRate.TextAlign = HorizontalAlignment.Center;
			tbSellRate.TextChanged += tbSellRate_TextChanged;
			// 
			// lRequiredQuest
			// 
			lRequiredQuest.AutoSize = true;
			lRequiredQuest.ForeColor = Color.FromArgb(208, 203, 148);
			lRequiredQuest.Location = new Point(8, 123);
			lRequiredQuest.Margin = new Padding(4, 0, 4, 0);
			lRequiredQuest.Name = "lRequiredQuest";
			lRequiredQuest.Size = new Size(88, 15);
			lRequiredQuest.TabIndex = 1048;
			lRequiredQuest.Text = "Required Quest";
			lRequiredQuest.TextAlign = ContentAlignment.MiddleRight;
			lRequiredQuest.Visible = false;
			// 
			// btnRequiredQuest
			// 
			btnRequiredQuest.BackColor = Color.FromArgb(40, 40, 40);
			btnRequiredQuest.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnRequiredQuest.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnRequiredQuest.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnRequiredQuest.FlatStyle = FlatStyle.Flat;
			btnRequiredQuest.ForeColor = Color.FromArgb(208, 203, 148);
			btnRequiredQuest.ImageAlign = ContentAlignment.MiddleLeft;
			btnRequiredQuest.Location = new Point(104, 117);
			btnRequiredQuest.Margin = new Padding(4, 3, 4, 3);
			btnRequiredQuest.Name = "btnRequiredQuest";
			btnRequiredQuest.Size = new Size(250, 27);
			btnRequiredQuest.TabIndex = 1047;
			btnRequiredQuest.UseVisualStyleBackColor = false;
			btnRequiredQuest.Visible = false;
			btnRequiredQuest.Click += btnRequiredQuest_Click;
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
			btnUpdate.ForeColor = Color.FromArgb(208, 203, 148);
			btnUpdate.Location = new Point(8, 22);
			btnUpdate.Margin = new Padding(4, 3, 4, 3);
			btnUpdate.Name = "btnUpdate";
			btnUpdate.Size = new Size(634, 27);
			btnUpdate.TabIndex = 999;
			btnUpdate.Text = "Update";
			btnUpdate.UseVisualStyleBackColor = false;
			btnUpdate.Click += btnUpdate_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.ForeColor = Color.FromArgb(208, 203, 148);
			label1.Location = new Point(8, 61);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(31, 15);
			label1.TabIndex = 1;
			label1.Text = "NPC";
			label1.TextAlign = ContentAlignment.MiddleRight;
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
			btnCopy.Location = new Point(169, 481);
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
			btnDelete.Location = new Point(247, 481);
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
			// cbRenderDialog
			// 
			cbRenderDialog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			cbRenderDialog.AutoSize = true;
			cbRenderDialog.ForeColor = Color.FromArgb(208, 203, 148);
			cbRenderDialog.Location = new Point(13, 514);
			cbRenderDialog.Margin = new Padding(4, 3, 4, 3);
			cbRenderDialog.Name = "cbRenderDialog";
			cbRenderDialog.Size = new Size(105, 19);
			cbRenderDialog.TabIndex = 3;
			cbRenderDialog.Text = "3D View Dialog";
			cbRenderDialog.UseVisualStyleBackColor = true;
			cbRenderDialog.CheckedChanged += cbRenderDialog_CheckedChanged;
			// 
			// ShopEditor
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(40, 40, 40);
			ClientSize = new Size(988, 545);
			Controls.Add(cbRenderDialog);
			Controls.Add(tbSearch);
			Controls.Add(btnDelete);
			Controls.Add(btnCopy);
			Controls.Add(MainList);
			Controls.Add(btnAddNew);
			Controls.Add(btnReload);
			Controls.Add(groupBox1);
			ForeColor = Color.FromArgb(208, 203, 148);
			Icon = Properties.Resources.NG;
			Margin = new Padding(4, 3, 4, 3);
			MinimumSize = new Size(1004, 584);
			Name = "ShopEditor";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Shop Editor";
			FormClosing += ShopEditor_FormClosing;
			Load += ShopEditor_LoadAsync;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox3.ResumeLayout(false);
			((ISupportInitialize)gridSaleItems).EndInit();
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
        private Button btnUpdate;
        private Button btnCopy;
        private Button btnDelete;
        private Label label1;
        private TextBox tbSearch;
		private DataGridView gridSaleItems;
		private Button btnRequiredQuest;
		private Label lRequiredQuest;
		private ComboBox cbZoneNumSelector;
		private Label label5;
		private Label label2;
		private TextBox tbSellRate;
		private Label label3;
		private TextBox tbBuyRate;
		private Label label4;
		private TextBox tbPosX;
		private GroupBox groupBox2;
		private TextBox tbPosZ;
		private Label label6;
		private TextBox tbPosH;
		private Label label7;
		private TextBox tbPosR;
		private Label label8;
		private TextBox tbYLayer;
		private Label label9;
		private CheckBox cbRenderDialog;
		private Button btnNPCID;
		private GroupBox groupBox3;
		private DataGridViewImageColumn itemIcon;
		private DataGridViewTextBoxColumn item;
		private DataGridViewTextBoxColumn itemPlus;
		private DataGridViewTextBoxColumn nation;
	}
}