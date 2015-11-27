using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using BruTile;
using BruTile.Cache;
using BruTile.Extensions;
using BruTile.Web;
using BrutileArcGIS.lib;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;

namespace BrutileArcGIS.Lib
{
    public class BruTileHelper
    {
        private static readonly log4net.ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");
        private static int _tileTimeOut;
        private static ITrackCancel _trackCancel;
        private static ISpatialReference _layerSpatialReference;
        private static ISpatialReference _dataSpatialReference;
        private string _currentLevel;
        private static FileCache _fileCache;
        private static ITileSource _tileSource;
        static bool _needReproject;
        List<TileInfo> _tiles;
        private static IDisplay _display;
        static WebTileProvider _tileProvider;
        private static List<TileInfo> _showTiles = new List<TileInfo>(); 

        public BruTileHelper(int tileTimeOut)
        {
            _tileTimeOut = tileTimeOut;
        }

        
        public void Draw(IStepProgressor stepProgressor,
                         IActiveView activeView,
                         FileCache fileCache,
                         ITrackCancel trackCancel,
                         ISpatialReference layerSpatialReference,
                         ref string currentLevel, ITileSource tileSource, IDisplay display)
        {
            _tileSource = tileSource;
            _trackCancel = trackCancel;
            _layerSpatialReference = layerSpatialReference;
            _currentLevel = currentLevel;
            _fileCache = fileCache;
            _tileProvider = (WebTileProvider)tileSource.Provider;
            var spatialReferences = new SpatialReferences();
            _dataSpatialReference = spatialReferences.GetSpatialReference(tileSource.Schema.Srs);

            //var fetcher = new FileFetcher<Image>(osmTileSource, fileCache);

            _display = display;

            if (!activeView.Extent.IsEmpty)
            {
                _tiles = GetTiles(activeView);
                currentLevel = _currentLevel;
                Logger.Debug("Number of tiles to draw: " + _tiles.Count);

                if (_tiles.ToList().Count > 0)
                {
                    
                    stepProgressor.MinRange = 0;
                    stepProgressor.MaxRange = _tiles.ToList().Count;
                    stepProgressor.Show();

                    var downloadFinished = new ManualResetEvent(false);
        
                    // this is a hack, otherwise we get error message...
                    // "WaitAll for multiple handles on a STA thread is not supported. (mscorlib)"
                    // so lets start a thread first...
                    var t = new Thread(DownloadTiles);
                    t.Start(downloadFinished);

                    // wait till finished
                    downloadFinished.WaitOne();

                    // 3. Now draw all tiles...

                    //if (layerSpatialReference != null && _dataSpatialReference != null)
                    //{
                    //    _needReproject = (layerSpatialReference.FactoryCode != _dataSpatialReference.FactoryCode);
                    //}
                    //Logger.Debug("Need reproject tile: " + _needReproject.ToString());

                    //foreach (var tile in _tiles)
                    //{
                    //    stepProgressor.Step();

                    //    /*                        if (IsBeAdded(tile))
                    //                                continue;                    
                    //                            _showTiles.Add(tile);*/

                    //    /*                        if (tile != null)
                    //                            {
                    //                                var name = _fileCache.GetFileName(tile.Index);

                    //                                if (!File.Exists(name)) continue;
                    //                                DrawRaster(name);
                    //                            }*/
                    //}

                    //stepProgressor.Hide();
                }
                else
                {
                    Logger.Debug("No tiles to retrieve or draw");
                }

                Logger.Debug("End drawing tiles: " + _tiles.ToList().Count);
            }
        }

        private static bool IsBeAdded(TileInfo _tile){
            return _showTiles.Any(t => _tile.Index.Col == t.Index.Col && _tile.Index.Row == t.Index.Row && _tile.Index.Level == t.Index.Level);
        }

