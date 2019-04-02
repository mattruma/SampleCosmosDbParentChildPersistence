using Demo3ConsoleApp.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo3ConsoleApp.Data
{
    public class OrderCosmosDbDataStore : CosmosDbDataStore, IOrderCosmosDbDataStore
    {
        public OrderCosmosDbDataStore(
            IOptions<CosmosDbDataStoreOptions> cosmosDbDataStoreOptions) : base(cosmosDbDataStoreOptions)
        {
        }

        public async Task<OrderDocument> AddAsync(
            OrderDocument order)
        {
            ConsoleHelper.WriteLine("Creating order...");

            var orderContainer =
               _cosmosDatabase.Containers["orders"];

            var orderDocument =
                await orderContainer.Items.CreateItemAsync<OrderDocument>(
                    order.Id.ToString(),
                    order);

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderDocument.Resource, Formatting.Indented),
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order created.");

            return orderDocument.Resource;
        }

        public async Task<OrderDocument> DeleteByIdAsync(
            Guid id)
        {
            var orderContainer =
               _cosmosDatabase.Containers["orders"];

            var orderDocument =
                await orderContainer.Items.DeleteItemAsync<OrderDocument>(
                    id.ToString(),
                    id.ToString());

            return orderDocument.Resource;
        }

        public async Task<OrderDocument> FetchByIdAsync(
            Guid id)
        {
            var orderContainer =
               _cosmosDatabase.Containers["orders"];

            var orderDocument =
                await orderContainer.Items.ReadItemAsync<OrderDocument>(
                    id.ToString(),
                    id.ToString());

            return orderDocument.Resource;
        }

        public async Task<OrderDocument> UpdateByIdAsync(
            Guid id,
            OrderDocument order)
        {
            var orderContainer =
               _cosmosDatabase.Containers["orders"];

            var orderDocument =
                await orderContainer.Items.ReplaceItemAsync<OrderDocument>(
                    id.ToString(),
                    id.ToString(),
                    order);

            return orderDocument.Resource;
        }
    }
}
