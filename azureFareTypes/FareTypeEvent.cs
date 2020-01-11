using System;
using System.Collections.Generic;
using System.Text;

namespace azureFareTypes
{
    public class FareTypeEvent
    {
        public string Id { get; set; }
        public string Table { get; set; }
        public string Action { get; set; }
        public DateTime TimeStamp { get; set; }
        public FareType Data { get; set; }
    }
}
