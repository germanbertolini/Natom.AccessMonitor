
namespace Natom.AccessMonitor.Sync.Transmitter
{
    partial class frmConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfig));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtServicioURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtCUIT = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRazonSocial = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtQuienInstala = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtInstalacionAlias = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtMinutosRelojes = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMinutosSincronizacion = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtServicioURL);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(13, 14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(775, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SERVICIO";
            // 
            // txtServicioURL
            // 
            this.txtServicioURL.Location = new System.Drawing.Point(143, 26);
            this.txtServicioURL.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtServicioURL.Name = "txtServicioURL";
            this.txtServicioURL.Size = new System.Drawing.Size(609, 22);
            this.txtServicioURL.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL del servicio:";
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(681, 430);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(109, 32);
            this.btnSave.TabIndex = 99999;
            this.btnSave.Text = "GUARDAR";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtCUIT);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtRazonSocial);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(15, 225);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(775, 107);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CLIENTE";
            // 
            // txtCUIT
            // 
            this.txtCUIT.Location = new System.Drawing.Point(241, 65);
            this.txtCUIT.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCUIT.Name = "txtCUIT";
            this.txtCUIT.Size = new System.Drawing.Size(511, 22);
            this.txtCUIT.TabIndex = 50;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(189, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "CUIT:";
            // 
            // txtRazonSocial
            // 
            this.txtRazonSocial.Location = new System.Drawing.Point(241, 26);
            this.txtRazonSocial.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.Size = new System.Drawing.Size(511, 22);
            this.txtRazonSocial.TabIndex = 40;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Razon social / Nombre fantasía:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtQuienInstala);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtInstalacionAlias);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(15, 102);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(775, 105);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ESTA INSTALACIÓN DE SINCRONIZADOR";
            // 
            // txtQuienInstala
            // 
            this.txtQuienInstala.Location = new System.Drawing.Point(241, 63);
            this.txtQuienInstala.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtQuienInstala.Name = "txtQuienInstala";
            this.txtQuienInstala.Size = new System.Drawing.Size(511, 22);
            this.txtQuienInstala.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(166, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Nombre de quién instala:";
            // 
            // txtInstalacionAlias
            // 
            this.txtInstalacionAlias.Location = new System.Drawing.Point(241, 26);
            this.txtInstalacionAlias.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtInstalacionAlias.Name = "txtInstalacionAlias";
            this.txtInstalacionAlias.Size = new System.Drawing.Size(511, 22);
            this.txtInstalacionAlias.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Alias / Nombre (referencia):";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtMinutosSincronizacion);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.txtMinutosRelojes);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.ForeColor = System.Drawing.Color.White;
            this.groupBox4.Location = new System.Drawing.Point(13, 348);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Size = new System.Drawing.Size(775, 71);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "SINCRONIZACIÓN";
            // 
            // txtMinutosRelojes
            // 
            this.txtMinutosRelojes.Location = new System.Drawing.Point(149, 30);
            this.txtMinutosRelojes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMinutosRelojes.Name = "txtMinutosRelojes";
            this.txtMinutosRelojes.Size = new System.Drawing.Size(35, 22);
            this.txtMinutosRelojes.TabIndex = 60;
            this.txtMinutosRelojes.Text = "1";
            this.txtMinutosRelojes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(227, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Leer relojes cada:              minutos";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // txtMinutosSincronizacion
            // 
            this.txtMinutosSincronizacion.Location = new System.Drawing.Point(518, 30);
            this.txtMinutosSincronizacion.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMinutosSincronizacion.Name = "txtMinutosSincronizacion";
            this.txtMinutosSincronizacion.Size = new System.Drawing.Size(35, 22);
            this.txtMinutosSincronizacion.TabIndex = 65;
            this.txtMinutosSincronizacion.Text = "5";
            this.txtMinutosSincronizacion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(356, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(262, 17);
            this.label7.TabIndex = 101;
            this.label7.Text = "Sincronizar datos cada:              minutos";
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(800, 473);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtServicioURL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtCUIT;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRazonSocial;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtInstalacionAlias;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtQuienInstala;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtMinutosRelojes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMinutosSincronizacion;
        private System.Windows.Forms.Label label7;
    }
}