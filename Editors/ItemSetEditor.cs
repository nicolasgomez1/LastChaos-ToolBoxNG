// NOTE: The order of wearing in UI doesn't matter. I compose the queries in correct order, don't worry :D

namespace LastChaos_ToolBoxNG
{
	public partial class ItemSetEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempSetRow;
		private string[] strItemWearingButtonNames =
		{
			// Standards
			"Helmet",
			"Shirt",
			"Weapon",
			"Pants",
			"Shield",
			"Gloves",
			"Boots",
			"Accesory1",
			"Accesory2",
			"Accesory3",
			"Pet",
			"Back"
		};

		public ItemSetEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.ItemSetTable.Select("a_set_idx=" + pTempSetRow["a_set_idx"]).FirstOrDefault() != null)    // the current selected Set, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Item Set Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else    // the current selected Set is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Set is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Item Set Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private async Task LoadItemSetDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_job",
				"a_enable",
				"a_item_idx",
				"a_option_count",
				"a_wear_count",
				"a_option_type",
				"a_option_idx",
				"a_option_level"
			];

			foreach (string strNation in pMain.pSettings.NationSupported)
				listQueryCompose.Add("a_set_name_" + strNation.ToLower());

			if (pMain.pTables.ItemSetTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.ItemSetTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_set_idx, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_set_item ORDER BY a_set_idx");
				});

				if (pMain.pTables.ItemSetTable == null)
					pMain.pTables.ItemSetTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_set_idx", ref pMain.pTables.ItemSetTable);
			}
		}

		private async Task LoadItemDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_name_" + pMain.pSettings.WorkLocale,
				"a_descr_" + pMain.pSettings.WorkLocale,
				"a_texture_id",
				"a_texture_row",
				"a_texture_col",
				"a_set_4"
			];

			if (pMain.pTables.ItemTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.ItemTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_item ORDER BY a_index;");
				});

				if (pMain.pTables.ItemTable == null)
					pMain.pTables.ItemTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.ItemTable);
			}
		}

		private async void ItemSetEditor_LoadAsync(object sender, EventArgs e)
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
			cbJobSelector.BeginUpdate();

			foreach (var ClassData in Defs.CharactersClassNJobsTypes)
				cbJobSelector.Items.Add(ClassData.Value[0].Substring(4));

			cbJobSelector.EndUpdate();
			/****************************************/
			for (int i = 0; i < Defs.MAX_SET_ITEM_OPTION; i++)
			{
				// (new ToolTip()).SetToolTip(((Button)Controls.Find("btnOption" + i, true)[0]), "Left Click to open Picker for current Type/Right Click to open the opposite Type.");
				// Rigid/Fixed version
				(new ToolTip()).SetToolTip(((Button)Controls.Find("btnOption" + i, true)[0]), "Left Click to open Skills Picker/Right Click to open Options.");
			}
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadItemSetDataAsync(),
				LoadItemDataAsync(),
				pMain.GenericLoadOptionDataAsync(),
				pMain.GenericLoadSkillDataAsync(),
				pMain.GenericLoadSkillLevelDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Item Sets, Items, Options, Skills & Skills Levels Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.ItemSetTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.ItemSetTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_set_idx"]), pRow["a_set_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Item Sets, Items, Options & Skills Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void ItemSetEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				// Do nothing
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Item Set Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nSetID, bool bLoadFrompItemSetTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			cbJobSelector.SelectedIndex = -1;
			/****************************************/
			if (bLoadFrompItemSetTable && pMain.pTables.ItemSetTable != null)
			{
				pTempSetRow = pMain.pTables.ItemSetTable.NewRow();
				pTempSetRow.ItemArray = (object[])pMain.pTables.ItemSetTable.Select("a_set_idx=" + nSetID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nSetID.ToString();
			/****************************************/
			if (pTempSetRow["a_enable"].ToString() == "1")
				cbEnable.Checked = true;
			else
				cbEnable.Checked = false;
			/****************************************/
			int nJobValue = Convert.ToInt32(pTempSetRow["a_job"]);
			if (nJobValue >= Defs.CharactersClassNJobsTypes.Count)
				pMain.Logger(LogTypes.Error, $"Item Set Editor > Set: {nSetID} Error: a_job out of range.");
			else
				cbJobSelector.SelectedIndex = nJobValue;
			/****************************************/
			tbName.Text = pTempSetRow["a_set_name_" + cbNationSelector.SelectedItem.ToString().ToLower()].ToString();
			/****************************************/
			int i = 0;
			Button btnObj;
			DataRow? pItemRow;
			string[] strSetParts = (pTempSetRow["a_item_idx"].ToString() ?? string.Empty).Split(' ');

			foreach (string strWearingPos in strItemWearingButtonNames)
			{
				int nItemID = Convert.ToInt32(strSetParts[i]);
				string strItemName = nItemID.ToString();
				btnObj = ((Button)Controls.Find("btn" + strWearingPos, true)[0]);

				if (nItemID > 0)
				{
					pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).FirstOrDefault();
					if (pItemRow != null)
					{
						strItemName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

						btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
					}
				}
				else
				{
					btnObj.Image = null;
				}

				btnObj.Text = strItemName;

				i++;
			}
			/****************************************/
			ComboBox cbObj;
			DataRow? pOptionRow, pSkillRow;
			DataRow[]? pSkillLevelRows;
			string[] strWearCount = (pTempSetRow["a_wear_count"].ToString() ?? string.Empty).Split(' ');
			string[] strOptionTypes = (pTempSetRow["a_option_type"].ToString() ?? string.Empty).Split(' '); // 0 Option, 1 Skill
			string[] strOptionIDs = (pTempSetRow["a_option_idx"].ToString() ?? string.Empty).Split(' ');
			string[] strOptionLevels = (pTempSetRow["a_option_level"].ToString() ?? string.Empty).Split(' ');

			for (i = 0; i < Defs.MAX_SET_ITEM_OPTION; i++)
			{
				((NumericUpDown)Controls.Find("nudWearCount" + i, true)[0]).Value = Convert.ToInt32(strWearCount[i]);

				string strType = "Option";
				string strOptionName = strOptionIDs[i];

				btnObj = ((Button)Controls.Find("btnOption" + i, true)[0]);
				btnObj.Image = null;

				cbObj = (ComboBox)Controls.Find("cbOptionLevel" + i, true)[0];
				cbObj.Items.Clear();
				cbObj.SelectedIndex = -1;
				cbObj.Enabled = false;

				if (strOptionTypes[i] == "0")   // Option
				{
					pOptionRow = pMain.pTables.OptionTable?.Select("a_type=" + strOptionIDs[i]).FirstOrDefault();
					if (pOptionRow != null)
					{
						strOptionName += " - " + pOptionRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

						string[] strProb = pOptionRow["a_prob"].ToString().Split(' ');

						cbObj.Enabled = true;

						cbObj.BeginUpdate();

						int j = 0;
						foreach (string strLevel in pOptionRow["a_level"].ToString().Split(' '))
						{
							if (j < strProb.Length && strProb[j] != null)
							{
								cbObj.Items.Add($"{(j + 1)} - Value: {strLevel} Prob: {strProb[j]}");

								if ((j + 1) == Convert.ToInt32(strOptionLevels[i]))
									cbObj.SelectedIndex = j;
							}
							else
							{
								pMain.Logger(LogTypes.Error, $"Item Set Editor > Option: {strOptionIDs[i]}: Error: a_level & a_prob not match.");
							}

							j++;
						}

						cbObj.EndUpdate();

						if (cbObj.SelectedIndex == -1)
							cbObj.SelectedIndex = 0;
					}
					else
					{
						pMain.Logger(LogTypes.Error, $"Item Set Editor > Set: {nSetID} Error: a_option_idx N°: {i} not exist in t_option.");
					}
				}
				else if (strOptionTypes[i] == "1")  // Skill
				{
					strType = "Skill";
					pSkillRow = pMain.pTables.SkillTable?.Select("a_index=" + strOptionIDs[i]).FirstOrDefault();

					if (pSkillRow != null)
					{
						strOptionName += " - " + pSkillRow["a_name_" + pMain.pSettings.WorkLocale].ToString();
						btnObj.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSkillRow["a_client_icon_texid"].ToString(), Convert.ToInt32(pSkillRow["a_client_icon_row"]), Convert.ToInt32(pSkillRow["a_client_icon_col"])), new Size(24, 24));

						cbObj.Enabled = true;

						cbObj.BeginUpdate();

						pSkillLevelRows = pMain.pTables.SkillLevelTable?.AsEnumerable().Where(row => row["a_index"].ToString() == strOptionIDs[i]).ToArray();

						foreach (DataRow pRowSkillLevel in pSkillLevelRows)
						{
							int nSkillLevel = Convert.ToInt32(pRowSkillLevel["a_level"]);

							int nLastItemAdded = cbObj.Items.Add($"Level: {nSkillLevel} - Power: {pRowSkillLevel["a_dummypower"]}");

							if (nSkillLevel == Convert.ToInt32(strOptionLevels[i]))
								cbObj.SelectedIndex = nLastItemAdded;
						}

						cbObj.EndUpdate();

						if (cbObj.SelectedIndex == -1)
							cbObj.SelectedIndex = 0;
					}
					else
					{
						pMain.Logger(LogTypes.Error, $"Item Set Editor > Set: {nSetID} Error: a_option_idx N° {i} not exist in t_skill.");
					}
				}

				((Label)Controls.Find("lbOption" + i, true)[0]).Text = strType;

				btnObj.Text = strOptionName;
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
			void Reload()
			{
				MainList.Enabled = false;
				btnReload.Enabled = false;

				nSearchPosition = 0;

				pMain.pTables.ItemTable?.Dispose();
				pMain.pTables.ItemTable = null;

				pMain.pTables.OptionTable?.Dispose();
				pMain.pTables.OptionTable = null;

				pMain.pTables.SkillTable?.Dispose();
				pMain.pTables.SkillTable = null;

				pMain.pTables.SkillLevelTable?.Dispose();
				pMain.pTables.SkillLevelTable = null;

				pMain.pTables.ItemSetTable?.Dispose();
				pMain.pTables.ItemSetTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				ItemSetEditor_LoadAsync(sender, e);
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
				int i, nNewSetID = 9999;
				DataRow pNewRow;

				List<string> listIntColumns =
				[
					"a_set_idx",
					"a_job",
					"a_enable",
					"a_option_count"
				];

				List<string> listVarcharColumns =
				[
					"a_item_idx",
					"a_wear_count",
					"a_option_type",
					"a_option_idx",
					"a_option_level"
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listVarcharColumns.Add("a_set_name_" + strNation.ToLower());

				if (pMain.pTables.ItemSetTable == null)
				{
					DataTable pItemSetTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pItemSetTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listVarcharColumns)
						pItemSetTableStruct.Columns.Add(strColumnName, typeof(string));

					pNewRow = pItemSetTableStruct.NewRow();

					pItemSetTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_set_idx FROM {pMain.pSettings.DBData}.t_set_item ORDER BY a_set_idx DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewSetID = Convert.ToInt32(QueryReturn.Rows[0]["a_set_idx"]) + 1;
					}
					else
					{
						if ((nNewSetID = pMain.AskForIndex(this.Text, "a_set_idx")) == -1)  // I don't test it...
							return;
					}
				}
				else
				{
					nNewSetID = Convert.ToInt32(pMain.pTables.ItemSetTable.Select().LastOrDefault()["a_set_idx"]) + 1;

					pNewRow = pMain.pTables.ItemSetTable.NewRow();
				}

				List<object> listDefaultValue =
				[
					nNewSetID,	// a_set_idx
					0,	// a_job
					0,	// a_enable
					0,	// a_option_count
					"-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1",	// a_item_idx	// Hardcode!
					"0 0 0 0 0 0 0 0 0 0 0 0",				// a_wear_count	// Hardcode!
					"-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1",	// a_option_type	// Hardcode!
					"-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1",	// a_option_idx	// Hardcode!
					"0 0 0 0 0 0 0 0 0 0 0 0"				// a_option_level	// Hardcode!
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listDefaultValue.Add("New Set");

				i = 0;
				foreach (string strColumnName in listIntColumns.Concat(listVarcharColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempSetRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"Item Set Editor > Set: {nNewSetID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Item Set Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewSetID, "New Set", true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Set. Because it's temporary.", "Item Set Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nSetIDToCopy = Convert.ToInt32(pTempSetRow["a_set_idx"]);
				int nNewSetID = Convert.ToInt32(pMain.pTables.ItemSetTable.Select().LastOrDefault()["a_set_idx"]) + 1;

				pTempSetRow = pMain.pTables.ItemSetTable.NewRow();
				pTempSetRow.ItemArray = (object[])pMain.pTables.ItemSetTable.Select("a_set_idx=" + nSetIDToCopy)[0].ItemArray.Clone();

				pTempSetRow["a_set_idx"] = nNewSetID;
				pTempSetRow["a_item_idx"] = "-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1";

				foreach (string strNation in pMain.pSettings.NationSupported)
					pTempSetRow["a_set_name_" + strNation.ToLower()] = pTempSetRow["a_set_name_" + strNation.ToLower()] + " Copy";

				AddToList(nNewSetID, pTempSetRow["a_set_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, true);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nSetID = Convert.ToInt32(pTempSetRow["a_set_idx"]);
			DataRow? pSetRow = pMain.pTables.ItemSetTable?.Select("a_set_idx=" + nSetID).FirstOrDefault();

			if (pSetRow != null)
			{
				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_set_item WHERE a_set_idx={nSetID};", out long _)))
				{
					string strError = $"Item Set Editor > Set: {nSetID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Item Set Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pSetRow != null)
						pMain.pTables.ItemSetTable.Rows.Remove(pSetRow);
				}
				catch (Exception ex)
				{
					string strError = $"Item Set Editor > Set: {nSetID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Item Set Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Set Deleted successfully!", "Item Set Editor", MessageBoxButtons.OK);

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
				string strEnable = "0";

				if (cbEnable.Checked)
					strEnable = "1";

				pTempSetRow["a_enable"] = strEnable;

				bUnsavedChanges = true;
			}
		}

		private void cbJobSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbJobSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempSetRow["a_job"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbNationSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				bUserAction = false;

				tbName.Text = pTempSetRow["a_set_name_" + cbNationSelector.SelectedItem.ToString().ToLower()].ToString();

				bUserAction = true;
			}
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempSetRow["a_set_name_" + cbNationSelector.SelectedItem.ToString().ToLower()] = tbName.Text;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void SetWearCount(int nPos)
		{
			if (bUserAction)
			{
				string[] strWearCount = (pTempSetRow["a_wear_count"].ToString() ?? string.Empty).Split(' ');

				strWearCount[nPos] = ((NumericUpDown)Controls.Find("nudWearCount" + nPos, true)[0]).Value.ToString();

				pTempSetRow["a_wear_count"] = string.Join(" ", strWearCount);

				bUnsavedChanges = true;
			}
		}

		private void nudWearCount0_ValueChanged(object sender, EventArgs e) { SetWearCount(0); }
		private void nudWearCount1_ValueChanged(object sender, EventArgs e) { SetWearCount(1); }
		private void nudWearCount2_ValueChanged(object sender, EventArgs e) { SetWearCount(2); }
		private void nudWearCount3_ValueChanged(object sender, EventArgs e) { SetWearCount(3); }
		private void nudWearCount4_ValueChanged(object sender, EventArgs e) { SetWearCount(4); }
		private void nudWearCount5_ValueChanged(object sender, EventArgs e) { SetWearCount(5); }
		private void nudWearCount6_ValueChanged(object sender, EventArgs e) { SetWearCount(6); }
		private void nudWearCount7_ValueChanged(object sender, EventArgs e) { SetWearCount(7); }
		private void nudWearCount8_ValueChanged(object sender, EventArgs e) { SetWearCount(8); }
		private void nudWearCount9_ValueChanged(object sender, EventArgs e) { SetWearCount(9); }
		private void nudWearCount10_ValueChanged(object sender, EventArgs e) { SetWearCount(10); }
		/****************************************/
		private void SetWearPart(int nPos)
		{
			if (bUserAction)
			{
				string[] strSetParts = (pTempSetRow["a_item_idx"].ToString() ?? string.Empty).Split(' ');
				Button btnObj = (Button)Controls.Find("btn" + strItemWearingButtonNames[nPos], true)[0];

				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(strSetParts[nPos]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);

				if (nItemID > 0)
				{
					DataRow? pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).FirstOrDefault();
					if (pItemRow != null)
					{
						int nItemSetID = Convert.ToInt32(pItemRow["a_set_4"]);

						if (nItemSetID > 0 && nItemSetID != Convert.ToInt32(pTempSetRow["a_set_idx"]))
						{
							DataRow? pSetRow = pMain.pTables.ItemSetTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_set_idx"]) == nItemSetID).FirstOrDefault();
							string strSetName = $"NOT FOUND (ID: {nItemSetID})";
							if (pSetRow != null)
								strSetName = $"{nItemSetID} - {pSetRow["a_set_name_" + pMain.pSettings.WorkLocale].ToString()}";

							MessageBox.Show($"This Item is already part of Set {strSetName}. An item cannot belong to multiple sets.", "Item Set Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}
					}
				}

				string strItemName = nItemID.ToString();

				if (nItemID > 0)
				{
					strItemName += $" - {pItemSelector.ReturnValues[1]}";

					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
				}
				else
				{
					btnObj.Image = null;
				}

				btnObj.Text = strItemName;

				strSetParts[nPos] = nItemID.ToString();

				pTempSetRow["a_item_idx"] = string.Join(" ", strSetParts);

				bUnsavedChanges = true;
			}
		}

		private void btnHelmet_Click(object sender, EventArgs e) { SetWearPart(0); }
		private void btnShirt_Click(object sender, EventArgs e) { SetWearPart(1); }
		private void btnPants_Click(object sender, EventArgs e) { SetWearPart(3); }
		private void btnGloves_Click(object sender, EventArgs e) { SetWearPart(5); }
		private void btnBoots_Click(object sender, EventArgs e) { SetWearPart(6); }
		private void btnBack_Click(object sender, EventArgs e) { SetWearPart(11); }
		private void btnWeapon_Click(object sender, EventArgs e) { SetWearPart(2); }
		private void btnShield_Click(object sender, EventArgs e) { SetWearPart(4); }
		private void btnPet_Click(object sender, EventArgs e) { SetWearPart(10); }
		private void btnAccesory1_Click(object sender, EventArgs e) { SetWearPart(7); }
		private void btnAccesory2_Click(object sender, EventArgs e) { SetWearPart(8); }
		private void btnAccesory3_Click(object sender, EventArgs e) { SetWearPart(9); }
		/****************************************/
		private void SetOption(int nPos, MouseEventArgs e)
		{
			if (bUserAction)
			{
				string[] strOptionTypes = (pTempSetRow["a_option_type"].ToString() ?? string.Empty).Split(' '); // 0 Option, 1 Skill
				string[] strOptionIDs = (pTempSetRow["a_option_idx"].ToString() ?? string.Empty).Split(' ');
				string[] strOptionLevels = (pTempSetRow["a_option_level"].ToString() ?? string.Empty).Split(' ');
				Button btnObj = (Button)Controls.Find("btnOption" + nPos, true)[0];
				ComboBox cbObj = (ComboBox)Controls.Find("cbOptionLevel" + nPos, true)[0];

				void ApplyUpdate(string strOptionType, string strOptionID, string strOptionLevel)
				{
					strOptionTypes[nPos] = strOptionType;
					pTempSetRow["a_option_type"] = string.Join(" ", strOptionTypes);

					strOptionIDs[nPos] = strOptionID;
					pTempSetRow["a_option_idx"] = string.Join(" ", strOptionIDs);

					strOptionLevels[nPos] = strOptionLevel;
					pTempSetRow["a_option_level"] = string.Join(" ", strOptionLevels);

					string strType = "Skill";

					if (strOptionType == "0" || strOptionID == "-1")
						strType = "Option";

					((Label)Controls.Find("lbOption" + nPos, true)[0]).Text = strType;

					bUnsavedChanges = true;
				}

				void OpenOptionPicker()
				{
					OptionPicker pOptionSelector = new(pMain, this, new int[] { Convert.ToInt32(strOptionIDs[nPos]), Convert.ToInt32(strOptionLevels[nPos]) });
					if (pOptionSelector.ShowDialog() != DialogResult.OK)
						return;

					int nOptionType = Convert.ToInt32(pOptionSelector.ReturnValues[0]);
					int nOptionLevel = Convert.ToInt32(pOptionSelector.ReturnValues[1]);
					string strOptionName = nOptionType.ToString();

					btnObj.Image = null;

					cbObj.Items.Clear();
					cbObj.SelectedIndex = -1;
					cbObj.Enabled = false;

					if (nOptionType != -1)
					{
						strOptionName += $" - {pOptionSelector.ReturnValues[2]}";

						cbObj = (ComboBox)Controls.Find("cbOptionLevel" + nPos, true)[0];
						cbObj.Enabled = true;

						cbObj.BeginUpdate();

						int j = 0;
						foreach (string strLevel in pOptionSelector.ReturnValues[3].ToString().Split(','))
						{
							cbObj.Items.Add(strLevel);

							if ((j + 1) == Convert.ToInt32(pOptionSelector.ReturnValues[1]))
								cbObj.SelectedIndex = j;

							j++;
						}

						cbObj.EndUpdate();
					}

					btnObj.Text = strOptionName;

					ApplyUpdate("0", nOptionType.ToString(), nOptionLevel.ToString());
				}

				void OpenSkillPicker()
				{
					SkillPicker pSkillSelector = new(pMain, this, [Convert.ToInt32(strOptionIDs[nPos]), Convert.ToInt32(strOptionLevels[nPos])]);
					if (pSkillSelector.ShowDialog() != DialogResult.OK)
						return;

					int nSkillID = Convert.ToInt32(pSkillSelector.ReturnValues[0]);
					int nSkillLevel = Convert.ToInt32(pSkillSelector.ReturnValues[1]);
					string strSkillName = nSkillID.ToString();

					btnObj.Image = null;

					cbObj.Items.Clear();
					cbObj.SelectedIndex = -1;
					cbObj.Enabled = false;

					if (nSkillID != -1)
					{
						strSkillName += $" - {pSkillSelector.ReturnValues[2]}";
						btnObj.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSkillSelector.ReturnValues[4].ToString(), Convert.ToInt32(pSkillSelector.ReturnValues[5]), Convert.ToInt32(pSkillSelector.ReturnValues[6])), new Size(24, 24));

						cbObj = (ComboBox)Controls.Find("cbOptionLevel" + nPos, true)[0];
						cbObj.Enabled = true;

						cbObj.BeginUpdate();

						int j = 0;
						foreach (string strLevel in pSkillSelector.ReturnValues[7].ToString().Split(','))
						{
							cbObj.Items.Add(strLevel);

							if ((j + 1) == nSkillLevel)
								cbObj.SelectedIndex = j;

							j++;
						}

						cbObj.EndUpdate();
					}

					btnObj.Text = strSkillName;

					ApplyUpdate("1", nSkillID.ToString(), nSkillLevel.ToString());
				}

				/*if (e.Button == MouseButtons.Left)  // Open current Type
				{
					if (strOptionTypes[nPos] == "0")
						OpenOptionPicker();
					else if (strOptionTypes[nPos] == "1")
						OpenSkillPicker();
					else
						OpenOptionPicker();
				}
				else    // Open opposite Type
				{
					if (strOptionTypes[nPos] == "0")
						OpenSkillPicker();
					else if (strOptionTypes[nPos] == "1")
						OpenOptionPicker();
					else
						OpenOptionPicker();
				}*/
				// Rigid/Fixed version
				if (e.Button == MouseButtons.Left)
					OpenSkillPicker();
				else
					OpenOptionPicker();
			}
		}

		private void btnOption0_MouseDown(object sender, MouseEventArgs e) { SetOption(0, e); }
		private void btnOption1_MouseDown(object sender, MouseEventArgs e) { SetOption(1, e); }
		private void btnOption2_MouseDown(object sender, MouseEventArgs e) { SetOption(2, e); }
		private void btnOption3_MouseDown(object sender, MouseEventArgs e) { SetOption(3, e); }
		private void btnOption4_MouseDown(object sender, MouseEventArgs e) { SetOption(4, e); }
		private void btnOption5_MouseDown(object sender, MouseEventArgs e) { SetOption(5, e); }
		private void btnOption6_MouseDown(object sender, MouseEventArgs e) { SetOption(6, e); }
		private void btnOption7_MouseDown(object sender, MouseEventArgs e) { SetOption(7, e); }
		private void btnOption8_MouseDown(object sender, MouseEventArgs e) { SetOption(8, e); }
		private void btnOption9_MouseDown(object sender, MouseEventArgs e) { SetOption(9, e); }
		private void btnOption10_MouseDown(object sender, MouseEventArgs e) { SetOption(10, e); }
		/****************************************/
		private void SetOptionLevel(int nPos)
		{
			if (bUserAction)
			{
				string[] strOptionLevels = (pTempSetRow["a_option_level"].ToString() ?? string.Empty).Split(' ');
				ComboBox cbObj = (ComboBox)Controls.Find("cbOptionLevel" + nPos, true)[0];
				int nLevel = cbObj.SelectedIndex;

				if (nLevel != -1)
				{
					strOptionLevels[nPos] = (nLevel + 1).ToString();

					pTempSetRow["a_option_level"] = string.Join(" ", strOptionLevels);

					bUnsavedChanges = true;
				}
			}
		}

		private void cbOptionLevel0_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(0); }
		private void cbOptionLevel1_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(1); }
		private void cbOptionLevel2_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(2); }
		private void cbOptionLevel3_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(3); }
		private void cbOptionLevel4_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(4); }
		private void cbOptionLevel5_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(5); }
		private void cbOptionLevel6_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(6); }
		private void cbOptionLevel7_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(7); }
		private void cbOptionLevel8_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(8); }
		private void cbOptionLevel9_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(9); }
		private void cbOptionLevel10_SelectedIndexChanged(object sender, EventArgs e) { SetOptionLevel(10); }
		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nSetID = Convert.ToInt32(pTempSetRow["a_set_idx"]);
			string[] strSetParts = (pTempSetRow["a_item_idx"].ToString() ?? string.Empty).Split(' ');
			StringBuilder strbuilderQuery = new();

			// Check if Set exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pSetRow = pMain.pTables.ItemSetTable?.Select("a_set_idx=" + nSetID).FirstOrDefault();
			if (pSetRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_set_item SET");

				foreach (DataColumn pCol in pTempSetRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempSetRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_set_idx={nSetID};");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempSetRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempSetRow[pCol].ToString())}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_set_item ({strColumnsNames}) VALUES ({strColumnsValues});");
			}

			strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item SET a_set_4=0 WHERE a_set_4={nSetID};");

			foreach (string strItemID in strSetParts)
			{
				if (strItemID != "-1" && strItemID != "0")
					strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item SET a_set_4={nSetID} WHERE a_index={strItemID};");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.ToString(), out long _))
			{
				try
				{
					foreach (string strItemID in strSetParts)
					{
						if (strItemID != "-1" && strItemID != "0")
						{
							DataRow[] pItemTableRows = pMain.pTables.ItemTable.Select("a_index=" + strItemID);
							foreach (DataRow pRow in pItemTableRows)
								pRow["a_set_4"] = nSetID.ToString();
						}
					}

					pMain.pTables.ItemTable.AcceptChanges();

					if (pSetRow != null)  // Row exist in Global Table, update it.
					{
						pSetRow.ItemArray = (object[])pTempSetRow.ItemArray.Clone();
					}
					else // Row not exist in Global Table, insert it.
					{
						pSetRow = pMain.pTables.ItemSetTable.NewRow();
						pSetRow.ItemArray = (object[])pTempSetRow.ItemArray.Clone();
						pMain.pTables.ItemSetTable.Rows.Add(pSetRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Item Set Editor > Set: {nSetID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Item Set Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nSetID;
						pSelectedItem.Text = nSetID + " - " + tbName.Text.ToString();

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "Item Set Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Item Set Editor > Set: {nSetID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Item Set Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
