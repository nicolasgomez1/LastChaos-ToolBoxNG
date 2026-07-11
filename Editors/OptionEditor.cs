namespace LastChaos_ToolBoxNG
{
	public partial class OptionEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempOptionRow;
		private ToolTip? pToolTip;
		private Dictionary<Control, ToolTip>? pToolTips = new();
		private ContextMenuStrip? cmLevels;

		public OptionEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridLevels.TopLeftHeaderCell.Value = "N°";
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.OptionTable.Select("a_index=" + pTempOptionRow["a_index"]).FirstOrDefault() != null)	// the current selected Option, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Option Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else	// the current selected Option is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Option is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Option Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private async Task LoadOptionDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_type", "a_level", "a_prob", "a_weapon_type", "a_wear_type", "a_accessory_type"];

			foreach (string strNation in pMain.pSettings.NationSupported)
				listQueryCompose.Add("a_name_" + strNation.ToLower());

			if (pMain.pTables.OptionTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.OptionTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_option ORDER BY a_index;");
				});

				if (pMain.pTables.OptionTable == null)
					pMain.pTables.OptionTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.OptionTable);
			}
		}

		private async void OptionEditor_LoadAsync(object sender, EventArgs e)
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
			cbTypeSelector.BeginUpdate();

			int j = 0;
			foreach (string strType in Defs.OptionTypes)
			{
				cbTypeSelector.Items.Add($"{j} - {strType}");

				j++;
			}

			cbTypeSelector.EndUpdate();
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await LoadOptionDataAsync();
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Options Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.OptionTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.OptionTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_index"]), pRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Options Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void OptionEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				foreach (var toolTip in pToolTips.Values.Distinct())
					toolTip.Dispose();

				pToolTips = null;

				if (cmLevels != null)
				{
					cmLevels.Dispose();
					cmLevels = null;
				}
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Option Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nOptionID, bool bLoadFrompOptionTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			cbTypeSelector.SelectedIndex = -1;
			gridLevels.Rows.Clear();

			foreach (var toolTip in pToolTips.Values)
				toolTip.Dispose();
			/****************************************/
			if (bLoadFrompOptionTable && pMain.pTables.OptionTable != null)
			{
				pTempOptionRow = pMain.pTables.OptionTable.NewRow();
				pTempOptionRow.ItemArray = (object[])pMain.pTables.OptionTable.Select("a_index=" + nOptionID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nOptionID.ToString();
			/****************************************/
			int nTypeValue = Convert.ToInt32(pTempOptionRow["a_type"]);
			if (nTypeValue >= Defs.OptionTypes.Length)
				pMain.Logger(LogTypes.Error, $"Option Editor > Option: {nOptionID} Error: a_type out of range.");
			else
				cbTypeSelector.SelectedIndex = nTypeValue;
			/****************************************/
			btnWeaponTypeFlag.Text = pTempOptionRow["a_weapon_type"].ToString();

			StringBuilder strTooltip = new();
			long lWeaponFlag = Convert.ToInt64(pTempOptionRow["a_weapon_type"]);
			int i = 0;

			foreach (string strType in Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(0)])
			{
				if ((lWeaponFlag & 1L << i) != 0)
					strTooltip.Append(strType + "\n");
				
				i++;
			}

			if (lWeaponFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"Option Editor > Option: {nOptionID} Error: a_weapon_type out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnWeaponTypeFlag, strTooltip.ToString());
			pToolTips[btnWeaponTypeFlag] = pToolTip;
			/****************************************/
			btnWearTypeFlag.Text = pTempOptionRow["a_wear_type"].ToString();

			long lWearFlag = Convert.ToInt64(pTempOptionRow["a_wear_type"]);
			i = 0;

			foreach (string strType in Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(1)])
			{
				if ((lWearFlag & 1L << i) != 0)
					strTooltip.Append(strType + "\n");

				i++;
			}

			if (lWearFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"Option Editor > Option: {nOptionID} Error: a_wear_type out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnWearTypeFlag, strTooltip.ToString());
			pToolTips[btnWearTypeFlag] = pToolTip;
			/****************************************/
			btnAccessoryTypeFlag.Text = pTempOptionRow["a_accessory_type"].ToString();

			long lAccessoryFlag = Convert.ToInt64(pTempOptionRow["a_accessory_type"]);
			i = 0;

			foreach (string strType in Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(5)])
			{
				if ((lAccessoryFlag & 1L << i) != 0)
					strTooltip.Append(strType + "\n");

				i++;
			}

			if (lAccessoryFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"Option Editor > Option: {nOptionID} Error: a_accessory_type out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnAccessoryTypeFlag, strTooltip.ToString());
			pToolTips[btnAccessoryTypeFlag] = pToolTip;
			/****************************************/
			tbName.Text = pTempOptionRow["a_name_" + cbNationSelector.SelectedItem.ToString().ToLower()].ToString();
			/****************************************/
			i = 0;

			string[] strProb = pTempOptionRow["a_prob"].ToString().Split(' ');

			gridLevels.SuspendLayout();

			foreach (string strLevel in pTempOptionRow["a_level"].ToString().Split(' '))
			{
				gridLevels.Rows.Insert(i);

				gridLevels.Rows[i].HeaderCell.Value = (i + 1).ToString();

				gridLevels.Rows[i].Cells["level"].Value = strLevel;

				if (i < strProb.Length && strProb[i] != null)
					gridLevels.Rows[i].Cells["prob"].Value = strProb[i];
				else
					pMain.Logger(LogTypes.Error, $"Option Editor > Option: {nOptionID} Error: a_level & a_prob not match.");

				i++;
			}

			gridLevels.ResumeLayout();
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

				pMain.pTables.OptionTable?.Dispose();
				pMain.pTables.OptionTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				OptionEditor_LoadAsync(sender, e);
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
				int i, nNewOptionID = 9999;
				DataRow pNewRow;

				List<string> listIntColumns =
				[
					"a_index",
					"a_type",
					"a_weapon_type",
					"a_wear_type",
					"a_accessory_type"
				];

				List<string> listVarcharColumns =
				[
					"a_level",
					"a_prob"
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listVarcharColumns.Add("a_name_" + strNation.ToLower());

				if (pMain.pTables.OptionTable == null)
				{
					DataTable pOptionTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pOptionTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listVarcharColumns)
						pOptionTableStruct.Columns.Add(strColumnName, typeof(string));

					pNewRow = pOptionTableStruct.NewRow();

					pOptionTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_option ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewOptionID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewOptionID = pMain.AskForIndex(this.Text, "a_index")) == -1)	// I don't test it...
							return;
					}
				}
				else
				{
					nNewOptionID = Convert.ToInt32(pMain.pTables.OptionTable.Select().LastOrDefault()["a_index"]) + 1;

					pNewRow = pMain.pTables.OptionTable.NewRow();
				}

				List<object> listDefaultValue =
				[
					nNewOptionID,	// a_index
					0,	// a_type
					0,	// a_weapon_type
					0,	// a_wear_type
					0,	// a_accessory_type
					0,	// a_level
					0	// a_prob
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listDefaultValue.Add("New Option");

				i = 0;
				foreach (string strColumnName in listIntColumns.Concat(listVarcharColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempOptionRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"Option Editor > Option: {nNewOptionID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Option Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewOptionID, "New Option", true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Option. Because it's temporary.", "Option Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nOptionIDToCopy = Convert.ToInt32(pTempOptionRow["a_index"]);
				int nNewOptionID = Convert.ToInt32(pMain.pTables.OptionTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempOptionRow = pMain.pTables.OptionTable.NewRow();
				pTempOptionRow.ItemArray = (object[])pMain.pTables.OptionTable.Select("a_index=" + nOptionIDToCopy)[0].ItemArray.Clone();

				pTempOptionRow["a_index"] = nNewOptionID;

				foreach (string strNation in pMain.pSettings.NationSupported)
					pTempOptionRow["a_name_" + strNation.ToLower()] = pTempOptionRow["a_name_" + strNation.ToLower()] + " Copy";

				AddToList(nNewOptionID, pTempOptionRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, true);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nOptionID = Convert.ToInt32(pTempOptionRow["a_index"]);
			DataRow? pOptionRow = pMain.pTables.OptionTable?.Select("a_index=" + nOptionID).FirstOrDefault();

			if (pOptionRow != null)
			{
				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_option WHERE a_index={nOptionID};", out long _)))
				{
					string strError = $"Option Editor > Option: {nOptionID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Option Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pOptionRow != null)
						pMain.pTables.OptionTable.Rows.Remove(pOptionRow);
				}
				catch (Exception ex)
				{
					string strError = $"Option Editor > Option: {nOptionID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Option Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Option Deleted successfully!", "Option Editor", MessageBoxButtons.OK);

						MainList.SelectedIndex = nPrevObjectID;

						bUnsavedChanges = false;
					}
				}
			}
		}
		/****************************************/
		private void cbNationSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				bUserAction = false;

				tbName.Text = pTempOptionRow["a_name_" + cbNationSelector.SelectedItem.ToString().ToLower()].ToString();

				bUserAction = true;
			}
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempOptionRow["a_name_" + cbNationSelector.SelectedItem.ToString().ToLower()] = tbName.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbTypeSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempOptionRow["a_type"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void btnWeaponTypeFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(0)].ToArray(), Convert.ToInt64(btnWeaponTypeFlag.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnWeaponTypeFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();
				int i = 0;

				foreach (string strType in Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(0)])
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(strType + "\n");

					i++;
				}

				pToolTips[btnWeaponTypeFlag].SetToolTip(btnWeaponTypeFlag, strTooltip.ToString());

				pTempOptionRow["a_weapon_type"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void btnWearTypeFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(1)].ToArray(), Convert.ToInt64(btnWearTypeFlag.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnWearTypeFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();
				int i = 0;

				foreach (string strType in Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(1)])
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(strType + "\n");

					i++;
				}

				pToolTips[btnWearTypeFlag].SetToolTip(btnWearTypeFlag, strTooltip.ToString());

				pTempOptionRow["a_wear_type"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void btnAccessoryTypeFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(5)].ToArray(), Convert.ToInt64(btnAccessoryTypeFlag.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnAccessoryTypeFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();
				int i = 0;

				foreach (string strType in Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(5)])
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(strType + "\n");

					i++;
				}

				pToolTips[btnAccessoryTypeFlag].SetToolTip(btnAccessoryTypeFlag, strTooltip.ToString());

				pTempOptionRow["a_accessory_type"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void gridLevels_CellValueChanged(object? sender, DataGridViewCellEventArgs e) { if (bUserAction) bUnsavedChanges = true; }

		private void gridLevels_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Right && e.ColumnIndex == -1) // Header Column
				{
					ToolStripMenuItem addItem = new("Add New");
					addItem.Click += (_, _) =>
					{
						int nRow = gridLevels.Rows.Count;

						gridLevels.Rows.Insert(nRow);

						gridLevels.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

						gridLevels.Rows[nRow].Cells["level"].Value = 0;
						gridLevels.Rows[nRow].Cells["prob"].Value = 0;

						gridLevels.FirstDisplayedScrollingRowIndex = nRow;
						gridLevels.Rows[nRow].Selected = true;
					};

					ToolStripMenuItem deleteItem = new("Delete") { Enabled = e.RowIndex >= 0 };
					deleteItem.Click += (_, _) =>
					{
						if (e.RowIndex >= 0)
						{
							gridLevels.SuspendLayout();

							gridLevels.Rows.RemoveAt(e.RowIndex);

							int i = 1;
							foreach (DataGridViewRow row in gridLevels.Rows)
							{
								row.HeaderCell.Value = i.ToString();

								i++;
							}

							gridLevels.ResumeLayout();
						}
					};

					cmLevels = new ContextMenuStrip();
					cmLevels.Items.AddRange(addItem, deleteItem);
					cmLevels.Show(Cursor.Position);
				}
			}
		}
		/****************************************/
		private async void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nItemID = Convert.ToInt32(pTempOptionRow["a_index"]);
			List<string> listLevels = new();
			List<string> listProbs = new();
			StringBuilder strbuilderQuery = new();

			if (gridLevels.Rows.Count == 0)
			{
				listLevels.Add("0");
				listProbs.Add("0");
			}
			else
			{
				foreach (DataGridViewRow row in gridLevels.Rows)
				{
					listLevels.Add(row.Cells["level"].Value.ToString());
					listProbs.Add(row.Cells["prob"].Value.ToString());
				}
			}

			pTempOptionRow["a_level"] = string.Join(" ", listLevels);
			pTempOptionRow["a_prob"] = string.Join(" ", listProbs);

			// Check if Option exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pOptionRow = pMain.pTables.OptionTable?.Select("a_index=" + nItemID).FirstOrDefault();
			if (pOptionRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_option SET");

				foreach (DataColumn pCol in pTempOptionRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempOptionRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nItemID};");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempOptionRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempOptionRow[pCol].ToString())}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_option ({strColumnsNames}) VALUES ({strColumnsValues});");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.ToString(), out long _))
			{
				try
				{
					if (pOptionRow != null)  // Row exist in Global Table, update it.
					{
						pOptionRow.ItemArray = (object[])pTempOptionRow.ItemArray.Clone();
					}
					else // Row not exist in Global Table, insert it.
					{
						pOptionRow = pMain.pTables.OptionTable.NewRow();
						pOptionRow.ItemArray = (object[])pTempOptionRow.ItemArray.Clone();
						pMain.pTables.OptionTable.Rows.Add(pOptionRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Option Editor > Option: {nItemID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Option Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nItemID;
						pSelectedItem.Text = nItemID + " - " + tbName.Text.ToString();

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						bool bExported = await TryExportOptionClientLodsAsync(nItemID);
						string strMessage = bExported
							? "Changes applied successfully! Option.lod and strOption .lod have been exported to the client."
							: "Changes applied successfully, but one or more option .lod exports failed. Check the Toolbox log before testing in-game.";
						MessageBox.Show(strMessage, "Option Editor", MessageBoxButtons.OK, bExported ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Option Editor > Option: {nItemID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Option Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task<bool> TryExportOptionClientLodsAsync(int nOptionID)
		{
			bool bSuccess = true;
			using Exporter pExporter = new(pMain);

			try
			{
				await pExporter.ExportOptionsLodAsync(true);
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Option Editor > Option: {nOptionID} saved, but Option.lod export failed: {ex.Message}");
				bSuccess = false;
			}

			try
			{
				await pExporter.ExportOptionStringsLodAsync(true);
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Option Editor > Option: {nOptionID} saved, but strOption .lod export failed: {ex.Message}");
				bSuccess = false;
			}

			return bSuccess;
		}
	}
}
