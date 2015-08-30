using System.Collections.Generic;
using System.Xml;
using BrutileArcGIS.lib;

namespace BrutileArcGIS.Lib
{
    public class TmsTileMapServiceParser
    {
        public static List<TileMap> GetTileMaps(string url)
        {
            var doc = Requester.GetXmlDocument(url);
            var nodes=doc.GetElementsByTagName("TileMap");
            
            var tilemaps=new List<TileMap>();
            foreach (XmlNode node in nodes)
            {
                var tileMap=new TileMap();
                if (node.Attributes != null)
                {
                    tileMap.Href = node.Attributes["href"].Value;
                    tileMap.Srs = node.Attributes["srs"].Value;
                    tileMap.Profile = node.Attributes["profile"].Value;
                    tileMap.Title= node.Attributes["title"].Value;
                    tileMap.Title = node.Attributes["title"].Value;
                    if (node.Attributes["type"] != null)
                    {
                        tileMap.Type = node.Attributes["type"].Value;
                    }
                    if (node.Attributes["overwriteurls"] != null)
                    {
                        tileMap.OverwriteUrls = bool.Parse(node.Attributes["overwriteurls"].Value);
                    }
                }


                tilemaps.Add(tileMap);
            }

            return tilemaps;
        }
    }
}
