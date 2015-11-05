using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddGoogleRoadLayerCommand")]
    public sealed class AddGoogleRoadLayerCommand : AddBruTileLayerCommandBase
    {
        public AddGoogleRoadLayerCommand()
            : base("BruTile", "&道路地图", "添加谷歌道路地图", "GoogleRoad", Resources.google, EnumBruTileLayer.GoogleMap)
        {
        }
    }

    [ProgId("AddGoogleArialCommand")]
    public sealed class AddGoogleArialCommand : AddBruTileLayerCommandBase
    {
        public AddGoogleArialCommand()
            : base("BruTile", "&影像地图", "添加谷歌影像地图", "GoogleArial", Resources.google, EnumBruTileLayer.GoogleHybrid)
        {
        }
    }
}
