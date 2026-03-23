using LastChaos_ToolBoxNG.Properties;

namespace LastChaos_ToolBoxNG
{
    partial class Main
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(Main));
			btnReconnect = new Button();
			btnReloadSettings = new Button();
			Monitor = new Timer(components);
			button1 = new Button();
			btnUpdateHelper = new Button();
			btnCompareFiles = new Button();
			btnControlPanel = new Button();
			btnFileEncrypter = new Button();
			btnFileDecrypter = new Button();
			btnExporter = new Button();
			rtbConsole = new RichTextBox();
			btnCheckUpdates = new Button();
			cbDockToTop = new CheckBox();
			lbEditors = new ListBox();
			Status = new StatusStrip();
			btnDBStringTranslator = new Button();
			cbMaximizeOnStartUp = new CheckBox();
			SuspendLayout();
			// 
			// btnReconnect
			// 
			btnReconnect.BackColor = Color.FromArgb(40, 40, 40);
			btnReconnect.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnReconnect.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnReconnect.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnReconnect.FlatStyle = FlatStyle.Flat;
			btnReconnect.ForeColor = Color.FromArgb(208, 203, 148);
			btnReconnect.Location = new Point(366, 12);
			btnReconnect.Margin = new Padding(4, 3, 4, 3);
			btnReconnect.Name = "btnReconnect";
			btnReconnect.Size = new Size(100, 27);
			btnReconnect.TabIndex = 0;
			btnReconnect.Text = "Reconnect";
			btnReconnect.UseVisualStyleBackColor = false;
			btnReconnect.Click += btnReconnect_Click;
			// 
			// btnReloadSettings
			// 
			btnReloadSettings.BackColor = Color.FromArgb(40, 40, 40);
			btnReloadSettings.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnReloadSettings.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnReloadSettings.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnReloadSettings.FlatStyle = FlatStyle.Flat;
			btnReloadSettings.ForeColor = Color.FromArgb(208, 203, 148);
			btnReloadSettings.Location = new Point(258, 12);
			btnReloadSettings.Margin = new Padding(4, 3, 4, 3);
			btnReloadSettings.Name = "btnReloadSettings";
			btnReloadSettings.Size = new Size(100, 27);
			btnReloadSettings.TabIndex = 0;
			btnReloadSettings.Text = "Reload Settings";
			btnReloadSettings.UseVisualStyleBackColor = false;
			btnReloadSettings.Click += btnReloadSettings_Click;
			// 
			// Monitor
			// 
			Monitor.Interval = 1000;
			Monitor.Tick += monitor_Tick;
			// 
			// button1
			// 
			button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			button1.BackColor = Color.FromArgb(40, 40, 40);
			button1.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			button1.FlatStyle = FlatStyle.Flat;
			button1.ForeColor = Color.FromArgb(208, 203, 148);
			button1.Location = new Point(861, 210);
			button1.Margin = new Padding(4, 3, 4, 3);
			button1.Name = "button1";
			button1.Size = new Size(134, 27);
			button1.TabIndex = 0;
			button1.Text = "Effect.dat Read WiP";
			button1.UseVisualStyleBackColor = false;
			button1.Click += button1_Click;
			// 
			// btnUpdateHelper
			// 
			btnUpdateHelper.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnUpdateHelper.BackColor = Color.FromArgb(40, 40, 40);
			btnUpdateHelper.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnUpdateHelper.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnUpdateHelper.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnUpdateHelper.FlatStyle = FlatStyle.Flat;
			btnUpdateHelper.ForeColor = Color.FromArgb(208, 203, 148);
			btnUpdateHelper.Location = new Point(861, 177);
			btnUpdateHelper.Margin = new Padding(4, 3, 4, 3);
			btnUpdateHelper.Name = "btnUpdateHelper";
			btnUpdateHelper.Size = new Size(134, 27);
			btnUpdateHelper.TabIndex = 0;
			btnUpdateHelper.Text = "Update Helper WiP";
			btnUpdateHelper.UseVisualStyleBackColor = false;
			btnUpdateHelper.Click += btnUpdateHelper_Click;
			// 
			// btnCompareFiles
			// 
			btnCompareFiles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnCompareFiles.BackColor = Color.FromArgb(40, 40, 40);
			btnCompareFiles.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnCompareFiles.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnCompareFiles.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnCompareFiles.FlatStyle = FlatStyle.Flat;
			btnCompareFiles.ForeColor = Color.FromArgb(208, 203, 148);
			btnCompareFiles.Location = new Point(861, 144);
			btnCompareFiles.Margin = new Padding(4, 3, 4, 3);
			btnCompareFiles.Name = "btnCompareFiles";
			btnCompareFiles.Size = new Size(134, 27);
			btnCompareFiles.TabIndex = 0;
			btnCompareFiles.Text = "Compare Files";
			btnCompareFiles.UseVisualStyleBackColor = false;
			btnCompareFiles.Click += btnCompareFiles_Click;
			// 
			// btnControlPanel
			// 
			btnControlPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnControlPanel.BackColor = Color.FromArgb(40, 40, 40);
			btnControlPanel.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnControlPanel.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnControlPanel.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnControlPanel.FlatStyle = FlatStyle.Flat;
			btnControlPanel.ForeColor = Color.FromArgb(208, 203, 148);
			btnControlPanel.Location = new Point(861, 12);
			btnControlPanel.Margin = new Padding(4, 3, 4, 3);
			btnControlPanel.Name = "btnControlPanel";
			btnControlPanel.Size = new Size(134, 27);
			btnControlPanel.TabIndex = 0;
			btnControlPanel.Text = "Control Panel";
			btnControlPanel.UseVisualStyleBackColor = false;
			btnControlPanel.Click += btnControlPanel_Click;
			// 
			// btnFileEncrypter
			// 
			btnFileEncrypter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnFileEncrypter.BackColor = Color.FromArgb(40, 40, 40);
			btnFileEncrypter.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnFileEncrypter.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnFileEncrypter.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnFileEncrypter.FlatStyle = FlatStyle.Flat;
			btnFileEncrypter.ForeColor = Color.FromArgb(208, 203, 148);
			btnFileEncrypter.Location = new Point(810, 111);
			btnFileEncrypter.Margin = new Padding(4, 3, 4, 3);
			btnFileEncrypter.Name = "btnFileEncrypter";
			btnFileEncrypter.Size = new Size(93, 27);
			btnFileEncrypter.TabIndex = 0;
			btnFileEncrypter.Text = "Encrypt Files";
			btnFileEncrypter.UseVisualStyleBackColor = false;
			btnFileEncrypter.Click += btnFileEncrypter_ClickAsync;
			// 
			// btnFileDecrypter
			// 
			btnFileDecrypter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnFileDecrypter.BackColor = Color.FromArgb(40, 40, 40);
			btnFileDecrypter.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnFileDecrypter.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnFileDecrypter.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnFileDecrypter.FlatStyle = FlatStyle.Flat;
			btnFileDecrypter.ForeColor = Color.FromArgb(208, 203, 148);
			btnFileDecrypter.Location = new Point(902, 111);
			btnFileDecrypter.Margin = new Padding(4, 3, 4, 3);
			btnFileDecrypter.Name = "btnFileDecrypter";
			btnFileDecrypter.Size = new Size(93, 27);
			btnFileDecrypter.TabIndex = 0;
			btnFileDecrypter.Text = "Decrypt Files";
			btnFileDecrypter.UseVisualStyleBackColor = false;
			btnFileDecrypter.Click += btnFileDecrypter_ClickAsync;
			// 
			// btnExporter
			// 
			btnExporter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnExporter.BackColor = Color.FromArgb(40, 40, 40);
			btnExporter.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnExporter.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnExporter.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnExporter.FlatStyle = FlatStyle.Flat;
			btnExporter.ForeColor = Color.FromArgb(208, 203, 148);
			btnExporter.Location = new Point(861, 45);
			btnExporter.Margin = new Padding(4, 3, 4, 3);
			btnExporter.Name = "btnExporter";
			btnExporter.Size = new Size(134, 27);
			btnExporter.TabIndex = 0;
			btnExporter.Text = "Exporter";
			btnExporter.UseVisualStyleBackColor = false;
			btnExporter.Click += btnExporter_Click;
			// 
			// rtbConsole
			// 
			rtbConsole.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			rtbConsole.BackColor = Color.FromArgb(28, 30, 31);
			rtbConsole.BorderStyle = BorderStyle.None;
			rtbConsole.Font = new Font("Consolas", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
			rtbConsole.Location = new Point(0, 274);
			rtbConsole.Margin = new Padding(4, 3, 4, 3);
			rtbConsole.Name = "rtbConsole";
			rtbConsole.ReadOnly = true;
			rtbConsole.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
			rtbConsole.Size = new Size(1008, 241);
			rtbConsole.TabIndex = 2;
			rtbConsole.Text = "";
			rtbConsole.KeyDown += rtbConsole_KeyDown;
			// 
			// btnCheckUpdates
			// 
			btnCheckUpdates.BackColor = Color.FromArgb(40, 40, 40);
			btnCheckUpdates.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnCheckUpdates.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnCheckUpdates.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnCheckUpdates.FlatStyle = FlatStyle.Flat;
			btnCheckUpdates.ForeColor = Color.FromArgb(208, 203, 148);
			btnCheckUpdates.Location = new Point(474, 12);
			btnCheckUpdates.Margin = new Padding(4, 3, 4, 3);
			btnCheckUpdates.Name = "btnCheckUpdates";
			btnCheckUpdates.Size = new Size(115, 27);
			btnCheckUpdates.TabIndex = 0;
			btnCheckUpdates.Text = "Check for Updates";
			btnCheckUpdates.UseVisualStyleBackColor = false;
			btnCheckUpdates.Click += btnCheckUpdates_ClickAsync;
			// 
			// cbDockToTop
			// 
			cbDockToTop.AutoSize = true;
			cbDockToTop.ForeColor = Color.FromArgb(208, 203, 148);
			cbDockToTop.Location = new Point(597, 16);
			cbDockToTop.Margin = new Padding(4, 3, 4, 3);
			cbDockToTop.Name = "cbDockToTop";
			cbDockToTop.Size = new Size(92, 19);
			cbDockToTop.TabIndex = 0;
			cbDockToTop.Text = "Dock To Top";
			cbDockToTop.UseVisualStyleBackColor = true;
			cbDockToTop.CheckedChanged += cbDockToTop_CheckedChanged;
			// 
			// lbEditors
			// 
			lbEditors.BackColor = Color.FromArgb(28, 30, 31);
			lbEditors.BorderStyle = BorderStyle.None;
			lbEditors.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
			lbEditors.ForeColor = Color.FromArgb(208, 203, 148);
			lbEditors.FormattingEnabled = true;
			lbEditors.IntegralHeight = false;
			lbEditors.Location = new Point(0, 0);
			lbEditors.Margin = new Padding(4, 3, 4, 3);
			lbEditors.Name = "lbEditors";
			lbEditors.Size = new Size(250, 274);
			lbEditors.TabIndex = 1;
			lbEditors.KeyDown += lbEditors_KeyDown;
			lbEditors.MouseUp += lbEditors_MouseUp;
			// 
			// Status
			// 
			Status.Location = new Point(0, 515);
			Status.Name = "Status";
			Status.Padding = new Padding(1, 0, 16, 0);
			Status.Size = new Size(1008, 22);
			Status.TabIndex = 14;
			// 
			// btnDBStringTranslator
			// 
			btnDBStringTranslator.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btnDBStringTranslator.BackColor = Color.FromArgb(40, 40, 40);
			btnDBStringTranslator.FlatAppearance.BorderColor = Color.FromArgb(91, 85, 76);
			btnDBStringTranslator.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
			btnDBStringTranslator.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 56, 54);
			btnDBStringTranslator.FlatStyle = FlatStyle.Flat;
			btnDBStringTranslator.ForeColor = Color.FromArgb(208, 203, 148);
			btnDBStringTranslator.Location = new Point(861, 78);
			btnDBStringTranslator.Margin = new Padding(4, 3, 4, 3);
			btnDBStringTranslator.Name = "btnDBStringTranslator";
			btnDBStringTranslator.Size = new Size(134, 27);
			btnDBStringTranslator.TabIndex = 15;
			btnDBStringTranslator.Text = "DB String Translator";
			btnDBStringTranslator.UseVisualStyleBackColor = false;
			btnDBStringTranslator.Click += btnDBStringTranslator_Click;
			// 
			// cbMaximizeOnStartUp
			// 
			cbMaximizeOnStartUp.AutoSize = true;
			cbMaximizeOnStartUp.ForeColor = Color.FromArgb(208, 203, 148);
			cbMaximizeOnStartUp.Location = new Point(697, 16);
			cbMaximizeOnStartUp.Margin = new Padding(4, 3, 4, 3);
			cbMaximizeOnStartUp.Name = "cbMaximizeOnStartUp";
			cbMaximizeOnStartUp.Size = new Size(137, 19);
			cbMaximizeOnStartUp.TabIndex = 16;
			cbMaximizeOnStartUp.Text = "Maximize On StartUp";
			cbMaximizeOnStartUp.UseVisualStyleBackColor = true;
			cbMaximizeOnStartUp.CheckedChanged += cbMaximizeOnStartUp_CheckedChanged;
			// 
			// Main
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(40, 40, 40);
			ClientSize = new Size(1008, 537);
			Controls.Add(lbEditors);
			Controls.Add(cbMaximizeOnStartUp);
			Controls.Add(btnDBStringTranslator);
			Controls.Add(Status);
			Controls.Add(button1);
			Controls.Add(btnUpdateHelper);
			Controls.Add(cbDockToTop);
			Controls.Add(btnCompareFiles);
			Controls.Add(btnControlPanel);
			Controls.Add(rtbConsole);
			Controls.Add(btnCheckUpdates);
			Controls.Add(btnFileEncrypter);
			Controls.Add(btnFileDecrypter);
			Controls.Add(btnReloadSettings);
			Controls.Add(btnExporter);
			Controls.Add(btnReconnect);
			Icon = (Icon)resources.GetObject("$this.Icon");
			Margin = new Padding(4, 3, 4, 3);
			MinimumSize = new Size(1024, 576);
			Name = "Main";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "LastChaos ToolBoxNG";
			FormClosing += Main_FormClosing;
			Load += Main_Load;
			Shown += Main_Shown;
			ResizeEnd += Main_ResizeEnd;
			ResumeLayout(false);
			PerformLayout();

		}
		#endregion
		private System.Windows.Forms.Button btnReconnect;
        private System.Windows.Forms.Button btnReloadSettings;
        private System.Windows.Forms.Timer Monitor;
        private System.Windows.Forms.RichTextBox rtbConsole;
		private System.Windows.Forms.Button btnCheckUpdates;
		private System.Windows.Forms.Button btnCompareFiles;
		private System.Windows.Forms.Button btnExporter;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button btnFileDecrypter;
		private System.Windows.Forms.Button btnFileEncrypter;
		private System.Windows.Forms.Button btnControlPanel;
		private System.Windows.Forms.Button btnUpdateHelper;
		private System.Windows.Forms.CheckBox cbDockToTop;
		private System.Windows.Forms.ListBox lbEditors;
		private System.Windows.Forms.StatusStrip Status;
		private Button btnDBStringTranslator;
		private CheckBox cbMaximizeOnStartUp;
	}
}

