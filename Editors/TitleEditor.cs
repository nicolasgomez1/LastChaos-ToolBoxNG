//#define ENABLE_FLAG	// NOTE: This Column are not actually used.

/* NOTE: I recommend modificated this, It should be -1 by default. Thats allow to create Castle Titles even for Juno.
# Server Side ↓
	CHANGE: if ( node->m_title->m_proto->m_castleNum > 0) TO: if ( node->m_title->m_proto->m_castleNum != -1)
# DataBase ↓
	Execute: ALTER TABLE t_title CHANGE a_castle_num a_castle_num INT(11) NOT NULL DEFAULT '-1'; UPDATE t_title SET a_castle_num = '-1' WHERE a_castle_num = 0;
*/
#define USE_CASTLE_NUM_DEFAULT_LESS1
#define FIXED_TITLE_TEXT_OPACITY    // NOTE: Actually, the Engine doesn't use AA value from RRGGBBAA. So even if you change Opacity, its doesn't do any change in Game
#define APPLY_OFFSET_TO_BACKGROUND_COLOR    // NOTE: Since the engine do some blends o so. I added this offset to try simulate Alpha in BG Color, This is only for visual in this Tool.

namespace LastChaos_ToolBoxNG
{
	public partial class TitleEditor : Form
	{
		private readonly Main pMain;
		private bool bUserAction = false;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow pTempTitleRow;
		private int nOriginalClaimItemID = -1;
#if ENABLE_FLAG
		private ToolTip? pToolTip;
		private Dictionary<Control, ToolTip>? pToolTips = new();
#endif
#if APPLY_OFFSET_TO_BACKGROUND_COLOR
		private int nBackgroundColorAlphaOffset = 50;
#endif
		private sealed class TitleOptionLevelInfo
		{
			public int Level { get; init; }
			public int Value { get; init; }
			public string Probability { get; init; } = "0";
			public bool IsInvalidExisting { get; init; }
		}

		private static readonly Dictionary<int, string> SafeTitleOptionNames = new()
		{
			[0] = "Strength",
			[1] = "Dexterity",
			[2] = "Intelligence",
			[3] = "Constitution",
			[4] = "HP",
			[5] = "MP",
			[6] = "Physical attack",
			[7] = "Melee physical attack",
			[8] = "Ranged physical attack",
			[9] = "Melee hit rate",
			[10] = "Ranged hit rate",
			[11] = "Physical defense",
			[12] = "Melee physical defense",
			[13] = "Ranged physical defense",
			[14] = "Melee evasion",
			[15] = "Ranged evasion",
			[16] = "Magic attack",
			[17] = "Magic hit rate",
			[18] = "Magic defense",
			[19] = "Magic evasion",
			[20] = "All attack",
			[21] = "All hit rate",
			[22] = "All defense",
			[23] = "All evasion",
			[51] = "MP absorption",
			[52] = "HP absorption",
			[53] = "Blind attack",
			[54] = "Poison attack",
			[55] = "Critical chance %",
			[56] = "HP regen %",
			[57] = "MP regen %",
			[58] = "Skill cooldown reduction",
			[59] = "MP consumption reduction",
			[60] = "Stone resistance %",
			[61] = "Stun resistance %",
			[62] = "Silence resistance %",
			[63] = "Block rate %",
			[64] = "Move speed %",
			[65] = "Flight speed",
			[66] = "Deadly chance %",
			[67] = "Strength %",
			[68] = "Dexterity %",
			[69] = "Intelligence %",
			[70] = "Constitution %",
			[71] = "HP %",
			[72] = "MP %",
			[73] = "Weapon attack %",
			[74] = "Armor defense %",
			[75] = "Physical hit rate %",
			[76] = "Magic hit rate %",
			[77] = "Physical evasion %",
			[78] = "Magic evasion %",
			[79] = "HP regen from base %",
			[80] = "MP regen from base %",
			[82] = "EXP gain %",
			[83] = "SP gain %",
			[86] = "Pet attack while active",
			[99] = "All stats",
			[100] = "PVP damage absorb %",
			[101] = "Debuff time reduction %",
			[102] = "Flat HP recovery",
			[103] = "Flat MP recovery",
			[104] = "Strong defense",
			[105] = "Hard attack chance",
			[106] = "Flat max HP",
			[109] = "Gold gain",
			[112] = "Gold gain (legacy wearer-only)",
			[113] = "Double item drop chance",
			[114] = "Double gold drop chance",
			[115] = "Primary item drop chance",
			[116] = "Hand appearance rate",
			[117] = "P2 EXP gain"
		};

		public TitleEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			ConfigureOptionValueCombos();
		}

		private void ConfigureOptionValueCombos()
		{
			for (int i = 0; i < Defs.MAX_TITLE_OPTION; i++)
			{
				int nSlot = i;
				ComboBox cbLevel = (ComboBox)Controls.Find("cbOptionLevel" + i, true)[0];
				cbLevel.DropDownStyle = ComboBoxStyle.DropDown;
				cbLevel.TextChanged += (_, _) => OptionLevelAction(nSlot);
			}
		}

		public class AlphaLabel : Label
		{
			private Color _backColorAlpha = Color.FromArgb(128, 0, 0, 0);
			private Color _foreColorAlpha = Color.FromArgb(255, 0, 0, 0);

			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
			public Color BackColorAlpha
			{
				get => _backColorAlpha;
				set
				{
					_backColorAlpha = value;
					Invalidate();
				}
			}

			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
			public Color ForeColorAlpha
			{
				get => _foreColorAlpha;
				set
				{
					_foreColorAlpha = value;
					Invalidate();
				}
			}

			public AlphaLabel()
			{
				SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				using (SolidBrush pBackgroundBrush = new(_backColorAlpha))
					e.Graphics.FillRectangle(pBackgroundBrush, ClientRectangle);
				/****************************************/
				using (SolidBrush pTextBrush = new(_foreColorAlpha))
				{
					e.Graphics.DrawString(Text, Font, pTextBrush, ClientRectangle, new StringFormat {
						Alignment = StringAlignment.Center,
						LineAlignment = StringAlignment.Center,
						Trimming = StringTrimming.EllipsisCharacter
					});
				}
			}
		}

		private Color RGBAtoARGB(string strHex)
		{
			if (strHex.Length != 8)
			{
				pMain.Logger(LogTypes.Error, "Title Editor > Wrong Hex Color. It should be RRGGBBAA.");

				return Color.Magenta;
			}

			byte R = Convert.ToByte(strHex.Substring(0, 2), 16);
			byte G = Convert.ToByte(strHex.Substring(2, 2), 16);
			byte B = Convert.ToByte(strHex.Substring(4, 2), 16);
			byte A = Convert.ToByte(strHex.Substring(6, 2), 16);

			return Color.FromArgb(A, R, G, B);
		}

		private string ARGBtoRRGGBBAA(Color color) { return $"{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}"; }

		private void SetColors(string strBGColor, string strTitleColor)
		{
			Color ColorARGB = RGBAtoARGB(strBGColor);

			tbBGColorOpacity.Value = ColorARGB.A;
			lbBGOpacity.Text = ColorARGB.A.ToString();

			tbBGColorRed.Value = ColorARGB.R;
			lbBGColorRed.Text = ColorARGB.R.ToString();

			tbBGColorGreen.Value = ColorARGB.G;
			lbBGColorGreen.Text = ColorARGB.G.ToString();

			tbBGColorBlue.Value = ColorARGB.B;
			lbBGColorBlue.Text = ColorARGB.B.ToString();

#if APPLY_OFFSET_TO_BACKGROUND_COLOR
			if (ColorARGB.A < 255)
				ColorARGB = Color.FromArgb((byte)Math.Max(0, ColorARGB.A - nBackgroundColorAlphaOffset), ColorARGB.R, ColorARGB.G, ColorARGB.B);
#endif
			lbTitleViewer.BackColorAlpha = ColorARGB;
			/****************************************/
#if FIXED_TITLE_TEXT_OPACITY
			strTitleColor = strTitleColor.Substring(0, 6) + "FF";
#endif
			ColorARGB = RGBAtoARGB(strTitleColor);

			tbTitleColorOpacity.Value = ColorARGB.A;
			lbTitleColorOpacity.Text = ColorARGB.A.ToString();

			tbTitleColorRed.Value = ColorARGB.R;
			lbTitleColorRed.Text = ColorARGB.R.ToString();

			tbTitleColorGreen.Value = ColorARGB.G;
			lbTitleColorGreen.Text = ColorARGB.G.ToString();

			tbTitleColorBlue.Value = ColorARGB.B;
			lbTitleColorBlue.Text = ColorARGB.B.ToString();

			lbTitleViewer.ForeColorAlpha = ColorARGB;
		}

