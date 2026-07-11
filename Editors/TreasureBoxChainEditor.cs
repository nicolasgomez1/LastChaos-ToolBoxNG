namespace LastChaos_ToolBoxNG
{
	public class TreasureBoxChainEditor : Form
	{
		private const string BoxTable = "t_treasure_box";
		private const string RewardTable = "t_treasure_box_reward";
		private const string ClassTable = "t_treasure_box_reward_class";

		private readonly Main pMain;
		private readonly DataGridView gridBoxes = new();
		private readonly DataGridView gridRewards = new();
		private readonly Button btnReload = new();
		private readonly Button btnAddBox = new();
		private readonly Button btnAddReward = new();
		private readonly Button btnPickItem = new();
		private readonly Button btnSeed = new();
		private readonly Button btnDeleteBox = new();
		private readonly Button btnDeleteReward = new();
		private readonly Button btnSave = new();
		private readonly Label lblStatus = new();

		private DataTable? boxRows;
		private DataTable? rewardRows;
		private DataGridView? lastGridWithFocus;

		private static readonly (int Id, string Name)[] RewardTypes =
		[
			(0, "Static item"),
			(1, "NAS money"),
			(2, "Class item")
		];

		private static readonly (int Id, string Name)[] TryResults =
		[
			(0, "Level 10 box"),
			(1, "Level 14 box"),
			(2, "Level 18 box"),
			(3, "Level 22 box"),
			(4, "Level 26 box"),
			(5, "Level 30 box")
		];

		private static readonly (int Id, string Name)[] OpenResults =
		[
			(0, "13 weapon"),
			(1, "21 weapon"),
			(2, "29 weapon"),
			(3, "NAS"),
			(4, "Candy"),
			(5, "Attack potion"),
			(6, "Defense potion"),
			(7, "No treasure box"),
			(13, "Pink dragon"),
			(14, "Blue horse")
		];

		public TreasureBoxChainEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "TreasureBoxChainEditor";
			Text = "Treasure Box Chain Editor";
			MinimumSize = new Size(1180, 720);
			Size = new Size(1450, 860);
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
				RowCount = 6,
				Padding = new Padding(8)
			};
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			root.Controls.Add(new Label
			{
				Dock = DockStyle.Fill,
				Text = "Edits ep4_data.t_treasure_box and rewards. Probability ranges use 1-10000. Reward type 'Class item' reads the job-specific item columns; static/NAS use Item ID and Count. The server falls back to the old hardcoded boxes if a box is not configured here.",
				TextAlign = ContentAlignment.MiddleLeft
			}, 0, 0);

			FlowLayoutPanel top = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
			root.Controls.Add(top, 0, 1);

			ConfigureButton(btnReload, "Reload", async (_, _) => await LoadEditorAsync());
			ConfigureButton(btnAddBox, "Add box", (_, _) => AddBoxRow());
			ConfigureButton(btnAddReward, "Add reward", (_, _) => AddRewardRow());
			ConfigureButton(btnPickItem, "Pick item for selected cell", (_, _) => PickItemForSelectedCell());
			ConfigureButton(btnSeed, "Seed current chain", async (_, _) => await SeedDefaultsAsync());
			top.Controls.AddRange([btnReload, btnAddBox, btnAddReward, btnPickItem, btnSeed]);

			ConfigureBoxGrid();
			ConfigureRewardGrid();

			GroupBox boxGroup = new() { Dock = DockStyle.Fill, Text = "Boxes / chain steps" };
			boxGroup.Controls.Add(gridBoxes);
			root.Controls.Add(boxGroup, 0, 2);

			GroupBox rewardGroup = new() { Dock = DockStyle.Fill, Text = "Rewards" };
			rewardGroup.Controls.Add(gridRewards);
			root.Controls.Add(rewardGroup, 0, 3);

			FlowLayoutPanel bottom = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
			root.Controls.Add(bottom, 0, 4);

			ConfigureButton(btnDeleteBox, "Delete selected box", async (_, _) => await DeleteSelectedBoxAsync());
			ConfigureButton(btnDeleteReward, "Delete selected reward", async (_, _) => await DeleteSelectedRewardAsync());
			ConfigureButton(btnSave, "Save treasure boxes", async (_, _) => await SaveAsync());
			bottom.Controls.AddRange([btnDeleteBox, btnDeleteReward, btnSave]);

			lblStatus.Dock = DockStyle.Fill;
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblStatus, 0, 5);
		}

		private static void ConfigureButton(Button button, string text, EventHandler handler)
		{
			button.Text = text;
			button.AutoSize = true;
			button.Margin = new Padding(0, 4, 8, 0);
			button.Click += handler;
		}

		private void ConfigureBoxGrid()
		{
			gridBoxes.Dock = DockStyle.Fill;
			gridBoxes.AutoGenerateColumns = false;
			gridBoxes.AllowUserToAddRows = false;
			gridBoxes.AllowUserToDeleteRows = false;
			gridBoxes.RowHeadersVisible = false;
			gridBoxes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridBoxes.MultiSelect = false;
			gridBoxes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			gridBoxes.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			gridBoxes.DataError += (_, _) => { };
			gridBoxes.Enter += (_, _) => lastGridWithFocus = gridBoxes;
			gridBoxes.CellClick += (_, _) => lastGridWithFocus = gridBoxes;

			gridBoxes.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			gridBoxes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_box_item_idx", HeaderText = "Box item ID" });
			gridBoxes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BoxName", HeaderText = "Box item name", ReadOnly = true, Width = 220 });
			gridBoxes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_required_level", HeaderText = "Required level" });
			gridBoxes.Columns.Add(new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_try_result_msg",
				HeaderText = "Old UI try result",
				DataSource = TryResults.Select(r => new { r.Id, r.Name }).ToList(),
				ValueMember = "Id",
				DisplayMember = "Name",
				Width = 130
			});
			gridBoxes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_next_box_item_idx", HeaderText = "Next box item ID" });
			gridBoxes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NextBoxName", HeaderText = "Next box name", ReadOnly = true, Width = 220 });
			gridBoxes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_note", HeaderText = "Note", Width = 260 });
		}

		private void ConfigureRewardGrid()
		{
			gridRewards.Dock = DockStyle.Fill;
			gridRewards.AutoGenerateColumns = false;
			gridRewards.AllowUserToAddRows = false;
			gridRewards.AllowUserToDeleteRows = false;
			gridRewards.RowHeadersVisible = false;
			gridRewards.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridRewards.MultiSelect = false;
			gridRewards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			gridRewards.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			gridRewards.DataError += (_, _) => { };
			gridRewards.Enter += (_, _) => lastGridWithFocus = gridRewards;
			gridRewards.CellClick += (_, _) => lastGridWithFocus = gridRewards;

			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_index", HeaderText = "Reward ID", ReadOnly = true });
			gridRewards.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_box_item_idx", HeaderText = "Box item ID" });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_reward_order", HeaderText = "Order" });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_prob_min", HeaderText = "Prob min" });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_prob_max", HeaderText = "Prob max" });
			gridRewards.Columns.Add(new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_reward_type",
				HeaderText = "Reward type",
				DataSource = RewardTypes.Select(r => new { r.Id, r.Name }).ToList(),
				ValueMember = "Id",
				DisplayMember = "Name",
				Width = 120
			});
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_item_idx", HeaderText = "Item ID / fallback" });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RewardName", HeaderText = "Item name", ReadOnly = true, Width = 200 });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_item_count", HeaderText = "Count" });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_plus", HeaderText = "Plus" });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_flag", HeaderText = "Flag" });
			gridRewards.Columns.Add(new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_result_msg",
				HeaderText = "Open result text",
				DataSource = OpenResults.Select(r => new { r.Id, r.Name }).ToList(),
				ValueMember = "Id",
				DisplayMember = "Name",
				Width = 130
			});

			AddJobColumn("job0_item", "Titan item");
			AddJobColumn("job1_item", "Knight item");
			AddJobColumn("job2_item", "Healer item");
			AddJobColumn("job3_item", "Mage item");
			AddJobColumn("job4_item", "Rogue item");
			AddJobColumn("job5_item", "Sorcerer item");
			AddJobColumn("job6_item", "NightShadow item");
			AddJobColumn("job7_item", "EX Rogue item");
			AddJobColumn("job8_item", "EX Mage item");

			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_note", HeaderText = "Note", Width = 260 });
		}

		private void AddJobColumn(string property, string text)
		{
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = property, HeaderText = text });
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading treasure box chain...");

			if (!await Task.Run(EnsureTables))
			{
				SetBusy(false, "Could not create or verify treasure box tables. Check Logs.log for the MySQL error.");
				return;
			}

			boxRows = await Task.Run(LoadBoxes);
			rewardRows = await Task.Run(LoadRewards);
			gridBoxes.DataSource = boxRows;
			gridRewards.DataSource = rewardRows;
			SetBusy(false, $"Loaded {boxRows?.Rows.Count ?? 0} boxes and {rewardRows?.Rows.Count ?? 0} rewards. The rebuilt GameServer reads box rows live.");
		}

		private bool EnsureTables()
		{
			string db = pMain.pSettings.DBData;
			string query =
				$"CREATE TABLE IF NOT EXISTS {db}.{BoxTable} (" +
				"a_box_item_idx int(11) NOT NULL, " +
				"a_enable tinyint(1) NOT NULL DEFAULT 1, " +
				"a_required_level int(11) NOT NULL DEFAULT 1, " +
				"a_try_result_msg int(11) NOT NULL DEFAULT 0, " +
				"a_next_box_item_idx int(11) NOT NULL DEFAULT 0, " +
				"a_note varchar(255) NOT NULL DEFAULT '', " +
				"PRIMARY KEY (a_box_item_idx)" +
				") ENGINE=InnoDB DEFAULT CHARSET=latin1;\n" +
				$"CREATE TABLE IF NOT EXISTS {db}.{RewardTable} (" +
				"a_index int(11) NOT NULL AUTO_INCREMENT, " +
				"a_enable tinyint(1) NOT NULL DEFAULT 1, " +
				"a_box_item_idx int(11) NOT NULL, " +
				"a_reward_order int(11) NOT NULL DEFAULT 0, " +
				"a_prob_min int(11) NOT NULL DEFAULT 1, " +
				"a_prob_max int(11) NOT NULL DEFAULT 10000, " +
				"a_reward_type int(11) NOT NULL DEFAULT 0, " +
				"a_item_idx int(11) NOT NULL DEFAULT 0, " +
				"a_item_count bigint(20) NOT NULL DEFAULT 1, " +
				"a_plus int(11) NOT NULL DEFAULT 0, " +
				"a_flag int(11) NOT NULL DEFAULT 0, " +
				"a_result_msg int(11) NOT NULL DEFAULT 0, " +
				"a_note varchar(255) NOT NULL DEFAULT '', " +
				"PRIMARY KEY (a_index), " +
				"KEY idx_treasure_reward_box (a_box_item_idx, a_reward_order)" +
				") ENGINE=InnoDB DEFAULT CHARSET=latin1;\n" +
				$"CREATE TABLE IF NOT EXISTS {db}.{ClassTable} (" +
				"a_reward_index int(11) NOT NULL, " +
				"a_job int(11) NOT NULL, " +
				"a_item_idx int(11) NOT NULL DEFAULT 0, " +
				"PRIMARY KEY (a_reward_index, a_job)" +
				") ENGINE=InnoDB DEFAULT CHARSET=latin1;";
			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false);
		}

		private DataTable? LoadBoxes()
		{
			string db = pMain.pSettings.DBData;
			string locale = pMain.pSettings.WorkLocale;
			string boxName = $"COALESCE(NULLIF(bi.a_name_{locale}, ''), NULLIF(bi.a_name, ''), NULLIF(bi.a_name_usa, ''), CONCAT('Item ', b.a_box_item_idx))";
			string nextName = $"COALESCE(NULLIF(ni.a_name_{locale}, ''), NULLIF(ni.a_name, ''), NULLIF(ni.a_name_usa, ''), CONCAT('Item ', b.a_next_box_item_idx))";
			string query =
				"SELECT b.a_enable, b.a_box_item_idx, " +
				$"{boxName} AS BoxName, " +
				"b.a_required_level, b.a_try_result_msg, b.a_next_box_item_idx, " +
				$"{nextName} AS NextBoxName, " +
				"b.a_note " +
				$"FROM {db}.{BoxTable} b " +
				$"LEFT JOIN {db}.t_item bi ON bi.a_index=b.a_box_item_idx " +
				$"LEFT JOIN {db}.t_item ni ON ni.a_index=b.a_next_box_item_idx " +
				"ORDER BY b.a_required_level, b.a_box_item_idx;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private DataTable? LoadRewards()
		{
			string db = pMain.pSettings.DBData;
			string locale = pMain.pSettings.WorkLocale;
			string itemName = $"COALESCE(NULLIF(i.a_name_{locale}, ''), NULLIF(i.a_name, ''), NULLIF(i.a_name_usa, ''), CONCAT('Item ', r.a_item_idx))";
			string query =
				"SELECT r.a_index, r.a_enable, r.a_box_item_idx, r.a_reward_order, r.a_prob_min, r.a_prob_max, " +
				"r.a_reward_type, r.a_item_idx, " +
				$"{itemName} AS RewardName, " +
				"r.a_item_count, r.a_plus, r.a_flag, r.a_result_msg, " +
				JobSubselect(0) + ", " + JobSubselect(1) + ", " + JobSubselect(2) + ", " + JobSubselect(3) + ", " +
				JobSubselect(4) + ", " + JobSubselect(5) + ", " + JobSubselect(6) + ", " + JobSubselect(7) + ", " +
				JobSubselect(8) + ", r.a_note " +
				$"FROM {db}.{RewardTable} r " +
				$"LEFT JOIN {db}.t_item i ON i.a_index=r.a_item_idx " +
				"ORDER BY r.a_box_item_idx, r.a_reward_order, r.a_prob_min, r.a_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);

			string JobSubselect(int job) =>
				$"IFNULL((SELECT c.a_item_idx FROM {db}.{ClassTable} c WHERE c.a_reward_index=r.a_index AND c.a_job={job} LIMIT 1), 0) AS job{job}_item";
		}

		private void AddBoxRow()
		{
			if (boxRows == null)
				return;

			DataRow row = boxRows.NewRow();
			row["a_enable"] = 1;
			row["a_box_item_idx"] = 0;
			row["BoxName"] = "";
			row["a_required_level"] = 1;
			row["a_try_result_msg"] = 0;
			row["a_next_box_item_idx"] = 0;
			row["NextBoxName"] = "";
			row["a_note"] = "";
			boxRows.Rows.Add(row);
			SetBusy(false, "Added a box row. Pick the box item, then save.");
		}

		private void AddRewardRow()
		{
			if (rewardRows == null)
				return;

			int boxId = 760;
			if (gridBoxes.CurrentRow?.DataBoundItem is DataRowView boxView)
				boxId = GetInt(boxView.Row, "a_box_item_idx", 760);

			DataRow row = rewardRows.NewRow();
			row["a_index"] = 0;
			row["a_enable"] = 1;
			row["a_box_item_idx"] = boxId;
			row["a_reward_order"] = NextRewardOrder(boxId);
			row["a_prob_min"] = 1;
			row["a_prob_max"] = 10000;
			row["a_reward_type"] = 0;
			row["a_item_idx"] = 0;
			row["RewardName"] = "";
			row["a_item_count"] = 1;
			row["a_plus"] = 0;
			row["a_flag"] = 0;
			row["a_result_msg"] = 4;
			for (int job = 0; job <= 8; job++)
				row[$"job{job}_item"] = 0;
			row["a_note"] = "";
			rewardRows.Rows.Add(row);
			SetBusy(false, $"Added a reward row for box {boxId}.");
		}

		private int NextRewardOrder(int boxId)
		{
			int next = 0;
			if (rewardRows == null)
				return next;

			foreach (DataRow row in rewardRows.Rows)
			{
				if (row.RowState == DataRowState.Deleted || GetInt(row, "a_box_item_idx") != boxId)
					continue;
				next = Math.Max(next, GetInt(row, "a_reward_order") + 10);
			}
			return next;
		}

		private void PickItemForSelectedCell()
		{
			if (lastGridWithFocus == gridRewards && gridRewards.CurrentRow?.DataBoundItem is DataRowView rewardView)
			{
				string property = gridRewards.Columns[gridRewards.CurrentCell.ColumnIndex].DataPropertyName;
				if (property != "a_item_idx" && !property.StartsWith("job", StringComparison.Ordinal))
					property = "a_item_idx";

				SetPickedItem(rewardView.Row, property, "RewardName");
				return;
			}

			if (gridBoxes.CurrentRow?.DataBoundItem is DataRowView boxView)
			{
				string property = gridBoxes.Columns[gridBoxes.CurrentCell.ColumnIndex].DataPropertyName;
				if (property != "a_next_box_item_idx")
					property = "a_box_item_idx";

				SetPickedItem(boxView.Row, property, property == "a_next_box_item_idx" ? "NextBoxName" : "BoxName");
			}
		}

		private void SetPickedItem(DataRow row, string idColumn, string nameColumn)
		{
			int currentItem = GetInt(row, idColumn);
			using ItemPicker picker = new(pMain, this, currentItem, true);
			if (picker.ShowDialog(this) != DialogResult.OK)
				return;

			int pickedItem = Convert.ToInt32(picker.ReturnValues[0]);
			row[idColumn] = pickedItem;
			if (row.Table.Columns.Contains(nameColumn))
				row[nameColumn] = picker.ReturnValues[1]?.ToString() ?? "";
			SetBusy(false, $"Selected item {pickedItem}. Save to update the database.");
		}

		private async Task SeedDefaultsAsync()
		{
			DialogResult result = MessageBox.Show(
				"This replaces the treasure box tables with the current 760-765 chain defaults. Continue?",
				"Seed treasure box chain",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);
			if (result != DialogResult.Yes)
				return;

			SetBusy(true, "Seeding current treasure box chain...");
			bool ok = await Task.Run(SeedDefaults);
			await LoadEditorAsync();
			SetBusy(false, ok ? "Seeded current chain. Edit and save; box opens use the saved rows immediately." : "Seed failed. Check Logs.log for the MySQL error.");
		}

		private bool SeedDefaults()
		{
			string db = pMain.pSettings.DBData;
			string query =
				$"DELETE FROM {db}.{ClassTable};\n" +
				$"DELETE FROM {db}.{RewardTable};\n" +
				$"DELETE FROM {db}.{BoxTable};\n" +
				BuildSeedBoxes(db) + "\n" +
				BuildSeedRewards(db) + "\n" +
				BuildSeedClassRewards(db);
			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false);
		}

		private static string BuildSeedBoxes(string db)
		{
			return
				$"INSERT INTO {db}.{BoxTable} (a_box_item_idx,a_enable,a_required_level,a_try_result_msg,a_next_box_item_idx,a_note) VALUES " +
				"(760,1,10,0,761,'Level 10 starter treasure box')," +
				"(761,1,14,1,762,'Level 14 treasure box')," +
				"(762,1,18,2,763,'Level 18 treasure box')," +
				"(763,1,22,3,764,'Level 22 treasure box')," +
				"(764,1,26,4,765,'Level 26 treasure box')," +
				"(765,1,30,5,0,'Level 30 final treasure box');";
		}

		private static string BuildSeedRewards(string db)
		{
			List<string> rows = [];
			void Add(int id, int box, int order, int min, int max, int type, int item, long count, int plus, int result, string note)
			{
				rows.Add($"({id},1,{box},{order},{min},{max},{type},{item},{count},{plus},0,{result},'{note}')");
			}

			Add(76001, 760, 10, 1, 1000, 2, 0, 1, 4, 0, "13 weapon option A");
			Add(76002, 760, 20, 1001, 2000, 2, 0, 1, 4, 0, "13 weapon option B");
			Add(76003, 760, 30, 2001, 5000, 1, 19, 30000, 0, 3, "NAS");
			Add(76004, 760, 40, 5001, 10000, 0, 556, 5, 0, 4, "Candy");

			Add(76101, 761, 10, 1, 2000, 0, 510, 10, 0, 5, "Attack potion");
			Add(76102, 761, 20, 2001, 5000, 1, 19, 50000, 0, 3, "NAS");
			Add(76103, 761, 30, 5001, 10000, 0, 556, 10, 0, 4, "Candy");

			Add(76201, 762, 10, 1, 1000, 2, 0, 1, 4, 1, "21 weapon option A");
			Add(76202, 762, 20, 1001, 2000, 2, 0, 1, 4, 1, "21 weapon option B");
			Add(76203, 762, 30, 2001, 5000, 1, 19, 80000, 0, 3, "NAS");
			Add(76204, 762, 40, 5001, 10000, 0, 556, 15, 0, 4, "Candy");

			Add(76301, 763, 10, 1, 2000, 0, 511, 15, 0, 6, "Defense potion");
			Add(76302, 763, 20, 2001, 5000, 1, 19, 100000, 0, 3, "NAS");
			Add(76303, 763, 30, 5001, 10000, 0, 556, 20, 0, 4, "Candy");

			Add(76401, 764, 10, 1, 1000, 2, 0, 1, 4, 2, "29 weapon option A");
			Add(76402, 764, 20, 1001, 2000, 2, 0, 1, 4, 2, "29 weapon option B");
			Add(76403, 764, 30, 2001, 5000, 1, 19, 150000, 0, 3, "NAS");
			Add(76404, 764, 40, 5001, 10000, 0, 556, 30, 0, 4, "Candy");

			Add(76501, 765, 10, 1, 2000, 0, 510, 20, 0, 5, "Attack potion");
			Add(76502, 765, 20, 2001, 5000, 1, 19, 200000, 0, 3, "NAS");
			Add(76503, 765, 30, 5001, 10000, 0, 556, 40, 0, 4, "Candy");

			return
				$"INSERT INTO {db}.{RewardTable} " +
				"(a_index,a_enable,a_box_item_idx,a_reward_order,a_prob_min,a_prob_max,a_reward_type,a_item_idx,a_item_count,a_plus,a_flag,a_result_msg,a_note) VALUES " +
				string.Join(",", rows) + ";";
		}

		private static string BuildSeedClassRewards(string db)
		{
			List<string> rows = [];
			void Add(int reward, params int[] items)
			{
				for (int job = 0; job < items.Length; job++)
				{
					if (items[job] > 0)
						rows.Add($"({reward},{job},{items[job]})");
				}
			}

			Add(76001, 105, 107, 106, 359, 533, 979, 0, 533, 359);
			Add(76002, 603, 612, 621, 630, 638, 991, 0, 638, 630);
			Add(76201, 307, 322, 341, 361, 535, 981, 0, 535, 361);
			Add(76202, 605, 614, 623, 632, 640, 993, 0, 640, 632);
			Add(76401, 309, 324, 343, 363, 537, 983, 0, 537, 363);
			Add(76402, 607, 616, 625, 634, 642, 995, 0, 642, 634);

			return $"INSERT INTO {db}.{ClassTable} (a_reward_index,a_job,a_item_idx) VALUES " + string.Join(",", rows) + ";";
		}

		private async Task DeleteSelectedBoxAsync()
		{
			if (gridBoxes.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			int boxId = GetInt(view.Row, "a_box_item_idx");
			if (boxId <= 0)
			{
				boxRows?.Rows.Remove(view.Row);
				return;
			}

			DialogResult result = MessageBox.Show($"Delete box {boxId} and all of its rewards?", "Delete treasure box", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (result != DialogResult.Yes)
				return;

			string db = pMain.pSettings.DBData;
			string query =
				$"DELETE c FROM {db}.{ClassTable} c INNER JOIN {db}.{RewardTable} r ON r.a_index=c.a_reward_index WHERE r.a_box_item_idx={boxId};\n" +
				$"DELETE FROM {db}.{RewardTable} WHERE a_box_item_idx={boxId};\n" +
				$"DELETE FROM {db}.{BoxTable} WHERE a_box_item_idx={boxId};";
			SetBusy(true, $"Deleting box {boxId}...");
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			await LoadEditorAsync();
			SetBusy(false, ok ? $"Deleted box {boxId}." : "Delete failed. Check Logs.log for the MySQL error.");
		}

		private async Task DeleteSelectedRewardAsync()
		{
			if (gridRewards.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			int rewardId = GetInt(view.Row, "a_index");
			if (rewardId <= 0)
			{
				rewardRows?.Rows.Remove(view.Row);
				return;
			}

			DialogResult result = MessageBox.Show($"Delete reward row {rewardId}?", "Delete reward", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (result != DialogResult.Yes)
				return;

			string db = pMain.pSettings.DBData;
			string query = $"DELETE FROM {db}.{ClassTable} WHERE a_reward_index={rewardId};\nDELETE FROM {db}.{RewardTable} WHERE a_index={rewardId};";
			SetBusy(true, $"Deleting reward {rewardId}...");
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			await LoadEditorAsync();
			SetBusy(false, ok ? $"Deleted reward {rewardId}." : "Delete failed. Check Logs.log for the MySQL error.");
		}

		private async Task SaveAsync()
		{
			Validate();
			gridBoxes.EndEdit();
			gridRewards.EndEdit();
			SetBusy(true, "Saving treasure box chain...");

			bool ok = true;
			if (boxRows != null)
			{
				foreach (DataRow row in boxRows.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					NormalizeBox(row);
					ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, BuildSaveBoxQuery(row), out long _, false));
					if (!ok)
						break;
				}
			}

			if (ok && rewardRows != null)
			{
				foreach (DataRow row in rewardRows.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					NormalizeReward(row);
					ok = await SaveRewardRowAsync(row);
					if (!ok)
						break;
				}
			}

			await LoadEditorAsync();
			SetBusy(false, ok ? "Saved treasure box chain. Box opens use these rows immediately." : "Save failed. Check Logs.log for the MySQL error.");
		}

		private string BuildSaveBoxQuery(DataRow row)
		{
			return
				$"INSERT INTO {pMain.pSettings.DBData}.{BoxTable} " +
				"(a_box_item_idx,a_enable,a_required_level,a_try_result_msg,a_next_box_item_idx,a_note) VALUES " +
				$"({GetInt(row, "a_box_item_idx")},{GetBoolInt(row, "a_enable")},{GetInt(row, "a_required_level")},{GetInt(row, "a_try_result_msg")}," +
				$"{GetInt(row, "a_next_box_item_idx")},{SqlString(GetString(row, "a_note"))}) " +
				"ON DUPLICATE KEY UPDATE a_enable=VALUES(a_enable), a_required_level=VALUES(a_required_level), " +
				"a_try_result_msg=VALUES(a_try_result_msg), a_next_box_item_idx=VALUES(a_next_box_item_idx), a_note=VALUES(a_note);";
		}

		private async Task<bool> SaveRewardRowAsync(DataRow row)
		{
			int rewardId = GetInt(row, "a_index");
			long lastInsertId = -1;
			string query = BuildSaveRewardQuery(row, rewardId);
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out lastInsertId, false));
			if (!ok)
				return false;

			if (rewardId <= 0)
				rewardId = (int)lastInsertId;
			if (rewardId <= 0)
				return false;

			return await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, BuildSaveClassQuery(row, rewardId), out long _, false));
		}

		private string BuildSaveRewardQuery(DataRow row, int rewardId)
		{
			string idColumn = rewardId > 0 ? "a_index," : "";
			string idValue = rewardId > 0 ? $"{rewardId}," : "";
			return
				$"INSERT INTO {pMain.pSettings.DBData}.{RewardTable} " +
				$"({idColumn}a_enable,a_box_item_idx,a_reward_order,a_prob_min,a_prob_max,a_reward_type,a_item_idx,a_item_count,a_plus,a_flag,a_result_msg,a_note) VALUES " +
				$"({idValue}{GetBoolInt(row, "a_enable")},{GetInt(row, "a_box_item_idx")},{GetInt(row, "a_reward_order")}," +
				$"{GetInt(row, "a_prob_min")},{GetInt(row, "a_prob_max")},{GetInt(row, "a_reward_type")},{GetInt(row, "a_item_idx")}," +
				$"{GetLong(row, "a_item_count")},{GetInt(row, "a_plus")},{GetInt(row, "a_flag")},{GetInt(row, "a_result_msg")},{SqlString(GetString(row, "a_note"))}) " +
				"ON DUPLICATE KEY UPDATE a_enable=VALUES(a_enable), a_box_item_idx=VALUES(a_box_item_idx), " +
				"a_reward_order=VALUES(a_reward_order), a_prob_min=VALUES(a_prob_min), a_prob_max=VALUES(a_prob_max), " +
				"a_reward_type=VALUES(a_reward_type), a_item_idx=VALUES(a_item_idx), a_item_count=VALUES(a_item_count), " +
				"a_plus=VALUES(a_plus), a_flag=VALUES(a_flag), a_result_msg=VALUES(a_result_msg), a_note=VALUES(a_note);";
		}

		private string BuildSaveClassQuery(DataRow row, int rewardId)
		{
			string db = pMain.pSettings.DBData;
			List<string> values = [];
			for (int job = 0; job <= 8; job++)
			{
				int itemId = GetInt(row, $"job{job}_item");
				if (itemId > 0)
					values.Add($"({rewardId},{job},{itemId})");
			}

			string query = $"DELETE FROM {db}.{ClassTable} WHERE a_reward_index={rewardId};";
			if (values.Count > 0)
				query += $"\nINSERT INTO {db}.{ClassTable} (a_reward_index,a_job,a_item_idx) VALUES {string.Join(",", values)};";
			return query;
		}

		private static void NormalizeBox(DataRow row)
		{
			row["a_enable"] = GetBoolInt(row, "a_enable");
			row["a_box_item_idx"] = Math.Max(0, GetInt(row, "a_box_item_idx"));
			row["a_required_level"] = Math.Max(1, GetInt(row, "a_required_level"));
			row["a_try_result_msg"] = Math.Clamp(GetInt(row, "a_try_result_msg"), 0, 5);
			row["a_next_box_item_idx"] = Math.Max(0, GetInt(row, "a_next_box_item_idx"));
		}

		private static void NormalizeReward(DataRow row)
		{
			row["a_enable"] = GetBoolInt(row, "a_enable");
			row["a_box_item_idx"] = Math.Max(0, GetInt(row, "a_box_item_idx"));
			row["a_reward_order"] = Math.Max(0, GetInt(row, "a_reward_order"));
			row["a_prob_min"] = Math.Clamp(GetInt(row, "a_prob_min"), 1, 10000);
			row["a_prob_max"] = Math.Clamp(GetInt(row, "a_prob_max"), 1, 10000);
			if (GetInt(row, "a_prob_max") < GetInt(row, "a_prob_min"))
				row["a_prob_max"] = row["a_prob_min"];
			row["a_reward_type"] = Math.Clamp(GetInt(row, "a_reward_type"), 0, 2);
			row["a_item_count"] = Math.Max(1, GetLong(row, "a_item_count"));
			for (int job = 0; job <= 8; job++)
				row[$"job{job}_item"] = Math.Max(0, GetInt(row, $"job{job}_item"));
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
			btnAddBox.Enabled = !busy;
			btnAddReward.Enabled = !busy;
			btnPickItem.Enabled = !busy;
			btnSeed.Enabled = !busy;
			btnDeleteBox.Enabled = !busy;
			btnDeleteReward.Enabled = !busy;
			btnSave.Enabled = !busy;
			lblStatus.Text = message;
		}
	}
}
