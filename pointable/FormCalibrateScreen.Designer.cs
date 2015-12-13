namespace LeapProject
{
    partial class FormCalibrateScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCalibrateScreen));
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonRestart = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.timerAlternate = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxDone = new System.Windows.Forms.PictureBox();
            this.buttonSwitchScreen = new System.Windows.Forms.Button();
            this.timerSlow = new System.Windows.Forms.Timer(this.components);
            this.labelValues = new System.Windows.Forms.Label();
            this.macTrackBar1 = new System.Windows.Forms.TrackBar();
            this.labelValue11 = new System.Windows.Forms.Label();
            this.labelValue14 = new System.Windows.Forms.Label();
            this.labelValue17 = new System.Windows.Forms.Label();
            this.labelValue20 = new System.Windows.Forms.Label();
            this.labelValue23 = new System.Windows.Forms.Label();
            this.labelValue26 = new System.Windows.Forms.Label();
            this.labelValue29 = new System.Windows.Forms.Label();
            this.labelValue32 = new System.Windows.Forms.Label();
            this.labelValue35 = new System.Windows.Forms.Label();
            this.labelValue38 = new System.Windows.Forms.Label();
            this.labelValue41 = new System.Windows.Forms.Label();
            this.labelValue44 = new System.Windows.Forms.Label();
            this.pictureBoxScreenSize = new System.Windows.Forms.PictureBox();
            this.pictureBoxScreenTV = new System.Windows.Forms.PictureBox();
            this.pictureBoxScreenDesktop = new System.Windows.Forms.PictureBox();
            this.pictureBoxScreenLaptop = new System.Windows.Forms.PictureBox();
            this.pictureBoxScreenPoint = new System.Windows.Forms.PictureBox();
            this.pictureBoxScreenDone = new System.Windows.Forms.PictureBox();
            this.buttonInteractionZone = new System.Windows.Forms.Button();
            this.timerStartFirst = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.macTrackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenTV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenDesktop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenLaptop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenDone)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(529, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(294, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select screen size\r\n";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(351, 253);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 250);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonCancel.FlatAppearance.BorderSize = 3;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
            this.buttonCancel.Location = new System.Drawing.Point(947, 9);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(44, 44);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonOKCancel_Click);
            // 
            // buttonRestart
            // 
            this.buttonRestart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonRestart.FlatAppearance.BorderSize = 0;
            this.buttonRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRestart.Image = ((System.Drawing.Image)(resources.GetObject("buttonRestart.Image")));
            this.buttonRestart.Location = new System.Drawing.Point(393, 490);
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.Size = new System.Drawing.Size(216, 47);
            this.buttonRestart.TabIndex = 4;
            this.buttonRestart.UseVisualStyleBackColor = true;
            this.buttonRestart.Click += new System.EventHandler(this.buttonRestart_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonOK.FlatAppearance.BorderSize = 0;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOK.Image = ((System.Drawing.Image)(resources.GetObject("buttonOK.Image")));
            this.buttonOK.Location = new System.Drawing.Point(825, 586);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(117, 47);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxIcon.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxIcon.Image")));
            this.pictureBoxIcon.Location = new System.Drawing.Point(452, 253);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(100, 100);
            this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxIcon.TabIndex = 6;
            this.pictureBoxIcon.TabStop = false;
            this.pictureBoxIcon.Visible = false;
            // 
            // pictureBoxDone
            // 
            this.pictureBoxDone.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxDone.Image")));
            this.pictureBoxDone.Location = new System.Drawing.Point(351, 252);
            this.pictureBoxDone.Name = "pictureBoxDone";
            this.pictureBoxDone.Size = new System.Drawing.Size(300, 250);
            this.pictureBoxDone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxDone.TabIndex = 8;
            this.pictureBoxDone.TabStop = false;
            this.pictureBoxDone.Visible = false;
            // 
            // buttonSwitchScreen
            // 
            this.buttonSwitchScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonSwitchScreen.FlatAppearance.BorderSize = 0;
            this.buttonSwitchScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSwitchScreen.Image = ((System.Drawing.Image)(resources.GetObject("buttonSwitchScreen.Image")));
            this.buttonSwitchScreen.Location = new System.Drawing.Point(52, 581);
            this.buttonSwitchScreen.Name = "buttonSwitchScreen";
            this.buttonSwitchScreen.Size = new System.Drawing.Size(167, 47);
            this.buttonSwitchScreen.TabIndex = 9;
            this.buttonSwitchScreen.UseVisualStyleBackColor = true;
            this.buttonSwitchScreen.Click += new System.EventHandler(this.buttonSwitchScreen_Click);
            // 
            // timerSlow
            // 
            this.timerSlow.Tick += new System.EventHandler(this.timerSlow_Tick);
            // 
            // labelValues
            // 
            this.labelValues.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValues.Location = new System.Drawing.Point(354, 439);
            this.labelValues.Name = "labelValues";
            this.labelValues.Size = new System.Drawing.Size(84, 33);
            this.labelValues.TabIndex = 13;
            this.labelValues.Text = "23 inch";
            this.labelValues.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // macTrackBar1
            // 
            this.macTrackBar1.LargeChange = 2;
            this.macTrackBar1.Location = new System.Drawing.Point(126, 412);
            this.macTrackBar1.Maximum = 44;
            this.macTrackBar1.Minimum = 11;
            this.macTrackBar1.Name = "macTrackBar1";
            this.macTrackBar1.Size = new System.Drawing.Size(735, 45);
            this.macTrackBar1.TabIndex = 14;
            this.macTrackBar1.TickFrequency = 3;
            this.macTrackBar1.Value = 23;
            this.macTrackBar1.ValueChanged += new System.EventHandler(this.macTrackBar1_ValueChanged);
            this.macTrackBar1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.macTrackBar1_MouseDown);
            // 
            // labelValue11
            // 
            this.labelValue11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue11.Location = new System.Drawing.Point(115, 394);
            this.labelValue11.Name = "labelValue11";
            this.labelValue11.Size = new System.Drawing.Size(50, 15);
            this.labelValue11.TabIndex = 15;
            this.labelValue11.Text = "11";
            this.labelValue11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue11.Click += new System.EventHandler(this.labelValue11_Click);
            // 
            // labelValue14
            // 
            this.labelValue14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue14.Location = new System.Drawing.Point(179, 393);
            this.labelValue14.Name = "labelValue14";
            this.labelValue14.Size = new System.Drawing.Size(50, 15);
            this.labelValue14.TabIndex = 16;
            this.labelValue14.Text = "14";
            this.labelValue14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue14.Click += new System.EventHandler(this.labelValue14_Click);
            // 
            // labelValue17
            // 
            this.labelValue17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue17.Location = new System.Drawing.Point(244, 393);
            this.labelValue17.Name = "labelValue17";
            this.labelValue17.Size = new System.Drawing.Size(50, 15);
            this.labelValue17.TabIndex = 17;
            this.labelValue17.Text = "17";
            this.labelValue17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue17.Click += new System.EventHandler(this.labelValue17_Click);
            // 
            // labelValue20
            // 
            this.labelValue20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue20.Location = new System.Drawing.Point(308, 393);
            this.labelValue20.Name = "labelValue20";
            this.labelValue20.Size = new System.Drawing.Size(50, 15);
            this.labelValue20.TabIndex = 18;
            this.labelValue20.Text = "20";
            this.labelValue20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue20.Click += new System.EventHandler(this.labelValue20_Click);
            // 
            // labelValue23
            // 
            this.labelValue23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue23.Location = new System.Drawing.Point(371, 393);
            this.labelValue23.Name = "labelValue23";
            this.labelValue23.Size = new System.Drawing.Size(50, 15);
            this.labelValue23.TabIndex = 19;
            this.labelValue23.Text = "23";
            this.labelValue23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue23.Click += new System.EventHandler(this.labelValue23_Click);
            // 
            // labelValue26
            // 
            this.labelValue26.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue26.Location = new System.Drawing.Point(436, 393);
            this.labelValue26.Name = "labelValue26";
            this.labelValue26.Size = new System.Drawing.Size(50, 15);
            this.labelValue26.TabIndex = 20;
            this.labelValue26.Text = "26";
            this.labelValue26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue26.Click += new System.EventHandler(this.labelValue26_Click);
            // 
            // labelValue29
            // 
            this.labelValue29.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue29.Location = new System.Drawing.Point(501, 393);
            this.labelValue29.Name = "labelValue29";
            this.labelValue29.Size = new System.Drawing.Size(50, 15);
            this.labelValue29.TabIndex = 21;
            this.labelValue29.Text = "29";
            this.labelValue29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue29.Click += new System.EventHandler(this.labelValue29_Click);
            // 
            // labelValue32
            // 
            this.labelValue32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue32.Location = new System.Drawing.Point(566, 393);
            this.labelValue32.Name = "labelValue32";
            this.labelValue32.Size = new System.Drawing.Size(50, 15);
            this.labelValue32.TabIndex = 22;
            this.labelValue32.Text = "32";
            this.labelValue32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue32.Click += new System.EventHandler(this.labelValue32_Click);
            // 
            // labelValue35
            // 
            this.labelValue35.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue35.Location = new System.Drawing.Point(630, 393);
            this.labelValue35.Name = "labelValue35";
            this.labelValue35.Size = new System.Drawing.Size(50, 15);
            this.labelValue35.TabIndex = 23;
            this.labelValue35.Text = "35";
            this.labelValue35.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue35.Click += new System.EventHandler(this.labelValue35_Click);
            // 
            // labelValue38
            // 
            this.labelValue38.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue38.Location = new System.Drawing.Point(693, 393);
            this.labelValue38.Name = "labelValue38";
            this.labelValue38.Size = new System.Drawing.Size(50, 15);
            this.labelValue38.TabIndex = 24;
            this.labelValue38.Text = "38";
            this.labelValue38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue38.Click += new System.EventHandler(this.labelValue38_Click);
            // 
            // labelValue41
            // 
            this.labelValue41.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue41.Location = new System.Drawing.Point(758, 393);
            this.labelValue41.Name = "labelValue41";
            this.labelValue41.Size = new System.Drawing.Size(50, 15);
            this.labelValue41.TabIndex = 25;
            this.labelValue41.Text = "41";
            this.labelValue41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue41.Click += new System.EventHandler(this.labelValue41_Click);
            // 
            // labelValue44
            // 
            this.labelValue44.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelValue44.Location = new System.Drawing.Point(822, 393);
            this.labelValue44.Name = "labelValue44";
            this.labelValue44.Size = new System.Drawing.Size(50, 15);
            this.labelValue44.TabIndex = 26;
            this.labelValue44.Text = "44";
            this.labelValue44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelValue44.Click += new System.EventHandler(this.labelValue44_Click);
            // 
            // pictureBoxScreenSize
            // 
            this.pictureBoxScreenSize.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxScreenSize.Image")));
            this.pictureBoxScreenSize.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxScreenSize.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxScreenSize.Name = "pictureBoxScreenSize";
            this.pictureBoxScreenSize.Size = new System.Drawing.Size(1000, 651);
            this.pictureBoxScreenSize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxScreenSize.TabIndex = 27;
            this.pictureBoxScreenSize.TabStop = false;
            // 
            // pictureBoxScreenTV
            // 
            this.pictureBoxScreenTV.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxScreenTV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxScreenTV.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxScreenTV.Image")));
            this.pictureBoxScreenTV.Location = new System.Drawing.Point(679, 215);
            this.pictureBoxScreenTV.Name = "pictureBoxScreenTV";
            this.pictureBoxScreenTV.Size = new System.Drawing.Size(207, 175);
            this.pictureBoxScreenTV.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxScreenTV.TabIndex = 28;
            this.pictureBoxScreenTV.TabStop = false;
            this.pictureBoxScreenTV.Click += new System.EventHandler(this.pictureBoxScreenTV_Click);
            // 
            // pictureBoxScreenDesktop
            // 
            this.pictureBoxScreenDesktop.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxScreenDesktop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxScreenDesktop.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxScreenDesktop.Image")));
            this.pictureBoxScreenDesktop.Location = new System.Drawing.Point(307, 215);
            this.pictureBoxScreenDesktop.Name = "pictureBoxScreenDesktop";
            this.pictureBoxScreenDesktop.Size = new System.Drawing.Size(177, 175);
            this.pictureBoxScreenDesktop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxScreenDesktop.TabIndex = 29;
            this.pictureBoxScreenDesktop.TabStop = false;
            this.pictureBoxScreenDesktop.Click += new System.EventHandler(this.pictureBoxScreenDesktop_Click);
            // 
            // pictureBoxScreenLaptop
            // 
            this.pictureBoxScreenLaptop.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxScreenLaptop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxScreenLaptop.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxScreenLaptop.Image")));
            this.pictureBoxScreenLaptop.Location = new System.Drawing.Point(118, 215);
            this.pictureBoxScreenLaptop.Name = "pictureBoxScreenLaptop";
            this.pictureBoxScreenLaptop.Size = new System.Drawing.Size(177, 175);
            this.pictureBoxScreenLaptop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxScreenLaptop.TabIndex = 30;
            this.pictureBoxScreenLaptop.TabStop = false;
            this.pictureBoxScreenLaptop.Click += new System.EventHandler(this.pictureBoxScreenLaptop_Click);
            // 
            // pictureBoxScreenPoint
            // 
            this.pictureBoxScreenPoint.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxScreenPoint.Image")));
            this.pictureBoxScreenPoint.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxScreenPoint.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxScreenPoint.Name = "pictureBoxScreenPoint";
            this.pictureBoxScreenPoint.Size = new System.Drawing.Size(1000, 651);
            this.pictureBoxScreenPoint.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxScreenPoint.TabIndex = 31;
            this.pictureBoxScreenPoint.TabStop = false;
            // 
            // pictureBoxScreenDone
            // 
            this.pictureBoxScreenDone.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxScreenDone.Image")));
            this.pictureBoxScreenDone.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxScreenDone.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxScreenDone.Name = "pictureBoxScreenDone";
            this.pictureBoxScreenDone.Size = new System.Drawing.Size(1000, 651);
            this.pictureBoxScreenDone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxScreenDone.TabIndex = 32;
            this.pictureBoxScreenDone.TabStop = false;
            // 
            // buttonInteractionZone
            // 
            this.buttonInteractionZone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonInteractionZone.FlatAppearance.BorderSize = 0;
            this.buttonInteractionZone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonInteractionZone.Image = ((System.Drawing.Image)(resources.GetObject("buttonInteractionZone.Image")));
            this.buttonInteractionZone.Location = new System.Drawing.Point(182, 143);
            this.buttonInteractionZone.Name = "buttonInteractionZone";
            this.buttonInteractionZone.Size = new System.Drawing.Size(270, 52);
            this.buttonInteractionZone.TabIndex = 33;
            this.buttonInteractionZone.UseVisualStyleBackColor = true;
            this.buttonInteractionZone.Click += new System.EventHandler(this.buttonInteractionZone_Click);
            // 
            // timerStartFirst
            // 
            this.timerStartFirst.Interval = 200;
            this.timerStartFirst.Tick += new System.EventHandler(this.timerStartFirst_Tick);
            // 
            // FormCalibrateScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.buttonInteractionZone);
            this.Controls.Add(this.pictureBoxScreenLaptop);
            this.Controls.Add(this.pictureBoxScreenDesktop);
            this.Controls.Add(this.pictureBoxScreenTV);
            this.Controls.Add(this.labelValue44);
            this.Controls.Add(this.labelValue41);
            this.Controls.Add(this.labelValue38);
            this.Controls.Add(this.labelValue35);
            this.Controls.Add(this.labelValue32);
            this.Controls.Add(this.labelValue29);
            this.Controls.Add(this.labelValue26);
            this.Controls.Add(this.labelValue23);
            this.Controls.Add(this.labelValue20);
            this.Controls.Add(this.labelValue17);
            this.Controls.Add(this.labelValue14);
            this.Controls.Add(this.labelValue11);
            this.Controls.Add(this.labelValues);
            this.Controls.Add(this.macTrackBar1);
            this.Controls.Add(this.buttonSwitchScreen);
            this.Controls.Add(this.buttonRestart);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.pictureBoxDone);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxScreenPoint);
            this.Controls.Add(this.pictureBoxScreenSize);
            this.Controls.Add(this.pictureBoxScreenDone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCalibrateScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Screen Calibration";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormCalibrate_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCalibrate_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.macTrackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenTV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenDesktop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenLaptop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenDone)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonRestart;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Timer timerAlternate;
        private System.Windows.Forms.PictureBox pictureBoxDone;
        private System.Windows.Forms.Button buttonSwitchScreen;
        private System.Windows.Forms.Timer timerSlow;
        private System.Windows.Forms.Label labelValues;
        private System.Windows.Forms.TrackBar macTrackBar1;
        private System.Windows.Forms.Label labelValue11;
        private System.Windows.Forms.Label labelValue14;
        private System.Windows.Forms.Label labelValue17;
        private System.Windows.Forms.Label labelValue20;
        private System.Windows.Forms.Label labelValue23;
        private System.Windows.Forms.Label labelValue26;
        private System.Windows.Forms.Label labelValue29;
        private System.Windows.Forms.Label labelValue32;
        private System.Windows.Forms.Label labelValue35;
        private System.Windows.Forms.Label labelValue38;
        private System.Windows.Forms.Label labelValue41;
        private System.Windows.Forms.Label labelValue44;
        private System.Windows.Forms.PictureBox pictureBoxScreenSize;
        private System.Windows.Forms.PictureBox pictureBoxScreenTV;
        private System.Windows.Forms.PictureBox pictureBoxScreenDesktop;
        private System.Windows.Forms.PictureBox pictureBoxScreenLaptop;
        private System.Windows.Forms.PictureBox pictureBoxScreenPoint;
        private System.Windows.Forms.PictureBox pictureBoxScreenDone;
        private System.Windows.Forms.Button buttonInteractionZone;
        private System.Windows.Forms.Timer timerStartFirst;
    }
}