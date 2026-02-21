//#define USE_ORIGINAL_LACABALL_TABLE_LOCATION_AND_NAME
// WARNING NOTE: This Editor needs a primary key column called a_index in the lacaball table.

namespace LastChaos_ToolBoxNG
{
	public partial class LacaBallEditor : Form
	{
		private readonly Main pMain;
		private bool bHideAll = false;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow[] pTempLacaBallTokenRows;   // Row 0 is the references for a_item_order, a_tocken_index, etc etc
		private int nOriginalTokenID = -1;
		private ContextMenuStrip? cmRewards;

		public LacaBallEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridRewards.TopLeftHeaderCell.Value = "N°";
			gridRewards.TopLeftHeaderCell.ToolTipText = "Collapse / Expand All";
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.LacaBallTable?.Select("a_tocken_index=" + nOriginalTokenID).FirstOrDefault() != null) // the current selected LacaBall, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else    // the current selected LacaBall is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Token is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
					else if (pDialogReturn == DialogResult.Yes)
						bDeleteActual = true;
				}
			}

			return (bProceed, bDeleteActual);
		}

		private void AddToList(int nID, string strName, bool bIsTemp)
		{
			MainList.Items.Add(new Main.ListBoxItem
			{
				ID = nID,
				Text = $"{nID} - {strName}"
			});

			if (bIsTemp)
			{
				LoadUIData(nID, false);

				MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
				MainList.SelectedIndex = MainList.Items.Count - 1;
				MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

				pLastSelected = (Main.ListBoxItem?)MainList.SelectedItem;

				bUnsavedChanges = true;
			}
		}

		private async Task LoadLacaBallDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_item_order",
				"a_tocken_index",
				"a_course_code",
				"a_order",
				"a_item_index",
				"a_item_count",
				"a_item_max",
				"a_item_remain"
			];

			if (pMain.pTables.LacaBallTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.LacaBallTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
#if USE_ORIGINAL_LACABALL_TABLE_LOCATION_AND_NAME
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBUser}.t_lcball ORDER BY a_index;");
#else
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_lacaball ORDER BY a_index;");
#endif
				});

				if (pMain.pTables.LacaBallTable == null)
					pMain.pTables.LacaBallTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.LacaBallTable);
			}
		}

		private async void LacaBallEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadLacaBallDataAsync(),
				pMain.GenericLoadItemDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"LacaBall & Items Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.LacaBallTable != null && pMain.pTables.ItemTable != null)
			{
				MainList.BeginUpdate();

				int nRequiredItemID;
				string strRequiredItemName = "NOT FOUND";

				foreach (DataRow pRow in pMain.pTables.LacaBallTable.AsEnumerable().GroupBy(row => Convert.ToInt32(row["a_tocken_index"])).Select(group => group.First()).OrderBy(row => Convert.ToInt32(row["a_item_order"])).ThenByDescending(row => Convert.ToInt32(row["a_course_code"])))
				{
					nRequiredItemID = Convert.ToInt32(pRow["a_tocken_index"]);

					DataRow? pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nRequiredItemID).FirstOrDefault();
					if (pItemRow != null)
						strRequiredItemName = pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty;
					else
						pMain.Logger(LogTypes.Error, $"LacaBall Editor > Token: {nRequiredItemID} Error: a_tocken_index: {nRequiredItemID} not exist in t_item.");

					AddToList(Convert.ToInt32(pRow["a_item_order"]), strRequiredItemName, false);
				}

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload LacaBall & Items Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		DataGridViewRow CreateGroupRow(int nCourseCode) // NOTE: Cell 0 indicates Row is type Group. Cell 1 Indicates if Group Childs are Visible or Not.
		{
			DataGridViewRow pRow = new();

			pRow.CreateCells(gridRewards);

			pRow.HeaderCell.Value = "-";
			pRow.HeaderCell.Tag = nCourseCode;
			pRow.HeaderCell.ToolTipText = "Collapse/Expand Group";
			pRow.Cells[0].Tag = "GROUP";
			pRow.Cells[1].Tag = true;
			pRow.ReadOnly = true;
			pRow.Height = 26;

			for (int i = 0; i < gridRewards.Columns.Count; i++)
			{
				pRow.Cells[i].Style.BackColor = Color.FromArgb(91, 85, 76);
				pRow.Cells[i].Style.SelectionBackColor = Color.FromArgb(91, 85, 76);
				pRow.Cells[i].Style.Font = new Font(gridRewards.Font, FontStyle.Bold);
				pRow.Cells[i].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
				pRow.Cells[i].Value = (i == 0) ? new Bitmap(1, 1) : ((i == 1) ? $"Course N° {nCourseCode}" : null);
				pRow.Cells[i].ToolTipText = "Collapse/Expand Group";
			}

			return pRow;
		}

		int GetGroupRowID(int nCurrentRowID)
		{
			while (gridRewards.Rows[nCurrentRowID].Cells[0].Tag?.ToString() != "GROUP")
				nCurrentRowID--;

			return nCurrentRowID;
		}

		private void LacaBallEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				if (cmRewards != null)
				{
					cmRewards.Dispose();
					cmRewards = null;
				}
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (pDialogReturn == DialogResult.No)
					e.Cancel = true;
				else
					Clear();
			}
			else
			{
				Clear();
			}
		}

		private void LoadUIData(int nItemOrder, bool bLoadFrompLacaBallTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			btnRequiredItem.Image = null;
			/****************************************/
			if (bLoadFrompLacaBallTable && pMain.pTables.LacaBallTable != null)
			{
				pTempLacaBallTokenRows = pMain.pTables.LacaBallTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_item_order"]) == nItemOrder).Select(row => {
					DataRow newRow = pMain.pTables.LacaBallTable.NewRow();
					newRow.ItemArray = (object[])row.ItemArray.Clone();
					return newRow;
				}).ToArray();
			}
			/****************************************/
			// General
			nOriginalTokenID = Convert.ToInt32(pTempLacaBallTokenRows[0]["a_tocken_index"]);
			string strRequiredItemName = nOriginalTokenID.ToString();
			DataRow? pItemRow;

			if (nOriginalTokenID > 0)
			{
				pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nOriginalTokenID).FirstOrDefault();
				if (pItemRow != null)
				{
					strRequiredItemName += $" - {pItemRow["a_name_" + pMain.pSettings.WorkLocale]}";

					btnRequiredItem.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
#if USE_a_job_AND_a_item_type_TABLE
					cbItemResultType.SelectedIndex = Convert.ToInt32(pItemRow["a_subtype_idx"]);
#endif
				}
			}

			btnRequiredItem.Text = strRequiredItemName;
			/****************************************/
			gridRewards.Rows.Clear();

			gridRewards.SuspendLayout();

			int nRewardCount = 1, nAddedCourseCode = -1;

			foreach (DataRow pRow in pTempLacaBallTokenRows)
			{
				// Group Header
				int nCouseCode = Convert.ToInt32(pRow["a_course_code"]);

				if (nAddedCourseCode != nCouseCode)
				{
					gridRewards.Rows.Add(CreateGroupRow(nCouseCode));

					nRewardCount = 1;
					nAddedCourseCode = nCouseCode;
				}

				// Rewards of the Group/Course Code
				int nRewardID, nGroupRowIndex = gridRewards.Rows.Count;
				string strRewardItemName;

				gridRewards.Rows.Insert(nGroupRowIndex);

				gridRewards.Rows[nGroupRowIndex].HeaderCell.Value = nRewardCount.ToString();

				nRewardID = Convert.ToInt32(pRow["a_item_index"]);
				strRewardItemName = nRewardID.ToString();

				if (nRewardID > 0)
				{
					pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nRewardID).FirstOrDefault();
					if (pItemRow != null)
					{
						strRewardItemName += $" - {pItemRow["a_name_" + pMain.pSettings.WorkLocale]}";

						gridRewards.Rows[nGroupRowIndex].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
					}
				}

				gridRewards.Rows[nGroupRowIndex].Cells["item"].Value = strRewardItemName;
				gridRewards.Rows[nGroupRowIndex].Cells["item"].Tag = nRewardID;
				gridRewards.Rows[nGroupRowIndex].Cells["count"].Value = pRow["a_item_count"];

				if (nRewardID == Defs.NAS_ITEM_DB_INDEX)
				{
					gridRewards.Rows[nGroupRowIndex].Cells["count"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(pRow["a_item_count"]));
					gridRewards.Rows[nGroupRowIndex].Cells["count"].Style.BackColor = Color.FromArgb(166, 166, 166);
				}

				gridRewards.Rows[nGroupRowIndex].Cells["max"].Value = pRow["a_item_max"];
				gridRewards.Rows[nGroupRowIndex].Cells["remain"].Value = pRow["a_item_remain"];

				nRewardCount++;
				nGroupRowIndex++;
			}

			gridRewards.ResumeLayout();
			/****************************************/
			bUserAction = true;

			btnUpdate.Enabled = true;

			btnCopy.Enabled = true;
			btnDelete.Enabled = true;
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }

		private void tbSearch_KeyDown(object sender, KeyEventArgs e) { nSearchPosition = pMain.SearchInListBox(tbSearch, e, MainList, nSearchPosition); }

		private void MainList_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
				return;

			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bProceed)
			{
				if (bDeleteActual)
				{
					int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

					MainList.Items.RemoveAt(MainList.Items.Count - 1);

					object nSelected = MainList.Items[nPrevObjectID];

					LoadUIData(((Main.ListBoxItem)nSelected).ID, true);

					MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
					MainList.SelectedItem = nSelected;
					MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

					bUnsavedChanges = false;
				}
				else
				{
					bUnsavedChanges = false;

					LoadUIData(pSelectedItem.ID, true);
				}
			}
			else
			{
				MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
				MainList.SelectedItem = pLastSelected;
				MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
			}

			pLastSelected = pSelectedItem;
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			btnReload.Enabled = false;

			nSearchPosition = 0;

			pMain.pTables.LacaBallTable?.Dispose();
			pMain.pTables.LacaBallTable = null;

			pMain.pTables.ItemTable?.Dispose();
			pMain.pTables.ItemTable = null;

			LacaBallEditor_LoadAsync(sender, e);
		}

		private (int nItemID, string strItemName) AskForItemIndex()
		{
			ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempLacaBallTokenRows[0]["a_tocken_index"]), false);
			if (pItemSelector.ShowDialog() != DialogResult.OK)
				return (-1, "NONE");

			int nNewTokenID = Convert.ToInt32(pItemSelector.ReturnValues[0]);

			if (!pMain.pTables.LacaBallTable.AsEnumerable().Any(row => Convert.ToInt32(row["a_tocken_index"]) == nNewTokenID))
			{
				return (nNewTokenID, pItemSelector.ReturnValues[1].ToString() ?? string.Empty);
			}
			else
			{
				pMain.Logger(LogTypes.Error, "LacaBall Editor > Cannot duplicate an Item for a Token.");

				return AskForItemIndex();
			}
		}

		private void btnAddNew_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bProceed)
			{
				int i, nNewIndexID = 0, nNewTokenID, nNewTokenItemOrder = 0;

				var (nItemID, strItemName) = AskForItemIndex();
				if (nItemID == -1)
					return;
				else
					nNewTokenID = nItemID;

				DataRow pNewRow;

				List<string> listIntColumns =
				[
					"a_index",
					"a_item_order",
					"a_tocken_index",
					"a_item_index",
					"a_item_count",
					"a_item_max",
					"a_item_remain"
				];

				List<string> listTinyIntColumns =
				[
					"a_course_code",
					"a_order"
				];

				if (pMain.pTables.LacaBallTable == null)
				{
					DataTable pLacaBallTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pLacaBallTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listTinyIntColumns)
						pLacaBallTableStruct.Columns.Add(strColumnName, typeof(sbyte));

					pNewRow = pLacaBallTableStruct.NewRow();

					pLacaBallTableStruct.Dispose();
				}
				else
				{
					pNewRow = pMain.pTables.LacaBallTable.NewRow();

					nNewIndexID = pMain.pTables.LacaBallTable.AsEnumerable().Max(row => Convert.ToInt32(row["a_index"])) + 1;
					nNewTokenItemOrder = pMain.pTables.LacaBallTable.AsEnumerable().Max(row => Convert.ToInt32(row["a_item_order"])) + 1;
				}

				List<object> listDefaultValue =
				[
					nNewIndexID,	// a_index
					nNewTokenItemOrder,	// a_item_order
					nNewTokenID,	// a_tocken_index
					43,	// a_item_index
					1,	// a_item_count
					1,	// a_item_max
					1,	// a_item_remain
					0,	// a_course_code
					0	// a_order
				];

				i = 0;
				foreach (string strColumnName in listIntColumns.Concat(listTinyIntColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempLacaBallTokenRows = [pNewRow];
				}
				catch (Exception ex)
				{
					string strError = $"LacaBall Editor > Token: {nNewTokenID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewTokenItemOrder, strItemName, true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Token. Because it's temporary.", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				var (nNewTokenID, strItemName) = AskForItemIndex();

				if (nNewTokenID == -1)
					return;

				int nNewTokenItemOrder = pMain.pTables.LacaBallTable.AsEnumerable().Max(row => Convert.ToInt32(row["a_item_order"])) + 1;

				pTempLacaBallTokenRows = pMain.pTables.LacaBallTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_tocken_index"]) == nOriginalTokenID).Select(row => {
					DataRow newRow = pMain.pTables.LacaBallTable.NewRow();
					newRow.ItemArray = (object[])row.ItemArray.Clone();
					return newRow;
				}).ToArray();

				foreach (DataRow pRow in pTempLacaBallTokenRows)
				{
					pRow["a_tocken_index"] = nNewTokenID;
					pRow["a_item_order"] = nNewTokenItemOrder;
				}

				AddToList(nNewTokenItemOrder, strItemName, true);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;

			if (pMain.pTables.LacaBallTable?.Select("a_tocken_index=" + nOriginalTokenID).FirstOrDefault() != null)
			{
#if USE_ORIGINAL_LACABALL_TABLE_LOCATION_AND_NAME
				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBUser}.t_lcball WHERE a_tocken_index={nOriginalTokenID};", out long _)))
