namespace LeapProject
{
    partial class FormControlGroup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormControlGroup));
            this.tbMaster = new System.Windows.Forms.TrackBar();
            this.pictureBoxRight = new System.Windows.Forms.PictureBox();
            this.pictureBoxTop = new System.Windows.Forms.PictureBox();
            this.pictureBoxLeft = new System.Windows.Forms.PictureBox();
            this.pictureBoxBottom = new System.Windows.Forms.PictureBox();
            this.pictureBoxCenter = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaster)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCenter)).BeginInit();
            this.SuspendLayout();
            // 
            // tbMaster
            // 
            this.tbMaster.Location = new System.Drawing.Point(0, 0);
            this.tbMaster.Name = "tbMaster";
            this.tbMaster.Size = new System.Drawing.Size(104, 45);
            this.tbMaster.TabIndex = 6;
            this.tbMaster.Visible = false;
            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxRight.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxRight.Image")));
            this.pictureBoxRight.Location = new System.Drawing.Point(414, 266); //414, 266
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxRight.TabIndex = 57;
            this.pictureBoxRight.TabStop = false;
            // 
            // pictureBoxTop
            // 
            this.pictureBoxTop.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxTop.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxTop.Image")));
            this.pictureBoxTop.Location = new System.Drawing.Point(267, 117);
            this.pictureBoxTop.Name = "pictureBoxTop";
            this.pictureBoxTop.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxTop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxTop.TabIndex = 58;
            this.pictureBoxTop.TabStop = false;
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxLeft.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLeft.Image")));
            this.pictureBoxLeft.Location = new System.Drawing.Point(116, 266);
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLeft.TabIndex = 59;
            this.pictureBoxLeft.TabStop = false;
            // 
            // pictureBoxBottom
            // 
            this.pictureBoxBottom.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxBottom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxBottom.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxBottom.Image")));
            this.pictureBoxBottom.Location = new System.Drawing.Point(267, 415);
            this.pictureBoxBottom.Name = "pictureBoxBottom";
            this.pictureBoxBottom.Size = new System.Drawing.Size(128, 128);
            this.pictureBoxBottom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxBottom.TabIndex = 60;
            this.pictureBoxBottom.TabStop = false;
            // 
            // pictureBoxCenter
            // 
            this.pictureBoxCenter.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCenter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxCenter.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCenter.Image")));
            this.pictureBoxCenter.Location = new System.Drawing.Point(267, 266); //282,281
            this.pictureBoxCenter.Name = "pictureBoxCenter";
            this.pictureBoxCenter.Size = new System.Drawing.Size(128, 128); //100, 100
            this.pictureBoxCenter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCenter.TabIndex = 61;
            this.pictureBoxCenter.TabStop = false;
            // 
            // FormControlGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;//none
         //   this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(659, 659);
            this.Controls.Add(this.pictureBoxCenter);
            this.Controls.Add(this.pictureBoxBottom);
            this.Controls.Add(this.pictureBoxLeft);
            this.Controls.Add(this.pictureBoxTop);
            this.Controls.Add(this.tbMaster);
            this.Controls.Add(this.pictureBoxRight);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormControlGroup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Control";
            this.TopMost = true;
           // this.TransparencyKey = System.Drawing.Color.White;
            this.AllowTransparency = false;
            this.Load += new System.EventHandler(this.FormControlGroup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbMaster)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCenter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbMaster;
        private System.Windows.Forms.PictureBox pictureBoxRight;
        private System.Windows.Forms.PictureBox pictureBoxTop;
        private System.Windows.Forms.PictureBox pictureBoxLeft;
        private System.Windows.Forms.PictureBox pictureBoxBottom;
        private System.Windows.Forms.PictureBox pictureBoxCenter;
    }
}