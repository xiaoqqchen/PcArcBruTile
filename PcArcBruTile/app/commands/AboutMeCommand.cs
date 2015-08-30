using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BrutileArcGIS.Lib;
using BrutileArcGIS.forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.commands
{
    [ProgId("TileCacheCommand")]
    public sealed class TileCacheCommand : BaseCommand
    {
        private IApplication _application;

        public TileCacheCommand()
        {
            m_category = "BruTile";
            m_caption = "&下载切片";
            m_message = "下载切片";
            m_toolTip = m_caption;
            m_name = "TileCacheCommand";
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
            var mxdoc = (IMxDocument)_application.Document;
            IActiveView view = mxdoc.ActiveView;
            var tilecache = new TileCache(view);
            tilecache.ShowDialog(new ArcMapWindow(_application));
        }
    }

        [ProgId("AboutMeCommand")]
    public sealed class AboutMeCommand : BaseCommand
    {
  private IApplication _application;

  public AboutMeCommand()
        {
            m_category = "BruTile";
            m_caption = "&关于";
            m_message = "关于";
            m_toolTip = m_caption;
            m_name = "AboutMeCommand";
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
            var bruTileAboutBox = new AboutMe();
            bruTileAboutBox.ShowDialog(new ArcMapWindow(_application));
        }
    }
}
