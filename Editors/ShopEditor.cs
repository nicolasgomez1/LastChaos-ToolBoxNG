#define REQUIRED_QUEST_TO_USE_SYSTEM // NOTE: Custom system made by NicolasG, disable that if don't use it.
#define ITEM_PLUS_SYSTEM // NOTE: Custom system made by NicolasG, disable that if don't use it.

namespace LastChaos_ToolBoxNG
{
	public partial class ShopEditor : Form
	{
		private readonly Main pMain;
		private RenderDialog? pRenderDialog;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempShopRow;
		private DataRow[]? pTempShopItemRows;
		private string[]? strZones;
		private int nOriginalShopID = -1;
		private ContextMenuStrip? cmLevels;

		public ShopEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			gridSaleItems.TopLeftHeaderCell.Value = "N°";

			cbRenderDialog.Checked = pMain.pSettings.Show3DViewerDialog[this.Name];
		}
		
		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.ShopTable.Select("a_keeper_idx=" + pTempShopRow["a_keeper_idx"]).FirstOrDefault() != null) // the current selected shop, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Shop Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else    // the current selected shop is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Shop is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Shop Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void cbRenderDialog_CheckedChanged(object sender, EventArgs e)
		{
			bool bState = cbRenderDialog.Checked;

			if (bState && bUserAction)
			{
				if (pRenderDialog == null || pRenderDialog.IsDisposed)
					pRenderDialog = new RenderDialog(pMain);

				if (!pRenderDialog.Visible)
					pRenderDialog.Show();

				string strSMCPath = pMain.pTables.NPCTable.AsEnumerable().FirstOrDefault(row => Convert.ToInt32(row["a_index"]) == Convert.ToInt32(pTempShopRow["a_keeper_idx"]))?["a_file_smc"]?.ToString();

				if (strSMCPath != null)
				{
					if (File.Exists(pMain.pSettings.ClientPath + "\\" + strSMCPath))
						pRenderDialog.SetModel(pMain.pSettings.ClientPath + "\\" + strSMCPath, "big", -1);
				}
			}

			pMain.pSettings.Show3DViewerDialog[this.Name] = bState;

			pMain.WriteToINI(pMain.pSettings.SettingsFile, "3DViewerDialog", this.Name, bState.ToString());
		}

		private async Task LoadShopDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string>
			{
				"a_zone_num",
				"a_sell_rate",
				"a_buy_rate",
#if REQUIRED_QUEST_TO_USE_SYSTEM
				"a_required_quest",
#endif
				"a_pos_x",
				"a_pos_z",
				"a_pos_h",
				"a_pos_r",
				"a_y_layer"
			};

			if (pMain.pTables.ShopTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.ShopTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_keeper_idx, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_shop ORDER BY a_keeper_idx;");
				});

				if (pMain.pTables.ShopTable == null)
					pMain.pTables.ShopTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_keeper_idx", ref pMain.pTables.ShopTable);
			}

			bRequestNeeded = false;
			listQueryCompose.Clear();

			listQueryCompose = new List<string> {
				"a_item_idx",
#if ITEM_PLUS_SYSTEM
				"a_item_plus",
#endif
				"a_national",
			};

			if (pMain.pTables.ShopItemTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.ShopItemTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_keeper_idx, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_shopitem ORDER BY a_item_idx ASC;");
				});

				if (pMain.pTables.ShopItemTable == null)
					pMain.pTables.ShopItemTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_keeper_idx", ref pMain.pTables.ShopItemTable);
			}
		}

		private async Task LoadNPCDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = new List<string> { "a_level", "a_hp", "a_file_smc", "a_name_" + pMain.pSettings.WorkLocale, "a_descr_" + pMain.pSettings.WorkLocale };

			if (pMain.pTables.NPCTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.NPCTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_npc ORDER BY a_index;");
				});

				if (pMain.pTables.NPCTable == null)
					pMain.pTables.NPCTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.NPCTable);
			}
		}

		private async void ShopEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