		private (bool bProceed, bool bDeleteActual) CheckUnsavedChanges()
		{
			bool bProceed = true;
			bool bDeleteActual = false;

			if (bUnsavedChanges)
			{
				if (pMain.pTables.TitleTable?.Select("a_index=" + pTempTitleRow["a_index"]).FirstOrDefault() != null)   // the current selected Title, it is not temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Title Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (pDialogReturn != DialogResult.Yes)
						bProceed = false;
				}
				else    // the current selected Title is temporary.
				{
					DialogResult pDialogReturn = MessageBox.Show("The current Title is temporary, if you don't press Update. Do you want to continue and lose all the information regarding it?", "Title Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
				LoadUIData(nID, strName, false);

				MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
				MainList.SelectedIndex = MainList.Items.Count - 1;
				MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

				pLastSelected = (Main.ListBoxItem?)MainList.SelectedItem;

				bUnsavedChanges = true;
			}
		}

		private async Task LoadTitleDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_name",
				"a_enable",
				"a_effect_name",
				"a_attack",
				"a_damage",
				"a_time",
				"a_bgcolor",
				"a_color",
				"a_option_index0", "a_option_level0",
				"a_option_index1", "a_option_level1",
				"a_option_index2", "a_option_level2",
				"a_option_index3", "a_option_level3",
				"a_option_index4", "a_option_level4",
				"a_option_value0",
				"a_option_value1",
				"a_option_value2",
				"a_option_value3",
				"a_option_value4",
				"a_item_index",
#if ENABLE_FLAG
				"a_flag",
#endif
				"a_castle_num"
			];

			if (pMain.pTables.TitleTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pMain.pTables.TitleTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				await EnsureTitleRawOptionValueColumnsAsync();

				DataTable? pNewTable = await Task.Run(() =>
				{
					return pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pMain.pSettings.DBData}.t_title ORDER BY a_index;");
				});

				if (pMain.pTables.TitleTable == null)
					pMain.pTables.TitleTable = pNewTable;
				else
					pMain.MergeDataTables(pNewTable, "a_index", ref pMain.pTables.TitleTable);
			}
		}

		private async Task EnsureTitleRawOptionValueColumnsAsync()
		{
			await Task.Run(() =>
			{
				DataTable? pColumns = pMain.QuerySelect(
					pMain.pSettings.DBCharset,
					$"SHOW COLUMNS FROM {pMain.pSettings.DBData}.t_title LIKE 'a_option_value%';"
				);

				HashSet<string> existingColumns = [];
				if (pColumns != null)
				{
					foreach (DataRow pRow in pColumns.Rows)
						existingColumns.Add(pRow["Field"].ToString() ?? string.Empty);

					pColumns.Dispose();
				}

				StringBuilder strAlter = new();
				for (int i = 0; i < Defs.MAX_TITLE_OPTION; i++)
				{
					string strColumnName = "a_option_value" + i;
					if (!existingColumns.Contains(strColumnName))
						strAlter.Append($"ALTER TABLE {pMain.pSettings.DBData}.t_title ADD COLUMN {strColumnName} INT(11) NOT NULL DEFAULT 0 AFTER a_option_level{i};\n");
				}

				if (strAlter.Length > 0)
					pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strAlter.ToString(), out long _);
			});
		}

		private async Task EnsureTitleSupportOptionRowsAsync()
		{
			await Task.Run(() =>
			{
				EnsureTitleSupportOptionRow(117, "P2 EXP gain");
			});
		}

		private List<string> GetOptionNameColumns()
		{
			List<string> listColumns = [];
			DataTable? pColumns = pMain.QuerySelect(
				pMain.pSettings.DBCharset,
				$"SHOW COLUMNS FROM {pMain.pSettings.DBData}.t_option LIKE 'a_name%';",
				false
			);

			if (pColumns != null)
			{
				foreach (DataRow pRow in pColumns.Rows)
				{
					string strColumn = pRow["Field"].ToString() ?? string.Empty;
					if (!string.IsNullOrWhiteSpace(strColumn) && !listColumns.Contains(strColumn))
						listColumns.Add(strColumn);
				}

				pColumns.Dispose();
			}

			if (listColumns.Count == 0)
			{
				listColumns.Add("a_name");
				foreach (string strNation in pMain.pSettings.NationSupported)
				{
					string strColumn = "a_name_" + strNation.ToLower();
					if (!listColumns.Contains(strColumn))
						listColumns.Add(strColumn);
				}
			}

			return listColumns;
		}

		private void EnsureTitleSupportOptionRow(int nOptionType, string strOptionName)
		{
			DataTable? pExisting = pMain.QuerySelect(
				pMain.pSettings.DBCharset,
				$"SELECT a_index FROM {pMain.pSettings.DBData}.t_option WHERE a_type={nOptionType} LIMIT 1;",
				false
			);

			if (pExisting != null)
			{
				bool bExists = pExisting.Rows.Count > 0;
				pExisting.Dispose();

				if (bExists)
				{
					string strAssignments = string.Join(", ", GetOptionNameColumns().Select(strColumn => $"{strColumn}='{pMain.EscapeChars(strOptionName)}'"));
					pMain.QueryUpdateInsertDelete(
						pMain.pSettings.DBCharset,
						$"UPDATE {pMain.pSettings.DBData}.t_option SET {strAssignments} WHERE a_type={nOptionType};",
						out long _
					);
					return;
				}
			}

			DataTable? pIndexConflict = pMain.QuerySelect(
				pMain.pSettings.DBCharset,
				$"SELECT a_index FROM {pMain.pSettings.DBData}.t_option WHERE a_index={nOptionType} LIMIT 1;",
				false
			);

			int nOptionIndex = nOptionType;
			if (pIndexConflict != null)
			{
				bool bConflict = pIndexConflict.Rows.Count > 0;
				pIndexConflict.Dispose();

				if (bConflict)
				{
					DataTable? pMaxIndex = pMain.QuerySelect(
						pMain.pSettings.DBCharset,
						$"SELECT MAX(a_index) AS max_index FROM {pMain.pSettings.DBData}.t_option;",
						false
					);

					if (pMaxIndex != null && pMaxIndex.Rows.Count > 0)
						nOptionIndex = Convert.ToInt32(pMaxIndex.Rows[0]["max_index"]) + 1;

					pMaxIndex?.Dispose();
				}
			}

			List<string> listColumns =
			[
				"a_index",
				"a_type",
				"a_level",
				"a_prob",
				"a_weapon_type",
				"a_wear_type",
				"a_accessory_type"
			];

			List<string> listValues =
			[
				nOptionIndex.ToString(),
				nOptionType.ToString(),
				"1 2 3 4 5 6 7 8 9 10 15 20 25 30 35 40 45 50 60 70 80 90 100 0 0 0 0 0 0 0 0 0 0 0 0 0",
				"0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0",
				"0",
				"0",
				"0"
			];

			foreach (string strColumn in GetOptionNameColumns())
			{
				listColumns.Add(strColumn);
				listValues.Add(strOptionName);
			}

			string strColumns = string.Join(", ", listColumns);
			string strValues = string.Join(", ", listValues.Select(strValue => $"'{pMain.EscapeChars(strValue)}'"));

			pMain.QueryUpdateInsertDelete(
				pMain.pSettings.DBCharset,
				$"INSERT INTO {pMain.pSettings.DBData}.t_option ({strColumns}) VALUES ({strValues});",
				out long _
			);
		}

		private async void TitleEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
#if ENABLE_FLAG
			lbFlag.Visible = true;
			btnItemFlag.Visible = true;
