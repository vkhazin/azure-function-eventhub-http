using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(azureFareTypes.Startup))]
namespace azureFareTypes
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            builder.Services.AddDbContext<FareTypesDbContext>(
                options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));

            builder.Services.AddTransient<IFareTypesCosmosDb>(cosmosDb => new FareTypesCosmosDb(
                new CosmosDbSettings()
                {
                    EndpointUri = Environment.GetEnvironmentVariable("CosmosEndpointUri"),
                    PrimaryKey = Environment.GetEnvironmentVariable("CosmosPrimaryKey"),
                    DatabaseId = Environment.GetEnvironmentVariable("CosmosDatabaseId"),
                    ContainterId = Environment.GetEnvironmentVariable("CosmosContainterId")
                }));
        }
    }
}
