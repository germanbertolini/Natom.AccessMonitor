
namespace Natom.AccessMonitor.Sync.Transmitter
{
    partial class frmRelojes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRelojes));
            this.btnNuevoReloj = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnEditarReloj = new System.Windows.Forms.Button();
            this.btnEliminarReloj = new System.Windows.Forms.Button();
            this.btnRefrescar = new System.Windows.Forms.Button();
            this.Nombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UltimaSincronizacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Estado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnResync = new System.Windows.Forms.Button();
            this.btnReiniciar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNuevoReloj
            // 
            this.btnNuevoReloj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnNuevoReloj.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevoReloj.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevoReloj.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnNuevoReloj.Location = new System.Drawing.Point(860, 501);
            this.btnNuevoReloj.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnNuevoReloj.Name = "btnNuevoReloj";
            this.btnNuevoReloj.Size = new System.Drawing.Size(109, 32);
            this.btnNuevoReloj.TabIndex = 99999;
            this.btnNuevoReloj.Text = "NUEVO";
            this.btnNuevoReloj.UseVisualStyleBackColor = false;
            this.btnNuevoReloj.Click += new System.EventHandler(this.btnNuevoReloj_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nombre,
            this.IP,
            this.UltimaSincronizacion,
            this.Estado});
            this.dataGridView1.Location = new System.Drawing.Point(13, 12);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(956, 413);
            this.dataGridView1.TabIndex = 100001;
            // 
            // btnEditarReloj
            // 
            this.btnEditarReloj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnEditarReloj.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReloj.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditarReloj.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnEditarReloj.Location = new System.Drawing.Point(745, 501);
            this.btnEditarReloj.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEditarReloj.Name = "btnEditarReloj";
            this.btnEditarReloj.Size = new System.Drawing.Size(109, 32);
            this.btnEditarReloj.TabIndex = 100002;
            this.btnEditarReloj.Text = "EDITAR";
            this.btnEditarReloj.UseVisualStyleBackColor = false;
            this.btnEditarReloj.Click += new System.EventHandler(this.btnEditarReloj_Click);
            // 
            // btnEliminarReloj
            // 
            this.btnEliminarReloj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnEliminarReloj.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminarReloj.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarReloj.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnEliminarReloj.Location = new System.Drawing.Point(630, 501);
            this.btnEliminarReloj.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEliminarReloj.Name = "btnEliminarReloj";
            this.btnEliminarReloj.Size = new System.Drawing.Size(109, 32);
            this.btnEliminarReloj.TabIndex = 100003;
            this.btnEliminarReloj.Text = "ELIMINAR";
            this.btnEliminarReloj.UseVisualStyleBackColor = false;
            this.btnEliminarReloj.Click += new System.EventHandler(this.btnEliminarReloj_Click);
            // 
            // btnRefrescar
            // 
            this.btnRefrescar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnRefrescar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefrescar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefrescar.ForeColor = System.Drawing.Color.White;
            this.btnRefrescar.Location = new System.Drawing.Point(12, 501);
            this.btnRefrescar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new System.Drawing.Size(136, 32);
            this.btnRefrescar.TabIndex = 100004;
            this.btnRefrescar.Text = "REFRESCAR";
            this.btnRefrescar.UseVisualStyleBackColor = false;
            this.btnRefrescar.Click += new System.EventHandler(this.btnRefrescar_Click);
            // 
            // Nombre
            // 
            this.Nombre.Frozen = true;
            this.Nombre.HeaderText = "Nombre";
            this.Nombre.MinimumWidth = 6;
            this.Nombre.Name = "Nombre";
            this.Nombre.Width = 180;
            // 
            // IP
            // 
            this.IP.Frozen = true;
            this.IP.HeaderText = "IP";
            this.IP.MinimumWidth = 6;
            this.IP.Name = "IP";
            this.IP.Width = 150;
            // 
            // UltimaSincronizacion
            // 
            this.UltimaSincronizacion.Frozen = true;
            this.UltimaSincronizacion.HeaderText = "Ultima sincronización";
            this.UltimaSincronizacion.MinimumWidth = 6;
            this.UltimaSincronizacion.Name = "UltimaSincronizacion";
            this.UltimaSincronizacion.Width = 200;
            // 
            // Estado
            // 
            this.Estado.Frozen = true;
            this.Estado.HeaderText = "Estado";
            this.Estado.MinimumWidth = 6;
            this.Estado.Name = "Estado";
            this.Estado.Width = 150;
            // 
            // btnResync
            // 
            this.btnResync.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnResync.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResync.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResync.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnResync.Location = new System.Drawing.Point(271, 501);
            this.btnResync.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnResync.Name = "btnResync";
            this.btnResync.Size = new System.Drawing.Size(147, 32);
            this.btnResync.TabIndex = 110005;
            this.btnResync.Text = "RESYNC TODO";
            this.btnResync.UseVisualStyleBackColor = false;
            this.btnResync.Click += new System.EventHandler(this.btnResync_Click);
            // 
            // btnReiniciar
            // 
            this.btnReiniciar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnReiniciar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReiniciar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReiniciar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnReiniciar.Location = new System.Drawing.Point(424, 501);
            this.btnReiniciar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnReiniciar.Name = "btnReiniciar";
            this.btnReiniciar.Size = new System.Drawing.Size(123, 32);
            this.btnReiniciar.TabIndex = 110006;
            this.btnReiniciar.Text = "REINICIAR";
            this.btnReiniciar.UseVisualStyleBackColor = false;
            this.btnReiniciar.Visible = false;
            this.btnReiniciar.Click += new System.EventHandler(this.btnReiniciar_Click);
            // 
            // frmRelojes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(984, 544);
            this.Controls.Add(this.btnReiniciar);
            this.Controls.Add(this.btnResync);
            this.Controls.Add(this.btnRefrescar);
            this.Controls.Add(this.btnEliminarReloj);
            this.Controls.Add(this.btnEditarReloj);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnNuevoReloj);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmRelojes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Relojes";
            this.Load += new System.EventHandler(this.frmRelojes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnNuevoReloj;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnEditarReloj;
        private System.Windows.Forms.Button btnEliminarReloj;
        private System.Windows.Forms.Button btnRefrescar;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn IP;
        private System.Windows.Forms.DataGridViewTextBoxColumn UltimaSincronizacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn Estado;
        private System.Windows.Forms.Button btnResync;
        private System.Windows.Forms.Button btnReiniciar;
    }
}