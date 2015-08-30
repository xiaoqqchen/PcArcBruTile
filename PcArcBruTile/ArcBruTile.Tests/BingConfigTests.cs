using BruTile.Web;
using BrutileArcGIS.Lib;
using Xunit;

namespace ArcBruTile.Tests
{
    public class BingConfigtests
    {
        [Fact]
        public void CreateBingTileSourceReturnsTileSource()
        {
            // arrange
            var config = new ConfigBing(BingMapType.Aerial);

            // act
            var tileSource=config.CreateTileSource();

            // assert
            Assert.NotNull(tileSource);
        }

    }
}
