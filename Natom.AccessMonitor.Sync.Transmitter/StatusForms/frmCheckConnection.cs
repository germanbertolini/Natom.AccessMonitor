﻿using System;
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
    public partial class frmCheckConnection : Form
    {
        public frmCheckConnection()
        {
            InitializeComponent();
        }

        private void frmSync_Load(object sender, EventArgs e)
        {

        }

        public void SetStatus(string text)
        {
            txtStatus.Text = text.ToUpper();
        }
    }
}
