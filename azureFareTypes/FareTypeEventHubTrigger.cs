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
            [EventHubTrigger("faretypes", Connection = "EventHubConnectionString")] EventData eventData,
            ILogger log)
        {
            try
            {
                var eventBody = Encoding.UTF8.GetString(eventData.Body);
                var fareTypeEvent = JsonConvert.DeserializeObject<FareTypeEvent>(eventBody);
                if (fareTypeEvent != null && fareTypeEvent.Table == "FareTypes")
                {
                    log.LogInformation($"Action '{fareTypeEvent.Action}', data: {JsonConvert.SerializeObject(fareTypeEvent.Data)}");
                    switch (fareTypeEvent.Action?.ToLower())
                    {
                        case "insert":
                        case "update":
                            await _cosmosDb.Upsert(fareTypeEvent.Data);
                            break;

                        case "delete":
                            await _cosmosDb.Delete(fareTypeEvent.Id);
                            break;

                        case "sync":
                            var fareTypes = await _db.FareTypes.ToListAsync();
                            foreach (var fareType in fareTypes)
                            {
                                await _cosmosDb.Upsert(fareType);
                            }
                            break;

                        default:
                            log.LogWarning($"Unknown action: {fareTypeEvent.Action}");
                            break;
                    }
                }
                else
                {
                    log.LogWarning($"Unknown event: {eventBody}");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Unhandled exception: {ex.Message}");
            }
        }
    }
}
