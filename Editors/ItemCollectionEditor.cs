namespace LastChaos_ToolBoxNG
{
	public class ItemCollectionEditor : Form
	{
		private const int NeedCount = 6;
		private const int NeedNone = 0;
		private const int NeedItem = 1;
		private const int NeedTheme = 2;
		private const int ResultNone = 0;
		private const int ResultItem = 1;
		private const int ResultNas = 2;
		private const int ResultExp = 3;
		private const int ResultSp = 4;

		private readonly Main pMain;
		private DataTable? pCollectionTable;
		private bool bLoading = false;
		private bool bDirty = false;
		private int nLoadedTheme = -1;

		private readonly ListBox lbTasks = new();
		private readonly TextBox tbSearch = new();
		private readonly Button btnReload = new();
		private readonly Button btnNew = new();
		private readonly Button btnSave = new();
		private readonly Button btnDisable = new();
		private readonly Label lblStatus = new();

		private readonly NumericUpDown nudCategory = new();
		private readonly NumericUpDown nudTheme = new();
		private readonly CheckBox cbEnable = new();
		private readonly NumericUpDown nudIconId = new();
		private readonly NumericUpDown nudRow = new();
		private readonly NumericUpDown nudCol = new();
		private readonly TextBox tbName = new();
		private readonly TextBox tbDesc = new();

		private readonly DataGridView gridNeeds = new();
		private readonly Button btnPickNeedItem = new();
		private readonly Button btnClearNeed = new();

		private readonly ComboBox cbResultType = new();
		private readonly NumericUpDown nudResultIndex = new();
		private readonly NumericUpDown nudResultNum = new();
		private readonly Button btnPickRewardItem = new();

		private readonly CheckedListBox clbGroupTasks = new();
		private readonly Button btnRefreshGroupTasks = new();
		private readonly Button btnUseGroupTasks = new();
		private readonly Button btnNewGroupRewardTask = new();

		public ItemCollectionEditor(Main mainForm)
		{
			pMain = mainForm;

			Text = "Item Collection Editor";
			Name = "ItemCollectionEditor";
			MinimumSize = new Size(1040, 680);
			Size = new Size(1180, 760);
			StartPosition = FormStartPosition.CenterScreen;

			BuildInterface();

			Load += async (_, _) => await LoadDataAsync();
			FormClosing += ItemCollectionEditor_FormClosing;
		}

		private void BuildInterface()
		{
			SplitContainer split = new()
			{
				Dock = DockStyle.Fill,
				SplitterDistance = 360
			};
			Controls.Add(split);

			TableLayoutPanel left = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 4,
				Padding = new Padding(8)
			};
			left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			split.Panel1.Controls.Add(left);

			tbSearch.Dock = DockStyle.Top;
			tbSearch.PlaceholderText = "Search category, theme, or name...";
			tbSearch.TextChanged += (_, _) => PopulateTaskList(nLoadedTheme);
			left.Controls.Add(tbSearch, 0, 0);

			lbTasks.Dock = DockStyle.Fill;
			lbTasks.IntegralHeight = false;
			lbTasks.SelectedIndexChanged += LbTasks_SelectedIndexChanged;
			left.Controls.Add(lbTasks, 0, 1);

			FlowLayoutPanel leftButtons = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				AutoSize = true
			};
			left.Controls.Add(leftButtons, 0, 2);

			SetupButton(btnReload, "Reload", async (_, _) => await LoadDataAsync(nLoadedTheme));
			SetupButton(btnNew, "New Task", (_, _) => NewTask());
			SetupButton(btnSave, "Save", async (_, _) => await SaveTaskAsync());
			SetupButton(btnDisable, "Disable", async (_, _) => await DisableTaskAsync());
			leftButtons.Controls.AddRange([btnReload, btnNew, btnSave, btnDisable]);

			lblStatus.Dock = DockStyle.Fill;
			lblStatus.AutoSize = true;
			lblStatus.ForeColor = Color.FromArgb(80, 80, 80);
			lblStatus.Text = "Loading...";
			left.Controls.Add(lblStatus, 0, 3);

			TabControl tabs = new()
			{
				Dock = DockStyle.Fill
			};
			split.Panel2.Controls.Add(tabs);

			TabPage tabTask = new("Task");
			TabPage tabRequirements = new("Requirements");
			TabPage tabReward = new("Reward");
			TabPage tabGroup = new("Group Reward");
			tabs.TabPages.AddRange([tabTask, tabRequirements, tabReward, tabGroup]);

			BuildTaskTab(tabTask);
			BuildRequirementsTab(tabRequirements);
			BuildRewardTab(tabReward);
			BuildGroupTab(tabGroup);
		}

		private void BuildTaskTab(TabPage tab)
		{
			TableLayoutPanel layout = MakeFormLayout(4);
			tab.Controls.Add(layout);

			ConfigureNumeric(nudCategory, 0, 999999999);
			ConfigureNumeric(nudTheme, 0, 999999999);
			ConfigureNumeric(nudIconId, 0, 255);
			ConfigureNumeric(nudRow, 0, 255);
			ConfigureNumeric(nudCol, 0, 255);

			cbEnable.Text = "Enabled";
			cbEnable.CheckedChanged += MarkDirty;

			tbName.Dock = DockStyle.Fill;
			tbName.TextChanged += MarkDirty;
			tbDesc.Dock = DockStyle.Fill;
			tbDesc.TextChanged += MarkDirty;

			AddField(layout, 0, "Category / group", nudCategory);
			AddField(layout, 1, "Theme / task ID", nudTheme);
			layout.Controls.Add(cbEnable, 0, 2);
			layout.SetColumnSpan(cbEnable, 4);

			AddField(layout, 3, "Title", tbName, 3);
			AddField(layout, 4, "Description", tbDesc, 3);
			AddField(layout, 5, "Icon ID", nudIconId);
			AddField(layout, 6, "Icon row", nudRow);
			AddField(layout, 7, "Icon column", nudCol);

			Label hint = MakeHintLabel("Category is the visible grouping used by the client. Theme is the unique task ID saved in t_item_collection.");
			layout.Controls.Add(hint, 0, 8);
			layout.SetColumnSpan(hint, 4);
		}

		private void BuildRequirementsTab(TabPage tab)
		{
			TableLayoutPanel layout = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 3,
				Padding = new Padding(8)
			};
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			tab.Controls.Add(layout);

			Label hint = MakeHintLabel("Use Item requirements for normal collection items. Use Completed Theme when a task must be finished before this one can be completed.");
			layout.Controls.Add(hint, 0, 0);

			gridNeeds.Dock = DockStyle.Fill;
			gridNeeds.AllowUserToAddRows = false;
			gridNeeds.AllowUserToDeleteRows = false;
			gridNeeds.RowHeadersVisible = false;
			gridNeeds.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			gridNeeds.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			gridNeeds.Columns.Add(new DataGridViewTextBoxColumn { Name = "slot", HeaderText = "#", ReadOnly = true, FillWeight = 35 });
			DataGridViewComboBoxColumn typeColumn = new()
			{
				Name = "type",
				HeaderText = "Type",
				FlatStyle = FlatStyle.Flat,
				FillWeight = 130
			};
			typeColumn.Items.AddRange(["None", "Item", "Completed Theme"]);
			gridNeeds.Columns.Add(typeColumn);
			gridNeeds.Columns.Add(new DataGridViewTextBoxColumn { Name = "index", HeaderText = "Index", FillWeight = 70 });
			gridNeeds.Columns.Add(new DataGridViewTextBoxColumn { Name = "name", HeaderText = "Resolved name", ReadOnly = true, FillWeight = 190 });
			gridNeeds.Columns.Add(new DataGridViewTextBoxColumn { Name = "num", HeaderText = "Amount", FillWeight = 70 });
			gridNeeds.CurrentCellDirtyStateChanged += (_, _) =>
			{
				if (gridNeeds.IsCurrentCellDirty)
					gridNeeds.CommitEdit(DataGridViewDataErrorContexts.Commit);
			};
			gridNeeds.CellValueChanged += (_, e) =>
			{
				if (e.RowIndex >= 0)
				{
					UpdateNeedRowName(e.RowIndex);
					MarkDirty();
				}
			};
			gridNeeds.CellEndEdit += (_, e) =>
			{
				if (e.RowIndex >= 0)
					UpdateNeedRowName(e.RowIndex);
			};
			gridNeeds.DataError += (_, _) => { };
			layout.Controls.Add(gridNeeds, 0, 1);

			FlowLayoutPanel buttons = new()
			{
				Dock = DockStyle.Fill,
				AutoSize = true
			};
			SetupButton(btnPickNeedItem, "Pick Item For Selected Row", (_, _) => PickNeedItem());
			SetupButton(btnClearNeed, "Clear Selected Row", (_, _) => ClearSelectedNeed());
			buttons.Controls.AddRange([btnPickNeedItem, btnClearNeed]);
			layout.Controls.Add(buttons, 0, 2);

			ResetNeeds();
		}

		private void BuildRewardTab(TabPage tab)
		{
			TableLayoutPanel layout = MakeFormLayout(2);
			tab.Controls.Add(layout);

			FillCombo(cbResultType, [
				(ResultNone, "None"),
				(ResultItem, "Item"),
				(ResultNas, "Nas"),
				(ResultExp, "EXP"),
				(ResultSp, "SP")
			]);
			cbResultType.SelectedIndexChanged += MarkDirty;

			ConfigureNumeric(nudResultIndex, 0, 999999999);
			ConfigureNumeric(nudResultNum, 0, 999999999);

			AddField(layout, 0, "Reward type", cbResultType);
			AddField(layout, 1, "Reward index", nudResultIndex);
			AddField(layout, 2, "Reward amount", nudResultNum);

			SetupButton(btnPickRewardItem, "Pick Reward Item", (_, _) => PickRewardItem());
			layout.Controls.Add(btnPickRewardItem, 1, 3);

			Label hint = MakeHintLabel("For item rewards, index is the item ID. For Nas, EXP, and SP rewards, amount is the useful value and index is normally 0.");
			layout.Controls.Add(hint, 0, 4);
			layout.SetColumnSpan(hint, 2);
		}

		private void BuildGroupTab(TabPage tab)
		{
			TableLayoutPanel layout = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 4,
				Padding = new Padding(8)
			};
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			tab.Controls.Add(layout);

			Label hint = MakeHintLabel("To reward a full group, create a normal task in the category and fill its requirements with Completed Theme entries. The player can claim that task after the selected tasks are completed.");
			layout.Controls.Add(hint, 0, 0);

			clbGroupTasks.Dock = DockStyle.Fill;
			clbGroupTasks.CheckOnClick = true;
			layout.Controls.Add(clbGroupTasks, 0, 1);

			FlowLayoutPanel buttons = new()
			{
				Dock = DockStyle.Fill,
				AutoSize = true
			};
			SetupButton(btnRefreshGroupTasks, "Refresh Category Tasks", (_, _) => PopulateGroupTasks());
			SetupButton(btnUseGroupTasks, "Use Checked As Requirements", (_, _) => UseCheckedGroupTasks());
			SetupButton(btnNewGroupRewardTask, "New Group Reward Task", (_, _) => NewGroupRewardTask());
			buttons.Controls.AddRange([btnRefreshGroupTasks, btnUseGroupTasks, btnNewGroupRewardTask]);
			layout.Controls.Add(buttons, 0, 2);

			Label limit = MakeHintLabel("Current server format supports six requirements per task. Larger groups need chained group reward tasks or a server-side extension.");
			layout.Controls.Add(limit, 0, 3);
		}

		private async Task LoadDataAsync(int nThemeToSelect = -1)
		{
			if (!ConfirmDiscardChanges())
				return;

			try
			{
				SetStatus("Loading item collection data...");
				Enabled = false;

				await pMain.GenericLoadItemDataAsync();

				pCollectionTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT * FROM {pMain.pSettings.DBData}.t_item_collection ORDER BY a_category, a_theme;");
				});

				if (pCollectionTable == null)
				{
					MessageBox.Show("Could not load t_item_collection. Check the ToolBox console for the SQL error.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					SetStatus("Load failed.");
					return;
				}

				bDirty = false;
				PopulateTaskList(nThemeToSelect);
				SetStatus($"Loaded {pCollectionTable.Rows.Count} item collection tasks.");
			}
			finally
			{
				Enabled = true;
			}
		}

		private void PopulateTaskList(int nThemeToSelect = -1)
		{
			if (pCollectionTable == null)
				return;

			string strFilter = tbSearch.Text.Trim().ToLowerInvariant();
			bLoading = true;
			lbTasks.BeginUpdate();
			lbTasks.Items.Clear();

			foreach (DataRow row in pCollectionTable.Rows)
			{
				int nTheme = RowInt(row, "a_theme");
				int nCategory = RowInt(row, "a_category");
				bool bEnabled = RowInt(row, "a_enable") != 0;
				string strName = RowString(row, GetNameColumn());
				if (string.IsNullOrWhiteSpace(strName))
					strName = RowString(row, "a_theme_string");

				string strDisplay = $"[{nCategory}] {nTheme} - {strName}" + (bEnabled ? "" : " (disabled)");
				if (!string.IsNullOrEmpty(strFilter) && !strDisplay.ToLowerInvariant().Contains(strFilter))
					continue;

				lbTasks.Items.Add(new Main.ListBoxItem
				{
					ID = nTheme,
					Text = strDisplay
				});
			}

			lbTasks.EndUpdate();
			bLoading = false;

			if (nThemeToSelect > 0)
				SelectTaskInList(nThemeToSelect);
			else if (lbTasks.Items.Count > 0 && lbTasks.SelectedIndex < 0)
				lbTasks.SelectedIndex = 0;
		}

		private void LbTasks_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (bLoading || lbTasks.SelectedItem is not Main.ListBoxItem selected)
				return;

			if (selected.ID == nLoadedTheme)
				return;

			if (!ConfirmDiscardChanges())
			{
				SelectTaskInList(nLoadedTheme);
				return;
			}

			LoadTask(selected.ID);
		}

		private void LoadTask(int nTheme)
		{
			DataRow? row = FindThemeRow(nTheme);
			if (row == null)
				return;

			bLoading = true;
			nLoadedTheme = nTheme;

			nudCategory.Value = ClampToNumeric(nudCategory, RowInt(row, "a_category"));
			nudTheme.Value = ClampToNumeric(nudTheme, RowInt(row, "a_theme"));
			cbEnable.Checked = RowInt(row, "a_enable") != 0;
			nudIconId.Value = ClampToNumeric(nudIconId, RowInt(row, "a_id"));
			nudRow.Value = ClampToNumeric(nudRow, RowInt(row, "a_row"));
			nudCol.Value = ClampToNumeric(nudCol, RowInt(row, "a_col"));
			tbName.Text = RowString(row, GetNameColumn());
			if (string.IsNullOrWhiteSpace(tbName.Text))
				tbName.Text = RowString(row, "a_theme_string");
			tbDesc.Text = RowString(row, GetDescColumn());
			if (string.IsNullOrWhiteSpace(tbDesc.Text))
				tbDesc.Text = RowString(row, "a_desc_string");

			ResetNeeds();
			for (int i = 1; i <= NeedCount; i++)
				SetNeedRow(i - 1, RowInt(row, $"a_need{i}_type"), RowInt(row, $"a_need{i}_index"), RowInt(row, $"a_need{i}_num"));

			SetComboValue(cbResultType, RowInt(row, "a_result_type"));
			nudResultIndex.Value = ClampToNumeric(nudResultIndex, RowInt(row, "a_result_index"));
			nudResultNum.Value = ClampToNumeric(nudResultNum, RowInt(row, "a_result_num"));

			bLoading = false;
			bDirty = false;
			PopulateGroupTasks();
			SetStatus($"Editing theme {nTheme}.");
		}

		private void NewTask()
		{
			if (!ConfirmDiscardChanges())
				return;

			int nNextTheme = NextThemeId();
			int nCategory = nLoadedTheme > 0 && FindThemeRow(nLoadedTheme) is DataRow row ? RowInt(row, "a_category") : 1;

			bLoading = true;
			lbTasks.ClearSelected();
			nLoadedTheme = -1;
			nudCategory.Value = ClampToNumeric(nudCategory, nCategory);
			nudTheme.Value = ClampToNumeric(nudTheme, nNextTheme);
			cbEnable.Checked = true;
			nudIconId.Value = 0;
			nudRow.Value = 0;
			nudCol.Value = 0;
			tbName.Text = "New item collection task";
			tbDesc.Text = string.Empty;
			ResetNeeds();
			SetComboValue(cbResultType, ResultNone);
			nudResultIndex.Value = 0;
			nudResultNum.Value = 0;
			bLoading = false;
			bDirty = true;
			PopulateGroupTasks();
			SetStatus($"New task prepared with theme {nNextTheme}.");
		}

		private void NewGroupRewardTask()
		{
			int nCategory = (int)nudCategory.Value;
			NewTask();
			bLoading = true;
			nudCategory.Value = ClampToNumeric(nudCategory, nCategory > 0 ? nCategory : 1);
			tbName.Text = "Group reward";
			tbDesc.Text = "Complete every task in this group.";
			bLoading = false;
			bDirty = true;
			PopulateGroupTasks();
			SetStatus("New group reward task prepared. Check the tasks it should depend on, then set the reward.");
		}

		private async Task SaveTaskAsync()
		{
			if (!ValidateTask(out int nCategory, out int nTheme, out List<(int Type, int Index, int Num)> needs))
				return;

			NormalizeThemeRequirementsIfNeeded(needs);

			string strName = tbName.Text.Trim();
			string strDesc = tbDesc.Text.Trim();
			int nResultType = ComboValue(cbResultType);
			int nResultIndex = (int)nudResultIndex.Value;
			int nResultNum = (int)nudResultNum.Value;

			if (nResultType == ResultNone)
			{
				nResultIndex = 0;
				nResultNum = 0;
			}

			List<(string Column, string Value)> values =
			[
				("a_theme", nTheme.ToString()),
				("a_category", nCategory.ToString()),
				("a_theme_string", SqlString(strName)),
				("a_desc_string", SqlString(strDesc)),
				("a_enable", cbEnable.Checked ? "1" : "0"),
				("a_id", ((int)nudIconId.Value).ToString()),
				("a_row", ((int)nudRow.Value).ToString()),
				("a_col", ((int)nudCol.Value).ToString())
			];

			for (int i = 0; i < NeedCount; i++)
			{
				values.Add(($"a_need{i + 1}_type", needs[i].Type.ToString()));
				values.Add(($"a_need{i + 1}_index", needs[i].Index.ToString()));
				values.Add(($"a_need{i + 1}_num", needs[i].Num.ToString()));
			}

			values.Add(("a_result_type", nResultType.ToString()));
			values.Add(("a_result_index", nResultIndex.ToString()));
			values.Add(("a_result_num", nResultNum.ToString()));

			AddLocaleValue(values, GetNameColumn(), strName);
			AddLocaleValue(values, GetDescColumn(), strDesc);

			string strColumns = string.Join(", ", values.Select(v => v.Column));
			string strValues = string.Join(", ", values.Select(v => v.Value));
			string strUpdates = string.Join(", ", values.Where(v => v.Column != "a_theme").Select(v => $"{v.Column}=VALUES({v.Column})"));
			string strQuery = $"INSERT INTO {pMain.pSettings.DBData}.t_item_collection ({strColumns}) VALUES ({strValues}) ON DUPLICATE KEY UPDATE {strUpdates};";

			SetStatus("Saving item collection task...");
			bool bSuccess = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strQuery, out long _));
			if (!bSuccess)
			{
				MessageBox.Show("Save failed. Check the ToolBox console for the SQL error.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				SetStatus("Save failed.");
				return;
			}

			bDirty = false;
			await ExportItemCollectionLodAsync();
			await LoadDataAsync(nTheme);
			SetStatus($"Saved theme {nTheme}.");
		}

		private async Task ExportItemCollectionLodAsync()
		{
			string strClientPath = pMain.pSettings.ClientPath;
			if (string.IsNullOrWhiteSpace(strClientPath))
			{
				SetStatus("Saved DB. ClientPath is empty, so itemCollection.lod was not exported.");
				return;
			}

			string strDataPath = Path.Combine(strClientPath, "Data");
			string strFilePath = Path.Combine(strDataPath, "itemCollection.lod");
			DataTable? table = await Task.Run(() =>
			{
				return pMain.QuerySelect(
					"utf8",
					$"SELECT a_category, a_theme, a_id, a_row, a_col, a_need1_type, a_need2_type, a_need3_type, a_need4_type, a_need5_type, a_need6_type, a_need1_index, a_need2_index, a_need3_index, a_need4_index, a_need5_index, a_need6_index, a_need1_num, a_need2_num, a_need3_num, a_need4_num, a_need5_num, a_need6_num, a_result_type, a_result_index, a_result_num FROM {pMain.pSettings.DBData}.t_item_collection WHERE a_enable=1 ORDER BY a_theme, a_category;",
					false);
			});

			if (table == null)
			{
				SetStatus("Saved DB, but client itemCollection.lod export failed while reading DB.");
				return;
			}

			await Task.Run(() =>
			{
				Directory.CreateDirectory(strDataPath);
				using BinaryWriter stream = new(File.Create(strFilePath));
				stream.Write(table.Rows.Count);

				foreach (DataRow row in table.Rows)
				{
					int nCategoryID = Convert.ToInt32(row["a_category"]);
					nCategoryID = (nCategoryID << 24);
					nCategoryID |= (Convert.ToInt32(row["a_theme"]) & 0x00FFFFFF);

					stream.Write(nCategoryID);
					stream.Write(Convert.ToByte(row["a_id"]));
					stream.Write(Convert.ToByte(row["a_row"]));
					stream.Write(Convert.ToInt16(row["a_col"]));

					for (int i = 1; i <= NeedCount; i++)
					{
						stream.Write(Convert.ToInt32(row[$"a_need{i}_type"]));
						stream.Write(Convert.ToInt32(row[$"a_need{i}_index"]));
						stream.Write(Convert.ToInt32(row[$"a_need{i}_num"]));
					}

					stream.Write(Convert.ToInt32(row["a_result_type"]));
					stream.Write(Convert.ToInt32(row["a_result_index"]));
					stream.Write(Convert.ToInt32(row["a_result_num"]));
				}
			});

			table.Dispose();
			SetStatus($"Saved DB and exported {strFilePath}.");
		}

		private async Task DisableTaskAsync()
		{
			int nTheme = (int)nudTheme.Value;
			if (nTheme <= 0)
				return;

			if (MessageBox.Show($"Disable item collection task {nTheme}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			string strQuery = $"UPDATE {pMain.pSettings.DBData}.t_item_collection SET a_enable=0 WHERE a_theme={nTheme};";
			bool bSuccess = await Task.Run(() => pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strQuery, out long _));
			if (!bSuccess)
			{
				MessageBox.Show("Disable failed. Check the ToolBox console for the SQL error.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			bDirty = false;
			await LoadDataAsync(nTheme);
		}

		private bool ValidateTask(out int nCategory, out int nTheme, out List<(int Type, int Index, int Num)> needs)
		{
			nCategory = (int)nudCategory.Value;
			nTheme = (int)nudTheme.Value;
			needs = [];

			if (nCategory <= 0)
			{
				MessageBox.Show("Category must be greater than 0.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (nTheme <= 0)
			{
				MessageBox.Show("Theme/task ID must be greater than 0.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (string.IsNullOrWhiteSpace(tbName.Text))
			{
				MessageBox.Show("Title cannot be empty.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			for (int i = 0; i < NeedCount; i++)
			{
				DataGridViewRow row = gridNeeds.Rows[i];
				int nType = NeedTypeValue(row.Cells["type"].Value?.ToString());
				int nIndex = SafeInt(row.Cells["index"].Value?.ToString());
				int nNum = SafeInt(row.Cells["num"].Value?.ToString());

				if (nType == NeedNone)
				{
					needs.Add((NeedNone, 0, 0));
					continue;
				}

				if (nIndex <= 0)
				{
					MessageBox.Show($"Requirement slot {i + 1} needs a valid index.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}

				if (nType == NeedTheme && nIndex == nTheme)
				{
					MessageBox.Show($"Requirement slot {i + 1} points to the same task. A task cannot depend on itself.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}

				if (nNum <= 0)
					nNum = 1;

				needs.Add((nType, nIndex, nNum));
			}

			int nResultType = ComboValue(cbResultType);
			if (nResultType != ResultNone)
			{
				if (nResultType == ResultItem && (int)nudResultIndex.Value <= 0)
				{
					MessageBox.Show("Item rewards need a valid item index.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}

				if ((int)nudResultNum.Value <= 0)
				{
					MessageBox.Show("Reward amount must be greater than 0.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

			return true;
		}

		private void NormalizeThemeRequirementsIfNeeded(List<(int Type, int Index, int Num)> needs)
		{
			if (needs.Count == 0 || needs[0].Type == NeedTheme || !needs.Any(n => n.Type == NeedTheme))
				return;

			DialogResult result = MessageBox.Show(
				"This server unlocks item-collection task dependencies most reliably when completed-theme requirements are first. Move completed-theme requirements to the top before saving?",
				Text,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question);

			if (result != DialogResult.Yes)
				return;

			List<(int Type, int Index, int Num)> sorted = needs
				.Where(n => n.Type == NeedTheme)
				.Concat(needs.Where(n => n.Type != NeedTheme))
				.Take(NeedCount)
				.ToList();

			needs.Clear();
			needs.AddRange(sorted);

			bLoading = true;
			for (int i = 0; i < NeedCount; i++)
				SetNeedRow(i, needs[i].Type, needs[i].Index, needs[i].Num);
			bLoading = false;
		}

		private void PopulateGroupTasks()
		{
			if (pCollectionTable == null)
				return;

			int nCategory = (int)nudCategory.Value;
			int nCurrentTheme = (int)nudTheme.Value;
			HashSet<int> selectedThemeNeeds = GetThemeNeedIndexes();

			bLoading = true;
			clbGroupTasks.Items.Clear();

			foreach (DataRow row in pCollectionTable.Rows)
			{
				int nTheme = RowInt(row, "a_theme");
				if (nTheme == nCurrentTheme || RowInt(row, "a_category") != nCategory)
					continue;

				string strName = RowString(row, GetNameColumn());
				if (string.IsNullOrWhiteSpace(strName))
					strName = RowString(row, "a_theme_string");

				Main.ListBoxItem item = new()
				{
					ID = nTheme,
					Text = $"{nTheme} - {strName}"
				};
				clbGroupTasks.Items.Add(item, selectedThemeNeeds.Contains(nTheme));
			}

			bLoading = false;
		}

		private void UseCheckedGroupTasks()
		{
			List<int> themeIds = clbGroupTasks.CheckedItems
				.OfType<Main.ListBoxItem>()
				.Select(item => item.ID)
				.ToList();

			if (themeIds.Count == 0)
			{
				MessageBox.Show("Check at least one task first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (themeIds.Count > NeedCount)
			{
				MessageBox.Show($"Only {NeedCount} completed-task requirements fit in one item collection task.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			bLoading = true;
			ResetNeeds();
			for (int i = 0; i < themeIds.Count; i++)
				SetNeedRow(i, NeedTheme, themeIds[i], 1);
			bLoading = false;
			bDirty = true;
			SetStatus($"Added {themeIds.Count} completed-task requirements.");
		}

		private void PickNeedItem()
		{
			if (gridNeeds.CurrentRow == null)
				return;

			int nCurrentItem = SafeInt(gridNeeds.CurrentRow.Cells["index"].Value?.ToString());
			using ItemPicker picker = new(pMain, this, nCurrentItem, true);
			if (picker.ShowDialog() != DialogResult.OK)
				return;

			int nItemId = Convert.ToInt32(picker.ReturnValues[0]);
			if (nItemId <= 0)
			{
				ClearSelectedNeed();
				return;
			}

			gridNeeds.CurrentRow.Cells["type"].Value = NeedTypeName(NeedItem);
			gridNeeds.CurrentRow.Cells["index"].Value = nItemId;
			if (SafeInt(gridNeeds.CurrentRow.Cells["num"].Value?.ToString()) <= 0)
				gridNeeds.CurrentRow.Cells["num"].Value = 1;
			UpdateNeedRowName(gridNeeds.CurrentRow.Index);
			bDirty = true;
		}

		private void PickRewardItem()
		{
			using ItemPicker picker = new(pMain, this, (int)nudResultIndex.Value, true);
			if (picker.ShowDialog() != DialogResult.OK)
				return;

			int nItemId = Convert.ToInt32(picker.ReturnValues[0]);
			if (nItemId <= 0)
			{
				SetComboValue(cbResultType, ResultNone);
				nudResultIndex.Value = 0;
				nudResultNum.Value = 0;
				return;
			}

			SetComboValue(cbResultType, ResultItem);
			nudResultIndex.Value = ClampToNumeric(nudResultIndex, nItemId);
			if (nudResultNum.Value <= 0)
				nudResultNum.Value = 1;
			bDirty = true;
		}

		private void ClearSelectedNeed()
		{
			if (gridNeeds.CurrentRow == null)
				return;

			SetNeedRow(gridNeeds.CurrentRow.Index, NeedNone, 0, 0);
			bDirty = true;
		}

		private void ResetNeeds()
		{
			bool bWasLoading = bLoading;
			bLoading = true;
			gridNeeds.Rows.Clear();
			for (int i = 0; i < NeedCount; i++)
				SetNeedRow(i, NeedNone, 0, 0, true);
			bLoading = bWasLoading;
		}

		private void SetNeedRow(int nRow, int nType, int nIndex, int nNum, bool bAdd = false)
		{
			if (bAdd)
				gridNeeds.Rows.Add(nRow + 1, NeedTypeName(nType), nIndex, string.Empty, nNum);
			else
			{
				gridNeeds.Rows[nRow].Cells["type"].Value = NeedTypeName(nType);
				gridNeeds.Rows[nRow].Cells["index"].Value = nIndex;
				gridNeeds.Rows[nRow].Cells["num"].Value = nNum;
			}

			UpdateNeedRowName(nRow);
		}

		private void UpdateNeedRowName(int nRow)
		{
			if (nRow < 0 || nRow >= gridNeeds.Rows.Count)
				return;

			DataGridViewRow row = gridNeeds.Rows[nRow];
			int nType = NeedTypeValue(row.Cells["type"].Value?.ToString());
			int nIndex = SafeInt(row.Cells["index"].Value?.ToString());

			row.Cells["name"].Value = nType switch
			{
				NeedItem => ItemName(nIndex),
				NeedTheme => ThemeName(nIndex),
				_ => string.Empty
			};
		}

		private HashSet<int> GetThemeNeedIndexes()
		{
			HashSet<int> indexes = [];
			foreach (DataGridViewRow row in gridNeeds.Rows)
			{
				if (NeedTypeValue(row.Cells["type"].Value?.ToString()) == NeedTheme)
				{
					int nIndex = SafeInt(row.Cells["index"].Value?.ToString());
					if (nIndex > 0)
						indexes.Add(nIndex);
				}
			}

			return indexes;
		}

		private string ItemName(int nItemId)
		{
			if (nItemId <= 0)
				return string.Empty;

			DataRow? row = pMain.pTables.ItemTable?.Select($"a_index={nItemId}").FirstOrDefault();
			if (row == null)
				return $"{nItemId} - item not found";

			return $"{nItemId} - {row["a_name_" + pMain.pSettings.WorkLocale]}";
		}

		private string ThemeName(int nTheme)
		{
			if (nTheme <= 0)
				return string.Empty;

			DataRow? row = FindThemeRow(nTheme);
			if (row == null)
				return $"{nTheme} - theme not found";

			string strName = RowString(row, GetNameColumn());
			if (string.IsNullOrWhiteSpace(strName))
				strName = RowString(row, "a_theme_string");

			return $"{nTheme} - {strName}";
		}

		private DataRow? FindThemeRow(int nTheme)
		{
			if (pCollectionTable == null)
				return null;

			return pCollectionTable.Select($"a_theme={nTheme}").FirstOrDefault();
		}

		private int NextThemeId()
		{
			if (pCollectionTable == null || pCollectionTable.Rows.Count == 0)
				return 1;

			return pCollectionTable.AsEnumerable().Select(row => RowInt(row, "a_theme")).DefaultIfEmpty(0).Max() + 1;
		}

		private void SelectTaskInList(int nTheme)
		{
			if (nTheme <= 0)
				return;

			bLoading = true;
			for (int i = 0; i < lbTasks.Items.Count; i++)
			{
				if (lbTasks.Items[i] is Main.ListBoxItem item && item.ID == nTheme)
				{
					lbTasks.SelectedIndex = i;
					break;
				}
			}
			bLoading = false;

			if (lbTasks.SelectedItem is Main.ListBoxItem selected && selected.ID == nTheme)
				LoadTask(nTheme);
		}

		private bool ConfirmDiscardChanges()
		{
			if (!bDirty)
				return true;

			return MessageBox.Show("Discard unsaved item collection changes?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}

		private void ItemCollectionEditor_FormClosing(object? sender, FormClosingEventArgs e)
		{
			if (!ConfirmDiscardChanges())
				e.Cancel = true;
		}

		private void AddLocaleValue(List<(string Column, string Value)> values, string strColumn, string strValue)
		{
			if (pCollectionTable?.Columns.Contains(strColumn) == true && !values.Any(v => v.Column == strColumn))
				values.Add((strColumn, SqlString(strValue)));
		}

		private string GetNameColumn()
		{
			string strColumn = "a_theme_string_" + pMain.pSettings.WorkLocale;
			return pCollectionTable?.Columns.Contains(strColumn) == true ? strColumn : "a_theme_string";
		}

		private string GetDescColumn()
		{
			string strColumn = "a_desc_string_" + pMain.pSettings.WorkLocale;
			return pCollectionTable?.Columns.Contains(strColumn) == true ? strColumn : "a_desc_string";
		}

		private string SqlString(string strValue)
		{
			return $"'{pMain.EscapeChars(strValue)}'";
		}

		private static int SafeInt(string? strValue)
		{
			return int.TryParse(strValue, out int nValue) ? nValue : 0;
		}

		private static int RowInt(DataRow row, string strColumn)
		{
			if (!row.Table.Columns.Contains(strColumn) || row[strColumn] == DBNull.Value)
				return 0;

			return Convert.ToInt32(row[strColumn]);
		}

		private static string RowString(DataRow row, string strColumn)
		{
			if (!row.Table.Columns.Contains(strColumn) || row[strColumn] == DBNull.Value)
				return string.Empty;

			return row[strColumn].ToString() ?? string.Empty;
		}

		private static decimal ClampToNumeric(NumericUpDown numeric, int nValue)
		{
			return Math.Min(numeric.Maximum, Math.Max(numeric.Minimum, nValue));
		}

		private static string NeedTypeName(int nType)
		{
			return nType switch
			{
				NeedItem => "Item",
				NeedTheme => "Completed Theme",
				_ => "None"
			};
		}

		private static int NeedTypeValue(string? strName)
		{
			return strName switch
			{
				"Item" => NeedItem,
				"Completed Theme" => NeedTheme,
				_ => NeedNone
			};
		}

		private static void FillCombo(ComboBox combo, (int Value, string Text)[] values)
		{
			combo.DropDownStyle = ComboBoxStyle.DropDownList;
			combo.Items.Clear();

			foreach ((int nValue, string strText) in values)
			{
				combo.Items.Add(new Main.ComboBoxItem
				{
					Value = nValue,
					DisplayText = strText
				});
			}

			if (combo.Items.Count > 0)
				combo.SelectedIndex = 0;
		}

		private static int ComboValue(ComboBox combo)
		{
			return combo.SelectedItem is Main.ComboBoxItem item ? item.Value : 0;
		}

		private static void SetComboValue(ComboBox combo, int nValue)
		{
			for (int i = 0; i < combo.Items.Count; i++)
			{
				if (combo.Items[i] is Main.ComboBoxItem item && item.Value == nValue)
				{
					combo.SelectedIndex = i;
					return;
				}
			}

			if (combo.Items.Count > 0)
				combo.SelectedIndex = 0;
		}

		private static void SetupButton(Button button, string strText, EventHandler handler)
		{
			button.Text = strText;
			button.AutoSize = true;
			button.Margin = new Padding(3);
			button.Click += handler;
		}

		private static void ConfigureNumeric(NumericUpDown numeric, int nMin, int nMax)
		{
			numeric.Minimum = nMin;
			numeric.Maximum = nMax;
			numeric.Dock = DockStyle.Fill;
			numeric.ThousandsSeparator = false;
			numeric.ValueChanged += (_, _) =>
			{
				if (numeric.FindForm() is ItemCollectionEditor editor)
					editor.MarkDirty();
			};
		}

		private static TableLayoutPanel MakeFormLayout(int nColumns)
		{
			TableLayoutPanel layout = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = nColumns,
				RowCount = 12,
				Padding = new Padding(12)
			};

			for (int i = 0; i < nColumns; i++)
				layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / nColumns));

			for (int i = 0; i < 12; i++)
				layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

			return layout;
		}

		private static void AddField(TableLayoutPanel layout, int nRow, string strLabel, Control control, int nColumnSpan = 1)
		{
			Label label = new()
			{
				Text = strLabel,
				AutoSize = true,
				Dock = DockStyle.Fill,
				TextAlign = ContentAlignment.MiddleLeft,
				Margin = new Padding(3, 8, 3, 3)
			};

			control.Margin = new Padding(3, 3, 8, 3);
			control.Dock = DockStyle.Fill;

			layout.Controls.Add(label, 0, nRow);
			layout.Controls.Add(control, 1, nRow);
			if (nColumnSpan > 1)
				layout.SetColumnSpan(control, nColumnSpan);
		}

		private static Label MakeHintLabel(string strText)
		{
			return new Label
			{
				Text = strText,
				AutoSize = true,
				MaximumSize = new Size(720, 0),
				ForeColor = Color.FromArgb(80, 80, 80),
				Padding = new Padding(0, 0, 0, 8)
			};
		}

		private void MarkDirty(object? sender = null, EventArgs? e = null)
		{
			if (!bLoading)
				bDirty = true;
		}

		private void SetStatus(string strText)
		{
			lblStatus.Text = strText;
		}
	}
}
