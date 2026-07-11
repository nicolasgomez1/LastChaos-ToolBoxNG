using System.Text;

namespace LastChaos_ToolBoxNG
{
	public class AffinityEditor : Form
	{
		private const int AF_CONNECT = 1 << 0;
		private const int AF_CONTRIBUTE = 1 << 1;
		private const int AF_SHOP = 1 << 2;
		private const int AF_REWARD = 1 << 3;

		private readonly Main pMain;
		private DataTable? affinityTable;
		private DataTable? affinityNpcTable;
		private DataTable? affinityWorkTable;
		private DataTable? affinityRewardTable;
		private bool bLoading;
		private bool bDirty;
		private int nLastListIndex = -1;

		private readonly ListBox listAffinities = new();
		private readonly TextBox tbSearch = new();
		private readonly Button btnReload = new();
		private readonly Button btnNew = new();
		private readonly Button btnSave = new();
		private readonly Button btnDisable = new();
		private readonly Button btnPickNpc = new();
		private readonly Button btnPickItem = new();
		private readonly Button btnPickQuest = new();
		private readonly Button btnPickString = new();
		private readonly Label lStatus = new();
		private readonly TabControl tabs = new();

		private readonly TextBox tbIndex = new();
		private readonly TextBox tbName = new();
		private readonly TextBox tbMaxValue = new();
		private readonly TextBox tbNas = new();
		private readonly TextBox tbTextureId = new();
		private readonly TextBox tbTextureRow = new();
		private readonly TextBox tbTextureCol = new();
		private readonly TextBox tbNeedItemIdx = new();
		private readonly TextBox tbNeedItemCount = new();
		private readonly TextBox tbNeedLevel = new();
		private readonly TextBox tbNeedAffinityIdx = new();
		private readonly TextBox tbNeedAffinityValue = new();
		private readonly CheckBox cbEnable = new();

		private readonly DataGridView gridNpcs = new();
		private readonly DataGridView gridWork = new();
		private readonly DataGridView gridRewards = new();

		private readonly struct AffinityNpcLocation
		{
			public AffinityNpcLocation(int npcId, int zone, int yLayer, float x, float z)
			{
				NpcId = npcId;
				Zone = zone;
				YLayer = yLayer;
				X = x;
				Z = z;
			}

			public int NpcId { get; }
			public int Zone { get; }
			public int YLayer { get; }
			public float X { get; }
			public float Z { get; }
		}

		private sealed class MapNpcEntry
		{
			public int Index { get; set; }
			public int YLayer { get; set; }
			public float X { get; set; }
			public float Z { get; set; }
		}

		private sealed class MapZoneEntry
		{
			public int Zone { get; set; }
			public byte Layer { get; set; }
			public int NpcCountOffset { get; set; }
			public int ZoneEnd { get; set; }
			public List<MapNpcEntry> Npcs { get; } = new();
		}

		public AffinityEditor(Main mainForm)
		{
			pMain = mainForm;
			Text = "Affinity Editor";
			Name = "AffinityEditor";
			MinimumSize = new Size(1120, 680);
			Size = new Size(1260, 760);
			StartPosition = FormStartPosition.CenterParent;
			BackColor = Color.FromArgb(28, 30, 31);
			ForeColor = Color.FromArgb(208, 203, 148);

			InitializeComponent();

			Load += AffinityEditor_LoadAsync;
		}

		private void InitializeComponent()
		{
			Panel leftPanel = new()
			{
				Dock = DockStyle.Left,
				Width = 330,
				Padding = new Padding(10)
			};

			tbSearch.Dock = DockStyle.Top;
			tbSearch.PlaceholderText = "Search affinity...";
			StyleTextBox(tbSearch);
			tbSearch.TextChanged += (_, _) => FilterAffinityList();

			listAffinities.Dock = DockStyle.Fill;
			listAffinities.BackColor = Color.FromArgb(28, 30, 31);
			listAffinities.ForeColor = Color.FromArgb(208, 203, 148);
			listAffinities.BorderStyle = BorderStyle.FixedSingle;
			listAffinities.SelectedIndexChanged += listAffinities_SelectedIndexChanged;

			FlowLayoutPanel leftButtons = new()
			{
				Dock = DockStyle.Bottom,
				Height = 38,
				FlowDirection = FlowDirection.LeftToRight
			};

			SetupButton(btnReload, "Reload", 72);
			SetupButton(btnNew, "New", 72);
			SetupButton(btnSave, "Save", 72);
			SetupButton(btnDisable, "Disable", 72);
			btnReload.Click += async (_, _) => await LoadAllDataAsync();
			btnNew.Click += (_, _) => NewAffinity();
			btnSave.Click += async (_, _) => await SaveAffinityAsync();
			btnDisable.Click += async (_, _) => await DisableAffinityAsync();
			leftButtons.Controls.AddRange(new Control[] { btnReload, btnNew, btnSave, btnDisable });

			leftPanel.Controls.Add(listAffinities);
			leftPanel.Controls.Add(tbSearch);
			leftPanel.Controls.Add(leftButtons);

			Panel rightPanel = new()
			{
				Dock = DockStyle.Fill,
				Padding = new Padding(10)
			};

			FlowLayoutPanel pickButtons = new()
			{
				Dock = DockStyle.Top,
				Height = 38,
				FlowDirection = FlowDirection.LeftToRight
			};

			SetupButton(btnPickNpc, "Pick NPC/Mob", 110);
			SetupButton(btnPickItem, "Pick Item", 90);
			SetupButton(btnPickQuest, "Pick Quest", 90);
			SetupButton(btnPickString, "Pick String", 95);
			btnPickNpc.Click += (_, _) => PickNpcForActiveGrid();
			btnPickItem.Click += (_, _) => PickItemForActiveTarget();
			btnPickQuest.Click += (_, _) => PickQuestForWorkGrid();
			btnPickString.Click += (_, _) => PickStringForNpcGrid();
			pickButtons.Controls.AddRange(new Control[] { btnPickNpc, btnPickItem, btnPickQuest, btnPickString });

			tabs.Dock = DockStyle.Fill;
			tabs.Controls.Add(CreateGeneralTab());
			tabs.Controls.Add(CreateNpcTab());
			tabs.Controls.Add(CreateWorkTab());
			tabs.Controls.Add(CreateRewardTab());

			lStatus.Dock = DockStyle.Bottom;
			lStatus.Height = 24;
			lStatus.TextAlign = ContentAlignment.MiddleLeft;

			rightPanel.Controls.Add(tabs);
			rightPanel.Controls.Add(pickButtons);
			rightPanel.Controls.Add(lStatus);

			Controls.Add(rightPanel);
			Controls.Add(leftPanel);
		}

