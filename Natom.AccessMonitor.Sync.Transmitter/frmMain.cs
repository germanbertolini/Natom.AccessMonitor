using Natom.AccessMonitor.Sync.Transmitter.Entities.DTO;
using Natom.AccessMonitor.Sync.Transmitter.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            ValidarPermisosEscritura();
            RefrescarStatusDispositivosAsync().Wait();
            ValidarConfig();

            //MOCK
            //ConfigService.Config.Devices = new List<Entities.DeviceConfig>();
            //ConfigService.Config.Devices.Add(new Entities.DeviceConfig()
            //{
            //    DeviceId = 14092,
            //    Name = "Porteria principal",
            //    AddedAt = DateTime.Now
            //});
            //ConfigService.Config.Devices.Add(new Entities.DeviceConfig()
            //{
            //    DeviceId = 15428,
            //    Name = "Porteria proveedores",
            //    AddedAt = DateTime.Now
            //});
        }

        private void ValidarPermisosEscritura()
        {
            try
            {
                Directory.CreateDirectory("Logs");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se requieren permisos de escritura en la carpeta de instalación. Asigne permisos y vuelva a intentarlo.", "BioAnviz+   |   Permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
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

            toolStripStatusSyncDevices.Visible = false;
            toolStripStatusLabel2.Visible = false;
        }

        public void OcultarToolStripAlerta()
        {
            timerToolStripAlert.Enabled = false;
            toolStripAlert.Visible = false;

            toolStripStatusSyncDevices.Visible = true;
            toolStripStatusLabel2.Visible = true;
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
            try
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
            catch (Exception ex)
            {
                LoggingService.LogException(ex);
            }
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            await ActivarAsync();
        }

        private async Task ActivarAsync()
        {
            var form = new frmActivating();
            try
            {
                if (string.IsNullOrEmpty(ConfigService.Config?.ServiceURL))
                {
                    MessageBox.Show("Debes configurar primero la URL del servicio desde la ventana 'Configuraciones'.", "BioAnviz+   |   Activación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                toolStripButtonActivate.Enabled = false;

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
                ConfigService.SaveConfig(ConfigService.Config);

                OcultarToolStripAlerta();
                toolStripButtonActivate.Visible = false;
                form.Close();

                MessageBox.Show("¡Sincronizador activado correctamente!", "BioAnviz+   |   Activación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                form.Close();
                ActivationService.Inactivate();
                LoggingService.LogException(ex);
                MessageBox.Show(ex.Message, "BioAnviz+   |   Activación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripButtonActivate.Enabled = true;
            }
        }

        private void timerValidaActivacion_Tick(object sender, EventArgs e)
        {
            ValidarActivacion();
        }

        DateTime _lastDevicesCheck = DateTime.MinValue;
        private async Task RefrescarStatusDispositivosAsync()
        {
            try
            {
                if (DevicesService.IsSynchronizing())
                {
                    toolStripStatusSyncDevices.Text = "Sincronizando...";
                    toolStripStatusSyncDevices.ForeColor = Color.Blue;
                }
                else
                {
                    if (ConfigService.Devices == null || ConfigService.Devices.Count == 0)
                    {
                        toolStripStatusSyncDevices.Text = "Sin dispositivos";
                        toolStripStatusSyncDevices.ForeColor = Color.DarkGray;
                    }
                    else
                    {
                        if ((DateTime.Now - _lastDevicesCheck).TotalSeconds >= 30)
                        {
                            _lastDevicesCheck = DateTime.Now;

                            toolStripStatusSyncDevices.Text = "Verificando...";
                            toolStripStatusSyncDevices.ForeColor = Color.DarkGray;

                            var devices = ConfigService.Devices;
                            var connected = await DevicesService.GetConnectedDevicesAsync(devices);
                            var disconnected = devices.Where(d => !connected.Any(c => d.RelojId.Equals(c.RelojId))).ToList();
                            if (disconnected.Count > 0)
                            {
                                toolStripStatusSyncDevices.Text = "¡Relojes SIN CONEXIÓN!";
                                toolStripStatusSyncDevices.ForeColor = Color.Red;
                            }
                            else
                            {
                                toolStripStatusSyncDevices.Text = "Normal";
                                toolStripStatusSyncDevices.ForeColor = Color.Green;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                toolStripStatusSyncDevices.Text = "ERROR";
                toolStripStatusSyncDevices.ForeColor = Color.Red;
                LoggingService.LogException(ex);
            }
        }

        DateTime _lastDevicesSync = DateTime.MinValue;
        DateTime _lastServerSync = DateTime.MinValue;
        private async void timerRelojes_Tick(object sender, EventArgs e)
        {
            if (ConfigService.Config == null)
                return;

            //ANTES QUE NADA: SETEAMOS ESTE TIMER PARA QUE CORRA CADA 10 SEGUNDOS
            timerRelojes.Interval = 10000;
                
            //TAREA 1: REFRESCAR EN TIEMPO REAL STATUS DE RELOJES
            await RefrescarStatusDispositivosAsync();

            //TAREA 2: OBTENER LA DATA DESDE LOS RELOJES (Asincrónica)
            if (ConfigService.Devices != null
                        && ConfigService.Devices.Count > 0
                        && (DateTime.Now - _lastDevicesSync).TotalMinutes >= ConfigService.Config.SyncFromDevicesMinutes)
            {
                _lastDevicesSync = DateTime.Now;
                _ = Task.Run(() =>
                                    {
                                        try
                                        {
                                            CancellationToken cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds((ConfigService.Config.SyncFromDevicesMinutes * 60) - 5)).Token;
                                            DevicesService.GetAndStoreRecordsFromDevicesAsync(ConfigService.Devices, cancellationToken).Wait();
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggingService.LogException(ex);
                                        }
                                    });
            }

            //TAREA 3: SINCRONIZAR LA DATA STOREADA AL SERVIDOR
            if (ConfigService.Config.ActivatedAt.HasValue && (DateTime.Now - _lastServerSync).TotalMinutes >= ConfigService.Config.SyncToServerMinutes)
            {
                if ((DateTime.Now - ConfigService.Config.ActivatedAt.Value).TotalMinutes >= 1) //ESPERAMOS 1 MINUTO DESPUES DE ACTIVAR EL SINCRONIZADOR PARA QUE SE ACOMODEN LOS DATOS EN EL SERVIDOR
                {
                    _lastServerSync = DateTime.Now;

                    toolStripStatusSyncDevices.Text = "Sincronizando...";
                    toolStripStatusSyncDevices.ForeColor = Color.Blue;

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            CancellationToken cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds((ConfigService.Config.SyncToServerMinutes * 60) - 5)).Token;
                            await DevicesService.SyncStoredDataToServerAsync(ConfigService.Devices, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("403")) //FORBIDDEN
                            {
                                ActivationService.Inactivate();
                                toolStripButtonActivate.Visible = false;
                                toolStripButtonActivate.Enabled = true;
                                ValidarActivacion();
                            }

                            LoggingService.LogException(ex);
                        }
                    });
                }
            }

        }

        private void vincularDispositivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmRelojEditNew();
            form.Show();
        }

        private void verTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmRelojes();
            form.Show();
        }
    }
}
