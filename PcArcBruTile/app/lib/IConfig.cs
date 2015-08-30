using BruTile;

namespace BrutileArcGIS.Lib
{
    public interface IConfig
    {
        ITileSource CreateTileSource();
    }
}
