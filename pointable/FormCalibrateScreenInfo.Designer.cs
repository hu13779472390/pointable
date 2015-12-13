namespace LeapProject
{
    partial class FormCalibrateScreenInfo
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCalibrateScreenInfo));
            this.timerAlternate = new System.Windows.Forms.Timer(this.components);
            this.timerSlow = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxDone = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDone)).BeginInit();
            this.SuspendLayout();
            // 
            // timerSlow
            // 
            this.timerSlow.Tick += new System.EventHandler(this.timerSlow_Tick);
            // 
            // pictureBoxDone
            // 
            this.pictureBoxDone.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxDone.Image")));
            this.pictureBoxDone.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxDone.Name = "pictureBoxDone";
            this.pictureBoxDone.Size = new System.Drawing.Size(1000, 651);
            this.pictureBoxDone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxDone.TabIndex = 11;
            this.pictureBoxDone.TabStop = false;
            // 
            // button1
            // 
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(825, 586);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 47);
            this.button1.TabIndex = 12;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // FormCalibrateScreenInfo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBoxDone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCalibrateScreenInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Interaction Zone";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormCalibrate_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCalibrate_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDone)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerAlternate;
        private System.Windows.Forms.Timer timerSlow;
        private System.Windows.Forms.PictureBox pictureBoxDone;
        private System.Windows.Forms.Button button1;
    }
}