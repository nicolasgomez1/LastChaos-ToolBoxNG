//#define USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION	// NOTE: Uncomment this line if use my custom allowed zone, plus in items shoppers, monstercombo ui quests, rareoptions, catalog or world boss rework/systems
//#define REQUIRED_QUEST_TO_USE_SHOPPER_V1	// NOTE: Uncomment this line if use my custom required quest to use shopper v1 system
//#define USE_ORIGINAL_SKILL_CALCULATESUM_AND_END_TAG	// NOTE: Uncomment this line if use original Skills File format
//#define EXPORT_LACARETTES_FILES_FOR_EACH_LANGUAGE   // NOTE: This is not actually necessary; since the cliente source takes the Course names directly from strLacarette_X.lod

namespace LastChaos_ToolBoxNG
{
	public partial class Exporter : Form
	{
		private readonly Main pMain;
#if DEBUG
		private long lTotalElapsedTime;
#endif
		private List<Task> ExportTasks = new();

		private static readonly List<string[]> DataLodTypes = new List<string[]>
		{
			// NOTE: Change file names (Second string) to match with your source files.
			new string[2] { "ITEMS",             "ItemAll" },
			new string[2] { "MOONSTONES",        "MoonStones" },
			new string[2] { "LACARETTES",        "Lacarettes" },
			new string[2] { "CATALOGS",          "Catalog" },
			new string[2] { "SMC",               "Smc" },
			new string[2] { "MONSTERCOMBO",      "MonsterCombo" },
			new string[2] { "ITEMEXCHANGES",     "ItemExchanges" },
			new string[2] { "SPECIALSKILLS",     "SpecialSkills" },
			new string[2] { "SKILLS",            "Skills" },
			new string[2] { "EQUIPMENTEXCHANGES","EquipmentExchanges" },
			new string[2] { "NPCCHANNELS",       "NPCChannels" },
			new string[2] { "ACTIONS",           "Actions" },
			new string[2] { "SKILLTREES",        "SkillTrees" },
			new string[2] { "SHOPS",             "Shops" },
			new string[2] { "QUESTS",            "Quests" },
			new string[2] { "RAREOPTIONS",       "RareOptions" },
			new string[2] { "BIGPETS",           "BigPets" },
			new string[2] { "MOBS",              "Mobs" },
			new string[2] { "OPTIONS",           "Option" },
			new string[2] { "TITLES",            "titletool" },
			new string[2] { "MAPS",              "Maps" },
			new string[2] { "NPCHELP",           "NPCHelp" },
			new string[2] { "AFFINITIES",        "Affinity" },
			new string[2] { "DATAITEMCOLLECTION","ItemCollection" },
			new string[2] { "ITEMFORTUNE",       "ItemFortune" },
			new string[2] { "MOBHELP",           "MOBHelp" },
			new string[2] { "DATASETITEM",       "ItemSets" },
			new string[2] { "ZONEFLAG",          "FlagsZones" }
		};

		private class SMCData
		{
			public string Name;
			public List<string> MeshPaths;
			public List<string> TextureNames;
			public List<string> TexturePaths;

			public SMCData()
			{
				Name = "NONE";
				MeshPaths = new List<string>();
				TextureNames = new List<string>();
				TexturePaths = new List<string>();
			}
		}

		public Exporter(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
		}

		public async Task ExportSkillsLodAsync(bool exportToClientFolder = true)
		{
			ExportTasks.Clear();
			rbExportToLocalFolder.Checked = !exportToClientFolder;
			rbExportToClient.Checked = exportToClientFolder;

			pMain.Logger(LogTypes.Message, "Exporting: Skills.lod file...");
			ExportSKILLSAsync("Skills");
			await Task.WhenAll(ExportTasks);
			pMain.Logger(LogTypes.Success, "Skills.lod has been Exported.");
		}

		public async Task ExportItemsLodAsync(bool exportToClientFolder = true)
		{
			ExportTasks.Clear();
			rbExportToLocalFolder.Checked = !exportToClientFolder;
			rbExportToClient.Checked = exportToClientFolder;

			pMain.Logger(LogTypes.Message, "Exporting: ItemAll.lod file...");
			ExportITEMSAsync("ItemAll");
			await Task.WhenAll(ExportTasks);
			pMain.Logger(LogTypes.Success, "ItemAll.lod has been Exported.");
		}

		public async Task ExportItemStringsLodAsync(bool exportToClientFolder = true)
		{
			ExportTasks.Clear();
			rbExportToLocalFolder.Checked = !exportToClientFolder;
			rbExportToClient.Checked = exportToClientFolder;

			if (!Defs.StringTypes.TryGetValue("ITEM", out Defs.StringType? itemStringType))
				throw new InvalidOperationException("ITEM string export definition is missing.");

			List<string> nations = GetConfiguredStringExportNations();
			if (nations.Count == 0)
				throw new InvalidOperationException("No supported nation is configured for item string export.");

			foreach (string nation in nations)
				ExportStringsAsync(itemStringType, nation);

			await Task.WhenAll(ExportTasks);
			pMain.Logger(LogTypes.Success, "strItem .lod file(s) have been Exported.");
		}

		public async Task ExportOptionsLodAsync(bool exportToClientFolder = true)
		{
			ExportTasks.Clear();
			rbExportToLocalFolder.Checked = !exportToClientFolder;
			rbExportToClient.Checked = exportToClientFolder;

			pMain.Logger(LogTypes.Message, "Exporting: Option.lod file...");
			ExportOPTIONSAsync("Option");
			await Task.WhenAll(ExportTasks);
			pMain.Logger(LogTypes.Success, "Option.lod has been Exported.");
		}

		public async Task ExportOptionStringsLodAsync(bool exportToClientFolder = true)
		{
			ExportTasks.Clear();
			rbExportToLocalFolder.Checked = !exportToClientFolder;
			rbExportToClient.Checked = exportToClientFolder;

			if (!Defs.StringTypes.TryGetValue("OPTION", out Defs.StringType? optionStringType))
				throw new InvalidOperationException("OPTION string export definition is missing.");

			List<string> nations = GetConfiguredStringExportNations();
			if (nations.Count == 0)
				throw new InvalidOperationException("No supported nation is configured for option string export.");

			foreach (string nation in nations)
				ExportStringsAsync(optionStringType, nation);

			await Task.WhenAll(ExportTasks);
			pMain.Logger(LogTypes.Success, "strOption .lod file(s) have been Exported.");
		}

		public async Task ExportTitlesLodAsync(bool exportToClientFolder = true)
		{
			ExportTasks.Clear();
			rbExportToLocalFolder.Checked = !exportToClientFolder;
			rbExportToClient.Checked = exportToClientFolder;

			pMain.Logger(LogTypes.Message, "Exporting: titletool.lod file...");
			ExportTITLESAsync("titletool");
			await Task.WhenAll(ExportTasks);
			pMain.Logger(LogTypes.Success, "titletool.lod has been Exported.");
		}

		private List<string> GetConfiguredStringExportNations()
		{
			HashSet<string> nations = new();

			if (pMain.pSettings.NationSupported != null)
			{
				foreach (string nation in pMain.pSettings.NationSupported)
				{
					string key = NormalizeNationKey(nation);
					if (key.Length > 0)
						nations.Add(key);
				}
			}

			string workLocale = NormalizeNationKey(pMain.pSettings.WorkLocale);
			if (workLocale.Length > 0)
				nations.Add(workLocale);

			return nations.ToList();
		}

		private static string NormalizeNationKey(string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return string.Empty;

			string normalized = value.Trim().ToUpperInvariant();
			if (Defs.NationsCharsetsNPostfix.ContainsKey(normalized))
				return normalized;

			foreach (var nation in Defs.NationsCharsetsNPostfix)
			{
				if (nation.Value.Postfix.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase))
					return nation.Key;
			}

			return string.Empty;
		}

