using System;
using BruTile.Cache;
using BrutileArcGIS.Lib;

namespace BrutileArcGIS.lib
{
    public class CacheDirectory
    {
        public static FileCache GetFileCache(string baseCacheDir, IConfig config,EnumBruTileLayer enumBruTileLayer)
        {
            var schema = config.CreateTileSource().Schema;

            var cacheDirType = GetCacheDirectory(config, enumBruTileLayer, baseCacheDir);

            var format = schema.Format;

            if (format.Contains(@"image/"))
            {
                format = format.Substring(6, schema.Format.Length - 6);
            }
            if (format.Contains("png8"))
            {
                format = format.Replace("png8", "png");
            }
            var fileCache = new FileCache(cacheDirType, format);

            return fileCache;

        }

        private static string GetCacheDirectory(IConfig config, EnumBruTileLayer layerType, string baseCacheDir)
        {
            string cacheDirectory = String.Format("{0}{1}{2}", baseCacheDir, System.IO.Path.DirectorySeparatorChar, layerType);

            if (layerType == EnumBruTileLayer.TMS || layerType == EnumBruTileLayer.InvertedTMS)
            {
                string url = (layerType == EnumBruTileLayer.TMS ? ((ConfigTms)config).Url : ((ConfigInvertedTMS)config).Url);

                string service = url.Substring(7, url.Length - 7);
                service = service.Replace(@"/", "-");
                service = service.Replace(":", "-");

                if (service.EndsWith("-"))
                {
                    service = service.Substring(0, service.Length - 1);
                }
                cacheDirectory = String.Format("{0}{1}{2}{3}{4}", baseCacheDir, System.IO.Path.DirectorySeparatorChar, layerType, System.IO.Path.DirectorySeparatorChar, service);
            }

            return cacheDirectory;
        }

    }
}
