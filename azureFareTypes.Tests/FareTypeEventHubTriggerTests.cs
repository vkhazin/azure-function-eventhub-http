using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;

namespace azureFareTypes.Tests
{
    [TestClass()]
    public class FareTypeEventHubTriggerTests
    {
        private const string EventHubConnectionString = "Endpoint=sb://farerates-cosmos.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ysyjU6cJnh56UF7Pszn7bTi3Hso0JOueIngM5bfjLBs=";
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
    }
}