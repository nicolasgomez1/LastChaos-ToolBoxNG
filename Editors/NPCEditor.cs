#define NPC_CHANNEL // NOTE: Custom system made by NicolasG
#define REWARDS_BY_DAMAGE // NOTE: Custom system made by NicolasG
//#define USE_A_POS_H_COLUMN_IN_NPC_REGEN // NOTE: This columns is not used (at least in 2018 leak)

// Adapted for the 1776 localhost schema. These custom systems require
// t_npc.a_channel_flag and t_npc.a_reward_gold_* columns, which this database lacks.
#undef NPC_CHANNEL
#undef REWARDS_BY_DAMAGE

using System.Drawing.Drawing2D;

namespace LastChaos_ToolBoxNG
{
	public partial class NPCEditor : Form
	{
		private readonly Main pMain;
		private RenderDialog? pRenderDialog;
		private bool bUserAction = false;
		private bool bSettingSMCText = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempNPCRow, pTempDropJobRow;
		private DataRow[]? pTempDropAllRows, pTempDropRaidRows, pTempRegenRows;
		private string[]? strZones;
		private ToolTip? pToolTip;
		private ToolTip? pRegenToolTip;
		private Dictionary<Control, ToolTip>? pToolTips = new();
		private ContextMenuStrip? cmDropRaid;
		private ContextMenuStrip? cmRegenSpots;
		private int nLastSelectedRegenZone = -1;
		private float fWorldRatio = 0f;
		private bool bDelimitingArea = false, bDrawCoords = false, bZoomIn = false;
		private const float fConstMarkSize = 7f;
		private float fTemporalZoom = 1.0f, fPanX = 0f, fPanY = 0f;
		private PointF pCurrentCursorPosition, pSelectingStartPoint;
		private RectangleF pSelectingArea;

		public class CustomDataGridView : DataGridView
		{
			protected override bool ProcessDialogKey(Keys keyData)
			{
				if ((keyData & Keys.KeyCode) == Keys.Enter)
				{
					EndEdit();
					return true;
				}

				return base.ProcessDialogKey(keyData);
			}
		}

		public NPCEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;

#if NPC_CHANNEL
			labelChannelFlag.Visible = true;
			btnChannelFlag.Visible = true;
#endif
#if REWARDS_BY_DAMAGE
			gbWorldBossDrop.Visible = true;
#endif
			/****************************************/
			gridDropRaid.TopLeftHeaderCell.Value = "N°";

			pbWorldMap.MouseWheel += pbWorldMap_MouseWheel;

			cbRenderDialog.Checked = pMain.pSettings.Show3DViewerDialog[this.Name];
			/****************************************/
			gridRegenSpots.TopLeftHeaderCell.Value = "N°";
			gridRegenSpots.MouseDown += gridRegenSpots_MouseDown;
			gridRegenSpots.MouseWheel += gridRegenSpots_MouseWheel;
#if USE_A_POS_H_COLUMN_IN_NPC_REGEN
			gridRegenSpots.Columns["posH"].Visible = true;
#endif
			pRegenToolTip = new ToolTip();
			pRegenToolTip.SetToolTip(lbRegenZones, "Select the zone where this NPC should spawn. Zones marked with * already have spawn spots.");
			pRegenToolTip.SetToolTip(pbWorldMap, "Move over the map and press P, or right-click the map and choose Add Regen Spot Here.");
			pRegenToolTip.SetToolTip(gridRegenSpots, "Edit spawn rows here. Right-click for delete, or use Delete/Backspace on selected rows.");
			pRegenToolTip.SetToolTip(tbSMC, "Paste a model path here. Full paths and Data\\... paths are converted to the NPC relative path.");

			List<object> ptotalNumColumnData = new List<object>
			{	// Hardcode!
				new { Text = "-1 (Infinite)", Value = -1 },
				new { Text = "0 (Castle NPCs)", Value = 0 },
				new { Text = "1 (Dungeons Or Raids)", Value = 1 }
			};

			for (int i = 2; i <= 50; i++)   // Hardcode!
				ptotalNumColumnData.Add(new { Text = i.ToString(), Value = i });

			DataGridViewComboBoxColumn ptotalNumColumn = (DataGridViewComboBoxColumn)gridRegenSpots.Columns["totalNum"];
			ptotalNumColumn.DataSource = ptotalNumColumnData;
			ptotalNumColumn.DisplayMember = "Text";
			ptotalNumColumn.ValueMember = "Value";
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.NPCTable.Select("a_index=" + pTempNPCRow["a_index"]).FirstOrDefault() != null)    // the current selected npc, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "NPC Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else    // the current selected npc is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current NPC is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "NPC Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

				string strSMCPath = NormalizeSMCPathForDatabase(tbSMC.Text);
				if (File.Exists(pMain.pSettings.ClientPath + "\\" + strSMCPath))
					pRenderDialog.SetModel(pMain.pSettings.ClientPath + "\\" + strSMCPath, "big", -1);
			}

			pMain.pSettings.Show3DViewerDialog[this.Name] = bState;

			pMain.WriteToINI(pMain.pSettings.SettingsFile, "3DViewerDialog", this.Name, bState.ToString());
		}

		private void MakeTempDropJobRow()
		{
			if (pTempDropJobRow == null)
			{
				pTempDropJobRow = pMain.pTables.NPCDropJobTable.NewRow();

				pTempDropJobRow["a_npc_idx"] = pTempNPCRow["a_index"];

				foreach (var ClassData in Defs.CharactersClassNJobsTypes)
				{
					string strDBClass = ClassData.Value[0].Substring(4);
					strDBClass = strDBClass == "Ex-Rogue" ? "ExRogue" : strDBClass;
					strDBClass = strDBClass == "ArchMage" ? "ExMage" : strDBClass;
					strDBClass = strDBClass.ToLower();

					pTempDropJobRow[$"a_{strDBClass}_item"] = -1;
					pTempDropJobRow[$"a_{strDBClass}_item_prob"] = 0;
				}
			}
		}

		private void MakeTempDropAllRow()
		{
			if (pTempDropAllRows == null)
			{
				pTempDropAllRows = new DataRow[Defs.MAX_NPC_DROPITEM];

				for (int i = 0; i < Defs.MAX_NPC_DROPITEM; i++)
				{
					DataRow pRow = pMain.pTables.NPCDropAllTable.NewRow();
					pRow["a_npc_idx"] = pTempNPCRow["a_index"];
					pRow["a_item_idx"] = -1;
					pRow["a_prob"] = 0;

					pTempDropAllRows[i] = pRow;
				}
			}
		}

		private async Task LoadNPCDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_enable", "a_family", "a_skillmaster", "a_flag", "a_flag1", "a_state_flag", "a_level", "a_exp", "a_prize", "a_sight", "a_size", "a_move_area", "a_attack_area", "a_skill_point", "a_sskill_master", "a_str", "a_dex", "a_int", "a_con", "a_attack", "a_magic", "a_defense", "a_resist", "a_attacklevel", "a_defenselevel", "a_hp", "a_mp", "a_attackType", "a_attackSpeed", "a_recover_hp", "a_recover_mp", "a_walk_speed", "a_run_speed", "a_skill0", "a_skill1", "a_skill2", "a_skill3", "a_item_0", "a_item_1", "a_item_2", "a_item_3", "a_item_4", "a_item_5", "a_item_6", "a_item_7", "a_item_8", "a_item_9", "a_item_10", "a_item_11", "a_item_12", "a_item_13", "a_item_14", "a_item_15", "a_item_16", "a_item_17", "a_item_18", "a_item_19", "a_item_percent_0", "a_item_percent_1", "a_item_percent_2", "a_item_percent_3", "a_item_percent_4", "a_item_percent_5", "a_item_percent_6", "a_item_percent_7", "a_item_percent_8", "a_item_percent_9", "a_item_percent_10", "a_item_percent_11", "a_item_percent_12", "a_item_percent_13", "a_item_percent_14", "a_item_percent_15", "a_item_percent_16", "a_item_percent_17", "a_item_percent_18", "a_item_percent_19", "a_minplus", "a_maxplus", "a_probplus", "a_product0", "a_product1", "a_product2", "a_product3", "a_product4", "a_file_smc", "a_motion_walk", "a_motion_idle", "a_motion_dam", "a_motion_attack", "a_motion_die", "a_motion_run", "a_motion_idle2", "a_motion_attack2", "a_scale", "a_attribute", "a_fireDelayCount", "a_fireDelay0", "a_fireDelay1", "a_fireDelay2", "a_fireDelay3", "a_fireEffect0", "a_fireEffect1", "a_fireEffect2", "a_fireObject", "a_fireSpeed", "a_aitype", "a_aiflag", "a_aileader_flag", "a_ai_summonHp", "a_aileader_idx", "a_aileader_count", "a_hit", "a_dodge", "a_magicavoid", "a_job_attribute", "a_jewel_0", "a_jewel_1", "a_jewel_2", "a_jewel_3", "a_jewel_4", "a_jewel_5", "a_jewel_6", "a_jewel_7", "a_jewel_8", "a_jewel_9", "a_jewel_10", "a_jewel_11", "a_jewel_12", "a_jewel_13", "a_jewel_14", "a_jewel_15", "a_jewel_16", "a_jewel_17", "a_jewel_18", "a_jewel_19", "a_jewel_percent_0", "a_jewel_percent_1", "a_jewel_percent_2", "a_jewel_percent_3", "a_jewel_percent_4", "a_jewel_percent_5", "a_jewel_percent_6", "a_jewel_percent_7", "a_jewel_percent_8", "a_jewel_percent_9", "a_jewel_percent_10", "a_jewel_percent_11", "a_jewel_percent_12", "a_jewel_percent_13", "a_jewel_percent_14", "a_jewel_percent_15", "a_jewel_percent_16", "a_jewel_percent_17", "a_jewel_percent_18", "a_jewel_percent_19", "a_zone_flag", "a_extra_flag", "a_rvr_value", "a_rvr_grade", "a_bound", "a_lifetime"
#if NPC_CHANNEL
				, "a_channel_flag"
#endif
#if REWARDS_BY_DAMAGE
				, "a_reward_gold_min", "a_reward_gold_max", "a_reward_gold_multiplier"
#endif
			];

			foreach (string strNation in pMain.pSettings.NationSupported)
				listQueryCompose.AddRange("a_name_" + strNation.ToLower(), "a_descr_" + strNation.ToLower());

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

		private async Task LoadNPCRegenDataAsync()  // NOTE: This... is a little bit stupid cuz load 3k rows...
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_npc_idx",
				"a_zone_num",
				"a_pos_x",
				"a_pos_z",
#if USE_A_POS_H_COLUMN_IN_NPC_REGEN
				"a_pos_h",
