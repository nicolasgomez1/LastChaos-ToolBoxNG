namespace LastChaos_ToolBoxNG
{
	public partial class MonsterComboEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempMissionCase;
		private ToolTip? pToolTip;
		private Dictionary<Control, ToolTip>? pToolTips = new();
		private ContextMenuStrip? cmRegens;

		public MonsterComboEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridRegens.TopLeftHeaderCell.Value = "N°";
			((DataGridViewComboBoxColumn)gridRegens.Columns["step"]).DataSource = Enumerable.Range(1, Defs.MAX_MISSIONCASE_STEPS).ToList();
			((DataGridViewComboBoxColumn)gridRegens.Columns["step"]).ValueType = typeof(int);
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.MissionCaseTable.Select("a_index=" + pTempMissionCase["a_index"]).FirstOrDefault() != null)   // the current selected Monster Combo, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Monster Combo Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else    // the current selected Monster Combo is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Combo is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Monster Combo Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private async Task LoadMonsterComboDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string> { "a_enable", "a_nas", "a_texture_id", "a_texture_row", "a_texture_col", "a_point" };

			foreach (string strNation in pMain.pSettings.NationSupported)
				listQueryCompose.Add("a_name_" + strNation.ToLower());

			if (pMain.pTables.MissionCaseTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.MissionCaseTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_missioncase ORDER BY a_index;");
				});

				if (pMain.pTables.MissionCaseTable == null)
					pMain.pTables.MissionCaseTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.MissionCaseTable);
			}

			bRequestNeeded = false;
			listQueryCompose.Clear();

			listQueryCompose = new List<string> { "a_npcidx", "a_count", "a_enable", "a_step" };

			if (pMain.pTables.NPCRegenComboTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.NPCRegenComboTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_missioncase_idx, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_npc_regen_combo ORDER BY a_step;");
				});

				if (pMain.pTables.NPCRegenComboTable == null)
					pMain.pTables.NPCRegenComboTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_missioncase_idx", ref pMain.pTables.NPCRegenComboTable);
			}
		}

		private void LoadRegenData(int nCaseID)
		{
			int i = 0, nNPCID;
			string strNPCName;
			DataRow? pNPCRow;
			DataRow[]? pTempCaseRegensRows = pMain.pTables.NPCRegenComboTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_missioncase_idx"]) == nCaseID).ToArray();

			if (pTempCaseRegensRows != null && pTempCaseRegensRows.Length > 0)
			{
				gridRegens.SuspendLayout();

				foreach (DataRow pRow in pTempCaseRegensRows)
				{
					gridRegens.Rows.Insert(i);

					gridRegens.Rows[i].HeaderCell.Value = (i + 1).ToString();

					nNPCID = Convert.ToInt32(pRow["a_npcidx"]);
					strNPCName = nNPCID.ToString();

					if (nNPCID > 0)
					{
						pNPCRow = pMain.pTables.NPCTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nNPCID).FirstOrDefault();
						if (pNPCRow != null)
							strNPCName += " - " + pNPCRow["a_name_" + pMain.pSettings.WorkLocale].ToString();
					}

					gridRegens.Rows[i].Cells["mob"].Value = strNPCName;
					gridRegens.Rows[i].Cells["mob"].Tag = nNPCID;
					gridRegens.Rows[i].Cells["amount"].Value = pRow["a_count"];

					bool bChecked = false;

					if (pRow["a_enable"].ToString() == "1")
						bChecked = true;

					gridRegens.Rows[i].Cells["enable"].Value = bChecked;

					int nStep = Convert.ToInt32(pRow["a_step"]);
					int nMin = ((List<int>)((DataGridViewComboBoxColumn)gridRegens.Columns["step"]).DataSource).Min();
					int nMax = ((List<int>)((DataGridViewComboBoxColumn)gridRegens.Columns["step"]).DataSource).Max();

					if (nStep < nMin || nStep > nMax)
					{
						nStep = nMax;

						pMain.Logger(LogTypes.Error, $"Monster Combo Editor > Regen: {pRow["a_missioncase_idx"]} Error: a_step out of range (1 was asigned to it).");
					}

					gridRegens.Rows[i].Cells["step"].Value = nStep;

					i++;
				}

				gridRegens.ResumeLayout();
			}
		}

		private async void MonsterComboEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
			cbNationSelector.BeginUpdate();

			for (int i = 0; i < pMain.pSettings.NationSupported.Length; i++)
			{
				string strNation = pMain.pSettings.NationSupported[i];

				cbNationSelector.Items.Add(strNation);

				if (strNation.ToLower() == pMain.pSettings.WorkLocale)
					cbNationSelector.SelectedIndex = i;
			}

			cbNationSelector.EndUpdate();
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadMonsterComboDataAsync(),
				pMain.GenericLoadNPCDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Monster Combos, Monster Combo Regens & NPCs Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.MissionCaseTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.MissionCaseTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_index"]), pRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Monster Combos, Monster Combo Regens & NPCs Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void MonsterComboEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				foreach (var toolTip in pToolTips.Values.Distinct())
					toolTip.Dispose();

				pToolTips = null;

				if (cmRegens != null)
				{
					cmRegens.Dispose();
					cmRegens = null;
				}
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Monster Combo Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nCaseID, bool bLoadFrompMissionCaseTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			pbIcon.Image = null;

			foreach (var toolTip in pToolTips.Values)
				toolTip.Dispose();

			gridRegens.Rows.Clear();
			/****************************************/
			if (bLoadFrompMissionCaseTable)
			{
				pTempMissionCase = pMain.pTables.MissionCaseTable.NewRow();
				pTempMissionCase.ItemArray = (object[])pMain.pTables.MissionCaseTable.Select("a_index=" + nCaseID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nCaseID.ToString();
			/****************************************/
			if (pTempMissionCase["a_enable"].ToString() == "1")
				cbEnable.Checked = true;
			else
				cbEnable.Checked = false;
			/****************************************/
			tbPoint.Text = pTempMissionCase["a_point"].ToString();
			/****************************************/
			tbGoldCost.Text = pTempMissionCase["a_nas"].ToString();
			/****************************************/
			tbName.Text = pTempMissionCase["a_name_" + cbNationSelector.SelectedItem.ToString().ToLower()].ToString();
			/****************************************/
			string strTexID = pTempMissionCase["a_texture_id"].ToString();
			string strTexRow = pTempMissionCase["a_texture_row"].ToString();
			string strTexCol = pTempMissionCase["a_texture_col"].ToString();

			Image pIcon = pMain.GetIcon("ComboBtn", strTexID, Convert.ToInt32(strTexRow), Convert.ToInt32(strTexCol));
			if (pIcon != null)
			{
				pbIcon.Image = pIcon;

				pToolTip = new ToolTip();
				pToolTip.SetToolTip(pbIcon, $"FILE: {strTexID} ROW: {strTexRow} COL: " + strTexCol);
				pToolTips[pbIcon] = pToolTip;
			}
			/****************************************/
			LoadRegenData(nCaseID);
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
			void Reload()
			{
				MainList.Enabled = false;
				btnReload.Enabled = false;

				nSearchPosition = 0;

				pMain.pTables.MissionCaseTable?.Dispose();
				pMain.pTables.MissionCaseTable = null;

				pMain.pTables.NPCRegenComboTable?.Dispose();
				pMain.pTables.NPCRegenComboTable = null;

				pMain.pTables.NPCTable?.Dispose();
				pMain.pTables.NPCTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				MonsterComboEditor_LoadAsync(sender, e);
			}

			var (bProceed, _) = CheckUnsavedChanges();

			if (bProceed)
			{
				bUnsavedChanges = false;

				Reload();
			}
		}

		private void btnAddNew_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bProceed)
			{
				int i, nNewCaseID = 9999;
				DataRow pRow;

				List<string> listIntColumns = new List<string>	// Here add all int columns.
				{
					"a_index",
					"a_enable",
					"a_nas",
					"a_texture_id",
					"a_texture_row",
					"a_texture_col",
					"a_point",
				};

				List<string> listVarcharColumns = new();    // Here add all varchar columns.

				foreach (string strNation in pMain.pSettings.NationSupported)
					listVarcharColumns.Add("a_name_" + strNation.ToLower());

				if (pMain.pTables.MissionCaseTable == null) // If is null, create new DataTable and set schema (column name & datatype).
				{
					DataTable pMissionCaseTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pMissionCaseTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listVarcharColumns)
						pMissionCaseTableStruct.Columns.Add(strColumnName, typeof(string));

					pRow = pMissionCaseTableStruct.NewRow();

					pMissionCaseTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_missioncase ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewCaseID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewCaseID = pMain.AskForIndex(this.Text, "a_index")) == -1)   // I don't test it...
							return;
					}

					QueryReturn = null;
				}
				else
				{
					nNewCaseID = Convert.ToInt32(pMain.pTables.MissionCaseTable.Select().LastOrDefault()["a_index"]) + 1;

					pRow = pMain.pTables.MissionCaseTable.NewRow();
				}

				List<object> listDefaultValue = new List<object>
				{
					nNewCaseID,	// a_index
					0,	// a_enable
					0,	// a_nas
					0,	// a_texture_id
					0,	// a_texture_row
					0,	// a_texture_col
					0	// a_point
				};

				foreach (string strNation in pMain.pSettings.NationSupported)
					listDefaultValue.Add("New Combo");

				i = 0;
				foreach (string strColumnName in listIntColumns.Concat(listVarcharColumns))
				{
					pRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempMissionCase = pRow;
				}
				catch (Exception ex)
				{
					string strError = $"Monster Combo Editor > Combo: {nNewCaseID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Monster Combo Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewCaseID, "New Combo", true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Combo. Because it's temporary.", "Monster Combo Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nComboIDToCopy = Convert.ToInt32(pTempMissionCase["a_index"]);
				int nNewComboID = Convert.ToInt32(pMain.pTables.MissionCaseTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempMissionCase = pMain.pTables.MissionCaseTable.NewRow();
				pTempMissionCase.ItemArray = (object[])pMain.pTables.MissionCaseTable.Select("a_index=" + nComboIDToCopy)[0].ItemArray.Clone();

				pTempMissionCase["a_index"] = nNewComboID;

				foreach (string strNation in pMain.pSettings.NationSupported)
					pTempMissionCase["a_name_" + strNation.ToLower()] = pTempMissionCase["a_name_" + strNation.ToLower()] + " Copy";

				AddToList(nNewComboID, pTempMissionCase["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, true);

				LoadRegenData(nComboIDToCopy);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nComboID = Convert.ToInt32(pTempMissionCase["a_index"]);
			DataRow pMissionCaseRow = pMain.pTables.MissionCaseTable.Select("a_index=" + nComboID).FirstOrDefault();

			if (pMissionCaseRow != null)
			{
				StringBuilder strbuilderQuery = new();

				strbuilderQuery.Append("START TRANSACTION;\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_missioncase WHERE a_index={nComboID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_regen_combo WHERE a_missioncase_idx={nComboID};\n");

				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _)))
				{
					string strError = $"Monster Combo Editor > Combo: {nComboID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Monster Combo Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pMain.pTables.NPCRegenComboTable != null)
					{
						DataRow[] pRows = pMain.pTables.NPCRegenComboTable.Select("a_missioncase_idx=" + nComboID);

						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.NPCRegenComboTable.Rows.Remove(pRow);
						}
					}

					pMain.pTables.MissionCaseTable.Rows.Remove(pMissionCaseRow);
				}
				catch (Exception ex)
				{
					string strError = $"Monster Combo Editor > Combo: {nComboID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Monster Combo Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Combo Deleted successfully!", "Monster Combo Editor", MessageBoxButtons.OK);

						MainList.SelectedIndex = nPrevObjectID;

						bUnsavedChanges = false;
					}
				}
			}
		}
		/****************************************/
		private void cbEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempMissionCase["a_enable"] = cbEnable.Checked ? "1" : "0";

				bUnsavedChanges = true;
			}
		}

		private void tbPoint_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempMissionCase["a_point"] = tbPoint.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbGoldCost_TextChanged(object sender, EventArgs e)
		{
			tbGoldCost.ForeColor = pMain.GetGoldColor(Convert.ToInt64(tbGoldCost.Text));

			if (bUserAction)
			{
				pTempMissionCase["a_nas"] = tbGoldCost.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbNationSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				bUserAction = false;

				tbName.Text = pTempMissionCase["a_name_" + cbNationSelector.SelectedItem.ToString().ToLower()].ToString();

				bUserAction = true;
			}
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempMissionCase["a_name_" + cbNationSelector.SelectedItem?.ToString()?.ToLower()] = tbName.Text;

				bUnsavedChanges = true;
			}
		}

		private void pbIcon_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				IconPicker pIconSelector = new(pMain, this, "ComboBtn");
				if (pIconSelector.ShowDialog() != DialogResult.OK)
					return;

				string[] strReturns = pIconSelector.ReturnValues;

				pTempMissionCase["a_texture_id"] = strReturns[0];
				pTempMissionCase["a_texture_row"] = strReturns[1];
				pTempMissionCase["a_texture_col"] = strReturns[2];

				pbIcon.Image = pMain.GetIcon("ComboBtn", strReturns[0], Convert.ToInt32(strReturns[1]), Convert.ToInt32(strReturns[2]));

				pToolTips[pbIcon].SetToolTip(pbIcon, $"FILE: {strReturns[0]} ROW: {strReturns[1]} COL: " + strReturns[2]);

				bUnsavedChanges = true;
			}
		}

		private void gridLevels_CellValueChanged(object? sender, DataGridViewCellEventArgs e) { if (bUserAction) bUnsavedChanges = true; }

		private void gridLevels_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Right && e.ColumnIndex == -1)  // Header Column
				{
					ToolStripMenuItem addItem = new("Add New");
					addItem.Click += (_, _) =>
					{
						NPCPicker pNPCSelector = new(pMain, this, 0, false);
						if (pNPCSelector.ShowDialog() != DialogResult.OK)
							return;

						int nNPCID = Convert.ToInt32(pNPCSelector.ReturnValues[0]);
						string strNPCName = nNPCID.ToString();

						if (nNPCID > 0)
							strNPCName += " - " + pNPCSelector.ReturnValues[1].ToString();

						int nRow = gridRegens.Rows.Count;

						gridRegens.Rows.Insert(nRow);

						gridRegens.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

						gridRegens.Rows[nRow].Cells["mob"].Value = strNPCName;
						gridRegens.Rows[nRow].Cells["mob"].Tag = nNPCID;
						gridRegens.Rows[nRow].Cells["amount"].Value = 0;

						gridRegens.Rows[nRow].Cells["enable"].Value = true;

						gridRegens.Rows[nRow].Cells["step"].Value = ((List<int>)((DataGridViewComboBoxColumn)gridRegens.Columns["step"]).DataSource).Min();

						gridRegens.FirstDisplayedScrollingRowIndex = nRow;
						gridRegens.Rows[nRow].Selected = true;
					};

					ToolStripMenuItem deleteItem = new("Delete") { Enabled = e.RowIndex >= 0 };
					deleteItem.Click += (_, _) =>
					{
						if (e.RowIndex >= 0)
						{
							gridRegens.SuspendLayout();

							gridRegens.Rows.RemoveAt(e.RowIndex);

							int i = 1;
							foreach (DataGridViewRow row in gridRegens.Rows)
							{
								row.HeaderCell.Value = i.ToString();

								i++;
							}

							gridRegens.ResumeLayout();
						}
					};

					cmRegens = new ContextMenuStrip();
					cmRegens.Items.AddRange(addItem, deleteItem);
					cmRegens.Show(Cursor.Position);
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 0 && e.RowIndex >= 0) // Mob Selector
				{
					NPCPicker pNPCSelector = new(pMain, this, Convert.ToInt32(gridRegens.Rows[e.RowIndex].Cells["mob"].Tag), false);
					if (pNPCSelector.ShowDialog() != DialogResult.OK)
						return;

					int nNPCID = Convert.ToInt32(pNPCSelector.ReturnValues[0]);
					string strNPCName = nNPCID.ToString();

					if (nNPCID > 0)
						strNPCName += " - " + pNPCSelector.ReturnValues[1].ToString();

					gridRegens.Rows[e.RowIndex].Cells["mob"].Value = strNPCName;
					gridRegens.Rows[e.RowIndex].Cells["mob"].Tag = nNPCID;
				}
			}
		}
		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nCaseID = Convert.ToInt32(pTempMissionCase["a_index"]);
			StringBuilder strbuilderQuery = new();

			// Init transaction.
			strbuilderQuery.Append("START TRANSACTION;\n");

			strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_regen_combo WHERE a_missioncase_idx={nCaseID};\n");

			int nMaxLevel = gridRegens.Rows.Count;
			DataRow[] pLevelRows = new DataRow[nMaxLevel];

			if (nMaxLevel > 0)
			{
				int i = 1;
				bool bComposeColumnsNames = true;
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataGridViewRow row in gridRegens.Rows)
				{
					DataRow pRow = pMain.pTables.NPCRegenComboTable.NewRow();

					pRow["a_missioncase_idx"] = nCaseID;
					pRow["a_npcidx"] = Convert.ToInt32(row.Cells["mob"].Tag);
					pRow["a_count"] = Convert.ToInt32(row.Cells["amount"].Value);
					pRow["a_enable"] = Convert.ToInt32(row.Cells["enable"].Value);
					pRow["a_step"] = Convert.ToInt32(row.Cells["step"].Value);

					pLevelRows[i - 1] = pRow;

					strColumnsValues.Append("(");

					foreach (DataColumn pCol in pRow.Table.Columns)
					{
						if (bComposeColumnsNames)
							strColumnsNames.Append(pCol.ColumnName + ", ");

						strColumnsValues.Append($"'{pMain.EscapeChars(pRow[pCol.ColumnName].ToString() ?? string.Empty)}', ");
					}

					strColumnsValues.Length -= 2;

					strColumnsValues.Append("), ");

					bComposeColumnsNames = false;

					i++;
				}

				strColumnsValues.Length -= 2;
				strColumnsNames.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_npc_regen_combo ({strColumnsNames}) VALUES {strColumnsValues};\n");
			}

			// Check if Combo exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pMissionCaseRow = pMain.pTables.MissionCaseTable?.Select("a_index=" + nCaseID).FirstOrDefault();
			if (pMissionCaseRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_missioncase SET");

				foreach (DataColumn pCol in pTempMissionCase.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempMissionCase[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nCaseID};\n");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempMissionCase.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempMissionCase[pCol].ToString() ?? string.Empty)}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_missioncase ({strColumnsNames}) VALUES ({strColumnsValues});\n");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _))
			{
				try
				{
					if (pMain.pTables.NPCRegenComboTable != null)
					{
						DataRow[] pRows = pMain.pTables.NPCRegenComboTable.Select("a_missioncase_idx=" + nCaseID);

						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.NPCRegenComboTable.Rows.Remove(pRow);
						}
					}

					foreach (DataRow pRow in pLevelRows)
					{
						if (pRow != null)
							pMain.pTables.NPCRegenComboTable?.Rows.Add(pRow);
					}

					if (pMissionCaseRow != null)   // Row exist in Global Table, update it.
					{
						pMissionCaseRow.ItemArray = (object[])pTempMissionCase.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pMissionCaseRow = pMain.pTables.MissionCaseTable?.NewRow();
						pMissionCaseRow.ItemArray = (object[])pTempMissionCase.ItemArray.Clone();
						pMain.pTables.MissionCaseTable?.Rows.Add(pMissionCaseRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Monster Combo Editor > Combo: {nCaseID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Monster Combo Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nCaseID;
						pSelectedItem.Text = nCaseID + " - " + tbName.Text.ToString();

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "Monster Combo Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Monster Combo Editor > Combo: {nCaseID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Monster Combo Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
