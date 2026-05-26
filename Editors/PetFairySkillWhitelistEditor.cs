namespace LastChaos_ToolBoxNG
{
	public class PetFairySkillWhitelistEditor : Form
	{
		private const string TableName = "t_pet_fairy_skill_whitelist";

		private readonly Main pMain;
		private readonly DataGridView dgvAvailable = new();
		private readonly DataGridView dgvWhitelist = new();
		private readonly TextBox tbFilter = new();
		private readonly CheckBox cbSuggestedOnly = new();
		private readonly Button btnReload = new();
		private readonly Button btnAddSelected = new();
		private readonly Button btnAddSuggested = new();
		private readonly Button btnRemove = new();
		private readonly Button btnSave = new();
		private readonly Label lStatus = new();

		private DataTable? availableTable;
		private DataTable? whitelistTable;

		public PetFairySkillWhitelistEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "PetFairySkillWhitelistEditor";
			Text = "Pet Fairy Skill Whitelist";
			Width = 1260;
			Height = 760;
			StartPosition = FormStartPosition.CenterParent;

			BuildLayout();
			Load += async (_, _) => await LoadEditorAsync();
		}

		private void BuildLayout()
		{
			var root = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 4,
				Padding = new Padding(8)
			};
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			var info = new Label
			{
				Dock = DockStyle.Fill,
				Text = "Stored P1 pet passive skills are applied automatically. Use this whitelist for extra pet/APet buff skills that should become passive while the pet is stored in the Pet Fairy.",
				AutoSize = false,
				TextAlign = ContentAlignment.MiddleLeft
			};
			root.Controls.Add(info, 0, 0);

			var toolbar = new FlowLayoutPanel
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(toolbar, 0, 1);

			toolbar.Controls.Add(new Label
			{
				Text = "Filter:",
				AutoSize = true,
				Margin = new Padding(0, 8, 4, 0)
			});

			tbFilter.Width = 250;
			tbFilter.Margin = new Padding(0, 4, 12, 0);
			tbFilter.TextChanged += (_, _) => ApplyAvailableFilter();
			toolbar.Controls.Add(tbFilter);

			cbSuggestedOnly.Text = "Suggested only";
			cbSuggestedOnly.AutoSize = true;
			cbSuggestedOnly.Margin = new Padding(0, 8, 12, 0);
			cbSuggestedOnly.CheckedChanged += (_, _) => ApplyAvailableFilter();
			toolbar.Controls.Add(cbSuggestedOnly);

			ConfigureButton(btnReload, "Reload", async (_, _) => await LoadEditorAsync());
			ConfigureButton(btnAddSelected, "Add selected", async (_, _) => await AddSelectedAsync());
			ConfigureButton(btnAddSuggested, "Add suggested owner buffs", async (_, _) => await AddSuggestedAsync());
			ConfigureButton(btnRemove, "Remove selected whitelist rows", async (_, _) => await RemoveSelectedAsync());
			ConfigureButton(btnSave, "Save whitelist", async (_, _) => await SaveWhitelistAsync());

			toolbar.Controls.Add(btnReload);
			toolbar.Controls.Add(btnAddSelected);
			toolbar.Controls.Add(btnAddSuggested);
			toolbar.Controls.Add(btnRemove);
			toolbar.Controls.Add(btnSave);

			var split = new SplitContainer
			{
				Dock = DockStyle.Fill,
				Orientation = Orientation.Vertical,
				SplitterDistance = 700
			};
			root.Controls.Add(split, 0, 2);

			var left = BuildGridPanel("Available pet/APet skills", dgvAvailable);
			var right = BuildGridPanel("Whitelisted fairy passives", dgvWhitelist);
			split.Panel1.Controls.Add(left);
			split.Panel2.Controls.Add(right);

			ConfigureGrid(dgvAvailable, true);
			ConfigureGrid(dgvWhitelist, false);

			lStatus.Dock = DockStyle.Fill;
			lStatus.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lStatus, 0, 3);
		}

		private static void ConfigureButton(Button button, string text, EventHandler onClick)
		{
			button.Text = text;
			button.AutoSize = true;
			button.Margin = new Padding(0, 3, 6, 0);
			button.Click += onClick;
		}

		private static Control BuildGridPanel(string title, DataGridView grid)
		{
			var panel = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 2
			};
			panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
			panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

			panel.Controls.Add(new Label
			{
				Text = title,
				Dock = DockStyle.Fill,
				TextAlign = ContentAlignment.MiddleLeft,
				Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
			}, 0, 0);
			panel.Controls.Add(grid, 0, 1);
			return panel;
		}

		private static void ConfigureGrid(DataGridView grid, bool readOnly)
		{
			grid.Dock = DockStyle.Fill;
			grid.AllowUserToAddRows = false;
			grid.AllowUserToDeleteRows = false;
			grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			grid.MultiSelect = true;
			grid.ReadOnly = readOnly;
			grid.RowHeadersVisible = false;
			grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading pet fairy whitelist...");

			bool ready = await Task.Run(EnsureWhitelistTable);
			if (!ready)
			{
				SetBusy(false, "Could not create or load the whitelist table.");
				return;
			}

			availableTable = await Task.Run(LoadAvailableSkills);
			whitelistTable = await Task.Run(LoadWhitelist);

			dgvAvailable.DataSource = availableTable;
			dgvWhitelist.DataSource = whitelistTable;

			ApplyAvailableFilter();
			FormatAvailableGrid();
			FormatWhitelistGrid();

			SetBusy(false, $"Loaded {availableTable?.Rows.Count ?? 0} pet skills and {whitelistTable?.Rows.Count ?? 0} whitelist rows.");
		}

		private bool EnsureWhitelistTable()
		{
			string db = pMain.pSettings.DBData;
			string query =
				$"CREATE TABLE IF NOT EXISTS {db}.{TableName} (" +
				"a_skill_index INT(11) NOT NULL, " +
				"a_enable TINYINT(1) NOT NULL DEFAULT 1, " +
				"a_apply_rate INT(11) NOT NULL DEFAULT 100, " +
				"a_note VARCHAR(255) NOT NULL DEFAULT '', " +
				"PRIMARY KEY (a_skill_index)" +
				") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false);
		}

		private DataTable? LoadAvailableSkills()
		{
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT " +
				"s.a_index AS SkillID, " +
				"CASE s.a_job WHEN 10 THEN 'P1 Pet' WHEN 11 THEN 'APet' ELSE CAST(s.a_job AS CHAR) END AS PetType, " +
				"COALESCE(NULLIF(s.a_name_usa, ''), CONCAT('Skill ', s.a_index)) AS SkillName, " +
				"CASE s.a_type WHEN 5 THEN 'Passive' WHEN 6 THEN 'Pet Active' ELSE CONCAT('Type ', s.a_type) END AS SkillType, " +
				"s.a_targetType AS TargetType, " +
				"s.a_flag AS Flag, " +
				"IFNULL(sl.a_level, 1) AS Level, " +
				"IFNULL(sl.a_durtime, 0) AS Duration, " +
				"CASE " +
				"WHEN s.a_job = 10 AND s.a_type = 5 THEN 'Auto P1 passive' " +
				"WHEN (s.a_flag & 2) <> 0 AND s.a_targetType = 2 AND (m1.a_type = 0 OR m2.a_type = 0 OR m3.a_type = 0) THEN 'Yes' " +
				"ELSE '' END AS Suggested, " +
				"CASE WHEN sl.a_magicIndex1 > 0 THEN CONCAT(sl.a_magicIndex1, ' type ', IFNULL(m1.a_type, '?'), '/', IFNULL(m1.a_subtype, '?'), ' power ', IFNULL(ml1.a_power, '?')) ELSE '' END AS Magic1, " +
				"CASE WHEN sl.a_magicIndex2 > 0 THEN CONCAT(sl.a_magicIndex2, ' type ', IFNULL(m2.a_type, '?'), '/', IFNULL(m2.a_subtype, '?'), ' power ', IFNULL(ml2.a_power, '?')) ELSE '' END AS Magic2, " +
				"CASE WHEN sl.a_magicIndex3 > 0 THEN CONCAT(sl.a_magicIndex3, ' type ', IFNULL(m3.a_type, '?'), '/', IFNULL(m3.a_subtype, '?'), ' power ', IFNULL(ml3.a_power, '?')) ELSE '' END AS Magic3, " +
				"COALESCE(NULLIF(s.a_client_tooltip_usa, ''), '') AS Tooltip " +
				$"FROM {db}.t_skill s " +
				$"LEFT JOIN {db}.t_skillLevel sl ON sl.a_index = s.a_index AND sl.a_level = 1 " +
				$"LEFT JOIN {db}.t_magic m1 ON m1.a_index = sl.a_magicIndex1 " +
				$"LEFT JOIN {db}.t_magicLevel ml1 ON ml1.a_index = sl.a_magicIndex1 AND ml1.a_level = sl.a_magicLevel1 " +
				$"LEFT JOIN {db}.t_magic m2 ON m2.a_index = sl.a_magicIndex2 " +
				$"LEFT JOIN {db}.t_magicLevel ml2 ON ml2.a_index = sl.a_magicIndex2 AND ml2.a_level = sl.a_magicLevel2 " +
				$"LEFT JOIN {db}.t_magic m3 ON m3.a_index = sl.a_magicIndex3 " +
				$"LEFT JOIN {db}.t_magicLevel ml3 ON ml3.a_index = sl.a_magicIndex3 AND ml3.a_level = sl.a_magicLevel3 " +
				"WHERE s.a_job IN (10, 11) " +
				"ORDER BY s.a_job, s.a_index;";

			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private DataTable? LoadWhitelist()
		{
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT " +
				"w.a_skill_index AS SkillID, " +
				"w.a_enable AS Enabled, " +
				"w.a_apply_rate AS ApplyRate, " +
				"w.a_note AS Note, " +
				"CASE s.a_job WHEN 10 THEN 'P1 Pet' WHEN 11 THEN 'APet' ELSE CAST(s.a_job AS CHAR) END AS PetType, " +
				"COALESCE(NULLIF(s.a_name_usa, ''), CONCAT('Skill ', w.a_skill_index)) AS SkillName, " +
				"CASE s.a_type WHEN 5 THEN 'Passive' WHEN 6 THEN 'Pet Active' ELSE CONCAT('Type ', s.a_type) END AS SkillType, " +
				"s.a_targetType AS TargetType, " +
				"s.a_flag AS Flag, " +
				"COALESCE(NULLIF(s.a_client_tooltip_usa, ''), '') AS Tooltip " +
				$"FROM {db}.{TableName} w " +
				$"LEFT JOIN {db}.t_skill s ON s.a_index = w.a_skill_index " +
				"ORDER BY w.a_skill_index;";

			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private async Task AddSelectedAsync()
		{
			List<int> skillIds = GetSelectedSkillIds(dgvAvailable);
			if (skillIds.Count == 0)
			{
				SetStatus("Select one or more available skills first.");
				return;
			}

			string db = pMain.pSettings.DBData;
			string values = string.Join(", ", skillIds.Select(id => $"({id}, 1, 100, '')"));
			string query = $"INSERT INTO {db}.{TableName} (a_skill_index, a_enable, a_apply_rate, a_note) VALUES {values} ON DUPLICATE KEY UPDATE a_enable=1;";

			SetBusy(true, "Adding selected skills...");
			bool success = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			if (success)
				await LoadEditorAsync();
			else
				SetBusy(false, "Failed to add selected skills.");
		}

		private async Task AddSuggestedAsync()
		{
			string db = pMain.pSettings.DBData;
			string query =
				$"INSERT INTO {db}.{TableName} (a_skill_index, a_enable, a_apply_rate, a_note) " +
				"SELECT DISTINCT s.a_index, 1, 100, 'Suggested owner-target stat buff' " +
				$"FROM {db}.t_skill s " +
				$"LEFT JOIN {db}.t_skillLevel sl ON sl.a_index = s.a_index AND sl.a_level = 1 " +
				$"LEFT JOIN {db}.t_magic m1 ON m1.a_index = sl.a_magicIndex1 " +
				$"LEFT JOIN {db}.t_magic m2 ON m2.a_index = sl.a_magicIndex2 " +
				$"LEFT JOIN {db}.t_magic m3 ON m3.a_index = sl.a_magicIndex3 " +
				"WHERE s.a_job IN (10, 11) " +
				"AND (s.a_flag & 2) <> 0 " +
				"AND s.a_targetType = 2 " +
				"AND (m1.a_type = 0 OR m2.a_type = 0 OR m3.a_type = 0) " +
				"ON DUPLICATE KEY UPDATE a_enable=1;";

			SetBusy(true, "Adding suggested owner-target stat buffs...");
			bool success = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			if (success)
				await LoadEditorAsync();
			else
				SetBusy(false, "Failed to add suggested skills.");
		}

		private async Task RemoveSelectedAsync()
		{
			List<int> skillIds = GetSelectedSkillIds(dgvWhitelist);
			if (skillIds.Count == 0)
			{
				SetStatus("Select one or more whitelist rows first.");
				return;
			}

			string db = pMain.pSettings.DBData;
			string query = $"DELETE FROM {db}.{TableName} WHERE a_skill_index IN ({string.Join(", ", skillIds)});";

			SetBusy(true, "Removing whitelist rows...");
			bool success = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			if (success)
				await LoadEditorAsync();
			else
				SetBusy(false, "Failed to remove whitelist rows.");
		}

		private async Task SaveWhitelistAsync()
		{
			dgvWhitelist.EndEdit();

			string db = pMain.pSettings.DBData;
			var query = new StringBuilder();
			query.Append("START TRANSACTION;");
			query.Append($"DELETE FROM {db}.{TableName};");

			foreach (DataGridViewRow row in dgvWhitelist.Rows)
			{
				int skillId = ToInt(row.Cells["SkillID"].Value);
				if (skillId <= 0)
					continue;

				int enabled = ToInt(row.Cells["Enabled"].Value, 1) == 0 ? 0 : 1;
				int applyRate = Math.Clamp(ToInt(row.Cells["ApplyRate"].Value, 100), 0, 1000);
				string note = Convert.ToString(row.Cells["Note"].Value) ?? string.Empty;
				query.Append($"INSERT INTO {db}.{TableName} (a_skill_index, a_enable, a_apply_rate, a_note) VALUES ({skillId}, {enabled}, {applyRate}, '{pMain.EscapeChars(note)}');");
			}

			query.Append("COMMIT;");

			SetBusy(true, "Saving whitelist...");
			bool success = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query.ToString(), out long _, false));
			if (success)
				await LoadEditorAsync();
			else
				SetBusy(false, "Failed to save whitelist.");
		}

		private static List<int> GetSelectedSkillIds(DataGridView grid)
		{
			return grid.SelectedRows
				.Cast<DataGridViewRow>()
				.Select(row => ToInt(row.Cells["SkillID"].Value))
				.Where(id => id > 0)
				.Distinct()
				.OrderBy(id => id)
				.ToList();
		}

		private void ApplyAvailableFilter()
		{
			if (availableTable == null)
				return;

			var filters = new List<string>();
			string text = tbFilter.Text.Trim();
			if (!string.IsNullOrWhiteSpace(text))
			{
				string filter = EscapeRowFilterLike(text);
				filters.Add($"(Convert(SkillID, 'System.String') LIKE '%{filter}%' OR SkillName LIKE '%{filter}%' OR Tooltip LIKE '%{filter}%')");
			}

			if (cbSuggestedOnly.Checked)
				filters.Add("Suggested <> ''");

			availableTable.DefaultView.RowFilter = string.Join(" AND ", filters);
		}

		private static string EscapeRowFilterLike(string value)
		{
			return value
				.Replace("'", "''")
				.Replace("[", "[[]")
				.Replace("%", "[%]")
				.Replace("*", "[*]");
		}

		private void FormatAvailableGrid()
		{
			if (dgvAvailable.Columns.Count == 0)
				return;

			SetWidth(dgvAvailable, "SkillID", 70);
			SetWidth(dgvAvailable, "PetType", 70);
			SetWidth(dgvAvailable, "SkillName", 160);
			SetWidth(dgvAvailable, "SkillType", 90);
			SetWidth(dgvAvailable, "Suggested", 125);
			SetWidth(dgvAvailable, "Tooltip", 280, DataGridViewAutoSizeColumnMode.Fill);
		}

		private void FormatWhitelistGrid()
		{
			if (dgvWhitelist.Columns.Count == 0)
				return;

			foreach (DataGridViewColumn column in dgvWhitelist.Columns)
				column.ReadOnly = true;

			if (dgvWhitelist.Columns.Contains("Enabled"))
				dgvWhitelist.Columns["Enabled"].ReadOnly = false;
			if (dgvWhitelist.Columns.Contains("ApplyRate"))
				dgvWhitelist.Columns["ApplyRate"].ReadOnly = false;
			if (dgvWhitelist.Columns.Contains("Note"))
				dgvWhitelist.Columns["Note"].ReadOnly = false;

			SetWidth(dgvWhitelist, "SkillID", 70);
			SetWidth(dgvWhitelist, "Enabled", 70);
			SetWidth(dgvWhitelist, "ApplyRate", 80);
			SetWidth(dgvWhitelist, "Note", 180);
			SetWidth(dgvWhitelist, "SkillName", 160);
			SetWidth(dgvWhitelist, "Tooltip", 260, DataGridViewAutoSizeColumnMode.Fill);
		}

		private static void SetWidth(DataGridView grid, string columnName, int width, DataGridViewAutoSizeColumnMode? mode = null)
		{
			if (!grid.Columns.Contains(columnName))
				return;

			grid.Columns[columnName].Width = width;
			if (mode.HasValue)
				grid.Columns[columnName].AutoSizeMode = mode.Value;
		}

		private static int ToInt(object? value, int fallback = 0)
		{
			if (value == null || value == DBNull.Value)
				return fallback;

			return int.TryParse(Convert.ToString(value), out int result) ? result : fallback;
		}

		private void SetBusy(bool busy, string status)
		{
			btnReload.Enabled = !busy;
			btnAddSelected.Enabled = !busy;
			btnAddSuggested.Enabled = !busy;
			btnRemove.Enabled = !busy;
			btnSave.Enabled = !busy;
			UseWaitCursor = busy;
			SetStatus(status);
		}

		private void SetStatus(string status)
		{
			lStatus.Text = status;
		}
	}
}