#if REQUIRED_QUEST_TO_USE_SYSTEM
			lRequiredQuest.Visible = true;
			btnRequiredQuest.Visible = true;
#endif
#if ITEM_PLUS_SYSTEM
			gridSaleItems.Columns["itemplus"].Visible = true;
#endif
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadShopDataAsync(),
				LoadNPCDataAsync(),
				pMain.GenericLoadItemDataAsync(),
				pMain.GenericLoadZoneDataAsync(),
				pMain.GenericLoadQuestDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Shops, Shop Items, NPCs, Items, Zones & Quests Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.ZoneTable != null)
			{
				int nTotalZones = pMain.pTables.ZoneTable.Rows.Count;
				strZones = new string[nTotalZones];
				string strZoneName;

				cbZoneNumSelector.BeginUpdate();

				for (int i = 0; i < nTotalZones; i++)
				{
					strZoneName = i + " - " + pMain.pTables.ZoneTable.Rows[i]["a_name"].ToString();
					
					cbZoneNumSelector.Items.Add(strZoneName);

					strZones[i] = strZoneName;
				}

				cbZoneNumSelector.EndUpdate();
			}
			/****************************************/
			if (pMain.pTables.ShopTable != null && pMain.pTables.NPCTable != null)
			{
				MainList.BeginUpdate();

				int nNPCID;
				string strNPCName = "NPC NOT FOUND";

				foreach (DataRow pRow in pMain.pTables.ShopTable.Rows)
				{
					nNPCID = Convert.ToInt32(pRow["a_keeper_idx"]);
					
					DataRow? pNPCRow = pMain.pTables.NPCTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nNPCID).FirstOrDefault();
					if (pNPCRow != null)
						strNPCName = pNPCRow["a_name_" + pMain.pSettings.WorkLocale].ToString();
					else
						pMain.Logger(LogTypes.Error, $"Shop Editor > Shop: {pRow["a_keeper_idx"]} Error: a_keeper_idx: {nNPCID} not exist in t_npc.");

					AddToList(nNPCID, strNPCName ?? string.Empty, false);
				}

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Shops, Shop Items, NPCs, Items, Zones & Quests Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void ShopEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				if (pRenderDialog != null)
				{
					pRenderDialog.Close();
					pRenderDialog = null;
				}

				if (cmLevels != null)
				{
					cmLevels.Dispose();
					cmLevels = null;
				}
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Shop Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nShopID, bool bLoadFrompShopTable)
		{
			bUserAction = false;

			nOriginalShopID = nShopID;
			/****************************************/
			// Reset Controls
			cbZoneNumSelector.SelectedIndex = -1;

			gridSaleItems.Rows.Clear();
			/****************************************/
			if (bLoadFrompShopTable && pMain.pTables.ShopTable != null && pMain.pTables.ShopItemTable != null)
			{
				pTempShopRow = pMain.pTables.ShopTable.NewRow();
				pTempShopRow.ItemArray = (object[])pMain.pTables.ShopTable.Select("a_keeper_idx=" + nShopID)[0].ItemArray.Clone();

				pTempShopItemRows = pMain.pTables.ShopItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_keeper_idx"]) == nShopID).Select(row => {
					DataRow newRow = pMain.pTables.ShopItemTable.NewRow();
					newRow.ItemArray = (object[])row.ItemArray.Clone();
					return newRow;
				}).ToArray();
			}
			/****************************************/
			// General
			DataRow? pNPCRow = pMain.pTables.NPCTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nShopID).FirstOrDefault();
			string strShoperName = nShopID.ToString();

			if (pNPCRow != null)
			{
				strShoperName += " - " + pNPCRow["a_name_" + pMain.pSettings.WorkLocale];

				if (pMain.pSettings.Show3DViewerDialog[this.Name])
				{
					if (pRenderDialog == null || pRenderDialog.IsDisposed)
						pRenderDialog = new RenderDialog(pMain);

					if (!pRenderDialog.Visible)
						pRenderDialog.Show();

					if (!File.Exists(pMain.pSettings.ClientPath + "\\" + pNPCRow["a_file_smc"]))
						pMain.Logger(LogTypes.Error, $"Shop Editor > Shop: {nShopID} Error: a_file_smc path not exist or empty.");
					else
						pRenderDialog.SetModel(pMain.pSettings.ClientPath + "\\" + pNPCRow["a_file_smc"], "big", -1);
				}
			}

			btnNPCID.Text = strShoperName;
			/****************************************/
			int nZoneNum = Convert.ToInt32(pTempShopRow["a_zone_num"]);

			if (nZoneNum >= pMain.pTables.ZoneTable.Rows.Count)
				pMain.Logger(LogTypes.Error, $"Shop Editor > Shop: {nShopID} Error: a_zone_num out of range.");
			else
				cbZoneNumSelector.SelectedIndex = nZoneNum;
			/****************************************/
			tbSellRate.Text = pTempShopRow["a_sell_rate"].ToString();
			tbBuyRate.Text = pTempShopRow["a_buy_rate"].ToString();
			/****************************************/
			tbPosX.Text = Convert.ToString(pTempShopRow["a_pos_x"], CultureInfo.InvariantCulture);
			tbPosZ.Text = Convert.ToString(pTempShopRow["a_pos_z"], CultureInfo.InvariantCulture);
			tbPosH.Text = Convert.ToString(pTempShopRow["a_pos_h"], CultureInfo.InvariantCulture);
			tbPosR.Text = Convert.ToString(pTempShopRow["a_pos_r"], CultureInfo.InvariantCulture);
			tbYLayer.Text = pTempShopRow["a_y_layer"].ToString();
			/****************************************/
