using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrutileArcGIS.forms;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.commands
{
    [ProgId("AddWmscCommand")]
    public sealed class AddWmsCLayerCommand : BaseCommand
    {
        private IMap _map;
        private IApplication _application;

        public AddWmsCLayerCommand()
        {
            m_category = "BruTile";
            m_caption = "Add &WMS-C service...";
            m_message = "Add WMS-C Layer";
            m_toolTip = m_message;
            m_name = "AddWmsCLayer";
            m_bitmap = Resources.WMS_icon;
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

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        public override void OnClick()
        {
            try
            {
                var mxdoc = (IMxDocument)_application.Document;
                
                _map = mxdoc.FocusMap;

                var addWmsCForm = new AddWmsCForm();
                var result = addWmsCForm.ShowDialog(new ArcMapWindow(_application));

                if (result == DialogResult.OK)
                {
                    var tileSource = addWmsCForm.SelectedTileSource;
                    
                    IConfig configWmsC = new ConfigWmsC(tileSource);
                    var brutileLayer = new BruTileLayer(_application,configWmsC)
                    {
                        Name = configWmsC.CreateTileSource().Schema.Name,
                        Visible = true
                    };
                    _map.AddLayer(brutileLayer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