        private void DownloadTiles(object args)
        {
            var downloadFinished = args as ManualResetEvent;

            // Loop through the tiles, and filter tiles that are already on disk.
            var downloadTiles=new List<TileInfo>();
            for (var i = 0; i < _tiles.ToList().Count; i++)
            {
/*                if (!_fileCache.Exists(_tiles[i].Index))
                {*/
                    downloadTiles.Add(_tiles[i]);
                //}
                //else
                //{
                //    // Read tiles from disk
                //    //var name = _fileCache.GetFileName(_tiles[i].Index);

                //    // Determine age of tile...
                //    //var fi = new FileInfo(name);
                //    //if ((DateTime.Now - fi.LastWriteTime).Days <= _tileTimeOut) continue;
                //    //File.Delete(name);
                //    downloadTiles.Add(_tiles[i]);
                //}
            }

            Logger.Debug("Number of download tiles:" + downloadTiles.Count);

            if (downloadTiles.Count > 0)
            {
                // 2. Download tiles...
                //var doneEvents = new ManualResetEvent[downloadTiles.Count];
                var doneEvents = new MultipleThreadResetEvent(downloadTiles.Count);

                foreach (var t in downloadTiles)
                {
                    object o = new object[] {t, doneEvents};
                    ThreadPool.SetMaxThreads(100, 100);
                    ThreadPool.QueueUserWorkItem(DownloadTile, o);
                }

                doneEvents.WaitAll();
                Logger.Debug("End waiting for remote tiles...");
            }
            if (downloadFinished != null) downloadFinished.Set();
        }

        private static Dictionary<string, IRasterLayer> layers = new Dictionary<string, IRasterLayer>();
        private static Queue<string> layerNames = new Queue<string>(); 
        private  static void DrawRaster(string file)
        {
            try{
                Stopwatch timer = new Stopwatch();
                timer.Start();
                IRasterLayer rl;
                if (!layers.ContainsKey(file)){

                    Logger.Debug("Start drawing tile" + file + "...");
                    rl = new RasterLayerClass();
                    
                    rl.CreateFromFilePath(file);
                    var props = (IRasterProps) rl.Raster;
                    props.SpatialReference = _dataSpatialReference;

                    if (_needReproject){
                        IRasterGeometryProc rasterGeometryProc = new RasterGeometryProcClass();
                        var missing = Type.Missing;
                        rasterGeometryProc.ProjectFast(_layerSpatialReference, rstResamplingTypes.RSP_NearestNeighbor,
                                                       ref missing, rl.Raster);
                    }

                    // Fix for issue "Each 256x256 tile rendering differently causing blockly effect."
                    // In 10.1 the StrecthType for rasters seems to have changed from esriRasterStretch_NONE to "Percent Clip",
                    // giving color problems with 24 or 32 bits tiles.
                    // http://arcbrutile.codeplex.com/workitem/11207
                    var image = new Bitmap(file, true);
                    var format = image.PixelFormat;
                    if (format == PixelFormat.Format24bppRgb || format == PixelFormat.Format32bppRgb){
                        var rasterRGBRenderer = new RasterRGBRendererClass();
                        ((IRasterStretch2) rasterRGBRenderer).StretchType =
                            esriRasterStretchTypesEnum.esriRasterStretch_NONE;
                        rl.Renderer = rasterRGBRenderer;
                    }

                    rl.Renderer.ResamplingType = rstResamplingTypes.RSP_BilinearInterpolation;
                    // Now set the spatial reference to the dataframe spatial reference! 
                    // Do not remove this line...
                    rl.SpatialReference = _layerSpatialReference;

                    timer.Stop();
                    Logger.Debug(file + "读取花费时间:" + timer.Elapsed.Milliseconds);
                    layers.Add(file,rl);
                    layerNames.Enqueue(file);
                    if (layerNames.Count > 100){
                        string tempname = layerNames.Dequeue();
                        layers.Remove(tempname);
                    }
                }
                else{
                    layers.TryGetValue(file, out rl);
                    timer.Stop();
                    Logger.Debug(file+"查询花费时间:" + timer.Elapsed.Milliseconds);
                }
                timer.Reset();

                //rl.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, (IDisplay)activeView.ScreenDisplay, null);
                rl.Draw(esriDrawPhase.esriDPGeography, _display, null);
                //activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, trackCancel, env);
                timer.Stop();
                Logger.Debug(file + "绘制花费时间:" + timer.Elapsed.Milliseconds);
                //Logger.Debug("End drawing tile.");

            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // what to do now...
                // just try to load next tile...
            }
        }

