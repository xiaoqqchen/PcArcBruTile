using System.Runtime.InteropServices;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddMapQuestOpenAerialMapLayerCommand")]
    public sealed class AddMapQuestOpenAerialMapLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapQuestOpenAerialMapLayerCommand()
            : base("BruTile", "&OpenAerialMap", "Add OpenAerialMap Layer", "MapQuest OpenAerialMap", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapQuest/OpenAerialMap.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }

    [ProgId("AddMapQuestOSMLayerCommand")]
    public sealed class AddMapQuestOSMLayerCommand : AddTmsLayerCommandBase
    {
        public AddMapQuestOSMLayerCommand()
            : base("BruTile", "&OSM", "Add OSM Layer", "MapQuest OSM", Resources.download, "http://dl.dropbox.com/u/9984329/ArcBruTile/Services/MapQuest/OSM.xml", EnumBruTileLayer.InvertedTMS)
        {
        }
    }
}