#else
				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_lacaball WHERE a_tocken_index={nOriginalTokenID};", out long _)))
#endif
				{
					string strError = $"LacaBall Editor > Token: {nOriginalTokenID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pMain.pTables.LacaBallTable != null)
					{
						DataRow[] pRows = pMain.pTables.LacaBallTable.Select("a_tocken_index=" + nOriginalTokenID);

						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.LacaBallTable.Rows.Remove(pRow);
						}
					}
				}
				catch (Exception ex)
				{
					string strError = $"LacaBall Editor > Token: {nOriginalTokenID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Token Deleted successfully!", "LacaBall Editor", MessageBoxButtons.OK);

						MainList.SelectedIndex = nPrevObjectID;

						bUnsavedChanges = false;
					}
				}
			}
		}
		/****************************************/
		private void btnRequiredItem_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempLacaBallTokenRows[0]["a_tocken_index"]), false);
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nRequiredItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				if (nRequiredItemID > 0)
				{
					btnRequiredItem.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
					btnRequiredItem.Text = $"{nRequiredItemID} - {pItemSelector.ReturnValues[1]}";

					foreach (DataRow pRow in pTempLacaBallTokenRows)
						pRow["a_tocken_index"] = nRequiredItemID;

					bUnsavedChanges = true;
				}
			}
		}

		private void gridRewards_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
		{
			if (bUserAction && gridRewards.Rows[e.RowIndex].Cells[0].Tag?.ToString() != "GROUP")
			{
				if (Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
				{
					gridRewards.Rows[e.RowIndex].Cells["count"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["count"].Value));
					gridRewards.Rows[e.RowIndex].Cells["count"].Style.BackColor = Color.FromArgb(166, 166, 166);
				}

				bUnsavedChanges = true;
			}
		}

		private void ChangeGroupVisibleState(int nRowID)
		{
			bool bVisible = Convert.ToBoolean(gridRewards.Rows[nRowID].Cells[1].Tag);

			gridRewards.SuspendLayout();

			int i = 1;
			while (nRowID + i < gridRewards.Rows.Count && gridRewards.Rows[nRowID + i].Cells[0].Tag?.ToString() != "GROUP")
			{
				gridRewards.Rows[nRowID + i].Visible = !bVisible;

				i++;
			}

			if (i > 1)  // At least the group have one child to Hide or Show
			{
				gridRewards.Rows[nRowID].Cells[1].Tag = !bVisible;
				gridRewards.Rows[nRowID].HeaderCell.Value = bVisible ? "+" : "-";
			}

			gridRewards.ResumeLayout();
		}

		private void gridRewards_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Left)
				{
					if (e.RowIndex == -1 && e.ColumnIndex == -1)    // Collapse / Expand All
					{
						gridRewards.SuspendLayout();

						int i = 0;
						foreach (DataGridViewRow row in gridRewards.Rows)
						{
							if (row.Cells[0].Tag?.ToString() != "GROUP")
							{
								row.Visible = bHideAll;
							}
							else
							{
								if (row.Index + 1 < gridRewards.Rows.Count && gridRewards.Rows[row.Index + 1].Cells[0].Tag?.ToString() != "GROUP")  // At least the group have one child to Hide or Show
								{
									row.Cells[1].Tag = bHideAll;
									row.HeaderCell.Value = bHideAll ? "-" : "+";
								}
							}

							i++;
						}

						gridRewards.ResumeLayout();

						bHideAll = !bHideAll;
					}
					else if (e.RowIndex >= 0 && gridRewards.Rows[e.RowIndex].Cells[0].Tag?.ToString() == "GROUP")   // Collapse/Expand this Group
					{
						ChangeGroupVisibleState(e.RowIndex);
					}
					else if (e.ColumnIndex == 1 && e.RowIndex >= 0)    // Item Selector
					{
						int nItemID = Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag);

						ItemPicker pItemSelector = new(pMain, this, nItemID, false);
						if (pItemSelector.ShowDialog() != DialogResult.OK)
							return;

						nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
						if (nItemID > 0)
						{
							gridRewards.Rows[e.RowIndex].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
							gridRewards.Rows[e.RowIndex].Cells["item"].Value = $"{nItemID} - {pItemSelector.ReturnValues[1]}";
							gridRewards.Rows[e.RowIndex].Cells["item"].Tag = nItemID;

							if (nItemID == Defs.NAS_ITEM_DB_INDEX)
							{
								gridRewards.Rows[e.RowIndex].Cells["count"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["count"].Value));
								gridRewards.Rows[e.RowIndex].Cells["count"].Style.BackColor = Color.FromArgb(166, 166, 166);
							}
							else
							{
								gridRewards.Rows[e.RowIndex].Cells["count"].Style.ForeColor = Color.FromArgb(208, 203, 148);
								gridRewards.Rows[e.RowIndex].Cells["count"].Style.BackColor = Color.FromArgb(40, 40, 40);
							}
						}
					}
				}
				else if (e.Button == MouseButtons.Right && e.ColumnIndex == -1) // Only in Header Cell
				{
					bool bIsHeader = e.RowIndex == -1;
					bool bIsGroupHeader = !bIsHeader && gridRewards.Rows[e.RowIndex].Cells[0].Tag?.ToString() == "GROUP";

					ToolStripMenuItem addItem = new(bIsHeader ? "Add New Course" : "Add New");
					addItem.Click += (_, _) =>
					{
						if (bIsHeader)
						{
							int nRow = gridRewards.Rows.Count - 1;
							while (gridRewards.Rows[nRow].Cells[0].Tag?.ToString() != "GROUP")
								nRow--;

							gridRewards.Rows.Add(CreateGroupRow(Convert.ToInt32(gridRewards.Rows[nRow].HeaderCell.Tag) + 1));

							bUnsavedChanges = true;
						}
						else
						{
							ItemPicker pItemSelector = new(pMain, this, 0, false);
							if (pItemSelector.ShowDialog() != DialogResult.OK)
								return;

							int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
							if (nItemID > 0)
							{
								int nDefaultAmount = 1;
								int nDefaultMax = 1;
								int nDefaultRemain = 1;

								if (!Convert.ToBoolean(gridRewards.Rows[e.RowIndex].Cells[1].Tag))
									ChangeGroupVisibleState(e.RowIndex);

								int nRow = e.RowIndex + 1;
								int nNumber = nRow;

								while (nRow < gridRewards.Rows.Count && gridRewards.Rows[nRow].Cells[0].Tag?.ToString() != "GROUP")
									nRow++;

								gridRewards.Rows.Insert(nRow);

								gridRewards.Rows[nRow].HeaderCell.Value = (nRow - nNumber + 1).ToString();

								gridRewards.Rows[nRow].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
								gridRewards.Rows[nRow].Cells["item"].Value = $"{nItemID} - {pItemSelector.ReturnValues[1]}";
								gridRewards.Rows[nRow].Cells["item"].Tag = nItemID;

								if (nItemID == Defs.NAS_ITEM_DB_INDEX)
								{
									gridRewards.Rows[nRow].Cells["count"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["count"].Value));
									gridRewards.Rows[nRow].Cells["count"].Style.BackColor = Color.FromArgb(166, 166, 166);
								}

								gridRewards.Rows[nRow].Cells["count"].Value = nDefaultAmount;
								gridRewards.Rows[nRow].Cells["max"].Value = nDefaultMax;
								gridRewards.Rows[nRow].Cells["remain"].Value = nDefaultRemain;

								gridRewards.FirstDisplayedScrollingRowIndex = e.RowIndex;
								gridRewards.Rows[nRow].Selected = true;
							}
						}
					};

					ToolStripMenuItem deleteItem = new(bIsGroupHeader ? "Delete Course" : "Delete") { Enabled = !bIsHeader };
					deleteItem.Click += (_, _) =>
					{
						if (bIsGroupHeader)
						{
							gridRewards.SuspendLayout();

							bool bDeleting = true;
							int nRow = e.RowIndex;

							while (nRow < gridRewards.Rows.Count)
							{
								if (bDeleting)
								{
									gridRewards.Rows.RemoveAt(e.RowIndex);

									if (e.RowIndex == gridRewards.Rows.Count)
										break;

									if (gridRewards.Rows[e.RowIndex].Cells[0].Tag?.ToString() == "GROUP")
										bDeleting = false;
								}
								else
								{
									if (gridRewards.Rows[nRow].Cells[0].Tag?.ToString() == "GROUP")
									{
										int nCourseCode = Convert.ToInt32(gridRewards.Rows[nRow].HeaderCell.Tag) - 1;

										gridRewards.Rows[nRow].HeaderCell.Tag = nCourseCode;
										gridRewards.Rows[nRow].Cells[1].Value = $"Course N° {nCourseCode}";
									}

									nRow++;
								}
							}

							gridRewards.ResumeLayout();
						}
						else
						{
							int nGroupRowIndex = GetGroupRowID(e.RowIndex);

							gridRewards.SuspendLayout();

							gridRewards.Rows.RemoveAt(e.RowIndex);

							int i = 1;
							while (nGroupRowIndex + i < gridRewards.Rows.Count && gridRewards.Rows[nGroupRowIndex + i].Cells[0].Tag?.ToString() != "GROUP")
							{
								gridRewards.Rows[nGroupRowIndex + i].HeaderCell.Value = i.ToString();

								i++;
							}

							gridRewards.ResumeLayout();
						}
					};

					ToolStripMenuItem deleteAllItems = new("Delete All from this Course") { Enabled = !bIsHeader && bIsGroupHeader };
					deleteAllItems.Click += (_, _) =>
					{
						DialogResult pDialogReturn = MessageBox.Show($"You sure want to Delete All the Rewards of {gridRewards.Rows[e.RowIndex].Cells[1].Value}?", "LacaBall Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						if (pDialogReturn == DialogResult.No)
							return;

						gridRewards.SuspendLayout();

						while (e.RowIndex + 1 < gridRewards.Rows.Count && gridRewards.Rows[e.RowIndex + 1].Cells[0].Tag?.ToString() != "GROUP")
							gridRewards.Rows.RemoveAt(e.RowIndex + 1);

						gridRewards.ResumeLayout();

						gridRewards.Rows[e.RowIndex].Cells[1].Tag = true;
						gridRewards.Rows[e.RowIndex].HeaderCell.Value = "-";

						bUnsavedChanges = true;
					};

					cmRewards = new ContextMenuStrip();
					cmRewards.Items.AddRange(addItem, deleteItem, deleteAllItems);
					cmRewards.Show(Cursor.Position);
				}
			}
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{/*
			// TODO: ...
#if USE_ORIGINAL_LACABALL_TABLE_LOCATION_AND_NAME
			//{pMain.pSettings.DBUser}.t_lcball
#else
			//{pMain.pSettings.DBData}.t_lacaball
#endif
			bool bSuccess = true;
			int i = 0, nShopID = Convert.ToInt32(pTempShopRow["a_keeper_idx"]);
			StringBuilder strbuilderQuery = new();

			// Init transaction.
			strbuilderQuery.Append("START TRANSACTION;\n");

			if (gridSaleItems.Rows.Count > 0)
			{
				pTempShopItemRows = new DataRow[gridSaleItems.Rows.Count];

				foreach (DataGridViewRow row in gridSaleItems.Rows)
				{
					DataRow pRow = pMain.pTables.ShopItemTable?.NewRow();

					pRow["a_keeper_idx"] = nShopID; // Put NEW Shop ID
					pRow["a_item_idx"] = row.Cells["item"].Tag;
#if ITEM_PLUS_SYSTEM
					pRow["a_item_plus"] = row.Cells["itemPlus"].Value;
#endif
					pRow["a_national"] = row.Cells["nation"].Value;

					pTempShopItemRows[i] = pRow;

					i++;
				}

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_shopitem WHERE a_keeper_idx={nOriginalShopID};\n");

				// Compose t_shopitem INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();
				HashSet<string> addedColumns = new();

				foreach (DataRow pRow in pTempShopItemRows)
				{
					strColumnsValues.Append("(");

					foreach (DataColumn pCol in pRow.Table.Columns)
					{
						string strColumnName = pCol.ColumnName;

						if (!addedColumns.Contains(strColumnName))
						{
							strColumnsNames.Append(strColumnName + ", ");
							addedColumns.Add(strColumnName);
						}

						strColumnsValues.Append(pRow[pCol] + ", ");
					}

					strColumnsValues.Length -= 2;

					strColumnsValues.Append("), ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_shopitem ({strColumnsNames}) VALUES {strColumnsValues};\n");
			}

			DataRow pShopTableRow = pMain.pTables.ShopTable.Select("a_keeper_idx=" + nOriginalShopID).FirstOrDefault();
			if (pShopTableRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_shop SET");

				foreach (DataColumn pCol in pTempShopRow.Table.Columns)
				{
					object objValue = pTempShopRow[pCol];
					if (objValue is string)
						objValue = pMain.EscapeChars(objValue.ToString());
					else
						objValue = Convert.ToString(objValue, CultureInfo.InvariantCulture);

					strbuilderQuery.Append($" {pCol.ColumnName}='{objValue}',");
				}

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_keeper_idx={nOriginalShopID};\n");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempShopRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					object objValue = pTempShopRow[pCol];
					if (objValue is string)
						objValue = pMain.EscapeChars(objValue.ToString());
					else
						objValue = Convert.ToString(objValue, CultureInfo.InvariantCulture);

					strColumnsValues.Append($"'{objValue}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_shop ({strColumnsNames}) VALUES ({strColumnsValues});\n");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _))
			{
				try
				{
					if (pTempShopItemRows != null && pTempShopItemRows.Length > 0)
					{
						DataRow[] pShopItemTableRowsOld = pMain.pTables.ShopItemTable?.Select("a_keeper_idx=" + nOriginalShopID);
						foreach (DataRow row in pShopItemTableRowsOld)
							row.Delete();

						if (nShopID != nOriginalShopID)
						{
							DataRow[] pShopItemTableRowsNew = pMain.pTables.ShopItemTable?.Select("a_keeper_idx=" + nShopID);
							foreach (DataRow row in pShopItemTableRowsNew)
								row.Delete();
						}

						foreach (DataRow pRow in pTempShopItemRows)
						{
							DataRow? newDataRow = pMain.pTables.ShopItemTable?.NewRow();
							newDataRow.ItemArray = (object[])pRow.ItemArray.Clone();
							pMain.pTables.ShopItemTable?.Rows.Add(newDataRow);
						}

						pMain.pTables.ShopItemTable?.AcceptChanges();
					}

					if (pShopTableRow != null)  // Row exist in Global Table, update it.
					{
						pShopTableRow.ItemArray = (object[])pTempShopRow.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pShopTableRow = pMain.pTables.ShopTable.NewRow();
						pShopTableRow.ItemArray = (object[])pTempShopRow.ItemArray.Clone();
						pMain.pTables.ShopTable.Rows.Add(pShopTableRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Shop Editor > Shop: {nOriginalShopID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Shop Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nShopID;

						DataRow pNPCRow = pMain.pTables.NPCTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nShopID).FirstOrDefault();
						string strShoperName = "NPC NOT FOUND";

						if (pNPCRow != null)
							strShoperName = pNPCRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

						pSelectedItem.Text = $"{nShopID} - {strShoperName}";

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "Shop Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Shop Editor > Shop: {nOriginalShopID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Shop Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}*/
		}
	}
}
