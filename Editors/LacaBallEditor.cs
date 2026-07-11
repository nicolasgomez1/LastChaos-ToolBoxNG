using System.Data;
using System.Text;

namespace LastChaos_ToolBoxNG
{
	public partial class LacaBallEditor : Form
	{
		private const int DefaultTokenItemId = 5123;
		private const int DefaultRewardItemId = 19;
		private const int DefaultRewardRowsPerCourse = 5;

		private readonly Main pMain;
		private bool bLoading = false;
		private bool bUnsavedChanges = false;
		private bool bCurrentTableIsTemporary = false;
		private int nSearchPosition = 0;
		private int nOriginalTokenId = -1;
		private Main.ListBoxItem? pLastSelected;

		private ListBox MainList = null!;
		private TextBox tbSearch = null!;
		private TextBox tbItemOrder = null!;
		private Button btnTokenItem = null!;
		private Button btnReload = null!;
		private Button btnAddNew = null!;
		private Button btnCopy = null!;
		private Button btnDelete = null!;
		private Button btnUpdate = null!;
		private Button btnAddCourse = null!;
		private Button btnAddReward = null!;
		private Button btnChooseReward = null!;
		private Button btnRemoveReward = null!;
		private DataGridView gridRewards = null!;

		public LacaBallEditor(Main mainForm)
		{
			pMain = mainForm;
			BuildUi();
		}

		private async Task LoadLacaBallDataAsync()
		{
			DataTable? pNewTable = await Task.Run(() =>
			{
				return pMain.QuerySelect(
					pMain.pSettings.DBCharset,
					$"SELECT a_item_order, a_tocken_index, a_course_code, a_order, a_item_index, a_item_count, a_item_max, a_item_remain FROM {pMain.pSettings.DBUser}.t_lcball ORDER BY a_item_order, a_tocken_index, a_course_code, a_order;"
				);
			});

			pMain.pTables.LacaBallTable?.Dispose();
			pMain.pTables.LacaBallTable = pNewTable;
		}