#if REQUIRED_QUEST_TO_USE_SYSTEM
			int nRequiredQuestID = Convert.ToInt32(pTempShopRow["a_required_quest"]);
			string strRequiredQuestName = "NONE";

			if (nRequiredQuestID > 0)
			{
				DataRow pQuestRow = pMain.pTables.QuestTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nRequiredQuestID).FirstOrDefault();
				if (pQuestRow != null)
					strRequiredQuestName = nRequiredQuestID + " - " + pQuestRow["a_name_" + pMain.pSettings.WorkLocale];
			}

			btnRequiredQuest.Text = strRequiredQuestName;
#endif
			// Sale Items
			if (pTempShopItemRows.Count() > 0)
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				gridSaleItems.SuspendLayout();

				int nSaleItemID, i = 0;
				string strSaleItemName;
				DataRow pItemRow;
				StringBuilder strNations = new();
				long lNationFlag;

				foreach (DataRow pRow in pTempShopItemRows)
				{
					gridSaleItems.Rows.Insert(i);

					gridSaleItems.Rows[i].HeaderCell.Value = (i + 1).ToString();

					nSaleItemID = Convert.ToInt32(pRow["a_item_idx"]);
					strSaleItemName = nSaleItemID.ToString();

					if (nSaleItemID > 0)
					{
						pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nSaleItemID).FirstOrDefault();
						if (pItemRow != null)
						{
							strSaleItemName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

							gridSaleItems.Rows[i].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
						}
					}

					gridSaleItems.Rows[i].Cells["item"].Value = strSaleItemName;
					gridSaleItems.Rows[i].Cells["item"].Tag = nSaleItemID;
#if ITEM_PLUS_SYSTEM
					gridSaleItems.Rows[i].Cells["itemPlus"].Value = pRow["a_item_plus"];
