namespace LastChaos_ToolBoxNG
{
	public partial class QuestEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow? pTempQuestRow;

		public QuestEditor(Main mainForm)
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
				if (pMain.pTables.RareOptionTable.Select("a_index=" + pTempQuestRow["a_index"]).FirstOrDefault() != null)	// the current selected Quest, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Quest Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else	// the current selected Option is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Quest is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Quest Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		public async Task LoadQuestDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_need_min_level", "a_need_max_level", "a_name_" + pMain.pSettings.WorkLocale, "a_desc_" + pMain.pSettings.WorkLocale];

			if (pMain.pTables.QuestTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.QuestTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_quest ORDER BY a_index;");
				});

				if (pMain.pTables.QuestTable == null)
					pMain.pTables.QuestTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.QuestTable);
			}
		}

		private async void RareOptionEditor_LoadAsync(object sender, EventArgs e)
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
			cbType0.BeginUpdate();

			int j = 0;
			foreach (string strType in Defs.RareOptionItemGrades)
			{
				cbType0.Items.Add($"{j} - {strType}");

				j++;
			}

			cbType0.EndUpdate();
#if ENABLE_TYPE
			label7.Visible = true;
			cbTypeSelector.Visible = true;
#endif
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadQuestDataAsync(),
				pMain.GenericLoadOptionDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Rare Options & Options Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.OptionTable != null)
			{
				ComboBox cbObj;
				TextBox tbProb;

				for (int i = 0; i < Defs.DEF_RAREOPTION_MAX; i++)
				{
					cbObj = (ComboBox)Controls.Find("cbOptionID" + i, true)[0];
					cbObj.Enabled = false;

					cbObj.BeginUpdate();

					cbObj.Items.Add(new Main.ComboBoxItem
					{
						Value = -1,
						DisplayText = "-1 - NONE"
					});

					foreach (DataRow pRow in pMain.pTables.OptionTable.Rows)
					{
						int nOptionID = Convert.ToInt32(pRow["a_type"]);

						cbObj.Items.Add(new Main.ComboBoxItem
						{
							Value = nOptionID,
							DisplayText = $"{nOptionID} - {pRow["a_name_" + pMain.pSettings.WorkLocale]}"
						});
					}

					cbObj.EndUpdate();

					cbObj = (ComboBox)Controls.Find("cbOptionLevel" + i, true)[0];
					cbObj.Enabled = false;

					tbProb = (TextBox)Controls.Find("tbOptionProb" + i, true)[0];
					tbProb.Enabled = false;
				}
			}
			/****************************************/
			if (pMain.pTables.RareOptionTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.RareOptionTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_index"]), pRow["a_prefix_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Rare Options & Options Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void RareOptionEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				// Do nothing
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Quest Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nRareOptionID, bool bLoadFrompRareOptionTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			cbType0.SelectedIndex = -1;
#if ENABLE_TYPE
			cbTypeSelector.SelectedIndex = -1;
#endif
			/****************************************/
			if (bLoadFrompRareOptionTable && pMain.pTables.RareOptionTable != null)
			{
				pTempQuestRow = pMain.pTables.RareOptionTable.NewRow();
				pTempQuestRow.ItemArray = (object[])pMain.pTables.RareOptionTable.Select("a_index=" + nRareOptionID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nRareOptionID.ToString();
			/****************************************/
			int nTypeValue = Convert.ToInt32(pTempQuestRow["a_grade"]);

			if (nTypeValue >= Defs.RareOptionItemGrades.Length)
			{
				pMain.Logger(LogTypes.Error, $"Quest Editor > Quest: {nRareOptionID} Error: a_grade out of range.");
			}
			else
			{
				cbType0.SelectedIndex = nTypeValue;
			}
			/****************************************/
			tbMinLevel.Text = pTempQuestRow["a_attack"].ToString();
			tbDefense.Text = pTempQuestRow["a_defense"].ToString();
			tbMagicAttack.Text = pTempQuestRow["a_magic"].ToString();
			tbResistance.Text = pTempQuestRow["a_resist"].ToString();
			/****************************************/
			tbName.Text = pTempQuestRow["a_prefix_" + cbNationSelector.SelectedItem.ToString().ToLower()].ToString();
			/****************************************/
			ComboBox cbObj;
			TextBox tbProb;

			for (int i = 0; i < Defs.DEF_RAREOPTION_MAX; i++)
			{
				cbObj = (ComboBox)Controls.Find("cbOptionID" + i, true)[0];
				
				cbObj.Enabled = true;

				for (int j = 0; j < cbObj.Items.Count; j++)
				{
					if (((Main.ComboBoxItem)cbObj.Items[j]).Value.ToString() == pTempQuestRow["a_option_index" + i].ToString())
					{
						cbObj.SelectedIndex = j;

						break;
					}
				}

				tbProb = (TextBox)Controls.Find("tbOptionProb" + i, true)[0];

				tbProb.Text = pTempQuestRow["a_option_prob" + i].ToString();
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

				pMain.pTables.RareOptionTable?.Dispose();
				pMain.pTables.RareOptionTable = null;

				pMain.pTables.OptionTable?.Dispose();
				pMain.pTables.OptionTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				RareOptionEditor_LoadAsync(sender, e);
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
				int i, nNewRareOptionID = 9999;
				DataRow pNewRow;

				List<string> listTinyIntColumns =
				[
					"a_grade"
#if ENABLE_TYPE
					, "a_type"
#endif
				];

				List<string> listIntColumns =
				[
					"a_index",
					"a_attack",
					"a_defense",
					"a_magic",
					"a_resist",
					"a_option_index0",
					"a_option_level0",
					"a_option_prob0",
					"a_option_index1",
					"a_option_level1",
					"a_option_prob1",
					"a_option_index2",
					"a_option_level2",
					"a_option_prob2",
					"a_option_index3",
					"a_option_level3",
					"a_option_prob3",
					"a_option_index4",
					"a_option_level4",
					"a_option_prob4",
					"a_option_index5",
					"a_option_level5",
					"a_option_prob5",
					"a_option_index6",
					"a_option_level6",
					"a_option_prob6",
					"a_option_index7",
					"a_option_level7",
					"a_option_prob7",
					"a_option_index8",
					"a_option_level8",
					"a_option_prob8",
					"a_option_index9",
					"a_option_level9",
					"a_option_prob9"
				];

				List<string> listVarcharColumns = new();	// Here add all varchar columns.

				foreach (string strNation in pMain.pSettings.NationSupported)
					listVarcharColumns.Add("a_prefix_" + strNation.ToLower());

				if (pMain.pTables.RareOptionTable == null)
				{
					DataTable pRareOptionTableStruct = new();

					foreach (string strColumnName in listTinyIntColumns)
						pRareOptionTableStruct.Columns.Add(strColumnName, typeof(sbyte));

					foreach (string strColumnName in listIntColumns)
						pRareOptionTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listVarcharColumns)
						pRareOptionTableStruct.Columns.Add(strColumnName, typeof(string));

					pNewRow = pRareOptionTableStruct.NewRow();

					pRareOptionTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_rareoption ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewRareOptionID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewRareOptionID = pMain.AskForIndex(this.Text, "a_index")) == -1)	// I don't test it...
							return;
					}
				}
				else
				{
					nNewRareOptionID = Convert.ToInt32(pMain.pTables.RareOptionTable.Select().LastOrDefault()["a_index"]) + 1;

					pNewRow = pMain.pTables.RareOptionTable.NewRow();
				}

				List<object> listDefaultValue =
				[
					0,	// a_grade
#if ENABLE_TYPE
					0,	// a_type
#endif
					nNewRareOptionID,	// a_index
					0,	// a_attack
					0,	// a_defense
					0,	// a_magic
					0,	// a_resist
					-1,	// a_option_index0
					0,	// a_option_level0
					0,	// a_option_prob0
					-1,	// a_option_index1
					0,	// a_option_level1
					0,	// a_option_prob1
					-1,	// a_option_index2
					0,	// a_option_level2
					0,	// a_option_prob2
					-1,	// a_option_index3
					0,	// a_option_level3
					0,	// a_option_prob3
					-1,	// a_option_index4
					0,	// a_option_level4
					0,	// a_option_prob4
					-1,	// a_option_index5
					0,	// a_option_level5
					0,	// a_option_prob5
					-1,	// a_option_index6
					0,	// a_option_level6
					0,	// a_option_prob6
					-1,	// a_option_index7
					0,	// a_option_level7
					0,	// a_option_prob7
					-1,	// a_option_index8
					0,	// a_option_level8
					0,	// a_option_prob8
					-1,	// a_option_index9
					0,	// a_option_level9
					0	// a_option_prob9
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listDefaultValue.Add("New Quest");

				i = 0;
				foreach (string strColumnName in listTinyIntColumns.Concat(listIntColumns).Concat(listVarcharColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempQuestRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"Quest Editor > Quest: {nNewRareOptionID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewRareOptionID, "New Quest", true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Quest. Because it's temporary.", "Quest Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nRareOptionIDToCopy = Convert.ToInt32(pTempQuestRow["a_index"]);
				int nNewRareOptionID = Convert.ToInt32(pMain.pTables.RareOptionTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempQuestRow = pMain.pTables.RareOptionTable.NewRow();
				pTempQuestRow.ItemArray = (object[])pMain.pTables.RareOptionTable.Select("a_index=" + nRareOptionIDToCopy)[0].ItemArray.Clone();

				pTempQuestRow["a_index"] = nNewRareOptionID;

				foreach (string strNation in pMain.pSettings.NationSupported)
					pTempQuestRow["a_prefix_" + strNation.ToLower()] = pTempQuestRow["a_prefix_" + strNation.ToLower()] + " Copy";

				AddToList(nNewRareOptionID, pTempQuestRow["a_prefix_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, true);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nRareOptionID = Convert.ToInt32(pTempQuestRow["a_index"]);
			DataRow? pRareOptionTableRow = pMain.pTables.RareOptionTable?.Select("a_index=" + nRareOptionID).FirstOrDefault();

			if (pRareOptionTableRow != null)
			{
				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_rareoption WHERE a_index={nRareOptionID};", out long _)))
				{
					string strError = $"Quest Editor > Quest: {nRareOptionID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pRareOptionTableRow != null)
						pMain.pTables.RareOptionTable?.Rows.Remove(pRareOptionTableRow);
				}
				catch (Exception ex)
				{
					string strError = $"Quest Editor > Quest: {nRareOptionID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Quest Deleted successfully!", "Quest Editor", MessageBoxButtons.OK);

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

				tbName.Text = pTempQuestRow["a_prefix_" + cbNationSelector.SelectedItem.ToString().ToLower()].ToString();

				bUserAction = true;
			}
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempQuestRow["a_prefix_" + cbNationSelector.SelectedItem.ToString().ToLower()] = tbName.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbGradeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbType0.SelectedIndex;
				if (nType != -1)
				{
					pTempQuestRow["a_grade"] = nType;


					label3.Focus();

					bUnsavedChanges = true;
				}
			}
		}
		/****************************************/
		private void tbAttack_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempQuestRow["a_attack"] = tbMinLevel.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbDefense_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempQuestRow["a_defense"] = tbDefense.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbMagicAttack_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempQuestRow["a_magic"] = tbMagicAttack.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbResistance_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempQuestRow["a_resist"] = tbResistance.Text;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void OptionSelectAction(int nNumber)
		{
			ComboBox cbObj = (ComboBox)Controls.Find("cbOptionID" + nNumber, true)[0];
			int nType = cbObj.SelectedIndex;

			if (nType != -1)
			{
				ComboBox cbLevel = (ComboBox)Controls.Find("cbOptionLevel" + nNumber, true)[0];
				TextBox tbObj = (TextBox)Controls.Find("tbOptionProb" + nNumber, true)[0];
				int nOptionID = Convert.ToInt32(((Main.ComboBoxItem)cbObj.SelectedItem).Value);

				if (nType > 0)
				{
					DataRow pRow = pMain.pTables.OptionTable?.Select("a_type=" + nOptionID).FirstOrDefault();
					string[] strProb = pRow["a_prob"].ToString().Split(' ');
					int i = 0;

					cbLevel.SelectedIndex = -1;
					cbLevel.Enabled = true;

					tbObj.Enabled = true;

					cbLevel.Items.Clear();
					cbLevel.BeginUpdate();

					foreach (string strLevel in pRow["a_level"].ToString().Split(' '))
					{
						if (i < strProb.Length && strProb[i] != null)
						{
							cbLevel.Items.Add($"{(i + 1)} - Value: {strLevel} Prob: {strProb[i]}");

							if ((i + 1) == Convert.ToInt32(pTempQuestRow["a_option_level" + nNumber]))
								cbLevel.SelectedIndex = i;
						}
						else
						{
							pMain.Logger(LogTypes.Error, $"Rate Option Editor > Option: {nOptionID} Error: a_level & a_prob not match.");
						}

						i++;
					}

					cbLevel.EndUpdate();

					if (cbLevel.SelectedIndex == -1)
						cbLevel.SelectedIndex = 0;
				}
				else
				{
					cbLevel.SelectedIndex = -1;
					cbLevel.Enabled = false;

					tbObj.Enabled = false;
				}

				if (bUserAction)
				{
					pTempQuestRow["a_option_index" + nNumber] = nOptionID;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbOptionID0_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(0); }
		private void cbOptionID1_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(1); }
		private void cbOptionID2_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(2); }
		private void cbOptionID3_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(3); }
		private void cbOptionID4_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(4); }
		private void cbOptionID5_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(5); }
		private void cbOptionID6_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(6); }
		private void cbOptionID7_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(7); }
		private void cbOptionID8_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(8); }
		private void cbOptionID9_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(9); }
		/****************************************/
		private void OptionLevelAction(int nNumber)
		{
			if (bUserAction)
			{
				ComboBox cbObj = (ComboBox)Controls.Find("cbOptionLevel" + nNumber, true)[0];
				int nLevel = cbObj.SelectedIndex;

				if (nLevel != -1)
				{
					pTempQuestRow["a_option_level" + nNumber] = nLevel + 1;

					bUnsavedChanges = true;
				}
			}
		}
		
		private void cbOptionLevel0_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(0); }
		private void cbOptionLevel1_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(1); }
		private void cbOptionLevel2_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(2); }
		private void cbOptionLevel3_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(3); }
		private void cbOptionLevel4_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(4); }
		private void cbOptionLevel5_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(5); }
		private void cbOptionLevel6_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(6); }
		private void cbOptionLevel7_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(7); }
		private void cbOptionLevel8_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(8); }
		private void cbOptionLevel9_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(9); }
		/****************************************/
		private void OptionProbAction(int nNumber)
		{
			TextBox cbObj = (TextBox)Controls.Find("tbOptionProb" + nNumber, true)[0];
			int nValue;

			if (cbObj.Text != null && int.TryParse(cbObj.Text, out nValue))
			{
				if (bUserAction)
				{
					pTempQuestRow["a_option_prob" + nNumber] = nValue;

					bUnsavedChanges = true;
				}

				((Label)Controls.Find("lOptionProbPercentage" + nNumber, true)[0]).Text = ((nValue * 100.0f) / 10000.0f) + "%";
			}
		}

		private void tbOptionProb0_TextChanged(object sender, EventArgs e) { OptionProbAction(0); }
		private void tbOptionProb1_TextChanged(object sender, EventArgs e) { OptionProbAction(1); }
		private void tbOptionProb2_TextChanged(object sender, EventArgs e) { OptionProbAction(2); }
		private void tbOptionProb3_TextChanged(object sender, EventArgs e) { OptionProbAction(3); }
		private void tbOptionProb4_TextChanged(object sender, EventArgs e) { OptionProbAction(4); }
		private void tbOptionProb5_TextChanged(object sender, EventArgs e) { OptionProbAction(5); }
		private void tbOptionProb6_TextChanged(object sender, EventArgs e) { OptionProbAction(6); }
		private void tbOptionProb7_TextChanged(object sender, EventArgs e) { OptionProbAction(7); }
		private void tbOptionProb8_TextChanged(object sender, EventArgs e) { OptionProbAction(8); }
		private void tbOptionProb9_TextChanged(object sender, EventArgs e) { OptionProbAction(9); }
		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nItemID = Convert.ToInt32(pTempQuestRow["a_index"]);
			StringBuilder strbuilderQuery = new();

			// Check if RareOtion exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pRareOptionTableRow = pMain.pTables.RareOptionTable?.Select("a_index=" + nItemID).FirstOrDefault();
			if (pRareOptionTableRow != null)	// UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_rareoption SET");

				foreach (DataColumn pCol in pTempQuestRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempQuestRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nItemID};");
			}
			else	// INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempQuestRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempQuestRow[pCol].ToString())}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_rareoption ({strColumnsNames}) VALUES ({strColumnsValues});");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.ToString(), out long _))
			{
				try
				{
					if (pRareOptionTableRow != null)    // Row exist in Global Table, update it.
					{
						pRareOptionTableRow.ItemArray = (object[])pTempQuestRow.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pRareOptionTableRow = pMain.pTables.RareOptionTable.NewRow();
						pRareOptionTableRow.ItemArray = (object[])pTempQuestRow.ItemArray.Clone();
						pMain.pTables.RareOptionTable.Rows.Add(pRareOptionTableRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Quest Editor > Quest: {nItemID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

						MessageBox.Show("Changes applied successfully!", "Quest Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Quest Editor > Quest: {nItemID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
