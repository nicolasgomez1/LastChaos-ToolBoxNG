namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int<Actual Title ID>
	 * Returns:
	 *	Int<Title ID>
	// Call and receive implementation
	RareOptionPicker pRareOptionSelector = new(pMain, this, 512);
	if (pRareOptionSelector.ShowDialog() != DialogResult.OK)
		return;

	int nRareOptionID = pRareOptionSelector.ReturnValues;
	/****************************************/
	public partial class RareOptionPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		private bool bUserAction = false;
		public int ReturnValues = -1;

		public RareOptionPicker(Main mainForm, Form ParentForm, int nActualRareOptionID)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues = nActualRareOptionID;
		}

		private async void RareOptionPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_prefix_" + pMain.pSettings.WorkLocale];

			if (pMain.pTables.RareOptionTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (var column in listQueryCompose.ToList())
				{
					if (!pMain.pTables.RareOptionTable.Columns.Contains(column))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(column);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_rareoption ORDER BY a_index;");
				});

				if (pMain.pTables.RareOptionTable == null)
					pMain.pTables.RareOptionTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.RareOptionTable);
			}

			if (pMain.pTables.RareOptionTable != null)
			{
				bUserAction = false;

				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.RareOptionTable.Rows)
				{
					int nItemID = Convert.ToInt32(pRow["a_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nItemID,
						Text = pRow["a_index"] + " - " + pRow["a_prefix_" + pMain.pSettings.WorkLocale]
					});

					if (nItemID == ReturnValues)
						MainList.SelectedIndex = MainList.Items.Count - 1;
				}

				if (MainList.SelectedIndex == -1)
					MainList.SelectedIndex = 0;

				MainList.EndUpdate();

				bUserAction = true;

				ReturnValues = -1;
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

				ReturnValues = pSelectedItem.ID;

				Close();
			}
		}

		private void btnRemoveRareOption_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = -1;

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
