using System.ComponentModel;
using System.Configuration.Install;
using System;
using BrutileArcGIS.Lib;
using log4net;
using log4net.Config;
using System.IO;
using System.Diagnostics;


namespace BruTileArcGIS
{
    [RunInstaller(true)]
    public partial class ArcBruTileInstaller : Installer
    {
        private static readonly ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");

        public ArcBruTileInstaller()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.OnAfterInstall(stateSaver);

            var esriRegAsmFilename = Path.Combine(
                          Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                          "ArcGIS\\bin\\ESRIRegAsm.exe");
            var esriRegAsm = new Process {StartInfo = {FileName = esriRegAsmFilename}};
            var cmd = string.Format("\"{0}\" /p:Desktop", GetType().Assembly.Location);
            esriRegAsm.StartInfo.Arguments = cmd;
            Logger.Debug("Register for ArcGIS 10: " + cmd);

            esriRegAsm.Start();
            Logger.Debug("Register for ArcGIS 10 finished.");
            
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);
            XmlConfigurator.Configure(new FileInfo(GetType().Assembly.Location + ".config"));
            Logger.Debug("Uninstall ArcBruTile");
            // Try to clean up stuff
            try
            {
                var cacheFolder = CacheSettings.GetCacheFolder();
                Logger.Debug("Trying to delete tile folder: " + cacheFolder );
                Directory.Delete(cacheFolder,true);
                Logger.Debug("Tile directory is deleted");
            }
            catch (Exception ex)
            {
                Logger.Debug("Delete folder failed, error: " + ex);
            }

           var esriRegAsmFilename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                "ArcGIS\\bin\\ESRIRegAsm.exe");
            var esriRegAsm = new Process {StartInfo = {FileName = esriRegAsmFilename}};
            var cmd=string.Format("\"{0}\" /p:Desktop /u", GetType().Assembly.Location);
            esriRegAsm.StartInfo.Arguments = cmd;
            Logger.Debug("Unregister for ArcGIS 10: " + cmd);
            esriRegAsm.Start();
            Logger.Debug("Unregister for ArcGIS 10 finished.");
        }
    }
}
