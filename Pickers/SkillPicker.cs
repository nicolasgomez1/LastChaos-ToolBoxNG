namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Object Array<Actual Skill ID, Actual Skill Level>
	 *	Boolean<Enable/Disable "Remove Skill" Button>
	 * Returns:
	 *	Object Array<Skill ID, Skill Level, Skill Name, Skill Description, Texture File, Texture Row, Texture Col, Skill Levels List>
	// Call and receive implementation
	SkillPicker pSkillSelector = new(pMain, this, [0, 1], true);
	if (pSkillSelector.ShowDialog() != DialogResult.OK)
		return;

	int nSkillID = Convert.ToInt32(pSkillSelector.ReturnValues[0]);
	string strSkillLevel = pSkillSelector.ReturnValues[1].ToString();
	string strSkillName = pSkillSelector.ReturnValues[2].ToString();
	string strSkillDescription = pSkillSelector.ReturnValues[3].ToString();
	string strSkillTextureID = pSkillSelector.ReturnValues[4].ToString();
	int nSkillTextureRow = Convert.ToInt32(pSkillSelector.ReturnValues[5]);
	int nSkillTextureCol = Convert.ToInt32(pSkillSelector.ReturnValues[6]);
	string[] strLevels = pSkillSelector.ReturnValues[7].ToString().Split(',');
	/****************************************/
	public partial class SkillPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		public object[] ReturnValues = new object[8];

		public SkillPicker(Main mainForm, Form ParentForm, object[] objArray, bool bRemoveSkillEnable = true)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = objArray[0];
			ReturnValues[1] = objArray[1];

			btnRemoveSkill.Enabled = bRemoveSkillEnable;
		}

		private async void SkillPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			await Task.WhenAll(
				pMain.GenericLoadSkillDataAsync(),
				pMain.GenericLoadSkillLevelDataAsync()
			);

			if (pMain.pTables.SkillTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.SkillTable.Rows)
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

			DataRow pRowSkill = pMain.pTables.SkillTable?.Select("a_index=" + nItemID).FirstOrDefault();

			Image pIcon = pMain.GetIcon("SkillBtn", pRowSkill["a_client_icon_texid"].ToString(), Convert.ToInt32(pRowSkill["a_client_icon_row"]), Convert.ToInt32(pRowSkill["a_client_icon_col"]));
			if (pIcon != null)
				pbIcon.Image = pIcon;

			string strSkillDescription = pRowSkill["a_client_description_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty;

			tbDescription.Text = strSkillDescription;

			ReturnValues[2] = pRowSkill["a_name_" + pMain.pSettings.WorkLocale];
			ReturnValues[3] = strSkillDescription;
			ReturnValues[4] = pRowSkill["a_client_icon_texid"];
			ReturnValues[5] = pRowSkill["a_client_icon_row"];
			ReturnValues[6] = pRowSkill["a_client_icon_col"];

			DataRow[] pSkillLevelRows = pMain.pTables.SkillLevelTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).ToArray();

			foreach (var RowSkillLevel in pSkillLevelRows)
			{
				string strSkillLevel = RowSkillLevel["a_level"].ToString() ?? "0";

				cbLevelSelector.Items.Add($"Level: {strSkillLevel} - Power: " + RowSkillLevel["a_dummypower"]);

				if (ReturnValues[1].ToString() == strSkillLevel)
					cbLevelSelector.SelectedIndex = cbLevelSelector.Items.Count - 1;
			}

			ReturnValues[7] = string.Join(",", pSkillLevelRows.Select(row => { string level = row["a_level"]?.ToString(); string power = row["a_dummypower"]?.ToString(); return $"Level: {level} - Power: {power}"; }));

			if (cbLevelSelector.SelectedIndex == -1)
				cbLevelSelector.SelectedIndex = 0;

			cbLevelSelector.EndUpdate();

			cbLevelSelector.Enabled = true;
			btnSelect.Enabled = true;

			ReturnValues[1] = "";
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			Main.ListBoxItem? pSelectedItem = (Main.ListBoxItem?)MainList.SelectedItem;
			int nSelectedSkillLevel = cbLevelSelector.SelectedIndex;

			if (pSelectedItem != null && nSelectedSkillLevel != -1)
			{
				DialogResult = DialogResult.OK;

				ReturnValues[0] = pSelectedItem.ID;
				ReturnValues[1] = nSelectedSkillLevel + 1;

				Close();
			}
		}

		private void btnRemoveSkill_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = [-1, 0, "", "", "", "", "", new string[] { }];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