		private TabPage CreateGeneralTab()
		{
			TabPage tab = new("Affinity");
			tab.BackColor = Color.FromArgb(28, 30, 31);
			tab.ForeColor = Color.FromArgb(208, 203, 148);

			TableLayoutPanel layout = new()
			{
				Dock = DockStyle.Top,
				ColumnCount = 4,
				RowCount = 8,
				Padding = new Padding(12),
				AutoSize = true
			};

			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

			AddField(layout, 0, "Affinity ID", tbIndex);
			AddField(layout, 1, "Name", tbName);
			AddField(layout, 2, "Max points", tbMaxValue);
			AddField(layout, 3, "Join NAS cost", tbNas);
			AddField(layout, 4, "Texture ID", tbTextureId);
			AddField(layout, 5, "Texture row", tbTextureRow);
			AddField(layout, 6, "Texture col", tbTextureCol);
			AddField(layout, 7, "Need item ID", tbNeedItemIdx);
			AddField(layout, 8, "Need item count", tbNeedItemCount);
			AddField(layout, 9, "Need character level", tbNeedLevel);
			AddField(layout, 10, "Need affinity ID", tbNeedAffinityIdx);
			AddField(layout, 11, "Need affinity points", tbNeedAffinityValue);

			cbEnable.Text = "Enabled";
			cbEnable.Checked = true;
			cbEnable.AutoSize = true;
			cbEnable.CheckedChanged += MarkDirty;
			layout.Controls.Add(cbEnable, 1, 6);

			Label note = new()
			{
				Dock = DockStyle.Fill,
				AutoSize = true,
				Text = "Shop contents still live in t_shop/t_shopitem. This editor links the affinity shop NPC and its required affinity points.",
				ForeColor = Color.FromArgb(180, 175, 130)
			};
			layout.Controls.Add(note, 0, 7);
			layout.SetColumnSpan(note, 4);

			tab.Controls.Add(layout);
			return tab;
		}

		private TabPage CreateNpcTab()
		{
			TabPage tab = new("NPCs / Dialogue");
			tab.BackColor = Color.FromArgb(28, 30, 31);
			ConfigureGrid(gridNpcs);
			gridNpcs.Columns.Add(MakeTextColumn("npcidx", "NPC ID", 70));
			gridNpcs.Columns.Add(MakeTextColumn("npcname", "NPC name", 170, true));
			gridNpcs.Columns.Add(MakeCheckColumn("connect", "Befriend"));
			gridNpcs.Columns.Add(MakeCheckColumn("donate", "Donate"));
			gridNpcs.Columns.Add(MakeCheckColumn("shop", "Shop"));
			gridNpcs.Columns.Add(MakeCheckColumn("reward", "Reward"));
			gridNpcs.Columns.Add(MakeTextColumn("usepoint", "Use points", 85));
			gridNpcs.Columns.Add(MakeTextColumn("stringidx", "String ID", 80));
			gridNpcs.Columns.Add(MakeTextColumn("dialogue", "Dialogue", 420));
			tab.Controls.Add(gridNpcs);
			return tab;
		}

		private TabPage CreateWorkTab()
		{
			TabPage tab = new("Progress Sources");
			tab.BackColor = Color.FromArgb(28, 30, 31);
			ConfigureGrid(gridWork);

			DataGridViewComboBoxColumn type = new()
			{
				Name = "type",
				HeaderText = "Type",
				Width = 90,
				FlatStyle = FlatStyle.Flat
			};
			type.Items.AddRange("ITEM", "MOB", "QUEST");
			gridWork.Columns.Add(type);
			gridWork.Columns.Add(MakeTextColumn("typeidx", "Item/Mob/Quest ID", 120));
			gridWork.Columns.Add(MakeTextColumn("name", "Name", 220, true));
			gridWork.Columns.Add(MakeTextColumn("value", "Points", 80));
			gridWork.Columns.Add(MakeTextColumn("mapid", "Map ID", 70));
			gridWork.Columns.Add(MakeTextColumn("row", "Map row", 70));
			gridWork.Columns.Add(MakeTextColumn("col", "Map col", 70));
			tab.Controls.Add(gridWork);
			return tab;
		}

		private TabPage CreateRewardTab()
		{
			TabPage tab = new("Rewards");
			tab.BackColor = Color.FromArgb(28, 30, 31);
			ConfigureGrid(gridRewards);
			gridRewards.Columns.Add(MakeTextColumn("npcidx", "Reward NPC ID", 95));
			gridRewards.Columns.Add(MakeTextColumn("npcname", "NPC name", 150, true));
			gridRewards.Columns.Add(MakeTextColumn("allowpoint", "At points", 80));
			gridRewards.Columns.Add(MakeTextColumn("itemidx", "Item ID", 80));
			gridRewards.Columns.Add(MakeTextColumn("itemname", "Item name", 190, true));
			gridRewards.Columns.Add(MakeTextColumn("count", "Count", 60));
			gridRewards.Columns.Add(MakeTextColumn("flag", "Item flag", 70));
			gridRewards.Columns.Add(MakeTextColumn("exp", "EXP", 80));
			gridRewards.Columns.Add(MakeTextColumn("sp", "SP", 80));
			gridRewards.Columns.Add(MakeTextColumn("needpclevel", "Need level", 85));
			gridRewards.Columns.Add(MakeTextColumn("needitemidx", "Need item ID", 95));
			gridRewards.Columns.Add(MakeTextColumn("needitemcount", "Need item count", 110));
			tab.Controls.Add(gridRewards);
			return tab;
		}

		private void AddField(TableLayoutPanel layout, int index, string label, TextBox textBox)
		{
			int row = index / 2;
			int col = (index % 2) * 2;
			Label l = new()
			{
				Text = label,
				Dock = DockStyle.Fill,
				TextAlign = ContentAlignment.MiddleLeft,
				AutoSize = true
			};
			StyleTextBox(textBox);
			textBox.Dock = DockStyle.Fill;
			textBox.TextChanged += MarkDirty;
			layout.Controls.Add(l, col, row);
			layout.Controls.Add(textBox, col + 1, row);
		}

