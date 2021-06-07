using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace KmlToJson.Models
{
    public class CeciRetainingWallLine
    {
        public CeciRetainingWallLine()
        {
            LineDots = new List<SharpKml.Base.Vector>();
        }
        public string Url { get; set; }
        [JsonPropertyName("Lv")]
        public string Level { get; set; }
        [JsonPropertyName("LDs")]
        public List<SharpKml.Base.Vector> LineDots { get; set; }
    }
}
