using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class StamenMenuDef : BaseMenu
    {
        public StamenMenuDef()
        {
            m_barCaption = "&Stamen";
            AddItem(typeof(AddStamenWaterColorLayerCommand));
            AddItem(typeof(AddStamenTerrainLayerCommand));
            AddItem(typeof(AddStamenTonerLayerCommand));
        }
    }
}

