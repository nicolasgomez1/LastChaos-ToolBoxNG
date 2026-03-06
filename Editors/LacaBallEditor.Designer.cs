using System.Windows.Forms;

namespace LastChaos_ToolBoxNG
{
	partial class LacaBallEditor
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
			groupBox1 = new GroupBox();
			label2 = new Label();
			btnRequiredItem = new Button();
			btnUpdate = new Button();
			gridRewards = new DataGridView();
			itemIcon = new DataGridViewImageColumn();
			item = new DataGridViewTextBoxColumn();
			count = new DataGridViewTextBoxColumn();
			max = new DataGridViewTextBoxColumn();
			remain = new DataGridViewTextBoxColumn();
			tbSearch = new TextBox();
			btnDelete = new Button();
			btnCopy = new Button();
			MainList = new ListBox();
			btnAddNew = new Button();
			groupBox1.SuspendLayout();
			((ISupportInitialize)gridRewards).BeginInit();
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
			btnReload.Location = new Point(13, 524);
			btnReload.Margin = new Padding(4, 3, 4, 3);
			btnReload.Name = "btnReload";
			btnReload.Size = new Size(70, 27);
			btnReload.TabIndex = 0;
			btnReload.Text = "Reload";
			btnReload.UseVisualStyleBackColor = false;
			btnReload.Click += btnReload_Click;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(btnRequiredItem);
			groupBox1.Controls.Add(btnUpdate);
			groupBox1.Controls.Add(gridRewards);
			groupBox1.FlatStyle = FlatStyle.Flat;
			groupBox1.ForeColor = Color.FromArgb(208, 203, 148);
			groupBox1.Location = new Point(325, 12);
			groupBox1.Margin = new Padding(4, 3, 4, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new Padding(4, 3, 4, 3);
			groupBox1.Size = new Size(746, 539);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "LacaBall Rewards Data";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.ForeColor = Color.FromArgb(208, 203, 148);
			label2.Location = new Point(8, 61);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(66, 15);
			label2.TabIndex = 1049;
			label2.Text = "Token Item";
			label2.TextAlign = ContentAlignment.MiddleRight;
			// 
			// btnRequiredItem
			// 
			btnRequiredItem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			btnRequiredItem.BackColor = Color.FromArgb(40, 40, 40);
			btnRequiredItem.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnRequiredItem.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnRequiredItem.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnRequiredItem.FlatStyle = FlatStyle.Flat;
			btnRequiredItem.ForeColor = Color.FromArgb(208, 203, 148);
			btnRequiredItem.ImageAlign = ContentAlignment.MiddleLeft;
			btnRequiredItem.Location = new Point(82, 55);
			btnRequiredItem.Margin = new Padding(4, 3, 4, 3);
			btnRequiredItem.Name = "btnRequiredItem";
			btnRequiredItem.Size = new Size(656, 27);
			btnRequiredItem.TabIndex = 1048;
			btnRequiredItem.UseVisualStyleBackColor = false;
			btnRequiredItem.Click += btnRequiredItem_Click;
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
			btnUpdate.Size = new Size(730, 27);
			btnUpdate.TabIndex = 1001;
			btnUpdate.Text = "Update";
			btnUpdate.UseVisualStyleBackColor = false;
			btnUpdate.Click += btnUpdate_Click;
			// 
			// gridRewards
			// 
			gridRewards.AllowUserToAddRows = false;
			gridRewards.AllowUserToDeleteRows = false;
			gridRewards.AllowUserToResizeRows = false;
			gridRewards.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gridRewards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			gridRewards.BackgroundColor = Color.FromArgb(28, 30, 31);
			gridRewards.BorderStyle = BorderStyle.None;
			gridRewards.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.BackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle4.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle4.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
			gridRewards.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
			gridRewards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			gridRewards.Columns.AddRange(new DataGridViewColumn[] { itemIcon, item, count, max, remain });
			dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.BackColor = Color.FromArgb(40, 40, 40);
			dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle5.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle5.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
			gridRewards.DefaultCellStyle = dataGridViewCellStyle5;
			gridRewards.EditMode = DataGridViewEditMode.EditOnEnter;
			gridRewards.EnableHeadersVisualStyles = false;
			gridRewards.GridColor = Color.FromArgb(91, 85, 76);
			gridRewards.Location = new Point(8, 88);
			gridRewards.Margin = new Padding(4, 3, 4, 3);
			gridRewards.MultiSelect = false;
			gridRewards.Name = "gridRewards";
			gridRewards.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle6.BackColor = Color.FromArgb(60, 56, 54);
			dataGridViewCellStyle6.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle6.ForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(208, 203, 148);
			dataGridViewCellStyle6.WrapMode = DataGridViewTriState.True;
			gridRewards.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
			gridRewards.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
			gridRewards.ScrollBars = ScrollBars.Vertical;
			gridRewards.Size = new Size(730, 445);
			gridRewards.TabIndex = 1000;
			gridRewards.CellMouseClick += gridRewards_CellMouseClick;
			gridRewards.CellValueChanged += gridRewards_CellValueChanged;
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
			// count
			// 
			count.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			count.HeaderText = "Amount to Give";
			count.MinimumWidth = 85;
			count.Name = "count";
			count.SortMode = DataGridViewColumnSortMode.NotSortable;
			count.Width = 85;
			// 
			// max
			// 
			max.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			max.HeaderText = "Max Available";
			max.MinimumWidth = 80;
			max.Name = "max";
			max.SortMode = DataGridViewColumnSortMode.NotSortable;
			max.Width = 80;
			// 
			// remain
			// 
			remain.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			remain.HeaderText = "Remaining";
			remain.Name = "remain";
			remain.SortMode = DataGridViewColumnSortMode.NotSortable;
			remain.Width = 62;
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
			tbSearch.TabIndex = 3;
			tbSearch.TextChanged += tbSearch_TextChanged;
			tbSearch.KeyDown += tbSearch_KeyDown;
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
			btnDelete.Location = new Point(247, 524);
			btnDelete.Margin = new Padding(4, 3, 4, 3);
			btnDelete.Name = "btnDelete";
			btnDelete.Size = new Size(70, 27);
			btnDelete.TabIndex = 6;
			btnDelete.Text = "Delete";
			btnDelete.UseVisualStyleBackColor = false;
			btnDelete.Click += btnDelete_Click;
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
			btnCopy.Location = new Point(169, 524);
			btnCopy.Margin = new Padding(4, 3, 4, 3);
			btnCopy.Name = "btnCopy";
			btnCopy.Size = new Size(70, 27);
			btnCopy.TabIndex = 7;
			btnCopy.Text = "Copy";
			btnCopy.UseVisualStyleBackColor = false;
			btnCopy.Click += btnCopy_Click;
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
			MainList.TabIndex = 9;
			MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
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
			btnAddNew.Location = new Point(91, 524);
			btnAddNew.Margin = new Padding(4, 3, 4, 3);
			btnAddNew.Name = "btnAddNew";
			btnAddNew.Size = new Size(70, 27);
			btnAddNew.TabIndex = 8;
			btnAddNew.Text = "Add New";
			btnAddNew.UseVisualStyleBackColor = false;
			btnAddNew.Click += btnAddNew_Click;
			// 
			// LacaBallEditor
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(40, 40, 40);
			ClientSize = new Size(1084, 563);
			Controls.Add(btnDelete);
			Controls.Add(btnCopy);
			Controls.Add(MainList);
			Controls.Add(btnAddNew);
			Controls.Add(tbSearch);
			Controls.Add(btnReload);
			Controls.Add(groupBox1);
			Icon = Properties.Resources.NG;
			Margin = new Padding(4, 3, 4, 3);
			MinimumSize = new Size(1100, 602);
			Name = "LacaBallEditor";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "LacaBall Editor";
			FormClosing += LacaBallEditor_FormClosing;
			Load += LacaBallEditor_LoadAsync;
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((ISupportInitialize)gridRewards).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnReload;
		private System.Windows.Forms.GroupBox groupBox1;
		private DataGridView gridRewards;
		private TextBox tbSearch;
		private Button btnDelete;
		private Button btnCopy;
		private ListBox MainList;
		private Button btnAddNew;
		private Button btnUpdate;
		private Button btnRequiredItem;
		private Label label2;
		private DataGridViewImageColumn itemIcon;
		private DataGridViewTextBoxColumn item;
		private DataGridViewTextBoxColumn count;
		private DataGridViewTextBoxColumn max;
		private DataGridViewTextBoxColumn remain;
	}
}