using BruTile;
using BruTile.Predefined;
using BruTile.Web;

namespace BrutileArcGIS.Lib
{
    public class ConfigOsm : IConfig
    {
        private readonly OsmMapType _osmMapType;

        public ConfigOsm(OsmMapType maptype)
        {
            _osmMapType = maptype;
            
        }

        public ITileSource CreateTileSource()
        {
            ITileSource result = null;

            if (_osmMapType == OsmMapType.Default)
            {
                result = KnownTileSources.Create(KnownTileSource.OpenStreetMap);
            }

            return result;
        }
    }
}

