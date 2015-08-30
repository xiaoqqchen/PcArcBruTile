namespace BrutileArcGIS.Lib
{
    public class TileMap
    {
        public TileMap()
        {
            OverwriteUrls = true;
        }

        public string Href { get; set; }
        public string Srs { get; set; }
        public string Title { get; set; }
        public string Profile { get; set; }
        public string Type { get; set; }
        public bool OverwriteUrls { get; set; }

        static public int Compare(TileMap a, TileMap b)
        {
            return(System.String.Compare(a.Title, b.Title, System.StringComparison.Ordinal));
        }
    }

    public class TileMapService
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string Href { get; set; }
    }
}
