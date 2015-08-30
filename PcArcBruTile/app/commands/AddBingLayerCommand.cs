using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddBingAerialLayerCommand")]
    public sealed class AddBingAerialLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingAerialLayerCommand()
            : base("BruTile", "&Aerial", "Add Bing Aerial Layer", "Bing Aerial", Resources.bing, EnumBruTileLayer.BingAerial)
        {
        }
    }

    [ProgId("AddBingHybridLayerCommand")]
    public sealed class AddBingHybridLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingHybridLayerCommand()
            : base("BruTile", "&Hybrid", "Add Bing Hybrid Layer", "Bing Hybrid", Resources.bing, EnumBruTileLayer.BingHybrid)
        {
        }
    }

    [ProgId("AddBingRoadLayerCommand")]
    public sealed class AddBingRoadLayerCommand : AddBruTileLayerCommandBase
    {
        public AddBingRoadLayerCommand()
            : base("BruTile", "&Roads", "Add Bing Road Layer", "Bing Road", Resources.bing, EnumBruTileLayer.BingRoad)
        {
        }
    }



}
