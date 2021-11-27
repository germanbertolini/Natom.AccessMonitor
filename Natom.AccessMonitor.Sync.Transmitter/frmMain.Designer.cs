
namespace Natom.AccessMonitor.Sync.Transmitter
{
    partial class frmMain
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripServiceStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusSyncDevices = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripAlert = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerToolStripAlert = new System.Windows.Forms.Timer(this.components);
            this.timerValidaActivacion = new System.Windows.Forms.Timer(this.components);
            this.timerRelojes = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.vincularDispositivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.verTodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonActivate = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripDropDownButton1,
            this.toolStripButtonActivate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(789, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripServiceStatus,
            this.toolStripStatusLabel2,
            this.toolStripStatusSyncDevices,
            this.toolStripAlert});
            this.statusStrip1.Location = new System.Drawing.Point(0, 460);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(789, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(57, 20);
            this.toolStripStatusLabel1.Text = "Estado:";
            // 
            // toolStripServiceStatus
            // 
            this.toolStripServiceStatus.BackColor = System.Drawing.Color.Transparent;
            this.toolStripServiceStatus.ForeColor = System.Drawing.Color.Gray;
            this.toolStripServiceStatus.Name = "toolStripServiceStatus";
            this.toolStripServiceStatus.Size = new System.Drawing.Size(96, 20);
            this.toolStripServiceStatus.Text = "Desconocido";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(136, 20);
            this.toolStripStatusLabel2.Text = "|  Sync dispositivos:";
            // 
            // toolStripStatusSyncDevices
            // 
            this.toolStripStatusSyncDevices.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusSyncDevices.ForeColor = System.Drawing.Color.Gray;
            this.toolStripStatusSyncDevices.Name = "toolStripStatusSyncDevices";
            this.toolStripStatusSyncDevices.Size = new System.Drawing.Size(96, 20);
            this.toolStripStatusSyncDevices.Text = "Desconocido";
            // 
            // toolStripAlert
            // 
            this.toolStripAlert.BackColor = System.Drawing.Color.Transparent;
            this.toolStripAlert.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripAlert.Name = "toolStripAlert";
            this.toolStripAlert.Size = new System.Drawing.Size(384, 20);
            this.toolStripAlert.Spring = true;
            this.toolStripAlert.Text = "toolStripAlert";
            this.toolStripAlert.Visible = false;
            // 
            // timerToolStripAlert
            // 
            this.timerToolStripAlert.Interval = 500;
            this.timerToolStripAlert.Tick += new System.EventHandler(this.timerToolStripAlert_Tick);
            // 
            // timerValidaActivacion
            // 
            this.timerValidaActivacion.Enabled = true;
            this.timerValidaActivacion.Interval = 5000;
            this.timerValidaActivacion.Tick += new System.EventHandler(this.timerValidaActivacion_Tick);
            // 
            // timerRelojes
            // 
            this.timerRelojes.Enabled = true;
            this.timerRelojes.Interval = 1000;
            this.timerRelojes.Tick += new System.EventHandler(this.timerRelojes_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Natom.AccessMonitor.Sync.Transmitter.Properties.Resources.IconMedium;
            this.pictureBox1.Location = new System.Drawing.Point(277, 210);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(373, 139);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(126, 24);
            this.toolStripButton1.Text = "Configuración";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vincularDispositivoToolStripMenuItem,
            this.toolStripSeparator2,
            this.verTodosToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(91, 24);
            this.toolStripDropDownButton1.Text = "Relojes";
            // 
            // vincularDispositivoToolStripMenuItem
            // 
            this.vincularDispositivoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.vincularDispositivoToolStripMenuItem.Image = global::Natom.AccessMonitor.Sync.Transmitter.Properties.Resources.Button_White_Add;
            this.vincularDispositivoToolStripMenuItem.Name = "vincularDispositivoToolStripMenuItem";
            this.vincularDispositivoToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.vincularDispositivoToolStripMenuItem.Text = "Vincular reloj";
            this.vincularDispositivoToolStripMenuItem.Click += new System.EventHandler(this.vincularDispositivoToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(221, 6);
            // 
            // verTodosToolStripMenuItem
            // 
            this.verTodosToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.verTodosToolStripMenuItem.Name = "verTodosToolStripMenuItem";
            this.verTodosToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.verTodosToolStripMenuItem.Text = "Ver todos";
            this.verTodosToolStripMenuItem.Click += new System.EventHandler(this.verTodosToolStripMenuItem_Click);
            // 
            // toolStripButtonActivate
            // 
            this.toolStripButtonActivate.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonActivate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.toolStripButtonActivate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonActivate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripButtonActivate.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonActivate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonActivate.Image")));
            this.toolStripButtonActivate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonActivate.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
            this.toolStripButtonActivate.Name = "toolStripButtonActivate";
            this.toolStripButtonActivate.Size = new System.Drawing.Size(166, 24);
            this.toolStripButtonActivate.Text = "CLICK PARA ACTIVAR";
            this.toolStripButtonActivate.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(789, 486);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sincronizador | BioAnviz +";
            this.Activated += new System.EventHandler(this.frmMain_Activated);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem vincularDispositivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem verTodosToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripServiceStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripAlert;
        private System.Windows.Forms.Timer timerToolStripAlert;
        private System.Windows.Forms.ToolStripButton toolStripButtonActivate;
        private System.Windows.Forms.Timer timerValidaActivacion;
        private System.Windows.Forms.Timer timerRelojes;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusSyncDevices;
    }
}

