namespace LastChaos_ToolBoxNG
{
	public partial class SectionsManager : Form
	{
		private readonly Main pMain;

		public SectionsManager(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
		}

		private void PopulateList()
		{
			MainList.BeginUpdate();
			MainList.Items.Clear();

			IniData pData = (new FileIniDataParser()).ReadFile(pMain.pSettings.SettingsFile);

			foreach (string strSectionName in pData["ControlPanel"][cbSectionSelector.Text].Split(',').Select(s => s.Trim()).ToList())
				MainList.Items.Add(new Main.ListBoxItem { Text = strSectionName });

			MainList.EndUpdate();
		}

		private void SectionsManager_Load(object sender, EventArgs e) { cbSectionSelector.SelectedIndex = 0; }

		private void cbSectionSelector_SelectedIndexChanged(object sender, EventArgs e) { PopulateList(); }

		private void btnAdd_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			MessageBox_Input pInput = new(this, "Please enter new Section name:");
			
			if (pInput.ShowDialog() == DialogResult.OK)
			{
				try
				{
					FileIniDataParser pParser = new();
					IniData pData = pParser.ReadFile(pMain.pSettings.SettingsFile);
					List<string> SectionsNames = pData["ControlPanel"][cbSectionSelector.Text].Split(',').Select(s => s.Trim()).ToList();

					SectionsNames.Add(pInput.strOutput);

					pData["ControlPanel"][cbSectionSelector.Text] = string.Join(", ", SectionsNames);

					pParser.WriteFile(pMain.pSettings.SettingsFile, pData);
				}
				catch (Exception ex)
				{
					pMain.Logger(LogTypes.Error, $"Sections Manager > {ex.Message}.");

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						MainList.Items.Add(new Main.ListBoxItem { Text = pInput.strOutput });
						MainList.SelectedIndex = MainList.Items.Count - 1;
					}
				}
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nSelected = MainList.SelectedIndex;

			if (nSelected != -1)
			{
				try
				{
					FileIniDataParser pParser = new();
					IniData pData = pParser.ReadFile(pMain.pSettings.SettingsFile);
					List<string> SectionsNames = pData["ControlPanel"][cbSectionSelector.Text].Split(',').Select(s => s.Trim()).ToList();

					SectionsNames.Remove(MainList.Text);

					pData["ControlPanel"][cbSectionSelector.Text] = string.Join(", ", SectionsNames);

					pParser.WriteFile(pMain.pSettings.SettingsFile, pData);
				}
				catch (Exception ex)
				{
					pMain.Logger(LogTypes.Error, $"Sections Manager > {ex.Message}.");

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						MainList.Items.RemoveAt(nSelected);
						MainList.SelectedIndex = MainList.Items.Count - 1;
					}
				}
			}
		}
	}
}
