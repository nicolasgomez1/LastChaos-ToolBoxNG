// NOTE: I modify the code related to this system, but i did it when I recently starts to learn c++ and know about LC code, so I do some changes without left marks and comments and do a completly mess. So in this case have no preprocessor directives to swap to stock Tables names and/or Columns names. But u can easy adapt this code by looking the needed shit in the newproject_auth → t_key and newproject_db → t_key. If u're no able to do that... better find other hobby :D.

namespace LastChaos_ToolBoxNG
{
	public partial class PackageItemEventEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempPackageRow;
		private ContextMenuStrip? cmRewards;

		public PackageItemEventEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridRewards.TopLeftHeaderCell.Value = "N°";
		}

		private void CalculateExpirationDate(DateTime dtDateLimit)
		{
			Color cColor = Color.White;
			string strTimeLeft;
			TimeSpan tsTimeLeft = dtDateLimit - DateTime.Now;

			if (tsTimeLeft.TotalSeconds <= 0)
			{
				cColor = Color.FromArgb(218, 54, 51);
				strTimeLeft = "Expired";
			}
			else
			{
				strTimeLeft = $"Expires in Days: {tsTimeLeft.Days} Hours: {tsTimeLeft.Hours}";
			}

			lbTimeLeft.ForeColor = cColor;
			lbTimeLeft.Text = strTimeLeft;

			mcExpirationDate.SetDate(dtDateLimit);
		}

		private string GenerateRandomCode()
		{
			int nCodeLength = 25;
			StringBuilder strCode = new(nCodeLength);
			string strChars = "abcdefghijklmnopqrstuvwxyz0123456789";   // NOTE: Change possible chars for Redeem Code here

			using (RandomNumberGenerator pRNG = RandomNumberGenerator.Create())
			{
				byte[] Buffer = new byte[1];

				while (strCode.Length < nCodeLength)
				{
					pRNG.GetBytes(Buffer);

					int nIndex = Buffer[0] % strChars.Length;
					char cRandomChar = strChars[nIndex];

					pRNG.GetBytes(Buffer);

					if ((Buffer[0] & 1) == 0)
						cRandomChar = char.ToUpperInvariant(cRandomChar);

					strCode.Append(cRandomChar);
				}
			}

			return strCode.ToString();
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.KeyTable.Select("a_index=" + pTempPackageRow["a_index"]).FirstOrDefault() != null)    // the current selected Item Package, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Package Item Event Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else    // the current selected Item Package is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Item Package is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Package Item Event Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
					else if (pDialogReturn == DialogResult.Yes)
						bDeleteActual = true;
				}
			}

			return (bProceed, bDeleteActual);
		}

		private void AddToList(int nID, string strName, bool bIsTemp)
		{
			MainList.Items.Add(new Main.ListBoxItem
			{
				ID = nID,
				Text = $"{nID} - {strName}"
			});

			if (bIsTemp)
			{
				LoadUIData(nID, false);

				MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
				MainList.SelectedIndex = MainList.Items.Count - 1;
				MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

				pLastSelected = (Main.ListBoxItem?)MainList.SelectedItem;

				bUnsavedChanges = true;
			}
		}

		private async Task LoadPackageItemEventDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string> { "a_strkey", "a_enable", "a_uses_limit", "a_date_limit", "a_rewards" };

			if (pMain.pTables.KeyTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.KeyTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_key ORDER BY a_index;");
				});

				if (pMain.pTables.KeyTable == null)
					pMain.pTables.KeyTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.KeyTable);
			}
		}

		private async void PackageItemEventEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadPackageItemEventDataAsync(),
				pMain.GenericLoadItemDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Item Packages Event & Items Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.KeyTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.KeyTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_index"]), pRow["a_strkey"].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Item Packages Event & Items Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void PackageItemEventEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				if (cmRewards != null)
				{
					cmRewards.Dispose();
					cmRewards = null;
				}
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Package Item Event Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (pDialogReturn == DialogResult.No)
					e.Cancel = true;
				else
					Clear();
			}
			else
			{
				Clear();
			}
		}

		private void LoadUIData(int nPackageID, bool bLoadFrompKeyTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			gridRewards.Rows.Clear();
			/****************************************/
			if (bLoadFrompKeyTable && pMain.pTables.KeyTable != null)
			{
				pTempPackageRow = pMain.pTables.KeyTable.NewRow();
				pTempPackageRow.ItemArray = (object[])pMain.pTables.KeyTable.Select("a_index=" + nPackageID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nPackageID.ToString();
			/****************************************/
			if (pTempPackageRow["a_enable"].ToString() == "1")
				cbEnable.Checked = true;
			else
				cbEnable.Checked = false;
			/****************************************/
			nudUsesLimit.Value = Convert.ToInt32(pTempPackageRow["a_uses_limit"]);
			/****************************************/
			tbCode.Text = pTempPackageRow["a_strkey"].ToString();
			/****************************************/
			CalculateExpirationDate((DateTime)pTempPackageRow["a_date_limit"]);
			/****************************************/
			int i = 0;
			DataRow pItemRow;

			gridRewards.SuspendLayout();

			foreach (string strRewardID in pTempPackageRow["a_rewards"].ToString().Split(' '))
			{
				gridRewards.Rows.Insert(i);

				gridRewards.Rows[i].HeaderCell.Value = (i + 1).ToString();

				int nRewardID = Convert.ToInt32(strRewardID);
				string strRewardItemName = nRewardID.ToString();

				if (nRewardID > 0)
				{
					pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nRewardID).FirstOrDefault();
					if (pItemRow != null)
					{
						strRewardItemName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

						gridRewards.Rows[i].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
					}
				}

				gridRewards.Rows[i].Cells["item"].Value = strRewardItemName;
				gridRewards.Rows[i].Cells["item"].Tag = nRewardID;

				i++;
			}

			gridRewards.ResumeLayout();
			/****************************************/
			bUserAction = true;

			btnUpdate.Enabled = true;

			btnCopy.Enabled = true;
			btnDelete.Enabled = true;
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }

		private void tbSearch_KeyDown(object sender, KeyEventArgs e) { nSearchPosition = pMain.SearchInListBox(tbSearch, e, MainList, nSearchPosition); }

		private void MainList_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
				return;

			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bProceed)
			{
				if (bDeleteActual)
				{
					int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

					MainList.Items.RemoveAt(MainList.Items.Count - 1);

					object nSelected = MainList.Items[nPrevObjectID];

					LoadUIData(((Main.ListBoxItem)nSelected).ID, true);

					MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
					MainList.SelectedItem = nSelected;
					MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

					bUnsavedChanges = false;
				}
				else
				{
					bUnsavedChanges = false;

					LoadUIData(pSelectedItem.ID, true);
				}
			}
			else
			{
				MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
				MainList.SelectedItem = pLastSelected;
				MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;
			}

			pLastSelected = pSelectedItem;
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			void Reload()
			{
				MainList.Enabled = false;
				btnReload.Enabled = false;

				nSearchPosition = 0;

				pMain.pTables.KeyTable?.Dispose();
				pMain.pTables.KeyTable = null;

				pMain.pTables.ItemTable?.Dispose();
				pMain.pTables.ItemTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				PackageItemEventEditor_LoadAsync(sender, e);
			}

			var (bProceed, _) = CheckUnsavedChanges();

			if (bProceed)
			{
				bUnsavedChanges = false;

				Reload();
			}
		}

		private void btnAddNew_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bProceed)
			{
				int i, nNewPackageID = 9999;
				DataRow pNewRow;

				List<string> listIntColumns = new List<string>	// Here add all int columns.
				{
					"a_index",
					"a_enable",
					"a_uses_limit"
				};

				List<string> listVarcharColumns = new List<string>	// Here add all varchar columns.
				{
					"a_strkey",
					"a_rewards"
				};

				List<string> listDateTimeColumns = new List<string>	// Here add all datetime columns.
				{
					"a_date_limit"
				};

				if (pMain.pTables.KeyTable == null) // If is null, create new DataTable and set schema (column name & datatype).
				{
					DataTable pKeyTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pKeyTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listVarcharColumns)
						pKeyTableStruct.Columns.Add(strColumnName, typeof(string));

					foreach (string strColumnName in listDateTimeColumns)
						pKeyTableStruct.Columns.Add(strColumnName, typeof(DateTime));

					pNewRow = pKeyTableStruct.NewRow();

					pKeyTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_key ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewPackageID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewPackageID = pMain.AskForIndex(this.Text, "a_index")) == -1) // I don't test it...
							return;
					}

					QueryReturn = null;
				}
				else
				{
					nNewPackageID = Convert.ToInt32(pMain.pTables.KeyTable.Select().LastOrDefault()["a_index"]) + 1;

					pNewRow = pMain.pTables.KeyTable.NewRow();
				}

				string strNewRandomCode = GenerateRandomCode();

				List<object> listDefaultValue = new List<object>
				{
					nNewPackageID,	// a_index
					0,	// a_enable
					-1,	// a_uses_limit
					strNewRandomCode,	// a_strkey
					43,	// a_rewards
					DateTime.Now	// a_date_limit
				};

				i = 0;
				foreach (string strColumnName in listIntColumns.Concat(listVarcharColumns).Concat(listDateTimeColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempPackageRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"Package Item Event Editor > Item Package: {nNewPackageID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Package Item Event Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewPackageID, strNewRandomCode, true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Item Package. Because it's temporary.", "Package Item Event Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				string strNewPackageCode = GenerateRandomCode();
				int nPackageIDToCopy = Convert.ToInt32(pTempPackageRow["a_index"]);
				int nNewPackageID = Convert.ToInt32(pMain.pTables.KeyTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempPackageRow = pMain.pTables.KeyTable.NewRow();
				pTempPackageRow.ItemArray = (object[])pMain.pTables.KeyTable.Select("a_index=" + nPackageIDToCopy)[0].ItemArray.Clone();

				pTempPackageRow["a_index"] = nNewPackageID;
				pTempPackageRow["a_strkey"] = strNewPackageCode;

				AddToList(nNewPackageID, strNewPackageCode, true);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nPackageID = Convert.ToInt32(pTempPackageRow["a_index"]);
			DataRow? pPackageRow = pMain.pTables.KeyTable?.Select("a_index=" + nPackageID).FirstOrDefault();

			if (pPackageRow != null)
			{
				StringBuilder strbuilderQuery = new();

				// Init transaction.
				strbuilderQuery.Append("START TRANSACTION;\n");

				// Compose DELETE Query.
				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_key WHERE a_index={nPackageID};");

				DialogResult pDialogReturn = MessageBox.Show("You want to Delete all related claimed Codes to this one in t_key_log?", "Package Item Event Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (pDialogReturn == DialogResult.Yes)
					strbuilderQuery.Append($"\nDELETE FROM {pMain.pSettings.DBUser}.t_key_log WHERE a_coupon='{pTempPackageRow["a_strkey"]}';");

				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _)))
				{
					string strError = $"Package Item Event Editor > Item Package: {nPackageID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Package Item Event Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pPackageRow != null)
						pMain.pTables.KeyTable.Rows.Remove(pPackageRow);
				}
				catch (Exception ex)
				{
					string strError = $"Package Item Event Editor > Item Package: {nPackageID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Package Item Event Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Item Package Deleted successfully!", "Package Item Event Editor", MessageBoxButtons.OK);

						MainList.SelectedIndex = nPrevObjectID;

						bUnsavedChanges = false;
					}
				}
			}
		}
		/****************************************/
		private void cbEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				string strEnable = "0";

				if (cbEnable.Checked)
					strEnable = "1";

				pTempPackageRow["a_enable"] = strEnable;

				bUnsavedChanges = true;
			}
		}

		private void nudUsesLimit_ValueChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempPackageRow["a_uses_limit"] = nudUsesLimit.Value.ToString();

				bUnsavedChanges = true;
			}
		}

		private void tbCode_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				string strKey = tbCode.Text;
				DataRow pKeyRow = pMain.pTables.KeyTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) != Convert.ToInt32(pTempPackageRow["a_index"]) && row["a_strkey"].ToString() == strKey).FirstOrDefault();

				if (pKeyRow == null)
				{
					tbCode.BackColor = Color.FromArgb(28, 30, 31);

					pTempPackageRow["a_strkey"] = strKey;

					bUnsavedChanges = true;
				}
				else
				{
					tbCode.BackColor = Color.FromArgb(218, 54, 51);

					string strError = $"Package Item Event Editor > Repeated Redeem Codes. {pKeyRow["a_index"]} and this one.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Package Item Event Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void btnRandomGenerator_Click(object sender, EventArgs e)
		{
			if (bUserAction)
				tbCode.Text = GenerateRandomCode();
		}

		private void btnSetExpirationDateManually_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				string strExpireDate = ((DateTime)pTempPackageRow["a_date_limit"]).ToString("yyyy-MM-dd");

				MessageBox_Input pInput = new(this, "Please set Expiration Date:", strExpireDate, "0000-00-00");
				if (pInput.ShowDialog() == DialogResult.OK)
				{
					if (DateTime.TryParseExact(pInput.strOutput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtExpirationDate))
					{
						CalculateExpirationDate(dtExpirationDate);

						pTempPackageRow["a_date_limit"] = dtExpirationDate;

						bUnsavedChanges = true;
					}
				}
			}
		}

		private void mcExpirationDate_DateChanged(object sender, DateRangeEventArgs e)
		{
			if (bUserAction)
			{
				DateTime dtExpirationDate = mcExpirationDate.SelectionEnd;

				CalculateExpirationDate(dtExpirationDate);

				pTempPackageRow["a_date_limit"] = dtExpirationDate;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void gridRewards_CellValueChanged(object? sender, DataGridViewCellEventArgs e) { if (bUserAction) bUnsavedChanges = true; }

		private void gridRewards_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Right && e.ColumnIndex == -1)  // Header Column
				{
					ToolStripMenuItem addItem = new("Add New");
					addItem.Click += (_, _) =>
					{
						ItemPicker pItemSelector = new(pMain, this, 0, false);
						if (pItemSelector.ShowDialog() != DialogResult.OK)
							return;

						int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
						if (nItemID > 0)
						{
							int nRow = gridRewards.Rows.Count;

							gridRewards.Rows.Insert(nRow);

							gridRewards.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

							gridRewards.Rows[nRow].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
							gridRewards.Rows[nRow].Cells["item"].Value = nItemID + " - " + pItemSelector.ReturnValues[1].ToString();
							gridRewards.Rows[nRow].Cells["item"].Tag = nItemID;

							gridRewards.FirstDisplayedScrollingRowIndex = nRow;
							gridRewards.Rows[nRow].Selected = true;
						}
					};

					ToolStripMenuItem deleteItem = new("Delete") { Enabled = e.RowIndex >= 0 };
					deleteItem.Click += (_, _) =>
					{
						if (e.RowIndex >= 0)
						{
							gridRewards.SuspendLayout();

							gridRewards.Rows.RemoveAt(e.RowIndex);

							int i = 1;
							foreach (DataGridViewRow row in gridRewards.Rows)
							{
								row.HeaderCell.Value = i.ToString();

								i++;
							}

							gridRewards.ResumeLayout();
						}
					};

					cmRewards = new ContextMenuStrip();
					cmRewards.Items.AddRange(addItem, deleteItem);
					cmRewards.Show(Cursor.Position);
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 1 && e.RowIndex >= 0)	// Item Selector
				{
					int nItemID = Convert.ToInt32(gridRewards.Rows[e.RowIndex].Cells["item"].Tag);

					ItemPicker pItemSelector = new(pMain, this, nItemID, false);
					if (pItemSelector.ShowDialog() != DialogResult.OK)
						return;

					nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
					if (nItemID > 0)
					{
						gridRewards.Rows[e.RowIndex].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
						gridRewards.Rows[e.RowIndex].Cells["item"].Value = $"{nItemID} - {pItemSelector.ReturnValues[1]}";
						gridRewards.Rows[e.RowIndex].Cells["item"].Tag = nItemID;
					}
				}
			}
		}
		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			DataRow pKeyRow = pMain.pTables.KeyTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) != Convert.ToInt32(pTempPackageRow["a_index"]) && row["a_strkey"].ToString() == pTempPackageRow["a_strkey"].ToString()).FirstOrDefault();

			if (pKeyRow != null)
			{
				MessageBox.Show($"This Redeem Code is already used in t_key: {pKeyRow["a_index"]}, cannot use duplicates.", "Package Item Event Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			int nPackageID = Convert.ToInt32(pTempPackageRow["a_index"]);
			List<string> listRewards = new();
			StringBuilder strbuilderQuery = new();

			if (gridRewards.Rows.Count == 0)
			{
				listRewards.Add("0");
			}
			else
			{
				foreach (DataGridViewRow row in gridRewards.Rows)
					listRewards.Add(row.Cells["item"].Tag.ToString());
			}

			pTempPackageRow["a_rewards"] = string.Join(" ", listRewards);
			// Check if Package exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pPackageRow = pMain.pTables.KeyTable?.Select("a_index=" + nPackageID).FirstOrDefault();
			if (pPackageRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_key SET");

				foreach (DataColumn pCol in pTempPackageRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempPackageRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nPackageID};");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempPackageRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempPackageRow[pCol].ToString())}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_key ({strColumnsNames}) VALUES ({strColumnsValues});");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.ToString(), out long _))
			{
				try
				{
					if (pPackageRow != null)  // Row exist in Global Table, update it.
					{
						pPackageRow.ItemArray = (object[])pTempPackageRow.ItemArray.Clone();
					}
					else // Row not exist in Global Table, insert it.
					{
						pPackageRow = pMain.pTables.KeyTable.NewRow();
						pPackageRow.ItemArray = (object[])pTempPackageRow.ItemArray.Clone();
						pMain.pTables.KeyTable.Rows.Add(pPackageRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Package Item Event Editor > Item Package: {nPackageID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Package Item Event Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nPackageID;
						pSelectedItem.Text = nPackageID + " - " + tbCode.Text.ToString();

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "Package Item Event Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Package Item Event Editor > Item Package: {nPackageID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Package Item Event Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
