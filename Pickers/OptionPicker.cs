namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int Array<Actual Option ID, Actual Option Level>
	 * Returns:
	 *	Object Array<Option Type, Option Level, Option Name, Option Levels List>
	// Call and receive implementation
	OptionPicker pOptionSelector = new(pMain, this, new int[] { 0, 1 });
	if (pOptionSelector.ShowDialog() != DialogResult.OK)
		return;

	int nOptionType = pOptionSelector.ReturnValues[0];
	int nOptionLevel = pOptionSelector.ReturnValues[1];
	string strOptionName = pOptionSelector.ReturnValues[2].ToString();
	string[] strLevels = pOptionSelector.ReturnValues[3].ToString().Split(',');
	/****************************************/
	public partial class OptionPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		private DataRow? pRowOption;
		private string[]? strValues;
		public object[] ReturnValues = { -1, 0, "", new string[] { "" } };

		public OptionPicker(Main mainForm, Form ParentForm, int[] nArray)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = nArray[0];
			ReturnValues[1] = nArray[1];
		}

		private void Picker_FormClosing(object sender, FormClosingEventArgs e) { pRowOption = null; }

		private void AddInfo(string strText, bool bHead = false)
		{
			rtbInformation.BeginInvoke((MethodInvoker)delegate
			{
				int nStartPos = rtbInformation.TextLength;
				rtbInformation.AppendText(strText.ToString() + Environment.NewLine);
				int nEndPos = rtbInformation.TextLength;

				rtbInformation.Select(nStartPos, nEndPos - nStartPos);
				rtbInformation.SelectionColor = bHead ? Color.FromArgb(100, 100, 100) : Color.FromArgb(208, 203, 148);
				rtbInformation.SelectionLength = 0;

				rtbInformation.ScrollToCaret();
			});
		}

		private async void OptionPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_type",
				"a_level",
				"a_prob",
				"a_weapon_type",
				"a_wear_type",
				"a_accessory_type",
				"a_name_" + pMain.pSettings.WorkLocale
			];

			if (pMain.pTables.OptionTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (var column in listQueryCompose.ToList())
				{
					if (!pMain.pTables.OptionTable.Columns.Contains(column))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(column);
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

			if (pMain.pTables.OptionTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.OptionTable.Rows)
				{
					int nItemID = Convert.ToInt32(pRow["a_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nItemID,
						Text = $"{pRow["a_type"]} - {pRow["a_name_" + pMain.pSettings.WorkLocale]}"
					});

					if (Convert.ToInt32(pRow["a_type"]) == Convert.ToInt32(ReturnValues[0]))
						MainList.SelectedIndex = MainList.Items.Count - 1;
				}

				if (MainList.SelectedIndex == -1)
					MainList.SelectedIndex = 0;

				MainList.EndUpdate();

				ReturnValues[0] = -1;
			}
		}

		private void tbSearch_KeyDown(object sender, KeyEventArgs e) { nSearchPosition = pMain.SearchInListBox(tbSearch, e, MainList, nSearchPosition); }

		private void MainList_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
				return;

			rtbInformation.Clear();

			cbLevelSelector.Items.Clear();
			cbLevelSelector.Enabled = false;

			btnSelect.Enabled = false;

			int nItemID = pSelectedItem.ID;

			pRowOption = pMain.pTables.OptionTable?.Select("a_index=" + nItemID).FirstOrDefault();

			AddInfo("Type:", true);

			AddInfo(Defs.OptionTypes[Convert.ToInt32(pRowOption["a_type"])]);

			AddInfo("Weapon Types:", true);
			// NOTE: I'm not sure of all this >>
			int i, nFlag = Convert.ToInt32(pRowOption["a_weapon_type"]);

			if (nFlag != 0)
			{
				i = 0;
				foreach (string strSubType in Defs.ItemTypesNSubTypes["Weapon"])
				{
					if ((nFlag & 1L << i) != 0)
						AddInfo(strSubType);

					i++;
				}
			}
			else
			{
				AddInfo("0");
			}

			AddInfo("Wear Types:", true);

			nFlag = Convert.ToInt32(pRowOption["a_wear_type"]);
			if (nFlag != 0)
			{
				i = 0;
				foreach (string strSubType in Defs.ItemTypesNSubTypes["Armor"])
				{
					if ((nFlag & 1L << i) != 0)
						AddInfo(strSubType);

					i++;
				}
			}
			else
			{
				AddInfo("0");
			}

			AddInfo("Accesory Types:", true);

			nFlag = Convert.ToInt32(pRowOption["a_accessory_type"]);
			if (nFlag != 0)
			{
				i = 0;
				foreach (string strSubType in Defs.ItemTypesNSubTypes["Accesory"])
				{
					if ((nFlag & 1L << i) != 0)
						AddInfo(strSubType);

					i++;
				}
			}
			else
			{
				AddInfo("0");
			}
			// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			strValues = pRowOption["a_level"].ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

			string[] strProbs = pRowOption["a_prob"].ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

			ReturnValues[2] = pRowOption["a_name_" + pMain.pSettings.WorkLocale];
			ReturnValues[3] = string.Join(",", strValues.Select((level, index) => index < strProbs.Length ? $"{index + 1} - Value: {level} Prob: {strProbs[index]}" : null).Where(x => x != null));

			i = 0;
			foreach (string strValue in strValues)
			{
				if (i < strProbs.Length)
				{
					cbLevelSelector.Items.Add($"{(i + 1)} - Value: {strValue} Prob: {strProbs[i]}");

					if ((i + 1) == Convert.ToInt32(ReturnValues[1]))
						cbLevelSelector.SelectedIndex = cbLevelSelector.Items.Count - 1;
				}
				else
				{
					pMain.Logger(LogTypes.Error, $"Option Picker > Option: {nItemID} Error: a_prob mismatched with a_level.");
				}

				i++;
			}

			if (cbLevelSelector.SelectedIndex == -1)
				cbLevelSelector.SelectedIndex = 0;

			cbLevelSelector.Enabled = true;
			btnSelect.Enabled = true;

			ReturnValues[1] = 0;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			Main.ListBoxItem? pSelectedItem = (Main.ListBoxItem?)MainList.SelectedItem;
			int nSelectedOptionLevel = cbLevelSelector.SelectedIndex;

			if (pSelectedItem != null && nSelectedOptionLevel != -1)
			{
				DialogResult = DialogResult.OK;

				ReturnValues[0] = Convert.ToInt32(pRowOption["a_type"]);
				ReturnValues[1] = nSelectedOptionLevel + 1;

				Close();
			}
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = [-1, 0, "NONE", new string[] { }];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
