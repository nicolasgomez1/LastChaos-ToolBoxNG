//#define ENABLE_MAXLEVEL	// NOTE: I removed it from my source, now i use m_nrecords
//#define ENABLE_ATTRIBUTE	// NOTE: Isn't used anymore (Was it really used sometime?)
//#define ENABLE_TOGGLE	// NOTE: Isn't used anymore

namespace LastChaos_ToolBoxNG
{
	public partial class MagicEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempMagicRow;
		private ContextMenuStrip? cmLevels;
		private string[] strParamsNames = { "PsP", "PtP", "HsP", "HtP" };

		public MagicEditor(Main mainForm)
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
				if (pMain.pTables.MagicTable?.Select("a_index=" + pTempMagicRow["a_index"]).FirstOrDefault() != null)	// the current selected Magic, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Magic Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else	// the current selected Magic is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Magic is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Magic Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private async Task LoadMagicDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_name",
#if ENABLE_MAXLEVEL
				"a_maxlevel",
#endif
				"a_type",
				"a_subtype",
				"a_damagetype",
				"a_hittype",
#if ENABLE_ATTRIBUTE
				"a_attribute",
#endif
				"a_psp",
				"a_ptp",
				"a_hsp",
				"a_htp"
#if ENABLE_TOGGLE
				, "a_togle"
#endif
			];

			if (pMain.pTables.MagicTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.MagicTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_magic ORDER BY a_index;");
				});

				if (pMain.pTables.MagicTable == null)
					pMain.pTables.MagicTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.MagicTable);
			}

			bRequestNeeded = false;
			listQueryCompose.Clear();

			listQueryCompose = ["a_level", "a_power", "a_hitrate"];

			if (pMain.pTables.MagicLevelTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.MagicLevelTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_magiclevel ORDER BY a_level;");
				});

				if (pMain.pTables.MagicLevelTable == null)
					pMain.pTables.MagicLevelTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.MagicLevelTable);
			}
		}

		private void LoadLevelData(int nMagicID)
		{
			int i = 0;
			DataRow[]? pTempMagicLevelRows = pMain.pTables.MagicLevelTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nMagicID).ToArray();

			if (pTempMagicLevelRows?.Length > 0)
			{
				gridLevels.SuspendLayout();

				foreach (DataRow pRow in pTempMagicLevelRows)
				{
					gridLevels.Rows.Insert(i);

					gridLevels.Rows[i].HeaderCell.Value = (i + 1).ToString();

					gridLevels.Rows[i].Cells["power"].Value = pRow["a_power"];
					gridLevels.Rows[i].Cells["hitrate"].Value = pRow["a_hitrate"];

					i++;
				}

				gridLevels.ResumeLayout();
#if ENABLE_MAXLEVEL
				if (Convert.ToInt32(pTempMagicRow["a_maxlevel"]) != pTempMagicLevelRows.Length)
					pMain.Logger($"Magic Editor > Magic: {nMagicID} Error: a_maxlevel doesn't match with t_magiclevel rows related to this Magic (a_maxlevel will be fixed when press Update).");
#endif
			}
			else
			{
#if ENABLE_MAXLEVEL
				if (Convert.ToInt32(pTempMagicRow["a_maxlevel"]) > 0)
					pMain.Logger($"Magic Editor > Magic: {nMagicID} Error: a_maxlevel doesn't match with t_magiclevel rows related to this Magic (a_maxlevel will be fixed when press Update).");
#endif
			}
		}

		private async void MagicEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
#if ENABLE_ATTRIBUTE
			lAttribute.Visible = true;
			tbAttribute.Visible = true;
#endif
#if ENABLE_TOGGLE
			cbToggle.Visible = true;
