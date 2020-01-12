using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;

namespace azureFareTypes.Tests
{
    [TestClass()]
    public class FareTypeEventHubTriggerTests : FareTypesTests
    {
        [TestMethod()]
        public async Task InsertTest()
        {
            var cosmosDb = new FakeCosmosDb();

            var fareTypeEvent = new FareTypeEvent()
            {
                Table = "FareTypes",
                Action = "insert",
                TimeStamp = DateTime.Now,
                Data = new FareType()
                {
                    FareTypeId = 123
                }
            };
            var eventData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(fareTypeEvent)));

            await new FareTypeEventHubTrigger(Db, cosmosDb).Run(eventData, Log);
            Assert.IsNotNull(await cosmosDb.Get(123));
        }

        [TestMethod()]
        public async Task DeleteTest()
        {
            var cosmosDb = new FakeCosmosDb();
            await cosmosDb.Insert(new FareType() { FareTypeId = 123 });

            var fareTypeEvent = new FareTypeEvent()
            {
                Table = "FareTypes",
                Action = "delete",
                TimeStamp = DateTime.Now,
                Id = "123"
            };
            var eventData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(fareTypeEvent)));

            await new FareTypeEventHubTrigger(Db, cosmosDb).Run(eventData, Log);
            Assert.IsNull(await cosmosDb.Get(123));
        }

        [TestMethod()]
        public async Task UpdateTest()
        {
            var cosmosDb = new FakeCosmosDb();
            await cosmosDb.Insert(new FareType() { FareTypeId = 123, DistanceRate = 4 });

            var fareTypeEvent = new FareTypeEvent()
            {
                Table = "FareTypes",
                Action = "update",
                TimeStamp = DateTime.Now,
                Id = "123",
                Data = new FareType()
                {
                    FareTypeId = 123,
                    DistanceRate = 5
                }
            };
            var eventData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(fareTypeEvent)));

            await new FareTypeEventHubTrigger(Db, cosmosDb).Run(eventData, Log);

            var fareType = await cosmosDb.Get(123);
            Assert.IsNotNull(fareType);
            Assert.AreEqual(5, fareType.DistanceRate);
        }

        /*
         * Send an event to the hub
         * 
        private const string EventHubConnectionString = "Endpoint=";
        private const string EventHubName = "farerates";

        [TestMethod()]
        public async Task RunTest()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            var fareTypeEvent = new FareTypeEvent()
            {
                Table = "FareTypes",
                Action = "insert",
                TimeStamp = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                Data = new FareType()
                {
                    FareTypeId = 123
                }
            };

            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(fareTypeEvent))));
            await eventHubClient.CloseAsync();
        }
        */
    }
}