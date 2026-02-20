namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int<Title ID>
	 * Returns:
	 *	Object Array<Title ID, Title Name>
	// Call and receive implementation
	TitlePicker pTitleSelector = new(pMain, this, 1);
	if (pTitleSelector.ShowDialog() != DialogResult.OK)
		return;

	int nTitleID = Convert.ToInt32(pTitleSelector.ReturnValues[0]);
	string strTitleName = pTitleSelector.ReturnValues[1].ToString();
	/****************************************/
	public partial class TitlePicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		private bool bUserAction = false;
		public object[] ReturnValues = new object[2];

		public TitlePicker(Main mainForm, Form ParentForm, int nActualTitleID)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = nActualTitleID;
		}

		private async void TitlePicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			await pMain.GenericLoadItemDataAsync();

			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_item_index"];

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
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_title ORDER BY a_index;");
				});

				if (pMain.pTables.TitleTable == null)
					pMain.pTables.TitleTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.TitleTable);
			}

			if (pMain.pTables.TitleTable != null && pMain.pTables.ItemTable != null)
			{
				bUserAction = false;

				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.TitleTable.Rows)
				{
					int nStringID = Convert.ToInt32(pRow["a_index"]);
					string strName = nStringID.ToString();

					DataRow? pItemRow = pMain.pTables.ItemTable?.Select($"a_index={pRow["a_item_index"]}").FirstOrDefault();
					if (pItemRow != null)
						strName += $" - {pItemRow["a_name_" + pMain.pSettings.WorkLocale]} ({pRow["a_item_index"]})";

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nStringID,
						Text = strName
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

		private void btnRemoveZone_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = [0, "NONE"];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
