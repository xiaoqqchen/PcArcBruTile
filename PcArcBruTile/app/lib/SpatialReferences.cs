using ESRI.ArcGIS.Geometry;

namespace BrutileArcGIS.Lib
{
    public class SpatialReferences
    {
        public ISpatialReference GetSpatialReference(string epsgCode)
        {
            ISpatialReference res=null;

            // first get the code
            var start=epsgCode.IndexOf(":", System.StringComparison.Ordinal)+1;
            var end = epsgCode.Length;

            int code = int.Parse(epsgCode.Substring(start, end-start));

            // Handle non official EPSG codes...
            if (code == 900913 | code==41001 ) code = 102113;

            if(IsProjectedSpatialReference(code))
            {
                res = GetProjectedSpatialReference(code);
            }
            else if(IsGeographicSpatialReference(code))
            {
                res = GetGeographicSpatialReference(code);
            }

            return res;
        }


        private static bool IsGeographicSpatialReference(int gcsType)
        {
            try
            {
                var pSrf = new SpatialReferenceEnvironmentClass();
                var geographicCoordinateSystem = pSrf.CreateGeographicCoordinateSystem(gcsType);
                // ReSharper disable once UnusedVariable
                var spatialReference = (ISpatialReference)geographicCoordinateSystem;
                return true;
            }
            catch
            {
                return false;
            }
        }


        private static bool IsProjectedSpatialReference(int pcsType)
        {
            try
            {
                var pSrf = new SpatialReferenceEnvironmentClass();
                var mProjectedCoordinateSystem = pSrf.CreateProjectedCoordinateSystem(pcsType);
                // ReSharper disable once UnusedVariable
                var spatialReference = (ISpatialReference)mProjectedCoordinateSystem;
                return true;
            }
            catch
            {
                return false;
            }
        }


        protected ISpatialReference GetGeographicSpatialReference(int gcsType)
        {
            var pSrf = new SpatialReferenceEnvironmentClass();
            var geographicCoordinateSystem = pSrf.CreateGeographicCoordinateSystem(gcsType);
            var spatialReference = (ISpatialReference)geographicCoordinateSystem;
            return spatialReference;
        }


        protected ISpatialReference GetProjectedSpatialReference(int pcsType)
        {
            var pSrf = new SpatialReferenceEnvironmentClass();
            var projectedCoordinateSystem = pSrf.CreateProjectedCoordinateSystem(pcsType);
            var spatialReference = (ISpatialReference)projectedCoordinateSystem;
            return spatialReference;
        }

        public static string GetWebMercator()
        {
            return "PROJCS[&quot;WGS_1984_Web_Mercator&quot;,GEOGCS[&quot;GCS_WGS_1984_Major_Auxiliary_Sphere&quot;,DATUM[&quot;WGS_1984_Major_Auxiliary_Sphere&quot;,SPHEROID[&quot;WGS_1984_Major_Auxiliary_Sphere&quot;,6378137.0,0.0]],PRIMEM[&quot;Greenwich&quot;,0.0],UNIT[&quot;Degree&quot;,0.0174532925199433]],PROJECTION[&quot;Mercator_1SP&quot;],PARAMETER[&quot;False_Easting&quot;,0.0],PARAMETER[&quot;False_Northing&quot;,0.0],PARAMETER[&quot;Central_Meridian&quot;,0.0],PARAMETER[&quot;latitude_of_origin&quot;,0.0],UNIT[&quot;Meter&quot;,1.0]]";
        }

        public static string GetWGS84()
        {
            return "GEOGCS[&quot;GCS_WGS_1984&quot;,DATUM[&quot;WGS_1984&quot;,SPHEROID[&quot;WGS_1984&quot;,6378137.0,298.257223563]],PRIMEM[&quot;Greenwich&quot;,0.0],UNIT[&quot;Degree&quot;,0.0174532925199433]]";
        }

        public static string GetRDNew()
        {
            return "PROJCS[&quot;RD_New&quot;,GEOGCS[&quot;GCS_Amersfoort&quot;,DATUM[&quot;D_Amersfoort&quot;,SPHEROID[&quot;Bessel_1841&quot;,6377397.155,299.1528128]],PRIMEM[&quot;Greenwich&quot;,0.0],UNIT[&quot;Degree&quot;,0.0174532925199433]],PROJECTION[&quot;Double_Stereographic&quot;],PARAMETER[&quot;False_Easting&quot;,155000.0],PARAMETER[&quot;False_Northing&quot;,463000.0],PARAMETER[&quot;Central_Meridian&quot;,5.38763888888889],PARAMETER[&quot;Scale_Factor&quot;,0.9999079],PARAMETER[&quot;Latitude_Of_Origin&quot;,52.15616055555555],UNIT[&quot;Meter&quot;,1.0]]";
        }

    }
}
