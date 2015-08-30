using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public  class PCMenuDef:IMenuDef
    {
        public string Caption
        {
            get { return "&关于"; }
        }

        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "TileCacheCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AboutMeCommand";
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