        private static void DownloadTile(object tile)
        {
            var parameters = (object[])tile;
            if (parameters.Length != 2) throw new ArgumentException("Two parameters expected");
            var tileInfo = (TileInfo)parameters[0];
            var doneEvent = (MultipleThreadResetEvent)parameters[1];

            if (!_trackCancel.Continue())
            {
                doneEvent.SetOne();
                //!!!multipleThreadResetEvent.SetOne();
                return;
            }

            if (!_fileCache.Exists(tileInfo.Index)){
                var url = _tileProvider.Request.GetUri(tileInfo);
                Logger.Debug("Url: " + url);
                var bytes = GetBitmap(url);

                try
                {
                    if (bytes != null)
                    {
                        var name = _fileCache.GetFileName(tileInfo.Index);
                        _fileCache.Add(tileInfo.Index, bytes);
                        CreateRaster(tileInfo, name);
                        Logger.Debug("Tile retrieved: " + url.AbsoluteUri);
                    }
                }
                catch (Exception)
                {
                }
            }

            _showTiles.Add(tileInfo);

            if (tileInfo != null)
            {
                var name = _fileCache.GetFileName(tileInfo.Index);

                if (File.Exists(name))
                    DrawRaster(name);
            }

            doneEvent.SetOne();
        }


        private static void CreateRaster(TileInfo tile, string name)
        {
            var schema = _tileSource.Schema;
            var fi = new FileInfo(name);
            var tfwFile = name.Replace(fi.Extension, "." + WorldFileWriter.GetWorldFile(schema.Format));
            WorldFileWriter.WriteWorldFile(tfwFile, tile.Extent, schema);
        }

        public static byte[] GetBitmap(Uri uri)
        {
            byte[] bytes = null;

            try
            {
                RequestHelper.Timeout = 800;

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";

                bytes = RequestHelper.FetchImage(webRequest);

            }
            catch (System.Net.WebException webException)
            {
                // if there is an error loading the tile
                // like tile doesn't exist on server (404)
                // just log a message and go on
                Logger.Error("Error loading tile webException: " + webException.Message + ". url: " + uri.AbsoluteUri);
            }
            return bytes;
        }

        private List<TileInfo> GetTiles(IActiveView activeView)
        {
            var schema = _tileSource.Schema;
            var env = Projector.ProjectEnvelope(activeView.Extent, schema.Srs);
            Logger.Debug("Tilesource schema srs: " + schema.Srs);
            Logger.Debug("Projected envelope: xmin:" + env.XMin +
                        ", ymin:" + env.YMin +
                        ", xmax:" + env.YMin +
                        ", ymax:" + env.YMin
                        );

            var mapWidth = activeView.ExportFrame.right;
            var mapHeight = activeView.ExportFrame.bottom;
            var resolution = env.GetMapResolution(mapWidth);
            Logger.Debug("Map resolution: " + resolution);

            var centerPoint = env.GetCenterPoint();

            var transform = new Transform(centerPoint, resolution, mapWidth, mapHeight);
            var level = Utilities.GetNearestLevel(schema.Resolutions, transform.Resolution);
            Logger.Debug("Current level: " + level);

            _currentLevel = level;

            var tiles = schema.GetTilesInView(transform.Extent, level);

            return tiles.ToList();
        }


    }
}