#endif
#if FIXED_TITLE_TEXT_OPACITY
			tbTitleColorOpacity.Enabled = false;
#endif
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await EnsureTitleSupportOptionRowsAsync();

			await Task.WhenAll(
				LoadTitleDataAsync(),
				pMain.GenericLoadItemDataAsync(),
				pMain.GenericLoadZoneDataAsync(),
				pMain.GenericLoadOptionDataAsync()
			);
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Titles, Items, Zones & Options Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			(new ToolTip()).SetToolTip(tbDuration, "Days (-1 Equals to Infinity duration)");
			/****************************************/
			if (pMain.pTables.ZoneTable != null)
			{
				cbCastleSelector.BeginUpdate();
#if USE_CASTLE_NUM_DEFAULT_LESS1
				cbCastleSelector.Items.Add("0 - NONE");
#else
				pMain.Logger(LogTypes.Warning, "Title Editor > Using '0 - Juno' in Castle field equals to NONE.");
#endif
				foreach (DataRow pRow in pMain.pTables.ZoneTable.Rows)
					cbCastleSelector.Items.Add($"{pRow["a_zone_index"]} - {pRow["a_name"]}");

				cbCastleSelector.EndUpdate();
			}
			/****************************************/
			PopulateTitleOptionSelectors();
			/****************************************/
			if (pMain.pTables.TitleTable != null && pMain.pTables.ItemTable != null)
			{
				MainList.BeginUpdate();

				int nClaimItemID;
				string strClaimItemName = "ITEM NOT FOUND";

				foreach (DataRow pRow in pMain.pTables.TitleTable.Rows)
				{
					nClaimItemID = Convert.ToInt32(pRow["a_item_index"]);

					DataRow? pItemRow = pMain.pTables.ItemTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nClaimItemID).FirstOrDefault();
					if (pItemRow != null)
						strClaimItemName = pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();
					else
						pMain.Logger(LogTypes.Error, $"Title Editor > Title: {pRow["a_index"]} Error: a_item_index: {nClaimItemID} not exist in t_item.");

					AddToList(Convert.ToInt32(pRow["a_index"]), strClaimItemName, false);
				}

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Titles, Items, Zones & Options Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;
			btnAddNew.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void TitleEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
#if ENABLE_FLAG
				foreach (var toolTip in pToolTips.Values.Distinct())
					toolTip.Dispose();

				pToolTips = null;
#endif
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Title Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nTitleID, string strTitle, bool bLoadFrompTitleTable)
		{
			bUserAction = false;
			/****************************************/
			// Reset Controls
			cbCastleSelector.SelectedIndex = -1;
			PopulateTitleOptionSelectors();
			/****************************************/
			if (bLoadFrompTitleTable && pMain.pTables.TitleTable != null)
			{
				pTempTitleRow = pMain.pTables.TitleTable.NewRow();
				pTempTitleRow.ItemArray = (object[])pMain.pTables.TitleTable.Select("a_index=" + nTitleID)[0].ItemArray.Clone();
			}
			/****************************************/
			// General
			tbID.Text = nTitleID.ToString();
			/****************************************/
			if (pTempTitleRow["a_enable"].ToString() == "1")
				cbEnable.Checked = true;
			else
				cbEnable.Checked = false;
			/****************************************/
			int nClaimItemID = 0;
			string strClaimItemName = "";

			if (bLoadFrompTitleTable)
			{
				nClaimItemID = Convert.ToInt32(pTempTitleRow["a_item_index"]);
				bool bClaimItemExist = true;

				nOriginalClaimItemID = nClaimItemID;

				if (nClaimItemID > 0)
				{
					DataRow? pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nClaimItemID).FirstOrDefault();
					if (pItemRow != null)
					{
						strClaimItemName = pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

						strTitle = strClaimItemName;

						btnClaimItem.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
					}
					else
					{
						bClaimItemExist = false;
					}
				}
				else
				{
					bClaimItemExist = false;
				}

				if (!bClaimItemExist)
					pMain.Logger(LogTypes.Warning, $"Title Editor > Title: {nTitleID} have no Claim Item in t_item.");
			}
			else
			{
				strClaimItemName = "Not defined yet";

				btnClaimItem.Image = null;
			}

			btnClaimItem.Text = $"{nClaimItemID} - {strClaimItemName}";
			/****************************************/
			tbDuration.Text = pTempTitleRow["a_time"].ToString();
			/****************************************/
			int nCastleNum = Convert.ToInt32(pTempTitleRow["a_castle_num"]);
			if (pMain.pTables.ZoneTable != null && nCastleNum >= pMain.pTables.ZoneTable.Rows.Count)
				pMain.Logger(LogTypes.Error, $"Title Editor > Title: {nTitleID} Error: a_castle_num out of range.");
			else
#if USE_CASTLE_NUM_DEFAULT_LESS1
				cbCastleSelector.SelectedIndex = nCastleNum <= 0 ? 0 : nCastleNum + 1;
#else
			cbCastleSelector.SelectedIndex = nCastleNum;
#endif
			/****************************************/
			tbEffectName.Text = pTempTitleRow["a_effect_name"].ToString();
			tbEffectAttack.Text = pTempTitleRow["a_attack"].ToString();
			tbEffectDamage.Text = pTempTitleRow["a_damage"].ToString();
			/****************************************/
#if ENABLE_FLAG
			string strItemFlag = pTempTitleRow["a_flag"].ToString();

			btnItemFlag.Text = strItemFlag;

			StringBuilder strTooltip = new();
			long lItemFlag = Convert.ToInt64(strItemFlag);

			for (int i = 0; i < Defs.ItemFlags.Length; i++)
			{
				if ((lItemFlag & 1L << i) != 0)
					strTooltip.Append(Defs.ItemFlags[i] + "\n");
			}

			if (lItemFlag > 0 && strTooltip.Length <= 0)
				pMain.Logger(LogTypes.Error, $"Title Editor > Title: {nTitleID} Error: a_flag out of range.");

			pToolTip = new ToolTip();
			pToolTip.SetToolTip(btnItemFlag, strTooltip.ToString());
			pToolTips[btnItemFlag] = pToolTip;
