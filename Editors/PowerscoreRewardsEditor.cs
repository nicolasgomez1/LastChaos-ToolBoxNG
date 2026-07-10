using System.ComponentModel;

namespace LastChaos_ToolBoxNG
{
	public class PowerscoreRewardsEditor : Form
	{
		private const string DefinitionFileName = "PowerscoreRewards.xml";
		private const string WindowTitle = "Powerscore Reward Editor";

		private readonly Main pMain;
		private readonly BindingSource bindingSource = new();
		private readonly DataGridView grid = new();
		private readonly TextBox tbPath = new();
		private readonly Button btnOpen = new();
		private readonly Button btnReload = new();
		private readonly Button btnSave = new();
		private readonly Button btnSaveAs = new();
		private readonly Button btnOpenFolder = new();
		private readonly Button btnAdd = new();
		private readonly Button btnDuplicate = new();
		private readonly Button btnPickItem = new();
		private readonly Button btnDelete = new();
		private readonly Label lblStatus = new();

		private BindingList<PowerscoreRewardTier> tiers = [];
		private string? currentFile;
		private bool loading;
		private bool dirty;

		public PowerscoreRewardsEditor(Main mainForm)
		{
			pMain = mainForm;

			Name = "PowerscoreRewardsEditor";
			Text = WindowTitle;
			MinimumSize = new Size(1040, 620);
			Size = new Size(1260, 740);
			StartPosition = FormStartPosition.CenterParent;
			KeyPreview = true;

			BuildLayout();
			ReplaceTiers([]);

			Load += async (_, _) => await LoadInitialDefinitionAsync();
			FormClosing += PowerscoreRewardsEditor_FormClosing;
			KeyDown += (_, e) =>
			{
				if (e.Control && e.KeyCode == Keys.S)
				{
					e.SuppressKeyPress = true;
					SaveCurrentFile();
				}
			};
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
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
			Controls.Add(root);

			root.Controls.Add(new Label
			{
				Dock = DockStyle.Fill,
				Text = "Edits the Powerscore reward tiers displayed by the client. Milestone IDs and Powerscore thresholds must be unique; the server remains authoritative for progress and reward claims.",
				TextAlign = ContentAlignment.MiddleLeft
			}, 0, 0);

			TableLayoutPanel fileBar = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 7,
				RowCount = 1
			};
			fileBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			fileBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			for (int i = 2; i < fileBar.ColumnCount; ++i)
				fileBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			root.Controls.Add(fileBar, 0, 1);

			fileBar.Controls.Add(new Label
			{
				AutoSize = true,
				Text = "Definition:",
				Margin = new Padding(0, 10, 6, 0)
			}, 0, 0);

			tbPath.Dock = DockStyle.Fill;
			tbPath.ReadOnly = true;
			tbPath.Margin = new Padding(0, 6, 8, 5);
			fileBar.Controls.Add(tbPath, 1, 0);

			ConfigureButton(btnOpen, "Open...", async (_, _) => await OpenDefinitionAsync());
			ConfigureButton(btnReload, "Reload", async (_, _) => await ReloadCurrentDefinitionAsync());
			ConfigureButton(btnSave, "Save", (_, _) => SaveCurrentFile());
			ConfigureButton(btnSaveAs, "Save As...", (_, _) => SaveAs());
			ConfigureButton(btnOpenFolder, "Open folder", (_, _) => OpenCurrentFolder());
			fileBar.Controls.Add(btnOpen, 2, 0);
			fileBar.Controls.Add(btnReload, 3, 0);
			fileBar.Controls.Add(btnSave, 4, 0);
			fileBar.Controls.Add(btnSaveAs, 5, 0);
			fileBar.Controls.Add(btnOpenFolder, 6, 0);

			FlowLayoutPanel actions = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(actions, 0, 2);

			ConfigureButton(btnAdd, "Add tier", (_, _) => AddTier());
			ConfigureButton(btnDuplicate, "Duplicate tier", (_, _) => DuplicateSelectedTier());
			ConfigureButton(btnPickItem, "Pick reward item", (_, _) => PickItemForSelectedTier());
			ConfigureButton(btnDelete, "Delete tier", (_, _) => DeleteSelectedTier());
			actions.Controls.AddRange([btnAdd, btnDuplicate, btnPickItem, btnDelete]);

