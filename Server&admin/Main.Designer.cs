namespace WindowsFormsApp6
{
    partial class Main
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
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.구역알림ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.기록조회ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.구역알림ToolStripMenuItem,
            this.기록조회ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1184, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 구역알림ToolStripMenuItem
            // 
            this.구역알림ToolStripMenuItem.Name = "구역알림ToolStripMenuItem";
            this.구역알림ToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.구역알림ToolStripMenuItem.Text = "구역알림";
            this.구역알림ToolStripMenuItem.Click += new System.EventHandler(this.구역알림ToolStripMenuItem_Click);
            // 
            // 기록조회ToolStripMenuItem
            // 
            this.기록조회ToolStripMenuItem.Name = "기록조회ToolStripMenuItem";
            this.기록조회ToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.기록조회ToolStripMenuItem.Text = "기록조회";
            this.기록조회ToolStripMenuItem.Click += new System.EventHandler(this.기록조회ToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 구역알림ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 기록조회ToolStripMenuItem;
    }
}