using Natom.AccessMonitor.Sync.Transmitter.Entities.DTO;
using Natom.AccessMonitor.Sync.Transmitter.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Natom.AccessMonitor.Sync.Transmitter
{
    public partial class frmMain : Form
    {
        private DateTime estadoUltimaActualizado = DateTime.MinValue;
        private Color toolStripAlertColorA = Color.FromArgb(3, 127, 129);
        private Color toolStripAlertColorB = Color.FromArgb(17, 87, 157);

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            toolStripButtonActivate.Visible = false;
            ValidarConfig();
        }

        private void ValidarConfig()
        {
            if (ConfigService.Config == null || string.IsNullOrEmpty(ConfigService.Config.ServiceURL))
            {
                MessageBox.Show("¡BioAnviz+ te da la bienvenida!\nEstás iniciando por primera vez la aplicación. A continuación te solicitaremos configurar este sincronizador.", "BioAnviz+   |   Nueva instalación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var form = new frmConfig();
                form.ShowDialog();
            }            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var form = new frmConfig();
            form.ShowDialog();
        }

        private async void frmMain_Activated(object sender, EventArgs e)
        {
            if (ConfigService.Config != null && (DateTime.Now - estadoUltimaActualizado).TotalMilliseconds >= 5)
            {
                toolStripServiceStatus.ForeColor = Color.Gray;
                toolStripServiceStatus.Text = "Desconocido";

                if (await ValidarConexionAsync())
                {
                    toolStripServiceStatus.ForeColor = Color.Green;
                    toolStripServiceStatus.Text = "Online";
                    ValidarActivacion();
                }
                else
                {
                    toolStripServiceStatus.ForeColor = Color.Red;
                    toolStripServiceStatus.Text = "Offline";
                }

                estadoUltimaActualizado = DateTime.Now;
            }
        }

        private async Task<bool> ValidarConexionAsync()
        {
            if (ConfigService.Config == null)
                return false;

            bool valido = false;
            try
            {
                Uri baseUri = new Uri(ConfigService.Config.ServiceURL);
                Uri healthCheckUri = new Uri(baseUri, "Echo/HealthCheckForTransmitter");
                var response = await NetworkService.DoHttpGetAsync(healthCheckUri.AbsoluteUri);
                if (!response.Success)
                    throw new Exception("Error del lado del Servidor.");

                valido = true;
            }
            catch (Exception ex)
            {
                
            }

            return valido;
        }

        public void MostrarToolStripAlerta(string texto)
        {
            timerToolStripAlert.Enabled = true;
            toolStripAlert.Visible = true;
            toolStripAlert.BackColor = toolStripAlertColorA;
            toolStripAlert.Text = texto.ToUpper();
        }

        public void OcultarToolStripAlerta()
        {
            timerToolStripAlert.Enabled = false;
            toolStripAlert.Visible = false;
        }

        private void timerToolStripAlert_Tick(object sender, EventArgs e)
        {
            if (toolStripAlert.BackColor == toolStripAlertColorA)
                toolStripAlert.BackColor = toolStripAlertColorB;
            else
                toolStripAlert.BackColor = toolStripAlertColorA;
        }

        private void ValidarActivacion()
        {
            timerValidaActivacion.Enabled = true;
            if (ConfigService.Config?.ActivatedAt == null && !toolStripButtonActivate.Visible)
            {
                MostrarToolStripAlerta("SINCRONIZADOR PENDIENTE DE ACTIVACIÓN");
                toolStripButtonActivate.Visible = true;
            }
            else if (ConfigService.Config?.ActivatedAt != null && toolStripButtonActivate.Visible)
            {
                toolStripButtonActivate.Visible = false;
            }
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            await ActivarAsync();
        }

        private async Task ActivarAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigService.Config?.ServiceURL))
                {
                    MessageBox.Show("Debes configurar primero la URL del servicio desde la ventana 'Configuraciones'.", "BioAnviz+   |   Activación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                toolStripButtonActivate.Enabled = false;

                var form = new frmActivating();
                form.Show();
                form.SetStatus("CONECTANDO CON SERVIDOR...");
                var accessToken = await ActivationService.StartAsync();
                ConfigService.Config.AccessToken = accessToken;
                
                form.Focus();
                form.SetStatus("¡NO CIERRE EL PROGRAMA!               ESPERANDO APROBACIÓN DEL ADMIN");
                await ActivationService.WaitForAprobacionAsync();
                
                form.Focus();
                form.SetStatus("FINALIZANDO ACTIVACIÓN...");
                ActivationService.Activate();
                var definitiveAccessToken = await ActivationService.ConfirmActivationAsync();
                ConfigService.Config.AccessToken = definitiveAccessToken;
                ConfigService.Save(ConfigService.Config);

                OcultarToolStripAlerta();
                toolStripButtonActivate.Visible = false;
                form.Close();

                MessageBox.Show("¡Sincronizador activado correctamente!", "BioAnviz+   |   Activación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "BioAnviz+   |   Activación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripButtonActivate.Enabled = true;
            }
        }

        private void timerValidaActivacion_Tick(object sender, EventArgs e)
        {
            ValidarActivacion();
        }
    }
}
