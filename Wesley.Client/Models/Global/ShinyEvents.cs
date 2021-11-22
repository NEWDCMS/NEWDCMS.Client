using LiteDB;
using System;

namespace Wesley.Client.Models
{
    public class GpsEvent
    {
        [BsonId]

        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double PositionAccuracy { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public double Heading { get; set; }
        public double HeadingAccuracy { get; set; }

        public DateTime Date { get; set; }
    }

    public class GeofenceEvent
    {
        [BsonId]

        public int Id { get; set; }

        public bool Entered { get; set; }
        public string Identifier { get; set; }
        public DateTime Date { get; set; }

        public string Text => this.Identifier;
        public string Detail => this.Entered
            ? $"Entered on {this.Date:MMM d 'at' h:mm tt}"
            : $"Exited on {this.Date:MMM d 'at' h:mm tt}";
    }
}
