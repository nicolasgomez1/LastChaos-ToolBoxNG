namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Main<Pointer to Main Form>
	 *	Form<Parent Form to center the Window>
	 *	String<name the image type>
	 * Returns:
	 *	String Array<File Number, Row, Col>
	// Call and receive implementation
	IconPicker pIconSelector = new(pMain, this, "ItemBtn");
	if (pIconSelector.ShowDialog() != DialogResult.OK)
		return;

	string[] strReturns = pIconSelector.ReturnValues;
	/****************************************/
	public partial class IconPicker : Form
	{
		private readonly Main pMain;
		private Form pParentForm;
		private double dX, dY, dIconSize;
		private string strBtnType;
		public string[] ReturnValues = new string[3] { "0", "0", "0" };

		public IconPicker(Main mainForm, Form ParentForm, String strBtnType)
		{
			InitializeComponent();

			foreach (Control control in Controls)
				control.MouseMove += IconPicker_MouseMove;

			pMain = mainForm;
			pParentForm = ParentForm;
			this.strBtnType = strBtnType;
		}

		private void IconPicker_Load(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			if (strBtnType == "ComboBtn")
			{
				this.MinimumSize = new Size(this.Width, this.Height + 18);

				cbFileSelector.Location = new Point(13, 21);

				pbIcon.Width = 50;
				pbIcon.Height = 50;
				pbIcon.Image = Properties.Resources.DefaultMonster;

				btnSelect.Location = new Point(302, 19);

				lbLocation.Location = new Point(450, 25);

				pbImageViewer.Location = new Point(13, 64);
			}

			cbFileSelector.BeginUpdate();

			try
			{
				string[] strFilePaths = Directory.GetFiles("Resources", strBtnType + "*.png");

				strFilePaths = strFilePaths.OrderBy(f => ExtractNumberFromFileName(f)).ToArray();

				foreach (string strFilePath in strFilePaths)
				{
					if (Path.GetExtension(strFilePath) == ".png")
						cbFileSelector.Items.Add(Path.GetFileNameWithoutExtension(strFilePath));
				}
			}
			catch (Exception ex)
			{
				pMain.Logger(LogTypes.Error, "Icon Picker > " + ex.Message);
			}

			cbFileSelector.EndUpdate();

			if (cbFileSelector.Items.Count > 0)
			{
				cbFileSelector.SelectedIndex = 0;
			}
			else
			{
				pMain.Logger(LogTypes.Error, $"Icon Picker > No {strBtnType}*.png files found in Resources. Icon picking disabled.");
				btnSelect.Enabled = false;
				pbImageViewer.Image = strBtnType switch
				{
					"ComboBtn" => Properties.Resources.DefaultMonster,
					"SkillBtn" => Properties.Resources.DefaultSkill,
					_ => Properties.Resources.DefaultItem
				};
			}

			(new ToolTip()).SetToolTip(pbImageViewer, "Can press Ctrl when do Left Click for instant Pick and Close");
		}

		private void IconPicker_MouseMove(object? sender, MouseEventArgs e)
		{
			if (dIconSize <= 0)
				return;

			dX = Math.Floor(e.X / dIconSize);
			dY = Math.Floor(e.Y / dIconSize);

			lbLocation.Text = $"Row: {dY} Col: " + dX;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			Close();
		}

		private int ExtractNumberFromFileName(string fileName)
		{
			string strNumber = Path.GetFileNameWithoutExtension(fileName);

			strNumber = new string(strNumber.Where(char.IsDigit).ToArray());

			if (int.TryParse(strNumber, out int result))
				return result;
			else
				return int.MaxValue;
		}

		private void cbFileSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbFileSelector.SelectedItem != null)
			{
				btnSelect.Enabled = false;

				string strSelectedFile = cbFileSelector.SelectedItem.ToString() ?? string.Empty;

				ReturnValues[0] = strSelectedFile.Replace(strBtnType, "");

				string strPathCompose = $"Resources\\{strSelectedFile}.png";

				Image pImage = Image.FromFile(strPathCompose);
				if (pImage != null)
				{
					if (strBtnType == "ItemBtn")
					{
						if (pImage.Width == 512 && pImage.Height == 512)
						{
							dIconSize = 32.0;
							pbImageViewer.SizeMode = PictureBoxSizeMode.Normal;
						}
						else
						{
							dIconSize = 16.0;
							pbImageViewer.SizeMode = PictureBoxSizeMode.StretchImage;
						}
					}
					else if (strBtnType == "ComboBtn")
					{
						dIconSize = 50.0;
						pbImageViewer.SizeMode = PictureBoxSizeMode.Normal;
					}

					pbImageViewer.Image = pImage;
				}
				else
				{
					pMain.Logger(LogTypes.Error, $"Icon Picker > Something went wrong while try load: ({strPathCompose}).");
				}
			}
		}

		private void pbImageViewer_Click(object sender, EventArgs e)
		{
			if (dIconSize <= 0)
				return;

			ReturnValues[1] = dY.ToString();
			ReturnValues[2] = dX.ToString();

			btnSelect.Enabled = true;

			pbIcon.Image = pMain.GetIcon(strBtnType, ReturnValues[0], Convert.ToInt32(ReturnValues[1]), Convert.ToInt32(ReturnValues[2]));

			if (Control.ModifierKeys == Keys.Control)	// NOTE: Thats avoid everything
			{
				DialogResult = DialogResult.OK;

				Close();
			}
		}
	}
}
