using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{

    public class Osm2MenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&OSM地图"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddOSMRoadCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddOSMBikeCommand";
                    itemDef.Group = false;
                    break;
                case 2:
                    itemDef.ID = "AddOSMTrafficCommand";
                    itemDef.Group = false;
                    break;
            }
        }

        public int ItemCount
        {
            get { return 3; }
        }

        public string Name
        {
            get { return "BruTile"; }
        }
    }
}
