using System;
using System.Collections.Generic;
using System.Text;

namespace azureFareTypes
{
    public class CosmosDbSettings
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public string ContainterId { get; set; }
    }
}
