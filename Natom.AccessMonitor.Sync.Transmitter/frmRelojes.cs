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
            var devices = ConfigService.Devices;
            if (devices != null)
            {
                var form = new frmCheckRelojes();
                form.Show();
                form.SetStatus("INTENTANDO CONECTAR CON RELOJ...");

                var connectedDevices = DevicesService.GetConnectedDevicesAsync(devices).GetAwaiter().GetResult();
                devices.ForEach(device =>
                {
                    if (connectedDevices.Any(d => d.RelojId.Equals(device.RelojId)))
                        RelojesStatus.Add(device.RelojId, "CONECTADO");
                    else
                        RelojesStatus.Add(device.RelojId, "DESCONECTADO");
                });

                form.Close();
            }
        }

        private void RefrescarListado()
        {
            var ultimasSyncs = DevicesService.GetLastDeviceSynchronization();

            dataGridView1.Rows.Clear();
            if (ConfigService.Devices != null && ConfigService.Devices.Count > 0)
            {
                ConfigService.Devices.ForEach(device =>
                {
                    dataGridView1.Rows.Add(new object[] {
                        device.Name,
                        device.DeviceHost,
                        ultimasSyncs.ContainsKey(device.RelojId) ? ultimasSyncs[device.RelojId].ToString("dd/MM/yyyy HH:mm:ss") + " hs" : "NUNCA",
                        RelojesStatus[device.RelojId]
                    });
                });
                btnRefrescar.Enabled = true;
                btnEliminarReloj.Enabled = true;
                btnEditarReloj.Enabled = true;
                btnResync.Enabled = true;
                btnReiniciar.Enabled = true;
            }
            else
            {
                btnRefrescar.Enabled = false;
                btnEliminarReloj.Enabled = false;
                btnEditarReloj.Enabled = false;
                btnResync.Enabled = false;
                btnReiniciar.Enabled = false;
            }
        }

        private void btnNuevoReloj_Click(object sender, EventArgs e)
        {
            var form = new frmRelojEditNew();
            form.Show();
            this.Close();
        }

        private void btnEditarReloj_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show($"Debes seleccionar primero el Reloj a Editar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var reloj = ConfigService.Devices[dataGridView1.SelectedRows[0].Index];
            var form = new frmRelojEditNew(editMode: true, reloj.RelojId);
            form.Show();
            this.Close();
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            ObtenerEstadoRelojes();
            RefrescarListado();
        }

        private void btnEliminarReloj_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show($"Debes seleccionar primero el Reloj a Eliminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var reloj = ConfigService.Devices[dataGridView1.SelectedRows[0].Index];
            if (MessageBox.Show($"¿Está seguro que desea ELIMINAR el Reloj '{reloj.Name}'?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var devices = ConfigService.Devices;
                devices = devices.Where(d => d.RelojId != reloj.RelojId).ToList();
                ConfigService.SaveDevices(devices);

                reloj.ConnectionWrapper.Disconnect();

                btnRefrescar_Click(sender, e);
            }
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show($"Debes seleccionar primero el Reloj a Reiniciar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var reloj = ConfigService.Devices[dataGridView1.SelectedRows[0].Index];
            var form = new frmCheckRelojes();

            if (MessageBox.Show($"¿Está seguro que desea APAGAR y VOLVER A ENCENDER el Reloj '{reloj.Name}'?", "APAGAR y VOLVER A ENCENDER", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    form.Show();
                    form.SetStatus("ENVIANDO SEÑAL AL RELOJ...");

                    reloj.ConnectionWrapper.SendRebootSignalAsync().Wait();

                    form.Close();

                    MessageBox.Show($"Señal enviada con éxito al equipo", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    form.Close();
                    MessageBox.Show($"Se produjo un error al intentar enviar la señal.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnResync_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show($"Debes seleccionar primero el Reloj a Resincronizar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var reloj = ConfigService.Devices[dataGridView1.SelectedRows[0].Index];
            var form = new frmCheckRelojes();
            if (MessageBox.Show($"Está solicitando RESINCRONIZAR todo el registro de Ingresos y Egresos del Reloj '{reloj.Name}'. Esto puede llegar a demorar unos minutos.\n¿Desea continuar?", "Resincronizar registro por completo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    form.Show();
                    form.SetStatus("RESINCRONIZANDO EQUIPO...");

                    await DevicesService.GetAndStoreRecordsFromDevicesAsync(new List<DeviceConfig> { reloj }, default, resyncAllRegisters: true);

                    form.Close();

                    MessageBox.Show($"¡Resincronización realizada con éxito! En los próximos minutos se verán reflejados los datos en la aplicación Web.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    form.Close();
                    MessageBox.Show($"Se produjo un error al intentar resincronizar el equipo.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
