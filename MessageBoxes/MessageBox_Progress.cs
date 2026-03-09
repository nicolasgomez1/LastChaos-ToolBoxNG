#define ENABLE_PROGRESSBAR

namespace LastChaos_ToolBoxNG
{
	/*// For call new Instance
	MessageBox_Progress pProgressDialog = new(this, "Loading Data, Please Wait...", false);

	// For Update text from existing Instance
	pProgressDialog.UpdateText("Spoon");

	// For Update Progress Bar
	pProgressDialog.UpdateProgress(50);

	// For close Instance
	pProgressDialog.Close();
	/****************************************/
	public class MessageBox_Progress
	{
		private Form pDialogForm;
		private Label pLabel;
#if ENABLE_PROGRESSBAR
		private ProgressBar? pProgressBar;

		public MessageBox_Progress(Form pParentForm, string strMsg, bool bProgressBar = false)
#else
		public MessageBox_Progress(Form pParentForm, string strMsg)
#endif
		{
			pDialogForm = new Form();
			pDialogForm.ShowInTaskbar = false;
			pDialogForm.FormBorderStyle = FormBorderStyle.None;
			pDialogForm.StartPosition = FormStartPosition.Manual;
			pDialogForm.Size = new Size(200, 70);
			pDialogForm.TopMost = true;
			pDialogForm.BackColor = Color.FromArgb(40, 40, 40);
			pDialogForm.Location = new Point(pParentForm.Location.X + (pParentForm.Width - pDialogForm.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - pDialogForm.Height) / 2);

			Panel Panel = new();
			Panel.Dock = DockStyle.Fill;
			Panel.BorderStyle = BorderStyle.FixedSingle;

			pLabel = new Label();
			pLabel.Text = strMsg;
			pLabel.Dock = DockStyle.Fill;
			pLabel.TextAlign = ContentAlignment.MiddleCenter;
			pLabel.ForeColor = Color.FromArgb(208, 203, 148);
			pLabel.Font = new Font(pLabel.Font.FontFamily, 12);

			Panel.Controls.Add(pLabel);
#if ENABLE_PROGRESSBAR
			if (bProgressBar)
			{
				pProgressBar = new ProgressBar();
				pProgressBar.Dock = DockStyle.Bottom;
				pProgressBar.Style = ProgressBarStyle.Marquee;
				pProgressBar.BackColor = Color.FromArgb(28, 30, 31);
				pProgressBar.ForeColor = Color.FromArgb(208, 203, 148);
				Panel.Controls.Add(pProgressBar);
			}
#endif
			pDialogForm.Controls.Add(Panel);

			pDialogForm.Show();
			pDialogForm.Update();

			ResizeForm();
		}

		private void ResizeForm() {
			Size sizeCaption = TextRenderer.MeasureText(pLabel.Text, pLabel.Font);

			pDialogForm.Size = new Size(sizeCaption.Width + 2, pDialogForm.Height);
		}
#if ENABLE_PROGRESSBAR
		public void UpdateText(string strText)
		{
			pLabel.Invoke((MethodInvoker)delegate
			{
				pLabel.Text = strText;

				ResizeForm();
			});
		}

		public void UpdateProgress(int nProgress)
		{
			if (pProgressBar != null)
			{
				pProgressBar.Invoke((MethodInvoker)delegate
				{
					int nProgcess = Math.Clamp(nProgress, 0, 100);

					if (nProgress > 0)
						pProgressBar.Style = ProgressBarStyle.Continuous;

					pProgressBar.Value = nProgcess;
				});
			}
		}
#endif
		public void Close()
		{
			if (pDialogForm != null && !pDialogForm.IsDisposed)
				pDialogForm.Invoke((MethodInvoker)delegate { pDialogForm.Close(); });
		}
	}
}
