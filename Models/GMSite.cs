using System;
using System.Collections.Generic;
using System.Text;

namespace KmlToJson.Models
{
    public class GMSite
    {
        public GMSite(string name)
        {
            Name = name;
            Instruments = new List<GMInstrument>();
        }
        public GMSite(string name, float longi, float lati)
        {
            Name = name;
            Instruments = new List<GMInstrument>();
            Longitude = longi;
            Latitude = lati;
        }

        public string Name { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public List<GMInstrument> Instruments { get; set; }
    }
}
