using System;
using System.Configuration;
using System.Reflection;

namespace BrutileArcGIS.Lib
{
    public class ConfigurationHelper
    {
        public static int GetTileTimeOut()
        {
            var config = GetConfig();
            var tileTimeOut = Int32.Parse(config.AppSettings.Settings["tileTimeout"].Value);
            return tileTimeOut;
        }

        public static Configuration GetConfig()
        {
            Configuration config;

            var configFileName = Assembly.GetExecutingAssembly().Location + ".config";
            var fileMap = new ExeConfigurationFileMap();
            try
            {
                // You may want to map to your own exe.config file here.
                fileMap.ExeConfigFilename = configFileName;
                config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }
            catch
            {
                var msg = String.Format("Can not find ({0})", configFileName);
                throw new ApplicationException(msg);
            }

            return config;
        }
    }
}
