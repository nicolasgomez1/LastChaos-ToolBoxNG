namespace LastChaos_ToolBoxNG
{
	public partial class ControlPanel : Form
	{
		private readonly Main pMain;
		private ContextMenuStrip? pContextMenu;

		public ControlPanel(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
			/****************************************/
			tbServerFolder.Text = pMain.pSettings.ServerPath;
			tbTextEditor.Text = pMain.pSettings.TextEditorPath;
			cbAutoReUp.Checked = pMain.pSettings.AutoReUp;

			tbDBHost.Text = pMain.pSettings.DBHost;
			tbDBUsername.Text = pMain.pSettings.DBUsername;
			tbDBPassword.Text = pMain.pSettings.DBPassword;
			tbDBAuth.Text = pMain.pSettings.DBAuth;
			tbDBData.Text = pMain.pSettings.DBData;
			tbDBUser.Text = pMain.pSettings.DBUser;
		}

		private void ServicesControlPanel_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (pContextMenu != null)
			{
				pContextMenu.Dispose();
				pContextMenu = null;
			}
		}

		// Services
		private string GetLogFileName(string strService)
		{
			switch (strService)
			{
				case "Connector": strService = "Connector_1"; break;        // Hardcode!
				case "Helper": strService = "Helper_1"; break;              // Hardcode!
				case "SubHelper": strService = "SubHelper_1"; break;        // Hardcode!
				case "GameServer1": strService = "GameServer_1_1"; break;   // Hardcode!
				case "GameServer2": strService = "GameServer_2_1"; break;   // Hardcode!
				case "GameServer3": strService = "GameServer_3_1"; break;   // Hardcode!
				case "GameServer4": strService = "GameServer_4_1"; break;   // Hardcode!
			}

			return strService;
		}

		private string GetLogFilePath(string strService)
		{
			string[] LogFiles = Directory.GetFiles(pMain.pSettings.ServerPath, DateTime.Now.ToString("yyyyMMdd") + $"_{GetLogFileName(strService)}.log", SearchOption.AllDirectories);

			return LogFiles.Length > 0 ? LogFiles[0] : "";
		}

		private void ServiceRunner(string strService, bool bStart)
		{
			Task.Run(() =>
			{
				try
				{
					Process? pProcess = null;

					if (bStart)
					{
						pProcess = new Process();
						pProcess.StartInfo.FileName = pMain.pSettings.ServicesData[strService].BinaryPath;
						pProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(pMain.pSettings.ServicesData[strService].BinaryPath);
						pProcess.StartInfo.UseShellExecute = false;
						pProcess.StartInfo.CreateNoWindow = true;	// Show or hide Window on startup
						pProcess.Start();
					}
					else
					{
						pProcess = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.Equals(Path.GetFileNameWithoutExtension(pMain.pSettings.ServicesData[strService].BinaryName), StringComparison.OrdinalIgnoreCase));

						if (pProcess != null)
							pMain.Logger(LogTypes.Message, $"Control Panel > Service: {pMain.pSettings.ServicesData[strService].BinaryName} Found running in background.");
						else
							return;
					}

					pMain.pSettings.ServicesData[strService].ProcessID = pProcess.Id;

					if (bStart)
						pMain.Logger(LogTypes.Success, $"Control Panel > Service: {pMain.pSettings.ServicesData[strService].BinaryName} Started.");

					Invoke((Action)(() =>
					{
						((Label)Controls.Find($"lb{strService}Status", true)[0]).BackColor = Color.FromArgb(56, 85, 54);

						if (File.Exists(pMain.pSettings.ServicesData[strService].ConfigPath))
						{
							IniData pData = (new FileIniDataParser()).ReadFile(pMain.pSettings.ServicesData[strService].ConfigPath);

							((RichTextBox)Controls.Find($"rtb{strService}IPPort", true)[0]).Text = pData["Server"]["IP"] + ":" + pData["Server"]["Port"];
						}

						((Button)Controls.Find($"btn{strService}StartStop", true)[0]).Text = "Stop";

						btnStartStopAll.Text = pMain.pSettings.Services.Any(strCheckService => pMain.pSettings.ServicesData[strCheckService].ProcessID != -1) ? "Stop All" : "Start All";
					}));

					string? strLogPath = null;

					while (string.IsNullOrEmpty(strLogPath))    // Cuz file may be cannot exist yet
						strLogPath = GetLogFilePath(strService);

					if (!string.IsNullOrEmpty(strLogPath))
					{
						Task.Run(async () =>
						{
							while (!pProcess.HasExited)
							{
								try
								{
									using (FileStream pFileStream = new(strLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
									using (StreamReader pStreamReader = new(pFileStream, Encoding.UTF8))
									{
										pFileStream.Seek(0, SeekOrigin.End);

										while (!pProcess.HasExited)
										{
											string strLine;
											bool bHasReadLine = false;

											while ((strLine = await pStreamReader.ReadLineAsync()) != null)
											{
												bHasReadLine = true;

												if (pMain.pSettings.ServicesData[strService].CatchLog && !string.IsNullOrEmpty(strLine))
												{
													if (pMain.pSettings.ServicesData[strService].FilterKeyWord.Any(keyword => strLine.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
														pMain.Logger(LogTypes.ServiceLog, $"{strService} {strLine}", false);
												}
											}

											string strPossibleNewLogPath = GetLogFilePath(strService);
											if (strPossibleNewLogPath != strLogPath)
											{
												strLogPath = strPossibleNewLogPath;
												break;
											}

											if (!bHasReadLine)
												await Task.Delay(500);  // Read log file interval if have no new lines readed
										}
									}
								}
								catch (IOException)
								{
									await Task.Delay(1000);
								}
							}
						});
					}

					pProcess.WaitForExit();

					if (pProcess.HasExited && pMain.pSettings.ServicesData[strService].ProcessID != -1)
					{
						pMain.pSettings.ServicesData[strService].ProcessID = -1;

						pMain.Logger(LogTypes.Error, $"Control Panel > Service: {pMain.pSettings.ServicesData[strService].BinaryName} Stopped unexpectedly.");

						if (pMain.pSettings.AutoReUp)
						{
							if (pMain.pSettings.ServicesData[strService].ReUpTrysCount < pMain.pSettings.MaxReUpTrys)
							{
								pMain.Logger(LogTypes.Warning, $"Control Panel > Service: {pMain.pSettings.ServicesData[strService].BinaryName} Re-Up System > Try: {(pMain.pSettings.ServicesData[strService].ReUpTrysCount + 1)}/{pMain.pSettings.MaxReUpTrys}...");

								pMain.pSettings.ServicesData[strService].ReUpTrysCount++;

								Invoke((Action)(() => { ((Label)Controls.Find($"lb{strService}Status", true)[0]).BackColor = Color.FromArgb(228, 210, 121); }));

								ServiceRunner(strService, true);

								return;
							}
							else
							{
								pMain.Logger(LogTypes.Warning, $"Control Panel > Service: {pMain.pSettings.ServicesData[strService].BinaryName} Re-Up System > Out of Re-Up Tries.");
							}
						}

						Invoke((Action)(() =>
						{
							((Label)Controls.Find($"lb{strService}Status", true)[0]).BackColor = Color.FromArgb(218, 54, 51);

							((Button)Controls.Find($"btn{strService}StartStop", true)[0]).Text = "Start";

							btnStartStopAll.Text = pMain.pSettings.Services.Any(strCheckService => pMain.pSettings.ServicesData[strCheckService].ProcessID != -1) ? "Stop All" : "Start All";
						}));
					}
				}
				catch (Exception ex)
				{
					pMain.Logger(LogTypes.Error, $"Control Panel > Service: {pMain.pSettings.ServicesData[strService].BinaryName} Cannot Start: {ex.Message}.");
				}
			});
		}

		private void ServiceStopped(string strService, object ObjSender)
		{
			Process pProcess = Process.GetProcessById(pMain.pSettings.ServicesData[strService].ProcessID);

			if (pProcess != null)
			{
				pProcess.Kill();

				pMain.pSettings.ServicesData[strService].ProcessID = -1;

				pMain.Logger(LogTypes.Success, $"Control Panel > Service: {pMain.pSettings.ServicesData[strService].BinaryName} Stopped.");

				(ObjSender as Button).Text = "Start";

				((Label)Controls.Find($"lb{strService}Status", true)[0]).BackColor = Color.FromArgb(40, 40, 40);

				btnStartStopAll.Text = pMain.pSettings.Services.Any(strCheckService => pMain.pSettings.ServicesData[strCheckService].ProcessID != -1) ? "Stop All" : "Start All";
			}
		}

		private void ServicesControlPanel_Load(object sender, EventArgs e)
		{
			Screen? pScreen = Screen.PrimaryScreen;
			if (pScreen == null)
				return;

			Rectangle recScreenBounds = pScreen.WorkingArea;
			this.Location = new Point(recScreenBounds.Right - this.Width, recScreenBounds.Bottom - this.Height);

			ToolTip pToolTip = new();
			pToolTip.SetToolTip(btnLaunchGame, "Right Click to Launch LC.exe");
			pToolTip.SetToolTip(btnStartStopAll, "Right Click to Enable/Disable Start CashServer when Start All");
			pToolTip.SetToolTip(tbMessengerSubHelperPort, "SubHelper Port inside of Messenger Service it self");

			foreach (string strService in pMain.pSettings.Services)
				pToolTip.SetToolTip((Button)Controls.Find($"btn{strService}Logs", true)[0], "Left Click to Enable/Disable Logs catching. Right Click to see more Options");

			Button btnObj;

			pMain.pSettings.TotalGameServers = 0;

			string strIP, strPort;
			int nSaltsChecked = 0, /*nIntegrationServerChecked = 0,*/ nHardcoreChecked = 0, nAllowedExternalIPChecked = 0;
			string[] strSalts = { "", "" };
			//string[] strIntegrationValues = { "", "" };
			string[] strHardcoreValues = { "", "", "", "", "" };
			string[] strAllowedExternalIPValues = { "", "", "", "", "" };

			foreach (string strService in pMain.pSettings.Services)
			{
				((RichTextBox)Controls.Find($"rtb{strService}IPPort", true)[0]).SelectionAlignment = HorizontalAlignment.Center;

				string[] BinaryFilesFound = Directory.GetFiles(pMain.pSettings.ServerPath, pMain.pSettings.ServicesData[strService].BinaryName, SearchOption.AllDirectories);

				if (BinaryFilesFound.Length > 0)
				{
					if (strService.Contains("GameServer"))
						pMain.pSettings.TotalGameServers++;

					if (BinaryFilesFound.Length > 1)
					{
						string[] Paths = new string[BinaryFilesFound.Length];

						for (int i = 0; i < BinaryFilesFound.Length; i++)
							Paths[i] = BinaryFilesFound[i].Replace(pMain.pSettings.ServerPath, "");

						while (BinaryFilesFound.Length > 1)
						{
							MessageBox_ComboBox pComboBox = new(this, "There are multiple Executables with the same name, please select which one you want to Disable:", Paths);
							if (pComboBox.ShowDialog() == DialogResult.OK && pComboBox.nSelected != -1)
							{
								File.Move(BinaryFilesFound[pComboBox.nSelected], BinaryFilesFound[pComboBox.nSelected] + "_DISABLE");

								BinaryFilesFound = BinaryFilesFound.Where(file => file != BinaryFilesFound[pComboBox.nSelected]).ToArray();
								Paths = BinaryFilesFound.Select(file => file.Replace(pMain.pSettings.ServerPath, "")).ToArray();
							}
						}
					}
					/****************************************/
					string[] ConfigFilesFound = Directory.GetFiles(Path.GetDirectoryName(BinaryFilesFound[0]) ?? string.Empty, "newStobm.bin", SearchOption.AllDirectories);

					if (ConfigFilesFound.Length > 1)
					{
						string[] Paths = new string[ConfigFilesFound.Length];

						for (int i = 0; i < ConfigFilesFound.Length; i++)
							Paths[i] = ConfigFilesFound[i].Replace(pMain.pSettings.ServerPath, "");

						while (ConfigFilesFound.Length > 1)
						{
							MessageBox_ComboBox pComboBox = new(this, "There are multiple Config files in one Service folder. Please select which one you want to Disable:", Paths);
							if (pComboBox.ShowDialog() == DialogResult.OK && pComboBox.nSelected != -1)
							{
								File.Move(ConfigFilesFound[pComboBox.nSelected], ConfigFilesFound[pComboBox.nSelected] + "_DISABLE");

								ConfigFilesFound = ConfigFilesFound.Where(file => file != ConfigFilesFound[pComboBox.nSelected]).ToArray();
								Paths = ConfigFilesFound.Select(file => file.Replace(pMain.pSettings.ServerPath, "")).ToArray();
							}
						}
					}

					if (ConfigFilesFound.Length > 0)
					{
						pMain.pSettings.ServicesData[strService].ConfigPath = ConfigFilesFound[0];
						FileIniDataParser pParser = new();
						IniData pData = pParser.ReadFile(ConfigFilesFound[0]);

						// Salt
						if (strService == "Connector" || strService == "LoginServer")
						{
							strSalts[nSaltsChecked] = pData["SHA256"]["Salt"] ?? "NONE";

							nSaltsChecked++;
						}

						// Hardcore
						if (strService == "Connector" || strService.Contains("GameServer"))
						{
							strHardcoreValues[nHardcoreChecked] = pData["Server"]["HARDCORE"] ?? "NONE";

							nHardcoreChecked++;
						}

						// Allowed External IP & Integration Server
						if (strService == "LoginServer" || strService.Contains("GameServer"))
						{
							strAllowedExternalIPValues[nAllowedExternalIPChecked] = pData["Server"]["AllowExternalIP"] ?? "NONE";

							nAllowedExternalIPChecked++;

							/*strIntegrationValues[nIntegrationServerChecked] = pData["Server"]["IntergrationServer"] ?? "NONE";

							nIntegrationServerChecked++;*/
						}

						// IP & PORT
						strIP = pData["Server"]["IP"] ?? "127.0.0.1";
						strPort = pData["Server"]["Port"] ?? "0";

						((TextBox)Controls.Find($"tb{strService}IP", true)[0]).Text = strIP;
						((TextBox)Controls.Find($"tb{strService}Port", true)[0]).Text = strPort;

						if (strService == "Messenger")
							tbMessengerSubHelperPort.Text = pData["Server"]["SubHelperPort"] ?? "0";

						if (strService == "LoginServer")
							tbLoginServerConnectorsCount.Text = pData["Connector Server"]["Count"] ?? "1";

						if (strService.Contains("GameServer"))
						{
							((CheckBox)Controls.Find($"cb{strService}PK", true)[0]).Checked = (pData["Server"]["NON_PK"] == "TRUE" ? false : true);
							((CheckBox)Controls.Find($"cb{strService}Speed", true)[0]).Checked = (pData["Server"]["SPEED_SERVER"] == "TRUE" ? true : false);
						}
					}

					pMain.pSettings.ServicesData[strService].Found = true;

					pMain.pSettings.ServicesData[strService].BinaryPath = BinaryFilesFound[0];

					((Button)Controls.Find($"btn{strService}StartStop", true)[0]).Enabled = true;
					((Button)Controls.Find($"btn{strService}Settings", true)[0]).Enabled = true;

					btnObj = (Button)Controls.Find($"btn{strService}Logs", true)[0];
					btnObj.Enabled = true;

					if (pMain.pSettings.ServicesData[strService].CatchLog)
						btnObj.BackColor = Color.FromArgb(124, 111, 100);
					else
						btnObj.BackColor = Color.FromArgb(40, 40, 40);

					pMain.pSettings.ServicesData[strService].ReUpTrysCount = 0;

					ServiceRunner(strService, false);
				}
				else
				{
					pMain.pSettings.ServicesData[strService].Found = false;
				}
			}

			// Salt
			if (nSaltsChecked == 2)
			{
				if (strSalts[0] == "NONE" || strSalts[1] == "NONE")
				{
					pMain.Logger(LogTypes.Error, $"Control Panel > Missing Salt from: {(strSalts[0] == "NONE" ? "Connector" : "LoginServer")}. Needed set an common Salt value and press 'Save All'.");
				}
				else
				{
					if (strSalts[0] != strSalts[1])
					{
						while (true)
						{
							MessageBox_ComboBox pComboBox = new(this, "Salts values mismatch. Please select which want to use:", strSalts);
							if (pComboBox.ShowDialog() == DialogResult.OK && pComboBox.nSelected != -1)
							{
								tbSalt.Text = strSalts[pComboBox.nSelected];
								break;
							}
						}
					}
					else
					{
						tbSalt.Text = strSalts[0];
					}
				}
			}

			List<string> strFilteredValues = new();

			// Hardcore
			if (nHardcoreChecked > 2)
			{
				strFilteredValues.Clear();

				for (int i = 0; i < 1/*Connector*/ + pMain.pSettings.TotalGameServers; i++)
				{
					if (strHardcoreValues[i] == "NONE")
						pMain.Logger(LogTypes.Error, $"Control Panel > Missing HARDCORE value from: {(i == 0 ? "Connector" : "GameServer" + i)}. Needed set an common HARDCORE value and press 'Save All'.");

					strFilteredValues.Add(strHardcoreValues[i]);
				}

				if (!strFilteredValues.All(x => x == strFilteredValues[0]))
				{
					while (true)
					{
						MessageBox_ComboBox pComboBox = new(this, "HARDCORE Values mismatch. Please set a value (Remember press 'Save All' to apply the change):", ["FALSE", "TRUE"]);
						if (pComboBox.ShowDialog() == DialogResult.OK && pComboBox.nSelected != -1)
						{
							cbHardcore.Checked = Convert.ToBoolean(pComboBox.nSelected);
							break;
						}
					}
				}
				else
				{
					cbHardcore.Checked = Convert.ToBoolean(strFilteredValues[0]);
				}
			}

			// Allowed External IP
			strFilteredValues.Clear();

			for (int i = 0; i < 1/*LoginServer*/ + pMain.pSettings.TotalGameServers; i++)
			{
				if (strAllowedExternalIPValues[i] == "NONE")
					pMain.Logger(LogTypes.Error, $"Control Panel > Missing AllowedExternalIP value from: {(i == 0 ? "LoginServer" : "GameServer" + i)}. Needed set an common AllowedExternalIP value and press 'Save All'.");

				strFilteredValues.Add(strAllowedExternalIPValues[i]);
			}

			if (!strFilteredValues.All(x => x == strFilteredValues[0]))
			{
				while (true)
				{
					MessageBox_ComboBox pComboBox = new(this, "AllowedExternalIP Values mismatch. Please set a value (Remember press 'Save All' to apply the change):", ["FALSE", "TRUE"]);
					if (pComboBox.ShowDialog() == DialogResult.OK && pComboBox.nSelected != -1)
					{
						cbAllowedExternalIP.Checked = Convert.ToBoolean(pComboBox.nSelected);
						break;
					}
				}
			}
			else
			{
				cbAllowedExternalIP.Checked = Convert.ToBoolean(strFilteredValues[0]);
			}

			// Integration Server
			/*strFilteredValues.Clear();
								 //LoginServer
			for (int i = 0; i < 1 + pMain.pSettings.TotalGameServers; i++)
			{
				if (strIntegrationValues[i] == "NONE")
					pMain.Logger(LogTypes.Error, $"Control Panel > Missing IntergrationServer value from: {(i == 0 ? "LoginServer" : "GameServer" + i)}. Needed set an common IntergrationServer value and press 'Save All'.");

				strFilteredValues.Add(strIntegrationValues[i]);
			}

			if (!strFilteredValues.All(x => x == strFilteredValues[0]))
			{
				while (true)
				{
					MessageBox_ComboBox pComboBox = new(this, "IntergrationServer Values mismatch. Please set a value (Remember press 'Save All' to apply the change):", ["FALSE", "TRUE"]);
					if (pComboBox.ShowDialog() == DialogResult.OK && pComboBox.nSelected != -1)
					{
						cbIntegrationServer.Checked = Convert.ToBoolean(pComboBox.nSelected);
						break;
					}
				}
			}
			else
			{
				cbIntegrationServer.Checked = Convert.ToBoolean(strFilteredValues[0]);
			}*/
		}

		private void btnStartStopAll_Click(object sender, EventArgs e)
		{
			Button btnObj;
			string strNeeded = "Start";

			if (btnStartStopAll.Text == "Stop All")
				strNeeded = "Stop";

			foreach (string strService in pMain.pSettings.Services)
			{
				if (strNeeded == "Start" && !pMain.pSettings.StartCashServerOnStartAll && strService == "CashServer")
					continue;

				btnObj = (Button)Controls.Find($"btn{strService}StartStop", true)[0];

				if (btnObj.Text == strNeeded)
					btnObj.PerformClick();
			}

			btnStartStopAll.Text = pMain.pSettings.Services.Any(strCheckService => pMain.pSettings.ServicesData[strCheckService].ProcessID != -1) ? "Stop All" : "Start All";
		}

		private void btnLaunchGame_MouseDown(object sender, MouseEventArgs e)
		{
			Process pProcess = new();
			string strPath = pMain.pSettings.ClientPath;

			if (e.Button == MouseButtons.Left)
				strPath += "\\Bin\\Nksp.exe";
			else
				strPath += "\\LC.exe";

			string strFileName = Path.GetFileName(strPath);

			if (File.Exists(strPath))
			{
				pMain.Logger(LogTypes.Message, $"Control Panel > Executing: {strFileName}...");

				pProcess.StartInfo.FileName = strPath;
				pProcess.StartInfo.Arguments = pMain.pSettings.NkspLaunchArgument;
				pProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(pMain.pSettings.ClientPath + "\\Bin");
				pProcess.Start();
			}
			else
			{
				pMain.Logger(LogTypes.Error, $"Control Panel > {strFileName} not found.");
			}
		}

		private void btnPanelSettings_Click(object sender, EventArgs e)
		{
			GroupBox gbServicesInfo = (GroupBox)Controls.Find("gbServicesInfo", true)[0];
			GroupBox gbServicesSettings = (GroupBox)Controls.Find("gbServicesSettings", true)[0];

			if (btnPanelSettings.Text == "Settings")
				btnPanelSettings.Text = "Control Panel";
			else if (btnPanelSettings.Text == "Control Panel")
				btnPanelSettings.Text = "Settings";

			gbServicesInfo.Visible = !gbServicesInfo.Visible;
			gbServicesSettings.Visible = !gbServicesSettings.Visible;
		}

		private void btnOpenServerFolder_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(pMain.pSettings.ServerPath))
				Process.Start("explorer.exe", pMain.pSettings.ServerPath);
		}

		private void btnOpenGameFolder_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(pMain.pSettings.ClientPath))
				Process.Start("explorer.exe", pMain.pSettings.ClientPath);
		}

		private void btnStartStop_Click(object sender, EventArgs e)
		{
			string strService = (sender as Button).Name.Substring(3).Substring(0, (sender as Button).Name.Length - 12);

			if ((sender as Button).Text.ToString() == "Start")
			{
				pMain.pSettings.ServicesData[strService].ReUpTrysCount = 0;

				ServiceRunner(strService, true);
			}
			else
			{
				ServiceStopped(strService, sender);
			}
		}

		private void btnSettings_Click(object sender, EventArgs e)
		{
			string strService = (sender as Button).Name.Substring(3).Substring(0, (sender as Button).Name.Length - 11);

			if (File.Exists(pMain.pSettings.ServicesData[strService].ConfigPath))
				Process.Start(new ProcessStartInfo(pMain.pSettings.TextEditorPath, '"' + pMain.pSettings.ServicesData[strService].ConfigPath + '"') { UseShellExecute = true });
		}

		private void btnLogs(object sender, EventArgs e)
		{
			string strService = (sender as Button).Name.Substring(3).Substring(0, (sender as Button).Name.Length - 7);

			if (pMain.pSettings.ServicesData[strService].CatchLog)
				(sender as Button).BackColor = Color.FromArgb(40, 40, 40);
			else
				(sender as Button).BackColor = Color.FromArgb(124, 111, 100);

			pMain.WriteToINI(pMain.pSettings.SettingsFile, "ControlPanel", strService + "CatchLogs", (!pMain.pSettings.ServicesData[strService].CatchLog).ToString());

			pMain.pSettings.ServicesData[strService].CatchLog = !pMain.pSettings.ServicesData[strService].CatchLog;
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			Control? cControl = (sender as ContextMenuStrip)?.SourceControl;

			if (cControl != null)
			{
				pContextMenu = new ContextMenuStrip();

				if (cControl.Name == "btnStartStopAll")
				{
					ToolStripMenuItem menuStartStopAll = new("Start CashSever On Start All")
					{
						CheckOnClick = true,
						Checked = pMain.pSettings.StartCashServerOnStartAll
					};

					menuStartStopAll.Click += (_, _) =>
					{
						bool bState = false;

						if (menuStartStopAll.Checked)
							bState = true;

						FileIniDataParser pParser = new();
						IniData pData = pParser.ReadFile(pMain.pSettings.SettingsFile);

						pData["ControlPanel"]["StartCashServerOnStartAll"] = bState.ToString();

						pParser.WriteFile(pMain.pSettings.SettingsFile, pData);

						pMain.pSettings.StartCashServerOnStartAll = bState;
					};

					pContextMenu.Items.Add(menuStartStopAll);
				}
				else
				{
					string strService = cControl.Name.Substring(3).Substring(0, cControl.Name.Length - 7);

					ToolStripMenuItem menuFiltersManager = new("Open Filters Manager");
					menuFiltersManager.Click += (_, _) => { new FiltersManager(pMain).ShowDialog(); };

					ToolStripMenuItem menuOpenLogsFolder = new("Open Logs Folder");
					menuOpenLogsFolder.Click += (_, _) =>
					{
						string[] LogsFolderPath = Directory.GetDirectories(pMain.pSettings.ServerPath, "LogFiles", SearchOption.AllDirectories);

						if (LogsFolderPath.Length > 0)
							Process.Start("explorer.exe", LogsFolderPath[0]);
					};

					ToolStripMenuItem menuLogsFiles = new("Logs Files");

					foreach (string strLogFilePath in Directory.GetFiles(pMain.pSettings.ServerPath, $"*_{Regex.Escape(GetLogFileName(strService))}*.log", SearchOption.AllDirectories).Reverse().ToArray())
					{
						ToolStripMenuItem menuLogsFilesSub = new(strLogFilePath);
						menuLogsFilesSub.Click += (_, _) =>
						{
							Process.Start(new ProcessStartInfo(pMain.pSettings.TextEditorPath, '"' + strLogFilePath + '"') { UseShellExecute = true });
						};
						menuLogsFiles.DropDownItems.Add(menuLogsFilesSub);
					}

					pContextMenu.Items.AddRange(
						new ToolStripLabel(strService),
						menuFiltersManager,
						new ToolStripSeparator(),
						menuOpenLogsFolder,
						menuLogsFiles,
						new ToolStripSeparator()
					);

					IniData pData = (new FileIniDataParser()).ReadFile(pMain.pSettings.SettingsFile);

					foreach (KeyData Key in pData["ControlPanelFilters"])
					{
						ToolStripMenuItem menuLogsFilesSub = new(Key.KeyName)
						{
							CheckOnClick = true,
							Checked = pMain.pSettings.ServicesData[strService].FilterKeyWord.Contains(Key.KeyName)
						};

						menuLogsFilesSub.Click += (_, _) =>
						{
							FileIniDataParser pParser = new();
							List<string> Services = pData["ControlPanelFilters"][menuLogsFilesSub.Text].Split(',').Select(s => s.Trim()).ToList();

							if (!menuLogsFilesSub.Checked)
							{
								pMain.pSettings.ServicesData[strService].FilterKeyWord.Remove(menuLogsFilesSub.Text ?? string.Empty);

								Services.Remove(strService);
							}
							else
							{
								pMain.pSettings.ServicesData[strService].FilterKeyWord.Add(menuLogsFilesSub.Text ?? string.Empty);

								Services.Add(strService);
							}

							pData["ControlPanelFilters"][menuLogsFilesSub.Text] = string.Join(", ", Services);

							pParser.WriteFile(pMain.pSettings.SettingsFile, pData);
						};

						pContextMenu.Items.Add(menuLogsFilesSub);
					}
				}

				pContextMenu.Show(Cursor.Position);
			}
		}

		// Settings
		private void btnSectionsManager_Click(object sender, EventArgs e) { new SectionsManager(pMain).ShowDialog(); }

		private void SaveToolBoxSettings()
		{
			FileIniDataParser pParser = new();
			IniData pData = pParser.ReadFile(pMain.pSettings.SettingsFile);
			/****************************************/
			pData["ControlPanel"]["ServerPath"] = tbServerFolder.Text;
			pData["ControlPanel"]["TextEditorPath"] = tbTextEditor.Text;
			/****************************************/
			string strAutoReupState = "False";

			if (cbAutoReUp.Checked)
				strAutoReupState = "True";

			pData["ControlPanel"]["AutoReUp"] = strAutoReupState;
			/****************************************/
			pData["Settings"]["MySQLHost"] = tbDBHost.Text;
			pData["Settings"]["MySQLUsername"] = tbDBUsername.Text;
			pData["Settings"]["MySQLPassword"] = tbDBPassword.Text;
			pData["Settings"]["MySQLDBAuth"] = tbDBAuth.Text;
			pData["Settings"]["MySQLDBData"] = tbDBData.Text;
			pData["Settings"]["MySQLDBUser"] = tbDBUser.Text;
			/****************************************/
			pParser.WriteFile(pMain.pSettings.SettingsFile, pData);

			pMain.Logger(LogTypes.Success, "Control Panel > Server, Client & Text Editor Paths, AutoReUp & Database Settings saved.");
		}

		private void tbServerFolder_DoubleClick(object sender, EventArgs e)
		{
			FolderBrowserDialog pFolderDialog = new FolderBrowserDialog { Description = "Please select the Server Folder" };
			if (pFolderDialog.ShowDialog() == DialogResult.OK)
				tbServerFolder.Text = pFolderDialog.SelectedPath;

			pMain.pSettings.ServerPath = tbServerFolder.Text;

			SaveToolBoxSettings();
		}

		private void tbTextEditor_DoubleClick(object sender, EventArgs e)
		{
			OpenFileDialog pFileDialog = new OpenFileDialog { Title = "Services Control Panel", Filter = "Executable Files|*.exe" };
			if (pFileDialog.ShowDialog() == DialogResult.OK)
				tbTextEditor.Text = pFileDialog.FileName;

			pMain.pSettings.TextEditorPath = tbTextEditor.Text;

			SaveToolBoxSettings();
		}

		private void cbAutoReUp_CheckedChanged(object sender, EventArgs e)
		{
			pMain.pSettings.AutoReUp = cbAutoReUp.Checked;

			SaveToolBoxSettings();
		}

		private void tbDBAnyField_KeyUp(object sender, EventArgs e)
		{
			pMain.pSettings.DBHost = tbDBHost.Text;
			pMain.pSettings.DBUsername = tbDBUsername.Text;
			pMain.pSettings.DBPassword = tbDBPassword.Text;
			pMain.pSettings.DBAuth = tbDBAuth.Text;
			pMain.pSettings.DBData = tbDBData.Text;
			pMain.pSettings.DBUser = tbDBUser.Text;

			SaveToolBoxSettings();
		}

		private void btnSetCommonIPForAll_Click(object sender, EventArgs e)
		{
			MessageBox_Input pInput = new(this, "Please Set an IP:", Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString());
			if (pInput.ShowDialog() != DialogResult.OK)
				return;

			foreach (string strService in pMain.pSettings.Services)
				((TextBox)Controls.Find($"tb{strService}IP", true)[0]).Text = pInput.strOutput;
		}

		private void btnTestDBConnection_Click(object sender, EventArgs e)
		{
			foreach (string strDBName in new string[3] { "Auth", "Data", "User" })
			{
				LogTypes ltTypeResult = LogTypes.Message;
				string strResult = "", strConnect = $"SERVER={tbDBHost.Text};DATABASE={((TextBox)Controls.Find("tbDB" + strDBName, true)[0]).Text};UID={tbDBUsername.Text};PASSWORD={tbDBPassword.Text};CHARSET=utf8";

				try
				{
					using (MySqlConnection mysqlConnection = new(strConnect))
					{
						mysqlConnection.Open();

						if (mysqlConnection.State == ConnectionState.Open)
						{
							strResult = $"Database: {strDBName} Connected successfully.";
							ltTypeResult = LogTypes.Success;
						}
					}
				}
				catch (Exception ex)
				{
					strResult = $"Database: {strDBName} Connection failed ({ex.Message}).";
					ltTypeResult = LogTypes.Error;
				}

				pMain.Logger(ltTypeResult, "Control Panel > Database Connection Test: " + strResult);
			}
		}

		private async void btnSaveSettings_ClickAsync(object sender, EventArgs e)
		{
			bool bSuccess = true;
			MessageBox_Progress pProgressDialog = new(this, "Writing Settings Please Wait...");
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await pMain.GenericLoadZoneDataAsync();
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Zones Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			Dictionary<string, List<string>> strDBSectionsNKeys = new Dictionary<string, List<string>> {
				{ "Auth", new List<string> { } },
				{ "Data", new List<string> { } },
				{ "User", new List<string> { } }
			};

			FileIniDataParser pParser = new();
			IniData pData = pParser.ReadFile(pMain.pSettings.SettingsFile);

			foreach (string strSectionsName in new string[3] { "Auth", "Data", "User" })
			{
				foreach (string strKeyName in pData["ControlPanel"][strSectionsName].Split(',').Select(s => s.Trim()).ToList())
					strDBSectionsNKeys[strSectionsName].Add(strKeyName);
			}

			// Services Settings
			try
			{
				IniParserConfiguration pParserConfiguration = new IniParserConfiguration { KeyValueAssigmentChar = '=', AssigmentSpacer = "" };

				foreach (string strService in pMain.pSettings.Services)
				{
					if (pMain.pSettings.ServicesData[strService].Found)
					{
						if (File.Exists(pMain.pSettings.ServicesData[strService].ConfigPath))
						{
							pParser = new FileIniDataParser(new IniDataParser(pParserConfiguration));
							pData = pParser.ReadFile(pMain.pSettings.ServicesData[strService].ConfigPath);

							pData["Server"]["IP"] = ((TextBox)Controls.Find($"tb{strService}IP", true)[0]).Text;
							pData["Server"]["Port"] = ((TextBox)Controls.Find($"tb{strService}Port", true)[0]).Text;

							if (strService == "Messenger")
							{
								pData["Server"]["SubHelperPort"] = tbMessengerSubHelperPort.Text;
							}
							else if (strService == "Connector")
							{
								pData["Server"]["Number"] = "1";
								pData["Server"]["MaxSubServer"] = pMain.pSettings.TotalGameServers.ToString();
								pData["Server"]["HARDCORE"] = cbHardcore.Checked ? "TRUE" : "FALSE";

								pData["Billing Server"]["IP"] = tbCashServerIP.Text;
								pData["Billing Server"]["Port"] = tbCashServerPort.Text;

								pData["SHA256"]["Salt"] = tbSalt.Text;
							}
							else if (strService == "Helper")
							{
								pData["Server"]["Number"] = "1";
							}
							else if (strService == "SubHelper")
							{
								pData["Server"]["Number"] = "1";

								pData["Messenger Server"]["IP"] = tbMessengerIP.Text;
								pData["Messenger Server"]["Port"] = tbMessengerPort.Text;
							}
							else if (strService == "LoginServer")
							{
								pData["Server"]["No"] = "1";
								pData["Server"]["AllowExternalIP"] = cbAllowedExternalIP.Checked ? "TRUE" : "FALSE";
								//pData["Server"]["IntergrationServer"] = cbIntegrationServer.Checked ? "TRUE" : "FALSE";

								pData["Messenger Server"]["IP"] = tbMessengerIP.Text;
								pData["Messenger Server"]["Port"] = tbMessengerPort.Text;

								pData["Connector Server"]["Count"] = tbLoginServerConnectorsCount.Text;

								pData["Connector_0 Server"]["Number"] = "1";
								pData["Connector_0 Server"]["MaxSubNumber"] = pMain.pSettings.TotalGameServers.ToString();
								pData["Connector_0 Server"]["IP"] = tbConnectorIP.Text;
								pData["Connector_0 Server"]["Port"] = tbConnectorPort.Text;

								pData["SHA256"]["Salt"] = tbSalt.Text;
							}
							else if (strService.Contains("GameServer"))
							{
								pData["Server"]["Number"] = "1";
								pData["Server"]["SubNumber"] = strService[strService.Length - 1].ToString();
								pData["Server"]["AllowExternalIP"] = cbAllowedExternalIP.Checked ? "TRUE" : "FALSE";
								//pData["Server"]["IntergrationServer"] = cbIntegrationServer.Checked ? "TRUE" : "FALSE";
								pData["Server"]["HARDCORE"] = cbHardcore.Checked ? "TRUE" : "FALSE";
								pData["Server"]["SPEED_SERVER"] = ((CheckBox)Controls.Find($"cb{strService}Speed", true)[0]).Checked ? "TRUE" : "FALSE";
								pData["Server"]["NON_PK"] = ((CheckBox)Controls.Find($"cb{strService} PK", true)[0]).Checked ? "FALSE" : "TRUE";

								pData["Connector Server"]["IP"] = tbConnectorIP.Text;
								pData["Connector Server"]["Port"] = tbConnectorPort.Text;

								pData["Messenger Server"]["IP"] = tbMessengerIP.Text;
								pData["Messenger Server"]["Port"] = tbMessengerPort.Text;

								pData["Helper Server"]["IP"] = tbHelperIP.Text;
								pData["Helper Server"]["Port"] = tbHelperPort.Text;

								pData["SubHelper Server"]["IP"] = tbSubHelperIP.Text;
								pData["SubHelper Server"]["Port"] = tbSubHelperPort.Text;

								if (pMain.pTables.ZoneTable != null)
								{
									pData["Zones"]["Count"] = pMain.pTables.ZoneTable.Rows.Count.ToString();

									for (int i = 0; i < pMain.pTables.ZoneTable.Rows.Count; i++)
									{
										pData["Zone_" + i]["No"] = i.ToString();
										pData["Zone_" + i]["Remote"] = "FALSE";
									}
								}
							}

							foreach (var SectionData in strDBSectionsNKeys)
							{
								foreach (string strKeyName in SectionData.Value)
								{
									pData[strKeyName]["IP"] = tbDBHost.Text;
									pData[strKeyName]["User"] = tbDBUsername.Text;
									pData[strKeyName]["Password"] = tbDBPassword.Text;
									pData[strKeyName]["DBName"] = ((TextBox)Controls.Find("tbDB" + SectionData.Key, true)[0]).Text;
								}
							}

							pParser.WriteFile(pMain.pSettings.ServicesData[strService].ConfigPath, pData, new UTF8Encoding(false));
						}
					}
				}
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, $"Control Panel > {ex.Message}.");

				bSuccess = false;
			}
			finally
			{
				if (bSuccess)
					pMain.Logger(LogTypes.Success, "Control Panel > Services values has been changed successfully.");
			}

			pProgressDialog.Close();
		}
	}
}
