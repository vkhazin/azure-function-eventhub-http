using System;
using System.Collections.Generic;
using System.Text;

namespace azureFareTypes
{
    public class CosmosFareType : FareType
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public int Partion { get; set; }
        public DateTime SyncTimestamp { get; set; }

        public CosmosFareType()
        {
        }

        public CosmosFareType(FareType parent, int partion)
        {
            Id = parent.FareTypeId.ToString();
            SyncTimestamp = DateTime.Now;
            Partion = partion;

            foreach (var prop in parent.GetType().GetFields())
                GetType().GetField(prop.Name).SetValue(this, prop.GetValue(parent));

            foreach (var prop in parent.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(parent, null), null);
        }

        public FareType ToFareType()
        {
            var data = new FareType();

            foreach (var prop in data.GetType().GetFields())
                GetType().GetField(prop.Name).SetValue(data, prop.GetValue(this));

            foreach (var prop in data.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(data, prop.GetValue(this, null), null);

            return data;
        }
    }
}
