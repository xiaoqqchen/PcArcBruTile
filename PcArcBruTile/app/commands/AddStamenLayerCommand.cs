using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddStamenWaterColorLayerCommand")]
    public sealed class AddStamenWaterColorLayerCommand : AddTmsLayerCommandBase
    {
        public AddStamenWaterColorLayerCommand()
            : base("BruTile", "&Watercolor", "Add Watercolor Layer", "Stamen WaterColor", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/Stamen/watercolor.xml",EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddStamenTonerLayerCommand")]
    public sealed class AddStamenTonerLayerCommand : AddTmsLayerCommandBase
    {
        public AddStamenTonerLayerCommand()
            : base("BruTile", "&Toner", "Add Toner Layer", "Stamen Toner", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/Stamen/toner.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddStamenTerrainLayerCommand")]
    public sealed class AddStamenTerrainLayerCommand : AddTmsLayerCommandBase
    {
        public AddStamenTerrainLayerCommand()
            : base("BruTile", "&Terrain", "Add Terrain Layer", "Stamen Terrain", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/Stamen/terrain.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

}
