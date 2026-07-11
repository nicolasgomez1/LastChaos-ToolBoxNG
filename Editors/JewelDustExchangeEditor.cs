namespace LastChaos_ToolBoxNG
{
	public class JewelDustExchangeEditor : Form
	{
		private const string TableName = "t_jewel_dust_exchange";
		private const int DefaultDustItemId = 640100;

		private readonly Main pMain;
		private readonly ComboBox cbJewel = new();
		private readonly Button btnReload = new();
		private readonly Button btnAddSelected = new();
		private readonly Button btnAddMissing = new();
		private readonly Button btnDisable = new();
		private readonly Button btnDelete = new();
		private readonly Button btnSave = new();
		private readonly DataGridView grid = new();
		private readonly Label lblStatus = new();

		private DataTable? exchangeTable;
		private DataTable? jewelTable;

		public JewelDustExchangeEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "JewelDustExchangeEditor";
			Text = "Jewel Dust Exchange Editor";
			MinimumSize = new Size(980, 620);
			Size = new Size(1240, 720);
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
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			root.Controls.Add(new Label
			{
				Dock = DockStyle.Fill,
				Text = "Edits t_jewel_dust_exchange. Enabled rows allow the listed jewel item to be destroyed into Jewel Dust. Dust item should normally stay 640100 because the upgrade trainer consumes that item.",
				TextAlign = ContentAlignment.MiddleLeft
			}, 0, 0);

			FlowLayoutPanel jewelRow = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(jewelRow, 0, 1);

			jewelRow.Controls.Add(new Label
			{
				Text = "Socket jewel:",
				AutoSize = true,
				Margin = new Padding(0, 9, 6, 0)
			});

			cbJewel.DropDownStyle = ComboBoxStyle.DropDownList;
			cbJewel.Width = 460;
			cbJewel.Margin = new Padding(0, 4, 10, 0);
			jewelRow.Controls.Add(cbJewel);

			ConfigureButton(btnReload, "Reload", async (_, _) => await LoadEditorAsync());
			ConfigureButton(btnAddSelected, "Add selected jewel", (_, _) => AddSelectedJewel());
			ConfigureButton(btnAddMissing, "Add all missing socket jewels", (_, _) => AddMissingJewels());
			jewelRow.Controls.AddRange([btnReload, btnAddSelected, btnAddMissing]);

			ConfigureGrid();
			root.Controls.Add(grid, 0, 2);

			FlowLayoutPanel actionRow = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(actionRow, 0, 3);

			ConfigureButton(btnDisable, "Disable selected row", (_, _) => DisableSelectedRow());
			ConfigureButton(btnDelete, "Delete selected row", async (_, _) => await DeleteSelectedRowAsync());
			ConfigureButton(btnSave, "Save exchange table", async (_, _) => await SaveExchangeAsync());
			actionRow.Controls.AddRange([btnDisable, btnDelete, btnSave]);

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

			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_jewel_index", HeaderText = "Jewel item ID" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "JewelName", HeaderText = "Jewel name", ReadOnly = true, Width = 220 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", HeaderText = "Type", ReadOnly = true });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtype", HeaderText = "Subtype", ReadOnly = true });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Tier", HeaderText = "Tier", ReadOnly = true });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_dust_item_index", HeaderText = "Dust item ID" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DustItemName", HeaderText = "Dust item name", ReadOnly = true, Width = 180 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_dust_count", HeaderText = "Dust amount" });
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading Jewel Dust exchange table...");

			bool tableOk = await Task.Run(EnsureExchangeTable);
			if (!tableOk)
			{
				SetBusy(false, "Could not create or verify t_jewel_dust_exchange. Check the Toolbox console.");
				return;
			}

			jewelTable = await Task.Run(LoadSocketJewels);
			exchangeTable = await Task.Run(LoadExchangeRows);

			cbJewel.DataSource = jewelTable;
			cbJewel.DisplayMember = "DisplayName";
			cbJewel.ValueMember = "a_index";

			grid.DataSource = exchangeTable;
			SetBusy(false, $"Loaded {exchangeTable?.Rows.Count ?? 0} exchange rows. Save changes, then test the next jewel use; the server reads this table live.");
		}

		private bool EnsureExchangeTable()
		{
			string db = pMain.pSettings.DBData;
			string query =
				$"CREATE TABLE IF NOT EXISTS {db}.{TableName} (" +
				"a_jewel_index int(11) NOT NULL, " +
				"a_dust_item_index int(11) NOT NULL, " +
				"a_dust_count int(11) NOT NULL, " +
				"a_enable tinyint(1) NOT NULL DEFAULT 1, " +
				"PRIMARY KEY (a_jewel_index)" +
				") ENGINE=InnoDB DEFAULT CHARSET=latin1;";
			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false);
		}

		private DataTable? LoadSocketJewels()
		{
			string db = pMain.pSettings.DBData;
			string nameExpr = "COALESCE(NULLIF(a_name, ''), NULLIF(a_name_usa, ''), CONCAT('Item ', a_index))";
			string query =
				"SELECT " +
				"a_index, " +
				$"{nameExpr} AS ItemName, " +
				"IFNULL(a_num_0, 0) AS Tier, " +
				"IFNULL(a_type_idx, -1) AS Type, " +
				"IFNULL(a_subtype_idx, -1) AS Subtype, " +
				$"CONCAT(a_index, ' - ', {nameExpr}, ' (tier ', IFNULL(a_num_0, 0), ')') AS DisplayName " +
				$"FROM {db}.t_item " +
				"WHERE a_enable=1 AND a_type_idx=4 AND a_subtype_idx IN (16, 22) " +
				"ORDER BY a_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private DataTable? LoadExchangeRows()
		{
			string db = pMain.pSettings.DBData;
			string jewelName = "COALESCE(NULLIF(j.a_name, ''), NULLIF(j.a_name_usa, ''), CONCAT('Item ', e.a_jewel_index))";
			string dustName = "COALESCE(NULLIF(d.a_name, ''), NULLIF(d.a_name_usa, ''), CONCAT('Item ', e.a_dust_item_index))";
			string query =
				"SELECT " +
				"e.a_enable, " +
				"e.a_jewel_index, " +
				$"{jewelName} AS JewelName, " +
				"IFNULL(j.a_type_idx, -1) AS Type, " +
				"IFNULL(j.a_subtype_idx, -1) AS Subtype, " +
				"IFNULL(j.a_num_0, 0) AS Tier, " +
				"e.a_dust_item_index, " +
				$"{dustName} AS DustItemName, " +
				"e.a_dust_count " +
				$"FROM {db}.{TableName} e " +
				$"LEFT JOIN {db}.t_item j ON j.a_index=e.a_jewel_index " +
				$"LEFT JOIN {db}.t_item d ON d.a_index=e.a_dust_item_index " +
				"ORDER BY e.a_jewel_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private void AddSelectedJewel()
		{
			if (exchangeTable == null || cbJewel.SelectedItem is not DataRowView selected)
				return;

			int jewelId = Convert.ToInt32(selected.Row["a_index"]);
			if (FindExchangeRow(jewelId) != null)
			{
				SelectExchangeRow(jewelId);
				SetBusy(false, $"Jewel {jewelId} already exists in the exchange table.");
				return;
			}

			AddJewelRow(selected.Row);
			SetBusy(false, $"Added jewel {jewelId}. Save to update the database.");
		}

		private void AddMissingJewels()
		{
			if (exchangeTable == null || jewelTable == null)
				return;

			int added = 0;
			foreach (DataRow jewel in jewelTable.Rows)
			{
				int jewelId = GetInt(jewel, "a_index");
				if (FindExchangeRow(jewelId) != null)
					continue;

				AddJewelRow(jewel);
				added++;
			}

			SetBusy(false, added == 0 ? "No missing socket jewels found." : $"Added {added} missing socket jewels. Save to update the database.");
		}

		private void AddJewelRow(DataRow jewel)
		{
			if (exchangeTable == null)
				return;

			int tier = Math.Max(1, GetInt(jewel, "Tier", 1));
			DataRow row = exchangeTable.NewRow();
			row["a_enable"] = 1;
			row["a_jewel_index"] = GetInt(jewel, "a_index");
			row["JewelName"] = GetString(jewel, "ItemName");
			row["Type"] = GetInt(jewel, "Type", 4);
			row["Subtype"] = GetInt(jewel, "Subtype", 16);
			row["Tier"] = tier;
			row["a_dust_item_index"] = DefaultDustItemId;
			row["DustItemName"] = "Jewel Dust";
			row["a_dust_count"] = tier;
			exchangeTable.Rows.Add(row);
		}

		private DataRow? FindExchangeRow(int jewelId)
		{
			if (exchangeTable == null)
				return null;

			foreach (DataRow row in exchangeTable.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				if (GetInt(row, "a_jewel_index") == jewelId)
					return row;
			}

			return null;
		}

		private void SelectExchangeRow(int jewelId)
		{
			foreach (DataGridViewRow gridRow in grid.Rows)
			{
				if (gridRow.DataBoundItem is not DataRowView view)
					continue;
				if (GetInt(view.Row, "a_jewel_index") != jewelId)
					continue;

				grid.ClearSelection();
				gridRow.Selected = true;
				grid.CurrentCell = gridRow.Cells[0];
				return;
			}
		}

		private void DisableSelectedRow()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			view.Row["a_enable"] = 0;
			SetBusy(false, "Selected row marked disabled. Save to update the database.");
		}

		private async Task DeleteSelectedRowAsync()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			int jewelId = GetInt(view.Row, "a_jewel_index");
			DialogResult result = MessageBox.Show(
				$"Delete Jewel Dust exchange row for item {jewelId}?",
				"Delete exchange row",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);
			if (result != DialogResult.Yes)
				return;

			if (view.Row.RowState == DataRowState.Added)
			{
				exchangeTable?.Rows.Remove(view.Row);
				SetBusy(false, "Removed unsaved row.");
				return;
			}

			SetBusy(true, $"Deleting exchange row {jewelId}...");
			string query = $"DELETE FROM {pMain.pSettings.DBData}.{TableName} WHERE a_jewel_index={jewelId};";
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			await LoadEditorAsync();
			SetBusy(false, ok ? $"Deleted exchange row {jewelId}." : "Delete failed. Check the Toolbox console.");
		}

		private async Task SaveExchangeAsync()
		{
			if (exchangeTable == null)
				return;

			Validate();
			grid.EndEdit();
			SetBusy(true, "Saving Jewel Dust exchange table...");

			bool ok = true;
			foreach (DataRow row in exchangeTable.Rows)
			{
				NormalizeExchangeRow(row);
				string query = BuildSaveQuery(row);
				ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
				if (!ok)
					break;
			}

			await LoadEditorAsync();
			SetBusy(false, ok ? "Saved Jewel Dust exchange table. The next jewel use should read the new values." : "Save failed. Check the Toolbox console.");
		}

		private string BuildSaveQuery(DataRow row)
		{
			int jewelId = GetInt(row, "a_jewel_index");
			int dustItemId = GetInt(row, "a_dust_item_index", DefaultDustItemId);
			int dustCount = GetInt(row, "a_dust_count");
			int enabled = GetBoolInt(row, "a_enable");

			return
				$"INSERT INTO {pMain.pSettings.DBData}.{TableName} " +
				"(a_jewel_index, a_dust_item_index, a_dust_count, a_enable) " +
				$"VALUES ({jewelId}, {dustItemId}, {dustCount}, {enabled}) " +
				"ON DUPLICATE KEY UPDATE " +
				"a_dust_item_index=VALUES(a_dust_item_index), " +
				"a_dust_count=VALUES(a_dust_count), " +
				"a_enable=VALUES(a_enable);";
		}

		private static void NormalizeExchangeRow(DataRow row)
		{
			row["a_enable"] = GetBoolInt(row, "a_enable");
			row["a_jewel_index"] = Math.Max(1, GetInt(row, "a_jewel_index"));
			row["a_dust_item_index"] = Math.Max(1, GetInt(row, "a_dust_item_index", DefaultDustItemId));
			row["a_dust_count"] = Math.Max(0, GetInt(row, "a_dust_count"));
		}

		private static int GetInt(DataRow row, string column, int defaultValue = 0)
		{
			object value = row[column];
			if (value == DBNull.Value || value == null)
				return defaultValue;
			if (int.TryParse(value.ToString(), out int parsed))
				return parsed;
			return defaultValue;
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
			if (int.TryParse(value?.ToString(), out int parsed))
				return parsed != 0 ? 1 : 0;
			return 0;
		}

		private void SetBusy(bool busy, string message)
		{
			btnReload.Enabled = !busy;
			btnAddSelected.Enabled = !busy;
			btnAddMissing.Enabled = !busy;
			btnDisable.Enabled = !busy;
			btnDelete.Enabled = !busy;
			btnSave.Enabled = !busy;
			lblStatus.Text = message;
		}
	}
}
