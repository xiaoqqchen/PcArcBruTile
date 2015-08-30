using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.forms;
using BrutileArcGIS.Lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.commands
{
    [ProgId("AddServicesCommand")]
    public sealed class AddServicesCommand : BaseCommand
    {
        private IApplication _application;

        public AddServicesCommand()
        {
            m_category = "BruTile";
            m_caption = "&Add TMS service...";
            m_message = "Add TMS service...";
            m_toolTip = m_caption;
            m_name = "ServicesCommand";
        }

        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                m_enabled = true;
            else
                m_enabled = false;

        }

        public override void OnClick()
        {
            try
            {
                var mxdoc = (IMxDocument)_application.Document;
                var map = mxdoc.FocusMap;

                var addServicesForm = new AddServicesForm();

                var result = addServicesForm.ShowDialog(new ArcMapWindow(_application));
                if (result == DialogResult.OK)
                {
                    var selectedService = addServicesForm.SelectedService;

                    // Fix the service labs.metacarta.com bug: it doubles the version :-(
                    selectedService.Href = selectedService.Href.Replace(@"1.0.0/1.0.0", @"1.0.0").Trim();


                    var layerType=EnumBruTileLayer.TMS;

                    // If the type is inverted TMS we have to do something special
                    if (selectedService.Type != null)
                    {
                        if (selectedService.Type == "InvertedTMS")
                        {
                            layerType = EnumBruTileLayer.InvertedTMS;
                        }
                    }

                    var brutileLayer = new BruTileLayer(_application, layerType, selectedService.Href, selectedService.OverwriteUrls)
                    {
                        Name = selectedService.Title,
                        Visible = true
                    };
                    map.AddLayer(brutileLayer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
