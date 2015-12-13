namespace LeapProject
{
    partial class FormControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormControl));
            this.tbMaster = new System.Windows.Forms.TrackBar();
            this.timerCheckVol = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelObjectName = new System.Windows.Forms.Label();
            this.buttonBottom = new System.Windows.Forms.Button();
            this.buttonTop = new System.Windows.Forms.Button();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.buttonRight = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonPoint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tbMaster)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbMaster
            // 
            this.tbMaster.LargeChange = 20;
            this.tbMaster.Location = new System.Drawing.Point(205, 13);
            this.tbMaster.Maximum = 100;
            this.tbMaster.Name = "tbMaster";
            this.tbMaster.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbMaster.Size = new System.Drawing.Size(45, 434);
            this.tbMaster.SmallChange = 5;
            this.tbMaster.TabIndex = 1;
            this.tbMaster.TickFrequency = 10;
            this.tbMaster.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbMaster.Visible = false;
            this.tbMaster.Scroll += new System.EventHandler(this.tbMaster_Scroll);
            // 
            // timerCheckVol
            // 
            this.timerCheckVol.Enabled = true;
            this.timerCheckVol.Tick += new System.EventHandler(this.timerCheckVol_Tick);
            // 
            // panel1
            // 
            //this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel1.Controls.Add(this.labelObjectName);
            this.panel1.Controls.Add(this.tbMaster);
            this.panel1.Controls.Add(this.buttonBottom);
            this.panel1.Controls.Add(this.buttonTop);
            this.panel1.Controls.Add(this.buttonLeft);
            this.panel1.Controls.Add(this.buttonRight);
            this.panel1.Location = new System.Drawing.Point(25, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 450);
            this.panel1.TabIndex = 4;
            // 
            // labelObjectName
            // 
            this.labelObjectName.BackColor = System.Drawing.Color.White;
            this.labelObjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelObjectName.Location = new System.Drawing.Point(150, 199);
            this.labelObjectName.Name = "labelObjectName";
            this.labelObjectName.Size = new System.Drawing.Size(150, 59);
            this.labelObjectName.TabIndex = 5;
            this.labelObjectName.Text = "Object";
            this.labelObjectName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonBottom
            // 
            this.buttonBottom.BackColor = System.Drawing.Color.Black;
            this.buttonBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBottom.ForeColor = System.Drawing.Color.White;
            this.buttonBottom.Location = new System.Drawing.Point(150, 300);
            this.buttonBottom.Name = "buttonBottom";
            this.buttonBottom.Size = new System.Drawing.Size(150, 150);
            this.buttonBottom.TabIndex = 5;
            this.buttonBottom.UseVisualStyleBackColor = false;
            this.buttonBottom.Visible = false;
            // 
            // buttonTop
            // 
            this.buttonTop.BackColor = System.Drawing.Color.Black;
            this.buttonTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTop.ForeColor = System.Drawing.Color.White;
            this.buttonTop.Location = new System.Drawing.Point(150, 0);
            this.buttonTop.Name = "buttonTop";
            this.buttonTop.Size = new System.Drawing.Size(150, 150);
            this.buttonTop.TabIndex = 2;
            this.buttonTop.UseVisualStyleBackColor = false;
            this.buttonTop.Visible = false;
            // 
            // buttonLeft
            // 
            this.buttonLeft.BackColor = System.Drawing.Color.Black;
            this.buttonLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLeft.ForeColor = System.Drawing.Color.White;
            this.buttonLeft.Location = new System.Drawing.Point(0, 150);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(150, 150);
            this.buttonLeft.TabIndex = 3;
            this.buttonLeft.Text = "lI>";
            this.buttonLeft.UseVisualStyleBackColor = false;
            this.buttonLeft.Visible = false;
            // 
            // buttonRight
            // 
            this.buttonRight.BackColor = System.Drawing.Color.Black;
            this.buttonRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRight.ForeColor = System.Drawing.Color.White;
            this.buttonRight.Location = new System.Drawing.Point(300, 150);
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.Size = new System.Drawing.Size(150, 150);
            this.buttonRight.TabIndex = 4;
            this.buttonRight.Text = ">>";
            this.buttonRight.UseVisualStyleBackColor = false;
            this.buttonRight.Visible = false;
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 20;
            this.trackBar1.Location = new System.Drawing.Point(430, 287);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 82);
            this.trackBar1.SmallChange = 5;
            this.trackBar1.TabIndex = 3;
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            // 
            // buttonPoint
            // 
            this.buttonPoint.BackColor = System.Drawing.Color.LightSalmon;
            this.buttonPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPoint.Location = new System.Drawing.Point(501, 12);
            this.buttonPoint.Name = "buttonPoint";
            this.buttonPoint.Size = new System.Drawing.Size(26, 29);
            this.buttonPoint.TabIndex = 5;
            this.buttonPoint.Text = "X";
            this.buttonPoint.UseVisualStyleBackColor = false;
            // 
            // FormControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Controls.Add(this.buttonPoint);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.trackBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormControl";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Control";
            this.TopMost = true;
          //  this.TransparencyKey = System.Drawing.SystemColors.;
            ((System.ComponentModel.ISupportInitialize)(this.tbMaster)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbMaster;
        private System.Windows.Forms.Timer timerCheckVol;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonBottom;
        private System.Windows.Forms.Button buttonRight;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonPoint;
        private System.Windows.Forms.Label labelObjectName;
        private System.Windows.Forms.Button buttonTop;
    }
}