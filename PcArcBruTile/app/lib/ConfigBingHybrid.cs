using BruTile;
using BruTile.Web;

namespace BrutileArcGIS.Lib
{
    public class ConfigBingHybrid : IConfig
    {
        public ITileSource CreateTileSource()
        {
            var config = ConfigurationHelper.GetConfig();

            var bingToken = config.AppSettings.Settings["BingToken"].Value;
            var bingUrl = config.AppSettings.Settings["BingUrl"].Value;
            const BingMapType mapType = BingMapType.Hybrid;

            return new BingTileSource(
                bingUrl, bingToken, mapType);
        }
    }
}
