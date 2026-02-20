namespace LastChaos_ToolBoxNG
{
	/* Args:
	 *	Form<Parent Form to center the Window>
	 *	String<Prompt to show>
	 *	String Array<Options>
	 * Returns:
	 *	Int<ID of selected Item from ComboBox>
	// Call and receive implementation
	MessageBox_ComboBox pComboBox = new(this, "Please enter a value:", ["Option 1", "Option 2"]);
	if (pComboBox.ShowDialog() == DialogResult.OK && pComboBox.nSelected != -1)
		 return;

	int nSelected = pComboBox.nSelected;
	/****************************************/
	public partial class MessageBox_ComboBox : Form
	{
		private Form pParentForm;
		public int nSelected = -1;

		public MessageBox_ComboBox(Form pParentForm, string strCaption, string[] strOptions)
		{
			InitializeComponent();

			rtbMessage.Text = strCaption;

			cbSelector.Items.AddRange(strOptions);

			this.Width = Math.Max(TextRenderer.MeasureText(strCaption, rtbMessage.Font).Width, 350);

			this.pParentForm = pParentForm;
		}

		private void MessageBox_ComboBox_Load(object sender, EventArgs e)
		{
			this.Location = new Point(pParentForm.Location.X + (pParentForm.Width - this.Width) / 2, pParentForm.Location.Y + (pParentForm.Height - this.Height) / 2);

			cbSelector.SelectedIndex = 0;

			(new ToolTip()).SetToolTip(cbSelector, "Press Enter to Close this Window");
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			nSelected = cbSelector.SelectedIndex;

			Close();
		}

		private void cbSelector_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				btnOk_Click(sender, e);
		}
	}
}
