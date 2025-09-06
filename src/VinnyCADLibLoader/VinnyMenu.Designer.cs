namespace VinnyCADLibLoader
{
    partial class VinnyMenu
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.vinnyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VinnyImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vinnyExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.VinnyAboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vinnyToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // vinnyToolStripMenuItem
            // 
            this.vinnyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VinnyImportToolStripMenuItem,
            this.vinnyExportToolStripMenuItem,
            this.toolStripSeparator1,
            this.VinnyAboutToolStripMenuItem});
            this.vinnyToolStripMenuItem.Name = "vinnyToolStripMenuItem";
            this.vinnyToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.vinnyToolStripMenuItem.Text = "Vinny";
            // 
            // VinnyImportToolStripMenuItem
            // 
            this.VinnyImportToolStripMenuItem.Image = global::VinnyCADLibLoader.Resource.vinnyIcon_32x32;
            this.VinnyImportToolStripMenuItem.Name = "VinnyImportToolStripMenuItem";
            this.VinnyImportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.VinnyImportToolStripMenuItem.Text = "Импорт данных";
            this.VinnyImportToolStripMenuItem.Click += new System.EventHandler(this.VinnyImportToolStripMenuItem_Click);
            // 
            // vinnyExportToolStripMenuItem
            // 
            this.vinnyExportToolStripMenuItem.Image = global::VinnyCADLibLoader.Resource.vinnyIcon_32x32;
            this.vinnyExportToolStripMenuItem.Name = "vinnyExportToolStripMenuItem";
            this.vinnyExportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.vinnyExportToolStripMenuItem.Text = "Экспорт данных";
            this.vinnyExportToolStripMenuItem.Click += new System.EventHandler(this.vinnyExportToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // VinnyAboutToolStripMenuItem
            // 
            this.VinnyAboutToolStripMenuItem.Name = "VinnyAboutToolStripMenuItem";
            this.VinnyAboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.VinnyAboutToolStripMenuItem.Text = "О плагине";
            this.VinnyAboutToolStripMenuItem.Click += new System.EventHandler(this.VinnyAboutToolStripMenuItem_Click);
            // 
            // VinnyMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "VinnyMenu";
            this.Text = "VinnyMenu";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem vinnyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem VinnyImportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vinnyExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem VinnyAboutToolStripMenuItem;
    }
}
