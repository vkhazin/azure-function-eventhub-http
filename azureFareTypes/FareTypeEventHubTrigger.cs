using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azureFareTypes
{
    public class FareTypeEventHubTrigger
    {
        private readonly FareTypesDbContext _db;
        private readonly IFareTypesCosmosDb _cosmosDb;

        public FareTypeEventHubTrigger(FareTypesDbContext db, IFareTypesCosmosDb cosmosDb)
        {
            _db = db;
            _cosmosDb = cosmosDb;
        }

        [FunctionName("EventHubTrigger")]
        public async Task Run(
            [EventHubTrigger("farerates", Connection = "EventHubConnectionString")] EventData eventData,
            ILogger log)
        {
            var fareTypeEvent = JsonConvert.DeserializeObject<FareTypeEvent>(Encoding.UTF8.GetString(eventData.Body));
            if (fareTypeEvent != null && fareTypeEvent.Table == "FareTypes")
            {
                switch (fareTypeEvent.Action)
                {
                    case "insert":
                        await _cosmosDb.Insert(fareTypeEvent.Data);
                        break;

                    case "update":
                        await _cosmosDb.Update(fareTypeEvent.Id, fareTypeEvent.Data);
                        break;

                    case "delete":
                        await _cosmosDb.Delete(fareTypeEvent.Id);
                        break;

                    case "sync":
                        var fareTypes = await _db.FareTypes.ToListAsync();
                        foreach (var fareType in fareTypes)
                        {
                            if (!await _cosmosDb.Insert(fareType))
                            {
                                await _cosmosDb.Update(fareType.FareTypeId.ToString(), fareType);
                            }
                        }
                        break;
                }
            }
        }
    }
}
