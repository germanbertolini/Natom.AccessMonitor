using Anviz.SDK;
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
    public partial class frmRelojes : Form
    {
        private Dictionary<string, string> RelojesStatus;


        public frmRelojes()
        {
            InitializeComponent();
        }

        private void frmRelojes_Load(object sender, EventArgs e)
        {
            if (ConfigService.Config == null)
            {
                MessageBox.Show("No se encuentra la configuración!");
                this.Close();
                return;
            }

            ObtenerEstadoRelojes();
            RefrescarListado();
        }

        private void ObtenerEstadoRelojes()
        {
            RelojesStatus = new Dictionary<string, string>();
            if (ConfigService.Config.Devices != null)
            {
                var form = new frmCheckRelojes();
                form.Show();
                form.SetStatus("INTENTANDO CONECTAR CON RELOJ...");

                var tasks = new List<Task>();
                ConfigService.Config.Devices.ForEach(device =>
                {
                    tasks.Add(Task.Run(() =>
                    {
                        if (ValidarConexionAsync(device).GetAwaiter().GetResult())
                            RelojesStatus.Add(device.RelojId, "CONECTADO");
                        else
                            RelojesStatus.Add(device.RelojId, "DESCONECTADO");
                    }));
                });

                Task.WaitAll(tasks.ToArray());
                form.Close();
            }
        }

        private void RefrescarListado()
        {
            dataGridView1.Rows.Clear();
            if (ConfigService.Config.Devices != null)
            {
                ConfigService.Config.Devices.ForEach(device =>
                {
                    dataGridView1.Rows.Add(new object[] {
                        device.Name,
                        device.DeviceHost,
                        DateTime.MinValue.ToString("dd/mm/yyyy HH:mm:ss") + " hs",
                        RelojesStatus[device.RelojId]
                    });
                });
            }
        }

        private async Task<bool> ValidarConexionAsync(DeviceConfig device)
        {
            bool valido = false;

            try
            {
                

                var manager = new AnvizManager();
                manager.ConnectionUser = device.User;
                manager.ConnectionPassword = device.Password;
                manager.AuthenticateConnection = device.AuthenticateConnection;

                await manager.Connect(device.DeviceHost, (int)device.DevicePort);

                valido = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"No se pudo establecer la conexión con el reloj.\n\n{(ex.InnerException?.InnerException ?? ex.InnerException ?? ex).Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
            }

            return valido;
        }

        private void btnNuevoReloj_Click(object sender, EventArgs e)
        {
            var form = new frmRelojEditNew();
            form.Show();
            this.Close();
        }

        private void btnEditarReloj_Click(object sender, EventArgs e)
        {
            var form = new frmRelojEditNew(editMode: true, relojId: "asd123");
            form.Show();
            this.Close();
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            ObtenerEstadoRelojes();
            RefrescarListado();
        }
    }
}