#endif
			/****************************************/
			string strBGColor = pTempTitleRow["a_bgcolor"].ToString() ?? "FFFFFFFF";
			string strTitleColor = pTempTitleRow["a_color"].ToString() ?? "FFFFFFFF";

			SetColors(strBGColor, strTitleColor);

			tbBGColor.Text = strBGColor;
			tbTitleColor.Text = strTitleColor;
			/****************************************/
			lbTitleViewer.Text = strTitle;
			/****************************************/
			ComboBox cbObj;

			for (int i = 0; i < Defs.MAX_TITLE_OPTION; i++)
			{
				cbObj = (ComboBox)Controls.Find("cbOptionID" + i, true)[0];

				cbObj.Enabled = true;
				EnsureCurrentTitleOptionVisible(cbObj, Convert.ToInt32(pTempTitleRow["a_option_index" + i]));

				for (int j = 0; j < cbObj.Items.Count; j++)
				{
					if (((Main.ComboBoxItem)cbObj.Items[j]).Value.ToString() == pTempTitleRow["a_option_index" + i].ToString())
					{
						cbObj.SelectedIndex = j;

						break;
					}
				}
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

					LoadUIData(((Main.ListBoxItem)nSelected).ID, "", true);

					MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
					MainList.SelectedItem = nSelected;
					MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

					bUnsavedChanges = false;
				}
				else
				{
					bUnsavedChanges = false;

					LoadUIData(pSelectedItem.ID, "", true);
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

				pMain.pTables.TitleTable?.Dispose();
				pMain.pTables.TitleTable = null;

				pMain.pTables.ItemTable?.Dispose();
				pMain.pTables.ItemTable = null;

				pMain.pTables.ZoneTable?.Dispose();
				pMain.pTables.ZoneTable = null;

				pMain.pTables.OptionTable?.Dispose();
				pMain.pTables.OptionTable = null;

				btnUpdate.Enabled = false;

				btnAddNew.Enabled = false;
				btnCopy.Enabled = false;
				btnDelete.Enabled = false;

				TitleEditor_LoadAsync(sender, e);
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
				int i, nNewTitleID = 9999;
				string strNewTitleName = "New Title";
				DataRow pNewRow;

				List<string> listTinyIntColumns =
				[
					"a_enable",
					"a_option_level0",
					"a_option_level1",
					"a_option_level2",
					"a_option_level3",
					"a_option_level4"
				];

				List<string> listIntColumns =
				[
					"a_index",
					"a_time",
					"a_option_index0",
					"a_option_index1",
					"a_option_index2",
					"a_option_index3",
					"a_option_index4",
					"a_option_value0",
					"a_option_value1",
					"a_option_value2",
					"a_option_value3",
					"a_option_value4",
					"a_item_index",
#if ENABLE_FLAG
					"a_flag",
#endif
					"a_castle_num"
				];

				List<string> listVarcharColumns =
				[
					"a_name",
					"a_effect_name",
					"a_attack",
					"a_damage",
					"a_bgcolor",
					"a_color"
				];

				if (pMain.pTables.TitleTable == null)
				{
					DataTable pTitleTableStruct = new();

					foreach (string strColumnName in listTinyIntColumns)
						pTitleTableStruct.Columns.Add(strColumnName, typeof(sbyte));

					foreach (string strColumnName in listIntColumns)
						pTitleTableStruct.Columns.Add(strColumnName, typeof(int));

					foreach (string strColumnName in listVarcharColumns)
						pTitleTableStruct.Columns.Add(strColumnName, typeof(string));

					pNewRow = pTitleTableStruct.NewRow();

					pTitleTableStruct.Dispose();

					DataTable? QueryReturn = pMain.QuerySelect(pMain.pSettings.DBCharset, $"SELECT a_index FROM {pMain.pSettings.DBData}.t_title ORDER BY a_index DESC LIMIT 1;");
					if (QueryReturn != null && QueryReturn.Rows.Count > 0)
					{
						nNewTitleID = Convert.ToInt32(QueryReturn.Rows[0]["a_index"]) + 1;
					}
					else
					{
						if ((nNewTitleID = pMain.AskForIndex(this.Text, "a_index")) == -1)	// I don't test it...
							return;
					}
				}
				else
				{
					nNewTitleID = Convert.ToInt32(pMain.pTables.TitleTable.Select().LastOrDefault()["a_index"]) + 1;

					pNewRow = pMain.pTables.TitleTable.NewRow();
				}

				MessageBox_Input pInput = new(this, "Please set Title:", strNewTitleName);
				if (pInput.ShowDialog() == DialogResult.OK)
					strNewTitleName = pInput.strOutput;

				List<object> listDefaultValue =
				[
					1,	// a_enable
					0,	// a_option_level0
					0,	// a_option_level1
					0,	// a_option_level2
					0,	// a_option_level3
					0,	// a_option_level4
					nNewTitleID,	// a_index
					-1,	// a_time
					-1,	// a_option_index0
					-1,	// a_option_index1
					-1,	// a_option_index2
					-1,	// a_option_index3
					-1,	// a_option_index4
					0,	// a_option_value0
					0,	// a_option_value1
					0,	// a_option_value2
					0,	// a_option_value3
					0,	// a_option_value4
					0,	// a_item_index
#if ENABLE_FLAG
					0,	// a_flag
#endif
#if USE_CASTLE_NUM_DEFAULT_LESS1
					0, // a_castle_num
#else
					0,	// a_castle_num
#endif
					strNewTitleName,	// a_name
					"",	// a_effect_name
					"",	// a_attack
					"",	// a_damage
					"000000FF",	// a_bgcolor
					"FFFFFFFF"	// a_color
				];

				i = 0;
				foreach (string strColumnName in listTinyIntColumns.Concat(listIntColumns).Concat(listVarcharColumns))
				{
					pNewRow[strColumnName] = listDefaultValue[i];

					i++;
				}

				try
				{
					pTempTitleRow = pNewRow;
					nOriginalClaimItemID = 0;
				}
				catch (Exception ex)
				{
					string strError = $"Title Editor > Title: {nNewTitleID} Something got wrong. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						if (bDeleteActual)
							MainList.Items.RemoveAt(MainList.SelectedIndex);

						AddToList(nNewTitleID, strNewTitleName, true);
					}
				}
			}
		}

		private void btnCopy_Click(object sender, EventArgs e)
		{
			var (bProceed, bDeleteActual) = CheckUnsavedChanges();

			if (bDeleteActual)
			{
				MessageBox.Show("You cannot copy this Title. Because it's temporary.", "Title Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				return;
			}

			if (bProceed)
			{
				int nTitleIDToCopy = Convert.ToInt32(pTempTitleRow["a_index"]);
				int nNewTitleID = Convert.ToInt32(pMain.pTables.TitleTable.Select().LastOrDefault()["a_index"]) + 1;

				pTempTitleRow = pMain.pTables.TitleTable.NewRow();
				pTempTitleRow.ItemArray = (object[])pMain.pTables.TitleTable.Select("a_index=" + nTitleIDToCopy)[0].ItemArray.Clone();

				pTempTitleRow["a_index"] = nNewTitleID;

				string strClaimItemNameToCopy = nNewTitleID.ToString();
				bool bClaimItemExist = true;

				DataRow? pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nOriginalClaimItemID).FirstOrDefault();
				if (pItemRow != null)
				{
					strClaimItemNameToCopy = pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString();

					btnClaimItem.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
				}
				else
				{
					bClaimItemExist = false;
				}

				if (!bClaimItemExist)
					pMain.Logger(LogTypes.Warning, $"Title Editor > Title: {nTitleIDToCopy} have no Claim Item in t_item.");

				AddToList(nNewTitleID, strClaimItemNameToCopy + " Copy", true);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nTitleID = Convert.ToInt32(pTempTitleRow["a_index"]);
			DataRow? pTitleRow = pMain.pTables.TitleTable?.Select("a_index=" + nTitleID).FirstOrDefault();

			if (pTitleRow != null)
			{
				if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"DELETE FROM {pMain.pSettings.DBData}.t_title WHERE a_index={nTitleID};", out long _)))
				{
					string strError = $"Title Editor > Title: {nTitleID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			if (bSuccess)
			{
				try
				{
					if (pTitleRow != null)
						pMain.pTables.TitleTable?.Rows.Remove(pTitleRow);

					int nClaimItemID = nOriginalClaimItemID;
					bSuccess = true;
					DataRow? pItemRow = pMain.pTables.ItemTable?.Select("a_index=" + nClaimItemID).FirstOrDefault();

					if (pItemRow != null)
					{
						if (!(bSuccess = pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"UPDATE {pMain.pSettings.DBData}.t_item SET a_enable=0 WHERE a_index={nClaimItemID};", out long _)))
						{
							string strError = $"Title Editor > Claim Item: {nClaimItemID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

							pMain.Logger(LogTypes.Error, strError);

							MessageBox.Show(strError, "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}

					if (bSuccess)
					{
						try
						{
							if (pItemRow != null)
								pItemRow["a_enable"] = 0;
						}
						catch (Exception ex)
						{
							string strError = $"Title Editor > Claim Item: {nClaimItemID} Changes applied in DataBase, but something got wrong while transferring temp to main tables. Please restart the application ({ex.Message}).";

							pMain.Logger(LogTypes.Error, strError);

							MessageBox.Show(strError, "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
							
							bSuccess = false;
						}
						finally
						{
							if (bSuccess)
								pMain.Logger(LogTypes.Success, $"Title Editor > Claim Item: {nClaimItemID} for Title: {nTitleID} has been Disabled successfully!");
						}
					}
				}
				catch (Exception ex)
				{
					string strError = $"Title Editor > Title: {nTitleID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nPrevObjectID = MainList.SelectedIndex <= 0 ? 0 : MainList.SelectedIndex - 1;

						MainList.Items.Remove(MainList.SelectedItem);

						MessageBox.Show("Title Deleted successfully!", "Title Editor", MessageBoxButtons.OK);

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

				pTempTitleRow["a_enable"] = strEnable;

				bUnsavedChanges = true;
			}
		}

		private void btnClaimItem_Click(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				ItemPicker pItemSelector = new(pMain, this, Convert.ToInt32(pTempTitleRow["a_item_index"]));
				if (pItemSelector.ShowDialog() != DialogResult.OK)
					return;

				int nClaimItemID = Convert.ToInt32(pItemSelector.ReturnValues[0]);
				string strItemName = nClaimItemID.ToString();

				if (nClaimItemID > 0)
				{
					strItemName += $" - {pItemSelector.ReturnValues[1]}";

					btnClaimItem.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemSelector.ReturnValues[3].ToString(), Convert.ToInt32(pItemSelector.ReturnValues[4]), Convert.ToInt32(pItemSelector.ReturnValues[5])), new Size(24, 24));
				}
				else
				{
					btnClaimItem.Image = null;
				}

				btnClaimItem.Text = strItemName;

				pTempTitleRow["a_item_index"] = nClaimItemID;

				bUnsavedChanges = true;
			}
		}

		private void tbDuration_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				pTempTitleRow["a_time"] = tbDuration.Text;

				bUnsavedChanges = true;
			}
		}

		private void cbGradeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				int nType = cbCastleSelector.SelectedIndex;
				if (nType != -1)
				{
#if USE_CASTLE_NUM_DEFAULT_LESS1
					pTempTitleRow["a_castle_num"] = nType <= 0 ? 0 : nType - 1;
#else
					pTempTitleRow["a_castle_num"] = nType;
#endif
					bUnsavedChanges = true;
				}
			}
		}

		private void tbEffectName_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				bUserAction = false;

				tbEffectAttack.Text = "";
				pTempTitleRow["a_attack"] = "";

				tbEffectDamage.Text = "";
				pTempTitleRow["a_damage"] = "";

				bUserAction = true;

				pTempTitleRow["a_effect_name"] = tbEffectName.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbEffectAttack_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				bUserAction = false;

				tbEffectName.Text = "";
				pTempTitleRow["a_effect_name"] = "";

				tbEffectDamage.Text = "";
				pTempTitleRow["a_damage"] = "";

				bUserAction = true;

				pTempTitleRow["a_attack"] = tbEffectAttack.Text;

				bUnsavedChanges = true;
			}
		}

		private void tbEffectDamage_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				bUserAction = false;

				tbEffectName.Text = "";
				pTempTitleRow["a_effect_name"] = "";

				tbEffectAttack.Text = "";
				pTempTitleRow["a_attack"] = "";

				bUserAction = true;

				pTempTitleRow["a_damage"] = tbEffectDamage.Text;

				bUnsavedChanges = true;
			}
		}

		private void btnItemFlag_Click(object sender, EventArgs e)
		{
#if ENABLE_FLAG
			if (bUserAction)
			{
				FlagPicker pFlagSelector = new(this, Defs.ItemFlags, Convert.ToInt32(btnItemFlag.Text));
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

				pTempTitleRow["a_flag"] = lFlag;

				bUnsavedChanges = true;
			}
#endif
		}

		private void TrackBar_Scroll(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				Color ColorARGB = Color.FromArgb(tbBGColorOpacity.Value, tbBGColorRed.Value, tbBGColorGreen.Value, tbBGColorBlue.Value);

				lbBGOpacity.Text = ColorARGB.A.ToString();
				lbBGColorRed.Text = ColorARGB.R.ToString();
				lbBGColorGreen.Text = ColorARGB.G.ToString();
				lbBGColorBlue.Text = ColorARGB.B.ToString();

				tbBGColor.Text = ARGBtoRRGGBBAA(ColorARGB);
#if APPLY_OFFSET_TO_BACKGROUND_COLOR
				if (ColorARGB.A < 255)
					ColorARGB = Color.FromArgb((byte)Math.Max(0, ColorARGB.A - nBackgroundColorAlphaOffset), ColorARGB.R, ColorARGB.G, ColorARGB.B);
#endif
				lbTitleViewer.BackColorAlpha = ColorARGB;
				/****************************************/
#if FIXED_TITLE_TEXT_OPACITY
				ColorARGB = Color.FromArgb(255, tbTitleColorRed.Value, tbTitleColorGreen.Value, tbTitleColorBlue.Value);
#else
				ColorARGB = Color.FromArgb(tbTitleColorOpacity.Value, tbTitleColorRed.Value, tbTitleColorGreen.Value, tbTitleColorBlue.Value);
#endif
				lbTitleColorOpacity.Text = ColorARGB.A.ToString();
				lbTitleColorRed.Text = ColorARGB.R.ToString();
				lbTitleColorGreen.Text = ColorARGB.G.ToString();
				lbTitleColorBlue.Text = ColorARGB.B.ToString();

				tbTitleColor.Text = ARGBtoRRGGBBAA(ColorARGB);

				lbTitleViewer.ForeColorAlpha = ColorARGB;

				TrySyncColorFieldsToTempRow(false);

				bUnsavedChanges = true;
			}
		}

		private Main.ComboBoxItem CreateOptionComboBoxItem(int nOptionType)
		{
			if (nOptionType == -1)
			{
				return new Main.ComboBoxItem
				{
					Value = -1,
					DisplayText = "-1 - NONE"
				};
			}

			if (SafeTitleOptionNames.TryGetValue(nOptionType, out string? strDisplayName))
			{
				return new Main.ComboBoxItem
				{
					Value = nOptionType,
					DisplayText = $"{nOptionType} - {strDisplayName}"
				};
			}

			return new Main.ComboBoxItem
			{
				Value = nOptionType,
				DisplayText = $"{nOptionType} - UNSAFE/legacy option (change before saving)"
			};
		}

		private DataRow? FindOptionRow(int nOptionType)
		{
			return pMain.pTables.OptionTable?.Select("a_type=" + nOptionType).FirstOrDefault();
		}

		private static string[] SplitOptionValues(object? value)
		{
			return (value?.ToString() ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
		}

		private List<TitleOptionLevelInfo> GetTitleOptionLevels(DataRow pOptionRow, int nCurrentLevel, bool bIncludeCurrentInvalid)
		{
			List<TitleOptionLevelInfo> listLevels = [];
			string[] strLevels = SplitOptionValues(pOptionRow["a_level"]);
			string[] strProb = SplitOptionValues(pOptionRow["a_prob"]);

			for (int i = 0; i < strLevels.Length; i++)
			{
				if (!int.TryParse(strLevels[i], out int nValue))
					continue;

				int nLevel = i + 1;
				bool bInvalidCurrent = bIncludeCurrentInvalid && nLevel == nCurrentLevel && nValue == 0;

				if (nValue != 0 || bInvalidCurrent)
				{
					listLevels.Add(new TitleOptionLevelInfo
					{
						Level = nLevel,
						Value = nValue,
						Probability = i < strProb.Length ? strProb[i] : "0",
						IsInvalidExisting = bInvalidCurrent
					});
				}
			}

			if (bIncludeCurrentInvalid && nCurrentLevel > strLevels.Length)
			{
				listLevels.Add(new TitleOptionLevelInfo
				{
					Level = nCurrentLevel,
					Value = 0,
					IsInvalidExisting = true
				});
			}

			return listLevels;
		}

		private static string FormatOptionLevel(TitleOptionLevelInfo pLevelInfo)
		{
			if (pLevelInfo.IsInvalidExisting)
				return $"0 (invalid legacy level {pLevelInfo.Level})";

			string strText = $"{pLevelInfo.Value} (legacy level {pLevelInfo.Level})";

			if (!string.IsNullOrWhiteSpace(pLevelInfo.Probability) && pLevelInfo.Probability != "0")
				strText += $" Prob: {pLevelInfo.Probability}";

			return strText;
		}

		private int GetStoredTitleOptionRawValue(int nSlot, DataRow? pOptionRow, int nSavedLevel)
		{
			string strColumnName = "a_option_value" + nSlot;
			if (pTempTitleRow.Table.Columns.Contains(strColumnName))
			{
				int nRawValue = Convert.ToInt32(pTempTitleRow[strColumnName]);
				if (nRawValue != 0)
					return nRawValue;
			}

			if (pOptionRow == null || nSavedLevel <= 0)
				return 0;

			return GetTitleOptionLevels(pOptionRow, nSavedLevel, true)
				.FirstOrDefault(level => level.Level == nSavedLevel)?.Value ?? 0;
		}

		private int FindLegacyLevelForRawValue(DataRow? pOptionRow, int nRawValue)
		{
			if (pOptionRow == null || nRawValue == 0)
				return 0;

			return GetTitleOptionLevels(pOptionRow, 0, false)
				.FirstOrDefault(level => level.Value == nRawValue)?.Level ?? 0;
		}

		private static bool TryParseLeadingInt(string? strText, out int nValue)
		{
			nValue = 0;
			strText = (strText ?? string.Empty).Trim();

			if (strText.Length <= 0)
				return false;

			int nLength = 0;
			if (strText[0] == '-' || strText[0] == '+')
				nLength = 1;

			while (nLength < strText.Length && char.IsDigit(strText[nLength]))
				nLength++;

			if (nLength == 0 || (nLength == 1 && (strText[0] == '-' || strText[0] == '+')))
				return false;

			return int.TryParse(strText[..nLength], out nValue);
		}

		private bool TryGetComboBoxRawValue(ComboBox cbObj, out int nRawValue)
		{
			nRawValue = 0;

			if (cbObj.SelectedItem is Main.ComboBoxItem pSelectedItem &&
				string.Equals(cbObj.Text, pSelectedItem.ToString(), StringComparison.Ordinal))
			{
				nRawValue = pSelectedItem.Value;
				return true;
			}

			return TryParseLeadingInt(cbObj.Text, out nRawValue);
		}

		private void SetTitleOptionRawValue(int nSlot, int nOptionID, int nRawValue)
		{
			DataRow? pOptionRow = FindOptionRow(nOptionID);
			pTempTitleRow["a_option_level" + nSlot] = FindLegacyLevelForRawValue(pOptionRow, nRawValue);

			string strColumnName = "a_option_value" + nSlot;
			if (pTempTitleRow.Table.Columns.Contains(strColumnName))
				pTempTitleRow[strColumnName] = nRawValue;
		}

		private bool SyncOptionValueFromControl(int nSlot, bool bShowError)
		{
			int nOptionID = Convert.ToInt32(pTempTitleRow["a_option_index" + nSlot]);
			string strValueColumn = "a_option_value" + nSlot;

			if (nOptionID == -1)
			{
				pTempTitleRow["a_option_level" + nSlot] = 0;
				if (pTempTitleRow.Table.Columns.Contains(strValueColumn))
					pTempTitleRow[strValueColumn] = 0;
				return true;
			}

			ComboBox cbObj = (ComboBox)Controls.Find("cbOptionLevel" + nSlot, true)[0];
			if (!TryGetComboBoxRawValue(cbObj, out int nRawValue))
			{
				if (bShowError)
					MessageBox.Show($"Slot {nSlot + 1}: enter a numeric raw stat value.", "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

				return false;
			}

			SetTitleOptionRawValue(nSlot, nOptionID, nRawValue);
			return true;
		}

		private void EnsureCurrentTitleOptionVisible(ComboBox cbObj, int nOptionType)
		{
			foreach (object item in cbObj.Items)
			{
				if (item is Main.ComboBoxItem comboItem && comboItem.Value == nOptionType)
					return;
			}

			cbObj.Items.Add(CreateOptionComboBoxItem(nOptionType));
		}

		private void PopulateTitleOptionSelectors()
		{
			if (pMain.pTables.OptionTable == null)
				return;

			for (int i = 0; i < Defs.MAX_TITLE_OPTION; i++)
			{
				ComboBox cbOption = (ComboBox)Controls.Find("cbOptionID" + i, true)[0];

				cbOption.BeginUpdate();
				cbOption.Items.Clear();
				cbOption.Items.Add(CreateOptionComboBoxItem(-1));

				foreach (int nOptionType in SafeTitleOptionNames.Keys.OrderBy(n => n))
				{
					DataRow? pOptionRow = FindOptionRow(nOptionType);
					if (pOptionRow == null)
						continue;

					cbOption.Items.Add(CreateOptionComboBoxItem(nOptionType));
				}

				cbOption.EndUpdate();

				ComboBox cbLevel = (ComboBox)Controls.Find("cbOptionLevel" + i, true)[0];
				cbLevel.Items.Clear();
				cbLevel.Enabled = false;
			}
		}

		private void tbBGOrTitleColor_TextChanged(object sender, EventArgs e)
		{
			if (bUserAction)
			{
				if (TrySyncColorFieldsToTempRow(false))
					SetColors(tbBGColor.Text, tbTitleColor.Text);

				bUnsavedChanges = true;
			}
		}

		private bool TrySyncColorFieldsToTempRow(bool bShowError)
		{
			if (!TryNormalizeColorHex(tbBGColor.Text, out string strBGColor) ||
				!TryNormalizeColorHex(tbTitleColor.Text, out string strTitleColor))
			{
				if (bShowError)
				{
					MessageBox.Show("Title colors must be 8-digit RRGGBBAA hex values.", "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}

				return false;
			}

			tbBGColor.Text = strBGColor;
			tbTitleColor.Text = strTitleColor;

			pTempTitleRow["a_bgcolor"] = strBGColor;
			pTempTitleRow["a_color"] = strTitleColor;
			return true;
		}

		private static bool TryNormalizeColorHex(string strInput, out string strColor)
		{
			strColor = (strInput ?? string.Empty).Trim().ToUpperInvariant();

			if (strColor.Length != 8)
				return false;

			foreach (char c in strColor)
			{
				if (!Uri.IsHexDigit(c))
					return false;
			}

			return true;
		}
		/****************************************/
		private void OptionSelectAction(int nNumber)
		{
			ComboBox cbObj = (ComboBox)Controls.Find("cbOptionID" + nNumber, true)[0];

			if (cbObj.SelectedItem is not Main.ComboBoxItem pSelectedOption)
				return;

			ComboBox cbLevel = (ComboBox)Controls.Find("cbOptionLevel" + nNumber, true)[0];
			int nOptionID = Convert.ToInt32(pSelectedOption.Value);
			int nSavedLevel = Convert.ToInt32(pTempTitleRow["a_option_level" + nNumber]);

			cbLevel.SelectedIndex = -1;
			cbLevel.Text = string.Empty;
			cbLevel.Items.Clear();

			if (nOptionID == -1)
			{
				cbLevel.Enabled = false;

				if (bUserAction)
				{
					pTempTitleRow["a_option_index" + nNumber] = -1;
					pTempTitleRow["a_option_level" + nNumber] = 0;
					if (pTempTitleRow.Table.Columns.Contains("a_option_value" + nNumber))
						pTempTitleRow["a_option_value" + nNumber] = 0;
					bUnsavedChanges = true;
				}

				return;
			}

			DataRow? pRow = FindOptionRow(nOptionID);
			bool bSafeOption = SafeTitleOptionNames.ContainsKey(nOptionID);

			if (pRow == null || !bSafeOption)
			{
				cbLevel.Items.Add(new Main.ComboBoxItem
				{
					Value = GetStoredTitleOptionRawValue(nNumber, pRow, nSavedLevel),
					DisplayText = "Unsupported/unsafe option. Choose another stat before saving."
				});
				cbLevel.SelectedIndex = 0;
				cbLevel.Enabled = false;

				if (bUserAction)
				{
					pTempTitleRow["a_option_index" + nNumber] = nOptionID;
					bUnsavedChanges = true;
				}

				return;
			}

			List<TitleOptionLevelInfo> listLevels = GetTitleOptionLevels(pRow, nSavedLevel, true);
			int nSavedValue = GetStoredTitleOptionRawValue(nNumber, pRow, nSavedLevel);

			cbLevel.BeginUpdate();
			foreach (TitleOptionLevelInfo pLevelInfo in listLevels)
			{
				cbLevel.Items.Add(new Main.ComboBoxItem
				{
					Value = pLevelInfo.Value,
					DisplayText = FormatOptionLevel(pLevelInfo)
				});
			}
			cbLevel.EndUpdate();
			cbLevel.Enabled = true;

			for (int i = 0; i < cbLevel.Items.Count; i++)
			{
				if (cbLevel.Items[i] is Main.ComboBoxItem pLevelItem && pLevelItem.Value == nSavedValue)
				{
					cbLevel.SelectedIndex = i;
					break;
				}
			}

			if (cbLevel.SelectedIndex == -1)
			{
				if (nSavedValue != 0)
					cbLevel.Text = nSavedValue.ToString();
				else if (bUserAction && cbLevel.Items.Count > 0)
					cbLevel.SelectedIndex = 0;
			}

			if (bUserAction)
			{
				pTempTitleRow["a_option_index" + nNumber] = nOptionID;
				SyncOptionValueFromControl(nNumber, false);

				bUnsavedChanges = true;
			}
		}

		private void cbOptionID0_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(0); }
		private void cbOptionID1_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(1); }
		private void cbOptionID2_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(2); }
		private void cbOptionID3_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(3); }
		private void cbOptionID4_SelectedIndexChanged(object sender, EventArgs e) { OptionSelectAction(4); }
		/****************************************/
		private void OptionLevelAction(int nNumber)
		{
			ComboBox cbObj = (ComboBox)Controls.Find("cbOptionLevel" + nNumber, true)[0];

			if (bUserAction && cbObj.Focused)
			{
				SyncOptionValueFromControl(nNumber, false);
				bUnsavedChanges = true;
			}
		}

		private void cbOptionLevel0_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(0); }
		private void cbOptionLevel1_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(1); }
		private void cbOptionLevel2_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(2); }
		private void cbOptionLevel3_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(3); }
		private void cbOptionLevel4_SelectedIndexChanged(object sender, EventArgs e) { OptionLevelAction(4); }
		/****************************************/
		private bool ValidateTitleOptionsForSave()
		{
			List<string> listErrors = [];

			for (int i = 0; i < Defs.MAX_TITLE_OPTION; i++)
			{
				int nOptionID = Convert.ToInt32(pTempTitleRow["a_option_index" + i]);
				int nLevel = Convert.ToInt32(pTempTitleRow["a_option_level" + i]);
				int nRawValue = pTempTitleRow.Table.Columns.Contains("a_option_value" + i)
					? Convert.ToInt32(pTempTitleRow["a_option_value" + i])
					: 0;

				if (nOptionID == -1)
				{
					pTempTitleRow["a_option_level" + i] = 0;
					if (pTempTitleRow.Table.Columns.Contains("a_option_value" + i))
						pTempTitleRow["a_option_value" + i] = 0;
					continue;
				}

				if (!SafeTitleOptionNames.TryGetValue(nOptionID, out string? strOptionName))
				{
					listErrors.Add($"Slot {i + 1}: option {nOptionID} is not safe for titles or is not handled by the title runtime.");
					continue;
				}

				DataRow? pOptionRow = FindOptionRow(nOptionID);
				if (pOptionRow == null)
				{
					listErrors.Add($"Slot {i + 1}: option {nOptionID} ({strOptionName}) is missing from t_option.");
					continue;
				}

				if (!SyncOptionValueFromControl(i, false))
				{
					listErrors.Add($"Slot {i + 1}: option {nOptionID} ({strOptionName}) has no numeric raw value.");
					continue;
				}

				nLevel = Convert.ToInt32(pTempTitleRow["a_option_level" + i]);
				nRawValue = pTempTitleRow.Table.Columns.Contains("a_option_value" + i)
					? Convert.ToInt32(pTempTitleRow["a_option_value" + i])
					: 0;

				if (nRawValue == 0)
				{
					if (nLevel <= 0)
					{
						listErrors.Add($"Slot {i + 1}: option {nOptionID} ({strOptionName}) has no raw value.");
						continue;
					}

					bool bValidLevel = GetTitleOptionLevels(pOptionRow, 0, false)
						.Any(level => level.Level == nLevel && level.Value != 0);

					if (!bValidLevel)
						listErrors.Add($"Slot {i + 1}: option {nOptionID} ({strOptionName}) level {nLevel} has a zero or missing fallback value.");
				}
			}

			if (listErrors.Count <= 0)
				return true;

			MessageBox.Show(
				"Please correct the title option setup before saving:\n\n" + string.Join("\n", listErrors.Take(10)),
				"Title Editor",
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning
			);

			return false;
		}
		/****************************************/
		private string SyncTitleNameFromClaimItem()
		{
			if (pTempTitleRow == null || !pTempTitleRow.Table.Columns.Contains("a_name"))
				return string.Empty;

			string strTitleName = pTempTitleRow["a_name"].ToString()?.Trim() ?? string.Empty;
			int nClaimItemID = Convert.ToInt32(pTempTitleRow["a_item_index"]);

			if (nClaimItemID > 0 && pMain.pTables.ItemTable != null)
			{
				DataRow? pItemRow = pMain.pTables.ItemTable.Select("a_index=" + nClaimItemID).FirstOrDefault();
				if (pItemRow != null)
				{
					string strLocaleColumn = "a_name_" + pMain.pSettings.WorkLocale;
					if (pItemRow.Table.Columns.Contains(strLocaleColumn))
						strTitleName = pItemRow[strLocaleColumn].ToString()?.Trim() ?? strTitleName;

					if (string.IsNullOrWhiteSpace(strTitleName) && pItemRow.Table.Columns.Contains("a_name"))
						strTitleName = pItemRow["a_name"].ToString()?.Trim() ?? string.Empty;
				}
			}

			if (string.IsNullOrWhiteSpace(strTitleName))
				strTitleName = lbTitleViewer.Text.Trim();

			pTempTitleRow["a_name"] = strTitleName;
			return strTitleName;
		}
		/****************************************/
		private async void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			bool bShouldExportItemsLod = false;
			int nTitleID = Convert.ToInt32(pTempTitleRow["a_index"]);
			StringBuilder strbuilderQuery = new();
			DataRow? pNewClaimItemRowToInsert = null;

			if (!TrySyncColorFieldsToTempRow(true))
				return;

			if (!ValidateTitleOptionsForSave())
				return;

			string strSyncedTitleName = SyncTitleNameFromClaimItem();

			// Init transaction.
			strbuilderQuery.Append("START TRANSACTION;\n");

			int nCurrentClaimItemID = Convert.ToInt32(pTempTitleRow["a_item_index"]);
			if (nCurrentClaimItemID > 0 && !string.IsNullOrWhiteSpace(strSyncedTitleName))
			{
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item SET a_name=IF(a_name='', '{pMain.EscapeChars(strSyncedTitleName)}', a_name) WHERE a_index={nCurrentClaimItemID};\n");
			}

			// Check if Title exist in Global Table, if exist, do a UPDATE. If not, do a INSERT.
			DataRow? pTitleTableRow = pMain.pTables.TitleTable?.Select("a_index=" + nTitleID).FirstOrDefault();
			if (pTitleTableRow != null)    // UPDATE
			{
				int nClaimItemID = Convert.ToInt32(pTempTitleRow["a_item_index"]);

				if (nClaimItemID != nOriginalClaimItemID)
				{
					strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item SET a_num_0=0 WHERE a_index={nOriginalClaimItemID};\n");

					strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item SET a_num_0={nTitleID} WHERE a_index={nClaimItemID};\n");

					bShouldExportItemsLod = true;
				}

				// Compose UPDATE Query.
				strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_title SET");

				foreach (DataColumn pCol in pTempTitleRow.Table.Columns)
					strbuilderQuery.Append($" {pCol.ColumnName}='{pMain.EscapeChars(pTempTitleRow[pCol].ToString() ?? string.Empty)}',");

				strbuilderQuery.Length -= 1;

				strbuilderQuery.Append($" WHERE a_index={nTitleID};\n");
			}
			else    // INSERT
			{
				// Compose INSERT Query.
				StringBuilder strColumnsNames = new();
				StringBuilder strColumnsValues = new();

				if (Convert.ToInt32(pTempTitleRow["a_item_index"]) == 0)	// Is a New Title. Ask if want to create and Item to Claim it.
				{
					DialogResult pDialogReturn = MessageBox.Show("Do you want to create an Item to Claim this Title?", "Title Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (pDialogReturn == DialogResult.Yes)
					{
						using (ItemEditor pItemEditor = new(pMain))
						{
							pNewClaimItemRowToInsert = pItemEditor.CreateNewItem(true, lbTitleViewer.Text, 2 /*ITYPE_ONCE*/, 11 /*IONCE_TITLE*/, nTitleID);
							if (pNewClaimItemRowToInsert != null)
							{
								bShouldExportItemsLod = true;

								foreach (DataColumn pCol in pNewClaimItemRowToInsert.Table.Columns)
								{
									strColumnsNames.Append(pCol.ColumnName + ", ");

									strColumnsValues.Append($"'{pMain.EscapeChars(pNewClaimItemRowToInsert[pCol].ToString())}', ");
								}

								strColumnsNames.Length -= 2;
								strColumnsValues.Length -= 2;

								strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_item ({strColumnsNames}) VALUES ({strColumnsValues});\n");

								strColumnsNames.Clear();
								strColumnsValues.Clear();

								int nClaimItemID = Convert.ToInt32(pNewClaimItemRowToInsert["a_index"]);

								pTempTitleRow["a_item_index"] = nClaimItemID;
							}
						}
					}
				}

				foreach (DataColumn pCol in pTempTitleRow.Table.Columns)
				{
					strColumnsNames.Append(pCol.ColumnName + ", ");

					strColumnsValues.Append($"'{pMain.EscapeChars(pTempTitleRow[pCol].ToString())}', ");
				}

				strColumnsNames.Length -= 2;
				strColumnsValues.Length -= 2;

				if (pNewClaimItemRowToInsert == null)
				{
					int nExistingClaimItemID = Convert.ToInt32(pTempTitleRow["a_item_index"]);
					if (nExistingClaimItemID > 0)
					{
						strbuilderQuery.Append($"UPDATE {pMain.pSettings.DBData}.t_item SET a_num_0={nTitleID} WHERE a_index={nExistingClaimItemID};\n");
						bShouldExportItemsLod = true;
					}
				}

				strbuilderQuery.Append($"INSERT INTO {pMain.pSettings.DBData}.t_title ({strColumnsNames}) VALUES ({strColumnsValues});\n");
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, strbuilderQuery.Append("COMMIT;").ToString(), out long _))
			{
				try
				{
					DataRow pOldClaimItemRow = pMain.pTables.ItemTable.Select("a_index=" + nOriginalClaimItemID).FirstOrDefault();
					if (pOldClaimItemRow != null && pOldClaimItemRow.Table.Columns.Contains("a_num_0"))
						pOldClaimItemRow["a_num_0"] = 0;

					int nClaimItemID = Convert.ToInt32(pTempTitleRow["a_item_index"]);

					DataRow pNewClaimItemRow = pMain.pTables.ItemTable.Select("a_index=" + nClaimItemID).FirstOrDefault();
					if (pNewClaimItemRow != null)
					{
						if (pNewClaimItemRow.Table.Columns.Contains("a_num_0"))
							pNewClaimItemRow["a_num_0"] = nTitleID;

						if (
							pNewClaimItemRow.Table.Columns.Contains("a_name")
							&& string.IsNullOrWhiteSpace(pNewClaimItemRow["a_name"].ToString())
							&& !string.IsNullOrWhiteSpace(strSyncedTitleName)
						)
						{
							pNewClaimItemRow["a_name"] = strSyncedTitleName;
						}
					}

					nOriginalClaimItemID = nClaimItemID;

					if (pTitleTableRow != null)    // Row exist in Global Table, update it.
					{
						pTitleTableRow.ItemArray = (object[])pTempTitleRow.ItemArray.Clone();
					}
					else    // Row not exist in Global Table, insert it.
					{
						pTitleTableRow = pMain.pTables.TitleTable?.NewRow();
						pTitleTableRow.ItemArray = (object[])pTempTitleRow.ItemArray.Clone();
						pMain.pTables.TitleTable?.Rows.Add(pTitleTableRow);

						if (pNewClaimItemRowToInsert != null)
						{
							DataRow pItemRow = pMain.pTables.ItemTable.NewRow();

							foreach (DataColumn col in pMain.pTables.ItemTable.Columns)
							{
								if (pNewClaimItemRowToInsert.Table.Columns.Contains(col.ColumnName))
									pItemRow[col.ColumnName] = pNewClaimItemRowToInsert[col.ColumnName];
							}

							pMain.pTables.ItemTable.Rows.Add(pItemRow);

							if (nClaimItemID > 0)
							{
								pItemRow = pMain.pTables.ItemTable?.AsEnumerable().Where(row => Convert.ToInt32(row["a_index"]) == nClaimItemID).FirstOrDefault();
								if (pItemRow != null)
								{
									btnClaimItem.Image = new Bitmap(pMain.GetIcon("ItemBtn", pItemRow["a_texture_id"].ToString(), Convert.ToInt32(pItemRow["a_texture_row"]), Convert.ToInt32(pItemRow["a_texture_col"])), new Size(24, 24));
									btnClaimItem.Text = $"{nClaimItemID} - {lbTitleViewer.Text}";
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					string strError = $"Title Editor > Title: {nTitleID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						int nSelectedIndex = MainList.SelectedIndex;
						Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)MainList.Items[nSelectedIndex];
						pSelectedItem.ID = nTitleID;

						DataRow? pItemRow = pMain.pTables.ItemTable?.Select("a_index=" + pTitleTableRow["a_item_index"]).FirstOrDefault();
						string strTitleName = "ITEM NOT FOUND";

						if (pItemRow != null)
							strTitleName = pItemRow["a_name_" + pMain.pSettings.WorkLocale].ToString() ?? string.Empty;

						pSelectedItem.Text = $"{nTitleID} - {strTitleName}";

						MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
						MainList.Items[nSelectedIndex] = pSelectedItem;
						MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

						bool bTitlesExported = await TryExportTitlesLodAsync(nTitleID);
						bool bItemsExported = !bShouldExportItemsLod || await TryExportItemClientLodsAsync(nTitleID);
						bool bOptionsExported = await TryExportOptionClientLodsAsync(nTitleID);
						bool bExported = bTitlesExported && bItemsExported && bOptionsExported;
						string strMessage = bExported
							? "Changes applied successfully! titletool.lod and option .lod files have been exported to the client."
							: "Changes applied successfully, but one or more client .lod exports failed. Check the Toolbox log before testing in-game.";
						MessageBox.Show(strMessage, "Title Editor", MessageBoxButtons.OK, bExported ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Title Editor > Title: {nTitleID} Something got wrong while trying to execute the MySQL Transaction. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Title Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task<bool> TryExportTitlesLodAsync(int nTitleID)
		{
			try
			{
				using Exporter pExporter = new(pMain);
				await pExporter.ExportTitlesLodAsync(true);
				return true;
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Title Editor > Title: {nTitleID} saved, but titletool.lod export failed: {ex.Message}");
				return false;
			}
		}

		private async Task<bool> TryExportItemClientLodsAsync(int nTitleID)
		{
			bool bSuccess = true;
			using Exporter pExporter = new(pMain);

			try
			{
				await pExporter.ExportItemsLodAsync(true);
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Title Editor > Title: {nTitleID} saved, but ItemAll.lod export failed: {ex.Message}");
				bSuccess = false;
			}

			try
			{
				await pExporter.ExportItemStringsLodAsync(true);
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Title Editor > Title: {nTitleID} saved, but strItem .lod export failed: {ex.Message}");
				bSuccess = false;
			}

			return bSuccess;
		}

		private async Task<bool> TryExportOptionClientLodsAsync(int nTitleID)
		{
			bool bSuccess = true;
			using Exporter pExporter = new(pMain);

			try
			{
				await pExporter.ExportOptionsLodAsync(true);
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Title Editor > Title: {nTitleID} saved, but Option.lod export failed: {ex.Message}");
				bSuccess = false;
			}

			try
			{
				await pExporter.ExportOptionStringsLodAsync(true);
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Title Editor > Title: {nTitleID} saved, but strOption .lod export failed: {ex.Message}");
				bSuccess = false;
			}

			return bSuccess;
		}
	}
}
