using Natom.AccessMonitor.Sync.Transmitter.Entities;
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
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            txtServicioURL.Text = ConfigService.Config.ServiceURL;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var config = new TransmitterConfig
            {
                ServiceURL = txtServicioURL.Text
            };
            ConfigService.Save(config);
        }
    }
}
