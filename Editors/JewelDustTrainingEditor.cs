namespace LastChaos_ToolBoxNG
{
	public class JewelDustTrainingEditor : Form
	{
		private const int JewelDustTrainingType = 14;
		private const int MaxLevels = Defs.SSKILL_MAX_LEVEL;
		private const int DefaultIconTex = 0;
		private const int DefaultIconRow = 4;
		private const int DefaultIconCol = 4;

		private readonly Main pMain;
		private readonly ComboBox cbNpc = new();
		private readonly Button btnReload = new();
		private readonly Button btnAssignNpc = new();
		private readonly Button btnClearNpc = new();
		private readonly Button btnAddBoost = new();
		private readonly Button btnDisableBoost = new();
		private readonly Button btnSave = new();
		private readonly DataGridView grid = new();
		private readonly Label lblStatus = new();

		private DataTable? boostTable;
		private int nextSkillIndex;

		public JewelDustTrainingEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "JewelDustTrainingEditor";
			Text = "Jewel Dust Upgrade Chain Editor";
			MinimumSize = new Size(1180, 680);
			Size = new Size(1480, 780);
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
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			root.Controls.Add(new Label
			{
				Dock = DockStyle.Fill,
				Text = "Edits Jewel Dust upgrade chains stored as special skill type JEWEL_DUST_TRAINING. For the same NPC as gold training, keep the NPC assigned in Gold Stat Training Editor; that combined NPC shows both choices. Assigning here makes a dust-only NPC.",
				TextAlign = ContentAlignment.MiddleLeft
			}, 0, 0);

			FlowLayoutPanel npcRow = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(npcRow, 0, 1);

			npcRow.Controls.Add(new Label
			{
				Text = "NPC:",
				AutoSize = true,
				Margin = new Padding(0, 9, 6, 0)
			});

			cbNpc.DropDownStyle = ComboBoxStyle.DropDownList;
			cbNpc.Width = 420;
			cbNpc.Margin = new Padding(0, 4, 10, 0);
			npcRow.Controls.Add(cbNpc);

			ConfigureButton(btnReload, "Reload", async (_, _) => await LoadEditorAsync());
			ConfigureButton(btnAssignNpc, "Assign selected NPC as dust-only", async (_, _) => await AssignSelectedNpcAsync());
			ConfigureButton(btnClearNpc, "Clear dust-only assignment", async (_, _) => await ClearSelectedNpcAsync());
			npcRow.Controls.AddRange([btnReload, btnAssignNpc, btnClearNpc]);

			ConfigureGrid();
			root.Controls.Add(grid, 0, 2);

			FlowLayoutPanel actionRow = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(actionRow, 0, 3);

			ConfigureButton(btnAddBoost, "Add chain segment", (_, _) => AddBoostRow());
			ConfigureButton(btnDisableBoost, "Disable selected segment", (_, _) => DisableSelectedBoost());
			ConfigureButton(btnSave, "Save upgrade chains", async (_, _) => await SaveBoostsAsync());
			actionRow.Controls.AddRange([btnAddBoost, btnDisableBoost, btnSave]);

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

			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_index", HeaderText = "Chain row ID", ReadOnly = true });
			grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "a_enable", HeaderText = "Enabled", TrueValue = 1, FalseValue = 0 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_name", HeaderText = "Segment name", Width = 180 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_desc", HeaderText = "Segment description", Width = 260 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_max_level", HeaderText = "Segment levels" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_need_sskill", HeaderText = "Previous chain ID" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_need_sskill_level", HeaderText = "Previous level" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_texture_id", HeaderText = "Icon tex" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_texture_row", HeaderText = "Icon row" });
			grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "a_texture_col", HeaderText = "Icon col" });

			for (int level = 0; level < MaxLevels; level++)
			{
				int displayLevel = level + 1;
				grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = $"a_level{level}_need_level", HeaderText = $"L{displayLevel} character level" });
				grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = $"a_level{level}_need_sp", HeaderText = $"L{displayLevel} dust cost" });
				grid.Columns.Add(new DataGridViewComboBoxColumn
				{
					DataPropertyName = $"a_level{level}_num0",
					HeaderText = $"L{displayLevel} effect",
					DataSource = BuildOptionChoices(),
					DisplayMember = "Name",
					ValueMember = "Id",
					ValueType = typeof(int)
				});
				grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = $"a_level{level}_num1", HeaderText = $"L{displayLevel} effect amount" });
			}
		}

		private static List<OptionChoice> BuildOptionChoices()
		{
			return
			[
				new(0, "STR"),
				new(1, "DEX"),
				new(2, "INT"),
				new(3, "CON"),
				new(4, "Max HP"),
				new(5, "Max MP"),
				new(6, "Physical attack"),
				new(7, "Melee attack"),
				new(8, "Range attack"),
				new(9, "Melee hit"),
				new(10, "Range hit"),
				new(11, "Physical defense"),
				new(12, "Melee defense"),
				new(13, "Range defense"),
				new(14, "Melee avoid"),
				new(15, "Range avoid"),
				new(16, "Magic attack"),
				new(17, "Magic hit"),
				new(18, "Magic defense"),
				new(19, "Magic avoid"),
				new(20, "All attack"),
				new(21, "All hit"),
				new(22, "All defense"),
				new(23, "All avoid")
			];
		}

		private async Task LoadEditorAsync()
		{
			SetBusy(true, "Loading Jewel Dust training data...");

			DataTable? npcs = await Task.Run(LoadNpcs);
			boostTable = await Task.Run(LoadBoosts);
			nextSkillIndex = await Task.Run(GetNextSkillIndex);

			cbNpc.DataSource = npcs;
			cbNpc.DisplayMember = "DisplayName";
			cbNpc.ValueMember = "NpcId";

			grid.DataSource = boostTable;
			SetBusy(false, $"Loaded {boostTable?.Rows.Count ?? 0} Jewel Dust upgrade-chain segments. Save, export SPECIALSKILLS/MOBS, then restart server and client.");
		}

		private DataTable? LoadNpcs()
		{
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT " +
				"a_index AS NpcId, " +
				"CONCAT(a_index, ' - ', COALESCE(NULLIF(a_name, ''), CONCAT('NPC ', a_index)), CASE WHEN a_sskill_master = 14 THEN ' [JEWEL_DUST_TRAINING]' ELSE '' END) AS DisplayName " +
				$"FROM {db}.t_npc WHERE a_enable=1 ORDER BY a_index;";
			return pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
		}

		private DataTable? LoadBoosts()
		{
			string db = pMain.pSettings.DBData;
			List<string> columns =
			[
				"a_index",
				"a_enable",
				"COALESCE(NULLIF(a_name, ''), a_name_usa) AS a_name",
				"COALESCE(NULLIF(a_desc, ''), a_desc_usa) AS a_desc",
				"a_max_level",
				"a_need_sskill",
				"a_need_sskill_level",
				"a_texture_id",
				"a_texture_row",
				"a_texture_col"
			];

			for (int level = 0; level < MaxLevels; level++)
			{
				columns.Add($"a_level{level}_need_level");
				columns.Add($"a_level{level}_need_sp");
				columns.Add($"a_level{level}_num0");
				columns.Add($"a_level{level}_num1");
			}

			return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT {string.Join(", ", columns)} FROM {db}.t_special_skill WHERE a_type={JewelDustTrainingType} ORDER BY a_index;", false);
		}

		private int GetNextSkillIndex()
		{
			string db = pMain.pSettings.DBData;
			DataTable? table = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT IFNULL(MAX(a_index), 0) + 1 AS NextId FROM {db}.t_special_skill;", false);
			if (table?.Rows.Count > 0)
				return Convert.ToInt32(table.Rows[0]["NextId"]);
			return 1;
		}

		private async Task AssignSelectedNpcAsync()
		{
			if (cbNpc.SelectedValue == null)
				return;

			int npcId = Convert.ToInt32(cbNpc.SelectedValue);
			string query = $"UPDATE {pMain.pSettings.DBData}.t_npc SET a_sskill_master={JewelDustTrainingType} WHERE a_index={npcId};";
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			await LoadEditorAsync();
			SetBusy(false, ok ? $"NPC {npcId} now opens the Jewel Dust-only training UI. To use the same NPC as gold training, assign it in Gold Stat Training Editor instead." : "Failed to assign NPC.");
		}

		private async Task ClearSelectedNpcAsync()
		{
			if (cbNpc.SelectedValue == null)
				return;

			int npcId = Convert.ToInt32(cbNpc.SelectedValue);
			string query = $"UPDATE {pMain.pSettings.DBData}.t_npc SET a_sskill_master=-1 WHERE a_index={npcId} AND a_sskill_master={JewelDustTrainingType};";
			bool ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
			await LoadEditorAsync();
			SetBusy(false, ok ? $"NPC {npcId} cleared from Jewel Dust training." : "Failed to clear NPC.");
		}

		private void AddBoostRow()
		{
			if (boostTable == null)
				return;

			DataRow row = boostTable.NewRow();
			row["a_index"] = nextSkillIndex++;
			row["a_enable"] = 1;
			row["a_name"] = "New Jewel Dust boost";
			row["a_desc"] = "Permanent Jewel Dust training boost.";
			row["a_max_level"] = 1;
			row["a_need_sskill"] = -1;
			row["a_need_sskill_level"] = 0;
			row["a_texture_id"] = DefaultIconTex;
			row["a_texture_row"] = DefaultIconRow;
			row["a_texture_col"] = DefaultIconCol;

			for (int level = 0; level < MaxLevels; level++)
			{
				row[$"a_level{level}_need_level"] = 1;
				row[$"a_level{level}_need_sp"] = 0;
				row[$"a_level{level}_num0"] = 0;
				row[$"a_level{level}_num1"] = 1;
			}

			boostTable.Rows.Add(row);
			SetBusy(false, "Added a new Jewel Dust chain segment. Edit it, then save.");
		}

		private void DisableSelectedBoost()
		{
			if (grid.CurrentRow?.DataBoundItem is not DataRowView view)
				return;

			view.Row["a_enable"] = 0;
			SetBusy(false, "Selected chain segment marked disabled. Save to update the database.");
		}

		private async Task SaveBoostsAsync()
		{
			if (boostTable == null)
				return;

			Validate();
			grid.EndEdit();
			SetBusy(true, "Saving Jewel Dust training boosts...");

			bool ok = true;
			foreach (DataRow row in boostTable.Rows)
			{
				NormalizeBoostRow(row);
				string query = BuildSaveQuery(row);
				ok = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query, out long _, false));
				if (!ok)
					break;
			}

			await LoadEditorAsync();
			SetBusy(false, ok ? "Saved Jewel Dust upgrade chains. Export SPECIALSKILLS and MOBS, then restart server and client before testing." : "Save failed. Check the Toolbox console.");
		}

		private string BuildSaveQuery(DataRow row)
		{
			int boostId = GetInt(row, "a_index");
			string name = GetString(row, "a_name").Trim();
			if (string.IsNullOrWhiteSpace(name))
				name = $"Jewel Dust Boost {boostId}";

			string description = GetString(row, "a_desc").Trim();
			if (string.IsNullOrWhiteSpace(description))
				description = "Permanent Jewel Dust training boost.";

			List<(string Column, string Value)> values =
			[
				("a_index", boostId.ToString()),
				("a_enable", GetBoolInt(row, "a_enable").ToString()),
				("a_job", "-1"),
				("a_type", JewelDustTrainingType.ToString()),
				("a_name", SqlString(name)),
				("a_desc", SqlString(description)),
				("a_name_usa", SqlString(name)),
				("a_desc_usa", SqlString(description)),
				("a_max_level", Math.Clamp(GetInt(row, "a_max_level", 1), 1, MaxLevels).ToString()),
				("a_preference", "-1"),
				("a_need_sskill", GetInt(row, "a_need_sskill", -1).ToString()),
				("a_need_sskill_level", Math.Clamp(GetInt(row, "a_need_sskill_level"), 0, MaxLevels).ToString()),
				("a_texture_id", GetInt(row, "a_texture_id").ToString()),
				("a_texture_row", GetInt(row, "a_texture_row").ToString()),
				("a_texture_col", GetInt(row, "a_texture_col").ToString())
			];

			string locale = pMain.pSettings.WorkLocale.ToLowerInvariant();
			if (!string.IsNullOrWhiteSpace(locale) && locale != "usa")
			{
				values.Add(($"a_name_{locale}", SqlString(name)));
				values.Add(($"a_desc_{locale}", SqlString(description)));
			}

			for (int level = 0; level < MaxLevels; level++)
			{
				values.Add(($"a_level{level}_need_level", GetInt(row, $"a_level{level}_need_level", 1).ToString()));
				values.Add(($"a_level{level}_need_sp", Math.Max(0, GetInt(row, $"a_level{level}_need_sp")).ToString()));
				values.Add(($"a_level{level}_num0", GetInt(row, $"a_level{level}_num0").ToString()));
				values.Add(($"a_level{level}_num1", Math.Max(0, GetInt(row, $"a_level{level}_num1")).ToString()));
			}

			string columns = string.Join(", ", values.Select(v => v.Column));
			string vals = string.Join(", ", values.Select(v => v.Value));
			string updates = string.Join(", ", values.Where(v => v.Column != "a_index").Select(v => $"{v.Column}=VALUES({v.Column})"));
			return $"INSERT INTO {pMain.pSettings.DBData}.t_special_skill ({columns}) VALUES ({vals}) ON DUPLICATE KEY UPDATE {updates};";
		}

		private void NormalizeBoostRow(DataRow row)
		{
			int boostId = GetInt(row, "a_index");
			if (string.IsNullOrWhiteSpace(GetString(row, "a_name")))
				row["a_name"] = $"Jewel Dust Boost {boostId}";

			if (string.IsNullOrWhiteSpace(GetString(row, "a_desc")))
				row["a_desc"] = "Permanent Jewel Dust training boost.";

			if (GetInt(row, "a_texture_id") == 0 &&
				GetInt(row, "a_texture_row") == 0 &&
				GetInt(row, "a_texture_col") == 0)
			{
				row["a_texture_id"] = DefaultIconTex;
				row["a_texture_row"] = DefaultIconRow;
				row["a_texture_col"] = DefaultIconCol;
			}

			row["a_max_level"] = Math.Clamp(GetInt(row, "a_max_level", 1), 1, MaxLevels);
			row["a_need_sskill"] = Math.Clamp(GetInt(row, "a_need_sskill", -1), -1, 127);
			row["a_need_sskill_level"] = Math.Clamp(GetInt(row, "a_need_sskill_level"), 0, MaxLevels);

			for (int level = 0; level < MaxLevels; level++)
			{
				row[$"a_level{level}_need_level"] = Math.Max(1, GetInt(row, $"a_level{level}_need_level", 1));
				row[$"a_level{level}_need_sp"] = Math.Max(0, GetInt(row, $"a_level{level}_need_sp"));
				row[$"a_level{level}_num1"] = Math.Max(0, GetInt(row, $"a_level{level}_num1"));
			}
		}

		private string SqlString(string value)
		{
			return $"'{pMain.EscapeChars(value)}'";
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
			btnAssignNpc.Enabled = !busy;
			btnClearNpc.Enabled = !busy;
			btnAddBoost.Enabled = !busy;
			btnDisableBoost.Enabled = !busy;
			btnSave.Enabled = !busy;
			lblStatus.Text = message;
		}

		private sealed record OptionChoice(int Id, string Name);
	}
}
