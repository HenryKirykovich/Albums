#nullable disable

namespace AlbumsUI
{
    // This file is the "Designer" half of AlbumsForm.
    // Visual Studio writes/reads it when you drag controls in the form designer
    // (double-click AlbumsForm.cs -> design view). You normally do NOT edit it by hand.
    partial class AlbumsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.dgvAlbums = new System.Windows.Forms.DataGridView();
            this.panelInput = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblArtist = new System.Windows.Forms.Label();
            this.txtArtist = new System.Windows.Forms.TextBox();
            this.lblGenre = new System.Windows.Forms.Label();
            this.txtGenre = new System.Windows.Forms.TextBox();
            this.lblYear = new System.Windows.Forms.Label();
            this.nudYear = new System.Windows.Forms.NumericUpDown();
            this.chkAvailable = new System.Windows.Forms.CheckBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblLive = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlbums)).BeginInit();
            this.panelInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvAlbums  (the table with all albums)
            //
            this.dgvAlbums.AllowUserToAddRows = false;
            this.dgvAlbums.AllowUserToDeleteRows = false;
            this.dgvAlbums.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAlbums.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvAlbums.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAlbums.MultiSelect = false;
            this.dgvAlbums.Name = "dgvAlbums";
            this.dgvAlbums.ReadOnly = true;
            this.dgvAlbums.RowHeadersVisible = false;
            this.dgvAlbums.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAlbums.TabIndex = 0;
            this.dgvAlbums.SelectionChanged += new System.EventHandler(this.dgvAlbums_SelectionChanged);
            //
            // panelInput  (bottom panel with the input fields and buttons)
            //
            this.panelInput.Controls.Add(this.lblTitle);
            this.panelInput.Controls.Add(this.txtTitle);
            this.panelInput.Controls.Add(this.lblArtist);
            this.panelInput.Controls.Add(this.txtArtist);
            this.panelInput.Controls.Add(this.lblGenre);
            this.panelInput.Controls.Add(this.txtGenre);
            this.panelInput.Controls.Add(this.lblYear);
            this.panelInput.Controls.Add(this.nudYear);
            this.panelInput.Controls.Add(this.chkAvailable);
            this.panelInput.Controls.Add(this.btnAdd);
            this.panelInput.Controls.Add(this.btnUpdate);
            this.panelInput.Controls.Add(this.btnDelete);
            this.panelInput.Controls.Add(this.btnRefresh);
            this.panelInput.Controls.Add(this.btnClear);
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInput.Name = "panelInput";
            this.panelInput.Size = new System.Drawing.Size(820, 130);
            this.panelInput.TabIndex = 1;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(10, 17);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Title";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(70, 14);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(230, 27);
            this.txtTitle.TabIndex = 0;
            //
            // lblArtist
            //
            this.lblArtist.AutoSize = true;
            this.lblArtist.Location = new System.Drawing.Point(330, 17);
            this.lblArtist.Name = "lblArtist";
            this.lblArtist.Text = "Artist";
            //
            // txtArtist
            //
            this.txtArtist.Location = new System.Drawing.Point(390, 14);
            this.txtArtist.Name = "txtArtist";
            this.txtArtist.Size = new System.Drawing.Size(230, 27);
            this.txtArtist.TabIndex = 1;
            //
            // lblGenre
            //
            this.lblGenre.AutoSize = true;
            this.lblGenre.Location = new System.Drawing.Point(10, 50);
            this.lblGenre.Name = "lblGenre";
            this.lblGenre.Text = "Genre";
            //
            // txtGenre
            //
            this.txtGenre.Location = new System.Drawing.Point(70, 47);
            this.txtGenre.Name = "txtGenre";
            this.txtGenre.Size = new System.Drawing.Size(230, 27);
            this.txtGenre.TabIndex = 2;
            //
            // lblYear
            //
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(330, 50);
            this.lblYear.Name = "lblYear";
            this.lblYear.Text = "Year";
            //
            // nudYear  (the real maximum year is set in the AlbumsForm constructor)
            //
            this.nudYear.Location = new System.Drawing.Point(390, 47);
            this.nudYear.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            this.nudYear.Minimum = new decimal(new int[] { 1900, 0, 0, 0 });
            this.nudYear.Name = "nudYear";
            this.nudYear.Size = new System.Drawing.Size(90, 27);
            this.nudYear.TabIndex = 3;
            this.nudYear.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            //
            // chkAvailable
            //
            this.chkAvailable.AutoSize = true;
            this.chkAvailable.Checked = true;
            this.chkAvailable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAvailable.Location = new System.Drawing.Point(500, 49);
            this.chkAvailable.Name = "chkAvailable";
            this.chkAvailable.TabIndex = 4;
            this.chkAvailable.Text = "Available";
            this.chkAvailable.UseVisualStyleBackColor = true;
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(10, 85);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 32);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnUpdate
            //
            this.btnUpdate.Location = new System.Drawing.Point(110, 85);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(140, 32);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.Text = "Update selected";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            //
            // btnDelete
            //
            this.btnDelete.Location = new System.Drawing.Point(260, 85);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(140, 32);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Delete selected";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(410, 85);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 32);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnClear
            //
            this.btnClear.Location = new System.Drawing.Point(510, 85);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 32);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear fields";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // statusStrip  (bar at the very bottom)
            //
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblLive});
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.TabIndex = 2;
            //
            // lblStatus  (takes all the free space of the status bar)
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Starting...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblLive
            //
            this.lblLive.Name = "lblLive";
            this.lblLive.Text = "Live updates: OFF";
            //
            // AlbumsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(820, 520);
            this.Controls.Add(this.dgvAlbums);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(820, 480);
            this.Name = "AlbumsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VibeHive - Album Inventory (gRPC)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AlbumsForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AlbumsForm_FormClosed);
            this.Load += new System.EventHandler(this.AlbumsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlbums)).EndInit();
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAlbums;
        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblArtist;
        private System.Windows.Forms.TextBox txtArtist;
        private System.Windows.Forms.Label lblGenre;
        private System.Windows.Forms.TextBox txtGenre;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.NumericUpDown nudYear;
        private System.Windows.Forms.CheckBox chkAvailable;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblLive;
    }
}
