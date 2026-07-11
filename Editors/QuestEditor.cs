using System.Data;
using System.Text;

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

		private static readonly (int Value, string Text)[] QuestKindOptions =
		[
			(-1, "None"),
			(0, "Repeat"),
			(1, "Collection"),
			(2, "Delivery"),
			(3, "Defeat"),
			(4, "Save"),
			(5, "Mining Experience"),
			(6, "Gathering Experience"),
			(7, "Charge Experience"),
			(8, "Process Experience"),
			(9, "Make Experience"),
			(10, "Tutorial"),
			(11, "PK"),
			(12, "Search")
		];

		private static readonly (int Value, string Text)[] QuestRepeatOptions =
		[
			(-1, "None"),
			(0, "Once"),
			(1, "Unlimited"),
			(2, "Daily")
		];

		private static readonly (int Value, string Text)[] QuestStartOptions =
		[
			(-1, "None"),
			(0, "NPC"),
			(1, "Item"),
			(2, "Level"),
			(3, "Area")
		];

		private static readonly (int Value, string Text)[] QuestJobOptions =
		[
			(-1, "Any"),
			(0, "Titan"),
			(1, "Knight"),
			(2, "Healer"),
			(3, "Mage"),
			(4, "Rogue"),
			(5, "Sorcerer"),
			(6, "NightShadow")
		];

		public QuestEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			WireManualEvents();
		}

		private void WireManualEvents()
		{
			cbEnable.CheckedChanged += (_, _) => SetIntColumnFromCheckBox("a_enable", cbEnable);
			cbType1.SelectedIndexChanged += (_, _) => SetIntColumnFromComboBox("a_type2", cbType1);
			cbType2.SelectedIndexChanged += (_, _) => SetIntColumnFromComboBox("a_start_type", cbType2);
			cbJob.SelectedIndexChanged += (_, _) => SetIntColumnFromComboBox("a_need_job", cbJob);
			tbMaxLevel.TextChanged += (_, _) => SetIntColumnFromTextBox("a_need_max_level", tbMaxLevel);
			tbNeededExperience.TextChanged += (_, _) => SetIntColumnFromTextBox("a_need_exp", tbNeededExperience);
			tbStartDescription.TextChanged += (_, _) => SetStringColumnFromTextBox(LocaleColumn("a_desc"), tbStartDescription);
			tbRewardDescription.TextChanged += (_, _) => SetStringColumnFromTextBox(LocaleColumn("a_desc2"), tbRewardDescription);
			tbConditionDescription.TextChanged += (_, _) => SetStringColumnFromTextBox(LocaleColumn("a_desc3"), tbConditionDescription);
		}

		private void ConfigureControls()
		{
			label3.Text = "Type";
			label5.Text = "Repeat";
			label31.Text = "Start Type";
			groupBox5.Text = "Routing";
			label9.Text = "Start Data";
			label10.Text = "Prize NPC";
			label11.Text = "Prequest";

			cbType0.ForeColor = Color.FromArgb(208, 203, 148);
			cbType1.ForeColor = Color.FromArgb(208, 203, 148);
			cbType2.ForeColor = Color.FromArgb(208, 203, 148);

			cbJob.Visible = true;
			label7.Visible = true;

			PopulateComboBox(cbType0, QuestKindOptions);
			PopulateComboBox(cbType1, QuestRepeatOptions);
			PopulateComboBox(cbType2, QuestStartOptions);
			PopulateComboBox(cbJob, QuestJobOptions);

			(new ToolTip()).SetToolTip(btnReload, "Reload Quest data from Database");
		}

		private async Task LoadQuestDataAsync()
		{
			DataTable? pNewTable = await Task.Run(() =>
			{
				return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT * FROM {pMain.pSettings.DBData}.t_quest ORDER BY a_index;");
			});

			pMain.pTables.QuestTable?.Dispose();
			pMain.pTables.QuestTable = pNewTable;
		}

		private async void RareOptionEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");

			try
			{
				bUserAction = false;
				MainList.Enabled = false;
				btnReload.Enabled = false;
				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;
				btnUpdate.Enabled = false;

				SetupNationSelector();
				ConfigureControls();

				await LoadQuestDataAsync();

				MainList.BeginUpdate();
				MainList.Items.Clear();

				if (pMain.pTables.QuestTable != null)
				{
					foreach (DataRow pRow in pMain.pTables.QuestTable.Rows)
						AddToList(Convert.ToInt32(pRow["a_index"]), GetQuestName(pRow), false);

					if (MainList.Items.Count > 0)
						MainList.SelectedIndex = 0;
				}

				MainList.EndUpdate();

				MainList.Enabled = true;
				btnReload.Enabled = true;
				btnAddNew.Enabled = true;

				MainList.Focus();
			}
			catch (Exception ex)
			{
				string strError = $"Quest Editor > Loading failed: {ex.Message}";
				pMain.Logger(LogTypes.Error, strError);
				MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				pProgressDialog.Close();
			}
		}

		private void SetupNationSelector()
		{
			if (cbNationSelector.Items.Count > 0)
				return;

			cbNationSelector.BeginUpdate();

			for (int i = 0; i < pMain.pSettings.NationSupported.Length; i++)
			{
				string strNation = pMain.pSettings.NationSupported[i];
				cbNationSelector.Items.Add(strNation);

				if (strNation.ToLower() == pMain.pSettings.WorkLocale)
					cbNationSelector.SelectedIndex = i;
			}

			if (cbNationSelector.SelectedIndex < 0 && cbNationSelector.Items.Count > 0)
				cbNationSelector.SelectedIndex = 0;

			cbNationSelector.EndUpdate();
		}

		private void PopulateComboBox(ComboBox comboBox, IEnumerable<(int Value, string Text)> values)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Clear();

			foreach ((int nValue, string strText) in values)
			{
				comboBox.Items.Add(new Main.ComboBoxItem
				{
					Value = nValue,
					DisplayText = $"{nValue} - {strText}"
				});
			}

			comboBox.EndUpdate();
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

		private string LocaleColumn(string strBaseName)
		{
			string strLocale = cbNationSelector.SelectedItem?.ToString()?.ToLower() ?? pMain.pSettings.WorkLocale;
			string strColumnName = $"{strBaseName}_{strLocale}";

			if (pTempQuestRow?.Table.Columns.Contains(strColumnName) == true)
				return strColumnName;

			if (pTempQuestRow?.Table.Columns.Contains(strBaseName) == true)
				return strBaseName;

			return strColumnName;
		}

		private string GetQuestName(DataRow pRow)
		{
			string strColumnName = "a_name_" + pMain.pSettings.WorkLocale;
			if (!pRow.Table.Columns.Contains(strColumnName))
				strColumnName = "a_name";

			return pRow[strColumnName].ToString() ?? string.Empty;
		}

		private void LoadUIData(int nQuestID, bool bLoadFromQuestTable)
		{
			if (bLoadFromQuestTable && pMain.pTables.QuestTable != null)
			{
				DataRow? pSourceRow = pMain.pTables.QuestTable.Select("a_index=" + nQuestID).FirstOrDefault();
				if (pSourceRow == null)
					return;

				pTempQuestRow = pMain.pTables.QuestTable.NewRow();
				pTempQuestRow.ItemArray = (object[])pSourceRow.ItemArray.Clone();
			}

			if (pTempQuestRow == null)
				return;

			bUserAction = false;

			tbID.Text = nQuestID.ToString();
			cbEnable.Checked = GetInt("a_enable") != 0;
			SelectComboValue(cbType0, GetInt("a_type1"));
			SelectComboValue(cbType1, GetInt("a_type2"));
			SelectComboValue(cbType2, GetInt("a_start_type"));
			SelectComboValue(cbJob, GetInt("a_need_job"));
			tbMinLevel.Text = GetString("a_need_min_level");
			tbMaxLevel.Text = GetString("a_need_max_level");
			tbNeededExperience.Text = GetString("a_need_exp");
			tbDefense.Text = GetString("a_start_data");
			tbMagicAttack.Text = GetString("a_prize_npc");
			tbResistance.Text = GetString("a_prequest_num");
			tbName.Text = GetString(LocaleColumn("a_name"));
			tbStartDescription.Text = GetString(LocaleColumn("a_desc"));
			tbRewardDescription.Text = GetString(LocaleColumn("a_desc2"));
			tbConditionDescription.Text = GetString(LocaleColumn("a_desc3"));

			bUserAction = true;

			btnUpdate.Enabled = true;
			btnCopy.Enabled = true;
			btnDelete.Enabled = true;
		}

		private string GetString(string strColumnName)
		{
			if (pTempQuestRow?.Table.Columns.Contains(strColumnName) != true)
				return string.Empty;

			return pTempQuestRow[strColumnName].ToString() ?? string.Empty;
		}

		private int GetInt(string strColumnName)
		{
			if (pTempQuestRow?.Table.Columns.Contains(strColumnName) != true)
				return 0;

			return int.TryParse(pTempQuestRow[strColumnName].ToString(), out int nValue) ? nValue : 0;
		}

		private void SelectComboValue(ComboBox comboBox, int nValue)
		{
			for (int i = 0; i < comboBox.Items.Count; i++)
			{
				if (comboBox.Items[i] is Main.ComboBoxItem pItem && pItem.Value == nValue)
				{
					comboBox.SelectedIndex = i;
					return;
				}
			}

			comboBox.Items.Add(new Main.ComboBoxItem
			{
				Value = nValue,
				DisplayText = $"{nValue} - Custom"
			});
			comboBox.SelectedIndex = comboBox.Items.Count - 1;
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges && pTempQuestRow != null)
			{
				DataRow? pQuestRow = pMain.pTables.QuestTable?.Select("a_index=" + pTempQuestRow["a_index"]).FirstOrDefault();

				if (pQuestRow != null)
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Quest Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Quest is temporary. If you do not press Update, it will be lost.\nDo you want to continue?", "Quest Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
					else
						bDeleteActual = true;
				}
			}

			return (bProceed, bDeleteActual);
		}

		private void RareOptionEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Quest Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (pDialogReturn == DialogResult.No)
					e.Cancel = true;
			}
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

					if (MainList.Items.Count == 0)
						return;

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

			pLastSelected = (Main.ListBoxItem?)MainList.SelectedItem;
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			var (bProceed, _) = CheckUnsavedChanges();
			if (!bProceed)
				return;

			bUnsavedChanges = false;
			RareOptionEditor_LoadAsync(sender, e);
		}

		private void btnAddNew_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();
			if (!bProceed || pMain.pTables.QuestTable == null)
				return;

			int nNewQuestID = GetNextQuestID();
			DataRow pNewRow = pMain.pTables.QuestTable.NewRow();

			foreach (DataColumn pColumn in pMain.pTables.QuestTable.Columns)
				pNewRow[pColumn.ColumnName] = GetDefaultValue(pColumn, nNewQuestID);

			pTempQuestRow = pNewRow;

			if (bDeleteActual && MainList.SelectedIndex >= 0)
				MainList.Items.RemoveAt(MainList.SelectedIndex);

			AddToList(nNewQuestID, "New Quest", true);
		}

		private int GetNextQuestID()
		{
			if (pMain.pTables.QuestTable?.Rows.Count > 0)
				return pMain.pTables.QuestTable.AsEnumerable().Max(row => Convert.ToInt32(row["a_index"])) + 1;

			DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_quest ORDER BY a_index DESC LIMIT 1;");
			if (QueryReturn != null && QueryReturn.Rows.Count > 0)
				return Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;

			return 1;
		}

		private object GetDefaultValue(DataColumn pColumn, int nNewQuestID)
		{
			string strColumnName = pColumn.ColumnName;

			if (strColumnName == "a_index")
				return nNewQuestID;

			if (strColumnName.StartsWith("a_name"))
				return "New Quest";

			if (strColumnName.StartsWith("a_desc") || strColumnName.StartsWith("a_start_give"))
				return string.Empty;

			if (pColumn.DataType == typeof(string))
				return string.Empty;

			if (strColumnName.Contains("type") || strColumnName.Contains("npc") || strColumnName.Contains("item") || strColumnName == "a_need_job" || strColumnName == "a_failvalue")
				return CanStoreNegative(pColumn.DataType) ? -1 : 0;

			if (strColumnName == "a_need_min_level")
				return 1;

			if (strColumnName == "a_need_max_level")
				return 999;

			return 0;
		}

		private bool CanStoreNegative(Type pType)
		{
			return pType == typeof(sbyte)
				|| pType == typeof(short)
				|| pType == typeof(int)
				|| pType == typeof(long)
				|| pType == typeof(float)
				|| pType == typeof(double)
				|| pType == typeof(decimal);
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Quest because it is temporary.", "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (!bProceed || pTempQuestRow == null || pMain.pTables.QuestTable == null)
				return;

			int nNewQuestID = GetNextQuestID();
			DataRow pNewRow = pMain.pTables.QuestTable.NewRow();
			pNewRow.ItemArray = (object[])pTempQuestRow.ItemArray.Clone();
			pNewRow["a_index"] = nNewQuestID;

			foreach (string strNation in pMain.pSettings.NationSupported)
			{
				string strColumnName = "a_name_" + strNation.ToLower();
				if (pNewRow.Table.Columns.Contains(strColumnName))
					pNewRow[strColumnName] = pNewRow[strColumnName] + " Copy";
			}

			if (pNewRow.Table.Columns.Contains("a_name"))
				pNewRow["a_name"] = pNewRow["a_name"] + " Copy";

			pTempQuestRow = pNewRow;
			AddToList(nNewQuestID, GetQuestName(pNewRow), true);
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (pTempQuestRow == null)
				return;

			int nQuestID = Convert.ToInt32(pTempQuestRow["a_index"]);
			DataRow? pQuestTableRow = pMain.pTables.QuestTable?.Select("a_index=" + nQuestID).FirstOrDefault();

			if (MessageBox.Show($"Delete quest {nQuestID}?", "Quest Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;

			bool bSuccess = true;
			if (pQuestTableRow != null)
			{
				bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_quest WHERE a_index={nQuestID};", out long _);
				if (!bSuccess)
				{
					string strError = $"Quest Editor > Quest: {nQuestID} Something went wrong while deleting from the database.";
					pMain.Logger(LogTypes.Error, strError);
					MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (!bSuccess)
				return;

			if (pQuestTableRow != null)
				pMain.pTables.QuestTable?.Rows.Remove(pQuestTableRow);

			int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;
			MainList.Items.Remove(MainList.SelectedItem);

			if (MainList.Items.Count > 0)
				MainList.SelectedIndex = Math.Min(nPrevObjectID, MainList.Items.Count - 1);

			bUnsavedChanges = false;
			MessageBox.Show("Quest deleted successfully.", "Quest Editor", MessageBoxButtons.OK);
		}

		private void cbNationSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction && pTempQuestRow != null)
			{
				bUserAction = false;
				tbName.Text = GetString(LocaleColumn("a_name"));
				tbStartDescription.Text = GetString(LocaleColumn("a_desc"));
				tbRewardDescription.Text = GetString(LocaleColumn("a_desc2"));
				tbConditionDescription.Text = GetString(LocaleColumn("a_desc3"));
				bUserAction = true;
			}
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			SetStringColumnFromTextBox(LocaleColumn("a_name"), tbName);
		}

		private void cbGradeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetIntColumnFromComboBox("a_type1", cbType0);
		}

		private void tbAttack_TextChanged(object sender, EventArgs e)
		{
			SetIntColumnFromTextBox("a_need_min_level", tbMinLevel);
		}

		private void tbDefense_TextChanged(object sender, EventArgs e)
		{
			SetIntColumnFromTextBox("a_start_data", tbDefense);
		}

		private void tbMagicAttack_TextChanged(object sender, EventArgs e)
		{
			SetIntColumnFromTextBox("a_prize_npc", tbMagicAttack);
		}

		private void tbResistance_TextChanged(object sender, EventArgs e)
		{
			SetIntColumnFromTextBox("a_prequest_num", tbResistance);
		}

		private void SetIntColumnFromCheckBox(string strColumnName, CheckBox checkBox)
		{
			if (!bUserAction || pTempQuestRow?.Table.Columns.Contains(strColumnName) != true)
				return;

			pTempQuestRow[strColumnName] = checkBox.Checked ? 1 : 0;
			bUnsavedChanges = true;
		}

		private void SetIntColumnFromComboBox(string strColumnName, ComboBox comboBox)
		{
			if (!bUserAction || pTempQuestRow?.Table.Columns.Contains(strColumnName) != true || comboBox.SelectedItem is not Main.ComboBoxItem pItem)
				return;

			pTempQuestRow[strColumnName] = pItem.Value;
			bUnsavedChanges = true;
		}

		private void SetIntColumnFromTextBox(string strColumnName, TextBox textBox)
		{
			if (!bUserAction || pTempQuestRow?.Table.Columns.Contains(strColumnName) != true)
				return;

			if (int.TryParse(textBox.Text, out int nValue))
			{
				pTempQuestRow[strColumnName] = nValue;
				bUnsavedChanges = true;
			}
		}

		private void SetStringColumnFromTextBox(string strColumnName, TextBox textBox)
		{
			if (!bUserAction || pTempQuestRow?.Table.Columns.Contains(strColumnName) != true)
				return;

			pTempQuestRow[strColumnName] = textBox.Text;
			bUnsavedChanges = true;
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			if (pTempQuestRow == null)
				return;

			bool bSuccess = true;
			int nQuestID = Convert.ToInt32(pTempQuestRow["a_index"]);
			StringBuilder strbuilderQuery = new();

			DataRow? pQuestTableRow = pMain.pTables.QuestTable?.Select("a_index=" + nQuestID).FirstOrDefault();
			if (pQuestTableRow != null)
			{
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_quest SET");

				foreach (DataColumn pCol in pTempQuestRow.Table.Columns)
				{
					if (pCol.ColumnName == "a_index")
						continue;

					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempQuestRow[pCol].ToString() ?? string.Empty)}',");
				}

				strbuilderQuery.Length -= 1;
				strbuilderQuery.Append($" WHERE a_index={nQuestID};");
			}
			else
			{
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempQuestRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");
					strColumnsValues.Append($"'{pMain.EscapeChars(pTempQuestRow[pCol].ToString() ?? string.Empty)}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_quest ({strColumnsNames}) VALUES ({strColumnsValues});");
			}

			if (!pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.ToString(), out long _))
			{
				string strError = $"Quest Editor > Quest: {nQuestID} Something went wrong while updating the database.";
				pMain.Logger(LogTypes.Error, strError);
				MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
			{
				if (pQuestTableRow != null)
				{
					pQuestTableRow.ItemArray = (object[])pTempQuestRow.ItemArray.Clone();
				}
				else if (pMain.pTables.QuestTable != null)
				{
					pQuestTableRow = pMain.pTables.QuestTable.NewRow();
					pQuestTableRow.ItemArray = (object[])pTempQuestRow.ItemArray.Clone();
					pMain.pTables.QuestTable.Rows.Add(pQuestTableRow);
				}
			}
			catch (Exception ex)
			{
				string strError = $"Quest Editor > Quest: {nQuestID} Changes applied in database, but updating the local table failed. Please restart the application ({ex.Message}).";
				pMain.Logger(LogTypes.Error, strError);
				MessageBox.Show(strError, "Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				bSuccess = false;
			}

			if (!bSuccess)
				return;

			int nSelectedIndex = MainList.SelectedIndex;
			if (nSelectedIndex >= 0)
			{
				Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
				pSelectedItem.ID = nQuestID;
				pSelectedItem.Text = nQuestID + " - " + tbName.Text;

				MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
				MainList.Items[nSelectedIndex] = pSelectedItem;
				MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
			}

			bUnsavedChanges = false;
			MessageBox.Show("Changes applied successfully.", "Quest Editor", MessageBoxButtons.OK);
		}
	}
}
