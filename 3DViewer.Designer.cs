namespace LastChaos_ToolBoxNG
{
    partial class RenderDialog
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
			panel3DView = new Panel();
			cbAlpha = new CheckBox();
			label4 = new Label();
			cbRotation = new CheckBox();
			timerRender = new Timer(components);
			panel3DView.SuspendLayout();
			SuspendLayout();
			// 
			// panel3DView
			// 
			panel3DView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			panel3DView.BackColor = Color.Black;
			panel3DView.Controls.Add(cbAlpha);
			panel3DView.Controls.Add(label4);
			panel3DView.Controls.Add(cbRotation);
			panel3DView.Location = new Point(0, 0);
			panel3DView.Margin = new Padding(4, 3, 4, 3);
			panel3DView.Name = "panel3DView";
			panel3DView.Size = new Size(314, 281);
			panel3DView.TabIndex = 0;
			panel3DView.TabStop = true;
			// 
			// cbAlpha
			// 
			cbAlpha.AutoSize = true;
			cbAlpha.BackColor = Color.Black;
			cbAlpha.Checked = true;
			cbAlpha.CheckState = CheckState.Checked;
			cbAlpha.ForeColor = Color.FromArgb(208, 203, 148);
			cbAlpha.Location = new Point(80, -1);
			cbAlpha.Margin = new Padding(4, 3, 4, 3);
			cbAlpha.Name = "cbAlpha";
			cbAlpha.Size = new Size(57, 19);
			cbAlpha.TabIndex = 3;
			cbAlpha.Text = "Alpha";
			cbAlpha.UseVisualStyleBackColor = false;
			// 
			// label4
			// 
			label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			label4.AutoSize = true;
			label4.BackColor = Color.Black;
			label4.ForeColor = Color.FromArgb(208, 203, 148);
			label4.Location = new Point(90, 267);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(227, 15);
			label4.TabIndex = 2;
			label4.Text = "C Print | Wheel Zoom + Ctrl ↕ Shift↔ Alt↺";
			label4.TextAlign = ContentAlignment.MiddleRight;
			// 
			// cbRotation
			// 
			cbRotation.AutoSize = true;
			cbRotation.BackColor = Color.Black;
			cbRotation.Checked = true;
			cbRotation.CheckState = CheckState.Checked;
			cbRotation.ForeColor = Color.FromArgb(208, 203, 148);
			cbRotation.Location = new Point(1, -1);
			cbRotation.Margin = new Padding(4, 3, 4, 3);
			cbRotation.Name = "cbRotation";
			cbRotation.Size = new Size(71, 19);
			cbRotation.TabIndex = 1;
			cbRotation.Text = "Rotation";
			cbRotation.UseVisualStyleBackColor = false;
			// 
			// timerRender
			// 
			timerRender.Interval = 1;
			timerRender.Tick += timerRender_Tick;
			// 
			// RenderDialog
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.Black;
			ClientSize = new Size(314, 281);
			Controls.Add(panel3DView);
			Icon = Properties.Resources.NG;
			KeyPreview = true;
			Margin = new Padding(4, 3, 4, 3);
			MinimizeBox = false;
			MinimumSize = new Size(330, 320);
			Name = "RenderDialog";
			Text = "3D Viewer";
			TopMost = true;
			FormClosing += RenderDialog_FormClosing;
			Load += RenderDialog_Load;
			Resize += RenderDialog_Resize;
			panel3DView.ResumeLayout(false);
			panel3DView.PerformLayout();
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel3DView;
        private System.Windows.Forms.Timer timerRender;
        private System.Windows.Forms.CheckBox cbRotation;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox cbAlpha;
	}
}