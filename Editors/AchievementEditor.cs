namespace LastChaos_ToolBoxNG
{
	public class AchievementEditor : Form
	{
		private const string AchievementTable = "t_achievement";
		private const string StageTable = "t_achievement_stage";
		private const string ClientDefinitionFile = "AchievementDefinitions.txt";

		private readonly Main pMain;
		private readonly DataGridView gridAchievements = new();
		private readonly DataGridView gridStages = new();
		private readonly Button btnReload = new();
		private readonly Button btnAddAchievement = new();
		private readonly Button btnDisableAchievement = new();
		private readonly Button btnAddStage = new();
		private readonly Button btnRemoveStage = new();
		private readonly Button btnSave = new();
		private readonly ComboBox cbMapTarget = new();
		private readonly Button btnApplyMapTarget = new();
		private readonly Label lblTextureInfo = new();
		private readonly Label lblStatus = new();

		private DataTable? achievementRows;
		private DataTable? stageRows;
		private readonly List<ZoneOption> zoneOptions = new();

		private const int MetricMapVisited = 4;
		private const int MetricEquipmentCollected = 5;
		private const int MetricP2PetLevelups = 6;
		private const int ZoneDratan = 4;
		private const int ZoneMerac = 7;

		private sealed class ZoneOption
		{
			public int Id { get; set; }
			public string Name { get; set; } = "";
		}

		private static readonly (int Id, string Name)[] Tabs =
		[
			(0, "Personal"),
			(1, "Guild")
		];

		private static readonly (int Id, string Name)[] Metrics =
		[
			(1, "Unique login days"),
			(2, "Unique one-time quests completed"),
			(3, "Hostile NPCs killed"),
			(MetricMapVisited, "Map visited"),
			(MetricEquipmentCollected, "Equipment collection items registered"),
			(MetricP2PetLevelups, "P2 pet level-ups")
		];

		public AchievementEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "AchievementEditor";
			Text = "Achievement Editor";
			MinimumSize = new Size(1180, 720);
			Size = new Size(1480, 840);
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
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			root.Controls.Add(new Label
			{
				Dock = DockStyle.Fill,
				Text = "Creates achievement definitions read live by the rebuilt GameServer. Save also exports the client UI definition file, so labels, tabs, descriptions, and category texture names stay in sync.",
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
			ConfigureButton(btnAddAchievement, "Add achievement", (_, _) => AddAchievementRow());
			ConfigureButton(btnDisableAchievement, "Disable selected", (_, _) => DisableSelectedAchievement());
			ConfigureButton(btnAddStage, "Add stage", (_, _) => AddStageRow());
			ConfigureButton(btnRemoveStage, "Remove selected stage", (_, _) => RemoveSelectedStage());
			ConfigureButton(btnSave, "Save achievements", async (_, _) => await SaveAsync());
			toolbar.Controls.AddRange([btnReload, btnAddAchievement, btnDisableAchievement, btnAddStage, btnRemoveStage, btnSave]);

			lblTextureInfo.Dock = DockStyle.Fill;
			lblTextureInfo.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblTextureInfo, 0, 2);

			SplitContainer split = new()
			{
				Dock = DockStyle.Fill,
				Orientation = Orientation.Horizontal,
				SplitterDistance = 360
			};
			root.Controls.Add(split, 0, 3);

			GroupBox achievementsGroup = new() { Dock = DockStyle.Fill, Text = "Achievements" };
			GroupBox stagesGroup = new() { Dock = DockStyle.Fill, Text = "Stages for selected achievement" };
			achievementsGroup.Controls.Add(gridAchievements);

			TableLayoutPanel stagesPanel = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 2
			};
			stagesPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
			stagesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			FlowLayoutPanel stageTools = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			stageTools.Controls.Add(new Label
			{
				AutoSize = true,
				Text = "Map target:",
				TextAlign = ContentAlignment.MiddleLeft,
				Margin = new Padding(0, 8, 6, 0)
			});
			cbMapTarget.DropDownStyle = ComboBoxStyle.DropDownList;
			cbMapTarget.Width = 320;
			cbMapTarget.Margin = new Padding(0, 5, 8, 0);
			ConfigureButton(btnApplyMapTarget, "Apply map to selected stage", (_, _) => ApplyMapTargetToSelectedStage());
			stageTools.Controls.Add(cbMapTarget);
			stageTools.Controls.Add(btnApplyMapTarget);
			stagesPanel.Controls.Add(stageTools, 0, 0);
			stagesPanel.Controls.Add(gridStages, 0, 1);
			stagesGroup.Controls.Add(stagesPanel);

			split.Panel1.Controls.Add(achievementsGroup);
			split.Panel2.Controls.Add(stagesGroup);

			ConfigureAchievementGrid();
			ConfigureStageGrid();

			root.Controls.Add(new Label
			{
				Dock = DockStyle.Fill,
				Text = "The active stage description is the text shown in game; use %d where the current target should appear.",
				TextAlign = ContentAlignment.MiddleLeft
			}, 0, 4);

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

		private void ConfigureAchievementGrid()
		{
			gridAchievements.Dock = DockStyle.Fill;
			gridAchievements.AutoGenerateColumns = false;
			gridAchievements.AllowUserToAddRows = false;
			gridAchievements.AllowUserToDeleteRows = false;
			gridAchievements.RowHeadersVisible = false;
			gridAchievements.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridAchievements.MultiSelect = false;
			gridAchievements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			gridAchievements.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			gridAchievements.DataError += (_, _) => { };
			gridAchievements.CurrentCellDirtyStateChanged += (_, _) =>
			{
				if (gridAchievements.IsCurrentCellDirty)
					gridAchievements.CommitEdit(DataGridViewDataErrorContexts.Commit);
			};
			gridAchievements.SelectionChanged += (_, _) => ApplyStageFilter();
			gridAchievements.CellValueChanged += (_, _) =>
			{
				RefreshStageTargetInfo();
				UpdateMapTargetControls();
			};
			gridAchievements.CellEndEdit += (_, _) => RefreshExpectedTextureColumn();

			gridAchievements.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_id", HeaderText = "Achievement ID" });
			gridAchievements.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			gridAchievements.Columns.Add(new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_tab",
				HeaderText = "Tab",
				DataSource = Tabs.Select(t => new { t.Id, t.Name }).ToList(),
				ValueMember = "Id",
				DisplayMember = "Name",
				ValueType = typeof(int)
			});
			gridAchievements.Columns.Add(new DataGridViewComboBoxColumn
			{
				DataPropertyName = "a_metric_type",
				HeaderText = "Tracked metric",
				DataSource = Metrics.Select(m => new { m.Id, m.Name }).ToList(),
				ValueMember = "Id",
				DisplayMember = "Name",
				ValueType = typeof(int),
				Width = 210
			});
			gridAchievements.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_category", HeaderText = "Category title", Width = 160 });
			gridAchievements.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ExpectedTexture", HeaderText = "Expected icon_category .tex", ReadOnly = true, Width = 220 });
			gridAchievements.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_sort", HeaderText = "Sort" });
		}

		private void ConfigureStageGrid()
		{
			gridStages.Dock = DockStyle.Fill;
			gridStages.AutoGenerateColumns = false;
			gridStages.AllowUserToAddRows = false;
			gridStages.AllowUserToDeleteRows = false;
			gridStages.RowHeadersVisible = false;
			gridStages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridStages.MultiSelect = false;
			gridStages.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			gridStages.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			gridStages.DataError += (_, _) => { };
			gridStages.SelectionChanged += (_, _) => UpdateMapTargetControls();
			gridStages.CellEndEdit += (_, _) =>
			{
				RefreshStageTargetInfo();
				UpdateMapTargetControls();
			};

			gridStages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_achievement_id", HeaderText = "Achievement ID", ReadOnly = true });
			gridStages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_stage", HeaderText = "Stage" });
			gridStages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_target", HeaderText = "Required progress / zone ID" });
			gridStages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TargetInfo", HeaderText = "Target info", ReadOnly = true, Width = 220 });
			gridStages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_description", HeaderText = "Stage description format", Width = 360 });
			gridStages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_reward_item_idx", HeaderText = "Reward item ID" });
			gridStages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RewardName", HeaderText = "Reward item name", ReadOnly = true, Width = 220 });
			gridStages.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_reward_count", HeaderText = "Reward amount" });
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading achievements...");

			if (!await Task.Run(EnsureTables))
			{
				SetBusy(false, "Could not create or verify achievement tables. Check Logs.log for the MySQL error.");
				return;
			}

			await pMain.GenericLoadZoneDataAsync();
			RefreshZoneOptions();

			achievementRows = await Task.Run(LoadAchievements);
			stageRows = await Task.Run(LoadStages);
			EnsureStageTargetInfoColumn();
			RefreshStageTargetInfo();
			RefreshStageDescriptions();

			gridAchievements.DataSource = achievementRows;
			if (stageRows != null)
				gridStages.DataSource = stageRows.DefaultView;

			RefreshExpectedTextureColumn();
			ApplyStageFilter();
			UpdateMapTargetControls();
			SetBusy(false, $"Loaded {achievementRows?.Rows.Count ?? 0} achievements and {stageRows?.Rows.Count ?? 0} stages.");
		}

		private bool EnsureTables()
		{
			string db = pMain.pSettings.DBData;
			StringBuilder query = new();
			query.Append($"CREATE TABLE IF NOT EXISTS {db}.{AchievementTable} (");
			query.Append("a_id int(11) NOT NULL,");
			query.Append("a_enable tinyint(1) NOT NULL DEFAULT 1,");
			query.Append("a_tab int(11) NOT NULL DEFAULT 0,");
			query.Append("a_metric_type int(11) NOT NULL DEFAULT 1,");
			query.Append("a_category varchar(64) NOT NULL DEFAULT '',");
			query.Append("a_description varchar(160) NOT NULL DEFAULT '',");
			query.Append("a_icon_texture varchar(64) NOT NULL DEFAULT '',");
			query.Append("a_sort int(11) NOT NULL DEFAULT 0,");
			query.Append("PRIMARY KEY (a_id)");
			query.Append(") ENGINE=InnoDB DEFAULT CHARSET=latin1;");
			query.Append($"CREATE TABLE IF NOT EXISTS {db}.{StageTable} (");
			query.Append("a_achievement_id int(11) NOT NULL,");
			query.Append("a_stage int(11) NOT NULL,");
			query.Append("a_target int(11) NOT NULL DEFAULT 1,");
			query.Append("a_description varchar(160) NOT NULL DEFAULT '',");
			query.Append("a_reward_item_idx int(11) NOT NULL DEFAULT -1,");
			query.Append("a_reward_count int(11) NOT NULL DEFAULT 1,");
			query.Append("PRIMARY KEY (a_achievement_id, a_stage)");
			query.Append(") ENGINE=InnoDB DEFAULT CHARSET=latin1;");
			query.Append($"INSERT IGNORE INTO {db}.{AchievementTable} (a_id, a_enable, a_tab, a_metric_type, a_category, a_description, a_icon_texture, a_sort) VALUES ");
			query.Append("(2001, 1, 0, 1, 'Activity', '', 'AchievementCategory_2001.tex', 2001),");
			query.Append("(2002, 1, 0, 2, 'Questing', '', 'AchievementCategory_2002.tex', 2002);");
			query.Append($"INSERT IGNORE INTO {db}.{StageTable} (a_achievement_id, a_stage, a_target, a_description, a_reward_item_idx, a_reward_count) VALUES ");
			query.Append("(2001, 1, 10, 'Login during %d different days.', 85, 1),");
			query.Append("(2001, 2, 30, 'Login during %d different days.', 85, 1),");
			query.Append("(2002, 1, 10, 'Complete %d unique quests.', 85, 1);");

			if (!pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query.ToString(), out long _, false))
				return false;

			return EnsureStageDescriptionColumn();
		}

		private bool EnsureStageDescriptionColumn()
		{
			string db = pMain.pSettings.DBData;
			string checkQuery =
				"SELECT COUNT(*) AS a_count FROM information_schema.COLUMNS " +
				$"WHERE TABLE_SCHEMA='{pMain.EscapeChars(db)}' AND TABLE_NAME='{StageTable}' AND COLUMN_NAME='a_description';";
			DataTable? columns = pMain.QuerySelect(pMain.pSettings.DBCharset, checkQuery, false);
			if (columns == null || columns.Rows.Count == 0)
				return false;

			if (ToInt(columns.Rows[0]["a_count"]) > 0)
				return true;

			string alterQuery = $"ALTER TABLE {db}.{StageTable} ADD COLUMN a_description varchar(160) NOT NULL DEFAULT '' AFTER a_target;";
			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, alterQuery, out long _, false);
		}

		private DataTable? LoadAchievements()
		{
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT a_id, a_enable, a_tab, a_metric_type, a_category, a_description, a_icon_texture, a_sort, " +
				"CONCAT('AchievementCategory_', a_id, '.tex') AS ExpectedTexture " +
				$"FROM {db}.{AchievementTable} ORDER BY a_tab, a_sort, a_id;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private DataTable? LoadStages()
		{
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT s.a_achievement_id, s.a_stage, s.a_target, s.a_description, s.a_reward_item_idx, " +
				"COALESCE(NULLIF(i.a_name_usa, ''), NULLIF(i.a_name, ''), CONCAT('Item ', s.a_reward_item_idx)) AS RewardName, " +
				"s.a_reward_count " +
				$"FROM {db}.{StageTable} s " +
				$"LEFT JOIN {db}.t_item i ON i.a_index = s.a_reward_item_idx " +
				"ORDER BY s.a_achievement_id, s.a_stage;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private void ApplyStageFilter()
		{
			if (stageRows == null)
				return;

			int achievementId = GetCurrentAchievementId();
			stageRows.DefaultView.RowFilter = achievementId > 0 ? $"a_achievement_id = {achievementId}" : "a_achievement_id = -1";
			RefreshStageTargetInfo();
			RefreshTextureInfo(achievementId);
			UpdateMapTargetControls();
		}

		private void RefreshExpectedTextureColumn()
		{
			if (achievementRows == null)
				return;

			foreach (DataRow row in achievementRows.Rows)
			{
				int id = ToInt(row["a_id"]);
				row["ExpectedTexture"] = id > 0 ? ExpectedTexture(id) : "";
				row["a_icon_texture"] = id > 0 ? ExpectedTexture(id) : "";
			}

			RefreshTextureInfo(GetCurrentAchievementId());
		}

		private void RefreshTextureInfo(int achievementId)
		{
			string texture = achievementId > 0 ? ExpectedTexture(achievementId) : "AchievementCategory_<id>.tex";
			string folder = GetClientInterfaceFolder();
			lblTextureInfo.Text = $"icon_category expects: {texture}    Save .tex files in: {folder}";
		}

		private void AddAchievementRow()
		{
			if (achievementRows == null)
				return;

			int nextId = achievementRows.Rows.Cast<DataRow>().Select(r => ToInt(r["a_id"])).DefaultIfEmpty(2000).Max() + 1;
			DataRow row = achievementRows.NewRow();
			row["a_id"] = nextId;
			row["a_enable"] = 1;
			row["a_tab"] = 0;
			row["a_metric_type"] = 1;
			row["a_category"] = "New Achievement";
			row["a_description"] = "";
			row["a_icon_texture"] = ExpectedTexture(nextId);
			row["ExpectedTexture"] = ExpectedTexture(nextId);
			row["a_sort"] = nextId;
			achievementRows.Rows.Add(row);

			if (gridAchievements.Rows.Count > 0)
				gridAchievements.CurrentCell = gridAchievements.Rows[gridAchievements.Rows.Count - 1].Cells[0];

			AddStageRow(nextId);
			SetBusy(false, "Added an achievement with one stage. Edit it, then save.");
		}

		private void DisableSelectedAchievement()
		{
			if (gridAchievements.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			view.Row["a_enable"] = 0;
			SetBusy(false, "Selected achievement marked disabled. Save to update the database and client definition file.");
		}

		private void AddStageRow()
		{
			AddStageRow(GetCurrentAchievementId());
		}

		private void AddStageRow(int achievementId)
		{
			if (stageRows == null || achievementId <= 0)
				return;

			int nextStage = stageRows.Rows.Cast<DataRow>()
				.Where(r => r.RowState != DataRowState.Deleted)
				.Where(r => ToInt(r["a_achievement_id"]) == achievementId)
				.Select(r => ToInt(r["a_stage"]))
				.DefaultIfEmpty(0)
				.Max() + 1;

			DataRow row = stageRows.NewRow();
			row["a_achievement_id"] = achievementId;
			row["a_stage"] = nextStage;
			row["a_target"] = GetDefaultStageTarget(achievementId, nextStage);
			row["TargetInfo"] = GetStageTargetInfo(achievementId, ToInt(row["a_target"]));
			row["a_description"] = GetDefaultStageDescription(achievementId, ToInt(row["a_target"]), nextStage);
			row["a_reward_item_idx"] = 85;
			row["RewardName"] = "";
			row["a_reward_count"] = 1;
			stageRows.Rows.Add(row);
			ApplyStageFilter();
			SetBusy(false, $"Added stage {nextStage}.");
		}

		private void RemoveSelectedStage()
		{
			if (gridStages.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			view.Row.Delete();
			SetBusy(false, "Selected stage removed locally. Save to update the database.");
		}

		private async Task SaveAsync()
		{
			if (achievementRows == null || stageRows == null)
				return;

			Validate();
			gridAchievements.EndEdit();
			gridStages.EndEdit();
			RefreshExpectedTextureColumn();

			SetBusy(true, "Saving achievements...");

			string? validation = ValidateRows();
			if (validation != null)
			{
				SetBusy(false, validation);
				return;
			}

			string query = BuildSaveQuery();
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			bool clientOk = false;
			string clientMessage = "";

			if (ok)
			{
				try
				{
					WriteClientDefinitionFile();
					clientOk = true;
				}
				catch (Exception ex)
				{
					clientMessage = ex.Message;
				}
			}

			await LoadEditorAsync();

			if (!ok)
				SetBusy(false, "Save failed. Check the Toolbox console.");
			else if (!clientOk)
				SetBusy(false, $"Saved DB, but client definition export failed: {clientMessage}");
			else
				SetBusy(false, $"Saved achievements and exported {ClientDefinitionFile}. Restart the server; reopen the achievement UI or restart the client to retest.");
		}

		private string? ValidateRows()
		{
			HashSet<int> ids = new();
			foreach (DataRow row in achievementRows!.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;

				int id = ToInt(row["a_id"]);
				if (id <= 0)
					return "Every achievement needs a positive ID.";
				if (!ids.Add(id))
					return $"Achievement ID {id} is duplicated.";

				int metric = ToInt(row["a_metric_type"], 1);
				if (!Metrics.Any(m => m.Id == metric))
					return $"Achievement {id} uses an unknown metric.";
			}

			foreach (int id in ids)
			{
				bool hasStage = stageRows!.Rows.Cast<DataRow>().Any(r => r.RowState != DataRowState.Deleted && ToInt(r["a_achievement_id"]) == id);
				if (!hasStage)
					return $"Achievement {id} needs at least one stage.";
			}

			foreach (DataRow row in stageRows!.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;

				int achievementId = ToInt(row["a_achievement_id"]);
				if (!ids.Contains(achievementId))
					return $"A stage points to missing achievement {achievementId}.";
				int metric = GetAchievementMetric(achievementId);
				int target = ToInt(row["a_target"]);
				if (metric == MetricMapVisited)
				{
					if (target < 0)
						return $"Achievement {achievementId} has a stage with no required map.";
				}
				else if (target <= 0)
				{
					return $"Achievement {achievementId} has a stage with no required progress.";
				}
				if (ToInt(row["a_reward_item_idx"]) <= 0)
					return $"Achievement {achievementId} has a stage with no reward item ID.";
				if (ToInt(row["a_reward_count"]) <= 0)
					return $"Achievement {achievementId} has a stage with no reward amount.";
			}

			return null;
		}

		private string BuildSaveQuery()
		{
			string db = pMain.pSettings.DBData;
			StringBuilder query = new();
			query.Append("START TRANSACTION;");

			List<int> achievementIds = new();
			foreach (DataRow row in achievementRows!.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted).OrderBy(r => ToInt(r["a_tab"])).ThenBy(r => ToInt(r["a_sort"])).ThenBy(r => ToInt(r["a_id"])))
			{
				int id = ToInt(row["a_id"]);
				achievementIds.Add(id);
				string category = CleanText(ToStr(row["a_category"]), 64);
				if (string.IsNullOrWhiteSpace(category))
					category = $"Achievement {id}";

				int enabled = ToInt(row["a_enable"], 1) == 0 ? 0 : 1;
				int tab = ToInt(row["a_tab"], 0) == 1 ? 1 : 0;
				int metric = ToInt(row["a_metric_type"], 1);
				int sort = ToInt(row["a_sort"], id);
				string texture = ExpectedTexture(id);

				query.Append($"INSERT INTO {db}.{AchievementTable} (a_id, a_enable, a_tab, a_metric_type, a_category, a_description, a_icon_texture, a_sort) VALUES ");
				query.Append($"({id}, {enabled}, {tab}, {metric}, '{pMain.EscapeChars(category)}', '', '{pMain.EscapeChars(texture)}', {sort}) ");
				query.Append("ON DUPLICATE KEY UPDATE ");
				query.Append("a_enable=VALUES(a_enable), a_tab=VALUES(a_tab), a_metric_type=VALUES(a_metric_type), ");
				query.Append("a_category=VALUES(a_category), a_description=VALUES(a_description), a_icon_texture=VALUES(a_icon_texture), a_sort=VALUES(a_sort);");
			}

			query.Append($"DELETE FROM {db}.{StageTable} WHERE a_achievement_id IN ({string.Join(", ", achievementIds)});");

			Dictionary<int, int> stageNumbers = new();
			foreach (DataRow row in stageRows!.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted).OrderBy(r => ToInt(r["a_achievement_id"])).ThenBy(r => ToInt(r["a_stage"])).ThenBy(r => ToInt(r["a_target"])))
			{
				int achievementId = ToInt(row["a_achievement_id"]);
				if (!achievementIds.Contains(achievementId))
					continue;

				stageNumbers.TryGetValue(achievementId, out int stageNo);
				stageNo++;
				stageNumbers[achievementId] = stageNo;

				int metric = GetAchievementMetric(achievementId);
				int target = metric == MetricMapVisited ? Math.Max(0, ToInt(row["a_target"], 0)) : Math.Max(1, ToInt(row["a_target"], 1));
				string stageDescription = GetStageDisplayDescription(row);
				int rewardItem = Math.Max(1, ToInt(row["a_reward_item_idx"], 85));
				int rewardCount = Math.Max(1, ToInt(row["a_reward_count"], 1));
				query.Append($"INSERT INTO {db}.{StageTable} (a_achievement_id, a_stage, a_target, a_description, a_reward_item_idx, a_reward_count) VALUES ({achievementId}, {stageNo}, {target}, '{pMain.EscapeChars(stageDescription)}', {rewardItem}, {rewardCount});");
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
			HashSet<int> achievementsWithStages = stageRows!.Rows.Cast<DataRow>()
				.Where(r => r.RowState != DataRowState.Deleted)
				.Select(r => ToInt(r["a_achievement_id"]))
				.Where(id => id > 0)
				.ToHashSet();

			StringBuilder file = new();
			file.AppendLine("# id\ttab\tcategory\tcompat_description\ticon_texture");
			file.AppendLine("# stage\tachievement_id\tstage\tdescription_format");

			foreach (DataRow row in achievementRows!.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted).OrderBy(r => ToInt(r["a_tab"])).ThenBy(r => ToInt(r["a_sort"])).ThenBy(r => ToInt(r["a_id"])))
			{
				int id = ToInt(row["a_id"]);
				if (ToInt(row["a_enable"], 1) == 0 || !achievementsWithStages.Contains(id))
					continue;

				int tab = ToInt(row["a_tab"], 0) == 1 ? 1 : 0;
				string category = CleanText(ToStr(row["a_category"]), 64);
				if (string.IsNullOrWhiteSpace(category))
					category = $"Achievement {id}";

				List<DataRow> achievementStages = stageRows!.Rows.Cast<DataRow>()
					.Where(r => r.RowState != DataRowState.Deleted && ToInt(r["a_achievement_id"]) == id)
					.OrderBy(r => ToInt(r["a_stage"]))
					.ThenBy(r => ToInt(r["a_target"]))
					.ToList();
				string compatDescription = achievementStages.Count > 0 ? GetStageDisplayDescription(achievementStages[0]) : "%d required.";
				if (string.IsNullOrWhiteSpace(compatDescription))
					compatDescription = "%d required.";

				file.Append(id);
				file.Append('\t');
				file.Append(tab);
				file.Append('\t');
				file.Append(category);
				file.Append('\t');
				file.Append(compatDescription);
				file.Append('\t');
				file.Append(ExpectedTexture(id));
				file.AppendLine();

				int exportStageNo = 0;
				foreach (DataRow stageRow in achievementStages)
				{
					exportStageNo++;
					string stageDescription = GetStageDisplayDescription(stageRow);
					if (string.IsNullOrWhiteSpace(stageDescription))
						continue;

					file.Append("stage");
					file.Append('\t');
					file.Append(id);
					file.Append('\t');
					file.Append(exportStageNo);
					file.Append('\t');
					file.Append(stageDescription);
					file.AppendLine();
				}
			}

			File.WriteAllText(path, file.ToString(), new UTF8Encoding(false));
		}

		private void RefreshZoneOptions()
		{
			zoneOptions.Clear();

			if (pMain.pTables.ZoneTable != null)
			{
				foreach (DataRow row in pMain.pTables.ZoneTable.Rows)
				{
					int id = ToInt(row["a_zone_index"], -1);
					if (id < 0)
						continue;

					string name = ToStr(row["a_name"]);
					AddZoneOption(id, string.IsNullOrWhiteSpace(name) ? $"Zone {id}" : name);
				}
			}

			AddZoneOption(ZoneDratan, "Dratan");
			AddZoneOption(ZoneMerac, "Merac");

			List<ZoneOption> sorted = zoneOptions.OrderBy(z => z.Id).ToList();
			zoneOptions.Clear();
			zoneOptions.AddRange(sorted);

			cbMapTarget.DataSource = null;
			cbMapTarget.DisplayMember = nameof(ZoneOption.Name);
			cbMapTarget.ValueMember = nameof(ZoneOption.Id);
			cbMapTarget.DataSource = zoneOptions.ToList();
		}

		private void AddZoneOption(int id, string name)
		{
			if (zoneOptions.Any(z => z.Id == id))
				return;

			zoneOptions.Add(new ZoneOption { Id = id, Name = $"{id} - {name}" });
		}

		private int GetDefaultStageTarget(int achievementId, int stage)
		{
			if (GetAchievementMetric(achievementId) != MetricMapVisited)
				return stage == 1 ? 10 : stage * 10;

			if (stage == 1)
				return ZoneDratan;
			if (stage == 2)
				return ZoneMerac;
			if (cbMapTarget.SelectedValue != null && int.TryParse(Convert.ToString(cbMapTarget.SelectedValue), out int selectedZone))
				return selectedZone;
			return zoneOptions.FirstOrDefault()?.Id ?? ZoneDratan;
		}

		private void RefreshStageDescriptions()
		{
			if (stageRows == null)
				return;

			foreach (DataRow row in stageRows.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;

				if (string.IsNullOrWhiteSpace(ToStr(row["a_description"])))
					row["a_description"] = GetDefaultStageDescription(ToInt(row["a_achievement_id"]), ToInt(row["a_target"]), ToInt(row["a_stage"]));
			}
		}

		private string GetStageDisplayDescription(DataRow row)
		{
			string description = CleanText(ToStr(row["a_description"]), 160);
			if (!string.IsNullOrWhiteSpace(description))
				return description;

			return CleanText(GetDefaultStageDescription(ToInt(row["a_achievement_id"]), ToInt(row["a_target"]), ToInt(row["a_stage"])), 160);
		}

		private string GetDefaultStageDescription(int achievementId, int target, int stage)
		{
			switch (GetAchievementMetric(achievementId))
			{
				case 1:
					return "Login during %d different days.";
				case 2:
					return "Complete %d unique quests.";
				case 3:
					return "Kill %d hostile NPCs.";
				case MetricMapVisited:
					break;
				case MetricP2PetLevelups:
					return "Level up P2 pets %d times.";
				default:
					return "Reach %d progress.";
			}

			string targetInfo = GetStageTargetInfo(achievementId, target);
			int separator = targetInfo.IndexOf(" - ", StringComparison.Ordinal);
			string mapName = separator >= 0 ? targetInfo[(separator + 3)..] : targetInfo;
			if (string.IsNullOrWhiteSpace(mapName) || mapName.Contains("Unknown zone", StringComparison.OrdinalIgnoreCase))
				return "";

			return $"Visit {mapName}.";
		}

		private void EnsureStageTargetInfoColumn()
		{
			if (stageRows != null && !stageRows.Columns.Contains("TargetInfo"))
				stageRows.Columns.Add("TargetInfo", typeof(string));

			if (stageRows != null && stageRows.Columns["TargetInfo"] is DataColumn targetInfoColumn)
				targetInfoColumn.MaxLength = -1;

			if (stageRows != null && stageRows.Columns["a_description"] is DataColumn stageDescriptionColumn)
				stageDescriptionColumn.MaxLength = -1;
		}

		private void RefreshStageTargetInfo()
		{
			if (stageRows == null)
				return;

			EnsureStageTargetInfoColumn();
			foreach (DataRow row in stageRows.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;

				int achievementId = ToInt(row["a_achievement_id"]);
				row["TargetInfo"] = GetStageTargetInfo(achievementId, ToInt(row["a_target"]));
			}
		}

		private string GetStageTargetInfo(int achievementId, int target)
		{
			if (GetAchievementMetric(achievementId) != MetricMapVisited)
				return "";

			ZoneOption? zone = zoneOptions.FirstOrDefault(z => z.Id == target);
			return zone?.Name ?? $"{target} - Unknown zone";
		}

		private int GetCurrentAchievementMetric()
		{
			return GetAchievementMetric(GetCurrentAchievementId());
		}

		private int GetAchievementMetric(int achievementId)
		{
			if (achievementRows == null)
				return 1;

			foreach (DataRow row in achievementRows.Rows)
			{
				if (row.RowState == DataRowState.Deleted || ToInt(row["a_id"]) != achievementId)
					continue;

				return ToInt(row["a_metric_type"], 1);
			}

			return 1;
		}

		private void UpdateMapTargetControls()
		{
			bool isMapMetric = GetCurrentAchievementMetric() == MetricMapVisited;
			bool hasStage = gridStages.CurrentRow?.DataBoundItem is DataRowView;
			bool enabled = isMapMetric && hasStage && zoneOptions.Count > 0;

			cbMapTarget.Enabled = enabled;
			btnApplyMapTarget.Enabled = enabled;

			if (enabled && gridStages.CurrentRow?.DataBoundItem is DataRowView view)
			{
				int target = ToInt(view.Row["a_target"], ZoneDratan);
				if (zoneOptions.Any(z => z.Id == target))
					cbMapTarget.SelectedValue = target;
			}
		}

		private void ApplyMapTargetToSelectedStage()
		{
			if (GetCurrentAchievementMetric() != MetricMapVisited)
			{
				SetBusy(false, "Select a Map visited achievement before applying a map target.");
				return;
			}

			if (gridStages.CurrentRow?.DataBoundItem is not DataRowView view || cbMapTarget.SelectedValue == null)
				return;

			int zoneId = ToInt(cbMapTarget.SelectedValue, ZoneDratan);
			view.Row["a_target"] = zoneId;
			int achievementId = ToInt(view.Row["a_achievement_id"]);
			string targetInfo = GetStageTargetInfo(achievementId, zoneId);
			view.Row["TargetInfo"] = targetInfo;
			if (string.IsNullOrWhiteSpace(ToStr(view.Row["a_description"])))
				view.Row["a_description"] = GetDefaultStageDescription(achievementId, zoneId, ToInt(view.Row["a_stage"]));
			SetBusy(false, $"Selected stage now requires map: {targetInfo}.");
		}

		private int GetCurrentAchievementId()
		{
			if (gridAchievements.CurrentRow?.DataBoundItem is DataRowView view)
				return ToInt(view.Row["a_id"]);
			return 0;
		}

		private string GetClientInterfaceFolder()
		{
			string clientPath = pMain.pSettings.ClientPath;
			if (string.IsNullOrWhiteSpace(clientPath))
				return "";
			return Path.Combine(clientPath.TrimEnd('\\'), "Data", "Interface");
		}

		private static string ExpectedTexture(int achievementId)
		{
			return $"AchievementCategory_{achievementId}.tex";
		}

		private static string CleanText(string value, int maxLength)
		{
			value = value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Trim();
			if (value.Length > maxLength)
				value = value[..maxLength];
			return value;
		}

		private static int ToInt(object? value, int fallback = 0)
		{
			if (value == null || value == DBNull.Value)
				return fallback;
			if (value is bool b)
				return b ? 1 : 0;
			return int.TryParse(Convert.ToString(value), out int result) ? result : fallback;
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
			gridAchievements.Enabled = !busy;
			gridStages.Enabled = !busy;
			btnReload.Enabled = !busy;
			btnAddAchievement.Enabled = !busy;
			btnDisableAchievement.Enabled = !busy;
			btnAddStage.Enabled = !busy;
			btnRemoveStage.Enabled = !busy;
			btnSave.Enabled = !busy;
			lblStatus.Text = text;
		}
	}
}
