﻿using Natom.AccessMonitor.Sync.Transmitter.Entities;
using Natom.AccessMonitor.Sync.Transmitter.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Natom.AccessMonitor.Sync.Transmitter
{
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            txtServicioURL.Text = ConfigService.Config?.ServiceURL;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServicioURL.Text))
            {
                MessageBox.Show("Debes ingresar una URL de servicio.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtInstalacionAlias.Text))
            {
                MessageBox.Show("Debes definirle un Alias a esta instalación.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtQuienInstala.Text))
            {
                MessageBox.Show("Debes indicar tu nombre en 'Quién instala'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtRazonSocial.Text))
            {
                MessageBox.Show("Debes ingresar Razon social / Nombre fantasía del cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtCUIT.Text))
            {
                MessageBox.Show("Debes ingresar CUIT del cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            if (await ValidarConexionAsync())
            {
                var config = ConfigService.Config;
                if (config == null)
                    config = new TransmitterConfig();

                config.ServiceURL = txtServicioURL.Text;
                config.InstallationAlias = txtInstalacionAlias.Text;
                config.InstallerName = txtQuienInstala.Text;
                config.ClientName = txtRazonSocial.Text;
                config.ClientCUIT = txtCUIT.Text;

                ConfigService.Save(config);

                this.Close();
            }
        }

        private async Task<bool> ValidarConexionAsync()
        {
            var form = new frmCheckConnection();
            bool valido = false;
            try
            {
                form.Show();
                form.SetStatus("INTENTANDO CONECTAR CON SERVIDOR...");

                Uri baseUri = new Uri(txtServicioURL.Text);
                Uri healthCheckUri = new Uri(baseUri, "Echo/HealthCheckForTransmitter");
                var response = await NetworkService.DoHttpGetAsync(healthCheckUri.AbsoluteUri);
                if (!response.Success)
                    throw new Exception("Error del lado del Servidor.");

                valido = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo establecer la conexión con el servidor. Verifique la URL ingresada.\n\n{(ex.InnerException?.InnerException??ex.InnerException??ex).Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                form.Close();
            }

            return valido;
        }
    }
}
