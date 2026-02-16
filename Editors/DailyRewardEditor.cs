namespace LastChaos_ToolBoxNG
{
	public partial class DailyRewardEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private int nSearchPosition = 0;
		private int nLastRowEdited = -1;
		private ContextMenuStrip? cmRewards;

		public DailyRewardEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridRewards.TopLeftHeaderCell.Value = "Day";

			cbChangesAppliedNotification.Checked = pMain.pSettings.ChangesAppliedNotification[this.Name];
		}

		private void DoINSERT(int nRowID)
		{
			bool bSuccess = true;
			int nDay = Convert.ToInt32(gridRewards.Rows[nRowID].HeaderCell.Value);
			/****************************************/
			int nItemID = Convert.ToInt32(gridRewards.Rows[nRowID].Cells["item"].Tag);
			long lFlag = Convert.ToInt64(gridRewards.Rows[nRowID].Cells["flag"].Value);
			long nAmount = Convert.ToInt64(gridRewards.Rows[nRowID].Cells["amount"].Value);

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"INSERT INTO {pMain.pSettings.DBData}.t_daily_reward_item (a_index, a_item_idx, a_item_flag, a_item_amount) VALUES ({nDay}, '{nItemID}', '{lFlag}', '{nAmount}');", out long _))
			{
				try
				{
					DataRow? pDailyRewardRow = pMain.pTables.DailyRewardTable?.NewRow();
					if (pDailyRewardRow != null)
					{
						pDailyRewardRow["a_index"] = nDay;
						pDailyRewardRow["a_item_idx"] = nItemID;
						pDailyRewardRow["a_item_flag"] = lFlag;
						pDailyRewardRow["a_item_amount"] = nAmount;

						pMain.pTables.DailyRewardTable?.Rows.Add(pDailyRewardRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Daily Reward Editor > Reward: {nDay} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Daily Reward Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (cbChangesAppliedNotification.Checked)
							MessageBox.Show("Changes applied successfully!", "Daily Reward Editor", MessageBoxButtons.OK);
						else
							pMain.Logger(LogTypes.Success, $"Daily Reward Editor > Reward: {nDay} Changes applied successfully!");
					}
				}
			}
			else
			{
				string strError = $"Daily Reward Editor > Reward: {nDay} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Daily Reward Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void DoUPDATE(int nRowID)
		{
			bool bSuccess = true;
			int nDay = Convert.ToInt32(gridRewards.Rows[nRowID].HeaderCell.Value);
			/****************************************/
			int nItemID = Convert.ToInt32(gridRewards.Rows[nRowID].Cells["item"].Tag);
			long lFlag = Convert.ToInt64(gridRewards.Rows[nRowID].Cells["flag"].Value);
			long nAmount = Convert.ToInt64(gridRewards.Rows[nRowID].Cells["amount"].Value);

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"UPDATE {pMain.pSettings.DBData}.t_daily_reward_item SET a_item_idx={nItemID}, a_item_flag={lFlag}, a_item_amount='{nAmount}' WHERE a_index={nDay};", out long _))
			{
				try
				{
					DataRow? pDailyRewardRow = pMain.pTables.DailyRewardTable?.Select("a_index=" + nDay).FirstOrDefault();
					if (pDailyRewardRow != null)
					{
						pDailyRewardRow["a_item_idx"] = nItemID;
						pDailyRewardRow["a_item_flag"] = lFlag;
						pDailyRewardRow["a_item_amount"] = nAmount;
					}
				}
				catch (Exception ex)
				{
					string strError = $"Daily Reward Editor > Reward: {nDay} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Daily Reward Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (cbChangesAppliedNotification.Checked)
							MessageBox.Show("Changes applied successfully!", "Daily Reward Editor", MessageBoxButtons.OK);
						else
							pMain.Logger(LogTypes.Success, $"Daily Reward Editor > Reward: {nDay} Changes applied successfully!");

						nLastRowEdited = -1;
					}
				}
			}
			else
			{
				string strError = $"Daily Reward Editor > Reward: {nDay} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Daily Reward Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task LoadDailyRewardsDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string> { "a_item_idx", "a_item_flag", "a_item_amount" };

			if (pMain.pTables.DailyRewardTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.DailyRewardTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_daily_reward_item ORDER BY a_index;");
				});

				if (pMain.pTables.DailyRewardTable == null)
					pMain.pTables.DailyRewardTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.DailyRewardTable);
			}
		}

		private async void DailyRewardEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadDailyRewardsDataAsync(),
				pMain.GenericLoadItemDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Daily Rewards & Items Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			bUserAction = false;
			/****************************************/
			// Reset Controls
			nLastRowEdited = -1;

			gridRewards.Rows.Clear();
			/****************************************/
			// General
			if (pMain.pTables.DailyRewardTable != null)
			{
				string strItemName;
				int nItemID, nDay = 0;
				StringBuilder strTooltip = new();

				gridRewards.SuspendLayout();

				foreach (DataRow pRow in pMain.pTables.DailyRewardTable.Rows)
				{
					gridRewards.Rows.Insert(nDay);

					gridRewards.Rows[nDay].HeaderCell.Value = (nDay + 1).ToString();

					nItemID = Convert.ToInt32(pRow["a_item_idx"]);
					strItemName = nItemID.ToString();

					if (nItemID > 0)
					{
						DataRow? pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).FirstOrDefault();
						if (pItemRow != null)
						{
							gridRewards.Rows[nDay].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));

							strItemName += $" - {pItemRow["a_name_" + pMain.pSettings.WorkLocale]}";
						}
						else
						{
							pMain.Logger(LogTypes.Error, $"Daily Reward Editor > Reward: {pRow["a_index"]} Error: a_item_idx: {nItemID} not exist in t_item.");
						}
					}
					else
					{
						pMain.Logger(LogTypes.Error, $"Daily Reward Editor > Reward: {pRow["a_index"]} Error: No Item defined.");
					}

					gridRewards.Rows[nDay].Cells["item"].Value = strItemName;
					gridRewards.Rows[nDay].Cells["item"].Tag = nItemID;
					gridRewards.Rows[nDay].Cells["flag"].Value = pRow["a_item_flag"];

					strTooltip.Clear();
					strTooltip = new StringBuilder();

					long lFlag = Convert.ToInt64(pRow["a_item_flag"]);

					for (int i = 0; i < Defs.ItemFlags.Length; i++)
					{
						if ((lFlag & 1L << i) != 0)
							strTooltip.Append(Defs.ItemFlags[i] + "\n");
					}

					gridRewards.Rows[nDay].Cells["flag"].ToolTipText = strTooltip.ToString();

					gridRewards.Rows[nDay].Cells["amount"].Value = pRow["a_item_amount"];

					if (nItemID == Defs.NAS_ITEM_DB_INDEX)
					{
						gridRewards.Rows[nDay].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(pRow["a_item_amount"]));
						gridRewards.Rows[nDay].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
					}

					nDay++;
				}

				gridRewards.ResumeLayout();
			}
			/****************************************/
			bUserAction = true;
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Daily Rewards & Items Data from Database");
			/****************************************/
			pProgressDialog.Close();

			btnReload.Enabled = true;

			gridRewards.Enabled = true;
			gridRewards.Focus();
		}

		private void DailyRewardEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (cmRewards != null)
			{
				cmRewards.Dispose();
				cmRewards = null;
			}
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }

		private void tbSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				void Search()
				{
					int i = 0;
					string[] searchTokens = pMain.NormalizeText(tbSearch.Text).Split(' ', StringSplitOptions.RemoveEmptyEntries);

					foreach (DataGridViewRow row in gridRewards.Rows)
					{
						if (pMain.ContainsAllTokens(row.Cells["item"].Value.ToString(), searchTokens) && i > nSearchPosition)
						{
							gridRewards.FirstDisplayedScrollingRowIndex = row.Index;
							row.Selected = true;
							nSearchPosition = row.Index;
							return;
						}

						i++;
					}

					for (i = 0; i <= nSearchPosition; i++)
					{
						if (pMain.ContainsAllTokens(gridRewards.Rows[i].Cells["item"].Value.ToString(), searchTokens))
						{
							gridRewards.FirstDisplayedScrollingRowIndex = gridRewards.Rows[i].Index;
							gridRewards.Rows[i].Selected = true;
							nSearchPosition = gridRewards.Rows[i].Index;
							return;
						}
					}
				}

				Search();

				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void cbChangesAppliedNotification_CheckedChanged(object sender, EventArgs e)
		{
			bool bState = cbChangesAppliedNotification.Checked;

			pMain.pSettings.ChangesAppliedNotification[this.Name] = bState;

			pMain.WriteToINI(pMain.pSettings.SettingsFile, "ChangesAppliedNotification", this.Name, bState.ToString());
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			gridRewards.Enabled = false;
			btnReload.Enabled = false;

			nSearchPosition = 0;

			pMain.pTables.DailyRewardTable?.Dispose();
			pMain.pTables.DailyRewardTable = null;

			pMain.pTables.ItemTable?.Dispose();
			pMain.pTables.ItemTable = null;

			DailyRewardEditor_LoadAsync(sender, e);
		}
		/****************************************/
		private void gridRewards_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
		{
			if (bUserAction)
			{
				if (Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
				{
					gridRewards.Rows[e.RowIndex].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["amount"].Value));
					gridRewards.Rows[e.RowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
				}

				if (nLastRowEdited != e.RowIndex)
				{
					nLastRowEdited = e.RowIndex;

					DoUPDATE(e.RowIndex);
				}
			}
		}

		private void gridRewards_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Right && e.ColumnIndex == -1) // Header Column
				{
					ToolStripMenuItem addItem = new("Add New");
					addItem.Click += (_, _) =>
					{
						ItemPicker pItemSelector = new(pMain, this, 0, false);
						if (pItemSelector.ShowDialog() != DialogResult.OK)
							return;

						int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
						if (nItemID > 0)
						{
							bUserAction = false;

							int nRow = gridRewards.Rows.Count;

							gridRewards.Rows.Insert(nRow);

							gridRewards.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

							gridRewards.Rows[nRow].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
							gridRewards.Rows[nRow].Cells["item"].Value = $"{nItemID} - {pItemSelector.ReturnValues[1]}";
							gridRewards.Rows[nRow].Cells["item"].Tag = nItemID;

							long lDefaultFlag = 0;

							if (nItemID != Defs.NAS_ITEM_DB_INDEX)
							{
								lDefaultFlag = 1025;

								StringBuilder strTooltip = new();

								for (int i = 0; i < Defs.ItemFlags.Length; i++)
								{
									if ((lDefaultFlag & 1L << i) != 0)
										strTooltip.Append(Defs.ItemFlags[i] + "\n");
								}

								gridRewards.Rows[nRow].Cells["flag"].ToolTipText = strTooltip.ToString();
							}

							gridRewards.Rows[nRow].Cells["flag"].Value = lDefaultFlag;
							gridRewards.Rows[nRow].Cells["amount"].Value = 1;

							if (nItemID == Defs.NAS_ITEM_DB_INDEX)
							{
								gridRewards.Rows[nRow].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[nRow].Cells["amount"].Value));
								gridRewards.Rows[nRow].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
							}

							gridRewards.FirstDisplayedScrollingRowIndex = nRow;
							gridRewards.Rows[nRow].Selected = true;

							DoINSERT(nRow);

							bUserAction = true;
						}
					};

					ToolStripMenuItem copyItem = new("Copy");
					copyItem.Enabled = e.RowIndex >= 0;
					copyItem.Click += (_, _) =>
					{
						bUserAction = false;

						int nRow = gridRewards.Rows.Count;

						gridRewards.Rows.Insert(nRow);

						gridRewards.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

						gridRewards.Rows[nRow].Cells["itemIcon"].Value = gridRewards.Rows[e.RowIndex].Cells["itemIcon"].Value;
						gridRewards.Rows[nRow].Cells["item"].Value = gridRewards.Rows[e.RowIndex].Cells["item"].Value;
						gridRewards.Rows[nRow].Cells["item"].Tag = gridRewards.Rows[e.RowIndex].Cells["item"].Tag;
						gridRewards.Rows[nRow].Cells["flag"].Value = gridRewards.Rows[e.RowIndex].Cells["flag"].Value;
						gridRewards.Rows[nRow].Cells["amount"].Value = gridRewards.Rows[e.RowIndex].Cells["amount"].Value;

						if (Convert.ToInt32(gridRewards.Rows[nRow].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
						{
							gridRewards.Rows[nRow].Cells["amount"].Style.ForeColor = gridRewards.Rows[e.RowIndex].Cells["amount"].Style.ForeColor;
							gridRewards.Rows[nRow].Cells["amount"].Style.BackColor = gridRewards.Rows[e.RowIndex].Cells["amount"].Style.BackColor;
						}

						gridRewards.FirstDisplayedScrollingRowIndex = nRow;
						gridRewards.Rows[nRow].Selected = true;

						DoINSERT(nRow);

						bUserAction = true;
					};

					cmRewards = new ContextMenuStrip();
					cmRewards.Items.AddRange(addItem, copyItem);
					cmRewards.Show(Cursor.Position);
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 1 && e.RowIndex >= 0) // Item Selector
				{
					ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag), false);
					if (pItemSelector.ShowDialog() != DialogResult.OK)
						return;

					int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
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
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 2 && e.RowIndex >= 0) // Flag Selector
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
		}
	}
}
