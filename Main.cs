// TODO: After migrates from framework 4.7 to 9 have a lot of warnings. I'll fix all eventually... xd of course i'm (;8600;8601;8602;8603;8604;8618;8619;8620;8621;8622;8625)
// TODO: ¿All btnReload_Click functions should call a global trigger to warning all open Forms to reload too?

namespace LastChaos_ToolBoxNG
{
	public enum LogTypes
	{
		Message, Warning, Success, Error,
		ServiceLog,
		Other1, Other2
	};

	public partial class Main : Form
	{
		// Private Vals
		private string strWindowsTitle = "Status: DB Connected: NONE";
		private ToolStripStatusLabel tsslStatus = new();
		private MySqlConnection pMySQLConnection = new();
		private Timer mouseTimer = new();
		private static readonly object LogLock = new();
		private static readonly Font pConsoleFont = new("Consolas", 9f, FontStyle.Bold);
		private static readonly StreamWriter pStreamWriter = new("Logs.log", true) { AutoFlush = false };
		private Dictionary<string, Func<Form>> pEditors = new();

		// Public Vals
		public sealed class MainTables : IDisposable
		{   // Add Main DataTables Here ↓
			public DataTable? ItemTable;
			public DataTable? ZoneTable;
			public DataTable? SkillTable;
			public DataTable? SkillLevelTable;
			public DataTable? SpecialSkillTable;
			public DataTable? RareOptionTable;
			public DataTable? ItemFortuneHeadTable;
			public DataTable? ItemFortuneDataTable;
			public DataTable? StringTable;
			public DataTable? OptionTable;
			public DataTable? MagicTable;
			public DataTable? MagicLevelTable;
			public DataTable? CraftingTable;
			public DataTable? DailyRewardTable;
			public DataTable? NPCTable;
			public DataTable? NPCDropJobTable;
			public DataTable? NPCDropRaidTable;
			public DataTable? NPCDropAllTable;
			public DataTable? ShopTable;
			public DataTable? ShopItemTable;
			public DataTable? QuestTable;
			public DataTable? OXQuizTable;
			public DataTable? OXRewardTable;
			public DataTable? MissionCaseTable;
			public DataTable? NPCRegenComboTable;
			public DataTable? MoonstoneRewardTable;
			public DataTable? TitleTable;
			public DataTable? ItemSetTable;
			public DataTable? KeyTable;
			public DataTable? NPCRegenTable;
			public DataTable? LacaBallTable;

			public void Dispose()
			{
				foreach (FieldInfo field in GetType().GetFields())
				{
					if (field.FieldType == typeof(DataTable))
					{
						(field.GetValue(this) as DataTable)?.Dispose();
						field.SetValue(this, null);
					}
				}
			}
		}

		public MainTables pTables = new();

		public class Settings
		{
			public string SettingsFile = "Settings.ini";

			public string DBHost = "";
			public string DBUsername = "";
			public string DBPassword = "";
			public string DBAuth = "";
			public string DBData = "";
			public string DBUser = "";
			public string DBCharset = "";

			public string WorkLocale = "";

			public bool DockToTop = false;
			public bool MaximizeOnStartUp = false;

			public string ServerPath = "";
			public string ClientPath = "";
			public string TextEditorPath = "";

			public string[]? NationSupported;

			public string DeepLURL = "";
			public string DeepLAPIKey = "";

			public Dictionary<string, bool> Show3DViewerDialog = new();

			public Dictionary<string, bool> ChangesAppliedNotification = new();

			// Exporter
			public Dictionary<string, string> Exporter = new();

			public bool ExportToLocal = true;
			public int LogVerbose = 1;

			// Control Panel
			public string NkspLaunchArgument = "fkzktlfgod!";   // Change Nksp Launch Parameter Here ←
			public bool AutoReUp = false;
			public int MaxReUpTrys = 2;
			public bool StartCashServerOnStartAll = false;
			public int TotalGameServers = 0;
			public string[] Services = [	// Add Services Here ↓
				"Messenger",
				"Connector",
				"Helper",
				"SubHelper",
				"LoginServer",
				"CashServer",
				"GameServer1",
				"GameServer2",
				"GameServer3",
				"GameServer4"
			];

			public class ServiceDataStruct
			{
				public bool Found = false;
				public string BinaryName = "NONE";
				public string BinaryPath = "";
				public string ConfigPath = "";
				public bool CatchLog = false;
				public int ReUpTrysCount = 0;
				public int ProcessID = -1;
				public HashSet<string> FilterKeyWord = new();
			};

			public Dictionary<string, ServiceDataStruct> ServicesData = new();
		}

		public Settings pSettings = new();

		public Main()
		{
			InitializeComponent();

			Assembly? pAssembly = Assembly.GetAssembly(typeof(Main));

			this.Text = $"{pAssembly?.GetName().Name} Build: {pAssembly?.GetName()?.Version?.Revision}";

			mouseTimer.Interval = 10;
			mouseTimer.Tick += MouseTimer_Tick;
		}

		private void MouseTimer_Tick(object? sender, EventArgs e)
		{
			Rectangle rectWindow = new(this.Left, this.Top, this.Width, this.Height);

			if (rectWindow.Contains(Cursor.Position))
				this.Top = 0;
			else
				this.Top = -(this.Height - 5);
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			lock (LogLock)
			{
				pStreamWriter.Flush();
				pStreamWriter.Close();
			}
		}

		private async void Main_Load(object sender, EventArgs e)
		{
			Monitor.Start();
			Status.BackColor = Color.FromArgb(40, 40, 40);
			Status.ForeColor = Color.FromArgb(208, 203, 148);

			Status.Items.Add(tsslStatus);

			LoadSettings();

			ConnectToDatabase();
			/****************************************/
			if (pSettings.DockToTop)
			{
				mouseTimer.Start();

				Screen? pScreen = Screen.PrimaryScreen;
				if (pScreen == null)
					return;

				this.Left = (pScreen.Bounds.Width - this.Width) / 2;
				this.Top = -(this.Height - 5);
			}

			cbDockToTop.Checked = pSettings.DockToTop;
			/****************************************/
			if (pSettings.MaximizeOnStartUp)
				this.WindowState = FormWindowState.Maximized;

			cbMaximizeOnStartUp.Checked = pSettings.MaximizeOnStartUp;
			/****************************************/
			(new ToolTip()).SetToolTip(lbEditors, "Press Enter or Space to Open any Tool");
			(new ToolTip()).SetToolTip(rtbConsole, "Press C to clear");

			pEditors = new Dictionary<string, Func<Form>>
			{	// Add Editors or Tools Here ↓
				{ "Item Editor",				() => new ItemEditor(this) },
				{ "Option Editor",				() => new OptionEditor(this) },
				{ "Rare Option Editor",			() => new RareOptionEditor(this) },
				{ "Crafting Editor",			() => new CraftingEditor(this) },
				{ "Daily Reward Editor",		() => new DailyRewardEditor(this) },
				{ "NPC Editor",					() => new NPCEditor(this) },
				{ "Shop Editor",				() => new ShopEditor(this) },
				{ "Treasure Map Editor",		() => new TreasureMapEditor(this) },
				{ "OX Editor",					() => new OXEditor(this) },
				{ "Magic Editor",				() => new MagicEditor(this) },
				{ "Monster Combo Editor",		() => new MonsterComboEditor(this) },
				{ "Moonstone Editor",			() => new MoonstoneEditor(this) },
				{ "Title Editor",				() => new TitleEditor(this) },
				{ "Item Set Editor",			() => new ItemSetEditor(this) },
				{ "Package Item Event Editor",	() => new PackageItemEventEditor(this) },
				{ "LacaBall Editor",			() => new LacaBallEditor(this) },
				{ "Quest Editor",				() => new QuestEditor(this) }
			};

			lbEditors.DataSource = pEditors.Keys.ToList();

			if (await CheckInternetConnection())
				btnCheckUpdates.PerformClick();
		}

