namespace LastChaos_ToolBoxNG
{
	public class GoodEvilEditor : Form
	{
		private const int MaxRewardBits = 31;
		private const string ServerTierMarkerBegin = "/* TOOLBOX_GOOD_EVIL_REWARD_TIERS_BEGIN */";
		private const string ServerTierMarkerEnd = "/* TOOLBOX_GOOD_EVIL_REWARD_TIERS_END */";
		private const string ClientTierMarkerBegin = "/* TOOLBOX_GOOD_EVIL_CLIENT_REWARD_CHECK_BEGIN */";
		private const string ClientTierMarkerEnd = "/* TOOLBOX_GOOD_EVIL_CLIENT_REWARD_CHECK_END */";
		private const string NameColorMarkerBegin = "/* TOOLBOX_GOOD_EVIL_NAME_COLORS_BEGIN */";
		private const string NameColorMarkerEnd = "/* TOOLBOX_GOOD_EVIL_NAME_COLORS_END */";
		private const string TitleMarkerBegin = "/* TOOLBOX_GOOD_EVIL_TITLE_REWARDS_BEGIN */";
		private const string TitleMarkerEnd = "/* TOOLBOX_GOOD_EVIL_TITLE_REWARDS_END */";

		private readonly Main pMain;
		private readonly TextBox tbSourceRoot = new();
		private readonly Button btnBrowse = new();
		private readonly Button btnReload = new();
		private readonly Button btnAddGood = new();
		private readonly Button btnAddEvil = new();
		private readonly Button btnDelete = new();
		private readonly Button btnPickTitle = new();
		private readonly Button btnSave = new();
		private readonly DataGridView grid = new();
		private readonly Label lblStatus = new();
		private readonly Dictionary<int, string> titleNames = new();

		private string configPath = "";
		private string pcPath = "";
		private string doFuncEtcPath = "";
		private string clientSessionInfoPath = "";
		private string clientNameColorPath = "";

		private readonly List<RewardRow> rewardRows = [];

		public GoodEvilEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "GoodEvilEditor";
			Text = "Good/Evil Reward Editor";
			MinimumSize = new Size(1030, 600);
			Size = new Size(1220, 700);
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
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			Label info = new()
			{
				Dock = DockStyle.Fill,
				Text = "Edits Good/Evil reward tiers, title rewards, and client name colors. Reward # is the saved claim bit, so existing rows keep their number. Rebuild GameServer and the client after saving.",
				TextAlign = ContentAlignment.MiddleLeft
			};
			root.Controls.Add(info, 0, 0);

			TableLayoutPanel pathRow = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 3
			};
			pathRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			pathRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			pathRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			root.Controls.Add(pathRow, 0, 1);

			pathRow.Controls.Add(new Label
			{
				Text = "Source root:",
				AutoSize = true,
				Margin = new Padding(0, 8, 8, 0)
			}, 0, 0);

			tbSourceRoot.Dock = DockStyle.Fill;
			pathRow.Controls.Add(tbSourceRoot, 1, 0);

			btnBrowse.Text = "Browse...";
			btnBrowse.AutoSize = true;
			btnBrowse.Click += (_, _) => BrowseSourceRoot();
			pathRow.Controls.Add(btnBrowse, 2, 0);

			ConfigureGrid();
			root.Controls.Add(grid, 0, 2);

			FlowLayoutPanel buttons = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(buttons, 0, 3);

			SetupButton(btnReload, "Reload from source", async (_, _) => await LoadEditorAsync());
			SetupButton(btnAddGood, "Add Good tier", (_, _) => AddTier(true));
			SetupButton(btnAddEvil, "Add Evil tier", (_, _) => AddTier(false));
			SetupButton(btnDelete, "Delete selected tier", (_, _) => DeleteSelectedTier());
			SetupButton(btnPickTitle, "Pick title for selected row", (_, _) => PickTitleForSelectedRow());
			SetupButton(btnSave, "Save source changes", (_, _) => SaveChanges());
			buttons.Controls.AddRange([btnReload, btnAddGood, btnAddEvil, btnDelete, btnPickTitle, btnSave]);

