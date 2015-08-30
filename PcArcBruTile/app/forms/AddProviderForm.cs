using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using BrutileArcGIS.Lib;

namespace BrutileArcGIS.forms
{
    public partial class AddProviderForm : Form
    {
        private string _providedServiceUrl = string.Empty;
        private EnumBruTileLayer _enumBruTileLayer = EnumBruTileLayer.TMS;

        public string ProviderName { get; set; }

        public string ProvidedServiceUrl
        {
            get { return _providedServiceUrl; }
            set { _providedServiceUrl = value; }
        }

        public EnumBruTileLayer EnumBruTileLayer
        {
            get { return _enumBruTileLayer; }
            set { _enumBruTileLayer = value; }
        }

        public AddProviderForm()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            var config = ConfigurationHelper.GetConfig();
            var sampleProviders = config.AppSettings.Settings["sampleProviders"].Value;
            var providers = GetList(sampleProviders);
            lbProviders.DataSource = providers;
            lbProviders.DisplayMember = "Title";
        }

        protected List<TileMapService> GetList(string url)
        {
            var providers = new List<TileMapService>();

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14";
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = proxy;

            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            if (stream != null)
            {
                var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var tileMapService = new TileMapService();
                    if (line != null)
                    {
                        tileMapService.Title = line.Split(',')[0];
                        tileMapService.Href = line.Split(',')[1];
                        tileMapService.Version = line.Split(',')[2];
                    }
                    providers.Add(tileMapService);
                }
            }

            return providers;
        }

        private bool CheckUrl(string url)
        {
            var result = false;
            if (!UrlIsValid(url)) return false;
            try
            {
                TmsTileMapServiceParser.GetTileMaps(url);
                result = true;
            }
            catch (WebException)
            {
                errorProvider1.SetError(tbTmsUrl, "Could not download document. Please specify valid url");
            }
            catch (XmlException)
            {
                errorProvider1.SetError(tbTmsUrl, "Could not download XML document. Please specify valid url");
            }
            return result;

        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!CheckUrl(tbTmsUrl.Text)) return;
            ProviderName = tbName.Text;
            ProvidedServiceUrl = tbTmsUrl.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void tbName_Validating(object sender, CancelEventArgs e)
        {
            if (tbName.Text == String.Empty)
            {
                errorProvider1.SetError(tbName, "Please give name");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(tbName, "");
            }

        }

        private void tbTmsUrl_Validating(object sender, CancelEventArgs e)
        {
            if (tbTmsUrl.Text == String.Empty)
            {
                errorProvider1.SetError(tbTmsUrl, "Please give url");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(tbTmsUrl, "");
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        protected bool UrlIsValid(string url)
        {
            Uri result;
            return (Uri.TryCreate(url, UriKind.Absolute, out result));
        }


        private void lbProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tileMapService=(TileMapService)lbProviders.SelectedItem;
            tbName.Text = tileMapService.Title;
            tbTmsUrl.Text = tileMapService.Href;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi=new ProcessStartInfo {UseShellExecute = true, FileName = "http://arcbrutile.codeplex.com"};
            Process.Start(psi);
        }

    }
}
