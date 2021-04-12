using System;
using System.Collections.Generic;
using System.Text;

namespace KmlToJson.Models
{
    public class GMInstrumentType
    {
        public GMInstrumentType(string insTypeName)
        {
            Name = insTypeName;
        }

        public string Name { get; set; }
    }
}
