using System.Windows.Forms;

namespace LastChaos_ToolBoxNG
{
	partial class ControlPanel
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new Container();
			btnMessengerStartStop = new Button();
			gbServicesInfo = new GroupBox();
			btnGameServer4Logs = new Button();
			contextMenuStrip1 = new ContextMenuStrip(components);
			btnGameServer4Settings = new Button();
			rtbGameServer4IPPort = new RichTextBox();
			lbGameServer4Status = new Label();
			btnGameServer4StartStop = new Button();
			btnGameServer3Logs = new Button();
			btnGameServer3Settings = new Button();
			rtbGameServer3IPPort = new RichTextBox();
			lbGameServer3Status = new Label();
			btnGameServer3StartStop = new Button();
			btnGameServer2Logs = new Button();
			btnGameServer2Settings = new Button();
			rtbGameServer2IPPort = new RichTextBox();
			lbGameServer2Status = new Label();
			btnGameServer2StartStop = new Button();
			btnCashServerLogs = new Button();
			btnCashServerSettings = new Button();
			rtbCashServerIPPort = new RichTextBox();
			lbCashServerStatus = new Label();
			btnCashServerStartStop = new Button();
			btnGameServer1Logs = new Button();
			btnGameServer1Settings = new Button();
			rtbGameServer1IPPort = new RichTextBox();
			lbGameServer1Status = new Label();
			btnGameServer1StartStop = new Button();
			btnLoginServerLogs = new Button();
			btnLoginServerSettings = new Button();
			rtbLoginServerIPPort = new RichTextBox();
			lbLoginServerStatus = new Label();
			btnLoginServerStartStop = new Button();
			btnSubHelperLogs = new Button();
			btnSubHelperSettings = new Button();
			rtbSubHelperIPPort = new RichTextBox();
			lbSubHelperStatus = new Label();
			btnSubHelperStartStop = new Button();
			btnHelperLogs = new Button();
			btnHelperSettings = new Button();
			rtbHelperIPPort = new RichTextBox();
			lbHelperStatus = new Label();
			btnHelperStartStop = new Button();
			btnConnectorLogs = new Button();
			btnConnectorSettings = new Button();
			rtbConnectorIPPort = new RichTextBox();
			lbConnectorStatus = new Label();
			btnConnectorStartStop = new Button();
			btnMessengerLogs = new Button();
			btnMessengerSettings = new Button();
			rtbMessengerIPPort = new RichTextBox();
			label3 = new Label();
			label2 = new Label();
			label1 = new Label();
			lbMessengerStatus = new Label();
			btnStartStopAll = new Button();
			btnPanelSettings = new Button();
			gbServicesSettings = new GroupBox();
			tbServerFolder = new TextBox();
			label4 = new Label();
			tbMessengerSubHelperPort = new TextBox();
			tbLoginServerConnectorsCount = new TextBox();
			label24 = new Label();
			cbGameServer4Speed = new CheckBox();
			cbGameServer3Speed = new CheckBox();
			cbGameServer2Speed = new CheckBox();
			cbGameServer1Speed = new CheckBox();
			btnSetCommonIPForAll = new Button();
			tbDBUser = new TextBox();
			label23 = new Label();
			tbDBData = new TextBox();
			label22 = new Label();
			tbDBAuth = new TextBox();
			label20 = new Label();
			tbDBPassword = new TextBox();
			label21 = new Label();
			tbDBUsername = new TextBox();
			label19 = new Label();
			btnTestDBConnection = new Button();
			tbDBHost = new TextBox();
			label18 = new Label();
			tbSalt = new TextBox();
			label17 = new Label();
			tbGameServer4Port = new TextBox();
			btnSectionsManager = new Button();
			tbGameServer4IP = new TextBox();
			btnSaveSettings = new Button();
			label15 = new Label();
			tbTextEditor = new TextBox();
			tbGameServer3Port = new TextBox();
			label6 = new Label();
			tbGameServer3IP = new TextBox();
			label16 = new Label();
			cbGameServer4PK = new CheckBox();
			cbHardcore = new CheckBox();
			cbGameServer3PK = new CheckBox();
			tbGameServer2Port = new TextBox();
			cbGameServer2PK = new CheckBox();
			cbAllowedExternalIP = new CheckBox();
			cbGameServer1PK = new CheckBox();
			tbGameServer2IP = new TextBox();
			label14 = new Label();
			tbGameServer1Port = new TextBox();
			cbAutoReUp = new CheckBox();
			tbGameServer1IP = new TextBox();
			tbMessengerIP = new TextBox();
			label13 = new Label();
			label7 = new Label();
			tbCashServerPort = new TextBox();
			tbMessengerPort = new TextBox();
			tbCashServerIP = new TextBox();
			label8 = new Label();
			label12 = new Label();
			tbConnectorIP = new TextBox();
			tbLoginServerPort = new TextBox();
			tbConnectorPort = new TextBox();
			tbLoginServerIP = new TextBox();
			label9 = new Label();
			label11 = new Label();
			tbHelperIP = new TextBox();
			tbSubHelperPort = new TextBox();
			tbHelperPort = new TextBox();
			tbSubHelperIP = new TextBox();
			label10 = new Label();
			btnLaunchGame = new Button();
			btnOpenServerFolder = new Button();
			btnOpenGameFolder = new Button();
			gbServicesInfo.SuspendLayout();
			gbServicesSettings.SuspendLayout();
			SuspendLayout();
			// 
			// btnMessengerStartStop
			// 
			btnMessengerStartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnMessengerStartStop.Enabled = false;
			btnMessengerStartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnMessengerStartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnMessengerStartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnMessengerStartStop.FlatStyle = FlatStyle.Flat;
			btnMessengerStartStop.Location = new Point(284, 35);
			btnMessengerStartStop.Margin = new Padding(4, 3, 4, 3);
			btnMessengerStartStop.Name = "btnMessengerStartStop";
			btnMessengerStartStop.Size = new Size(88, 27);
			btnMessengerStartStop.TabIndex = 1;
			btnMessengerStartStop.Text = "Start";
			btnMessengerStartStop.UseVisualStyleBackColor = false;
			btnMessengerStartStop.Click += btnStartStop_Click;
			// 
			// gbServicesInfo
			// 
			gbServicesInfo.Controls.Add(btnGameServer4Logs);
			gbServicesInfo.Controls.Add(btnGameServer4Settings);
			gbServicesInfo.Controls.Add(rtbGameServer4IPPort);
			gbServicesInfo.Controls.Add(lbGameServer4Status);
			gbServicesInfo.Controls.Add(btnGameServer4StartStop);
			gbServicesInfo.Controls.Add(btnGameServer3Logs);
			gbServicesInfo.Controls.Add(btnGameServer3Settings);
			gbServicesInfo.Controls.Add(rtbGameServer3IPPort);
			gbServicesInfo.Controls.Add(lbGameServer3Status);
			gbServicesInfo.Controls.Add(btnGameServer3StartStop);
			gbServicesInfo.Controls.Add(btnGameServer2Logs);
			gbServicesInfo.Controls.Add(btnGameServer2Settings);
			gbServicesInfo.Controls.Add(rtbGameServer2IPPort);
			gbServicesInfo.Controls.Add(lbGameServer2Status);
			gbServicesInfo.Controls.Add(btnGameServer2StartStop);
			gbServicesInfo.Controls.Add(btnCashServerLogs);
			gbServicesInfo.Controls.Add(btnCashServerSettings);
			gbServicesInfo.Controls.Add(rtbCashServerIPPort);
			gbServicesInfo.Controls.Add(lbCashServerStatus);
			gbServicesInfo.Controls.Add(btnCashServerStartStop);
			gbServicesInfo.Controls.Add(btnGameServer1Logs);
			gbServicesInfo.Controls.Add(btnGameServer1Settings);
			gbServicesInfo.Controls.Add(rtbGameServer1IPPort);
			gbServicesInfo.Controls.Add(lbGameServer1Status);
			gbServicesInfo.Controls.Add(btnGameServer1StartStop);
			gbServicesInfo.Controls.Add(btnLoginServerLogs);
			gbServicesInfo.Controls.Add(btnLoginServerSettings);
			gbServicesInfo.Controls.Add(rtbLoginServerIPPort);
			gbServicesInfo.Controls.Add(lbLoginServerStatus);
			gbServicesInfo.Controls.Add(btnLoginServerStartStop);
			gbServicesInfo.Controls.Add(btnSubHelperLogs);
			gbServicesInfo.Controls.Add(btnSubHelperSettings);
			gbServicesInfo.Controls.Add(rtbSubHelperIPPort);
			gbServicesInfo.Controls.Add(lbSubHelperStatus);
			gbServicesInfo.Controls.Add(btnSubHelperStartStop);
			gbServicesInfo.Controls.Add(btnHelperLogs);
			gbServicesInfo.Controls.Add(btnHelperSettings);
			gbServicesInfo.Controls.Add(rtbHelperIPPort);
			gbServicesInfo.Controls.Add(lbHelperStatus);
			gbServicesInfo.Controls.Add(btnHelperStartStop);
			gbServicesInfo.Controls.Add(btnConnectorLogs);
			gbServicesInfo.Controls.Add(btnConnectorSettings);
			gbServicesInfo.Controls.Add(rtbConnectorIPPort);
			gbServicesInfo.Controls.Add(lbConnectorStatus);
			gbServicesInfo.Controls.Add(btnConnectorStartStop);
			gbServicesInfo.Controls.Add(btnMessengerLogs);
			gbServicesInfo.Controls.Add(btnMessengerSettings);
			gbServicesInfo.Controls.Add(rtbMessengerIPPort);
			gbServicesInfo.Controls.Add(label3);
			gbServicesInfo.Controls.Add(label2);
			gbServicesInfo.Controls.Add(label1);
			gbServicesInfo.Controls.Add(lbMessengerStatus);
			gbServicesInfo.Controls.Add(btnMessengerStartStop);
			gbServicesInfo.ForeColor = Color.FromArgb(208, 203, 148);
			gbServicesInfo.Location = new Point(115, 12);
			gbServicesInfo.Margin = new Padding(4, 3, 4, 3);
			gbServicesInfo.Name = "gbServicesInfo";
			gbServicesInfo.Padding = new Padding(4, 3, 4, 3);
			gbServicesInfo.Size = new Size(572, 365);
			gbServicesInfo.TabIndex = 1001;
			gbServicesInfo.TabStop = false;
			gbServicesInfo.Text = "Services";
			// 
			// btnGameServer4Logs
			// 
			btnGameServer4Logs.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer4Logs.ContextMenuStrip = contextMenuStrip1;
			btnGameServer4Logs.Enabled = false;
			btnGameServer4Logs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer4Logs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnGameServer4Logs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer4Logs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer4Logs.FlatStyle = FlatStyle.Flat;
			btnGameServer4Logs.Location = new Point(476, 332);
			btnGameServer4Logs.Margin = new Padding(4, 3, 4, 3);
			btnGameServer4Logs.Name = "btnGameServer4Logs";
			btnGameServer4Logs.Size = new Size(88, 27);
			btnGameServer4Logs.TabIndex = 39;
			btnGameServer4Logs.Text = "Catch Logs";
			btnGameServer4Logs.UseVisualStyleBackColor = false;
			btnGameServer4Logs.Click += btnLogs;
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new Size(61, 4);
			contextMenuStrip1.Opening += contextMenuStrip1_Opening;
			// 
			// btnGameServer4Settings
			// 
			btnGameServer4Settings.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer4Settings.Enabled = false;
			btnGameServer4Settings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer4Settings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer4Settings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer4Settings.FlatStyle = FlatStyle.Flat;
			btnGameServer4Settings.Location = new Point(380, 332);
			btnGameServer4Settings.Margin = new Padding(4, 3, 4, 3);
			btnGameServer4Settings.Name = "btnGameServer4Settings";
			btnGameServer4Settings.Size = new Size(88, 27);
			btnGameServer4Settings.TabIndex = 38;
			btnGameServer4Settings.Text = "Settings";
			btnGameServer4Settings.UseVisualStyleBackColor = false;
			btnGameServer4Settings.Click += btnSettings_Click;
			// 
			// rtbGameServer4IPPort
			// 
			rtbGameServer4IPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbGameServer4IPPort.BorderStyle = BorderStyle.None;
			rtbGameServer4IPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbGameServer4IPPort.Location = new Point(107, 337);
			rtbGameServer4IPPort.Margin = new Padding(4, 3, 4, 3);
			rtbGameServer4IPPort.Name = "rtbGameServer4IPPort";
			rtbGameServer4IPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbGameServer4IPPort.Size = new Size(169, 16);
			rtbGameServer4IPPort.TabIndex = 36;
			rtbGameServer4IPPort.Text = "";
			// 
			// lbGameServer4Status
			// 
			lbGameServer4Status.AutoSize = true;
			lbGameServer4Status.Location = new Point(8, 338);
			lbGameServer4Status.Margin = new Padding(4, 0, 4, 0);
			lbGameServer4Status.MinimumSize = new Size(91, 15);
			lbGameServer4Status.Name = "lbGameServer4Status";
			lbGameServer4Status.Size = new Size(91, 15);
			lbGameServer4Status.TabIndex = 1050;
			lbGameServer4Status.Text = "GameServer 4";
			lbGameServer4Status.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnGameServer4StartStop
			// 
			btnGameServer4StartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer4StartStop.Enabled = false;
			btnGameServer4StartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer4StartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer4StartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer4StartStop.FlatStyle = FlatStyle.Flat;
			btnGameServer4StartStop.Location = new Point(284, 332);
			btnGameServer4StartStop.Margin = new Padding(4, 3, 4, 3);
			btnGameServer4StartStop.Name = "btnGameServer4StartStop";
			btnGameServer4StartStop.Size = new Size(88, 27);
			btnGameServer4StartStop.TabIndex = 37;
			btnGameServer4StartStop.Text = "Start";
			btnGameServer4StartStop.UseVisualStyleBackColor = false;
			btnGameServer4StartStop.Click += btnStartStop_Click;
			// 
			// btnGameServer3Logs
			// 
			btnGameServer3Logs.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer3Logs.ContextMenuStrip = contextMenuStrip1;
			btnGameServer3Logs.Enabled = false;
			btnGameServer3Logs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer3Logs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnGameServer3Logs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer3Logs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer3Logs.FlatStyle = FlatStyle.Flat;
			btnGameServer3Logs.Location = new Point(476, 299);
			btnGameServer3Logs.Margin = new Padding(4, 3, 4, 3);
			btnGameServer3Logs.Name = "btnGameServer3Logs";
			btnGameServer3Logs.Size = new Size(88, 27);
			btnGameServer3Logs.TabIndex = 35;
			btnGameServer3Logs.Text = "Catch Logs";
			btnGameServer3Logs.UseVisualStyleBackColor = false;
			btnGameServer3Logs.Click += btnLogs;
			// 
			// btnGameServer3Settings
			// 
			btnGameServer3Settings.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer3Settings.Enabled = false;
			btnGameServer3Settings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer3Settings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer3Settings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer3Settings.FlatStyle = FlatStyle.Flat;
			btnGameServer3Settings.Location = new Point(380, 299);
			btnGameServer3Settings.Margin = new Padding(4, 3, 4, 3);
			btnGameServer3Settings.Name = "btnGameServer3Settings";
			btnGameServer3Settings.Size = new Size(88, 27);
			btnGameServer3Settings.TabIndex = 34;
			btnGameServer3Settings.Text = "Settings";
			btnGameServer3Settings.UseVisualStyleBackColor = false;
			btnGameServer3Settings.Click += btnSettings_Click;
			// 
			// rtbGameServer3IPPort
			// 
			rtbGameServer3IPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbGameServer3IPPort.BorderStyle = BorderStyle.None;
			rtbGameServer3IPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbGameServer3IPPort.Location = new Point(107, 304);
			rtbGameServer3IPPort.Margin = new Padding(4, 3, 4, 3);
			rtbGameServer3IPPort.Name = "rtbGameServer3IPPort";
			rtbGameServer3IPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbGameServer3IPPort.Size = new Size(169, 16);
			rtbGameServer3IPPort.TabIndex = 32;
			rtbGameServer3IPPort.Text = "";
			// 
			// lbGameServer3Status
			// 
			lbGameServer3Status.AutoSize = true;
			lbGameServer3Status.Location = new Point(8, 305);
			lbGameServer3Status.Margin = new Padding(4, 0, 4, 0);
			lbGameServer3Status.MinimumSize = new Size(91, 15);
			lbGameServer3Status.Name = "lbGameServer3Status";
			lbGameServer3Status.Size = new Size(91, 15);
			lbGameServer3Status.TabIndex = 1045;
			lbGameServer3Status.Text = "GameServer 3";
			lbGameServer3Status.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnGameServer3StartStop
			// 
			btnGameServer3StartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer3StartStop.Enabled = false;
			btnGameServer3StartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer3StartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer3StartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer3StartStop.FlatStyle = FlatStyle.Flat;
			btnGameServer3StartStop.Location = new Point(284, 299);
			btnGameServer3StartStop.Margin = new Padding(4, 3, 4, 3);
			btnGameServer3StartStop.Name = "btnGameServer3StartStop";
			btnGameServer3StartStop.Size = new Size(88, 27);
			btnGameServer3StartStop.TabIndex = 33;
			btnGameServer3StartStop.Text = "Start";
			btnGameServer3StartStop.UseVisualStyleBackColor = false;
			btnGameServer3StartStop.Click += btnStartStop_Click;
			// 
			// btnGameServer2Logs
			// 
			btnGameServer2Logs.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer2Logs.ContextMenuStrip = contextMenuStrip1;
			btnGameServer2Logs.Enabled = false;
			btnGameServer2Logs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer2Logs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnGameServer2Logs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer2Logs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer2Logs.FlatStyle = FlatStyle.Flat;
			btnGameServer2Logs.Location = new Point(476, 266);
			btnGameServer2Logs.Margin = new Padding(4, 3, 4, 3);
			btnGameServer2Logs.Name = "btnGameServer2Logs";
			btnGameServer2Logs.Size = new Size(88, 27);
			btnGameServer2Logs.TabIndex = 31;
			btnGameServer2Logs.Text = "Catch Logs";
			btnGameServer2Logs.UseVisualStyleBackColor = false;
			btnGameServer2Logs.Click += btnLogs;
			// 
			// btnGameServer2Settings
			// 
			btnGameServer2Settings.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer2Settings.Enabled = false;
			btnGameServer2Settings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer2Settings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer2Settings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer2Settings.FlatStyle = FlatStyle.Flat;
			btnGameServer2Settings.Location = new Point(380, 266);
			btnGameServer2Settings.Margin = new Padding(4, 3, 4, 3);
			btnGameServer2Settings.Name = "btnGameServer2Settings";
			btnGameServer2Settings.Size = new Size(88, 27);
			btnGameServer2Settings.TabIndex = 30;
			btnGameServer2Settings.Text = "Settings";
			btnGameServer2Settings.UseVisualStyleBackColor = false;
			btnGameServer2Settings.Click += btnSettings_Click;
			// 
			// rtbGameServer2IPPort
			// 
			rtbGameServer2IPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbGameServer2IPPort.BorderStyle = BorderStyle.None;
			rtbGameServer2IPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbGameServer2IPPort.Location = new Point(107, 271);
			rtbGameServer2IPPort.Margin = new Padding(4, 3, 4, 3);
			rtbGameServer2IPPort.Name = "rtbGameServer2IPPort";
			rtbGameServer2IPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbGameServer2IPPort.Size = new Size(169, 16);
			rtbGameServer2IPPort.TabIndex = 28;
			rtbGameServer2IPPort.Text = "";
			// 
			// lbGameServer2Status
			// 
			lbGameServer2Status.AutoSize = true;
			lbGameServer2Status.Location = new Point(8, 272);
			lbGameServer2Status.Margin = new Padding(4, 0, 4, 0);
			lbGameServer2Status.MinimumSize = new Size(91, 15);
			lbGameServer2Status.Name = "lbGameServer2Status";
			lbGameServer2Status.Size = new Size(91, 15);
			lbGameServer2Status.TabIndex = 1040;
			lbGameServer2Status.Text = "GameServer 2";
			lbGameServer2Status.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnGameServer2StartStop
			// 
			btnGameServer2StartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer2StartStop.Enabled = false;
			btnGameServer2StartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer2StartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer2StartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer2StartStop.FlatStyle = FlatStyle.Flat;
			btnGameServer2StartStop.Location = new Point(284, 266);
			btnGameServer2StartStop.Margin = new Padding(4, 3, 4, 3);
			btnGameServer2StartStop.Name = "btnGameServer2StartStop";
			btnGameServer2StartStop.Size = new Size(88, 27);
			btnGameServer2StartStop.TabIndex = 29;
			btnGameServer2StartStop.Text = "Start";
			btnGameServer2StartStop.UseVisualStyleBackColor = false;
			btnGameServer2StartStop.Click += btnStartStop_Click;
			// 
			// btnCashServerLogs
			// 
			btnCashServerLogs.BackColor = Color.FromArgb(40, 40, 40);
			btnCashServerLogs.ContextMenuStrip = contextMenuStrip1;
			btnCashServerLogs.Enabled = false;
			btnCashServerLogs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnCashServerLogs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnCashServerLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnCashServerLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnCashServerLogs.FlatStyle = FlatStyle.Flat;
			btnCashServerLogs.Location = new Point(476, 200);
			btnCashServerLogs.Margin = new Padding(4, 3, 4, 3);
			btnCashServerLogs.Name = "btnCashServerLogs";
			btnCashServerLogs.Size = new Size(88, 27);
			btnCashServerLogs.TabIndex = 23;
			btnCashServerLogs.Text = "Catch Logs";
			btnCashServerLogs.UseVisualStyleBackColor = false;
			btnCashServerLogs.Click += btnLogs;
			// 
			// btnCashServerSettings
			// 
			btnCashServerSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnCashServerSettings.Enabled = false;
			btnCashServerSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnCashServerSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnCashServerSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnCashServerSettings.FlatStyle = FlatStyle.Flat;
			btnCashServerSettings.Location = new Point(380, 200);
			btnCashServerSettings.Margin = new Padding(4, 3, 4, 3);
			btnCashServerSettings.Name = "btnCashServerSettings";
			btnCashServerSettings.Size = new Size(88, 27);
			btnCashServerSettings.TabIndex = 22;
			btnCashServerSettings.Text = "Settings";
			btnCashServerSettings.UseVisualStyleBackColor = false;
			btnCashServerSettings.Click += btnSettings_Click;
			// 
			// rtbCashServerIPPort
			// 
			rtbCashServerIPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbCashServerIPPort.BorderStyle = BorderStyle.None;
			rtbCashServerIPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbCashServerIPPort.Location = new Point(107, 205);
			rtbCashServerIPPort.Margin = new Padding(4, 3, 4, 3);
			rtbCashServerIPPort.Name = "rtbCashServerIPPort";
			rtbCashServerIPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbCashServerIPPort.Size = new Size(169, 16);
			rtbCashServerIPPort.TabIndex = 20;
			rtbCashServerIPPort.Text = "";
			// 
			// lbCashServerStatus
			// 
			lbCashServerStatus.AutoSize = true;
			lbCashServerStatus.Location = new Point(8, 206);
			lbCashServerStatus.Margin = new Padding(4, 0, 4, 0);
			lbCashServerStatus.MinimumSize = new Size(91, 15);
			lbCashServerStatus.Name = "lbCashServerStatus";
			lbCashServerStatus.Size = new Size(91, 15);
			lbCashServerStatus.TabIndex = 1035;
			lbCashServerStatus.Text = "CashServer";
			lbCashServerStatus.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnCashServerStartStop
			// 
			btnCashServerStartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnCashServerStartStop.Enabled = false;
			btnCashServerStartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnCashServerStartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnCashServerStartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnCashServerStartStop.FlatStyle = FlatStyle.Flat;
			btnCashServerStartStop.Location = new Point(284, 200);
			btnCashServerStartStop.Margin = new Padding(4, 3, 4, 3);
			btnCashServerStartStop.Name = "btnCashServerStartStop";
			btnCashServerStartStop.Size = new Size(88, 27);
			btnCashServerStartStop.TabIndex = 21;
			btnCashServerStartStop.Text = "Start";
			btnCashServerStartStop.UseVisualStyleBackColor = false;
			btnCashServerStartStop.Click += btnStartStop_Click;
			// 
			// btnGameServer1Logs
			// 
			btnGameServer1Logs.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer1Logs.ContextMenuStrip = contextMenuStrip1;
			btnGameServer1Logs.Enabled = false;
			btnGameServer1Logs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer1Logs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnGameServer1Logs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer1Logs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer1Logs.FlatStyle = FlatStyle.Flat;
			btnGameServer1Logs.Location = new Point(476, 233);
			btnGameServer1Logs.Margin = new Padding(4, 3, 4, 3);
			btnGameServer1Logs.Name = "btnGameServer1Logs";
			btnGameServer1Logs.Size = new Size(88, 27);
			btnGameServer1Logs.TabIndex = 27;
			btnGameServer1Logs.Text = "Catch Logs";
			btnGameServer1Logs.UseVisualStyleBackColor = false;
			btnGameServer1Logs.Click += btnLogs;
			// 
			// btnGameServer1Settings
			// 
			btnGameServer1Settings.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer1Settings.Enabled = false;
			btnGameServer1Settings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer1Settings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer1Settings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer1Settings.FlatStyle = FlatStyle.Flat;
			btnGameServer1Settings.Location = new Point(380, 233);
			btnGameServer1Settings.Margin = new Padding(4, 3, 4, 3);
			btnGameServer1Settings.Name = "btnGameServer1Settings";
			btnGameServer1Settings.Size = new Size(88, 27);
			btnGameServer1Settings.TabIndex = 26;
			btnGameServer1Settings.Text = "Settings";
			btnGameServer1Settings.UseVisualStyleBackColor = false;
			btnGameServer1Settings.Click += btnSettings_Click;
			// 
			// rtbGameServer1IPPort
			// 
			rtbGameServer1IPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbGameServer1IPPort.BorderStyle = BorderStyle.None;
			rtbGameServer1IPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbGameServer1IPPort.Location = new Point(107, 238);
			rtbGameServer1IPPort.Margin = new Padding(4, 3, 4, 3);
			rtbGameServer1IPPort.Name = "rtbGameServer1IPPort";
			rtbGameServer1IPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbGameServer1IPPort.Size = new Size(169, 16);
			rtbGameServer1IPPort.TabIndex = 24;
			rtbGameServer1IPPort.Text = "";
			// 
			// lbGameServer1Status
			// 
			lbGameServer1Status.AutoSize = true;
			lbGameServer1Status.Location = new Point(8, 239);
			lbGameServer1Status.Margin = new Padding(4, 0, 4, 0);
			lbGameServer1Status.MinimumSize = new Size(91, 15);
			lbGameServer1Status.Name = "lbGameServer1Status";
			lbGameServer1Status.Size = new Size(91, 15);
			lbGameServer1Status.TabIndex = 1030;
			lbGameServer1Status.Text = "GameServer 1";
			lbGameServer1Status.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnGameServer1StartStop
			// 
			btnGameServer1StartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnGameServer1StartStop.Enabled = false;
			btnGameServer1StartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnGameServer1StartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnGameServer1StartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnGameServer1StartStop.FlatStyle = FlatStyle.Flat;
			btnGameServer1StartStop.Location = new Point(284, 233);
			btnGameServer1StartStop.Margin = new Padding(4, 3, 4, 3);
			btnGameServer1StartStop.Name = "btnGameServer1StartStop";
			btnGameServer1StartStop.Size = new Size(88, 27);
			btnGameServer1StartStop.TabIndex = 25;
			btnGameServer1StartStop.Text = "Start";
			btnGameServer1StartStop.UseVisualStyleBackColor = false;
			btnGameServer1StartStop.Click += btnStartStop_Click;
			// 
			// btnLoginServerLogs
			// 
			btnLoginServerLogs.BackColor = Color.FromArgb(40, 40, 40);
			btnLoginServerLogs.ContextMenuStrip = contextMenuStrip1;
			btnLoginServerLogs.Enabled = false;
			btnLoginServerLogs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnLoginServerLogs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnLoginServerLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnLoginServerLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnLoginServerLogs.FlatStyle = FlatStyle.Flat;
			btnLoginServerLogs.Location = new Point(476, 167);
			btnLoginServerLogs.Margin = new Padding(4, 3, 4, 3);
			btnLoginServerLogs.Name = "btnLoginServerLogs";
			btnLoginServerLogs.Size = new Size(88, 27);
			btnLoginServerLogs.TabIndex = 19;
			btnLoginServerLogs.Text = "Catch Logs";
			btnLoginServerLogs.UseVisualStyleBackColor = false;
			btnLoginServerLogs.Click += btnLogs;
			// 
			// btnLoginServerSettings
			// 
			btnLoginServerSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnLoginServerSettings.Enabled = false;
			btnLoginServerSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnLoginServerSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnLoginServerSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnLoginServerSettings.FlatStyle = FlatStyle.Flat;
			btnLoginServerSettings.Location = new Point(380, 167);
			btnLoginServerSettings.Margin = new Padding(4, 3, 4, 3);
			btnLoginServerSettings.Name = "btnLoginServerSettings";
			btnLoginServerSettings.Size = new Size(88, 27);
			btnLoginServerSettings.TabIndex = 18;
			btnLoginServerSettings.Text = "Settings";
			btnLoginServerSettings.UseVisualStyleBackColor = false;
			btnLoginServerSettings.Click += btnSettings_Click;
			// 
			// rtbLoginServerIPPort
			// 
			rtbLoginServerIPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbLoginServerIPPort.BorderStyle = BorderStyle.None;
			rtbLoginServerIPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbLoginServerIPPort.Location = new Point(107, 172);
			rtbLoginServerIPPort.Margin = new Padding(4, 3, 4, 3);
			rtbLoginServerIPPort.Name = "rtbLoginServerIPPort";
			rtbLoginServerIPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbLoginServerIPPort.Size = new Size(169, 16);
			rtbLoginServerIPPort.TabIndex = 16;
			rtbLoginServerIPPort.Text = "";
			// 
			// lbLoginServerStatus
			// 
			lbLoginServerStatus.AutoSize = true;
			lbLoginServerStatus.Location = new Point(8, 173);
			lbLoginServerStatus.Margin = new Padding(4, 0, 4, 0);
			lbLoginServerStatus.MinimumSize = new Size(91, 15);
			lbLoginServerStatus.Name = "lbLoginServerStatus";
			lbLoginServerStatus.Size = new Size(91, 15);
			lbLoginServerStatus.TabIndex = 1025;
			lbLoginServerStatus.Text = "LoginServer";
			lbLoginServerStatus.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnLoginServerStartStop
			// 
			btnLoginServerStartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnLoginServerStartStop.Enabled = false;
			btnLoginServerStartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnLoginServerStartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnLoginServerStartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnLoginServerStartStop.FlatStyle = FlatStyle.Flat;
			btnLoginServerStartStop.Location = new Point(284, 167);
			btnLoginServerStartStop.Margin = new Padding(4, 3, 4, 3);
			btnLoginServerStartStop.Name = "btnLoginServerStartStop";
			btnLoginServerStartStop.Size = new Size(88, 27);
			btnLoginServerStartStop.TabIndex = 17;
			btnLoginServerStartStop.Text = "Start";
			btnLoginServerStartStop.UseVisualStyleBackColor = false;
			btnLoginServerStartStop.Click += btnStartStop_Click;
			// 
			// btnSubHelperLogs
			// 
			btnSubHelperLogs.BackColor = Color.FromArgb(40, 40, 40);
			btnSubHelperLogs.ContextMenuStrip = contextMenuStrip1;
			btnSubHelperLogs.Enabled = false;
			btnSubHelperLogs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnSubHelperLogs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnSubHelperLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnSubHelperLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnSubHelperLogs.FlatStyle = FlatStyle.Flat;
			btnSubHelperLogs.Location = new Point(476, 134);
			btnSubHelperLogs.Margin = new Padding(4, 3, 4, 3);
			btnSubHelperLogs.Name = "btnSubHelperLogs";
			btnSubHelperLogs.Size = new Size(88, 27);
			btnSubHelperLogs.TabIndex = 15;
			btnSubHelperLogs.Text = "Catch Logs";
			btnSubHelperLogs.UseVisualStyleBackColor = false;
			btnSubHelperLogs.Click += btnLogs;
			// 
			// btnSubHelperSettings
			// 
			btnSubHelperSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnSubHelperSettings.Enabled = false;
			btnSubHelperSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnSubHelperSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnSubHelperSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnSubHelperSettings.FlatStyle = FlatStyle.Flat;
			btnSubHelperSettings.Location = new Point(380, 134);
			btnSubHelperSettings.Margin = new Padding(4, 3, 4, 3);
			btnSubHelperSettings.Name = "btnSubHelperSettings";
			btnSubHelperSettings.Size = new Size(88, 27);
			btnSubHelperSettings.TabIndex = 14;
			btnSubHelperSettings.Text = "Settings";
			btnSubHelperSettings.UseVisualStyleBackColor = false;
			btnSubHelperSettings.Click += btnSettings_Click;
			// 
			// rtbSubHelperIPPort
			// 
			rtbSubHelperIPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbSubHelperIPPort.BorderStyle = BorderStyle.None;
			rtbSubHelperIPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbSubHelperIPPort.Location = new Point(107, 139);
			rtbSubHelperIPPort.Margin = new Padding(4, 3, 4, 3);
			rtbSubHelperIPPort.Name = "rtbSubHelperIPPort";
			rtbSubHelperIPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbSubHelperIPPort.Size = new Size(169, 16);
			rtbSubHelperIPPort.TabIndex = 12;
			rtbSubHelperIPPort.Text = "";
			// 
			// lbSubHelperStatus
			// 
			lbSubHelperStatus.AutoSize = true;
			lbSubHelperStatus.Location = new Point(8, 140);
			lbSubHelperStatus.Margin = new Padding(4, 0, 4, 0);
			lbSubHelperStatus.MinimumSize = new Size(91, 15);
			lbSubHelperStatus.Name = "lbSubHelperStatus";
			lbSubHelperStatus.Size = new Size(91, 15);
			lbSubHelperStatus.TabIndex = 1020;
			lbSubHelperStatus.Text = "SubHelper";
			lbSubHelperStatus.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnSubHelperStartStop
			// 
			btnSubHelperStartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnSubHelperStartStop.Enabled = false;
			btnSubHelperStartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnSubHelperStartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnSubHelperStartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnSubHelperStartStop.FlatStyle = FlatStyle.Flat;
			btnSubHelperStartStop.Location = new Point(284, 134);
			btnSubHelperStartStop.Margin = new Padding(4, 3, 4, 3);
			btnSubHelperStartStop.Name = "btnSubHelperStartStop";
			btnSubHelperStartStop.Size = new Size(88, 27);
			btnSubHelperStartStop.TabIndex = 13;
			btnSubHelperStartStop.Text = "Start";
			btnSubHelperStartStop.UseVisualStyleBackColor = false;
			btnSubHelperStartStop.Click += btnStartStop_Click;
			// 
			// btnHelperLogs
			// 
			btnHelperLogs.BackColor = Color.FromArgb(40, 40, 40);
			btnHelperLogs.ContextMenuStrip = contextMenuStrip1;
			btnHelperLogs.Enabled = false;
			btnHelperLogs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnHelperLogs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnHelperLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnHelperLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnHelperLogs.FlatStyle = FlatStyle.Flat;
			btnHelperLogs.Location = new Point(476, 101);
			btnHelperLogs.Margin = new Padding(4, 3, 4, 3);
			btnHelperLogs.Name = "btnHelperLogs";
			btnHelperLogs.Size = new Size(88, 27);
			btnHelperLogs.TabIndex = 11;
			btnHelperLogs.Text = "Catch Logs";
			btnHelperLogs.UseVisualStyleBackColor = false;
			btnHelperLogs.Click += btnLogs;
			// 
			// btnHelperSettings
			// 
			btnHelperSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnHelperSettings.Enabled = false;
			btnHelperSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnHelperSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnHelperSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnHelperSettings.FlatStyle = FlatStyle.Flat;
			btnHelperSettings.Location = new Point(380, 101);
			btnHelperSettings.Margin = new Padding(4, 3, 4, 3);
			btnHelperSettings.Name = "btnHelperSettings";
			btnHelperSettings.Size = new Size(88, 27);
			btnHelperSettings.TabIndex = 10;
			btnHelperSettings.Text = "Settings";
			btnHelperSettings.UseVisualStyleBackColor = false;
			btnHelperSettings.Click += btnSettings_Click;
			// 
			// rtbHelperIPPort
			// 
			rtbHelperIPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbHelperIPPort.BorderStyle = BorderStyle.None;
			rtbHelperIPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbHelperIPPort.Location = new Point(107, 106);
			rtbHelperIPPort.Margin = new Padding(4, 3, 4, 3);
			rtbHelperIPPort.Name = "rtbHelperIPPort";
			rtbHelperIPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbHelperIPPort.Size = new Size(169, 16);
			rtbHelperIPPort.TabIndex = 8;
			rtbHelperIPPort.Text = "";
			// 
			// lbHelperStatus
			// 
			lbHelperStatus.AutoSize = true;
			lbHelperStatus.Location = new Point(8, 107);
			lbHelperStatus.Margin = new Padding(4, 0, 4, 0);
			lbHelperStatus.MinimumSize = new Size(91, 15);
			lbHelperStatus.Name = "lbHelperStatus";
			lbHelperStatus.Size = new Size(91, 15);
			lbHelperStatus.TabIndex = 1015;
			lbHelperStatus.Text = "Helper";
			lbHelperStatus.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnHelperStartStop
			// 
			btnHelperStartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnHelperStartStop.Enabled = false;
			btnHelperStartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnHelperStartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnHelperStartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnHelperStartStop.FlatStyle = FlatStyle.Flat;
			btnHelperStartStop.Location = new Point(284, 101);
			btnHelperStartStop.Margin = new Padding(4, 3, 4, 3);
			btnHelperStartStop.Name = "btnHelperStartStop";
			btnHelperStartStop.Size = new Size(88, 27);
			btnHelperStartStop.TabIndex = 9;
			btnHelperStartStop.Text = "Start";
			btnHelperStartStop.UseVisualStyleBackColor = false;
			btnHelperStartStop.Click += btnStartStop_Click;
			// 
			// btnConnectorLogs
			// 
			btnConnectorLogs.BackColor = Color.FromArgb(40, 40, 40);
			btnConnectorLogs.ContextMenuStrip = contextMenuStrip1;
			btnConnectorLogs.Enabled = false;
			btnConnectorLogs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnConnectorLogs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnConnectorLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnConnectorLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnConnectorLogs.FlatStyle = FlatStyle.Flat;
			btnConnectorLogs.Location = new Point(476, 68);
			btnConnectorLogs.Margin = new Padding(4, 3, 4, 3);
			btnConnectorLogs.Name = "btnConnectorLogs";
			btnConnectorLogs.Size = new Size(88, 27);
			btnConnectorLogs.TabIndex = 7;
			btnConnectorLogs.Text = "Catch Logs";
			btnConnectorLogs.UseVisualStyleBackColor = false;
			btnConnectorLogs.Click += btnLogs;
			// 
			// btnConnectorSettings
			// 
			btnConnectorSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnConnectorSettings.Enabled = false;
			btnConnectorSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnConnectorSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnConnectorSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnConnectorSettings.FlatStyle = FlatStyle.Flat;
			btnConnectorSettings.Location = new Point(380, 68);
			btnConnectorSettings.Margin = new Padding(4, 3, 4, 3);
			btnConnectorSettings.Name = "btnConnectorSettings";
			btnConnectorSettings.Size = new Size(88, 27);
			btnConnectorSettings.TabIndex = 6;
			btnConnectorSettings.Text = "Settings";
			btnConnectorSettings.UseVisualStyleBackColor = false;
			btnConnectorSettings.Click += btnSettings_Click;
			// 
			// rtbConnectorIPPort
			// 
			rtbConnectorIPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbConnectorIPPort.BorderStyle = BorderStyle.None;
			rtbConnectorIPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbConnectorIPPort.Location = new Point(107, 73);
			rtbConnectorIPPort.Margin = new Padding(4, 3, 4, 3);
			rtbConnectorIPPort.Name = "rtbConnectorIPPort";
			rtbConnectorIPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbConnectorIPPort.Size = new Size(169, 16);
			rtbConnectorIPPort.TabIndex = 4;
			rtbConnectorIPPort.Text = "";
			// 
			// lbConnectorStatus
			// 
			lbConnectorStatus.AutoSize = true;
			lbConnectorStatus.Location = new Point(8, 74);
			lbConnectorStatus.Margin = new Padding(4, 0, 4, 0);
			lbConnectorStatus.MinimumSize = new Size(91, 15);
			lbConnectorStatus.Name = "lbConnectorStatus";
			lbConnectorStatus.Size = new Size(91, 15);
			lbConnectorStatus.TabIndex = 1010;
			lbConnectorStatus.Text = "Connector";
			lbConnectorStatus.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnConnectorStartStop
			// 
			btnConnectorStartStop.BackColor = Color.FromArgb(40, 40, 40);
			btnConnectorStartStop.Enabled = false;
			btnConnectorStartStop.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnConnectorStartStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnConnectorStartStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnConnectorStartStop.FlatStyle = FlatStyle.Flat;
			btnConnectorStartStop.Location = new Point(284, 68);
			btnConnectorStartStop.Margin = new Padding(4, 3, 4, 3);
			btnConnectorStartStop.Name = "btnConnectorStartStop";
			btnConnectorStartStop.Size = new Size(88, 27);
			btnConnectorStartStop.TabIndex = 5;
			btnConnectorStartStop.Text = "Start";
			btnConnectorStartStop.UseVisualStyleBackColor = false;
			btnConnectorStartStop.Click += btnStartStop_Click;
			// 
			// btnMessengerLogs
			// 
			btnMessengerLogs.BackColor = Color.FromArgb(40, 40, 40);
			btnMessengerLogs.ContextMenuStrip = contextMenuStrip1;
			btnMessengerLogs.Enabled = false;
			btnMessengerLogs.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnMessengerLogs.FlatAppearance.CheckedBackColor = Color.Lime;
			btnMessengerLogs.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnMessengerLogs.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnMessengerLogs.FlatStyle = FlatStyle.Flat;
			btnMessengerLogs.Location = new Point(476, 35);
			btnMessengerLogs.Margin = new Padding(4, 3, 4, 3);
			btnMessengerLogs.Name = "btnMessengerLogs";
			btnMessengerLogs.Size = new Size(88, 27);
			btnMessengerLogs.TabIndex = 3;
			btnMessengerLogs.Text = "Catch Logs";
			btnMessengerLogs.UseVisualStyleBackColor = false;
			btnMessengerLogs.Click += btnLogs;
			// 
			// btnMessengerSettings
			// 
			btnMessengerSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnMessengerSettings.Enabled = false;
			btnMessengerSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnMessengerSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnMessengerSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnMessengerSettings.FlatStyle = FlatStyle.Flat;
			btnMessengerSettings.Location = new Point(380, 35);
			btnMessengerSettings.Margin = new Padding(4, 3, 4, 3);
			btnMessengerSettings.Name = "btnMessengerSettings";
			btnMessengerSettings.Size = new Size(88, 27);
			btnMessengerSettings.TabIndex = 2;
			btnMessengerSettings.Text = "Settings";
			btnMessengerSettings.UseVisualStyleBackColor = false;
			btnMessengerSettings.Click += btnSettings_Click;
			// 
			// rtbMessengerIPPort
			// 
			rtbMessengerIPPort.BackColor = Color.FromArgb(40, 40, 40);
			rtbMessengerIPPort.BorderStyle = BorderStyle.None;
			rtbMessengerIPPort.ForeColor = Color.FromArgb(208, 203, 148);
			rtbMessengerIPPort.Location = new Point(107, 40);
			rtbMessengerIPPort.Margin = new Padding(4, 3, 4, 3);
			rtbMessengerIPPort.Name = "rtbMessengerIPPort";
			rtbMessengerIPPort.ScrollBars = RichTextBoxScrollBars.None;
			rtbMessengerIPPort.Size = new Size(169, 16);
			rtbMessengerIPPort.TabIndex = 0;
			rtbMessengerIPPort.Text = "";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			label3.Location = new Point(287, 19);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(49, 13);
			label3.TabIndex = 1004;
			label3.Text = "Actions";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			label2.Location = new Point(166, 19);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(46, 13);
			label2.TabIndex = 1003;
			label2.Text = "IP:Port";
			label2.TextAlign = ContentAlignment.TopCenter;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
			label1.Location = new Point(30, 19);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(50, 13);
			label1.TabIndex = 1002;
			label1.Text = "Service";
			label1.TextAlign = ContentAlignment.TopCenter;
			// 
			// lbMessengerStatus
			// 
			lbMessengerStatus.AutoSize = true;
			lbMessengerStatus.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
			lbMessengerStatus.Location = new Point(8, 41);
			lbMessengerStatus.Margin = new Padding(4, 0, 4, 0);
			lbMessengerStatus.MinimumSize = new Size(91, 15);
			lbMessengerStatus.Name = "lbMessengerStatus";
			lbMessengerStatus.Size = new Size(91, 15);
			lbMessengerStatus.TabIndex = 1001;
			lbMessengerStatus.Text = "Messenger";
			lbMessengerStatus.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnStartStopAll
			// 
			btnStartStopAll.BackColor = Color.FromArgb(40, 40, 40);
			btnStartStopAll.ContextMenuStrip = contextMenuStrip1;
			btnStartStopAll.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnStartStopAll.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnStartStopAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnStartStopAll.FlatStyle = FlatStyle.Flat;
			btnStartStopAll.ForeColor = Color.FromArgb(208, 203, 148);
			btnStartStopAll.Location = new Point(13, 12);
			btnStartStopAll.Margin = new Padding(4, 3, 4, 3);
			btnStartStopAll.Name = "btnStartStopAll";
			btnStartStopAll.Size = new Size(94, 27);
			btnStartStopAll.TabIndex = 0;
			btnStartStopAll.Text = "Start All";
			btnStartStopAll.UseVisualStyleBackColor = false;
			btnStartStopAll.Click += btnStartStopAll_Click;
			// 
			// btnPanelSettings
			// 
			btnPanelSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnPanelSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnPanelSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnPanelSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnPanelSettings.FlatStyle = FlatStyle.Flat;
			btnPanelSettings.ForeColor = Color.FromArgb(208, 203, 148);
			btnPanelSettings.Location = new Point(13, 78);
			btnPanelSettings.Margin = new Padding(4, 3, 4, 3);
			btnPanelSettings.Name = "btnPanelSettings";
			btnPanelSettings.Size = new Size(94, 27);
			btnPanelSettings.TabIndex = 2;
			btnPanelSettings.Text = "Settings";
			btnPanelSettings.UseVisualStyleBackColor = false;
			btnPanelSettings.Click += btnPanelSettings_Click;
			// 
			// gbServicesSettings
			// 
			gbServicesSettings.Controls.Add(tbServerFolder);
			gbServicesSettings.Controls.Add(label4);
			gbServicesSettings.Controls.Add(tbMessengerSubHelperPort);
			gbServicesSettings.Controls.Add(tbLoginServerConnectorsCount);
			gbServicesSettings.Controls.Add(label24);
			gbServicesSettings.Controls.Add(cbGameServer4Speed);
			gbServicesSettings.Controls.Add(cbGameServer3Speed);
			gbServicesSettings.Controls.Add(cbGameServer2Speed);
			gbServicesSettings.Controls.Add(cbGameServer1Speed);
			gbServicesSettings.Controls.Add(btnSetCommonIPForAll);
			gbServicesSettings.Controls.Add(tbDBUser);
			gbServicesSettings.Controls.Add(label23);
			gbServicesSettings.Controls.Add(tbDBData);
			gbServicesSettings.Controls.Add(label22);
			gbServicesSettings.Controls.Add(tbDBAuth);
			gbServicesSettings.Controls.Add(label20);
			gbServicesSettings.Controls.Add(tbDBPassword);
			gbServicesSettings.Controls.Add(label21);
			gbServicesSettings.Controls.Add(tbDBUsername);
			gbServicesSettings.Controls.Add(label19);
			gbServicesSettings.Controls.Add(btnTestDBConnection);
			gbServicesSettings.Controls.Add(tbDBHost);
			gbServicesSettings.Controls.Add(label18);
			gbServicesSettings.Controls.Add(tbSalt);
			gbServicesSettings.Controls.Add(label17);
			gbServicesSettings.Controls.Add(tbGameServer4Port);
			gbServicesSettings.Controls.Add(btnSectionsManager);
			gbServicesSettings.Controls.Add(tbGameServer4IP);
			gbServicesSettings.Controls.Add(btnSaveSettings);
			gbServicesSettings.Controls.Add(label15);
			gbServicesSettings.Controls.Add(tbTextEditor);
			gbServicesSettings.Controls.Add(tbGameServer3Port);
			gbServicesSettings.Controls.Add(label6);
			gbServicesSettings.Controls.Add(tbGameServer3IP);
			gbServicesSettings.Controls.Add(label16);
			gbServicesSettings.Controls.Add(cbGameServer4PK);
			gbServicesSettings.Controls.Add(cbHardcore);
			gbServicesSettings.Controls.Add(cbGameServer3PK);
			gbServicesSettings.Controls.Add(tbGameServer2Port);
			gbServicesSettings.Controls.Add(cbGameServer2PK);
			gbServicesSettings.Controls.Add(cbAllowedExternalIP);
			gbServicesSettings.Controls.Add(cbGameServer1PK);
			gbServicesSettings.Controls.Add(tbGameServer2IP);
			gbServicesSettings.Controls.Add(label14);
			gbServicesSettings.Controls.Add(tbGameServer1Port);
			gbServicesSettings.Controls.Add(cbAutoReUp);
			gbServicesSettings.Controls.Add(tbGameServer1IP);
			gbServicesSettings.Controls.Add(tbMessengerIP);
			gbServicesSettings.Controls.Add(label13);
			gbServicesSettings.Controls.Add(label7);
			gbServicesSettings.Controls.Add(tbCashServerPort);
			gbServicesSettings.Controls.Add(tbMessengerPort);
			gbServicesSettings.Controls.Add(tbCashServerIP);
			gbServicesSettings.Controls.Add(label8);
			gbServicesSettings.Controls.Add(label12);
			gbServicesSettings.Controls.Add(tbConnectorIP);
			gbServicesSettings.Controls.Add(tbLoginServerPort);
			gbServicesSettings.Controls.Add(tbConnectorPort);
			gbServicesSettings.Controls.Add(tbLoginServerIP);
			gbServicesSettings.Controls.Add(label9);
			gbServicesSettings.Controls.Add(label11);
			gbServicesSettings.Controls.Add(tbHelperIP);
			gbServicesSettings.Controls.Add(tbSubHelperPort);
			gbServicesSettings.Controls.Add(tbHelperPort);
			gbServicesSettings.Controls.Add(tbSubHelperIP);
			gbServicesSettings.Controls.Add(label10);
			gbServicesSettings.FlatStyle = FlatStyle.Flat;
			gbServicesSettings.ForeColor = Color.FromArgb(208, 203, 148);
			gbServicesSettings.Location = new Point(115, 12);
			gbServicesSettings.Margin = new Padding(4, 3, 4, 3);
			gbServicesSettings.Name = "gbServicesSettings";
			gbServicesSettings.Padding = new Padding(4, 3, 4, 3);
			gbServicesSettings.Size = new Size(572, 365);
			gbServicesSettings.TabIndex = 1054;
			gbServicesSettings.TabStop = false;
			gbServicesSettings.Text = "Settings";
			gbServicesSettings.Visible = false;
			// 
			// tbServerFolder
			// 
			tbServerFolder.BackColor = Color.FromArgb(40, 40, 40);
			tbServerFolder.BorderStyle = BorderStyle.FixedSingle;
			tbServerFolder.ForeColor = Color.FromArgb(208, 203, 148);
			tbServerFolder.Location = new Point(96, 22);
			tbServerFolder.Margin = new Padding(4, 3, 4, 3);
			tbServerFolder.Name = "tbServerFolder";
			tbServerFolder.ReadOnly = true;
			tbServerFolder.Size = new Size(188, 23);
			tbServerFolder.TabIndex = 1127;
			tbServerFolder.DoubleClick += tbServerFolder_DoubleClick;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.ForeColor = Color.FromArgb(208, 203, 148);
			label4.Location = new Point(13, 26);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(75, 15);
			label4.TabIndex = 1128;
			label4.Text = "Server Folder";
			label4.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbMessengerSubHelperPort
			// 
			tbMessengerSubHelperPort.BackColor = Color.FromArgb(40, 40, 40);
			tbMessengerSubHelperPort.BorderStyle = BorderStyle.FixedSingle;
			tbMessengerSubHelperPort.ForeColor = Color.FromArgb(208, 203, 148);
			tbMessengerSubHelperPort.Location = new Point(250, 69);
			tbMessengerSubHelperPort.Margin = new Padding(4, 3, 4, 3);
			tbMessengerSubHelperPort.Name = "tbMessengerSubHelperPort";
			tbMessengerSubHelperPort.Size = new Size(50, 23);
			tbMessengerSubHelperPort.TabIndex = 1125;
			tbMessengerSubHelperPort.TextAlign = HorizontalAlignment.Center;
			// 
			// tbLoginServerConnectorsCount
			// 
			tbLoginServerConnectorsCount.BackColor = Color.FromArgb(40, 40, 40);
			tbLoginServerConnectorsCount.BorderStyle = BorderStyle.FixedSingle;
			tbLoginServerConnectorsCount.ForeColor = Color.FromArgb(208, 203, 148);
			tbLoginServerConnectorsCount.Location = new Point(345, 185);
			tbLoginServerConnectorsCount.Margin = new Padding(4, 3, 4, 3);
			tbLoginServerConnectorsCount.Name = "tbLoginServerConnectorsCount";
			tbLoginServerConnectorsCount.Size = new Size(40, 23);
			tbLoginServerConnectorsCount.TabIndex = 1123;
			tbLoginServerConnectorsCount.TextAlign = HorizontalAlignment.Center;
			// 
			// label24
			// 
			label24.AutoSize = true;
			label24.ForeColor = Color.FromArgb(208, 203, 148);
			label24.Location = new Point(258, 189);
			label24.Margin = new Padding(4, 0, 4, 0);
			label24.Name = "label24";
			label24.Size = new Size(75, 15);
			label24.TabIndex = 1124;
			label24.Text = "Server Count";
			label24.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbGameServer4Speed
			// 
			cbGameServer4Speed.AutoSize = true;
			cbGameServer4Speed.ForeColor = Color.FromArgb(208, 203, 148);
			cbGameServer4Speed.Location = new Point(313, 332);
			cbGameServer4Speed.Margin = new Padding(4, 3, 4, 3);
			cbGameServer4Speed.Name = "cbGameServer4Speed";
			cbGameServer4Speed.Size = new Size(58, 19);
			cbGameServer4Speed.TabIndex = 1122;
			cbGameServer4Speed.Text = "Speed";
			cbGameServer4Speed.UseVisualStyleBackColor = true;
			// 
			// cbGameServer3Speed
			// 
			cbGameServer3Speed.AutoSize = true;
			cbGameServer3Speed.ForeColor = Color.FromArgb(208, 203, 148);
			cbGameServer3Speed.Location = new Point(313, 303);
			cbGameServer3Speed.Margin = new Padding(4, 3, 4, 3);
			cbGameServer3Speed.Name = "cbGameServer3Speed";
			cbGameServer3Speed.Size = new Size(58, 19);
			cbGameServer3Speed.TabIndex = 1121;
			cbGameServer3Speed.Text = "Speed";
			cbGameServer3Speed.UseVisualStyleBackColor = true;
			// 
			// cbGameServer2Speed
			// 
			cbGameServer2Speed.AutoSize = true;
			cbGameServer2Speed.ForeColor = Color.FromArgb(208, 203, 148);
			cbGameServer2Speed.Location = new Point(313, 274);
			cbGameServer2Speed.Margin = new Padding(4, 3, 4, 3);
			cbGameServer2Speed.Name = "cbGameServer2Speed";
			cbGameServer2Speed.Size = new Size(58, 19);
			cbGameServer2Speed.TabIndex = 1120;
			cbGameServer2Speed.Text = "Speed";
			cbGameServer2Speed.UseVisualStyleBackColor = true;
			// 
			// cbGameServer1Speed
			// 
			cbGameServer1Speed.AutoSize = true;
			cbGameServer1Speed.ForeColor = Color.FromArgb(208, 203, 148);
			cbGameServer1Speed.Location = new Point(313, 245);
			cbGameServer1Speed.Margin = new Padding(4, 3, 4, 3);
			cbGameServer1Speed.Name = "cbGameServer1Speed";
			cbGameServer1Speed.Size = new Size(58, 19);
			cbGameServer1Speed.TabIndex = 1119;
			cbGameServer1Speed.Text = "Speed";
			cbGameServer1Speed.UseVisualStyleBackColor = true;
			// 
			// btnSetCommonIPForAll
			// 
			btnSetCommonIPForAll.BackColor = Color.FromArgb(40, 40, 40);
			btnSetCommonIPForAll.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnSetCommonIPForAll.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnSetCommonIPForAll.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnSetCommonIPForAll.FlatStyle = FlatStyle.Flat;
			btnSetCommonIPForAll.ForeColor = Color.FromArgb(208, 203, 148);
			btnSetCommonIPForAll.Location = new Point(258, 154);
			btnSetCommonIPForAll.Margin = new Padding(4, 3, 4, 3);
			btnSetCommonIPForAll.Name = "btnSetCommonIPForAll";
			btnSetCommonIPForAll.Size = new Size(127, 27);
			btnSetCommonIPForAll.TabIndex = 1118;
			btnSetCommonIPForAll.Text = "Set common IP";
			btnSetCommonIPForAll.UseVisualStyleBackColor = false;
			btnSetCommonIPForAll.Click += btnSetCommonIPForAll_Click;
			// 
			// tbDBUser
			// 
			tbDBUser.BackColor = Color.FromArgb(40, 40, 40);
			tbDBUser.BorderStyle = BorderStyle.FixedSingle;
			tbDBUser.ForeColor = Color.FromArgb(208, 203, 148);
			tbDBUser.Location = new Point(452, 303);
			tbDBUser.Margin = new Padding(4, 3, 4, 3);
			tbDBUser.Name = "tbDBUser";
			tbDBUser.Size = new Size(112, 23);
			tbDBUser.TabIndex = 1116;
			tbDBUser.KeyUp += tbDBAnyField_KeyUp;
			// 
			// label23
			// 
			label23.AutoSize = true;
			label23.ForeColor = Color.FromArgb(208, 203, 148);
			label23.Location = new Point(396, 307);
			label23.Margin = new Padding(4, 0, 4, 0);
			label23.Name = "label23";
			label23.Size = new Size(48, 15);
			label23.TabIndex = 1117;
			label23.Text = "DB User";
			label23.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbDBData
			// 
			tbDBData.BackColor = Color.FromArgb(40, 40, 40);
			tbDBData.BorderStyle = BorderStyle.FixedSingle;
			tbDBData.ForeColor = Color.FromArgb(208, 203, 148);
			tbDBData.Location = new Point(452, 274);
			tbDBData.Margin = new Padding(4, 3, 4, 3);
			tbDBData.Name = "tbDBData";
			tbDBData.Size = new Size(112, 23);
			tbDBData.TabIndex = 1114;
			tbDBData.KeyUp += tbDBAnyField_KeyUp;
			// 
			// label22
			// 
			label22.AutoSize = true;
			label22.ForeColor = Color.FromArgb(208, 203, 148);
			label22.Location = new Point(395, 278);
			label22.Margin = new Padding(4, 0, 4, 0);
			label22.Name = "label22";
			label22.Size = new Size(49, 15);
			label22.TabIndex = 1115;
			label22.Text = "DB Data";
			label22.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbDBAuth
			// 
			tbDBAuth.BackColor = Color.FromArgb(40, 40, 40);
			tbDBAuth.BorderStyle = BorderStyle.FixedSingle;
			tbDBAuth.ForeColor = Color.FromArgb(208, 203, 148);
			tbDBAuth.Location = new Point(452, 245);
			tbDBAuth.Margin = new Padding(4, 3, 4, 3);
			tbDBAuth.Name = "tbDBAuth";
			tbDBAuth.Size = new Size(112, 23);
			tbDBAuth.TabIndex = 1112;
			tbDBAuth.KeyUp += tbDBAnyField_KeyUp;
			// 
			// label20
			// 
			label20.AutoSize = true;
			label20.ForeColor = Color.FromArgb(208, 203, 148);
			label20.Location = new Point(393, 249);
			label20.Margin = new Padding(4, 0, 4, 0);
			label20.Name = "label20";
			label20.Size = new Size(51, 15);
			label20.TabIndex = 1113;
			label20.Text = "DB Auth";
			label20.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbDBPassword
			// 
			tbDBPassword.BackColor = Color.FromArgb(40, 40, 40);
			tbDBPassword.BorderStyle = BorderStyle.FixedSingle;
			tbDBPassword.ForeColor = Color.FromArgb(208, 203, 148);
			tbDBPassword.Location = new Point(452, 216);
			tbDBPassword.Margin = new Padding(4, 3, 4, 3);
			tbDBPassword.Name = "tbDBPassword";
			tbDBPassword.PasswordChar = '*';
			tbDBPassword.Size = new Size(112, 23);
			tbDBPassword.TabIndex = 1110;
			tbDBPassword.KeyUp += tbDBAnyField_KeyUp;
			// 
			// label21
			// 
			label21.AutoSize = true;
			label21.ForeColor = Color.FromArgb(208, 203, 148);
			label21.Location = new Point(396, 220);
			label21.Margin = new Padding(4, 0, 4, 0);
			label21.Name = "label21";
			label21.Size = new Size(48, 15);
			label21.TabIndex = 1111;
			label21.Text = "DB Pwd";
			label21.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbDBUsername
			// 
			tbDBUsername.BackColor = Color.FromArgb(40, 40, 40);
			tbDBUsername.BorderStyle = BorderStyle.FixedSingle;
			tbDBUsername.ForeColor = Color.FromArgb(208, 203, 148);
			tbDBUsername.Location = new Point(452, 187);
			tbDBUsername.Margin = new Padding(4, 3, 4, 3);
			tbDBUsername.Name = "tbDBUsername";
			tbDBUsername.Size = new Size(112, 23);
			tbDBUsername.TabIndex = 1108;
			tbDBUsername.KeyUp += tbDBAnyField_KeyUp;
			// 
			// label19
			// 
			label19.AutoSize = true;
			label19.ForeColor = Color.FromArgb(208, 203, 148);
			label19.Location = new Point(396, 191);
			label19.Margin = new Padding(4, 0, 4, 0);
			label19.Name = "label19";
			label19.Size = new Size(48, 15);
			label19.TabIndex = 1109;
			label19.Text = "DB User";
			label19.TextAlign = ContentAlignment.MiddleRight;
			// 
			// btnTestDBConnection
			// 
			btnTestDBConnection.BackColor = Color.FromArgb(40, 40, 40);
			btnTestDBConnection.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnTestDBConnection.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnTestDBConnection.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnTestDBConnection.FlatStyle = FlatStyle.Flat;
			btnTestDBConnection.ForeColor = Color.FromArgb(208, 203, 148);
			btnTestDBConnection.Location = new Point(395, 125);
			btnTestDBConnection.Margin = new Padding(4, 3, 4, 3);
			btnTestDBConnection.Name = "btnTestDBConnection";
			btnTestDBConnection.Size = new Size(169, 27);
			btnTestDBConnection.TabIndex = 1107;
			btnTestDBConnection.Text = "Test Connection";
			btnTestDBConnection.UseVisualStyleBackColor = false;
			btnTestDBConnection.Click += btnTestDBConnection_Click;
			// 
			// tbDBHost
			// 
			tbDBHost.BackColor = Color.FromArgb(40, 40, 40);
			tbDBHost.BorderStyle = BorderStyle.FixedSingle;
			tbDBHost.ForeColor = Color.FromArgb(208, 203, 148);
			tbDBHost.Location = new Point(452, 158);
			tbDBHost.Margin = new Padding(4, 3, 4, 3);
			tbDBHost.Name = "tbDBHost";
			tbDBHost.Size = new Size(112, 23);
			tbDBHost.TabIndex = 1105;
			tbDBHost.KeyUp += tbDBAnyField_KeyUp;
			// 
			// label18
			// 
			label18.AutoSize = true;
			label18.ForeColor = Color.FromArgb(208, 203, 148);
			label18.Location = new Point(394, 162);
			label18.Margin = new Padding(4, 0, 4, 0);
			label18.Name = "label18";
			label18.Size = new Size(50, 15);
			label18.TabIndex = 1106;
			label18.Text = "DB Host";
			label18.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbSalt
			// 
			tbSalt.BackColor = Color.FromArgb(40, 40, 40);
			tbSalt.BorderStyle = BorderStyle.FixedSingle;
			tbSalt.ForeColor = Color.FromArgb(208, 203, 148);
			tbSalt.Location = new Point(342, 69);
			tbSalt.Margin = new Padding(4, 3, 4, 3);
			tbSalt.Name = "tbSalt";
			tbSalt.PasswordChar = '*';
			tbSalt.Size = new Size(84, 23);
			tbSalt.TabIndex = 1103;
			// 
			// label17
			// 
			label17.AutoSize = true;
			label17.ForeColor = Color.FromArgb(208, 203, 148);
			label17.Location = new Point(308, 73);
			label17.Margin = new Padding(4, 0, 4, 0);
			label17.Name = "label17";
			label17.Size = new Size(26, 15);
			label17.TabIndex = 1104;
			label17.Text = "Salt";
			label17.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbGameServer4Port
			// 
			tbGameServer4Port.BackColor = Color.FromArgb(40, 40, 40);
			tbGameServer4Port.BorderStyle = BorderStyle.FixedSingle;
			tbGameServer4Port.ForeColor = Color.FromArgb(208, 203, 148);
			tbGameServer4Port.Location = new Point(201, 330);
			tbGameServer4Port.Margin = new Padding(4, 3, 4, 3);
			tbGameServer4Port.Name = "tbGameServer4Port";
			tbGameServer4Port.Size = new Size(50, 23);
			tbGameServer4Port.TabIndex = 1102;
			tbGameServer4Port.TextAlign = HorizontalAlignment.Center;
			// 
			// btnSectionsManager
			// 
			btnSectionsManager.BackColor = Color.FromArgb(40, 40, 40);
			btnSectionsManager.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnSectionsManager.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnSectionsManager.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnSectionsManager.FlatStyle = FlatStyle.Flat;
			btnSectionsManager.ForeColor = Color.FromArgb(208, 203, 148);
			btnSectionsManager.Location = new Point(434, 92);
			btnSectionsManager.Margin = new Padding(4, 3, 4, 3);
			btnSectionsManager.Name = "btnSectionsManager";
			btnSectionsManager.Size = new Size(130, 27);
			btnSectionsManager.TabIndex = 1064;
			btnSectionsManager.Text = "DB Sections Manager";
			btnSectionsManager.UseVisualStyleBackColor = false;
			btnSectionsManager.Click += btnSectionsManager_Click;
			// 
			// tbGameServer4IP
			// 
			tbGameServer4IP.BackColor = Color.FromArgb(40, 40, 40);
			tbGameServer4IP.BorderStyle = BorderStyle.FixedSingle;
			tbGameServer4IP.ForeColor = Color.FromArgb(208, 203, 148);
			tbGameServer4IP.Location = new Point(97, 330);
			tbGameServer4IP.Margin = new Padding(4, 3, 4, 3);
			tbGameServer4IP.Name = "tbGameServer4IP";
			tbGameServer4IP.Size = new Size(105, 23);
			tbGameServer4IP.TabIndex = 1100;
			tbGameServer4IP.TextAlign = HorizontalAlignment.Center;
			// 
			// btnSaveSettings
			// 
			btnSaveSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnSaveSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnSaveSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnSaveSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnSaveSettings.FlatStyle = FlatStyle.Flat;
			btnSaveSettings.ForeColor = Color.FromArgb(208, 203, 148);
			btnSaveSettings.Location = new Point(395, 332);
			btnSaveSettings.Margin = new Padding(4, 3, 4, 3);
			btnSaveSettings.Name = "btnSaveSettings";
			btnSaveSettings.Size = new Size(169, 27);
			btnSaveSettings.TabIndex = 1058;
			btnSaveSettings.Text = "Save to Services";
			btnSaveSettings.UseVisualStyleBackColor = false;
			btnSaveSettings.Click += btnSaveSettings_ClickAsync;
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.ForeColor = Color.FromArgb(208, 203, 148);
			label15.Location = new Point(9, 334);
			label15.Margin = new Padding(4, 0, 4, 0);
			label15.Name = "label15";
			label15.Size = new Size(79, 15);
			label15.TabIndex = 1101;
			label15.Text = "GameServer 4";
			label15.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbTextEditor
			// 
			tbTextEditor.BackColor = Color.FromArgb(40, 40, 40);
			tbTextEditor.BorderStyle = BorderStyle.FixedSingle;
			tbTextEditor.ForeColor = Color.FromArgb(208, 203, 148);
			tbTextEditor.Location = new Point(376, 22);
			tbTextEditor.Margin = new Padding(4, 3, 4, 3);
			tbTextEditor.Name = "tbTextEditor";
			tbTextEditor.ReadOnly = true;
			tbTextEditor.Size = new Size(188, 23);
			tbTextEditor.TabIndex = 1062;
			tbTextEditor.DoubleClick += tbTextEditor_DoubleClick;
			// 
			// tbGameServer3Port
			// 
			tbGameServer3Port.BackColor = Color.FromArgb(40, 40, 40);
			tbGameServer3Port.BorderStyle = BorderStyle.FixedSingle;
			tbGameServer3Port.ForeColor = Color.FromArgb(208, 203, 148);
			tbGameServer3Port.Location = new Point(201, 301);
			tbGameServer3Port.Margin = new Padding(4, 3, 4, 3);
			tbGameServer3Port.Name = "tbGameServer3Port";
			tbGameServer3Port.Size = new Size(50, 23);
			tbGameServer3Port.TabIndex = 1099;
			tbGameServer3Port.TextAlign = HorizontalAlignment.Center;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.ForeColor = Color.FromArgb(208, 203, 148);
			label6.Location = new Point(305, 26);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new Size(62, 15);
			label6.TabIndex = 1063;
			label6.Text = "Text Editor";
			label6.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbGameServer3IP
			// 
			tbGameServer3IP.BackColor = Color.FromArgb(40, 40, 40);
			tbGameServer3IP.BorderStyle = BorderStyle.FixedSingle;
			tbGameServer3IP.ForeColor = Color.FromArgb(208, 203, 148);
			tbGameServer3IP.Location = new Point(97, 301);
			tbGameServer3IP.Margin = new Padding(4, 3, 4, 3);
			tbGameServer3IP.Name = "tbGameServer3IP";
			tbGameServer3IP.Size = new Size(105, 23);
			tbGameServer3IP.TabIndex = 1097;
			tbGameServer3IP.TextAlign = HorizontalAlignment.Center;
			// 
			// label16
			// 
			label16.AutoSize = true;
			label16.ForeColor = Color.FromArgb(208, 203, 148);
			label16.Location = new Point(9, 305);
			label16.Margin = new Padding(4, 0, 4, 0);
			label16.Name = "label16";
			label16.Size = new Size(79, 15);
			label16.TabIndex = 1098;
			label16.Text = "GameServer 3";
			label16.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbGameServer4PK
			// 
			cbGameServer4PK.AutoSize = true;
			cbGameServer4PK.ForeColor = Color.FromArgb(208, 203, 148);
			cbGameServer4PK.Location = new Point(259, 332);
			cbGameServer4PK.Margin = new Padding(4, 3, 4, 3);
			cbGameServer4PK.Name = "cbGameServer4PK";
			cbGameServer4PK.Size = new Size(46, 19);
			cbGameServer4PK.TabIndex = 1071;
			cbGameServer4PK.Text = "PvP";
			cbGameServer4PK.UseVisualStyleBackColor = true;
			// 
			// cbHardcore
			// 
			cbHardcore.AutoSize = true;
			cbHardcore.ForeColor = Color.FromArgb(208, 203, 148);
			cbHardcore.Location = new Point(258, 129);
			cbHardcore.Margin = new Padding(4, 3, 4, 3);
			cbHardcore.Name = "cbHardcore";
			cbHardcore.Size = new Size(109, 19);
			cbHardcore.TabIndex = 1065;
			cbHardcore.Text = "Hardcore Mode";
			cbHardcore.UseVisualStyleBackColor = true;
			// 
			// cbGameServer3PK
			// 
			cbGameServer3PK.AutoSize = true;
			cbGameServer3PK.ForeColor = Color.FromArgb(208, 203, 148);
			cbGameServer3PK.Location = new Point(259, 303);
			cbGameServer3PK.Margin = new Padding(4, 3, 4, 3);
			cbGameServer3PK.Name = "cbGameServer3PK";
			cbGameServer3PK.Size = new Size(46, 19);
			cbGameServer3PK.TabIndex = 1070;
			cbGameServer3PK.Text = "PvP";
			cbGameServer3PK.UseVisualStyleBackColor = true;
			// 
			// tbGameServer2Port
			// 
			tbGameServer2Port.BackColor = Color.FromArgb(40, 40, 40);
			tbGameServer2Port.BorderStyle = BorderStyle.FixedSingle;
			tbGameServer2Port.ForeColor = Color.FromArgb(208, 203, 148);
			tbGameServer2Port.Location = new Point(201, 272);
			tbGameServer2Port.Margin = new Padding(4, 3, 4, 3);
			tbGameServer2Port.Name = "tbGameServer2Port";
			tbGameServer2Port.Size = new Size(50, 23);
			tbGameServer2Port.TabIndex = 1096;
			tbGameServer2Port.TextAlign = HorizontalAlignment.Center;
			// 
			// cbGameServer2PK
			// 
			cbGameServer2PK.AutoSize = true;
			cbGameServer2PK.ForeColor = Color.FromArgb(208, 203, 148);
			cbGameServer2PK.Location = new Point(259, 274);
			cbGameServer2PK.Margin = new Padding(4, 3, 4, 3);
			cbGameServer2PK.Name = "cbGameServer2PK";
			cbGameServer2PK.Size = new Size(46, 19);
			cbGameServer2PK.TabIndex = 1069;
			cbGameServer2PK.Text = "PvP";
			cbGameServer2PK.UseVisualStyleBackColor = true;
			// 
			// cbAllowedExternalIP
			// 
			cbAllowedExternalIP.AutoSize = true;
			cbAllowedExternalIP.ForeColor = Color.FromArgb(208, 203, 148);
			cbAllowedExternalIP.Location = new Point(259, 100);
			cbAllowedExternalIP.Margin = new Padding(4, 3, 4, 3);
			cbAllowedExternalIP.Name = "cbAllowedExternalIP";
			cbAllowedExternalIP.Size = new Size(126, 19);
			cbAllowedExternalIP.TabIndex = 1067;
			cbAllowedExternalIP.Text = "Allowed External IP";
			cbAllowedExternalIP.UseVisualStyleBackColor = true;
			// 
			// cbGameServer1PK
			// 
			cbGameServer1PK.AutoSize = true;
			cbGameServer1PK.ForeColor = Color.FromArgb(208, 203, 148);
			cbGameServer1PK.Location = new Point(259, 245);
			cbGameServer1PK.Margin = new Padding(4, 3, 4, 3);
			cbGameServer1PK.Name = "cbGameServer1PK";
			cbGameServer1PK.Size = new Size(46, 19);
			cbGameServer1PK.TabIndex = 1068;
			cbGameServer1PK.Text = "PvP";
			cbGameServer1PK.UseVisualStyleBackColor = true;
			// 
			// tbGameServer2IP
			// 
			tbGameServer2IP.BackColor = Color.FromArgb(40, 40, 40);
			tbGameServer2IP.BorderStyle = BorderStyle.FixedSingle;
			tbGameServer2IP.ForeColor = Color.FromArgb(208, 203, 148);
			tbGameServer2IP.Location = new Point(97, 272);
			tbGameServer2IP.Margin = new Padding(4, 3, 4, 3);
			tbGameServer2IP.Name = "tbGameServer2IP";
			tbGameServer2IP.Size = new Size(105, 23);
			tbGameServer2IP.TabIndex = 1094;
			tbGameServer2IP.TextAlign = HorizontalAlignment.Center;
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.ForeColor = Color.FromArgb(208, 203, 148);
			label14.Location = new Point(9, 276);
			label14.Margin = new Padding(4, 0, 4, 0);
			label14.Name = "label14";
			label14.Size = new Size(79, 15);
			label14.TabIndex = 1095;
			label14.Text = "GameServer 2";
			label14.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbGameServer1Port
			// 
			tbGameServer1Port.BackColor = Color.FromArgb(40, 40, 40);
			tbGameServer1Port.BorderStyle = BorderStyle.FixedSingle;
			tbGameServer1Port.ForeColor = Color.FromArgb(208, 203, 148);
			tbGameServer1Port.Location = new Point(201, 243);
			tbGameServer1Port.Margin = new Padding(4, 3, 4, 3);
			tbGameServer1Port.Name = "tbGameServer1Port";
			tbGameServer1Port.Size = new Size(50, 23);
			tbGameServer1Port.TabIndex = 1093;
			tbGameServer1Port.TextAlign = HorizontalAlignment.Center;
			// 
			// cbAutoReUp
			// 
			cbAutoReUp.AutoSize = true;
			cbAutoReUp.ForeColor = Color.FromArgb(208, 203, 148);
			cbAutoReUp.Location = new Point(476, 51);
			cbAutoReUp.Margin = new Padding(4, 3, 4, 3);
			cbAutoReUp.Name = "cbAutoReUp";
			cbAutoReUp.Size = new Size(88, 19);
			cbAutoReUp.TabIndex = 0;
			cbAutoReUp.Text = "Auto Re-Up";
			cbAutoReUp.UseVisualStyleBackColor = true;
			cbAutoReUp.CheckedChanged += cbAutoReUp_CheckedChanged;
			// 
			// tbGameServer1IP
			// 
			tbGameServer1IP.BackColor = Color.FromArgb(40, 40, 40);
			tbGameServer1IP.BorderStyle = BorderStyle.FixedSingle;
			tbGameServer1IP.ForeColor = Color.FromArgb(208, 203, 148);
			tbGameServer1IP.Location = new Point(97, 243);
			tbGameServer1IP.Margin = new Padding(4, 3, 4, 3);
			tbGameServer1IP.Name = "tbGameServer1IP";
			tbGameServer1IP.Size = new Size(105, 23);
			tbGameServer1IP.TabIndex = 1091;
			tbGameServer1IP.TextAlign = HorizontalAlignment.Center;
			// 
			// tbMessengerIP
			// 
			tbMessengerIP.BackColor = Color.FromArgb(40, 40, 40);
			tbMessengerIP.BorderStyle = BorderStyle.FixedSingle;
			tbMessengerIP.ForeColor = Color.FromArgb(208, 203, 148);
			tbMessengerIP.Location = new Point(97, 69);
			tbMessengerIP.Margin = new Padding(4, 3, 4, 3);
			tbMessengerIP.Name = "tbMessengerIP";
			tbMessengerIP.Size = new Size(105, 23);
			tbMessengerIP.TabIndex = 1073;
			tbMessengerIP.TextAlign = HorizontalAlignment.Center;
			// 
			// label13
			// 
			label13.AutoSize = true;
			label13.ForeColor = Color.FromArgb(208, 203, 148);
			label13.Location = new Point(9, 247);
			label13.Margin = new Padding(4, 0, 4, 0);
			label13.Name = "label13";
			label13.Size = new Size(79, 15);
			label13.TabIndex = 1092;
			label13.Text = "GameServer 1";
			label13.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.ForeColor = Color.FromArgb(208, 203, 148);
			label7.Location = new Point(24, 73);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new Size(64, 15);
			label7.TabIndex = 1074;
			label7.Text = "Messenger";
			label7.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbCashServerPort
			// 
			tbCashServerPort.BackColor = Color.FromArgb(40, 40, 40);
			tbCashServerPort.BorderStyle = BorderStyle.FixedSingle;
			tbCashServerPort.ForeColor = Color.FromArgb(208, 203, 148);
			tbCashServerPort.Location = new Point(201, 214);
			tbCashServerPort.Margin = new Padding(4, 3, 4, 3);
			tbCashServerPort.Name = "tbCashServerPort";
			tbCashServerPort.Size = new Size(50, 23);
			tbCashServerPort.TabIndex = 1090;
			tbCashServerPort.TextAlign = HorizontalAlignment.Center;
			// 
			// tbMessengerPort
			// 
			tbMessengerPort.BackColor = Color.FromArgb(40, 40, 40);
			tbMessengerPort.BorderStyle = BorderStyle.FixedSingle;
			tbMessengerPort.ForeColor = Color.FromArgb(208, 203, 148);
			tbMessengerPort.Location = new Point(201, 69);
			tbMessengerPort.Margin = new Padding(4, 3, 4, 3);
			tbMessengerPort.Name = "tbMessengerPort";
			tbMessengerPort.Size = new Size(50, 23);
			tbMessengerPort.TabIndex = 1075;
			tbMessengerPort.TextAlign = HorizontalAlignment.Center;
			// 
			// tbCashServerIP
			// 
			tbCashServerIP.BackColor = Color.FromArgb(40, 40, 40);
			tbCashServerIP.BorderStyle = BorderStyle.FixedSingle;
			tbCashServerIP.ForeColor = Color.FromArgb(208, 203, 148);
			tbCashServerIP.Location = new Point(97, 214);
			tbCashServerIP.Margin = new Padding(4, 3, 4, 3);
			tbCashServerIP.Name = "tbCashServerIP";
			tbCashServerIP.Size = new Size(105, 23);
			tbCashServerIP.TabIndex = 1088;
			tbCashServerIP.TextAlign = HorizontalAlignment.Center;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.ForeColor = Color.FromArgb(208, 203, 148);
			label8.Location = new Point(25, 102);
			label8.Margin = new Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new Size(63, 15);
			label8.TabIndex = 1077;
			label8.Text = "Connector";
			label8.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.ForeColor = Color.FromArgb(208, 203, 148);
			label12.Location = new Point(23, 218);
			label12.Margin = new Padding(4, 0, 4, 0);
			label12.Name = "label12";
			label12.Size = new Size(65, 15);
			label12.TabIndex = 1089;
			label12.Text = "CashServer";
			label12.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbConnectorIP
			// 
			tbConnectorIP.BackColor = Color.FromArgb(40, 40, 40);
			tbConnectorIP.BorderStyle = BorderStyle.FixedSingle;
			tbConnectorIP.ForeColor = Color.FromArgb(208, 203, 148);
			tbConnectorIP.Location = new Point(97, 98);
			tbConnectorIP.Margin = new Padding(4, 3, 4, 3);
			tbConnectorIP.Name = "tbConnectorIP";
			tbConnectorIP.Size = new Size(105, 23);
			tbConnectorIP.TabIndex = 1076;
			tbConnectorIP.TextAlign = HorizontalAlignment.Center;
			// 
			// tbLoginServerPort
			// 
			tbLoginServerPort.BackColor = Color.FromArgb(40, 40, 40);
			tbLoginServerPort.BorderStyle = BorderStyle.FixedSingle;
			tbLoginServerPort.ForeColor = Color.FromArgb(208, 203, 148);
			tbLoginServerPort.Location = new Point(201, 185);
			tbLoginServerPort.Margin = new Padding(4, 3, 4, 3);
			tbLoginServerPort.Name = "tbLoginServerPort";
			tbLoginServerPort.Size = new Size(50, 23);
			tbLoginServerPort.TabIndex = 1087;
			tbLoginServerPort.TextAlign = HorizontalAlignment.Center;
			// 
			// tbConnectorPort
			// 
			tbConnectorPort.BackColor = Color.FromArgb(40, 40, 40);
			tbConnectorPort.BorderStyle = BorderStyle.FixedSingle;
			tbConnectorPort.ForeColor = Color.FromArgb(208, 203, 148);
			tbConnectorPort.Location = new Point(201, 98);
			tbConnectorPort.Margin = new Padding(4, 3, 4, 3);
			tbConnectorPort.Name = "tbConnectorPort";
			tbConnectorPort.Size = new Size(50, 23);
			tbConnectorPort.TabIndex = 1078;
			tbConnectorPort.TextAlign = HorizontalAlignment.Center;
			// 
			// tbLoginServerIP
			// 
			tbLoginServerIP.BackColor = Color.FromArgb(40, 40, 40);
			tbLoginServerIP.BorderStyle = BorderStyle.FixedSingle;
			tbLoginServerIP.ForeColor = Color.FromArgb(208, 203, 148);
			tbLoginServerIP.Location = new Point(97, 185);
			tbLoginServerIP.Margin = new Padding(4, 3, 4, 3);
			tbLoginServerIP.Name = "tbLoginServerIP";
			tbLoginServerIP.Size = new Size(105, 23);
			tbLoginServerIP.TabIndex = 1085;
			tbLoginServerIP.TextAlign = HorizontalAlignment.Center;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.ForeColor = Color.FromArgb(208, 203, 148);
			label9.Location = new Point(46, 131);
			label9.Margin = new Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new Size(42, 15);
			label9.TabIndex = 1080;
			label9.Text = "Helper";
			label9.TextAlign = ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.ForeColor = Color.FromArgb(208, 203, 148);
			label11.Location = new Point(19, 189);
			label11.Margin = new Padding(4, 0, 4, 0);
			label11.Name = "label11";
			label11.Size = new Size(69, 15);
			label11.TabIndex = 1086;
			label11.Text = "LoginServer";
			label11.TextAlign = ContentAlignment.MiddleRight;
			// 
			// tbHelperIP
			// 
			tbHelperIP.BackColor = Color.FromArgb(40, 40, 40);
			tbHelperIP.BorderStyle = BorderStyle.FixedSingle;
			tbHelperIP.ForeColor = Color.FromArgb(208, 203, 148);
			tbHelperIP.Location = new Point(97, 127);
			tbHelperIP.Margin = new Padding(4, 3, 4, 3);
			tbHelperIP.Name = "tbHelperIP";
			tbHelperIP.Size = new Size(105, 23);
			tbHelperIP.TabIndex = 1079;
			tbHelperIP.TextAlign = HorizontalAlignment.Center;
			// 
			// tbSubHelperPort
			// 
			tbSubHelperPort.BackColor = Color.FromArgb(40, 40, 40);
			tbSubHelperPort.BorderStyle = BorderStyle.FixedSingle;
			tbSubHelperPort.ForeColor = Color.FromArgb(208, 203, 148);
			tbSubHelperPort.Location = new Point(201, 156);
			tbSubHelperPort.Margin = new Padding(4, 3, 4, 3);
			tbSubHelperPort.Name = "tbSubHelperPort";
			tbSubHelperPort.Size = new Size(50, 23);
			tbSubHelperPort.TabIndex = 1084;
			tbSubHelperPort.TextAlign = HorizontalAlignment.Center;
			// 
			// tbHelperPort
			// 
			tbHelperPort.BackColor = Color.FromArgb(40, 40, 40);
			tbHelperPort.BorderStyle = BorderStyle.FixedSingle;
			tbHelperPort.ForeColor = Color.FromArgb(208, 203, 148);
			tbHelperPort.Location = new Point(201, 127);
			tbHelperPort.Margin = new Padding(4, 3, 4, 3);
			tbHelperPort.Name = "tbHelperPort";
			tbHelperPort.Size = new Size(50, 23);
			tbHelperPort.TabIndex = 1081;
			tbHelperPort.TextAlign = HorizontalAlignment.Center;
			// 
			// tbSubHelperIP
			// 
			tbSubHelperIP.BackColor = Color.FromArgb(40, 40, 40);
			tbSubHelperIP.BorderStyle = BorderStyle.FixedSingle;
			tbSubHelperIP.ForeColor = Color.FromArgb(208, 203, 148);
			tbSubHelperIP.Location = new Point(97, 156);
			tbSubHelperIP.Margin = new Padding(4, 3, 4, 3);
			tbSubHelperIP.Name = "tbSubHelperIP";
			tbSubHelperIP.Size = new Size(105, 23);
			tbSubHelperIP.TabIndex = 1082;
			tbSubHelperIP.TextAlign = HorizontalAlignment.Center;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.ForeColor = Color.FromArgb(208, 203, 148);
			label10.Location = new Point(26, 160);
			label10.Margin = new Padding(4, 0, 4, 0);
			label10.Name = "label10";
			label10.Size = new Size(62, 15);
			label10.TabIndex = 1083;
			label10.Text = "SubHelper";
			label10.TextAlign = ContentAlignment.MiddleRight;
			// 
			// btnLaunchGame
			// 
			btnLaunchGame.BackColor = Color.FromArgb(40, 40, 40);
			btnLaunchGame.ContextMenuStrip = contextMenuStrip1;
			btnLaunchGame.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnLaunchGame.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnLaunchGame.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnLaunchGame.FlatStyle = FlatStyle.Flat;
			btnLaunchGame.ForeColor = Color.FromArgb(208, 203, 148);
			btnLaunchGame.Location = new Point(13, 45);
			btnLaunchGame.Margin = new Padding(4, 3, 4, 3);
			btnLaunchGame.Name = "btnLaunchGame";
			btnLaunchGame.Size = new Size(94, 27);
			btnLaunchGame.TabIndex = 1;
			btnLaunchGame.Text = "Launch Nksp";
			btnLaunchGame.UseVisualStyleBackColor = false;
			btnLaunchGame.Click += btnLaunchGame_Click;
			// 
			// btnOpenServerFolder
			// 
			btnOpenServerFolder.BackColor = Color.FromArgb(40, 40, 40);
			btnOpenServerFolder.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnOpenServerFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnOpenServerFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnOpenServerFolder.FlatStyle = FlatStyle.Flat;
			btnOpenServerFolder.ForeColor = Color.FromArgb(208, 203, 148);
			btnOpenServerFolder.Location = new Point(13, 111);
			btnOpenServerFolder.Margin = new Padding(4, 3, 4, 3);
			btnOpenServerFolder.Name = "btnOpenServerFolder";
			btnOpenServerFolder.Size = new Size(94, 27);
			btnOpenServerFolder.TabIndex = 3;
			btnOpenServerFolder.Text = "Server Folder";
			btnOpenServerFolder.UseVisualStyleBackColor = false;
			btnOpenServerFolder.Click += btnOpenServerFolder_Click;
			// 
			// btnOpenGameFolder
			// 
			btnOpenGameFolder.BackColor = Color.FromArgb(40, 40, 40);
			btnOpenGameFolder.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnOpenGameFolder.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnOpenGameFolder.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnOpenGameFolder.FlatStyle = FlatStyle.Flat;
			btnOpenGameFolder.ForeColor = Color.FromArgb(208, 203, 148);
			btnOpenGameFolder.Location = new Point(13, 144);
			btnOpenGameFolder.Margin = new Padding(4, 3, 4, 3);
			btnOpenGameFolder.Name = "btnOpenGameFolder";
			btnOpenGameFolder.Size = new Size(94, 27);
			btnOpenGameFolder.TabIndex = 4;
			btnOpenGameFolder.Text = "Game Folder";
			btnOpenGameFolder.UseVisualStyleBackColor = false;
			btnOpenGameFolder.Click += btnOpenGameFolder_Click;
			// 
			// ControlPanel
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(40, 40, 40);
			ClientSize = new Size(700, 389);
			Controls.Add(btnOpenGameFolder);
			Controls.Add(btnOpenServerFolder);
			Controls.Add(btnLaunchGame);
			Controls.Add(gbServicesSettings);
			Controls.Add(btnPanelSettings);
			Controls.Add(btnStartStopAll);
			Controls.Add(gbServicesInfo);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = Properties.Resources.NG;
			Margin = new Padding(4, 3, 4, 3);
			MaximizeBox = false;
			MaximumSize = new Size(716, 428);
			MinimizeBox = false;
			MinimumSize = new Size(716, 428);
			Name = "ControlPanel";
			Text = "Services Control Panel";
			FormClosing += ServicesControlPanel_FormClosing;
			Load += ServicesControlPanel_Load;
			gbServicesInfo.ResumeLayout(false);
			gbServicesInfo.PerformLayout();
			gbServicesSettings.ResumeLayout(false);
			gbServicesSettings.PerformLayout();
			ResumeLayout(false);

		}
		#endregion

		private Button btnMessengerStartStop;
		private GroupBox gbServicesInfo;
		private Label lbMessengerStatus;
		private Label label1;
		private Label label2;
		private Label label3;
		private RichTextBox rtbMessengerIPPort;
		private Button btnMessengerSettings;
		private Button btnMessengerLogs;
		private Button btnConnectorLogs;
		private Button btnConnectorSettings;
		private RichTextBox rtbConnectorIPPort;
		private Label lbConnectorStatus;
		private Button btnConnectorStartStop;
		private Button btnHelperLogs;
		private Button btnHelperSettings;
		private RichTextBox rtbHelperIPPort;
		private Label lbHelperStatus;
		private Button btnHelperStartStop;
		private Button btnSubHelperLogs;
		private Button btnSubHelperSettings;
		private RichTextBox rtbSubHelperIPPort;
		private Label lbSubHelperStatus;
		private Button btnSubHelperStartStop;
		private Button btnLoginServerLogs;
		private Button btnLoginServerSettings;
		private RichTextBox rtbLoginServerIPPort;
		private Label lbLoginServerStatus;
		private Button btnLoginServerStartStop;
		private Button btnGameServer1Logs;
		private Button btnGameServer1Settings;
		private RichTextBox rtbGameServer1IPPort;
		private Label lbGameServer1Status;
		private Button btnGameServer1StartStop;
		private Button btnCashServerLogs;
		private Button btnCashServerSettings;
		private RichTextBox rtbCashServerIPPort;
		private Label lbCashServerStatus;
		private Button btnCashServerStartStop;
		private Button btnGameServer2Logs;
		private Button btnGameServer2Settings;
		private RichTextBox rtbGameServer2IPPort;
		private Label lbGameServer2Status;
		private Button btnGameServer2StartStop;
		private Button btnGameServer3Logs;
		private Button btnGameServer3Settings;
		private RichTextBox rtbGameServer3IPPort;
		private Label lbGameServer3Status;
		private Button btnGameServer3StartStop;
		private Button btnGameServer4Logs;
		private Button btnGameServer4Settings;
		private RichTextBox rtbGameServer4IPPort;
		private Label lbGameServer4Status;
		private Button btnGameServer4StartStop;
		private Button btnStartStopAll;
		private Button btnPanelSettings;
		private GroupBox gbServicesSettings;
		private Button btnLaunchGame;
		private Button btnOpenServerFolder;
		private Button btnOpenGameFolder;
		private ContextMenuStrip contextMenuStrip1;
		private CheckBox cbAutoReUp;
		private Button btnSaveSettings;
		private Button btnSectionsManager;
		private CheckBox cbHardcore;
		private CheckBox cbAllowedExternalIP;
		private CheckBox cbGameServer1PK;
		private CheckBox cbGameServer4PK;
		private CheckBox cbGameServer3PK;
		private CheckBox cbGameServer2PK;
		private TextBox tbMessengerIP;
		private Label label7;
		private TextBox tbMessengerPort;
		private TextBox tbConnectorPort;
		private TextBox tbConnectorIP;
		private Label label8;
		private TextBox tbHelperPort;
		private TextBox tbHelperIP;
		private Label label9;
		private TextBox tbSubHelperPort;
		private TextBox tbSubHelperIP;
		private Label label10;
		private TextBox tbLoginServerPort;
		private Label label11;
		private TextBox tbCashServerPort;
		private TextBox tbCashServerIP;
		private Label label12;
		private TextBox tbLoginServerIP;
		private TextBox tbGameServer1Port;
		private TextBox tbGameServer1IP;
		private Label label13;
		private TextBox tbGameServer2Port;
		private TextBox tbGameServer2IP;
		private Label label14;
		private TextBox tbGameServer4Port;
		private TextBox tbGameServer4IP;
		private Label label15;
		private TextBox tbGameServer3Port;
		private TextBox tbGameServer3IP;
		private Label label16;
		private TextBox tbSalt;
		private Label label17;
		private TextBox tbDBHost;
		private Label label18;
		private Button btnTestDBConnection;
		private TextBox tbDBUsername;
		private Label label19;
		private TextBox tbDBAuth;
		private Label label20;
		private TextBox tbDBPassword;
		private Label label21;
		private TextBox tbDBData;
		private Label label22;
		private TextBox tbDBUser;
		private Label label23;
		private Button btnSetCommonIPForAll;
		private CheckBox cbGameServer1Speed;
		private CheckBox cbGameServer4Speed;
		private CheckBox cbGameServer3Speed;
		private CheckBox cbGameServer2Speed;
		private TextBox tbLoginServerConnectorsCount;
		private Label label24;
		private TextBox tbMessengerSubHelperPort;
		private TextBox tbTextEditor;
		private Label label6;
		private TextBox tbServerFolder;
		private Label label4;
	}
}