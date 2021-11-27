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
    public partial class frmRelojEditNew : Form
    {
        private bool IsEditMode { get; set; }
        private string RelojId { get; set; }

        public frmRelojEditNew(bool editMode = false, string relojId = null)
        {
            IsEditMode = editMode;
            if (IsEditMode && !string.IsNullOrEmpty(relojId))
            {
                RelojId = relojId;
                var reloj = ConfigService.Config.Devices.First(d => d.RelojId.Equals(relojId));
                Guid.NewGuid().ToString("N");
                txtIP.Text = reloj.DeviceHost;
                txtPuerto.Text = reloj.DevicePort.ToString();
                txtUsuario.Text = reloj.User;
                txtClave.Text = reloj.Password;
                checkBox1.Checked = reloj.AuthenticateConnection;
                txtAlias.Text = Name;
            }

            InitializeComponent();
        }

        private void frmRelojEditNew_Load(object sender, EventArgs e)
        {
            txtPuerto.Text = "5010"; //POR DEFECTO
        }

        private async Task<DeviceConfig> ValidarConexionAsync()
        {
            var form = new frmCheckRelojes();
            DeviceConfig toReturn = null;
            try
            {
                form.Show();
                form.SetStatus("INTENTANDO CONECTAR CON RELOJ...");

                this.Enabled = false;

                var manager = new AnvizManager();
                manager.ConnectionUser = txtUsuario.Text;
                manager.ConnectionPassword = txtClave.Text;
                manager.AuthenticateConnection = checkBox1.Checked; //true;

                var device = await manager.Connect(txtIP.Text, Convert.ToInt32(txtPuerto.Text));

                toReturn = new DeviceConfig
                {
                    DeviceId = device.DeviceId
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo establecer la conexión con el reloj. Verifique los parámetros de conexión ingresados.\n\n{(ex.InnerException?.InnerException??ex.InnerException??ex).Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                form.Close();
                this.Enabled = true;
            }

            return toReturn;
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

            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            if (ip.Matches(txtIP.Text).Count < 1)
            {
                MessageBox.Show("Debes ingresar una dirección IP del Reloj válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


            DeviceConfig device;
            if ((device = await ValidarConexionAsync()) != null)
            {
                if (!IsEditMode) //NUEVO
                {
                    var config = ConfigService.Config;
                    if (config.Devices == null)
                        config.Devices = new List<DeviceConfig>();

                    config.Devices.Add(new DeviceConfig
                    {
                        RelojId = Guid.NewGuid().ToString("N"),
                        DeviceId = device.DeviceId,
                        DeviceHost = txtIP.Text,
                        DevicePort = Convert.ToUInt32(txtPuerto.Text),
                        User = txtUsuario.Text,
                        Password = txtClave.Text,
                        AuthenticateConnection = checkBox1.Checked,
                        Name = txtAlias.Text,
                        AddedAt = DateTime.Now,
                        LastConfigUpdateAt = null
                    });

                    ConfigService.Save(config);
                }
                else //EDICION
                {
                    var config = ConfigService.Config;
                    var reloj = config.Devices.First(d => d.RelojId.Equals(this.RelojId));
                    reloj.DeviceId = device.DeviceId;
                    reloj.DeviceHost = txtIP.Text;
                    reloj.DevicePort = Convert.ToUInt32(txtPuerto.Text);
                    reloj.User = txtUsuario.Text;
                    reloj.Password = txtClave.Text;
                    reloj.AuthenticateConnection = checkBox1.Checked;
                    reloj.Name = txtAlias.Text;
                    reloj.LastConfigUpdateAt = DateTime.Now;

                    ConfigService.Save(config);
                }

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

            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            if (ip.Matches(txtIP.Text).Count < 1)
            {
                MessageBox.Show("Debes ingresar una dirección IP del Reloj válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
