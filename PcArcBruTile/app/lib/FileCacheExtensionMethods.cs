using System;
using System.Globalization;
using System.IO;
using BruTile;
using BruTile.Cache;

namespace BrutileArcGIS.lib
{
    public static class ExtensionMethods
    {
        public static void AddWorldFile(this FileCache fileCache, TileInfo tileInfo, int width, int height, string format)
        {
            var fileName = fileCache.GetFileName(tileInfo.Index);
            var fi = new FileInfo(fileName);

            var tfwFile = fileName.Replace(fi.Extension, "." + GetWorldFile(format));

            var extent = tileInfo.Extent;

            using (var sw = new StreamWriter(tfwFile))
            {
                var resX = (extent.MaxX - extent.MinX) / width;
                var resY = (extent.MaxY - extent.MinY) / height;
                sw.WriteLine(resX.ToString(CultureInfo.InvariantCulture));
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine((resY * -1).ToString(CultureInfo.InvariantCulture));
                sw.WriteLine(extent.MinX.ToString(CultureInfo.InvariantCulture));
                sw.WriteLine(extent.MaxY.ToString(CultureInfo.InvariantCulture));
                sw.Close();
            }
            
        }

        private static string GetWorldFile(string format)
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

    }
}
