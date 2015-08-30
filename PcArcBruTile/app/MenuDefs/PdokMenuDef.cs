using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;
namespace BrutileArcGIS.MenuDefs
{
    public class PdokMenuDef:BaseMenu
    {
        public PdokMenuDef()
        {
            m_barCaption = "&PDOK";
            AddItem(typeof(AddPdokBrtAchtergrondLayerCommand));
            AddItem(typeof(AddPdokBrpGewaspercelenLayerCommand));
        }
    }
}
