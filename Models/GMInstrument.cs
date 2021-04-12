using System;
using System.Collections.Generic;
using System.Text;

namespace KmlToJson.Models
{
    public class GMInstrument
    {
        public GMInstrument(string insName, double longitude, double latitude, GMInstrumentType gMInstrumentType)
        {
            Name = insName;
            Longitude = longitude;
            Latitude = latitude;
            Type = gMInstrumentType;
        }

        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public GMInstrumentType Type { get; set; }
    }
}
