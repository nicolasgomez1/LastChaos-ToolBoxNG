namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int<Actual NPC ID>
	 *	Bool<Enable/Disable "Remove NPC" Button>
	 * Returns:
	 *	Object Array<NPC ID, NPC Name>
	// Call and receive implementation
	NPCPicker pNPCSelector = new(pMain, this, 0, true);
	if (pNPCSelector.ShowDialog() != DialogResult.OK)
		return;

	int nNPCID = Convert.ToInt32(pNPCSelector.ReturnValues[0]);
	string strNPCName = pNPCSelector.ReturnValues[1].ToString();
	/****************************************/
	public partial class NPCPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		public object[] ReturnValues = new object[2];

		public NPCPicker(Main mainForm, Form ParentForm, int nActualNPCID, bool bRemoveNPCEnable = true)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = nActualNPCID;

			btnRemoveNPC.Enabled = bRemoveNPCEnable;
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

		private async void NPCPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			await pMain.GenericLoadNPCDataAsync();

			if (pMain.pTables.NPCTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.NPCTable.Rows)
				{
					int nNPCID = Convert.ToInt32(pRow["a_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nNPCID,
						Text = pRow["a_index"] + " - " + pRow["a_name_" + pMain.pSettings.WorkLocale]
					});

					if (nNPCID == Convert.ToInt32(ReturnValues[0]))
						MainList.SelectedIndex = MainList.Items.Count - 1;
				}

				if (MainList.SelectedIndex == -1)
					MainList.SelectedIndex = 0;

				MainList.EndUpdate();

				ReturnValues[0] = 0;
			}
		}

		private void tbSearch_KeyDown(object sender, KeyEventArgs e) { nSearchPosition = pMain.SearchInListBox(tbSearch, e, MainList, nSearchPosition); }

		private void MainList_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
				return;

			rtbInformation.Clear();

			btnSelect.Enabled = false;

			int nNPCID = pSelectedItem.ID;
			DataRow? pRowNPC = pMain.pTables.NPCTable?.Select("a_index=" + nNPCID).FirstOrDefault();

			if (pRowNPC != null)
			{
				AddInfo("Level:", true);
				AddInfo(pRowNPC["a_level"].ToString() ?? string.Empty);

				AddInfo("HP:", true);
				AddInfo(pRowNPC["a_HP"].ToString() ?? string.Empty);

				ReturnValues[1] = pRowNPC["a_name_" + pMain.pSettings.WorkLocale];
			}

			btnSelect.Enabled = true;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			Main.ListBoxItem? pSelectedItem = (Main.ListBoxItem?)MainList.SelectedItem;

			if (pSelectedItem != null)
			{
				DialogResult = DialogResult.OK;

				ReturnValues[0] = pSelectedItem.ID;

				Close();
			}
		}

		private void btnRemoveNPC_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = [0, "NONE"];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
