//#define ENABLE_SECOND_SKILL_TO_CRAFT	// NOTE: These values are required by the server, but are not actually used
#define ALLOWED_ZONE_SYSTEM	// NOTE: Custom system made by NicolasG, disable that to use normal a_zone_flag
#define DAILY_REWARD_SYSTEM	// NOTE: Custom system made by NicolasG, disable that if don't use it.
#define REWORKED_EXCHANGE_SYSTEM	// NOTE: Custom system made by NicolasG, disable that if don't use it.
#define REWORKED_EVENT_PACKAGE_ITEM	// NOTE: Custom system made by NicolasG, disable that if don't use it.

namespace LastChaos_ToolBoxNG
{
	public partial class ItemEditor : Form
	{
		private readonly Main pMain;
		private RenderDialog? pRenderDialog;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempItemRow = null!;
		private DataRow? pTempFortuneHeadRow;
		private DataRow[]? pTempFortuneDataRows;
		private string[]? strZones;
		private ToolTip? pToolTip;
		private Dictionary<Control, ToolTip>? pToolTips = new();
		private ContextMenuStrip? cmFortune, cmCommonInput;

		public ItemEditor(Main mainForm)
		{
			InitializeComponent();

#if ENABLE_SECOND_SKILL_TO_CRAFT
			label44.Visible = true;

			btnSkill2RequiredID.Visible = true;

			label43.Visible = true;

			tbSkill2RequiredLevel.Visible = true;
#endif
#if ALLOWED_ZONE_SYSTEM
			btnAllowedZoneFlag.Visible = true;
#else
			lAllowedZone.Text = "Zone Flag";

			tbAllowedZoneFlag.Visible = true;
#endif
			pMain = mainForm;
			/****************************************/
			gridFortune.TopLeftHeaderCell.Value = "N°";
			//gridFortune.CellValueChanged += gridFortune_CellValueChanged;
			/****************************************/
			cbRenderDialog.Checked = pMain.pSettings.Show3DViewerDialog[this.Name];
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			Control? cControl = (sender as ContextMenuStrip)?.SourceControl;
			// NOTE: Some pickers return 0, others -1, I don't know how the update or insert action will behave, it could fail completely.
			if (cControl != null)
			{
				string strControlName = cControl.Name;
				int nActualValue = Convert.ToInt32(cControl.Text);

				ToolStripMenuItem menuItemPicker = new("Item Picker");
				menuItemPicker.Click += (_, _) =>
				{
					ItemPicker pItemSelector = new(pMain, this, nActualValue);
					if (pItemSelector.ShowDialog() != DialogResult.OK)
						return;

					cControl.Text = pItemSelector.ReturnValues[0].ToString();
				};

				ToolStripMenuItem menuZonePicker = new("Zone Picker");
				menuZonePicker.Click += (_, _) =>
				{
					ZonePicker pZoneSelector = new(pMain, this, nActualValue);
					if (pZoneSelector.ShowDialog() != DialogResult.OK)
						return;

					cControl.Text = pZoneSelector.ReturnValues[0].ToString();
				};

				ToolStripMenuItem menuSkillPicker = new("Skill Picker");
				menuSkillPicker.Click += (_, _) =>
				{
					int nSkillLevel = 0;
					TextBox? tbSecondInputObject = null;

					if (strControlName.Length >= 11 && strControlName.Substring(0, 11) == "tbRareIndex")
					{
						tbSecondInputObject = (TextBox)Controls.Find("tbRareProb" + strControlName[strControlName.Length - 1], true)[0];
						nSkillLevel = Convert.ToInt32(tbSecondInputObject.Text);
					}

					if (strControlName.Length >= 9 && strControlName == "tbOption0")
					{
						tbSecondInputObject = (TextBox)Controls.Find("tbOption1", true)[0];
						nSkillLevel = Convert.ToInt32(tbSecondInputObject.Text);
					}

					SkillPicker pSkillSelector = new(pMain, this, [nActualValue, nSkillLevel], false);
					if (pSkillSelector.ShowDialog() != DialogResult.OK)
						return;

					cControl.Text = pSkillSelector.ReturnValues[0].ToString();

					if (tbSecondInputObject != null)
						tbSecondInputObject.Text = pSkillSelector.ReturnValues[1].ToString();
				};

				ToolStripMenuItem menuRarePicker = new("Rare Option Picker");
				menuRarePicker.Click += (_, _) =>
				{
					RareOptionPicker pRareOptionSelector = new(pMain, this, nActualValue);
					if (pRareOptionSelector.ShowDialog() != DialogResult.OK)
						return;
#pragma warning disable
					cControl.Text = pRareOptionSelector.ReturnValues.ToString();
#pragma warning restore
				};

				ToolStripMenuItem menuOptionPicker = new("Option Picker");
				menuOptionPicker.Click += (_, _) =>
				{
					int nOptionLevel = 0;
					TextBox? tbSecondInputObject = null;

					if (strControlName.Length >= 11 && strControlName.Substring(0, 11) == "tbRareIndex")
					{
						tbSecondInputObject = (TextBox)Controls.Find("tbRareProb" + strControlName[strControlName.Length - 1], true)[0];
						nOptionLevel = Convert.ToInt32(tbSecondInputObject.Text);
					}

					OptionPicker pOptionSelector = new(pMain, this, new int[] { nActualValue, nOptionLevel });
					if (pOptionSelector.ShowDialog() != DialogResult.OK)
						return;

					cControl.Text = pOptionSelector.ReturnValues[0].ToString();

					if (tbSecondInputObject != null)
						tbSecondInputObject.Text = pOptionSelector.ReturnValues[1].ToString();
				};

				ToolStripMenuItem menuMagicPicker = new("Magic Picker");
				menuMagicPicker.Click += (_, _) =>
				{
					int nMagicLevel = 0;
					TextBox? tbSecondInputObject = null;

					if (strControlName.Length >= 11 && strControlName.Substring(0, 11) == "tbRareIndex")
					{
						tbSecondInputObject = (TextBox)Controls.Find("tbRareProb" + strControlName[strControlName.Length - 1], true)[0];
						nMagicLevel = Convert.ToInt32(tbSecondInputObject.Text);
					}

					if (strControlName.Length >= 6 && strControlName == "tbSet0")
					{
						tbSecondInputObject = (TextBox)Controls.Find("tbSet1", true)[0];
						nMagicLevel = Convert.ToInt32(tbSecondInputObject.Text);
					}

					MagicPicker pMagicSelector = new(pMain, this, new int[] { nActualValue, nMagicLevel });
					if (pMagicSelector.ShowDialog() != DialogResult.OK)
						return;

					cControl.Text = pMagicSelector.ReturnValues[0].ToString();

					if (tbSecondInputObject != null)
						tbSecondInputObject.Text = pMagicSelector.ReturnValues[1].ToString();
				};

				ToolStripMenuItem menuGenericTypePicker = new("Generic Type Picker");
				menuGenericTypePicker.Click += (_, _) =>
				{
					GenericTypePicker pGenericTypeSelector = new(pMain, this);
					if (pGenericTypeSelector.ShowDialog() != DialogResult.OK)
						return;
#pragma warning disable
					cControl.Text = pGenericTypeSelector.ReturnValues.ToString();
#pragma warning restore
				};

				ToolStripMenuItem menuSpecialSkillPicker = new("Special Skill Picker");
				menuSpecialSkillPicker.Click += (_, _) =>
				{
					int nSkillLevel = 0;
					TextBox? tbSecondInputObject = null;

					if (strControlName.Length >= 11 && strControlName.Substring(0, 11) == "tbRareIndex")
					{
						tbSecondInputObject = (TextBox)Controls.Find("tbRareProb" + strControlName[strControlName.Length - 1], true)[0];
						nSkillLevel = Convert.ToInt32(tbSecondInputObject.Text);
					}

					if (strControlName.Length >= 9 && strControlName == "tbOption0")
					{
						tbSecondInputObject = (TextBox)Controls.Find("tbOption1", true)[0];
						nSkillLevel = Convert.ToInt32(tbSecondInputObject.Text);
					}

					SpecialSkillPicker pSpecialSkillSelector = new(pMain, this, [nActualValue, nSkillLevel], false);
					if (pSpecialSkillSelector.ShowDialog() != DialogResult.OK)
						return;

					cControl.Text = pSpecialSkillSelector.ReturnValues[0].ToString();

					if (tbSecondInputObject != null)
						tbSecondInputObject.Text = pSpecialSkillSelector.ReturnValues[1].ToString();
				};

				ToolStripMenuItem menuTitlePicker = new("Title Picker");
				menuTitlePicker.Click += (_, _) =>
				{
					TitlePicker pTitleSelector = new(pMain, this, nActualValue);
					if (pTitleSelector.ShowDialog() != DialogResult.OK)
						return;

					cControl.Text = pTitleSelector.ReturnValues[0].ToString();
				};

				cmCommonInput = new ContextMenuStrip();
				cmCommonInput.Items.AddRange(
					menuItemPicker,
					menuZonePicker,
					menuSkillPicker,
					menuOptionPicker,
					menuRarePicker,
					menuMagicPicker,
					menuGenericTypePicker,
					menuSpecialSkillPicker,
					menuTitlePicker
				);
				cmCommonInput.Show(Cursor.Position);
			}
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.ItemTable?.Select("a_index=" + pTempItemRow["a_index"]).FirstOrDefault() != null)	// the current selected item, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Item Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else	// the current selected item is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Item is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Item Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
					else if (pDialogReturn == DialogResult.Yes)
						bDeleteActual = true;
				}
			}

			return (bProceed, bDeleteActual);
		}

		private void SetSetDataTexts()	// NOTE: Here can set label text for "a_set_X" columns textboxs ↓
		{
			int nType = cbTypeSelector.SelectedIndex;
			string[] strTexts = { "0", "1", "2", "3", "4" };

			if ((nType == 2 /*ITYPE_ONCE*/ || nType == 6 /*ITYPE_POTION*/) && ((Convert.ToInt64(pTempItemRow["a_flag"]) & (1L << Array.IndexOf(Defs.ItemFlags, "QUEST"))) != 0))
			{
				strTexts[0] = "Zone ID";
				strTexts[1] = "Position X";
				strTexts[2] = "Position Z";
				strTexts[3] = "Position Y";
				strTexts[4] = "Range Limit";
			}

			for (int i = 0; i < strTexts.Length; i++)
				((Label)Controls.Find("lSet" + i, true)[0]).Text = strTexts[i];
		}

		private void SetOptionDataTexts()   // NOTE: Here can set label text for "a_num_X" columns textboxs ↓
		{
			int nType = cbTypeSelector.SelectedIndex;
			int nSubType = cbSubTypeSelector.SelectedIndex;
			string[] strTexts = { "0", "1", "2", "3", "4" };

			if (nType == 0 /*ITYPE_WEAPON*/)
			{
				strTexts[0] = "Physical Damage";
				strTexts[1] = "Magic Damage";
				strTexts[2] = "Attack Speed";
			}
			else if (nType == 1 /*ITYPE_WEAR*/)
			{
				strTexts[0] = "Physical Defense";
				strTexts[1] = "Magic Defense";

				if (nSubType == 6 /*IWEAR_BACKWING*/)
					strTexts[2] = "Fly Speed";
				else
					strTexts[2] = "???";    // NOTE: Some items have values here

				if (nSubType == 7 /*IWEAR_SUIT*/ || ((Convert.ToInt64(pTempItemRow["a_flag"]) & (1L << Array.IndexOf(Defs.ItemFlags, "COSTUME2"))) != 0))
					strTexts[4] = "Duration Time";
			}
			else if (nType == 2 /*ITYPE_ONCE*/)
			{
				if (nSubType == 11 /*IONCE_TITLE*/)
				{
					strTexts[0] = "Title ID";
					strTexts[1] = "???";    // NOTE: Some items have values here
					strTexts[4] = "???";    // NOTE: Some items have values here
				}
			}
			else if (nType == 6 /*ITYPE_POTION*/)
			{
				if (nSubType == 5 /*IPOTION_ETC*/)
				{
					strTexts[0] = "Skill ID";
					strTexts[1] = "Skill Level";
				}
			}

			for (int i = 0; i < strTexts.Length; i++)
				((Label)Controls.Find("lOption" + i, true)[0]).Text = strTexts[i];
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

				if (pTempItemRow["a_file_smc"] != null)
				{
					if (File.Exists(pMain.pSettings.ClientPath + "\\" + pTempItemRow["a_file_smc"]))
						pRenderDialog.SetModel(pMain.pSettings.ClientPath + "\\" + pTempItemRow["a_file_smc"], "small", Convert.ToInt32(pTempItemRow["a_wearing"]));
				}
			}

			pMain.pSettings.Show3DViewerDialog[this.Name] = bState;

			pMain.WriteToINI(pMain.pSettings.SettingsFile, "3DViewerDialog", this.Name, bState.ToString());
		}

		private async Task LoadItemDataAsync()
		{
			bool bRequestNeeded = false;
			// NOTE: Here you must define the columns that you want to request from the database.
			List<string> listQueryCompose =
			[
				"a_enable",
				"a_texture_id",
				"a_texture_row",
				"a_texture_col",
				"a_file_smc",
				"a_weight",
				"a_price",
				"a_level",
				"a_level2",
				"a_durability",
				"a_fame",
				"a_max_use",
				"a_grade",
				"a_type_idx",
				"a_subtype_idx",
				"a_wearing",
				"a_rvr_value",
				"a_rvr_grade",
				"a_effect_name",
				"a_attack_effect_name",
				"a_damage_effect_name",
				"a_castle_war",
				"a_job_flag",
				"a_zone_flag",
				"a_flag",
				"a_origin_variation1",
				"a_origin_variation2",
				"a_origin_variation3",
				"a_origin_variation4",
				"a_origin_variation5",
				"a_origin_variation6",
				"a_set_0",
				"a_set_1",
				"a_set_2",
				"a_set_3",
				"a_set_4",
				"a_num_0",
				"a_num_1",
				"a_num_2",
				"a_num_3",
				"a_num_4",
				"a_need_sskill",
				"a_need_sskill_level",
#if ENABLE_SECOND_SKILL_TO_CRAFT
				"a_need_sskill2",
				"a_need_sskill_level2",
#endif
				"a_need_item0", "a_need_item_count0",
				"a_need_item1", "a_need_item_count1",
				"a_need_item2", "a_need_item_count2",
				"a_need_item3", "a_need_item_count3",
				"a_need_item4", "a_need_item_count4",
				"a_need_item5", "a_need_item_count5",
				"a_need_item6", "a_need_item_count6",
				"a_need_item7", "a_need_item_count7",
				"a_need_item8", "a_need_item_count8",
				"a_need_item9", "a_need_item_count9",
				"a_rare_index_0", "a_rare_prob_0",
				"a_rare_index_1", "a_rare_prob_1",
				"a_rare_index_2", "a_rare_prob_2",
				"a_rare_index_3", "a_rare_prob_3",
				"a_rare_index_4", "a_rare_prob_4",
				"a_rare_index_5", "a_rare_prob_5",
				"a_rare_index_6", "a_rare_prob_6",
				"a_rare_index_7", "a_rare_prob_7",
				"a_rare_index_8", "a_rare_prob_8",
				"a_rare_index_9", "a_rare_prob_9"
			];

			// NOTE: If columns related to locale are required, they must be defined here.
			foreach (string strNation in pMain.pSettings.NationSupported)
				listQueryCompose.AddRange("a_name_" + strNation.ToLower(), "a_descr_" + strNation.ToLower());

			if (pMain.pTables.ItemTable == null)    // NOTE: If the global table is empty, directly indicate that a query must be executed requesting all previously defined columns.
			{
				bRequestNeeded = true;
			}
			else    // NOTE: If the global table is not empty, check if any of the columns to request are already present. To remove it from the query and not request redundant information.
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.ItemTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					// WARNING NOTE: Possible problem: I don't know how this will work when do query with multiple locales that are not compatible with a single charset are requested.
					// NOTE: As you can see, regardless of the columns to request, it is always necessary to request the reference column, in this case a_index.
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_item ORDER BY a_index;");
				});

				if (pMain.pTables.ItemTable == null)   // If Global table is null, just pass the structure and data from pNewTable to it.
					pMain.pTables.ItemTable = pNewTable;
				else	// In otherwise, just add new columns, to existing rows, or add new rows and values to Global table.
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.ItemTable);   // Copy data from missing columns to Global table.
			}
		}

		private async Task LoadSpecialSkillDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_name_" + pMain.pSettings.WorkLocale,
				"a_desc_" + pMain.pSettings.WorkLocale,
				"a_texture_id",
				"a_texture_row",
				"a_texture_col",
			];

			if (pMain.pTables.SpecialSkillTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.SpecialSkillTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_special_skill ORDER BY a_index;");
				});

				if (pMain.pTables.SpecialSkillTable == null)
					pMain.pTables.SpecialSkillTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.SpecialSkillTable);
			}
		}

		private async Task LoadRareOptionDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_prefix_" + pMain.pSettings.WorkLocale];

			if (pMain.pTables.RareOptionTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.RareOptionTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
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
		}

		private async Task LoadFortuneDataAsync()	// Head & Data
		{
			// Load t_fortune_head
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_prob_type", "a_enable"];

			if (pMain.pTables.ItemFortuneHeadTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.ItemFortuneHeadTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_item_idx, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_fortune_head ORDER BY a_item_idx;");
				});

				if (pMain.pTables.ItemFortuneHeadTable == null)
					pMain.pTables.ItemFortuneHeadTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_item_idx", ref pMain.pTables.ItemFortuneHeadTable);
			}

			// Load t_fortune_data
			bRequestNeeded = false;

			listQueryCompose.Clear();
			listQueryCompose = ["a_skill_index", "a_skill_level", "a_string_index", "a_prob"];

			if (pMain.pTables.ItemFortuneDataTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.ItemFortuneDataTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_item_idx, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_fortune_data ORDER BY a_item_idx;");
				});

				if (pMain.pTables.ItemFortuneDataTable == null)
					pMain.pTables.ItemFortuneDataTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_item_idx", ref pMain.pTables.ItemFortuneDataTable);
			}
		}

		private void LoadFortuneData(int nItemID)
		{
			gridFortune.Rows.Clear();

			if (pMain.pTables.ItemFortuneHeadTable.Select("a_item_idx=" + nItemID).Length > 0)
			{
				pTempFortuneHeadRow = pMain.pTables.ItemFortuneHeadTable.NewRow();
				pTempFortuneHeadRow.ItemArray = (object[])pMain.pTables.ItemFortuneHeadTable.Select("a_item_idx=" + nItemID)[0].ItemArray.Clone();
			}

			if (pTempFortuneHeadRow != null)
			{
				if (pTempFortuneHeadRow["a_enable"].ToString() == "1")
					cbFortuneEnable.Checked = true;
				else
					cbFortuneEnable.Checked = false;

				cbFortuneProbType.SelectedIndex = Convert.ToInt32(pTempFortuneHeadRow["a_prob_type"]);
				/****************************************/
				pTempFortuneDataRows = pMain.pTables.ItemFortuneDataTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_item_idx"]) == nItemID).Select(row => {
					DataRow newRow = pMain.pTables.ItemFortuneDataTable.NewRow();
					newRow.ItemArray = (object[])row.ItemArray.Clone();
					return newRow;
				}).ToArray();

				if (pTempFortuneDataRows.Length > 0)
				{
					int i = 0;
					DataRow? pSkillRow, pStringRow;

					gridFortune.SuspendLayout();

					foreach (DataRow pFortuneRow in pTempFortuneDataRows)
					{
						int nFortuneSkillID = Convert.ToInt32(pFortuneRow["a_skill_index"]);
						int nFortuneSkillLevel = Convert.ToInt32(pFortuneRow["a_skill_level"]);
						int nFortuneSkillProb = Convert.ToInt32(pFortuneRow["a_prob"]);
						string strSkillID = nFortuneSkillID.ToString();

						pSkillRow = pMain.pTables.SkillTable?.Select($"a_index={nFortuneSkillID}").FirstOrDefault();
						if (pSkillRow != null)
						{
							gridFortune.Rows.Insert(i);

							gridFortune.Rows[i].HeaderCell.Value = (i + 1).ToString();

							gridFortune.Rows[i].Cells["skillIcon"].Value = new Bitmap(pMain.GetIcon("SkillBtn", pSkillRow["a_client_icon_texid"].ToString(), Convert.ToInt32(pSkillRow["a_client_icon_row"]), Convert.ToInt32(pSkillRow["a_client_icon_col"])), new Size(24, 24));
							gridFortune.Rows[i].Cells["skill"].Value = nFortuneSkillID + " - " + pSkillRow["a_name_" + pMain.pSettings.WorkLocale];
							gridFortune.Rows[i].Cells["skill"].Tag = nFortuneSkillID;
							gridFortune.Rows[i].Cells["skill"].ToolTipText = pSkillRow["a_client_description_" + pMain.pSettings.WorkLocale].ToString();

							using (DataGridViewComboBoxCell cbSkillLevel = (DataGridViewComboBoxCell)gridFortune.Rows[i].Cells["level"])
							{
								DataRow[] pSkillLevelRows = pMain.pTables.SkillLevelTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nFortuneSkillID).ToArray();

								foreach (DataRow pRowSkillLevel in pSkillLevelRows)
								{
									int nSkillLevel = Convert.ToInt32(pRowSkillLevel["a_level"]);

									int nLastItemAdded = cbSkillLevel.Items.Add($"Level: {nSkillLevel} - Power: {pRowSkillLevel["a_dummypower"]}");

									if (nFortuneSkillLevel == nSkillLevel)
										cbSkillLevel.Value = cbSkillLevel.Items[nLastItemAdded];
								}
							}

							gridFortune.Rows[i].Cells["level"].Tag = nFortuneSkillLevel;
							gridFortune.Rows[i].Cells["prob"].Value = nFortuneSkillProb;
							//gridFortune.Rows[i].Cells["prob"].ToolTipText = ((nFortuneSkillProb * 100.0f) / 10000.0f) + "%";	// NOTE: I disable it cuz looks like have more values involved in prob calc than only nFortuneSkillProb
							gridFortune.Rows[i].Cells["string_id"].Value = pFortuneRow["a_string_index"].ToString();

							string strString = "";

							pStringRow = pMain.pTables.StringTable.Select($"a_index={pFortuneRow["a_string_index"]}").FirstOrDefault();
							if (pStringRow != null)
								strString = pStringRow["a_string_" + pMain.pSettings.WorkLocale].ToString();

							gridFortune.Rows[i].Cells["string_id"].ToolTipText = strString;
						}

						i++;
					}

					gridFortune.ResumeLayout();
				}
				else
				{
					pMain.Logger(LogTypes.Warning, $"Item Editor > Item: {nItemID} Warning: This Item have a entry in a_fortune_head, but not in a_fortune_data.");
				}
			}
		}

		private async void ItemEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
			cbNationSelector.BeginUpdate();

			for (int i = 0; i < pMain.pSettings.NationSupported.Length; i++)
			{
				string strNation = pMain.pSettings.NationSupported[i];

				cbNationSelector.Items.Add(strNation);

				if (strNation.ToLower() == pMain.pSettings.WorkLocale)
					cbNationSelector.SelectedIndex = i;
			}

			cbNationSelector.EndUpdate();
			/****************************************/
			cbGrade.BeginUpdate();

			foreach (string strAPetType in Defs.APetTypes)
				cbGrade.Items.Add(strAPetType);

			cbGrade.EndUpdate();
			/****************************************/
			cbCastleType.BeginUpdate();

			foreach (string strCastleType in Defs.ItemCastleTypes)
				cbCastleType.Items.Add(strCastleType);

			cbCastleType.EndUpdate();
			/****************************************/
			cbWearingPositionSelector.BeginUpdate();

			foreach (string strWearingPos in Defs.ItemWearingPositions)
				cbWearingPositionSelector.Items.Add(strWearingPos);

			cbWearingPositionSelector.EndUpdate();
			/****************************************/
			cbTypeSelector.BeginUpdate();

			foreach (string strType in Defs.ItemTypesNSubTypes.Keys)
				cbTypeSelector.Items.Add(strType);

			cbTypeSelector.EndUpdate();
			/****************************************/
			cbRvRValueSelector.BeginUpdate();

			foreach (string strSyndicateType in Defs.SyndicateTypesNGrades.Keys)
				cbRvRValueSelector.Items.Add(strSyndicateType);

			cbRvRValueSelector.EndUpdate();
			/****************************************/
			cbFortuneProbType.BeginUpdate();

			foreach (string strProbType in Defs.FortuneItemProbTypes)
				cbFortuneProbType.Items.Add(strProbType);

			cbFortuneProbType.EndUpdate();
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll( // NOTE: Here information is requested from the mysql server asynchronously, thus reducing waiting times to the minimum possible.
				LoadItemDataAsync(),
				pMain.GenericLoadZoneDataAsync(),
				pMain.GenericLoadSkillDataAsync(),
				pMain.GenericLoadSkillLevelDataAsync(),
				LoadSpecialSkillDataAsync(),
				LoadRareOptionDataAsync(),
				pMain.GenericLoadStringDataAsync(),
				LoadFortuneDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Items, Zones, Skills, Skills Levels, Special Skill, Rare Options, Strings & Fortune Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.ItemTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.ItemTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_index"]), pRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			if (pMain.pTables.ZoneTable != null)
			{
				int nTotalZones = pMain.pTables.ZoneTable.Rows.Count;
				strZones = new string[nTotalZones];

				for (int i = 0; i < nTotalZones; i++)
					strZones[i] = pMain.pTables.ZoneTable.Rows[i]["a_name"].ToString() ?? string.Empty;
			}
			/****************************************/
			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnReload, "Reload Items, Zones, Skills, Skills Levels, Special Skills, Rare Options, Strings & Fortune Data from Database");	// Not dispose onreload
			pToolTip.SetToolTip(tbSMC, "Double Click to edit");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void ItemEditor_FormClosing(object sender, FormClosingEventArgs e)	// NOTE: Here is an example of the unsaved data warning messages in case want to close the form.
		{
			void Clear()
			{
				foreach (var toolTip in pToolTips.Values.Distinct())
					toolTip.Dispose();

				pToolTips = null;

				if (pRenderDialog != null)
				{
					pRenderDialog.Close();
					pRenderDialog = null;
				}

				if (cmFortune != null)
				{
					cmFortune.Dispose();
					cmFortune = null;
				}

				if (cmCommonInput != null)
				{
					cmCommonInput.Dispose();
					cmCommonInput = null;
				}
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Item Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nItemID, bool bLoadFrompItemTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			cbTypeSelector.SelectedIndex = -1;
			cbSubTypeSelector.SelectedIndex = -1;
			cbWearingPositionSelector.SelectedIndex = -1;

			btnSkill1RequiredID.Image = null;
#if ENABLE_SECOND_SKILL_TO_CRAFT
			btnSkill2RequiredID.Image = null;
#endif
			for (int i = 0; i < Defs.MAX_MAKE_ITEM_MATERIAL; i++)
				((Button)Controls.Find($"btnItem{i}Required", true)[0]).Image = null;

			cbFortuneEnable.Checked = false;
			cbFortuneProbType.SelectedIndex = -1;

			pTempFortuneHeadRow = null;
			pTempFortuneDataRows = null;

			gridFortune.Rows.Clear();

			foreach (var toolTip in pToolTips.Values)
				toolTip.Dispose();
			/****************************************/
			if (bLoadFrompItemTable && pMain.pTables.ItemTable != null)
			{
				pTempItemRow = pMain.pTables.ItemTable.NewRow();	// Replicate struct in temp row val.
				pTempItemRow.ItemArray = (object[])pMain.pTables.ItemTable.Select("a_index=" + nItemID)[0].ItemArray.Clone();	// Copy data from main table to temp one.
			}
			/****************************************/
			// General
			tbID.Text = nItemID.ToString();
			/****************************************/
			if (pTempItemRow["a_enable"].ToString() == "1")
				cbEnable.Checked = true;
			else
				cbEnable.Checked = false;
			/****************************************/
			string strTexID = pTempItemRow["a_texture_id"].ToString();
			string strTexRow = pTempItemRow["a_texture_row"].ToString();
			string strTexCol = pTempItemRow["a_texture_col"].ToString();

			Image pIcon = pMain.GetIcon("ItemBtn", strTexID, Convert.ToInt32(strTexRow), Convert.ToInt32(strTexCol));
			if (pIcon != null)
			{
				pbIcon.Image = pIcon;

				pToolTip = new ToolTip();
				pToolTip.SetToolTip(pbIcon, $"FILE: {strTexID} ROW: {strTexRow} COL: " + strTexCol);
				pToolTips[pbIcon] = pToolTip;
			}
			/****************************************/
			string strSMCPath = pTempItemRow["a_file_smc"].ToString();

			tbSMC.Text = strSMCPath.Replace("Data\\", "");

			int nWearingPosition = Convert.ToInt32(pTempItemRow["a_wearing"]);

			if (pMain.pSettings.Show3DViewerDialog[this.Name])
			{
				if (pRenderDialog == null || pRenderDialog.IsDisposed)
					pRenderDialog = new RenderDialog(pMain);

				if (!pRenderDialog.Visible)
					pRenderDialog.Show();

				if (!File.Exists(pMain.pSettings.ClientPath + "\\" + strSMCPath))
					pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_file_smc path not exist or empty.");
				else
					pRenderDialog.SetModel(pMain.pSettings.ClientPath + "\\" + strSMCPath, "small", nWearingPosition);
			}
			/****************************************/
			tbMaxStack.Text = pTempItemRow["a_weight"].ToString();
			tbPrice.Text = pTempItemRow["a_price"].ToString();
			tbMinLevel.Text = pTempItemRow["a_level"].ToString();
			tbMaxLevel.Text = pTempItemRow["a_level2"].ToString();
			tbDurability.Text = pTempItemRow["a_durability"].ToString();
			tbFame.Text = pTempItemRow["a_fame"].ToString();
			tbMaxUse.Text = pTempItemRow["a_max_use"].ToString();
			/****************************************/
			int nGrade = Convert.ToInt32(pTempItemRow["a_grade"]);

			if (nGrade < 0 || nGrade >= Defs.ItemCastleTypes.Length)
				pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_grade out of range.");
			else
				cbGrade.SelectedIndex = nGrade;
			/****************************************/
			string strNation = cbNationSelector.SelectedItem.ToString().ToLower();

			tbName.Text = pTempItemRow["a_name_" + strNation].ToString();
			tbDescription.Text = pTempItemRow["a_descr_" + strNation].ToString();
			/****************************************/
			int nCastleType = Convert.ToInt32(pTempItemRow["a_castle_war"]);

			if (nCastleType < 0 || nCastleType >= Defs.ItemCastleTypes.Length)
				pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_castle_war out of range.");
			else
				cbCastleType.SelectedIndex = nCastleType;
			/****************************************/
			if (nWearingPosition < -1 || nWearingPosition >= Defs.ItemWearingPositions.Length)
			{
				cbWearingPositionSelector.Enabled = false;
				cbWearingPositionSelector.Text = "";

				pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_wearing out of range.");
			}
			else
			{
				if (nWearingPosition == -1)
					nWearingPosition = 0;
				else
					nWearingPosition++;

				cbWearingPositionSelector.Enabled = true;
				cbWearingPositionSelector.SelectedIndex = nWearingPosition;
			}
			/****************************************/
			btnClassFlag.Text = pTempItemRow["a_job_flag"].ToString();

			StringBuilder strTooltip = new();

			long lJobFlag = Convert.ToInt64(pTempItemRow["a_job_flag"]);

			for (int i = 0; i < Defs.ItemClass.Length; i++)
			{
				if ((lJobFlag & 1L << i) != 0)
					strTooltip.Append(Defs.ItemClass[i] + "\n");
			}

			if (lJobFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_job_flag out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnClassFlag, strTooltip.ToString());
			pToolTips[btnClassFlag] = pToolTip;
			/****************************************/
			string strZoneFlag = pTempItemRow["a_zone_flag"].ToString();

#if ALLOWED_ZONE_SYSTEM
			btnAllowedZoneFlag.Text = strZoneFlag;

			strTooltip = new StringBuilder();
			long lZoneFlag = Convert.ToInt64(strZoneFlag);

			for (int i = 0; i < pMain.pTables.ZoneTable.Rows.Count; i++)
			{
				if ((lZoneFlag & 1L << i) != 0)
					strTooltip.Append(pMain.pTables.ZoneTable.Rows[i]["a_name"] + "\n");
			}

			if (lZoneFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_zone_flag out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnAllowedZoneFlag, strTooltip.ToString());
			pToolTips[btnAllowedZoneFlag] = pToolTip;
#else
			tbAllowedZoneFlag.Text = strZoneFlag;
#endif
			/****************************************/
			string strItemFlag = pTempItemRow["a_flag"].ToString();

			btnItemFlag.Text = strItemFlag;

			strTooltip = new StringBuilder();
			long lItemFlag = Convert.ToInt64(strItemFlag);

			for (int i = 0; i < Defs.ItemFlags.Length; i++)
			{
				if ((lItemFlag & 1L << i) != 0)
					strTooltip.Append(Defs.ItemFlags[i] + "\n");
			}

			if (lItemFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_flag out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnItemFlag, strTooltip.ToString());
			pToolTips[btnItemFlag] = pToolTip;
			/****************************************/
			int nType = Convert.ToInt32(pTempItemRow["a_type_idx"]);

			if (nType < 0 || nType >= Defs.ItemTypesNSubTypes.Keys.Count)
			{
				cbTypeSelector.Enabled = false;
				cbTypeSelector.Text = "";

				cbSubTypeSelector.Items.Clear();
				cbSubTypeSelector.Enabled = false;
				cbSubTypeSelector.Text = "";

				pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_type_idx out of range.");
			}
			else
			{
				cbTypeSelector.Enabled = true;
				cbTypeSelector.SelectedIndex = nType;

				int nSubType = Convert.ToInt32(pTempItemRow["a_subtype_idx"]);

				if (nSubType < 0 || nSubType >= Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(nType)].Count)
				{
					cbSubTypeSelector.Items.Clear();
					cbSubTypeSelector.Enabled = false;
					cbSubTypeSelector.Text = "";

					pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_subtype_idx out of range.");
				}
				else
				{
					cbSubTypeSelector.SelectedIndex = nSubType;
				}
			}
			/****************************************/
			int nRvRValue = Convert.ToInt32(pTempItemRow["a_rvr_value"]);

			if (nRvRValue >= Defs.SyndicateTypesNGrades.Keys.Count)
			{
				pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_rvr_value out of range.");
			}
			else
			{
				cbRvRValueSelector.SelectedIndex = nRvRValue;

				if (nRvRValue == 0)
				{
					cbRvRGradeSelector.Items.Clear();
					cbRvRGradeSelector.Enabled = false;
					cbRvRGradeSelector.Text = "";
				}
				else
				{
					cbRvRGradeSelector.SelectedIndex = Convert.ToInt32(pTempItemRow["a_rvr_grade"]);
				}
			}
			/****************************************/
			tbEffectNormal.Text = pTempItemRow["a_effect_name"].ToString();
			tbEffectAttack.Text = pTempItemRow["a_attack_effect_name"].ToString();
			tbEffectDamage.Text = pTempItemRow["a_damage_effect_name"].ToString();
			/****************************************/
			for (int i = 1; i <= 6; i++)
				((TextBox)Controls.Find("tbVariation" + i, true)[0]).Text = pTempItemRow["a_origin_variation" + i].ToString();
			/****************************************/
			SetSetDataTexts();

			for (int i = 0; i <= 4; i++)
				((TextBox)Controls.Find("tbSet" + i, true)[0]).Text = pTempItemRow["a_set_" + i].ToString();
			/****************************************/
			for (int i = 0; i <= 4; i++)
				((TextBox)Controls.Find("tbOption" + i, true)[0]).Text = pTempItemRow["a_num_" + i].ToString();

			// Crafting
			int nSpecialSkillNeededID = Convert.ToInt32(pTempItemRow["a_need_sskill"]);
			string strSpecialSkillName = nSpecialSkillNeededID.ToString();

			if (nSpecialSkillNeededID != -1)
			{
				DataRow pSpecialSkillData = pMain.pTables.SpecialSkillTable.Select("a_index=" + nSpecialSkillNeededID).FirstOrDefault();

				if (pSpecialSkillData != null)
				{
					strSpecialSkillName += " - " + pSpecialSkillData["a_name_" + pMain.pSettings.WorkLocale];

					btnSkill1RequiredID.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSpecialSkillData["a_texture_id"].ToString(), Convert.ToInt32(pSpecialSkillData["a_texture_row"]), Convert.ToInt32(pSpecialSkillData["a_texture_col"])), new Size(24, 24));
				}
				else
				{
					pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_need_sskill: {nSpecialSkillNeededID} not exist.");
				}
			}

			btnSkill1RequiredID.Text = strSpecialSkillName;

			tbSkill1RequiredLevel.Text = pTempItemRow["a_need_sskill_level"].ToString();