#endif
					strNations.Clear();
					lNationFlag = Convert.ToInt64(pRow["a_national"]);

					if (lNationFlag > 0)
					{
						foreach (var NationData in Defs.NationsIDNName)
						{
							if ((lNationFlag & 1L << NationData.Key) != 0)
								strNations.Append(NationData.Value + "\n");
						}

						if (strNations.Length <= 0)
							pMain.Logger(LogTypes.Error, $"Shop Editor > Shop: {nShopID} Error: a_national out of range.");
					}
					else
					{
						strNations.Append("Available in All Nations");
					}

					gridSaleItems.Rows[i].Cells["nation"].Value = lNationFlag.ToString();
					gridSaleItems.Rows[i].Cells["nation"].ToolTipText = strNations.ToString();

					i++;
				}

				gridSaleItems.ResumeLayout();
#if DEBUG
				stopwatch.Stop();
				pMain.Logger(LogTypes.Message, $"Populate the GridView took: {stopwatch.ElapsedMilliseconds}ms or {stopwatch.ElapsedTicks} ticks");
#endif
			}
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

				pMain.pTables.ShopTable?.Dispose();
				pMain.pTables.ShopTable = null;

				pMain.pTables.ShopItemTable?.Dispose();
				pMain.pTables.ShopItemTable = null;

				pMain.pTables.NPCTable?.Dispose();
				pMain.pTables.NPCTable = null;

				pMain.pTables.ItemTable?.Dispose();
				pMain.pTables.ItemTable = null;

				pMain.pTables.ZoneTable?.Dispose();
				pMain.pTables.ZoneTable = null;

				pMain.pTables.QuestTable?.Dispose();
				pMain.pTables.QuestTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				ShopEditor_LoadAsync(sender, e);
			}

			var (bProceed, _) = CheckUnsavedChanges();

			if (bProceed)
			{
				bUnsavedChanges = false;

				Reload();
			}
		}

		private (int nNPCID, string strNPCName) AskForNPCIndex()
		{
			NPCPicker pNPCSelector = new(pMain, this, Convert.ToInt32(pTempShopRow["a_keeper_idx"]), false);
			if (pNPCSelector.ShowDialog() != DialogResult.OK)
				return (-1, "NONE");

			int nNewShopID = Convert.ToInt32(pNPCSelector.ReturnValues[0]);

			if (!pMain.pTables.ShopTable.AsEnumerable().Any(row => Convert.ToInt32(row["a_keeper_idx"]) == nNewShopID))
			{
				return (nNewShopID, pNPCSelector.ReturnValues[1].ToString() ?? string.Empty);
			}
			else
			{
				pMain.Logger(LogTypes.Error, "Shop Editor > Cannot duplicate an NPC for a Shop.");

				return AskForNPCIndex();
			}
		}

		private void btnAddNew_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bProceed)
			{
				int i, nNewShopID;

				var (nNPCID, strNPCName) = AskForNPCIndex();

				if (nNPCID == -1)
					return;
				else
					nNewShopID = nNPCID;

				DataRow pNewRow;

				List<string> listIntColumns = new List<string>
				{
					"a_keeper_idx",
					"a_zone_num",
					"a_sell_rate",
					"a_buy_rate",
#if REQUIRED_QUEST_TO_USE_SYSTEM
					"a_required_quest",
#endif
					"a_y_layer"
				};

				List<string> listFloatColumns = new List<string>
				{
					"a_pos_x",
					"a_pos_z",
					"a_pos_h",
					"a_pos_r"
				};

				if (pMain.pTables.ShopTable == null)
				{
					DataTable pShopTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pShopTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listFloatColumns)
						pShopTableStruct.Columns.Add(strColumnName, typeof(float));

					pNewRow = pShopTableStruct.NewRow();

					pShopTableStruct.Dispose();
				}
				else
				{
					pNewRow = pMain.pTables.ShopTable.NewRow();
				}

				List<object> listDefaultValue = new List<object>
				{
					nNewShopID,	// a_keeper_idx
					0,	// a_zone_num
					0,	// a_sell_rate
					0,	// a_buy_rate
#if REQUIRED_QUEST_TO_USE_SYSTEM
					0,	// a_required_quest
#endif
					0,	// a_y_layer
					0,	// a_pos_x
					0,	// a_pos_z
					0,	// a_pos_h
					0	// a_pos_r
				};

				i = 0;
				foreach (string strColumnName in listIntColumns.Concat(listFloatColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempShopRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"Shop Editor > Shop: {nNewShopID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Shop Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewShopID, strNPCName, true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Shop. Because it's temporary.", "Shop Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				var (nNewShopID, strNPCName) = AskForNPCIndex();

				if (nNewShopID == -1)
					return;

				pTempShopRow = pMain.pTables.ShopTable.NewRow();
				pTempShopRow.ItemArray = (object[])pMain.pTables.ShopTable.Select("a_keeper_idx=" + nOriginalShopID)[0].ItemArray.Clone();

				pTempShopRow["a_keeper_idx"] = nNewShopID;

				AddToList(nNewShopID, strNPCName, true);

				// Sale Items
				if (pTempShopItemRows != null)	// NOTE: Una posibilidad sería simplemente no redibujar el GridView cuando el argumento bLoadFrompShopTable es false, pero se podrían agregar Items que realmente no estaban en pTempShopItemRows originalmente, entonces estaría copiando un listado de venta temporal, así que lo mejor creo que es hacerlo de esta manera. No es consistente a lo largo de todas las herramientas pero who cares.
				{
					DataRow[] clonedRows = new DataRow[pTempShopItemRows.Length];
					for (int i = 0; i < pTempShopItemRows.Length; i++)
					{
						DataRow newRow = pMain.pTables.ShopItemTable.NewRow();
						newRow.ItemArray = (object[])pTempShopItemRows[i].ItemArray.Clone();
						newRow["a_keeper_idx"] = nNewShopID;
						clonedRows[i] = newRow;
					}

					pTempShopItemRows = clonedRows;
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			DataRow pShopTableRow = pMain.pTables.ShopTable.Select("a_keeper_idx=" + nOriginalShopID).FirstOrDefault();

			if (pShopTableRow != null)
			{
				StringBuilder strbuilderQuery = new();

				strbuilderQuery.Append("START TRANSACTION;\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_shopitem WHERE a_keeper_idx={nOriginalShopID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_shop WHERE a_keeper_idx={nOriginalShopID};\n");

				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _)))
				{
					string strError = $"Shop Editor > Shop: {nOriginalShopID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Shop Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pMain.pTables.ShopItemTable != null)
					{
						DataRow pRow = pMain.pTables.ShopItemTable.Select("a_keeper_idx=" + nOriginalShopID).FirstOrDefault();
						if (pRow != null)
							pMain.pTables.ShopItemTable.Rows.Remove(pRow);
					}

					if (pShopTableRow != null)
						pMain.pTables.ShopTable.Rows.Remove(pShopTableRow);
				}
				catch (Exception ex)
				{
					string strError = $"Shop Editor > Shop: {nOriginalShopID} Changes applied in DataBase, but something got wrong while transferring temp to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Shop Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Shop Deleted successfully!", "Shop Editor", MessageBoxButtons.OK);

						MainList.SelectedIndex = nPrevObjectID;

						bUnsavedChanges = false;
					}
				}
			}
		}
		/****************************************/
		private void btnNPCID_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				NPCPicker pNPCSelector = new(pMain, this, Convert.ToInt32(pTempShopRow["a_keeper_idx"]), false);
				if (pNPCSelector.ShowDialog() != DialogResult.OK)
					return;

				int nNPCID = Convert.ToInt32(pNPCSelector.ReturnValues[0]);

				btnNPCID.Text = nNPCID + " - " + pNPCSelector.ReturnValues[1].ToString();

				pTempShopRow["a_keeper_idx"] = nNPCID;

				bUnsavedChanges = true;
			}
		}

		private void cbZoneNumSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbZoneNumSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempShopRow["a_zone_num"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void tbSellRate_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempShopRow["a_sell_rate"] = tbSellRate.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbBuyRate_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempShopRow["a_buy_rate"] = tbBuyRate.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPosX_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempShopRow["a_pos_x"] = tbPosX.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPosZ_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempShopRow["a_pos_z"] = tbPosZ.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPosH_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempShopRow["a_pos_h"] = tbPosH.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPosR_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempShopRow["a_pos_r"] = tbPosR.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbYLayer_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempShopRow["a_y_layer"] = tbYLayer.Text;

				bUnsavedChanges = true;
			}
		}

		private void btnRequiredQuest_Click(object sender, EventArgs e)
		{
#if REQUIRED_QUEST_TO_USE_SYSTEM
			if (bUserAction)
			{
				QuestPicker pQuestSelector = new(pMain, this, Convert.ToInt32(pTempShopRow["a_required_quest"]));
				if (pQuestSelector.ShowDialog() != DialogResult.OK)
					return;

				int nQuestID = Convert.ToInt32(pQuestSelector.ReturnValues[0]);
				string strQuestName = pQuestSelector.ReturnValues[1].ToString() ?? string.Empty;

				if (nQuestID > 0)
					strQuestName = nQuestID + " - " + strQuestName;

				btnRequiredQuest.Text = strQuestName;

				pTempShopRow["a_required_quest"] = nQuestID;

				bUnsavedChanges = true;
			}
#endif
		}

		private void gridSaleItems_CellValueChanged(object? sender, DataGridViewCellEventArgs e) { if (bUserAction) bUnsavedChanges = true; }

		private void gridSaleItems_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Right && e.ColumnIndex == -1) // Header Column
				{
					ToolStripMenuItem addItem = new("Add New");
					addItem.Click += (_, _) =>
					{
						bool bSuccess = true;
						ItemPicker pItemSelector = new(pMain, this, 0, false);
						if (pItemSelector.ShowDialog() != DialogResult.OK)
							return;

						int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
						if (nItemID > 0)
						{
							Image pIcon = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])));
