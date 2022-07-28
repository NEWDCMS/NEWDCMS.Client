using System;
namespace Wesley.Client.BaiduMaps
{
    public struct Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Ctype { get; set; }


        public Coordinate(double latitude, double longitude, int ctype = 0)
        {
            Latitude = Math.Min(Math.Max(latitude, -90d), 90d);
            Longitude = Math.Min(Math.Max(longitude, -180d), 180d);
            Ctype = ctype;
        }


        public override string ToString()
        {
            return $"[{Latitude:F4}, {Longitude:F4}]";
        }

        public static implicit operator string(Coordinate coord)
        {
            return coord.ToString();
        }

        private const double EPSILON = 0.0000001;
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || GetType() != obj.GetType())
            {
                return false;
            }

            var right = (Coordinate)obj;
            return Math.Abs(right.Latitude - Latitude) < EPSILON
                   && Math.Abs(right.Longitude - Longitude) < EPSILON;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !Equals(left, right);
        }
    }
}

