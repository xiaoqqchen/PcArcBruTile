using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrutileArcGIS.Lib;

namespace BrutileArcGIS.forms
{
    public partial class AboutMe : Form
    {
        public AboutMe()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:"+linkLabel1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string _cacheDir = CacheSettings.GetCacheFolder();
            Process.Start(_cacheDir);
        }
    }
}