			lblStatus.Dock = DockStyle.Fill;
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblStatus, 0, 4);
		}

		private static void SetupButton(Button button, string text, EventHandler click)
		{
			button.Text = text;
			button.AutoSize = true;
			button.Margin = new Padding(0, 5, 8, 0);
			button.Click += click;
		}

		private void ConfigureGrid()
		{
			grid.Dock = DockStyle.Fill;
			grid.AllowUserToAddRows = false;
			grid.AllowUserToDeleteRows = false;
			grid.RowHeadersVisible = false;
			grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			grid.MultiSelect = false;
			grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

			grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "reward", HeaderText = "Reward #", ReadOnly = true, FillWeight = 60 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "side", HeaderText = "Side", ReadOnly = true, FillWeight = 75 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "points", HeaderText = "Required value", FillWeight = 100 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "titleId", HeaderText = "Title ID", FillWeight = 85 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "titleName", HeaderText = "Title name", ReadOnly = true, FillWeight = 210 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colorIndex", HeaderText = "Name color #", FillWeight = 95 });
			grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "note", HeaderText = "Meaning", ReadOnly = true, FillWeight = 320 });

			grid.CellEndEdit += (_, e) =>
			{
				if (e.RowIndex < 0)
					return;

				if (grid.Columns[e.ColumnIndex].Name == "titleId")
					UpdateTitleName(e.RowIndex);

				if (grid.Columns[e.ColumnIndex].Name == "points")
					UpdateDerivedCells(e.RowIndex);

				if (grid.Columns[e.ColumnIndex].Name == "colorIndex")
					UpdateDerivedCells(e.RowIndex);
			};
		}

		private async Task LoadEditorAsync()
		{
			try
			{
				lblStatus.Text = "Loading Good/Evil source values...";
				await Task.Run(LoadTitleNames);

				string sourceRoot = ResolveSourceRoot();
				tbSourceRoot.Text = sourceRoot;
				SetSourcePaths(sourceRoot);

				rewardRows.Clear();
				rewardRows.AddRange(LoadRewardRows());
				Dictionary<int, int> titleIds = LoadTitleRewardMapping();
				Dictionary<int, int> colorIds = LoadNameColorMapping();

				foreach (RewardRow row in rewardRows)
				{
					row.TitleId = titleIds.TryGetValue(row.RewardNum, out int titleId) ? titleId : 85 + row.RewardNum;
					row.NameColorIndex = colorIds.TryGetValue(row.PointValue, out int colorId) ? colorId : GetDefaultNameColor(row.PointValue);
				}

				PopulateGrid();
				lblStatus.Text = $"Loaded {rewardRows.Count} reward tiers from {sourceRoot}. Save changes, then rebuild GameServer and the client.";
			}
			catch (Exception ex)
			{
				lblStatus.Text = "Failed to load Good/Evil editor.";
				MessageBox.Show(ex.Message, "Good/Evil Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private string ResolveSourceRoot()
		{
			List<string> candidates = [];

			if (!string.IsNullOrWhiteSpace(tbSourceRoot.Text))
				candidates.Add(tbSourceRoot.Text.Trim());

			if (!string.IsNullOrWhiteSpace(pMain.pSettings.ServerPath))
			{
				DirectoryInfo? serverRelease = Directory.GetParent(pMain.pSettings.ServerPath);
				if (serverRelease != null)
					candidates.Add(serverRelease.FullName);
			}

			if (!string.IsNullOrWhiteSpace(pMain.pSettings.ClientPath))
			{
				DirectoryInfo? client = Directory.GetParent(pMain.pSettings.ClientPath);
				if (client?.Parent != null)
					candidates.Add(client.Parent.FullName);
			}

			DirectoryInfo? dir = new(AppContext.BaseDirectory);
			for (int i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
			{
				candidates.Add(Path.Combine(dir.FullName, "RezaRePack1776"));
				candidates.Add(dir.FullName);
			}

			foreach (string candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (IsSourceRoot(candidate))
					return Path.GetFullPath(candidate);
			}

			throw new InvalidOperationException("Could not locate the RezaRePack1776 source root. Browse to the RezaRePack1776 folder that contains Server\\ShareLib\\Config.h.");
		}

		private static bool IsSourceRoot(string path)
		{
			return File.Exists(Path.Combine(path, "Server", "ShareLib", "Config.h"))
				&& File.Exists(Path.Combine(path, "Server", "GameServer", "PC.cpp"))
				&& File.Exists(Path.Combine(path, "Server", "GameServer", "doFuncEtc.cpp"))
				&& File.Exists(Path.Combine(path, "SRCDesign", "Engine", "Network", "SessionStateInfo.cpp"));
		}

		private void SetSourcePaths(string sourceRoot)
		{
			configPath = Path.Combine(sourceRoot, "Server", "ShareLib", "Config.h");
			pcPath = Path.Combine(sourceRoot, "Server", "GameServer", "PC.cpp");
			doFuncEtcPath = Path.Combine(sourceRoot, "Server", "GameServer", "doFuncEtc.cpp");
			clientSessionInfoPath = Path.Combine(sourceRoot, "SRCDesign", "Engine", "Network", "SessionStateInfo.cpp");
			clientNameColorPath = Path.Combine(sourceRoot, "SRCDesign", "Engine", "Interface", "GoodEvilNameColor.h");
		}

		private List<RewardRow> LoadRewardRows()
		{
			string pcText = ReadLegacyText(pcPath);
			List<RewardRow> rows = [];

			Match generated = Regex.Match(pcText, Regex.Escape(ServerTierMarkerBegin) + @"(?<body>.*?)" + Regex.Escape(ServerTierMarkerEnd), RegexOptions.Singleline);
			if (generated.Success)
			{
				foreach (Match match in Regex.Matches(generated.Groups["body"].Value, @"\{\s*(\d+)\s*,\s*(-?\d+)\s*\}"))
				{
					rows.Add(new RewardRow(
						int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
						int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
						0,
						9));
				}
			}

			if (rows.Count == 0)
			{
				Dictionary<string, int> pointValues = LoadLegacyPointDefines();
				rows.Add(new RewardRow(0, pointValues["PK_CHAOTIC_POINT_MAX"], 85, 19));
				rows.Add(new RewardRow(1, pointValues["PK_CHAOTIC_POINT_2"], 86, 18));
				rows.Add(new RewardRow(2, pointValues["PK_CHAOTIC_POINT_1"], 87, 17));
				rows.Add(new RewardRow(3, pointValues["PK_HUNTER_POINT_1"], 88, 16));
				rows.Add(new RewardRow(4, pointValues["PK_HUNTER_POINT_2"], 89, 15));
				rows.Add(new RewardRow(5, pointValues["PK_HUNTER_POINT_MAX"], 90, 14));
			}

			return rows.OrderBy(row => row.PointValue).ThenBy(row => row.RewardNum).ToList();
		}

		private Dictionary<string, int> LoadLegacyPointDefines()
		{
			string text = ReadLegacyText(configPath);
			string[] defineNames =
			[
				"PK_CHAOTIC_POINT_MAX",
				"PK_CHAOTIC_POINT_2",
				"PK_CHAOTIC_POINT_1",
				"PK_HUNTER_POINT_1",
				"PK_HUNTER_POINT_2",
				"PK_HUNTER_POINT_MAX"
			];

			Dictionary<string, int> values = [];
			foreach (string defineName in defineNames)
			{
				Match match = Regex.Match(text, $@"(?m)^\s*#define\s+{Regex.Escape(defineName)}\s+(-?\d+)");
				if (!match.Success)
					throw new InvalidOperationException($"Could not find {defineName} in Config.h.");

				values[defineName] = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
			}

			return values;
		}

		private Dictionary<int, int> LoadTitleRewardMapping()
		{
			string text = ReadLegacyText(doFuncEtcPath);
			Dictionary<int, int> titleIds = [];

			Match generated = Regex.Match(text, Regex.Escape(TitleMarkerBegin) + @"(?<body>.*?)" + Regex.Escape(TitleMarkerEnd), RegexOptions.Singleline);
			if (generated.Success)
			{
				foreach (Match match in Regex.Matches(generated.Groups["body"].Value, @"case\s+(\d+)\s*:\s*titleIndex\s*=\s*(\d+)\s*;"))
					titleIds[int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture)] = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
			}
			else if (Regex.IsMatch(text, @"titleIndex\s*=\s*85\s*\+\s*rewardNum\s*;"))
			{
				for (int i = 0; i < 6; i++)
					titleIds[i] = 85 + i;
			}

			return titleIds;
		}

		private Dictionary<int, int> LoadNameColorMapping()
		{
			Dictionary<int, int> colorIds = [];
			if (!File.Exists(clientNameColorPath))
				return colorIds;

			string text = ReadLegacyText(clientNameColorPath);
			Match generated = Regex.Match(text, Regex.Escape(NameColorMarkerBegin) + @"(?<body>.*?)" + Regex.Escape(NameColorMarkerEnd), RegexOptions.Singleline);
			if (!generated.Success)
				return colorIds;

			foreach (Match match in Regex.Matches(generated.Groups["body"].Value, @"\{\s*(-?\d+)\s*,\s*(\d+)\s*\}"))
			{
				int pointValue = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
				int colorIndex = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
				colorIds[pointValue] = colorIndex;
			}

			return colorIds;
		}

		private void LoadTitleNames()
		{
			titleNames.Clear();

			try
			{
				string locale = string.IsNullOrWhiteSpace(pMain.pSettings.WorkLocale) ? "usa" : pMain.pSettings.WorkLocale.ToLowerInvariant();
				string query =
					"SELECT t.a_index AS TitleID, " +
					$"COALESCE(NULLIF(i.a_name_{locale}, ''), CONCAT('Item ', t.a_item_index)) AS TitleName, " +
					"t.a_item_index AS ItemID " +
					$"FROM {pMain.pSettings.DBData}.t_title t " +
					$"LEFT JOIN {pMain.pSettings.DBData}.t_item i ON i.a_index = t.a_item_index " +
					"ORDER BY t.a_index;";

				DataTable? table = pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
				if (table == null)
					return;

				foreach (DataRow row in table.Rows)
				{
					int titleId = Convert.ToInt32(row["TitleID"], CultureInfo.InvariantCulture);
					titleNames[titleId] = $"{row["TitleName"]} (item {row["ItemID"]})";
				}
			}
			catch
			{
				// The editor can still work with raw title IDs if DB title lookup is unavailable.
			}
		}

		private void PopulateGrid()
		{
			grid.Rows.Clear();
			foreach (RewardRow row in rewardRows.OrderBy(row => row.PointValue).ThenBy(row => row.RewardNum))
			{
				int index = grid.Rows.Add(row.RewardNum, GetSide(row.PointValue), row.PointValue, row.TitleId, ResolveTitleName(row.TitleId), row.NameColorIndex, GetMeaning(row));
				grid.Rows[index].Tag = row;
			}
		}

		private void AddTier(bool goodTier)
		{
			try
			{
				ReadGridValues();
				ValidateRewardNumbersOnly();

				int rewardNum = Enumerable.Range(0, MaxRewardBits).FirstOrDefault(i => rewardRows.All(row => row.RewardNum != i));
				if (rewardRows.Any(row => row.RewardNum == rewardNum))
					throw new InvalidOperationException($"All {MaxRewardBits} reward bits are already used. The current database flag cannot safely store more tiers.");

				int pointValue;
				if (goodTier)
				{
					int maxGood = rewardRows.Where(row => row.PointValue > 0).Select(row => row.PointValue).DefaultIfEmpty(0).Max();
					pointValue = maxGood + 1000;
				}
				else
				{
					int minEvil = rewardRows.Where(row => row.PointValue < 0).Select(row => row.PointValue).DefaultIfEmpty(0).Min();
					pointValue = minEvil - 1000;
				}

				int defaultColor = goodTier
					? rewardRows.Where(row => row.PointValue > 0).OrderByDescending(row => row.PointValue).Select(row => row.NameColorIndex).DefaultIfEmpty(GetDefaultNameColor(pointValue)).First()
					: rewardRows.Where(row => row.PointValue < 0).OrderBy(row => row.PointValue).Select(row => row.NameColorIndex).DefaultIfEmpty(GetDefaultNameColor(pointValue)).First();

				RewardRow row = new(rewardNum, pointValue, 85 + rewardNum, defaultColor);
				rewardRows.Add(row);
				PopulateGrid();
				SelectRewardRow(rewardNum);
				lblStatus.Text = $"Added {(goodTier ? "Good" : "Evil")} tier using reward #{rewardNum}. Set required value, title, and name color before saving.";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Good/Evil Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void DeleteSelectedTier()
		{
			if (grid.CurrentRow?.Tag is not RewardRow selected)
				return;

			ReadGridValues();
			if (rewardRows.Count <= 2)
			{
				MessageBox.Show("Keep at least one Good tier and one Evil tier.", "Good/Evil Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			DialogResult result = MessageBox.Show(
				$"Delete reward #{selected.RewardNum}? Existing characters that already claimed this reward keep their database bit, but this tier will no longer grant a title.",
				"Good/Evil Editor",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);

			if (result != DialogResult.Yes)
				return;

			rewardRows.RemoveAll(row => row.RewardNum == selected.RewardNum);
			PopulateGrid();
		}

		private void PickTitleForSelectedRow()
		{
			if (grid.CurrentRow == null)
				return;

			int currentTitleId = ToInt(grid.CurrentRow.Cells["titleId"].Value);
			TitlePicker picker = new(pMain, this, currentTitleId);
			if (picker.ShowDialog(this) != DialogResult.OK)
				return;

			int titleId = Convert.ToInt32(picker.ReturnValues[0], CultureInfo.InvariantCulture);
			if (titleId <= 0)
				return;

			grid.CurrentRow.Cells["titleId"].Value = titleId;
			UpdateTitleName(grid.CurrentRow.Index);
		}

		private void UpdateTitleName(int rowIndex)
		{
			int titleId = ToInt(grid.Rows[rowIndex].Cells["titleId"].Value);
			grid.Rows[rowIndex].Cells["titleName"].Value = ResolveTitleName(titleId);
		}

		private void UpdateDerivedCells(int rowIndex)
		{
			if (grid.Rows[rowIndex].Tag is not RewardRow row)
				return;

			row.PointValue = ToInt(grid.Rows[rowIndex].Cells["points"].Value);
			row.NameColorIndex = ToInt(grid.Rows[rowIndex].Cells["colorIndex"].Value);
			grid.Rows[rowIndex].Cells["side"].Value = GetSide(row.PointValue);
			grid.Rows[rowIndex].Cells["note"].Value = GetMeaning(row);
		}

		private string ResolveTitleName(int titleId)
		{
			return titleNames.TryGetValue(titleId, out string? name) ? name : $"Title {titleId}";
		}

		private static string GetSide(int pointValue)
		{
			if (pointValue < 0)
				return "Evil";

			if (pointValue > 0)
				return "Good";

			return "Neutral";
		}

		private static string GetMeaning(RewardRow row)
		{
			string colorText = $" Name color index: {row.NameColorIndex}.";

			if (row.PointValue < 0)
				return $"Granted when Good/Evil is at or below {row.PointValue}. This color applies from this value toward neutral.{colorText} Claim bit: 1 << {row.RewardNum}.";

			if (row.PointValue > 0)
				return $"Granted when Good/Evil is at or above {row.PointValue}. This color applies from the previous Good tier up to this value.{colorText} Claim bit: 1 << {row.RewardNum}.";

			return "Point value 0 is not allowed.";
		}

		private static int GetDefaultNameColor(int pointValue)
		{
			if (pointValue < 0)
			{
				if (pointValue <= -32000)
					return 19;
				if (pointValue <= -19000)
					return 18;
				return 17;
			}

			if (pointValue > 0)
			{
				if (pointValue >= 32000)
					return 14;
				if (pointValue >= 19000)
					return 15;
				return 16;
			}

			return 9;
		}

		private void BrowseSourceRoot()
		{
			using FolderBrowserDialog dialog = new()
			{
				Description = "Select the RezaRePack1776 source folder",
				SelectedPath = Directory.Exists(tbSourceRoot.Text) ? tbSourceRoot.Text : AppContext.BaseDirectory
			};

			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				tbSourceRoot.Text = dialog.SelectedPath;
				SetSourcePaths(tbSourceRoot.Text);
			}
		}

		private void SaveChanges()
		{
			try
			{
				SetSourcePaths(tbSourceRoot.Text.Trim());
				if (!IsSourceRoot(tbSourceRoot.Text.Trim()))
					throw new InvalidOperationException("Source root is invalid. Select the RezaRePack1776 folder.");

				ReadGridValues();
				ValidateValues();
				rewardRows.Sort((left, right) =>
				{
					int valueCompare = left.PointValue.CompareTo(right.PointValue);
					return valueCompare != 0 ? valueCompare : left.RewardNum.CompareTo(right.RewardNum);
				});

				SavePointDefines();
				SaveServerRewardLogic();
				SaveClientRewardLogic();
				SaveNameColorHelper();
				SaveTitleRewardMapping();

				PopulateGrid();
				lblStatus.Text = "Saved Good/Evil source changes. Rebuild GameServer and the client for them to take effect.";
				MessageBox.Show("Good/Evil source changes saved.\n\nRebuild GameServer and the client before testing.", "Good/Evil Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				lblStatus.Text = "Save failed.";
				MessageBox.Show(ex.Message, "Good/Evil Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ReadGridValues()
		{
			foreach (DataGridViewRow gridRow in grid.Rows)
			{
				if (gridRow.Tag is not RewardRow row)
					continue;

				row.PointValue = ToInt(gridRow.Cells["points"].Value);
				row.TitleId = ToInt(gridRow.Cells["titleId"].Value);
				row.NameColorIndex = ToInt(gridRow.Cells["colorIndex"].Value);
			}
		}

		private void ValidateValues()
		{
			ValidateRewardNumbersOnly();

			if (rewardRows.Count == 0)
				throw new InvalidOperationException("At least one reward tier is required.");

			if (rewardRows.Count > MaxRewardBits)
				throw new InvalidOperationException($"The existing reward flag can store at most {MaxRewardBits} tiers.");

			if (!rewardRows.Any(row => row.PointValue < 0))
				throw new InvalidOperationException("Keep at least one Evil tier so the negative cap remains valid.");

			if (!rewardRows.Any(row => row.PointValue > 0))
				throw new InvalidOperationException("Keep at least one Good tier so the positive cap remains valid.");

			List<int> duplicatePoints = rewardRows
				.GroupBy(row => row.PointValue)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToList();

			if (duplicatePoints.Count > 0)
				throw new InvalidOperationException($"Duplicate required value: {duplicatePoints[0]}. Every tier needs a unique point value.");

			foreach (RewardRow row in rewardRows)
			{
				if (row.PointValue == 0)
					throw new InvalidOperationException($"Reward #{row.RewardNum} uses value 0. Use a positive Good value or negative Evil value.");

				if (row.TitleId <= 0)
					throw new InvalidOperationException($"Reward #{row.RewardNum} needs a valid Title ID.");

				if (row.NameColorIndex < 0 || row.NameColorIndex > 19)
					throw new InvalidOperationException($"Reward #{row.RewardNum} uses name color #{row.NameColorIndex}. Use an existing target-name color index from 0 to 19.");
			}
		}

		private void ValidateRewardNumbersOnly()
		{
			foreach (RewardRow row in rewardRows)
			{
				if (row.RewardNum < 0 || row.RewardNum >= MaxRewardBits)
					throw new InvalidOperationException($"Reward #{row.RewardNum} is outside the supported range 0-{MaxRewardBits - 1}.");
			}

			List<int> duplicateRewardNums = rewardRows
				.GroupBy(row => row.RewardNum)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToList();

			if (duplicateRewardNums.Count > 0)
				throw new InvalidOperationException($"Duplicate reward number: {duplicateRewardNums[0]}.");
		}

		private void SavePointDefines()
		{
			string text = ReadLegacyText(configPath);
			List<int> evilValues = rewardRows.Where(row => row.PointValue < 0).Select(row => row.PointValue).OrderBy(value => value).ToList();
			List<int> goodValues = rewardRows.Where(row => row.PointValue > 0).Select(row => row.PointValue).OrderBy(value => value).ToList();

			int evilCap = evilValues.First();
			int evilClosest = evilValues.Last();
			int evilSecond = evilValues.Count >= 2 ? evilValues[^2] : evilClosest;
			int goodFirst = goodValues.First();
			int goodSecond = goodValues.Count >= 2 ? goodValues[1] : goodFirst;
			int goodCap = goodValues.Last();

			text = ReplaceDefine(text, "PK_CHAOTIC_POINT_MAX", evilCap);
			text = ReplaceDefine(text, "PK_CHAOTIC_POINT_2", evilSecond);
			text = ReplaceDefine(text, "PK_CHAOTIC_POINT_1", evilClosest);
			text = ReplaceDefine(text, "PK_HUNTER_POINT_1", goodFirst);
			text = ReplaceDefine(text, "PK_HUNTER_POINT_2", goodSecond);
			text = ReplaceDefine(text, "PK_HUNTER_POINT_MAX", goodCap);

			WriteLegacyText(configPath, text);
		}

		private static string ReplaceDefine(string text, string defineName, int value)
		{
			Regex regex = new($@"(?m)^(\s*#define\s+{Regex.Escape(defineName)}\s+)-?\d+");
			if (!regex.IsMatch(text))
				throw new InvalidOperationException($"Could not find {defineName} in Config.h.");

			return regex.Replace(text, $"${{1}}{value.ToString(CultureInfo.InvariantCulture)}", 1);
		}

		private void SaveServerRewardLogic()
		{
			string text = ReadLegacyText(pcPath);
			string function = BuildServerRewardFunction();
			Regex regex = new(@"int\s+CPC::GetPKPenaltyRewardNum\s*\(\s*\)\s*\{.*?\r?\n\}\s*(?=void\s+CPC::AddPkPenalty)", RegexOptions.Singleline);
			if (!regex.IsMatch(text))
				throw new InvalidOperationException("Could not find CPC::GetPKPenaltyRewardNum in PC.cpp.");

			text = regex.Replace(text, function + Environment.NewLine, 1);
			WriteLegacyText(pcPath, text);
		}

		private string BuildServerRewardFunction()
		{
			StringBuilder sb = new();
			sb.AppendLine("int CPC::GetPKPenaltyRewardNum()");
			sb.AppendLine("{");
			sb.AppendLine($"\t{ServerTierMarkerBegin}");
			sb.AppendLine("\tstruct SPKPenaltyRewardTier");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tint rewardNum;");
			sb.AppendLine("\t\tint point;");
			sb.AppendLine("\t};");
			sb.AppendLine();
			sb.AppendLine("\tstatic const SPKPenaltyRewardTier rewardTiers[] =");
			sb.AppendLine("\t{");
			foreach (RewardRow row in rewardRows.OrderBy(row => row.PointValue).ThenBy(row => row.RewardNum))
				sb.AppendLine($"\t\t{{ {row.RewardNum}, {row.PointValue} }}, // {GetSide(row.PointValue)}");
			sb.AppendLine("\t};");
			sb.AppendLine();
			sb.AppendLine("\tconst int rewardCount = sizeof(rewardTiers) / sizeof(rewardTiers[0]);");
			sb.AppendLine("\tfor (int i = 0; i < rewardCount; ++i)");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tint rewardNum = rewardTiers[i].rewardNum;");
			sb.AppendLine("\t\tint point = rewardTiers[i].point;");
			sb.AppendLine("\t\tbool eligible = false;");
			sb.AppendLine();
			sb.AppendLine("\t\tif (point < 0 && m_pkPenalty <= point)");
			sb.AppendLine("\t\t\teligible = true;");
			sb.AppendLine("\t\telse if (point > 0 && m_pkPenalty >= point)");
			sb.AppendLine("\t\t\teligible = true;");
			sb.AppendLine();
			sb.AppendLine("\t\tif (eligible && !CheckPKPenaltyReward(rewardNum))");
			sb.AppendLine("\t\t\treturn rewardNum;");
			sb.AppendLine("\t}");
			sb.AppendLine($"\t{ServerTierMarkerEnd}");
			sb.AppendLine();
			sb.AppendLine("\treturn -1;");
			sb.Append("}");
			return sb.ToString();
		}

		private void SaveClientRewardLogic()
		{
			string text = ReadLegacyText(clientSessionInfoPath);
			string block = BuildClientRewardBlock();
			Regex regex = new(@"#ifdef\s+NEW_CHAO_SYS.*?#endif\s*//NEW_CHAO_SYS", RegexOptions.Singleline);
			if (!regex.IsMatch(text))
				throw new InvalidOperationException("Could not find the NEW_CHAO_SYS reward block in SessionStateInfo.cpp.");

			text = regex.Replace(text, block, 1);
			WriteLegacyText(clientSessionInfoPath, text);
		}

		private string BuildClientRewardBlock()
		{
			StringBuilder sb = new();
			sb.AppendLine("#ifdef NEW_CHAO_SYS");
			sb.AppendLine($"\t{ClientTierMarkerBegin}");
			sb.AppendLine("\tstruct SPKPenaltyRewardTier");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tLONG rewardNum;");
			sb.AppendLine("\t\tLONG point;");
			sb.AppendLine("\t};");
			sb.AppendLine();
			sb.AppendLine("\tstatic const SPKPenaltyRewardTier rewardTiers[] =");
			sb.AppendLine("\t{");
			foreach (RewardRow row in rewardRows.OrderBy(row => row.PointValue).ThenBy(row => row.RewardNum))
				sb.AppendLine($"\t\t{{ {row.RewardNum}, {row.PointValue} }}, // {GetSide(row.PointValue)}");
			sb.AppendLine("\t};");
			sb.AppendLine();
			sb.AppendLine("\tconst int rewardCount = sizeof(rewardTiers) / sizeof(rewardTiers[0]);");
			sb.AppendLine("\tfor (int i = 0; i < rewardCount; ++i)");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tLONG rewardNum = rewardTiers[i].rewardNum;");
			sb.AppendLine("\t\tLONG point = rewardTiers[i].point;");
			sb.AppendLine("\t\tBOOL eligible = FALSE;");
			sb.AppendLine();
			sb.AppendLine("\t\tif (point < 0 && pPack->pkPenalty <= point)");
			sb.AppendLine("\t\t\teligible = TRUE;");
			sb.AppendLine("\t\telse if (point > 0 && pPack->pkPenalty >= point)");
			sb.AppendLine("\t\t\teligible = TRUE;");
			sb.AppendLine();
			sb.AppendLine("\t\tif (eligible && rewardNum >= 0 && rewardNum < 31 && !((1UL << rewardNum) & _pNetwork->MyCharacterInfo.pkSysRewardFlag))");
			sb.AppendLine("\t\t{");
			sb.AppendLine("\t\t\tif (_pNetwork->MyCharacterInfo.bpkSysRewardLate == FALSE)");
			sb.AppendLine("\t\t\t{");
			sb.AppendLine("\t\t\t\t_pNetwork->pkPenaltyReformRewardReq(rewardNum);");
			sb.AppendLine("\t\t\t\t_pNetwork->MyCharacterInfo.bpkSysRewardLate = TRUE;");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t\tbreak;");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine($"\t{ClientTierMarkerEnd}");
			sb.Append("#endif //NEW_CHAO_SYS");
			return sb.ToString();
		}

		private void SaveNameColorHelper()
		{
			WriteLegacyText(clientNameColorPath, BuildNameColorHelper());
		}

		private string BuildNameColorHelper()
		{
			StringBuilder sb = new();
			sb.AppendLine("#ifndef GOOD_EVIL_NAME_COLOR_H");
			sb.AppendLine("#define GOOD_EVIL_NAME_COLOR_H");
			sb.AppendLine();
			sb.AppendLine("static inline int GetGoodEvilNameColorIndex(SLONG pkPenalty, BOOL useGoodEvilColor)");
			sb.AppendLine("{");
			sb.AppendLine("\tif (!useGoodEvilColor)");
			sb.AppendLine("\t\treturn 9;");
			sb.AppendLine();
			sb.AppendLine($"\t{NameColorMarkerBegin}");
			sb.AppendLine("\tstruct SGoodEvilNameColorTier");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tSLONG point;");
			sb.AppendLine("\t\tint colorIndex;");
			sb.AppendLine("\t};");
			sb.AppendLine();
			sb.AppendLine("\tstatic const SGoodEvilNameColorTier tiers[] =");
			sb.AppendLine("\t{");

			foreach (RewardRow row in rewardRows.Where(row => row.PointValue < 0).OrderByDescending(row => row.PointValue))
				sb.AppendLine($"\t\t{{ {row.PointValue}, {row.NameColorIndex} }}, // {GetSide(row.PointValue)}");

			foreach (RewardRow row in rewardRows.Where(row => row.PointValue > 0).OrderBy(row => row.PointValue))
				sb.AppendLine($"\t\t{{ {row.PointValue}, {row.NameColorIndex} }}, // {GetSide(row.PointValue)}");

			sb.AppendLine("\t};");
			sb.AppendLine($"\t{NameColorMarkerEnd}");
			sb.AppendLine();
			sb.AppendLine("\tconst int tierCount = sizeof(tiers) / sizeof(tiers[0]);");
			sb.AppendLine("\tfor (int i = 0; i < tierCount; ++i)");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tif (pkPenalty < 0 && tiers[i].point < 0 && pkPenalty >= tiers[i].point)");
			sb.AppendLine("\t\t\treturn tiers[i].colorIndex;");
			sb.AppendLine("\t\tif (pkPenalty > 0 && tiers[i].point > 0 && pkPenalty <= tiers[i].point)");
			sb.AppendLine("\t\t\treturn tiers[i].colorIndex;");
			sb.AppendLine("\t}");
			sb.AppendLine();
			sb.AppendLine("\tif (pkPenalty < 0)");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tfor (int i = tierCount - 1; i >= 0; --i)");
			sb.AppendLine("\t\t{");
			sb.AppendLine("\t\t\tif (tiers[i].point < 0)");
			sb.AppendLine("\t\t\t\treturn tiers[i].colorIndex;");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("\telse if (pkPenalty > 0)");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tfor (int i = tierCount - 1; i >= 0; --i)");
			sb.AppendLine("\t\t{");
			sb.AppendLine("\t\t\tif (tiers[i].point > 0)");
			sb.AppendLine("\t\t\t\treturn tiers[i].colorIndex;");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine();
			sb.AppendLine("\treturn 9;");
			sb.AppendLine("}");
			sb.AppendLine();
			sb.AppendLine("#endif // GOOD_EVIL_NAME_COLOR_H");
			return sb.ToString();
		}

		private void SaveTitleRewardMapping()
		{
			string text = ReadLegacyText(doFuncEtcPath);
			string block = BuildTitleRewardBlock();
			string pattern = Regex.Escape(TitleMarkerBegin) + @".*?" + Regex.Escape(TitleMarkerEnd);

			if (Regex.IsMatch(text, pattern, RegexOptions.Singleline))
			{
				text = Regex.Replace(text, pattern, block, RegexOptions.Singleline);
			}
			else
			{
				Regex oldFormula = new(@"titleIndex\s*=\s*85\s*\+\s*rewardNum\s*;", RegexOptions.Multiline);
				if (!oldFormula.IsMatch(text))
					throw new InvalidOperationException("Could not find the existing PK title reward mapping in doFuncEtc.cpp.");

				text = oldFormula.Replace(text, block, 1);
			}

			WriteLegacyText(doFuncEtcPath, text);
		}

		private string BuildTitleRewardBlock()
		{
			StringBuilder sb = new();
			sb.AppendLine(TitleMarkerBegin);
			sb.AppendLine("\tswitch (rewardNum)");
			sb.AppendLine("\t{");
			foreach (RewardRow row in rewardRows.OrderBy(row => row.RewardNum))
				sb.AppendLine($"\tcase {row.RewardNum}: titleIndex = {row.TitleId}; break; // {GetSide(row.PointValue)} {row.PointValue}");
			sb.AppendLine("\tdefault: titleIndex = -1; break;");
			sb.Append("\t}");
			sb.AppendLine();
			sb.Append(TitleMarkerEnd);
			return sb.ToString();
		}

		private void SelectRewardRow(int rewardNum)
		{
			foreach (DataGridViewRow row in grid.Rows)
			{
				if (ToInt(row.Cells["reward"].Value) != rewardNum)
					continue;

				row.Selected = true;
				grid.CurrentCell = row.Cells["points"];
				break;
			}
		}

		private static int ToInt(object? value)
		{
			if (value == null)
				return 0;

			string text = Convert.ToString(value, CultureInfo.InvariantCulture)?.Trim() ?? "";
			return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;
		}

		private static string ReadLegacyText(string path)
		{
			return File.ReadAllText(path, Encoding.Latin1);
		}

		private static void WriteLegacyText(string path, string text)
		{
			File.WriteAllText(path, text, Encoding.Latin1);
		}

		private sealed class RewardRow(int rewardNum, int pointValue, int titleId, int nameColorIndex)
		{
			public int RewardNum { get; } = rewardNum;
			public int PointValue { get; set; } = pointValue;
			public int TitleId { get; set; } = titleId;
			public int NameColorIndex { get; set; } = nameColorIndex;
		}
	}
}
