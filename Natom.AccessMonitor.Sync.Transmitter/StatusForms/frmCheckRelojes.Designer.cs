
namespace Natom.AccessMonitor.Sync.Transmitter
{
    partial class frmCheckRelojes
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
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.picScream = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picScream)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(36)))));
            this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtStatus.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.txtStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStatus.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.txtStatus.Location = new System.Drawing.Point(24, 85);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(295, 39);
            this.txtStatus.TabIndex = 0;
            this.txtStatus.Text = "CONECTANDO CON RELOJ...";
            this.txtStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // picScream
            // 
            this.picScream.Image = global::Natom.AccessMonitor.Sync.Transmitter.Properties.Resources.influencer;
            this.picScream.Location = new System.Drawing.Point(133, 15);
            this.picScream.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picScream.Name = "picScream";
            this.picScream.Size = new System.Drawing.Size(67, 62);
            this.picScream.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picScream.TabIndex = 2;
            this.picScream.TabStop = false;
            // 
            // frmCheckRelojes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(335, 129);
            this.ControlBox = false;
            this.Controls.Add(this.picScream);
            this.Controls.Add(this.txtStatus);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCheckRelojes";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmSync_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picScream)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.PictureBox picScream;
    }
}