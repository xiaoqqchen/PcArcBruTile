using ESRI.ArcGIS.Geometry;

namespace BrutileArcGIS.Lib
{
    public class Projector
    {
        public static IEnvelope ProjectEnvelope(IEnvelope envelope, string srs)
        {
            var spatialReferences = new SpatialReferences();
            var dataSpatialReference = spatialReferences.GetSpatialReference(srs);
            envelope.Project(dataSpatialReference);
            return envelope;
        }

    }
}
