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
		private int nSelectedRow, nSelectedCol, nSheetRows, nSheetCols;
		private string strBtnType;
		public string[] ReturnValues = new string[3] { "0", "0", "0" };

		public IconPicker(Main mainForm, Form ParentForm, String strBtnType)
		{
			InitializeComponent();

			pbImageViewer.MouseMove += IconPicker_MouseMove;

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
				string[] strFilePaths = GetIconSheetPaths();

				foreach (string strFilePath in strFilePaths)
					cbFileSelector.Items.Add(Path.GetFileNameWithoutExtension(strFilePath));
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
			if (!TryGetIconCell(e.Location, out int row, out int col))
				return;

			lbLocation.Text = $"Row: {row} Col: {col}";
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			Close();
		}

		private string[] GetIconSheetPaths()
		{
			Dictionary<string, string> pPaths = new(StringComparer.OrdinalIgnoreCase);

			if (Directory.Exists("Resources"))
			{
				foreach (string strFilePath in Directory.GetFiles("Resources", strBtnType + "*.png"))
					pPaths[Path.GetFileNameWithoutExtension(strFilePath)] = strFilePath;
			}

			string strInterfacePath = Path.Combine(pMain.pSettings.ClientPath, "Data", "Interface");
			if (Directory.Exists(strInterfacePath))
			{
				foreach (string strFilePath in Directory.GetFiles(strInterfacePath, strBtnType + "*.tex"))
				{
					string strName = Path.GetFileNameWithoutExtension(strFilePath);
					if (!pPaths.ContainsKey(strName))
						pPaths[strName] = strFilePath;
				}
			}

			return pPaths.Values.OrderBy(ExtractNumberFromFileName).ToArray();
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
				ReturnValues[1] = "0";
				ReturnValues[2] = "0";

				string strPathCompose = GetSelectedIconSheetPath(strSelectedFile);

				Image? pImage = LoadIconSheet(strPathCompose);
				if (pImage != null)
				{
					int nIconSize = GetIconSourceSize(pImage);
					nSheetCols = Math.Max(1, pImage.Width / nIconSize);
					nSheetRows = Math.Max(1, pImage.Height / nIconSize);
					pbImageViewer.SizeMode = pImage.Width == pbImageViewer.Width && pImage.Height == pbImageViewer.Height ? PictureBoxSizeMode.Normal : PictureBoxSizeMode.StretchImage;

					pbImageViewer.Image = pImage;
					lbLocation.Text = "Row: 0 Col: 0";
					pbIcon.Image = pMain.GetIcon(strBtnType, ReturnValues[0], 0, 0);
				}
				else
				{
					pMain.Logger(LogTypes.Error, $"Icon Picker > Something went wrong while try load: ({strPathCompose}).");
					nSheetRows = 0;
					nSheetCols = 0;
					pbImageViewer.Image = null;
				}
			}
		}

		private string GetSelectedIconSheetPath(string strSelectedFile)
		{
			string strPngPath = Path.Combine("Resources", strSelectedFile + ".png");
			if (File.Exists(strPngPath))
				return strPngPath;

			return Path.Combine(pMain.pSettings.ClientPath, "Data", "Interface", strSelectedFile + ".tex");
		}

		private Image? LoadIconSheet(string strPathCompose)
		{
			if (Path.GetExtension(strPathCompose).Equals(".tex", StringComparison.OrdinalIgnoreCase))
				return TexImageLoader.Load(strPathCompose);

			using Image pImage = Image.FromFile(strPathCompose);
			return new Bitmap(pImage);
		}

		private int GetIconSourceSize(Image pImage)
		{
			if (strBtnType == "ComboBtn")
				return 50;

			if ((strBtnType == "ItemBtn" || strBtnType == "SkillBtn") && pImage.Width < 512)
				return 16;

			return 32;
		}

		private bool TryGetIconCell(Point pPoint, out int nRow, out int nCol)
		{
			nRow = 0;
			nCol = 0;

			if (pbImageViewer.Image == null || nSheetRows <= 0 || nSheetCols <= 0 || pPoint.X < 0 || pPoint.Y < 0 || pPoint.X >= pbImageViewer.Width || pPoint.Y >= pbImageViewer.Height)
				return false;

			double dCellWidth = pbImageViewer.Width / (double)nSheetCols;
			double dCellHeight = pbImageViewer.Height / (double)nSheetRows;

			nCol = Math.Min(nSheetCols - 1, (int)(pPoint.X / dCellWidth));
			nRow = Math.Min(nSheetRows - 1, (int)(pPoint.Y / dCellHeight));

			return true;
		}

		private void pbImageViewer_Click(object sender, EventArgs e)
		{
			Point pPoint = pbImageViewer.PointToClient(Cursor.Position);

			if (!TryGetIconCell(pPoint, out nSelectedRow, out nSelectedCol))
				return;

			ReturnValues[1] = nSelectedRow.ToString();
			ReturnValues[2] = nSelectedCol.ToString();

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
