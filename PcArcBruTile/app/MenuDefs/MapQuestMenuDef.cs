using ESRI.ArcGIS.SystemUI;

namespace BrutileArcGIS.MenuDefs
{
    public class MapQuestMenuDef : IMenuDef
    {
        public string Caption
        {
            get { return "&MapQuest"; }
        }
        public void GetItemInfo(int pos, IItemDef itemDef)
        {
            switch (pos)
            {
                case 0:
                    itemDef.ID = "AddMapQuestOpenAerialMapLayerCommand";
                    itemDef.Group = false;
                    break;
                case 1:
                    itemDef.ID = "AddMapQuestOSMLayerCommand";
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