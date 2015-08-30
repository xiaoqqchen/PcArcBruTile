using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BrutileArcGIS.Lib;
using BrutileArcGIS.Properties;

namespace BrutileArcGIS.commands
{
    [ProgId("AddGaodeRoadLayerCommand")]
    public sealed class AddGaodeRoadLayerCommand : AddBruTileLayerCommandBase
    {
        public AddGaodeRoadLayerCommand()
            : base("BruTile", "&道路地图", "添加高德道路地图", "GaodeRoad", Resources.gaode,  EnumBruTileLayer.GaodeRoad)
        {
        }
    }

    [ProgId("AddGaodeArialCommand")]
    public sealed class AddGaodeArialCommand : AddBruTileLayerCommandBase
    {
        public AddGaodeArialCommand()
            : base("BruTile", "&影像地图", "添加高德影像地图", "GaodeArial", Resources.gaode, EnumBruTileLayer.GaodeArial)
        {
        }
    }

    [ProgId("AddGaodeHybridCommand")]
    public sealed class AddGaodeHybridCommand : AddBruTileLayerCommandBase
    {
        public AddGaodeHybridCommand()
            : base("BruTile", "&路网地图", "添加高德路网地图", "GaodeHybrid", Resources.gaode, EnumBruTileLayer.GaodeHybrid)
        {
        }
    }

    [ProgId("AddGaodeTrafficCommand")]
    public sealed class AddGaodeTrafficCommand : AddBruTileLayerCommandBase
    {
        public AddGaodeTrafficCommand()
            : base("BruTile", "&交通流量地图", "添加高德交通流量地图", "GaodeTraffic", Resources.gaode, EnumBruTileLayer.GaodeTraffic)
        {
        }
    }

    [ProgId("AddTDTLabelCommand")]
    public sealed class AddTDTLabelCommand : AddBruTileLayerCommandBase
    {
        public AddTDTLabelCommand()
            : base("BruTile", "&注记地图", "添加天地图注记地图", "TDTLabel", Resources.gaode, EnumBruTileLayer.GaodeLabel)
        {
        }
    }


    [ProgId("AddOSMRoadCommand")]
    public sealed class AddOSMRoadCommand : AddBruTileLayerCommandBase
    {
        public AddOSMRoadCommand()
            : base("BruTile", "&道路地图", "添加OSM道路地图", "OSMRoad", Resources.TDT, EnumBruTileLayer.OSMRoad)
        {
        }
    }

    [ProgId("AddOSMBikeCommand")]
    public sealed class AddOSMBikeCommand : AddBruTileLayerCommandBase
    {
        public AddOSMBikeCommand()
            : base("BruTile", "&自行车地图", "添加OSM自行车地图", "OSMBike", Resources.osm_logo, EnumBruTileLayer.OSMBike)
        {
        }
    }

    [ProgId("AddOSMTrafficCommand")]
    public sealed class AddOSMTrafficCommand : AddBruTileLayerCommandBase
    {
        public AddOSMTrafficCommand()
            : base("BruTile", "&交通地图", "添加OSM交通流量地图", "OSMTraffic", Resources.osm_logo, EnumBruTileLayer.OsmTraffic)
        {
        }
    }
}
