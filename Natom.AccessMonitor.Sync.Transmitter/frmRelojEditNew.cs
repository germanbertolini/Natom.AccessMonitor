using Natom.AccessMonitor.Sync.Transmitter.Entities;
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
    public partial class frmRelojEditNew : Form
    {
        public frmRelojEditNew()
        {
            InitializeComponent();
        }

        private void frmRelojEditNew_Load(object sender, EventArgs e)
        {
            txtPuerto.Text = "5010"; //POR DEFECTO
        }

        private async Task<bool> ValidarConexionAsync()
        {
            var form = new frmCheckRelojes();
            bool valido = false;
            try
            {
                form.Show();
                form.SetStatus("INTENTANDO CONECTAR CON RELOJ...");

                Uri baseUri = new Uri(txtServicioURL.Text);
                Uri healthCheckUri = new Uri(baseUri, "Echo/HealthCheckForTransmitter");
                var response = await NetworkService.DoHttpGetAsync(healthCheckUri.AbsoluteUri);
                if (!response.Success)
                    throw new Exception("Error del lado del Servidor.");

                valido = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo establecer la conexión con el reloj. Verifique los parámetros de conexión ingresados.\n\n{(ex.InnerException?.InnerException??ex.InnerException??ex).Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                form.Close();
            }

            return valido;
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAlias.Text))
            {
                MessageBox.Show("Debes definirle un Alias a este Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtQuienInstala.Text))
            {
                MessageBox.Show("Debes indicar tu nombre en 'Quién instala'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtIP.Text))
            {
                MessageBox.Show("Debes ingresar la dirección IP del Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtPuerto.Text))
            {
                MessageBox.Show("Debes ingresar el puerto del Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int port;
            if (!int.TryParse(txtPuerto.Text, out port))
            {
                MessageBox.Show("Debes ingresar un valor numérico para el puerto del reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtUsuario.Text))
            {
                MessageBox.Show("Debes ingresar el usuario configurado en el Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtPuerto.Text))
            {
                MessageBox.Show("Debes ingresar la clave del usuario configurado en el Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (await ValidarConexionAsync())
            {
                var config = ConfigService.Config;
                if (config == null)
                    config = new TransmitterConfig();

                config.ServiceURL = txtServicioURL.Text;
                config.InstallationAlias = txtAlias.Text;
                config.InstallerName = txtQuienInstala.Text;
                config.ClientName = txtIP.Text;
                config.ClientCUIT = txtPuerto.Text;
                config.SyncFromDevicesMinutes = port;
                config.SyncToServerMinutes = syncToServerMinutes;

                ConfigService.Save(config);

                this.Close();
            }
        }

        private async void btnProbarConexion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIP.Text))
            {
                MessageBox.Show("Debes ingresar la dirección IP del Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtPuerto.Text))
            {
                MessageBox.Show("Debes ingresar el puerto del Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int port;
            if (!int.TryParse(txtPuerto.Text, out port))
            {
                MessageBox.Show("Debes ingresar un valor numérico para el puerto del reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtUsuario.Text))
            {
                MessageBox.Show("Debes ingresar el usuario configurado en el Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtPuerto.Text))
            {
                MessageBox.Show("Debes ingresar la clave del usuario configurado en el Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await ValidarConexionAsync();
        }
    }
}
