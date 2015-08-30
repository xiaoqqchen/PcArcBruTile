using BruTile;
using BruTile.Web;

namespace BrutileArcGIS.Lib
{
    public class ConfigBing : IConfig
    {
        private readonly BingMapType _mapType = BingMapType.Roads;

        public ConfigBing(BingMapType mapType)
        {
            _mapType = mapType;
        }

        public ITileSource CreateTileSource()
        {
            var config = ConfigurationHelper.GetConfig();

            var bingToken=config.AppSettings.Settings["BingToken"].Value;
            var bingUrl = config.AppSettings.Settings["BingUrl"].Value;

            return new BingTileSource(
                bingUrl,bingToken,_mapType);
        }
    }
}
