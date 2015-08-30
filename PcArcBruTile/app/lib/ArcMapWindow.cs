using System;
using ESRI.ArcGIS.Framework;

namespace BrutileArcGIS.Lib
{
    public class ArcMapWindow : System.Windows.Forms.IWin32Window
    {
        private readonly IApplication _app;

        public ArcMapWindow(IApplication application)
        {
            _app = application;
        }

        public IntPtr Handle
        {
            get { return new IntPtr(_app.hWnd); }
        }
    }
}