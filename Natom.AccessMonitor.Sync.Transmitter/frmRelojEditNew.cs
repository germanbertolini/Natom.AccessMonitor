using Anviz.SDK;
using Natom.AccessMonitor.Sync.Transmitter.DeviceWrappers;
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
            RelojId = relojId;
            InitializeComponent();
        }

        private void frmRelojEditNew_Load(object sender, EventArgs e)
        {
            if (IsEditMode && !string.IsNullOrEmpty(RelojId))
            {
                RelojId = RelojId;
                var reloj = ConfigService.Devices.First(d => d.RelojId.Equals(RelojId));
                Guid.NewGuid().ToString("N");
                txtIP.Text = reloj.DeviceHost;
                txtPuerto.Text = reloj.DevicePort.ToString();
                txtUsuario.Text = reloj.User;
                txtClave.Text = reloj.Password;
                checkBox1.Checked = reloj.AuthenticateConnection;
                txtAlias.Text = reloj.Name;
                txtQuienInstala.Text = reloj.InstalledBy;
                cboTipo.SelectedIndex = 0;
            }
            else
            {
                txtPuerto.Text = "5010"; //POR DEFECTO
                cboTipo.SelectedIndex = 0;
            }

            txtUsuario.Enabled = checkBox1.Checked;
            txtClave.Enabled = checkBox1.Checked;
        }

        private async Task<DeviceConfig> ValidarConexionAsync()
        {
            var form = new frmCheckRelojes();
            DeviceConfig device = null;
            try
            {
                form.Show();
                form.SetStatus("INTENTANDO CONECTAR CON RELOJ...");

                this.Enabled = false;

                device = new DeviceConfig
                {
                    User = txtUsuario.Text,
                    Password = txtClave.Text,
                    AuthenticateConnection = checkBox1.Checked,
                    DeviceHost = txtIP.Text.Trim(),
                    DevicePort = Convert.ToUInt32(txtPuerto.Text),
                    DeviceBrand = "Anviz", //cboTipo.SelectedIndex == 0 ? "Anviz" : "Sarasa"
                };
                device.ConnectionWrapper = new AnvizDeviceWrapper(device); //cboTipo.SelectedIndex == 0 ? new AnvizConnectionWrapper() : new OtroWrapper
                await device.ConnectionWrapper.TryConnectionAsync(raiseException: true);
            }
            catch (Exception ex)
            {
                device = null;
                MessageBox.Show($"No se pudo establecer la conexión con el reloj. Verifique los parámetros de conexión ingresados.\n\n{(ex.InnerException?.InnerException??ex.InnerException??ex).Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                form.Close();
                this.Enabled = true;
            }

            return device;
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

            if (checkBox1.Checked)
            {
                if (string.IsNullOrEmpty(txtUsuario.Text))
                {
                    MessageBox.Show("Debes ingresar el usuario configurado en el Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtClave.Text))
                {
                    MessageBox.Show("Debes ingresar la clave del usuario configurado en el Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            if (!IsEditMode)
            {
                if (ConfigService.Devices != null && ConfigService.Devices.Any(d => d.DeviceHost.Equals(txtIP.Text.Trim())))
                {
                    MessageBox.Show("Ya se encuentra configurado un Reloj con misma IP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            if (!IsEditMode) //NUEVO
            {
                DeviceConfig device = await ValidarConexionAsync();
                if (device != null)
                {
                    var devices = ConfigService.Devices;
                    if (devices == null)
                        devices = new List<DeviceConfig>();


                    device.RelojId = Guid.NewGuid().ToString("N");
                    device.Name = txtAlias.Text;
                    device.AddedAt = DateTime.Now;
                    device.LastConfigUpdateAt = null;
                    device.InstalledBy = txtQuienInstala.Text;

                    devices.Add(device);

                    ConfigService.SaveDevices(devices);

                    this.Close();
                }
            }
            else //EDICION
            {
                var devices = ConfigService.Devices;
                var reloj = devices.First(d => d.RelojId.Equals(this.RelojId));
                reloj.ConnectionWrapper.Disconnect();

                DeviceConfig device = await ValidarConexionAsync();
                reloj.DeviceId = device.DeviceId;
                reloj.Name = txtAlias.Text;
                reloj.LastConfigUpdateAt = DateTime.Now;
                reloj.InstalledBy = txtQuienInstala.Text;
                reloj.DeviceBrand = device.DeviceBrand;
                reloj.DeviceDateTimeFormat = device.DeviceDateTimeFormat;
                reloj.DeviceFirmwareVersion = device.DeviceFirmwareVersion;
                reloj.DeviceHost = device.DeviceHost;
                reloj.DeviceModel = device.DeviceModel;
                reloj.DevicePort = device.DevicePort;
                reloj.DeviceSerialNumber = device.DeviceSerialNumber;
                device.ConnectionWrapper.Disconnect();

                ConfigService.SaveDevices(devices);

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

            if (checkBox1.Checked)
            {
                if (string.IsNullOrEmpty(txtUsuario.Text))
                {
                    MessageBox.Show("Debes ingresar el usuario configurado en el Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtClave.Text))
                {
                    MessageBox.Show("Debes ingresar la clave del usuario configurado en el Reloj.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (await ValidarConexionAsync() != null)
            {
                MessageBox.Show("¡Conexión exitosa!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtUsuario.Enabled = checkBox1.Checked;
            txtClave.Enabled = checkBox1.Checked;

            if (checkBox1.Checked && string.IsNullOrEmpty(txtUsuario.Text) && string.IsNullOrEmpty(txtClave.Text))
            {
                txtUsuario.Text = "admin";
                txtClave.Text = "12345";
            }
        }
    }
}
