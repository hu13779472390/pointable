namespace Pointable
{
    partial class FormFullScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFullScreen));
            this.timerSlow = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerSlow
            // 
            this.timerSlow.Interval = 50;
            this.timerSlow.Tick += new System.EventHandler(this.timerSlow_Tick);
            // 
            // FormFullScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(746, 472);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "FormFullScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Pointable Tutorial";
            this.Activated += new System.EventHandler(this.FormFullScreen_Activated);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerSlow;
    }
}