		private static DataGridViewTextBoxColumn MakeTextColumn(string name, string header, int width, bool readOnly = false)
		{
			return new DataGridViewTextBoxColumn
			{
				Name = name,
				HeaderText = header,
				Width = width,
				ReadOnly = readOnly
			};
		}

		private static DataGridViewCheckBoxColumn MakeCheckColumn(string name, string header)
		{
			return new DataGridViewCheckBoxColumn
			{
				Name = name,
				HeaderText = header,
				Width = 75
			};
		}

		private void ConfigureGrid(DataGridView grid)
		{
			grid.Dock = DockStyle.Fill;
			grid.AllowUserToAddRows = true;
			grid.AllowUserToDeleteRows = true;
			grid.AllowUserToResizeRows = false;
			grid.BackgroundColor = Color.FromArgb(28, 30, 31);
			grid.BorderStyle = BorderStyle.None;
			grid.EnableHeadersVisualStyles = false;
			grid.GridColor = Color.FromArgb(91, 85, 76);
			grid.MultiSelect = false;
			grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			grid.RowHeadersWidth = 55;
			grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 56, 54);
			grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(208, 203, 148);
			grid.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
			grid.DefaultCellStyle.ForeColor = Color.FromArgb(208, 203, 148);
			grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(60, 56, 54);
			grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(208, 203, 148);
			grid.CellValueChanged += MarkDirty;
			grid.UserAddedRow += MarkDirty;
			grid.UserDeletedRow += MarkDirty;
			grid.DataError += (_, e) => { e.ThrowException = false; };
		}

		private static void StyleTextBox(TextBox textBox)
		{
			textBox.BackColor = Color.FromArgb(40, 40, 40);
			textBox.ForeColor = Color.FromArgb(208, 203, 148);
			textBox.BorderStyle = BorderStyle.FixedSingle;
		}

		private static void SetupButton(Button button, string text, int width)
		{
			button.Text = text;
			button.Width = width;
			button.Height = 27;
			button.Margin = new Padding(3);
			button.BackColor = Color.FromArgb(40, 40, 40);
			button.ForeColor = Color.FromArgb(208, 203, 148);
			button.FlatStyle = FlatStyle.Flat;
			button.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
		}

		private async void AffinityEditor_LoadAsync(object? sender, EventArgs e)
		{
			await LoadAllDataAsync();
		}

