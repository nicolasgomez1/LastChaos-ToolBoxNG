namespace LastChaos_ToolBoxNG
{
	public class StartingItemsEditor : Form
	{
		private const string TableName = "t_start_item";

		private readonly Main pMain;
		private readonly DataGridView grid = new();
		private readonly Button btnReload = new();
		private readonly Button btnAdd = new();
		private readonly Button btnDuplicate = new();
		private readonly Button btnPickItem = new();
		private readonly Button btnSeed = new();
		private readonly Button btnDelete = new();
		private readonly Button btnSave = new();
		private readonly Label lblStatus = new();

		private DataTable? rows;

		private static readonly (int Id, string Name)[] Jobs =
		[
			(-1, "All jobs"),
			(0, "Titan"),
			(1, "Knight"),
			(2, "Healer"),
			(3, "Mage"),
			(4, "Rogue"),
			(5, "Sorcerer"),
			(6, "NightShadow"),
			(7, "EX Rogue"),
			(8, "EX Mage")
		];

		public StartingItemsEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "StartingItemsEditor";
			Text = "Starting Items Editor";
			MinimumSize = new Size(1120, 650);
			Size = new Size(1320, 760);
			StartPosition = FormStartPosition.CenterParent;

			BuildLayout();
			Load += async (_, _) => await LoadEditorAsync();
		}

		private void BuildLayout()
		{
			TableLayoutPanel root = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 5,
				Padding = new Padding(8)
			};
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			root.Controls.Add(new Label
			{
				Dock = DockStyle.Fill,
				Text = "Edits ep4_db.t_start_item. If a job has rows here, character creation uses these rows instead of the old hardcoded starter gear. Worn rows use Wear pos >= 0; inventory rows use Wear pos -1 plus tab/row/column.",
				TextAlign = ContentAlignment.MiddleLeft
			}, 0, 0);

			FlowLayoutPanel top = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
			root.Controls.Add(top, 0, 1);

			ConfigureButton(btnReload, "Reload", async (_, _) => await LoadEditorAsync());
			ConfigureButton(btnAdd, "Add row", (_, _) => AddRow());
			ConfigureButton(btnDuplicate, "Duplicate row", (_, _) => DuplicateSelectedRow());
			ConfigureButton(btnPickItem, "Pick item for selected row", (_, _) => PickItemForSelectedRow());
			ConfigureButton(btnSeed, "Seed current defaults", async (_, _) => await SeedDefaultsAsync());
			top.Controls.AddRange([btnReload, btnAdd, btnDuplicate, btnPickItem, btnSeed]);

			ConfigureGrid();
			root.Controls.Add(grid, 0, 2);

			FlowLayoutPanel bottom = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
			root.Controls.Add(bottom, 0, 3);

			ConfigureButton(btnDelete, "Delete selected row", async (_, _) => await DeleteSelectedRowAsync());
			ConfigureButton(btnSave, "Save starting items", async (_, _) => await SaveAsync());
			bottom.Controls.AddRange([btnDelete, btnSave]);

			lblStatus.Dock = DockStyle.Fill;
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblStatus, 0, 4);
		}

		private static void ConfigureButton(Button button, string text, EventHandler handler)
		{
			button.Text = text;
			button.AutoSize = true;
			button.Margin = new Padding(0, 4, 8, 0);
			button.Click += handler;
		}

		private void ConfigureGrid()
		{
			grid.Dock = DockStyle.Fill;
			grid.AutoGenerateColumns = false;
			grid.AllowUserToAddRows = false;
			grid.AllowUserToDeleteRows = false;
			grid.RowHeadersVisible = false;
			grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			grid.MultiSelect = false;
			grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			grid.DataError += (_, _) => { };
			grid.CellDoubleClick += (_, e) =>
			{
				if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && grid.Columns[e.ColumnIndex].DataPropertyName is "a_item_idx")
					PickItemForSelectedRow();
			};

			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_index", HeaderText = "Row ID", ReadOnly = true });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_job",
				HeaderText = "Job",
				DataSource = Jobs.Select(j => new { j.Id, j.Name }).ToList(),
				ValueMember = "Id",
				DisplayMember = "Name",
				Width = 120
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_order", HeaderText = "Order" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_item_idx", HeaderText = "Item ID" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "Item name", ReadOnly = true, Width = 220 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_plus", HeaderText = "Plus" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_wear_pos", HeaderText = "Wear pos (-1 inventory)" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_tab_idx", HeaderText = "Tab" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_row_idx", HeaderText = "Row" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_col_idx", HeaderText = "Column" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_flag", HeaderText = "Flag" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_count", HeaderText = "Count" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_used", HeaderText = "Used (-1 default)" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_used2", HeaderText = "Used2" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_note", HeaderText = "Note", Width = 260 });
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading starting item table...");

			if (!await Task.Run(EnsureTable))
			{
				SetBusy(false, "Could not create or verify t_start_item. Check Logs.log for the MySQL error.");
				return;
			}

			rows = await Task.Run(LoadRows);
			grid.DataSource = rows;
			SetBusy(false, $"Loaded {rows?.Rows.Count ?? 0} starting item rows. The rebuilt GameServer reads this table when a character is created.");
		}

		private bool EnsureTable()
		{
			string db = pMain.pSettings.DBUser;
			string query =
				$"CREATE TABLE IF NOT EXISTS {db}.{TableName} (" +
				"a_index int(11) NOT NULL AUTO_INCREMENT, " +
				"a_enable tinyint(1) NOT NULL DEFAULT 1, " +
				"a_job int(11) NOT NULL DEFAULT -1, " +
				"a_order int(11) NOT NULL DEFAULT 0, " +
				"a_tab_idx int(11) NOT NULL DEFAULT 0, " +
				"a_row_idx int(11) NOT NULL DEFAULT 0, " +
				"a_col_idx int(11) NOT NULL DEFAULT 0, " +
				"a_item_idx int(11) NOT NULL DEFAULT 0, " +
				"a_plus int(11) NOT NULL DEFAULT 0, " +
				"a_wear_pos int(11) NOT NULL DEFAULT -1, " +
				"a_flag int(11) NOT NULL DEFAULT 0, " +
				"a_count bigint(20) NOT NULL DEFAULT 1, " +
				"a_used int(11) NOT NULL DEFAULT -1, " +
				"a_used2 int(11) NOT NULL DEFAULT 0, " +
				"a_note varchar(255) NOT NULL DEFAULT '', " +
				"PRIMARY KEY (a_index), " +
				"KEY idx_start_item_job (a_job, a_order)" +
				") ENGINE=InnoDB DEFAULT CHARSET=latin1;";
			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false);
		}

		private DataTable? LoadRows()
		{
			string userDb = pMain.pSettings.DBUser;
			string dataDb = pMain.pSettings.DBData;
			string locale = pMain.pSettings.WorkLocale;
			string itemName = $"COALESCE(NULLIF(i.a_name_{locale}, ''), NULLIF(i.a_name, ''), NULLIF(i.a_name_usa, ''), CONCAT('Item ', s.a_item_idx))";
			string query =
				"SELECT s.a_index, s.a_enable, s.a_job, s.a_order, s.a_tab_idx, s.a_row_idx, s.a_col_idx, " +
				"s.a_item_idx, " +
				$"{itemName} AS ItemName, " +
				"s.a_plus, s.a_wear_pos, s.a_flag, s.a_count, s.a_used, s.a_used2, s.a_note " +
				$"FROM {userDb}.{TableName} s " +
				$"LEFT JOIN {dataDb}.t_item i ON i.a_index=s.a_item_idx " +
				"ORDER BY s.a_job, s.a_order, s.a_tab_idx, s.a_row_idx, s.a_col_idx, s.a_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private void AddRow()
		{
			if (rows == null)
				return;

			DataRow row = rows.NewRow();
			row["a_index"] = 0;
			row["a_enable"] = 1;
			row["a_job"] = 0;
			row["a_order"] = NextOrderForJob(0);
			row["a_tab_idx"] = 0;
			row["a_row_idx"] = 0;
			row["a_col_idx"] = 0;
			row["a_item_idx"] = 0;
			row["ItemName"] = "";
			row["a_plus"] = 0;
			row["a_wear_pos"] = -1;
			row["a_flag"] = 0;
			row["a_count"] = 1;
			row["a_used"] = -1;
			row["a_used2"] = 0;
			row["a_note"] = "";
			rows.Rows.Add(row);
			SetBusy(false, "Added a new starting item row. Pick an item, then save.");
		}

		private void DuplicateSelectedRow()
		{
			if (rows == null || grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			DataRow row = rows.NewRow();
			foreach (DataColumn column in rows.Columns)
				row[column.ColumnName] = view.Row[column.ColumnName];
			row["a_index"] = 0;
			row["a_order"] = NextOrderForJob(GetInt(view.Row, "a_job"));
			rows.Rows.Add(row);
			SetBusy(false, "Duplicated selected row. Save to create it in the database.");
		}

		private int NextOrderForJob(int job)
		{
			int next = 0;
			if (rows == null)
				return next;

			foreach (DataRow row in rows.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				if (GetInt(row, "a_job") != job)
					continue;
				next = Math.Max(next, GetInt(row, "a_order") + 10);
			}
			return next;
		}

		private void PickItemForSelectedRow()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			int currentItem = GetInt(view.Row, "a_item_idx");
			using ItemPicker picker = new(pMain, this, currentItem, true);
			if (picker.ShowDialog(this) != DialogResult.OK)
				return;

			int pickedItem = Convert.ToInt32(picker.ReturnValues[0]);
			view.Row["a_item_idx"] = pickedItem;
			view.Row["ItemName"] = picker.ReturnValues[1]?.ToString() ?? "";
			SetBusy(false, $"Selected item {pickedItem}. Save to update the database.");
		}

		private async Task SeedDefaultsAsync()
		{
			DialogResult result = MessageBox.Show(
				"This replaces t_start_item with the current hardcoded starter gear and box 760 defaults. Continue?",
				"Seed starting item defaults",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);
			if (result != DialogResult.Yes)
				return;

			SetBusy(true, "Seeding current hardcoded defaults...");
			bool ok = await Task.Run(SeedDefaultRows);
			await LoadEditorAsync();
			SetBusy(false, ok ? "Seeded current defaults. Edit and save; new characters use the saved rows immediately." : "Seed failed. Check Logs.log for the MySQL error.");
		}

		private bool SeedDefaultRows()
		{
			string db = pMain.pSettings.DBUser;
			string query = $"DELETE FROM {db}.{TableName};\n" + BuildDefaultInsertQuery(db);
			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false);
		}

		private static string BuildDefaultInsertQuery(string db)
		{
			List<string> values = [];
			void Add(int job, int order, int item, int wear, int count = 1, int row = 0, int col = 0, string note = "")
			{
				values.Add($"(1,{job},{order},0,{row},{col},{item},0,{wear},0,{count},-1,0,'{note}')");
			}

			AddJob(values, 0, [72, 2, 12, 3, 4, 8], [0, 1, 2, 3, 5, 6], "Titan");
			AddJob(values, 1, [75, 34, 48, 38, 49, 39, 41], [0, 1, 2, 3, 4, 5, 6], "Knight");
			AddJob(values, 2, [78, 26, 50, 28, 30, 32], [0, 1, 2, 3, 5, 6], "Healer");
			AddJob(values, 3, [24, 266, 356, 18, 22, 20], [0, 1, 2, 3, 5, 6], "Mage");
			AddJob(values, 4, [552, 524, 528, 525, 527, 526], [0, 1, 2, 3, 5, 6], "Rogue");
			AddJob(values, 5, [1040, 1000, 988, 1010, 1020, 1030], [0, 1, 2, 3, 5, 6], "Sorcerer");
			AddJob(values, 6, [4539, 4487, 4474, 4500, 4513, 4526, 4552], [0, 1, 2, 3, 5, 6, 11], "NightShadow");

			for (int job = 0; job <= 5; job++)
			{
				Add(job, 900, 5958, -1, 5, 4, 0, "Starter supply");
				Add(job, 910, 2658, -1, 5, 4, 1, "Starter supply");
				Add(job, 920, 2659, -1, 5, 4, 2, "Starter supply");
				Add(job, 930, 6085, -1, 1, 4, 3, "Starter supply");
				Add(job, 1000, 760, -1, 1, 3, 0, "Treasure box chain starter");
			}

			return
				$"INSERT INTO {db}.{TableName} " +
				"(a_enable,a_job,a_order,a_tab_idx,a_row_idx,a_col_idx,a_item_idx,a_plus,a_wear_pos,a_flag,a_count,a_used,a_used2,a_note) VALUES " +
				string.Join(",", values) + ";";
		}

		private static void AddJob(List<string> values, int job, int[] items, int[] wearPositions, string note)
		{
			for (int i = 0; i < items.Length; i++)
				values.Add($"(1,{job},{i * 10},0,0,0,{items[i]},0,{wearPositions[i]},0,1,-1,0,'{note} starter gear')");
		}

		private async Task DeleteSelectedRowAsync()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			int id = GetInt(view.Row, "a_index");
			if (id <= 0)
			{
				rows?.Rows.Remove(view.Row);
				SetBusy(false, "Removed unsaved row.");
				return;
			}

			DialogResult result = MessageBox.Show($"Delete starting item row {id}?", "Delete row", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (result != DialogResult.Yes)
				return;

			SetBusy(true, $"Deleting row {id}...");
			string query = $"DELETE FROM {pMain.pSettings.DBUser}.{TableName} WHERE a_index={id};";
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			await LoadEditorAsync();
			SetBusy(false, ok ? $"Deleted row {id}." : "Delete failed. Check Logs.log for the MySQL error.");
		}

		private async Task SaveAsync()
		{
			if (rows == null)
				return;

			Validate();
			grid.EndEdit();
			SetBusy(true, "Saving starting item rows...");

			bool ok = true;
			foreach (DataRow row in rows.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;

				Normalize(row);
				string query = BuildSaveQuery(row);
				ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
				if (!ok)
					break;
			}

			await LoadEditorAsync();
			SetBusy(false, ok ? "Saved starting items. New characters use these rows immediately." : "Save failed. Check Logs.log for the MySQL error.");
		}

		private string BuildSaveQuery(DataRow row)
		{
			int id = GetInt(row, "a_index");
			string idColumn = id > 0 ? "a_index," : "";
			string idValue = id > 0 ? $"{id}," : "";
			string query =
				$"INSERT INTO {pMain.pSettings.DBUser}.{TableName} " +
				$"({idColumn}a_enable,a_job,a_order,a_tab_idx,a_row_idx,a_col_idx,a_item_idx,a_plus,a_wear_pos,a_flag,a_count,a_used,a_used2,a_note) VALUES " +
				$"({idValue}{GetBoolInt(row, "a_enable")},{GetInt(row, "a_job")},{GetInt(row, "a_order")},{GetInt(row, "a_tab_idx")},{GetInt(row, "a_row_idx")},{GetInt(row, "a_col_idx")}," +
				$"{GetInt(row, "a_item_idx")},{GetInt(row, "a_plus")},{GetInt(row, "a_wear_pos")},{GetInt(row, "a_flag")},{GetLong(row, "a_count")}," +
				$"{GetInt(row, "a_used")},{GetInt(row, "a_used2")},{SqlString(GetString(row, "a_note"))}) " +
				"ON DUPLICATE KEY UPDATE " +
				"a_enable=VALUES(a_enable), a_job=VALUES(a_job), a_order=VALUES(a_order), a_tab_idx=VALUES(a_tab_idx), " +
				"a_row_idx=VALUES(a_row_idx), a_col_idx=VALUES(a_col_idx), a_item_idx=VALUES(a_item_idx), a_plus=VALUES(a_plus), " +
				"a_wear_pos=VALUES(a_wear_pos), a_flag=VALUES(a_flag), a_count=VALUES(a_count), a_used=VALUES(a_used), " +
				"a_used2=VALUES(a_used2), a_note=VALUES(a_note);";
			return query;
		}

		private static void Normalize(DataRow row)
		{
			row["a_enable"] = GetBoolInt(row, "a_enable");
			row["a_order"] = Math.Max(0, GetInt(row, "a_order"));
			row["a_tab_idx"] = Math.Max(0, GetInt(row, "a_tab_idx"));
			row["a_row_idx"] = Math.Max(0, GetInt(row, "a_row_idx"));
			row["a_col_idx"] = Math.Clamp(GetInt(row, "a_col_idx"), 0, 4);
			row["a_item_idx"] = Math.Max(0, GetInt(row, "a_item_idx"));
			row["a_count"] = Math.Max(1, GetLong(row, "a_count"));
			row["a_used2"] = Math.Max(0, GetInt(row, "a_used2"));
		}

		private static int GetInt(DataRow row, string column, int defaultValue = 0)
		{
			object value = row[column];
			return value == DBNull.Value || !int.TryParse(value.ToString(), out int parsed) ? defaultValue : parsed;
		}

		private static long GetLong(DataRow row, string column, long defaultValue = 0)
		{
			object value = row[column];
			return value == DBNull.Value || !long.TryParse(value.ToString(), out long parsed) ? defaultValue : parsed;
		}

		private static string GetString(DataRow row, string column)
		{
			object value = row[column];
			return value == DBNull.Value ? "" : value.ToString() ?? "";
		}

		private static int GetBoolInt(DataRow row, string column)
		{
			object value = row[column];
			if (value is bool b)
				return b ? 1 : 0;
			return int.TryParse(value?.ToString(), out int parsed) && parsed != 0 ? 1 : 0;
		}

		private static string SqlString(string value)
		{
			return "'" + value.Replace("\\", "\\\\").Replace("'", "''") + "'";
		}

		private void SetBusy(bool busy, string message)
		{
			btnReload.Enabled = !busy;
			btnAdd.Enabled = !busy;
			btnDuplicate.Enabled = !busy;
			btnPickItem.Enabled = !busy;
			btnSeed.Enabled = !busy;
			btnDelete.Enabled = !busy;
			btnSave.Enabled = !busy;
			lblStatus.Text = message;
		}
	}
}
