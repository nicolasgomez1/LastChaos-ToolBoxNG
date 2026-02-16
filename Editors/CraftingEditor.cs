//#define USE_a_job_AND_a_item_type_TABLE	// NOTE: I remove it cuz is not used.

namespace LastChaos_ToolBoxNG
{
	public partial class CraftingEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempCraftingRow;
		private ContextMenuStrip? cmLevels;

		public CraftingEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridMaterials.TopLeftHeaderCell.Value = "N°";
		}
		
		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.CraftingTable?.Select("a_index=" + pTempCraftingRow["a_index"]).FirstOrDefault() != null)	// the current selected crafting, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Crafting Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else	// the current selected crafting is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Crafting is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Crafting Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private async Task LoadCraftingDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string> {
#if USE_a_job_AND_a_item_type_TABLE
				"a_job",
#endif
				"a_enable",
				"a_seal_type",
#if USE_a_job_AND_a_item_type_TABLE
				"a_item_type",
#endif
				"a_item_idx",
				"a_make_exp",
				"a_need_exp",
				"a_nas",
				"a_stuff",
				"a_stuff_cnt"
			};

			if (pMain.pTables.CraftingTable == null) 
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.CraftingTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_factory_item ORDER BY a_index;");
				});

				if (pMain.pTables.CraftingTable == null)
					pMain.pTables.CraftingTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.CraftingTable);
			}
		}

		private async Task LoadItemDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string> {
				"a_name_" + pMain.pSettings.WorkLocale,
				"a_descr_" + pMain.pSettings.WorkLocale,
				"a_texture_id",
				"a_texture_row",
				"a_texture_col"
#if USE_a_job_AND_a_item_type_TABLE
				, "a_subtype_idx"
#endif
			};

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

		private async void CraftingEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
#if USE_a_job_AND_a_item_type_TABLE
			label5.Visible = true;
			cbJobSelector.Visible = true;

			cbJobSelector.BeginUpdate();

			foreach (var ClassData in Defs.CharactersClassNJobsTypes)
				cbJobSelector.Items.Add(ClassData.Value[0].Substring(4));

			cbJobSelector.Items.Add("All");
			cbJobSelector.EndUpdate();

			label3.Visible = true;

			cbItemResultType.Visible = true;
			cbItemResultType.SelectedIndex = -1;

			cbItemResultType.BeginUpdate();

			for (int i = 0; i < 25; i++)
				cbItemResultType.Items.Add(i);

			cbItemResultType.EndUpdate();