		private int ReadTitleHexColor(DataRow pRow, string strColumnName, int nTitleIndex, int nFallback)
		{
			string strValue = Convert.ToString(pRow[strColumnName], CultureInfo.InvariantCulture)?.Trim() ?? string.Empty;

			if (strValue.StartsWith("#", StringComparison.Ordinal))
				strValue = strValue[1..];

			if (strValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				strValue = strValue[2..];

			if (strValue.Length == 0)
			{
				pMain.Logger(LogTypes.Warning, $"Exporter > titletool.lod: title {nTitleIndex} has empty {strColumnName}; using {nFallback:X8}.");
				return nFallback;
			}

			if (int.TryParse(strValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int nColor))
				return nColor;

			pMain.Logger(LogTypes.Warning, $"Exporter > titletool.lod: title {nTitleIndex} has invalid {strColumnName}='{strValue}'; using {nFallback:X8}.");
			return nFallback;
		}

		private void Exporter_Load(object sender, EventArgs e)
		{
			bool bState, bAllChecked = true;

			clbDataFilesSelector.ColumnWidth = 150;

			clbNationsSelector.BeginUpdate();

			int i = 0;
			foreach (var pKey in Defs.NationsCharsetsNPostfix)
			{
				clbNationsSelector.Items.Add(pKey.Key);

				bState = bool.Parse(pMain.pSettings.Exporter[pKey.Key]);

				clbNationsSelector.SetItemChecked(i, bState);

				if (bAllChecked != false && bState == false)
					bAllChecked = bState;

				i++;
			}

			btnCheckNationsSelector.Text = (bAllChecked ? "Uncheck All" : "Check All");

			clbNationsSelector.EndUpdate();
			/****************************************/
			i = 0;
			bAllChecked = true;

			clbStringsFilesSelector.BeginUpdate();

			foreach(var StringTypeData in Defs.StringTypes)
			{
				clbStringsFilesSelector.Items.Add(StringTypeData.Key);

				bState = bool.Parse(pMain.pSettings.Exporter[StringTypeData.Key]);

				clbStringsFilesSelector.SetItemChecked(i, bState);

				if (bAllChecked != false && bState == false)
					bAllChecked = bState;

				i++;
			}

			btnCheckStringsFilesSelector.Text = (bAllChecked ? "Uncheck All" : "Check All");

			clbStringsFilesSelector.EndUpdate();
			/****************************************/
			bAllChecked = true;

			clbDataFilesSelector.BeginUpdate();

			for (i = 0; i < DataLodTypes.Count; i++)
			{
				clbDataFilesSelector.Items.Add(DataLodTypes[i][0]);

				bState = bool.Parse(pMain.pSettings.Exporter[DataLodTypes[i][0]]);

				clbDataFilesSelector.SetItemChecked(i, bState);

				if (bAllChecked != false && bState == false)
					bAllChecked = bState;
			}

			btnCheckDataFilesSelector.Text = (bAllChecked ? "Uncheck All" : "Check All");

			clbDataFilesSelector.EndUpdate();
			/****************************************/
			if (pMain.pSettings.ExportToLocal)
				rbExportToLocalFolder.Checked = true;
			else
				rbExportToClient.Checked = true;
		}

		private void Exporter_FormClosing(object sender, FormClosingEventArgs e)
		{
			FileIniDataParser pParser = new();
			IniData pData = pParser.ReadFile(pMain.pSettings.SettingsFile);
			string strState;
			int i = 0;

			foreach (var pKey in Defs.NationsCharsetsNPostfix)
			{
				strState = "false";

				if (clbNationsSelector.GetItemChecked(i))
					strState = "true";

				pMain.pSettings.Exporter[pKey.Key] = strState;

				pData["Exporter"][pKey.Key] = strState;

				i++;
			}
			/****************************************/
			i = 0;
			foreach (var StringTypeData in Defs.StringTypes)
			{
				strState = "false";

				if (clbStringsFilesSelector.GetItemChecked(i))
					strState = "true";

				pMain.pSettings.Exporter[StringTypeData.Key] = strState;

				pData["Exporter"][StringTypeData.Key] = strState;
				
				i++;
			}
			/****************************************/
			for (i = 0; i < DataLodTypes.Count; ++i)
			{
				strState = "false";

				if (clbDataFilesSelector.GetItemChecked(i))
					strState = "true";

				pMain.pSettings.Exporter[DataLodTypes[i][0]] = strState;

				pData["Exporter"][DataLodTypes[i][0]] = strState;
			}
			/****************************************/
			strState = "false";

			if (rbExportToLocalFolder.Checked)
				strState = "true";

			pMain.pSettings.ExportToLocal = rbExportToLocalFolder.Checked;

			pData["Settings"]["ExportToLocal"] = strState;

			pParser.WriteFile(pMain.pSettings.SettingsFile, pData);
		}
		/****************************************/
		private void WriteLengthNText(BinaryWriter Stream, string strText, int nTextLength, Type? Type)
		{
			if (nTextLength != 0)
			{
				byte[] strBytes = new byte[nTextLength], strStringBytes = Encoding.Default.GetBytes(strText);

				Array.Copy(strStringBytes, strBytes, Math.Min(strStringBytes.Length, strBytes.Length));

				Stream.Write(strBytes, 0, nTextLength);	// Write Text
			}
			else
			{
				byte[] strBytes = Encoding.Default.GetBytes(strText);

				if (Type != null && Type == typeof(int))
				{
					nTextLength = strText.Length;

					Stream.Write(nTextLength);	// Write Text Length
					Stream.Write(strBytes, 0, nTextLength);	// Write Text
				}
				else if (Type != null && Type == typeof(short))
				{
					short sTextLength = (short)strText.Length;

					Stream.Write(sTextLength);  // Write Text Length
					Stream.Write(strBytes, 0, sTextLength); // Write Text
				}
			}
		}
		/****************************************/
		private void ReplaceFileWithTemp(string strTempFilePath, string strFilePath)
		{
			if (File.Exists(strFilePath))
			{
				string strBackupFilePath = strFilePath + ".bak";

				if (File.Exists(strBackupFilePath))
					File.Delete(strBackupFilePath);

				File.Replace(strTempFilePath, strFilePath, strBackupFilePath);

				if (File.Exists(strBackupFilePath))
					File.Delete(strBackupFilePath);
			}
			else
			{
				File.Move(strTempFilePath, strFilePath);
			}
		}
		/****************************************/
#if USE_ORIGINAL_SKILL_CALCULATESUM_AND_END_TAG
		private void CalculateSum(string strFilePath)
		{
			using (FileStream Stream = new(strFilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				int nFileSum = 0;
				byte[] sBuffer = new byte[4];
				long lFileSize = Stream.Length;

				for (long i = 0; i < lFileSize - 1; i += Defs.JUMPSIZE)
				{
					Stream.Seek(i, SeekOrigin.Begin);

					Stream.Read(sBuffer, 0, 4);

					nFileSum += BitConverter.ToInt32(sBuffer, 0);
				}

				Stream.Seek(0, SeekOrigin.End);

				Stream.Write(BitConverter.GetBytes(nFileSum), 0, 4);    // Write File Checksum
			}
		}
#endif
		/****************************************/
		private static SMCData ParseSMC(string strFilePath)
		{
			SMCData SMCFileData = new();
			string? strLine;

			using (StreamReader stream = new(strFilePath))
			{
				while ((strLine = stream.ReadLine()) != null)
				{
					if (strLine.Contains("NAME \""))
					{
						SMCFileData.Name = Regex.Match(strLine, @"NAME\s+""([^""]+)""").Groups[1].Value;
					}
					else if (strLine.Contains("MESH"))
					{
						SMCFileData.MeshPaths.Add(strLine.Replace("MESH       \tTFNM \"", "").Replace(" ", "").Replace("\";", ""));
					}
					else if (strLine.Contains("TFNM") && !strLine.Contains("ANIMSET") && !strLine.Contains("SKELETON"))
					{
						string[] strTextureData = strLine.Split("TFNM", StringSplitOptions.None);

						SMCFileData.TextureNames.Add(Regex.Replace(strTextureData[0], @"^\s+""", "").Replace("\"", "").Replace("\t", ""));
						SMCFileData.TexturePaths.Add(strTextureData[1].Replace(" \"", "").Replace("\"", "").Replace("\t", "").Replace(";", ""));
					}
				}
			}

			return SMCFileData;
		}
		/****************************************/
		private void CheckUnCheckAll(string strObject, bool bChecked)
		{
			CheckedListBox Obj = (CheckedListBox)Controls.Find(strObject, true)[0];

			for (int i = 0; i < Obj.Items.Count; ++i)
				Obj.SetItemChecked(i, bChecked);

			string strAction = "Check All";

			if (bChecked)
				strAction = "Uncheck All";

			if (strObject == "clbNationsSelector")
				btnCheckNationsSelector.Text = strAction;
			else if(strObject == "clbStringsFilesSelector")
				btnCheckStringsFilesSelector.Text = strAction;
			else if (strObject == "clbDataFilesSelector")
				btnCheckDataFilesSelector.Text = strAction;
		}

		private void btnCheckNationsSelector_Click(object sender, EventArgs e) { CheckUnCheckAll("clbNationsSelector", (sender as Button)?.Text.ToString() == "Check All" ? true : false); }
		private void btnCheckStringsFilesSelector_Click(object sender, EventArgs e) { CheckUnCheckAll("clbStringsFilesSelector", (sender as Button)?.Text.ToString() == "Check All" ? true : false); }
		private void btnCheckDataFilesSelector_Click(object sender, EventArgs e) { CheckUnCheckAll("clbDataFilesSelector", (sender as Button)?.Text.ToString() == "Check All" ? true : false); }
		/****************************************/
		private async void btnExport_ClickAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Exporting, Please Wait...", true);
#if DEBUG
			lTotalElapsedTime = 0;
#endif
			ExportTasks.Clear();

			int i = 0, j;
			MethodInfo? pFunc;
			Type type = typeof(Exporter);
			string strFileName, strLodType;

			// Exports Strings Files
			foreach (var pKey in Defs.NationsCharsetsNPostfix)
			{
				if (clbNationsSelector.GetItemChecked(i))
				{
					j = 0;
					foreach (var StringTypeData in Defs.StringTypes)
					{
						if (clbStringsFilesSelector.GetItemChecked(j))
							ExportStringsAsync(Defs.StringTypes[StringTypeData.Key], pKey.Key);

						j++;
					}
				}

				i++;
			}

			// Export Data Files
			for (i = 0; i < DataLodTypes.Count; ++i)
			{
				if (clbDataFilesSelector.GetItemChecked(i))
				{
					strLodType = DataLodTypes[i][0];
					strFileName = DataLodTypes[i][1];
					pFunc = type.GetMethod($"Export{DataLodTypes[i][0]}Async", BindingFlags.NonPublic | BindingFlags.Instance);

					if (pFunc != null)
					{
						switch (strLodType)
						{
#if EXPORT_LACARETTES_FILES_FOR_EACH_LANGUAGE
							case "LACARETTES":
#endif
							case "CATALOGS":
								{
									j = 0;
									foreach (var pKey in Defs.NationsCharsetsNPostfix)
									{
										if (clbNationsSelector.GetItemChecked(j))
										{
											pMain.Logger(LogTypes.Message, $"Exporting: {strFileName}_{pKey.Key}.lod file...");

											pFunc.Invoke(this, new object[] { strFileName, pKey.Key.ToLower() });
										}

										j++;
									}
								}
								break;
							default:
								{
									if (strLodType == "SHOPS")
									{
										pMain.Logger(LogTypes.Warning, $"Exporting: {strFileName}.lod (With USA for a_national (t_itemshop) as argument) file...");

										pFunc.Invoke(this, new object[] { strFileName, "USA" });	// Hardcode!
									}
									else
									{
										pMain.Logger(LogTypes.Message, $"Exporting: {strFileName}.lod file...");

										pFunc.Invoke(this, new object[] { strFileName });
									}
								}
								break;
						}
					}
				}
			}

			btnExport.Enabled = false;

			await Task.WhenAll(ExportTasks);
#if DEBUG
			long lFiltered = lTotalElapsedTime / 1000;
			
			pMain.Logger(LogTypes.Success, $"Export all Files Took: {(lFiltered / 60)}:{(lFiltered % 60)}.{lTotalElapsedTime}ms.");
#else
			pMain.Logger(LogTypes.Success, "All Files has been Exported.");
#endif
			btnExport.Enabled = true;

			pProgressDialog.Close();
		}
		
		// String Export
		private void ExportStringsAsync(Defs.StringType StringTypeData, string strNation)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int i = 0;
				List<string> listColumns = new();
				string strFilePath = pMain.pSettings.ClientPath + "\\";
				Defs.NationCharSetNPostfix pNationCharsetNPostfix = Defs.NationsCharsetsNPostfix[strNation];

				if (rbExportToLocalFolder.Checked)
					strFilePath = "";

				strFilePath = strFilePath + $"Local\\{pNationCharsetNPostfix.Postfix}\\String";

				if (!Directory.Exists(strFilePath))
					Directory.CreateDirectory(strFilePath);

				strFilePath += $"\\{StringTypeData.FileName}_{pNationCharsetNPostfix.Postfix}.lod";

				pMain.Logger(LogTypes.Message, $"Exporting: {StringTypeData.FileName}_{pNationCharsetNPostfix.Postfix}.lod file...");

				foreach (string strColumnName in StringTypeData.Columns) // Iterate over each column
				{
					if (i == 0) // The first one generally is the index, don't need add postfix to it.
						listColumns.Add(strColumnName);
					else    // The rest of columns are strings, iterate over each nation checked and add the postfix of each nation to each string column, to do a big request with all required strings.
						listColumns.Add(strColumnName + "_" + strNation.ToLower());

					i++;
				}

				DataTable? pTable = pMain.QuerySelect(pNationCharsetNPostfix.Charset, $"{StringTypeData.Clause} {string.Join(", ", listColumns)} FROM {pMain.pSettings.DBData}.{StringTypeData.TableName} {StringTypeData.Condition} ORDER BY {StringTypeData.Columns[0]}");
				if (pTable != null)
				{
					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);
						Stream.Write(Convert.ToInt32(pTable.AsEnumerable().LastOrDefault()?[StringTypeData.Columns[0]]));	// Write Last Row ID

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow[StringTypeData.Columns[0]])); // Write Row ID

							foreach (string strColumnName in listColumns.Skip(1))
								WriteLengthNText(Stream, pRow[strColumnName].ToString() ?? string.Empty, 0, typeof(int));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}

		// Data Export Functions
		private void ExportITEMSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strQuery = $"SELECT a_index, a_job_flag, a_weight, a_fame, a_level, a_flag, a_wearing, a_type_idx, a_subtype_idx, a_need_item0, a_need_item1, a_need_item2, a_need_item3, a_need_item4, a_need_item5, a_need_item6, a_need_item7, a_need_item8, a_need_item9, a_need_item_count0, a_need_item_count1, a_need_item_count2, a_need_item_count3, a_need_item_count4, a_need_item_count5, a_need_item_count6, a_need_item_count7, a_need_item_count8, a_need_item_count9, a_need_sskill, a_need_sskill_level, a_need_sskill2, a_need_sskill_level2, a_texture_id, a_texture_row, a_texture_col, a_num_0, a_num_1, a_num_2, a_num_3, a_price, a_set_0, a_set_1, a_set_2, a_set_3, a_set_4, a_file_smc, a_effect_name, a_attack_effect_name, a_damage_effect_name, a_rare_index_0, a_rare_prob_0, a_rare_index_0, a_rare_index_1, a_rare_index_2, a_rare_index_3, a_rare_index_4, a_rare_index_5, a_rare_index_6, a_rare_index_7, a_rare_index_8, a_rare_index_9, a_rare_prob_0, a_rare_prob_1, a_rare_prob_2, a_rare_prob_3, a_rare_prob_4, a_rare_prob_5, a_rare_prob_6, a_rare_prob_7, a_rare_prob_8, a_rare_prob_9, a_rvr_value, a_rvr_grade, a_castle_war FROM {pMain.pSettings.DBData}.t_item WHERE a_zone_flag=1023 AND a_enable=1 ORDER by a_index;";
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";
				byte sCastleWar;
				int nItemID, nFortuneIndex;
				DataTable? pTableItems, pTableFortune;

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				pTableItems = pMain.QuerySelect("utf8", strQuery);  // Hardcode!
				if (pTableItems != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTableItems.Rows.Count);

						foreach (DataRow pRow in pTableItems.Rows)
						{
							nItemID = Convert.ToInt32(pRow["a_index"]);

							Stream.Write(nItemID);
							Stream.Write(Convert.ToInt32(pRow["a_job_flag"]));
							Stream.Write(Convert.ToInt32(pRow["a_weight"]));
							Stream.Write(Convert.ToInt32(pRow["a_fame"]));
							Stream.Write(Convert.ToInt32(pRow["a_level"]));
							Stream.Write(Convert.ToInt64(pRow["a_flag"]));
							Stream.Write(Convert.ToInt32(pRow["a_wearing"]));
							Stream.Write(Convert.ToInt32(pRow["a_type_idx"]));
							Stream.Write(Convert.ToInt32(pRow["a_subtype_idx"]));

							for (int i = 0; i < Defs.MAX_MAKE_ITEM_MATERIAL; i++)
								Stream.Write(Convert.ToInt32(pRow["a_need_item" + i]));

							for (int i = 0; i < Defs.MAX_MAKE_ITEM_MATERIAL; i++)
								Stream.Write(Convert.ToInt32(pRow["a_need_item_count" + i]));

							Stream.Write(Convert.ToInt32(pRow["a_need_sskill"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_sskill_level"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_sskill2"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_sskill_level2"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_id"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_row"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_col"]));

							for (int i = 0; i < Defs.MAX_MAKE_ITEM_NUM; i++)
								Stream.Write(Convert.ToInt32(pRow["a_num_" + i]));

							Stream.Write(Convert.ToInt32(pRow["a_price"]));

							for (int i = 0; i < Defs.MAX_MAKE_ITEM_SET; i++)
								Stream.Write(Convert.ToInt32(pRow["a_set_" + i]));

							WriteLengthNText(Stream, pRow["a_file_smc"].ToString() ?? string.Empty, Defs.DEF_SMC_DEFAULT_LENGTH, typeof(int));
							WriteLengthNText(Stream, pRow["a_effect_name"].ToString() ?? string.Empty, Defs.DEF_EFFECT_DEFAULT_LENGTH, typeof(int));
							WriteLengthNText(Stream, pRow["a_attack_effect_name"].ToString() ?? string.Empty, Defs.DEF_EFFECT_DEFAULT_LENGTH, typeof(int));
							WriteLengthNText(Stream, pRow["a_damage_effect_name"].ToString() ?? string.Empty, Defs.DEF_EFFECT_DEFAULT_LENGTH, typeof(int));

							Stream.Write(Convert.ToInt32(pRow["a_rare_index_0"]));
							Stream.Write(Convert.ToInt32(pRow["a_rare_prob_0"]));

							for (int i = 0; i < Defs.DEF_MAX_ORIGIN_OPTION; i++)
								Stream.Write(Convert.ToInt32(pRow["a_rare_index_" + i]));

							for (int i = 0; i < Defs.DEF_MAX_ORIGIN_OPTION; i++)
								Stream.Write(Convert.ToInt32(pRow["a_rare_prob_" + i]));

							Stream.Write(Convert.ToInt32(pRow["a_rvr_value"]));

							Stream.Write(Convert.ToInt32(pRow["a_rvr_grade"]));

							sCastleWar = Convert.ToByte(pRow["a_castle_war"]);
							nFortuneIndex = 0;

							pTableFortune = pMain.QuerySelect("utf8", $"SELECT COUNT(*) FROM {pMain.pSettings.DBData}.t_fortune_data WHERE a_item_idx={nItemID} ORDER BY a_skill_index, a_skill_level LIMIT 1;", (pMain.pSettings.LogVerbose < 2 ? false : true));  // Hardcode!
							if (pTableFortune != null)
							{
								if (Convert.ToInt32(pTableFortune.Rows[0]["COUNT(*)"]) > 0)
									nFortuneIndex = 1;

								pTableFortune.Dispose();
							}
							else
							{
								pMain.Logger(LogTypes.Error, $"Exporter > Error while exporting: {strFileName}, Missing Fortune Data for Item: {nItemID}.");
							}

							Stream.Write(nFortuneIndex);    // Write Fortune "ID"
							Stream.Write(sCastleWar);
						}
					}

					pTableItems.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportMOONSTONESAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				DataRow[] pRow;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_type, a_giftindex FROM {pMain.pSettings.DBData}.t_moonstone_reward WHERE a_type IN({string.Join(", ", Defs.MoonStonesNamesNIDS.Values)});");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						foreach (var MoonstoneData in Defs.MoonStonesNamesNIDS)
						{
							pRow = pTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_type"]) == MoonstoneData.Value).ToArray();

							Stream.Write(pRow.Count());

							foreach (DataRow pRowData in pRow)
								Stream.Write(Convert.ToInt32(pRowData["a_giftindex"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
#if EXPORT_LACARETTES_FILES_FOR_EACH_LANGUAGE
		private void ExportLACARETTESAsync(string strFileName, string strNation)
#else
		private void ExportLACARETTESAsync(string strFileName)
#endif
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				// NOTE: The higher the number, the more likely you are to win the reward.
				int[] LacaretteNumCounts = { 1, 1, 2, 2, 2, 2, 3, 3, 4, 4 };    // Hardcode! (How many times is the prize in Lacarette)

				List<List<int>> LacaretteNums = [	// Hardcode! (Where is the prize in Lacarette each time)
					[1],
					[13],
					[3, 18],
					[9, 21],
					[11, 23],
					[5, 16],
					[7, 12, 20],
					[6, 14, 24],
					[4, 10, 17, 22],
					[2, 8, 15, 19]
				];

				int i, nCoinCount, nPrizeCount;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";
				string[] strCoinRequiredID, strCoinAddCount, strReward;
#if EXPORT_LACARETTES_FILES_FOR_EACH_LANGUAGE
				NationCharSetNPostfix pNationCharsetNPostfix = Defs.NationsCharsetsNPostfix[strNation.ToUpper()];
#else
				string strNation = "usa";	// Hardcode!
				Defs.NationCharSetNPostfix pNationCharsetNPostfix = Defs.NationsCharsetsNPostfix[strNation.ToUpper()];
#endif
				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect(pNationCharsetNPostfix.Charset, $"SELECT a_index, a_name_{strNation}, a_useCoinCount, a_coinIndex, a_coinDefCount, a_coinAddCount, a_giveItem_1, a_giveItem_2, a_giveItem_3, a_giveItem_4, a_giveItem_5, a_giveItem_6, a_giveItem_7, a_giveItem_8, a_giveItem_9, a_giveItem_10 FROM {pMain.pSettings.DBData}.t_lacarette WHERE a_enable=1 ORDER BY a_index;");
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);
#if EXPORT_LACARETTES_FILES_FOR_EACH_LANGUAGE
					strFilePath += $"\\{strFileName}_{pNationCharsetNPostfix.Postfix}.lod";
#else
					strFilePath += $"\\{strFileName}.lod";
#endif
					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));

							WriteLengthNText(Stream, pRow["a_name_" + strNation].ToString() ?? string.Empty, 0, typeof(int));

							nCoinCount = Convert.ToInt32(pRow["a_useCoinCount"]);

							Stream.Write(nCoinCount);

							strCoinRequiredID = (pRow["a_coinDefCount"].ToString() ?? string.Empty).Split(' ');
							strCoinAddCount = (pRow["a_coinAddCount"].ToString() ?? string.Empty).Split(' ');
							i = 0;

							foreach (string strCoinID in (pRow["a_coinIndex"].ToString() ?? string.Empty).Split(' '))
							{
								Stream.Write(Convert.ToInt32(strCoinID));
								Stream.Write(Convert.ToInt32(strCoinRequiredID[i]));
								Stream.Write(Convert.ToByte(strCoinAddCount[i]));

								i++;
							}

							for (int j = 1; j <= Defs.LACARETTE_GIFTDATA_TOTAL; j++)
							{
								strReward = (pRow["a_giveItem_" + j].ToString() ?? string.Empty).Split(' ');

								Stream.Write(Convert.ToInt32(strReward[0]));	// Write Reward ID
								Stream.Write(Convert.ToInt64(strReward[1]));	// Write Reward Amount

								nPrizeCount = LacaretteNumCounts[j - 1];

								Stream.Write(Convert.ToInt32(nPrizeCount));	// Write Prize Count

								for (int k = 0; k < nPrizeCount; k++)
									Stream.Write(Convert.ToByte(LacaretteNums[j - 1][k]));	// Write Prize Position
							}
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportCATALOGSAsync(string strFileName, string strNation)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nItemID;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";
				Defs.NationCharSetNPostfix pNationCharsetNPostfix = Defs.NationsCharsetsNPostfix[strNation.ToUpper()];
				DataTable? pTableCatalog, pTableCTItem;
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
				string strQuery = $"SELECT a_ctid, a_category, a_cash, a_mileage, a_flag, a_enable, a_ctname_{strNation}, a_ctdesc_{strNation}, a_icon FROM {pMain.pSettings.DBData}.t_catalog WHERE a_enable=1 ORDER BY a_ctid ASC;";
#else
				string strQuery = $"SELECT a_ctid, a_category, a_cash, a_mileage, a_flag, a_enable, a_ctname_{strNation}, a_ctdesc_{strNation}, a_icon FROM {pMain.pSettings.DBData}.t_catalog WHERE a_enable=1 ORDER BY a_dummy DESC, a_ctid ASC;";
#endif
				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				pTableCatalog = pMain.QuerySelect(pNationCharsetNPostfix.Charset, strQuery);
				if (pTableCatalog != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName} _{strNation.ToLower()}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTableCatalog.Rows.Count);

						foreach (DataRow pRow in pTableCatalog.Rows)
						{
							nItemID = Convert.ToInt32(pRow["a_ctid"]);

							Stream.Write(nItemID);
							Stream.Write(Convert.ToInt32(pRow["a_category"]));
							Stream.Write(Convert.ToInt32(pRow["a_cash"]));
							Stream.Write(Convert.ToInt32(pRow["a_mileage"]));
							Stream.Write(Convert.ToInt32(pRow["a_flag"]));
							Stream.Write(Convert.ToByte(pRow["a_enable"]));

							WriteLengthNText(Stream, pRow["a_ctname_" + strNation].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pRow["a_ctdesc_" + strNation].ToString() ?? string.Empty, 0, typeof(int));

							pTableCTItem = pMain.QuerySelect("utf8", $"SELECT a_item_idx, a_item_flag, a_item_plus, a_item_option, a_item_num FROM {pMain.pSettings.DBData}.t_ct_item WHERE a_ctid={nItemID} ORDER BY a_index;", (pMain.pSettings.LogVerbose < 2 ? false : true));   // Hardcode!
							if (pTableCTItem != null)
							{
								Stream.Write(pTableCTItem.Rows.Count);

								foreach (DataRow pRowItem in pTableCTItem.Rows)
								{
									Stream.Write(Convert.ToInt32(pRowItem["a_item_idx"]));
									Stream.Write(Convert.ToInt32(pRowItem["a_item_flag"]));
									Stream.Write(Convert.ToInt32(pRowItem["a_item_plus"]));
									Stream.Write(Convert.ToInt32(pRowItem["a_item_option"]));
									Stream.Write(Convert.ToInt32(pRowItem["a_item_num"]));
								}

								pTableCTItem.Dispose();
							}
							else
							{
								pMain.Logger(LogTypes.Error, $"Exporter > Error while exporting: {strFileName}_{strNation.ToLower()}, Missing Catalog Items for Catalog: {nItemID}.");
							}

							Stream.Write(Convert.ToInt32(pRow["a_icon"]));
						}
					}

					pTableCatalog.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportSMCAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int i = 0, j, k, nDiff, nDiffCount, nItemID, nExpectedItemID = 1;
				string strSMCFilePath, strFilePath = pMain.pSettings.ClientPath + "\\Data";
				StringBuilder strBuilder = new();

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_file_smc FROM {pMain.pSettings.DBData}.t_item ORDER BY a_index ASC");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);
						Stream.Write(0);    // Write first Item null?
						Stream.Write(0);    // Write second Item null?

						foreach (DataRow pRow in pTable.Rows)
						{
							nItemID = Convert.ToInt32(pRow["a_index"]);

							if (i > 1 && nItemID != nExpectedItemID)
							{
								nDiffCount = 0;
								nDiff = nItemID - nExpectedItemID;

								while (nDiffCount < nDiff)
								{
									Stream.Write(0);    // Write Item ID null

									nDiffCount++;
								}
							}

							strSMCFilePath = pMain.pSettings.ClientPath + "\\" + pRow["a_file_smc"];

							if (File.Exists(strSMCFilePath))
							{
								SMCData SMCDataInfo = ParseSMC(strSMCFilePath);

								Stream.Write(nItemID + 1);  // Write Item ID

								WriteLengthNText(Stream, SMCDataInfo.Name, 0, typeof(short));

								Stream.Write(SMCDataInfo.MeshPaths.Count);

								j = 0;
								foreach (string strMeshPath in SMCDataInfo.MeshPaths)
								{
									Stream.Write(j + 1);    // Write Model ID

									WriteLengthNText(Stream, strMeshPath, 0, typeof(short));

									Stream.Write(SMCDataInfo.TexturePaths.Count);

									k = 0;
									foreach (string strTexturePath in SMCDataInfo.TexturePaths)
									{
										WriteLengthNText(Stream, SMCDataInfo.TextureNames[k], 0, typeof(short));
										WriteLengthNText(Stream, strTexturePath, 0, typeof(short));

										k++;
									}

									j++;
								}
							}
							else
							{
								strBuilder.Append($"\nItem ID: {nItemID} > {strSMCFilePath} Not exist!");

								Stream.Write(0);    // Write Item ID null
							}

							nExpectedItemID = nItemID + 1;
							i++;
						}
					}

					pMain.Logger(LogTypes.Warning, "SMC Exporter > " + strBuilder.ToString());

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportMONSTERCOMBOAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_nas, a_texture_id, a_texture_row, a_texture_col, a_point FROM {pMain.pSettings.DBData}.t_missioncase WHERE a_enable=1 ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_nas"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_id"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_row"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_col"]));
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							Stream.Write(0);
#endif
							Stream.Write(Convert.ToInt32(pRow["a_point"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportITEMEXCHANGESAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_npc_index, result_itemIndex, result_itemCount, source_itemIndex0, source_itemCount0, source_itemIndex1, source_itemCount1, source_itemIndex2, source_itemCount2, source_itemIndex3, source_itemCount3, source_itemIndex4, source_itemCount4 FROM {pMain.pSettings.DBData}.t_item_exchange WHERE a_enable=1 ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);
						Stream.Write(Convert.ToInt32(pTable.AsEnumerable().LastOrDefault()?["a_index"]));    // Write Last Row ID

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_npc_index"]));
							Stream.Write(Convert.ToInt32(pRow["result_itemIndex"]));
							Stream.Write(Convert.ToInt32(pRow["result_itemCount"]));

							for (int i = 0; i < Defs.DEF_CONDITION_ITEM_MAX; i++)
							{
								Stream.Write(Convert.ToInt32(pRow["source_itemIndex" + i]));
								Stream.Write(Convert.ToInt32(pRow["source_itemCount" + i]));
							}
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportSPECIALSKILLSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_type, a_level0_need_level, a_level0_need_sp, a_level1_need_level, a_level1_need_sp, a_level2_need_level, a_level2_need_sp, a_level3_need_level, a_level3_need_sp, a_level4_need_level, a_level4_need_sp, a_need_sskill, a_need_sskill_level, a_max_level, a_preference, a_texture_id, a_texture_row, a_texture_col FROM {pMain.pSettings.DBData}.t_special_skill WHERE a_enable=1 ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_type"]));
							Stream.Write(Convert.ToInt32(pRow["a_max_level"]));
							Stream.Write(Convert.ToInt32(pRow["a_preference"]));

							for (int i = 0; i < Defs.SSKILL_MAX_LEVEL; i++)
								Stream.Write(Convert.ToInt32(pRow[$"a_level{i}_need_level"]));   // Write Needed Level

							for (int i = 0; i < Defs.SSKILL_MAX_LEVEL; i++)
								Stream.Write(Convert.ToInt32(pRow[$"a_level{i}_need_sp"]));   // Write Needed Skill Points

							Stream.Write(Convert.ToInt32(pRow["a_need_sskill"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_sskill_level"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_id"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_row"]));
							Stream.Write(Convert.ToInt32(pRow["a_texture_col"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportSKILLSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				byte nMagicLevel;
				int nSkillID, nMagicID, nDamageType;
				DataTable? pTableSkills, pTableSkillLevels, pCombinedTable;
				sbyte sbAttackSubType, sbAttackPower, sbDefenseSubType, sbDefensePower;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				pTableSkills = pMain.QuerySelect("utf8", $"SELECT a_index, a_job, a_job2, a_apet_index, a_type, a_flag, a_sorcerer_flag, a_maxLevel, a_appRange, a_fireRange, " +
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
					$"a_fireRange2, " +
#endif
					$"a_targetType, a_useState, a_useWeaponType0, a_useWeaponType1, a_useMagicIndex1, a_useMagicLevel1, a_useMagicIndex2, a_useMagicLevel2, a_useMagicIndex3, a_useMagicLevel3, a_soul_consum, a_appState, a_readyTime, a_stillTime, a_fireTime, a_reuseTime, a_cd_ra, a_cd_re, a_cd_sa, a_cd_fa, a_cd_fe0, a_cd_fe1, a_cd_fe2, a_cd_fot, a_cd_fos, a_cd_ox, a_cd_oz, a_cd_oh, a_cd_oc, a_cd_fdc, a_cd_fd0, a_cd_fd1, a_cd_fd2, a_cd_fd3, a_cd_dd, a_cd_ra2, a_cd_re2, a_cd_sa2, a_cd_fa2, a_cd_fe3, a_cd_fe4, a_cd_fe5, a_cd_fot2, a_cd_fos2, a_cd_ox2, a_cd_oz2, a_cd_oh2, a_cd_oc2, a_cd_fdc2, a_cd_fd4, a_cd_fd5, a_cd_fd6, a_cd_fd7, a_cd_dd2, a_cd_fe_after, a_client_icon_texid, a_client_icon_row, a_client_icon_col FROM {pMain.pSettings.DBData}.t_skill WHERE a_job>=0 ORDER BY a_index;");  // Hardcode!
				if (pTableSkills != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
						Stream.Write(Convert.ToInt32(pTableSkills.AsEnumerable().LastOrDefault()?["a_index"]));  // Write Last Row ID
#else
						Stream.Write(pTableSkills.Rows.Count);
#endif

						foreach (DataRow pSkillRow in pTableSkills.Rows)
						{
							nSkillID = Convert.ToInt32(pSkillRow["a_index"]);

							Stream.Write(nSkillID);
							Stream.Write(Convert.ToInt32(pSkillRow["a_job"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_job2"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_apet_index"]));
							Stream.Write(Convert.ToByte(pSkillRow["a_type"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_flag"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_sorcerer_flag"]));
							Stream.Write(Convert.ToByte(pSkillRow["a_maxLevel"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_appRange"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_fireRange"]));
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							Stream.Write(Convert.ToSingle(pSkillRow["a_fireRange2"]));
#endif
							Stream.Write(Convert.ToByte(pSkillRow["a_targetType"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_useState"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_useWeaponType0"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_useWeaponType1"]));

							for (int i = 1; i <= Defs.DEF_SKILL_MAX_MAGIC; i++)
							{
								Stream.Write(Convert.ToInt32(pSkillRow["a_useMagicIndex" + i]));	// Write Magic ID
								Stream.Write(Convert.ToByte(pSkillRow["a_useMagicLevel" + i]));	// Write Magic Level
							}

							Stream.Write(Convert.ToInt32(pSkillRow["a_soul_consum"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_appState"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_readyTime"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_stillTime"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_fireTime"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_reuseTime"]));

							WriteLengthNText(Stream, pSkillRow["a_cd_ra"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_re"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_sa"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_fa"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_fe0"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_fe1"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_fe2"].ToString() ?? string.Empty, 0, typeof(int));

							Stream.Write(Convert.ToByte(pSkillRow["a_cd_fot"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fos"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_ox"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_oz"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_oh"]));
							Stream.Write(Convert.ToByte(pSkillRow["a_cd_oc"]));
							Stream.Write(Convert.ToByte(pSkillRow["a_cd_fdc"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fd0"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fd1"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fd2"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fd3"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_dd"]));

							WriteLengthNText(Stream, pSkillRow["a_cd_ra2"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_re2"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_sa2"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_fa2"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_fe3"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_fe4"].ToString() ?? string.Empty, 0, typeof(int));
							WriteLengthNText(Stream, pSkillRow["a_cd_fe5"].ToString() ?? string.Empty, 0, typeof(int));

							Stream.Write(Convert.ToByte(pSkillRow["a_cd_fot2"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fos2"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_ox2"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_oz2"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_oh2"]));
							Stream.Write(Convert.ToByte(pSkillRow["a_cd_oc2"]));
							Stream.Write(Convert.ToByte(pSkillRow["a_cd_fdc2"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fd4"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fd5"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fd6"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_fd7"]));
							Stream.Write(Convert.ToSingle(pSkillRow["a_cd_dd2"]));

							WriteLengthNText(Stream, pSkillRow["a_cd_fe_after"].ToString() ?? string.Empty, 0, typeof(int));

							Stream.Write(Convert.ToInt32(pSkillRow["a_client_icon_texid"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_client_icon_row"]));
							Stream.Write(Convert.ToInt32(pSkillRow["a_client_icon_col"]));

							pTableSkillLevels = pMain.QuerySelect("utf8", $"SELECT a_needHP, a_needMP, a_needGP, a_durtime, a_dummypower, a_needItemIndex1, a_needItemCount1, a_needItemIndex2, a_needItemCount2, a_learnLevel, a_learnSP, a_learnSkillIndex1, a_learnSkillLevel1, a_learnSkillIndex2, a_learnSkillLevel2, a_learnSkillIndex3, a_learnSkillLevel3, a_learnItemIndex1, a_learnItemCount1, a_learnItemIndex2, a_learnItemCount2, a_learnItemIndex3, a_learnItemCount3, a_learnstr, a_learndex, a_learnint, a_learncon, a_appMagicIndex1, a_appMagicLevel1, a_appMagicIndex2, a_appMagicLevel2, a_appMagicIndex3, a_appMagicLevel3, a_magicIndex1, a_magicLevel1, a_magicIndex2, a_magicLevel2, a_magicIndex3, a_magicLevel3, a_learnGP, a_targetNum FROM {pMain.pSettings.DBData}.t_skillLevel WHERE a_index={nSkillID} ORDER BY a_level;", (pMain.pSettings.LogVerbose < 2 ? false : true));  // Hardcode!
							if (pTableSkillLevels != null)
							{
								foreach (DataRow pSkillLevelRow in pTableSkillLevels.Rows)
								{
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_needHP"]));
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_needMP"]));
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_needGP"]));
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_durtime"]));
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_dummypower"]));

									for (int i = 1; i <= Defs.DEF_SKILL_LEVEL_NEEDED_ITEM; i++)
									{
										Stream.Write(Convert.ToInt32(pSkillLevelRow["a_needItemIndex" + i]));	// Write Item ID
										Stream.Write(Convert.ToInt32(pSkillLevelRow["a_needItemCount" + i]));	// Write Item Amount
									}

									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learnLevel"]));
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learnSP"]));

									for (int i = 1; i <= Defs.DEF_SKILL_LEVEL_LEARN_SKILL_NEEDED; i++)
									{
										Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learnSkillIndex" + i]));	// Write Learn Skill ID
										Stream.Write(Convert.ToByte(pSkillLevelRow["a_learnSkillLevel" + i]));	// Write Learn Skill Level
									}

									for (int i = 1; i <= Defs.DEF_SKILL_LEVEL_LEARN_ITEM_NEEDED; i++)
									{
										Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learnItemIndex" + i]));	// Write Learn Item ID
										Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learnItemCount" + i]));	// Write Learn Item Count
									}

									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learnstr"]));
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learndex"]));
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learnint"]));
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learncon"]));

									for (int i = 1; i <= Defs.DEF_SKILL_LEVEL_APP_MAGIC; i++)
									{
										Stream.Write(Convert.ToInt32(pSkillLevelRow["a_appMagicIndex" + i]));	// Write App Magic ID
										Stream.Write(Convert.ToByte(pSkillLevelRow["a_appMagicLevel" + i]));	// Write App Magic Level
									}

									sbAttackSubType = 0;
									sbAttackPower = 0;
									sbDefenseSubType = 0;
									sbDefensePower = 0;

									for (int i = 1; i <= Defs.DEF_SKILL_LEVEL_MAGIC; i++)
									{
										nMagicID = Convert.ToInt32(pSkillLevelRow["a_magicIndex" + i]);
										nMagicLevel = Convert.ToByte(pSkillLevelRow["a_magicLevel" + i]);

										Stream.Write(nMagicID);  // Write Magic ID
										Stream.Write(nMagicLevel);  // Write Magic Level

										// NOTE: Ported directly from my Lua code, too dirty, but... who cares.
										/*DataTable? pTableMagic = pMain.QuerySelect("utf8", $"SELECT a_damagetype, a_subtype FROM {pMain.pSettings.DBData}.t_magic WHERE a_type=1 AND a_index={nMagicID};");  // Hardcode!
										if (pTableMagic != null && pTableMagic.Rows.Count > 0)
										{
											nDamageType = Convert.ToInt32(pTableMagic.Rows[0]["a_damagetype"]);

											if (nDamageType == 1)
												nAttackSubType = Convert.ToByte(pTableMagic.Rows[0]["a_subtype"]);
											else if (nDamageType == 2)
												nDefenseSubType = Convert.ToByte(pTableMagic.Rows[0]["a_subtype"]);

											DataTable? pTableMagicLevel = pMain.QuerySelect("utf8", $"SELECT a_power FROM {pMain.pSettings.DBData}.t_magiclevel WHERE a_index={nMagicID} AND a_level={nMagicLevel};");  // Hardcode!
											if (pTableMagicLevel != null && pTableMagicLevel.Rows.Count > 0)
											{
												if (nDamageType == 1)
													nAttackPower = Convert.ToByte(pTableMagicLevel.Rows[0]["a_power"]);
												else if (nDamageType == 2)
													nDefensePower = Convert.ToByte(pTableMagicLevel.Rows[0]["a_power"]);

												pTableMagicLevel.Dispose();
											}

											pTableMagic.Dispose();
										}*/

										pCombinedTable = pMain.QuerySelect("utf8", $"SELECT m.a_damagetype, m.a_subtype, ml.a_power FROM {pMain.pSettings.DBData}.t_magic m LEFT JOIN {pMain.pSettings.DBData}.t_magiclevel ml ON m.a_index=ml.a_index WHERE m.a_type=1 AND m.a_index={nMagicID} AND ml.a_level={nMagicLevel};", (pMain.pSettings.LogVerbose < 2 ? false : true));	// Hardcode!
										if (pCombinedTable != null && pCombinedTable.Rows.Count > 0)
										{
											nDamageType = Convert.ToInt32(pCombinedTable.Rows[0]["a_damagetype"]);

											if (nDamageType == 1)
												sbAttackSubType = Convert.ToSByte(pCombinedTable.Rows[0]["a_subtype"]);
											else if (nDamageType == 2)
												sbDefenseSubType = Convert.ToSByte(pCombinedTable.Rows[0]["a_subtype"]);

											if (nDamageType == 1)
												sbAttackPower = Convert.ToSByte(pCombinedTable.Rows[0]["a_power"]);
											else if (nDamageType == 2)
												sbDefensePower = Convert.ToSByte(pCombinedTable.Rows[0]["a_power"]);

											pCombinedTable.Dispose();
										}
									}

									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_learnGP"]));
									Stream.Write(sbAttackSubType);
									Stream.Write(sbAttackPower);
									Stream.Write(sbDefenseSubType);
									Stream.Write(sbDefensePower);
									Stream.Write(Convert.ToInt32(pSkillLevelRow["a_targetNum"]));
								}

								pTableSkillLevels.Dispose();
							}
							else
							{
								pMain.Logger(LogTypes.Error, $"Exporter > Error while exporting: {strFileName}, Missing Skill Levels for Skill: {nSkillID}.");
							}
						}
#if USE_ORIGINAL_SKILL_CALCULATESUM_AND_END_TAG
						Stream.Write(-9999);	// Write End Tag

						Stream.Close();

						CalculateSum(strFilePath);	// Write File Checksum
#endif
					}

					pTableSkills.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportEQUIPMENTEXCHANGESAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nGroupsIter;
				string[] strItemIDS;
				DataRow[]? pRows;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_type, a_items_idxs FROM {pMain.pSettings.DBData}.t_equipment_exchange WHERE a_enable=1 ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						for (int i = 0; i < Defs.DEF_TRADE_GROUP_TYPES; i++)
						{
							pRows = pTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_type"]) == i).ToArray();

							if (i == 0)
								nGroupsIter = Defs.DEF_TRADE_WEAPON_MAX;
							else
								nGroupsIter = Defs.DEF_TRADE_ARMOR_MAX;

							Stream.Write(nGroupsIter * pRows.Length);  // Write Total Weapon/Armors Rows

							foreach (DataRow rowData in pRows)
							{
								strItemIDS = (rowData["a_items_idxs"].ToString() ?? string.Empty).Split(' ');

								for (int j = 0; j < nGroupsIter; j++)
								{
									Stream.Write(Convert.ToInt32(strItemIDS[j]));   // Write Input Item ID

									if (i != 0) // NOTE: Because Weapon group is more bigger than armors group
										nGroupsIter = Defs.DEF_TRADE_ARMOR_MAX;

									for (int k = 0; k < nGroupsIter; k++)
										Stream.Write(Convert.ToInt32(strItemIDS[k]));   // Write Output Item ID
								}
							}
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportNPCCHANNELSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_channel_flag FROM {pMain.pSettings.DBData}.t_npc WHERE a_enable=1 AND a_channel_flag!=-1 ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_channel_flag"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportACTIONSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_type, a_job, a_iconid, a_iconrow, a_iconcol FROM {pMain.pSettings.DBData}.t_action ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToByte(pRow["a_type"]));
							Stream.Write(Convert.ToInt32(pRow["a_job"]));
							Stream.Write(Convert.ToInt32(pRow["a_iconid"]));
							Stream.Write(Convert.ToInt32(pRow["a_iconrow"]));
							Stream.Write(Convert.ToInt32(pRow["a_iconcol"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportSKILLTREESAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nRowsCount, nSkillID;
				DataRow[]? pRows;
				DataRow? pColumns;
				string strJob2, strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_job, a_job2, a_tree_row, a_tree_col FROM {pMain.pSettings.DBData}.t_skill WHERE a_tree_row!=-1 AND a_tree_col!=-1 ORDER BY a_index;");	// Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						foreach (var ClassData in Defs.CharactersClassNJobsTypes)
						{
							foreach (var Class in ClassData.Value)
							{
								strJob2 = Class.Split(" - ", StringSplitOptions.None)[0];
								pRows = pTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_job"]) == ClassData.Key && row["a_job2"].ToString() == strJob2).OrderBy(row => Convert.ToInt32(row["a_tree_row"])).ThenBy(row => row["a_tree_col"]).ToArray();

								if (pRows.Any())
								{
									nRowsCount = pRows.Max(row => Convert.ToInt32(row["a_tree_row"]));

									Stream.Write(nRowsCount);	// Write Total Rows of Class: X Job: X

									for (int i = 0; i < nRowsCount; i++)
									{
										for (int j = 0; j < Defs.DEF_SKILL_COL; j++)
										{
											nSkillID = 0;   // NOTE: By default put 0, to do not show any skill in that column of the row

											pColumns = pRows.AsEnumerable().Where(row => Convert.ToInt32(row["a_tree_row"]) == (i + 1) && Convert.ToInt32(row["a_tree_col"]) == (j + 1)).FirstOrDefault();
											if (pColumns != null)
												nSkillID = Convert.ToInt32(pColumns["a_index"]);

											Stream.Write(nSkillID);    // Write this row Skill ID
										}
									}
								}
								else    // NOTE: That is for Nightshadow Jobs
								{
									Stream.Write(1);    // Write Dummy Rows of Class: X Job: X

									for (int j = 0; j < Defs.DEF_SKILL_COL; j++)
										Stream.Write(0);    // Write Dummy Skill ID
								}
							}
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportBIGPETSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nAPetID, nParam1, nParam2, nParam3, nParam4;
				DataRow pRow;
				DataTable? pTableAPet, pTableAPetEV, pTableAPetEXP;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
				string strQuery = $"SELECT a_index, a_name, a_type, a_item_idx, a_AISlot, a_mount_1, a_mount_2, a_summon_skill_1, a_summon_skill_2, a_flag, a_smcFileName_1, a_smcFileName_2, a_ani_idle1_1, a_ani_idle1_2, a_ani_idle2_1, a_ani_idle2_2, a_ani_attack1_1, a_ani_attack1_2, a_ani_attack2_1, a_ani_attack2_2, a_ani_damage_1, a_ani_damage_2, a_ani_die_1, a_ani_die_2, a_ani_walk_1, a_ani_walk_2, a_ani_run_1, a_ani_run_2, a_ani_levelup_1, a_ani_levelup_2 FROM {pMain.pSettings.DBData}.t_attack_pet ORDER BY a_index";
#else
				string strQuery = $"SELECT a_index, a_type, a_item_idx, a_AISlot, a_mount_1, a_mount_2, a_summon_skill_1, a_summon_skill_2, a_flag, a_smcFileName_1, a_smcFileName_2, a_ani_idle1_1, a_ani_idle1_2, a_ani_idle2_1, a_ani_idle2_2, a_ani_attack1_1, a_ani_attack1_2, a_ani_attack2_1, a_ani_attack2_2, a_ani_damage_1, a_ani_damage_2, a_ani_die_1, a_ani_die_2, a_ani_walk_1, a_ani_walk_2, a_ani_run_1, a_ani_run_2, a_ani_levelup_1, a_ani_levelup_2 FROM {pMain.pSettings.DBData}.t_attack_pet ORDER BY a_index";
#endif
				pTableAPet = pMain.QuerySelect("utf8", strQuery);  // Hardcode!
				if (pTableAPet != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTableAPet.Rows.Count);

						foreach (DataRow pAPetRow in pTableAPet.Rows)
						{
							nAPetID = Convert.ToInt32(pAPetRow["a_index"]);

							Stream.Write(nAPetID);
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							WriteLengthNText(Stream, pAPetRow["a_name"].ToString(), Defs.DEF_APET_NAME_LENGTH, null);
#endif
							Stream.Write(Convert.ToInt32(pAPetRow["a_type"]));
							Stream.Write(Convert.ToInt32(pAPetRow["a_item_idx"]));
							Stream.Write(Convert.ToInt32(pAPetRow["a_AISlot"]));
							Stream.Write(Convert.ToInt32(pAPetRow["a_mount_1"]));
							Stream.Write(Convert.ToInt32(pAPetRow["a_mount_2"]));
							Stream.Write(Convert.ToInt32(pAPetRow["a_summon_skill_1"]));
							Stream.Write(Convert.ToInt32(pAPetRow["a_summon_skill_2"]));
							Stream.Write(Convert.ToInt32(pAPetRow["a_flag"]));

							WriteLengthNText(Stream, pAPetRow["a_smcFileName_1"].ToString() ?? string.Empty, Defs.DEF_SMCFILE_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_smcFileName_2"].ToString() ?? string.Empty, Defs.DEF_SMCFILE_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_idle1_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_idle1_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_idle2_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_idle2_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_attack1_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_attack1_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_attack2_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_attack2_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_damage_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_damage_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_die_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_die_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_walk_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_walk_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_run_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_run_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_levelup_1"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);
							WriteLengthNText(Stream, pAPetRow["a_ani_levelup_2"].ToString() ?? string.Empty, Defs.DEF_APET_ANI_LENGTH, null);

							pTableAPetEV = pMain.QuerySelect("utf8", $"SELECT a_level, a_stemina, a_faith, a_ev_pet_index FROM {pMain.pSettings.DBData}.t_attack_pet_ev WHERE a_pet_index={nAPetID} ORDER BY a_order;", (pMain.pSettings.LogVerbose < 2 ? false : true));    // Hardcode!
							if (pTableAPetEV != null)
							{
								for (int i = 0; i < Defs.DEF_MAX_EVOLUTION; i++)
								{
									nParam1 = 0;
									nParam2 = 0;
									nParam3 = 0;
									nParam4 = 0;

									if (i < pTableAPetEV.Rows.Count)
									{
										pRow = pTableAPetEV.Rows[i];
										if (pRow != null)
										{
											nParam1 = Convert.ToInt32(pRow["a_level"]);
											nParam2 = Convert.ToInt32(pRow["a_stemina"]);
											nParam3 = Convert.ToInt32(pRow["a_faith"]);
											nParam4 = Convert.ToInt32(pRow["a_ev_pet_index"]);
										}
									}

									Stream.Write(nParam1);
									Stream.Write(nParam2);
									Stream.Write(nParam3);
									Stream.Write(nParam4);
								}

								pTableAPetEV.Dispose();
							}

							pTableAPetEXP = pMain.QuerySelect("utf8", $"SELECT a_max_acc_param1, a_max_acc_param2, a_acc_rate_param1, a_acc_rate_param2 FROM {pMain.pSettings.DBData}.t_attack_pet_exp WHERE a_pet_index={nAPetID};", (pMain.pSettings.LogVerbose < 2 ? false : true));  // Hardcode!
							if (pTableAPetEXP != null)
							{
								for (int i = 0; i < Defs.DEF_MAX_ACCEXP; i++)
								{
									nParam1 = 0;
									nParam2 = 0;
									nParam3 = 0;
									nParam4 = 0;

									if (i < pTableAPetEXP.Rows.Count)
									{
										pRow = pTableAPetEXP.Rows[i];
										if (pRow != null)
										{
											nParam1 = Convert.ToInt32(pRow["a_max_acc_param1"]);
											nParam2 = Convert.ToInt32(pRow["a_max_acc_param2"]);
											nParam3 = Convert.ToInt32(pRow["a_acc_rate_param1"]);
											nParam4 = Convert.ToInt32(pRow["a_acc_rate_param2"]);
										}
									}

									Stream.Write(nParam1);
									Stream.Write(nParam2);
									Stream.Write(nParam3);
									Stream.Write(nParam4);
								}

								pTableAPetEXP.Dispose();
							}
						}
					}

					pTableAPet.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportSHOPSAsync(string strFileName, string strNation)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nNationID;
				DataTable? pShopsTable, pItemsTable;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				switch (strNation)
				{
					case "DEV":
						nNationID = 0;
						break;
					case "THAI":
						nNationID = 4;
						break;
					case "USA":
						nNationID = 9;
						break;
					case "BRZ":
						nNationID = 10;
						break;
					case "RUS":
						nNationID = 17;
						break;
					case "MEX":
						nNationID = 20;
						break;
					case "GER":
					case "SPN":
					case "FRC":
					case "ITA":
					case "PLD":
					case "UK":
						nNationID = 13;
						break;
					default:
						nNationID = -1;
						break;
				}

#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
				string strQuery = $"SELECT a_keeper_idx, a_name, a_sell_rate, a_buy_rate FROM {pMain.pSettings.DBData}.t_shop ORDER by a_keeper_idx";
#else
				string strQuery = $"SELECT a_keeper_idx, a_sell_rate, a_buy_rate FROM {pMain.pSettings.DBData}.t_shop ORDER by a_keeper_idx";
#endif
				pShopsTable = pMain.QuerySelect("utf8", strQuery);  // Hardcode!
				if (pShopsTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
						Stream.Write(Convert.ToInt32(pShopsTable.AsEnumerable().LastOrDefault()?["a_keeper_idx"]));  // Write Last Row ID
#else
						Stream.Write(pShopsTable.Rows.Count);
#endif
						foreach (DataRow pShopsRow in pShopsTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pShopsRow["a_keeper_idx"]));
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							WriteLengthNText(Stream, pShopsRow["a_name"].ToString(), 0, typeof(int));
#endif
							Stream.Write(Convert.ToInt32(pShopsRow["a_sell_rate"]));
							Stream.Write(Convert.ToInt32(pShopsRow["a_buy_rate"]));

#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							strQuery = $"SELECT t1.a_item_idx AS itemindex FROM {pMain.pSettings.DBData}.t_shopitem t1, {pMain.pSettings.DBData}.t_item t2 WHERE NOT (t1.a_national & ({1L << nNationID})) AND t1.a_keeper_idx={pShopsRow["a_keeper_idx"]} AND t1.a_item_idx=t2.a_index ORDER BY t2.a_job_flag, t2.a_level, t2.a_type_idx, t2.a_subtype_idx, t2.a_index;";
#else
							strQuery = $"SELECT t1.a_item_idx AS itemindex, 0 AS itemplus FROM {pMain.pSettings.DBData}.t_shopitem t1, {pMain.pSettings.DBData}.t_item t2 WHERE NOT (t1.a_national & ({1L << nNationID})) AND t1.a_keeper_idx={pShopsRow["a_keeper_idx"]} AND t1.a_item_idx=t2.a_index ORDER BY t2.a_job_flag, t2.a_level, t2.a_type_idx, t2.a_subtype_idx, t2.a_index;";   // Adapted for schemas without t_shopitem.a_item_plus.
#endif
							pItemsTable = pMain.QuerySelect("utf8", strQuery, (pMain.pSettings.LogVerbose < 2 ? false : true));
							if (pItemsTable != null)
							{
								Stream.Write(pItemsTable.Rows.Count);   // Write Total Items Rows

								foreach (DataRow pItemsRow in pItemsTable.Rows)
								{
									Stream.Write(Convert.ToInt32(pItemsRow["itemindex"]));
									Stream.Write(Convert.ToInt32(pItemsRow["itemplus"]));
								}

								pItemsTable.Dispose();
							}
						}
					}

					pShopsTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportQUESTSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_type1, a_type2, a_start_type, a_start_data, a_prize_npc, a_option_prize, a_prequest_num, a_start_npc_zone_num, a_prize_npc_zone_num, a_need_exp, a_need_min_level, a_need_max_level, a_need_job, a_need_rvr_type, a_need_rvr_grade, a_partyscale, a_only_opt_prize, a_need_item0, a_need_item_count0, a_need_item1, a_need_item_count1, a_need_item2, a_need_item_count2, a_need_item3, a_need_item_count3, a_need_item4, a_need_item_count4, a_condition0_type, a_condition0_index, a_condition0_num, a_condition0_data0, a_condition0_data1, a_condition0_data2, a_condition0_data3, a_condition1_type, a_condition1_index, a_condition1_num, a_condition1_data0, a_condition1_data1, a_condition1_data2, a_condition1_data3, a_condition2_type, a_condition2_index, a_condition2_num, a_condition2_data0, a_condition2_data1, a_condition2_data2, a_condition2_data3, a_prize_type0, a_prize_index0, a_prize_data0, a_prize_type1, a_prize_index1, a_prize_data1, a_prize_type2, a_prize_index2, a_prize_data2, a_prize_type3, a_prize_index3, a_prize_data3, a_prize_type4, a_prize_index4, a_prize_data4, a_opt_prize_type0, a_opt_prize_index0, a_opt_prize_data0, a_opt_prize_plus0, a_opt_prize_type1, a_opt_prize_index1, a_opt_prize_data1, a_opt_prize_plus1, a_opt_prize_type2, a_opt_prize_index2, a_opt_prize_data2, a_opt_prize_plus2, a_opt_prize_type3, a_opt_prize_index3, a_opt_prize_data3, a_opt_prize_plus3, a_opt_prize_type4, a_opt_prize_index4, a_opt_prize_data4, a_opt_prize_plus4, a_opt_prize_type5, a_opt_prize_index5, a_opt_prize_data5, a_opt_prize_plus5, a_opt_prize_type6, a_opt_prize_index6, a_opt_prize_data6, a_opt_prize_plus6 FROM {pMain.pSettings.DBData}.t_quest WHERE a_enable=1 ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_type1"]));
							Stream.Write(Convert.ToInt32(pRow["a_type2"]));
							Stream.Write(Convert.ToInt32(pRow["a_start_type"]));
							Stream.Write(Convert.ToInt32(pRow["a_start_data"]));
							Stream.Write(Convert.ToInt32(pRow["a_prize_npc"]));
							Stream.Write(Convert.ToInt32(pRow["a_prequest_num"]));
							Stream.Write(Convert.ToInt32(pRow["a_start_npc_zone_num"]));
							Stream.Write(Convert.ToInt32(pRow["a_prize_npc_zone_num"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_exp"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_min_level"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_max_level"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_job"]));
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							Stream.Write(0);	// Write Need Min Penalty
							Stream.Write(0);	// Write Need Max Penalty
#endif
							for (int i = 0; i < Defs.MAX_MAX_NEED_ITEM; i++)
								Stream.Write(Convert.ToInt32(pRow["a_need_item" + i]));

							for (int i = 0; i < Defs.MAX_MAX_NEED_ITEM; i++)
								Stream.Write(Convert.ToInt32(pRow["a_need_item_count" + i]));

							Stream.Write(Convert.ToInt32(pRow["a_need_rvr_type"]));
							Stream.Write(Convert.ToInt32(pRow["a_need_rvr_grade"]));

							for (int i = 0; i < Defs.QUEST_MAX_CONDITION; i++)
								Stream.Write(Convert.ToInt32(pRow[$"a_condition{i}_type"]));

							for (int i = 0; i < Defs.QUEST_MAX_CONDITION; i++)
								Stream.Write(Convert.ToInt32(pRow[$"a_condition{i}_index"]));

							for (int i = 0; i < Defs.QUEST_MAX_CONDITION; i++)
								Stream.Write(Convert.ToInt32(pRow[$"a_condition{i}_num"]));

							for (int i = 0; i < Defs.QUEST_MAX_CONDITION; i++)
							{
								for (int j = 0; j < Defs.QUEST_MAX_CONDITION_DATA; j++)
									Stream.Write(Convert.ToInt32(pRow[$"a_condition{i}_data" + j]));
							}

							for (int i = 0; i < Defs.QUEST_MAX_PRIZE; i++)
								Stream.Write(Convert.ToInt32(pRow["a_prize_type" + i]));

							for (int i = 0; i < Defs.QUEST_MAX_PRIZE; i++)
								Stream.Write(Convert.ToInt32(pRow["a_prize_index" + i]));

							for (int i = 0; i < Defs.QUEST_MAX_PRIZE; i++)
								Stream.Write(Convert.ToInt64(pRow["a_prize_data" + i]));

							Stream.Write(Convert.ToInt32(pRow["a_option_prize"]));

							for (int i = 0; i < Defs.QUEST_MAX_OPTION_PRIZE; i++)
								Stream.Write(Convert.ToInt32(pRow["a_opt_prize_type" + i]));

							for (int i = 0; i < Defs.QUEST_MAX_OPTION_PRIZE; i++)
								Stream.Write(Convert.ToInt32(pRow["a_opt_prize_index" + i]));

							for (int i = 0; i < Defs.QUEST_MAX_OPTION_PRIZE; i++)
								Stream.Write(Convert.ToInt32(pRow["a_opt_prize_data" + i]));

							for (int i = 0; i < Defs.QUEST_MAX_OPTION_PRIZE; i++)
								Stream.Write(Convert.ToInt32(pRow["a_opt_prize_plus" + i]));

							Stream.Write(Convert.ToInt32(pRow["a_partyscale"]));
							Stream.Write(Convert.ToInt32(pRow["a_only_opt_prize"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportRAREOPTIONSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
				string strQuery = $"SELECT a_index, a_grade, a_type, a_attack, a_defense, a_magic, a_resist, a_option_index0, a_option_level0, a_option_index1, a_option_level1, a_option_index2, a_option_level2, a_option_index3, a_option_level3, a_option_index4, a_option_level4, a_option_index5, a_option_level5, a_option_index6, a_option_level6, a_option_index7, a_option_level7, a_option_index8, a_option_level8, a_option_index9, a_option_level9 FROM {pMain.pSettings.DBData}.t_rareoption WHERE a_grade!=-1 ORDER BY a_index;";
#else
				string strQuery = $"SELECT a_index, a_grade, a_attack, a_defense, a_magic, a_resist, a_option_index0, a_option_level0, a_option_index1, a_option_level1, a_option_index2, a_option_level2, a_option_index3, a_option_level3, a_option_index4, a_option_level4, a_option_index5, a_option_level5, a_option_index6, a_option_level6, a_option_index7, a_option_level7, a_option_index8, a_option_level8, a_option_index9, a_option_level9 FROM {pMain.pSettings.DBData}.t_rareoption WHERE a_grade!=-1 ORDER BY a_index;";
#endif
				DataTable? pTable = pMain.QuerySelect("utf8", strQuery);  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_grade"]));
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							Stream.Write(Convert.ToInt32(pRow["a_type"]));
#endif
							Stream.Write(Convert.ToInt32(pRow["a_attack"]));
							Stream.Write(Convert.ToInt32(pRow["a_defense"]));
							Stream.Write(Convert.ToInt32(pRow["a_magic"]));
							Stream.Write(Convert.ToInt32(pRow["a_resist"]));

							for (int i = 0; i < Defs.DEF_RAREOPTION_MAX; i++)
							{
								Stream.Write(Convert.ToInt32(pRow["a_option_index" + i]));
								Stream.Write(Convert.ToInt32(pRow["a_option_level" + i]));
							}
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportMOBSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nSkillID;
				sbyte sbSkillLevel;
				string[] strSkillData;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT DISTINCT a_index, a_level, a_walk_speed, a_run_speed, a_size, a_hp, a_mp, a_attack_area, a_skillmaster, a_flag, a_flag1, a_scale, a_sskill_master, a_file_smc, a_motion_idle, a_motion_walk, a_motion_dam, a_motion_attack, a_motion_die, a_motion_run, a_motion_idle2, a_motion_attack2, a_attackSpeed, a_skill0, a_skill1, a_attackType, a_fireDelayCount, a_fireDelay0, a_fireDelay1, a_fireDelay2, a_fireDelay3, a_fireEffect0, a_fireEffect1, a_fireEffect2, a_fireObject, a_fireSpeed, a_rvr_value, a_rvr_grade, a_bound FROM {pMain.pSettings.DBData}.t_npc WHERE a_enable=1 ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_level"]));
							Stream.Write(Convert.ToInt32(pRow["a_hp"]));
							Stream.Write(Convert.ToInt32(pRow["a_mp"]));
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							Stream.Write(Convert.ToInt32(pRow["a_flag"]));
#else
							Stream.Write(Convert.ToInt64(pRow["a_flag"]));
#endif
							Stream.Write(Convert.ToInt32(pRow["a_flag1"]));
							Stream.Write(Convert.ToInt32(pRow["a_attackSpeed"]));
							Stream.Write(Convert.ToSingle(pRow["a_walk_speed"]));
							Stream.Write(Convert.ToSingle(pRow["a_run_speed"]));
							Stream.Write(Convert.ToSingle(pRow["a_scale"]));
							Stream.Write(Convert.ToSingle(pRow["a_attack_area"]));
							Stream.Write(Convert.ToSingle(pRow["a_size"]));
							Stream.Write(Convert.ToSByte(pRow["a_skillmaster"]));
							Stream.Write(Convert.ToSByte(pRow["a_sskill_master"]));
#if USE_ORIGINAL_SQL_REQUEST_AND_ORIGINAL_FILE_SERIALIZATION
							for (int i = 0; i < 5; i++)
								Stream.Write(0);	//Stream.Write(Convert.ToInt32(pRow[$"a_skill{i}_effect"]));
#endif
							Stream.Write(Convert.ToSByte(pRow["a_attackType"]));
							Stream.Write(Convert.ToByte(pRow["a_fireDelayCount"]));

							for (int i = 0; i < Defs.DEF_MAX_NPC_FIRE_DELAY; i++)
								Stream.Write(Convert.ToSingle(pRow["a_fireDelay" + i]));

							Stream.Write(Convert.ToByte(pRow["a_fireObject"]));
							Stream.Write(Convert.ToSingle(pRow["a_fireSpeed"]));

							for (int i = 0; i < Defs.MAX_NPC_SKILL_CLIENT; i++)
							{
								nSkillID = -1;
								sbSkillLevel = 0;
								strSkillData = (pRow["a_skill" + i].ToString() ?? string.Empty).Split(' ');

								if (strSkillData.Length >= 2 && strSkillData[0] != "-1")
								{
									// NOTE: do that in t_npc:
									// UPDATE t_npc SET a_skill0 = LTRIM(a_skill0) WHERE LEFT(a_skill0, 1) = ' ';
									// UPDATE t_npc SET a_skill1 = LTRIM(a_skill1) WHERE LEFT(a_skill1, 1) = ' ';
									// UPDATE t_npc SET a_skill2 = LTRIM(a_skill2) WHERE LEFT(a_skill2, 1) = ' ';
									// UPDATE t_npc SET a_skill3 = LTRIM(a_skill3) WHERE LEFT(a_skill3, 1) = ' ';
									// I can add a trim, but i don't want :D
									nSkillID = Convert.ToInt32(strSkillData[0]);
									sbSkillLevel = Convert.ToSByte(strSkillData[1]);
								}

								Stream.Write(nSkillID);
								Stream.Write(sbSkillLevel);
							}

							Stream.Write(Convert.ToInt32(pRow["a_rvr_grade"]));
							Stream.Write(Convert.ToInt32(pRow["a_rvr_value"]));
							Stream.Write(Convert.ToSingle(pRow["a_bound"]));

							WriteLengthNText(Stream, pRow["a_file_smc"].ToString() ?? string.Empty, Defs.DEF_SMC_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_motion_idle"].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_motion_walk"].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_motion_dam"].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_motion_attack"].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_motion_die"].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_motion_run"].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_motion_idle2"].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_motion_attack2"].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);

							for (int i = 0; i < Defs.DEF_MOB_FIRE_EFFECT; i++)
								WriteLengthNText(Stream, pRow["a_fireEffect" + i].ToString() ?? string.Empty, Defs.DEF_ANI_LENGTH, null);
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportOPTIONSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nValue;
				string[] strValues;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_type, a_level FROM {pMain.pSettings.DBData}.t_option ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";
					string strTempFilePath = strFilePath + ".tmp";

					try
					{
						using (BinaryWriter Stream = new(File.Create(strTempFilePath)))
						{
							Stream.Write(pTable.Rows.Count);

							foreach (DataRow pRow in pTable.Rows)
							{
								int nOptionIndex = Convert.ToInt32(pRow["a_index"]);

								Stream.Write(nOptionIndex);
								Stream.Write(Convert.ToInt32(pRow["a_type"]));

								strValues = (pRow["a_level"].ToString() ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

								for (int i = 0; i < Defs.DEF_OPTION_MAX_LEVEL; i++)
								{
									nValue = 0;

									if (strValues.Length > i && !int.TryParse(strValues[i], out nValue))
										throw new FormatException($"Option {nOptionIndex} has invalid a_level value '{strValues[i]}' at position {i + 1}.");

									Stream.Write(nValue);
								}
							}
						}

						ReplaceFileWithTemp(strTempFilePath, strFilePath);
					}
					catch
					{
						if (File.Exists(strTempFilePath))
							File.Delete(strTempFilePath);

						throw;
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportTITLESAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_enable, a_effect_name, a_attack, a_damage, a_color, a_bgcolor, a_option_index0, a_option_index1, a_option_index2, a_option_index3, a_option_index4, a_option_level0, a_option_level1, a_option_level2, a_option_level3, a_option_level4, a_option_value0, a_option_value1, a_option_value2, a_option_value3, a_option_value4, a_item_index FROM {pMain.pSettings.DBData}.t_title ORDER BY a_index");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							int nTitleIndex = Convert.ToInt32(pRow["a_index"]);

							Stream.Write(nTitleIndex);
							Stream.Write(Convert.ToByte(pRow["a_enable"]));

							WriteLengthNText(Stream, pRow["a_effect_name"].ToString() ?? string.Empty, Defs.MAX_TITLE_EFFECT_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_attack"].ToString() ?? string.Empty, Defs.MAX_TITLE_EFFECT_LENGTH, null);
							WriteLengthNText(Stream, pRow["a_damage"].ToString() ?? string.Empty, Defs.MAX_TITLE_EFFECT_LENGTH, null);

							Stream.Write(ReadTitleHexColor(pRow, "a_color", nTitleIndex, unchecked((int)0xFFFFFFFF)));
							Stream.Write(ReadTitleHexColor(pRow, "a_bgcolor", nTitleIndex, 0x000000FF));

							for (int i = 0; i < Defs.DEF_NICK_OPTION_MAX; i++)
								Stream.Write(Convert.ToInt32(pRow["a_option_index" + i]));

							for (int i = 0; i < Defs.DEF_NICK_OPTION_MAX; i++)
								Stream.Write(Convert.ToByte(pRow["a_option_level" + i]));

							for (int i = 0; i < Defs.DEF_NICK_OPTION_MAX; i++)
								Stream.Write(Convert.ToInt32(pRow["a_option_value" + i]));

							Stream.Write(Convert.ToInt32(pRow["a_item_index"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportMAPSAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nZoneID;
				string[] strDMLeft, strDMTop, strDMRight, strDMBottom, strDMWLeft, strDMWTop, strDMWRight, strDMWBottom, strDMX, strDMZ, strDMRate, strDungeonID, strDungeonX, strDungeonZ, strDungeonType;
				byte sDetailMapCount, sDungeonCount;
				DataTable? pZonesTable, pNPCTable;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";
				
				pZonesTable = pMain.QuerySelect("utf8", $"SELECT a_zone_index, a_ylayer, a_map_left, a_map_top, a_map_right, a_map_bottom, a_map_rate, a_map_offset_x, a_map_offset_z, a_detailmap_count, a_detailmap_left, a_detailmap_top, a_detailmap_right, a_detailmap_bottom, a_detailmap_w_left, a_detailmap_w_top, a_detailmap_w_right, a_detailmap_w_bottom, a_detailmap_x, a_detailmap_z, a_detailmap_rate, a_dungeon_count, a_dungeon_index, a_dungeon_x, a_dungeon_z, a_dungeon_type FROM {pMain.pSettings.DBData}.t_mapinfo ORDER BY a_sort_key");  // Hardcode!
				if (pZonesTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pZonesTable.Rows.Count);

						foreach (DataRow pRow in pZonesTable.Rows)
						{
							nZoneID = Convert.ToInt32(pRow["a_zone_index"]);

							Stream.Write(nZoneID);
							Stream.Write(Convert.ToByte(pRow["a_ylayer"]));	
							Stream.Write(Convert.ToInt32(pRow["a_map_left"]));
							Stream.Write(Convert.ToInt32(pRow["a_map_top"]));
							Stream.Write(Convert.ToInt32(pRow["a_map_right"]));
							Stream.Write(Convert.ToInt32(pRow["a_map_bottom"]));
							Stream.Write(Convert.ToSingle(pRow["a_map_rate"], CultureInfo.InvariantCulture));	// Just in case...
							Stream.Write(Convert.ToInt32(pRow["a_map_offset_x"]));
							Stream.Write(Convert.ToInt32(pRow["a_map_offset_z"]));

							sDetailMapCount = Convert.ToByte(pRow["a_detailmap_count"]);
							// NOTE: do that in t_mapinfo:
							// UPDATE t_mapinfo SET a_detailmap_left = LTRIM(a_detailmap_left) WHERE LEFT(a_detailmap_left, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_top = LTRIM(a_detailmap_top) WHERE LEFT(a_detailmap_top, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_right = LTRIM(a_detailmap_right) WHERE LEFT(a_detailmap_right, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_bottom = LTRIM(a_detailmap_bottom) WHERE LEFT(a_detailmap_bottom, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_w_left = LTRIM(a_detailmap_w_left) WHERE LEFT(a_detailmap_w_left, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_w_top = LTRIM(a_detailmap_w_top) WHERE LEFT(a_detailmap_w_top, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_w_right = LTRIM(a_detailmap_w_right) WHERE LEFT(a_detailmap_w_right, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_w_bottom = LTRIM(a_detailmap_w_bottom) WHERE LEFT(a_detailmap_w_bottom, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_x = LTRIM(a_detailmap_x) WHERE LEFT(a_detailmap_x, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_z = LTRIM(a_detailmap_z) WHERE LEFT(a_detailmap_z, 1) = ' ';
							// UPDATE t_mapinfo SET a_detailmap_rate = LTRIM(a_detailmap_rate) WHERE LEFT(a_detailmap_rate, 1) = ' ';
							// I can add a trim, but i don't want :D
							strDMLeft = (pRow["a_detailmap_left"].ToString() ?? string.Empty).Split(' ');
							strDMTop = (pRow["a_detailmap_top"].ToString() ?? string.Empty).Split(' ');
							strDMRight = (pRow["a_detailmap_right"].ToString() ?? string.Empty).Split(' ');
							strDMBottom = (pRow["a_detailmap_bottom"].ToString() ?? string.Empty).Split(' ');
							strDMWLeft = (pRow["a_detailmap_w_left"].ToString() ?? string.Empty).Split(' ');
							strDMWTop = (pRow["a_detailmap_w_top"].ToString() ?? string.Empty).Split(' ');
							strDMWRight = (pRow["a_detailmap_w_right"].ToString() ?? string.Empty).Split(' ');
							strDMWBottom = (pRow["a_detailmap_w_bottom"].ToString() ?? string.Empty).Split(' ');
							strDMX = (pRow["a_detailmap_x"].ToString() ?? string.Empty).Split(' ');
							strDMZ = (pRow["a_detailmap_z"].ToString() ?? string.Empty).Split(' ');
							strDMRate = (pRow["a_detailmap_rate"].ToString() ?? string.Empty).Split(' ');

							Stream.Write(sDetailMapCount);

							for (int i = 0; i < sDetailMapCount; i++)
							{
								Stream.Write(Convert.ToInt32(strDMLeft[i]));	// Write a_detailmap_left
								Stream.Write(Convert.ToInt32(strDMTop[i]));		// Write a_detailmap_top
								Stream.Write(Convert.ToInt32(strDMRight[i]));	// Write a_detailmap_right
								Stream.Write(Convert.ToInt32(strDMBottom[i]));	// Write a_detailmap_bottom
								Stream.Write(Convert.ToInt32(strDMWLeft[i]));	// Write a_detailmap_w_left
								Stream.Write(Convert.ToInt32(strDMWTop[i]));	// Write a_detailmap_w_top
								Stream.Write(Convert.ToInt32(strDMWRight[i]));	// Write a_detailmap_w_right
								Stream.Write(Convert.ToInt32(strDMWBottom[i])); // Write a_detailmap_w_bottom
								Stream.Write(Convert.ToSingle(strDMX[i], CultureInfo.InvariantCulture));	// Write a_detailmap_x
								Stream.Write(Convert.ToSingle(strDMZ[i], CultureInfo.InvariantCulture));	// Write a_detailmap_z
								Stream.Write(Convert.ToSingle(strDMRate[i], CultureInfo.InvariantCulture));	// Write a_detailmap_rate
							}
							
							sDungeonCount = Convert.ToByte(pRow["a_dungeon_count"]);
							// NOTE: do that in t_mapinfo:
							// UPDATE t_mapinfo SET a_dungeon_index = LTRIM(a_dungeon_index) WHERE LEFT(a_dungeon_index, 1) = ' ';
							// UPDATE t_mapinfo SET a_dungeon_x = LTRIM(a_dungeon_x) WHERE LEFT(a_dungeon_x, 1) = ' ';
							// UPDATE t_mapinfo SET a_dungeon_z = LTRIM(a_dungeon_z) WHERE LEFT(a_dungeon_z, 1) = ' ';
							// UPDATE t_mapinfo SET a_dungeon_type = LTRIM(a_dungeon_type) WHERE LEFT(a_dungeon_type, 1) = ' ';
							// I can add a trim, but i don't want :D
							strDungeonID = (pRow["a_dungeon_index"].ToString() ?? string.Empty).Split(' ');
							strDungeonX = (pRow["a_dungeon_x"].ToString() ?? string.Empty).Split(' ');
							strDungeonZ = (pRow["a_dungeon_z"].ToString() ?? string.Empty).Split(' ');
							strDungeonType = (pRow["a_dungeon_type"].ToString() ?? string.Empty).Split(' ');

							Stream.Write(Convert.ToByte(sDungeonCount));
							
							for (int i = 0; i < sDungeonCount; i++)
							{
								Stream.Write(Convert.ToInt32(strDungeonID[i]));	// Write a_dungeon_index
								Stream.Write(Convert.ToSingle(strDungeonX[i], CultureInfo.InvariantCulture));	// Write a_dungeon_x
								Stream.Write(Convert.ToSingle(strDungeonZ[i], CultureInfo.InvariantCulture));	// Write a_dungeon_z
								Stream.Write(Convert.ToByte(strDungeonType[i]));	// Write a_dungeon_type
							}
							
							// Minimap Data
							pNPCTable = pMain.QuerySelect("utf8", $"(SELECT t1.a_npc_idx npcindex, t1.a_y_layer, t1.a_pos_x, t1.a_pos_z FROM {pMain.pSettings.DBData}.t_npc_regen t1, {pMain.pSettings.DBData}.t_npc t2 WHERE t1.a_zone_num={nZoneID} AND t1.a_npc_idx=t2.a_index AND ((t2.a_flag & {(1L << Array.IndexOf(Defs.NPCFlags, "DISPLAY_MAP"))})!=0 OR EXISTS (SELECT 1 FROM {pMain.pSettings.DBData}.t_affinity_npc an WHERE an.a_npcidx=t1.a_npc_idx AND an.a_enable=1)) AND t2.a_enable=1) UNION (SELECT t3.a_keeper_idx npcindex, t3.a_y_layer, t3.a_pos_x, t3.a_pos_z FROM {pMain.pSettings.DBData}.t_shop t3, {pMain.pSettings.DBData}.t_npc t4 WHERE t3.a_zone_num={nZoneID} AND t3.a_keeper_idx=t4.a_index AND t4.a_enable=1) ORDER BY npcindex;", (pMain.pSettings.LogVerbose < 2 ? false : true));	 // Hardcode!
							if (pNPCTable != null)
							{
								Stream.Write(Convert.ToInt32(pNPCTable.Rows.Count));

								foreach (DataRow pNPCRow in pNPCTable.Rows)
								{
									Stream.Write(Convert.ToInt32(pNPCRow["npcindex"]));
									Stream.Write(Convert.ToInt32(pNPCRow["a_y_layer"]));
									Stream.Write(Convert.ToSingle(pNPCRow["a_pos_x"], CultureInfo.InvariantCulture));
									Stream.Write(Convert.ToSingle(pNPCRow["a_pos_z"], CultureInfo.InvariantCulture));
								}

								pNPCTable.Dispose();
							}
						}
					}

					pZonesTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportNPCHELPAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nZoneNum, nNPCIndex;
				DataTable? pNPCTable, pShopTable;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				pNPCTable = pMain.QuerySelect("utf8", $"SELECT DISTINCT nr.a_zone_num, n.a_index FROM {pMain.pSettings.DBData}.t_npc_regen nr, {pMain.pSettings.DBData}.t_npc n WHERE nr.a_npc_idx=n.a_index AND (((nr.a_zone_num IN(0, 4, 7, 15, 23, 32, 39, 40)) AND (n.a_flag & {(1L << Array.IndexOf(Defs.NPCFlags, "DISPLAY_MAP"))})>0 AND !(n.a_flag1 & {(1L << Array.IndexOf(Defs.NPCFlags1, "NOT_NPCPORTAL"))})) OR EXISTS (SELECT 1 FROM {pMain.pSettings.DBData}.t_affinity_npc an WHERE an.a_npcidx=nr.a_npc_idx AND an.a_enable=1)) AND n.a_enable=1 ORDER BY nr.a_zone_num, n.a_index;");  // Hardcode!
				if (pNPCTable != null)
				{
					pShopTable = pMain.QuerySelect("utf8", $"SELECT DISTINCT shop_npc.a_zone_num, n.a_index FROM {pMain.pSettings.DBData}.t_npc AS n, {pMain.pSettings.DBData}.t_shop AS shop_npc WHERE n.a_index=shop_npc.a_keeper_idx AND (((shop_npc.a_zone_num IN(0, 4, 7, 15, 23, 32, 39, 40)) AND (n.a_flag & {(1L << Array.IndexOf(Defs.NPCFlags, "DISPLAY_MAP"))}) AND !(n.a_flag1 & {(1L << Array.IndexOf(Defs.NPCFlags1, "NOT_NPCPORTAL"))})) OR EXISTS (SELECT 1 FROM {pMain.pSettings.DBData}.t_affinity_npc an WHERE an.a_npcidx=shop_npc.a_keeper_idx AND an.a_enable=1)) AND n.a_enable=1;");  // Hardcode!
					if (pShopTable != null)
					{
						if (!Directory.Exists(strFilePath))
							Directory.CreateDirectory(strFilePath);

						strFilePath += $"\\{strFileName}.lod";

						using (BinaryWriter Stream = new(File.Create(strFilePath)))
						{
							Stream.Write((pNPCTable.Rows.Count + pShopTable.Rows.Count));   // Write the Total Rows of both Tables combined

							for (int i = 0; i < 2; i++)
							{
								foreach (DataRow pRow in (i == 0 ? pNPCTable : pShopTable).Rows)  // NOTE: Look this MASTERPIECE DUDEEEE sheesh
								{
									nZoneNum = Convert.ToInt32(pRow["a_zone_num"]);

									nNPCIndex = (nZoneNum << 24);
									nNPCIndex |= (Convert.ToInt32(pRow["a_index"]) & 0x00FFFFFF);

									Stream.Write(nNPCIndex);
									Stream.Write(nZoneNum);
								}
							}
						}

						pShopTable.Dispose();
					}

					pNPCTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportAFFINITIESAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nAffinityID, nAffinityNPCID, nAffinityNPCFlag;
				DataRow[] pArray;
				List<(int, int)> pRewards = new();
				DataTable? pAffinityTable, pAffinityNPCTable, pAffinityWorkTable, pAffinityRewardTable;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				pAffinityTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_texture_id, a_texture_row, a_texture_col, a_needitemidx, a_needitemcount, a_needlevel, a_affinity_idx, a_affinity_value FROM {pMain.pSettings.DBData}.t_affinity WHERE a_enable=1;"); // Hardcode!
				if (pAffinityTable != null)
				{
					pAffinityNPCTable = pMain.QuerySelect("utf8", $"SELECT a_affinity_idx, a_npcidx, a_flag, a_string_idx FROM {pMain.pSettings.DBData}.t_affinity_npc WHERE a_enable=1;");  // Hardcode!
					if (pAffinityNPCTable != null)
					{
						pAffinityWorkTable = pMain.QuerySelect("utf8", $"SELECT a_affinity_idx, a_work_type, a_type_idx, a_value, a_id, a_row, a_col FROM {pMain.pSettings.DBData}.t_affinity_work WHERE a_enable=1;");    // Hardcode!
						if (pAffinityWorkTable != null)
						{
							pAffinityRewardTable = pMain.QuerySelect("utf8", $"SELECT a_npcidx, a_itemidx, a_allow_point FROM {pMain.pSettings.DBData}.t_affinity_reward_item;");    // Hardcode!
							if (pAffinityRewardTable != null)
							{
								if (!Directory.Exists(strFilePath))
									Directory.CreateDirectory(strFilePath);

								strFilePath += $"\\{strFileName}.lod";

								using (BinaryWriter Stream = new(File.Create(strFilePath)))
								{
									Stream.Write(pAffinityTable.Rows.Count);

									foreach (DataRow pAffinityRow in pAffinityTable.Rows)
									{
										nAffinityID = Convert.ToInt32(pAffinityRow["a_index"]);

										Stream.Write(nAffinityID);
										Stream.Write(Convert.ToInt32(pAffinityRow["a_texture_id"]));
										Stream.Write(Convert.ToInt32(pAffinityRow["a_texture_row"]));
										Stream.Write(Convert.ToInt32(pAffinityRow["a_texture_col"]));
										Stream.Write(Convert.ToInt32(pAffinityRow["a_needitemidx"]));
										Stream.Write(Convert.ToInt32(pAffinityRow["a_needitemcount"]));
										Stream.Write(Convert.ToInt32(pAffinityRow["a_needlevel"]));
										Stream.Write(Convert.ToInt32(pAffinityRow["a_affinity_idx"]));
										Stream.Write(Convert.ToInt32(pAffinityRow["a_affinity_value"]));

										pRewards.Clear();

										pArray = pAffinityNPCTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_affinity_idx"]) == nAffinityID).ToArray();

										Stream.Write(pArray.Length);    // Write Total Affinity NPC Rows

										foreach (DataRow pAffinityNPCRow in pArray)
										{
											nAffinityNPCID = Convert.ToInt32(pAffinityNPCRow["a_npcidx"]);

											Stream.Write(nAffinityNPCID);

											nAffinityNPCFlag = Convert.ToInt32(pAffinityNPCRow["a_flag"]);

											Stream.Write(nAffinityNPCFlag);
											Stream.Write(Convert.ToInt32(pAffinityNPCRow["a_string_idx"]));

											if ((nAffinityNPCFlag & (1L << Array.IndexOf(Defs.AffinityFlags, "PRESENT"))) != 0)
											{
												foreach (DataRow pRewardRow in pAffinityRewardTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_npcidx"]) == nAffinityNPCID).OrderBy(row => Convert.ToInt32(row["a_allow_point"])).ToArray())
													pRewards.Add((Convert.ToInt32(pRewardRow["a_itemidx"]), Convert.ToInt32(pRewardRow["a_allow_point"])));
											}
										}

										pArray = pAffinityWorkTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_affinity_idx"]) == nAffinityID && Convert.ToInt32(row["a_work_type"]) == Array.IndexOf(Defs.AffinityWorkTypes, "ITEM")).ToArray();

										Stream.Write(pArray.Length);    // Write Total Affinity Item Work Rows

										foreach (DataRow pAffinityWorkRow in pArray)
										{
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_type_idx"]));  // Write Item ID
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_value"])); // Write Affinity Points
										}

										pArray = pAffinityWorkTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_affinity_idx"]) == nAffinityID && Convert.ToInt32(row["a_work_type"]) == Array.IndexOf(Defs.AffinityWorkTypes, "MOB")).ToArray();

										Stream.Write(pArray.Length);  // Write Total Affinity Item Work Rows

										foreach (DataRow pAffinityWorkRow in pArray)
										{
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_type_idx"]));  // Write Mob ID
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_value"])); // Write Affinity Points
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_id"]));
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_row"]));
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_col"]));
										}

										pArray = pAffinityWorkTable.AsEnumerable().Where(row => Convert.ToInt32(row["a_affinity_idx"]) == nAffinityID && Convert.ToInt32(row["a_work_type"]) == Array.IndexOf(Defs.AffinityWorkTypes, "QUEST")).ToArray();

										Stream.Write(pArray.Length);  // Write Total Affinity Quest Work Rows

										foreach (DataRow pAffinityWorkRow in pArray)
										{
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_type_idx"]));  // Write Quest ID
											Stream.Write(Convert.ToInt32(pAffinityWorkRow["a_value"])); // Write Affinity Points
										}

										Stream.Write(pRewards.Count);   // Write Total Affinity Quest Work Rows

										foreach (var RewardData in pRewards)
										{
											Stream.Write(RewardData.Item1);    // Write Item ID
											Stream.Write(RewardData.Item2);    // Write Affinity Points
										}
									}
								}

								pAffinityRewardTable.Dispose();
							}

							pAffinityWorkTable.Dispose();
						}

						pAffinityNPCTable.Dispose();
					}

					pAffinityTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/

		private void ExportDATAITEMCOLLECTIONAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				int nCategoryID;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_category, a_theme, a_id, a_row, a_col, a_need1_type, a_need2_type, a_need3_type, a_need4_type, a_need5_type, a_need6_type, a_need1_index, a_need2_index, a_need3_index, a_need4_index, a_need5_index, a_need6_index, a_need1_num, a_need2_num, a_need3_num, a_need4_num, a_need5_num, a_need6_num, a_result_type, a_result_index, a_result_num FROM {pMain.pSettings.DBData}.t_item_collection WHERE a_enable=1 ORDER BY a_theme, a_category;");	// Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							nCategoryID = Convert.ToInt32(pRow["a_category"]);

							nCategoryID = (nCategoryID << 24);
							nCategoryID |= (Convert.ToInt32(pRow["a_theme"]) & 0x00FFFFFF);

							Stream.Write(nCategoryID);
							Stream.Write(Convert.ToByte(pRow["a_id"]));
							Stream.Write(Convert.ToByte(pRow["a_row"]));
							Stream.Write(Convert.ToInt16(pRow["a_col"]));
							
							for (int i = 1; i <= Defs.DEF_NEED_ITEM_COUNT; i++)
							{
								Stream.Write(Convert.ToInt32(pRow[$"a_need{i}_type"]));
								Stream.Write(Convert.ToInt32(pRow[$"a_need{i}_index"]));
								Stream.Write(Convert.ToInt32(pRow[$"a_need{i}_num"]));
							}

							Stream.Write(Convert.ToInt32(pRow["a_result_type"]));
							Stream.Write(Convert.ToInt32(pRow["a_result_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_result_num"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportITEMFORTUNEAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";
				
				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_skill_index, a_skill_level, a_string_index, a_prob FROM {pMain.pSettings.DBData}.t_fortune_data GROUP BY a_skill_index ORDER BY a_skill_index");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_skill_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_skill_level"]));
							Stream.Write(Convert.ToInt32(pRow["a_string_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_prob"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportMOBHELPAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";
				
				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT DISTINCT nr.a_zone_num, n.a_index, n.a_name from {pMain.pSettings.DBData}.t_npc_regen AS nr, {pMain.pSettings.DBData}.t_npc AS n WHERE nr.a_zone_num IN(0, 4, 7, 15, 23, 32, 39, 40) AND nr.a_npc_idx=n.a_index AND !(a_flag & {(1L << Array.IndexOf(Defs.NPCFlags, "PEACEFUL"))}) AND !(a_flag1 & {(1L << Array.IndexOf(Defs.NPCFlags1, "AFFINITY"))}) AND n.a_enable=1 ORDER BY nr.a_zone_num, n.a_index;");	  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt32(pRow["a_zone_num"]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportDATASETITEMAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				string[] strSplited;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_set_idx, a_job, a_item_idx, a_option_count, a_wear_count, a_option_type, a_option_idx, a_option_level FROM {pMain.pSettings.DBData}.t_set_item WHERE a_enable=1 ORDER BY a_set_idx;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";

					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_set_idx"]));
							Stream.Write(Convert.ToInt32(pRow["a_job"]));
							Stream.Write(Convert.ToInt32(pRow["a_option_count"]));

							strSplited = (pRow["a_item_idx"].ToString() ?? string.Empty).Split(' ');

							for (int i = 0; i < Defs.MAX_WEARING; i++)
								Stream.Write(Convert.ToInt32(strSplited[i]));

							strSplited = (pRow["a_wear_count"].ToString() ?? string.Empty).Split(' ');

							for (int i = 0; i < Defs.MAX_SET_ITEM_OPTION; i++)
								Stream.Write(Convert.ToInt32(strSplited[i]));

							strSplited = (pRow["a_option_type"].ToString() ?? string.Empty).Split(' ');

							for (int i = 0; i < Defs.MAX_SET_ITEM_OPTION; i++)
								Stream.Write(Convert.ToInt32(strSplited[i]));

							strSplited = (pRow["a_option_idx"].ToString() ?? string.Empty).Split(' ');

							for (int i = 0; i < Defs.MAX_SET_ITEM_OPTION; i++)
								Stream.Write(Convert.ToInt32(strSplited[i]));

							strSplited = (pRow["a_option_level"].ToString() ?? string.Empty).Split(' ');

							for (int i = 0; i < Defs.MAX_SET_ITEM_OPTION; i++)
								Stream.Write(Convert.ToInt32(strSplited[i]));
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
		/****************************************/
		private void ExportZONEFLAGAsync(string strFileName)
		{
			Task pTask = Task.Run(() =>
			{
#if DEBUG
				Stopwatch stopwatch = Stopwatch.StartNew();
#endif
				long lExtraFlag, lFlag;
				string strFilePath = pMain.pSettings.ClientPath + "\\Data";

				if (rbExportToLocalFolder.Checked)
					strFilePath = "Data";

				DataTable? pTable = pMain.QuerySelect("utf8", $"SELECT a_index, a_zone_flag, a_extra_flag FROM {pMain.pSettings.DBData}.t_npc WHERE a_flag!=0 AND a_zone_flag>0 ORDER BY a_index;");  // Hardcode!
				if (pTable != null)
				{
					if (!Directory.Exists(strFilePath))
						Directory.CreateDirectory(strFilePath);

					strFilePath += $"\\{strFileName}.lod";
					
					using (BinaryWriter Stream = new(File.Create(strFilePath)))
					{
						Stream.Write(pTable.Rows.Count);

						foreach (DataRow pRow in pTable.Rows)
						{
							Stream.Write(Convert.ToInt32(pRow["a_index"]));
							Stream.Write(Convert.ToInt64(pRow["a_zone_flag"]));

							lExtraFlag = 1L << Convert.ToInt32(pRow["a_extra_flag"]);

							for (int i = 0; i < Defs.MAX_AREA_COUNT; i++)
							{
								lFlag = (1L << i);

								if ((lExtraFlag & lFlag) != 0L)
								{
									Stream.Write(lFlag);
									break;
								}
							}
						}
					}

					pTable.Dispose();
				}
#if DEBUG
				stopwatch.Stop();
				long elapsedTime = stopwatch.ElapsedMilliseconds;
				Interlocked.Add(ref lTotalElapsedTime, elapsedTime);

				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done! (Took: {elapsedTime}ms)");
#else
				pMain.Logger(LogTypes.Success, $"Export: {strFilePath} Done!");
#endif
			});

			ExportTasks.Add(pTask);
		}
	}
}
