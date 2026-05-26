namespace LastChaos_ToolBoxNG
{
	public class PetBuffEditor : Form
	{
		private readonly Main pMain;
		private readonly DataGridView dgvSkills = new();
		private readonly DataGridView dgvLevels = new();
		private readonly TextBox tbFilter = new();
		private readonly CheckBox cbSuggestedOnly = new();
		private readonly Button btnReload = new();
		private readonly Button btnLoadSelected = new();
		private readonly Button btnAddLevel = new();
		private readonly Button btnRemoveLevel = new();
		private readonly Button btnSave = new();
		private readonly Label lblSelected = new();
		private readonly Label lblStatus = new();

		private DataTable? skillsTable;
		private DataTable? skillLevelsTable;
		private List<string> skillLevelDbColumns = [];
		private int selectedSkillId;
		private string selectedSkillName = string.Empty;

		public PetBuffEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "PetBuffEditor";
			Text = "Pet Buff Editor";
			MinimumSize = new Size(1240, 700);
			Size = new Size(1520, 820);
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
				RowCount = 4,
				Padding = new Padding(8)
			};
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			FlowLayoutPanel toolbar = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(toolbar, 0, 0);

			toolbar.Controls.Add(new Label
			{
				Text = "Filter:",
				AutoSize = true,
				Margin = new Padding(0, 10, 5, 0)
			});

			tbFilter.Width = 260;
			tbFilter.Margin = new Padding(0, 6, 12, 0);
			tbFilter.TextChanged += (_, _) => ApplySkillFilter();
			toolbar.Controls.Add(tbFilter);

			cbSuggestedOnly.Text = "Suggested only";
			cbSuggestedOnly.AutoSize = true;
			cbSuggestedOnly.Margin = new Padding(0, 10, 12, 0);
			cbSuggestedOnly.CheckedChanged += (_, _) => ApplySkillFilter();
			toolbar.Controls.Add(cbSuggestedOnly);

			ConfigureButton(btnReload, "Reload", async (_, _) => await LoadEditorAsync());
			ConfigureButton(btnLoadSelected, "Load selected skill", async (_, _) => await LoadSelectedSkillAsync());
			ConfigureButton(btnAddLevel, "Add level", (_, _) => AddLevel());
			ConfigureButton(btnRemoveLevel, "Remove highest level", (_, _) => RemoveHighestLevel());
			ConfigureButton(btnSave, "Save + export Skills.lod", async (_, _) => await SaveSelectedSkillAsync());
			toolbar.Controls.AddRange([btnReload, btnLoadSelected, btnAddLevel, btnRemoveLevel, btnSave]);

			lblSelected.Dock = DockStyle.Fill;
			lblSelected.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblSelected, 0, 1);

			SplitContainer split = new()
			{
				Dock = DockStyle.Fill,
				Orientation = Orientation.Vertical,
				SplitterDistance = 560
			};
			root.Controls.Add(split, 0, 2);

			split.Panel1.Controls.Add(BuildGridPanel("Pet/APet skills", dgvSkills));
			split.Panel2.Controls.Add(BuildGridPanel("Selected skill levels and buff effects", dgvLevels));

			ConfigureSkillsGrid();
			ConfigureLevelsGrid();

			lblStatus.Dock = DockStyle.Fill;
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblStatus, 0, 3);
		}

		private static void ConfigureButton(Button button, string text, EventHandler handler)
		{
			button.Text = text;
			button.AutoSize = true;
			button.Margin = new Padding(0, 5, 8, 0);
			button.Click += handler;
		}

		private static Control BuildGridPanel(string title, DataGridView grid)
		{
			TableLayoutPanel panel = new()
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

		private void ConfigureSkillsGrid()
		{
			dgvSkills.Dock = DockStyle.Fill;
			dgvSkills.AllowUserToAddRows = false;
			dgvSkills.AllowUserToDeleteRows = false;
			dgvSkills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			dgvSkills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvSkills.MultiSelect = false;
			dgvSkills.ReadOnly = true;
			dgvSkills.RowHeadersVisible = false;
			dgvSkills.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			dgvSkills.CellDoubleClick += async (_, _) => await LoadSelectedSkillAsync();
		}

		private void ConfigureLevelsGrid()
		{
			dgvLevels.Dock = DockStyle.Fill;
			dgvLevels.AutoGenerateColumns = false;
			dgvLevels.AllowUserToAddRows = false;
			dgvLevels.AllowUserToDeleteRows = false;
			dgvLevels.RowHeadersVisible = false;
			dgvLevels.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvLevels.MultiSelect = false;
			dgvLevels.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			dgvLevels.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			dgvLevels.DataError += (_, e) => e.ThrowException = false;

			dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_level", HeaderText = "Level", ReadOnly = true });
			dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_learnLevel", HeaderText = "Pet level req" });

			for (int slot = 1; slot <= 3; slot++)
			{
				dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = $"a_magicIndex{slot}", HeaderText = $"M{slot} magic", ReadOnly = true });
				dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = $"Magic{slot}Name", HeaderText = $"M{slot} type", ReadOnly = true });
				dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = $"a_magicLevel{slot}", HeaderText = $"M{slot} level" });
				dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = $"Magic{slot}Power", HeaderText = $"M{slot} power" });
				dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = $"Magic{slot}HitRate", HeaderText = $"M{slot} hit" });
			}
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading pet buff skills...");
			skillsTable = await Task.Run(LoadSkills);
			dgvSkills.DataSource = skillsTable;
			ApplySkillFilter();
			FormatSkillsGrid();

			selectedSkillId = 0;
			selectedSkillName = string.Empty;
			skillLevelsTable = null;
			dgvLevels.DataSource = null;
			lblSelected.Text = "Select a pet/APet skill, then load it.";
			SetBusy(false, $"Loaded {skillsTable?.Rows.Count ?? 0} pet/APet skills.");
		}

		private DataTable? LoadSkills()
		{
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT " +
				"s.a_index AS SkillID, " +
				"CASE s.a_job WHEN 10 THEN 'P1 Pet' WHEN 11 THEN 'APet' ELSE CAST(s.a_job AS CHAR) END AS PetType, " +
				"COALESCE(NULLIF(s.a_name_usa, ''), CONCAT('Skill ', s.a_index)) AS SkillName, " +
				"CASE s.a_type WHEN 4 THEN 'Command' WHEN 5 THEN 'Passive' WHEN 6 THEN 'Pet Active' ELSE CONCAT('Type ', s.a_type) END AS SkillType, " +
				"s.a_maxLevel AS MaxLevel, " +
				"(SELECT COUNT(*) FROM " + db + ".t_skillLevel slc WHERE slc.a_index=s.a_index) AS LevelRows, " +
				"s.a_targetType AS TargetType, " +
				"s.a_flag AS Flag, " +
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

		private async Task LoadSelectedSkillAsync()
		{
			int skillId = GetSelectedSkillId();
			if (skillId <= 0)
			{
				SetStatus("Select a skill first.");
				return;
			}

			string skillName = GetSelectedSkillName();
			SetBusy(true, $"Loading skill {skillId}...");
			DataTable? table = await Task.Run(() => LoadSkillLevels(skillId));
			if (table == null)
			{
				SetBusy(false, $"Failed to load skill {skillId}.");
				return;
			}

			selectedSkillId = skillId;
			selectedSkillName = skillName;
			skillLevelsTable = table;
			skillLevelDbColumns = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
			AddEffectColumns(table);
			await Task.Run(() => FillEffectColumns(table));

			dgvLevels.DataSource = table;
			FormatLevelsGrid();
			SetSelectedLabel();
			SetBusy(false, $"Loaded {table.Rows.Count} levels for {selectedSkillName}.");
		}

		private DataTable? LoadSkillLevels(int skillId)
		{
			string db = pMain.pSettings.DBData;
			return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT * FROM {db}.t_skillLevel WHERE a_index={skillId} ORDER BY a_level;", false);
		}

		private void AddEffectColumns(DataTable table)
		{
			for (int slot = 1; slot <= 3; slot++)
			{
				AddColumn(table, $"Magic{slot}Name", typeof(string));
				AddColumn(table, $"Magic{slot}Power", typeof(int));
				AddColumn(table, $"Magic{slot}HitRate", typeof(int));
			}
		}

		private static void AddColumn(DataTable table, string columnName, Type type)
		{
			if (!table.Columns.Contains(columnName))
				table.Columns.Add(columnName, type);
		}

		private void FillEffectColumns(DataTable table)
		{
			foreach (DataRow row in table.Rows)
			{
				for (int slot = 1; slot <= 3; slot++)
				{
					int magicIndex = GetInt(row, $"a_magicIndex{slot}");
					int magicLevel = GetInt(row, $"a_magicLevel{slot}");
					if (magicIndex <= 0 || magicLevel <= 0)
					{
						row[$"Magic{slot}Name"] = "";
						row[$"Magic{slot}Power"] = 0;
						row[$"Magic{slot}HitRate"] = 0;
						continue;
					}

					DataTable? magic = pMain.QuerySelect(
						pMain.pSettings.DBCharset,
						"SELECT " +
						"CONCAT('type ', IFNULL(m.a_type, '?'), '/', IFNULL(m.a_subtype, '?')) AS MagicName, " +
						"IFNULL(ml.a_power, 0) AS Power, " +
						"IFNULL(ml.a_hitrate, 0) AS HitRate " +
						$"FROM {pMain.pSettings.DBData}.t_magic m " +
						$"LEFT JOIN {pMain.pSettings.DBData}.t_magicLevel ml ON ml.a_index=m.a_index AND ml.a_level={magicLevel} " +
						$"WHERE m.a_index={magicIndex} LIMIT 1;",
						false);

					if (magic?.Rows.Count > 0)
					{
						row[$"Magic{slot}Name"] = magic.Rows[0]["MagicName"];
						row[$"Magic{slot}Power"] = ToInt(magic.Rows[0]["Power"]);
						row[$"Magic{slot}HitRate"] = ToInt(magic.Rows[0]["HitRate"]);
					}
					else
					{
						row[$"Magic{slot}Name"] = "";
						row[$"Magic{slot}Power"] = 0;
						row[$"Magic{slot}HitRate"] = 0;
					}

					magic?.Dispose();
				}
			}
		}

		private void AddLevel()
		{
			if (selectedSkillId <= 0 || skillLevelsTable == null)
			{
				SetStatus("Load a skill before adding a level.");
				return;
			}

			if (skillLevelsTable.Rows.Count == 0)
			{
				SetStatus("This skill has no base level row to clone.");
				return;
			}

			Validate();
			dgvLevels.EndEdit();

			DataRow source = skillLevelsTable.Rows.Cast<DataRow>()
				.OrderBy(row => GetInt(row, "a_level"))
				.Last();
			DataRow row = skillLevelsTable.NewRow();
			foreach (DataColumn column in skillLevelsTable.Columns)
				row[column.ColumnName] = source[column.ColumnName];

			row["a_index"] = selectedSkillId;
			row["a_level"] = skillLevelsTable.Rows.Count + 1;
			skillLevelsTable.Rows.Add(row);
			RenumberLevels();
			SetSelectedLabel();
			SetStatus("Cloned the highest level. Edit the new row, then save.");
		}

		private void RemoveHighestLevel()
		{
			if (selectedSkillId <= 0 || skillLevelsTable == null || skillLevelsTable.Rows.Count == 0)
			{
				SetStatus("Load a skill before removing a level.");
				return;
			}

			if (skillLevelsTable.Rows.Count <= 1)
			{
				SetStatus("A skill needs at least one level row.");
				return;
			}

			Validate();
			dgvLevels.EndEdit();

			DataRow row = skillLevelsTable.Rows.Cast<DataRow>()
				.OrderBy(r => GetInt(r, "a_level"))
				.Last();
			skillLevelsTable.Rows.Remove(row);
			RenumberLevels();
			SetSelectedLabel();
			SetStatus("Removed the highest level. Save to update the database.");
		}

		private async Task SaveSelectedSkillAsync()
		{
			if (selectedSkillId <= 0 || skillLevelsTable == null)
			{
				SetStatus("Load a skill before saving.");
				return;
			}

			Validate();
			dgvLevels.EndEdit();
			RenumberLevels();
			NormalizeRows();

			string query = BuildSaveQuery();
			SetBusy(true, $"Saving {selectedSkillName}...");
			bool success = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			if (!success)
			{
				SetBusy(false, "Save failed. Check the Toolbox console.");
				return;
			}

			string clientPath = pMain.pSettings.ClientPath;
			if (string.IsNullOrWhiteSpace(clientPath))
			{
				SetBusy(false, "Saved DB. ClientPath is empty, so Skills.lod was not exported.");
				return;
			}

			try
			{
				using Exporter exporter = new(pMain);
				await exporter.ExportSkillsLodAsync(true);
			}
			catch (Exception ex)
			{
				SetBusy(false, $"Saved DB, but Skills.lod export failed: {ex.Message}");
				return;
			}

			await RefreshAfterSaveAsync(selectedSkillId);
			SetBusy(false, $"Saved {selectedSkillName} and exported {Path.Combine(clientPath, "Data", "Skills.lod")}.");
		}

		private async Task RefreshAfterSaveAsync(int skillId)
		{
			skillsTable = await Task.Run(LoadSkills);
			dgvSkills.DataSource = skillsTable;
			ApplySkillFilter();
			FormatSkillsGrid();

			SelectSkillInGrid(skillId);

			DataTable? table = await Task.Run(() => LoadSkillLevels(skillId));
			if (table == null)
				return;

			skillLevelsTable = table;
			skillLevelDbColumns = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
			AddEffectColumns(table);
			await Task.Run(() => FillEffectColumns(table));
			dgvLevels.DataSource = table;
			FormatLevelsGrid();
			SetSelectedLabel();
		}

		private string BuildSaveQuery()
		{
			var query = new StringBuilder();
			string db = pMain.pSettings.DBData;
			int maxLevel = skillLevelsTable?.Rows.Count ?? 0;

			query.Append("START TRANSACTION;");
			query.Append($"UPDATE {db}.t_skill SET a_maxLevel={maxLevel} WHERE a_index={selectedSkillId};");
			query.Append($"DELETE FROM {db}.t_skillLevel WHERE a_index={selectedSkillId};");

			if (skillLevelsTable != null && skillLevelsTable.Rows.Count > 0)
			{
				string columns = string.Join(", ", skillLevelDbColumns);
				List<string> rows = [];
				foreach (DataRow row in skillLevelsTable.Rows)
				{
					string values = string.Join(", ", skillLevelDbColumns.Select(column => SqlValue(row[column])));
					rows.Add($"({values})");
				}

				query.Append($"INSERT INTO {db}.t_skillLevel ({columns}) VALUES {string.Join(", ", rows)};");

				foreach (DataRow row in skillLevelsTable.Rows)
				{
					for (int slot = 1; slot <= 3; slot++)
					{
						int magicIndex = GetInt(row, $"a_magicIndex{slot}");
						int magicLevel = GetInt(row, $"a_magicLevel{slot}");
						if (magicIndex <= 0 || magicLevel <= 0)
							continue;

						int power = GetInt(row, $"Magic{slot}Power");
						int hitRate = GetInt(row, $"Magic{slot}HitRate");
						query.Append($"INSERT INTO {db}.t_magicLevel (a_index, a_level, a_power, a_hitrate) VALUES ({magicIndex}, {magicLevel}, {power}, {hitRate}) ON DUPLICATE KEY UPDATE a_power=VALUES(a_power), a_hitrate=VALUES(a_hitrate);");
					}
				}
			}

			query.Append("COMMIT;");
			return query.ToString();
		}

		private void NormalizeRows()
		{
			if (skillLevelsTable == null)
				return;

			foreach (DataRow row in skillLevelsTable.Rows)
			{
				row["a_index"] = selectedSkillId;
				row["a_learnLevel"] = Math.Max(1, GetInt(row, "a_learnLevel", 1));

				for (int slot = 1; slot <= 3; slot++)
				{
					string magicLevelColumn = $"a_magicLevel{slot}";
					if (skillLevelsTable.Columns.Contains(magicLevelColumn))
						row[magicLevelColumn] = Math.Clamp(GetInt(row, magicLevelColumn), 0, 255);
				}
			}
		}

		private void RenumberLevels()
		{
			if (skillLevelsTable == null)
				return;

			int level = 1;
			foreach (DataRow row in skillLevelsTable.Rows.Cast<DataRow>().OrderBy(row => GetInt(row, "a_level")).ToList())
				row["a_level"] = level++;
		}

		private void ApplySkillFilter()
		{
			if (skillsTable == null)
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

			skillsTable.DefaultView.RowFilter = string.Join(" AND ", filters);
		}

		private static string EscapeRowFilterLike(string value)
		{
			return value
				.Replace("'", "''")
				.Replace("[", "[[]")
				.Replace("%", "[%]")
				.Replace("*", "[*]");
		}

		private int GetSelectedSkillId()
		{
			if (dgvSkills.CurrentRow?.DataBoundItem is DataRowView view)
				return GetInt(view.Row, "SkillID");
			return 0;
		}

		private string GetSelectedSkillName()
		{
			if (dgvSkills.CurrentRow?.DataBoundItem is DataRowView view)
				return GetString(view.Row, "SkillName");
			return string.Empty;
		}

		private void SelectSkillInGrid(int skillId)
		{
			if (skillsTable == null)
				return;

			foreach (DataGridViewRow row in dgvSkills.Rows)
			{
				if (ToInt(row.Cells["SkillID"].Value) != skillId)
					continue;

				row.Selected = true;
				dgvSkills.CurrentCell = row.Cells["SkillID"];
				return;
			}
		}

		private void SetSelectedLabel()
		{
			if (selectedSkillId <= 0 || skillLevelsTable == null)
			{
				lblSelected.Text = "Select a pet/APet skill, then load it.";
				return;
			}

			lblSelected.Text = $"{selectedSkillId} - {selectedSkillName} | max learnable level: {skillLevelsTable.Rows.Count}. Magic power/hit edits update the referenced t_magicLevel row.";
		}

		private void FormatSkillsGrid()
		{
			if (dgvSkills.Columns.Count == 0)
				return;

			SetWidth(dgvSkills, "SkillID", 70);
			SetWidth(dgvSkills, "PetType", 70);
			SetWidth(dgvSkills, "SkillName", 170);
			SetWidth(dgvSkills, "SkillType", 90);
			SetWidth(dgvSkills, "MaxLevel", 70);
			SetWidth(dgvSkills, "LevelRows", 76);
			SetWidth(dgvSkills, "Suggested", 125);
			SetWidth(dgvSkills, "Tooltip", 240, DataGridViewAutoSizeColumnMode.Fill);
		}

		private void FormatLevelsGrid()
		{
			if (dgvLevels.Columns.Count == 0)
				return;

			SetWidth(dgvLevels, "a_level", 55);
			SetWidth(dgvLevels, "a_learnLevel", 95);

			for (int slot = 1; slot <= 3; slot++)
			{
				SetWidth(dgvLevels, $"a_magicIndex{slot}", 75);
				SetWidth(dgvLevels, $"Magic{slot}Name", 88);
				SetWidth(dgvLevels, $"a_magicLevel{slot}", 75);
				SetWidth(dgvLevels, $"Magic{slot}Power", 75);
				SetWidth(dgvLevels, $"Magic{slot}HitRate", 65);
			}
		}

		private static void SetWidth(DataGridView grid, string columnName, int width, DataGridViewAutoSizeColumnMode? mode = null)
		{
			if (!grid.Columns.Contains(columnName))
				return;

			grid.Columns[columnName].Width = width;
			if (mode.HasValue)
				grid.Columns[columnName].AutoSizeMode = mode.Value;
		}

		private string SqlValue(object? value)
		{
			if (value == null || value == DBNull.Value)
				return "'0'";

			string text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
			return $"'{pMain.EscapeChars(text)}'";
		}

		private static int GetInt(DataRow row, string column, int fallback = 0)
		{
			if (!row.Table.Columns.Contains(column))
				return fallback;
			return ToInt(row[column], fallback);
		}

		private static int ToInt(object? value, int fallback = 0)
		{
			if (value == null || value == DBNull.Value)
				return fallback;
			return int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out int result) ? result : fallback;
		}

		private static string GetString(DataRow row, string column)
		{
			if (!row.Table.Columns.Contains(column))
				return string.Empty;

			object value = row[column];
			return value == DBNull.Value ? string.Empty : value.ToString() ?? string.Empty;
		}

		private void SetBusy(bool busy, string status)
		{
			btnReload.Enabled = !busy;
			btnLoadSelected.Enabled = !busy;
			btnAddLevel.Enabled = !busy && selectedSkillId > 0;
			btnRemoveLevel.Enabled = !busy && selectedSkillId > 0;
			btnSave.Enabled = !busy && selectedSkillId > 0;
			UseWaitCursor = busy;
			SetStatus(status);
		}

		private void SetStatus(string status)
		{
			lblStatus.Text = status;
		}
	}
}
