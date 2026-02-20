namespace LastChaos_ToolBoxNG
{
	public partial class MoonstoneEditor : Form
	{
		private readonly Main pMain;
		private bool bHideAll = false;
		private bool bUserAction = false;
		private int nSearchPosition = 0;
		private int nLastRowEdited = -1;
		private ContextMenuStrip? cmRewards;

		public MoonstoneEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridRewards.TopLeftHeaderCell.Value = "N°";
			gridRewards.TopLeftHeaderCell.ToolTipText = "Collapse / Expand All";
			/****************************************/
			cbChangesAppliedNotification.Checked = pMain.pSettings.ChangesAppliedNotification[this.Name];
		}

		private async Task LoadMoonstoneRewardDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_type", "a_giftindex", "a_giftcount", "a_giftprob", "a_giftflag"];

			if (pMain.pTables.MoonstoneRewardTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.MoonstoneRewardTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_moonstone_reward ORDER BY a_index;");
				});

				if (pMain.pTables.MoonstoneRewardTable == null)
					pMain.pTables.MoonstoneRewardTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.MoonstoneRewardTable);
			}
		}

		private async void MoonstoneEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadMoonstoneRewardDataAsync(),
				pMain.GenericLoadItemDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Moonstones Redwards & Items Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			bUserAction = false;
			/****************************************/
			// Reset Controls
			gridRewards.Rows.Clear();
			/****************************************/
			// General
			gridRewards.SuspendLayout();

			Image? pIcon = null;

			foreach (var MoonstoneData in Defs.MoonStonesNamesNIDS)
			{
				// Group Header
				DataRow? pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == MoonstoneData.Value).FirstOrDefault();
				if (pItemRow != null)
					pIcon = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
				else
					pIcon = null;

				gridRewards.Rows.Add(CreateGroupRow(pIcon, MoonstoneData.Value, $"{MoonstoneData.Key} ({MoonstoneData.Value})"));

				// Rewards of the Group/Moonstone Type
				DataRow[] pTempMoonstoneRewardsRows = pMain.pTables.MoonstoneRewardTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_type"]) == MoonstoneData.Value).ToArray();

				if (pTempMoonstoneRewardsRows.Length > 0)
				{
					int nRewardCount = 1, nRewardID, nGroupRowIndex = gridRewards.Rows.Count;
					string strRewardItemName;
					StringBuilder strTooltip = new();

					foreach (DataRow pRow in pTempMoonstoneRewardsRows)
					{
						gridRewards.Rows.Insert(nGroupRowIndex);

						gridRewards.Rows[nGroupRowIndex].HeaderCell.Value = nRewardCount.ToString();
						gridRewards.Rows[nGroupRowIndex].HeaderCell.Tag = pRow["a_index"];

						nRewardID = Convert.ToInt32(pRow["a_giftindex"]);
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
						gridRewards.Rows[nGroupRowIndex].Cells["amount"].Value = pRow["a_giftcount"];

						if (nRewardID == Defs.NAS_ITEM_DB_INDEX)
						{
							gridRewards.Rows[nGroupRowIndex].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(pRow["a_giftcount"]));
							gridRewards.Rows[nGroupRowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
						}

						gridRewards.Rows[nGroupRowIndex].Cells["prob"].Value = pRow["a_giftprob"];
						gridRewards.Rows[nGroupRowIndex].Cells["prob"].ToolTipText = ((float.Parse(pRow["a_giftprob"].ToString()) * 100.0f) / 10000.0f) + "%";
						gridRewards.Rows[nGroupRowIndex].Cells["flag"].Value = pRow["a_giftflag"];

						strTooltip.Clear();
						strTooltip = new StringBuilder();

						long lRewardItemFlag = Convert.ToInt64(pRow["a_giftflag"]);

						for (int i = 0; i < Defs.ItemFlags.Length; i++)
						{
							if ((lRewardItemFlag & 1L << i) != 0)
								strTooltip.Append(Defs.ItemFlags[i] + "\n");
						}

						gridRewards.Rows[nGroupRowIndex].Cells["flag"].ToolTipText = strTooltip.ToString();

						nRewardCount++;
						nGroupRowIndex++;
					}
				}
			}

			gridRewards.ResumeLayout();
			/****************************************/
			bUserAction = true;
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Moonstones Redwards & Items Data from Database");
			/****************************************/
			pProgressDialog.Close();

			btnReload.Enabled = true;

			gridRewards.Enabled = true;
			gridRewards.Focus();
		}

		private void MoonstoneEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (cmRewards != null)
			{
				cmRewards.Dispose();
				cmRewards = null;
			}
		}

		DataGridViewRow CreateGroupRow(Image pImage, int nMoonstoneID, string strText) // NOTE: Cell 0 indicates Row is type Group. Cell 1 Indicates if Group Childs are Visible or Not.
		{
			DataGridViewRow pRow = new();

			pRow.CreateCells(gridRewards);

			pRow.HeaderCell.Value = "-";
			pRow.HeaderCell.Tag = nMoonstoneID;
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
				pRow.Cells[i].Value = (i == 0) ? pImage : ((i == 1) ? strText : null);
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

		private (bool bProceed, long lLastInsertID) DoINSERT(object[] objArray)	// Group Row ID, Reward Item ID, Amount, Probability & Item Reward Flag
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
					DataRow? pMoonstoneRewardTableRow = pMain.pTables.MoonstoneRewardTable?.NewRow();
					if (pMoonstoneRewardTableRow != null)
					{
						pMoonstoneRewardTableRow["a_index"] = lLastInsertID;
						pMoonstoneRewardTableRow["a_type"] = nMoonstoneID;
						pMoonstoneRewardTableRow["a_giftindex"] = nItemID;
						pMoonstoneRewardTableRow["a_giftcount"] = nAmount;
						pMoonstoneRewardTableRow["a_giftprob"] = fProb;
						pMoonstoneRewardTableRow["a_giftflag"] = lFlag;

						pMain.pTables.MoonstoneRewardTable?.Rows.Add(pMoonstoneRewardTableRow);
					}

					lNewLastInsertID = lLastInsertID;
				}
				catch (Exception ex)
				{
					string strError = $"Moonstone Editor > Reward: {strGroupName}: {lLastInsertID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Moonstone Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
			}
			else
			{
				string strError = $"Moonstone Editor > Reward: {strGroupName}: {lLastInsertID} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Moonstone Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

				bSuccess = false;
			}

			if (bSuccess)
			{
				if (cbChangesAppliedNotification.Checked)
					MessageBox.Show("Changes applied successfully!", "Moonstone Editor", MessageBoxButtons.OK);
				else
					pMain.Logger(LogTypes.Success, $"Moonstone Editor > Reward: {strGroupName}: {lLastInsertID} Changes applied successfully!");
			}

			return (bSuccess, lNewLastInsertID);
		}

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

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"UPDATE {pMain.pSettings.DBData}.t_moonstone_reward SET a_type='{nMoonstoneID}', a_giftindex='{nItemID}', a_giftcount='{nAmount}', a_giftprob='{fProb}', a_giftflag='{lFlag}' WHERE a_index={nRowIndex};", out long _))
			{
				try
				{
					DataRow? pMoonstoneRewardTableRow = pMain.pTables.MoonstoneRewardTable?.Select("a_index=" + nRowIndex).FirstOrDefault();
					if (pMoonstoneRewardTableRow != null)
					{
						pMoonstoneRewardTableRow["a_type"] = nMoonstoneID;
						pMoonstoneRewardTableRow["a_giftindex"] = nItemID;
						pMoonstoneRewardTableRow["a_giftcount"] = nAmount;
						pMoonstoneRewardTableRow["a_giftprob"] = fProb;
						pMoonstoneRewardTableRow["a_giftflag"] = lFlag;
					}
				}
				catch (Exception ex)
				{
					string strError = $"Moonstone Editor > Reward: {strGroupName}: {nRowIndex} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Moonstone Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
			}
			else
			{
				string strError = $"Moonstone Editor > Reward: {strGroupName}: {nRowIndex} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Moonstone Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

				bSuccess = false;
			}

			if (bSuccess)
			{
				if (cbChangesAppliedNotification.Checked)
					MessageBox.Show("Changes applied successfully!", "Moonstone Editor", MessageBoxButtons.OK);
				else
					pMain.Logger(LogTypes.Success, $"Moonstone Editor > Reward: {strGroupName}: {nRowIndex} Changes applied successfully!");

				nLastRowEdited = -1;
			}
		}

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
					if (pMain.pTables.MoonstoneRewardTable != null)
					{
						foreach (int nRowID in nRowIDS)
						{
							DataRow? pRow = pMain.pTables.MoonstoneRewardTable?.Select("a_index=" + nRowID).FirstOrDefault();
							if (pRow != null)
								pMain.pTables.MoonstoneRewardTable?.Rows.Remove(pRow);
						}
					}
				}
				catch (Exception ex)
				{
					string strError = $"Moonstone Editor > Reward{((nRowIDS.Count > 1) ? "s" : "")}: {strGroupName}: {strRowIDS} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Moonstone Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
			}
			else
			{
				string strError = $"Moonstone Editor > Reward{((nRowIDS.Count > 1) ? "s" : "")}: {strGroupName}: {strRowIDS} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Moonstone Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

				bSuccess = false;
			}

			if (bSuccess)
			{
				if (cbChangesAppliedNotification.Checked)
					MessageBox.Show("Changes applied successfully!", "Moonstone Editor", MessageBoxButtons.OK);
				else
					pMain.Logger(LogTypes.Success, $"Moonstone Editor > Reward{((nRowIDS.Count > 1) ? "s" : "")}: {strGroupName}: {strRowIDS} Changes applied successfully!");
			}

			return bSuccess;
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }

		private void tbSearch_KeyDown(object sender, KeyEventArgs e) { nSearchPosition = pMain.SearchInGridView(tbSearch, e, gridRewards, nSearchPosition); }

		private void cbChangesAppliedNotification_CheckedChanged(object sender, EventArgs e)
		{
			bool bState = cbChangesAppliedNotification.Checked;

			pMain.pSettings.ChangesAppliedNotification[this.Name] = bState;

			pMain.WriteToINI(pMain.pSettings.SettingsFile, "ChangesAppliedNotification", this.Name, bState.ToString());
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			btnReload.Enabled = false;

			nSearchPosition = 0;

			pMain.pTables.MoonstoneRewardTable?.Dispose();
			pMain.pTables.MoonstoneRewardTable = null;

			pMain.pTables.ItemTable?.Dispose();
			pMain.pTables.ItemTable = null;

			MoonstoneEditor_LoadAsync(sender, e);
		}
		/****************************************/
		private void gridRewards_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
		{
			if (bUserAction && gridRewards.Rows[e.RowIndex].Cells[0].Tag?.ToString() != "GROUP")
			{
				gridRewards.Rows[e.RowIndex].Cells["prob"].ToolTipText = ((float.Parse(gridRewards.Rows[e.RowIndex].Cells["prob"].Value.ToString()) * 100.0f) / 10000.0f) + "%";
				/****************************************/
				if (Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
				{
					gridRewards.Rows[e.RowIndex].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["amount"].Value));
					gridRewards.Rows[e.RowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
				}
			}

			if (bUserAction && nLastRowEdited != e.RowIndex && gridRewards.Rows[e.RowIndex].Cells[0].Tag?.ToString() != "GROUP")
			{
				nLastRowEdited = e.RowIndex;

				DoUPDATE(e.RowIndex);
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
							bUserAction = false;

							gridRewards.Rows[e.RowIndex].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
							gridRewards.Rows[e.RowIndex].Cells["item"].Value = $"{nItemID} - {pItemSelector.ReturnValues[1]}";
							gridRewards.Rows[e.RowIndex].Cells["item"].Tag = nItemID;

							if (nItemID == Defs.NAS_ITEM_DB_INDEX)
							{
								gridRewards.Rows[e.RowIndex].Cells["flag"].Value = 0;
								gridRewards.Rows[e.RowIndex].Cells["flag"].ToolTipText = "";
								gridRewards.Rows[e.RowIndex].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["amount"].Value));
								gridRewards.Rows[e.RowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
							}
							else
							{
								gridRewards.Rows[e.RowIndex].Cells["amount"].Style.ForeColor = Color.FromArgb(208, 203, 148);
								gridRewards.Rows[e.RowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(40, 40, 40);
							}

							DoUPDATE(e.RowIndex);

							bUserAction = true;
						}
					}
					else if (e.ColumnIndex == 4 && e.RowIndex >= 0)    // Flag Selector
					{
						if (Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
							return;

						FlagPicker pFlagSelector = new(this, Defs.ItemFlags, Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["flag"].Value));
						if (pFlagSelector.ShowDialog() != DialogResult.OK)
							return;

						bUserAction = false;

						gridRewards.Rows[e.RowIndex].Cells["flag"].Value = pFlagSelector.ReturnValues;

						StringBuilder strTooltip = new();

						for (int i = 0; i < Defs.ItemFlags.Length; i++)
						{
							if ((pFlagSelector.ReturnValues & 1L << i) != 0)
								strTooltip.Append(Defs.ItemFlags[i] + "\n");
						}

						gridRewards.Rows[e.RowIndex].Cells["flag"].ToolTipText = strTooltip.ToString();

						DoUPDATE(e.RowIndex);

						bUserAction = true;
					}
				}
				else if (e.Button == MouseButtons.Right && e.ColumnIndex == -1) // Only in Header Cell
				{
					bool bIsHeader = e.RowIndex == -1;
					bool bIsGroupHeader = !bIsHeader && gridRewards.Rows[e.RowIndex].Cells[0].Tag?.ToString() == "GROUP";

					ToolStripMenuItem addItem = new("Add New") { Enabled = !bIsHeader && bIsGroupHeader };
					addItem.Click += (_, _) =>
					{
						ItemPicker pItemSelector = new(pMain, this, 0, false);
						if (pItemSelector.ShowDialog() != DialogResult.OK)
							return;

						int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
						if (nItemID > 0)
						{
							bUserAction = false;

							int nDefaultAmount = 1;
							float fDefaultProbability = 10.0f;
							long lDefaultFlag = 0;

							if (nItemID != Defs.NAS_ITEM_DB_INDEX)
								lDefaultFlag = 1025;

							var (bProceed, lLastInsertID) = DoINSERT([e.RowIndex, nItemID, nDefaultAmount, fDefaultProbability, lDefaultFlag]);
							if (bProceed)
							{
								if (!Convert.ToBoolean(gridRewards.Rows[e.RowIndex].Cells[1].Tag))
									ChangeGroupVisibleState(e.RowIndex);

								int nRow = e.RowIndex + 1;
								int nNumber = nRow;

								while (nRow < gridRewards.Rows.Count && gridRewards.Rows[nRow].Cells[0].Tag?.ToString() != "GROUP")
									nRow++;

								gridRewards.Rows.Insert(nRow);

								gridRewards.Rows[nRow].HeaderCell.Value = (nRow - nNumber + 1).ToString();
								gridRewards.Rows[nRow].HeaderCell.Tag = lLastInsertID;

								gridRewards.Rows[nRow].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
								gridRewards.Rows[nRow].Cells["item"].Value = nItemID + " - " + pItemSelector.ReturnValues[1].ToString();
								gridRewards.Rows[nRow].Cells["item"].Tag = nItemID;

								if (nItemID == Defs.NAS_ITEM_DB_INDEX)
								{
									gridRewards.Rows[nRow].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["amount"].Value));
									gridRewards.Rows[nRow].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
								}

								gridRewards.Rows[nRow].Cells["amount"].Value = nDefaultAmount;
								gridRewards.Rows[nRow].Cells["prob"].Value = fDefaultProbability;
								gridRewards.Rows[nRow].Cells["prob"].ToolTipText = ((fDefaultProbability * 100.0f) / 10000.0f) + "%";

								if (nItemID != Defs.NAS_ITEM_DB_INDEX)
								{
									StringBuilder strTooltip = new();

									for (int i = 0; i < Defs.ItemFlags.Length; i++)
									{
										if ((lDefaultFlag & 1L << i) != 0)
											strTooltip.Append(Defs.ItemFlags[i] + "\n");
									}

									gridRewards.Rows[nRow].Cells["flag"].ToolTipText = strTooltip.ToString();
								}

								gridRewards.Rows[nRow].Cells["flag"].Value = lDefaultFlag;

								gridRewards.FirstDisplayedScrollingRowIndex = e.RowIndex;
								gridRewards.Rows[nRow].Selected = true;
							}

							bUserAction = true;
						}
					};

					ToolStripMenuItem deleteItem = new("Delete") { Enabled = !bIsHeader && !bIsGroupHeader };
					deleteItem.Click += (_, _) =>
					{
						bUserAction = false;

						int nGroupRowIndex = GetGroupRowID(e.RowIndex);

						if (DoDELETE(nGroupRowIndex, new List<int> { Convert.ToInt32(gridRewards.Rows[e.RowIndex].HeaderCell.Tag) }))
						{
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

						bUserAction = true;
					};

					ToolStripMenuItem deleteAllItems = new("Delete All from this Moonstone") { Enabled = !bIsHeader && bIsGroupHeader };
					deleteAllItems.Click += (_, _) =>
					{
						DialogResult pDialogReturn = MessageBox.Show($"You sure want to Delete All the Rewards of {gridRewards.Rows[e.RowIndex].Cells[1].Value}?", "Moonstone Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						if (pDialogReturn == DialogResult.No)
							return;

						int i = 1;
						List<int> nRowIDSToDelete = new();

						while (e.RowIndex + i < gridRewards.Rows.Count && gridRewards.Rows[e.RowIndex + i].Cells[0].Tag?.ToString() != "GROUP")
						{
							nRowIDSToDelete.Add(Convert.ToInt32(gridRewards.Rows[e.RowIndex + i].HeaderCell.Tag));

							i++;
						}

						if (DoDELETE(e.RowIndex, nRowIDSToDelete))
						{
							gridRewards.SuspendLayout();

							while (e.RowIndex + 1 < gridRewards.Rows.Count && gridRewards.Rows[e.RowIndex + 1].Cells[0].Tag?.ToString() != "GROUP")
								gridRewards.Rows.RemoveAt(e.RowIndex + 1);

							gridRewards.ResumeLayout();

							gridRewards.Rows[e.RowIndex].Cells[1].Tag = true;
							gridRewards.Rows[e.RowIndex].HeaderCell.Value = "-";
						}
					};

					cmRewards = new ContextMenuStrip();
					cmRewards.Items.AddRange(addItem, deleteItem, deleteAllItems);
					cmRewards.Show(Cursor.Position);
				}
			}
		}
	}
}
