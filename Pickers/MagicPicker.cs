//#define ENABLE_ATTRIBUTE	// NOTE: Isn't used anymore (Was it really used sometime?)
//#define ENABLE_TOGGLE	// NOTE: Isn't used anymore

namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int Array<Actual Magic ID, Actual Magic Level>
	 * Returns:
	 *	Int Array<Magic Type, Magic Level>
	// Call and receive implementation
	MagicPicker pMagicSelector = new(pMain, this, new int[] { 0, 1 });
	if (pMagicSelector.ShowDialog() != DialogResult.OK)
		return;

	int nMagicType = pMagicSelector.ReturnValues[0];
	int nMagicLevel = pMagicSelector.ReturnValues[1];
	/****************************************/
	public partial class MagicPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		private DataRow? pRowMagic;
		private DataRow[]? listMagicLevels;
		public int[] ReturnValues = { -1, 0 };

		public MagicPicker(Main mainForm, Form ParentForm, int[] nArray)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues = nArray;
		}

		private void Picker_FormClosing(object sender, FormClosingEventArgs e)
		{
			pRowMagic = null;
			listMagicLevels = null;
		}

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

		private async void MagicPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_name",
				"a_maxlevel",
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
				foreach (var column in listQueryCompose.ToList())
				{
					if (!pMain.pTables.MagicTable.Columns.Contains(column))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(column);
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

				bRequestNeeded = false;
				listQueryCompose.Clear();

				listQueryCompose = ["a_level", "a_power", "a_hitrate"];

				if (pMain.pTables.MagicLevelTable == null)
				{
					bRequestNeeded = true;
				}
				else
				{
					foreach (var column in listQueryCompose.ToList())
					{
						if (!pMain.pTables.MagicLevelTable.Columns.Contains(column))
							bRequestNeeded = true;
						else
							listQueryCompose.Remove(column);
					}
				}

				if (bRequestNeeded)
				{
					pNewTable = await Task.Run(() =>
					{
						return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_magiclevel ORDER BY a_level;");
					});

					if (pMain.pTables.MagicLevelTable == null)
						pMain.pTables.MagicLevelTable = pNewTable;
					else
						pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.MagicLevelTable);
				}
			}

			if (pMain.pTables.MagicTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.MagicTable.Rows)
				{
					int nSkillID = Convert.ToInt32(pRow["a_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nSkillID,
						Text = $"{pRow["a_index"]} - {pRow["a_name"]}"
					});

					if (nSkillID == Convert.ToInt32(ReturnValues[0]))
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

			pRowMagic = pMain.pTables.MagicTable?.Select("a_index=" + nItemID).FirstOrDefault();

			AddInfo("Type:", true);

			var varType = Defs.MagicTypesAndSubTypes.ElementAtOrDefault(Convert.ToInt32(pRowMagic["a_type"]));
			if (varType.Value == null)
			{
				pMain.Logger(LogTypes.Error, $"Magic Picker > Magic: {nItemID} Error: a_type out of range.");
				return;
			}

			AddInfo(varType.Key);

			AddInfo("Sub Type:", true);

			if (varType.Value.Count < Convert.ToInt32(pRowMagic["a_subtype"]))
			{
				pMain.Logger(LogTypes.Error, $"Magic Picker > Magic: {nItemID} Error: a_subtype out of range.");
				return;
			}

			AddInfo(varType.Value[Convert.ToInt32(pRowMagic["a_subtype"])]);

			AddInfo("Damage Type:", true);

			string strDamageType = Defs.MagicDamageTypes[Convert.ToInt32(pRowMagic["a_damagetype"])];

			if (strDamageType == null)
			{
				pMain.Logger(LogTypes.Error, $"Magic Picker > Magic: {nItemID} Error: a_damagetype out of range.");
				return;
			}

			AddInfo(strDamageType);

			AddInfo("Hit Type:", true);

			string strHitType = Defs.MagicHitTypes[Convert.ToInt32(pRowMagic["a_hittype"])];

			if (strHitType == null)
			{
				pMain.Logger(LogTypes.Error, $"Magic Picker > Magic: {nItemID} Error: a_hittype out of range.");
				return;
			}

			AddInfo(strHitType);
#if ENABLE_ATTRIBUTE
			AddInfo("Attribute:", true);
			AddInfo(pRowMagic["a_attribute"].ToString());
#endif
			AddInfo("Self Power:", true);

			string strParam = Defs.MagicParamTypes[Convert.ToInt32(pRowMagic["a_psp"])];

			if (strParam == null)
			{
				pMain.Logger(LogTypes.Error, $"Magic Picker > Magic: {nItemID} Error: a_psp out of range.");
				return;
			}

			AddInfo(strParam);

			AddInfo("Target Power:", true);

			strParam = Defs.MagicParamTypes[Convert.ToInt32(pRowMagic["a_ptp"])];

			if (strParam == null)
			{
				pMain.Logger(LogTypes.Error, $"Magic Picker > Magic: {nItemID} Error: a_ptp out of range.");
				return;
			}

			AddInfo(strParam);

			AddInfo("Self Hit Rate:", true);

			strParam = Defs.MagicParamTypes[Convert.ToInt32(pRowMagic["a_hsp"])];

			if (strParam == null)
			{
				pMain.Logger(LogTypes.Error, $"Magic Picker > Magic: {nItemID} Error: a_hsp out of range.");
				return;
			}

			AddInfo(strParam);

			AddInfo("Target Hit Rate:", true);

			strParam = Defs.MagicParamTypes[Convert.ToInt32(pRowMagic["a_htp"])];

			if (strParam == null)
			{
				pMain.Logger(LogTypes.Error, $"Magic Picker > Magic: {nItemID} Error: a_htp out of range.");
				return;
			}

			AddInfo(strParam);
#if ENABLE_TOGGLE
			AddInfo("Toggle:", true);

			string strToggle = "False";

			if (pRowMagic["a_togle"].ToString() == "1")
				strToggle = "True";

			AddInfo(strToggle);
#endif
			listMagicLevels = pMain.pTables.MagicLevelTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).ToArray();

			foreach (var RowMagicLevel in listMagicLevels)
			{
				int nLevel = Convert.ToInt32(RowMagicLevel["a_level"]);

				cbLevelSelector.Items.Add($"Lvl: {nLevel} Power: {RowMagicLevel["a_power"]} Hit Rate: " + RowMagicLevel["a_hitrate"]);

				if (nLevel == ReturnValues[1])
					cbLevelSelector.SelectedIndex = cbLevelSelector.Items.Count - 1;
			}

			if (cbLevelSelector.SelectedIndex == -1)
				cbLevelSelector.SelectedIndex = 0;

			cbLevelSelector.Enabled = true;
			btnSelect.Enabled = true;

			ReturnValues[1] = -1;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			Main.ListBoxItem? pSelectedItem = (Main.ListBoxItem?)MainList.SelectedItem;
			int nSelectedMagicLevel = cbLevelSelector.SelectedIndex;

			if (pSelectedItem != null && nSelectedMagicLevel != -1)
			{
				DialogResult = DialogResult.OK;

				ReturnValues[0] = Convert.ToInt32(pRowMagic["a_index"]);
				ReturnValues[1] = Convert.ToInt32(listMagicLevels[nSelectedMagicLevel]["a_level"]);

				Close();
			}
		}

		private void btnRemoveMagic_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = [-1, 0];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
