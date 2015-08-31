using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class GaodeMenuDef: IMenuDef
    {
        public string Caption
        {
            get { return "&高德地图"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddGaodeRoadLayerCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddGaodeArialCommand";
                    itemDef.Group = false;
                    break;
                case 2:
                    itemDef.ID = "AddGaodeHybridCommand";
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
