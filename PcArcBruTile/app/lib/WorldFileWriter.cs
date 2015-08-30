using System;
using System.Globalization;
using System.IO;
using BruTile;

namespace BrutileArcGIS.lib
{
    public class WorldFileWriter
    {
        public static string GetWorldFile(string format)
        {
            var res = String.Empty;

            format = (format.Contains(@"image/") ? format.Substring(6, format.Length - 6) : format);

            if (format == "jpg")
            {
                res = "jgw";
            }
            if (format == "jpeg")
            {
                res = "jgw";
            }
            else if (format == "png")
            {
                res = "pgw";
            }
            else if (format == "png8")
            {
                res = "pgw";
            }

            else if (format == "tif")
            {
                res = "tfw";
            }

            return res;

        }


        public static void WriteWorldFile(string f, Extent extent, ITileSchema schema)
        {
            using (var sw = new StreamWriter(f))
            {
                var resX = (extent.MaxX - extent.MinX) / schema.GetTileWidth("0");
                var resY = (extent.MaxY - extent.MinY) / schema.GetTileHeight("0");
                sw.WriteLine(resX.ToString(CultureInfo.InvariantCulture));
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine((resY * -1).ToString(CultureInfo.InvariantCulture));
                sw.WriteLine(extent.MinX.ToString(CultureInfo.InvariantCulture));
                sw.WriteLine(extent.MaxY.ToString(CultureInfo.InvariantCulture));
                sw.Close();
            }
        }


    }
}
