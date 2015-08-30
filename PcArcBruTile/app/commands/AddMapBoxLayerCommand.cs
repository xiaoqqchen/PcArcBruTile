using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddMapBoxSatelliteLayerCommand")]
    public sealed class AddMapBoxSatelliteLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxSatelliteLayerCommand()
            : base("BruTile", "&Satellite", "Add Satellite Layer", "MapBox Satellite", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/Satellite.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddMapBoxStreetsLayerCommand")]
    public sealed class AddMapBoxStreetsLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxStreetsLayerCommand()
            : base("BruTile", "&Streets", "Add Streets Layer", "MapBox Streets", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/Streets.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddMapBoxTerrainLayerCommand")]
    public sealed class AddMapBoxTerrainLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapBoxTerrainLayerCommand()
            : base("BruTile", "&Terrain", "Add Terrain Layer", "MapBox Terrain", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapBox/terrain.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }
}