#if ENABLE_SECOND_SKILL_TO_CRAFT
			nSpecialSkillNeededID = Convert.ToInt32(pTempItemRow["a_need_sskill2"]);
			strSpecialSkillName = nSpecialSkillNeededID.ToString();

			if (nSpecialSkillNeededID != -1)
			{
				DataRow pSpecialSkillData = pMain.pMainTables.SpecialSkillTable.Select("a_index=" + nSpecialSkillNeededID).FirstOrDefault();

				if (pSpecialSkillData != null)
				{
					strSpecialSkillName += " - " + pSpecialSkillData["a_name_" + pMain.pSettings.WorkLocale];

					btnSkill2RequiredID.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSpecialSkillData["a_texture_id"].ToString(), Convert.ToInt32(pSpecialSkillData["a_texture_row"]), Convert.ToInt32(pSpecialSkillData["a_texture_col"])), new Size(24, 24));
				}
				else
				{
					pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_need_sskill: {nSpecialSkillNeededID} not exist.");
				}
			}

			btnSkill2RequiredID.Text = strSpecialSkillName;

			tbSkill2RequiredLevel.Text = pTempItemRow["a_need_sskill_level"].ToString();
#endif
			/****************************************/
			DataRow pItemRow;
			Button btnObj;

			for (int i = 0; i < Defs.MAX_MAKE_ITEM_MATERIAL; i++)
			{
				int nRequiredItemID = Convert.ToInt32(pTempItemRow["a_need_item" + i]);
				string strRequiredItemID = nRequiredItemID.ToString();
				btnObj = (Button)Controls.Find($"btnItem{i}Required", true)[0];

				if (nRequiredItemID != -1)
				{
					pItemRow = pMain.pTables.ItemTable.Select($"a_index={nRequiredItemID}").FirstOrDefault();
					if (pItemRow != null)
					{
						strRequiredItemID += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale];

						btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
					}
					else
					{
						pMain.Logger(LogTypes.Error, $"Item Editor > Item: {nItemID} Error: a_need_item: {i} not exist.");
					}
				}

				btnObj.Text = strRequiredItemID;

				((TextBox)Controls.Find($"tbItem{i}RequiredAmount", true)[0]).Text = pTempItemRow["a_need_item_count" + i].ToString();
			}

			// Rare
			for (int i = 0; i < Defs.DEF_MAX_ORIGIN_OPTION; i++)
			{
				int nRareOptionProb = Convert.ToInt32(pTempItemRow["a_rare_prob_" + i]);

				((TextBox)Controls.Find("tbRareIndex" + i, true)[0]).Text = pTempItemRow["a_rare_index_" + i].ToString();

				((TextBox)Controls.Find("tbRareProb" + i, true)[0]).Text = nRareOptionProb.ToString();

				((Label)Controls.Find($"lRareProb{i}Percentage", true)[0]).Text = ((nRareOptionProb * 100.0f) / 10000.0f) + "%";
			}

			// Fortune
			if (bLoadFrompItemTable)
				LoadFortuneData(nItemID);

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

		private void btnReload_Click(object sender, EventArgs e)    // NOTE: Here is an example on how to manage the reloading of information from global tables.
		{
			void Reload()
			{
				MainList.Enabled = false;
				btnReload.Enabled = false;

				nSearchPosition = 0;

				pMain.pTables.ItemTable?.Dispose();
				pMain.pTables.ItemTable = null;

				pMain.pTables.ZoneTable?.Dispose();
				pMain.pTables.ZoneTable = null;

				pMain.pTables.SkillTable?.Dispose();
				pMain.pTables.SkillTable = null;

				pMain.pTables.SkillLevelTable?.Dispose();
				pMain.pTables.SkillLevelTable = null;

				pMain.pTables.SpecialSkillTable?.Dispose();
				pMain.pTables.SpecialSkillTable = null;

				pMain.pTables.RareOptionTable?.Dispose();
				pMain.pTables.RareOptionTable = null;

				pMain.pTables.StringTable?.Dispose();
				pMain.pTables.StringTable = null;

				if (pMain.pTables.ItemFortuneHeadTable != null)
				{
					pMain.pTables.ItemFortuneHeadTable.Dispose();
					pMain.pTables.ItemFortuneHeadTable = null;
				}

				if (pMain.pTables.ItemFortuneDataTable != null)
				{
					pMain.pTables.ItemFortuneDataTable.Dispose();
					pMain.pTables.ItemFortuneDataTable = null;
				}

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				ItemEditor_LoadAsync(sender, e);
			}

			var (bProceed, _) = CheckUnsavedChanges();

			if (bProceed)
			{
				bUnsavedChanges = false;

				Reload();
			}
		}

		public DataRow? CreateNewItem(bool bCallFromOutsideForm, string strName, int nType, int nSubType, int nNum0)
		{
			DataRow? pNewRow = null;
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bProceed)
			{
				int i, nNewItemID = -1;

				List<string> listUIntColumns = ["a_durability"];	// Here add all unsigned int columns.

				List<string> listIntColumns =	// Here add all int columns.
				[
					"a_index",
					"a_enable",
					"a_weight",
					"a_price",
					"a_level",
					"a_level2",
					"a_fame",
					"a_max_use",
					"a_grade",
					"a_type_idx",
					"a_subtype_idx",
					"a_job_flag",
#if !ALLOWED_ZONE_SYSTEM
					"a_zone_flag",
#endif
					"a_set_0",
					"a_set_1",
					"a_set_2",
					"a_set_3",
					"a_set_4",
					"a_num_0",
					"a_num_1",
					"a_num_2",
					"a_num_3",
					"a_num_4",
					"a_need_sskill",
#if ENABLE_SECOND_SKILL_TO_CRAFT
					"a_need_sskill2",
#endif
					"a_need_item0",
					"a_need_item1",
					"a_need_item2",
					"a_need_item3",
					"a_need_item4",
					"a_need_item5",
					"a_need_item6",
					"a_need_item7",
					"a_need_item8",
					"a_need_item9",
					"a_rare_index_0", "a_rare_prob_0",
					"a_rare_index_1", "a_rare_prob_1",
					"a_rare_index_2", "a_rare_prob_2",
					"a_rare_index_3", "a_rare_prob_3",
					"a_rare_index_4", "a_rare_prob_4",
					"a_rare_index_5", "a_rare_prob_5",
					"a_rare_index_6", "a_rare_prob_6",
					"a_rare_index_7", "a_rare_prob_7",
					"a_rare_index_8", "a_rare_prob_8",
					"a_rare_index_9", "a_rare_prob_9"
				];

				List<string> listUTinyIntColumns =  // Here add all unsigned tinyint columns.
				[
					"a_origin_variation1",
					"a_origin_variation2",
					"a_origin_variation3",
					"a_origin_variation4",
					"a_origin_variation5",
					"a_origin_variation6",
					"a_rvr_value",
					"a_rvr_grade",
					"a_castle_war"
				];

				List<string> listTinyIntColumns =	// Here add all tinyint columns.
				[
					"a_texture_id",
					"a_texture_row",
					"a_texture_col",
					"a_wearing",
					"a_need_sskill_level"
#if ENABLE_SECOND_SKILL_TO_CRAFT
					, "a_need_sskill_level2"
#endif
				];

				List<string> listVarcharColumns =   // Here add all varchar columns.
				[
					"a_file_smc",
					"a_effect_name",
					"a_attack_effect_name",
					"a_damage_effect_name"
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listVarcharColumns.AddRange("a_name_" + strNation.ToLower(), "a_descr_" + strNation.ToLower());

				List<string> listBigIntColumns =	// Here add all bigint columns.
				[
#if ALLOWED_ZONE_SYSTEM
					"a_zone_flag",
#endif
					"a_flag"
				];

				List<string> listSmallIntColumns =	// Here add all smallint columns.
				[
					"a_need_item_count0",
					"a_need_item_count1",
					"a_need_item_count2",
					"a_need_item_count3",
					"a_need_item_count4",
					"a_need_item_count5",
					"a_need_item_count6",
					"a_need_item_count7",
					"a_need_item_count8",
					"a_need_item_count9"
				];

				if (pMain.pTables.ItemTable == null || bCallFromOutsideForm)    // If is null, create new DataTable and set schema (column name & datatype).
				{
					DataTable pItemTableStruct = new();

					foreach (string strColumnName in listUIntColumns)
						pItemTableStruct.Columns.Add(strColumnName, typeof(uint));

					foreach (string strColumnName in listIntColumns)
						pItemTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listUTinyIntColumns)
						pItemTableStruct.Columns.Add(strColumnName, typeof(byte));

					foreach (string strColumnName in listTinyIntColumns)
						pItemTableStruct.Columns.Add(strColumnName, typeof(sbyte));

					foreach (string strColumnName in listVarcharColumns)
						pItemTableStruct.Columns.Add(strColumnName, typeof(string));

					foreach (string strColumnName in listBigIntColumns)
						pItemTableStruct.Columns.Add(strColumnName, typeof(long));

					foreach (string strColumnName in listSmallIntColumns)
						pItemTableStruct.Columns.Add(strColumnName, typeof(short));

					pNewRow = pItemTableStruct.NewRow();

					pItemTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_item ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewItemID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewItemID = pMain.AskForIndex(this.Text, "a_index")) == -1)	// I don't test it...
							return null;
					}
				}
				else
				{
					nNewItemID = Convert.ToInt32(pMain.pTables.ItemTable.Select().LastOrDefault()["a_index"]) + 1;

					pNewRow = pMain.pTables.ItemTable.NewRow();
				}

				List<object> listDefaultValue =
				[
					0,	// a_durability
					nNewItemID,	// a_index
					0,	// a_enable
					0,	// a_weight
					0,	// a_price
					0,	// a_level
					0,	// a_level2
					-1,	// a_fame
					-1,	// a_max_use
					0,	// a_grade
					nType,	// a_type_idx
					nSubType,	// a_subtype_idx
					0,	// a_job_flag
#if !ALLOWED_ZONE_SYSTEM
					0,	// a_zone_flag
#endif
					0,	// a_set_0
					0,	// a_set_1
					0,	// a_set_2
					0,	// a_set_3
					0,	// a_set_4
					nNum0,	// a_num_0
					0,	// a_num_1
					0,	// a_num_2
					0,	// a_num_3
					0,	// a_num_4
					-1,	// a_need_sskill
#if ENABLE_SECOND_SKILL_TO_CRAFT
					-1,	// a_need_sskill2
#endif
					-1,	// a_need_item0
					-1,	// a_need_item1
					-1,	// a_need_item2
					-1,	// a_need_item3
					-1,	// a_need_item4
					-1,	// a_need_item5
					-1,	// a_need_item6
					-1,	// a_need_item7
					-1,	// a_need_item8
					-1,	// a_need_item9
					-1,	// a_rare_index_0
					0,	// a_rare_prob_0
					-1,	// a_rare_index_1
					0,	// a_rare_prob_1
					-1,	// a_rare_index_2
					0,	// a_rare_prob_2
					-1,	// a_rare_index_3
					0,	// a_rare_prob_3
					-1,	// a_rare_index_4
					0,	// a_rare_prob_4
					-1,	// a_rare_index_5
					0,	// a_rare_prob_5
					-1,	// a_rare_index_6
					0,	// a_rare_prob_6
					-1,	// a_rare_index_7
					0,	// a_rare_prob_7
					-1,	// a_rare_index_8
					0,	// a_rare_prob_8
					-1,	// a_rare_index_9
					0,	// a_rare_prob_9
					0,	// a_origin_variation1
					0,	// a_origin_variation2
					0,	// a_origin_variation3
					0,	// a_origin_variation4
					0,	// a_origin_variation5
					0,	// a_origin_variation6
					0,	// a_rvr_value
					0,	// a_rvr_grade
					0,	// a_castle_war
					0,	// a_texture_id
					1,	// a_texture_row
					1,	// a_texture_col
					-1,	// a_wearing
					0,	// a_need_sskill_level
#if ENABLE_SECOND_SKILL_TO_CRAFT
					0,	// a_need_sskill_level2
#endif
					"Item\\Common\\ITEM_treasure02.smc",	// a_file_smc
					"",	// a_effect_name
					"",	// a_attack_effect_name
					""	// a_damage_effect_name
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listDefaultValue.AddRange(strName, "Created with NicolasG LastChaos ToolBox");

				listDefaultValue.AddRange(
#if ALLOWED_ZONE_SYSTEM
					0,  // a_zone_flag
#endif
					0,  // a_flag
					0,  // a_need_item_count0
					0,  // a_need_item_count1
					0,  // a_need_item_count2
					0,  // a_need_item_count3
					0,  // a_need_item_count4
					0,  // a_need_item_count5
					0,  // a_need_item_count6
					0,  // a_need_item_count7
					0,  // a_need_item_count8
					0   // a_need_item_count9
				);

				i = 0;
				foreach (string strColumnName in listUIntColumns.Concat(listIntColumns).Concat(listUTinyIntColumns).Concat(listTinyIntColumns).Concat(listVarcharColumns).Concat(listBigIntColumns).Concat(listSmallIntColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					if (!bCallFromOutsideForm)
						pTempItemRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"Item Editor > Item: {nNewItemID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Item Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					return null;
				}
				finally
				{
					if (!bCallFromOutsideForm)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewItemID, strName, true);
					}
				}
			}

			return pNewRow;
		}

		private void btnAddNew_Click(object sender, EventArgs e) { CreateNewItem(false, "New Item", 0, 0, 0); }

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Item. Because it's temporary.", "Item Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nItemIDToCopy = Convert.ToInt32(pTempItemRow["a_index"]);
				int nNewItemID = Convert.ToInt32(pMain.pTables.ItemTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempItemRow = pMain.pTables.ItemTable.NewRow();
				pTempItemRow.ItemArray = (object[])pMain.pTables.ItemTable.Select("a_index=" + nItemIDToCopy)[0].ItemArray.Clone();

				pTempItemRow["a_index"] = nNewItemID;

				foreach (string strNation in pMain.pSettings.NationSupported)
					pTempItemRow["a_name_" + strNation.ToLower()] = pTempItemRow["a_name_" + strNation.ToLower()] + " Copy";

				AddToList(nNewItemID, pTempItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, true);

				LoadFortuneData(nItemIDToCopy);

				if (pTempFortuneHeadRow != null)
				{
					DataRow newRow = pMain.pTables.ItemFortuneHeadTable.NewRow();
					newRow.ItemArray = (object[])pTempFortuneHeadRow.ItemArray.Clone();
					newRow["a_item_idx"] = nNewItemID;

					pTempFortuneHeadRow = newRow;
				}

				if (pTempFortuneDataRows != null)
				{
					DataRow[] clonedRows = new DataRow[pTempFortuneDataRows.Length];
					for (int i = 0; i < pTempFortuneDataRows.Length; i++)
					{
						DataRow newRow = pMain.pTables.ItemFortuneDataTable.NewRow();
						newRow.ItemArray = (object[])pTempFortuneDataRows[i].ItemArray.Clone();
						newRow["a_item_idx"] = nNewItemID;
						clonedRows[i] = newRow;
					}

					pTempFortuneDataRows = clonedRows;
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nItemID = Convert.ToInt32(pTempItemRow["a_index"]);
			DataRow? pItemRow = pMain.pTables.ItemTable?.Select("a_index=" + nItemID).FirstOrDefault();

			string[] NPCJobDropColumns = {
				"a_titan_item",
				"a_knight_item",
				"a_healer_item",
				"a_mage_item",
				"a_rogue_item",
				"a_sorcerer_item",
				"a_nightshadow_item",
				"a_exrogue_item",
				"a_exmage_item"
			};

			string[] ItemCollectionColumns = {
				"a_need1_index",
				"a_need2_index",
				"a_need3_index",
				"a_need4_index",
				"a_need5_index",
				"a_need6_index",
				"a_result_index"
			};

			string[] ItemExchangeColumns =
			{
				"result_itemIndex",
				"source_itemIndex0",
				"source_itemIndex1",
				"source_itemIndex2",
				"source_itemIndex3",
				"source_itemIndex4"
			};

			string[] QuestColumns =
			{
				"a_need_item0",
				"a_need_item1",
				"a_need_item2",
				"a_need_item3",
				"a_need_item4",
				"a_condition0_index",
				"a_condition1_index",
				"a_condition2_index",
				"a_prize_index0",
				"a_prize_index1",
				"a_prize_index2",
				"a_prize_index3",
				"a_prize_index4",
				"a_opt_prize_index0",
				"a_opt_prize_index1",
				"a_opt_prize_index2",
				"a_opt_prize_index3",
				"a_opt_prize_index4",
				"a_opt_prize_index5",
				"a_opt_prize_index6"
			};

			string[] SkillLevelColumns =
			{
				"a_needItemIndex1",
				"a_needItemIndex2",
				"a_learnItemIndex1",
				"a_learnItemIndex2",
				"a_learnItemIndex3"
			};

			if (pItemRow != null)
			{
				StringBuilder strbuilderQuery = new();

				strbuilderQuery.Append("START TRANSACTION;\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_fortune_head WHERE a_item_idx={nItemID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_fortune_data WHERE a_item_idx={nItemID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_dropjob WHERE {string.Join(" OR ", NPCJobDropColumns.Select(col => $"{col}={nItemID}"))};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_dropraid WHERE a_item_index={nItemID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_drop_all WHERE a_item_idx={nItemID};\n");
#if DAILY_REWARD_SYSTEM
				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_daily_reward_item WHERE a_item_idx={nItemID};\n");
#endif
				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_affinity_reward_item WHERE a_itemidx={nItemID};\n");

				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_factory_item SET a_enable=0 WHERE a_item_idx={nItemID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_holy_water WHERE a_item_index={nItemID};\n");

				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_title SET a_enable=0 WHERE a_item_index={nItemID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_shopitem WHERE a_item_idx={nItemID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_moonstone_reward WHERE a_giftindex={nItemID};\n");

				strbuilderQuery.Append("CREATE TEMPORARY TABLE Temp_DropIDToDelete (a_drop_idx INT);\n" +
					$"INSERT INTO Temp_DropIDToDelete(a_drop_idx) SELECT DISTINCT a_drop_idx FROM {pMain.pSettings.DBData}.t_drop_item_data WHERE a_item_idx={nItemID};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_drop_item_data WHERE a_item_idx={nItemID};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_drop_item_head WHERE a_drop_idx IN(SELECT a_drop_idx FROM Temp_DropIDToDelete);\n" +
					"DROP TEMPORARY TABLE Temp_DropIDToDelete;\n");

				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item_collection SET a_enable=0 WHERE {string.Join(" OR ", ItemCollectionColumns.Select(col => $"{col}={nItemID}"))};\n");

				strbuilderQuery.Append("CREATE TEMPORARY TABLE Temp_RewardIDToDelete (a_reward_idx INT);\n" +
					$"INSERT INTO Temp_RewardIDToDelete (a_reward_idx) SELECT DISTINCT a_reward_idx FROM {pMain.pSettings.DBData}.t_reward_data WHERE a_idx={nItemID};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_reward_data WHERE a_idx={nItemID};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_reward_head WHERE a_reward_idx IN(SELECT a_reward_idx FROM Temp_RewardIDToDelete);\n" +
					"DROP TEMPORARY TABLE Temp_RewardIDToDelete;\n");

				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item_exchange SET a_enable=0 WHERE {string.Join(" OR ", ItemExchangeColumns.Select(col => $"{col}={nItemID}"))};\n");

				strbuilderQuery.Append("CREATE TEMPORARY TABLE Temp_CatalogIDToDelete (a_ctid INT);\n" +
					$"INSERT INTO Temp_CatalogIDToDelete (a_ctid) SELECT DISTINCT a_ctid FROM {pMain.pSettings.DBData}.t_ct_item WHERE a_item_idx={nItemID};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_ct_item WHERE a_item_idx={nItemID};\n" +
					$"UPDATE {pMain.pSettings.DBData}.t_catalog SET a_enable=0 WHERE a_ctid IN(SELECT a_ctid FROM Temp_CatalogIDToDelete);\n" +
					"DROP TEMPORARY TABLE Temp_CatalogIDToDelete;\n");

#if REWORKED_EXCHANGE_SYSTEM
				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_equipment_exchange WHERE a_items_idxs LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %') OR a_items_idxs LIKE CONCAT(CAST({nItemID} AS CHAR), ' %') OR a_items_idxs LIKE CONCAT('% ', CAST({nItemID} AS CHAR)) OR a_items_idxs=CAST({nItemID} AS CHAR);\n");
#endif
#if REWORKED_EVENT_PACKAGE_ITEM
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_key SET a_enable=0 WHERE a_rewards LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %') OR a_rewards LIKE CONCAT(CAST({nItemID} AS CHAR), ' %') OR a_rewards LIKE CONCAT('% ', CAST({nItemID} AS CHAR)) OR a_rewards=CAST({nItemID} AS CHAR);\n");
#endif
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_set_item SET a_enable=0 WHERE a_item_idx LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %') OR a_item_idx LIKE CONCAT(CAST({nItemID} AS CHAR), ' %') OR a_item_idx LIKE CONCAT('% ', CAST({nItemID} AS CHAR)) OR a_item_idx=CAST({nItemID} AS CHAR);\n");

				// NOTE: This probably is so bad idea, but, who cares.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_lacarette SET a_enable=0 WHERE " +
					$"a_coinIndex LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_coinIndex LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_coinIndex LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_coinIndex=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_1 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_1 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_1 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_1=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_2 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_2 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_2 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_2=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_3 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_3 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_3 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_3=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_4 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_4 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_4 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_4=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_5 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_5 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_5 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_5=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_6 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_6 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_6 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_6=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_7 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_7 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_7 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_7=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_8 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_8 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_8 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_8=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_9 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_9 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_9 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_9=CAST({nItemID} AS CHAR)" +
					$"OR a_giveItem_10 LIKE CONCAT('% ', CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_10 LIKE CONCAT(CAST({nItemID} AS CHAR), ' %')" +
					$"OR a_giveItem_10 LIKE CONCAT('% ', CAST({nItemID} AS CHAR))" +
					$"OR a_giveItem_10=CAST({nItemID} AS CHAR);\n");

				strbuilderQuery.Append("CREATE TEMPORARY TABLE Temp_LuckyDrawIDToDelete (a_luckydraw_idx INT);\n" +
					$"INSERT INTO Temp_LuckyDrawIDToDelete (a_luckydraw_idx) SELECT DISTINCT a_luckydraw_idx FROM {pMain.pSettings.DBData}.t_luckydrawneed WHERE a_item_idx={nItemID};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_luckydrawneed WHERE a_item_idx={nItemID};\n" +
					$"INSERT INTO Temp_LuckyDrawIDToDelete (a_luckydraw_idx) SELECT DISTINCT a_luckydraw_idx FROM {pMain.pSettings.DBData}.t_luckydrawresult WHERE a_item_idx={nItemID};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_luckydrawresult WHERE a_item_idx={nItemID};\n" +
					$"UPDATE {pMain.pSettings.DBData}.t_luckydrawbox SET a_enable=0 WHERE a_index IN(SELECT a_luckydraw_idx FROM Temp_LuckyDrawIDToDelete);\n" +
					"DROP TEMPORARY TABLE Temp_LuckyDrawIDToDelete;\n");
				
				// NOTE: Just disable it.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_quest SET a_enable=0 WHERE {string.Join(" OR ", QuestColumns.Select(col => $"{col}={nItemID}"))};\n");

				// NOTE: If there no more levels from same skill, delete the skill itself.
				strbuilderQuery.Append("CREATE TEMPORARY TABLE Temp_SkillIDToDisable (a_index INT);\n" +
					$"INSERT INTO Temp_SkillIDToDisable (a_index) SELECT DISTINCT a_index FROM {pMain.pSettings.DBData}.t_skilllevel WHERE {string.Join(" OR ", SkillLevelColumns.Select(col => $"{col}={nItemID}"))};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_skilllevel WHERE {string.Join(" OR ", SkillLevelColumns.Select(col => $"{col}={nItemID}"))};\n" +
					$"DELETE FROM {pMain.pSettings.DBData}.t_skill WHERE a_index IN(SELECT a_index FROM Temp_SkillIDToDisable)AND NOT EXISTS(SELECT 1 FROM t_skilllevel WHERE a_index = t_skill.a_index);\n" +
					"DROP TEMPORARY TABLE Temp_SkillIDToDisable;\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_item WHERE a_index={nItemID};\n");

				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _)))
				{
					string strError = $"Item Editor > Item: {nItemID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Item Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pMain.pTables.ItemFortuneHeadTable != null)
					{
						DataRow? pRow = pMain.pTables.ItemFortuneHeadTable.Select("a_item_idx=" + nItemID).FirstOrDefault();
						if (pRow != null)
							pMain.pTables.ItemFortuneHeadTable.Rows.Remove(pRow);
					}

					if (pMain.pTables.ItemFortuneDataTable != null)
					{
						DataRow[] pRows = pMain.pTables.ItemFortuneDataTable.Select("a_item_idx=" + nItemID);

						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.ItemFortuneDataTable.Rows.Remove(pRow);
						}
					}

					if (pMain.pTables.NPCDropJobTable != null)
					{
						DataRow? pRow = pMain.pTables.NPCDropJobTable.Select(string.Join(" OR ", NPCJobDropColumns.Select(col => $"{col} = {nItemID}"))).FirstOrDefault();
						if (pRow != null)
							pMain.pTables.NPCDropJobTable.Rows.Remove(pRow);
					}

					if (pMain.pTables.NPCDropRaidTable != null)
					{
						DataRow[] pRows = pMain.pTables.NPCDropRaidTable.Select("a_item_index=" + nItemID);

						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.NPCDropRaidTable.Rows.Remove(pRow);
						}
					}

					if (pMain.pTables.NPCDropAllTable != null)
					{
						DataRow[] pRows = pMain.pTables.NPCDropAllTable.Select("a_item_idx=" + nItemID);

						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.NPCDropAllTable.Rows.Remove(pRow);
						}
					}
