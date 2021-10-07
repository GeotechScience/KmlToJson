using System;
using System.Collections.Generic;
using System.Text;

namespace KmlToJson.Models
{
    public class PointWithName : SharpKml.Dom.Point
    {
        public string Name { get; set; }
    }
}
