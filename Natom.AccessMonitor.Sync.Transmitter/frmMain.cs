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
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ValidarConfig();
        }

        private void ValidarConfig()
        {
            if (ConfigService.Config == null || string.IsNullOrEmpty(ConfigService.Config.ServiceURL))
            {
                var form = new frmConfig();
                form.ShowDialog();
            }            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var form = new frmConfig();
            form.Show();
        }
    }
}
