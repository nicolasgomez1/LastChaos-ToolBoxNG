namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int<String ID>
	 *	Boolean<Enable/Disable "Remove String" Button>
	 * Returns:
	 *	Object Array<String ID, String>
	// Call and receive implementation
	StringPicker pStringSelector = new(pMain, this, 1, false);
	if (pStringSelector.ShowDialog() != DialogResult.OK)
		return;

	int nStringID = Convert.ToInt32(pStringSelector.ReturnValues[0]);
	string strString = pStringSelector.ReturnValues[1].ToString();
	/****************************************/
	public partial class StringPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		private bool bUserAction = false;
		public object[] ReturnValues = new object[2];

		public StringPicker(Main mainForm, Form ParentForm, int nActualStringID, bool bRemoveStringEnable = true)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = nActualStringID;

			btnRemoveString.Enabled = bRemoveStringEnable;
		}

		private async void StringPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			await pMain.GenericLoadStringDataAsync();

			if (pMain.pTables.StringTable != null)
			{
				bUserAction = false;

				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.StringTable.Rows)
				{
					int nStringID = Convert.ToInt32(pRow["a_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nStringID,
						Text = pRow["a_index"] + " - " + pRow["a_string_" + pMain.pSettings.WorkLocale]
					});

					if (nStringID == Convert.ToInt32(ReturnValues[0]))
						MainList.SelectedIndex = MainList.Items.Count - 1;
				}

				if (MainList.SelectedIndex == -1)
					MainList.SelectedIndex = 0;

				MainList.EndUpdate();

				bUserAction = true;

				ReturnValues[0] = -1;
			}
		}

		private void tbSearch_KeyDown(object sender, KeyEventArgs e) { nSearchPosition = pMain.SearchInListBox(tbSearch, e, MainList, nSearchPosition); }

		private void MainList_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (bUserAction)
			{
				if (MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
					return;

				DialogResult = DialogResult.OK;

				ReturnValues[0] = pSelectedItem.ID;
				ReturnValues[1] = pSelectedItem.Text.Split(" - ", StringSplitOptions.None)[1].Trim();

				Close();
			}
		}

		private void btnRemoveString_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = [-1, ""];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