#endif
				"a_pos_r",
				"a_y_layer",
				"a_regen_sec",
				"a_total_num"
			];

			if (pMain.pTables.NPCRegenTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.NPCRegenTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_npc_regen ORDER BY a_index;");
				});

				if (pMain.pTables.NPCRegenTable == null)
					pMain.pTables.NPCRegenTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.NPCRegenTable);
			}
		}

		private async Task LoadZoneDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_name", "a_poscount"];

			if (pMain.pTables.ZoneTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.ZoneTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_zone_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_zonedata ORDER BY a_zone_index;");
				});

				if (pMain.pTables.ZoneTable == null)
					pMain.pTables.ZoneTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_zone_index", ref pMain.pTables.ZoneTable);
			}
		}

		private async Task LoadNPCDropJobAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_titan_item",
				"a_titan_item_prob",
				"a_knight_item",
				"a_knight_item_prob",
				"a_healer_item",
				"a_healer_item_prob",
				"a_mage_item",
				"a_mage_item_prob",
				"a_rogue_item",
				"a_rogue_item_prob",
				"a_sorcerer_item",
				"a_sorcerer_item_prob",
				"a_nightshadow_item",
				"a_nightshadow_item_prob",
				"a_exrogue_item",
				"a_exrogue_item_prob",
				"a_exmage_item",
				"a_exmage_item_prob"
			];

			if (pMain.pTables.NPCDropJobTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.NPCDropJobTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_npc_idx, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_npc_dropjob ORDER BY a_npc_idx;");
				});

				if (pMain.pTables.NPCDropJobTable == null)
					pMain.pTables.NPCDropJobTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_npc_idx", ref pMain.pTables.NPCDropJobTable);
			}
		}

		private void LoadDropJobData(int nNPCID)
		{
			if (pMain.pTables.NPCDropJobTable.Select("a_npc_idx=" + nNPCID).Length > 0)
			{
				pTempDropJobRow = pMain.pTables.NPCDropJobTable.NewRow();
				pTempDropJobRow.ItemArray = (object[])pMain.pTables.NPCDropJobTable.Select("a_npc_idx=" + nNPCID)[0].ItemArray.Clone();
			}

			if (pTempDropJobRow != null)
			{
				DataRow pItemRow;
				Button btnObj;
				TextBox tbObj;
				Label lObj;

				foreach (var ClassData in Defs.CharactersClassNJobsTypes)
				{
					string strClass = ClassData.Value[0].Substring(4);
					strClass = strClass == "Ex-Rogue" ? "ExRogue" : strClass;

					string strDBClass = strClass == "Ex-Rogue" ? "ExRogue" : strClass;
					strDBClass = strDBClass == "ArchMage" ? "ExMage" : strDBClass;
					strDBClass = strDBClass.ToLower();

					int nItemID = Convert.ToInt32(pTempDropJobRow[$"a_{strDBClass}_item"]);
					string strItemID = nItemID.ToString();
					int nItemProb = Convert.ToInt32(pTempDropJobRow[$"a_{strDBClass}_item_prob"]);
					
					btnObj = (Button)Controls.Find($"btnDropJob{strClass}ID", true)[0];
					tbObj = (TextBox)Controls.Find($"tbDropJob{strClass}Prob", true)[0];
					lObj = (Label)Controls.Find($"lDropJob{strClass}ProbPercentage", true)[0];

					pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).FirstOrDefault();
					if (pItemRow != null)
					{
						btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));

						strItemID += $" - {pItemRow["a_name_" + pMain.pSettings.WorkLocale]}";

						tbObj.Text = nItemProb.ToString();
						tbObj.Enabled = true;

						lObj.Text = ((nItemProb * 100.0f) / 10000.0f) + "%";
					}
					else
					{
						if (nItemID > 0)
							pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: (t_npc_dropjob)a_{strDBClass}_item: {nItemID} not exist in t_item.");

						btnObj.Image = null;

						tbObj.Text = "0";
						tbObj.Enabled = false;

						lObj.Text = "0%";
					}

					btnObj.Text = strItemID;
				}
			}
		}

		private async Task LoadNPCDropAllAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_item_idx", "a_prob"];

			if (pMain.pTables.NPCDropAllTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.NPCDropAllTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_npc_idx, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_npc_drop_all ORDER BY a_npc_idx;");
				});

				if (pMain.pTables.NPCDropAllTable == null)
					pMain.pTables.NPCDropAllTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_npc_idx", ref pMain.pTables.NPCDropAllTable);
			}
		}

		private void LoadDropAllData(int nNPCID)
		{
			pTempDropAllRows = pMain.pTables.NPCDropAllTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_npc_idx"]) == nNPCID).Select(row => {
				DataRow newRow = pMain.pTables.NPCDropAllTable.NewRow();
				newRow.ItemArray = (object[])row.ItemArray.Clone();
				return newRow;
			}).ToArray();

			if (pTempDropAllRows.Length > 0)
			{
				DataRow pItemRow;
				Button btnObj;
				TextBox tbObj;
				Label lObj;
				int i = 0;

				foreach (DataRow pRow in pTempDropAllRows)
				{
					int nItemID = Convert.ToInt32(pRow["a_item_idx"]);
					int nItemProb = Convert.ToInt32(pRow["a_prob"]);
					string strItemID = nItemID.ToString();

					btnObj = (Button)Controls.Find("btnDropAll" + i, true)[0];
					tbObj = (TextBox)Controls.Find($"tbDropAll{i}Prob", true)[0];
					lObj = (Label)Controls.Find("lDropAllProbPercentage" + i, true)[0];

					pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).FirstOrDefault();
					if (pItemRow != null)
					{
						btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));

						strItemID += $" - {pItemRow["a_name_" + pMain.pSettings.WorkLocale]}";

						tbObj.Text = nItemProb.ToString();
						tbObj.Enabled = true;

						lObj.Text = ((nItemProb * 100.0f) / 10000.0f) + "%";
					}
					else
					{
						if (nItemID > 0)
							pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: (t_npc_drop_all)a_item_idx: {nItemID} not exist in t_item.");

						btnObj.Image = null;

						tbObj.Text = "0";
						tbObj.Enabled = false;

						lObj.Text = "0%";
					}

					btnObj.Text = strItemID;

					i++;
				}
			}
		}

		private async Task LoadNPCDropRaidAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_item_index",
				"a_count",
				"a_prob",
				"a_flag",
				"a_spec_item_index1",
				"a_spec_item_index2",
				"a_spec_item_index3",
				"a_spec_item_index4",
				"a_spec_item_index5",
				"a_spec_item_index6",
				"a_spec_item_index7",
				"a_spec_item_index8",
				"a_spec_item_index9",
				"a_spec_item_index10",
				"a_spec_item_index11",
				"a_spec_item_index12",
				"a_spec_item_index13",
				"a_spec_item_index14",
				"a_spec_count",
				"a_spec_prob"
			];

			if (pMain.pTables.NPCDropRaidTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.NPCDropRaidTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_npc_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_npc_dropraid ORDER BY a_npc_index;");
				});

				if (pMain.pTables.NPCDropRaidTable == null)
					pMain.pTables.NPCDropRaidTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_npc_index", ref pMain.pTables.NPCDropRaidTable);
			}
		}

		private void LoadDropRaidData(int nNPCID)
		{
			gridDropRaid.Rows.Clear();

			pTempDropRaidRows = pMain.pTables.NPCDropRaidTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_npc_index"]) == nNPCID).Select(row => {
				DataRow newRow = pMain.pTables.NPCDropRaidTable.NewRow();
				newRow.ItemArray = (object[])row.ItemArray.Clone();
				return newRow;
			}).ToArray();

			if (pTempDropRaidRows.Length > 0)
			{
				int i = 0;
				DataRow pItemRow;

				gridDropRaid.SuspendLayout();

				foreach (DataRow pRow in pTempDropRaidRows)
				{
					gridDropRaid.Rows.Insert(i);

					gridDropRaid.Rows[i].HeaderCell.Value = (i + 1).ToString();

					int nItemID = Convert.ToInt32(pRow["a_item_index"]);
					string strItemIDName = nItemID.ToString();
					Bitmap pBitmapItemIcon = new(1, 1);
					long lItemCount = Convert.ToInt64(pRow["a_count"]);
					int nItemProb = Convert.ToInt32(pRow["a_prob"]);
					long lItemFlag = Convert.ToInt64(pRow["a_flag"]);
					int nSpecialBoxRewardProb = Convert.ToInt32(pRow["a_spec_prob"]);

					pItemRow = pMain.pTables.ItemTable.Select($"a_index={nItemID}").FirstOrDefault();
					if (pItemRow != null)
					{
						strItemIDName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale];

						pBitmapItemIcon = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));

						if (nItemID == Defs.NAS_ITEM_DB_INDEX)
						{
							gridDropRaid.Rows[i].Cells["count"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(pRow["a_count"]));
							gridDropRaid.Rows[i].Cells["count"].Style.BackColor = Color.FromArgb(166, 166, 166);
						}
					}
					else
					{
						if (nItemID > 0)
							pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: (t_npc_dropraid)a_item_index: {nItemID} not exist in t_item.");

						nItemID = -1;
						strItemIDName = "-1";
						lItemCount = 0;
						nItemProb = 0;
						lItemFlag = 0;
					}

					gridDropRaid.Rows[i].Cells["itemIcon"].Value = pBitmapItemIcon;
					gridDropRaid.Rows[i].Cells["item"].Value = strItemIDName;
					gridDropRaid.Rows[i].Cells["item"].Tag = nItemID;
					gridDropRaid.Rows[i].Cells["count"].Value = lItemCount;
					gridDropRaid.Rows[i].Cells["prob"].Value = nItemProb;
					gridDropRaid.Rows[i].Cells["prob"].ToolTipText = ((nItemProb * 100.0f) / 10000.0f) + "%";
					gridDropRaid.Rows[i].Cells["flag"].Value = lItemFlag;

					StringBuilder strTooltip = new();

					for (int j = 0; j < Defs.ItemFlags.Length; j++)
					{
						if ((lItemFlag & 1L << j) != 0)
							strTooltip.Append(Defs.ItemFlags[j] + "\n");
					}

					gridDropRaid.Rows[i].Cells["flag"].ToolTipText = strTooltip.ToString();

					for (int j = 1; j <= 14; j++)
					{
						pBitmapItemIcon = new Bitmap(1, 1);
						nItemID = Convert.ToInt32(pRow["a_spec_item_index" + j]);
						strItemIDName = nItemID.ToString();

						pItemRow = pMain.pTables.ItemTable.Select($"a_index={nItemID}").FirstOrDefault();
						if (pItemRow != null)
						{
							strItemIDName += " - " + pItemRow["a_name_" + pMain.pSettings.WorkLocale];

							pBitmapItemIcon = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
						}
						else
						{
							if (nItemID > 0)
								pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_spec_item_index{j}: {nItemID} not exist in t_item.");

							nItemID = -1;
							strItemIDName = "-1";
						}

						gridDropRaid.Rows[i].Cells[$"specitem{j}icon"].Value = pBitmapItemIcon;
						gridDropRaid.Rows[i].Cells["specitem" + j].Value = strItemIDName;
						gridDropRaid.Rows[i].Cells["specitem" + j].Tag = nItemID;
					}

					gridDropRaid.Rows[i].Cells["speccount"].Value = pRow["a_spec_count"];
					gridDropRaid.Rows[i].Cells["specprob"].Value = nSpecialBoxRewardProb;
					gridDropRaid.Rows[i].Cells["specprob"].ToolTipText = ((nSpecialBoxRewardProb * 100.0f) / 1000.0f) + "%";

					i++;
				}

				gridDropRaid.ResumeLayout();
			}
		}

		private void LoadRegenData(int nNPCID)
		{
			lbRegenZones.Items.Clear();

			pTempRegenRows = pMain.pTables.NPCRegenTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_npc_idx"]) == nNPCID).Select(row => {
				DataRow newRow = pMain.pTables.NPCRegenTable.NewRow();
				newRow.ItemArray = (object[])row.ItemArray.Clone();
				return newRow;
			}).ToArray();

			lbRegenZones.BeginUpdate();

			foreach (DataRow pRow in pMain.pTables.ZoneTable.Rows)
			{
				int nZoneID = Convert.ToInt32(pRow["a_zone_index"]);
				string strZoneName = pRow["a_name"].ToString() ?? string.Empty;

				if (pTempRegenRows.AsEnumerable().Any(row => Convert.ToInt32(row["a_zone_num"]) == nZoneID))
					strZoneName += "\t*";

				lbRegenZones.Items.Add(new Main.ListBoxItem
				{
					ID = nZoneID,
					Text = $"{nZoneID} - {strZoneName}"
				});
			}

			lbRegenZones.EndUpdate();

			lbRegenZones.SelectedIndex = -1;    // Reset

			if (lbRegenZones.Items.Count > 0)
			{
				if (nLastSelectedRegenZone >= 0 && nLastSelectedRegenZone < lbRegenZones.Items.Count)
					lbRegenZones.SelectedIndex = nLastSelectedRegenZone;
				else
					lbRegenZones.SelectedIndex = 0;
			}
		}

		private async void NPCEditor_LoadAsync(object sender, EventArgs e)
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
			cbSkillMaster.BeginUpdate();
			cbSkillMaster.Items.Add("No");

			foreach (var ClassData in Defs.CharactersClassNJobsTypes)
				cbSkillMaster.Items.Add(ClassData.Value[0].Substring(4));

			cbSkillMaster.EndUpdate();
			/****************************************/
			cbSpecialSkillMaster.BeginUpdate();
			cbSpecialSkillMaster.Items.Add("No");

			foreach (string strType in Defs.SpecialSkillTypes)
				cbSpecialSkillMaster.Items.Add(strType);

			cbSpecialSkillMaster.EndUpdate();
			/****************************************/
			cbAttackTypeSelector.BeginUpdate();

			foreach (string strType in Defs.NPCAttackType)
				cbAttackTypeSelector.Items.Add(strType);

			cbAttackTypeSelector.EndUpdate();
			/****************************************/
			cbRvRValueSelector.BeginUpdate();

			foreach (string strSyndicateType in Defs.SyndicateTypesNGrades.Keys)
				cbRvRValueSelector.Items.Add(strSyndicateType);

			cbRvRValueSelector.EndUpdate();
			/****************************************/
			cbAttackAttributeType.BeginUpdate();
			cbDefenseAttributeType.BeginUpdate();

			var magicList = Defs.MagicTypesAndSubTypes[Defs.MagicTypesAndSubTypes.Keys.ElementAt(1)];

			foreach (string strSubType in magicList.Take(magicList.Count() - 1 /* Max Type is AT_LIGHT*/))
			{
				cbAttackAttributeType.Items.Add(strSubType);
				cbDefenseAttributeType.Items.Add(strSubType);
			}

			cbAttackAttributeType.EndUpdate();
			cbDefenseAttributeType.EndUpdate();
			/****************************************/
			cbDefenseAttributeLevel.Items.Clear();

			cbAttackAttributeLevel.BeginUpdate();
			cbDefenseAttributeLevel.BeginUpdate();

			for (int i = 0; i < Defs.AT_LEVELMAX; i++)
			{
				cbAttackAttributeLevel.Items.Add(i);
				cbDefenseAttributeLevel.Items.Add(i);
			}

			cbAttackAttributeLevel.EndUpdate();
			cbDefenseAttributeLevel.EndUpdate();
			/****************************************/
			cbAIType.BeginUpdate();

			foreach (string strSyndicateType in Defs.NPCAIType)
				cbAIType.Items.Add(strSyndicateType);

			cbAIType.EndUpdate();
			/****************************************/
			cbAILeaderFlag.BeginUpdate();

			foreach (string strSyndicateType in Defs.NPCAILeaderFlag)
				cbAILeaderFlag.Items.Add(strSyndicateType);

			cbAILeaderFlag.EndUpdate();
			/****************************************/
			cbWorldRatioSelector.BeginUpdate();

			cbWorldRatioSelector.Items.AddRange(
				"512x512 - " + Defs.WorldRatio.ToString(),
				"1024x1024 - " + (Defs.WorldRatio * 2).ToString()
			);  // Hardcode!

			cbWorldRatioSelector.EndUpdate();
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await Task.WhenAll(
				LoadNPCDataAsync(),
				LoadNPCRegenDataAsync(),
				LoadZoneDataAsync(),
				LoadNPCDropJobAsync(),
				LoadNPCDropAllAsync(),
				LoadNPCDropRaidAsync(),
				pMain.GenericLoadSkillDataAsync(),
				pMain.GenericLoadSkillLevelDataAsync(),
				pMain.GenericLoadItemDataAsync(),
				pMain.GenericLoadStringDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"NPCs, NPC Regens, Zones, Skills, Skills Levels, Items, Strings, NPC Drop Job, NPC Drop All, NPC Drop Raid Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.ZoneTable != null)
			{
				int nTotalZones = pMain.pTables.ZoneTable.Rows.Count;
				strZones = new string[nTotalZones];

				for (int i = 0; i < nTotalZones; i++)
					strZones[i] = pMain.pTables.ZoneTable.Rows[i]["a_name"].ToString() ?? string.Empty;
			}
			/****************************************/
			if (pMain.pTables.NPCTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.NPCTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_index"]), pRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnReload, "Reload NPCs, NPC Regens, Zones, Skills, Skills Levels, Items, Strings, NPC Drop Job, NPC Drop All & NPC Drop Raid Data from Database"); // Not dispose onreload
			pToolTip.SetToolTip(cbSkillMaster, "This is actually an On/Off. The value is used by the Client, but I prefer to put a selector since in the future it could be useful");   // Not dispose onreload
			/*#if NPC_CHANNEL	// NOTE: I disable it cuz is overlapping with the Flags Tooltip
						pToolTip.SetToolTip(btnChannelFlag, "Channels where NPC has to be shown");  // Not dispose onreload
			#endif*/
#if REWARDS_BY_DAMAGE
			pToolTip.SetToolTip(gbWorldBossDrop, "WORLD_BOSS_DROP Gold drop when Hit (Damage * GetRandom(GOLD_MIN, GOLD_MAX) / 100) * GOLD_MULTIPLIER");    // Not dispose onreload
#endif
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void NPCEditor_FormClosing(object sender, FormClosingEventArgs e)
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

				if (cmDropRaid != null)
				{
					cmDropRaid.Dispose();
					cmDropRaid = null;
				}

				if (cmRegenSpots != null)
				{
					cmRegenSpots.Dispose();
					cmRegenSpots = null;
				}

				if (pRegenToolTip != null)
				{
					pRegenToolTip.Dispose();
					pRegenToolTip = null;
				}
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "NPC Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nNPCID, bool bLoadFrompNPCTable)
		{
			bUserAction = false;
			StringBuilder strTooltip;
			Button? btnObj = null;
			ComboBox? cbObj = null;
			TextBox? tbObj = null;
			long lFlag;
			int i;
			/****************************************/
			// Resets
			gridDropRaid.Rows.Clear();

			for (i = 0; i < Defs.MAX_NPC_SKILL_SERVER; i++)
			{
				btnObj = (Button)Controls.Find($"btnSkill{i}ID", true)[0];
				cbObj = (ComboBox)Controls.Find("cbSkillLevel" + i, true)[0];
				tbObj = (TextBox)Controls.Find("tbSkillProb" + i, true)[0];

				btnObj.Image = null;
				btnObj.Text = "-1";

				cbObj.Items.Clear();
				cbObj.Enabled = false;

				tbObj.Text = "0";
				tbObj.Enabled = false;
			}

			for (i = 0; i < Defs.MAX_NPC_PRODUCT; i++)
				((Button)Controls.Find("btnProduct" + i, true)[0]).Image = null;

			for (i = 0; i < Defs.MAX_NPC_DROPITEM; i++)
				((Button)Controls.Find("btnItemDrop" + i, true)[0]).Image = null;

			for (i = 0; i < Defs.MAX_NPC_DROPJEWEL; i++)
				((Button)Controls.Find("btnJewelDrop" + i, true)[0]).Image = null;

			// Drop Job
			foreach (var ClassData in Defs.CharactersClassNJobsTypes)
			{
				string strClass = ClassData.Value[0].Substring(4);
				strClass = strClass == "Ex-Rogue" ? "ExRogue" : strClass;

				btnObj = (Button)Controls.Find($"btnDropJob{strClass}ID", true)[0];

				btnObj.Image = null;
				btnObj.Text = "-1";

				tbObj = (TextBox)Controls.Find($"tbDropJob{strClass}Prob", true)[0];

				tbObj.Text = "0";
				tbObj.Enabled = false;

				((Label)Controls.Find($"lDropJob{strClass}ProbPercentage", true)[0]).Text = "0%";
			}

			// Drop All
			for (i = 0; i < Defs.MAX_NPC_DROPITEM; i++)
			{
				btnObj = (Button)Controls.Find("btnDropAll" + i, true)[0];

				btnObj.Image = null;
				btnObj.Text = "-1";

				tbObj = (TextBox)Controls.Find($"tbDropAll{i}Prob", true)[0];

				tbObj.Text = "0";
				tbObj.Enabled = false;

				((Label)Controls.Find("lDropAllProbPercentage" + i, true)[0]).Text = "0%";
			}

			cbWorldRatioSelector.SelectedIndex = 0;

			lbRegenZones.Items.Clear();

			lbRegenZones.BeginUpdate();

			foreach (DataRow pRow in pMain.pTables.ZoneTable.Rows)
			{
				int nZoneID = Convert.ToInt32(pRow["a_zone_index"]);

				lbRegenZones.Items.Add(new Main.ListBoxItem
				{
					ID = nZoneID,
					Text = $"{nZoneID} - {pRow["a_name"].ToString() ?? string.Empty}"
				});
			}

			lbRegenZones.EndUpdate();

			pbWorldMap.Invalidate();

			gridRegenSpots.Rows.Clear();

			foreach (var toolTip in pToolTips.Values)
				toolTip.Dispose();
			/****************************************/
			if (bLoadFrompNPCTable && pMain.pTables.NPCTable != null)
			{
				pTempNPCRow = pMain.pTables.NPCTable.NewRow();
				pTempNPCRow.ItemArray = (object[])pMain.pTables.NPCTable.Select("a_index=" + nNPCID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nNPCID.ToString();
			/****************************************/
			if (pTempNPCRow["a_enable"].ToString() == "1")
				cbEnable.Checked = true;
			else
				cbEnable.Checked = false;
			/****************************************/
			string strSMCPath = NormalizeSMCPathForDatabase(pTempNPCRow["a_file_smc"].ToString());

			SetSMCPath(strSMCPath, false);

			if (pMain.pSettings.Show3DViewerDialog[this.Name])
			{
				if (pRenderDialog == null || pRenderDialog.IsDisposed)
					pRenderDialog = new RenderDialog(pMain);

				if (!pRenderDialog.Visible)
					pRenderDialog.Show();

				if (!File.Exists(pMain.pSettings.ClientPath + "\\" + strSMCPath))
					pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_file_smc path not exist or empty.");
				else
					pRenderDialog.SetModel(pMain.pSettings.ClientPath + "\\" + strSMCPath, "big", -1);
			}

			tbLevel.Text = pTempNPCRow["a_level"].ToString();
			tbFamily.Text = pTempNPCRow["a_family"].ToString();
			tbLifeTime.Text = pTempNPCRow["a_lifetime"].ToString();
			tbWalkSpeed.Text = Convert.ToString(pTempNPCRow["a_walk_speed"], CultureInfo.InvariantCulture);
			tbRunSpeed.Text = Convert.ToString(pTempNPCRow["a_run_speed"], CultureInfo.InvariantCulture);
			tbMoveArea.Text = pTempNPCRow["a_move_area"].ToString();
			tbSight.Text = Convert.ToString(pTempNPCRow["a_sight"], CultureInfo.InvariantCulture);
			/****************************************/
			string strNation = cbNationSelector.SelectedItem.ToString().ToLower();

			tbName.Text = pTempNPCRow["a_name_" + strNation].ToString();
			tbDescription.Text = pTempNPCRow["a_descr_" + strNation].ToString();
			/****************************************/
			tbHP.Text = pTempNPCRow["a_hp"].ToString();
			tbMP.Text = pTempNPCRow["a_mp"].ToString();
			tbRecoverHP.Text = pTempNPCRow["a_recover_hp"].ToString();
			tbRecoverMP.Text = pTempNPCRow["a_recover_mp"].ToString();
			tbStr.Text = pTempNPCRow["a_str"].ToString();
			tbDex.Text = pTempNPCRow["a_dex"].ToString();
			tbInt.Text = pTempNPCRow["a_int"].ToString();
			tbCon.Text = pTempNPCRow["a_con"].ToString();
			tbSize.Text = Convert.ToString(pTempNPCRow["a_size"], CultureInfo.InvariantCulture);
			tbScale.Text = Convert.ToString(pTempNPCRow["a_scale"], CultureInfo.InvariantCulture);
			tbBound.Text = Convert.ToString(pTempNPCRow["a_bound"], CultureInfo.InvariantCulture);
			/****************************************/
			btnFlag.Text = pTempNPCRow["a_flag"].ToString();

			strTooltip = new StringBuilder();
			lFlag = Convert.ToInt64(pTempNPCRow["a_flag"]);
#if REWARDS_BY_DAMAGE
			if ((lFlag & (1L << Array.IndexOf(Defs.NPCFlags, "WORLD_BOSS_DROP"))) != 0)
				gbWorldBossDrop.Enabled = true;
			else
				gbWorldBossDrop.Enabled = false;
#endif
			for (i = 0; i < Defs.NPCFlags.Length; i++)
			{
				if ((lFlag & 1L << i) != 0)
					strTooltip.Append(Defs.NPCFlags[i] + "\n");
			}

			if (lFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_flag out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnFlag, strTooltip.ToString());
			pToolTips[btnFlag] = pToolTip;
			/****************************************/
			btnFlag1.Text = pTempNPCRow["a_flag1"].ToString();

			strTooltip = new StringBuilder();
			lFlag = Convert.ToInt64(pTempNPCRow["a_flag1"]);

			for (i = 0; i < Defs.NPCFlags1.Length; i++)
			{
				if ((lFlag & 1L << i) != 0)
					strTooltip.Append(Defs.NPCFlags1[i] + "\n");
			}

			if (lFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_flag1 out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnFlag1, strTooltip.ToString());
			pToolTips[btnFlag1] = pToolTip;
			/****************************************/
			btnStateFlag.Text = pTempNPCRow["a_state_flag"].ToString();

			strTooltip = new StringBuilder();
			lFlag = Convert.ToInt64(pTempNPCRow["a_state_flag"]);
			i = 0;

			foreach (string strSubType in Defs.MagicTypesAndSubTypes[Defs.MagicTypesAndSubTypes.Keys.ElementAt(2)])
			{
				if ((lFlag & 1L << i) != 0)
					strTooltip.Append(strSubType + "\n");

				i++;
			}

			if (lFlag != 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_state_flag out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnStateFlag, strTooltip.ToString());
			pToolTips[btnStateFlag] = pToolTip;
			/****************************************/
#if NPC_CHANNEL
			lFlag = Convert.ToInt64(pTempNPCRow["a_channel_flag"]);

			btnChannelFlag.Text = lFlag.ToString();

			strTooltip = new StringBuilder();

			if (lFlag != -1 && pMain.pTables.StringTable != null)
			{
				DataRow[] listChannelNames = pMain.pTables.StringTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) >= 6291 && Convert.ToInt32(row["a_index"]) <= (6290 + Defs.MAX_CHANNELS)).ToArray();

				i = 0;
				foreach (DataRow pRowString in listChannelNames)
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(pRowString["a_string_" + pMain.pSettings.WorkLocale] + "\n");

					i++;
				}

				if (i > Defs.MAX_CHANNELS)
					pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_channel_flag out of range.");
			}
			else
			{
				strTooltip.Append("All");
			}

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnChannelFlag, strTooltip.ToString());
			pToolTips[btnChannelFlag] = pToolTip;
#endif
			/****************************************/
			int nSkillMasterType = Convert.ToInt32(pTempNPCRow["a_skillmaster"]);

			if (nSkillMasterType < -1 || nSkillMasterType >= Defs.CharactersClassNJobsTypes.Count() + 1 /*Cuz a_skillmaster min is -1*/)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_skillmaster out of range.");
			else
				cbSkillMaster.SelectedIndex = nSkillMasterType == -1 ? 0 : nSkillMasterType;
			/****************************************/
			int nSSkillMasterType = Convert.ToInt32(pTempNPCRow["a_sskill_master"]);

			if (nSSkillMasterType < -1 || nSSkillMasterType >= Defs.SpecialSkillTypes.Count() + 1 /*Cuz a_skillmaster min is -1*/)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_sskill_master out of range.");
			else
				cbSpecialSkillMaster.SelectedIndex = nSSkillMasterType == -1 ? 0 : nSSkillMasterType;
			/****************************************/
#if REWARDS_BY_DAMAGE
			tbWorldBossGoldMin.Text = pTempNPCRow["a_reward_gold_min"].ToString();
			tbWorldBossGoldMax.Text = pTempNPCRow["a_reward_gold_max"].ToString();
			tbWorldBossGoldMultiplier.Text = pTempNPCRow["a_reward_gold_multiplier"].ToString();
#endif
			/****************************************/
			tbPrizeExperience.Text = pTempNPCRow["a_exp"].ToString();
			tbPrizeGoldCoin.Text = pTempNPCRow["a_prize"].ToString();
			tbPrizeSkillPoint.Text = pTempNPCRow["a_skill_point"].ToString();
			/****************************************/
			btnZoneFlag.Text = pTempNPCRow["a_zone_flag"].ToString();

			strTooltip = new StringBuilder();
			lFlag = Convert.ToInt64(pTempNPCRow["a_zone_flag"]);

			for (i = 0; i < pMain.pTables.ZoneTable.Rows.Count; i++)
			{
				if ((lFlag & 1L << i) != 0)
					strTooltip.Append(pMain.pTables.ZoneTable.Rows[i]["a_name"] + "\n");
			}

			if (lFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_zone_flag out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnZoneFlag, strTooltip.ToString());
			pToolTips[btnZoneFlag] = pToolTip;

			int nExtraZone = Convert.ToInt32(pTempNPCRow["a_extra_flag"]);

			if (nExtraZone < 0 || nExtraZone > 63)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_extra_flag out of range.");
			else
				cbExtraZone.SelectedIndex = nExtraZone;
			/****************************************/
			int nRvRValue = Convert.ToInt32(pTempNPCRow["a_rvr_value"]);

			if (nRvRValue >= Defs.SyndicateTypesNGrades.Keys.Count)
			{
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_rvr_value out of range.");
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
					cbRvRGradeSelector.SelectedIndex = Convert.ToInt32(pTempNPCRow["a_rvr_grade"]);
				}
			}
			/****************************************/
			int nAttackType = Convert.ToInt32(pTempNPCRow["a_attackType"]);

			if (nAttackType < 0 || nAttackType >= Defs.NPCAttackType.Count())
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_attackType out of range.");
			else
				cbAttackTypeSelector.SelectedIndex = nAttackType;

			tbAttackLevel.Text = pTempNPCRow["a_attacklevel"].ToString();
			tbAttackSpeed.Text = pTempNPCRow["a_attackSpeed"].ToString();
			tbAttackArea.Text = Convert.ToString(pTempNPCRow["a_attack_area"], CultureInfo.InvariantCulture);
			tbAttack.Text = pTempNPCRow["a_attack"].ToString();
			tbMagic.Text = pTempNPCRow["a_magic"].ToString();
			tbHit.Text = pTempNPCRow["a_hit"].ToString();
			/****************************************/
			tbDefenseLevel.Text = pTempNPCRow["a_defenselevel"].ToString();
			tbDefense.Text = pTempNPCRow["a_defense"].ToString();
			tbResist.Text = pTempNPCRow["a_resist"].ToString();
			tbDodge.Text = pTempNPCRow["a_dodge"].ToString();
			tbMagicAvoid.Text = pTempNPCRow["a_magicavoid"].ToString();

			btnJobAttribute.Text = pTempNPCRow["a_job_attribute"].ToString();

			strTooltip = new StringBuilder();
			lFlag = Convert.ToInt64(pTempNPCRow["a_job_attribute"]);

			foreach (var ClassData in Defs.CharactersClassNJobsTypes)
			{
				if ((lFlag & 1L << ClassData.Key) != 0)
					strTooltip.Append(ClassData.Value[0].Substring(4) + "\n");
			}

			if (lFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_job_attribute out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnJobAttribute, strTooltip.ToString());
			pToolTips[btnJobAttribute] = pToolTip;
			/****************************************/
			DataRow pTempDataRow;

			for (i = 0; i < Defs.MAX_NPC_SKILL_SERVER; i++)
			{
				string[] strSkillsData = pTempNPCRow["a_skill" + i].ToString().Split(' ');

				if (strSkillsData[0] != "" && strSkillsData[0] != "-1")
				{
					int nSkillID = Convert.ToInt32(strSkillsData[0]);
					int nSkillLevel = Convert.ToInt32(strSkillsData[1]);
					int nSkillProb = Convert.ToInt32(strSkillsData[2]);
					string strSkillName = nSkillID.ToString();
					pTempDataRow = pMain.pTables.SkillTable.Select($"a_index={nSkillID}").FirstOrDefault();
					btnObj = (Button)Controls.Find($"btnSkill{i}ID", true)[0];
					cbObj = (ComboBox)Controls.Find("cbSkillLevel" + i, true)[0];
					tbObj = (TextBox)Controls.Find("tbSkillProb" + i, true)[0];

					if (pTempDataRow != null)
					{
						strSkillName += " - " + pTempDataRow["a_name_" + pMain.pSettings.WorkLocale];

						btnObj.Image = new Bitmap(pMain.GetIcon("SkillBtn", pTempDataRow["a_client_icon_texid"].ToString(), Convert.ToInt32(pTempDataRow["a_client_icon_row"]), Convert.ToInt32(pTempDataRow["a_client_icon_col"])), new Size(24, 24));

						cbObj.Items.Clear();
						cbObj.BeginUpdate();

						DataRow[] pSkillLevelRows = pMain.pTables.SkillLevelTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nSkillID).ToArray();

						foreach (DataRow pRowSkillLevel in pSkillLevelRows)
						{
							int nSkillLevelToAdd = Convert.ToInt32(pRowSkillLevel["a_level"]);

							cbObj.Items.Add(nSkillLevelToAdd);

							if (nSkillLevel == nSkillLevelToAdd)
								cbObj.SelectedIndex = cbObj.Items.Count - 1;
						}

						if (cbObj.SelectedIndex == -1)
							cbObj.SelectedIndex = 0;

						cbObj.EndUpdate();
						cbObj.Enabled = true;
					}
					else
					{
						pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_skill{i}: {nSkillID} not exist in t_skill.");
					}

					btnObj.Text = strSkillName;

					tbObj.Text = nSkillProb.ToString();
					tbObj.Enabled = true;

					((Label)Controls.Find("lSkillProbPercentage" + i, true)[0]).Text = ((nSkillProb * 100.0f) / 10000.0f) + "%";
				}
				else if (strSkillsData[0] == "")
				{
					pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_skill{i} is empty (-1 Will be asigned temporary until press Update).");

					pTempNPCRow["a_skill" + i] = "-1";
				}
			}
			/****************************************/
			int uAttribute = Convert.ToInt32(pTempNPCRow["a_attribute"]);
			byte cDefenseAttribute = pMain.GET_AT_DEF(uAttribute);
			byte cAttackAttribute = pMain.GET_AT_ATT(uAttribute);
			int nDefenseAttributeType = pMain.GET_AT_VAR(cDefenseAttribute);
			int nDefenseAttributeLevel = pMain.GET_AT_LV(cDefenseAttribute);
			int nAttackAttributeType = pMain.GET_AT_VAR(cAttackAttribute);
			int nAttackAttributeLevel = pMain.GET_AT_LV(cAttackAttribute);
			var magicList = Defs.MagicTypesAndSubTypes[Defs.MagicTypesAndSubTypes.Keys.ElementAt(1)];
			Image pIcon;

			if (nDefenseAttributeType > magicList.Take(magicList.Count() - 1 /* Max Type is AT_LIGHT*/).Count())
			{
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: Defense Attribute Type out of range.");
			}
			else
			{
				pIcon = pMain.GetIcon("Elemental", "", nDefenseAttributeType, 1);
				if (pIcon != null)
					pbDefenseIcon.Image = pIcon;

				cbDefenseAttributeType.SelectedIndex = nDefenseAttributeType;
			}

			if (nDefenseAttributeLevel >= Defs.AT_LEVELMAX)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: Defense Attribute Level out of range.");
			else
				cbDefenseAttributeLevel.SelectedIndex = pMain.GET_AT_LV(cDefenseAttribute);

			if (nAttackAttributeType > magicList.Take(magicList.Count() - 1 /* Max Type is AT_LIGHT*/).Count())
			{
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: Attack Attribute Type out of range.");
			}
			else
			{
				pIcon = pMain.GetIcon("Elemental", "", nAttackAttributeType, 0);
				if (pIcon != null)
					pbAttackIcon.Image = pIcon;

				cbAttackAttributeType.SelectedIndex = nAttackAttributeType;
			}

			if (nAttackAttributeLevel >= Defs.AT_LEVELMAX)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: Attack Attribute Level out of range.");
			else
				cbAttackAttributeLevel.SelectedIndex = nAttackAttributeLevel;
			/****************************************/
			int nAIType = Convert.ToInt32(pTempNPCRow["a_aitype"]);

			if (nAIType < 0 || nAIType >= Defs.NPCAIType.Count())
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_aitype out of range.");
			else
				cbAIType.SelectedIndex = nAIType;

			btnAIFlag.Text = pTempNPCRow["a_aiflag"].ToString();

			strTooltip = new StringBuilder();
			lFlag = Convert.ToInt64(pTempNPCRow["a_aiflag"]);

			for (i = 0; i < Defs.NPCAIFlag.Length; i++)
			{
				if ((lFlag & 1L << i) != 0)
					strTooltip.Append(Defs.NPCAIFlag[i] + "\n");
			}

			if (lFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_aiflag out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnAIFlag, strTooltip.ToString());
			pToolTips[btnAIFlag] = pToolTip;

			int nAILeaderFlag = Convert.ToInt32(pTempNPCRow["a_aileader_flag"]);

			if (nAILeaderFlag < 0 || nAILeaderFlag >= Defs.NPCAILeaderFlag.Count())
				pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_aileader_flag out of range.");
			else
				cbAILeaderFlag.SelectedIndex = nAILeaderFlag;

			tbAISummonHP.Text = pTempNPCRow["a_ai_summonHp"].ToString();

			int nAILeaderID = Convert.ToInt32(pTempNPCRow["a_aileader_idx"]);

			DataRow pNPCRow = pMain.pTables.NPCTable.Select("a_index=" + nAILeaderID).FirstOrDefault();
			if (pNPCRow != null)
			{
				btnAILeaderIDX.Text = $"{nAILeaderID} - {pNPCRow["a_name_" + pMain.pSettings.WorkLocale]}";
			}
			else
			{
				if (nAILeaderFlag > 0)
					pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_aileader_idx: {nAILeaderID} not exist in t_npc.");

				btnAILeaderIDX.Text = "0";
			}

			tbAILeaderCount.Text = pTempNPCRow["a_aileader_count"].ToString();
			/****************************************/
			for (i = 0; i < Defs.MAX_NPC_PRODUCT; i++)
			{
				int nProductItemID = Convert.ToInt32(pTempNPCRow["a_product" + i]);
				string strProductID = nProductItemID.ToString();
				btnObj = (Button)Controls.Find("btnProduct" + i, true)[0];

				pTempDataRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nProductItemID).FirstOrDefault();
				if (pTempDataRow != null)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pTempDataRow["a_texture_id"].ToString(), Convert.ToInt32(pTempDataRow["a_texture_row"]), Convert.ToInt32(pTempDataRow["a_texture_col"])), new Size(24, 24));

					strProductID += " - " + pTempDataRow["a_name_" + pMain.pSettings.WorkLocale];
				}
				else
				{
					if (nProductItemID > 0)
						pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_product{i}: {nProductItemID} not exist in t_item.");
				}

				btnObj.Text = strProductID;
			}
			/****************************************/
			tbPlusMin.Text = pTempNPCRow["a_minplus"].ToString();
			tbPlusMax.Text = pTempNPCRow["a_maxplus"].ToString();
			tbPlusProb.Text = pTempNPCRow["a_probplus"].ToString();
			/****************************************/
			tbAnimationIdle.Text = pTempNPCRow["a_motion_idle"].ToString();
			tbAnimationIdle2.Text = pTempNPCRow["a_motion_idle2"].ToString();
			tbAnimationWalk.Text = pTempNPCRow["a_motion_walk"].ToString();
			tbAnimationRun.Text = pTempNPCRow["a_motion_run"].ToString();
			tbAnimationDamage.Text = pTempNPCRow["a_motion_dam"].ToString();
			tbAnimationAttack.Text = pTempNPCRow["a_motion_attack"].ToString();
			tbAnimationAttack2.Text = pTempNPCRow["a_motion_attack2"].ToString();
			tbAnimationDie.Text = pTempNPCRow["a_motion_die"].ToString();
			/****************************************/
			tbFireDelayCount.Text = pTempNPCRow["a_fireDelayCount"].ToString();

			for (i = 0; i < Defs.DEF_MAX_NPC_FIRE_DELAY; i++)
				((TextBox)Controls.Find("tbFireDelay" + i, true)[0]).Text = Convert.ToString(pTempNPCRow["a_fireDelay" + i], CultureInfo.InvariantCulture);

			tbFireObject.Text = pTempNPCRow["a_fireObject"].ToString();
			tbFireSpeed.Text = Convert.ToString(pTempNPCRow["a_fireSpeed"], CultureInfo.InvariantCulture);

			for (i = 0; i < Defs.DEF_MOB_FIRE_EFFECT; i++)
				((TextBox)Controls.Find("tbFireEffect" + i, true)[0]).Text = pTempNPCRow["a_fireEffect" + i].ToString();
			/****************************************/
			for (i = 0; i < Defs.MAX_NPC_DROPITEM; i++)
			{
				int nItemID = Convert.ToInt32(pTempNPCRow["a_item_" + i]);
				int nItemProb = Convert.ToInt32(pTempNPCRow["a_item_percent_" + i]);
				string strItemID = nItemID.ToString();
				btnObj = (Button)Controls.Find("btnItemDrop" + i, true)[0];
				tbObj = (TextBox)Controls.Find($"tbItemDrop{i}Prob", true)[0];

				pTempDataRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).FirstOrDefault();
				if (pTempDataRow != null)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pTempDataRow["a_texture_id"].ToString(), Convert.ToInt32(pTempDataRow["a_texture_row"]), Convert.ToInt32(pTempDataRow["a_texture_col"])), new Size(24, 24));

					tbObj.Enabled = true;

					strItemID += " - " + pTempDataRow["a_name_" + pMain.pSettings.WorkLocale];
				}
				else
				{
					if (nItemID > 0)
						pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_item_{i}: {nItemID} not exist in t_item.");

					tbObj.Enabled = false;
				}

				btnObj.Text = strItemID;

				tbObj.Text = nItemProb.ToString();

				((Label)Controls.Find("lItemDropProbPercentage" + i, true)[0]).Text = ((nItemProb * 100.0f) / 10000.0f) + "%";
			}
			/****************************************/
			for (i = 0; i < Defs.MAX_NPC_DROPJEWEL; i++)
			{
				int nItemID = Convert.ToInt32(pTempNPCRow["a_jewel_" + i]);
				int nItemProb = Convert.ToInt32(pTempNPCRow["a_jewel_percent_" + i]);
				string strItemID = nItemID.ToString();
				btnObj = (Button)Controls.Find("btnJewelDrop" + i, true)[0];
				tbObj = (TextBox)Controls.Find($"tbJewelDrop{i}Prob", true)[0];

				pTempDataRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nItemID).FirstOrDefault();
				if (pTempDataRow != null)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pTempDataRow["a_texture_id"].ToString(), Convert.ToInt32(pTempDataRow["a_texture_row"]), Convert.ToInt32(pTempDataRow["a_texture_col"])), new Size(24, 24));

					tbObj.Enabled = true;

					strItemID += " - " + pTempDataRow["a_name_" + pMain.pSettings.WorkLocale];
				}
				else
				{
					if (nItemID > 0)
						pMain.Logger(LogTypes.Error, $"NPC Editor > NPC: {nNPCID} Error: a_jewel_{i}: {nItemID} not exist in t_item.");

					tbObj.Enabled = false;
				}

				btnObj.Text = strItemID;

				tbObj.Text = nItemProb.ToString();

				((Label)Controls.Find("lJewelDropProbPercentage" + i, true)[0]).Text = ((nItemProb * 100.0f) / 10000.0f) + "%";
			}
			/****************************************/
			if (bLoadFrompNPCTable)
			{
				LoadDropJobData(nNPCID);
				LoadDropAllData(nNPCID);
				LoadDropRaidData(nNPCID);

				LoadRegenData(nNPCID);
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

				pMain.pTables.NPCTable?.Dispose();
				pMain.pTables.NPCTable = null;

				pMain.pTables.NPCRegenTable?.Dispose();
				pMain.pTables.NPCRegenTable = null;

				pMain.pTables.ZoneTable?.Dispose();
				pMain.pTables.ZoneTable = null;

				pMain.pTables.SkillTable?.Dispose();
				pMain.pTables.SkillTable = null;

				pMain.pTables.SkillLevelTable?.Dispose();
				pMain.pTables.SkillLevelTable = null;

				pMain.pTables.ItemTable?.Dispose();
				pMain.pTables.ItemTable = null;

				pMain.pTables.StringTable?.Dispose();
				pMain.pTables.StringTable = null;

				if (pMain.pTables.NPCDropJobTable != null)
				{
					pMain.pTables.NPCDropJobTable.Dispose();
					pMain.pTables.NPCDropJobTable = null;
				}

				if (pMain.pTables.NPCDropAllTable != null)
				{
					pMain.pTables.NPCDropAllTable.Dispose();
					pMain.pTables.NPCDropAllTable = null;
				}

				if (pMain.pTables.NPCDropRaidTable != null)
				{
					pMain.pTables.NPCDropRaidTable.Dispose();
					pMain.pTables.NPCDropRaidTable = null;
				}

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				NPCEditor_LoadAsync(sender, e);
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
				int i, nNewNPCID = 9999;
				DataRow pNewRow;

				List<string> listIntColumns =   // Here add all int columns.
				[
					"a_index",
					"a_enable",
					"a_flag1",
					"a_state_flag",
					"a_level",
					"a_exp",
					"a_prize",
					"a_skill_point",
					"a_str",
					"a_dex",
					"a_int",
					"a_con",
					"a_attack",
					"a_magic",
					"a_defense",
					"a_resist",
					"a_attacklevel",
					"a_defenselevel",
					"a_hp",
					"a_mp",
					"a_recover_hp",
					"a_recover_mp",
					"a_item_0", "a_item_percent_0",
					"a_item_1", "a_item_percent_1",
					"a_item_2", "a_item_percent_2",
					"a_item_3", "a_item_percent_3",
					"a_item_4", "a_item_percent_4",
					"a_item_5", "a_item_percent_5",
					"a_item_6", "a_item_percent_6",
					"a_item_7", "a_item_percent_7",
					"a_item_8", "a_item_percent_8",
					"a_item_9", "a_item_percent_9",
					"a_item_10", "a_item_percent_10",
					"a_item_11", "a_item_percent_11",
					"a_item_12", "a_item_percent_12",
					"a_item_13", "a_item_percent_13",
					"a_item_14", "a_item_percent_14",
					"a_item_15", "a_item_percent_15",
					"a_item_16", "a_item_percent_16",
					"a_item_17", "a_item_percent_17",
					"a_item_18", "a_item_percent_18",
					"a_item_19", "a_item_percent_19",
					"a_minplus",
					"a_maxplus",
					"a_probplus",
					"a_product0",
					"a_product1",
					"a_product2",
					"a_product3",
					"a_product4",
					"a_attribute",
					"a_aitype",
					"a_aiflag",
					"a_aileader_flag",
					"a_ai_summonHp",
					"a_aileader_idx",
					"a_aileader_count",
					"a_hit",
					"a_dodge",
					"a_magicavoid",
					"a_job_attribute",
					"a_jewel_0", "a_jewel_percent_0",
					"a_jewel_1", "a_jewel_percent_1",
					"a_jewel_2", "a_jewel_percent_2",
					"a_jewel_3", "a_jewel_percent_3",
					"a_jewel_4", "a_jewel_percent_4",
					"a_jewel_5", "a_jewel_percent_5",
					"a_jewel_6", "a_jewel_percent_6",
					"a_jewel_7", "a_jewel_percent_7",
					"a_jewel_8", "a_jewel_percent_8",
					"a_jewel_9", "a_jewel_percent_9",
					"a_jewel_10", "a_jewel_percent_10",
					"a_jewel_11", "a_jewel_percent_11",
					"a_jewel_12", "a_jewel_percent_12",
					"a_jewel_13", "a_jewel_percent_13",
					"a_jewel_14", "a_jewel_percent_14",
					"a_jewel_15", "a_jewel_percent_15",
					"a_jewel_16", "a_jewel_percent_16",
					"a_jewel_17", "a_jewel_percent_17",
					"a_jewel_18", "a_jewel_percent_18",
					"a_jewel_19", "a_jewel_percent_19",
					"a_rvr_value",
					"a_rvr_grade",
					"a_lifetime"
#if NPC_CHANNEL
					, "a_channel_flag"
#endif
#if REWARDS_BY_DAMAGE
					, "a_reward_gold_min", "a_reward_gold_max", "a_reward_gold_multiplier"
#endif
				];

				List<string> listTinyIntColumns =   // Here add all tinyint columns.
				[
					"a_skillmaster",
					"a_sskill_master",
					"a_attackType",
					"a_attackSpeed",
					"a_fireDelayCount",
					"a_fireObject"
				];

				List<string> listVarcharColumns =   // Here add all varchar columns.
				[
					"a_skill0",
					"a_skill1",
					"a_skill2",
					"a_skill3",
					"a_file_smc",
					"a_motion_walk",
					"a_motion_idle",
					"a_motion_dam",
					"a_motion_attack",
					"a_motion_die",
					"a_motion_run",
					"a_motion_idle2",
					"a_motion_attack2",
					"a_fireEffect0",
					"a_fireEffect1",
					"a_fireEffect2"
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listVarcharColumns.AddRange("a_name_" + strNation.ToLower(), "a_descr_" + strNation.ToLower());

				List<string> listBigIntColumns =    // Here add all bigint columns.
				[
					"a_flag",
					"a_zone_flag",
					"a_extra_flag"
				];

				List<string> listSmallIntColumns =  // Here add all smallint columns.
				[
					"a_family",
					"a_move_area"
				];

				List<string> listFloatColumns =	// Here add all float columns.
				[
					"a_sight",
					"a_size",
					"a_attack_area",
					"a_walk_speed",
					"a_run_speed",
					"a_scale",
					"a_fireDelay0",
					"a_fireDelay1",
					"a_fireDelay2",
					"a_fireDelay3",
					"a_fireSpeed",
					"a_bound"
				];

				if (pMain.pTables.NPCTable == null)    // If is null, create new DataTable and set schema (column name & datatype).
				{
					DataTable pNPCTableStruct = new();

					foreach (string strColumnName in listIntColumns)
						pNPCTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listTinyIntColumns)
						pNPCTableStruct.Columns.Add(strColumnName, typeof(sbyte));

					foreach (string strColumnName in listVarcharColumns)
						pNPCTableStruct.Columns.Add(strColumnName, typeof(string));

					foreach (string strColumnName in listBigIntColumns)
						pNPCTableStruct.Columns.Add(strColumnName, typeof(long));

					foreach (string strColumnName in listSmallIntColumns)
						pNPCTableStruct.Columns.Add(strColumnName, typeof(short));

					foreach (string strColumnName in listFloatColumns)
						pNPCTableStruct.Columns.Add(strColumnName, typeof(float));

					pNewRow = pNPCTableStruct.NewRow();

					pNPCTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_npc ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewNPCID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewNPCID = pMain.AskForIndex(this.Text, "a_index")) == -1)    // I don't test it...
							return;
					}
				}
				else
				{
					nNewNPCID = Convert.ToInt32(pMain.pTables.NPCTable.Select().LastOrDefault()["a_index"]) + 1;

					pNewRow = pMain.pTables.NPCTable.NewRow();
				}

				List<object> listDefaultValue =
				[
					nNewNPCID,	// a_index
					0,	// a_enable
					0,	// a_flag1
					0,	// a_state_flag
					0,	// a_level
					0,	// a_exp
					0,	// a_prize
					0,	// a_skill_point
					0,	// a_str
					0,	// a_dex
					0,	// a_int
					0,	// a_con
					0,	// a_attack
					0,	// a_magic
					0,	// a_defense
					0,	// a_resist
					0,	// a_attacklevel
					0,	// a_defenselevel
					0,	// a_hp
					0,	// a_mp
					0,	// a_recover_hp
					0,	// a_recover_mp
					-1,	// a_item_0
					0,	// a_item_percent_0
					-1,	// a_item_1
					0,	// a_item_percent_1
					-1,	// a_item_2
					0,	// a_item_percent_2
					-1,	// a_item_3
					0,	// a_item_percent_3
					-1,	// a_item_4
					0,	// a_item_percent_4
					-1,	// a_item_5
					0,	// a_item_percent_5
					-1,	// a_item_6
					0,	// a_item_percent_6
					-1,	// a_item_7
					0,	// a_item_percent_7
					-1,	// a_item_8
					0,	// a_item_percent_8
					-1,	// a_item_9
					0,	// a_item_percent_9
					-1,	// a_item_10
					0,	// a_item_percent_10
					-1,	// a_item_11
					0,	// a_item_percent_11
					-1,	// a_item_12
					0,	// a_item_percent_12
					-1,	// a_item_13
					0,	// a_item_percent_13
					-1,	// a_item_14
					0,	// a_item_percent_14
					-1,	// a_item_15
					0,	// a_item_percent_15
					-1,	// a_item_16
					0,	// a_item_percent_16
					-1,	// a_item_17
					0,	// a_item_percent_17
					-1,	// a_item_18
					0,	// a_item_percent_18
					-1,	// a_item_19
					0,	// a_item_percent_19
					0,	// a_minplus
					0,	// a_maxplus
					0,	// a_probplus
					-1,	// a_product4
					-1,	// a_product0
					-1,	// a_product1
					-1,	// a_product2
					-1,	// a_product3
					0,	// a_attribute
					0,	// a_aitype
					0,	// a_aiflag
					0,	// a_aileader_flag
					0,	// a_ai_summonHp
					0,	// a_aileader_idx
					0,	// a_aileader_count
					0,	// a_hit
					0,	// a_dodge
					0,	// a_magicavoid
					0,	// a_job_attribute
					-1,	// a_jewel_0
					0,	// a_jewel_percent_0
					-1,	// a_jewel_1
					0,	// a_jewel_percent_1
					-1,	// a_jewel_2
					0,	// a_jewel_percent_2
					-1,	// a_jewel_3
					0,	// a_jewel_percent_3
					-1,	// a_jewel_4
					0,	// a_jewel_percent_4
					-1,	// a_jewel_5
					0,	// a_jewel_percent_5
					-1,	// a_jewel_6
					0,	// a_jewel_percent_6
					-1,	// a_jewel_7
					0,	// a_jewel_percent_7
					-1,	// a_jewel_8
					0,	// a_jewel_percent_8
					-1,	// a_jewel_9
					0,	// a_jewel_percent_9
					-1,	// a_jewel_10
					0,	// a_jewel_percent_10
					-1,	// a_jewel_11
					0,	// a_jewel_percent_11
					-1,	// a_jewel_12
					0,	// a_jewel_percent_12
					-1,	// a_jewel_13
					0,	// a_jewel_percent_13
					-1,	// a_jewel_14
					0,	// a_jewel_percent_14
					-1,	// a_jewel_15
					0,	// a_jewel_percent_15
					-1,	// a_jewel_16
					0,	// a_jewel_percent_16
					-1,	// a_jewel_17
					0,	// a_jewel_percent_17
					-1,	// a_jewel_18
					0,	// a_jewel_percent_18
					-1,	// a_jewel_19
					0,	// a_jewel_percent_19
					0,	// a_rvr_value",
					0,	// a_rvr_grade",
					0,	// a_lifetime",
#if NPC_CHANNEL
					-1,	// a_channel_flag
#endif
#if REWARDS_BY_DAMAGE
					0,	// a_reward_gold_min
					0,	// a_reward_gold_max
					0,	// a_reward_gold_multiplier
#endif
					-1,	// a_skillmaster
					-1,	// a_sskill_master
					-1,	// a_attackType
					0,	// a_attackSpeed
					0,	// a_fireDelayCount
					-1,	// a_fireObject
					"-1",	// a_skill0
					"-1",	// a_skill1
					"-1",	// a_skill2
					"-1",	// a_skill3
					"",	// a_file_smc
					"",	// a_motion_walk
					"",	// a_motion_idle
					"",	// a_motion_dam
					"",	// a_motion_attack
					"",	// a_motion_die
					"",	// a_motion_run
					"",	// a_motion_idle2
					"",	// a_motion_attack2
					"",	// a_fireEffect0
					"",	// a_fireEffect1
					""	// a_fireEffect2
				];

				foreach (string strNation in pMain.pSettings.NationSupported)
					listDefaultValue.AddRange("New NPC", "Created with NicolasG LastChaos ToolBox");

				listDefaultValue.AddRange(
					0,	// a_flag
					0,	// a_zone_flag
					0,	// a_extra_flag
					-1,	// a_family
					0,	// a_move_area
					0,	// a_sight
					0,	// a_size
					0,	// a_attack_area
					0,	// a_walk_speed
					0,	// a_run_speed
					1,	// a_scale
					0,	// a_fireDelay0
					0,	// a_fireDelay1
					0,	// a_fireDelay2
					0,	// a_fireDelay3
					0,	// a_fireSpeed
					0	// a_bound
				);

				i = 0;

				foreach (string strColumnName in listIntColumns.Concat(listTinyIntColumns).Concat(listVarcharColumns).Concat(listBigIntColumns).Concat(listSmallIntColumns).Concat(listFloatColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempNPCRow = pNewRow;
				}
				catch (Exception ex)
				{
					string strError = $"NPC Editor > NPC: {nNewNPCID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "NPC Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						pTempRegenRows = Array.Empty<DataRow>();

						AddToList(nNewNPCID, "New NPC", true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this NPC. Because it's temporary.", "NPC Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nNPCIDToCopy = Convert.ToInt32(pTempNPCRow["a_index"]);
				int nNewNPCID = Convert.ToInt32(pMain.pTables.NPCTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempNPCRow = pMain.pTables.NPCTable.NewRow();
				pTempNPCRow.ItemArray = (object[])pMain.pTables.NPCTable.Select("a_index=" + nNPCIDToCopy)[0].ItemArray.Clone();

				pTempNPCRow["a_index"] = nNewNPCID;

				foreach (string strNation in pMain.pSettings.NationSupported)
					pTempNPCRow["a_name_" + strNation.ToLower()] = pTempNPCRow["a_name_" + strNation.ToLower()] + " Copy";

				AddToList(nNewNPCID, pTempNPCRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty, true);

				// Drop Job
				LoadDropJobData(nNPCIDToCopy);

				if (pTempDropJobRow != null)
				{
					DataRow newRow = pMain.pTables.NPCDropJobTable.NewRow();
					newRow.ItemArray = (object[])pTempDropJobRow.ItemArray.Clone();
					newRow["a_npc_idx"] = nNewNPCID;

					pTempDropJobRow = newRow;
				}

				// Drop All
				LoadDropAllData(nNPCIDToCopy);

				if (pTempDropAllRows != null)
				{
					DataRow[] clonedRows = new DataRow[pTempDropAllRows.Length];
					for (int i = 0; i < pTempDropAllRows.Length; i++)
					{
						DataRow newRow = pMain.pTables.NPCDropAllTable.NewRow();
						newRow.ItemArray = (object[])pTempDropAllRows[i].ItemArray.Clone();
						newRow["a_npc_idx"] = nNewNPCID;
						clonedRows[i] = newRow;
					}

					pTempDropAllRows = clonedRows;
				}

				// Drop Raid
				LoadDropRaidData(nNPCIDToCopy);

				if (pTempDropRaidRows != null)
				{
					DataRow[] clonedRows = new DataRow[pTempDropRaidRows.Length];
					for (int i = 0; i < pTempDropRaidRows.Length; i++)
					{
						DataRow newRow = pMain.pTables.NPCDropRaidTable.NewRow();
						newRow.ItemArray = (object[])pTempDropRaidRows[i].ItemArray.Clone();
						newRow["a_npc_index"] = nNewNPCID;
						clonedRows[i] = newRow;
					}

					pTempDropRaidRows = clonedRows;
				}

				// Regen
				LoadRegenData(nNPCIDToCopy);

				if (pTempRegenRows != null)
				{
					DataRow[] clonedRows = new DataRow[pTempRegenRows.Length];
					for (int i = 0; i < pTempRegenRows.Length; i++)
					{
						DataRow newRow = pMain.pTables.NPCRegenTable.NewRow();
						newRow.ItemArray = (object[])pTempRegenRows[i].ItemArray.Clone();
						newRow["a_npc_idx"] = nNewNPCID;
						clonedRows[i] = newRow;
					}

					pTempRegenRows = clonedRows;
				}

				/* TODO: Aks if want to copy: (This needs changes in btnDelete, btnCopy & btnUpdate)
					t_npc_cube
					t_npc_regen_raid
				 */
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nNPCID = Convert.ToInt32(pTempNPCRow["a_index"]);
			DataRow pNPCTableRow = pMain.pTables.NPCTable.Select("a_index=" + nNPCID).FirstOrDefault();

			if (pNPCTableRow != null)
			{
				StringBuilder strbuilderQuery = new();

				strbuilderQuery.Append("START TRANSACTION;\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_dropjob WHERE a_npc_idx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_dropraid WHERE a_npc_index={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_drop_all WHERE a_npc_idx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_affinity_npc WHERE a_npcidx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_cube WHERE a_npcidx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_regen WHERE a_npc_idx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_regen_combo WHERE a_npcidx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_regen_raid WHERE a_npc_idx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_monster_mercenary WHERE a_npc_idx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_shop WHERE a_keeper_idx={nNPCID};\n");

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc WHERE a_index={nNPCID};\n");

				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _)))
				{
					string strError = $"NPC Editor > NPC: {nNPCID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "NPC Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pMain.pTables.NPCDropJobTable != null)
					{
						DataRow pRow = pMain.pTables.NPCDropJobTable.Select("a_npc_idx=" + nNPCID).FirstOrDefault();
						if (pRow != null)
							pMain.pTables.NPCDropJobTable.Rows.Remove(pRow);
					}

					if (pMain.pTables.NPCDropRaidTable != null)
					{
						DataRow[] pRows = pMain.pTables.NPCDropRaidTable.Select("a_npc_index=" + nNPCID);
						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.NPCDropRaidTable.Rows.Remove(pRow);
						}
					}

					if (pMain.pTables.NPCDropAllTable != null)
					{
						DataRow[] pRows = pMain.pTables.NPCDropAllTable.Select("a_npc_idx=" + nNPCID);
						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.NPCDropAllTable.Rows.Remove(pRow);
						}
					}

					if (pMain.pTables.NPCRegenTable != null)
					{
						DataRow[] pRows = pMain.pTables.NPCRegenTable.Select("a_npc_idx=" + nNPCID);
						if (pRows.Length > 0)
						{
							foreach (DataRow pRow in pRows)
								pMain.pTables.NPCRegenTable.Rows.Remove(pRow);
						}
					}

					if (pNPCTableRow != null)
						pMain.pTables.NPCTable.Rows.Remove(pNPCTableRow);
				}
				catch (Exception ex)
				{
					string strError = $"NPC Editor > NPC: {nNPCID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "NPC Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("NPC Deleted successfully!", "NPC Editor", MessageBoxButtons.OK);

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

				pTempNPCRow["a_enable"] = strEnable;

				bUnsavedChanges = true;
			}
		}

		private string NormalizeSMCPathForDatabase(string? strPath)
		{
			string strSMCPath = (strPath ?? string.Empty).Trim().Trim('"').Replace('/', '\\');

			if (string.IsNullOrWhiteSpace(strSMCPath))
				return string.Empty;

			string strClientPath = pMain.pSettings.ClientPath.TrimEnd('\\');

			if (strSMCPath.StartsWith(strClientPath + "\\", StringComparison.OrdinalIgnoreCase))
				strSMCPath = strSMCPath.Substring(strClientPath.Length + 1);

			int nDataIndex = strSMCPath.IndexOf("\\Data\\", StringComparison.OrdinalIgnoreCase);
			if (nDataIndex >= 0)
				strSMCPath = strSMCPath.Substring(nDataIndex + 1);

			while (strSMCPath.StartsWith("\\") || strSMCPath.StartsWith(".\\"))
				strSMCPath = strSMCPath.TrimStart('\\').TrimStart('.', '\\');

			if (!strSMCPath.StartsWith("Data\\", StringComparison.OrdinalIgnoreCase))
				strSMCPath = "Data\\" + strSMCPath;

			return strSMCPath;
		}

		private string GetSMCPathForDisplay(string strPath)
		{
			string strSMCPath = NormalizeSMCPathForDatabase(strPath);
			const string strDataPrefix = "Data\\";

			if (strSMCPath.StartsWith(strDataPrefix, StringComparison.OrdinalIgnoreCase))
				return strSMCPath.Substring(strDataPrefix.Length);

			return strSMCPath;
		}

		private void SetSMCPath(string? strPath, bool bMarkUnsaved)
		{
			string strDatabasePath = NormalizeSMCPathForDatabase(strPath);
			string strDisplayPath = GetSMCPathForDisplay(strDatabasePath);

			bSettingSMCText = true;
			tbSMC.Text = strDisplayPath;
			tbSMC.SelectionStart = tbSMC.TextLength;
			tbSMC.SelectionLength = 0;
			bSettingSMCText = false;

			if (bMarkUnsaved && bUserAction)
			{
				pTempNPCRow["a_file_smc"] = strDatabasePath;

				bUnsavedChanges = true;
			}
		}

		private void tbSMC_DoubleClick(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				OpenFileDialog pFileDialog = new OpenFileDialog { Title = "NPC Editor", Filter = "SMC Files|*.smc", InitialDirectory = pMain.pSettings.ClientPath + "\\Data" };
				if (pFileDialog.ShowDialog() == DialogResult.OK)
					SetSMCPath(pFileDialog.FileName, true);
			}
		}

		private void tbSMC_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction && !bSettingSMCText)
			{
				pTempNPCRow["a_file_smc"] = NormalizeSMCPathForDatabase(tbSMC.Text);

				bUnsavedChanges = true;
			}
		}

		private void tbSMC_Leave(object sender, EventArgs e)
		{
			if (bUserAction)
				SetSMCPath(tbSMC.Text, true);
		}

		private void tbSMC_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A)
			{
				tbSMC.SelectAll();
				e.SuppressKeyPress = true;
			}
			else if (e.Control && e.KeyCode == Keys.V && Clipboard.ContainsText())
			{
				SetSMCPath(Clipboard.GetText(), true);
				e.SuppressKeyPress = true;
			}
		}

		private void tbHP_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_hp"] = tbHP.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbMP_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_mp"] = tbMP.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbLevel_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_level"] = tbLevel.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbFamily_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_family"] = tbFamily.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbSkillMaster_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbSkillMaster.SelectedIndex;
				if (nType != -1)
				{
					pTempNPCRow["a_skillmaster"] = nType == 0 ? -1 : nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void cbSpecialSkillMaster_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbSpecialSkillMaster.SelectedIndex;
				if (nType != -1)
				{
					pTempNPCRow["a_sskill_master"] = nType == 0 ? -1 : nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void btnFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.NPCFlags, Convert.ToInt64(btnFlag.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();
#if REWARDS_BY_DAMAGE
				if ((lFlag & (1L << Array.IndexOf(Defs.NPCFlags, "WORLD_BOSS_DROP"))) != 0)
					gbWorldBossDrop.Enabled = true;
				else
					gbWorldBossDrop.Enabled = false;
#endif
				for (int i = 0; i < Defs.NPCFlags.Length; i++)
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(Defs.NPCFlags[i] + "\n");
				}

				pToolTips[btnFlag].SetToolTip(btnFlag, strTooltip.ToString());

				pTempNPCRow["a_flag"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void btnFlag1_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.NPCFlags1, Convert.ToInt64(btnFlag1.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnFlag1.Text = lFlag.ToString();

				StringBuilder strTooltip = new();

				for (int i = 0; i < Defs.NPCFlags1.Length; i++)
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(Defs.NPCFlags1[i] + "\n");
				}

				pToolTips[btnFlag1].SetToolTip(btnFlag1, strTooltip.ToString());

				pTempNPCRow["a_flag1"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void btnStateFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.MagicTypesAndSubTypes[Defs.MagicTypesAndSubTypes.Keys.ElementAt(2)].ToArray(), Convert.ToInt64(btnStateFlag.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnStateFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();
				int i = 0;

				foreach (string strSubType in Defs.MagicTypesAndSubTypes[Defs.MagicTypesAndSubTypes.Keys.ElementAt(2)])
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(strSubType + "\n");

					i++;
				}

				pToolTips[btnStateFlag].SetToolTip(btnStateFlag, strTooltip.ToString());

				pTempNPCRow["a_state_flag"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void btnZoneFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, strZones, Convert.ToInt64(btnZoneFlag.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnZoneFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();

				for (int i = 0; i < pMain.pTables.ZoneTable.Rows.Count; i++)
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(pMain.pTables.ZoneTable.Rows[i]["a_name"] + "\n");
				}

				pToolTips[btnZoneFlag].SetToolTip(btnZoneFlag, strTooltip.ToString());

				pTempNPCRow["a_zone_flag"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void cbExtraZone_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbExtraZone.SelectedIndex;
				if (nType != -1)
				{
					pTempNPCRow["a_extra_flag"] = nType;

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

				tbName.Text = pTempNPCRow["a_name_" + strNation].ToString();

				tbDescription.Text = pTempNPCRow["a_descr_" + strNation].ToString();

				bUserAction = true;
			}
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_name_" + cbNationSelector.SelectedItem.ToString().ToLower()] = tbName.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbDescription_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_descr_" + cbNationSelector.SelectedItem.ToString().ToLower()] = tbDescription.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPrizeExperience_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_exp"] = tbPrizeExperience.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPrizeGoldCoin_TextChanged(object sender, EventArgs e)
		{
			tbPrizeGoldCoin.ForeColor = pMain.GetGoldColor(Convert.ToInt64(tbPrizeGoldCoin.Text));

			if (bUserAction)
			{
				pTempNPCRow["a_prize"] = tbPrizeGoldCoin.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPrizeSkillPoint_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_skill_point"] = tbPrizeSkillPoint.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbStr_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_str"] = tbStr.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbDex_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_dex"] = tbDex.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbInt_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_int"] = tbInt.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbCon_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_con"] = tbCon.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbWalkSpeed_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_walk_speed"] = tbWalkSpeed.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbRunSpeed_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_run_speed"] = tbRunSpeed.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbAttackTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbAttackTypeSelector.SelectedIndex;
				if (nType != -1)
				{
					pTempNPCRow["a_attackType"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void tbAttackSpeed_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_attackSpeed"] = tbAttackSpeed.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAttackArea_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_attack_area"] = tbAttackArea.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAttackLevel_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_attacklevel"] = tbAttackLevel.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAttack_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_attack"] = tbAttack.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbMagic_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_magic"] = tbMagic.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbHit_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_hit"] = tbHit.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbMoveArea_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_move_area"] = tbMoveArea.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbSight_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_sight"] = tbSight.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbRecoverHP_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_recover_hp"] = tbRecoverHP.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbRecoverMP_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_recover_mp"] = tbRecoverMP.Text;

				bUnsavedChanges = true;
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
				pTempNPCRow["a_rvr_value"] = nType;

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
					pTempNPCRow["a_rvr_grade"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void tbSize_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_size"] = tbSize.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbScale_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_scale"] = tbScale.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbBound_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_bound"] = tbBound.Text;

				bUnsavedChanges = true;
			}
		}

		private void btnChannelFlag_Click(object sender, EventArgs e)
		{
#if NPC_CHANNEL
			if (bUserAction && pMain.pTables.StringTable != null)
			{
				long lActualFlag = Convert.ToInt64(btnChannelFlag.Text.ToString());

				FlagPicker pFlagSelector = new(this, pMain.pTables.StringTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) >= 6291 && Convert.ToInt32(row["a_index"]) <= (6290 + Defs.MAX_CHANNELS)).Select(row => row["a_string_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty).ToArray(), lActualFlag != -1 ? lActualFlag : 0);
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;
				if (lFlag == 0)
					lFlag = -1;

				btnChannelFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();

				if (lFlag != -1)
				{
					DataRow[] listChannelNames = pMain.pTables.StringTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) >= 6291 && Convert.ToInt32(row["a_index"]) <= (6290 + Defs.MAX_CHANNELS)).ToArray();
					int i = 0;

					foreach (DataRow pRowString in listChannelNames)
					{
						if ((lFlag & 1L << i) != 0)
							strTooltip.Append(pRowString["a_string_" + pMain.pSettings.WorkLocale] + "\n");

						i++;
					}
				}
				else
				{
					strTooltip.Append("All");
				}

				pToolTips[btnChannelFlag].SetToolTip(btnChannelFlag, strTooltip.ToString());

				pTempNPCRow["a_channel_flag"] = lFlag;

				bUnsavedChanges = true;
			}
