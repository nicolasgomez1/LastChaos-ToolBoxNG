namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int<Actual Quest ID>
	 * Returns:
	 *	Object Array<Quest ID, Quest Name>
	// Call and receive implementation
	QuestPicker pQuestSelector = new(pMain, this, 1);
	if (pQuestSelector.ShowDialog() != DialogResult.OK)
		return;

	int nQuestID = Convert.ToInt32(pQuestSelector.ReturnValues[0]);
	string strQuestName = pQuestSelector.ReturnValues[1].ToString();
	/****************************************/
	public partial class QuestPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		public object[] ReturnValues = new object[2];

		public QuestPicker(Main mainForm, Form ParentForm, int nActualQuestID)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = nActualQuestID;
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

		private async void QuestPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			await pMain.GenericLoadQuestDataAsync();

			if (pMain.pTables.QuestTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.QuestTable.Rows)
				{
					int nQuestID = Convert.ToInt32(pRow["a_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nQuestID,
						Text = nQuestID + " - " + pRow["a_name_" + pMain.pSettings.WorkLocale]
					});

					if (nQuestID == Convert.ToInt32(ReturnValues[0]))
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

			int nQuestID = pSelectedItem.ID;
			DataRow? pRowQuest = pMain.pTables.QuestTable?.Select("a_index=" + nQuestID).FirstOrDefault();

			if (pRowQuest != null)
			{
				AddInfo("Name:", true);

				string strQuestName = pRowQuest["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty;

				AddInfo(strQuestName);

				AddInfo("Description:", true);

				AddInfo(pRowQuest["a_desc_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty);

				AddInfo("Minimum Required Level:", true);

				AddInfo(pRowQuest["a_need_min_level"].ToString() ?? string.Empty);

				AddInfo("Maximum Allowed Level:", true);

				AddInfo(pRowQuest["a_need_max_level"].ToString() ?? string.Empty);

				ReturnValues[1] = strQuestName;
			}
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

		private void btnRemove_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = [-1, "NONE"];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
