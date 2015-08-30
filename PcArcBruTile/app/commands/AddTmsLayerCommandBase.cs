using System;
using System.Drawing;
using System.Windows.Forms;
using BrutileArcGIS.Lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.commands
{
    public class AddTmsLayerCommandBase : BaseCommand
    {
        private IApplication _application;
        private readonly string _url;
        private readonly EnumBruTileLayer _enumBruTileLayer;

        public AddTmsLayerCommandBase(string category, string caption, string message, string name, Bitmap bitmap, string url, EnumBruTileLayer enumBruTileLayer)
        {
            m_category = category;
            m_caption = caption;
            m_message = message;
            m_toolTip = message;
            m_name = name;
            m_bitmap = bitmap;
            _url = url;
            _enumBruTileLayer = enumBruTileLayer;
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
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
                var view = mxdoc.ActiveView;
                var brutileLayer = new BruTileLayer(_application, _enumBruTileLayer, _url, true)
                {
                    Name = m_name,
                    Visible = true
                };

                map.AddLayer(brutileLayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
