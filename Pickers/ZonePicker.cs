namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int<Zone ID>
	 * Returns:
	 *	Object Array<Zone ID, Zone Name>
	// Call and receive implementation
	ZonePicker pZoneSelector = new(pMain, this, 1);
	if (pZoneSelector.ShowDialog() != DialogResult.OK)
		return;

	int nZoneID = Convert.ToInt32(pZoneSelector.ReturnValues[0]);
	string strZoneName = pZoneSelector.ReturnValues[1].ToString();
	/****************************************/
	public partial class ZonePicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		private bool bUserAction = false;
		public object[] ReturnValues = new object[2];

		public ZonePicker(Main mainForm, Form ParentForm, int nActualZoneNumber)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = nActualZoneNumber;
		}

		private async void ZonePicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			await pMain.GenericLoadZoneDataAsync();

			if (pMain.pTables.ZoneTable != null)
			{
				bUserAction = false;

				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.ZoneTable.Rows)
				{
					int nStringID = Convert.ToInt32(pRow["a_zone_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nStringID,
						Text = $"{pRow["a_zone_index"]} - {pRow["a_name"]}"
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

			ReturnValues = [-1, "NONE"];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
