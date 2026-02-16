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
					// TODO: Esto en realidad va en donde se llama a esta función y se da este caso concreto; NO HAY QUE BORRAR SI es que se reemplazó el item requerido.
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
			List<string> listQueryCompose = new List<string> {
				"a_item_order",
				"a_tocken_index",
				"a_course_code",
				"a_order",
				"a_item_index",
				"a_item_count",
				"a_item_max",
				"a_item_remain"
			};

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
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_lacaball ORDER BY a_index;");
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
		// TODO: This needs to be removed.↓
		private (bool bProceed, long lLastInsertID) DoINSERT(object[] objArray) // Group Row ID, Reward Item ID, Amount, Probability & Item Reward Flag
		{
			int nGroupRowID = Convert.ToInt32(objArray[0]);
			string strGroupName = gridRewards.Rows[nGroupRowID].Cells[1].Value?.ToString() ?? string.Empty;
			bool bSuccess = true;
			long lNewLastInsertID = -1;
			/****************************************/
			int nMoonstoneID = Convert.ToInt32(gridRewards.Rows[nGroupRowID].HeaderCell.Tag);
			int nItemID = Convert.ToInt32(objArray[1]);
			int nAmount = Convert.ToInt32(objArray[2]);
			float fProb = float.Parse(objArray[3].ToString(), CultureInfo.InvariantCulture);
			long lFlag = Convert.ToInt64(objArray[4]);

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"INSERT INTO {pMain.pSettings.DBData}.t_moonstone_reward (a_type, a_giftindex, a_giftcount, a_giftprob, a_giftflag) VALUES ({nMoonstoneID}, '{nItemID}', '{nAmount}', '{fProb}', '{lFlag}');", out long lLastInsertID))
			{
				try
				{
					DataRow? pLacaBallTableRow = pMain.pTables.LacaBallTable?.NewRow();
					if (pLacaBallTableRow != null)
					{
						pLacaBallTableRow["a_index"] = lLastInsertID;
						pLacaBallTableRow["a_type"] = nMoonstoneID;
						pLacaBallTableRow["a_giftindex"] = nItemID;
						pLacaBallTableRow["a_giftcount"] = nAmount;
						pLacaBallTableRow["a_giftprob"] = fProb;
						pLacaBallTableRow["a_giftflag"] = lFlag;

						pMain.pTables.LacaBallTable?.Rows.Add(pLacaBallTableRow);
					}

					lNewLastInsertID = lLastInsertID;
				}
				catch (Exception ex)
				{
					string strError = $"LacaBall Editor > Reward: {strGroupName}: {lLastInsertID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
			}
			else
			{
				string strError = $"LacaBall Editor > Reward: {strGroupName}: {lLastInsertID} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

				bSuccess = false;
			}

			/*if (bSuccess)
			{
				if (cbChangesAppliedNotification.Checked)
					MessageBox.Show("Changes applied successfully!", "LacaBall Editor", MessageBoxButtons.OK);
				else
					pMain.Logger(LogTypes.Success, $"LacaBall Editor > Reward: {strGroupName}: {lLastInsertID} Changes applied successfully!");
			}*/

			return (bSuccess, lNewLastInsertID);
		}
		// TODO: This needs to be removed.↓
		private void DoUPDATE(int nGridRowID)
		{
			bool bSuccess = true;
			int nGroupRowID = GetGroupRowID(nGridRowID);
			string strGroupName = gridRewards.Rows[nGroupRowID].Cells[1].Value.ToString() ?? string.Empty;
			int nMoonstoneID = Convert.ToInt32(gridRewards.Rows[nGroupRowID].HeaderCell.Tag);
			int nRowIndex = Convert.ToInt32(gridRewards.Rows[nGridRowID].HeaderCell.Tag);
			/****************************************/
			int nItemID = Convert.ToInt32(gridRewards.Rows[nGridRowID].Cells["item"].Tag);
			int nAmount = Convert.ToInt32(gridRewards.Rows[nGridRowID].Cells["amount"].Value);
			float fProb = float.Parse(gridRewards.Rows[nGridRowID].Cells["prob"].Value?.ToString() ?? "0");
			long lFlag = Convert.ToInt64(gridRewards.Rows[nGridRowID].Cells["flag"].Value);

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"UPDATE {pMain.pSettings.DBData}.t_moonstone_reward SET a_type={nMoonstoneID}, a_giftindex={nItemID}, a_giftcount={nAmount}, a_giftprob='{fProb}', a_giftflag={lFlag} WHERE a_index={nRowIndex};", out long _))
			{
				try
				{
					DataRow? pLacaBallTableRow = pMain.pTables.LacaBallTable?.Select("a_index=" + nRowIndex).FirstOrDefault();
					if (pLacaBallTableRow != null)
					{
						pLacaBallTableRow["a_type"] = nMoonstoneID;
						pLacaBallTableRow["a_giftindex"] = nItemID;
						pLacaBallTableRow["a_giftcount"] = nAmount;
						pLacaBallTableRow["a_giftprob"] = fProb;
						pLacaBallTableRow["a_giftflag"] = lFlag;
					}
				}
				catch (Exception ex)
				{
					string strError = $"LacaBall Editor > Reward: {strGroupName}: {nRowIndex} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
			}
			else
			{
				string strError = $"LacaBall Editor > Reward: {strGroupName}: {nRowIndex} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

				bSuccess = false;
			}

			if (bSuccess)
			{
				/*if (cbChangesAppliedNotification.Checked)
					MessageBox.Show("Changes applied successfully!", "LacaBall Editor", MessageBoxButtons.OK);
				else
					pMain.Logger(LogTypes.Success, $"LacaBall Editor > Reward: {strGroupName}: {nRowIndex} Changes applied successfully!");

				nLastRowEdited = -1;*/
			}
		}
		// TODO: This needs to be removed.↓
		private bool DoDELETE(int nGroupRowID, List<int> nRowIDS)
		{
			if (nRowIDS == null || nRowIDS.Count <= 0)
				return false;

			string strGroupName = gridRewards.Rows[nGroupRowID].Cells[1].Value.ToString() ?? string.Empty;
			string strRowIDS = string.Join(", ", nRowIDS);
			bool bSuccess = true;

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_moonstone_reward WHERE a_index IN({strRowIDS});", out long _))
			{
				try
				{
					if (pMain.pTables.LacaBallTable != null)
					{
						foreach (int nRowID in nRowIDS)
						{
							DataRow? pRow = pMain.pTables.LacaBallTable?.Select("a_index=" + nRowID).FirstOrDefault();
							if (pRow != null)
								pMain.pTables.LacaBallTable?.Rows.Remove(pRow);
						}
					}
				}
				catch (Exception ex)
				{
					string strError = $"LacaBall Editor > Reward{((nRowIDS.Count > 1) ? "s" : "")}: {strGroupName}: {strRowIDS} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
			}
			else
			{
				string strError = $"LacaBall Editor > Reward{((nRowIDS.Count > 1) ? "s" : "")}: {strGroupName}: {strRowIDS} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "LacaBall Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

				bSuccess = false;
			}

			if (bSuccess)
			{
				/*if (cbChangesAppliedNotification.Checked)
					MessageBox.Show("Changes applied successfully!", "LacaBall Editor", MessageBoxButtons.OK);
				else
					pMain.Logger(LogTypes.Success, $"LacaBall Editor > Reward{((nRowIDS.Count > 1) ? "s" : "")}: {strGroupName}: {strRowIDS} Changes applied successfully!");*/
			}

			return bSuccess;
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
				pTempLacaBallTokenRows = pMain.pTables.LacaBallTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_item_order"]) == nItemOrder).ToArray();
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
			// TODO: Eventualmente dependiendo de bLoadFrompLacaBallTable hay que repopular la grilla
			if (bLoadFrompLacaBallTable)
			{
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
							strRewardItemName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

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
			}
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

		private void btnAddNew_Click(object sender, EventArgs e)
		{
			// TODO: ...
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			// TODO: ...
		}

		private void btnDelete_Click(object sender, EventArgs e)	// TODO: ...
		{
			bool bSuccess = true;
			int nTokenID = Convert.ToInt32(pTempLacaBallTokenRows[0]["a_tocken_index"]);
			DataRow? pMagicRow = pMain.pTables.LacaBallTable?.Select("a_tocken_index=" + nTokenID).FirstOrDefault();

			if (pMagicRow != null)
			{
				StringBuilder strbuilderQuery = new();

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_magic WHERE a_index={nTokenID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_magiclevel WHERE a_index={nMagicID};\n");

				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _)))
				{
					string strError = $"Magic Editor > Magic: {nTokenID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Magic Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pMain.pTables.MagicLevelTable != null)
					{
						DataRow[] pRows = pMain.pTables.MagicLevelTable.Select("a_index=" + nMagicID);

						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.MagicLevelTable.Rows.Remove(pRow);
						}
					}

					if (pMagicRow != null)
						pMain.pTables.MagicTable.Rows.Remove(pMagicRow);
				}
				catch (Exception ex)
				{
					string strError = $"Magic Editor > Magic: {nMagicID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Magic Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Magic Deleted successfully!", "Magic Editor", MessageBoxButtons.OK);

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
		{

		}
	}
}