			ConfigureGrid();
			root.Controls.Add(grid, 0, 3);

			lblStatus.Dock = DockStyle.Fill;
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			root.Controls.Add(lblStatus, 0, 4);
		}

		private static void ConfigureButton(Button button, string text, EventHandler handler)
		{
			button.Text = text;
			button.AutoSize = true;
			button.Margin = new Padding(0, 5, 7, 0);
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
			grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			grid.DataError += (_, e) =>
			{
				e.ThrowException = false;
				SetStatus("That value is not valid for this column.", true);
			};
			grid.CurrentCellDirtyStateChanged += (_, _) =>
			{
				if (grid.IsCurrentCellDirty)
					grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
			};
			grid.CellValueChanged += (_, _) => MarkDirty();
			grid.CellEndEdit += async (_, e) =>
			{
				if (e.RowIndex < 0 || e.ColumnIndex < 0 || grid.Columns[e.ColumnIndex].DataPropertyName != nameof(PowerscoreRewardTier.ItemIndex))
					return;

				if (grid.Rows[e.RowIndex].DataBoundItem is PowerscoreRewardTier tier)
					await RefreshItemNamesAsync([tier]);
			};
			grid.CellDoubleClick += (_, e) =>
			{
				if (e.RowIndex < 0 || e.ColumnIndex < 0)
					return;

				string property = grid.Columns[e.ColumnIndex].DataPropertyName;
				if (property is nameof(PowerscoreRewardTier.ItemIndex) or nameof(PowerscoreRewardTier.ItemName))
					PickItemForSelectedTier();
			};

			DataGridViewCellStyle numberStyle = new() { Alignment = DataGridViewContentAlignment.MiddleRight };
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(PowerscoreRewardTier.Id),
				HeaderText = "Milestone ID",
				FillWeight = 75,
				DefaultCellStyle = numberStyle
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(PowerscoreRewardTier.Powerscore),
				HeaderText = "Required PC",
				FillWeight = 85,
				DefaultCellStyle = numberStyle
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(PowerscoreRewardTier.Label),
				HeaderText = "Tier label",
				FillWeight = 130
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(PowerscoreRewardTier.ItemIndex),
				HeaderText = "Reward item ID",
				FillWeight = 85,
				DefaultCellStyle = numberStyle
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(PowerscoreRewardTier.ItemName),
				HeaderText = "Reward item",
				ReadOnly = true,
				FillWeight = 190
			});
			grid.Columns.Add(new DataGridViewTextBoxColumn
			{
				DataPropertyName = nameof(PowerscoreRewardTier.Quantity),
				HeaderText = "Quantity",
				FillWeight = 65,
				DefaultCellStyle = numberStyle
			});

			grid.DataSource = bindingSource;
		}

		private async Task LoadInitialDefinitionAsync()
		{
			currentFile = GetConfiguredDefinitionPath();
			UpdatePathDisplay();

			if (currentFile != null && File.Exists(currentFile))
			{
				await LoadDefinitionAsync(currentFile, false);
				return;
			}

			ReplaceTiers([]);
			string path = currentFile ?? "the configured client Data\\Interface folder";
			SetStatus($"{DefinitionFileName} was not found at {path}. Use Open... to select the active client definition.", true);
		}

		private async Task OpenDefinitionAsync()
		{
			if (!PromptToSaveDirty())
				return;

			using OpenFileDialog dialog = new()
			{
				Filter = "Powerscore reward definition (PowerscoreRewards.xml)|PowerscoreRewards.xml|XML files (*.xml)|*.xml|All files (*.*)|*.*",
				InitialDirectory = GetInitialDirectory(),
				FileName = DefinitionFileName
			};

			if (dialog.ShowDialog(this) == DialogResult.OK)
				await LoadDefinitionAsync(dialog.FileName, false);
		}

		private async Task ReloadCurrentDefinitionAsync()
		{
			if (currentFile == null || !File.Exists(currentFile))
			{
				SetStatus("There is no existing definition file to reload.", true);
				return;
			}

			await LoadDefinitionAsync(currentFile, true);
		}

		private async Task LoadDefinitionAsync(string path, bool promptForDirty)
		{
			if (promptForDirty && !PromptToSaveDirty())
				return;

			SetBusy(true, $"Loading {Path.GetFileName(path)}...");
			try
			{
				List<PowerscoreRewardTier> loaded = await Task.Run(() => PowerscoreRewardsDefinition.Load(path));
				Dictionary<int, string> itemNames = await Task.Run(() => QueryItemNames(loaded.Select(t => t.ItemIndex)));
				ApplyItemNames(loaded, itemNames);

				currentFile = Path.GetFullPath(path);
				ReplaceTiers(loaded);
				UpdatePathDisplay();
				dirty = false;
				UpdateWindowTitle();
				SetBusy(false, $"Loaded {tiers.Count} Powerscore reward tiers. Save writes them in ascending Powerscore order.");
			}
			catch (Exception ex)
			{
				SetBusy(false, "Could not load the Powerscore reward definition.");
				MessageBox.Show(this, ex.Message, WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddTier()
		{
			PowerscoreRewardTier? selected = GetSelectedTier();
			PowerscoreRewardTier tier = new()
			{
				Id = GetNextId(),
				Powerscore = GetNextPowerscore(),
				Label = "New Tier",
				ItemIndex = selected?.ItemIndex ?? tiers.FirstOrDefault()?.ItemIndex ?? 85,
				ItemName = selected?.ItemName ?? tiers.FirstOrDefault()?.ItemName ?? "Item 85",
				Quantity = 1
			};

			tiers.Add(tier);
			bindingSource.Position = tiers.Count - 1;
			MarkDirty();
			SetStatus($"Added milestone {tier.Id}. Set its Powerscore, label, and reward before saving.");
		}

		private void DuplicateSelectedTier()
		{
			PowerscoreRewardTier? selected = GetSelectedTier();
			if (selected == null)
				return;

			string label = selected.Label.Trim();
			if (label.Length > 59)
				label = label[..59];

			PowerscoreRewardTier tier = selected.Copy();
			tier.Id = GetNextId();
			tier.Powerscore = GetNextPowerscore();
			tier.Label = label + " Copy";
			tiers.Add(tier);
			bindingSource.Position = tiers.Count - 1;
			MarkDirty();
			SetStatus($"Duplicated the selected reward as milestone {tier.Id}.");
		}

		private void PickItemForSelectedTier()
		{
			PowerscoreRewardTier? tier = GetSelectedTier();
			if (tier == null)
				return;

			using ItemPicker picker = new(pMain, this, tier.ItemIndex, true);
			if (picker.ShowDialog(this) != DialogResult.OK)
				return;

			tier.ItemIndex = Convert.ToInt32(picker.ReturnValues[0]);
			tier.ItemName = picker.ReturnValues[1]?.ToString() ?? $"Item {tier.ItemIndex}";
			bindingSource.ResetCurrentItem();
			MarkDirty();
			SetStatus($"Selected {tier.ItemName} ({tier.ItemIndex}) for milestone {tier.Id}.");
		}

		private void DeleteSelectedTier()
		{
			PowerscoreRewardTier? tier = GetSelectedTier();
			if (tier == null)
				return;

			DialogResult result = MessageBox.Show(
				this,
				$"Delete milestone {tier.Id} at {tier.Powerscore} PC?",
				WindowTitle,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);
			if (result != DialogResult.Yes)
				return;

			tiers.Remove(tier);
			MarkDirty();
			SetStatus($"Removed milestone {tier.Id}. Save to update the definition file.");
		}

		private bool SaveCurrentFile()
		{
			if (currentFile == null)
				return SaveAs();

			return SaveTo(currentFile);
		}

		private bool SaveAs()
		{
			using SaveFileDialog dialog = new()
			{
				Filter = "Powerscore reward definition (PowerscoreRewards.xml)|PowerscoreRewards.xml|XML files (*.xml)|*.xml",
				InitialDirectory = GetInitialDirectory(),
				FileName = DefinitionFileName,
				OverwritePrompt = true
			};

			if (dialog.ShowDialog(this) != DialogResult.OK)
				return false;

			return SaveTo(dialog.FileName);
		}

		private bool SaveTo(string path)
		{
			Validate();
			grid.EndEdit();
			bindingSource.EndEdit();

			List<PowerscoreRewardTier> values = tiers.Select(t => t.Copy()).ToList();
			string? validation = PowerscoreRewardsDefinition.Validate(values);
			if (validation != null)
			{
				SetStatus(validation, true);
				MessageBox.Show(this, validation, WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			SetBusy(true, $"Saving {DefinitionFileName}...");
			try
			{
				string backup = PowerscoreRewardsDefinition.Save(path, values);
				currentFile = Path.GetFullPath(path);
				List<PowerscoreRewardTier> ordered = values.OrderBy(t => t.Powerscore).ThenBy(t => t.Id).ToList();
				ReplaceTiers(ordered);
				UpdatePathDisplay();
				dirty = false;
				UpdateWindowTitle();

				string backupText = string.IsNullOrEmpty(backup) ? "No previous file existed." : $"Backup: {Path.GetFileName(backup)}";
				SetBusy(false, $"Saved {tiers.Count} tiers to {DefinitionFileName}. {backupText}");
				pMain.Logger(LogTypes.Success, $"Powerscore Reward Editor > Saved {currentFile}", false);
				return true;
			}
			catch (Exception ex)
			{
				SetBusy(false, "Could not save the Powerscore reward definition.");
				MessageBox.Show(this, ex.Message, WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		private async Task RefreshItemNamesAsync(IReadOnlyCollection<PowerscoreRewardTier> values)
		{
			foreach (PowerscoreRewardTier tier in values)
				tier.ItemName = tier.ItemIndex > 0 ? $"Item {tier.ItemIndex}" : "Invalid item";

			Dictionary<int, string> itemNames = await Task.Run(() => QueryItemNames(values.Select(t => t.ItemIndex)));
			ApplyItemNames(values, itemNames);
			bindingSource.ResetBindings(false);
		}

		private Dictionary<int, string> QueryItemNames(IEnumerable<int> itemIds)
		{
			int[] ids = itemIds.Where(id => id > 0).Distinct().Order().ToArray();
			if (ids.Length == 0)
				return [];

			string locale = Regex.IsMatch(pMain.pSettings.WorkLocale ?? "", "^[A-Za-z0-9_]+$")
				? pMain.pSettings.WorkLocale.ToLowerInvariant()
				: "usa";
			string localizedName = $"a_name_{locale}";
			string query =
				"SELECT a_index, " +
				$"COALESCE(NULLIF({localizedName}, ''), NULLIF(a_name_usa, ''), NULLIF(a_name, ''), CONCAT('Item ', a_index)) AS ItemName " +
				$"FROM {pMain.pSettings.DBData}.t_item WHERE a_index IN ({string.Join(',', ids)});";

			DataTable? table = pMain.QuerySelect(pMain.pSettings.DBCharset, query, false);
			if (table == null)
				return [];

			Dictionary<int, string> names = [];
			foreach (DataRow row in table.Rows)
			{
				if (!int.TryParse(row["a_index"]?.ToString(), out int id))
					continue;
				names[id] = row["ItemName"]?.ToString() ?? $"Item {id}";
			}
			return names;
		}

		private static void ApplyItemNames(IEnumerable<PowerscoreRewardTier> values, IReadOnlyDictionary<int, string> itemNames)
		{
			foreach (PowerscoreRewardTier tier in values)
				tier.ItemName = itemNames.TryGetValue(tier.ItemIndex, out string? name) ? name : $"Item {tier.ItemIndex}";
		}

		private void ReplaceTiers(IEnumerable<PowerscoreRewardTier> values)
		{
			loading = true;
			tiers.ListChanged -= TierListChanged;
			tiers = new BindingList<PowerscoreRewardTier>(values.ToList());
			tiers.ListChanged += TierListChanged;
			bindingSource.DataSource = tiers;
			loading = false;
		}

		private void TierListChanged(object? sender, ListChangedEventArgs e)
		{
			if (!loading)
				MarkDirty();
		}

		private PowerscoreRewardTier? GetSelectedTier()
		{
			return grid.CurrentRow?.DataBoundItem as PowerscoreRewardTier;
		}

		private int GetNextId()
		{
			int highest = tiers.Select(t => t.Id).DefaultIfEmpty(1000).Max();
			if (highest == int.MaxValue)
				throw new InvalidOperationException("No additional milestone ID can be generated.");
			return Math.Max(1001, highest + 1);
		}

		private int GetNextPowerscore()
		{
			int[] scores = tiers.Select(t => t.Powerscore).Where(score => score > 0).Distinct().Order().ToArray();
			if (scores.Length == 0)
				return 250;

			int gap = scores.Length > 1 ? scores[^1] - scores[^2] : Math.Max(250, scores[^1]);
			gap = Math.Max(1, gap);
			long candidate = (long)scores[^1] + gap;
			if (candidate > int.MaxValue)
				throw new InvalidOperationException("No additional Powerscore threshold can be generated.");
			return (int)candidate;
		}

		private string? GetConfiguredDefinitionPath()
		{
			if (string.IsNullOrWhiteSpace(pMain.pSettings.ClientPath))
				return null;
			return Path.Combine(pMain.pSettings.ClientPath.TrimEnd('\\', '/'), "Data", "Interface", DefinitionFileName);
		}

		private string GetInitialDirectory()
		{
			string? currentFolder = currentFile == null ? null : Path.GetDirectoryName(currentFile);
			if (currentFolder != null && Directory.Exists(currentFolder))
				return currentFolder;

			string? configured = GetConfiguredDefinitionPath();
			string? configuredFolder = configured == null ? null : Path.GetDirectoryName(configured);
			if (configuredFolder != null && Directory.Exists(configuredFolder))
				return configuredFolder;

			return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}

		private void OpenCurrentFolder()
		{
			string? folder = currentFile == null ? null : Path.GetDirectoryName(currentFile);
			if (folder == null || !Directory.Exists(folder))
			{
				SetStatus("The current definition folder does not exist.", true);
				return;
			}

			Process.Start(new ProcessStartInfo(folder) { UseShellExecute = true });
		}

		private bool PromptToSaveDirty()
		{
			if (!dirty)
				return true;

			DialogResult result = MessageBox.Show(
				this,
				"Save changes before continuing?",
				WindowTitle,
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question);
			if (result == DialogResult.Cancel)
				return false;
			if (result == DialogResult.Yes)
				return SaveCurrentFile();
			return true;
		}

		private void PowerscoreRewardsEditor_FormClosing(object? sender, FormClosingEventArgs e)
		{
			if (!PromptToSaveDirty())
				e.Cancel = true;
		}

		private void MarkDirty()
		{
			if (loading)
				return;
			dirty = true;
			UpdateWindowTitle();
		}

		private void UpdatePathDisplay()
		{
			tbPath.Text = currentFile ?? "No definition selected";
			btnOpenFolder.Enabled = currentFile != null && Directory.Exists(Path.GetDirectoryName(currentFile));
		}

		private void UpdateWindowTitle()
		{
			string file = currentFile == null ? "" : $" - {Path.GetFileName(currentFile)}";
			Text = WindowTitle + file + (dirty ? " *" : "");
		}

		private void SetBusy(bool busy, string message)
		{
			UseWaitCursor = busy;
			grid.Enabled = !busy;
			btnOpen.Enabled = !busy;
			btnReload.Enabled = !busy;
			btnSave.Enabled = !busy;
			btnSaveAs.Enabled = !busy;
			btnOpenFolder.Enabled = !busy && currentFile != null && Directory.Exists(Path.GetDirectoryName(currentFile));
			btnAdd.Enabled = !busy;
			btnDuplicate.Enabled = !busy;
			btnPickItem.Enabled = !busy;
			btnDelete.Enabled = !busy;
			SetStatus(message);
		}

		private void SetStatus(string message, bool error = false)
		{
			lblStatus.Text = message;
			lblStatus.ForeColor = error ? Color.Firebrick : Color.FromArgb(70, 70, 70);
		}
	}
}
