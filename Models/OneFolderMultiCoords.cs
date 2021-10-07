using System;
using System.Collections.Generic;
using System.Text;

namespace KmlToJson.Models
{
    public class OneFolderMultiCoords
    {
        public string Text { get; set; }
        public List<SharpKml.Base.Vector> Coords { get; set; }
    }
}
