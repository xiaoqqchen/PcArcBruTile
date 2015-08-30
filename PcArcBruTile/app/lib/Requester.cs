using System.Net;
using System.Text;
using System.Xml;

namespace BrutileArcGIS.lib
{
    public class Requester
    {
        public static XmlDocument GetXmlDocument(string url)
        {
            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14");

            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            client.Proxy = proxy;
            
            var theBytes = client.DownloadData(url);
            var test = Encoding.UTF8.GetString(theBytes);
            client.Dispose();
            var doc = new XmlDocument();
            doc.LoadXml(test);
            return doc;
        }
    }
}
