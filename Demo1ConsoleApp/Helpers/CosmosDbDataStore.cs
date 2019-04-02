using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Demo1ConsoleApp.Helpers
{
    public abstract class CosmosDbDataStore : ICosmosDbDataStore
    {
        private readonly IOptions<CosmosDbDataStoreOptions> _cosmosDbDataStoreOptions;
        private readonly CosmosClient _cosmosClient;
        protected readonly CosmosDatabase _cosmosDatabase;

        public CosmosDbDataStore(
            IOptions<CosmosDbDataStoreOptions> cosmosDbDataStoreOptions)
        {
            _cosmosDbDataStoreOptions =
                cosmosDbDataStoreOptions;

            _cosmosClient =
                new CosmosClient(
                    cosmosDbDataStoreOptions.Value.ConnectionString);
            _cosmosDatabase = _cosmosClient.Databases[cosmosDbDataStoreOptions.Value.DatabaseId];
        }
    }
}
