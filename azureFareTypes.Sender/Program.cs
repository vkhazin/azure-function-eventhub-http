using Microsoft.Azure.EventHubs;
using System.Text;
using System.Threading.Tasks;
using System;

namespace azureFareTypes.Sender
{
    class Program
    {
        private const string EventHubConnectionString = "Endpoint=sb://farerates-cosmos.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ysyjU6cJnh56UF7Pszn7bTi3Hso0JOueIngM5bfjLBs=;EntityPath=farerates";
        private const string EventHubName = "faretypes";

        static async Task Main(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("Please pass event body as parameter");
                return;
            }

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            Console.WriteLine($"Sending: {args[0]}");

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(args[0])));
            await eventHubClient.CloseAsync();
        }
    }
}
