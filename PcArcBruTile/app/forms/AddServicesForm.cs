using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using BruTileArcGIS;
using BrutileArcGIS.Lib;

namespace BrutileArcGIS.forms
{
    public partial class AddServicesForm : Form
    {
        private string _servicesDir;
        private bool _init=true;
        private string _file;

        public AddServicesForm()
        {
            InitializeComponent();
        }

        public TileMap SelectedService { get; set; }
        public TileMapService SelectedTileMapService { get; set; }

        private void AddPredefinedServicesForm_Load(object sender, EventArgs e)
        {
            InitForm();
        }

        private void InitForm()
        {
            // Read the files in de services directory
            _servicesDir = CacheSettings.GetServicesConfigDir();
            var di = new DirectoryInfo(_servicesDir);
            var files= di.GetFiles("*.xml").Select(f => Path.GetFileNameWithoutExtension(f.FullName)).ToList();

            lbProvider.DataSource = files;

            if (files.Count==0)
            {
                dgvServices.DataSource = null;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            _init = true;
            _file = (String)lbProvider.SelectedItem;
            var res = _servicesDir + Path.DirectorySeparatorChar + _file + ".xml";

            var xdoc=XDocument.Load(res);
            var el=xdoc.Element("Services");
            if (el != null)
            {
                var el1 = el.Element("TileMapService");
                if (el1 != null)
                    SelectedTileMapService = new TileMapService
                    {
                        Title = el1.Attribute("title").Value,
                        Version = el1.Attribute("version").Value,
                        Href = el1.Attribute("href").Value            };
            }

            btnRemoveProvider.Enabled = true;

            var tilemaps=TmsTileMapServiceParser.GetTileMaps(SelectedTileMapService.Href);
            tilemaps.Sort(TileMap.Compare);

            dgvServices.DataSource = tilemaps;
            dgvServices.Columns.Remove("Href");
            dgvServices.Columns.Remove("Profile");
            dgvServices.Columns.Remove("Srs");
            dgvServices.Columns.Remove("Type");
            dgvServices.Columns.Remove("OverwriteUrls");

            //resize columns
            dgvServices.Columns[0].Width=120;
            dgvServices.ClearSelection();
            _init = false;
            if (tilemaps.Count > 0)
            {
                btnOk.Enabled = false;
            }
        }

        private void dgvServices_SelectionChanged(object sender, EventArgs e)
        {
            if (!_init)
            {
                btnOk.Enabled = true;
                if (dgvServices.CurrentRow != null) SelectedService = (TileMap)dgvServices.CurrentRow.DataBoundItem;
                //SelectedService.
            }
        }

        private void btnAddProvider_Click(object sender, EventArgs e)
        {
            var addProviderForm = new AddProviderForm();
            var dr=addProviderForm.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                var name = addProviderForm.ProviderName;
                var url=addProviderForm.ProvidedServiceUrl;
                var enumBruTileLayer = addProviderForm.EnumBruTileLayer;

                // Now write an XML file to the services...
                WriteProviderXml(name,url,enumBruTileLayer);

                // now refresh...
                InitForm();
            }
        }

        private void WriteProviderXml(string name, string url, EnumBruTileLayer enumBruTileLayer)
        {
            var xml=@"<?xml version='1.0' encoding='utf-8' ?><Services>";
            xml+=String.Format(@"<TileMapService title='{0}' version='1.0.0' href='{1}' type='{2}'/>",name,url,enumBruTileLayer);
            xml += "</Services>";

            var xmlfile = _servicesDir + Path.DirectorySeparatorChar + name + ".xml";
            if (!File.Exists(xmlfile))
            {
                var tw = new StreamWriter(xmlfile);
                tw.WriteLine(xml);
                tw.Close();
            }
            else
            {
                MessageBox.Show(string.Format("Provider {0} does already exist.", name));
            }
        }

        private void btnRemoveProvider_Click(object sender, EventArgs e)
        {
            var selectedFile = (String)lbProvider.SelectedItem;
            var res = _servicesDir + Path.DirectorySeparatorChar + selectedFile + ".xml";

            if(File.Exists(res))
            {
                File.Delete(res);
                InitForm();
            }
            else
            {
                MessageBox.Show(string.Format("File {0} does not exist. Cannot remove provider.", selectedFile), @"Error");
            }
        }

    }
}
