using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class DutchMenuDef:BaseMenu
    {
        public DutchMenuDef()
        {
            m_barCaption = "ArcBruTile - &Dutch";
            AddItem(typeof (AboutBruTileCommand));
        }
    }
}