		private async Task LoadAllDataAsync()
		{
			if (!ConfirmDiscardChanges())
				return;

			UseWaitCursor = true;
			lStatus.Text = "Loading affinity data...";
			bLoading = true;
			try
			{
				string db = pMain.pSettings.DBData;
				Task<DataTable?> loadAffinities = Task.Run(() => pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT * FROM {db}.t_affinity ORDER BY a_enable DESC, a_index;", false));
				Task<DataTable?> loadNpcs = Task.Run(() => pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT * FROM {db}.t_affinity_npc ORDER BY a_affinity_idx, a_npcidx;", false));
				Task<DataTable?> loadWork = Task.Run(() => pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT * FROM {db}.t_affinity_work ORDER BY a_affinity_idx, a_work_type, a_type_idx;", false));
				Task<DataTable?> loadRewards = Task.Run(() => pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT * FROM {db}.t_affinity_reward_item ORDER BY a_npcidx, a_allow_point;", false));

				await Task.WhenAll(
					loadAffinities,
					loadNpcs,
					loadWork,
					loadRewards,
					pMain.GenericLoadNPCDataAsync(),
					pMain.GenericLoadItemDataAsync(),
					pMain.GenericLoadQuestDataAsync(),
					pMain.GenericLoadStringDataAsync()
				);

				affinityTable = loadAffinities.Result;
				affinityNpcTable = loadNpcs.Result;
				affinityWorkTable = loadWork.Result;
				affinityRewardTable = loadRewards.Result;

				PopulateAffinityList();
				if (listAffinities.SelectedIndex >= 0)
				{
					nLastListIndex = listAffinities.SelectedIndex;
					LoadSelectedAffinity();
				}
				lStatus.Text = affinityTable == null ? "Affinity tables could not be loaded. Check the DB connection." : "Loaded affinity data.";
				bDirty = false;
			}
			finally
			{
				bLoading = false;
				UseWaitCursor = false;
			}
		}

		private void PopulateAffinityList()
		{
			listAffinities.Items.Clear();

			if (affinityTable == null)
				return;

			string search = tbSearch.Text.Trim();
			foreach (DataRow row in affinityTable.Rows)
			{
				int id = RowInt(row, "a_index");
				string name = RowString(row, "a_name");
				bool enabled = RowInt(row, "a_enable", 1) != 0;
				string text = $"{id} - {name}" + (enabled ? "" : " [disabled]");
				if (search.Length > 0 && !text.Contains(search, StringComparison.OrdinalIgnoreCase))
					continue;

				listAffinities.Items.Add(new Main.ListBoxItem { ID = id, Text = text });
			}

			if (listAffinities.Items.Count > 0)
				listAffinities.SelectedIndex = 0;
		}

		private void FilterAffinityList()
		{
			if (bLoading)
				return;

			int currentId = SelectedAffinityId();
			bLoading = true;
			PopulateAffinityList();
			SelectAffinityInList(currentId);
			bLoading = false;

			if (listAffinities.SelectedIndex >= 0)
			{
				nLastListIndex = listAffinities.SelectedIndex;
				LoadSelectedAffinity();
			}
		}

		private void listAffinities_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (bLoading)
				return;

			if (!ConfirmDiscardChanges())
			{
				bLoading = true;
				listAffinities.SelectedIndex = nLastListIndex;
				bLoading = false;
				return;
			}

			nLastListIndex = listAffinities.SelectedIndex;
			LoadSelectedAffinity();
		}

		private void LoadSelectedAffinity()
		{
			if (listAffinities.SelectedItem is not Main.ListBoxItem item || affinityTable == null)
				return;

			DataRow? row = affinityTable.Select("a_index=" + item.ID).FirstOrDefault();
			if (row == null)
				return;

			bLoading = true;
			tbIndex.Text = RowInt(row, "a_index").ToString();
			tbName.Text = RowString(row, "a_name");
			tbMaxValue.Text = RowInt(row, "a_maxvalue").ToString();
			tbNas.Text = RowInt(row, "a_nas").ToString();
			tbTextureId.Text = RowInt(row, "a_texture_id").ToString();
			tbTextureRow.Text = RowInt(row, "a_texture_row").ToString();
			tbTextureCol.Text = RowInt(row, "a_texture_col").ToString();
			tbNeedItemIdx.Text = RowInt(row, "a_needitemidx").ToString();
			tbNeedItemCount.Text = RowInt(row, "a_needitemcount").ToString();
			tbNeedLevel.Text = RowInt(row, "a_needlevel").ToString();
			tbNeedAffinityIdx.Text = RowInt(row, "a_affinity_idx").ToString();
			tbNeedAffinityValue.Text = RowInt(row, "a_affinity_value").ToString();
			cbEnable.Checked = RowInt(row, "a_enable", 1) != 0;

			LoadNpcRows(item.ID);
			LoadWorkRows(item.ID);
			LoadRewardRows();

			bDirty = false;
			bLoading = false;
			lStatus.Text = $"Editing affinity {item.ID}.";
		}

		private void LoadNpcRows(int affinityId)
		{
			gridNpcs.Rows.Clear();
			if (affinityNpcTable == null)
				return;

			foreach (DataRow row in affinityNpcTable.Select("a_affinity_idx=" + affinityId))
			{
				if (RowInt(row, "a_enable", 1) == 0)
					continue;

				int npcId = RowInt(row, "a_npcidx");
				int flag = RowInt(row, "a_flag");
				int stringId = RowInt(row, "a_string_idx");
				gridNpcs.Rows.Add(
					npcId,
					NpcName(npcId),
					(flag & AF_CONNECT) != 0,
					(flag & AF_CONTRIBUTE) != 0,
					(flag & AF_SHOP) != 0,
					(flag & AF_REWARD) != 0,
					RowInt(row, "a_use_point"),
					stringId,
					StringText(stringId)
				);
			}
		}

		private void LoadWorkRows(int affinityId)
		{
			gridWork.Rows.Clear();
			if (affinityWorkTable == null)
				return;

			foreach (DataRow row in affinityWorkTable.Select("a_affinity_idx=" + affinityId))
			{
				if (RowInt(row, "a_enable", 1) == 0)
					continue;

				int type = RowInt(row, "a_work_type");
				int typeIdx = RowInt(row, "a_type_idx");
				string typeName = WorkTypeName(type);
				gridWork.Rows.Add(
					typeName,
					typeIdx,
					WorkSourceName(typeName, typeIdx),
					RowInt(row, "a_value"),
					RowInt(row, "a_id"),
					RowInt(row, "a_row"),
					RowInt(row, "a_col")
				);
			}
		}

		private void LoadRewardRows()
		{
			gridRewards.Rows.Clear();
			if (affinityRewardTable == null)
				return;

			HashSet<int> npcIds = CurrentNpcIds();
			foreach (DataRow row in affinityRewardTable.Rows)
			{
				int npcId = RowInt(row, "a_npcidx");
				if (!npcIds.Contains(npcId))
					continue;

				int itemId = RowInt(row, "a_itemidx");
				gridRewards.Rows.Add(
					npcId,
					NpcName(npcId),
					RowInt(row, "a_allow_point"),
					itemId,
					ItemName(itemId),
					RowInt(row, "a_count", 1),
					RowInt(row, "a_flag"),
					RowInt(row, "a_exp"),
					RowInt(row, "a_sp"),
					RowInt(row, "a_needpclevel"),
					RowInt(row, "a_needitemidx"),
					RowInt(row, "a_needitemcount")
				);
			}
		}

		private HashSet<int> CurrentNpcIds()
		{
			HashSet<int> ids = new();
			foreach (DataGridViewRow row in gridNpcs.Rows)
			{
				if (!row.IsNewRow)
					ids.Add(GridInt(row, "npcidx"));
			}
			return ids;
		}

		private void NewAffinity()
		{
			if (!ConfirmDiscardChanges())
				return;

			bLoading = true;
			int newId = 1;
			if (affinityTable != null && affinityTable.Rows.Count > 0)
				newId = affinityTable.AsEnumerable().Max(row => RowInt(row, "a_index")) + 1;

			tbIndex.Text = newId.ToString();
			tbName.Text = "New affinity";
			tbMaxValue.Text = "50000";
			tbNas.Text = "0";
			tbTextureId.Text = "0";
			tbTextureRow.Text = "0";
			tbTextureCol.Text = "0";
			tbNeedItemIdx.Text = "0";
			tbNeedItemCount.Text = "0";
			tbNeedLevel.Text = "0";
			tbNeedAffinityIdx.Text = "0";
			tbNeedAffinityValue.Text = "0";
			cbEnable.Checked = true;
			gridNpcs.Rows.Clear();
			gridWork.Rows.Clear();
			gridRewards.Rows.Clear();
			bLoading = false;
			bDirty = true;
			lStatus.Text = "New affinity draft. Add NPCs, progress sources, and rewards, then Save.";
		}

		private async Task SaveAffinityAsync()
		{
			int id = IntFromText(tbIndex, "Affinity ID");
			if (id <= 0)
				return;

			string db = pMain.pSettings.DBData;
			string localeColumn = "a_string_" + pMain.pSettings.WorkLocale;
			HashSet<int> rewardNpcIds = RewardNpcIdsWithValidRows();
			StringBuilder query = new();
			query.Append("START TRANSACTION;\n");
			query.Append($"INSERT INTO {db}.t_affinity (`a_index`, `a_name`, `a_maxvalue`, `a_nas`, `a_texture_id`, `a_texture_row`, `a_texture_col`, `a_needitemidx`, `a_needitemcount`, `a_needlevel`, `a_affinity_idx`, `a_affinity_value`, `a_enable`) VALUES ");
			query.Append($"({id}, '{SqlText(tbName.Text)}', {IntText(tbMaxValue)}, {IntText(tbNas)}, {IntText(tbTextureId)}, {IntText(tbTextureRow)}, {IntText(tbTextureCol)}, {IntText(tbNeedItemIdx)}, {IntText(tbNeedItemCount)}, {IntText(tbNeedLevel)}, {IntText(tbNeedAffinityIdx)}, {IntText(tbNeedAffinityValue)}, {(cbEnable.Checked ? 1 : 0)}) ");
			query.Append("ON DUPLICATE KEY UPDATE ");
			query.Append($"`a_name`=VALUES(`a_name`), `a_maxvalue`=VALUES(`a_maxvalue`), `a_nas`=VALUES(`a_nas`), `a_texture_id`=VALUES(`a_texture_id`), `a_texture_row`=VALUES(`a_texture_row`), `a_texture_col`=VALUES(`a_texture_col`), `a_needitemidx`=VALUES(`a_needitemidx`), `a_needitemcount`=VALUES(`a_needitemcount`), `a_needlevel`=VALUES(`a_needlevel`), `a_affinity_idx`=VALUES(`a_affinity_idx`), `a_affinity_value`=VALUES(`a_affinity_value`), `a_enable`=VALUES(`a_enable`);\n");

			query.Append($"DELETE FROM {db}.t_affinity_reward_item WHERE a_npcidx IN (SELECT a_npcidx FROM {db}.t_affinity_npc WHERE a_affinity_idx={id});\n");
			query.Append($"DELETE FROM {db}.t_affinity_npc WHERE a_affinity_idx={id};\n");
			query.Append($"DELETE FROM {db}.t_affinity_work WHERE a_affinity_idx={id};\n");

			foreach (DataGridViewRow row in gridNpcs.Rows)
			{
				if (row.IsNewRow)
					continue;

				int npcId = GridInt(row, "npcidx");
				if (npcId <= 0)
					continue;

				int flag = 0;
				if (GridBool(row, "connect")) flag |= AF_CONNECT;
				if (GridBool(row, "donate")) flag |= AF_CONTRIBUTE;
				if (GridBool(row, "shop")) flag |= AF_SHOP;
				if (GridBool(row, "reward"))
				{
					if (rewardNpcIds.Contains(npcId))
						flag |= AF_REWARD;
					else
						pMain.Logger(LogTypes.Warning, $"Affinity Editor > Reward flag skipped for NPC {npcId}: no valid reward rows are configured.");
				}

				int stringId = GridInt(row, "stringidx");
				query.Append($"INSERT INTO {db}.t_affinity_npc (`a_affinity_idx`, `a_npcidx`, `a_use_point`, `a_flag`, `a_string_idx`, `a_enable`) VALUES ({id}, {npcId}, {GridInt(row, "usepoint")}, {flag}, {stringId}, 1);\n");

				string dialogue = GridString(row, "dialogue");
				if (stringId > 0 && dialogue.Length > 0)
					query.Append($"UPDATE {db}.t_string SET `{localeColumn}`='{SqlText(dialogue)}' WHERE a_index={stringId};\n");
			}

			foreach (DataGridViewRow row in gridWork.Rows)
			{
				if (row.IsNewRow)
					continue;

				string typeName = GridString(row, "type");
				int type = WorkTypeIndex(typeName);
				int typeIdx = GridInt(row, "typeidx");
				if (type < 0 || typeIdx <= 0)
					continue;

				query.Append($"INSERT INTO {db}.t_affinity_work (`a_affinity_idx`, `a_work_type`, `a_type_idx`, `a_value`, `a_id`, `a_row`, `a_col`, `a_enable`) VALUES ({id}, {type}, {typeIdx}, {GridInt(row, "value")}, {GridInt(row, "mapid")}, {GridInt(row, "row")}, {GridInt(row, "col")}, 1);\n");
			}

			foreach (DataGridViewRow row in gridRewards.Rows)
			{
				if (row.IsNewRow)
					continue;

				int npcId = GridInt(row, "npcidx");
				int itemId = GridInt(row, "itemidx");
				int allowPoint = GridInt(row, "allowpoint");
				if (npcId <= 0 || itemId <= 0 || allowPoint <= 0)
					continue;

				query.Append($"INSERT INTO {db}.t_affinity_reward_item (`a_npcidx`, `a_itemidx`, `a_allow_point`, `a_flag`, `a_count`, `a_exp`, `a_sp`, `a_needpclevel`, `a_needitemidx`, `a_needitemcount`) VALUES ({npcId}, {itemId}, {allowPoint}, {GridInt(row, "flag")}, {GridInt(row, "count", 1)}, {GridInt(row, "exp")}, {GridInt(row, "sp")}, {GridInt(row, "needpclevel")}, {GridInt(row, "needitemidx")}, {GridInt(row, "needitemcount")});\n");
			}

			query.Append("COMMIT;");

			HashSet<int> savedNpcIds = CurrentNpcIds();

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query.ToString(), out long _))
			{
				string syncStatus = SyncAffinityClientHelpFiles(savedNpcIds);
				bDirty = false;
				await LoadAllDataAsync();
				SelectAffinityInList(id);
				lStatus.Text = $"Affinity {id} saved. {syncStatus}";
			}
			else
			{
				lStatus.Text = "Save failed. Check the ToolBoxNG log output.";
			}
		}

		private HashSet<int> RewardNpcIdsWithValidRows()
		{
			HashSet<int> ids = new();
			foreach (DataGridViewRow row in gridRewards.Rows)
			{
				if (row.IsNewRow)
					continue;

				int npcId = GridInt(row, "npcidx");
				int itemId = GridInt(row, "itemidx");
				int allowPoint = GridInt(row, "allowpoint");
				if (npcId > 0 && itemId > 0 && allowPoint > 0)
					ids.Add(npcId);
			}

			return ids;
		}

		private async Task DisableAffinityAsync()
		{
			int id = SelectedOrTypedAffinityId();
			if (id <= 0)
				return;

			if (MessageBox.Show($"Disable affinity {id}? This keeps the data but hides it from server/client loads.", "Affinity Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			string db = pMain.pSettings.DBData;
			StringBuilder query = new();
			query.Append("START TRANSACTION;\n");
			query.Append($"UPDATE {db}.t_affinity SET a_enable=0 WHERE a_index={id};\n");
			query.Append($"UPDATE {db}.t_affinity_npc SET a_enable=0 WHERE a_affinity_idx={id};\n");
			query.Append($"UPDATE {db}.t_affinity_work SET a_enable=0 WHERE a_affinity_idx={id};\n");
			query.Append("COMMIT;");

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, query.ToString(), out long _))
			{
				bDirty = false;
				await LoadAllDataAsync();
				SelectAffinityInList(id);
				lStatus.Text = $"Affinity {id} disabled.";
			}
		}

		private void PickNpcForActiveGrid()
		{
			NPCPicker picker = new(pMain, this, 0, true);
			if (picker.ShowDialog() != DialogResult.OK)
				return;

			int npcId = Convert.ToInt32(picker.ReturnValues[0]);
			if (npcId <= 0)
				return;

			if (tabs.SelectedTab?.Text == "Progress Sources")
			{
				DataGridViewRow row = CurrentOrNewRow(gridWork);
				row.Cells["type"].Value = "MOB";
				row.Cells["typeidx"].Value = npcId;
				row.Cells["name"].Value = NpcName(npcId);
			}
			else if (tabs.SelectedTab?.Text == "Rewards")
			{
				DataGridViewRow row = CurrentOrNewRow(gridRewards);
				row.Cells["npcidx"].Value = npcId;
				row.Cells["npcname"].Value = NpcName(npcId);
			}
			else
			{
				DataGridViewRow row = CurrentOrNewRow(gridNpcs);
				row.Cells["npcidx"].Value = npcId;
				row.Cells["npcname"].Value = NpcName(npcId);
			}

			bDirty = true;
		}

		private void PickItemForActiveTarget()
		{
			int currentItem = tabs.SelectedTab?.Text == "Affinity" ? SafeInt(tbNeedItemIdx.Text) : 0;
			ItemPicker picker = new(pMain, this, currentItem, true);
			if (picker.ShowDialog() != DialogResult.OK)
				return;

			int itemId = Convert.ToInt32(picker.ReturnValues[0]);
			if (itemId <= 0)
				return;

			if (tabs.SelectedTab?.Text == "Progress Sources")
			{
				DataGridViewRow row = CurrentOrNewRow(gridWork);
				row.Cells["type"].Value = "ITEM";
				row.Cells["typeidx"].Value = itemId;
				row.Cells["name"].Value = ItemName(itemId);
			}
			else if (tabs.SelectedTab?.Text == "Rewards")
			{
				DataGridViewRow row = CurrentOrNewRow(gridRewards);
				if (gridRewards.CurrentCell?.OwningColumn?.Name == "needitemidx")
				{
					row.Cells["needitemidx"].Value = itemId;
					row.Cells["needitemcount"].Value = Math.Max(1, GridInt(row, "needitemcount"));
				}
				else
				{
					row.Cells["itemidx"].Value = itemId;
					row.Cells["itemname"].Value = ItemName(itemId);
					row.Cells["count"].Value = Math.Max(1, GridInt(row, "count", 1));
				}
			}
			else
			{
				tbNeedItemIdx.Text = itemId.ToString();
				tbNeedItemCount.Text = Math.Max(1, SafeInt(tbNeedItemCount.Text)).ToString();
			}

			bDirty = true;
		}

		private void PickQuestForWorkGrid()
		{
			QuestPicker picker = new(pMain, this, 0);
			if (picker.ShowDialog() != DialogResult.OK)
				return;

			int questId = Convert.ToInt32(picker.ReturnValues[0]);
			if (questId <= 0)
				return;

			DataGridViewRow row = CurrentOrNewRow(gridWork);
			row.Cells["type"].Value = "QUEST";
			row.Cells["typeidx"].Value = questId;
			row.Cells["name"].Value = QuestName(questId);
			bDirty = true;
		}

		private void PickStringForNpcGrid()
		{
			StringPicker picker = new(pMain, this, 0, true);
			if (picker.ShowDialog() != DialogResult.OK)
				return;

			int stringId = Convert.ToInt32(picker.ReturnValues[0]);
			DataGridViewRow row = CurrentOrNewRow(gridNpcs);
			row.Cells["stringidx"].Value = stringId;
			row.Cells["dialogue"].Value = StringText(stringId);
			bDirty = true;
		}

		private DataGridViewRow CurrentOrNewRow(DataGridView grid)
		{
			if (grid.CurrentRow != null && !grid.CurrentRow.IsNewRow)
				return grid.CurrentRow;

			int rowIndex = grid.Rows.Add();
			return grid.Rows[rowIndex];
		}

		private bool ConfirmDiscardChanges()
		{
			if (!bDirty)
				return true;

			return MessageBox.Show("There are unsaved affinity changes. Discard them?", "Affinity Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
		}

		private void SelectAffinityInList(int affinityId)
		{
			if (affinityId <= 0)
				return;

			for (int i = 0; i < listAffinities.Items.Count; i++)
			{
				if (listAffinities.Items[i] is Main.ListBoxItem item && item.ID == affinityId)
				{
					listAffinities.SelectedIndex = i;
					return;
				}
			}
		}

		private int SelectedAffinityId()
		{
			return listAffinities.SelectedItem is Main.ListBoxItem item ? item.ID : -1;
		}

		private int SelectedOrTypedAffinityId()
		{
			int selected = SelectedAffinityId();
			return selected > 0 ? selected : SafeInt(tbIndex.Text);
		}

		private string NpcName(int npcId)
		{
			DataRow? row = pMain.pTables.NPCTable?.Select("a_index=" + npcId).FirstOrDefault();
			return row == null ? "" : RowString(row, "a_name_" + pMain.pSettings.WorkLocale);
		}

		private string ItemName(int itemId)
		{
			DataRow? row = pMain.pTables.ItemTable?.Select("a_index=" + itemId).FirstOrDefault();
			return row == null ? "" : RowString(row, "a_name_" + pMain.pSettings.WorkLocale);
		}

		private string QuestName(int questId)
		{
			DataRow? row = pMain.pTables.QuestTable?.Select("a_index=" + questId).FirstOrDefault();
			return row == null ? "" : RowString(row, "a_name_" + pMain.pSettings.WorkLocale);
		}

		private string StringText(int stringId)
		{
			DataRow? row = pMain.pTables.StringTable?.Select("a_index=" + stringId).FirstOrDefault();
			return row == null ? "" : RowString(row, "a_string_" + pMain.pSettings.WorkLocale);
		}

		private string WorkSourceName(string type, int id)
		{
			return type switch
			{
				"ITEM" => ItemName(id),
				"MOB" => NpcName(id),
				"QUEST" => QuestName(id),
				_ => ""
			};
		}

		private static string WorkTypeName(int type)
		{
			return type switch
			{
				0 => "ITEM",
				1 => "MOB",
				2 => "QUEST",
				_ => "ITEM"
			};
		}

		private static int WorkTypeIndex(string type)
		{
			return type.ToUpperInvariant() switch
			{
				"ITEM" => 0,
				"MOB" => 1,
				"QUEST" => 2,
				_ => -1
			};
		}

		private string SyncAffinityClientHelpFiles(IEnumerable<int> npcIds)
		{
			List<int> ids = npcIds.Where(id => id > 0).Distinct().OrderBy(id => id).ToList();
			if (ids.Count == 0)
				return "No affinity NPCs to sync.";

			string clientPath = pMain.pSettings.ClientPath.TrimEnd('\\');
			if (string.IsNullOrWhiteSpace(clientPath) || !Directory.Exists(clientPath))
			{
				pMain.Logger(LogTypes.Warning, "Affinity Editor > ClientPath is missing or invalid; npchelp.lod and Map.dta were not synced.");
				return "Client help sync skipped: ClientPath is invalid.";
			}

			List<AffinityNpcLocation> locations = LoadAffinityNpcLocations(ids);
			int missingCount = ids.Count - locations.Select(location => location.NpcId).Distinct().Count();
			if (locations.Count == 0)
			{
				pMain.Logger(LogTypes.Warning, "Affinity Editor > No t_npc_regen/t_shop coordinates found for the affinity NPCs; npchelp.lod and Map.dta were not synced.");
				return "Client help sync skipped: no NPC coordinates found.";
			}

			string dataPath = Path.Combine(clientPath, "Data");
			int syncedFiles = 0;

			try
			{
				string npcHelpPath = Path.Combine(dataPath, "npchelp.lod");
				if (File.Exists(npcHelpPath))
				{
					SyncNpcHelpFile(npcHelpPath, locations);
					syncedFiles++;
				}
				else
				{
					pMain.Logger(LogTypes.Warning, $"Affinity Editor > Could not sync NPC help data because this file is missing: {npcHelpPath}");
				}

				string mapPath = Path.Combine(dataPath, "Map.dta");
				if (File.Exists(mapPath))
				{
					SyncMapDataFile(mapPath, locations);
					syncedFiles++;
				}
				else
				{
					pMain.Logger(LogTypes.Warning, $"Affinity Editor > Could not sync affinity map marker data because this file is missing: {mapPath}");
				}
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Affinity Editor > Failed to sync affinity client help files: {ex.Message}");
				return "Client help sync failed; check Logs.log.";
			}

			string missingText = missingCount > 0 ? $" {missingCount} NPC(s) had no coordinates." : "";
			return syncedFiles > 0 ? $"Client help synced for {locations.Count} NPC(s).{missingText}" : $"Client help sync skipped; files missing.{missingText}";
		}

		private List<AffinityNpcLocation> LoadAffinityNpcLocations(IReadOnlyCollection<int> npcIds)
		{
			if (npcIds.Count == 0)
				return new List<AffinityNpcLocation>();

			string idList = string.Join(",", npcIds);
			string db = pMain.pSettings.DBData;
			string query =
				"SELECT npcindex, a_zone_num, a_y_layer, a_pos_x, a_pos_z, source_order FROM (" +
				$"SELECT a_npc_idx AS npcindex, a_zone_num, a_y_layer, a_pos_x, a_pos_z, 0 AS source_order FROM {db}.t_npc_regen WHERE a_npc_idx IN ({idList}) " +
				"UNION ALL " +
				$"SELECT a_keeper_idx AS npcindex, a_zone_num, a_y_layer, a_pos_x, a_pos_z, 1 AS source_order FROM {db}.t_shop WHERE a_keeper_idx IN ({idList})" +
				") affinity_locations ORDER BY npcindex, source_order;";

			DataTable? table = pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
			List<AffinityNpcLocation> locations = new();
			HashSet<int> seen = new();

			if (table == null)
				return locations;

			foreach (DataRow row in table.Rows)
			{
				int npcId = RowInt(row, "npcindex");
				if (npcId <= 0 || !seen.Add(npcId))
					continue;

				locations.Add(new AffinityNpcLocation(
					npcId,
					RowInt(row, "a_zone_num"),
					RowInt(row, "a_y_layer"),
					Convert.ToSingle(row["a_pos_x"], CultureInfo.InvariantCulture),
					Convert.ToSingle(row["a_pos_z"], CultureInfo.InvariantCulture)
				));
			}

			table.Dispose();
			return locations;
		}

		private static void SyncNpcHelpFile(string filePath, IReadOnlyCollection<AffinityNpcLocation> locations)
		{
			byte[] oldBytes = File.ReadAllBytes(filePath);
			if (oldBytes.Length < 4 || (oldBytes.Length - 4) % 8 != 0)
				throw new InvalidDataException("npchelp.lod has an unexpected layout.");

			int count = BitConverter.ToInt32(oldBytes, 0);
			if (count != (oldBytes.Length - 4) / 8)
				throw new InvalidDataException("npchelp.lod count does not match the file length.");

			HashSet<int> syncNpcIds = locations.Select(location => location.NpcId).ToHashSet();
			List<(int RawIndex, int Zone)> records = new();

			for (int i = 0; i < count; i++)
			{
				int offset = 4 + (i * 8);
				int rawIndex = BitConverter.ToInt32(oldBytes, offset);
				int npcId = rawIndex & 0x00FFFFFF;
				if (syncNpcIds.Contains(npcId))
					continue;

				records.Add((rawIndex, BitConverter.ToInt32(oldBytes, offset + 4)));
			}

			foreach (AffinityNpcLocation location in locations.OrderBy(location => location.Zone).ThenBy(location => location.NpcId))
			{
				int rawIndex = unchecked((location.Zone << 24) | (location.NpcId & 0x00FFFFFF));
				records.Add((rawIndex, location.Zone));
			}

			using MemoryStream memory = new();
			using (BinaryWriter writer = new(memory, Encoding.Default, true))
			{
				writer.Write(records.Count);
				foreach ((int rawIndex, int zone) in records)
				{
					writer.Write(rawIndex);
					writer.Write(zone);
				}
			}

			WriteIfChanged(filePath, oldBytes, memory.ToArray());
		}

		private static void SyncMapDataFile(string filePath, IReadOnlyCollection<AffinityNpcLocation> locations)
		{
			byte[] oldBytes = File.ReadAllBytes(filePath);
			List<MapZoneEntry> zones = ParseMapData(oldBytes);
			Dictionary<int, AffinityNpcLocation> locationsByNpc = locations
				.GroupBy(location => location.NpcId)
				.ToDictionary(group => group.Key, group => group.First());

			using MemoryStream memory = new();
			using BinaryWriter writer = new(memory, Encoding.Default, true);

			writer.Write(BitConverter.ToInt32(oldBytes, 0));
			int copyStart = 4;

			foreach (MapZoneEntry zone in zones)
			{
				writer.Write(oldBytes, copyStart, zone.NpcCountOffset - copyStart);

				List<MapNpcEntry> npcEntries = zone.Npcs
					.Select(npc => new MapNpcEntry { Index = npc.Index, YLayer = npc.YLayer, X = npc.X, Z = npc.Z })
					.ToList();

				foreach (AffinityNpcLocation location in locationsByNpc.Values.Where(location => location.Zone == zone.Zone && zone.Layer == 0))
				{
					int existingIndex = npcEntries.FindIndex(npc => npc.Index == location.NpcId);
					if (existingIndex >= 0)
					{
						npcEntries[existingIndex].YLayer = location.YLayer;
						npcEntries[existingIndex].X = location.X;
						npcEntries[existingIndex].Z = location.Z;
					}
					else
					{
						npcEntries.Add(new MapNpcEntry { Index = location.NpcId, YLayer = location.YLayer, X = location.X, Z = location.Z });
					}
				}

				writer.Write(npcEntries.Count);
				foreach (MapNpcEntry npc in npcEntries)
				{
					writer.Write(npc.Index);
					writer.Write(npc.YLayer);
					writer.Write(npc.X);
					writer.Write(npc.Z);
				}

				copyStart = zone.ZoneEnd;
			}

			if (copyStart < oldBytes.Length)
				writer.Write(oldBytes, copyStart, oldBytes.Length - copyStart);

			WriteIfChanged(filePath, oldBytes, memory.ToArray());
		}

		private static List<MapZoneEntry> ParseMapData(byte[] bytes)
		{
			int offset = 0;
			int zoneCount = ReadInt32(bytes, ref offset);
			List<MapZoneEntry> zones = new();

			for (int i = 0; i < zoneCount; i++)
			{
				MapZoneEntry zone = new()
				{
					Zone = ReadInt32(bytes, ref offset),
					Layer = ReadByte(bytes, ref offset)
				};

				Skip(bytes, ref offset, 28);

				int detailMapCount = ReadByte(bytes, ref offset);
				Skip(bytes, ref offset, detailMapCount * 44);

				int subZoneCount = ReadByte(bytes, ref offset);
				Skip(bytes, ref offset, subZoneCount * 13);

				zone.NpcCountOffset = offset;
				int npcCount = ReadInt32(bytes, ref offset);
				for (int j = 0; j < npcCount; j++)
				{
					zone.Npcs.Add(new MapNpcEntry
					{
						Index = ReadInt32(bytes, ref offset),
						YLayer = ReadInt32(bytes, ref offset),
						X = ReadSingle(bytes, ref offset),
						Z = ReadSingle(bytes, ref offset)
					});
				}

				zone.ZoneEnd = offset;
				zones.Add(zone);
			}

			if (offset != bytes.Length)
				throw new InvalidDataException("Map.dta has trailing or malformed data.");

			return zones;
		}

		private static int ReadInt32(byte[] bytes, ref int offset)
		{
			EnsureBytes(bytes, offset, sizeof(int));
			int value = BitConverter.ToInt32(bytes, offset);
			offset += sizeof(int);
			return value;
		}

		private static float ReadSingle(byte[] bytes, ref int offset)
		{
			EnsureBytes(bytes, offset, sizeof(float));
			float value = BitConverter.ToSingle(bytes, offset);
			offset += sizeof(float);
			return value;
		}

		private static byte ReadByte(byte[] bytes, ref int offset)
		{
			EnsureBytes(bytes, offset, sizeof(byte));
			return bytes[offset++];
		}

		private static void Skip(byte[] bytes, ref int offset, int count)
		{
			EnsureBytes(bytes, offset, count);
			offset += count;
		}

		private static void EnsureBytes(byte[] bytes, int offset, int count)
		{
			if (offset < 0 || count < 0 || offset + count > bytes.Length)
				throw new InvalidDataException("Unexpected end of file while parsing client map/help data.");
		}

		private static void WriteIfChanged(string filePath, byte[] oldBytes, byte[] newBytes)
		{
			if (oldBytes.SequenceEqual(newBytes))
				return;

			string backupPath = $"{filePath}.bak_{DateTime.Now:yyyyMMdd_HHmmssfff}";
			File.Copy(filePath, backupPath, false);
			File.WriteAllBytes(filePath, newBytes);
		}

		private static int RowInt(DataRow row, string column, int fallback = 0)
		{
			if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
				return fallback;

			return Convert.ToInt32(row[column]);
		}

		private static string RowString(DataRow row, string column)
		{
			if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
				return "";

			return row[column].ToString() ?? "";
		}

		private static int GridInt(DataGridViewRow row, string column, int fallback = 0)
		{
			object? value = row.Cells[column].Value;
			if (value == null || value == DBNull.Value || value.ToString() == "")
				return fallback;

			return Convert.ToInt32(value);
		}

		private static string GridString(DataGridViewRow row, string column)
		{
			return row.Cells[column].Value?.ToString() ?? "";
		}

		private static bool GridBool(DataGridViewRow row, string column)
		{
			object? value = row.Cells[column].Value;
			return value != null && value != DBNull.Value && Convert.ToBoolean(value);
		}

		private int IntFromText(TextBox textBox, string fieldName)
		{
			if (!int.TryParse(textBox.Text.Trim(), out int value))
			{
				MessageBox.Show($"{fieldName} must be a number.", "Affinity Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				textBox.Focus();
				return -1;
			}

			return value;
		}

		private static int SafeInt(string text)
		{
			return int.TryParse(text.Trim(), out int value) ? value : 0;
		}

		private static string IntText(TextBox textBox)
		{
			return SafeInt(textBox.Text).ToString();
		}

		private string SqlText(string value)
		{
			return pMain.EscapeChars(value);
		}

		private void MarkDirty(object? sender, EventArgs e)
		{
			if (!bLoading)
				bDirty = true;
		}
	}
}
