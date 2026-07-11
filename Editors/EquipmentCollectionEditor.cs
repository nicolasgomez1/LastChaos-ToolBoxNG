namespace LastChaos_ToolBoxNG
{
	public class EquipmentCollectionEditor : Form
	{
		private const string LayoutTable = "t_equipment_collection_layout";
		private const string GroupTable = "t_equipment_collection_group";
		private const string BonusTable = "t_equipment_collection_bonus";
		private const string ClientDefinitionFile = "EquipmentCollectionGroups.txt";
		private const int CustomGroupStart = 10000;

		private const int GroupKindAuto = 0;
		private const int GroupKindSet = 1;
		private const int GroupKindCustom = 2;

		private readonly Main pMain;
		private readonly DataGridView gridGroups = new();
		private readonly DataGridView gridItems = new();
		private readonly DataGridView gridBonuses = new();
		private readonly Button btnReload = new();
		private readonly Button btnAddCustomGroup = new();
		private readonly Button btnAddBonus = new();
		private readonly Button btnSave = new();
		private readonly Label lblStatus = new();
		private readonly Label lblExportInfo = new();
		private DataGridViewComboBoxColumn? groupColumn;

		private DataTable? groupRows;
		private DataTable? itemRows;
		private DataTable? bonusRows;

		private sealed class GroupOption
		{
			public int Id { get; set; }
			public string Name { get; set; } = "";
		}

		private static readonly (int Id, string Name)[] GroupKinds =
		[
			(GroupKindAuto, "No Set / automatic"),
			(GroupKindSet, "Set"),
			(GroupKindCustom, "Custom")
		];

		private static readonly (int Id, string Name)[] ScalingStats =
		[
			(21, "All hitrate"),
			(9, "Melee hitrate"),
			(10, "Ranged hitrate"),
			(17, "Magic hitrate"),
			(20, "All attack"),
			(6, "Physical attack"),
			(7, "Melee attack"),
			(8, "Ranged attack"),
			(16, "Magic attack"),
			(22, "All defense"),
			(11, "Physical defense"),
			(12, "Melee defense"),
			(13, "Ranged defense"),
			(18, "Magic defense"),
			(23, "All avoid"),
			(14, "Melee avoid"),
			(15, "Ranged avoid"),
			(19, "Magic avoid"),
			(0, "Strength"),
			(1, "Dexterity"),
			(2, "Intelligence"),
			(3, "Constitution"),
			(4, "Max HP"),
			(5, "Max MP"),
			(55, "Critical chance"),
			(66, "Deadly chance"),
			(56, "HP recovery %"),
			(57, "MP recovery %"),
			(99, "All stats"),
			(102, "HP recovery flat"),
			(103, "MP recovery flat"),
			(104, "Strong against monsters"),
			(105, "Hard against monsters"),
			(106, "Max HP flat")
		];

		public EquipmentCollectionEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "EquipmentCollectionEditor";
			Text = "Equipment Collection Editor";
			MinimumSize = new Size(1180, 720);
			Size = new Size(1500, 840);
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
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
			Controls.Add(root);

			root.Controls.Add(new Label
			{
				Dock = DockStyle.Fill,
				Text = "Curates which equipment and accessories are collectable, assigns them to display groups, and exports the client group-name file used by the equipment collection UI.",
				TextAlign = ContentAlignment.MiddleLeft
			}, 0, 0);

			FlowLayoutPanel toolbar = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(toolbar, 0, 1);

			ConfigureButton(btnReload, "Reload", async (_, _) => await LoadEditorAsync());
			ConfigureButton(btnAddCustomGroup, "Add custom group", (_, _) => AddCustomGroup());
			ConfigureButton(btnAddBonus, "Add scaling boost", (_, _) => AddScalingBonus());
			ConfigureButton(btnSave, "Save collection layout", async (_, _) => await SaveAsync());
			toolbar.Controls.AddRange([btnReload, btnAddCustomGroup, btnAddBonus, btnSave]);

			TableLayoutPanel dataLayout = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 3
			};
			dataLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220));
			dataLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			dataLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 190));
			root.Controls.Add(dataLayout, 0, 2);

			GroupBox groupBox = new() { Dock = DockStyle.Fill, Text = "Groups" };
			GroupBox itemBox = new() { Dock = DockStyle.Fill, Text = "Collectable items" };
			GroupBox bonusBox = new() { Dock = DockStyle.Fill, Text = "Scaling boosts per collected item" };
			groupBox.Controls.Add(gridGroups);
			itemBox.Controls.Add(gridItems);
			bonusBox.Controls.Add(gridBonuses);
			dataLayout.Controls.Add(groupBox, 0, 0);
			dataLayout.Controls.Add(itemBox, 0, 1);
			dataLayout.Controls.Add(bonusBox, 0, 2);

			lblExportInfo.Dock = DockStyle.Fill;
			lblExportInfo.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblExportInfo, 0, 3);

			lblStatus.Dock = DockStyle.Fill;
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblStatus, 0, 4);

			ConfigureGroupGrid();
			ConfigureItemGrid();
			ConfigureBonusGrid();
		}

		private static void ConfigureButton(Button button, string text, EventHandler handler)
		{
			button.Text = text;
			button.AutoSize = true;
			button.Margin = new Padding(0, 4, 8, 0);
			button.Click += handler;
		}

		private void ConfigureGroupGrid()
		{
			gridGroups.Dock = DockStyle.Fill;
			gridGroups.AutoGenerateColumns = false;
			gridGroups.AllowUserToAddRows = false;
			gridGroups.AllowUserToDeleteRows = false;
			gridGroups.RowHeadersVisible = false;
			gridGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridGroups.MultiSelect = false;
			gridGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			gridGroups.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			gridGroups.DataError += (_, _) => { };
			gridGroups.CurrentCellDirtyStateChanged += (_, _) =>
			{
				if (gridGroups.IsCurrentCellDirty)
					gridGroups.CommitEdit(DataGridViewDataErrorContexts.Commit);
			};
			gridGroups.CellEndEdit += (_, _) => RefreshGroupOptions();

			gridGroups.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_group_index", HeaderText = "Group ID", ReadOnly = true });
			gridGroups.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			gridGroups.Columns.Add(new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_kind",
				HeaderText = "Kind",
				DataSource = GroupKinds.Select(k => new { k.Id, k.Name }).ToList(),
				ValueMember = "Id",
				DisplayMember = "Name",
				ValueType = typeof(int),
				Width = 160
			});
			gridGroups.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_name", HeaderText = "Name", Width = 220 });
			gridGroups.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_sort", HeaderText = "Sort" });
			gridGroups.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemCount", HeaderText = "Items", ReadOnly = true });
			gridGroups.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EnabledItemCount", HeaderText = "Enabled items", ReadOnly = true });
		}

		private void ConfigureItemGrid()
		{
			gridItems.Dock = DockStyle.Fill;
			gridItems.AutoGenerateColumns = false;
			gridItems.AllowUserToAddRows = false;
			gridItems.AllowUserToDeleteRows = false;
			gridItems.RowHeadersVisible = false;
			gridItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridItems.MultiSelect = false;
			gridItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			gridItems.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			gridItems.DataError += (_, _) => { };
			gridItems.CurrentCellDirtyStateChanged += (_, _) =>
			{
				if (gridItems.IsCurrentCellDirty)
					gridItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
			};

			gridItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_item_index", HeaderText = "Item ID", ReadOnly = true });
			gridItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "Item name", ReadOnly = true, Width = 260 });
			gridItems.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Collectable", TrueValue = 1, FalseValue = 0 });

			groupColumn = new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_group_index",
				HeaderText = "Group",
				ValueMember = "Id",
				DisplayMember = "Name",
				ValueType = typeof(int),
				Width = 220
			};
			gridItems.Columns.Add(groupColumn);

			gridItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SlotName", HeaderText = "Slot", ReadOnly = true });
			gridItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_slot_type", HeaderText = "Slot ID" });
			gridItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_set_index", HeaderText = "Set ID" });
			gridItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_sort", HeaderText = "Sort" });
			gridItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_note", HeaderText = "Note", Width = 320 });
		}

		private void ConfigureBonusGrid()
		{
			gridBonuses.Dock = DockStyle.Fill;
			gridBonuses.AutoGenerateColumns = false;
			gridBonuses.AllowUserToAddRows = false;
			gridBonuses.AllowUserToDeleteRows = false;
			gridBonuses.RowHeadersVisible = false;
			gridBonuses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridBonuses.MultiSelect = false;
			gridBonuses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			gridBonuses.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			gridBonuses.DataError += (_, _) => { };
			gridBonuses.CurrentCellDirtyStateChanged += (_, _) =>
			{
				if (gridBonuses.IsCurrentCellDirty)
					gridBonuses.CommitEdit(DataGridViewDataErrorContexts.Commit);
			};

			gridBonuses.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_bonus_index", HeaderText = "Boost ID", ReadOnly = true });
			gridBonuses.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			gridBonuses.Columns.Add(new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_option_type",
				HeaderText = "Stat",
				DataSource = ScalingStats.Select(s => new { s.Id, s.Name }).ToList(),
				ValueMember = "Id",
				DisplayMember = "Name",
				ValueType = typeof(int),
				Width = 220
			});
			gridBonuses.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_value_per_item", HeaderText = "Value per item" });
			gridBonuses.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_max_value", HeaderText = "Max total (0 = none)" });
			gridBonuses.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_note", HeaderText = "Note", Width = 420 });
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading equipment collection layout...");

			if (!await Task.Run(EnsureTables))
			{
				SetBusy(false, "Could not verify equipment collection tables. Check Logs.log for the MySQL error.");
				return;
			}

			groupRows = await Task.Run(LoadGroups);
			itemRows = await Task.Run(LoadItems);
			bonusRows = await Task.Run(LoadBonuses);
			EnsureSlotNameColumn();

			gridGroups.DataSource = groupRows;
			RefreshGroupOptions();
			gridItems.DataSource = itemRows;
			gridBonuses.DataSource = bonusRows;
			lblExportInfo.Text = $"Exports {ClientDefinitionFile} to: {GetClientInterfaceFolder()}";
			SetBusy(false, $"Loaded {groupRows?.Rows.Count ?? 0} groups, {itemRows?.Rows.Count ?? 0} collectable layout rows, and {bonusRows?.Rows.Count ?? 0} scaling boosts.");
		}

		private bool EnsureTables()
		{
			string db = pMain.pSettings.DBData;
			StringBuilder query = new();
			query.Append($"CREATE TABLE IF NOT EXISTS {db}.{LayoutTable} (");
			query.Append("a_item_index int(11) NOT NULL,");
			query.Append("a_enable tinyint(1) NOT NULL DEFAULT 1,");
			query.Append("a_group_index int(11) NOT NULL DEFAULT 0,");
			query.Append("a_slot_index int(11) NOT NULL DEFAULT 0,");
			query.Append("a_slot_type int(11) NOT NULL DEFAULT -1,");
			query.Append("a_set_index int(11) NOT NULL DEFAULT 0,");
			query.Append("a_sort int(11) NOT NULL DEFAULT 0,");
			query.Append("a_note varchar(255) NOT NULL DEFAULT '',");
			query.Append("PRIMARY KEY (a_item_index)");
			query.Append(") ENGINE=InnoDB DEFAULT CHARSET=latin1;");
			query.Append($"CREATE TABLE IF NOT EXISTS {db}.{GroupTable} (");
			query.Append("a_group_index int(11) NOT NULL,");
			query.Append("a_enable tinyint(1) NOT NULL DEFAULT 1,");
			query.Append("a_kind tinyint(1) NOT NULL DEFAULT 1,");
			query.Append("a_name varchar(64) NOT NULL DEFAULT '',");
			query.Append("a_sort int(11) NOT NULL DEFAULT 0,");
			query.Append("PRIMARY KEY (a_group_index)");
			query.Append(") ENGINE=InnoDB DEFAULT CHARSET=latin1;");
			query.Append($"CREATE TABLE IF NOT EXISTS {db}.{BonusTable} (");
			query.Append("a_bonus_index int(11) NOT NULL AUTO_INCREMENT,");
			query.Append("a_enable tinyint(1) NOT NULL DEFAULT 1,");
			query.Append("a_option_type int(11) NOT NULL DEFAULT 21,");
			query.Append("a_value_per_item int(11) NOT NULL DEFAULT 1,");
			query.Append("a_max_value int(11) NOT NULL DEFAULT 0,");
			query.Append("a_note varchar(255) NOT NULL DEFAULT '',");
			query.Append("PRIMARY KEY (a_bonus_index)");
			query.Append(") ENGINE=InnoDB DEFAULT CHARSET=latin1;");
			query.Append($"INSERT IGNORE INTO {db}.{LayoutTable} ");
			query.Append("(a_item_index, a_enable, a_group_index, a_slot_index, a_slot_type, a_set_index, a_sort, a_note) ");
			query.Append("SELECT a_index, IF(a_set_4 BETWEEN 249 AND 262, 0, 1), ");
			query.Append("IF(a_set_4 > 0, a_set_4, 0), a_index, a_wearing, a_set_4, a_index, ");
			query.Append("IF(a_set_4 BETWEEN 249 AND 262, 'Missing client model assets; kept in LuckyDraw rewards', '') ");
			query.Append($"FROM {db}.t_item WHERE a_enable=1 AND a_type_idx IN (0, 1, 5) AND NOT (a_type_idx=5 AND a_subtype_idx IN (6, 7));");
			query.Append($"UPDATE {db}.{LayoutTable} SET a_enable=0, a_note='Missing client model assets; kept in LuckyDraw rewards' WHERE a_set_index BETWEEN 249 AND 262;");
			query.Append($"INSERT IGNORE INTO {db}.{GroupTable} (a_group_index, a_enable, a_kind, a_name, a_sort) VALUES (0, 1, {GroupKindAuto}, 'No Set', 0);");
			query.Append($"INSERT IGNORE INTO {db}.{GroupTable} (a_group_index, a_enable, a_kind, a_name, a_sort) ");
			query.Append($"SELECT DISTINCT a_group_index, 1, {GroupKindSet}, CONCAT('Set ', a_group_index), a_group_index FROM {db}.{LayoutTable} WHERE a_group_index > 0;");
			query.Append($"INSERT IGNORE INTO {db}.{BonusTable} (a_bonus_index, a_enable, a_option_type, a_value_per_item, a_max_value, a_note) ");
			query.Append("VALUES (1, 1, 21, 1, 0, '+1 all hitrate per collected item');");
			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query.ToString(), out long _, false);
		}

		private DataTable? LoadGroups()
		{
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT g.a_group_index, g.a_enable, g.a_kind, g.a_name, g.a_sort, " +
				"COUNT(l.a_item_index) AS ItemCount, COALESCE(SUM(IF(l.a_enable=1, 1, 0)), 0) AS EnabledItemCount " +
				$"FROM {db}.{GroupTable} g " +
				$"LEFT JOIN {db}.{LayoutTable} l ON l.a_group_index=g.a_group_index " +
				"GROUP BY g.a_group_index, g.a_enable, g.a_kind, g.a_name, g.a_sort " +
				"ORDER BY g.a_sort, g.a_group_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private DataTable? LoadBonuses()
		{
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT a_bonus_index, a_enable, a_option_type, a_value_per_item, a_max_value, a_note " +
				$"FROM {db}.{BonusTable} " +
				"ORDER BY a_bonus_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private DataTable? LoadItems()
		{
			string db = pMain.pSettings.DBData;
			string locale = pMain.pSettings.WorkLocale;
			string itemName = string.IsNullOrWhiteSpace(locale)
				? "COALESCE(NULLIF(i.a_name_usa, ''), NULLIF(i.a_name, ''), CONCAT('Item ', l.a_item_index))"
				: $"COALESCE(NULLIF(i.a_name_{locale}, ''), NULLIF(i.a_name_usa, ''), NULLIF(i.a_name, ''), CONCAT('Item ', l.a_item_index))";
			string query =
				"SELECT l.a_item_index, " +
				$"{itemName} AS ItemName, " +
				"l.a_enable, l.a_group_index, l.a_slot_index, l.a_slot_type, l.a_set_index, l.a_sort, l.a_note " +
				$"FROM {db}.{LayoutTable} l " +
				$"LEFT JOIN {db}.t_item i ON i.a_index=l.a_item_index " +
				"ORDER BY l.a_group_index, l.a_sort, l.a_slot_type, l.a_item_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private void EnsureSlotNameColumn()
		{
			if (itemRows == null)
				return;

			if (!itemRows.Columns.Contains("SlotName"))
				itemRows.Columns.Add("SlotName", typeof(string));

			foreach (DataRow row in itemRows.Rows)
				row["SlotName"] = SlotName(ToInt(row["a_slot_type"], -1));
		}

		private void RefreshGroupOptions()
		{
			if (groupColumn == null || groupRows == null)
				return;

			List<GroupOption> options = groupRows.Rows.Cast<DataRow>()
				.Where(r => r.RowState != DataRowState.Deleted)
				.OrderBy(r => ToInt(r["a_sort"]))
				.ThenBy(r => ToInt(r["a_group_index"]))
				.Select(r =>
				{
					int id = ToInt(r["a_group_index"]);
					string name = CleanGroupName(id, ToInt(r["a_kind"], id > 0 ? GroupKindSet : GroupKindAuto), ToStr(r["a_name"]));
					return new GroupOption { Id = id, Name = $"{id} - {name}" };
				})
				.ToList();

			if (!options.Any(o => o.Id == 0))
				options.Insert(0, new GroupOption { Id = 0, Name = "0 - No Set" });

			groupColumn.DataSource = null;
			groupColumn.DataSource = options;
			groupColumn.ValueMember = nameof(GroupOption.Id);
			groupColumn.DisplayMember = nameof(GroupOption.Name);
		}

		private void AddCustomGroup()
		{
			if (groupRows == null)
				return;

			int nextId = groupRows.Rows.Cast<DataRow>()
				.Where(r => r.RowState != DataRowState.Deleted)
				.Select(r => ToInt(r["a_group_index"]))
				.Where(id => id >= CustomGroupStart)
				.DefaultIfEmpty(CustomGroupStart - 1)
				.Max() + 1;

			DataRow row = groupRows.NewRow();
			row["a_group_index"] = nextId;
			row["a_enable"] = 1;
			row["a_kind"] = GroupKindCustom;
			row["a_name"] = $"Custom Group {nextId}";
			row["a_sort"] = nextId;
			row["ItemCount"] = 0;
			row["EnabledItemCount"] = 0;
			groupRows.Rows.Add(row);
			RefreshGroupOptions();
			SetBusy(false, $"Added custom group {nextId}. Assign items to it, then save.");
		}

		private void AddScalingBonus()
		{
			if (bonusRows == null)
				return;

			int nextId = bonusRows.Rows.Cast<DataRow>()
				.Where(r => r.RowState != DataRowState.Deleted)
				.Select(r => ToInt(r["a_bonus_index"]))
				.DefaultIfEmpty(0)
				.Max() + 1;

			DataRow row = bonusRows.NewRow();
			row["a_bonus_index"] = nextId;
			row["a_enable"] = 1;
			row["a_option_type"] = 21;
			row["a_value_per_item"] = 1;
			row["a_max_value"] = 0;
			row["a_note"] = "+1 all hitrate per collected item";
			bonusRows.Rows.Add(row);
			SetBusy(false, $"Added scaling boost {nextId}. Edit the stat/value, then save.");
		}

		private async Task SaveAsync()
		{
			if (groupRows == null || itemRows == null || bonusRows == null)
				return;

			Validate();
			gridGroups.EndEdit();
			gridItems.EndEdit();
			gridBonuses.EndEdit();

			string? validation = ValidateRows();
			if (validation != null)
			{
				SetBusy(false, validation);
				return;
			}

			SetBusy(true, "Saving equipment collection layout...");
			string query = BuildSaveQuery();
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			string exportError = "";

			if (ok)
			{
				try
				{
					WriteClientDefinitionFile();
				}
				catch (Exception ex)
				{
					exportError = ex.Message;
				}
			}

			await LoadEditorAsync();

			if (!ok)
				SetBusy(false, "Save failed. Check Logs.log for the MySQL error.");
			else if (!string.IsNullOrEmpty(exportError))
				SetBusy(false, $"Saved DB, but {ClientDefinitionFile} export failed: {exportError}");
			else
				SetBusy(false, $"Saved DB and exported {ClientDefinitionFile}. Restart the server; reopen the collection UI or restart the client to retest.");
		}

		private string? ValidateRows()
		{
			HashSet<int> groupIds = new();
			foreach (DataRow row in groupRows!.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;

				int id = ToInt(row["a_group_index"], -1);
				if (id < 0)
					return "Every group needs a non-negative group ID.";
				if (!groupIds.Add(id))
					return $"Group ID {id} is duplicated.";

				int kind = ToInt(row["a_kind"], id > 0 ? GroupKindSet : GroupKindAuto);
				if (kind < GroupKindAuto || kind > GroupKindCustom)
					return $"Group {id} uses an unknown kind.";
			}

			foreach (DataRow row in itemRows!.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;

				int itemId = ToInt(row["a_item_index"]);
				int groupId = ToInt(row["a_group_index"]);
				if (ToBoolInt(row["a_enable"]) != 0 && !groupIds.Contains(groupId))
					return $"Item {itemId} is assigned to missing group {groupId}.";
			}

			HashSet<int> bonusIds = new();
			HashSet<int> allowedStats = ScalingStats.Select(s => s.Id).ToHashSet();
			foreach (DataRow row in bonusRows!.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;

				int id = ToInt(row["a_bonus_index"], -1);
				if (id <= 0)
					return "Every scaling boost needs a positive boost ID.";
				if (!bonusIds.Add(id))
					return $"Scaling boost ID {id} is duplicated.";

				int optionType = ToInt(row["a_option_type"], -1);
				if (!allowedStats.Contains(optionType))
					return $"Scaling boost {id} uses an unsupported stat option.";

				if (ToInt(row["a_max_value"]) < 0)
					return $"Scaling boost {id} has a negative max total. Use 0 for no cap.";
			}

			return null;
		}

		private string BuildSaveQuery()
		{
			string db = pMain.pSettings.DBData;
			StringBuilder query = new();
			query.Append("START TRANSACTION;");

			foreach (DataRow row in groupRows!.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted).OrderBy(r => ToInt(r["a_group_index"])))
			{
				int id = Math.Max(0, ToInt(row["a_group_index"]));
				int enabled = ToBoolInt(row["a_enable"]);
				int kind = Math.Clamp(ToInt(row["a_kind"], id > 0 ? GroupKindSet : GroupKindAuto), GroupKindAuto, GroupKindCustom);
				string name = CleanGroupName(id, kind, ToStr(row["a_name"]));
				int sort = ToInt(row["a_sort"], id);

				query.Append($"INSERT INTO {db}.{GroupTable} (a_group_index, a_enable, a_kind, a_name, a_sort) VALUES ");
				query.Append($"({id}, {enabled}, {kind}, '{pMain.EscapeChars(name)}', {sort}) ");
				query.Append("ON DUPLICATE KEY UPDATE a_enable=VALUES(a_enable), a_kind=VALUES(a_kind), a_name=VALUES(a_name), a_sort=VALUES(a_sort);");
			}

			foreach (DataRow row in itemRows!.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted).OrderBy(r => ToInt(r["a_item_index"])))
			{
				int itemId = Math.Max(0, ToInt(row["a_item_index"]));
				int enabled = ToBoolInt(row["a_enable"]);
				int groupId = Math.Max(0, ToInt(row["a_group_index"]));
				int slotIndex = Math.Max(0, ToInt(row["a_slot_index"], itemId));
				int slotType = ToInt(row["a_slot_type"], -1);
				int setIndex = Math.Max(0, ToInt(row["a_set_index"]));
				int sort = ToInt(row["a_sort"], itemId);
				string note = CleanText(ToStr(row["a_note"]), 255);

				query.Append($"INSERT INTO {db}.{LayoutTable} (a_item_index, a_enable, a_group_index, a_slot_index, a_slot_type, a_set_index, a_sort, a_note) VALUES ");
				query.Append($"({itemId}, {enabled}, {groupId}, {slotIndex}, {slotType}, {setIndex}, {sort}, '{pMain.EscapeChars(note)}') ");
				query.Append("ON DUPLICATE KEY UPDATE a_enable=VALUES(a_enable), a_group_index=VALUES(a_group_index), a_slot_index=VALUES(a_slot_index), ");
				query.Append("a_slot_type=VALUES(a_slot_type), a_set_index=VALUES(a_set_index), a_sort=VALUES(a_sort), a_note=VALUES(a_note);");
			}

			foreach (DataRow row in bonusRows!.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted).OrderBy(r => ToInt(r["a_bonus_index"])))
			{
				int id = Math.Max(1, ToInt(row["a_bonus_index"]));
				int enabled = ToBoolInt(row["a_enable"]);
				int optionType = ToInt(row["a_option_type"], 21);
				int valuePerItem = ToInt(row["a_value_per_item"], 1);
				int maxValue = Math.Max(0, ToInt(row["a_max_value"]));
				string note = CleanText(ToStr(row["a_note"]), 255);

				query.Append($"INSERT INTO {db}.{BonusTable} (a_bonus_index, a_enable, a_option_type, a_value_per_item, a_max_value, a_note) VALUES ");
				query.Append($"({id}, {enabled}, {optionType}, {valuePerItem}, {maxValue}, '{pMain.EscapeChars(note)}') ");
				query.Append("ON DUPLICATE KEY UPDATE a_enable=VALUES(a_enable), a_option_type=VALUES(a_option_type), ");
				query.Append("a_value_per_item=VALUES(a_value_per_item), a_max_value=VALUES(a_max_value), a_note=VALUES(a_note);");
			}

			query.Append("COMMIT;");
			return query.ToString();
		}

		private void WriteClientDefinitionFile()
		{
			string folder = GetClientInterfaceFolder();
			if (string.IsNullOrWhiteSpace(folder))
				throw new InvalidOperationException("ClientPath is empty.");

			Directory.CreateDirectory(folder);
			string path = Path.Combine(folder, ClientDefinitionFile);

			StringBuilder file = new();
			file.AppendLine("# group_index\tkind\tsort\tname");
			file.AppendLine("# kind: 0=No Set automatic, 1=Set, 2=Custom");

			foreach (DataRow row in groupRows!.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted).OrderBy(r => ToInt(r["a_sort"])).ThenBy(r => ToInt(r["a_group_index"])))
			{
				if (ToBoolInt(row["a_enable"]) == 0)
					continue;

				int id = Math.Max(0, ToInt(row["a_group_index"]));
				int kind = Math.Clamp(ToInt(row["a_kind"], id > 0 ? GroupKindSet : GroupKindAuto), GroupKindAuto, GroupKindCustom);
				int sort = ToInt(row["a_sort"], id);
				string name = CleanGroupName(id, kind, ToStr(row["a_name"]));
				file.Append(id);
				file.Append('\t');
				file.Append(kind);
				file.Append('\t');
				file.Append(sort);
				file.Append('\t');
				file.Append(CleanText(name, 64));
				file.AppendLine();
			}

			File.WriteAllText(path, file.ToString(), new UTF8Encoding(false));
		}

		private string GetClientInterfaceFolder()
		{
			string clientPath = pMain.pSettings.ClientPath;
			if (string.IsNullOrWhiteSpace(clientPath))
				return "";

			return Path.Combine(clientPath.TrimEnd('\\'), "Data", "Interface");
		}

		private static string CleanGroupName(int id, int kind, string name)
		{
			name = CleanText(name, 64);
			if (!string.IsNullOrWhiteSpace(name))
				return name;

			if (kind == GroupKindAuto)
				return "No Set";
			if (kind == GroupKindSet)
				return $"Set {id}";
			return $"Custom Group {id}";
		}

		private static string CleanText(string value, int maxLength)
		{
			value = value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Trim();
			if (value.Length > maxLength)
				value = value[..maxLength];
			return value;
		}

		private static string SlotName(int slotType)
		{
			return slotType switch
			{
				0 => "Helm",
				1 => "Armor",
				2 => "Weapon",
				3 => "Pants",
				4 => "Shield",
				5 => "Glove",
				6 => "Boots",
				7 => "Accessory 1",
				8 => "Accessory 2",
				9 => "Accessory 3",
				10 => "Pet",
				11 => "Back",
				20 => "Costume Helm",
				21 => "Costume Armor",
				22 => "Costume Weapon",
				23 => "Costume Pants",
				24 => "Costume Shield",
				25 => "Costume Glove",
				26 => "Costume Boots",
				27 => "Costume Back",
				_ => "Other"
			};
		}

		private static int ToInt(object? value, int fallback = 0)
		{
			if (value == null || value == DBNull.Value)
				return fallback;
			if (value is bool b)
				return b ? 1 : 0;
			return int.TryParse(Convert.ToString(value), out int result) ? result : fallback;
		}

		private static int ToBoolInt(object? value)
		{
			if (value == null || value == DBNull.Value)
				return 0;
			if (value is bool b)
				return b ? 1 : 0;
			return int.TryParse(Convert.ToString(value), out int result) && result != 0 ? 1 : 0;
		}

		private static string ToStr(object? value)
		{
			if (value == null || value == DBNull.Value)
				return "";
			return Convert.ToString(value) ?? "";
		}

		private void SetBusy(bool busy, string text)
		{
			UseWaitCursor = busy;
			gridGroups.Enabled = !busy;
			gridItems.Enabled = !busy;
			gridBonuses.Enabled = !busy;
			btnReload.Enabled = !busy;
			btnAddCustomGroup.Enabled = !busy;
			btnAddBonus.Enabled = !busy;
			btnSave.Enabled = !busy;
			lblStatus.Text = text;
		}
	}
}
