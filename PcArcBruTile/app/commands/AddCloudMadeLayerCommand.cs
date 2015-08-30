using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddCloudMadeFreshLayerCommand")]
    public sealed class AddCloudMadeFreshLayerCommand : AddTmsLayerCommandBase
    {
        public AddCloudMadeFreshLayerCommand()
            : base("BruTile", "&Fresh", "Add Fresh Layer", "CloudMade Fresh", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/CloudMade/Fresh.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddCloudMadeMidnightCommanderLayerCommand")]
    public sealed class AddCloudMadeMidnightCommanderLayerCommand : AddTmsLayerCommandBase
    {
        public AddCloudMadeMidnightCommanderLayerCommand()
            : base("BruTile", "&Midnight Commander", "Add Midnight Commander Layer", "CloudMade Midnight Commander", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/CloudMade/Midnight Commander.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddCloudMadePaleDawnLayerCommand")]
    public sealed class AddCloudMadePaleDawnLayerCommand : AddTmsLayerCommandBase
    {
        public AddCloudMadePaleDawnLayerCommand()
            : base("BruTile", "&Pale Dawn", "Add Pale Dawn Layer", "CloudMade Pale Dawn", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/CloudMade/Pale Dawn.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }
}