		private void Main_Shown(object sender, EventArgs e) { lbEditors.Focus(); }

		private void Main_ResizeEnd(object sender, EventArgs e) { rtbConsole.ScrollToCaret(); }

		private void monitor_Tick(object sender, EventArgs e)
		{
			long lUsedMemory = GC.GetTotalMemory(true);
			string strUnit = "KB";

			if (lUsedMemory >= 1024 * 1024)
			{
				lUsedMemory = lUsedMemory / 1024 / 1024;
				strUnit = "MB";
			}
			else
			{
				lUsedMemory = lUsedMemory / 1024;
			}

			tsslStatus.Text = $"{strWindowsTitle} | RAM Usage: {lUsedMemory}{strUnit}";
		}

		private void btnReloadSettings_Click(object sender, EventArgs e) { LoadSettings(); }

		private void btnReconnect_Click(object sender, EventArgs e)
		{
			if (pMySQLConnection.State == ConnectionState.Open)
			{
				Logger(LogTypes.Message, "MySQL > Closing existing connection...");

				pMySQLConnection.Close();
			}

			ConnectToDatabase();
		}

		private async void btnCheckUpdates_ClickAsync(object sender, EventArgs e)
		{
#if DEBUG
			//return; // API rate limit exceeded for 000.000.000.000... :$
#endif
			Logger(LogTypes.Message, "Main > Checking version, please wait...");

			using (HttpClient httpClient = new())
			{
				httpClient.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");

				using (HttpResponseMessage httpResponse = await httpClient.GetAsync("https://api.github.com/repos/nicolasgomez1/LastChaos-ToolBoxNG/releases"))
				{
					if (httpResponse.IsSuccessStatusCode)
					{
						using (JsonDocument jsonData = JsonDocument.Parse(await httpResponse.Content.ReadAsStringAsync()))
						{
							JsonElement pRoot = jsonData.RootElement[0];
							Assembly? pAssembly = Assembly.GetAssembly(typeof(Main));
							int nRevisionVersion = Convert.ToInt32(pRoot.GetProperty("tag_name").GetString());

							if (pAssembly?.GetName().Version?.Revision < nRevisionVersion)
							{
								Logger(LogTypes.Message, $"Main > Update available!\nCurrent Version:\t{pAssembly?.GetName().Version?.Revision}\nNewer Version:\t{nRevisionVersion}\n\nChangeLog:\n{pRoot.GetProperty("body").GetString()}");

								/*if (MessageBox.Show($"nCurrent Version: {pAssembly?.GetName().Version?.Revision}\nNewer Version: {nRevisionVersion}\n\nChangeLog:\n{pRoot.GetProperty("body").GetString()}\n\n Want upgrade?", "Update available!", MessageBoxButtons.YesNo) == DialogResult.Yes)
								{
									pRoot = pRoot.GetProperty("assets")[0];

									MessageBox_Progress pProgressDialog = new(this, "Downloading, Please Wait...", true);

									using (HttpResponseMessage httpDownloadUrlResponse = await httpClient.GetAsync(pRoot.GetProperty("browser_download_url").GetString()))
									{
										if (httpDownloadUrlResponse.IsSuccessStatusCode)
										{
											using (Stream Stream = await httpDownloadUrlResponse.Content.ReadAsStreamAsync())
											{
												string strFileName = pRoot.GetProperty("name").GetString() ?? "LastChaos ToolBoxNG.exe"; // Hardcode!
												string strFolderPath = strFileName.Substring(0, strFileName.Length - 4);

												using (FileStream fileStreamOutput = File.Create(strFileName))
													await Stream.CopyToAsync(fileStreamOutput);

												if (Directory.Exists(strFolderPath))
													Directory.Delete(strFolderPath, true);

												ZipFile.ExtractToDirectory(strFileName, ".");

												File.Delete(strFileName);

												string strFilePath = "Updater.bat";
												string strFileContent = @"
													timeout /t 2 /nobreak >nul
													move /y """ + strFolderPath + @"\*"" """"
													rmdir /s /q """ + strFolderPath + @"""
													""" + pAssembly?.GetName().Name + @".exe""
													del Updater.bat
												";

												File.WriteAllText(strFilePath, strFileContent);

												ProcessStartInfo psi = new ProcessStartInfo
												{
													FileName = strFilePath,
													UseShellExecute = false,
													CreateNoWindow = true,
													WindowStyle = ProcessWindowStyle.Hidden
												};

												Process.Start(psi);

												Application.Exit();
											}
										}
										else
										{
											Logger(LogTypes.Error, "Main > HTTP Request failed: " + httpDownloadUrlResponse.StatusCode);
										}
									}

									pProgressDialog.Close();
								}*/
							}
							else
							{
								Logger(LogTypes.Message, "Main > Tool Version up to date!");
							}
						}
					}
					else
					{
						Logger(LogTypes.Error, "Main > HTTP Request failed: " + httpResponse.StatusCode);
					}
				}
			}
		}

		private void cbDockToTop_CheckedChanged(object sender, EventArgs e)
		{
			bool bState = cbDockToTop.Checked;
			Screen? pScreen = Screen.PrimaryScreen;
			if (pScreen == null)
				return;

			this.Left = (pScreen.Bounds.Width - this.Width) / 2;

			if (bState)
			{
				mouseTimer.Start();

				this.Top = -(this.Height - 5);
			}
			else
			{
				mouseTimer.Stop();

				this.Top = this.Height / 2;
			}

			pSettings.DockToTop = bState;

			WriteToINI(pSettings.SettingsFile, "Settings", "DockToTop", bState.ToString());
		}

		private void cbMaximizeOnStartUp_CheckedChanged(object sender, EventArgs e)
		{
			bool bState = cbMaximizeOnStartUp.Checked;

			if (bState && this.WindowState != FormWindowState.Maximized)
				this.WindowState = FormWindowState.Maximized;
			else if (!bState && this.WindowState == FormWindowState.Maximized)
				this.WindowState = FormWindowState.Normal;

			pSettings.MaximizeOnStartUp = bState;

			WriteToINI(pSettings.SettingsFile, "Settings", "MaximizeOnStartUp", bState.ToString());
		}

		private void lbEditors_MouseUp(object sender, MouseEventArgs e)
		{
			if (lbEditors.SelectedItem is string strKey && pEditors.TryGetValue(strKey, out var pForm))
				pForm().Show();
		}

		private void lbEditors_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
			{
				if (lbEditors.SelectedItem is string strKey && pEditors.TryGetValue(strKey, out var pForm))
					pForm().Show();
			}
		}