#if ITEM_PLUS_SYSTEM
							int nDefaultItemPlus = 0;
#endif
							long lDefaultItemNation = 0;

							try
							{
								if (pTempShopItemRows == null)
									pTempShopItemRows = new DataRow[1];

								int nPosition = pTempShopItemRows.Length - 1;

								pTempShopItemRows[nPosition] = pMain.pTables.ShopItemTable.NewRow();

								pTempShopItemRows[nPosition]["a_keeper_idx"] = pTempShopRow["a_keeper_idx"];
								pTempShopItemRows[nPosition]["a_item_idx"] = nItemID;
#if ITEM_PLUS_SYSTEM
								pTempShopItemRows[nPosition]["a_item_plus"] = nDefaultItemPlus;
#endif
								pTempShopItemRows[nPosition]["a_national"] = lDefaultItemNation;
							}
							catch (Exception ex)
							{
								pMain.Logger(LogTypes.Error, $"Shop Editor > {ex.Message}.");

								bSuccess = false;
							}
							finally
							{
								if (bSuccess)
								{
									int nRow = gridSaleItems.Rows.Count;

									gridSaleItems.Rows.Insert(nRow);

									gridSaleItems.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();
									gridSaleItems.Rows[nRow].Cells["itemIcon"].Value = new Bitmap(pIcon, new Size(24, 24));
									gridSaleItems.Rows[nRow].Cells["item"].Value = nItemID + " - " + pItemSelector.ReturnValues[1].ToString();
									gridSaleItems.Rows[nRow].Cells["item"].Tag = nItemID;
#if ITEM_PLUS_SYSTEM
									gridSaleItems.Rows[nRow].Cells["itemPlus"].Value = nDefaultItemPlus;
#endif
									gridSaleItems.Rows[nRow].Cells["nation"].Value = lDefaultItemNation;

									StringBuilder strTooltip = new();

									if (lDefaultItemNation > 0)
									{
										foreach (var NationData in Defs.NationsIDNName)
										{
											if ((lDefaultItemNation & 1L << NationData.Key) != 0)
												strTooltip.Append(NationData.Value + "\n");
										}
									}
									else
									{
										strTooltip.Append("Available in All Nations");
									}

									gridSaleItems.Rows[nRow].Cells["nation"].ToolTipText = strTooltip.ToString();

									gridSaleItems.FirstDisplayedScrollingRowIndex = nRow;
									gridSaleItems.Rows[nRow].Selected = true;
								}
							}
						}
					};

					ToolStripMenuItem deleteItem = new("Delete") { Enabled = e.RowIndex >= 0 };
					deleteItem.Click += (_, _) =>
					{
						if (e.RowIndex >= 0)
						{
							gridSaleItems.SuspendLayout();

							gridSaleItems.Rows.RemoveAt(e.RowIndex);

							int i = 1;
							foreach (DataGridViewRow row in gridSaleItems.Rows)
							{
								row.HeaderCell.Value = i.ToString();

								i++;
							}

							gridSaleItems.ResumeLayout();
						}
					};

					cmLevels = new ContextMenuStrip();
					cmLevels.Items.AddRange(addItem, deleteItem);
					cmLevels.Show(Cursor.Position);
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 1 && e.RowIndex >= 0)    // Item Selector
				{
					int nItemID = Convert.ToInt32(gridSaleItems.Rows[e.RowIndex].Cells["item"].Tag);

					ItemPicker pItemSelector = new(pMain, this, nItemID, false);
					if (pItemSelector.ShowDialog() != DialogResult.OK)
						return;

					nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
					if (nItemID > 0)
					{
						gridSaleItems.Rows[e.RowIndex].Cells["itemIcon"].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
						gridSaleItems.Rows[e.RowIndex].Cells["item"].Value = $"{nItemID} - {pItemSelector.ReturnValues[1]}";
						gridSaleItems.Rows[e.RowIndex].Cells["item"].Tag = nItemID;
					}
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 3 && e.RowIndex >= 0)	// Nation Selector
				{
					FlagPicker pFlagSelector = new(this, Defs.NationsIDNName.Select(n => n.Value).ToArray(), Convert.ToInt64(gridSaleItems.Rows[e.RowIndex].Cells["nation"].Value));
					if (pFlagSelector.ShowDialog() != DialogResult.OK)
						return;

					gridSaleItems.Rows[e.RowIndex].Cells["nation"].Value = pFlagSelector.ReturnValues;

					StringBuilder strTooltip = new();

					if (pFlagSelector.ReturnValues > 0)
					{
						foreach (var NationData in Defs.NationsIDNName)
						{
							if ((pFlagSelector.ReturnValues & 1L << NationData.Key) != 0)
								strTooltip.Append(NationData.Value + "\n");
						}
					}
					else
					{
						strTooltip.Append("Available in All Nations");
					}

					gridSaleItems.Rows[e.RowIndex].Cells["nation"].ToolTipText = strTooltip.ToString();
				}
			}
		}
		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int i = 0, nShopID = Convert.ToInt32(pTempShopRow["a_keeper_idx"]);
			StringBuilder strbuilderQuery = new();

			// Init transaction.
			strbuilderQuery.Append("START TRANSACTION;\n");

			if (gridSaleItems.Rows.Count > 0)
			{
				pTempShopItemRows = new DataRow[gridSaleItems.Rows.Count];

				foreach (DataGridViewRow row in gridSaleItems.Rows)
				{
					DataRow pRow = pMain.pTables.ShopItemTable?.NewRow();

					pRow["a_keeper_idx"] = nShopID;	// Put NEW Shop ID
					pRow["a_item_idx"] = row.Cells["item"].Tag;
#if ITEM_PLUS_SYSTEM
					pRow["a_item_plus"] = row.Cells["itemPlus"].Value;
#endif
					pRow["a_national"] = row.Cells["nation"].Value;

					pTempShopItemRows[i] = pRow;

					i++;
				}

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_shopitem WHERE a_keeper_idx={nOriginalShopID};\n");

				// Compose t_shopitem INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();
				HashSet<string> addedColumns = new();

				foreach (DataRow pRow in pTempShopItemRows)
				{
					strColumnsValues.Append("(");

					foreach (DataColumn pCol in pRow.Table.Columns)
					{
						string strColumnName = pCol.ColumnName;

						if (!addedColumns.Contains(strColumnName))
						{
							strColumnsNames.Append(strColumnName + ", ");
							addedColumns.Add(strColumnName);
						}

						strColumnsValues.Append(pRow[pCol] + ", ");
					}

					strColumnsValues.Length -= 2;

					strColumnsValues.Append("), ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_shopitem ({strColumnsNames}) VALUES {strColumnsValues};\n");
			}

			DataRow pShopTableRow = pMain.pTables.ShopTable.Select("a_keeper_idx=" + nOriginalShopID).FirstOrDefault();
			if (pShopTableRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_shop SET");

				foreach (DataColumn pCol in pTempShopRow.Table.Columns)
				{
					object objValue = pTempShopRow[pCol];
					if (objValue is string)
						objValue = pMain.EscapeChars(objValue.ToString());
					else
						objValue = Convert.ToString(objValue, CultureInfo.InvariantCulture);

					strbuilderQuery.Append($" {pCol.ColumnName}='{objValue}',");
				}

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_keeper_idx={nOriginalShopID};\n");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempShopRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					object objValue = pTempShopRow[pCol];
					if (objValue is string)
						objValue = pMain.EscapeChars(objValue.ToString());
					else
						objValue = Convert.ToString(objValue, CultureInfo.InvariantCulture);

					strColumnsValues.Append($"'{objValue}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_shop ({strColumnsNames}) VALUES ({strColumnsValues});\n");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _))
			{
				try
				{
					if (pTempShopItemRows != null && pTempShopItemRows.Length > 0)
					{
						DataRow[] pShopItemTableRowsOld = pMain.pTables.ShopItemTable?.Select("a_keeper_idx=" + nOriginalShopID);
						foreach (DataRow row in pShopItemTableRowsOld)
							row.Delete();

						if (nShopID != nOriginalShopID)
						{
							DataRow[] pShopItemTableRowsNew = pMain.pTables.ShopItemTable?.Select("a_keeper_idx=" + nShopID);
							foreach (DataRow row in pShopItemTableRowsNew)
								row.Delete();
						}

						foreach (DataRow pRow in pTempShopItemRows)
						{
							DataRow? newDataRow = pMain.pTables.ShopItemTable?.NewRow();
							newDataRow.ItemArray = (object[])pRow.ItemArray.Clone();
							pMain.pTables.ShopItemTable?.Rows.Add(newDataRow);
						}

						pMain.pTables.ShopItemTable?.AcceptChanges();
					}

					if (pShopTableRow != null)  // Row exist in Global Table, update it.
					{
						pShopTableRow.ItemArray = (object[])pTempShopRow.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pShopTableRow = pMain.pTables.ShopTable.NewRow();
						pShopTableRow.ItemArray = (object[])pTempShopRow.ItemArray.Clone();
						pMain.pTables.ShopTable.Rows.Add(pShopTableRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Shop Editor > Shop: {nOriginalShopID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Shop Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nShopID;

						DataRow pNPCRow = pMain.pTables.NPCTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nShopID).FirstOrDefault();
						string strShoperName = "NPC NOT FOUND";

						if (pNPCRow != null)
							strShoperName = pNPCRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

						pSelectedItem.Text = $"{nShopID} - {strShoperName}";

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "Shop Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Shop Editor > Shop: {nOriginalShopID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Shop Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
