using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class TDTMenuDef:IMenuDef
    {
        public string Caption
        {
            get { return "&天地图地图"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                //case 0:
                //    itemDef.ID = "AddTDTRoadCommand";
                //    itemDef.Group = false;
                //    break;
                case 0:
                    itemDef.ID = "AddTDTArialCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddTDTLabelCommand";
                    itemDef.Group = false;
                    break;
            }
        }

        public int ItemCount
        {
            get { return 2; }
        }

        public string Name
        {
            get { return "BruTile"; }
        }
    }
}