#endif
		}

		private void tbLifeTime_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_lifetime"] = tbLifeTime.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbDefenseLevel_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_defenselevel"] = tbDefenseLevel.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbDefense_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_defense"] = tbDefense.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbResist_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_resist"] = tbResist.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbDodge_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_dodge"] = tbDodge.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbMagicAvoid_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_magicavoid"] = tbMagicAvoid.Text;

				bUnsavedChanges = true;
			}
		}

		private void btnJobAttribute_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.CharactersClassNJobsTypes.Select(kvp => kvp.Value[0].Substring(4)).ToArray(), Convert.ToInt64(btnJobAttribute.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnJobAttribute.Text = lFlag.ToString();

				StringBuilder strTooltip = new();

				foreach (var ClassData in Defs.CharactersClassNJobsTypes)
				{
					if ((lFlag & 1L << ClassData.Key) != 0)
						strTooltip.Append(ClassData.Value[0].Substring(4) + "\n");
				}

				pToolTips[btnJobAttribute].SetToolTip(btnJobAttribute, strTooltip.ToString());

				pTempNPCRow["a_job_attribute"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void cbAttribute_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nDefenseType = cbDefenseAttributeType.SelectedIndex;
				int nDefenseLevel = cbDefenseAttributeLevel.SelectedIndex;
				int nAttackType = cbAttackAttributeType.SelectedIndex;
				int nAttackLevel = cbAttackAttributeLevel.SelectedIndex;

				if (nDefenseType != -1 && nDefenseLevel != -1 && nAttackType != -1 && nAttackLevel != -1)
				{
					Image pIcon = pMain.GetIcon("Elemental", "", nDefenseType, 1);
					pbDefenseIcon.Image = pIcon;

					pIcon = pMain.GetIcon("Elemental", "", nAttackType, 0);
					pbAttackIcon.Image = pIcon;

					pTempNPCRow["a_attribute"] = pMain.AT_ADMIX(pMain.AT_MIX(nAttackType, nAttackLevel), pMain.AT_MIX(nDefenseType, nDefenseLevel));

					bUnsavedChanges = true;
				}
			}
		}
		/****************************************/
		private void ChangeSkillLevelAndProb(int nNumber)
		{
			int nSkillProb = Convert.ToInt32(((TextBox)Controls.Find("tbSkillProb" + nNumber, true)[0]).Text);

			((Label)Controls.Find("lSkillProbPercentage" + nNumber, true)[0]).Text = ((nSkillProb * 100.0f) / 10000.0f) + "%";

			if (bUserAction)
			{
				int nSkillLevel = ((ComboBox)Controls.Find("cbSkillLevel" + nNumber, true)[0]).SelectedIndex;

				if (nSkillLevel != -1)
					nSkillLevel++;
				else
					nSkillLevel = 1;

				pTempNPCRow["a_skill" + nNumber] = $"{pTempNPCRow["a_skill" + nNumber].ToString().Split(' ')[0]} {nSkillLevel} {nSkillProb}";

				bUnsavedChanges = true;
			}
		}

		private void ChangeSkillID(int nNumber)
		{
			if (bUserAction)
			{
				int nSkillLevel = ((ComboBox)Controls.Find("cbSkillLevel" + nNumber, true)[0]).SelectedIndex;

				if (nSkillLevel != -1)
					nSkillLevel++;
				else
					nSkillLevel = 1;

				SkillPicker pSkillSelector = new(pMain, this, [pTempNPCRow["a_skill" + nNumber].ToString().Split(' ')[0], nSkillLevel], true);
				if (pSkillSelector.ShowDialog() != DialogResult.OK)
					return;

				int nSkillID = Convert.ToInt32(pSkillSelector.ReturnValues[0]);
				nSkillLevel = Convert.ToInt32(pSkillSelector.ReturnValues[1]);
				string strItemName = nSkillID.ToString();
				Button btnObj = (Button)Controls.Find($"btnSkill{nNumber}ID", true)[0];
				ComboBox cbObj = (ComboBox)Controls.Find("cbSkillLevel" + nNumber, true)[0];
				TextBox tbObj = (TextBox)Controls.Find("tbSkillProb" + nNumber, true)[0];

				if (nSkillID > 0)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("SkillBtn", pSkillSelector.ReturnValues[4].ToString(), Convert.ToInt32(pSkillSelector.ReturnValues[5]), Convert.ToInt32(pSkillSelector.ReturnValues[6])), new Size(24, 24));

					strItemName += " - " + pSkillSelector.ReturnValues[2].ToString();

					cbObj.Items.Clear();
					cbObj.BeginUpdate();

					int i = 1;
					foreach (string strLevel in pSkillSelector.ReturnValues[7].ToString().Split(','))
					{
						cbObj.Items.Add(i);

						if (nSkillLevel == i)
							cbObj.SelectedIndex = cbObj.Items.Count - 1;

						i++;
					}

					if (cbObj.SelectedIndex == -1)
						cbObj.SelectedIndex = 0;

					cbObj.EndUpdate();
					cbObj.Enabled = true;

					tbObj.Enabled = true;
				}
				else
				{
					btnObj.Image = null;

					cbObj.Items.Clear();
					cbObj.Enabled = false;

					tbObj.Text = "0";
					tbObj.Enabled = false;

					((Label)Controls.Find("lSkillProbPercentage" + nNumber, true)[0]).Text = "0%";
				}

				btnObj.Text = strItemName;

				pTempNPCRow["a_skill" + nNumber] = nSkillID + $" {nSkillLevel} " + tbObj.Text;

				bUnsavedChanges = true;
			}
		}

		private void btnSkill0ID_Click(object sender, EventArgs e) { ChangeSkillID(0); }
		private void btnSkill1ID_Click(object sender, EventArgs e) { ChangeSkillID(1); }
		private void btnSkill2ID_Click(object sender, EventArgs e) { ChangeSkillID(2); }
		private void btnSkill3ID_Click(object sender, EventArgs e) { ChangeSkillID(3); }

		private void cbSkillLevel0_SelectedIndexChanged(object sender, EventArgs e) { ChangeSkillLevelAndProb(0); }
		private void cbSkillLevel1_SelectedIndexChanged(object sender, EventArgs e) { ChangeSkillLevelAndProb(1); }
		private void cbSkillLevel2_SelectedIndexChanged(object sender, EventArgs e) { ChangeSkillLevelAndProb(2); }
		private void cbSkillLevel3_SelectedIndexChanged(object sender, EventArgs e) { ChangeSkillLevelAndProb(3); }

		private void tbSkillProb0_TextChanged(object sender, EventArgs e) { ChangeSkillLevelAndProb(0); }
		private void tbSkillProb1_TextChanged(object sender, EventArgs e) { ChangeSkillLevelAndProb(1); }
		private void tbSkillProb2_TextChanged(object sender, EventArgs e) { ChangeSkillLevelAndProb(2); }
		private void tbSkillProb3_TextChanged(object sender, EventArgs e) { ChangeSkillLevelAndProb(3); }
		/****************************************/
		private void cbAIType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbAIType.SelectedIndex;
				if (nType != -1)
				{
					pTempNPCRow["a_aitype"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void btnAIFlag_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.NPCAIFlag, Convert.ToInt64(btnAIFlag.Text.ToString()));
				if (pFlagSelector.ShowDialog() != DialogResult.OK)
					return;

				long lFlag = pFlagSelector.ReturnValues;

				btnAIFlag.Text = lFlag.ToString();

				StringBuilder strTooltip = new();

				for (int i = 0; i < Defs.NPCAIFlag.Length; i++)
				{
					if ((lFlag & 1L << i) != 0)
						strTooltip.Append(Defs.NPCAIFlag[i] + "\n");
				}

				pToolTips[btnAIFlag].SetToolTip(btnAIFlag, strTooltip.ToString());

				pTempNPCRow["a_aiflag"] = lFlag;

				bUnsavedChanges = true;
			}
		}

		private void cbAILeaderFlag_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbAILeaderFlag.SelectedIndex;
				if (nType != -1)
				{
					pTempNPCRow["a_aileader_flag"] = nType;

					bUnsavedChanges = true;
				}
			}
		}

		private void tbAISummonHP_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_ai_summonHp"] = tbAISummonHP.Text;

				bUnsavedChanges = true;
			}
		}

		private void btnAILeaderIDX_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				string strColumnName = "a_aileader_idx";

				NPCPicker pNPCSelector = new(pMain, this, Convert.ToInt32(pTempNPCRow[strColumnName]), true);
				if (pNPCSelector.ShowDialog() != DialogResult.OK)
					return;

				int nNPCID = Convert.ToInt32(pNPCSelector.ReturnValues[0]);
				string strNPCName = nNPCID.ToString();

				if (nNPCID > 0)
					strNPCName += " - " + pNPCSelector.ReturnValues[1].ToString();

				btnAILeaderIDX.Text = strNPCName;

				pTempNPCRow[strColumnName] = nNPCID;

				bUnsavedChanges = true;
			}
		}

		private void tbAILeaderCount_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_aileader_count"] = tbAILeaderCount.Text;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void ChangeProduct(int nNumber)
		{
			if (bUserAction)
			{
				string strItemIDColumn = "a_product" + nNumber;
				Button btnObj = (Button)Controls.Find("btnProduct" + nNumber, true)[0];

				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempNPCRow[strItemIDColumn]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nItemNeededID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				string strItemName = nItemNeededID.ToString();

				if (nItemNeededID > 0)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));

					strItemName += $" - {pItemSelector.ReturnValues[1]}";
				}
				else
				{
					btnObj.Image = null;
				}

				btnObj.Text = strItemName;

				pTempNPCRow[strItemIDColumn] = nItemNeededID.ToString();

				bUnsavedChanges = true;
			}
		}

		private void btnProduct0_Click(object sender, EventArgs e) { ChangeProduct(0); }
		private void btnProduct1_Click(object sender, EventArgs e) { ChangeProduct(1); }
		private void btnProduct2_Click(object sender, EventArgs e) { ChangeProduct(2); }
		private void btnProduct3_Click(object sender, EventArgs e) { ChangeProduct(3); }
		private void btnProduct4_Click(object sender, EventArgs e) { ChangeProduct(4); }
		/****************************************/
		private void tbPlusMin_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_minplus"] = tbPlusMin.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPlusMax_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_maxplus"] = tbPlusMax.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbPlusProb_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_probplus"] = tbPlusProb.Text;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void tbAnimationIdle_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_motion_idle"] = tbAnimationIdle.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAnimationIdle2_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_motion_idle2"] = tbAnimationIdle2.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAnimationWalk_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_motion_walk"] = tbAnimationWalk.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAnimationRun_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_motion_run"] = tbAnimationRun.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAnimationDamage_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_motion_dam"] = tbAnimationDamage.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAnimationAttack_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_motion_attack"] = tbAnimationAttack.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAnimationAttack2_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_motion_attack2"] = tbAnimationAttack2.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbAnimationDie_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_motion_die"] = tbAnimationDie.Text;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void ChangeFireDelay(int nNumber)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_fireDelay" + nNumber] = ((TextBox)Controls.Find("tbFireDelay" + nNumber, true)[0]).Text;

				bUnsavedChanges = true;
			}
		}

		private void tbFireDelay0_TextChanged(object sender, EventArgs e) { ChangeFireDelay(0); }
		private void tbFireDelay1_TextChanged(object sender, EventArgs e) { ChangeFireDelay(1); }
		private void tbFireDelay2_TextChanged(object sender, EventArgs e) { ChangeFireDelay(2); }
		private void tbFireDelay3_TextChanged(object sender, EventArgs e) { ChangeFireDelay(3); }
		/****************************************/
		private void tbFireDelayCount_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_fireDelayCount"] = tbFireDelayCount.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbFireObject_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_fireObject"] = tbFireObject.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbFireSpeed_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_fireSpeed"] = tbFireSpeed.Text;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void ChangeFireEffect(int nNumber)
		{
			if (bUserAction)
			{
				pTempNPCRow["a_fireEffect" + nNumber] = ((TextBox)Controls.Find("tbFireEffect" + nNumber, true)[0]).Text;

				bUnsavedChanges = true;
			}
		}

		private void tbFireEffect0_TextChanged(object sender, EventArgs e) { ChangeFireEffect(0); }
		private void tbFireEffect1_TextChanged(object sender, EventArgs e) { ChangeFireEffect(1); }
		private void tbFireEffect2_TextChanged(object sender, EventArgs e) { ChangeFireEffect(2); }
		/****************************************/
		private void ChangeItemDrop(int nNumber)
		{
			if (bUserAction)
			{
				string strItemIDColumn = "a_item_" + nNumber;
				Button btnObj = (Button)Controls.Find("btnItemDrop" + nNumber, true)[0];
				TextBox tbObj = (TextBox)Controls.Find($"tbItemDrop{nNumber}Prob", true)[0];

				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempNPCRow[strItemIDColumn]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nItemNeededID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				string strItemName = nItemNeededID.ToString();

				if (nItemNeededID > 0)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));

					tbObj.Enabled = true;

					strItemName += $" - {pItemSelector.ReturnValues[1]}";
				}
				else
				{
					btnObj.Image = null;

					tbObj.Text = "0";
					tbObj.Enabled = false;

					((Label)Controls.Find("lItemDropProbPercentage" + nNumber, true)[0]).Text = "0%";
				}

				btnObj.Text = strItemName;

				tbObj.Focus();

				pTempNPCRow[strItemIDColumn] = nItemNeededID.ToString();

				bUnsavedChanges = true;
			}
		}

		private void btnItemDrop0_Click(object sender, EventArgs e) { ChangeItemDrop(0); }
		private void btnItemDrop1_Click(object sender, EventArgs e) { ChangeItemDrop(1); }
		private void btnItemDrop2_Click(object sender, EventArgs e) { ChangeItemDrop(2); }
		private void btnItemDrop3_Click(object sender, EventArgs e) { ChangeItemDrop(3); }
		private void btnItemDrop4_Click(object sender, EventArgs e) { ChangeItemDrop(4); }
		private void btnItemDrop5_Click(object sender, EventArgs e) { ChangeItemDrop(5); }
		private void btnItemDrop6_Click(object sender, EventArgs e) { ChangeItemDrop(6); }
		private void btnItemDrop7_Click(object sender, EventArgs e) { ChangeItemDrop(7); }
		private void btnItemDrop8_Click(object sender, EventArgs e) { ChangeItemDrop(8); }
		private void btnItemDrop9_Click(object sender, EventArgs e) { ChangeItemDrop(9); }
		private void btnItemDrop10_Click(object sender, EventArgs e) { ChangeItemDrop(10); }
		private void btnItemDrop11_Click(object sender, EventArgs e) { ChangeItemDrop(11); }
		private void btnItemDrop12_Click(object sender, EventArgs e) { ChangeItemDrop(12); }
		private void btnItemDrop13_Click(object sender, EventArgs e) { ChangeItemDrop(13); }
		private void btnItemDrop14_Click(object sender, EventArgs e) { ChangeItemDrop(14); }
		private void btnItemDrop15_Click(object sender, EventArgs e) { ChangeItemDrop(15); }
		private void btnItemDrop16_Click(object sender, EventArgs e) { ChangeItemDrop(16); }
		private void btnItemDrop17_Click(object sender, EventArgs e) { ChangeItemDrop(17); }
		private void btnItemDrop18_Click(object sender, EventArgs e) { ChangeItemDrop(18); }
		private void btnItemDrop19_Click(object sender, EventArgs e) { ChangeItemDrop(19); }
		/****************************************/
		private void ChangeItemDropProb(int nNumber)
		{
			if (bUserAction)
			{
				int nItemProb = Convert.ToInt32(((TextBox)Controls.Find($"tbItemDrop{nNumber}Prob", true)[0]).Text);

				((Label)Controls.Find("lItemDropProbPercentage" + nNumber, true)[0]).Text = ((nItemProb * 100.0f) / 10000.0f) + "%";

				pTempNPCRow["a_item_percent_" + nNumber] = nItemProb;

				bUnsavedChanges = true;
			}
		}

		private void tbItemDrop0Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(0); }
		private void tbItemDrop1Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(1); }
		private void tbItemDrop2Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(2); }
		private void tbItemDrop3Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(3); }
		private void tbItemDrop4Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(4); }
		private void tbItemDrop5Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(5); }
		private void tbItemDrop6Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(6); }
		private void tbItemDrop7Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(7); }
		private void tbItemDrop8Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(8); }
		private void tbItemDrop9Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(9); }
		private void tbItemDrop10Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(10); }
		private void tbItemDrop11Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(11); }
		private void tbItemDrop12Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(12); }
		private void tbItemDrop13Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(13); }
		private void tbItemDrop14Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(14); }
		private void tbItemDrop15Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(15); }
		private void tbItemDrop16Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(16); }
		private void tbItemDrop17Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(17); }
		private void tbItemDrop18Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(18); }
		private void tbItemDrop19Prob_TextChanged(object sender, EventArgs e) { ChangeItemDropProb(19); }
		/****************************************/
		private void ChangeJewelDrop(int nNumber)
		{
			if (bUserAction)
			{
				string strItemIDColumn = "a_jewel_" + nNumber;
				Button btnObj = (Button)Controls.Find("btnJewelDrop" + nNumber, true)[0];
				TextBox tbObj = (TextBox)Controls.Find($"tbJewelDrop{nNumber}Prob", true)[0];

				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempNPCRow[strItemIDColumn]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nItemNeededID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				string strItemName = nItemNeededID.ToString();

				if (nItemNeededID > 0)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));

					tbObj.Enabled = true;

					strItemName += $" - {pItemSelector.ReturnValues[1]}";
				}
				else
				{
					btnObj.Image = null;

					tbObj.Text = "0";
					tbObj.Enabled = false;

					((Label)Controls.Find("lJewelDropProbPercentage" + nNumber, true)[0]).Text = "0%";
				}

				btnObj.Text = strItemName;

				tbObj.Focus();

				pTempNPCRow[strItemIDColumn] = nItemNeededID.ToString();

				bUnsavedChanges = true;
			}
		}

		private void btnJewelDrop0_Click(object sender, EventArgs e) { ChangeJewelDrop(0); }
		private void btnJewelDrop1_Click(object sender, EventArgs e) { ChangeJewelDrop(1); }
		private void btnJewelDrop2_Click(object sender, EventArgs e) { ChangeJewelDrop(2); }
		private void btnJewelDrop3_Click(object sender, EventArgs e) { ChangeJewelDrop(3); }
		private void btnJewelDrop4_Click(object sender, EventArgs e) { ChangeJewelDrop(4); }
		private void btnJewelDrop5_Click(object sender, EventArgs e) { ChangeJewelDrop(5); }
		private void btnJewelDrop6_Click(object sender, EventArgs e) { ChangeJewelDrop(6); }
		private void btnJewelDrop7_Click(object sender, EventArgs e) { ChangeJewelDrop(7); }
		private void btnJewelDrop8_Click(object sender, EventArgs e) { ChangeJewelDrop(8); }
		private void btnJewelDrop9_Click(object sender, EventArgs e) { ChangeJewelDrop(9); }
		private void btnJewelDrop10_Click(object sender, EventArgs e) { ChangeJewelDrop(10); }
		private void btnJewelDrop11_Click(object sender, EventArgs e) { ChangeJewelDrop(11); }
		private void btnJewelDrop12_Click(object sender, EventArgs e) { ChangeJewelDrop(12); }
		private void btnJewelDrop13_Click(object sender, EventArgs e) { ChangeJewelDrop(13); }
		private void btnJewelDrop14_Click(object sender, EventArgs e) { ChangeJewelDrop(14); }
		private void btnJewelDrop15_Click(object sender, EventArgs e) { ChangeJewelDrop(15); }
		private void btnJewelDrop16_Click(object sender, EventArgs e) { ChangeJewelDrop(16); }
		private void btnJewelDrop17_Click(object sender, EventArgs e) { ChangeJewelDrop(17); }
		private void btnJewelDrop18_Click(object sender, EventArgs e) { ChangeJewelDrop(18); }
		private void btnJewelDrop19_Click(object sender, EventArgs e) { ChangeJewelDrop(19); }
		/****************************************/
		private void ChangeJewelDropProb(int nNumber)
		{
			if (bUserAction)
			{
				int nItemProb = Convert.ToInt32(((TextBox)Controls.Find($"tbJewelDrop{nNumber}Prob", true)[0]).Text);

				((Label)Controls.Find("lJewelDropProbPercentage" + nNumber, true)[0]).Text = ((nItemProb * 100.0f) / 10000.0f) + "%";

				pTempNPCRow["a_jewel_percent_" + nNumber] = nItemProb;

				bUnsavedChanges = true;
			}
		}

		private void tbJewelDrop0Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(0); }
		private void tbJewelDrop1Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(1); }
		private void tbJewelDrop2Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(2); }
		private void tbJewelDrop3Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(3); }
		private void tbJewelDrop4Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(4); }
		private void tbJewelDrop5Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(5); }
		private void tbJewelDrop6Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(6); }
		private void tbJewelDrop7Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(7); }
		private void tbJewelDrop8Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(8); }
		private void tbJewelDrop9Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(9); }
		private void tbJewelDrop10Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(10); }
		private void tbJewelDrop11Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(11); }
		private void tbJewelDrop12Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(12); }
		private void tbJewelDrop13Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(13); }
		private void tbJewelDrop14Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(14); }
		private void tbJewelDrop15Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(15); }
		private void tbJewelDrop16Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(16); }
		private void tbJewelDrop17Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(17); }
		private void tbJewelDrop18Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(18); }
		private void tbJewelDrop19Prob_TextChanged(object sender, EventArgs e) { ChangeJewelDropProb(19); }
		/****************************************/
		private void tbWorldBossGoldMin_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction && pTempNPCRow?.Table.Columns.Contains("a_reward_gold_min") == true)
			{
				pTempNPCRow["a_reward_gold_min"] = tbWorldBossGoldMin.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbWorldBossGoldMax_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction && pTempNPCRow?.Table.Columns.Contains("a_reward_gold_max") == true)
			{
				pTempNPCRow["a_reward_gold_max"] = tbWorldBossGoldMax.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbWorldBossGoldMultiplier_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction && pTempNPCRow?.Table.Columns.Contains("a_reward_gold_multiplier") == true)
			{
				pTempNPCRow["a_reward_gold_multiplier"] = tbWorldBossGoldMultiplier.Text;

				bUnsavedChanges = true;
			}
		}
		/****************************************/
		private void ChangeDropJobItem(string strClass)
		{
			if (bUserAction)
			{
				MakeTempDropJobRow();

				string strDBClass = strClass == "ArchMage" ? "ExMage" : strClass;
				strDBClass = $"a_{strDBClass}_item".ToLower();

				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempDropJobRow[strDBClass]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				string strItemName = nItemID.ToString();
				Button btnObj = (Button)Controls.Find($"btnDropJob{strClass}ID", true)[0];
				TextBox tbObj = (TextBox)Controls.Find($"tbDropJob{strClass}Prob", true)[0];

				if (nItemID > 0)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));

					tbObj.Enabled = true;

					strItemName += $" - {pItemSelector.ReturnValues[1]}";
				}
				else
				{
					btnObj.Image = null;

					tbObj.Text = "0";
					tbObj.Enabled = false;

					((Label)Controls.Find($"lDropJob{strClass}ProbPercentage", true)[0]).Text = "0%";
				}

				btnObj.Text = strItemName;

				tbObj.Focus();

				pTempDropJobRow[strDBClass] = nItemID;

				bUnsavedChanges = true;
			}
		}

		private void btnDropJobTitanID_Click(object sender, EventArgs e) { ChangeDropJobItem("Titan"); }
		private void btnDropJobKnightID_Click(object sender, EventArgs e) { ChangeDropJobItem("Knight"); }
		private void btnDropJobHealerID_Click(object sender, EventArgs e) { ChangeDropJobItem("Healer"); }
		private void btnDropJobMageID_Click(object sender, EventArgs e) { ChangeDropJobItem("Mage"); }
		private void btnDropJobRogueID_Click(object sender, EventArgs e) { ChangeDropJobItem("Rogue"); }
		private void btnDropJobSorcererID_Click(object sender, EventArgs e) { ChangeDropJobItem("Sorcerer"); }
		private void btnDropJobNightShadowID_Click(object sender, EventArgs e) { ChangeDropJobItem("NightShadow"); }
		private void btnDropJobExRogueID_Click(object sender, EventArgs e) { ChangeDropJobItem("ExRogue"); }
		private void btnDropJobArchMageID_Click(object sender, EventArgs e) { ChangeDropJobItem("ArchMage"); }
		/****************************************/
		private void ChangeDropJobItemProb(string strClass)
		{
			if (bUserAction)
			{
				MakeTempDropJobRow();

				string strDBClass = strClass == "ArchMage" ? "ExMage" : strClass;
				strDBClass = $"a_{strDBClass}_item_prob".ToLower();
				int nItemProb = Convert.ToInt32(((TextBox)Controls.Find($"tbDropJob{strClass}Prob", true)[0]).Text);

				((Label)Controls.Find($"lDropJob{strClass}ProbPercentage", true)[0]).Text = ((nItemProb * 100.0f) / 10000.0f) + "%";

				pTempDropJobRow[strDBClass] = nItemProb;

				bUnsavedChanges = true;
			}
		}

		private void tbDropJobTitanProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("Titan"); }
		private void tbDropJobKnightProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("Knight"); }
		private void tbDropJobHealerProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("Healer"); }
		private void tbDropJobMageProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("Mage"); }
		private void tbDropJobRogueProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("Rogue"); }
		private void tbDropJobSorcererProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("Sorcerer"); }
		private void tbDropJobNightShadowProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("NightShadow"); }
		private void tbDropJobExRogueProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("ExRogue"); }
		private void tbDropJobArchMageProb_TextChanged(object sender, EventArgs e) { ChangeDropJobItemProb("ArchMage"); }
		/****************************************/
		private void ChangeDropAllItem(int nNumber)
		{
			if (bUserAction)
			{
				MakeTempDropAllRow();

				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempDropAllRows[nNumber]["a_item_idx"]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				string strItemName = nItemID.ToString();
				Button btnObj = (Button)Controls.Find("btnDropAll" + nNumber, true)[0];
				TextBox tbObj = (TextBox)Controls.Find($"tbDropAll{nNumber}Prob", true)[0];

				if (nItemID > 0)
				{
					btnObj.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));

					tbObj.Enabled = true;

					strItemName += $" - {pItemSelector.ReturnValues[1]}";
				}
				else
				{
					btnObj.Image = null;

					tbObj.Text = "0";
					tbObj.Enabled = false;

					((Label)Controls.Find("lDropAllProbPercentage" + nNumber, true)[0]).Text = "0%";
				}

				btnObj.Text = strItemName;

				tbObj.Focus();

				pTempDropAllRows[nNumber]["a_item_idx"] = nItemID;

				bUnsavedChanges = true;
			}
		}

		private void btnDropAll0_Click(object sender, EventArgs e) { ChangeDropAllItem(0); }
		private void btnDropAll1_Click(object sender, EventArgs e) { ChangeDropAllItem(1); }
		private void btnDropAll2_Click(object sender, EventArgs e) { ChangeDropAllItem(2); }
		private void btnDropAll3_Click(object sender, EventArgs e) { ChangeDropAllItem(3); }
		private void btnDropAll4_Click(object sender, EventArgs e) { ChangeDropAllItem(4); }
		private void btnDropAll5_Click(object sender, EventArgs e) { ChangeDropAllItem(5); }
		private void btnDropAll6_Click(object sender, EventArgs e) { ChangeDropAllItem(6); }
		private void btnDropAll7_Click(object sender, EventArgs e) { ChangeDropAllItem(7); }
		private void btnDropAll8_Click(object sender, EventArgs e) { ChangeDropAllItem(8); }
		private void btnDropAll9_Click(object sender, EventArgs e) { ChangeDropAllItem(9); }
		private void btnDropAll10_Click(object sender, EventArgs e) { ChangeDropAllItem(10); }
		private void btnDropAll11_Click(object sender, EventArgs e) { ChangeDropAllItem(11); }
		private void btnDropAll12_Click(object sender, EventArgs e) { ChangeDropAllItem(12); }
		private void btnDropAll13_Click(object sender, EventArgs e) { ChangeDropAllItem(13); }
		private void btnDropAll14_Click(object sender, EventArgs e) { ChangeDropAllItem(14); }
		private void btnDropAll15_Click(object sender, EventArgs e) { ChangeDropAllItem(15); }
		private void btnDropAll16_Click(object sender, EventArgs e) { ChangeDropAllItem(16); }
		private void btnDropAll17_Click(object sender, EventArgs e) { ChangeDropAllItem(17); }
		private void btnDropAll18_Click(object sender, EventArgs e) { ChangeDropAllItem(18); }
		private void btnDropAll19_Click(object sender, EventArgs e) { ChangeDropAllItem(19); }
		/****************************************/
		private void ChangeDropAllProb(int nNumber)
		{
			if (bUserAction)
			{
				TextBox tbObj = (TextBox)Controls.Find($"tbDropAll{nNumber}Prob", true)[0];
				int nItemProb = Convert.ToInt32(tbObj.Text);

				((Label)Controls.Find("lDropAllProbPercentage" + nNumber, true)[0]).Text = ((nItemProb * 100.0f) / 10000.0f) + "%";

				pTempDropAllRows[nNumber]["a_prob"] = nItemProb;

				bUnsavedChanges = true;
			}
		}

		private void tbDropAll0Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(0); }
		private void tbDropAll1Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(1); }
		private void tbDropAll2Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(2); }
		private void tbDropAll3Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(3); }
		private void tbDropAll4Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(4); }
		private void tbDropAll5Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(5); }
		private void tbDropAll6Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(6); }
		private void tbDropAll7Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(7); }
		private void tbDropAll8Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(8); }
		private void tbDropAll9Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(9); }
		private void tbDropAll10Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(10); }
		private void tbDropAll11Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(11); }
		private void tbDropAll12Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(12); }
		private void tbDropAll13Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(13); }
		private void tbDropAll14Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(14); }
		private void tbDropAll15Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(15); }
		private void tbDropAll16Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(16); }
		private void tbDropAll17Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(17); }
		private void tbDropAll18Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(18); }
		private void tbDropAll19Prob_TextChanged(object sender, EventArgs e) { ChangeDropAllProb(19); }
		/****************************************/
		private void gridDropRaid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
		{
			if (bUserAction)
			{
				if (e.ColumnIndex == 2) // Item Count
				{
					if (Convert.ToInt32(gridDropRaid.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
					{
						gridDropRaid.Rows[e.RowIndex].Cells["count"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridDropRaid.Rows[e.RowIndex].Cells["count"].Value));
						gridDropRaid.Rows[e.RowIndex].Cells["count"].Style.BackColor = Color.FromArgb(166, 166, 166);
					}
				}
				else if (e.ColumnIndex == 3 || e.ColumnIndex == 34) // Item Prob Or Special Box Reward Prob
				{
					string strCellName = e.ColumnIndex == 3 ? "prob" : "specprob";

					float fMax = e.ColumnIndex == 3 ? 10000.0f : 1000.0f;

					gridDropRaid.Rows[e.RowIndex].Cells[strCellName].ToolTipText = ((Convert.ToInt32(gridDropRaid.Rows[e.RowIndex].Cells[strCellName].Value) * 100.0f) / fMax) + "%";
				}

				bUnsavedChanges = true;
			}
		}

		private void gridDropRaid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (bUserAction)
			{
				if (e.Button == MouseButtons.Left && e.RowIndex >= 0 && (e.ColumnIndex == 1 ||  // Item ID
																		e.ColumnIndex == 6 ||   // Special Box Reward 1 ID
																		e.ColumnIndex == 8 ||
																		e.ColumnIndex == 10 ||
																		e.ColumnIndex == 12 ||
																		e.ColumnIndex == 14 ||
																		e.ColumnIndex == 16 ||
																		e.ColumnIndex == 18 ||
																		e.ColumnIndex == 20 ||
																		e.ColumnIndex == 22 ||
																		e.ColumnIndex == 24 ||
																		e.ColumnIndex == 26 ||
																		e.ColumnIndex == 28 ||
																		e.ColumnIndex == 30 ||
																		e.ColumnIndex == 32)
				) // Item Selector
				{
					string strCellIconName = e.ColumnIndex == 1 ? "itemicon" : $"specitem{((e.ColumnIndex - 6) / 2 + 1)}icon";
					string strCellItemID = e.ColumnIndex == 1 ? "item" : "specitem" + ((e.ColumnIndex - 6) / 2 + 1);
					int nItemID = Convert.ToInt32(gridDropRaid.Rows[e.RowIndex].Cells[strCellItemID].Tag);

					ItemPicker pItemSelector = new(pMain, this, nItemID, false);
					if (pItemSelector.ShowDialog() != DialogResult.OK)
						return;

					nItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
					if (nItemID > 0)
					{
						if (nItemID == Defs.NAS_ITEM_DB_INDEX)
						{
							gridDropRaid.Rows[e.RowIndex].Cells["count"].Style.ForeColor = pMain.GetGoldColor(Convert.ToInt64(gridDropRaid.Rows[e.RowIndex].Cells["count"].Value));
							gridDropRaid.Rows[e.RowIndex].Cells["count"].Style.BackColor = Color.FromArgb(166, 166, 166);
						}
						else
						{
							gridDropRaid.Rows[e.RowIndex].Cells["count"].Style.ForeColor = Color.FromArgb(208, 203, 148);
							gridDropRaid.Rows[e.RowIndex].Cells["count"].Style.BackColor = Color.FromArgb(40, 40, 40);
						}

						gridDropRaid.Rows[e.RowIndex].Cells[strCellIconName].Value = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
						gridDropRaid.Rows[e.RowIndex].Cells[strCellItemID].Value = $"{nItemID} - {pItemSelector.ReturnValues[1]}";
						gridDropRaid.Rows[e.RowIndex].Cells[strCellItemID].Tag = nItemID;

						bUnsavedChanges = true;
					}
				}
				else if (e.Button == MouseButtons.Left && e.ColumnIndex == 4 && e.RowIndex >= 0) // Flag Selector
				{
					if (Convert.ToInt32(gridDropRaid.Rows[e.RowIndex].Cells["item"].Tag) == Defs.NAS_ITEM_DB_INDEX)
						return;

					FlagPicker pFlagSelector = new(this, Defs.ItemFlags, Convert.ToInt64(gridDropRaid.Rows[e.RowIndex].Cells["flag"].Value));
					if (pFlagSelector.ShowDialog() != DialogResult.OK)
						return;

					gridDropRaid.Rows[e.RowIndex].Cells["flag"].Value = pFlagSelector.ReturnValues;

					StringBuilder strTooltip = new();

					for (int i = 0; i < Defs.ItemFlags.Length; i++)
					{
						if ((pFlagSelector.ReturnValues & 1L << i) != 0)
							strTooltip.Append(Defs.ItemFlags[i] + "\n");
					}

					gridDropRaid.Rows[e.RowIndex].Cells["flag"].ToolTipText = strTooltip.ToString();

					bUnsavedChanges = true;
				}
				else if (e.Button == MouseButtons.Right && e.ColumnIndex == -1) // Header Column
				{
					ToolStripMenuItem addItem = new("Add New");
					addItem.Click += (_, _) =>
					{
						bool bSuccess = true;
						int nDefaultItemID = -1;
						int nDefaultItemCount = 0;
						int nDefaultItemProb = 0;
						long lDefaultItemFlag = 0;
						int[] nSpecialBoxRewardIDS = {
							-1, -1, -1, -1, -1, -1, -1,
							-1, -1, -1, -1, -1, -1, -1
						};
						int nSpecialBoxRewardsCount = 1;
						int nSpecialBoxRewardProb = 0;

						try
						{
							if (pTempDropRaidRows == null)
								pTempDropRaidRows = new DataRow[1];

							int nPosition = pTempDropRaidRows.Length - 1;

							pTempDropRaidRows[nPosition] = pMain.pTables.NPCDropRaidTable.NewRow();

							pTempDropRaidRows[nPosition]["a_npc_index"] = pTempNPCRow["a_index"];
							pTempDropRaidRows[nPosition]["a_item_index"] = nDefaultItemID;
							pTempDropRaidRows[nPosition]["a_count"] = nDefaultItemCount;
							pTempDropRaidRows[nPosition]["a_prob"] = nDefaultItemProb;
							pTempDropRaidRows[nPosition]["a_flag"] = lDefaultItemFlag;

							for (int i = 1; i <= 14; i++)
								pTempDropRaidRows[nPosition]["a_spec_item_index" + i] = nSpecialBoxRewardIDS[i - 1];

							pTempDropRaidRows[nPosition]["a_spec_count"] = nSpecialBoxRewardsCount;
							pTempDropRaidRows[nPosition]["a_spec_prob"] = nSpecialBoxRewardProb;
						}
						catch (Exception ex)
						{
							pMain.Logger(LogTypes.Error, $"NPC Editor > {ex.Message}.");

							bSuccess = false;
						}
						finally
						{
							if (bSuccess)
							{
								int nRow = gridDropRaid.Rows.Count;
								Bitmap pBitmapItemIcon = new(1, 1);

								gridDropRaid.Rows.Insert(nRow);

								gridDropRaid.Rows[nRow].HeaderCell.Value = (nRow + 1).ToString();

								gridDropRaid.Rows[nRow].Cells["itemIcon"].Value = pBitmapItemIcon;
								gridDropRaid.Rows[nRow].Cells["item"].Value = nDefaultItemID;
								gridDropRaid.Rows[nRow].Cells["item"].Tag = nDefaultItemID;
								gridDropRaid.Rows[nRow].Cells["count"].Value = nDefaultItemCount;
								gridDropRaid.Rows[nRow].Cells["prob"].Value = nDefaultItemProb;
								gridDropRaid.Rows[nRow].Cells["prob"].ToolTipText = ((nDefaultItemProb * 100.0f) / 10000.0f) + "%";
								gridDropRaid.Rows[nRow].Cells["flag"].Value = lDefaultItemFlag;

								for (int j = 1; j <= 14; j++)
								{
									gridDropRaid.Rows[nRow].Cells[$"specitem{j}icon"].Value = pBitmapItemIcon;
									gridDropRaid.Rows[nRow].Cells["specitem" + j].Value = nSpecialBoxRewardIDS[j - 1];
									gridDropRaid.Rows[nRow].Cells["specitem" + j].Tag = nSpecialBoxRewardIDS[j - 1];
								}

								gridDropRaid.Rows[nRow].Cells["speccount"].Value = nSpecialBoxRewardsCount;
								gridDropRaid.Rows[nRow].Cells["specprob"].Value = nSpecialBoxRewardProb;

								gridDropRaid.FirstDisplayedScrollingRowIndex = nRow;
								gridDropRaid.Rows[nRow].Selected = true;

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
								DataRow pDropRaidLastItemRow = pTempDropRaidRows.Cast<DataRow>().Where(row => row["a_item_index"].ToString() == gridDropRaid.Rows[e.RowIndex].Cells["item"].Tag.ToString()).LastOrDefault();
								if (pDropRaidLastItemRow != null)
									pTempDropRaidRows.ElementAt(Array.IndexOf(pTempDropRaidRows, pDropRaidLastItemRow)).Delete();
							}
							catch (Exception ex)
							{
								pMain.Logger(LogTypes.Error, $"NPC Editor > {ex.Message}.");

								bSuccess = false;
							}
							finally
							{
								if (bSuccess)
								{
									gridDropRaid.SuspendLayout();

									gridDropRaid.Rows.RemoveAt(e.RowIndex);

									int i = 1;
									foreach (DataGridViewRow pRow in gridDropRaid.Rows)
									{
										pRow.HeaderCell.Value = i.ToString();

										i++;
									}

									gridDropRaid.ResumeLayout();

									bUnsavedChanges = true;
								}
							}
						}
					};

					cmDropRaid = new ContextMenuStrip();
					cmDropRaid.Items.AddRange(addItem, deleteItem);
					cmDropRaid.Show(Cursor.Position);
				}
			}
		}
		/****************************************/
		private void UpdateTempRegenRows()
		{
			int nItemID = lbRegenZones.SelectedIndex;
			if (nItemID >= 0)
			{
				Main.ListBoxItem pItem = (Main.ListBoxItem)lbRegenZones.Items[nItemID];
				int nGridRows = gridRegenSpots.Rows.Count;
				
				// Remove all Regen spots from current Zone
				var pNewTempRegenRows = pTempRegenRows.Where(row => Convert.ToInt32(row["a_zone_num"]) != pItem.ID).Select(row => { DataRow newRow = pMain.pTables.NPCRegenTable.NewRow(); newRow.ItemArray = (object[])row.ItemArray.Clone(); return newRow; }).ToList();

				if (nGridRows >= 0)
				{
					foreach (DataGridViewRow pGridRow in gridRegenSpots.Rows)
					{
						DataRow pRow = pMain.pTables.NPCRegenTable.NewRow();

						pRow["a_npc_idx"] = pTempNPCRow["a_index"];
						pRow["a_zone_num"] = pItem.ID;
						pRow["a_pos_x"] = pGridRow.Cells["posX"].Value;
						pRow["a_pos_z"] = pGridRow.Cells["posZ"].Value;
#if USE_A_POS_H_COLUMN_IN_NPC_REGEN
						pRow["a_pos_h"] = pGridRow.Cells["posH"].Value;
#endif
						pRow["a_pos_r"] = pGridRow.Cells["posR"].Value;
						pRow["a_y_layer"] = pGridRow.Cells["yLayer"].Value;
						pRow["a_regen_sec"] = pGridRow.Cells["regenSec"].Value;
						pRow["a_total_num"] = pGridRow.Cells["totalNum"].Value;

						pNewTempRegenRows.Add(pRow);
					}

					pTempRegenRows = pNewTempRegenRows.ToArray();
				}

				pItem.Text = $"{pItem.ID} - {pMain.pTables.ZoneTable.Rows[pItem.ID]["a_name"]} {(nGridRows > 0 ? "\t*" : "")}";
				
				lbRegenZones.SelectedIndexChanged -= lbRegenZones_SelectedIndexChanged;
				lbRegenZones.Items[nItemID] = pItem;
				lbRegenZones.SelectedIndexChanged += lbRegenZones_SelectedIndexChanged;
			}

			bUnsavedChanges = true;
		}

		private void cbWorldRatioSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nType = cbWorldRatioSelector.SelectedIndex;
			if (nType != -1)
			{
				fWorldRatio = Defs.WorldRatio * (nType + 1);    // WARNING NOTE: This scales linearly based on Defs.WorldRatio and the selected item in cbWorldRatioSelector.

				pbWorldMap.Invalidate();
			}
		}
