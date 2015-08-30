using System;
using System.Drawing;
using ESRI.ArcGIS.Geometry;

namespace BrutileArcGIS.lib
{
    public static class EnvelopeExtensionMethods
    {
        public static float GetMapResolution(this IEnvelope env, int mapWidth)
        {
            var dx = env.XMax - env.XMin;
            var res = Convert.ToSingle(dx / mapWidth);
            return res;
        }


        public static PointF GetCenterPoint(this IEnvelope env)
        {
            var p = new PointF
            {
                X = Convert.ToSingle(env.XMin + (env.XMax - env.XMin) / 2),
                Y = Convert.ToSingle(env.YMin + (env.YMax - env.YMin) / 2)
            };
            return p;
        }
    }
}