#if DAILY_REWARD_SYSTEM
					if (pMain.pTables.DailyRewardTable != null)
					{
						DataRow[] pRows = pMain.pTables.DailyRewardTable.Select("a_item_idx=" + nItemID);

						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.DailyRewardTable.Rows.Remove(pRow);
						}
					}
#endif
					if (pItemRow != null)
						pMain.pTables.ItemTable.Rows.Remove(pItemRow);
				}
				catch (Exception ex)
				{
					string strError = $"Item Editor > Item: {nItemID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Item Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Item Deleted successfully!", "Item Editor", MessageBoxButtons.OK);

						MainList.SelectedIndex = nPrevObjectID;

						bUnsavedChanges = false;
					}
				}
			}
		}

		private void cbEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				string strEnable = "0";

				if (cbEnable.Checked)
					strEnable = "1";

				pTempItemRow["a_enable"] = strEnable;

				bUnsavedChanges = true;
			}
		}

		private void pbIcon_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				IconPicker pIconSelector = new(pMain, this, "ItemBtn");
				if (pIconSelector.ShowDialog() != DialogResult.OK)
					return;

				string[] strReturns = pIconSelector.ReturnValues;

				pTempItemRow["a_texture_id"] = strReturns[0];
				pTempItemRow["a_texture_row"] = strReturns[1];
				pTempItemRow["a_texture_col"] = strReturns[2];

				pbIcon.Image = pMain.GetIcon("ItemBtn", strReturns[0], Convert.ToInt32(strReturns[1]), Convert.ToInt32(strReturns[2]));

				pToolTips[pbIcon].SetToolTip(pbIcon, $"FILE: {strReturns[0]} ROW: {strReturns[1]} COL: " + strReturns[2]);

				bUnsavedChanges = true;
			}
		}

		private void tbSMC_DoubleClick(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				OpenFileDialog pFileDialog = new OpenFileDialog { Title = "Item Editor", Filter = "SMC Files|*.smc", InitialDirectory = pMain.pSettings.ClientPath + "\\Data" };
				if (pFileDialog.ShowDialog() == DialogResult.OK)
				{
					tbSMC.Text = pFileDialog.FileName.Replace(pMain.pSettings.ClientPath + "\\", "");

					pTempItemRow["a_file_smc"] = tbSMC.Text;

					bUnsavedChanges = true;
				}
			}
		}

		private void tbMaxStack_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_weight"] = tbMaxStack.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPrice_TextChanged(object sender, EventArgs e)
		{
			tbPrice.ForeColor = pMain.GetGoldColor(Convert.ToInt64(tbPrice.Text));

			if (bUserAction)
			{
				pTempItemRow["a_price"] = tbPrice.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbMinLevel_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_level"] = tbMinLevel.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbMaxLevel_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_level2"] = tbMaxLevel.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbDurability_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_durability"] = tbDurability.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbFame_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_fame"] = tbFame.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbMaxUse_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_max_use"] = tbMaxUse.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbGrade_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbGrade.SelectedIndex;

				if (nType != -1)
				{
					pTempItemRow["a_grade"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbNationSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				bUserAction = false;

				string strNation = cbNationSelector.SelectedItem.ToString().ToLower();

				tbName.Text = pTempItemRow["a_name_" + strNation].ToString();

				tbDescription.Text = pTempItemRow["a_descr_" + strNation].ToString();

				bUserAction = true;
			}
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_name_" + cbNationSelector.SelectedItem?.ToString()?.ToLower()] = tbName.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbDescription_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_descr_" + cbNationSelector.SelectedItem?.ToString()?.ToLower()] = tbDescription.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbCastleType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbTypeSelector.SelectedIndex;

				if (nType != -1)
				{
					pTempItemRow["a_castle_war"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void btnClassFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.ItemClass, Convert.ToInt32(btnClassFlag.Text));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnClassFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();

				for (int i = 0; i < Defs.ItemClass.Length; i++)
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(Defs.ItemClass[i] + "\n");
				}

				pToolTips[btnClassFlag].SetToolTip(btnClassFlag, strTooltip.ToString());

				pTempItemRow["a_job_flag"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void btnAllowedZoneFlag_Click(object sender, EventArgs e)
		{
#if ALLOWED_ZONE_SYSTEM
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, strZones, Convert.ToInt64(btnAllowedZoneFlag.Text));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnAllowedZoneFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();

				for (int i = 0; i < pMain.pTables.ZoneTable.Rows.Count; i++)
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(pMain.pTables.ZoneTable.Rows[i]["a_name"] + "\n");
				}

				pToolTips[btnAllowedZoneFlag].SetToolTip(btnAllowedZoneFlag, strTooltip.ToString());

				pTempItemRow["a_zone_flag"] = lFlag;

				bUnsavedChanges = true;
			}
