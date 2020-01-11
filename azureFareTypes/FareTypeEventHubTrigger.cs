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
        public FareTypesDbContext FareTypesDb;

        public FareTypeEventHubTrigger(FareTypesDbContext db)
        {
            FareTypesDb = db;
        }

        [FunctionName("EventHubTrigger")]
        public async Task Run(
            [EventHubTrigger("farerates", Connection = "EventHubConnectionString")] EventData eventData,
            ILogger log)
        {
            var fareTypeEvent = JsonConvert.DeserializeObject<FareTypeEvent>(Encoding.UTF8.GetString(eventData.Body));
            if (fareTypeEvent != null && fareTypeEvent.Table == "FareTypes")
            {
                var cosmosDb = new FareTypesCosmosDb();
                await cosmosDb.Init();

                switch (fareTypeEvent.Action)
                {
                    case "insert":
                        await cosmosDb.Insert(fareTypeEvent.Data);
                        break;

                    case "update":
                        await cosmosDb.Update(fareTypeEvent.Id, fareTypeEvent.Data);
                        break;

                    case "delete":
                        await cosmosDb.Delete(fareTypeEvent.Id);
                        break;

                    case "sync":
                        var fareTypes = await FareTypesDb.FareTypes.ToListAsync();
                        foreach (var fareType in fareTypes)
                        {
                            if (!await cosmosDb.Insert(fareType))
                            {
                                await cosmosDb.Update(fareType.FareTypeId.ToString(), fareType);
                            }
                        }
                        break;
                }
            }
        }
    }
}
