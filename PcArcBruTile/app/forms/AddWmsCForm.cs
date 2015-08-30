using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BruTile;
using BruTile.Wmsc;

namespace BrutileArcGIS.forms
{
    public partial class AddWmsCForm : Form
    {
        private IList<ITileSource> _tileSources;

        public ITileSource SelectedTileSource;

        public AddWmsCForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
            // Complete sample urrel:
            // http://labs.metacarta.com/wms-c/tilecache.py?version=1.1.1&request=GetCapabilities&service=wms-c
            // Does not work yet: http://public-wms.kaartenbalie.nl/wms/nederland
            //string url = String.Format("{0}?version={1}&request=GetCapabilities&service=wms-c", tbWmsCUrl.Text, cbbVersion.SelectedItem);
            var url = tbWmsCUrl.Text;

            try
            {
                _tileSources = WmscTileSource.CreateFromWmscCapabilties(new Uri(url)).ToList();

                var names = _tileSources.Select(t => t.Schema.Name).ToList();

                lbServices.DataSource = names;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void lbServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbServices.SelectedItem != null)
            {
                var name = (String)lbServices.SelectedItem;
                foreach (var tileSource in _tileSources.Where(t => t.Schema.Name == name))
                {
                    SelectedTileSource = tileSource;
                }
                btnOk.Enabled = true;
            }
        }
    }

}