#endif
		}

		private void tbAllowedZoneFlag_TextChanged(object sender, EventArgs e)
		{
#if !ALLOWED_ZONE_SYSTEM
			if (bUserAction)
			{
				pTempItemRow["a_zone_flag"] = tbAllowedZoneFlag.Text;

				bUnsavedChanges = true;
			}
#endif
		}

		private void btnItemFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.ItemFlags, Convert.ToInt64(btnItemFlag.Text));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnItemFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();

				for (int i = 0; i < Defs.ItemFlags.Length; i++)
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(Defs.ItemFlags[i] + "\n");
				}

				pToolTips[btnItemFlag].SetToolTip(btnItemFlag, strTooltip.ToString());

				pTempItemRow["a_flag"] = lFlag;

				bUnsavedChanges = true;
			}

			SetSetDataTexts();
		}

		private void cbTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nType = cbTypeSelector.SelectedIndex;
			if (nType != -1)
			{
				cbSubTypeSelector.Enabled = false;
				cbSubTypeSelector.Items.Clear();
				cbSubTypeSelector.BeginUpdate();

				foreach (string strSubType in Defs.ItemTypesNSubTypes[Defs.ItemTypesNSubTypes.Keys.ElementAt(nType)])
					cbSubTypeSelector.Items.Add(strSubType);

				cbSubTypeSelector.EndUpdate();
				cbSubTypeSelector.Enabled = true;
				
				SetOptionDataTexts();
				
				if (bUserAction)
				{
					pTempItemRow["a_type_idx"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbSubTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbSubTypeSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempItemRow["a_subtype_idx"] = nType;

					bUnsavedChanges = true;
				}
			}

			SetOptionDataTexts();
		}

		private void cbWearingPositionSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbWearingPositionSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempItemRow["a_wearing"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbRvRValueSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nType = cbRvRValueSelector.SelectedIndex;
			if (nType > 0)
			{
				cbRvRGradeSelector.Enabled = false;

				cbRvRGradeSelector.Items.Clear();
				cbRvRGradeSelector.BeginUpdate();

				foreach (string strGrade in Defs.SyndicateTypesNGrades[Defs.SyndicateTypesNGrades.Keys.ElementAt(nType)])
					cbRvRGradeSelector.Items.Add(strGrade);

				cbRvRGradeSelector.EndUpdate();
				cbRvRGradeSelector.Enabled = true;
			}
			else
			{
				cbRvRGradeSelector.Enabled = false;

				cbRvRGradeSelector.Items.Clear();
			}

			if (bUserAction)
			{
				pTempItemRow["a_rvr_value"] = nType;

				bUnsavedChanges = true;
			}
		}

		private void cbRvRGradeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbRvRGradeSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempItemRow["a_rvr_grade"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void tbEffectNormal_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_effect_name"] = tbEffectNormal.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbEffectAttack_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_attack_effect_name"] = tbEffectAttack.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbEffectDamage_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_damage_effect_name"] = tbEffectDamage.Text;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void ChangeVariation(int nNumber)
		{
			if (bUserAction)
			{
				pTempItemRow["a_origin_variation" + nNumber] = ((TextBox)Controls.Find("tbVariation" + nNumber, true)[0]).Text;

				bUnsavedChanges = true;
			}
		}

		private void tbVariation1_TextChanged(object sender, EventArgs e) { ChangeVariation(1); }
		private void tbVariation2_TextChanged(object sender, EventArgs e) { ChangeVariation(2); }
		private void tbVariation3_TextChanged(object sender, EventArgs e) { ChangeVariation(3); }
		private void tbVariation4_TextChanged(object sender, EventArgs e) { ChangeVariation(4); }
		private void tbVariation5_TextChanged(object sender, EventArgs e) { ChangeVariation(5); }
		private void tbVariation6_TextChanged(object sender, EventArgs e) { ChangeVariation(6); }
		/****************************************/
		private void ChangeSet(int nNumber)
		{
			if (bUserAction)
			{
				pTempItemRow["a_set_" + nNumber] = ((TextBox)Controls.Find("tbSet" + nNumber, true)[0]).Text;

				bUnsavedChanges = true;
			}
		}

		private void tbSet0_TextChanged(object sender, EventArgs e) { ChangeSet(0); }
		private void tbSet1_TextChanged(object sender, EventArgs e) { ChangeSet(1); }
		private void tbSet2_TextChanged(object sender, EventArgs e) { ChangeSet(2); }
		private void tbSet3_TextChanged(object sender, EventArgs e) { ChangeSet(3); }
		private void tbSet4_TextChanged(object sender, EventArgs e) { ChangeSet(4); }
		/****************************************/
		private void ChangeOption(int nNumber)
		{
			if (bUserAction)
			{
				pTempItemRow["a_num_" + nNumber] = ((TextBox)Controls.Find("tbOption" + nNumber, true)[0]).Text;

				bUnsavedChanges = true;
			}
		}

		private void tbOption0_TextChanged(object sender, EventArgs e) { ChangeOption(0); }
		private void tbOption1_TextChanged(object sender, EventArgs e) { ChangeOption(1); }
		private void tbOption2_TextChanged(object sender, EventArgs e) { ChangeOption(2); }
		private void tbOption3_TextChanged(object sender, EventArgs e) { ChangeOption(3); }
		private void tbOption4_TextChanged(object sender, EventArgs e) { ChangeOption(4); }
		/****************************************/
		private void btnSkill1RequiredID_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				string strIDColumn = "a_need_sskill";
				string strLevelColumn = "a_need_sskill_level";

				SpecialSkillPicker pSpecialSkillSelector = new(pMain, this, [pTempItemRow[strIDColumn], pTempItemRow[strLevelColumn]]);
				if (pSpecialSkillSelector.ShowDialog() != DialogResult.OK)
					return;

				int nSkillNeededID = Convert.ToInt32(pSpecialSkillSelector.ReturnValues[0]);
				string strSkillLevelNeeded = pSpecialSkillSelector.ReturnValues[1].ToString() ?? "0";
				string strSkillName = nSkillNeededID.ToString();

				if (nSkillNeededID > 0)
				{
					btnSkill1RequiredID.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSpecialSkillSelector.ReturnValues[4].ToString(), Convert.ToInt32(pSpecialSkillSelector.ReturnValues[5]), Convert.ToInt32(pSpecialSkillSelector.ReturnValues[6])), new Size(24, 24));

					strSkillName += " - " + pSpecialSkillSelector.ReturnValues[2];
				}
				else
				{
					btnSkill1RequiredID.Image = null;
				}

				btnSkill1RequiredID.Text = strSkillName;

				tbSkill1RequiredLevel.Text = strSkillLevelNeeded;

				pTempItemRow[strIDColumn] = nSkillNeededID.ToString();
				pTempItemRow[strLevelColumn] = strSkillLevelNeeded;

				bUnsavedChanges = true;
			}
		}

		private void tbSkill1RequiredLevel_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempItemRow["a_need_sskill_level"] = tbSkill1RequiredLevel.Text;

				bUnsavedChanges = true;
			}
		}

		private void btnSkill2RequiredID_Click(object sender, EventArgs e)
		{
#if ENABLE_SECOND_SKILL_TO_CRAFT
			if (bUserAction)
			{
				string strIDColumn = "a_need_sskill2";
				string strLevelColumn = "a_need_sskill_level2";

				SpecialSkillPicker pSpecialSkillSelector = new(pMain, this, [pTempItemRow[strIDColumn], pTempItemRow[strLevelColumn]]);
				if (pSpecialSkillSelector.ShowDialog() != DialogResult.OK)
					return;

				int nSkillNeededID = Convert.ToInt32(pSpecialSkillSelector.ReturnValues[0]);
				string strSkillLevelNeeded = pSpecialSkillSelector.ReturnValues[1].ToString() ?? "0";
				string strSkillName = nSkillNeededID.ToString();

				if (nSkillNeededID > 0)
				{
					strSkillName += " - " + pSpecialSkillSelector.ReturnValues[2];

					btnSkill2RequiredID.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSpecialSkillSelector.ReturnValues[4].ToString(), Convert.ToInt32(pSpecialSkillSelector.ReturnValues[5]), Convert.ToInt32(pSpecialSkillSelector.ReturnValues[6])), new Size(24, 24));
				}
				else
				{
					btnSkill2RequiredID.Image = null;
				}

				btnSkill2RequiredID.Text = strSkillName;

				tbSkill2RequiredLevel.Text = strSkillLevelNeeded;

				pTempItemRow[strIDColumn] = nSkillNeededID.ToString();
				pTempItemRow[strLevelColumn] = strSkillLevelNeeded;

				bUnsavedChanges = true;
			}
