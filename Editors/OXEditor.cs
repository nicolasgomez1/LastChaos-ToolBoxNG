namespace LastChaos_ToolBoxNG
{
	public partial class OXEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false, bUnsavedChangesRewards = false;
		private int nSearchPosition = 0, nSearchRewardsPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempQuestionRow;
		private ContextMenuStrip? cmRewards;

		public OXEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridRewards.TopLeftHeaderCell.Value = "N°";
			((DataGridViewComboBoxColumn)gridRewards.Columns["rewardStage"]).DataSource = Enumerable.Range(1, 999).ToList();
			((DataGridViewComboBoxColumn)gridRewards.Columns["rewardStage"]).ValueType = typeof(int);
		}
		
		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.OXQuizTable?.Select("a_index=" + pTempQuestionRow["a_index"]).FirstOrDefault() != null)   // the current selected quiz, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "OX Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else    // the current selected quiz is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Quiz is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "OX Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private async Task LoadOXDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string> { "a_name", "a_answer" };

			foreach (string strNation in pMain.pSettings.NationSupported)
				listQueryCompose.Add("a_question_" + strNation.ToLower());

			if (pMain.pTables.OXQuizTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.OXQuizTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_oxquiz ORDER BY a_index;");
				});

				if (pMain.pTables.OXQuizTable == null)
					pMain.pTables.OXQuizTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.OXQuizTable);
			}

			bRequestNeeded = false;
			listQueryCompose.Clear();

			listQueryCompose = new List<string> { "a_item_idx", "a_item_flag", "a_item_amount" };

			if (pMain.pTables.OXRewardTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.OXRewardTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, a_answered_required, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_oxreward ORDER BY a_answered_required;");
				});

				if (pMain.pTables.OXRewardTable == null)
					pMain.pTables.OXRewardTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.OXRewardTable);
			}
		}

		private async void OXEditor_LoadAsync(object sender, EventArgs e)
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
				LoadOXDataAsync(),
				pMain.GenericLoadItemDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"OX Quiz's, OX Quiz'sRewards & Items Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.OXQuizTable != null && pMain.pTables.OXRewardTable != null && pMain.pTables.ItemTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.OXQuizTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_index"]), pRow["a_name"].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
				/****************************************/
				bUserAction = false;

				// Rewards
				gridRewards.Rows.Clear();
				gridRewards.SuspendLayout();

				int nRewardItemID, i = 0;
				string strRewardItemName;
				StringBuilder strTooltip = new();

				foreach (DataRow pRow in pMain.pTables.OXRewardTable.Rows)
				{
					gridRewards.Rows.Insert(i);

					gridRewards.Rows[i].HeaderCell.Value = (i + 1).ToString();

					int nAnsweredRequired = Convert.ToInt32(pRow["a_answered_required"]);
					int nMin = ((List<int>)((DataGridViewComboBoxColumn)gridRewards.Columns["rewardStage"]).DataSource).Min();
					int nMax = ((List<int>)((DataGridViewComboBoxColumn)gridRewards.Columns["rewardStage"]).DataSource).Max();

					if (nAnsweredRequired < nMin || nAnsweredRequired > nMax)
					{
						nAnsweredRequired = nMax;

						pMain.Logger(LogTypes.Error, $"OX Editor > Reward: {pRow["a_index"]} Error: a_answered_required out of range (999 was asigned to it).");
					}

					gridRewards.Rows[i].Cells["rewardStage"].Value = nAnsweredRequired;

					nRewardItemID = Convert.ToInt32(pRow["a_item_idx"]);
					strRewardItemName = nRewardItemID.ToString();

					if (nRewardItemID > 0)
					{
						DataRow? pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nRewardItemID).FirstOrDefault();
						if (pItemRow != null)
						{
							strRewardItemName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

							gridRewards.Rows[i].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
						}
					}

					gridRewards.Rows[i].Cells["item"].Value = strRewardItemName;
					gridRewards.Rows[i].Cells["item"].Tag = nRewardItemID;
					gridRewards.Rows[i].Cells["flag"].Value = pRow["a_item_flag"];

					strTooltip.Clear();
					strTooltip = new StringBuilder();

					long lItemFlag = Convert.ToInt64(pRow["a_item_flag"]);

					for (int j = 0; j < Defs.ItemFlags.Length; j++)
					{
						if ((lItemFlag & 1L << j) != 0)
							strTooltip.Append(Defs.ItemFlags[j] + "\n");
					}

					gridRewards.Rows[i].Cells["flag"].ToolTipText = strTooltip.ToString();
					gridRewards.Rows[i].Cells["amount"].Value = pRow["a_item_amount"];

					if (nRewardItemID == Defs.NAS_ITEM_DB_INDEX)
					{
						gridRewards.Rows[i].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(pRow["a_item_amount"]));
						gridRewards.Rows[i].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
					}

					i++;
				}

				gridRewards.ResumeLayout();

				bUserAction = true;

				btnUpdateRewards.Enabled = true;
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload OX Quiz's, OX Quiz'sRewards & Items Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void OXEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				if (cmRewards != null)
				{
					cmRewards.Dispose();
					cmRewards = null;
				}
			}

			if (bUnsavedChanges || bUnsavedChangesRewards)
			{
				string strMessage = "You have unsaved changes";

				if (bUnsavedChangesRewards)
					strMessage += " (Rewards)";

				DialogResult pDialogReturn = MessageBox.Show(strMessage + ". Do you want to discard them and exit?", "OX Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nQuestionID, bool bLoadFrompOXQuizTable)
		{
			bUserAction = false;
			/****************************************/
			if (bLoadFrompOXQuizTable && pMain.pTables.OXQuizTable != null)
			{
				pTempQuestionRow = pMain.pTables.OXQuizTable.NewRow();
				pTempQuestionRow.ItemArray = (object[])pMain.pTables.OXQuizTable.Select("a_index=" + nQuestionID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = pTempQuestionRow["a_index"].ToString();

			tbName.Text = pTempQuestionRow["a_name"].ToString();

			string strAnswer = pTempQuestionRow["a_answer"].ToString() ?? string.Empty;

			if (strAnswer == "O")
			{
				rbO.Checked = true;
			}
			else if (strAnswer == "X")
			{
				rbX.Checked = true;
			}
			else
			{
				rbO.Checked = true;

				pMain.Logger(LogTypes.Error, $"OX Editor > Reward: {pTempQuestionRow["a_index"]} Error: Incorrect a_answer value (O was asigned to it).");
			}

			tbQuestion.Text = pTempQuestionRow["a_question_" + pMain.pSettings.WorkLocale].ToString();

			btnTranslate.Enabled = true;
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

		private void cbNationSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				bUserAction = false;

				tbQuestion.Text = pTempQuestionRow["a_question_" + cbNationSelector.SelectedItem?.ToString()?.ToLower()].ToString();

				bUserAction = true;
			}
		}

		private void btnTranslate_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				cbNationSelector.Enabled = false;

				btnTranslate.Enabled = false;

				tbQuestion.Enabled = false;

				pMain.Logger(LogTypes.Message, $"OX Editor > Translating Quiz ID: {tbID.Text}...");

				string strTranslatedString = tbQuestion.Text;

				using (HttpClient httpClient = new())
				{
					httpClient.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");

					if (rbGoogle.Checked)
					{
						JsonElement root;

						foreach (string strNation in cbNationSelector.Items)
						{
							if (cbNationSelector.Text != strNation)
							{
								using (HttpResponseMessage httpResponse = httpClient.GetAsync($"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl={Defs.LanguageCodes[strNation]}&dt=t&q=" + strTranslatedString.Replace("\n", "%0A")).Result)
								{
									if (httpResponse.IsSuccessStatusCode)
									{
										using (JsonDocument jsonData = JsonDocument.Parse(httpResponse.Content.ReadAsStringAsync().Result))
										{
											root = jsonData.RootElement[0];

											if (root.ValueKind == JsonValueKind.Array)
												strTranslatedString = root[0][0].GetString();
										}

										pTempQuestionRow["a_question_" + strNation.ToLower()] = strTranslatedString;

										pMain.Logger(LogTypes.Success, $"OX Editor > Translate of Quiz ID: {tbID.Text} To Language: {strNation}({Defs.LanguageCodes[strNation]}) Done!");

										bUnsavedChanges = true;
									}
									else
									{
										pMain.Logger(LogTypes.Error, $"OX Editor > Translate of Quiz ID: {tbID.Text} To Language: {strNation} HTTP Request failed: " + httpResponse.StatusCode);
									}
								}
							}
						}
					}
					else
					{
						httpClient.DefaultRequestHeaders.Add("Authorization", "DeepL-Auth-Key " + pMain.pSettings.DeepLAPIKey);

						foreach (string strNation in cbNationSelector.Items)
						{
							if (cbNationSelector.Text != strNation)
							{
								using (HttpResponseMessage httpResponse = httpClient.GetAsync(pMain.pSettings.DeepLURL + $"?text={strTranslatedString.Replace("\n", "%0A")}&source_lang=EN&target_lang=" + Defs.LanguageCodes[strNation]).Result)
								{
									if (httpResponse.IsSuccessStatusCode)
									{
										string strTempString;

										using (JsonDocument jsonData = JsonDocument.Parse(httpResponse.Content.ReadAsStringAsync().Result))
											strTempString = jsonData.RootElement.GetProperty("translations")[0].GetProperty("text").GetString() ?? string.Empty;

										if (strTempString.Length > 0)
										{
											pTempQuestionRow["a_question_" + strNation.ToLower()] = strTranslatedString;

											pMain.Logger(LogTypes.Success, $"OX Editor > Translate of Quiz ID: {tbID.Text} To Language: {strNation}({Defs.LanguageCodes[strNation]}) Done!");

											bUnsavedChanges = true;
										}
									}
									else
									{
										pMain.Logger(LogTypes.Error, $"OX Editor > Translate of Quiz ID: {tbID.Text} To Language: {strNation} HTTP Request failed: " + httpResponse.StatusCode);
									}
								}
							}
						}
					}
				}
#if DEBUG
				stopwatch.Stop();
				pMain.Logger(LogTypes.Success, $"OX Editor > Translate Done! (Took: {stopwatch.ElapsedMilliseconds}ms)");
#else
				pMain.Logger(LogTypes.Success, "OX Editor > Translate Done!");
#endif
				cbNationSelector.Enabled = true;

				btnTranslate.Enabled = true;

				tbQuestion.Enabled = true;
			}
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempQuestionRow["a_name"] = tbName.Text;

				bUnsavedChanges = true;
			}
		}

		private void rbOX_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				string strAnswer = "O";

				if (rbX.Checked)
					strAnswer = "X";

				pTempQuestionRow["a_answer"] = strAnswer;

				bUnsavedChanges = true;
			}
		}

		private void tbQuestion_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempQuestionRow["a_question_" + cbNationSelector.Text.ToLower()] = tbQuestion.Text;

				bUnsavedChanges = true;
			}
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			void Reload()
			{
				MainList.Enabled = false;
				btnReload.Enabled = false;

				nSearchPosition = 0;

				pMain.pTables.OXQuizTable?.Dispose();
				pMain.pTables.OXQuizTable = null;

				pMain.pTables.OXRewardTable?.Dispose();
				pMain.pTables.OXRewardTable = null;

				pMain.pTables.ItemTable?.Dispose();
				pMain.pTables.ItemTable = null;

				btnUpdate.Enabled = false;
				btnUpdateRewards.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				OXEditor_LoadAsync(sender, e);
			}

			var (bProceed, _) = CheckUnsavedChanges();

			if (!bUnsavedChanges /*Prevent show two MessageBox's*/ && bUnsavedChangesRewards)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes (Rewards). Do you want to discard them and exit?", "OX Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (pDialogReturn == DialogResult.No)
					return;
			}

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
				int i, nNewQuestionID = 9999;
				DataRow? pNewRow;

				List<string> listIntColumns = new List<string>
				{
					"a_index"
				};

				List<string> listVarcharColumns = new List<string>
				{
					"a_name",
					"a_answer"
				};

				foreach (string strNation in pMain.pSettings.NationSupported)
					listVarcharColumns.Add("a_question_" + strNation.ToLower());

				if (pMain.pTables.OXQuizTable == null)
				{
					DataTable pOXQuizTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pOXQuizTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listVarcharColumns)
						pOXQuizTableStruct.Columns.Add(strColumnName, typeof(string));

					pNewRow = pOXQuizTableStruct.NewRow();

					pOXQuizTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_oxquiz ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewQuestionID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewQuestionID = pMain.AskForIndex(this.Text, "a_index")) == -1)	// I don't test it...
							return;
					}
				}
				else
				{
					nNewQuestionID = Convert.ToInt32(pMain.pTables.OXQuizTable.Select().LastOrDefault()["a_index"]) + 1;

					pNewRow = pMain.pTables.OXQuizTable?.NewRow();
				}

				List<object> listDefaultValue = new List<object>
				{
					nNewQuestionID,	// a_index
					"New Quiz",		// a_name
					"O"				// a_answer
				};

				foreach (string strNation in pMain.pSettings.NationSupported)
					listDefaultValue.Add("Created with NicolasG LastChaos ToolBox");

				i = 0;

				foreach (string strColumnName in listIntColumns.Concat(listVarcharColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempQuestionRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"OX Editor > Quiz: {nNewQuestionID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "OX Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewQuestionID, "New Quiz", true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Quiz. Because it's temporary.", "OX Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nQuestionIDToCopy = Convert.ToInt32(pTempQuestionRow["a_index"]);
				int nNewQuestionID = Convert.ToInt32(pMain.pTables.OXQuizTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempQuestionRow = pMain.pTables.OXQuizTable.NewRow();
				pTempQuestionRow.ItemArray = (object[])pMain.pTables.OXQuizTable.Select("a_index=" + nQuestionIDToCopy)[0].ItemArray.Clone();

				pTempQuestionRow["a_index"] = nNewQuestionID;
				pTempQuestionRow["a_name"] = pTempQuestionRow["a_name"] + " Copy";

				AddToList(nNewQuestionID, pTempQuestionRow["a_name"].ToString() ?? string.Empty, true);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nQuestionID = Convert.ToInt32(pTempQuestionRow["a_index"]);
			DataRow pOXQuizRow = pMain.pTables.OXQuizTable?.Select("a_index=" + nQuestionID).FirstOrDefault();

			if (pOXQuizRow != null)
			{
				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_oxquiz WHERE a_index={nQuestionID};", out long _)))
				{
					string strError = $"OX Editor > Quiz: {nQuestionID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "OX Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pOXQuizRow != null)
						pMain.pTables.OXQuizTable.Rows.Remove(pOXQuizRow);
				}
				catch (Exception ex)
				{
					string strError = $"OX Editor > Quiz: {nQuestionID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "OX Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Quiz Deleted successfully!", "OX Editor", MessageBoxButtons.OK);

						MainList.SelectedIndex = nPrevObjectID;

						bUnsavedChanges = false;
					}
				}
			}
		}
		/****************************************/
		private void tbSearchRewards_TextChanged(object sender, EventArgs e) { nSearchRewardsPosition = 0; }

		private void tbSearchRewards_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				void Search()
				{
					int i = 0;
					string[] searchTokens = pMain.NormalizeText(tbSearchRewards.Text).Split(' ', StringSplitOptions.RemoveEmptyEntries);

					foreach (DataGridViewRow row in gridRewards.Rows)
					{
						if (pMain.ContainsAllTokens(row.Cells["item"].Value.ToString(), searchTokens) && i > nSearchRewardsPosition)
						{
							gridRewards.FirstDisplayedScrollingRowIndex = row.Index;
							row.Selected = true;
							nSearchRewardsPosition = row.Index;
							return;
						}

						i++;
					}

					for (i = 0; i <= nSearchRewardsPosition; i++)
					{
						if (pMain.ContainsAllTokens(gridRewards.Rows[i].Cells["item"].Value.ToString(), searchTokens))
						{
							gridRewards.FirstDisplayedScrollingRowIndex = gridRewards.Rows[i].Index;
							gridRewards.Rows[i].Selected = true;
							nSearchRewardsPosition = gridRewards.Rows[i].Index;
							return;
						}
					}
				}

				Search();

				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void gridRewards_CellValueChanged(object? sender, DataGridViewCellEventArgs e) {
			if (bUserAction)
			{
				if (Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
				{
					gridRewards.Rows[e.RowIndex].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["amount"].Value));
					gridRewards.Rows[e.RowIndex].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
				}

				bUnsavedChangesRewards = true;
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
							int nRow = gridRewards.Rows.Count;

							gridRewards.Rows.Insert(nRow);

							gridRewards.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

							gridRewards.Rows[nRow].Cells["rewardStage"].Value = 1;
							gridRewards.Rows[nRow].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
							gridRewards.Rows[nRow].Cells["item"].Value = $"{nItemID} - {pItemSelector.ReturnValues[1]}";
							gridRewards.Rows[nRow].Cells["item"].Tag = nItemID;

							long lItemFlag = 0;

							if (nItemID != Defs.NAS_ITEM_DB_INDEX)
							{
								lItemFlag = 1025;	// Hardcode

								StringBuilder strTooltip = new();

								for (int i = 0; i < Defs.ItemFlags.Length; i++)
								{
									if ((lItemFlag & 1L << i) != 0)
										strTooltip.Append(Defs.ItemFlags[i] + "\n");
								}

								gridRewards.Rows[nRow].Cells["flag"].ToolTipText = strTooltip.ToString();
							}

							gridRewards.Rows[nRow].Cells["flag"].Value = lItemFlag;
							gridRewards.Rows[nRow].Cells["amount"].Value = 1;

							if (nItemID == Defs.NAS_ITEM_DB_INDEX)
							{
								gridRewards.Rows[nRow].Cells["amount"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridRewards.Rows[nRow].Cells["amount"].Value));
								gridRewards.Rows[nRow].Cells["amount"].Style.BackColor = Color.FromArgb(166, 166, 166);
							}

							gridRewards.FirstDisplayedScrollingRowIndex = nRow;
							gridRewards.Rows[nRow].Selected = true;

							bUnsavedChangesRewards = true;
						}
					};

					ToolStripMenuItem deleteItem = new("Delete") { Enabled = e.RowIndex >= 0 };
					deleteItem.Click += (_, _) =>
					{
						if (e.RowIndex >= 0)
						{
							gridRewards.SuspendLayout();

							gridRewards.Rows.RemoveAt(e.RowIndex);

							int i = 1;
							foreach (DataGridViewRow row in gridRewards.Rows)
							{
								row.HeaderCell.Value = i.ToString();

								i++;
							}

							gridRewards.ResumeLayout();

							bUnsavedChangesRewards = true;
						}
					};

					cmRewards = new ContextMenuStrip();
					cmRewards.Items.AddRange(addItem, deleteItem);
					cmRewards.Show(Cursor.Position);
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 2 && e.RowIndex >= 0)    // Item Selector
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
					}
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 3 && e.RowIndex >= 0)	// Flag Selector
				{
					if (Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
						return;

					FlagPicker pFlagSelector = new(this, Defs.ItemFlags, Convert.ToInt64(gridRewards.Rows[e.RowIndex].Cells["flag"].Value));
					if (pFlagSelector.ShowDialog() != DialogResult.OK)
						return;

					gridRewards.Rows[e.RowIndex].Cells["flag"].Value = pFlagSelector.ReturnValues;

					StringBuilder strTooltip = new();

					for (int i = 0; i < Defs.ItemFlags.Length; i++)
					{
						if ((pFlagSelector.ReturnValues & 1L << i) != 0)
							strTooltip.Append(Defs.ItemFlags[i] + "\n");
					}

					gridRewards.Rows[e.RowIndex].Cells["flag"].ToolTipText = strTooltip.ToString();

					bUnsavedChangesRewards = true;
				}
			}
		}

		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nQuestID = Convert.ToInt32(pTempQuestionRow["a_index"]);
			StringBuilder strbuilderQuery = new();

			// Check if quiz exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pOXQuizRow = pMain.pTables.OXQuizTable?.Select("a_index=" + nQuestID).FirstOrDefault();
			if (pOXQuizRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_oxquiz SET");

				foreach (DataColumn pCol in pTempQuestionRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempQuestionRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nQuestID};");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempQuestionRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempQuestionRow[pCol].ToString() ?? string.Empty)}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_oxquiz ({strColumnsNames}) VALUES ({strColumnsValues});");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.ToString(), out long _))
			{
				try
				{
					if (pOXQuizRow != null)  // Row exist in Global Table, update it.
					{
						pOXQuizRow.ItemArray = (object[])pTempQuestionRow.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pOXQuizRow = pMain.pTables.OXQuizTable?.NewRow();
						pOXQuizRow.ItemArray = (object[])pTempQuestionRow.ItemArray.Clone();
						pMain.pTables.OXQuizTable?.Rows.Add(pOXQuizRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"OX Editor > Quiz: {nQuestID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "OX Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nQuestID;
						pSelectedItem.Text = nQuestID + " - " + tbName.Text.ToString();

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "OX Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"OX Editor > Quiz: {nQuestID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "OX Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnUpdateRewards_Click(object sender, EventArgs e)
		{
			int i = 0;
			bool bSuccess = true;
			StringBuilder strbuilderQuery = new();

			// Init transaction.
			strbuilderQuery.Append("START TRANSACTION;\n");

			if (gridRewards.Rows.Count > 0)
			{
				pMain.pTables.OXRewardTable.Clear();

				strbuilderQuery.Append($"TRUNCATE TABLE {pMain.pSettings.DBData}.t_oxreward;\n");	// Empty completely OX Rewards table to populate it from GridView content.

				DataRow[] pTempRewardsRows = new DataRow[gridRewards.Rows.Count];

				foreach (DataGridViewRow row in gridRewards.Rows)
				{
					DataRow pRow = pMain.pTables.OXRewardTable.NewRow();

					pRow["a_index"] = i + 1;
					pRow["a_answered_required"] = row.Cells["rewardStage"].Value;
					pRow["a_item_idx"] = row.Cells["item"].Tag;
					pRow["a_item_flag"] = row.Cells["flag"].Value;
					pRow["a_item_amount"] = row.Cells["amount"].Value;

					pTempRewardsRows[i] = pRow;

					i++;
				}

				// Compose t_oxreward INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();
				HashSet<string> addedColumns = new();

				foreach (DataRow pRow in pTempRewardsRows)	// NOTE: I know i can include that part in previous foreach, but is more consistent in this way.
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

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_oxreward ({strColumnsNames}) VALUES {strColumnsValues};\n");

				if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _))
				{
					try
					{
						foreach (DataRow pRow in pTempRewardsRows)
							pMain.pTables.OXRewardTable.Rows.Add(pRow);
					}
					catch (Exception ex)
					{
						string strError = $"OX Editor > Rewards: Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

						pMain.Logger(LogTypes.Error, strError);

						MessageBox.Show(strError, "OX Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

						bSuccess = false;
					}
					finally
					{
						if (bSuccess)
						{
							MessageBox.Show("Rewards changes applied successfully!", "OX Editor", MessageBoxButtons.OK);

							bUnsavedChangesRewards = false;
						}
					}
				}
				else
				{
					string strError = "OX Editor > Rewards: Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "OX Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