		private async void LacaBallEditor_LoadAsync(object? sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading LacaBall data...");

			await Task.WhenAll(
				LoadLacaBallDataAsync(),
				pMain.GenericLoadItemDataAsync()
			);

			pProgressDialog.Close();

			if (pMain.pTables.LacaBallTable == null)
			{
				MessageBox.Show("Could not load ep4_db.t_lcball. Check database settings.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			FillMainList();
			SetLoadedState(true);

			if (MainList.Items.Count > 0)
				MainList.SelectedIndex = 0;
		}

		private void FillMainList()
		{
			MainList.BeginUpdate();
			MainList.Items.Clear();

			foreach (DataRow pRow in GetTokenHeaderRows())
				AddTokenToList(Convert.ToInt32(pRow["a_tocken_index"]), Convert.ToInt32(pRow["a_item_order"]));

			MainList.EndUpdate();
		}

		private IEnumerable<DataRow> GetTokenHeaderRows()
		{
			if (pMain.pTables.LacaBallTable == null)
				yield break;

			foreach (DataRow pRow in pMain.pTables.LacaBallTable
				.AsEnumerable()
				.GroupBy(row => Convert.ToInt32(row["a_tocken_index"]))
				.Select(group => group.OrderBy(row => Convert.ToInt32(row["a_item_order"])).First())
				.OrderBy(row => Convert.ToInt32(row["a_item_order"]))
				.ThenBy(row => Convert.ToInt32(row["a_tocken_index"])))
			{
				yield return pRow;
			}
		}

		private void AddTokenToList(int nTokenId, int nItemOrder)
		{
			MainList.Items.Add(new Main.ListBoxItem
			{
				ID = nTokenId,
				Text = $"{nItemOrder}: {GetItemText(nTokenId)}"
			});
		}

		private void LoadUiData(int nTokenId)
		{
			if (pMain.pTables.LacaBallTable == null)
				return;

			bLoading = true;
			gridRewards.Rows.Clear();

			DataRow[] pRows = pMain.pTables.LacaBallTable
				.AsEnumerable()
				.Where(row => Convert.ToInt32(row["a_tocken_index"]) == nTokenId)
				.OrderBy(row => Convert.ToInt32(row["a_course_code"]))
				.ThenBy(row => Convert.ToInt32(row["a_order"]))
				.ToArray();

			if (pRows.Length == 0)
			{
				bLoading = false;
				return;
			}

			nOriginalTokenId = nTokenId;
			bCurrentTableIsTemporary = false;
			tbItemOrder.Text = pRows.Min(row => Convert.ToInt32(row["a_item_order"])).ToString();
			SetTokenButton(nTokenId);

			foreach (DataRow pRow in pRows)
			{
				AddRewardRow(
					Convert.ToInt32(pRow["a_course_code"]),
					Convert.ToInt32(pRow["a_order"]),
					Convert.ToInt32(pRow["a_item_index"]),
					Convert.ToInt64(pRow["a_item_count"]),
					Convert.ToInt64(pRow["a_item_max"]),
					Convert.ToInt64(pRow["a_item_remain"])
				);
			}

			bUnsavedChanges = false;
			btnUpdate.Enabled = false;
			bLoading = false;
		}

		private void LoadTemporaryTable(int nTokenId, int nItemOrder, IEnumerable<LcBallRewardRow>? rows = null)
		{
			bLoading = true;
			nOriginalTokenId = -1;
			bCurrentTableIsTemporary = true;
			tbItemOrder.Text = nItemOrder.ToString();
			SetTokenButton(nTokenId);
			gridRewards.Rows.Clear();

			if (rows == null)
			{
				for (int i = 0; i < DefaultRewardRowsPerCourse; i++)
					AddRewardRow(0, i, DefaultRewardItemId, 1, 1, 1);
			}
			else
			{
				foreach (LcBallRewardRow row in rows)
					AddRewardRow(row.Course, row.Order, row.ItemId, row.Count, row.Max, row.Remain);
			}

			bLoading = false;
			MarkDirty();
		}

		private void AddRewardRow(int nCourse, int nOrder, int nItemId, long lCount, long lMax, long lRemain)
		{
			int nRow = gridRewards.Rows.Add();
			gridRewards.Rows[nRow].Cells["course"].Value = nCourse;
			gridRewards.Rows[nRow].Cells["order"].Value = nOrder;
			gridRewards.Rows[nRow].Cells["itemIcon"].Value = GetItemIcon(nItemId);
			gridRewards.Rows[nRow].Cells["item"].Value = GetItemText(nItemId);
			gridRewards.Rows[nRow].Cells["item"].Tag = nItemId;
			gridRewards.Rows[nRow].Cells["count"].Value = lCount;
			gridRewards.Rows[nRow].Cells["max"].Value = lMax;
			gridRewards.Rows[nRow].Cells["remain"].Value = lRemain;
		}

		private bool CheckUnsavedChanges()
		{
			if (!bUnsavedChanges)
				return true;

			DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (pDialogReturn != DialogResult.Yes)
				return false;

			if (bCurrentTableIsTemporary)
				RemoveTemporaryListItem();

			bUnsavedChanges = false;
			btnUpdate.Enabled = false;
			return true;
		}

		private void RemoveTemporaryListItem()
		{
			int nTokenId = GetCurrentTokenId();

			for (int i = MainList.Items.Count - 1; i >= 0; i--)
			{
				if (MainList.Items[i] is Main.ListBoxItem item && item.ID == nTokenId)
					MainList.Items.RemoveAt(i);
			}

			bCurrentTableIsTemporary = false;
		}

		private void MarkDirty()
		{
			if (bLoading)
				return;

			bUnsavedChanges = true;
			btnUpdate.Enabled = true;
		}

		private int GetCurrentTokenId()
		{
			return Convert.ToInt32(btnTokenItem.Tag ?? -1);
		}

		private void SetTokenButton(int nTokenId)
		{
			btnTokenItem.Tag = nTokenId;
			btnTokenItem.Text = GetItemText(nTokenId);
			btnTokenItem.Image = GetItemIcon(nTokenId);
		}

		private int GetNextItemOrder()
		{
			if (pMain.pTables.LacaBallTable == null || pMain.pTables.LacaBallTable.Rows.Count == 0)
				return 0;

			return pMain.pTables.LacaBallTable.AsEnumerable().Max(row => Convert.ToInt32(row["a_item_order"])) + 1;
		}

		private int GetNextCourseCode()
		{
			if (gridRewards.Rows.Count == 0)
				return 0;

			return gridRewards.Rows
				.Cast<DataGridViewRow>()
				.Select(row => TryGetIntCell(row, "course", out int nCourse) ? nCourse : 0)
				.DefaultIfEmpty(0)
				.Max() + 1;
		}

		private int GetNextOrderInCourse(int nCourse)
		{
			return gridRewards.Rows
				.Cast<DataGridViewRow>()
				.Where(row => TryGetIntCell(row, "course", out int nRowCourse) && nRowCourse == nCourse)
				.Select(row => TryGetIntCell(row, "order", out int nOrder) ? nOrder : -1)
				.DefaultIfEmpty(-1)
				.Max() + 1;
		}

		private bool TokenExists(int nTokenId, int nIgnoredOriginalTokenId)
		{
			if (pMain.pTables.LacaBallTable == null)
				return false;

			return pMain.pTables.LacaBallTable
				.AsEnumerable()
				.Any(row =>
					Convert.ToInt32(row["a_tocken_index"]) == nTokenId &&
					Convert.ToInt32(row["a_tocken_index"]) != nIgnoredOriginalTokenId
				);
		}

		private bool TryComposeRows(out LcBallSaveData saveData)
		{
			saveData = new LcBallSaveData();

			saveData.TokenId = GetCurrentTokenId();
			saveData.OriginalTokenId = nOriginalTokenId;

			if (saveData.TokenId <= 0)
			{
				MessageBox.Show("Please choose the item consumed to play this LacaBall table.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (TokenExists(saveData.TokenId, saveData.OriginalTokenId))
			{
				MessageBox.Show("That consumed item already has a LacaBall reward table. Choose a different item or edit the existing table.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (!int.TryParse(tbItemOrder.Text, out saveData.ItemOrder) || saveData.ItemOrder < 0)
			{
				MessageBox.Show("Display order must be 0 or higher.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			foreach (DataGridViewRow pRow in gridRewards.Rows)
			{
				if (!TryGetIntCell(pRow, "course", out int nCourse) || nCourse < 0 || nCourse > 127)
				{
					MessageBox.Show("Course must be between 0 and 127.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}

				if (!TryGetIntCell(pRow, "order", out int nOrder) || nOrder < 0 || nOrder > 127)
				{
					MessageBox.Show("Slot/order must be between 0 and 127.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}

				int nItemId = Convert.ToInt32(pRow.Cells["item"].Tag ?? 0);
				if (nItemId <= 0)
				{
					MessageBox.Show("Every reward row needs a valid item.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}

				if (!TryGetUnsignedIntCell(pRow, "count", out long lCount) ||
					!TryGetUnsignedIntCell(pRow, "max", out long lMax) ||
					!TryGetUnsignedIntCell(pRow, "remain", out long lRemain))
				{
					MessageBox.Show("Reward count/max/remaining values must be unsigned 32-bit numbers.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}

				if (lRemain > lMax)
				{
					DialogResult pDialogReturn = MessageBox.Show("One reward has Remaining above Max. Save anyway?", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						return false;
				}

				saveData.Rows.Add(new LcBallRewardRow(nCourse, nOrder, nItemId, lCount, lMax, lRemain));
			}

			if (saveData.Rows.Count == 0)
			{
				MessageBox.Show("Please add at least one reward row.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			IEnumerable<IGrouping<string, LcBallRewardRow>> duplicateGroups = saveData.Rows
				.GroupBy(row => $"{row.Course}:{row.Order}")
				.Where(group => group.Count() > 1);

			if (duplicateGroups.Any())
			{
				MessageBox.Show("Two or more reward rows use the same Course + Slot/order. Each row needs a unique pair.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			saveData.Rows = saveData.Rows
				.OrderBy(row => row.Course)
				.ThenBy(row => row.Order)
				.ToList();

			return true;
		}

		private bool SaveRows(LcBallSaveData saveData)
		{
			StringBuilder strQuery = new();
			strQuery.Append("START TRANSACTION;\n");

			if (saveData.OriginalTokenId > 0)
				strQuery.Append($"DELETE FROM {pMain.pSettings.DBUser}.t_lcball WHERE a_tocken_index={saveData.OriginalTokenId};\n");

			if (saveData.OriginalTokenId != saveData.TokenId)
				strQuery.Append($"DELETE FROM {pMain.pSettings.DBUser}.t_lcball WHERE a_tocken_index={saveData.TokenId};\n");

			strQuery.Append($"INSERT INTO {pMain.pSettings.DBUser}.t_lcball (a_item_order, a_tocken_index, a_course_code, a_order, a_item_index, a_item_count, a_item_max, a_item_remain) VALUES ");

			foreach (LcBallRewardRow row in saveData.Rows)
			{
				strQuery.Append($"({saveData.ItemOrder}, {saveData.TokenId}, {row.Course}, {row.Order}, {row.ItemId}, {row.Count}, {row.Max}, {row.Remain}),");
			}

			strQuery.Length--;
			strQuery.Append(";\nCOMMIT;");

			return pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strQuery.ToString(), out long _);
		}

		private async Task ReloadAfterSaveAsync(int nTokenId)
		{
			await LoadLacaBallDataAsync();
			FillMainList();

			for (int i = 0; i < MainList.Items.Count; i++)
			{
				if (MainList.Items[i] is Main.ListBoxItem item && item.ID == nTokenId)
				{
					MainList.SelectedIndex = i;
					break;
				}
			}

			bCurrentTableIsTemporary = false;
			bUnsavedChanges = false;
			btnUpdate.Enabled = false;
		}

		private void ChooseTokenItem()
		{
			ItemPicker pItemSelector = new(pMain, this, GetCurrentTokenId() > 0 ? GetCurrentTokenId() : DefaultTokenItemId, false);
			if (pItemSelector.ShowDialog() != DialogResult.OK)
				return;

			int nItemId = Convert.ToInt32(pItemSelector.ReturnValues[0]);
			SetTokenButton(nItemId);
			MarkDirty();
		}

		private void ChooseRewardItem()
		{
			if (gridRewards.CurrentRow == null)
				return;

			int nItemId = Convert.ToInt32(gridRewards.CurrentRow.Cells["item"].Tag ?? DefaultRewardItemId);
			ItemPicker pItemSelector = new(pMain, this, nItemId, false);
			if (pItemSelector.ShowDialog() != DialogResult.OK)
				return;

			nItemId = Convert.ToInt32(pItemSelector.ReturnValues[0]);
			gridRewards.CurrentRow.Cells["itemIcon"].Value = GetItemIcon(nItemId);
			gridRewards.CurrentRow.Cells["item"].Value = GetItemText(nItemId);
			gridRewards.CurrentRow.Cells["item"].Tag = nItemId;
			MarkDirty();
		}

		private string GetItemText(int nItemId)
		{
			if (nItemId <= 0)
				return "0 - No item";

			DataRow? pItemRow = pMain.pTables.ItemTable?.Select("a_index=" + nItemId).FirstOrDefault();
			if (pItemRow == null)
				return $"{nItemId} - ITEM NOT FOUND";

			return $"{nItemId} - {pItemRow["a_name_" + pMain.pSettings.WorkLocale]}";
		}

		private Image? GetItemIcon(int nItemId)
		{
			if (nItemId <= 0)
				return null;

			DataRow? pItemRow = pMain.pTables.ItemTable?.Select("a_index=" + nItemId).FirstOrDefault();
			if (pItemRow == null)
				return null;

			Bitmap? pIcon = pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"]));
			if (pIcon == null)
				return null;

			return new Bitmap(pIcon, new Size(24, 24));
		}

		private static bool TryGetIntCell(DataGridViewRow pRow, string strCell, out int nValue)
		{
			return int.TryParse(pRow.Cells[strCell].Value?.ToString(), out nValue);
		}

		private static bool TryGetUnsignedIntCell(DataGridViewRow pRow, string strCell, out long lValue)
		{
			if (!long.TryParse(pRow.Cells[strCell].Value?.ToString(), out lValue))
				return false;

			return lValue >= 0 && lValue <= uint.MaxValue;
		}

		private void SetLoadedState(bool bLoaded)
		{
			MainList.Enabled = bLoaded;
			btnReload.Enabled = bLoaded;
			btnAddNew.Enabled = bLoaded;
			btnCopy.Enabled = bLoaded && MainList.Items.Count > 0;
			btnDelete.Enabled = bLoaded && MainList.Items.Count > 0;
			btnTokenItem.Enabled = bLoaded;
			gridRewards.Enabled = bLoaded;
			btnAddCourse.Enabled = bLoaded;
			btnAddReward.Enabled = bLoaded;
			btnChooseReward.Enabled = bLoaded;
			btnRemoveReward.Enabled = bLoaded;
		}

		private void BuildUi()
		{
			Text = "LacaBall Reward Editor";
			Name = "LacaBallEditor";
			StartPosition = FormStartPosition.CenterScreen;
			BackColor = Color.FromArgb(40, 40, 40);
			ForeColor = Color.FromArgb(208, 203, 148);
			MinimumSize = new Size(1160, 650);
			ClientSize = new Size(1200, 690);
			Icon = Properties.Resources.NG;

			tbSearch = CreateTextBox(new Point(12, 12), new Size(300, 23));
			tbSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			tbSearch.TextChanged += (_sender, _args) => nSearchPosition = 0;
			tbSearch.KeyDown += (_sender, args) => nSearchPosition = pMain.SearchInListBox(tbSearch, args, MainList, nSearchPosition);

			MainList = new ListBox
			{
				BackColor = Color.FromArgb(28, 30, 31),
				ForeColor = Color.FromArgb(208, 203, 148),
				BorderStyle = BorderStyle.FixedSingle,
				Enabled = false,
				Location = new Point(12, 42),
				Size = new Size(300, 575),
				Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
			};
			MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

			btnReload = CreateButton("Reload", new Point(12, 632), new Size(70, 28));
			btnReload.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnReload.Enabled = false;
			btnReload.Click += btnReload_Click;

			btnAddNew = CreateButton("Add New", new Point(90, 632), new Size(70, 28));
			btnAddNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnAddNew.Enabled = false;
			btnAddNew.Click += btnAddNew_Click;

			btnCopy = CreateButton("Copy", new Point(168, 632), new Size(70, 28));
			btnCopy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnCopy.Enabled = false;
			btnCopy.Click += btnCopy_Click;

			btnDelete = CreateButton("Delete", new Point(246, 632), new Size(66, 28));
			btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnDelete.Enabled = false;
			btnDelete.Click += btnDelete_Click;

			GroupBox gbToken = CreateGroupBox("Consumed item / token table", new Point(324, 12), new Size(860, 118));
			gbToken.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

			Label lblToken = CreateLabel("Item consumed to play", new Point(12, 31), new Size(140, 20));
			btnTokenItem = CreateButton("Choose token item", new Point(160, 24), new Size(510, 30));
			btnTokenItem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			btnTokenItem.ImageAlign = ContentAlignment.MiddleLeft;
			btnTokenItem.TextAlign = ContentAlignment.MiddleCenter;
			btnTokenItem.Enabled = false;
			btnTokenItem.Tag = -1;
			btnTokenItem.Click += (_sender, _args) => ChooseTokenItem();

			Label lblOrder = CreateLabel("Top strip order", new Point(12, 70), new Size(140, 20));
			tbItemOrder = CreateTextBox(new Point(160, 67), new Size(100, 23));
			tbItemOrder.TextChanged += (_sender, _args) => MarkDirty();

			btnUpdate = CreateButton("Save this LacaBall table", new Point(530, 65), new Size(340, 30));
			btnUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnUpdate.Enabled = false;
			btnUpdate.Click += btnUpdate_Click;

			gbToken.Controls.AddRange([lblToken, btnTokenItem, lblOrder, tbItemOrder, btnUpdate]);

			GroupBox gbRewards = CreateGroupBox("Rewards by course and slot", new Point(324, 142), new Size(860, 475));
			gbRewards.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

			gridRewards = CreateGrid(new Point(10, 24), new Size(840, 390));
			gridRewards.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { Name = "course", HeaderText = "Course", Width = 70, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { Name = "order", HeaderText = "Slot/order", Width = 80, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
			gridRewards.Columns.Add(new DataGridViewImageColumn { Name = "itemIcon", HeaderText = "", Width = 34, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { Name = "item", HeaderText = "Reward item", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { Name = "count", HeaderText = "Count", Width = 95, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { Name = "max", HeaderText = "Total/max", Width = 95, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
			gridRewards.Columns.Add(new DataGridViewTextBoxColumn { Name = "remain", HeaderText = "Remaining", Width = 95, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
			gridRewards.CellDoubleClick += (_sender, _args) => ChooseRewardItem();
			gridRewards.CellValueChanged += (_sender, _args) => MarkDirty();

			btnAddCourse = CreateButton("Add course with 5 rewards", new Point(10, 428), new Size(170, 28));
			btnAddCourse.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnAddCourse.Enabled = false;
			btnAddCourse.Click += btnAddCourse_Click;

			btnAddReward = CreateButton("Add reward row", new Point(188, 428), new Size(120, 28));
			btnAddReward.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnAddReward.Enabled = false;
			btnAddReward.Click += btnAddReward_Click;

			btnChooseReward = CreateButton("Choose selected reward", new Point(316, 428), new Size(160, 28));
			btnChooseReward.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnChooseReward.Enabled = false;
			btnChooseReward.Click += (_sender, _args) => ChooseRewardItem();

			btnRemoveReward = CreateButton("Remove selected row", new Point(484, 428), new Size(150, 28));
			btnRemoveReward.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btnRemoveReward.Enabled = false;
			btnRemoveReward.Click += btnRemoveReward_Click;

			Label lblHelp = CreateLabel("The client UI displays up to 5 reward rows per course. Max/remaining are stock limits used by the server.", new Point(645, 424), new Size(200, 42));
			lblHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			gbRewards.Controls.AddRange([gridRewards, btnAddCourse, btnAddReward, btnChooseReward, btnRemoveReward, lblHelp]);

			Controls.AddRange([tbSearch, MainList, btnReload, btnAddNew, btnCopy, btnDelete, gbToken, gbRewards]);

			Load += LacaBallEditor_LoadAsync;
			FormClosing += LacaBallEditor_FormClosing;
		}

		private static Button CreateButton(string strText, Point pLocation, Size pSize)
		{
			Button btn = new()
			{
				Text = strText,
				Location = pLocation,
				Size = pSize,
				BackColor = Color.FromArgb(40, 40, 40),
				ForeColor = Color.FromArgb(208, 203, 148),
				FlatStyle = FlatStyle.Flat,
				UseVisualStyleBackColor = false
			};

			btn.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			return btn;
		}

		private static TextBox CreateTextBox(Point pLocation, Size pSize)
		{
			return new TextBox
			{
				Location = pLocation,
				Size = pSize,
				BackColor = Color.FromArgb(28, 30, 31),
				ForeColor = Color.FromArgb(208, 203, 148),
				BorderStyle = BorderStyle.FixedSingle
			};
		}

		private static Label CreateLabel(string strText, Point pLocation, Size pSize)
		{
			return new Label
			{
				Text = strText,
				Location = pLocation,
				Size = pSize,
				ForeColor = Color.FromArgb(208, 203, 148),
				BackColor = Color.Transparent
			};
		}

		private static GroupBox CreateGroupBox(string strText, Point pLocation, Size pSize)
		{
			return new GroupBox
			{
				Text = strText,
				Location = pLocation,
				Size = pSize,
				ForeColor = Color.FromArgb(208, 203, 148),
				BackColor = Color.FromArgb(40, 40, 40)
			};
		}

		private static DataGridView CreateGrid(Point pLocation, Size pSize)
		{
			DataGridView grid = new()
			{
				Location = pLocation,
				Size = pSize,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AllowUserToResizeRows = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
				BackgroundColor = Color.FromArgb(28, 30, 31),
				BorderStyle = BorderStyle.None,
				ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
				DefaultCellStyle = new DataGridViewCellStyle
				{
					BackColor = Color.FromArgb(40, 40, 40),
					ForeColor = Color.FromArgb(208, 203, 148),
					SelectionBackColor = Color.FromArgb(60, 56, 54),
					SelectionForeColor = Color.FromArgb(208, 203, 148),
					Alignment = DataGridViewContentAlignment.MiddleCenter
				},
				EnableHeadersVisualStyles = false,
				GridColor = Color.FromArgb(91, 85, 76),
				MultiSelect = false,
				RowHeadersVisible = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect
			};

			grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
			{
				BackColor = Color.FromArgb(60, 56, 54),
				ForeColor = Color.FromArgb(208, 203, 148),
				SelectionBackColor = Color.FromArgb(60, 56, 54),
				SelectionForeColor = Color.FromArgb(208, 203, 148),
				Alignment = DataGridViewContentAlignment.MiddleCenter
			};

			return grid;
		}

		private void MainList_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (bLoading || MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
				return;

			if (!CheckUnsavedChanges())
			{
				MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
				MainList.SelectedItem = pLastSelected;
				MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
				return;
			}

			LoadUiData(pSelectedItem.ID);
			pLastSelected = pSelectedItem;
		}

		private async void btnReload_Click(object? sender, EventArgs e)
		{
			if (!CheckUnsavedChanges())
				return;

			SetLoadedState(false);
			await LoadLacaBallDataAsync();
			FillMainList();
			SetLoadedState(true);

			if (MainList.Items.Count > 0)
				MainList.SelectedIndex = 0;
		}

		private void btnAddNew_Click(object? sender, EventArgs e)
		{
			if (!CheckUnsavedChanges())
				return;

			ItemPicker pItemSelector = new(pMain, this, DefaultTokenItemId, false);
			if (pItemSelector.ShowDialog() != DialogResult.OK)
				return;

			int nTokenId = Convert.ToInt32(pItemSelector.ReturnValues[0]);
			if (TokenExists(nTokenId, -1))
			{
				MessageBox.Show("That consumed item already has a LacaBall reward table.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			int nOrder = GetNextItemOrder();
			AddTokenToList(nTokenId, nOrder);
			MainList.SelectedIndex = MainList.Items.Count - 1;
			LoadTemporaryTable(nTokenId, nOrder);
		}

		private void btnCopy_Click(object? sender, EventArgs e)
		{
			if (!CheckUnsavedChanges() || MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
				return;

			ItemPicker pItemSelector = new(pMain, this, DefaultTokenItemId, false);
			if (pItemSelector.ShowDialog() != DialogResult.OK)
				return;

			int nTokenId = Convert.ToInt32(pItemSelector.ReturnValues[0]);
			if (TokenExists(nTokenId, -1))
			{
				MessageBox.Show("That consumed item already has a LacaBall reward table.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			List<LcBallRewardRow> rows = gridRewards.Rows.Cast<DataGridViewRow>()
				.Select(row => new LcBallRewardRow(
					Convert.ToInt32(row.Cells["course"].Value),
					Convert.ToInt32(row.Cells["order"].Value),
					Convert.ToInt32(row.Cells["item"].Tag),
					Convert.ToInt64(row.Cells["count"].Value),
					Convert.ToInt64(row.Cells["max"].Value),
					Convert.ToInt64(row.Cells["remain"].Value)
				))
				.ToList();

			int nOrder = GetNextItemOrder();
			AddTokenToList(nTokenId, nOrder);
			MainList.SelectedIndex = MainList.Items.Count - 1;
			LoadTemporaryTable(nTokenId, nOrder, rows);
		}

		private void btnDelete_Click(object? sender, EventArgs e)
		{
			if (MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
				return;

			DialogResult pDialogReturn = MessageBox.Show($"Delete LacaBall table for {pSelectedItem.Text}?", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (pDialogReturn != DialogResult.Yes)
				return;

			if (!bCurrentTableIsTemporary && !pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBUser}.t_lcball WHERE a_tocken_index={pSelectedItem.ID};", out long _))
			{
				MessageBox.Show("Delete failed. Check Logs.log for the MySQL error.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			MainList.Items.Remove(pSelectedItem);
			gridRewards.Rows.Clear();

			if (MainList.Items.Count > 0)
				MainList.SelectedIndex = 0;
		}

		private async void btnUpdate_Click(object? sender, EventArgs e)
		{
			if (!TryComposeRows(out LcBallSaveData saveData))
				return;

			if (!SaveRows(saveData))
			{
				MessageBox.Show("Save failed. Check Logs.log for the MySQL error.", "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			bUnsavedChanges = false;
			btnUpdate.Enabled = false;
			await ReloadAfterSaveAsync(saveData.TokenId);
			MessageBox.Show("LacaBall reward table saved successfully.", "LacaBall Editor", MessageBoxButtons.OK);
		}

		private void btnAddCourse_Click(object? sender, EventArgs e)
		{
			int nCourse = GetNextCourseCode();

			for (int i = 0; i < DefaultRewardRowsPerCourse; i++)
				AddRewardRow(nCourse, i, DefaultRewardItemId, 1, 1, 1);

			MarkDirty();
		}

		private void btnAddReward_Click(object? sender, EventArgs e)
		{
			int nCourse = 0;
			if (gridRewards.CurrentRow != null && TryGetIntCell(gridRewards.CurrentRow, "course", out int nSelectedCourse))
				nCourse = nSelectedCourse;

			AddRewardRow(nCourse, GetNextOrderInCourse(nCourse), DefaultRewardItemId, 1, 1, 1);
			MarkDirty();
		}

		private void btnRemoveReward_Click(object? sender, EventArgs e)
		{
			if (gridRewards.CurrentRow == null)
				return;

			gridRewards.Rows.Remove(gridRewards.CurrentRow);
			MarkDirty();
		}

		private void LacaBallEditor_FormClosing(object? sender, FormClosingEventArgs e)
		{
			if (!bUnsavedChanges)
				return;

			DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			e.Cancel = pDialogReturn != DialogResult.Yes;
		}

		private readonly record struct LcBallRewardRow(int Course, int Order, int ItemId, long Count, long Max, long Remain);

		private sealed class LcBallSaveData
		{
			public int OriginalTokenId;
			public int TokenId;
			public int ItemOrder;
			public List<LcBallRewardRow> Rows = new();
		}
	}
}
