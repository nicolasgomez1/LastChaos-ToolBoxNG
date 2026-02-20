namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	Int<Actual Item ID>
	 *	Bool<Enable Remove Item Button> (Default: true)
	 * Returns:
	 *	Object Array<Item ID, Item Name, Item Description, Texture ID, Texture Row, Texture Col>
	// Call and receive implementation
	ItemPicker pItemSelector = new(pMain, this, 19, false);
	if (pItemSelector.ShowDialog() != DialogResult.OK)
		return;

	int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
	string strItemName = pItemSelector.ReturnValues[1].ToString();
	string strItemDescription = pItemSelector.ReturnValues[2].ToString();
	string strItemTextureID = pItemSelector.ReturnValues[3].ToString();
	int nItemTextureRow = Convert.ToInt32(pItemSelector.ReturnValues[4]);
	int nItemTextureCol = Convert.ToInt32(pItemSelector.ReturnValues[5]);
	/****************************************/
	public partial class ItemPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private int nSearchPosition = 0;
		public object[] ReturnValues = new object[6];

		public ItemPicker(Main mainForm, Form ParentForm, int nActualItem, bool bAllowRemove = true)
		{
			InitializeComponent();

			pMain = mainForm;
			pParentForm = ParentForm;
			ReturnValues[0] = nActualItem;

			btnRemoveItem.Enabled = bAllowRemove;
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

		private async void ItemPicker_LoadAsync(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			await pMain.GenericLoadItemDataAsync();

			if (pMain.pTables.ItemTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.ItemTable.Rows)
				{
					int nItemID = Convert.ToInt32(pRow["a_index"]);

					MainList.Items.Add(new Main.ListBoxItem
					{
						ID = nItemID,
						Text = pRow["a_index"] + " - " + pRow["a_name_" + pMain.pSettings.WorkLocale]
					});

					if (nItemID == Convert.ToInt32(ReturnValues[0]))
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

			btnSelect.Enabled = false;

			int nItemID = pSelectedItem.ID;
			DataRow? pRowItem = pMain.pTables.ItemTable?.Select("a_index=" + nItemID).FirstOrDefault();

			string strTextureID = pRowItem["a_texture_id"].ToString() ?? "0";
			int nTextureRow = Convert.ToInt32(pRowItem["a_texture_row"]);
			int nTextureCol = Convert.ToInt32(pRowItem["a_texture_col"]);

			Image? pIcon = pMain.GetIcon("ItemBtn", strTextureID, nTextureRow, nTextureCol);
			if (pIcon != null)
				pbIcon.Image = pIcon;

			AddInfo("Level:", true);
			AddInfo(pRowItem["a_level"].ToString());

			AddInfo("Description:", true);
			AddInfo(pRowItem["a_descr_" + pMain.pSettings.WorkLocale].ToString());

			ReturnValues[1] = pRowItem["a_name_" + pMain.pSettings.WorkLocale];
			ReturnValues[2] = pRowItem["a_descr_" + pMain.pSettings.WorkLocale];
			ReturnValues[3] = strTextureID;
			ReturnValues[4] = nTextureRow;
			ReturnValues[5] = nTextureCol;

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

		private void btnRemoveItem_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			ReturnValues = [-1, "", "", "", "", ""];

			Close();
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }
	}
}
