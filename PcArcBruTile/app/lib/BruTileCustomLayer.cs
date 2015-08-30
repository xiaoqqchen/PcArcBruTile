using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using BruTile;
using BruTile.Cache;
using BrutileArcGIS.Lib;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;

namespace BrutileArcGIS.lib
{
    public sealed class BruTileCustomLayer : BaseCustomLayer
    {
        private static readonly log4net.ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");
        private readonly TileSource _tileSource;
        private readonly FileCache _fileCache;
        private readonly SimpleFileFetcher _simplefilefetcher;
        private readonly ISpatialReference _dataSpatialReference;
        private readonly IMap _map;

        public BruTileCustomLayer(IApplication application, TileSource tileSource, FileCache fileCache)
        {
            _tileSource = tileSource;
            _fileCache = fileCache;
            _simplefilefetcher = new SimpleFileFetcher(tileSource, fileCache);
            var spatialReferences = new SpatialReferences();
            _dataSpatialReference = spatialReferences.GetSpatialReference(_tileSource.Schema.Srs);

            if (SpatialReference.FactoryCode == 0)
            {
                // zet dan de spatial ref...
                m_spatialRef = _dataSpatialReference;
            }
            var mxdoc = (IMxDocument)application.Document;
            _map = mxdoc.FocusMap;
            var envelope = GetDefaultEnvelope();

            // If there is only one layer in the TOC zoom to this layer...
            if (_map.LayerCount == 0)
            {
                ((IActiveView)_map).Extent = envelope;
            }

        }

        public override void Draw(esriDrawPhase drawPhase, IDisplay display, ITrackCancel trackCancel)
        {
            switch (drawPhase)
            {
                case esriDrawPhase.esriDPGeography:
                    if (Valid)
                    {
                        if (Visible)
                        {
                                Logger.Debug("Draw event Layer name: " + Name);
                                var activeView = (IActiveView)_map;
                                var clipEnvelope = display.ClipEnvelope;
                                Logger.Debug("Layer spatial reference: " + SpatialReference.FactoryCode);
                                Logger.Debug("Map spatial reference: " + _map.SpatialReference.FactoryCode);
                                var mapWidth = activeView.ExportFrame.right;
                                var resolution = clipEnvelope.GetMapResolution(mapWidth);
                                var ext = new Extent(clipEnvelope.XMin, clipEnvelope.YMin, clipEnvelope.XMax, clipEnvelope.YMax);
                                //_fetcher.ViewChanged(ext, resolution);
                                _simplefilefetcher.Fetch(ext,resolution);
                                var level = Utilities.GetNearestLevel(_tileSource.Schema.Resolutions, resolution);
                                var tileInfos = _tileSource.Schema.GetTilesInView(ext, level);
                                tileInfos = SortByPriority(tileInfos, ext.CenterX, ext.CenterY);

                                foreach (var tileInfo in tileInfos)
                                {
                                    var tile = _fileCache.Find(tileInfo.Index);
                                    if (tile != null)
                                    {
                                        var filename = _fileCache.GetFileName(tileInfo.Index);
                                        DrawRaster(filename, display);
                                    }
                                }

                        }
                    }
                    break;
                case esriDrawPhase.esriDPAnnotation:
                    break;
            }
        }

        public override bool Visible
        {
            get { return m_visible; }
            set
            {
                m_visible = value;
                if (!m_visible)
                {
                    ((IActiveView)_map).Refresh();
                }
            }
        }


        private static IEnumerable<TileInfo> SortByPriority(IEnumerable<TileInfo> tiles, double centerX, double centerY)
        {
            return tiles.OrderBy(t => Distance(centerX, centerY, t.Extent.CenterX, t.Extent.CenterY));
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0));
        }

        private IEnvelope GetDefaultEnvelope()
        {
            var ext = _tileSource.Schema.Extent;
            var envelope = new EnvelopeClass
            {
                XMin = ext.MinX,
                XMax = ext.MaxX,
                YMin = ext.MinY,
                YMax = ext.MaxY,
                SpatialReference = _dataSpatialReference
            };
            return envelope;
        }

        private void DrawRaster(string file, IDisplay display)
        {
            try
            {
                var rl = new RasterLayerClass();
                rl.CreateFromFilePath(file);
                var props = (IRasterProps)rl.Raster;
                props.SpatialReference = _dataSpatialReference;

                // Fix for issue "Each 256x256 tile rendering differently causing blockly effect."
                // In 10.1 the StrecthType for rasters seems to have changed from esriRasterStretch_NONE to "Percent Clip",
                // giving color problems with 24 or 32 bits tiles.
                // http://arcbrutile.codeplex.com/workitem/11207
                var image = new Bitmap(file, true);
                var format = image.PixelFormat;
                if (format == PixelFormat.Format24bppRgb || format == PixelFormat.Format32bppArgb || format == PixelFormat.Format32bppRgb)
                {
                    var rasterRGBRenderer = new RasterRGBRendererClass();
                    ((IRasterStretch2)rasterRGBRenderer).StretchType = esriRasterStretchTypesEnum.esriRasterStretch_NONE;
                    rl.Renderer = rasterRGBRenderer;
                }

                rl.Renderer.ResamplingType = rstResamplingTypes.RSP_BilinearInterpolation;
                // Now set the spatial reference to the dataframe spatial reference! 
                // Do not remove this line...
                rl.SpatialReference = SpatialReference;
                //rl.Draw(ESRI.ArcGIS.esriSystem.esriDrawPhase.esriDPGeography, (IDisplay)activeView.ScreenDisplay, null);
                rl.Draw(esriDrawPhase.esriDPGeography, display, null);
                //Logger.Debug("End drawing tile.");


            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // what to do now...
                // just try to load next tile...
            }
        }

    }
}
