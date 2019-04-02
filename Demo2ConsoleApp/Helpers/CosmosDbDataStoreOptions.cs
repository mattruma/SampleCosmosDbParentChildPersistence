using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Demo2ConsoleApp.Helpers
{
    public class CosmosDbDataStoreOptions
    {
        public string DatabaseId { get; set; }
        public string ConnectionString { get; set; }
        public int MaxConcurrency { get; set; }

        public CosmosDbDataStoreOptions()
        {
            this.DatabaseId =
                Environment.GetEnvironmentVariable(
                    "CosmosDbDataStoreOptions:DatabaseId");
            this.ConnectionString =
                Environment.GetEnvironmentVariable(
                    "CosmosDbDataStoreOptions:ConnectionString");
            this.MaxConcurrency = 4;
        }

        public CosmosDbDataStoreOptions(IConfigurationRoot configuration)
        {
            this.DatabaseId =
                configuration["CosmosDbDataStoreOptions:DatabaseId"];
            this.ConnectionString =
                configuration["CosmosDbDataStoreOptions:ConnectionString"];
            this.MaxConcurrency = 4;
        }

        public IOptions<CosmosDbDataStoreOptions> AsOptions()
        {
            return Options.Create(this);
        }
    }
}
