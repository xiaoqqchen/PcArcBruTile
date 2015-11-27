using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BruTile;
using BrutileArcGIS.lib;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using Microsoft.SqlServer.MessageBox;

namespace BrutileArcGIS.Lib
{
    [Guid("1EF3586D-8B42-4921-9958-A73F4833A6FA")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("BruTileArcGIS.BruTileLayer")]
    public class BruTileLayer : ILayer, ILayerPosition, IGeoDataset, IPersistVariant, ILayer2, IMapLevel,
        ILayerDrawingProperties, IDisplayAdmin2, ISymbolLevels, ILayerEffects
        , IDisplayFilterManager
    {
        private static readonly log4net.ILog Logger = LogManager.GetLogger("ArcBruTileSystemLogger");
        private readonly IApplication _application;
        private IEnvelope _envelope;
        private ISpatialReference _dataSpatialReference;
        private bool _visible;
        private IMap _map;
        private EnumBruTileLayer _enumBruTileLayer;
        private string _cacheDir;
        private int _tileTimeOut;
        private double _layerWeight=101;
        private IConfig _config;
        public const string Guid = "1EF3586D-8B42-4921-9958-A73F4833A6FA";
        private string _currentLevel;
        private ITileSchema _schema;
        private ITileSource _tileSource;
        private bool _supportsInteractive = true;
        private short _transparency;

        // used in the add services dialog
        public BruTileLayer(IApplication app, EnumBruTileLayer enumBruTileLayer, string tmsUrl, bool overwriteUrls)
        {
            ShowTips = false;
            Name = "BruTile";

            Cached = false;
            _config = ConfigHelper.GetConfig(enumBruTileLayer, tmsUrl, overwriteUrls);

            _application = app;
            _enumBruTileLayer = enumBruTileLayer;
            InitializeLayer();
        }

        // used by bing initializer
        public BruTileLayer(IApplication application,EnumBruTileLayer enumBruTileLayer)
        {
            //var config = ConfigurationHelper.GetConfig();
            //var bingToken=config.AppSettings.Settings["BingToken"].Value;
            // var bingUrl = config.AppSettings.Settings["BingUrl"].Value;
            
            ShowTips = false;
            Name = "BruTile";
            Cached = false;
            _config = ConfigHelper.GetConfig(enumBruTileLayer);
            
            _application = application;
            _enumBruTileLayer = enumBruTileLayer;
            InitializeLayer();
        }

        // used by WmsC
        public BruTileLayer(IApplication application, IConfig config)
        {
            ShowTips = false;
            Name = "BruTile";
            Cached = false;
            _application = application;
            _config = config;
            _enumBruTileLayer = EnumBruTileLayer.WMSC;
            InitializeLayer();
        }

        private void InitializeLayer()
        {
            var mxdoc = (IMxDocument)_application.Document;
            _map = mxdoc.FocusMap;
            _cacheDir = CacheSettings.GetCacheFolder();
            _tileTimeOut = ConfigurationHelper.GetTileTimeOut();

            var spatialReferences = new SpatialReferences();

            _tileSource=_config.CreateTileSource();
            _schema = _tileSource.Schema;
            _dataSpatialReference = spatialReferences.GetSpatialReference(_schema.Srs);
            _envelope = GetDefaultEnvelope();

            if (_map.SpatialReference == null)
            {
                // zet dan de spatial ref...
                _map.SpatialReference = _dataSpatialReference;
            }

            // If there is only one layer in the TOC zoom to this layer...
            if (_map.LayerCount == 0)
            {
                //envelope.Expand(-0.1, -0.1, true);
                _envelope.Project(_map.SpatialReference);
                ((IActiveView)_map).Extent = _envelope;
            }

            _displayFilter = new TransparencyDisplayFilterClass();
        }

        public void Draw(esriDrawPhase drawPhase, IDisplay display, ITrackCancel trackCancel)
        {
            switch (drawPhase)
            {
                case esriDrawPhase.esriDPGeography:
                    if (Valid)
                    {
                        if (Visible)
                        {
                            try
                            {

                                var clipEnvelope = display.ClipEnvelope;

                                // when loading from a file the active map doesn't exist yet 
                                // so just deal with it here.
                                if (_map == null)
                                {
                                    var mxdoc = (IMxDocument)_application.Document;
                                    _map = mxdoc.FocusMap;
                                }

                                Debug.WriteLine("Draw event");
                                var activeView = _map as IActiveView;
                                Logger.Debug("Layer name: " + Name);

                                if (activeView != null)
                                {
                                    //_envelope = activeView.Extent;
                                    _envelope = clipEnvelope;

                                    Logger.Debug("Draw extent: xmin:" + _envelope.XMin + 
                                                 ", ymin:" + _envelope.YMin +
                                                 ", xmax:" + _envelope.XMax +
                                                 ", ymax:" + _envelope.YMax
                                        );
                                    if (SpatialReference != null)
                                    {
                                        Logger.Debug("Layer spatial reference: " + SpatialReference.FactoryCode);
                                    }
                                    if (_map.SpatialReference != null)
                                    {
                                        Logger.Debug("Map spatial reference: " + _map.SpatialReference.FactoryCode);
                                    }

                                    var bruTileHelper = new BruTileHelper(_tileTimeOut);
                                    //_displayFilter.Transparency = (short)(255 - ((_transparency * 255) / 100));
                                    //if (display.Filter == null)
                                    //{
                                    //    display.Filter = _displayFilter;
                                    //}
                                    var fileCache = CacheDirectory.GetFileCache(_cacheDir,_config,_enumBruTileLayer);
                                    bruTileHelper.Draw(_application.StatusBar.ProgressBar, activeView, fileCache, trackCancel, SpatialReference, ref _currentLevel, _tileSource, display);
                                }
                            }
                            catch (Exception ex)
                            {
                                var mbox = new ExceptionMessageBox(ex);
                                mbox.Show(null);
                            }
                        } // isVisible
                    }  // isValid
                    break;
                case esriDrawPhase.esriDPAnnotation:
                    break;
            }
        }

        public string get_TipText(double x, double y, double tolerance)
        {
            return "brutile";
        }

        public IEnvelope AreaOfInterest
        {
            get{return _envelope;}
            set{_envelope=value;}
        }

        public bool Cached { get; set; }

        public EnumBruTileLayer EnumBruTileLayer
        {
            get { return _enumBruTileLayer; }
            set { _enumBruTileLayer = value; }
        }

        public double MaximumScale { get; set; }

        public double MinimumScale { get; set; }

        public string Name { get; set; }

        public bool ScaleRangeReadOnly
        {
            get {return true; }
        }

        public bool ShowTips { get; set; }

        public string CurrentLevel
        {
            get { return _currentLevel; }
            set { _currentLevel = value; }
        }


        public ISpatialReference SpatialReference { get; set; }

        public int SupportedDrawPhases
        {
            get { return -1; }
        }

        public bool Valid
        {
            get { return true; }
        }

        public bool Visible
        {
            get{return _visible;}
            set
            {
                _visible = value;
                if (!_visible)
                {
                    ((IGraphicsContainer)_map).DeleteAllElements();
                }
            }
        }

        private IEnvelope GetDefaultEnvelope()
        {
            var ext = _schema.Extent;
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

        public IEnvelope Extent
        {
            get 
            {
                return GetDefaultEnvelope();
            }
        }

        public double LayerWeight
        {
            get
            {
                return _layerWeight;
            }
            set
            {
                _layerWeight = value;
            }
        }

        public UID ID
        {
            get
            {
                UID uid = new UIDClass();
                uid.Value = "{" + Guid + "}";
                return uid;
            }
        }

        public void Load(IVariantStream stream)
        {
            try
            {
                Name = (string)stream.Read();
                _visible = (bool)stream.Read();
                _enumBruTileLayer = (EnumBruTileLayer)stream.Read();

                Logger.Debug("Load layer " + Name + ", type: " + _enumBruTileLayer.ToString());

                switch (_enumBruTileLayer)
                {
                    case EnumBruTileLayer.TMS:
                        var url = (string)stream.Read();
                        _config = ConfigHelper.GetTmsConfig(url, true);
                        Logger.Debug("Url: " + url);
                        break;
                    case EnumBruTileLayer.InvertedTMS:
                        var urlInverted = (string)stream.Read();
                        Logger.Debug("Url: " + urlInverted);
                        _config = ConfigHelper.GetConfig(EnumBruTileLayer.InvertedTMS, urlInverted, true);
                        break;

                    default:
                        _config = ConfigHelper.GetConfig(_enumBruTileLayer);
                        break;
                }

                InitializeLayer();
                // get the active map later when 
                _map = null;
            }
            catch (Exception ex)
            {
                Logger.Debug("Error loading custom layer: " + ex.Message);
            }
        }

        public void Save(IVariantStream stream)
        {
            stream.Write(Name);
            stream.Write(_visible);
            stream.Write(_enumBruTileLayer);

            switch (_enumBruTileLayer)
            {
                case EnumBruTileLayer.TMS:
                    var tms = _config as ConfigTms;
                    if (tms != null) stream.Write(tms.Url);
                    break;
                case EnumBruTileLayer.InvertedTMS:
                    var invertedtms = _config as ConfigInvertedTMS;
                    if (invertedtms != null) stream.Write(invertedtms.Url);
                    break;
            }

        }

        public int MapLevel { get; set; }
        public bool DrawingPropsDirty { get; set; }

        public bool DoesBlending
        {
            get { return true; }
        }

        public bool RequiresBanding
        {
            get { return true; }
        }

        public bool UsesFilter
        {
            get { return true; }
        }

        public bool UseSymbolLevels { get; set; }

        public short Brightness { get; set; }

        public short Contrast { get; set; }

        public bool SupportsBrightnessChange
        {
            get { return true; }
        }

        public bool SupportsContrastChange
        {
            get { return true; }
        }

        public bool SupportsInteractive
        {
            get
            {
                return _supportsInteractive;
            }
            set
            {
                _supportsInteractive = value;
            }
        }

        public bool SupportsTransparency
        {
            get { return true; }
        }

        public short Transparency
        {
            get
            {
                return _transparency;
            }
            set
            {
                _transparency = value;
            }
        }

        private ITransparencyDisplayFilter _displayFilter;

        public IDisplayFilter DisplayFilter
        {
            get
            {
                return _displayFilter;
            }
            set
            {
                _displayFilter = (ITransparencyDisplayFilter) value;
            }
        }
    }
}