#if USE_A_POS_H_COLUMN_IN_NPC_REGEN
		private void AddRegenSpots(int nIndex, float fX, float fZ, float fH, float fR, int nYLayer, int nRegenSec, int nTotalNum)
#else
		private void AddRegenSpots(int nIndex, float fX, float fZ, float fR, int nYLayer, int nRegenSec, int nTotalNum)
#endif
		{
			gridRegenSpots.Rows.Insert(nIndex);

			gridRegenSpots.Rows[nIndex].HeaderCell.Value = (nIndex + 1).ToString();

			gridRegenSpots.Rows[nIndex].Cells["posX"].Value = fX;
			gridRegenSpots.Rows[nIndex].Cells["posZ"].Value = fZ;
#if USE_A_POS_H_COLUMN_IN_NPC_REGEN
			gridRegenSpots.Rows[nIndex].Cells["posH"].Value = fH;
#endif
			gridRegenSpots.Rows[nIndex].Cells["posR"].Value = fR;
			gridRegenSpots.Rows[nIndex].Cells["yLayer"].Value = nYLayer;

			gridRegenSpots.Rows[nIndex].Cells["regenSec"].Value = nRegenSec;
			
			if (nRegenSec > 0)
				gridRegenSpots.Rows[nIndex].Cells["regenSec"].ToolTipText = $"Real Time Secs: {nRegenSec / 10}";

			gridRegenSpots.Rows[nIndex].Cells["totalNum"].Value = nTotalNum;
		}

		private void lbRegenZones_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (pTempRegenRows != null)
			{
				// Reset
				fTemporalZoom = 1f;
				fPanX = 0f;
				fPanY = 0f;
				bZoomIn = false;

				if (lbRegenZones.SelectedItem is not Main.ListBoxItem nSelectedZone)
					return;

				var (pImage, pWorldRatio) = pMain.GetWorldMap(nSelectedZone.ID + "0");

				// Hardcode!
				if (pWorldRatio == Defs.WorldRatio)
					cbWorldRatioSelector.SelectedIndex = 0;
				else if (pWorldRatio == Defs.WorldRatio * 2)
					cbWorldRatioSelector.SelectedIndex = 1;

				pbWorldMap.Image = pImage;

				// Re-draw after reseting
				pbWorldMap.Invalidate();

				bUserAction = false;

				gridRegenSpots.Rows.Clear();
				gridRegenSpots.SuspendLayout();

				foreach (DataRow pRow in pTempRegenRows)
				{
					if (Convert.ToInt32(pRow["a_zone_num"]) != nSelectedZone.ID)
						continue;
#if USE_A_POS_H_COLUMN_IN_NPC_REGEN
					AddRegenSpots(gridRegenSpots.Rows.Count, float.Parse(pRow["a_pos_x"].ToString()), float.Parse(pRow["a_pos_z"].ToString()), float.Parse(pRow["a_pos_h"].ToString()), float.Parse(pRow["a_pos_r"].ToString()), Convert.ToInt32(pRow["a_y_layer"]), Convert.ToInt32(pRow["a_regen_sec"]), Convert.ToInt32(pRow["a_total_num"]));
#else
					AddRegenSpots(gridRegenSpots.Rows.Count, float.Parse(pRow["a_pos_x"].ToString()), float.Parse(pRow["a_pos_z"].ToString()), float.Parse(pRow["a_pos_r"].ToString()), Convert.ToInt32(pRow["a_y_layer"]), Convert.ToInt32(pRow["a_regen_sec"]), Convert.ToInt32(pRow["a_total_num"]));
#endif
				}

				gridRegenSpots.ResumeLayout();

				bUserAction = true;
			}
		}

		private void pbWorldMap_MouseWheel(object? sender, MouseEventArgs e)
		{
			// Prevent move Panel scrollbar
			if (e is HandledMouseEventArgs pHandledMouseEventArgs)
				pHandledMouseEventArgs.Handled = true;

			float fOldZoom = fTemporalZoom;
			fTemporalZoom *= (e.Delta < 0) ? 1.25f : (1f / 1.25f);
			fTemporalZoom = Math.Clamp(fTemporalZoom, 1f, 10f);

			if (fTemporalZoom == 1f)
			{
				fPanX = 0f;
				fPanY = 0f;
				bZoomIn = false;

				pbWorldMap.Invalidate();

				return;
			}

			PointF pScreen = e.Location;
			PointF pWorld = new((pScreen.X - fPanX) / fOldZoom, (pScreen.Y - fPanY) / fOldZoom);
			float fScaledMapWidth = pbWorldMap.Width * fTemporalZoom;
			float fScaledMapHeight = pbWorldMap.Height * fTemporalZoom;
			float fMinPanX, fMaxPanX, fMinPanY, fMaxPanY;

			if (fScaledMapWidth > pbWorldMap.Width)
			{
				fMaxPanX = 0;
				fMinPanX = pbWorldMap.Width - fScaledMapWidth;
			}
			else
			{
				fMinPanX = fMaxPanX = (pbWorldMap.Width - fScaledMapWidth) / 2;
			}

			if (fScaledMapHeight > pbWorldMap.Height)
			{
				fMaxPanY = 0;
				fMinPanY = pbWorldMap.Height - fScaledMapHeight;
			}
			else
			{
				fMinPanY = fMaxPanY = (pbWorldMap.Height - fScaledMapHeight) / 2;
			}

			fPanX = Math.Clamp(pScreen.X - pWorld.X * fTemporalZoom, fMinPanX, fMaxPanX);
			fPanY = Math.Clamp(pScreen.Y - pWorld.Y * fTemporalZoom, fMinPanY, fMaxPanY);

			bZoomIn = true;

			pbWorldMap.Invalidate();
		}

		private void pbWorldMap_Paint(object sender, PaintEventArgs e)
		{
			if (bUserAction)
			{
				float fX, fZ;

				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // NOTE: Needed for arrows, cuz otherwise them looks like shit

				// Clear the Background
				e.Graphics.Clear(Color.Black);

				// Pan the World
				e.Graphics.TranslateTransform(fPanX, fPanY);
				e.Graphics.ScaleTransform(fTemporalZoom, fTemporalZoom);

				// Draw the World
				e.Graphics.DrawImage(pbWorldMap.Image, 0, 0);

				// Draw Selecting Area
				if (bDelimitingArea)
					e.Graphics.DrawRectangle(new(Color.Fuchsia, 1f / fTemporalZoom), pSelectingArea.X, pSelectingArea.Y, pSelectingArea.Width, pSelectingArea.Height);

				// Draw the Spawn Spots
				if (gridRegenSpots.Rows.Count > 0)
				{
					foreach (DataGridViewRow pRow in gridRegenSpots.Rows)
					{
						fX = float.Parse(pRow.Cells["posX"].Value.ToString()) / (Defs.m_fZoomDetail / fWorldRatio);
						fZ = float.Parse(pRow.Cells["posZ"].Value.ToString()) / (Defs.m_fZoomDetail / fWorldRatio);

						if (fWorldRatio > 0.3333f)  // Hardcode!
						{
							fX /= 4;
							fZ /= 4;
						}

						float fMarkSize = fConstMarkSize / fTemporalZoom;
						Brush pBrush = pRow.Selected ? Brushes.Fuchsia : Brushes.Blue;
						PointF[] pArrow = {
							new(0, -fMarkSize),
							new(-fMarkSize / 2, fMarkSize / 2),
							new(fMarkSize / 2, fMarkSize / 2)
						};

						using (Matrix pMatrix = new())
						{
							pMatrix.Rotate(-float.Parse(pRow.Cells["posR"].Value.ToString()));
							pMatrix.Translate(fX, fZ, MatrixOrder.Append);
							pMatrix.TransformPoints(pArrow);
						}

						//e.Graphics.FillEllipse(pBrush, fX - fMarkSize / 2, fZ - fMarkSize / 2, fMarkSize, fMarkSize);
						e.Graphics.FillPolygon(pBrush, pArrow);
					}
				}

				// Draw Coordinates next to Cursor
				if (bDrawCoords)
				{
					using (Font pFont = new("Arial", 10))
					{
						string strCoords;

						if (bDelimitingArea)
						{
							float fLeft = pSelectingArea.X * (Defs.m_fZoomDetail / fWorldRatio);
							float fTop = pSelectingArea.Y * (Defs.m_fZoomDetail / fWorldRatio);
							float fRight = fLeft + (pSelectingArea.Right - pSelectingArea.Left) * (Defs.m_fZoomDetail / fWorldRatio);
							float fBottom = fTop + (pSelectingArea.Bottom - pSelectingArea.Top) * (Defs.m_fZoomDetail / fWorldRatio);

							if (fWorldRatio > 0.3333f)  // Hardcode!
							{
								fLeft *= 4;
								fTop *= 4;
								fRight *= 4;
								fBottom *= 4;
							}

							strCoords = $"Left: {(int)fLeft} Top: {(int)fTop} Right: {(int)fRight} Bottom: {(int)fBottom}";
						}
						else
						{
							fX = ((pCurrentCursorPosition.X - fPanX) / fTemporalZoom) * (Defs.m_fZoomDetail / fWorldRatio);
							fZ = ((pCurrentCursorPosition.Y - fPanY) / fTemporalZoom) * (Defs.m_fZoomDetail / fWorldRatio);

							if (fWorldRatio > 0.3333f)  // Hardcode!
							{
								fX *= 4;
								fZ *= 4;
							}

							strCoords = $"X: {(int)fX} Z: {(int)fZ}";
						}

						SizeF pTextSize = e.Graphics.MeasureString(strCoords, pFont);
						PointF pCoordsTextPos = new(pCurrentCursorPosition.X, pCurrentCursorPosition.Y);

						if (pCoordsTextPos.X + pTextSize.Width > pbWorldMap.Width)
							pCoordsTextPos.X = pCurrentCursorPosition.X - pTextSize.Width;  // Move Text Left

						if (pCoordsTextPos.Y + pTextSize.Height > pbWorldMap.Height)
							pCoordsTextPos.Y = pCurrentCursorPosition.Y - pTextSize.Height; // Move Text Up

						e.Graphics.ResetTransform();    // Reset Font
						e.Graphics.DrawString(strCoords, pFont, Brushes.White, pCoordsTextPos);
					}
				}
			}
		}

		private PointF GetCorrectedMapPosition(PointF pPosition) { return new PointF((pPosition.X - fPanX) / fTemporalZoom, (pPosition.Y - fPanY) / fTemporalZoom); }

		private PointF GetCorrectedCursorPosition(MouseEventArgs e) { return GetCorrectedMapPosition(e.Location); }

		private PointF ConvertMapPositionToRegenPosition(PointF pMapPosition)
		{
			float fX = (float)Math.Round(pMapPosition.X * (Defs.m_fZoomDetail / fWorldRatio), 2);
			float fZ = (float)Math.Round(pMapPosition.Y * (Defs.m_fZoomDetail / fWorldRatio), 2);

			if (fWorldRatio > 0.3333f)  // Hardcode!
			{
				fX *= 4;
				fZ *= 4;
			}

			return new PointF(fX, fZ);
		}

		private void AddRegenSpotAtMapPosition(PointF pMapPosition)
		{
			if (pTempRegenRows == null || lbRegenZones.SelectedItem is not Main.ListBoxItem)
			{
				MessageBox.Show("Select an NPC and a zone before adding a Regen Spot.", "NPC Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (fWorldRatio <= 0f)
			{
				MessageBox.Show("Select a valid World Ratio before adding a Regen Spot.", "NPC Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			PointF pRegenPosition = ConvertMapPositionToRegenPosition(pMapPosition);
			int nIndex = gridRegenSpots.Rows.Count;

			gridRegenSpots.SuspendLayout();
			bUserAction = false;

#if USE_A_POS_H_COLUMN_IN_NPC_REGEN
			AddRegenSpots(nIndex, pRegenPosition.X, pRegenPosition.Y, 0, 0, 0, 300, -1);
#else
			AddRegenSpots(nIndex, pRegenPosition.X, pRegenPosition.Y, 0, 0, 300, -1);
#endif

			gridRegenSpots.ClearSelection();
			gridRegenSpots.Rows[nIndex].Selected = true;

			gridRegenSpots.ResumeLayout();
			pbWorldMap.Invalidate();
			bUserAction = true;

			UpdateTempRegenRows();
		}

		private void DeleteSelectedRegenSpots()
		{
			if (gridRegenSpots.SelectedRows.Count == 0)
				return;

			DialogResult pDialogReturn = MessageBox.Show("Delete selected Regen Spot(s)?", "NPC Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (pDialogReturn != DialogResult.Yes)
				return;

			gridRegenSpots.SuspendLayout();

			foreach (DataGridViewRow pRow in gridRegenSpots.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(row => row.Index))
				gridRegenSpots.Rows.RemoveAt(pRow.Index);

			int i = 1;
			foreach (DataGridViewRow pRow in gridRegenSpots.Rows)
			{
				pRow.HeaderCell.Value = i.ToString();

				i++;
			}

			gridRegenSpots.ResumeLayout();

			bDelimitingArea = false;

			pbWorldMap.Invalidate();

			UpdateTempRegenRows();
		}

		private void ShowRegenContextMenu(Point pScreenPosition, PointF? pMapPosition)
		{
			cmRegenSpots ??= new ContextMenuStrip();
			cmRegenSpots.Items.Clear();

			ToolStripMenuItem addItem = new("Add Regen Spot Here");
			addItem.Enabled = pMapPosition.HasValue && lbRegenZones.SelectedItem is Main.ListBoxItem && fWorldRatio > 0f;
			addItem.Click += (_, _) => {
				if (pMapPosition.HasValue)
					AddRegenSpotAtMapPosition(pMapPosition.Value);
			};

			ToolStripMenuItem deleteItem = new("Delete Selected Regen Spot(s)");
			deleteItem.Enabled = gridRegenSpots.SelectedRows.Count > 0;
			deleteItem.Click += (_, _) => DeleteSelectedRegenSpots();

			cmRegenSpots.Items.AddRange(new ToolStripItem[] { addItem, deleteItem });
			cmRegenSpots.Show(pScreenPosition);
		}

		private void gridRegenSpots_MouseDown(object? sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
				return;

			DataGridView.HitTestInfo pHit = gridRegenSpots.HitTest(e.X, e.Y);
			if (pHit.RowIndex >= 0)
			{
				if (!gridRegenSpots.Rows[pHit.RowIndex].Selected)
				{
					gridRegenSpots.ClearSelection();
					gridRegenSpots.Rows[pHit.RowIndex].Selected = true;
				}

				int nColumnIndex = pHit.ColumnIndex >= 0 ? pHit.ColumnIndex : 0;
				gridRegenSpots.CurrentCell = gridRegenSpots.Rows[pHit.RowIndex].Cells[nColumnIndex];
			}

			ShowRegenContextMenu(gridRegenSpots.PointToScreen(e.Location), null);
		}

		private void pbWorldMap_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				pbWorldMap.Focus();
				bDelimitingArea = false;
				ShowRegenContextMenu(pbWorldMap.PointToScreen(e.Location), GetCorrectedCursorPosition(e));
				return;
			}

			if (e.Button == MouseButtons.Left)
			{
				pbWorldMap.Focus();

				pSelectingStartPoint = GetCorrectedCursorPosition(e);
				pSelectingArea = new RectangleF(pSelectingStartPoint, new Size(0, 0));

				// Hit test
				bool bHit = false;

				foreach (DataGridViewRow pRow in gridRegenSpots.Rows)
				{
					float fX = float.Parse(pRow.Cells["posX"].Value.ToString()) / (Defs.m_fZoomDetail / fWorldRatio);
					float fZ = float.Parse(pRow.Cells["posZ"].Value.ToString()) / (Defs.m_fZoomDetail / fWorldRatio);

					if (fWorldRatio > 0.3333f)
					{
						fX /= 4;
						fZ /= 4;
					}

					fX = pSelectingStartPoint.X - fX;
					fZ = pSelectingStartPoint.Y - fZ;

					float fHitRadius = (fConstMarkSize / fTemporalZoom) * 1.2f;
					float fHitRadiusSquared = fHitRadius * fHitRadius;

					if ((fX * fX + fZ * fZ) <= fHitRadiusSquared)
					{
						gridRegenSpots.ClearSelection();
						pRow.Selected = true;

						bHit = true;

						break;
					}
				}

				// Doesn't hit any existing mark. can start selection.
				if (!bHit)
				{
					gridRegenSpots.ClearSelection();

					bDelimitingArea = true;
				}
			}
		}

		private void pbWorldMap_MouseLeave(object sender, EventArgs e)
		{
			bDrawCoords = false;    // Disable coordinates drawing when Cursor leaves pbWorldMap

			pbWorldMap.Invalidate();
		}

		private void pbWorldMap_MouseMove(object sender, MouseEventArgs e)
		{
			bDrawCoords = true;
			pCurrentCursorPosition = e.Location;

			if (bZoomIn)
			{
				const int nCursorMargin = 100;
				const float fPanSpeed = 6f;
				float fDeltaX = 0, fDeltaY = 0;

				if (e.X < nCursorMargin)
					fDeltaX = (nCursorMargin - e.X) * fPanSpeed / nCursorMargin;
				else if (e.X > pbWorldMap.Width - nCursorMargin)
					fDeltaX = -((e.X - (pbWorldMap.Width - nCursorMargin)) * fPanSpeed / nCursorMargin);

				if (e.Y < nCursorMargin)
					fDeltaY = (nCursorMargin - e.Y) * fPanSpeed / nCursorMargin;
				else if (e.Y > pbWorldMap.Height - nCursorMargin)
					fDeltaY = -((e.Y - (pbWorldMap.Height - nCursorMargin)) * fPanSpeed / nCursorMargin);

				if (fDeltaX != 0 || fDeltaY != 0)
				{
					fPanX += fDeltaX;
					fPanY += fDeltaY;

					float fScaledMapWidth = pbWorldMap.Width * fTemporalZoom;
					float fScaledMapHeight = pbWorldMap.Height * fTemporalZoom;
					float fMinPanX, fMaxPanX, fMinPanY, fMaxPanY;

					if (fScaledMapWidth > pbWorldMap.Width)
					{
						fMaxPanX = 0;
						fMinPanX = pbWorldMap.Width - fScaledMapWidth;
					}
					else
					{
						fMinPanX = fMaxPanX = (pbWorldMap.Width - fScaledMapWidth) / 2;
					}

					if (fScaledMapHeight > pbWorldMap.Height)
					{
						fMaxPanY = 0;
						fMinPanY = pbWorldMap.Height - fScaledMapHeight;
					}
					else
					{
						fMinPanY = fMaxPanY = (pbWorldMap.Height - fScaledMapHeight) / 2;
					}

					fPanX = Math.Clamp(fPanX, fMinPanX, fMaxPanX);
					fPanY = Math.Clamp(fPanY, fMinPanY, fMaxPanY);
				}
			}

			if (bDelimitingArea)
			{
				PointF pCurrentWorldPoint = GetCorrectedCursorPosition(e);

				pSelectingArea = new RectangleF(
					Math.Min(pSelectingStartPoint.X, pCurrentWorldPoint.X),   // Left
					Math.Min(pSelectingStartPoint.Y, pCurrentWorldPoint.Y),   // Top
					Math.Abs(pCurrentWorldPoint.X - pSelectingStartPoint.X),  // Right
					Math.Abs(pCurrentWorldPoint.Y - pSelectingStartPoint.Y)   // Bottom
				);

				if (gridRegenSpots.Rows.Count > 0)
				{
					gridRegenSpots.SuspendLayout();

					foreach (DataGridViewRow pRow in gridRegenSpots.Rows)
					{
						float fX = float.Parse(pRow.Cells["posX"].Value.ToString()) / (Defs.m_fZoomDetail / fWorldRatio);
						float fZ = float.Parse(pRow.Cells["posZ"].Value.ToString()) / (Defs.m_fZoomDetail / fWorldRatio);

						if (fWorldRatio > 0.3333f)  // Hardcode!
						{
							fX /= 4;
							fZ /= 4;
						}

						if (pSelectingArea.Contains(fX, fZ))
							pRow.Selected = true;
						else
							pRow.Selected = false;
					}

					gridRegenSpots.ResumeLayout();
				}

				// Prevent get out of pbWorldMap while is Selecting
				Point pCursorPos = Cursor.Position;
				Point pScreenPos = pbWorldMap.PointToScreen(Point.Empty);

				Cursor.Position = new Point(Math.Clamp(pCursorPos.X, pScreenPos.X, pScreenPos.X + pbWorldMap.Width), Math.Clamp(pCursorPos.Y, pScreenPos.Y, pScreenPos.Y + pbWorldMap.Height));
			}

			pbWorldMap.Invalidate();
		}

		private void pbWorldMap_MouseUp(object sender, MouseEventArgs e) { if (bDelimitingArea) bDelimitingArea = false; }

		private void NPCEditor_KeyDown(object sender, KeyEventArgs e)
		{
			if (pbWorldMap.Focused || gridRegenSpots.Focused)
			{
				if (bDelimitingArea)
				{
					if (e.KeyCode == Keys.P)
					{
						MessageBox_Input pInput = new(this, $"Please enter the number of Regen Spots to add:", "3"); // Hardcode!
						if (pInput.ShowDialog() != DialogResult.OK)
							return;

						Random pRandom = new(); // NOTE: This shit gonna make collisions, but who cares.

						gridRegenSpots.SuspendLayout();

						bUserAction = false;

						for (int i = 0; i < Convert.ToInt32(pInput.strOutput); i++)
						{
							float fX = (float)Math.Round((pSelectingArea.Left + (float)pRandom.NextDouble() * pSelectingArea.Width) * (Defs.m_fZoomDetail / fWorldRatio), 2);
							float fZ = (float)Math.Round((pSelectingArea.Top + (float)pRandom.NextDouble() * pSelectingArea.Height) * (Defs.m_fZoomDetail / fWorldRatio), 2);

							if (fWorldRatio > 0.3333f)  // Hardcode!
							{
								fX *= 4;
								fZ *= 4;
							}
#if USE_A_POS_H_COLUMN_IN_NPC_REGEN
							AddRegenSpots(gridRegenSpots.Rows.Count, fX, fZ, 0, 0, 0, 300, -1);
#else
							AddRegenSpots(gridRegenSpots.Rows.Count, fX, fZ, 0, 0, 300, -1);
#endif
						}

						gridRegenSpots.ClearSelection();

						gridRegenSpots.ResumeLayout();

						bDelimitingArea = false;

						pbWorldMap.Invalidate();

						bUserAction = true;

						UpdateTempRegenRows();
					}
				}
				else
				{
					if (e.KeyCode == Keys.P)
					{
						AddRegenSpotAtMapPosition(GetCorrectedMapPosition(pCurrentCursorPosition));
					}
					else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
					{
						DeleteSelectedRegenSpots();
					}
				}
			}
		}

		private void gridRegenSpots_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
		{
			if (bUserAction)
			{
				if (gridRegenSpots.Rows.Count > 0)
				{
					if (gridRegenSpots.CurrentCell.Value == null)
						gridRegenSpots.CurrentCell.Value = 0;

					int nRegenSec = Convert.ToInt32(gridRegenSpots.Rows[e.RowIndex].Cells["regenSec"].Value);
					if (nRegenSec > 0)
						gridRegenSpots.Rows[e.RowIndex].Cells["regenSec"].ToolTipText = $"Real Time Secs: {nRegenSec / 10}";
				}

				pbWorldMap.Invalidate();

				UpdateTempRegenRows();
			}
		}

		private void gridRegenSpots_SelectionChanged(object sender, EventArgs e) { if (bUserAction) pbWorldMap.Invalidate(); }

		private void gridRegenSpots_MouseWheel(object? sender, MouseEventArgs e)
		{
			if (bUserAction)
			{
				if (gridRegenSpots.CurrentCell != null && gridRegenSpots.CurrentCell.ColumnIndex == 3 && gridRegenSpots.IsCurrentCellInEditMode)
				{
					DataGridViewCell pCell = gridRegenSpots.Rows[gridRegenSpots.CurrentRow.Index].Cells["posR"];
					if (float.TryParse(pCell.Value?.ToString(), out float fCurrentValue))
					{
						int nMultiplier = (Control.ModifierKeys == Keys.Control) ? 10 : 1;

						if (e.Delta > 0)
							pCell.Value = fCurrentValue + nMultiplier;
						else
							pCell.Value = fCurrentValue - nMultiplier;

						gridRegenSpots.RefreshEdit();

						pbWorldMap.Invalidate();

						((HandledMouseEventArgs)e).Handled = true;
					}
				}
			}
		}
		/****************************************/
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int i = 0, j, nNPCID = Convert.ToInt32(pTempNPCRow["a_index"]);
			StringBuilder strbuilderQuery = new();

			strbuilderQuery.Append("START TRANSACTION;\n");

			if (pTempDropJobRow != null)
			{
				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_dropjob WHERE a_npc_idx={nNPCID};\n");

				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempDropJobRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"{pTempDropJobRow[pCol]}, ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_npc_dropjob ({strColumnsNames}) VALUES ({strColumnsValues});\n");
			}

			if (pTempDropAllRows != null)
			{
				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_drop_all WHERE a_npc_idx={nNPCID};\n");

				if (pTempDropAllRows.Any(row => Convert.ToInt32(row["a_item_idx"]) != -1))
				{
					StringBuilder strColumnsNames = new();
					StringBuilder strColumnsValues = new();
					HashSet<string> addedColumns = new();

					foreach (DataRow pRow in pTempDropAllRows)
					{
						if (Convert.ToInt32(pRow["a_item_idx"]) != -1)
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

								strColumnsValues.Append($"{pRow[pCol]}, ");
							}

							strColumnsValues.Length -= 2;
							strColumnsValues.Append("), ");
						}
					}

					strColumnsNames.Length -= 2;
					strColumnsValues.Length -= 2;

					strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_npc_drop_all ({strColumnsNames}) VALUES {strColumnsValues};\n");
				}
			}

			if (gridDropRaid.Rows.Count > 0)
			{
				pTempDropRaidRows = new DataRow[gridDropRaid.Rows.Count];

				foreach (DataGridViewRow pGridRow in gridDropRaid.Rows)
				{
					DataRow pRow = pMain.pTables.NPCDropRaidTable.NewRow();

					pRow["a_npc_index"] = nNPCID;
					pRow["a_item_index"] = pGridRow.Cells["item"].Tag;
					pRow["a_count"] = pGridRow.Cells["count"].Value;
					pRow["a_prob"] = pGridRow.Cells["prob"].Value;
					pRow["a_flag"] = pGridRow.Cells["flag"].Value;

					for (j = 1; j <= 14; j++)
						pRow["a_spec_item_index" + j] = pGridRow.Cells["specitem" + j].Tag;

					pRow["a_spec_count"] = pGridRow.Cells["speccount"].Value;
					pRow["a_spec_prob"] = pGridRow.Cells["specprob"].Value;

					pTempDropRaidRows[i] = pRow;

					i++;
				}

				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_dropraid WHERE a_npc_index={nNPCID};\n");

				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();
				HashSet<string> addedColumns = new();

				foreach (DataRow pRow in pTempDropRaidRows)
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

						strColumnsValues.Append($"{pRow[pCol]}, ");
					}

					strColumnsValues.Length -= 2;
					strColumnsValues.Append("), ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_npc_dropraid ({strColumnsNames}) VALUES {strColumnsValues};\n");
			}

			if (pTempRegenRows != null)
			{
				strbuilderQuery.Append($"DELETE FROM {pMain.pSettings.DBData}.t_npc_regen WHERE a_npc_idx={nNPCID};\n");

				if (pTempRegenRows.Length > 0)
				{
					StringBuilder strColumnsNames = new();
					StringBuilder strColumnsValues = new();
					HashSet<string> addedColumns = new();

					foreach (DataRow pRow in pTempRegenRows)
					{
						strColumnsValues.Append("(");

						foreach (DataColumn pCol in pRow.Table.Columns)
						{
							string strColumnName = pCol.ColumnName;

							if (strColumnName == "a_index")
								continue;

							if (!addedColumns.Contains(strColumnName))
							{
								strColumnsNames.Append(strColumnName + ", ");
								addedColumns.Add(strColumnName);
							}

							strColumnsValues.Append($"{Convert.ToString(pRow[pCol], CultureInfo.InvariantCulture)}, ");
						}

						strColumnsValues.Length -= 2;
						strColumnsValues.Append("), ");
					}

					strColumnsNames.Length -= 2;
					strColumnsValues.Length -= 2;

					strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_npc_regen ({strColumnsNames}) VALUES {strColumnsValues};\n");
				}
			}

			// Check if NPC exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow pNPCTableRow = pMain.pTables.NPCTable.Select("a_index=" + nNPCID).FirstOrDefault();
			if (pNPCTableRow != null)   // UPDATE
			{
				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_npc SET");

				foreach (DataColumn pCol in pTempNPCRow.Table.Columns)
				{
					object objValue = pTempNPCRow[pCol];
					if (objValue is string)
						objValue = pMain.EscapeChars(objValue.ToString());
					else
						objValue = Convert.ToString(objValue, CultureInfo.InvariantCulture);

					strbuilderQuery.Append($" {pCol.ColumnName}='{objValue}',");
				}

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nNPCID};\n");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				foreach (DataColumn pCol in pTempNPCRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					object objValue = pTempNPCRow[pCol];
					if (objValue is string)
						objValue = pMain.EscapeChars(objValue.ToString());
					else
						objValue = Convert.ToString(objValue, CultureInfo.InvariantCulture);

					strColumnsValues.Append($"'{objValue}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_npc ({strColumnsNames}) VALUES ({strColumnsValues});\n");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _))
			{
				try
				{
					if (pTempDropJobRow != null)
					{
						DataRow pNPCDropJobTableRow = pMain.pTables.NPCDropJobTable.Select("a_npc_idx=" + nNPCID).FirstOrDefault();
						if (pNPCDropJobTableRow != null)    // Row exist in Global Table.
						{
							pNPCDropJobTableRow.ItemArray = (object[])pTempDropJobRow.ItemArray.Clone();
						}
						else    // Row not exist in Global Table.
						{
							pNPCDropJobTableRow = pMain.pTables.NPCDropJobTable.NewRow();
							pNPCDropJobTableRow.ItemArray = (object[])pTempDropJobRow.ItemArray.Clone();
							pMain.pTables.NPCDropJobTable.Rows.Add(pNPCDropJobTableRow);
						}
					}

					if (pTempDropAllRows != null && pTempDropAllRows.Length > 0)
					{
						DataRow[] pNPCDropAllTableRows = pMain.pTables.NPCDropAllTable.Select("a_npc_idx=" + nNPCID);
						foreach (DataRow pRow in pNPCDropAllTableRows)
							pRow.Delete();

						foreach (DataRow pRow in pTempDropAllRows)
						{
							DataRow newDataRow = pMain.pTables.NPCDropAllTable.NewRow();
							newDataRow.ItemArray = (object[])pRow.ItemArray.Clone();
							pMain.pTables.NPCDropAllTable.Rows.Add(newDataRow);
						}

						pMain.pTables.NPCDropAllTable.AcceptChanges();
					}

					if (pTempDropRaidRows != null && pTempDropRaidRows.Length > 0)
					{
						DataRow[] pNPCDropRaidTableRows = pMain.pTables.NPCDropRaidTable.Select("a_npc_index=" + nNPCID);
						foreach (DataRow pRow in pNPCDropRaidTableRows)
							pRow.Delete();

						int nNextDropRaidIndex = pMain.pTables.NPCDropRaidTable.AsEnumerable()
							.Where(row => row.RowState != DataRowState.Deleted)
							.Select(row => Convert.ToInt32(row["a_index"]))
							.DefaultIfEmpty(0)
							.Max() + 1;

						foreach (DataRow pRow in pTempDropRaidRows)
						{
							DataRow newDataRow = pMain.pTables.NPCDropRaidTable.NewRow();
							newDataRow.ItemArray = (object[])pRow.ItemArray.Clone();

							newDataRow["a_index"] = nNextDropRaidIndex++;

							pMain.pTables.NPCDropRaidTable.Rows.Add(newDataRow);
						}

						pMain.pTables.NPCDropRaidTable.AcceptChanges();
					}

					if (pTempRegenRows != null && pTempRegenRows.Length > 0)
					{
						DataRow[] pNPCRegenRows = pMain.pTables.NPCRegenTable.Select("a_npc_idx=" + nNPCID);
						foreach (DataRow pRow in pNPCRegenRows)
							pRow.Delete();

						int nNextRegenIndex = pMain.pTables.NPCRegenTable.AsEnumerable()
							.Where(row => row.RowState != DataRowState.Deleted)
							.Select(row => Convert.ToInt32(row["a_index"]))
							.DefaultIfEmpty(0)
							.Max() + 1;

						foreach (DataRow pRow in pTempRegenRows)
						{
							DataRow newDataRow = pMain.pTables.NPCRegenTable.NewRow();
							newDataRow.ItemArray = (object[])pRow.ItemArray.Clone();

							newDataRow["a_index"] = nNextRegenIndex++;

							pMain.pTables.NPCRegenTable.Rows.Add(newDataRow);
						}

						pMain.pTables.NPCRegenTable.AcceptChanges();
					}

					if (pNPCTableRow != null)   // Row exist in Global Table, update it.
					{
						pNPCTableRow.ItemArray = (object[])pTempNPCRow.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pNPCTableRow = pMain.pTables.NPCTable.NewRow();
						pNPCTableRow.ItemArray = (object[])pTempNPCRow.ItemArray.Clone();
						pMain.pTables.NPCTable.Rows.Add(pNPCTableRow);
					}
				}
				catch (Exception ex)
				{
					string strError = $"NPC Editor > NPC: {nNPCID} Changes applied in DataBase, but something got wrong while transferring temp data to main tables. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "NPC Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nNPCID;
						pSelectedItem.Text = nNPCID + " - " + tbName.Text;

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						MessageBox.Show("Changes applied successfully!", "NPC Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"NPC Editor > NPC: {nNPCID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "NPC Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