#endif
			/****************************************/
			cbTypeSelector.BeginUpdate();

			int j = 0;
			foreach (var strType in Defs.MagicTypesAndSubTypes)
			{
				cbTypeSelector.Items.Add($"{j} - {strType.Key}");

				j++;
			}

			cbTypeSelector.EndUpdate();
			/****************************************/
			cbDamageTypeSelector.BeginUpdate();

			j = 0;
			foreach (string strDamageType in Defs.MagicDamageTypes)
			{
				cbDamageTypeSelector.Items.Add($"{j} - {strDamageType}");

				j++;
			}

			cbDamageTypeSelector.EndUpdate();
			/****************************************/
			cbHitTypeSelector.BeginUpdate();

			j = 0;
			foreach (string strHitType in Defs.MagicHitTypes)
			{
				cbHitTypeSelector.Items.Add($"{j} - {strHitType}");

				j++;
			}

			cbHitTypeSelector.EndUpdate();
			/****************************************/
			ComboBox? pObj = null;

			foreach (string strParamName in strParamsNames)
			{
				pObj = (ComboBox)Controls.Find("cb" + strParamName, true)[0];

				pObj.Items.Clear();
				pObj.BeginUpdate();

				j = 0;
				foreach (string strType in Defs.MagicParamTypes)
				{
					pObj.Items.Add($"{j} - {strType}");

					j++;
				}

				pObj.EndUpdate();
			}
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await LoadMagicDataAsync();
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Magics & Magic Levels Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.MagicTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.MagicTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_index"]), pRow["a_name"].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Magics & Magic Levels Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void MagicEditor_FormClosing(object sender, FormClosingEventArgs e)
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
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Magic Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nMagicID, bool bLoadFrompMagicTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			cbTypeSelector.SelectedIndex = -1;
			cbSubTypeSelector.SelectedIndex = -1;
			cbDamageTypeSelector.SelectedIndex = -1;
			cbHitTypeSelector.SelectedIndex = -1;

			foreach (string strParamName in strParamsNames)
				((ComboBox)Controls.Find("cb" + strParamName, true)[0]).SelectedIndex = -1;

			gridLevels.Rows.Clear();
			/****************************************/
			if (bLoadFrompMagicTable && pMain.pTables.MagicTable != null)
			{
				pTempMagicRow = pMain.pTables.MagicTable.NewRow();
				pTempMagicRow.ItemArray = (object[])pMain.pTables.MagicTable.Select("a_index=" + nMagicID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nMagicID.ToString();
			/****************************************/
			tbName.Text = pTempMagicRow["a_name"].ToString();
			/****************************************/
#if ENABLE_ATTRIBUTE
			tbAttribute.Text = pTempMagicRow["a_attribute"].ToString();
#endif
			/****************************************/
#if ENABLE_TOGGLE
			if (pTempMagicRow["a_togle"].ToString() == "1")
				cbToggle.Checked = true;
			else
				cbToggle.Checked = false;
#endif
			/****************************************/
			int nType = Convert.ToInt32(pTempMagicRow["a_type"]);
			if (nType < 0 || nType >= Defs.MagicTypesAndSubTypes.Count())
			{
				pMain.Logger(LogTypes.Error, $"Magic Editor > Magic: {nMagicID} Error: a_type out of range.");
			}
			else
			{
				cbTypeSelector.SelectedIndex = nType;

				int nSubType = Convert.ToInt32(pTempMagicRow["a_subtype"]);

				if (nSubType < 0 || nSubType >= Defs.MagicTypesAndSubTypes[Defs.MagicTypesAndSubTypes.Keys.ElementAt(nType)].Count)
					pMain.Logger(LogTypes.Error, $"Magic Editor > Magic: {nMagicID} Error: a_subtype out of range.");
				else
					cbSubTypeSelector.SelectedIndex = nSubType;
			}
			/****************************************/
			int nDamageType = Convert.ToInt32(pTempMagicRow["a_damagetype"]);
			if (nDamageType < 0 || nDamageType >= Defs.MagicDamageTypes.Length)
				pMain.Logger(LogTypes.Error, $"Magic Editor > Magic: {nMagicID} Error: a_damagetype out of range.");
			else
				cbDamageTypeSelector.SelectedIndex = nDamageType;
			/****************************************/
			int nHitType = Convert.ToInt32(pTempMagicRow["a_hittype"]);
			if (nHitType < 0 || nHitType >= Defs.MagicHitTypes.Length)
				pMain.Logger(LogTypes.Error, $"Magic Editor > Magic: {nMagicID} Error: a_hittype out of range.");
			else
				cbHitTypeSelector.SelectedIndex = nHitType;
			/****************************************/
			foreach (string strParamName in strParamsNames)
			{
				int nParamValue = Convert.ToInt32(pTempMagicRow["a_" + strParamName]);

				if (nParamValue < 0 || nParamValue >= Defs.MagicParamTypes.Length)
					pMain.Logger(LogTypes.Error, $"Magic Editor > Magic: {nMagicID} Error: a_{strParamName} out of range.");
				else
					((ComboBox)Controls.Find("cb" + strParamName, true)[0]).SelectedIndex = nParamValue;
			}
			/****************************************/
			LoadLevelData(nMagicID);
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

				pMain.pTables.MagicTable?.Dispose();
				pMain.pTables.MagicTable = null;

				pMain.pTables.MagicLevelTable?.Dispose();
				pMain.pTables.MagicLevelTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				MagicEditor_LoadAsync(sender, e);
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
				int i, nNewMagicID = 9999;
				DataRow pNewRow;

				List<string> listIntColumns =
				[
					"a_index",
					"a_psp",
					"a_ptp",
					"a_hsp",
					"a_htp"
				];

				List<string> listVarcharColumns =
				[
					"a_name"
				];

				List<string> listTinyIntColumns =
				[
#if ENABLE_MAXLEVEL
					"a_maxlevel",
#endif
					"a_type",
					"a_subtype",
					"a_damagetype",
					"a_hittype"
#if ENABLE_ATTRIBUTE
					, "a_attribute"
#endif
#if ENABLE_TOGGLE
					, "a_togle"
#endif
				];

				if (pMain.pTables.MagicTable == null)
				{
					DataTable pMagicTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pMagicTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listVarcharColumns)
						pMagicTableStruct.Columns.Add(strColumnName, typeof(string));

					foreach (string strColumnName in listTinyIntColumns)
						pMagicTableStruct.Columns.Add(strColumnName, typeof(sbyte));

					pNewRow = pMagicTableStruct.NewRow();

					pMagicTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_magic ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewMagicID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewMagicID = pMain.AskForIndex(this.Text, "a_index")) == -1)	// I don't test it...
							return;
					}
				}
				else
				{
					nNewMagicID = Convert.ToInt32(pMain.pTables.MagicTable.Select().LastOrDefault()["a_index"]) + 1;

					pNewRow = pMain.pTables.MagicTable.NewRow();
				}

				List<object> listDefaultValue =
				[
					nNewMagicID,	// a_index
					0,	// a_psp
					0,	// a_ptp
					0,	// a_hsp
					0,	// a_htp
					"New Magic",	// a_name
#if ENABLE_MAXLEVEL
					0,	// a_maxlevel
#endif
					0,	// a_type
					0,	// a_subtype
					0,	// a_damagetype
					0	// a_hittype
#if ENABLE_ATTRIBUTE
					, 0	// a_attribute
#endif
#if ENABLE_TOGGLE
					, 0	// a_togle
#endif
				];

				i = 0;
				foreach (string strColumnName in listIntColumns.Concat(listVarcharColumns).Concat(listTinyIntColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempMagicRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"Magic Editor > Magic: {nNewMagicID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Magic Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewMagicID, "New Magic", true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Magic. Because it's temporary.", "Magic Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nMagicIDToCopy = Convert.ToInt32(pTempMagicRow["a_index"]);
				int nNewMagicID = Convert.ToInt32(pMain.pTables.MagicTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempMagicRow = pMain.pTables.MagicTable.NewRow();
				pTempMagicRow.ItemArray = (object[])pMain.pTables.MagicTable.Select("a_index=" + nMagicIDToCopy)[0].ItemArray.Clone();

				pTempMagicRow["a_index"] = nNewMagicID;
				pTempMagicRow["a_name"] = pTempMagicRow["a_name"] + " Copy";

				AddToList(nNewMagicID, pTempMagicRow["a_name"].ToString() ?? string.Empty, true);

				LoadLevelData(nMagicIDToCopy);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nMagicID = Convert.ToInt32(pTempMagicRow["a_index"]);
			DataRow? pMagicRow = pMain.pTables.MagicTable?.Select("a_index=" + nMagicID).FirstOrDefault();

			if (pMagicRow != null)
			{
				StringBuilder strbuilderQuery = new();

				strbuilderQuery.Append("START TRANSACTION;\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_magic WHERE a_index={nMagicID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_magiclevel WHERE a_index={nMagicID};\n");

				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _)))
				{
					string strError = $"Magic Editor > Magic: {nMagicID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

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
		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempMagicRow["a_name"] = tbName.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAttribute_TextChanged(object sender, EventArgs e)
		{
#if ENABLE_ATTRIBUTE
			if (bUserAction)
			{
				pTempMagicRow["a_attribute"] = tbAttribute.Text;

				bUnsavedChanges = true;
			}
#endif
		}

		private void cbToggle_CheckedChanged(object sender, EventArgs e)
		{
#if ENABLE_TOGGLE
			if (bUserAction)
			{
				pTempMagicRow["a_togle"] = cbToggle.Checked ? "1" : "0";

				bUnsavedChanges = true;
			}
#endif
		}

		private void cbTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nType = cbTypeSelector.SelectedIndex;
			if (nType != -1)
			{
				cbSubTypeSelector.Items.Clear();
				cbSubTypeSelector.BeginUpdate();

				int i = 0;
				foreach (string strSubType in Defs.MagicTypesAndSubTypes[Defs.MagicTypesAndSubTypes.Keys.ElementAt(nType)])
				{
					cbSubTypeSelector.Items.Add($"{i} - {strSubType}");

					i++;
				}

				cbSubTypeSelector.EndUpdate();
				cbSubTypeSelector.Enabled = true;

				if (bUserAction)
				{
					pTempMagicRow["a_type"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbSubTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbSubTypeSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempMagicRow["a_subtype"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbDamageTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbDamageTypeSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempMagicRow["a_damagetype"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbHitTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbHitTypeSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempMagicRow["a_hittype"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbPsP_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbPsP.SelectedIndex;

				if (nType != -1)
				{
					pTempMagicRow["a_psp"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbPtP_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbPtP.SelectedIndex;

				if (nType != -1)
				{
					pTempMagicRow["a_ptp"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbHsP_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbHsP.SelectedIndex;

				if (nType != -1)
				{
					pTempMagicRow["a_hsp"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbHtP_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbHtP.SelectedIndex;

				if (nType != -1)
				{
					pTempMagicRow["a_htp"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void gridLevels_CellValueChanged(object? sender, DataGridViewCellEventArgs e) { if (bUserAction) bUnsavedChanges = true; }

		private void gridLevels_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Right && e.ColumnIndex == -1)	// Header Column
				{
					ToolStripMenuItem addItem = new("Add New");
					addItem.Click += (_, _) =>
					{
						int nRow = gridLevels.Rows.Count;

						gridLevels.Rows.Insert(nRow);

						gridLevels.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

						gridLevels.Rows[nRow].Cells["power"].Value = 0;
						gridLevels.Rows[nRow].Cells["hitrate"].Value = 0;

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
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nMagicID = Convert.ToInt32(pTempMagicRow["a_index"]);
			StringBuilder strbuilderQuery = new();

			// Init transaction.
			strbuilderQuery.Append("START TRANSACTION;\n");

			strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_magiclevel WHERE a_index={nMagicID};\n");

			int nMaxLevel = gridLevels.Rows.Count;
			DataRow[] pLevelRows = new DataRow[nMaxLevel];

			if (nMaxLevel > 0)
			{
				int i = 1;
				bool bComposeColumnsNames = true;
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataGridViewRow row in gridLevels.Rows)
				{
					DataRow pRow = pMain.pTables.MagicLevelTable.NewRow();

					pRow["a_index"] = nMagicID;
					pRow["a_level"] = i;
					pRow["a_power"] = Convert.ToInt32(row.Cells["power"].Value);
					pRow["a_hitrate"] = Convert.ToInt32(row.Cells["hitrate"].Value);

					pLevelRows[i - 1] = pRow;

					strColumnsValues.Append("(");

					foreach (DataColumn pCol in pRow.Table.Columns)
					{
						if (bComposeColumnsNames)
							strColumnsNames.Append(pCol.ColumnName + ", ");

						strColumnsValues.Append($"'{pMain.EscapeChars(pRow[pCol.ColumnName].ToString())}', ");
					}

					strColumnsValues.Length -= 2;

					strColumnsValues.Append("), ");

					bComposeColumnsNames = false;

					i++;
				}

				strColumnsValues.Length -= 2;
				strColumnsNames.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_magiclevel ({strColumnsNames}) VALUES {strColumnsValues};\n");
			}
#if ENABLE_MAXLEVEL
			pTempMagicRow["a_maxlevel"] = nMaxLevel.ToString();
#endif
			// Check if Magic exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pMagicRow = pMain.pTables.MagicTable?.Select("a_index=" + nMagicID).FirstOrDefault();
			if (pMagicRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_magic SET");

				foreach (DataColumn pCol in pTempMagicRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempMagicRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nMagicID};\n");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempMagicRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempMagicRow[pCol].ToString())}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_magic ({strColumnsNames}) VALUES ({strColumnsValues});\n");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _))
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

					foreach (DataRow pRow in pLevelRows)
					{
						if (pRow != null)
							pMain.pTables.MagicLevelTable?.Rows.Add(pRow);
					}

					if (pMagicRow != null) // Row exist in Global Table, update it.
					{
						pMagicRow.ItemArray = (object[])pTempMagicRow.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pMagicRow = pMain.pTables.MagicTable?.NewRow();
						pMagicRow.ItemArray = (object[])pTempMagicRow.ItemArray.Clone();
						pMain.pTables.MagicTable?.Rows.Add(pMagicRow);
					}
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
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nMagicID;
						pSelectedItem.Text = nMagicID + " - " + tbName.Text.ToString();

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "Magic Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Magic Editor > Magic: {nMagicID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Magic Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
