using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using BrutileArcGIS.Lib;
using BrutileArcGIS.lib;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace BrutileArcGIS.forms
{
    public partial class TileCache : Form
    {
        private ITileSource tileSource;
        private List<TileInfo> _tiles;
        private FileCache fileCache;
        private WebTileProvider tileProvider;
        private ITileSchema schema;
        private List<double> extent = new List<double>();
        private IActiveView view;
        public TileCache(IActiveView view)
        {
            InitializeComponent();
            this.view = view;
            var tiles = Enum.GetValues(typeof(DownloadTileLayer));
            foreach (var t in tiles)
            {
                comboBox1.Items.Add(t.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            extent.Add(double.Parse(textBox3.Text));
            extent.Add(double.Parse(textBox4.Text));
            extent.Add(double.Parse(textBox5.Text));
            extent.Add(double.Parse(textBox2.Text));

            var tile = comboBox1.SelectedItem;
            if (tile == null)
            {
                MessageBox.Show("请选择至少一种地图");
                return;
            }
            EnumBruTileLayer enumBruTileLayer = (EnumBruTileLayer)Enum.Parse(typeof(EnumBruTileLayer), tile.ToString());
            IConfig config = ConfigHelper.GetConfig(enumBruTileLayer);
            string cacheDir = CacheSettings.GetCacheFolder();
            tileSource = config.CreateTileSource();
            schema = tileSource.Schema;
            tileProvider = (WebTileProvider)tileSource.Provider;
            fileCache = CacheDirectory.GetFileCache(cacheDir, config, enumBruTileLayer);

            _tiles = GetTile();


            if (_tiles.ToList().Count > 0)
            {
                DownloadTiles();

            }
        }


        private void DownloadTiles()
        {
            var downloadTiles = new List<TileInfo>();
            for (var i = 0; i < _tiles.Count(); i++)
            {
                if (!fileCache.Exists(_tiles[i].Index))
                {
                    downloadTiles.Add(_tiles[i]);
                }
            }

            if (downloadTiles.Count == 0)
            {
                MessageBox.Show("已经存在缓存文件，不需要缓存");
                return;
            }

            var diare = MessageBox.Show("一共需要下载" + downloadTiles.Count + "张切片,是否继续？", "下载地图", MessageBoxButtons.OKCancel);
            if (diare == DialogResult.Cancel)
                return;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = downloadTiles.Count;
            progressBar1.Value = 0;
            progressBar1.Step = 50;

            if (downloadTiles.Count > 0)
            {
                int count = 1;
                int allCount = 50;
                ThreadPool.SetMaxThreads(100, 100);
                while ((count - 1)*allCount < downloadTiles.Count)
                {
                    try
                    {
                        int temp = allCount;
                        if (count*allCount > downloadTiles.Count)
                            temp = downloadTiles.Count - (count - 1)*allCount;
                        var doneEvents = new MultipleThreadResetEvent(temp);
                        for (int i = 0; i < temp; i++)
                        {
                            TileInfo t = downloadTiles[(count - 1)*allCount + i];
                            object o = new object[] {t, doneEvents};

                            ThreadPool.QueueUserWorkItem(DownloadTile, o);
                        }

                        int wt;
                        int ct;
                        ThreadPool.GetAvailableThreads(out wt, out ct);
                        if (wt < 15)
                        {
                            Thread.Sleep(5000);
                            continue;
                        }

                        doneEvents.WaitAll();
                        doneEvents.Dispose();
                        //Thread.Sleep(10);
                        progressBar1.PerformStep();
                        label4.Text = (allCount*count).ToString() + "/" + downloadTiles.Count;
                        Application.DoEvents();
                        count++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("下载异常:" + ex.Message);
                    }
                }
                MessageBox.Show("下载完毕！");
            }
        }

        private void DownloadTile(object tile)
        {
            try
            {
            var parameters = (object[])tile;
            if (parameters.Length != 2) throw new ArgumentException("Two parameters expected");
            var tileInfo = (TileInfo)parameters[0];
            var doneEvent = (MultipleThreadResetEvent)parameters[1];

            var url = tileProvider.Request.GetUri(tileInfo);
            var bytes = GetBitmap(url);


                if (bytes != null)
                {
                    var name = fileCache.GetFileName(tileInfo.Index);
                    fileCache.Add(tileInfo.Index, bytes);
                    CreateRaster(tileInfo, name);

                }
                doneEvent.SetOne();
            }
            catch (Exception ex)
            {
                MessageBox.Show("下载异常:" + ex.Message);
            }

        }

        private void CreateRaster(TileInfo tile, string name)
        {
            var schema = tileSource.Schema;
            var fi = new FileInfo(name);
            var tfwFile = name.Replace(fi.Extension, "." + WorldFileWriter.GetWorldFile(schema.Format));
            WorldFileWriter.WriteWorldFile(tfwFile, tile.Extent, schema);
        }

        public byte[] GetBitmap(Uri uri)
        {
            byte[] bytes = null;
            while (true)
            {
                try
                {
                    bytes = RequestHelper.FetchImage(uri);
                    break;
                }
                catch (System.Net.WebException ex)
                {
                }
            }

            return bytes;
        }

        private List<TileInfo> GetTile()
        {
            Extent exten = new Extent(extent[0], extent[2], extent[1], extent[3]);
            int level1 = int.Parse(levelTextBox1.Text);
            int level2 = int.Parse(levelTextBox2.Text);
            List<TileInfo> alltiles = new List<TileInfo>();
            for (; level1 <= level2; level1++)
            {
                var tiles = schema.GetTilesInView(exten, level1.ToString());
                if (tiles.Count()>0)
                    alltiles.AddRange(tiles);
            }
            
            return alltiles;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var tile = comboBox1.SelectedItem;
            if (tile == null)
            {
                MessageBox.Show("请选择至少一种地图");
                return;
            }
            EnumBruTileLayer enumBruTileLayer = (EnumBruTileLayer)Enum.Parse(typeof(EnumBruTileLayer), tile.ToString());
            IConfig config = ConfigHelper.GetConfig(enumBruTileLayer);
            tileSource = config.CreateTileSource();
            schema = tileSource.Schema;
            var env = Projector.ProjectEnvelope(view.Extent, schema.Srs);
            textBox3.Text = env.XMin.ToString();
            textBox4.Text = env.XMax.ToString();
            textBox5.Text = env.YMin.ToString();
            textBox2.Text = env.YMax.ToString();
        }
    }

    public enum DownloadTileLayer
    {
        GoogleMap, GoogleHybrid,
        GaodeRoad, GaodeArial, GaodeHybrid,
        TDTRoad, TDTArial, TDTLabel,
        OSMRoad, OSMBike, OsmTraffic

    }
}