#endif
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadCraftingDataAsync(),
				LoadItemDataAsync(),
				pMain.GenericLoadSkillDataAsync(),
				pMain.GenericLoadSkillLevelDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Craftings, Items, Skills & Skills Levels Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.CraftingTable != null && pMain.pTables.ItemTable != null)
			{
				MainList.BeginUpdate();

				int nResultItemID;
				string strResultItemName = "NOT FOUND";
				
				foreach (DataRow pRow in pMain.pTables.CraftingTable.Rows)
				{
					nResultItemID = Convert.ToInt32(pRow["a_item_idx"]);

					DataRow? pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nResultItemID).FirstOrDefault();
					if (pItemRow != null)
						strResultItemName = pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty;
					else
						pMain.Logger(LogTypes.Error, $"Crafting Editor > Craft: {pRow["a_index"]} Error: a_item_idx: {nResultItemID} not exist in t_item.");

					AddToList(Convert.ToInt32(pRow["a_index"]), strResultItemName, false);
				}

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Craftings, Items, Skills & Skill Levels Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void CraftingEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				if (cmLevels != null)
				{
					cmLevels.Dispose();
					cmLevels = null;
				}
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Crafting Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nCraftID, bool bLoadFrompCraftingTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			cbJobSelector.SelectedIndex = -1;
			btnSealType.Image = null;
			cbItemResultType.SelectedIndex = -1;
			btnItemResult.Image = null;
			gridMaterials.Rows.Clear();
			/****************************************/
			if (bLoadFrompCraftingTable && pMain.pTables.CraftingTable != null)
			{
				pTempCraftingRow = pMain.pTables.CraftingTable.NewRow();
				pTempCraftingRow.ItemArray = (object[])pMain.pTables.CraftingTable.Select("a_index=" + nCraftID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nCraftID.ToString();
			/****************************************/
			if (pTempCraftingRow["a_enable"].ToString() == "1")
				cbEnable.Checked = true;
			else
				cbEnable.Checked = false;
			/****************************************/
#if USE_a_job_AND_a_item_type_TABLE
			int nJobValue = Convert.ToInt32(pTempCraftingRow["a_job"]);
			if (nJobValue >= Defs.CharactersClassNJobsTypes.Count)
				pMain.Logger(LogTypes.Error, $"Crafting Editor > Craft: {nCraftID} Error: a_job out of range.");
			else
				cbJobSelector.SelectedIndex = nJobValue;
#endif
			/****************************************/
			int nResultItemID = Convert.ToInt32(pTempCraftingRow["a_item_idx"]);
			string strResultItemName = nResultItemID.ToString();
			DataRow? pItemRow;

			if (nResultItemID > 0)
			{
				pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nResultItemID).FirstOrDefault();
				if (pItemRow != null)
				{
					strResultItemName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

					btnItemResult.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
#if USE_a_job_AND_a_item_type_TABLE
					cbItemResultType.SelectedIndex = Convert.ToInt32(pItemRow["a_subtype_idx"]);
#endif
				}
			}

			tbResultItemName.Text = strResultItemName;

			btnItemResult.Text = strResultItemName;

			tbResultEXP.Text = pTempCraftingRow["a_make_exp"].ToString();
			/****************************************/
			int nRequiredSealID = Convert.ToInt32(pTempCraftingRow["a_seal_type"]);
			string strRequiredSealName = nRequiredSealID.ToString();

			if (nRequiredSealID > 0)
			{
				DataRow? pSkillRow = pMain.pTables.SkillTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nRequiredSealID).FirstOrDefault();
				if (pSkillRow != null)
				{
					btnSealType.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSkillRow["a_client_icon_texid"].ToString(), Convert.ToInt32(pSkillRow["a_client_icon_row"]), Convert.ToInt32(pSkillRow["a_client_icon_col"])), new Size(24, 24));

					strRequiredSealName += " - " + pSkillRow["a_name_" + pMain.pSettings.WorkLocale];
				}
			}
			/****************************************/
			btnSealType.Text = strRequiredSealName;
			/****************************************/
			tbNeededExp.Text = pTempCraftingRow["a_need_exp"].ToString();
			/****************************************/
			tbNeededGold.Text = pTempCraftingRow["a_nas"].ToString();
			/****************************************/
			// NOTE: do that in t_factory_item:
			// UPDATE t_factory_item SET a_stuff = LTRIM(a_stuff) WHERE LEFT(a_stuff, 1) = ' ';
			// UPDATE t_factory_item SET a_stuff_cnt = LTRIM(a_stuff_cnt) WHERE LEFT(a_stuff_cnt, 1) = ' ';
			// I can add a trim, but i don't want :D
			int nItemNeededID;
			string strItemName;
			string[] strItemsIDS = (pTempCraftingRow["a_stuff"].ToString() ?? string.Empty).Split(' ');
			string[] strItemsAmounts = (pTempCraftingRow["a_stuff_cnt"].ToString() ?? string.Empty).Split(' ');

			gridMaterials.SuspendLayout();

			for (int i = 0; i < Defs.MAX_FACTORY_ITEM_STUFF; i++)
			{
				gridMaterials.Rows.Insert(i);

				gridMaterials.Rows[i].HeaderCell.Value = (i + 1).ToString();

				nItemNeededID = Convert.ToInt32(strItemsIDS[i]);
				strItemName = nItemNeededID.ToString();

				if (nItemNeededID > 0)
				{
					pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemNeededID).FirstOrDefault();
					if (pItemRow != null)
					{
						strItemName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale];

						gridMaterials.Rows[i].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
					}
				}

				gridMaterials.Rows[i].Cells["item"].Value = strItemName;
				gridMaterials.Rows[i].Cells["item"].Tag = nItemNeededID;
				gridMaterials.Rows[i].Cells["amount"].Value = strItemsAmounts[i];

				if (nItemNeededID == Defs.NAS_ITEM_DB_INDEX)
				{
					gridMaterials.Rows[i].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(strItemsAmounts[i]));
					gridMaterials.Rows[i].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
				}
			}

			gridMaterials.ResumeLayout();
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

				pMain.pTables.CraftingTable?.Dispose();
				pMain.pTables.CraftingTable = null;

				pMain.pTables.ItemTable?.Dispose();
				pMain.pTables.ItemTable = null;

				pMain.pTables.SkillTable?.Dispose();
				pMain.pTables.SkillTable = null;

				pMain.pTables.SkillLevelTable?.Dispose();
				pMain.pTables.SkillLevelTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				CraftingEditor_LoadAsync(sender, e);
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
				int i, nNewCraftID = 9999;
				DataRow? pNewRow;
				int[] nRequiredItemID = new int[Defs.MAX_FACTORY_ITEM_STUFF], nRequiredItemCount = new int[Defs.MAX_FACTORY_ITEM_STUFF];

				List<string> listIntColumns = new List<string>	// Here add all int columns.
				{
					"a_index",
#if USE_a_job_AND_a_item_type_TABLE
					"a_job",
#endif
					"a_enable",
					"a_seal_type",
#if USE_a_job_AND_a_item_type_TABLE
					"a_item_type",
#endif
					"a_item_idx"
				};

				List<string> listBigIntColumns = new List<string>   // Here add all bigint columns.
				{
					"a_make_exp",
					"a_need_exp",
					"a_nas"
				};

				List<string> listVarcharColumns = new List<string>	// Here add all varchar columns.
				{
					"a_stuff",
					"a_stuff_cnt"
				};

				if (pMain.pTables.CraftingTable == null)   // If is null, create new DataTable and set schema (column name & datatype).
				{
					DataTable pCraftingTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pCraftingTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listBigIntColumns)
						pCraftingTableStruct.Columns.Add(strColumnName, typeof(long));

					foreach (string strColumnName in listVarcharColumns)
						pCraftingTableStruct.Columns.Add(strColumnName, typeof(string));

					pNewRow = pCraftingTableStruct.NewRow();

					pCraftingTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_factory_item ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewCraftID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewCraftID = pMain.AskForIndex(this.Text, "a_index")) == -1)	// I don't test it...
							return;
					}

					QueryReturn = null;
				}
				else
				{
					nNewCraftID = Convert.ToInt32(pMain.pTables.CraftingTable?.Select()?.LastOrDefault()?["a_index"]) + 1;

					pNewRow = pMain.pTables.CraftingTable?.NewRow();
				}

				List<object> listDefaultValue = new List<object>
				{
					nNewCraftID,	// a_index
#if USE_a_job_AND_a_item_type_TABLE
					0,	// a_job
#endif
					0,	// a_enable
					657,	// a_seal_type
#if USE_a_job_AND_a_item_type_TABLE
					0,	// a_item_type
#endif
					43,	// a_item_idx
					0,	// a_make_exp
					0,	// a_need_exp
					0	// a_nas
				};

				for (i = 0; i < Defs.MAX_FACTORY_ITEM_STUFF; i++)
				{
					nRequiredItemID[i] =  -1;
					nRequiredItemCount[i] = 0;
				}

				listDefaultValue.AddRange(
					string.Join(" ", nRequiredItemID),	// a_stuff
					string.Join(" ", nRequiredItemCount)	// a_stuff_cnt
				);

				i = 0;
				foreach (string strColumnName in listIntColumns.Concat(listBigIntColumns).Concat(listVarcharColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempCraftingRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"Crafting Editor > Craft: {nNewCraftID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Crafting Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewCraftID, pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nNewCraftID)?.FirstOrDefault()?["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Craft. Because it's temporary.", "Crafting Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nCraftIDToCopy = Convert.ToInt32(pTempCraftingRow["a_index"]);
				int nNewCraftID = Convert.ToInt32(pMain.pTables.CraftingTable?.Select().LastOrDefault()?["a_index"]) + 1;

				pTempCraftingRow = pMain.pTables.CraftingTable.NewRow();
				pTempCraftingRow.ItemArray = (object[])pMain.pTables.CraftingTable.Select("a_index=" + nCraftIDToCopy)[0].ItemArray.Clone();

				pTempCraftingRow["a_index"] = nNewCraftID;

				AddToList(nNewCraftID, pMain.pTables.ItemTable?.AsEnumerable()?.Where(row => Convert.ToInt32(row["a_index"]) == Convert.ToInt32(pTempCraftingRow["a_item_idx"])).FirstOrDefault()?["a_name_" + pMain.pSettings.WorkLocale].ToString() + " Copy", true);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nCraftID = Convert.ToInt32(pTempCraftingRow["a_index"]);
			DataRow? pCraftingRow = pMain.pTables.CraftingTable?.Select("a_index=" + nCraftID).FirstOrDefault();

			if (pCraftingRow != null)
			{
				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_factory_item WHERE a_index={nCraftID};", out long _)))
				{
					string strError = $"Crafting Editor > Craft: {nCraftID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Crafting Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pCraftingRow != null)
						pMain.pTables.CraftingTable?.Rows.Remove(pCraftingRow);
				}
				catch (Exception ex)
				{
					string strError = $"Crafting Editor > Craft: {nCraftID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Crafting Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Crafting Deleted successfully!", "Crafting Editor", MessageBoxButtons.OK);

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

				pTempCraftingRow["a_enable"] = strEnable;

				bUnsavedChanges = true;
			}
		}

		private void btnSealType_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				SkillPicker pSkillSelector = new(pMain, this, new object[] { pTempCraftingRow["a_seal_type"], 1 }, false);
				if (pSkillSelector.ShowDialog() != DialogResult.OK)
					return;

				int nSkillID = Convert.ToInt32(pSkillSelector.ReturnValues[0]);
				string strSkillName = pSkillSelector.ReturnValues[2].ToString() ?? string.Empty;

				btnSealType.Image = null;

				if (nSkillID > 0)
				{
					btnSealType.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSkillSelector.ReturnValues[4].ToString(), Convert.ToInt32(pSkillSelector.ReturnValues[5]), Convert.ToInt32(pSkillSelector.ReturnValues[6])), new Size(24, 24));

					strSkillName = nSkillID + " - " + strSkillName;
				}

				btnSealType.Text = strSkillName;

				pTempCraftingRow["a_seal_type"] = nSkillID;

				bUnsavedChanges = true;
			}
		}

		private void btnItemResult_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempCraftingRow["a_item_idx"]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nItemNeededID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				string strItemName = nItemNeededID.ToString();

				if (nItemNeededID > 0)
				{
					strItemName = pItemSelector.ReturnValues[1].ToString() ?? string.Empty;

					btnItemResult.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
				}
				else
				{
					btnItemResult.Image = null;
				}

				tbResultItemName.Text = strItemName;

				btnItemResult.Text = $"{nItemNeededID} - {strItemName}";

				pTempCraftingRow["a_item_idx"] = nItemNeededID;

				bUnsavedChanges = true;
			}
		}

		private void tbResultEXP_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempCraftingRow["a_make_exp"] = tbResultEXP.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbNeededGold_TextChanged(object sender, EventArgs e)
		{
			tbNeededGold.ForeColor = pMain.GetGoldColor(Convert.ToInt64(tbNeededGold.Text));

			if (bUserAction)
			{
				pTempCraftingRow["a_nas"] = tbNeededGold.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbNeededExp_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempCraftingRow["a_need_exp"] = tbNeededExp.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbItemResultType_SelectedIndexChanged(object sender, EventArgs e)
		{
#if USE_a_job_AND_a_item_type_TABLE
			if (bUserAction)
			{
				int nType = cbItemResultType.SelectedIndex;

				if (nType != -1)
				{
					pTempCraftingRow["a_type"] = nType;

					bUnsavedChanges = true;
				}
			}
#endif
		}

		private void cbJobSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
#if USE_a_job_AND_a_item_type_TABLE
			if (bUserAction)
			{
				int nJob = cbJobSelector.SelectedIndex;
				if (nJob != -1)
				{
					pTempCraftingRow["a_job"] = nJob;

					bUnsavedChanges = true;
				}
			}
#endif
		}

		private void gridMaterials_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
		{
			if (bUserAction)
			{
				if (Convert.ToInt32(gridMaterials.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
				{
					gridMaterials.Rows[e.RowIndex].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridMaterials.Rows[e.RowIndex].Cells["amount"].Value));
					gridMaterials.Rows[e.RowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
				}

				bUnsavedChanges = true;
			}
		}

		private void gridMaterials_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Right && e.ColumnIndex == -1 && e.RowIndex >= 0) // Header Column
				{
					ToolStripMenuItem clearItem = new("Clear");
					clearItem.Click += (_, _) =>
					{
						gridMaterials.Rows[e.RowIndex].Cells["itemIcon"].Value = new Bitmap(1, 1);
						gridMaterials.Rows[e.RowIndex].Cells["item"].Value = "-1";
						gridMaterials.Rows[e.RowIndex].Cells["item"].Tag = -1;
						gridMaterials.Rows[e.RowIndex].Cells["amount"].Value = 0;
					};

					cmLevels = new ContextMenuStrip();
					cmLevels.Items.Add(clearItem);
					cmLevels.Show(Cursor.Position);
				}
				else if (e.ColumnIndex == 1 && e.RowIndex >= 0) // Item Selector
				{
					ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempCraftingRow["a_item_idx"]), false);
					if (pItemSelector.ShowDialog() != DialogResult.OK)
						return;

					int nItemNeededID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
					if (nItemNeededID > 0)
					{
						gridMaterials.Rows[e.RowIndex].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));

						gridMaterials.Rows[e.RowIndex].Cells["item"].Value = $"{nItemNeededID} - {pItemSelector.ReturnValues[1]}";
						gridMaterials.Rows[e.RowIndex].Cells["item"].Tag = nItemNeededID;

						if (nItemNeededID == Defs.NAS_ITEM_DB_INDEX)
						{
							gridMaterials.Rows[e.RowIndex].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridMaterials.Rows[e.RowIndex].Cells["amount"].Value));
							gridMaterials.Rows[e.RowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
						}
						else
						{
							gridMaterials.Rows[e.RowIndex].Cells["amount"].Style.ForeColor = Color.FromArgb(208, 203, 148);
							gridMaterials.Rows[e.RowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(40, 40, 40);
						}
					}
				}
			}
		}
		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			if (pTempCraftingRow == null)
				return;

			bool bSuccess = true;
			int i = 0, nItemID = Convert.ToInt32(pTempCraftingRow["a_index"]);
			int[] nRequiredItemID = new int[Defs.MAX_FACTORY_ITEM_STUFF], nRequiredItemCount = new int[Defs.MAX_FACTORY_ITEM_STUFF];
			StringBuilder strbuilderQuery = new();

			foreach (DataGridViewRow row in gridMaterials.Rows)
			{
				nRequiredItemID[i] = Convert.ToInt32(row.Cells["item"].Tag);
				nRequiredItemCount[i] = Convert.ToInt32(row.Cells["amount"].Value);

				i++;
			}

			pTempCraftingRow["a_stuff"] = string.Join(" ", nRequiredItemID);
			pTempCraftingRow["a_stuff_cnt"] = string.Join(" ", nRequiredItemCount);

			// Check if crafting exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pCraftingRow = pMain.pTables.CraftingTable?.Select("a_index=" + nItemID).FirstOrDefault();
			if (pCraftingRow != null)	// UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_factory_item SET");

				foreach (DataColumn pCol in pTempCraftingRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempCraftingRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nItemID};");
			}
			else	// INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempCraftingRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempCraftingRow[pCol].ToString() ?? string.Empty)}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_factory_item ({strColumnsNames}) VALUES ({strColumnsValues});");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.ToString(), out long _))
			{
				try
				{
					if (pCraftingRow != null)  // Row exist in Global Table, update it.
					{
						pCraftingRow.ItemArray = (object[])pTempCraftingRow.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pCraftingRow = pMain.pTables.CraftingTable?.NewRow();
						if (pCraftingRow != null)
						{
							pCraftingRow.ItemArray = (object[])pTempCraftingRow.ItemArray.Clone();
							pMain.pTables.CraftingTable?.Rows.Add(pCraftingRow);
						}
					}

					pMain.pTables.CraftingTable.AcceptChanges();
				}
				catch (Exception ex)
				{
					string strError = $"Crafting Editor > Craft: {nItemID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Crafting Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nItemID;
						pSelectedItem.Text = nItemID + " - " + tbResultItemName.Text.ToString();

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "Crafting Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Crafting Editor > Craft: {nItemID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Crafting Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
