using FluentFTP;

namespace LastChaos_ToolBoxNG
{
	public partial class UpdateHelper : Form
	{
		private readonly Main pMain;
		private string strWorkingDirectory = "UpdateHelperWorkFolder\\";
		private FileSystemWatcher pFileSystemWatcher = new()
		{
			NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName,
			//Filter = "*.txt",
			IncludeSubdirectories = true
		};

		public UpdateHelper(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;

			pFileSystemWatcher.Path = pMain.pSettings.ClientPath;

			pFileSystemWatcher.Changed += AddToList;
			pFileSystemWatcher.Created += AddToList;
			pFileSystemWatcher.Deleted += AddToList;
			pFileSystemWatcher.Renamed += AddToList;
		}

		private void AdjustPathColumn()
		{
			int nOffset = 0;
			int nRowHeight = 19;
			int nVerticalScrollBarWidth = 17;
			int nHeaderRowHeight = 24;

			if ((lvFileList.Items.Count * nRowHeight) + nHeaderRowHeight > lvFileList.Height)
				nOffset = nVerticalScrollBarWidth;

			lvFileList.Columns[2].Width = lvFileList.Width - (lvFileList.Columns[0].Width + lvFileList.Columns[1].Width) - nOffset;
		}

		private void UpdateHelper_ResizeEnd(object sender, EventArgs e) { AdjustPathColumn(); }

		private void UpdateHelper_Load(object sender, EventArgs e) { tvStage.ExpandAll(); }

		private void UpdateHelper_FormClosing(object sender, FormClosingEventArgs e) { pFileSystemWatcher.Dispose(); }

		private void AddToList(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType == WatcherChangeTypes.Changed && Directory.Exists(e.FullPath))
				return;

			string strType = e.ChangeType.ToString();
			string strPaths = e.FullPath;

			if (e is RenamedEventArgs renamedEvent)
			{
				if (renamedEvent.OldFullPath != renamedEvent.FullPath)
				{
					strType = "Moved";
					strPaths = e.FullPath;	//strPaths = "Old Path: " + renamedEvent.OldFullPath + " New Path: " + e.FullPath;
				}
				else
				{
					strType = "Renamed";
				}
			}

			lvFileList.BeginInvoke((MethodInvoker)delegate
			{
				lvFileList.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString("HH:mm:ss"), strType, strPaths }));

				int nLastIndex = lvFileList.Items.Count - 1;

				lvFileList.Items[nLastIndex].Focused = true;
				lvFileList.Items[nLastIndex].EnsureVisible();

				AdjustPathColumn();
			});
		}

		private void btnStartWatcher_Click(object sender, EventArgs e)
		{
			bool bWatcherState = !pFileSystemWatcher.EnableRaisingEvents;

			btnStartWatcher.Text = bWatcherState ? "Stop Watcher" : "Start Watcher";
			btnStartWatcher.BackColor = bWatcherState ? Color.FromArgb(124, 111, 100) : Color.FromArgb(40, 40, 40);

			int nState = 0;

			if (!bWatcherState && lvFileList.Items.Count > 0)
				nState = 1;

			ChangeStageState(0, nState);

			btnMakeUpdateFile.Enabled = (lvFileList.Items.Count > 0) ? true : false;

			pFileSystemWatcher.EnableRaisingEvents = bWatcherState;
		}

		private void ChangeStageState(int nStage, int nState)   // nState Argument: 1 = Completed, 2 = Failed, 3 = Working...
		{
			string strText = tvStage.Nodes[nStage].Text;
			Color Color = Color.FromArgb(208, 203, 148);
			//✅❌
			strText = strText.Replace(" 🔴❌", "");
			strText = strText.Replace("...", "");

			if (nState == 1)
			{
				strText += " 🔴❌";
				Color = Color.FromArgb(9, 127, 10);
			}
			else if (nState == 2)
			{
				strText += " 🔴❌";
				Color = Color.FromArgb(243, 11, 10);
			}
			else if (nState == 3)
			{
				strText += "...";
				Color = Color.FromArgb(235, 235, 23);
			}

			tvStage.Nodes[nStage].Text = strText;
			tvStage.Nodes[nStage].ForeColor = Color;
		}

		private void btnMakeUpdateFile_Click(object sender, EventArgs e)    // TODO: En varios puntos de esta función, hay que poner checks para ver si en alguna etapa, algo falló (capaz usando try y catch)
		{
			MessageBox_Progress pProgressDialog = new(this, "Please Wait...");
			// Cleaning Working Directory
			ChangeStageState(1, 0);

			if (Directory.Exists(strWorkingDirectory))
				Directory.Delete(strWorkingDirectory, true);

			Directory.CreateDirectory(strWorkingDirectory);

			// Creating and copying folders and files
			ChangeStageState(1, 1);

			if (lvFileList.Items.Count > 0)
			{
				ChangeStageState(2, 3);

				string strAbsolutePath, strRelativePath, strName, strSubPath;
				FileAttributes faAttributes;

				foreach (ListViewItem pItem in lvFileList.Items)
				{
					strAbsolutePath = pItem.SubItems[2].Text;

					if (!Directory.Exists(strAbsolutePath) && !File.Exists(strAbsolutePath))
						continue;

					strRelativePath = strAbsolutePath.Replace(pMain.pSettings.ClientPath, "");
					faAttributes = File.GetAttributes(strAbsolutePath);

					if (faAttributes.HasFlag(FileAttributes.Directory)) // Is Folder
					{
						Directory.CreateDirectory(strWorkingDirectory + strRelativePath);
					}
					else    // Is File
					{
						strName = Path.GetFileName(strRelativePath);
						strSubPath = strWorkingDirectory + strRelativePath.Replace(strName, "");

						if (Directory.Exists(strSubPath))   // If folder/s for this file exist; copy the file
						{
							File.Copy(strAbsolutePath, strSubPath + strName, true);
						}
						else    // If folder/s for this file doesn't exists, create the entire path
						{
							Directory.CreateDirectory(strSubPath);

							if (Directory.Exists(strSubPath))
								File.Copy(strAbsolutePath, strSubPath + strName, true);
						}
					}
				}

				// TODO: Acá habría que conectarse a la DB o al servidor FTP para saber que version ponerle de nombre al archivo

				ChangeStageState(2, 1);

				// Compressing Update to .ZIP
				ChangeStageState(3, 3);

				ZipFile.CreateFromDirectory(strWorkingDirectory, "Update.zip", CompressionLevel.Optimal, false);

				ChangeStageState(3, 1);

				// TODO: Falta ver como hacer el update del checklist.txt (fix by CRC & path)

				// TODO: No tengo un servidor FTP instalado

				// TODO: Subir el archivo al servidor FTP
				FtpClient ftpClient = new("127.0.0.1", "username", "pwd");
				// TODO: Cambiar la version minima en la base de datos
			}
			else
			{
				pMain.Logger(LogTypes.Error, "Update Helper > Changes list in empty.");
			}

			pProgressDialog.Close();
		}

		private void btnRemoveSelected_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem pItem in lvFileList.SelectedItems)
				lvFileList.Items.Remove(pItem);
		}

		private void btnClearList_Click(object sender, EventArgs e) { lvFileList.Items.Clear(); }
	}
}