#endif
		}

		private void tbSkill2RequiredLevel_TextChanged(object sender, EventArgs e)
		{
#if ENABLE_SECOND_SKILL_TO_CRAFT
			if (bUserAction)
			{
				pTempItemRow["a_need_sskill_level2"] = tbSkill2RequiredLevel.Text;

				bUnsavedChanges = true;
			}
#endif
		}
		/****************************************/
		private void ChangeItemRequired(int nNumber)
		{
			if (bUserAction)
			{
				string strItemIDColumn = "a_need_item" + nNumber;
				Button btnObj = (Button)Controls.Find($"btnItem{nNumber}Required", true)[0];

				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempItemRow[strItemIDColumn]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nItemNeededID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				string strItemName = nItemNeededID.ToString();

				if (nItemNeededID > 0)
				{
					strItemName += $" - {pItemSelector.ReturnValues[1]}";

					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
				}
				else
				{
					btnObj.Image = null;
				}

				btnObj.Text = strItemName;

				((TextBox)Controls.Find($"tbItem{nNumber}RequiredAmount", true)[0]).Focus();

				pTempItemRow[strItemIDColumn] = nItemNeededID.ToString();

				bUnsavedChanges = true;
			}
		}

		private void btnItem0Required_Click(object sender, EventArgs e) { ChangeItemRequired(0); }
		private void btnItem1Required_Click(object sender, EventArgs e) { ChangeItemRequired(1); }
		private void btnItem2Required_Click(object sender, EventArgs e) { ChangeItemRequired(2); }
		private void btnItem3Required_Click(object sender, EventArgs e) { ChangeItemRequired(3); }
		private void btnItem4Required_Click(object sender, EventArgs e) { ChangeItemRequired(4); }
		private void btnItem5Required_Click(object sender, EventArgs e) { ChangeItemRequired(5); }
		private void btnItem6Required_Click(object sender, EventArgs e) { ChangeItemRequired(6); }
		private void btnItem7Required_Click(object sender, EventArgs e) { ChangeItemRequired(7); }
		private void btnItem8Required_Click(object sender, EventArgs e) { ChangeItemRequired(8); }
		private void btnItem9Required_Click(object sender, EventArgs e) { ChangeItemRequired(9); }

		private void ChangeItemRequiredAmount(int nNumber)
		{
			if (bUserAction)
			{
				pTempItemRow["a_need_item_count" + nNumber] = ((TextBox)Controls.Find($"tbItem{nNumber}RequiredAmount", true)[0]).Text;

				bUnsavedChanges = true;
			}
		}

		private void tbItem0RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(0); }
		private void tbItem1RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(1); }
		private void tbItem2RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(2); }
		private void tbItem3RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(3); }
		private void tbItem4RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(4); }
		private void tbItem5RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(5); }
		private void tbItem6RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(6); }
		private void tbItem7RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(7); }
		private void tbItem8RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(8); }
		private void tbItem9RequiredAmount_TextChanged(object sender, EventArgs e) { ChangeItemRequiredAmount(9); }
		/****************************************/
		private void ChangeRareOption(int nNumber)
		{
			if (bUserAction)
			{
				pTempItemRow["a_rare_index_" + nNumber] = ((TextBox)Controls.Find("tbRareIndex" + nNumber, true)[0]).Text;

				bUnsavedChanges = true;
			}
		}

		private void tbRareIndex0_TextChanged(object sender, EventArgs e) { ChangeRareOption(0); }
		private void tbRareIndex1_TextChanged(object sender, EventArgs e) { ChangeRareOption(1); }
		private void tbRareIndex2_TextChanged(object sender, EventArgs e) { ChangeRareOption(2); }
		private void tbRareIndex3_TextChanged(object sender, EventArgs e) { ChangeRareOption(3); }
		private void tbRareIndex4_TextChanged(object sender, EventArgs e) { ChangeRareOption(4); }
		private void tbRareIndex5_TextChanged(object sender, EventArgs e) { ChangeRareOption(5); }
		private void tbRareIndex6_TextChanged(object sender, EventArgs e) { ChangeRareOption(6); }
		private void tbRareIndex7_TextChanged(object sender, EventArgs e) { ChangeRareOption(7); }
		private void tbRareIndex8_TextChanged(object sender, EventArgs e) { ChangeRareOption(8); }
		private void tbRareIndex9_TextChanged(object sender, EventArgs e) { ChangeRareOption(9); }

		private void ChangeRareProb(int nNumber)
		{
			if (bUserAction)
			{
				string strProb = ((TextBox)Controls.Find("tbRareProb" + nNumber, true)[0]).Text;

				((Label)Controls.Find($"lRareProb{nNumber}Percentage", true)[0]).Text = ((Convert.ToInt32(strProb) * 100.0f) / 10000.0f) + "%";

				pTempItemRow["a_rare_prob_" + nNumber] = strProb;

				bUnsavedChanges = true;
			}
		}

		private void tbRareProb0_TextChanged(object sender, EventArgs e) { ChangeRareProb(0); }
		private void tbRareProb1_TextChanged(object sender, EventArgs e) { ChangeRareProb(1); }
		private void tbRareProb2_TextChanged(object sender, EventArgs e) { ChangeRareProb(2); }
		private void tbRareProb3_TextChanged(object sender, EventArgs e) { ChangeRareProb(3); }
		private void tbRareProb4_TextChanged(object sender, EventArgs e) { ChangeRareProb(4); }
		private void tbRareProb5_TextChanged(object sender, EventArgs e) { ChangeRareProb(5); }
		private void tbRareProb6_TextChanged(object sender, EventArgs e) { ChangeRareProb(6); }
		private void tbRareProb7_TextChanged(object sender, EventArgs e) { ChangeRareProb(7); }
		private void tbRareProb8_TextChanged(object sender, EventArgs e) { ChangeRareProb(8); }
		private void tbRareProb9_TextChanged(object sender, EventArgs e) { ChangeRareProb(9); }
		/****************************************/
		private void MakeTempFortuneHeadRow()
		{
			if (pTempFortuneHeadRow == null)
			{
				pTempFortuneHeadRow = pMain.pTables.ItemFortuneHeadTable.NewRow();
				pTempFortuneHeadRow["a_item_idx"] = pTempItemRow["a_index"];
				pTempFortuneHeadRow["a_prob_type"] = 0;
				pTempFortuneHeadRow["a_enable"] = 1;

				bUnsavedChanges = true;
			}
		}

		private void cbFortuneEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				MakeTempFortuneHeadRow();

				string strEnable = "0";

				if (cbFortuneEnable.Checked)
					strEnable = "1";

				pTempFortuneHeadRow["a_enable"] = strEnable;

				bUnsavedChanges = true;
			}
		}

		private void cbIFortuneProbType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				MakeTempFortuneHeadRow();

				int nType = cbFortuneProbType.SelectedIndex;

				if (nType != -1)
				{
					pTempFortuneHeadRow["a_prob_type"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		/*private void gridFortune_CellValueChanged(object sender, DataGridViewCellEventArgs e)	// NOTE: I disable it cuz looks like have more values involved in prob calc than only gridFortune.Rows[e.RowIndex].Cells["prob"].Value
		{
			if (bUserAction)
			{
				if (e.ColumnIndex == 3) // Skill Prob
					gridFortune.Rows[e.RowIndex].Cells["prob"].ToolTipText = ((Convert.ToInt32(gridFortune.Rows[e.RowIndex].Cells["prob"].Value) * 100.0f) / 10000.0f) + "%";

				bUnsavedChanges = true;
			}
		}*/

		private void gridFortune_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				MakeTempFortuneHeadRow();

				if (e.Button == MouseButtons.Left && e.ColumnIndex == 1 && e.RowIndex >= 0) // Skill Selector
				{
					int nSkillID = Convert.ToInt32(gridFortune.Rows[e.RowIndex].Cells["skill"].Tag);
					string strSkillLevel = gridFortune.Rows[e.RowIndex].Cells["level"].Tag.ToString() ?? "0";

					SkillPicker pSkillSelector = new(pMain, this, [nSkillID, strSkillLevel], false);
					if (pSkillSelector.ShowDialog() != DialogResult.OK)
						return;

					nSkillID = Convert.ToInt32(pSkillSelector.ReturnValues[0]);
					int nSelectedSkillLevel = Convert.ToInt32(pSkillSelector.ReturnValues[1]);
					strSkillLevel = pSkillSelector.ReturnValues[1].ToString() ?? "0";

					gridFortune.Rows[e.RowIndex].Cells["skillIcon"].Value = new Bitmap(pMain.GetIcon("SkillBtn", pSkillSelector.ReturnValues[4].ToString(), Convert.ToInt32(pSkillSelector.ReturnValues[5]), Convert.ToInt32(pSkillSelector.ReturnValues[6])));
					gridFortune.Rows[e.RowIndex].Cells["skill"].Value = nSkillID + " - " + pSkillSelector.ReturnValues[2];
					gridFortune.Rows[e.RowIndex].Cells["skill"].Tag = nSkillID;
					gridFortune.Rows[e.RowIndex].Cells["skill"].ToolTipText = pSkillSelector.ReturnValues[3].ToString();

					using (DataGridViewComboBoxCell cSkillLevel = (DataGridViewComboBoxCell)gridFortune.Rows[e.RowIndex].Cells["level"])
					{
						DataRow[] pSkillLevelRows = pMain.pTables.SkillLevelTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nSkillID).ToArray();

						foreach (DataRow pRowSkillLevel in pSkillLevelRows)
						{
							int nSkillLevel = Convert.ToInt32(pRowSkillLevel["a_level"]);

							int nLastItemAdded = cSkillLevel.Items.Add($"Level: {nSkillLevel} - Power: {pRowSkillLevel["a_dummypower"]}");

							if (nSelectedSkillLevel == nSkillLevel)
								cSkillLevel.Value = cSkillLevel.Items[nLastItemAdded];
						}
					}

					gridFortune.Rows[e.RowIndex].Cells["level"].Tag = nSelectedSkillLevel;

					bUnsavedChanges = true;
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 4 && e.RowIndex >= 0)    // String Selector
				{
					StringPicker pStringSelector = new(pMain, this, Convert.ToInt32(gridFortune.Rows[e.RowIndex].Cells["string_id"].Value), false);
					if (pStringSelector.ShowDialog() != DialogResult.OK)
						return;

					gridFortune.Rows[e.RowIndex].Cells["string_id"].Value = pStringSelector.ReturnValues[0];
					gridFortune.Rows[e.RowIndex].Cells["string_id"].ToolTipText = pStringSelector.ReturnValues[1].ToString();

					bUnsavedChanges = true;
				}
				else if (e.Button == MouseButtons.Right && e.ColumnIndex == -1) // Header Column
				{
					ToolStripMenuItem addItem = new("Add New");
					addItem.Click += (_, _) =>
					{
						bool bSuccess = true;
						SkillPicker pSkillSelector = new(pMain, this, [0, 1], false);
						if (pSkillSelector.ShowDialog() != DialogResult.OK)
							return;

						int nSkillID = Convert.ToInt32(pSkillSelector.ReturnValues[0]);
						int nSkillLevel = Convert.ToInt32(pSkillSelector.ReturnValues[1]);

						Image pIcon = new Bitmap(pMain.GetIcon("SkillBtn", pSkillSelector.ReturnValues[4].ToString(), Convert.ToInt32(pSkillSelector.ReturnValues[5]), Convert.ToInt32(pSkillSelector.ReturnValues[6])));
						/****************************************/
						StringPicker pStringSelector = new(pMain, this, 1, false);
						if (pStringSelector.ShowDialog() != DialogResult.OK)
							return;

						int nStringID = Convert.ToInt32(pStringSelector.ReturnValues[0]);
						string strString = pStringSelector.ReturnValues[1].ToString() ?? string.Empty;
						/****************************************/
						int nDefaultProb = 0;
						/****************************************/
						try
						{
							if (pTempFortuneDataRows == null)
								pTempFortuneDataRows = new DataRow[1];

							int nPosition = pTempFortuneDataRows.Length - 1;

							pTempFortuneDataRows[nPosition] = pMain.pTables.ItemFortuneDataTable.NewRow();

							pTempFortuneDataRows[nPosition]["a_item_idx"] = pTempItemRow["a_index"];
							pTempFortuneDataRows[nPosition]["a_skill_index"] = nSkillID;
							pTempFortuneDataRows[nPosition]["a_skill_level"] = nSkillLevel;
							pTempFortuneDataRows[nPosition]["a_string_index"] = nStringID;
							pTempFortuneDataRows[nPosition]["a_prob"] = nDefaultProb;
						}
						catch (Exception ex)
						{
							pMain.Logger(LogTypes.Error, $"Item Editor > {ex.Message}.");

							bSuccess = false;
						}
						finally
						{
							if (bSuccess)
							{
								int nRow = gridFortune.Rows.Count;

								gridFortune.Rows.Insert(nRow);

								gridFortune.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

								gridFortune.Rows[nRow].Cells["skillIcon"].Value = new Bitmap(pIcon, new Size(24, 24));
								gridFortune.Rows[nRow].Cells["skill"].Value = nSkillID + " - " + pSkillSelector.ReturnValues[2].ToString();
								gridFortune.Rows[nRow].Cells["skill"].Tag = nSkillID;
								gridFortune.Rows[nRow].Cells["skill"].ToolTipText = pSkillSelector.ReturnValues[3].ToString();

								using (DataGridViewComboBoxCell cSkillLevel = (DataGridViewComboBoxCell)gridFortune.Rows[nRow].Cells["level"])
								{
									DataRow[] pSkillLevelRows = pMain.pTables.SkillLevelTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nSkillID).ToArray();

									foreach (DataRow pRowSkillLevel in pSkillLevelRows)
									{
										int nFortuneSkillLevel = Convert.ToInt32(pRowSkillLevel["a_level"]);

										cSkillLevel.Items.Add($"Level: {nFortuneSkillLevel} - Power: " + pRowSkillLevel["a_dummypower"]);

										if (nSkillLevel == nFortuneSkillLevel)
											cSkillLevel.Value = cSkillLevel.Items[cSkillLevel.Items.Count - 1];
									}
								}

								gridFortune.Rows[nRow].Cells["level"].Tag = nSkillLevel;
								gridFortune.Rows[nRow].Cells["prob"].Value = nDefaultProb;
								//gridFortune.Rows[nRow].Cells["prob"].ToolTipText = ((nDefaultProb * 100.0f) / 10000.0f) + "%";	// NOTE: I disable it cuz looks like have more values involved in prob calc than only nDefaultProb
								gridFortune.Rows[nRow].Cells["string_id"].Value = nStringID;
								gridFortune.Rows[nRow].Cells["string_id"].ToolTipText = strString;

								gridFortune.FirstDisplayedScrollingRowIndex = nRow;
								gridFortune.Rows[nRow].Selected = true;

								bUnsavedChanges = true;
							}
						}
					};

					ToolStripMenuItem deleteItem = new("Delete") { Enabled = e.RowIndex >= 0 };
					deleteItem.Click += (_, _) =>
					{
						if (e.RowIndex >= 0)
						{
							bool bSuccess = true;
							try
							{
								DataRow pFortuneLastSkillRow = pTempFortuneDataRows.Cast<DataRow>().Where(row => row["a_skill_index"].ToString() == gridFortune.Rows[e.RowIndex].Cells["skill"].Tag.ToString()).LastOrDefault();
								if (pFortuneLastSkillRow != null)
									pTempFortuneDataRows.ElementAt(Array.IndexOf(pTempFortuneDataRows, pFortuneLastSkillRow)).Delete();
							}
							catch (Exception ex)
							{
								pMain.Logger(LogTypes.Error, $"Item Editor > {ex.Message}.");

								bSuccess = false;
							}
							finally
							{
								if (bSuccess)
								{
									gridFortune.SuspendLayout();

									gridFortune.Rows.RemoveAt(e.RowIndex);

									int i = 1;
									foreach (DataGridViewRow row in gridFortune.Rows)
									{
										row.HeaderCell.Value = i.ToString();

										i++;
									}

									gridFortune.ResumeLayout();

									bUnsavedChanges = true;
								}
							}
						}
					};

					cmFortune = new ContextMenuStrip();
					cmFortune.Items.AddRange(addItem, deleteItem);
					cmFortune.Show(Cursor.Position);
				}
			}
		}
		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int i = 0, nItemID = Convert.ToInt32(pTempItemRow["a_index"]);
			StringBuilder strbuilderQuery = new();

			// Init transaction.
			strbuilderQuery.Append("START TRANSACTION;\n");

			if (gridFortune.Rows.Count > 0)
			{
				// First clear and set size of DataRow Array
				pTempFortuneDataRows = new DataRow[gridFortune.Rows.Count];

				foreach (DataGridViewRow row in gridFortune.Rows)
				{
					DataRow? pRow = pMain.pTables.ItemFortuneDataTable?.NewRow();

					pRow["a_item_idx"] = pTempItemRow["a_index"];
					pRow["a_skill_index"] = row.Cells["skill"].Tag;
					pRow["a_skill_level"] = row.Cells["level"].Value.ToString().Split(" - ", StringSplitOptions.None)[0].Replace("Level: ", "").Trim(); // DUDE LOOK THAT SHIT HAHA, NOTE: in theory, the element index is equivalent to level, but i'm not trust so, by go in this way have not room to errors.	//row.Cells["level"].Tag;
					pRow["a_string_index"] = row.Cells["string_id"].Value;
					pRow["a_prob"] = row.Cells["prob"].Value;

					pTempFortuneDataRows[i] = pRow;

					i++;
				}
			}

			if (pTempFortuneHeadRow != null)
			{
				// Delete all rows in t_fortune_head related to nItemID.
				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_fortune_head WHERE a_item_idx={nItemID};\n");

				// Compose t_fortune_head INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempFortuneHeadRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append(pTempFortuneHeadRow[pCol] + ", ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_fortune_head ({strColumnsNames}) VALUES ({strColumnsValues});\n");

				if (pTempFortuneDataRows != null)
				{
					// Delete all rows in t_fortune_data related to nItemID.
					strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_fortune_data WHERE a_item_idx={nItemID};\n");

					// Compose t_fortune_data INSERT Query.
					strColumnsNames = new StringBuilder();
					strColumnsValues = new StringBuilder();
					HashSet<string> addedColumns = new();

					foreach (DataRow pRow in pTempFortuneDataRows)
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

					strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_fortune_data ({strColumnsNames}) VALUES {strColumnsValues};\n");
				}
			}

			// Check if Item exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pItemRow = pMain.pTables.ItemTable?.Select("a_index=" + nItemID).FirstOrDefault();
			if (pItemRow != null)  // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item SET");

				foreach (DataColumn pCol in pTempItemRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempItemRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nItemID};\n");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempItemRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempItemRow[pCol].ToString())}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_item ({strColumnsNames}) VALUES ({strColumnsValues});\n");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _))
			{
				try
				{
					// Transfer from pTempFortuneHead To pItemFortuneHeadTable
					if (pTempFortuneHeadRow != null)
					{
						DataRow pItemFortuneHeadTableRow = pMain.pTables.ItemFortuneHeadTable.Select("a_item_idx=" + nItemID).FirstOrDefault();

						if (pItemFortuneHeadTableRow != null)   // Row exist in Global Table.
						{
							pItemFortuneHeadTableRow.ItemArray = (object[])pTempFortuneHeadRow.ItemArray.Clone();
						}
						else    // Row not exist in Global Table.
						{
							pItemFortuneHeadTableRow = pMain.pTables.ItemFortuneHeadTable.NewRow();
							pItemFortuneHeadTableRow.ItemArray = (object[])pTempFortuneHeadRow.ItemArray.Clone();
							pMain.pTables.ItemFortuneHeadTable.Rows.Add(pItemFortuneHeadTableRow);
						}
					}

					// Transfer from pTempFortuneData To pItemFortuneDataTable
					if (pTempFortuneDataRows != null && pTempFortuneDataRows.Length > 0)
					{
						DataRow[] pItemFortuneDataTableRows = pMain.pTables.ItemFortuneDataTable.Select("a_item_idx=" + nItemID);
						foreach (DataRow pRow in pItemFortuneDataTableRows)
							pRow.Delete();

						foreach (DataRow pRow in pTempFortuneDataRows)
						{
							DataRow newDataRow = pMain.pTables.ItemFortuneDataTable.NewRow();
							newDataRow.ItemArray = (object[])pRow.ItemArray.Clone();
							pMain.pTables.ItemFortuneDataTable.Rows.Add(newDataRow);
						}

						pMain.pTables.ItemFortuneDataTable.AcceptChanges();
					}

					// Transfer from pTempItemRow To Global Table
					if (pItemRow != null)  // Row exist in Global Table, update it.
					{
						pItemRow.ItemArray = (object[])pTempItemRow.ItemArray.Clone();
					}
					else // Row not exist in Global Table, insert it.
					{
						// Creates a new empty DataRow using the schema of Global Table
						pItemRow = pMain.pTables.ItemTable.NewRow();

						// Copies all column values from the temporary row assuming identical column order
						pItemRow.ItemArray = (object[])pTempItemRow.ItemArray.Clone();

						// Adds the populated row to Global Table, triggering constraint validation
						pMain.pTables.ItemTable.Rows.Add(pItemRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"Item Editor > Item: {nItemID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Item Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nItemID;
						pSelectedItem.Text = nItemID + " - " + tbName.Text;

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "Item Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Item Editor > Item: {nItemID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Item Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