		private void rtbConsole_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.C && !ModifierKeys.HasFlag(Keys.Control))
				rtbConsole.Clear();
		}

		private void btnControlPanel_Click(object sender, EventArgs e) { new ControlPanel(this).Show(); }
		private void btnExporter_Click(object sender, EventArgs e) { new Exporter(this).Show(); }
		private void btnDBStringTranslator_Click(object sender, EventArgs e) { new DBStringTranslator(this).Show(); }
		private void btnUpdateHelper_Click(object sender, EventArgs e) { new UpdateHelper(this).Show(); }

		private void btnFileEncrypter_ClickAsync(object sender, EventArgs e)
		{
			byte[] Buffer;
			int nKey1Index, nKey2Index;
			int nHeaderSize = Defs.EncryptFileHeader.Length;
			int nKey1Size = Defs.EncryptKey1.Length;
			int nKey2Size = Defs.EncryptKey2.Length;
			OpenFileDialog pFileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Title = "Select Files to Encrypt",
				ReadOnlyChecked = true,
				InitialDirectory = Application.StartupPath,
				Filter = "Textures, UI Structures, Worlds Or Effect.dat (tex, xml, wld)|*.tex;*.xml;*.wld;Effect.dat"
			};

			if (pFileDialog.ShowDialog() == DialogResult.OK)
			{
				Task.Run(() =>
				{
					foreach (string strFilePath in pFileDialog.FileNames)
					{
						using (FileStream Stream = new(strFilePath, FileMode.Open, FileAccess.ReadWrite))
						{
							nKey1Index = 0;
							nKey2Index = 0;
							Buffer = new byte[Stream.Length];

							Stream.ReadExactly(Buffer, 0, Buffer.Length);

							File.Copy(strFilePath, strFilePath + ".bk_dec", true);

							for (int i = 0; i < Buffer.Length; i++)
							{
								Buffer[i] = (byte)(Buffer[i] ^ Defs.EncryptKey1[nKey1Index] ^ Defs.EncryptKey2[nKey2Index]);

								nKey1Index = (nKey1Index + 1) % nKey1Size;
								nKey2Index = (nKey2Index + 1) % nKey2Size;
							}

							Stream.Seek(0, SeekOrigin.Begin);
							Stream.Write(Defs.EncryptFileHeader, 0, nHeaderSize);
							Stream.Write(Buffer, 0, Buffer.Length);

							Logger(LogTypes.Success, $"Main > File Encrypter: File: {strFilePath} Has been encrypted!");
						}
					}
				});
			}
		}

		private void btnFileDecrypter_ClickAsync(object sender, EventArgs e)
		{
			byte[] Buffer;
			bool bContinue;
			int nHeaderSize = Defs.EncryptFileHeader.Length;
			int nKey1Size = Defs.EncryptKey1.Length;
			int nKey2Size = Defs.EncryptKey2.Length;
			OpenFileDialog pFileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Title = "Select Files to Decrypt",
				ReadOnlyChecked = true,
				InitialDirectory = Application.StartupPath,
				Filter = "Textures, UI Structures, Worlds Or Effect.dat (tex, xml, wld)|*.tex;*.xml;*.wld;Effect.dat"
			};

			if (pFileDialog.ShowDialog() == DialogResult.OK)
			{
				Task.Run(() =>
				{
					foreach (string strFilePath in pFileDialog.FileNames)
					{
						using (FileStream Stream = new(strFilePath, FileMode.Open, FileAccess.ReadWrite))
						{
							bContinue = true;
							Buffer = new byte[Stream.Length];

							Stream.ReadExactly(Buffer, 0, Buffer.Length);

							for (int i = 0; i < nHeaderSize; i++)
							{
								if (Buffer[i] != Defs.EncryptFileHeader[i])
								{
									bContinue = false;

									Logger(LogTypes.Error, $"Main > File Decrypter: File: {strFilePath} Wrong file.");

									break;
								}
							}

							if (bContinue)
							{
								File.Copy(strFilePath, strFilePath + ".bk_enc", true);

								for (int i = nHeaderSize; i < Buffer.Length; i++)
									Buffer[i] = (byte)(Buffer[i] ^ Defs.EncryptKey1[(i - nHeaderSize) % nKey1Size] ^ Defs.EncryptKey2[(i - nHeaderSize) % nKey2Size]);

								Stream.Seek(0, SeekOrigin.Begin);
								Stream.Write(Buffer, nHeaderSize, Buffer.Length - nHeaderSize);

								Logger(LogTypes.Success, $"Main > File Decrypter: File: {strFilePath} Has been decrypted!");
							}
						}
					}
				});
			}
		}

		private void btnCompareFiles_Click(object sender, EventArgs e)
		{
			OpenFileDialog pFileDialog = new OpenFileDialog { Multiselect = true, Title = "Select Files to get CRC32", ReadOnlyChecked = true, InitialDirectory = Application.StartupPath };

			if (pFileDialog.ShowDialog() == DialogResult.OK)
			{
				foreach (string strFilePath in pFileDialog.FileNames)
				{
					using (FileStream Stream = File.OpenRead(strFilePath))
						Logger(LogTypes.Other1, $"Main > File: {Path.GetFileName(strFilePath)}\tCRC: " + Crc32Algorithm.Compute(File.ReadAllBytes(strFilePath)));
				}
			}
		}
		/****************************************/
		private void WriteToConsole(string strTimestamp, string strStack, string strMsg, Color cStringColor)
		{
			if (rtbConsole.IsDisposed || !rtbConsole.IsHandleCreated)
				return;

			if (rtbConsole.InvokeRequired)
			{
				rtbConsole.BeginInvoke((MethodInvoker)(() => WriteToConsole(strTimestamp, strStack, strMsg, cStringColor)));
				return;
			}

			rtbConsole.SelectionFont = pConsoleFont;
			rtbConsole.SelectionCharOffset = 0;

			rtbConsole.SelectionColor = Color.White;
			rtbConsole.AppendText(strTimestamp + " ");

			if (strStack.Length > 0)
			{
				rtbConsole.SelectionColor = Color.FromArgb(208, 203, 148);
				rtbConsole.AppendText(strStack + " ");
			}

			rtbConsole.SelectionFont = pConsoleFont;

			rtbConsole.SelectionColor = cStringColor;
			rtbConsole.AppendText(strMsg + Environment.NewLine);

			rtbConsole.ScrollToCaret();
		}

		public void Logger(LogTypes lgType, string strMsg, bool bPrintStackFrame = true)
		{
			try
			{
				string strTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				Color cStringColor = Color.White;
				string strStackFrameInfo = "";

				switch (lgType)
				{
					case LogTypes.Message: cStringColor = Color.White; break;
					case LogTypes.Warning: cStringColor = Color.FromArgb(228, 210, 121); break;
					case LogTypes.Success: cStringColor = Color.FromArgb(27, 134, 45); break;
					case LogTypes.Error: cStringColor = Color.FromArgb(255, 64, 64); break;
					case LogTypes.ServiceLog: cStringColor = Color.Orchid; break;
					case LogTypes.Other1: cStringColor = Color.FromArgb(81, 92, 255); break;
					case LogTypes.Other2: cStringColor = Color.FromArgb(233, 82, 35); break;
				}
#if DEBUG
				if (bPrintStackFrame)
				{
					StackFrame pStackFrame = new(1, true);
					string strFileName = pStackFrame.GetFileName() ?? string.Empty;

					strStackFrameInfo = $"[{Path.GetFileName(strFileName)} : {pStackFrame.GetFileLineNumber()} : {pStackFrame.GetMethod()?.Name}]";
				}
#endif
				lock (LogLock)    // Write to Log file
				{
#if DEBUG
					pStreamWriter.WriteLine(bPrintStackFrame ? $"{strTimestamp} {strStackFrameInfo} > {strMsg}" : $"{strTimestamp} > {strMsg}");
#else
					pStreamWriter.WriteLine($"{strTimestamp} > {strMsg}");
#endif
					// TODO: Capaz es mejor que siempre sea instantaneo si el proyecto está compilado en modo debug
					if (lgType == LogTypes.Error)   // Insta flush if log type is Error
						pStreamWriter.Flush();
				}

				WriteToConsole(strTimestamp, strStackFrameInfo, strMsg, cStringColor);  // Write to Log Console
			}
			catch
			{
				// Do nothing
			}
		}
		/****************************************/
		private void LoadSettings()
		{
			Logger(LogTypes.Message, "Main > Loading Settings...");

			if (!File.Exists(pSettings.SettingsFile))
				File.Copy("Settings.ini.dummy", pSettings.SettingsFile);

			IniData pData = (new FileIniDataParser()).ReadFile(pSettings.SettingsFile);

			// General Settings
			pSettings.DBHost = pData["Settings"]["MySQLHost"];
			pSettings.DBUsername = pData["Settings"]["MySQLUsername"];
			pSettings.DBPassword = pData["Settings"]["MySQLPassword"];
			pSettings.DBAuth = pData["Settings"]["MySQLDBAuth"];
			pSettings.DBData = pData["Settings"]["MySQLDBData"];
			pSettings.DBUser = pData["Settings"]["MySQLDBUser"];
			pSettings.DBCharset = pData["Settings"]["Charset"];

			pSettings.WorkLocale = pData["Settings"]["WorkLocale"].ToLower();

			pSettings.DockToTop = bool.Parse(pData["Settings"]["DockToTop"]);

			pSettings.MaximizeOnStartUp = bool.Parse(pData["Settings"]["MaximizeOnStartUp"]);

			pSettings.ClientPath = pData["Settings"]["ClientPath"];

			string[] strNations = pData["Settings"]["NationSupported"].Split(',');

			pSettings.NationSupported = new string[strNations.Length];

			for (int i = 0; i < strNations.Length; i++)
				pSettings.NationSupported[i] = strNations[i];
			/****************************************/
			pSettings.DeepLURL = pData["Settings"]["DeepLURL"];
			pSettings.DeepLAPIKey = pData["Settings"]["DeepLAPIKey"];
			/****************************************/
			foreach (KeyData pKey in pData["3DViewerDialog"])
				pSettings.Show3DViewerDialog[pKey.KeyName] = bool.Parse(pKey.Value.ToLower());
			/****************************************/
			foreach (KeyData pKey in pData["ChangesAppliedNotification"])
				pSettings.ChangesAppliedNotification[pKey.KeyName] = bool.Parse(pKey.Value.ToLower());

			// Exporter
			foreach (KeyData pKey in pData["Exporter"])
				pSettings.Exporter[pKey.KeyName] = pKey.Value.ToLower();
			/****************************************/
			pSettings.ExportToLocal = bool.Parse(pData["Settings"]["ExportToLocal"]);
			pSettings.LogVerbose = int.Parse(pData["Settings"]["LogVerbose"]);

			// Control Panel
			pSettings.ServerPath = pData["ControlPanel"]["ServerPath"];
			pSettings.TextEditorPath = pData["ControlPanel"]["TextEditorPath"];
			pSettings.NkspLaunchArgument = pData["ControlPanel"]["NkspLaunchArgument"];
			pSettings.AutoReUp = Convert.ToBoolean(pData["ControlPanel"]["AutoReUp"]);
			pSettings.MaxReUpTrys = Convert.ToInt32(pData["ControlPanel"]["MaxReUpTrys"]);
			pSettings.StartCashServerOnStartAll = Convert.ToBoolean(pData["ControlPanel"]["StartCashServerOnStartAll"]);

			foreach (string strService in pSettings.Services)
			{
				List<string> KeyWords = new();

				foreach (KeyData Key in pData["ControlPanelFilters"])
				{
					foreach (string ServiceToLog in Key.Value.Split(',').Select(s => s.Trim()).ToList())
					{
						if (strService == ServiceToLog)
							KeyWords.Add(Key.KeyName);
					}
				}

				pSettings.ServicesData[strService] = new Settings.ServiceDataStruct
				{
					BinaryName = pData["ControlPanel"][strService + "Binary"],
					CatchLog = Convert.ToBoolean(pData["ControlPanel"][strService + "CatchLogs"]),
					FilterKeyWord = new HashSet<string>(KeyWords)
				};
			}
			/****************************************/
			Logger(LogTypes.Success, "Main > Settings: Load finished.");
		}
		/****************************************/
		private void ConnectToDatabase()
		{
			try
			{
				List<Form> listFormsToClose = new();

				foreach (Form pForm in Application.OpenForms)
				{
					if (pForm != this)
						listFormsToClose.Add(pForm);
				}

				foreach (Form pForm in listFormsToClose)
					pForm.Close();
				/****************************************/
				pTables.Dispose();
				/****************************************/
				string strConnect = $"SERVER={pSettings.DBHost};DATABASE={pSettings.DBData};UID={pSettings.DBUsername};PASSWORD={pSettings.DBPassword};CHARSET={pSettings.DBCharset}";

				pMySQLConnection = new MySqlConnection(strConnect);

				Logger(LogTypes.Message, $"MySQL > Trying to connect to ({strConnect})...");

				pMySQLConnection.Open();

				Logger(LogTypes.Success, "MySQL > Connected successfully.");
#if DEBUG
				strWindowsTitle = "Status: DB Connected: " + pSettings.DBData;
#endif
				DataTable? dtResults = QuerySelect(pSettings.DBCharset, "SELECT @@sql_mode;");
				if (dtResults != null && dtResults.Rows.Count > 0)
				{
					string strSQLMode = dtResults.Rows[0][0].ToString() ?? string.Empty;
					string[] strSQLModes = strSQLMode.Split(',');

					if (Array.Exists(strSQLModes, mode => mode.Trim() == "STRICT_TRANS_TABLES"))
					{
						DialogResult pDialogReturn = MessageBox.Show("Your Database have STRICT_TRANS_TABLES Enable. In order to use this ToolBox it should be disabled.\nDo you want disable it now? (The actual Database user need all privileges)", "LastChaos ToolBoxNG", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
						if (pDialogReturn == DialogResult.Yes)
						{
							if (QueryUpdateInsertDelete(pSettings.DBCharset, "SET GLOBAL sql_mode = REPLACE(@@sql_mode, 'STRICT_TRANS_TABLES', '');", out long _))
								MessageBox.Show("STRICT_TRANS_TABLES was disabled successfully.", "LastChaos ToolBoxNG", MessageBoxButtons.OK);
							else
								MessageBox.Show("Failed to disable STRICT_TRANS_TABLES.", "LastChaos ToolBoxNG", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger(LogTypes.Error, "MySQL > " + ex.Message);

				DialogResult pDialogReturn = MessageBox.Show(ex.Message + "\n\nWould you like to retry the connection?", "LastChaos ToolBoxNG", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
				if (pDialogReturn == DialogResult.Yes)
					ConnectToDatabase();
				else
					Environment.Exit(0);
			}
			finally
			{
				pMySQLConnection.Close();
			}
		}

		// General Helper Functions & Classes
		public class ListBoxItem
		{
			public int ID;
			public string Text = string.Empty;
			public override string ToString() { return Text; }
		}

		public class ComboBoxItem
		{
			public int Value;
			public string DisplayText = string.Empty;
			public override string ToString() { return DisplayText; }
		}
		/****************************************/
		private string NormalizeText(string strText) { return new string(strText.Where(c => char.IsLetterOrDigit(c) || char.IsLetterOrDigit(c)).ToArray()).ToLowerInvariant(); }
		private bool ContainsAllTokens(string strText, string[] strTokens) { return strTokens.All(token => NormalizeText(strText).Contains(token)); }

		public int SearchInListBox(TextBox pTextBoxInput, KeyEventArgs pTextBoxEvent, ListBox pListBoxToSearch, int nSearchPosition)
		{
			if (pTextBoxEvent.KeyCode == Keys.Enter)
			{
				pTextBoxEvent.Handled = true;
				pTextBoxEvent.SuppressKeyPress = true;

				int nSelected = pListBoxToSearch.SelectedIndex;
				if (nSelected != -1)
				{
					if (nSelected < nSearchPosition)
						return nSelected;

					string[] searchTokens = NormalizeText(pTextBoxInput.Text).Split(' ', StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < pListBoxToSearch.Items.Count; i++)
					{
						if (ContainsAllTokens(pListBoxToSearch.GetItemText(pListBoxToSearch.Items[i]) ?? string.Empty, searchTokens) && i > nSearchPosition)
						{
							pListBoxToSearch.SetSelected(i, true);

							return i;
						}
					}

					for (int i = 0; i <= nSearchPosition; i++)
					{
						if (ContainsAllTokens(pListBoxToSearch.GetItemText(pListBoxToSearch.Items[i]) ?? string.Empty, searchTokens))
						{
							pListBoxToSearch.SetSelected(i, true);

							return i;
						}
					}
				}
			}

			return nSearchPosition;
		}

		public int SearchInGridView(TextBox pTextBoxInput, KeyEventArgs pTextBoxEvent, DataGridView pGridViewToSearch, int nSearchPosition)
		{
			if (pTextBoxEvent.KeyCode == Keys.Enter)
			{
				pTextBoxEvent.Handled = true;
				pTextBoxEvent.SuppressKeyPress = true;

				int i = 0;
				string[] searchTokens = NormalizeText(pTextBoxInput.Text).Split(' ', StringSplitOptions.RemoveEmptyEntries);

				foreach (DataGridViewRow row in pGridViewToSearch.Rows)
				{
					if (ContainsAllTokens(row.Cells["item"].Value.ToString(), searchTokens) && i > nSearchPosition)
					{
						pGridViewToSearch.FirstDisplayedScrollingRowIndex = row.Index;
						row.Selected = true;
						nSearchPosition = row.Index;

						return i;
					}

					i++;
				}

				for (i = 0; i <= nSearchPosition; i++)
				{
					if (ContainsAllTokens(pGridViewToSearch.Rows[i].Cells["item"].Value.ToString(), searchTokens))
					{
						pGridViewToSearch.FirstDisplayedScrollingRowIndex = pGridViewToSearch.Rows[i].Index;
						pGridViewToSearch.Rows[i].Selected = true;
						nSearchPosition = pGridViewToSearch.Rows[i].Index;

						return i;
					}
				}
			}

			return nSearchPosition;
		}

		/*public int SearchInListView(TextBox pTextBoxInput, KeyEventArgs pTextBoxEvent, ListView pListViewToSearch, int nSearchPosition)	// not used
		{
			if (pTextBoxEvent.KeyCode == Keys.Enter)
			{
				pTextBoxEvent.Handled = true;
				pTextBoxEvent.SuppressKeyPress = true;

				int nSelected = pListViewToSearch.SelectedIndices[0];

				if (nSelected != -1)
				{
					if (nSelected < nSearchPosition)
						return nSelected;

					string[] searchTokens = NormalizeText(pTextBoxInput.Text).Split(' ', StringSplitOptions.RemoveEmptyEntries);

					void SelectItem(int nIndex)
					{
						pListViewToSearch.SelectedItems.Clear();

						var pItem = pListViewToSearch.Items[nIndex];
						pItem.Selected = true;
						pItem.EnsureVisible();
					}

					for (int i = 0; i < pListViewToSearch.Items.Count; i++)
					{
						if (ContainsAllTokens(pListViewToSearch.Items[i].Text, searchTokens) && i > nSearchPosition)
						{
							SelectItem(i);
							return i;
						}
					}

					for (int i = 0; i <= nSearchPosition; i++)
					{
						if (ContainsAllTokens(pListViewToSearch.Items[i].Text, searchTokens))
						{
							SelectItem(i);
							return i;
						}
					}
				}
			}

			return nSearchPosition;
		}*/
		/****************************************/
		public int AskForIndex(string strEditorName, string strPrimaryKeyColumnName)
		{
			DialogResult pDialogReturn = MessageBox.Show($"The database was queried for the highest {strPrimaryKeyColumnName} value, but failed. You will have to enter a value yourself, do you want to continue?", strEditorName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (pDialogReturn == DialogResult.Yes)
			{
				MessageBox_Input pInput = new(this, $"Please enter a {strEditorName.Replace(" Editor", "")} ID:"); // Hardcode!

				if (pInput.ShowDialog() != DialogResult.OK)
				{
					pDialogReturn = MessageBox.Show("Do you want to cancel?", strEditorName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (pDialogReturn == DialogResult.Yes)
						return -1;
					else
						return AskForIndex(strEditorName, strPrimaryKeyColumnName);
				}
				else
				{
					return Convert.ToInt32(pInput.strOutput);
				}
			}

			return -1;
		}
		/****************************************/
		public void WriteToINI(string strFilePath, string strGroupName, string strKeyName, string strValue)
		{
			FileIniDataParser pParser = new();
			IniData pData = pParser.ReadFile(strFilePath);

			pData[strGroupName][strKeyName] = strValue;

			pParser.WriteFile(strFilePath, pData);
		}
		/****************************************/
		public string EscapeChars(string strInput)
		{
			strInput = strInput.Replace("\\", "\\\\");  // Escape \ to \\\\
			strInput = strInput.Replace("'", "\\'");    // Escape ' to \\'
			strInput = strInput.Replace("\r", "\\\\r"); // Escape \r to \\r
			strInput = strInput.Replace("\n", "\\\\n"); // Escape \n to \\n

			return strInput;
		}
		/****************************************/
		public Bitmap? GetIcon(string strBtnType, string strImage, int nRow, int nCol)
		{
			int nSize = 32;

			if (strBtnType == "Elemental")
				nSize = 38;

			if (strBtnType == "ComboBtn")
				nSize = 50;

			string strComposePath = $"Resources\\{strBtnType + strImage}.png";

			if (strBtnType == "Elemental")
				strComposePath = $"Resources\\{strBtnType}.png";

			if (File.Exists(strComposePath))
			{
				using (Image pImage = Image.FromFile(strComposePath))
				{
					// Create new Bitmap
					Bitmap pBitmap = new(nSize, nSize);
					// Generate Bitmap content
					using (Graphics pGraphics = Graphics.FromImage(pBitmap))
						pGraphics.DrawImage(pImage, new Rectangle(0, 0, nSize, nSize), new Rectangle(nCol * nSize, nRow * nSize, nSize, nSize), GraphicsUnit.Pixel);

					return pBitmap;
				}
			}
			else
			{
				Logger(LogTypes.Error, "Main > Error while trying to get Icon Path: " + strComposePath);

				return null;
			}
		}

		public (Bitmap, float) GetWorldMap(string strMapID)
		{
			int nSize = 512;
			float fRatio = Defs.WorldRatio;
			string strComposePath = $"Resources\\Map_World{strMapID}.png";

			if (File.Exists(strComposePath))
			{
				using (Image pImage = Image.FromFile(strComposePath))
				{
					// Create new Bitmap
					Bitmap pBitmap = new(nSize, nSize);
					// Generate Bitmap content
					using (Graphics pGraphics = Graphics.FromImage(pBitmap))
						pGraphics.DrawImage(pImage, new Rectangle(0, 0, nSize, nSize), new Rectangle(0, 0, pImage.Width, pImage.Height), GraphicsUnit.Pixel);

					if (pImage.Width == 1024 && pImage.Height == 1024)
						fRatio *= 2;

					return (pBitmap, fRatio);
				}
			}
			else
			{
				Logger(LogTypes.Error, "Main > Error while trying to get World Map Path: " + strComposePath);

				return (new Bitmap(1, 1), 0f);
			}
		}

		public Color GetGoldColor(long lNas)
		{
			Color cReturn = Color.Black;

			if (lNas < 1000)
				cReturn = Color.FromArgb(255, 255, 255);
			else if (lNas < 1000000)
				cReturn = Color.FromArgb(0, 255, 255);  // 1,000,000
			else if (lNas < 1000000000)
				cReturn = Color.FromArgb(0, 255, 0);    // 1,000,000,000
			else if (lNas < 1000000000000)
				cReturn = Color.FromArgb(255, 255, 0);  // 1,000,000,000,000
			else if (lNas >= 1000000000000)
				cReturn = Color.FromArgb(255, 204, 0);  // 1,000,000,000,000+

			return cReturn;
		}

		public int AT_MIX(int a, int l) { return (((l) & Defs.AT_MASK) << Defs.AT_LVVEC) | ((a) & Defs.AT_MASK); }
		public int GET_AT_VAR(int m) { return m & Defs.AT_MASK; }
		public int GET_AT_LV(int m) { return ((m >> Defs.AT_LVVEC) & Defs.AT_MASK); }
		public int AT_ADMIX(int a, int d) { return ((a & Defs.AT_AD_MASK) << Defs.AT_ADVEC) | (d & Defs.AT_AD_MASK); }
		public byte GET_AT_DEF(int m) { return (byte)(m & Defs.AT_AD_MASK); }
		public byte GET_AT_ATT(int m) { return (byte)((m >> Defs.AT_ADVEC) & Defs.AT_AD_MASK); }
		/****************************************/
		async Task<bool> CheckInternetConnection()
		{
			try
			{
				using (HttpClient httpClient = new())
				{
					httpClient.Timeout = TimeSpan.FromSeconds(3);

					using (HttpResponseMessage httpResponse = await httpClient.GetAsync("https://www.google.com", HttpCompletionOption.ResponseHeadersRead))
						return httpResponse.IsSuccessStatusCode;
				}
			}
			catch
			{
				return false;
			}
		}

		// Database Functions
		public DataTable? QuerySelect(string strCharset, string strQuery, bool bLogSuccess = true)
		{
			try
			{
				string strConnect = $"SERVER={pSettings.DBHost};DATABASE={pSettings.DBData};UID={pSettings.DBUsername};PASSWORD={pSettings.DBPassword};CHARSET={strCharset}";

				using (MySqlConnection MySQLConnection = new(strConnect))
				{
					MySQLConnection.Open();

					using (MySqlCommand MySQLCommand = new(strQuery, MySQLConnection))
					{
						using (MySqlDataReader MySQLDataReader = MySQLCommand.ExecuteReader())
						{
							DataTable? pTable = new();
							pTable.Load(MySQLDataReader);

							if (bLogSuccess)
								Logger(LogTypes.Success, $"MySQL > Query (Charset: {strCharset})\r{strQuery.Replace(";", ";\r")}Execute successfully.");

							return pTable;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger(LogTypes.Error, $"MySQL > Query (Charset: {strCharset})\r{strQuery.Replace(";", ";\r")}Fail > {ex.Message}.");

				return null;
			}
		}

		public bool QueryUpdateInsertDelete(string strCharset, string strQuery, out long lLastInsertID, bool bLogSuccess = true)
		{
			lLastInsertID = -1;

			try
			{
				string strConnect = $"SERVER={pSettings.DBHost};DATABASE={pSettings.DBData};UID={pSettings.DBUsername};PASSWORD={pSettings.DBPassword};CHARSET={strCharset}";

				using (MySqlConnection MySQLConnection = new(strConnect))
				{
					MySQLConnection.Open();

					using (MySqlCommand MySQLCommand = new(strQuery, MySQLConnection))
					{
						MySQLCommand.ExecuteNonQuery();

						lLastInsertID = MySQLCommand.LastInsertedId;

						if (bLogSuccess)
							Logger(LogTypes.Success, $"MySQL > Query (Charset: {strCharset})\r{strQuery.Replace(";", ";\r")}Execute successfully.");

						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Logger(LogTypes.Error, $"MySQL > Query (Charset: {strCharset})\r{strQuery.Replace(";", ";\r")}Fail > {ex.Message}.");

				return false;
			}
		}

		public void MergeDataTables(DataTable? dtSource, string strPrimaryKey, ref DataTable dtDestination)  // NOTE: Primary Key is hardcoded to DataType Int
		{
			if (dtSource == null)
				return;
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			// Detect new Columns
			List<DataColumn> pNewColumns = new();

			foreach (DataColumn col in dtSource.Columns)
			{
				if (!dtDestination.Columns.Contains(col.ColumnName))
					pNewColumns.Add(col);
			}

			// Count current columns to check if need add News
			int nCountColumnsBefore = dtDestination.Columns.Count;

			// Use Dictionary instead of work directly over the DataTable
			Dictionary<int, object?[]> pCache = new(dtDestination.Rows.Count);

			// Copy Rows from DataTable to Dictionary
			foreach (DataRow pRow in dtDestination.Rows)
				pCache[Convert.ToInt32(pRow[strPrimaryKey])] = pRow.ItemArray;  // I prefer convert instead of cast. It is fast enough
#if DEBUG
			stopwatch.Stop();
			Logger(LogTypes.Message, $"Build Cache Dictionary took: {stopwatch.ElapsedMilliseconds}ms.");

			stopwatch = Stopwatch.StartNew();
#endif
			// Add new Columns to DataTable
			foreach (DataColumn newColumn in pNewColumns)
				dtDestination.Columns.Add(newColumn.ColumnName, newColumn.DataType);

			// Count new total of Columns
			int nCountColumnsAfter = dtDestination.Columns.Count;

			// Add new Columns to Cache Dictionary
			if (nCountColumnsAfter > nCountColumnsBefore)
			{
				foreach (var pObject in pCache)
				{
					var pOldValues = pObject.Value;

					if (pOldValues.Length < nCountColumnsAfter)
					{
						var pNewValues = new object[nCountColumnsAfter];
						Array.Copy(pOldValues, pNewValues, pOldValues.Length);

						pCache[pObject.Key] = pNewValues;
					}
				}
			}
#if DEBUG
			stopwatch.Stop();
			Logger(LogTypes.Message, $"Add new Columns to Cache took: {stopwatch.ElapsedMilliseconds}ms.");

			stopwatch = Stopwatch.StartNew();
#endif
			// Merge new Rows and Columns values to Cache Dictionary
			Dictionary<string, int> pColumnOrdinals = dtDestination.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName, c => c.Ordinal);

			foreach (DataRow newRow in dtSource.Rows)
			{
				int nKey = Convert.ToInt32(newRow[strPrimaryKey]);  // I prefer convert instead of cast. It is fast enough

				if (!pCache.TryGetValue(nKey, out var pValues))
				{
					// Add new Row
					var newValues = new object[nCountColumnsAfter];

					foreach (DataColumn pNewCol in dtSource.Columns)
						newValues[pColumnOrdinals[pNewCol.ColumnName]] = newRow[pNewCol];

					pCache[nKey] = newValues;
				}
				else
				{
					// Row exist previously but needs add new Columns
					foreach (var col in pNewColumns)
						pValues[pColumnOrdinals[col.ColumnName]] = newRow[col.ColumnName];
				}
			}
#if DEBUG
			stopwatch.Stop();
			Logger(LogTypes.Message, $"Dictionary merge took: {stopwatch.ElapsedMilliseconds}ms.");

			stopwatch = Stopwatch.StartNew();
#endif
			// Rebuild DataTable
			DataTable pNewDataTable = dtDestination.Clone();

			pNewDataTable.BeginLoadData();

			foreach (var pNewValues in pCache.Values)
				pNewDataTable.Rows.Add(pNewValues);

			pNewDataTable.EndLoadData();

			// Transfer to Global
			dtDestination = pNewDataTable;
#if DEBUG
			stopwatch.Stop();
			Logger(LogTypes.Message, $"Rebuild DataTable took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
		}

		// Generic DataTables loaders (NOTE: These functions are designed to request only the necessary columns that are commonly used. It might be tempting to adapt them to load at least as many columns as the pickers, but that's not the purpose.)
		public async Task GenericLoadZoneDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_name"];

			if (pTables.ZoneTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pTables.ZoneTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return QuerySelect(pSettings.DBCharset, $"SELECT a_zone_index, {string.Join(", ", listQueryCompose)} FROM {pSettings.DBData}.t_zonedata ORDER BY a_zone_index;");
				});

				if (pTables.ZoneTable == null)
					pTables.ZoneTable = pNewTable;
				else
					MergeDataTables(pNewTable, "a_zone_index", ref pTables.ZoneTable);
			}
		}

		public async Task GenericLoadStringDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_string_" + pSettings.WorkLocale];

			if (pTables.StringTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pTables.StringTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return QuerySelect(pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pSettings.DBData}.t_string ORDER BY a_index;");
				});

				if (pTables.StringTable == null)
					pTables.StringTable = pNewTable;
				else
					MergeDataTables(pNewTable, "a_index", ref pTables.StringTable);
			}
		}

		public async Task GenericLoadSkillDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose =
			[
				"a_name_" + pSettings.WorkLocale,
				"a_client_description_" + pSettings.WorkLocale,
				"a_client_icon_texid",
				"a_client_icon_row",
				"a_client_icon_col"
			];

			if (pTables.SkillTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pTables.SkillTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return QuerySelect(pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pSettings.DBData}.t_skill ORDER BY a_index;");
				});

				if (pTables.SkillTable == null)
					pTables.SkillTable = pNewTable;
				else
					MergeDataTables(pNewTable, "a_index", ref pTables.SkillTable);
			}
		}

		public async Task GenericLoadSkillLevelDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_level", "a_dummypower"];

			if (pTables.SkillLevelTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pTables.SkillLevelTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return QuerySelect(pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pSettings.DBData}.t_skilllevel ORDER BY a_level;");
				});

				if (pTables.SkillLevelTable == null)
					pTables.SkillLevelTable = pNewTable;
				else
					MergeDataTables(pNewTable, "a_index", ref pTables.SkillLevelTable);
			}
		}

		public async Task GenericLoadQuestDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_need_min_level", "a_need_max_level", "a_name_" + pSettings.WorkLocale, "a_desc_" + pSettings.WorkLocale];

			if (pTables.QuestTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pTables.QuestTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return QuerySelect(pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pSettings.DBData}.t_quest ORDER BY a_index;");
				});

				if (pTables.QuestTable == null)
					pTables.QuestTable = pNewTable;
				else
					MergeDataTables(pNewTable, "a_index", ref pTables.QuestTable);
			}
		}

		public async Task GenericLoadItemDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_texture_id", "a_texture_row", "a_texture_col", "a_name_" + pSettings.WorkLocale, "a_descr_" + pSettings.WorkLocale, "a_level"];

			if (pTables.ItemTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pTables.ItemTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return QuerySelect(pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pSettings.DBData}.t_item ORDER BY a_index;");
				});

				if (pTables.ItemTable == null)
					pTables.ItemTable = pNewTable;
				else
					MergeDataTables(pNewTable, "a_index", ref pTables.ItemTable);
			}
		}

		public async Task GenericLoadNPCDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_level", "a_hp", "a_name_" + pSettings.WorkLocale, "a_descr_" + pSettings.WorkLocale];

			if (pTables.NPCTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pTables.NPCTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return QuerySelect(pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pSettings.DBData}.t_npc ORDER BY a_index;");
				});

				if (pTables.NPCTable == null)
					pTables.NPCTable = pNewTable;
				else
					MergeDataTables(pNewTable, "a_index", ref pTables.NPCTable);
			}
		}

		public async Task GenericLoadOptionDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_type", "a_level", "a_prob", "a_name_" + pSettings.WorkLocale];

			if (pTables.OptionTable == null)
			{
				bRequestNeeded = true;
			}
			else
			{
				foreach (string strColumnName in listQueryCompose.ToList())
				{
					if (!pTables.OptionTable.Columns.Contains(strColumnName))
						bRequestNeeded = true;
					else
						listQueryCompose.Remove(strColumnName);
				}
			}

			if (bRequestNeeded)
			{
				DataTable? pNewTable = await Task.Run(() =>
				{
					return QuerySelect(pSettings.DBCharset, $"SELECT a_index, {string.Join(", ", listQueryCompose)} FROM {pSettings.DBData}.t_option ORDER BY a_index;");
				});

				if (pTables.OptionTable == null)
					pTables.OptionTable = pNewTable;
				else
					MergeDataTables(pNewTable, "a_index", ref pTables.OptionTable);
			}
		}
		/*********************************************************************************/
		/*public static int CParticleGroupManager_CURRENT_VERSION = 1;            // Standard
		public static int CParticleGroup_CURRENT_VERSION = 1;                   // Standard
		public static int CParticlesProcessDynamicState_CURRENT_VERSION = 5;    // Standard

		public struct VectorCommonProcess
		{
			public long Temp;
			public byte Version;
		}

		private class CParticleGroup
		{
			public string TagID;
			public byte Version;
			public string Name;
			public string FilePath;
			public long RenderType;
			public long BlendType;
			public long MexWith;
			public long MexHeight;
			public long iCol;
			public long iRow;
			public long VectorCommonProcessGroupCount;
			public string VectorCommonProcessGroupTagID;
			public Dictionary<long, VectorCommonProcess> VectorCommonProcessGroups;

			public CParticleGroup() // Initialize ParticleGroups in the constructor
			{
				VectorCommonProcessGroups = new Dictionary<long, VectorCommonProcess>();
				// TODO: inicializar el resto de grupos.
			}
		};

		class EffectFileData
		{
			public string MainTagID;
			public string SubTagID;
			public string ParticleGroupTagID;
			public byte ParticleGroupVersion;
			public long ParticleGroupCount;
			public Dictionary<long, CParticleGroup> ParticleGroups;

			// Initialize ParticleGroups in the constructor
			public EffectFileData()
			{
				ParticleGroups = new Dictionary<long, CParticleGroup>();
				// TODO: inicializar el resto de grupos.
			}
		};*/

		private void button1_Click(object sender, EventArgs e)  // TODO: this whole function xd
		{
			/*string ReadString(BinaryReader BinaryReader, int nStringLength = 0)
			{
				if (nStringLength > 0)
					return Encoding.UTF8.GetString(BinaryReader.ReadBytes(nStringLength));
				else
					return Encoding.UTF8.GetString(BinaryReader.ReadBytes(BinaryReader.ReadInt32()));
			}

			string ReadLine(BinaryReader BinaryReader)
			{
				byte ReadedByte;
				StringBuilder strBuilder = new();

				while (BinaryReader.BaseStream.Position < BinaryReader.BaseStream.Length)
				{
					ReadedByte = BinaryReader.ReadByte();

					if (ReadedByte == 0x0D) // \r
					{
						if (BinaryReader.PeekChar() == 0x0A)    // Check if next char is equal to \n
							BinaryReader.ReadByte(); // Read junk: 0x0A = \n

						break;
					}

					//if (ReadedByte == 0x0A) // just in case of another \n
					//break;

					strBuilder.Append((char)ReadedByte);
				}

				return strBuilder.ToString();
			}
			// OpenFileDialog openFileDialog = new OpenFileDialog();
			// openFileDialog.Multiselect = true;
			// openFileDialog.Title = "Select files to Encrypt";
			// openFileDialog.ReadOnlyChecked = true;
			// openFileDialog.InitialDirectory = Application.StartupPath;
			// openFileDialog.Filter = "Textures, UI Structures, Worlds Or Effect.dat (tex, xml, wld)|*.tex;*.xml;*.wld;Effect.dat";
			
			//if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				Task.Run(() =>
				{
					//foreach (string strFilePath in openFileDialog.FileNames)
					{
						//using (FileStream Stream = new(strFilePath, FileMode.Open, FileAccess.ReadWrite))
						using (BinaryReader BinaryReader = new(File.Open("Effect.dat", FileMode.Open)))
						{
							EffectFileData pData = new();

							pData.MainTagID = ReadString(BinaryReader, 4);

							if (pData.MainTagID == "EFTB")
							{
								pData.SubTagID = ReadString(BinaryReader, 4);

								if (pData.SubTagID == "EPGM")
								{
									pData.ParticleGroupTagID = ReadString(BinaryReader, 4);

									if (pData.ParticleGroupTagID == "PGMG")
									{
										pData.ParticleGroupVersion = BinaryReader.ReadByte();

										if (pData.ParticleGroupVersion == CParticleGroupManager_CURRENT_VERSION)
										{
											pData.ParticleGroupCount = BinaryReader.ReadInt64();

											long lIter = 0;
											string strTagID;
											while (lIter < pData.ParticleGroupCount)
											{
												CParticleGroup pParticleGroup = new CParticleGroup();   // TODO: Probar declarar antes de cualquier loop y limpiar al momento de usar, quizas la mejora vale la pena

												pParticleGroup.TagID = ReadString(BinaryReader, 4);

												if (pParticleGroup.TagID == "PTGR")
												{
													pParticleGroup.Version = BinaryReader.ReadByte();

													if (pParticleGroup.Version == CParticleGroup_CURRENT_VERSION)
													{
														pParticleGroup.Name = ReadLine(BinaryReader);

														BinaryReader.ReadBytes(4);  // Read junk: 0x44 0x46 0x4E 0x4D = DNM

														pParticleGroup.FilePath = ReadString(BinaryReader);

														MessageBox.Show(pParticleGroup.Name + "'", pParticleGroup.FilePath + "'");//####

														pParticleGroup.RenderType = BinaryReader.ReadInt64();
														pParticleGroup.BlendType = BinaryReader.ReadInt64();
														pParticleGroup.MexWith = BinaryReader.ReadInt64();
														pParticleGroup.MexHeight = BinaryReader.ReadInt64();
														pParticleGroup.iCol = BinaryReader.ReadInt64();
														pParticleGroup.iRow = BinaryReader.ReadInt64();

														pParticleGroup.VectorCommonProcessGroupCount = BinaryReader.ReadInt64();

														for (long i = 0; i < pParticleGroup.VectorCommonProcessGroupCount; i++)
														{
															VectorCommonProcess pVectorCommonProcessGroup = new();  // TODO: Probar declarar antes de cualquier loop y limpiar al momento de usar, quizas la mejora vale la pena
															pVectorCommonProcessGroup.Temp = BinaryReader.ReadInt64();
															strTagID = ReadString(BinaryReader, 4);

															if (pParticleGroup.TagID == "PPDS")
															{
																pParticleGroup.VectorCommonProcessGroupTagID = strTagID;    // To not repeat it each time in VectorCommonProcess
																pVectorCommonProcessGroup.Version = BinaryReader.ReadByte();

																if (pVectorCommonProcessGroup.Version == CParticlesProcessDynamicState_CURRENT_VERSION)
																{

																}   // TODO: rest of versions? i'm not sure if they read differents versions and then normalize to newer one
															}

															pParticleGroup.VectorCommonProcessGroups.Add(i, pVectorCommonProcessGroup);
														}
													}   // TODO: Print error
												}   // TODO: Print error

												pData.ParticleGroups.Add(lIter, pParticleGroup);

												lIter++;
											}
										}   // TODO: Print error
									}   // TODO: Print error
								}   // TODO: Print error
							}   // TODO: Print error
						}
					}
				});
			}*/
		}
	}
}
