using System.Drawing;

namespace BrutileArcGIS.Lib
{
    public class Transform
    {
        float _resolution;
        PointF _center;
        float _width;
        float _height;
        BruTile.Extent _extent;

        public Transform(PointF center, float resolution, float width, float height)
        {
            _center = center;
            _resolution = resolution;
            _width = width;
            _height = height;
            UpdateExtent();
        }

        public float Resolution
        {
            set
            {
                _resolution = value;
                UpdateExtent();
            }
            get
            {
                return _resolution;
            }
        }

        public PointF Center
        {
            set
            {
                _center = value;
                UpdateExtent();
            }
        }

        public float Width
        {
            set
            {
                _width = value;
                UpdateExtent();
            }
        }

        public float Height
        {
            set
            {
                _height = value;
                UpdateExtent();
            }
        }

        public BruTile.Extent Extent
        {
            get { return _extent; }
        }

        public PointF WorldToMap(double x, double y)
        {
            return new PointF((float)(x - _extent.MinX) / _resolution, (float)(_extent.MaxY - y) / _resolution);
        }

        public PointF MapToWorld(double x, double y)
        {
            return new PointF((float)(_extent.MinX + x) * _resolution, (float)(_extent.MaxY - y) * _resolution);
        }

        public RectangleF WorldToMap(double x1, double y1, double x2, double y2)
        {
            var point1 = WorldToMap(x1, y1);
            var point2 = WorldToMap(x2, y2);
            return new RectangleF(point1.X, point2.Y, point2.X - point1.X, point1.Y - point2.Y);
        }

       private void UpdateExtent()
        {
            var spanX = _width * _resolution;
            var spanY = _height * _resolution;
            _extent = new BruTile.Extent(_center.X - spanX * 0.5f, _center.Y - spanY * 0.5f,
              _center.X + spanX * 0.5f, _center.Y + spanY * 0.5f);
        }
    }
}
