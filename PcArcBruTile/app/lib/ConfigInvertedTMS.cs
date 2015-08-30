using System;
using System.Net;
using BruTile;
using BruTile.Predefined;
using BruTile.Tms;

namespace BrutileArcGIS.Lib
{
    public class ConfigInvertedTMS : IConfig
    {
        private string _url;

        public ConfigInvertedTMS(string url)
        {
            if (url == null) throw new ArgumentNullException("url");
            _url = url;
        }

        public ITileSource CreateTileSource()
        {
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14";
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            var tileSource = TileMapParser.CreateTileSource(stream);
            return new TileSource(tileSource.Provider, new SphericalMercatorInvertedWorldSchema());
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
}
