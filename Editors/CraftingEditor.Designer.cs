using System.Windows.Forms;

namespace LastChaos_ToolBoxNG
{
    partial class CraftingEditor
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
			DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
			btnReload = new Button();
			btnAddNew = new Button();
			MainList = new ListBox();
			groupBox1 = new GroupBox();
			groupBox3 = new GroupBox();
			gridMaterials = new DataGridView();
			itemIcon = new DataGridViewImageColumn();
			item = new DataGridViewTextBoxColumn();
			amount = new DataGridViewTextBoxColumn();
			label8 = new Label();
			tbNeededExp = new TextBox();
			label7 = new Label();
			tbNeededGold = new TextBox();
			label4 = new Label();
			btnSealType = new Button();
			groupBox2 = new GroupBox();
			label6 = new Label();
			btnItemResult = new Button();
			tbResultEXP = new TextBox();
			cbItemResultType = new ComboBox();
			label3 = new Label();
			cbJobSelector = new ComboBox();
			label5 = new Label();
			tbResultItemName = new TextBox();
			label2 = new Label();
			cbEnable = new CheckBox();
			btnUpdate = new Button();
			label1 = new Label();
			tbID = new TextBox();
			btnCopy = new Button();
			btnDelete = new Button();
			tbSearch = new TextBox();
			groupBox1.SuspendLayout();
			groupBox3.SuspendLayout();
			((ISupportInitialize)gridMaterials).BeginInit();
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
			btnReload.Location = new Point(13, 339);
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
			btnAddNew.Location = new Point(91, 339);
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
			MainList.Size = new Size(304, 287);
			MainList.TabIndex = 1;
			MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBox1.Controls.Add(groupBox3);
			groupBox1.Controls.Add(label8);
			groupBox1.Controls.Add(tbNeededExp);
			groupBox1.Controls.Add(label7);
			groupBox1.Controls.Add(tbNeededGold);
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(btnSealType);
			groupBox1.Controls.Add(groupBox2);
			groupBox1.Controls.Add(cbJobSelector);
			groupBox1.Controls.Add(label5);
			groupBox1.Controls.Add(tbResultItemName);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(cbEnable);
			groupBox1.Controls.Add(btnUpdate);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(tbID);
			groupBox1.FlatStyle = FlatStyle.Flat;
			groupBox1.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox1.Location = new Point(325, 12);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(585, 354);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Option Data";
			// 
			// groupBox3
			// 
			groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			groupBox3.Controls.Add(gridMaterials);
			groupBox3.FlatStyle = FlatStyle.Flat;
			groupBox3.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox3.Location = new Point(8, 178);
			groupBox3.Margin = new Padding(4, 3, 4, 3);
			groupBox3.Name = "groupBox3";
			groupBox3.Padding = new Padding(4, 3, 4, 3);
			groupBox3.Size = new Size(569, 170);
			groupBox3.TabIndex = 1051;
			groupBox3.TabStop = false;
			groupBox3.Text = "Needed Items";
			// 
			// gridMaterials
			// 
			gridMaterials.AllowUserToAddRows = false;
			gridMaterials.AllowUserToDeleteRows = false;
			gridMaterials.AllowUserToResizeRows = false;
			gridMaterials.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gridMaterials.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			gridMaterials.BackgroundColor = Color.FromArgb(28, 30, 31);
			gridMaterials.BorderStyle = BorderStyle.None;
			gridMaterials.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle7.BackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle7.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle7.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle7.SelectionBackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle7.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
			gridMaterials.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
			gridMaterials.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			gridMaterials.Columns.AddRange(new DataGridViewColumn[] { itemIcon, item, amount });
			dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle8.BackColor = Color.FromArgb(40, 40, 40);
			dataGridViewCellStyle8.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle8.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle8.SelectionBackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle8.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle8.WrapMode = DataGridViewTriState.False;
			gridMaterials.DefaultCellStyle = dataGridViewCellStyle8;
			gridMaterials.EditMode = DataGridViewEditMode.EditOnEnter;
			gridMaterials.EnableHeadersVisualStyles = false;
			gridMaterials.GridColor = Color.FromArgb(91, 85, 76);
			gridMaterials.Location = new Point(8, 22);
			gridMaterials.Margin = new Padding(4, 3, 4, 3);
			gridMaterials.MultiSelect = false;
			gridMaterials.Name = "gridMaterials";
			gridMaterials.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle9.BackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle9.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle9.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle9.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle9.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle9.WrapMode = DataGridViewTriState.True;
			gridMaterials.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
			gridMaterials.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
			gridMaterials.ScrollBars = ScrollBars.Vertical;
			gridMaterials.Size = new Size(553, 142);
			gridMaterials.TabIndex = 0;
			gridMaterials.CellMouseClick += gridMaterials_CellMouseClick;
			gridMaterials.CellValueChanged += gridMaterials_CellValueChanged;
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
			// amount
			// 
			amount.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			amount.HeaderText = "Amount";
			amount.Name = "amount";
			amount.SortMode = DataGridViewColumnSortMode.NotSortable;
			amount.Width = 48;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.ForeColor = Color.FromArgb(208, 203, 148);
			label8.Location = new Point(8, 153);
			label8.Margin = new Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new Size(107, 15);
			label8.TabIndex = 1051;
			label8.Text = "Needed Experience";
			label8.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbNeededExp
			// 
			tbNeededExp.BackColor = Color.FromArgb(28, 30, 31);
			tbNeededExp.BorderStyle = BorderStyle.FixedSingle;
			tbNeededExp.ForeColor = Color.FromArgb(208, 203, 148);
			tbNeededExp.Location = new Point(123, 149);
			tbNeededExp.Margin = new Padding(4, 3, 4, 3);
			tbNeededExp.Name = "tbNeededExp";
			tbNeededExp.Size = new Size(181, 23);
			tbNeededExp.TabIndex = 1052;
			tbNeededExp.TextChanged += tbNeededExp_TextChanged;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.ForeColor = Color.FromArgb(208, 203, 148);
			label7.Location = new Point(312, 153);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new Size(76, 15);
			label7.TabIndex = 1049;
			label7.Text = "Needed Gold";
			label7.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbNeededGold
			// 
			tbNeededGold.BackColor = Color.FromArgb(166, 166, 166);
			tbNeededGold.BorderStyle = BorderStyle.FixedSingle;
			tbNeededGold.ForeColor = Color.FromArgb(208, 203, 148);
			tbNeededGold.Location = new Point(396, 149);
			tbNeededGold.Margin = new Padding(4, 3, 4, 3);
			tbNeededGold.Name = "tbNeededGold";
			tbNeededGold.Size = new Size(181, 23);
			tbNeededGold.TabIndex = 1050;
			tbNeededGold.TextChanged += tbNeededGold_TextChanged;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.ForeColor = Color.FromArgb(208, 203, 148);
			label4.Location = new Point(8, 119);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(64, 15);
			label4.TabIndex = 1048;
			label4.Text = "Skill Group";
			label4.TextAlign = ContentAlignment.MiddleRight;
			// 
			// btnSealType
			// 
			btnSealType.BackColor = Color.FromArgb(40, 40, 40);
			btnSealType.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnSealType.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnSealType.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnSealType.FlatStyle = FlatStyle.Flat;
			btnSealType.ForeColor = Color.FromArgb(208, 203, 148);
			btnSealType.ImageAlign = ContentAlignment.MiddleLeft;
			btnSealType.Location = new Point(80, 113);
			btnSealType.Margin = new Padding(4, 3, 4, 3);
			btnSealType.Name = "btnSealType";
			btnSealType.Size = new Size(250, 27);
			btnSealType.TabIndex = 1047;
			btnSealType.UseVisualStyleBackColor = false;
			btnSealType.Click += btnSealType_Click;
			// 
			// groupBox2
			// 
			groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			groupBox2.Controls.Add(label6);
			groupBox2.Controls.Add(btnItemResult);
			groupBox2.Controls.Add(tbResultEXP);
			groupBox2.Controls.Add(cbItemResultType);
			groupBox2.Controls.Add(label3);
			groupBox2.FlatStyle = FlatStyle.Flat;
			groupBox2.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox2.Location = new Point(338, 55);
			groupBox2.Margin = new Padding(4, 3, 4, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new Padding(4, 3, 4, 3);
			groupBox2.Size = new Size(239, 88);
			groupBox2.TabIndex = 1046;
			groupBox2.TabStop = false;
			groupBox2.Text = "Results Data";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.ForeColor = Color.FromArgb(208, 203, 148);
			label6.Location = new Point(8, 59);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new Size(27, 15);
			label6.TabIndex = 1049;
			label6.Text = "EXP";
			label6.TextAlign = ContentAlignment.MiddleRight;
			// 
			// btnItemResult
			// 
			btnItemResult.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			btnItemResult.BackColor = Color.FromArgb(40, 40, 40);
			btnItemResult.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnItemResult.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnItemResult.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnItemResult.FlatStyle = FlatStyle.Flat;
			btnItemResult.ForeColor = Color.FromArgb(208, 203, 148);
			btnItemResult.ImageAlign = ContentAlignment.MiddleLeft;
			btnItemResult.Location = new Point(8, 22);
			btnItemResult.Margin = new Padding(4, 3, 4, 3);
			btnItemResult.Name = "btnItemResult";
			btnItemResult.Size = new Size(223, 27);
			btnItemResult.TabIndex = 1047;
			btnItemResult.UseVisualStyleBackColor = false;
			btnItemResult.Click += btnItemResult_Click;
			// 
			// tbResultEXP
			// 
			tbResultEXP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			tbResultEXP.BackColor = Color.FromArgb(28, 30, 31);
			tbResultEXP.BorderStyle = BorderStyle.FixedSingle;
			tbResultEXP.ForeColor = Color.FromArgb(208, 203, 148);
			tbResultEXP.Location = new Point(43, 55);
			tbResultEXP.Margin = new Padding(4, 3, 4, 3);
			tbResultEXP.Name = "tbResultEXP";
			tbResultEXP.Size = new Size(90, 23);
			tbResultEXP.TabIndex = 1050;
			tbResultEXP.TextChanged += tbResultEXP_TextChanged;
			// 
			// cbItemResultType
			// 
			cbItemResultType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			cbItemResultType.BackColor = Color.FromArgb(28, 30, 31);
			cbItemResultType.DropDownStyle = ComboBoxStyle.DropDownList;
			cbItemResultType.FlatStyle = FlatStyle.Flat;
			cbItemResultType.ForeColor = Color.FromArgb(208, 203, 148);
			cbItemResultType.FormattingEnabled = true;
			cbItemResultType.Location = new Point(181, 56);
			cbItemResultType.Margin = new Padding(4, 3, 4, 3);
			cbItemResultType.Name = "cbItemResultType";
			cbItemResultType.Size = new Size(50, 23);
			cbItemResultType.TabIndex = 16;
			cbItemResultType.Visible = false;
			cbItemResultType.SelectedIndexChanged += cbItemResultType_SelectedIndexChanged;
			// 
			// label3
			// 
			label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			label3.AutoSize = true;
			label3.ForeColor = Color.FromArgb(208, 203, 148);
			label3.Location = new Point(141, 60);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(32, 15);
			label3.TabIndex = 1006;
			label3.Text = "Type";
			label3.TextAlign = ContentAlignment.MiddleRight;
			label3.Visible = false;
			// 
			// cbJobSelector
			// 
			cbJobSelector.BackColor = Color.FromArgb(28, 30, 31);
			cbJobSelector.DropDownStyle = ComboBoxStyle.DropDownList;
			cbJobSelector.FlatStyle = FlatStyle.Flat;
			cbJobSelector.ForeColor = Color.FromArgb(208, 203, 148);
			cbJobSelector.FormattingEnabled = true;
			cbJobSelector.Location = new Point(230, 55);
			cbJobSelector.Margin = new Padding(4, 3, 4, 3);
			cbJobSelector.Name = "cbJobSelector";
			cbJobSelector.Size = new Size(100, 23);
			cbJobSelector.TabIndex = 1044;
			cbJobSelector.Visible = false;
			cbJobSelector.SelectedIndexChanged += cbJobSelector_SelectedIndexChanged;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.ForeColor = Color.FromArgb(208, 203, 148);
			label5.Location = new Point(195, 59);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new Size(25, 15);
			label5.TabIndex = 1045;
			label5.Text = "Job";
			label5.TextAlign = ContentAlignment.MiddleRight;
			label5.Visible = false;
			// 
			// tbResultItemName
			// 
			tbResultItemName.BackColor = Color.FromArgb(28, 30, 31);
			tbResultItemName.BorderStyle = BorderStyle.FixedSingle;
			tbResultItemName.ForeColor = Color.FromArgb(208, 203, 148);
			tbResultItemName.Location = new Point(55, 84);
			tbResultItemName.Margin = new Padding(4, 3, 4, 3);
			tbResultItemName.Name = "tbResultItemName";
			tbResultItemName.ReadOnly = true;
			tbResultItemName.Size = new Size(275, 23);
			tbResultItemName.TabIndex = 12;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.ForeColor = Color.FromArgb(208, 203, 148);
			label2.Location = new Point(8, 88);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(39, 15);
			label2.TabIndex = 14;
			label2.Text = "Name";
			label2.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbEnable
			// 
			cbEnable.AutoSize = true;
			cbEnable.ForeColor = Color.FromArgb(208, 203, 148);
			cbEnable.Location = new Point(99, 57);
			cbEnable.Margin = new Padding(4, 3, 4, 3);
			cbEnable.Name = "cbEnable";
			cbEnable.Size = new Size(61, 19);
			cbEnable.TabIndex = 1038;
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
			btnUpdate.Location = new Point(8, 22);
			btnUpdate.Margin = new Padding(4, 3, 4, 3);
			btnUpdate.Name = "btnUpdate";
			btnUpdate.Size = new Size(569, 27);
			btnUpdate.TabIndex = 999;
			btnUpdate.Text = "Update";
			btnUpdate.UseVisualStyleBackColor = false;
			btnUpdate.Click += btnUpdate_Click;
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
			btnCopy.Location = new Point(169, 339);
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
			btnDelete.Location = new Point(247, 339);
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
			// CraftingEditor
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(40, 40, 40);
			ClientSize = new Size(923, 378);
			Controls.Add(tbSearch);
			Controls.Add(btnDelete);
			Controls.Add(btnCopy);
			Controls.Add(MainList);
			Controls.Add(btnAddNew);
			Controls.Add(btnReload);
			Controls.Add(groupBox1);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = Properties.Resources.NG;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MinimumSize = new Size(939, 417);
			Name = "CraftingEditor";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Crafting Editor";
			FormClosing += CraftingEditor_FormClosing;
			Load += CraftingEditor_LoadAsync;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox3.ResumeLayout(false);
			((ISupportInitialize)gridMaterials).EndInit();
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
        private TextBox tbResultItemName;
        private Label label3;
        private TextBox tbSearch;
		private DataGridView gridMaterials;
		private ComboBox cbItemResultType;
		private CheckBox cbEnable;
		private ComboBox cbJobSelector;
		private Label label5;
		private GroupBox groupBox2;
		private Button btnItemResult;
		private Button btnSealType;
		private Label label4;
		private Label label6;
		private TextBox tbResultEXP;
		private Label label7;
		private TextBox tbNeededGold;
		private Label label8;
		private TextBox tbNeededExp;
		private GroupBox groupBox3;
		private DataGridViewImageColumn itemIcon;
		private DataGridViewTextBoxColumn item;
		private DataGridViewTextBoxColumn amount;
	}
}