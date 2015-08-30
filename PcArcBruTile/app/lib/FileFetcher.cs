using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BruTile;
using BruTile.Cache;
using BrutileArcGIS.lib;

namespace BrutileArcGIS.Lib
{
    public class FileFetcher<T>
    {
        private readonly ITileSource _tileSource;
        private readonly FileCache _fileCache;
        private readonly IList<TileIndex> _tilesInProgress = new List<TileIndex>();
        private const int MaxThreads = 4;
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private volatile bool _isAborted;
        private volatile bool _isViewChanged;
        //private readonly IFetchStrategy _strategy = new FetchStrategy();
        private double _resolution;
        //private readonly Retries _retries = new Retries();
        private Extent _extent;


        public event DataChangedEventHandler<T> DataChanged;

        public FileFetcher(ITileSource tileSource, FileCache filecache)
        {
            _tileSource = tileSource;
            _fileCache = filecache;

            StartFetchLoop();

        }

        public void ViewChanged(Extent newExtent, double newResolution)
        {
            _extent = newExtent;
            _resolution = newResolution;
            _isViewChanged = true;
            _waitHandle.Set();
        }

        private void StartFetchLoop()
        {
            ThreadPool.QueueUserWorkItem(FetchLoop);
        }


        private void FetchLoop(object state)
        {
            IEnumerable<TileInfo> tilesWanted = null;

            while (!_isAborted)
            {
                _waitHandle.WaitOne();

                if (_tileSource.Schema == null)
                {
                    _waitHandle.Reset();    // set in wait mode 
                    continue;              // and go to begin of loop to wait
                }

                if (_isViewChanged || tilesWanted == null)
                {
                    var levelId = Utilities.GetNearestLevel(_tileSource.Schema.Resolutions, _resolution);
                    tilesWanted = GetTilesWanted(_tileSource.Schema, _extent, levelId);
                    _isViewChanged = false;
                }

                var tilesMissing = GetTilesMissing(tilesWanted, _fileCache);

                FetchTiles(tilesMissing);

                if (tilesMissing.Count == 0)
                {
                    _waitHandle.Reset();
                }

                if (_tilesInProgress.Count >= MaxThreads) { _waitHandle.Reset(); }
            }
        }

        private void FetchTiles(IEnumerable<TileInfo> tilesMissing)
        {
            foreach (TileInfo info in tilesMissing)
            {
                if (_tilesInProgress.Count >= MaxThreads) return;
                FetchTile(info);
            }
        }



        public void FetchTiles(Extent currentExtent, string level)
        {
            var tilesMissing = GetTilesWanted(_tileSource.Schema, currentExtent, level);
            foreach (var info in tilesMissing)
            {
                if (_tilesInProgress.Count >= MaxThreads) return;
                FetchTile(info);
            }
        }

        private void FetchTile(TileInfo info)
        {
            // first some checks
            if (_tilesInProgress.Contains(info.Index)) return;

            // prepare for request
            lock (_tilesInProgress) { _tilesInProgress.Add(info.Index); }

            // now we can go for the request.
            FetchAsync(info);
        }

        public void AbortFetch()
        {
            _isAborted = true;
            _waitHandle.Set(); // active fetch loop so it can run out of the loop
        }


        private void FetchAsync(TileInfo tileInfo)
        {
            ThreadPool.QueueUserWorkItem(
                source =>
                {
                    Exception error = null;
                    Tile<T> tile = null;

                    try
                    {
                        if (_tileSource != null)
                        {
                            byte[] data = _tileSource.Provider.GetTile(tileInfo);
                            _fileCache.Add(tileInfo.Index, data);

                            _fileCache.AddWorldFile(tileInfo, _tileSource.Schema.GetTileHeight("0"), _tileSource.Schema.GetTileHeight("0"),
                                _tileSource.Schema.Format);
                            tile = new Tile<T> { Data = data, Info = tileInfo };
                           
                        }
                    }
                    catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
                    {
                        error = ex;
                    }

                    lock (_tilesInProgress)
                    {
                        if (_tilesInProgress.Contains(tileInfo.Index))
                            _tilesInProgress.Remove(tileInfo.Index);
                    }

                    _waitHandle.Set();
                    if (DataChanged != null && !_isAborted)
                        DataChanged(this, new DataChangedEventArgs<T>(error, false, tile));

                });
        }


        public IList<TileInfo> GetTilesMissing(IEnumerable<TileInfo> tilesWanted, FileCache fileCache)
        {
            return tilesWanted.Where(
                info => fileCache.Find(info.Index) == null).ToList();
        }


        public IList<TileInfo> GetTilesWanted(ITileSchema schema, Extent extent, string levelId)
        {
            return schema.GetTilesInView(extent, (levelId)).ToList();
        }

    }


    public delegate void DataChangedEventHandler<T>(object sender, DataChangedEventArgs<T> e);

    public class DataChangedEventArgs<T>
    {
        public DataChangedEventArgs(Exception error, bool cancelled, Tile<T> tile)
        {
            Error = error;
            Cancelled = cancelled;
            Tile = tile;
        }

        public Exception Error { get; private set; }
        public bool Cancelled { get; private set; }
        public Tile<T> Tile { get; private set; }
    }

}
