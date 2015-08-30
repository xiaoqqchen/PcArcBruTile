using System;
using System.Net;
using BruTile;
using BruTile.Tms;

namespace BrutileArcGIS.Lib
{
    public class ConfigTms: IConfig
    {
        private readonly bool _overwriteUrls;

        public ConfigTms(String url, bool overwriteUrls)
        {
            Url = url;
            _overwriteUrls = overwriteUrls;
        }

        public ITileSource CreateTileSource()
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14";
            var response = (HttpWebResponse)request.GetResponse();
            var stream= response.GetResponseStream();
            var tileSource = _overwriteUrls ? TileMapParser.CreateTileSource(stream, Url) : TileMapParser.CreateTileSource(stream);
            return tileSource;
        }

        public string Url { get; set; }
    }
}
