namespace LastChaos_ToolBoxNG
{
	public partial class TreasureMapEditor : Form
	{
		private readonly Main pMain;
		private bool bUnsavedChanges = false;
		private int nSearchPosition = 0;
		private Main.ListBoxItem? pLastSelected;
		private DataRow? pTempZoneRow;
		private PointF pMousePosition, pSelectingStartPoint;
		private RectangleF pSelectingArea;
		private bool bDrawing = false;
		private float fWorldRatio = 0f;
		private Dictionary<RectangleF, string> pArrayAreas = new();

		public TreasureMapEditor(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
		}

		private bool CheckUnsavedChanges()
		{
			bool bProceed = true;

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("There are unsaved changes. If you proceed, your changes will be discarded.\nDo you want to continue?", "Treasure Map Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (pDialogReturn != DialogResult.Yes)
					bProceed = false;
			}

			return bProceed;
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
				LoadUIData(nID);

				MainList.SelectedIndexChanged -= MainList_SelectedIndexChanged;
				MainList.SelectedIndex = MainList.Items.Count - 1;
				MainList.SelectedIndexChanged += MainList_SelectedIndexChanged;

				pLastSelected = (Main.ListBoxItem?)MainList.SelectedItem;

				bUnsavedChanges = true;
			}
		}

		private void TreasureMapEditor_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape && bDrawing)
			{
				bDrawing = false;

				pbWorldMap.Cursor = Cursors.Arrow;

				pbWorldMap.Invalidate();
			}
		}

		private void DoEDIT()
		{
			int nSelected = lbAreas.SelectedIndex;

			if (nSelected != -1)
			{
				MessageBox_Input pInput = new(this, "Please set new Regen Max:", pArrayAreas.Values.ElementAt(nSelected));
				if (pInput.ShowDialog() != DialogResult.OK)
					return;

				pArrayAreas[pArrayAreas.Keys.ElementAt(nSelected)] = pInput.strOutput;

				Main.ListBoxItem pSelectedItem = (Main.ListBoxItem)lbAreas.Items[nSelected];
				pSelectedItem.Text = $"Area: {(nSelected + 1)} Regen Max: {pInput.strOutput}";

				lbAreas.Items[nSelected] = pSelectedItem;

				bUnsavedChanges = true;
			}
		}

		private void DoDELETE()
		{
			int nSelected = lbAreas.SelectedIndex;

			if (nSelected != -1)
			{
				DialogResult pDialogReturn = MessageBox.Show("Do you want to delete this Area?", "Treasure Map Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (pDialogReturn != DialogResult.Yes)
					return;

				pArrayAreas.Remove(pArrayAreas.Keys.ElementAt(nSelected));

				pbWorldMap.Invalidate();

				int nPrevObjectID = nSelected <= 0 ? 0 : nSelected - 1;

				lbAreas.Items.RemoveAt(nSelected);

				for (int j = nSelected; j < lbAreas.Items.Count; j++)
				{
					Main.ListBoxItem pUpdatedItem = (Main.ListBoxItem)lbAreas.Items[j];
					pUpdatedItem.Text = $"Area: {(j + 1)} Regen Max: {pUpdatedItem.Text.Split(':')[2]}";

					lbAreas.Items[j] = pUpdatedItem;
				}

				if (lbAreas.Items.Count > 0)
					lbAreas.SelectedIndex = nPrevObjectID;

				bUnsavedChanges = true;
			}
		}

		private async Task LoadZoneDataAsync()
		{
			bool bRequestNeeded = false;
			List<string> listQueryCompose = ["a_name", "a_treasurecount", "a_treasure_area"];

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

		private async void TreasureMapEditor_LoadAsync(object sender, EventArgs e)
		{
			MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...");
			/****************************************/
#if DEBUG
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			await LoadZoneDataAsync();
#if DEBUG
			stopwatch.Stop();
			pMain.Logger(LogTypes.Message, $"Zones Data load took: {stopwatch.ElapsedMilliseconds}ms.");
#endif
			/****************************************/
			if (pMain.pTables.ZoneTable != null)
			{
				MainList.BeginUpdate();

				foreach (DataRow pRow in pMain.pTables.ZoneTable.Rows)
					AddToList(Convert.ToInt32(pRow["a_zone_index"]), pRow["a_name"].ToString() ?? string.Empty, false);

				MainList.SelectedIndex = 0;
				MainList.EndUpdate();
			}
			/****************************************/
			(new ToolTip()).SetToolTip(btnReload, "Reload Zones Data from Database");
			/****************************************/
			MainList.Enabled = true;

			btnReload.Enabled = true;

			pProgressDialog.Close();

			MainList.Focus();
		}

		private void TreasureMapEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			void Clear()
			{
				// Do nothing
			}

			if (bUnsavedChanges)
			{
				DialogResult pDialogReturn = MessageBox.Show("You have unsaved changes. Do you want to discard them and exit?", "Treasure Map Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void LoadUIData(int nZoneID)
		{
			// Reset Controls
			pArrayAreas.Clear();
			/****************************************/
			pTempZoneRow = pMain.pTables.ZoneTable.NewRow();
			pTempZoneRow.ItemArray = (object[])pMain.pTables.ZoneTable.Select("a_zone_index=" + nZoneID)[0].ItemArray.Clone();

			int nTreasureCount = Convert.ToInt32(pTempZoneRow["a_treasurecount"]);
			string[] strArrayTreasureAreasData = (pTempZoneRow["a_treasure_area"].ToString() ?? string.Empty).Split(' ');
			var (pImage, pWorldRatio) = pMain.GetWorldMap(nZoneID + "0");

			fWorldRatio = pWorldRatio;

			pbWorldMap.Image = pImage;

			lbAreas.Items.Clear();
			lbAreas.BeginUpdate();

			for (int i = 0; i < nTreasureCount * 5; i += 5)
			{
				float fLeft = Convert.ToInt32(strArrayTreasureAreasData[i]) / (Defs.m_fZoomDetail / fWorldRatio);
				float fTop = Convert.ToInt32(strArrayTreasureAreasData[i + 1]) / (Defs.m_fZoomDetail / fWorldRatio);
				float fRight = Convert.ToInt32(strArrayTreasureAreasData[i + 2]) / (Defs.m_fZoomDetail / fWorldRatio);
				float fBottom = Convert.ToInt32(strArrayTreasureAreasData[i + 3]) / (Defs.m_fZoomDetail / fWorldRatio);
				string strRegenMax = strArrayTreasureAreasData[i + 4].ToString();

				if (fWorldRatio > 0.3333f)  // Hardcode!
				{
					fLeft /= 4;
					fTop /= 4;
					fRight /= 4;
					fBottom /= 4;
				}

				pArrayAreas.Add(
					new RectangleF(
						fLeft,              // Left
						fTop,               // Top
						fRight - fLeft,     // Right
						fBottom - fTop      // Bottom
					),
					strRegenMax
				);

				int nAreaID = i / 5;

				lbAreas.Items.Add(new Main.ListBoxItem
				{
					ID = nAreaID,
					Text = $"Area: {(nAreaID + 1)} Regen Max: {strRegenMax}"
				});
			}

			if (lbAreas.Items.Count > 0)
				lbAreas.SelectedIndex = 0;

			lbAreas.EndUpdate();
			/****************************************/
			btnUpdate.Enabled = true;
		}

		private void tbSearch_TextChanged(object sender, EventArgs e) { nSearchPosition = 0; }

		private void tbSearch_KeyDown(object sender, KeyEventArgs e) { nSearchPosition = pMain.SearchInListBox(tbSearch, e, MainList, nSearchPosition); }

		private void MainList_SelectedIndexChanged(object? sender, EventArgs e)
		{
			if (MainList.SelectedItem is not Main.ListBoxItem pSelectedItem)
				return;

			if (CheckUnsavedChanges())
			{
				bUnsavedChanges = false;

				LoadUIData(pSelectedItem.ID);
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

				pMain.pTables.ZoneTable?.Dispose();
				pMain.pTables.ZoneTable = null;

				btnUpdate.Enabled = false;

				TreasureMapEditor_LoadAsync(sender, e);
			}

			if (CheckUnsavedChanges())
			{
				bUnsavedChanges = false;

				Reload();
			}
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Hold Left Clik and Move in Map image to Create new Area.\n" +
				"Left Click over Area in Map image to edit Regen Max.\n" +
				"Right Click over Area in Map image to remove it.\n" +
				"Pess 'Supr' or 'Backspace' in Areas Data list to remove it.\n" +
				"Press 'Enter' in Areas Data list to edit Regen Max.\n" +
				"Press 'ESC' when drawing to cancel.", "Treasure Map Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void lbAreas_SelectedIndexChanged(object sender, EventArgs e) { pbWorldMap.Invalidate(); }

		private void lbAreas_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				DoEDIT();
			else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
				DoDELETE();
		}

		private void pbWorldMap_Paint(object sender, PaintEventArgs e)
		{
			using (Font pFont = new("Arial", 10))
			{
				int i = 0;
				foreach (var pValues in pArrayAreas)    // Draw from stored rectangles
				{
					Pen pPen = Pens.Black;
					Brush pBrush = Brushes.Black;

					if (lbAreas.SelectedIndex == i)
					{
						pPen = Pens.Red;
						pBrush = Brushes.Red;
					}

					e.Graphics.DrawRectangle(pPen, pValues.Key.X, pValues.Key.Y, pValues.Key.Width, pValues.Key.Height);

					string strNumber = (i + 1).ToString();
					SizeF pTextSize = e.Graphics.MeasureString(strNumber, pFont);

					e.Graphics.DrawString(strNumber, pFont, pBrush, new PointF(pValues.Key.X + (pValues.Key.Width - pTextSize.Width) / 2, pValues.Key.Y + (pValues.Key.Height - pTextSize.Height) / 2));

					i++;
				}

				if (bDrawing)   // Draw if dragging
				{
					e.Graphics.DrawRectangle(Pens.Fuchsia, pSelectingArea.X, pSelectingArea.Y, pSelectingArea.Width, pSelectingArea.Height);

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

					string strCoords = $"Left: {(int)fLeft} Top: {(int)fTop} Right: {(int)fRight} Bottom: {(int)fBottom}";
					SizeF pTextSize = e.Graphics.MeasureString(strCoords, pFont);
					PointF pCoordsTextPos = new PointF(pMousePosition.X, pMousePosition.Y);

					if (pCoordsTextPos.X + pTextSize.Width > pbWorldMap.Width)
						pCoordsTextPos.X = pMousePosition.X - pTextSize.Width;  // Move Text Left

					if (pCoordsTextPos.Y + pTextSize.Height > pbWorldMap.Height)
						pCoordsTextPos.Y = pMousePosition.Y - pTextSize.Height; // Move Text Up

					e.Graphics.DrawString(strCoords, pFont, Brushes.White, pCoordsTextPos);
				}
			}
		}

		private void pbWorldMap_MouseClick(object sender, MouseEventArgs e)
		{
			if (!bDrawing)
			{
				int i = 0;
				foreach (var pValues in pArrayAreas)
				{
					if (pValues.Key.Contains(e.Location))
					{
						lbAreas.SelectedIndex = i;

						if (e.Button == MouseButtons.Left)
							DoEDIT();
						else
							DoDELETE();

						break;
					}

					i++;
				}
			}
		}

		private void pbWorldMap_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				bDrawing = true;
				pSelectingStartPoint = e.Location;
				pSelectingArea = new RectangleF(pSelectingStartPoint, new Size(0, 0));
			}
		}

		private void pbWorldMap_MouseMove(object sender, MouseEventArgs e)
		{
			if (bDrawing)
			{
				pMousePosition = e.Location;

				pbWorldMap.Cursor = Cursors.Cross;

				pSelectingArea = new RectangleF(
					Math.Min(pSelectingStartPoint.X, e.X),   // Left
					Math.Min(pSelectingStartPoint.Y, e.Y),   // Top
					Math.Abs(e.X - pSelectingStartPoint.X),  // Right
					Math.Abs(e.Y - pSelectingStartPoint.Y)   // Bottom
				);

				// Prevent get out of pbWorldMap while is Selecting
				Point pCursorPos = Cursor.Position;
				Point pScreenPos = pbWorldMap.PointToScreen(Point.Empty);

				Cursor.Position = new Point(Math.Clamp(pCursorPos.X, pScreenPos.X, pScreenPos.X + pbWorldMap.Width), Math.Clamp(pCursorPos.Y, pScreenPos.Y, pScreenPos.Y + pbWorldMap.Height));

				pbWorldMap.Invalidate();
			}
		}

		private void pbWorldMap_MouseUp(object sender, MouseEventArgs e)
		{
			if (bDrawing)
			{
				bDrawing = false;

				pbWorldMap.Cursor = Cursors.Arrow;

				if (pSelectingArea.Width < 5 || pSelectingArea.Height < 5)
				{
					pbWorldMap_MouseClick(sender, e);
					return;
				}

				MessageBox_Input pInput = new(this, "Please set Regen Max:", "6");
				if (pInput.ShowDialog() != DialogResult.OK)
				{
					pbWorldMap.Invalidate();
					return;
				}

				lbAreas.Items.Add(new Main.ListBoxItem
				{
					ID = lbAreas.Items.Count,
					Text = $"Area: {(lbAreas.Items.Count + 1)} Regen Max: {pInput.strOutput}"
				});

				lbAreas.SelectedIndex = lbAreas.Items.Count - 1;

				if (!pArrayAreas.ContainsKey(pSelectingArea))
					pArrayAreas.Add(pSelectingArea, pInput.strOutput);
				else
					pMain.Logger(LogTypes.Error, "Treasure Map Editor > Cannot add exact same Area.");

				pbWorldMap.Invalidate();

				bUnsavedChanges = true;
			}
		}

		private void btnUpdate_Click(object sender, EventArgs e)
		{
			bool bSuccess = true;
			int nZoneID = Convert.ToInt32(pTempZoneRow["a_zone_index"]);
			StringBuilder strbuilderTreasureArea = new();

			if (pArrayAreas.Count > 0)
			{
				foreach (var pValues in pArrayAreas)
				{
					float fLeft = (float)Math.Round(pValues.Key.Left * (Defs.m_fZoomDetail / fWorldRatio));
					float fTop = (float)Math.Round(pValues.Key.Top * (Defs.m_fZoomDetail / fWorldRatio));
					float fRight = fLeft + (float)Math.Round(pValues.Key.Width * (Defs.m_fZoomDetail / fWorldRatio));
					float fBottom = fTop + (float)Math.Round(pValues.Key.Height * (Defs.m_fZoomDetail / fWorldRatio));

					if (fWorldRatio > 0.3333f)  // Hardcode!
					{
						fLeft *= 4;
						fTop *= 4;
						fRight *= 4;
						fBottom *= 4;
					}

					strbuilderTreasureArea.Append($"{(int)fLeft} {(int)fTop} {(int)fRight} {(int)fBottom} {pValues.Value} ");
				}

				strbuilderTreasureArea.Length -= 1;
			}

			if (pMain.QueryUpdateInsertDelete(pMain.pSettings.DBCharset, $"UPDATE {pMain.pSettings.DBData}.t_zonedata SET a_treasurecount={lbAreas.Items.Count}, a_treasure_area='{strbuilderTreasureArea}' WHERE a_zone_index={nZoneID};", out long _))
			{
				try
				{
					DataRow? pZoneRow = pMain.pTables.ZoneTable?.Select("a_zone_index=" + nZoneID).FirstOrDefault();
					if (pZoneRow != null)
					{
						pZoneRow["a_treasurecount"] = lbAreas.Items.Count;
						pZoneRow["a_treasure_area"] = strbuilderTreasureArea;
					}
				}
				catch (Exception ex)
				{
					string strError = $"Treasure Map Editor > Zone: {nZoneID} Changes applied in DataBase, but something got wrong while transferring temp data to main table. Please restart the application ({ex.Message}).";

					pMain.Logger(LogTypes.Error, strError);

					MessageBox.Show(strError, "Treasure Map Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
					{
						MessageBox.Show("Changes applied successfully!", "Treasure Map Editor", MessageBoxButtons.OK);

						bUnsavedChanges = false;
					}
				}
			}
			else
			{
				string strError = $"Treasure Map Editor > Zone: {nZoneID} Something got wrong while trying to execute the MySQL query. Changes not applied.";

				pMain.Logger(LogTypes.Error, strError);

				MessageBox.Show(strError, "Treasure Map Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
