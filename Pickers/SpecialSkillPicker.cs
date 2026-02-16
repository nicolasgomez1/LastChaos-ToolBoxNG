namespace LastChaos_ToolBoxNG {
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Object Array<Actual Special Skill ID, Actual Special Skill Level>
	 *	Boolean<Enable/Disable "Remove Special Skill" Button>
	 * Returns:
	 *	Object Array<Special Skill ID, Special Skill Level, Special Skill Name, Special Skill Description, Texture File, Texture Row, Texture Col>
	// Call and receive implementation
	SpecialSkillPicker pSpecialSkillSelector = new(pMain, this, new object[] { 0, 1 });
	if (pSpecialSkillSelector.ShowDialog() != DialogResult.OK)
		return;

	int nSpecialSkillID = Convert.ToInt32(pSpecialSkillSelector.ReturnValues[0]);
	string strSpecialSkillLevel = pSpecialSkillSelector.ReturnValues[1].ToString();
	string strSpecialSkillName = pSpecialSkillSelector.ReturnValues[2].ToString();
	string strSpecialSkillDescription = pSpecialSkillSelector.ReturnValues[3].ToString();
	string strSpecialSkillTextureID = pSpecialSkillSelector.ReturnValues[4].ToString();
	int nSpecialSkillTextureRow = Convert.ToInt32(pSpecialSkillSelector.ReturnValues[5]);
	int nSpecialSkillTextureCol = Convert.ToInt32(pSpecialSkillSelector.ReturnValues[6]);
	/****************************************/
	public partial class SpecialSkillPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		private DataRow? pRowSpecialSkill;
		public object[] ReturnValues = new object[7];

		public SpecialSkillPicker(Main mainForm, Form ParentForm, object[] objArray, bool bRemoveSkillEnable = true)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = objArray[0];
			ReturnValues[1] = objArray[1];

			btnRemoveSkill.Enabled = bRemoveSkillEnable;
		}

		private void Picker_FormClosing(object sender, FormClosingEventArgs e) { pRowSpecialSkill = null; }

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

		private void SetSpecialSkillDescription(int nLevel)
		{
			rtbInformation.Clear();

			string strSkillDescription = pRowSpecialSkill["a_desc_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty;

			AddInfo("Description:", true);
			AddInfo(strSkillDescription);

			AddInfo("Preference:", true);
			AddInfo(pRowSpecialSkill["a_preference"].ToString() ?? string.Empty);

			AddInfo("Type:", true);

			int nType = Convert.ToInt32(pRowSpecialSkill["a_type"]);

			if (nType < 0 || nType >= Defs.SpecialSkillTypes.Length)
				pMain.Logger(LogTypes.Error, $"Special Skill Picker > Special Skill: {pRowSpecialSkill["a_index"]} Error: a_type out of range.");
			else
				AddInfo(Defs.SpecialSkillTypes[nType]);

			AddInfo("Jobs:", true);

			int nJobFlag = Convert.ToInt32(pRowSpecialSkill["a_job"]);

			foreach (var ClassData in Defs.CharactersClassNJobsTypes)
			{
				if ((nJobFlag & 1L << ClassData.Key) != 0)
					AddInfo(ClassData.Value[0].Substring(4));
			}

			AddInfo("Required Special Skill & Skill Level:", true);

			int nRequiredSpecialSkillID = Convert.ToInt32(pRowSpecialSkill["a_need_sskill"]);

			AddInfo(nRequiredSpecialSkillID != -1 ? $"ID: {pRowSpecialSkill["a_need_sskill"]} LVL: " + pRowSpecialSkill["a_need_sskill_level"] : "NONE");

			ReturnValues[2] = pRowSpecialSkill["a_name_" + pMain.pSettings.WorkLocale];
			ReturnValues[3] = strSkillDescription;

			if (nLevel != -1)
			{
				List<string> listStats = new();
				int nMaxLevel = Convert.ToInt32(pRowSpecialSkill["a_max_level"]);

				for (int i = 0; i < 2; i++)
				{
					AddInfo((i == 0 ? "Level Data\r\nNum0 Num1 Num2 Num3 Num4" : "Num5 Num6 Num7 Num8 Num9"), true);

					listStats.Clear();

					for (int j = 0; j < Defs.SSKILL_MAX_LEVEL; j++)
						listStats.Add(pRowSpecialSkill["a_level" + nLevel + "_num" + (j * i)].ToString() ?? string.Empty);

					AddInfo(string.Join(" ", listStats));
				}
			}
		}

		private async void SkillPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string> {
				"a_name_" + pMain.pSettings.WorkLocale,
				"a_desc_" + pMain.pSettings.WorkLocale,
				"a_texture_id",
				"a_texture_row",
				"a_texture_col",
				"a_max_level",
				"a_job",
				"a_type",
				"a_preference",
				"a_need_sskill",
				"a_need_sskill_level"
			};

			for( int i = 0; i < Defs.SSKILL_MAX_LEVEL; i++ )
			{
				listQueryCompose.AddRange(
					"a_level" + i + "_need_level",
					"a_level" + i + "_need_sp",
					"a_level" + i + "_num0", "a_level" + i + "_num1", "a_level" + i + "_num2", "a_level" + i + "_num3", "a_level" + i + "_num4",
					"a_level" + i + "_num5", "a_level" + i + "_num6", "a_level" + i + "_num7", "a_level" + i + "_num8", "a_level" + i + "_num9"
				);
			}

			if (pMain.pTables.SpecialSkillTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (var column in listQueryCompose.ToList())
				{
					if (!pMain.pTables.SpecialSkillTable.Columns.Contains(column))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(column);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_special_skill ORDER BY a_index;");
				});

				if (pMain.pTables.SpecialSkillTable == null)
					pMain.pTables.SpecialSkillTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.SpecialSkillTable);
			}

			if (pMain.pTables.SpecialSkillTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.SpecialSkillTable.Rows)
				{
					int nSkillID = Convert.ToInt32(pRow["a_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nSkillID,
						Text = pRow["a_index"] + " - " + pRow["a_name_" + pMain.pSettings.WorkLocale]
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

			cbLevelSelector.Enabled = false;
			btnSelect.Enabled = false;

			cbLevelSelector.Items.Clear();
			cbLevelSelector.BeginUpdate();

			int nItemID = pSelectedItem.ID;
			pRowSpecialSkill = pMain.pTables.SpecialSkillTable?.Select("a_index=" + nItemID).FirstOrDefault();

			ReturnValues[4] = pRowSpecialSkill["a_texture_id"];
			ReturnValues[5] = pRowSpecialSkill["a_texture_row"];
			ReturnValues[6] = pRowSpecialSkill["a_texture_col"];

			Image pIcon = pMain.GetIcon("SkillBtn", ReturnValues[4].ToString(), Convert.ToInt32(ReturnValues[5]), Convert.ToInt32(ReturnValues[6]));
			if (pIcon != null)
				pbIcon.Image = pIcon;

			for (int i = 0; i < Convert.ToInt32(pRowSpecialSkill["a_max_level"]); i++)
			{
				cbLevelSelector.Items.Add(i + $" - Need Level: {pRowSpecialSkill["a_level" + 1 + "_need_level"]} Need Skill Points: " + pRowSpecialSkill["a_level" + i + "_need_sp"]);

				if (Convert.ToInt32(ReturnValues[1]) == i)
					cbLevelSelector.SelectedIndex = cbLevelSelector.Items.Count - 1;
			}

			if (cbLevelSelector.SelectedIndex == -1)
				cbLevelSelector.SelectedIndex = 0;

			cbLevelSelector.EndUpdate();

			cbLevelSelector.Enabled = true;
			btnSelect.Enabled = true;

			ReturnValues[1] = -1;
		}

		private void cbLevelSelector_SelectedIndexChanged(object sender, EventArgs e) { SetSpecialSkillDescription(cbLevelSelector.SelectedIndex); }

		private void btnSelect_Click(object sender, EventArgs e)
		{
			Main.ListBoxItem? pSelectedItem = (Main.ListBoxItem?)MainList.SelectedItem;
			int nSelectedSkillLevel = cbLevelSelector.SelectedIndex;

			if (pSelectedItem != null && nSelectedSkillLevel != -1)
			{
				DialogResult = DialogResult.OK;

				ReturnValues[0] = pSelectedItem.ID;
				ReturnValues[1] = nSelectedSkillLevel;

				Close();
			}
		}

		private void btnRemoveSkill_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = new object[] { -1, 0, "", "", "", "", "" };

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
