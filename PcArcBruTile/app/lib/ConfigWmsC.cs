using BruTile;

namespace BrutileArcGIS.Lib
{
    public class ConfigWmsC: IConfig
    {
        private readonly ITileSource _tileSource;

        public ConfigWmsC(ITileSource tileSource)
        {
            _tileSource = tileSource;
        }

        public ITileSource CreateTileSource()
        {
            return _tileSource;
        }
    }
}
