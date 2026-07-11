namespace LastChaos_ToolBoxNG
{
	public class MercenaryEditor : Form
	{
		private const string TableName = "t_custom_mercenary";

		private readonly Main pMain;
		private readonly DataGridView grid = new();
		private readonly Button btnReload = new();
		private readonly Button btnAdd = new();
		private readonly Button btnDuplicate = new();
		private readonly Button btnPickItem = new();
		private readonly Button btnPickNpc = new();
		private readonly Button btnConfigureItem = new();
		private readonly Button btnDelete = new();
		private readonly Button btnSave = new();
		private readonly Label lblInfo = new();
		private readonly Label lblStatus = new();

		private DataTable? rows;

		public MercenaryEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "MercenaryEditor";
			Text = "Mercenary Summon Editor";
			MinimumSize = new Size(1280, 720);
			Size = new Size(1580, 840);
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
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			lblInfo.Dock = DockStyle.Fill;
			lblInfo.TextAlign = ContentAlignment.MiddleLeft;
			lblInfo.Text = "Creates deterministic monster mercenary summon definitions. The selected item must still be a usable Monster Mercenary Card item: Type 4, SubType 19. Existing random cards are not changed.";
			root.Controls.Add(lblInfo, 0, 0);

			FlowLayoutPanel toolbar = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
			root.Controls.Add(toolbar, 0, 1);

			ConfigureButton(btnReload, "Reload", async (_, _) => await LoadEditorAsync());
			ConfigureButton(btnAdd, "Add row", (_, _) => AddRow());
			ConfigureButton(btnDuplicate, "Duplicate row", (_, _) => DuplicateSelectedRow());
			ConfigureButton(btnPickItem, "Pick item", (_, _) => PickItemForSelectedRow());
			ConfigureButton(btnPickNpc, "Pick NPC", (_, _) => PickNpcForSelectedRow());
			ConfigureButton(btnConfigureItem, "Set item as summon card", async (_, _) => await ConfigureSelectedItemAsync());
			toolbar.Controls.AddRange([btnReload, btnAdd, btnDuplicate, btnPickItem, btnPickNpc, btnConfigureItem]);

			ConfigureGrid();
			root.Controls.Add(grid, 0, 2);

			FlowLayoutPanel bottom = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
			root.Controls.Add(bottom, 0, 3);

			ConfigureButton(btnDelete, "Delete selected row", async (_, _) => await DeleteSelectedRowAsync());
			ConfigureButton(btnSave, "Save mercenaries", async (_, _) => await SaveAsync());
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
			grid.CurrentCellDirtyStateChanged += (_, _) =>
			{
				if (grid.IsCurrentCellDirty)
					grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
			};
			grid.CellDoubleClick += (_, e) =>
			{
				if (e.RowIndex < 0 || e.ColumnIndex < 0)
					return;
				string property = grid.Columns[e.ColumnIndex].DataPropertyName;
				if (property == "a_item_index")
					PickItemForSelectedRow();
				else if (property == "a_npc_index")
					PickNpcForSelectedRow();
			};

			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_item_index", HeaderText = "Item ID" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "Item name", ReadOnly = true, Width = 230 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemUseStatus", HeaderText = "Use setup", ReadOnly = true, Width = 260 });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_name", HeaderText = "Editor name", Width = 180 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_npc_index", HeaderText = "NPC ID" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NpcName", HeaderText = "NPC name", ReadOnly = true, Width = 210 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_summon_skill_index", HeaderText = "Summon skill" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_summon_skill_level", HeaderText = "Skill level" });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_permanent", HeaderText = "Permanent", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_duration_hours", HeaderText = "Duration hours" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_hpmp_per_level", HeaderText = "HP/MP per lvl" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_attack_per_level", HeaderText = "Melee/Ranged per lvl" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_magic_per_level", HeaderText = "Magic per lvl" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_defense_per_level", HeaderText = "Defense per lvl" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_resist_per_level", HeaderText = "M.Def per lvl" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_hit_per_level", HeaderText = "Hit per lvl" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_dodge_per_level", HeaderText = "Evasion per lvl" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_bonus_attack", HeaderText = "Flat attack" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_bonus_defense", HeaderText = "Flat defense" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_walk_speed", HeaderText = "Walk speed" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_run_speed", HeaderText = "Run speed" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_size_percent", HeaderText = "Size %" });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_auto_target", HeaderText = "Auto target", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_use_owner_target", HeaderText = "Use owner target", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_use_skills", HeaderText = "Use skills", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_use_normal_attack", HeaderText = "Normal attack", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_owner_range", HeaderText = "Owner search range" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_mercenary_range", HeaderText = "Merc search range" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_retarget_seconds", HeaderText = "Retarget sec" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_follow_distance", HeaderText = "Follow dist" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_leash_distance", HeaderText = "Leash dist" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_target_break_distance", HeaderText = "Break target dist" });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_allow_pvp", HeaderText = "Allow PvP target", TrueValue = 1, FalseValue = 0 });
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading custom mercenaries...");
			if (!await Task.Run(EnsureTable))
			{
				SetBusy(false, "Could not create or verify t_custom_mercenary. Check Logs.log for the MySQL error.");
				return;
			}

			rows = await Task.Run(LoadRows);
			grid.DataSource = rows;
			SetBusy(false, $"Loaded {rows?.Rows.Count ?? 0} custom mercenary rows. Rebuild/restart GameServer after edits.");
		}

		private bool EnsureTable()
		{
			string db = pMain.pSettings.DBData;
			string query =
				$"CREATE TABLE IF NOT EXISTS {db}.{TableName} (" +
				"a_item_index int(11) NOT NULL, " +
				"a_enable tinyint(1) NOT NULL DEFAULT 1, " +
				"a_name varchar(64) NOT NULL DEFAULT '', " +
				"a_npc_index int(11) NOT NULL DEFAULT -1, " +
				"a_summon_skill_index int(11) NOT NULL DEFAULT 1134, " +
				"a_summon_skill_level int(11) NOT NULL DEFAULT 1, " +
				"a_permanent tinyint(1) NOT NULL DEFAULT 1, " +
				"a_duration_hours int(11) NOT NULL DEFAULT 0, " +
				"a_hpmp_per_level int(11) NOT NULL DEFAULT 100, " +
				"a_attack_per_level int(11) NOT NULL DEFAULT 55, " +
				"a_magic_per_level int(11) NOT NULL DEFAULT 55, " +
				"a_defense_per_level int(11) NOT NULL DEFAULT 110, " +
				"a_resist_per_level int(11) NOT NULL DEFAULT 1, " +
				"a_hit_per_level int(11) NOT NULL DEFAULT 5, " +
				"a_dodge_per_level int(11) NOT NULL DEFAULT 1, " +
				"a_bonus_attack int(11) NOT NULL DEFAULT 0, " +
				"a_bonus_defense int(11) NOT NULL DEFAULT 0, " +
				"a_walk_speed int(11) NOT NULL DEFAULT 8, " +
				"a_run_speed int(11) NOT NULL DEFAULT 8, " +
				"a_size_percent int(11) NOT NULL DEFAULT 100, " +
				"a_auto_target tinyint(1) NOT NULL DEFAULT 1, " +
				"a_use_owner_target tinyint(1) NOT NULL DEFAULT 1, " +
				"a_use_skills tinyint(1) NOT NULL DEFAULT 1, " +
				"a_use_normal_attack tinyint(1) NOT NULL DEFAULT 1, " +
				"a_owner_range int(11) NOT NULL DEFAULT 12, " +
				"a_mercenary_range int(11) NOT NULL DEFAULT 20, " +
				"a_retarget_seconds int(11) NOT NULL DEFAULT 1, " +
				"a_follow_distance int(11) NOT NULL DEFAULT 3, " +
				"a_leash_distance int(11) NOT NULL DEFAULT 50, " +
				"a_target_break_distance int(11) NOT NULL DEFAULT 40, " +
				"a_allow_pvp tinyint(1) NOT NULL DEFAULT 0, " +
				"PRIMARY KEY (a_item_index)" +
				") ENGINE=InnoDB DEFAULT CHARSET=latin1;";
			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false);
		}

		private DataTable? LoadRows()
		{
			string dataDb = pMain.pSettings.DBData;
			string locale = pMain.pSettings.WorkLocale;
			string itemName = $"COALESCE(NULLIF(i.a_name_{locale}, ''), NULLIF(i.a_name, ''), NULLIF(i.a_name_usa, ''), CONCAT('Item ', m.a_item_index))";
			string npcName = $"COALESCE(NULLIF(n.a_name_{locale}, ''), NULLIF(n.a_name, ''), NULLIF(n.a_name_usa, ''), CONCAT('NPC ', m.a_npc_index))";
			string query =
				"SELECT m.*, " +
				$"{itemName} AS ItemName, " +
				$"{npcName} AS NpcName, " +
				"CASE WHEN i.a_type_idx=4 AND i.a_subtype_idx=19 THEN 'OK: Monster Mercenary Card' " +
				"ELSE CONCAT('Needs Type 4 / SubType 19 (current ', COALESCE(i.a_type_idx, -1), '/', COALESCE(i.a_subtype_idx, -1), ')') END AS ItemUseStatus " +
				$"FROM {dataDb}.{TableName} m " +
				$"LEFT JOIN {dataDb}.t_item i ON i.a_index=m.a_item_index " +
				$"LEFT JOIN {dataDb}.t_npc n ON n.a_index=m.a_npc_index " +
				"ORDER BY m.a_item_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private void AddRow()
		{
			if (rows == null)
				return;

			DataRow row = rows.NewRow();
			row["a_item_index"] = 0;
			row["a_enable"] = 1;
			row["a_name"] = "";
			row["a_npc_index"] = -1;
			row["a_summon_skill_index"] = 1134;
			row["a_summon_skill_level"] = 1;
			row["a_permanent"] = 1;
			row["a_duration_hours"] = 0;
			row["a_hpmp_per_level"] = 100;
			row["a_attack_per_level"] = 55;
			row["a_magic_per_level"] = 55;
			row["a_defense_per_level"] = 110;
			row["a_resist_per_level"] = 1;
			row["a_hit_per_level"] = 5;
			row["a_dodge_per_level"] = 1;
			row["a_bonus_attack"] = 0;
			row["a_bonus_defense"] = 0;
			row["a_walk_speed"] = 8;
			row["a_run_speed"] = 8;
			row["a_size_percent"] = 100;
			row["a_auto_target"] = 1;
			row["a_use_owner_target"] = 1;
			row["a_use_skills"] = 1;
			row["a_use_normal_attack"] = 1;
			row["a_owner_range"] = 12;
			row["a_mercenary_range"] = 20;
			row["a_retarget_seconds"] = 1;
			row["a_follow_distance"] = 3;
			row["a_leash_distance"] = 50;
			row["a_target_break_distance"] = 40;
			row["a_allow_pvp"] = 0;
			row["ItemName"] = "";
			row["NpcName"] = "";
			row["ItemUseStatus"] = "Pick an item";
			rows.Rows.Add(row);
			SetBusy(false, "Added a custom mercenary row. Pick an item and NPC, then save.");
		}

		private void DuplicateSelectedRow()
		{
			if (rows == null || grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			DataRow row = rows.NewRow();
			foreach (DataColumn column in rows.Columns)
				row[column.ColumnName] = view.Row[column.ColumnName];
			row["a_item_index"] = 0;
			row["ItemName"] = "";
			row["ItemUseStatus"] = "Pick a new item";
			rows.Rows.Add(row);
			SetBusy(false, "Duplicated selected row. Pick a new item ID before saving.");
		}

		private void PickItemForSelectedRow()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			using ItemPicker picker = new(pMain, this, GetInt(view.Row, "a_item_index"), true);
			if (picker.ShowDialog(this) != DialogResult.OK)
				return;

			int itemId = Convert.ToInt32(picker.ReturnValues[0]);
			view.Row["a_item_index"] = itemId;
			view.Row["ItemName"] = picker.ReturnValues[1]?.ToString() ?? "";
			view.Row["ItemUseStatus"] = "Save and reload to verify Type/SubType";
			SetBusy(false, $"Selected item {itemId}. It must be Type 4 / SubType 19 to be usable.");
		}

		private void PickNpcForSelectedRow()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			using NPCPicker picker = new(pMain, this, GetInt(view.Row, "a_npc_index"), false);
			if (picker.ShowDialog(this) != DialogResult.OK)
				return;

			int npcId = Convert.ToInt32(picker.ReturnValues[0]);
			view.Row["a_npc_index"] = npcId;
			view.Row["NpcName"] = picker.ReturnValues[1]?.ToString() ?? "";
			SetBusy(false, $"Selected NPC {npcId}.");
		}

		private async Task ConfigureSelectedItemAsync()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			int itemId = GetInt(view.Row, "a_item_index");
			if (itemId <= 0)
			{
				MessageBox.Show("Pick a valid item first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			DialogResult result = MessageBox.Show(
				"This updates the selected item row in t_item to Type 4 / SubType 19, enables it, and writes safe summon-card defaults. Export item LODs afterward if the running client does not already have those item fields.\n\nContinue?",
				Text,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);
			if (result != DialogResult.Yes)
				return;

			int durationHours = GetBoolInt(view.Row, "a_permanent") != 0 ? 0 : Math.Max(1, GetInt(view.Row, "a_duration_hours", 1));
			string query =
				$"UPDATE {pMain.pSettings.DBData}.t_item SET " +
				"a_enable=1, a_type_idx=4, a_subtype_idx=19, " +
				$"a_num_0={Math.Max(1, GetInt(view.Row, "a_summon_skill_index", 1134))}, " +
				$"a_num_1={Math.Max(1, GetInt(view.Row, "a_summon_skill_level", 1))}, " +
				$"a_num_2={durationHours}, " +
				"a_weight=CASE WHEN a_weight<=0 THEN 1 ELSE a_weight END, " +
				"a_level2=CASE WHEN a_level2<=0 THEN 999 ELSE a_level2 END, " +
				"a_job_flag=CASE WHEN a_job_flag=0 THEN 511 ELSE a_job_flag END, " +
				"a_zone_flag=CASE WHEN a_zone_flag=0 THEN 1023 ELSE a_zone_flag END, " +
				"a_name=CASE WHEN a_name='' THEN a_name_usa ELSE a_name END, " +
				"a_descr=CASE WHEN a_descr='' THEN a_descr_usa ELSE a_descr END, " +
				"a_file_smc=CASE WHEN a_file_smc='' THEN 'Item\\\\Common\\\\ITEM_treasure02.smc' ELSE a_file_smc END " +
				$"WHERE a_index={itemId};";
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			bool exported = ok && await TryExportItemClientLodsAsync(itemId);
			await LoadEditorAsync();
			SetBusy(false, ok
				? exported
					? $"Configured item {itemId} as a mercenary summon card and exported item LODs."
					: $"Configured item {itemId} as a mercenary summon card, but item LOD export failed. Check Logs.log."
				: "Item configuration failed. Check Logs.log for the MySQL error.");
		}

		private async Task<bool> TryExportItemClientLodsAsync(int itemId)
		{
			bool success = true;

			using Exporter exporter = new(pMain);

			try
			{
				await exporter.ExportItemsLodAsync(true);
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Mercenary Summon Editor > Item: {itemId} configured, but ItemAll.lod export failed: {ex.Message}");
				success = false;
			}

			try
			{
				await exporter.ExportItemStringsLodAsync(true);
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Mercenary Summon Editor > Item: {itemId} configured, but strItem .lod export failed: {ex.Message}");
				success = false;
			}

			return success;
		}

		private async Task DeleteSelectedRowAsync()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			int itemId = GetInt(view.Row, "a_item_index");
			if (itemId <= 0)
			{
				rows?.Rows.Remove(view.Row);
				SetBusy(false, "Removed unsaved row.");
				return;
			}

			if (MessageBox.Show($"Delete custom mercenary row for item {itemId}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			string query = $"DELETE FROM {pMain.pSettings.DBData}.{TableName} WHERE a_item_index={itemId};";
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			await LoadEditorAsync();
			SetBusy(false, ok ? $"Deleted custom mercenary row for item {itemId}." : "Delete failed. Check Logs.log for the MySQL error.");
		}

		private async Task SaveAsync()
		{
			if (rows == null)
				return;

			Validate();
			grid.EndEdit();
			SetBusy(true, "Saving custom mercenaries...");

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
			SetBusy(false, ok ? "Saved custom mercenaries. Restart GameServer before testing changed behavior." : "Save failed. Check Logs.log for the MySQL error.");
		}

		private string BuildSaveQuery(DataRow row)
		{
			string columns =
				"a_item_index,a_enable,a_name,a_npc_index,a_summon_skill_index,a_summon_skill_level,a_permanent,a_duration_hours," +
				"a_hpmp_per_level,a_attack_per_level,a_magic_per_level,a_defense_per_level,a_resist_per_level,a_hit_per_level,a_dodge_per_level," +
				"a_bonus_attack,a_bonus_defense,a_walk_speed,a_run_speed,a_size_percent,a_auto_target,a_use_owner_target,a_use_skills,a_use_normal_attack," +
				"a_owner_range,a_mercenary_range,a_retarget_seconds,a_follow_distance,a_leash_distance,a_target_break_distance,a_allow_pvp";
			string values =
				$"{GetInt(row, "a_item_index")},{GetBoolInt(row, "a_enable")},{SqlString(GetString(row, "a_name"))},{GetInt(row, "a_npc_index")}," +
				$"{GetInt(row, "a_summon_skill_index", 1134)},{GetInt(row, "a_summon_skill_level", 1)},{GetBoolInt(row, "a_permanent")},{GetInt(row, "a_duration_hours")}," +
				$"{GetInt(row, "a_hpmp_per_level")},{GetInt(row, "a_attack_per_level")},{GetInt(row, "a_magic_per_level")},{GetInt(row, "a_defense_per_level")}," +
				$"{GetInt(row, "a_resist_per_level")},{GetInt(row, "a_hit_per_level")},{GetInt(row, "a_dodge_per_level")},{GetInt(row, "a_bonus_attack")}," +
				$"{GetInt(row, "a_bonus_defense")},{GetInt(row, "a_walk_speed")},{GetInt(row, "a_run_speed")},{GetInt(row, "a_size_percent")}," +
				$"{GetBoolInt(row, "a_auto_target")},{GetBoolInt(row, "a_use_owner_target")},{GetBoolInt(row, "a_use_skills")},{GetBoolInt(row, "a_use_normal_attack")}," +
				$"{GetInt(row, "a_owner_range")},{GetInt(row, "a_mercenary_range")},{GetInt(row, "a_retarget_seconds")},{GetInt(row, "a_follow_distance")}," +
				$"{GetInt(row, "a_leash_distance")},{GetInt(row, "a_target_break_distance")},{GetBoolInt(row, "a_allow_pvp")}";

			return
				$"INSERT INTO {pMain.pSettings.DBData}.{TableName} ({columns}) VALUES ({values}) " +
				"ON DUPLICATE KEY UPDATE " +
				"a_enable=VALUES(a_enable), a_name=VALUES(a_name), a_npc_index=VALUES(a_npc_index), " +
				"a_summon_skill_index=VALUES(a_summon_skill_index), a_summon_skill_level=VALUES(a_summon_skill_level), " +
				"a_permanent=VALUES(a_permanent), a_duration_hours=VALUES(a_duration_hours), " +
				"a_hpmp_per_level=VALUES(a_hpmp_per_level), a_attack_per_level=VALUES(a_attack_per_level), a_magic_per_level=VALUES(a_magic_per_level), " +
				"a_defense_per_level=VALUES(a_defense_per_level), a_resist_per_level=VALUES(a_resist_per_level), a_hit_per_level=VALUES(a_hit_per_level), " +
				"a_dodge_per_level=VALUES(a_dodge_per_level), a_bonus_attack=VALUES(a_bonus_attack), a_bonus_defense=VALUES(a_bonus_defense), " +
				"a_walk_speed=VALUES(a_walk_speed), a_run_speed=VALUES(a_run_speed), a_size_percent=VALUES(a_size_percent), " +
				"a_auto_target=VALUES(a_auto_target), a_use_owner_target=VALUES(a_use_owner_target), a_use_skills=VALUES(a_use_skills), " +
				"a_use_normal_attack=VALUES(a_use_normal_attack), a_owner_range=VALUES(a_owner_range), a_mercenary_range=VALUES(a_mercenary_range), " +
				"a_retarget_seconds=VALUES(a_retarget_seconds), a_follow_distance=VALUES(a_follow_distance), a_leash_distance=VALUES(a_leash_distance), " +
				"a_target_break_distance=VALUES(a_target_break_distance), a_allow_pvp=VALUES(a_allow_pvp);";
		}

		private static void Normalize(DataRow row)
		{
			row["a_item_index"] = Math.Max(0, GetInt(row, "a_item_index"));
			row["a_enable"] = GetBoolInt(row, "a_enable");
			row["a_npc_index"] = GetInt(row, "a_npc_index", -1);
			row["a_summon_skill_index"] = Math.Max(1, GetInt(row, "a_summon_skill_index", 1134));
			row["a_summon_skill_level"] = Math.Max(1, GetInt(row, "a_summon_skill_level", 1));
			row["a_permanent"] = GetBoolInt(row, "a_permanent");
			row["a_duration_hours"] = Math.Max(0, GetInt(row, "a_duration_hours"));
			row["a_hpmp_per_level"] = Math.Max(1, GetInt(row, "a_hpmp_per_level", 100));
			row["a_attack_per_level"] = Math.Max(0, GetInt(row, "a_attack_per_level", 55));
			row["a_magic_per_level"] = Math.Max(0, GetInt(row, "a_magic_per_level", 55));
			row["a_defense_per_level"] = Math.Max(0, GetInt(row, "a_defense_per_level", 110));
			row["a_resist_per_level"] = Math.Max(0, GetInt(row, "a_resist_per_level", 1));
			row["a_hit_per_level"] = Math.Max(0, GetInt(row, "a_hit_per_level", 5));
			row["a_dodge_per_level"] = Math.Max(0, GetInt(row, "a_dodge_per_level", 1));
			row["a_walk_speed"] = Math.Max(1, GetInt(row, "a_walk_speed", 8));
			row["a_run_speed"] = Math.Max(1, GetInt(row, "a_run_speed", 8));
			row["a_size_percent"] = Math.Clamp(GetInt(row, "a_size_percent", 100), 1, 500);
			row["a_auto_target"] = GetBoolInt(row, "a_auto_target");
			row["a_use_owner_target"] = GetBoolInt(row, "a_use_owner_target");
			row["a_use_skills"] = GetBoolInt(row, "a_use_skills");
			row["a_use_normal_attack"] = GetBoolInt(row, "a_use_normal_attack");
			row["a_owner_range"] = Math.Max(1, GetInt(row, "a_owner_range", 12));
			row["a_mercenary_range"] = Math.Max(1, GetInt(row, "a_mercenary_range", 20));
			row["a_retarget_seconds"] = Math.Max(1, GetInt(row, "a_retarget_seconds", 1));
			row["a_follow_distance"] = Math.Max(1, GetInt(row, "a_follow_distance", 3));
			row["a_leash_distance"] = Math.Max(1, GetInt(row, "a_leash_distance", 50));
			row["a_target_break_distance"] = Math.Max(1, GetInt(row, "a_target_break_distance", 40));
			row["a_allow_pvp"] = GetBoolInt(row, "a_allow_pvp");
		}

		private static int GetInt(DataRow row, string column, int defaultValue = 0)
		{
			object value = row[column];
			return value == DBNull.Value || !int.TryParse(value.ToString(), out int parsed) ? defaultValue : parsed;
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
			btnPickNpc.Enabled = !busy;
			btnConfigureItem.Enabled = !busy;
			btnDelete.Enabled = !busy;
			btnSave.Enabled = !busy;
			lblStatus.Text = message;
		}
	}
}
