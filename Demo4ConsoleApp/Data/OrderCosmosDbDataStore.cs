using Demo4ConsoleApp.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo4ConsoleApp.Data
{
    public class OrderCosmosDbDataStore : CosmosDbDataStore, IOrderCosmosDbDataStore
    {
        public OrderCosmosDbDataStore(
            IOptions<CosmosDbDataStoreOptions> cosmosDbDataStoreOptions) : base(cosmosDbDataStoreOptions)
        {
        }

        public async Task<IOrderDocument> AddAsync(
            Guid orderId,
            IOrderDocument order)
        {
            ConsoleHelper.WriteLine("Creating order...");

            var orderContainer =
               _cosmosDatabase.Containers["orders"];

            var orderDocument =
                await orderContainer.Items.CreateItemAsync<IOrderDocument>(
                    orderId.ToString(),
                    order);

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderDocument.Resource, Formatting.Indented),
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order created.");

            return orderDocument.Resource;
        }

        public async Task<IOrderDocument> DeleteByIdAsync(
            Guid orderId,
            Guid id)
        {
            var orderContainer =
               _cosmosDatabase.Containers["orders"];

            var orderDocument =
                await orderContainer.Items.DeleteItemAsync<IOrderDocument>(
                    orderId.ToString(),
                    id.ToString());

            return orderDocument.Resource;
        }

        public async Task<IOrderDocument> FetchByIdAsync(
            Guid orderId,
            Guid id)
        {
            var orderContainer =
               _cosmosDatabase.Containers["orders"];

            var orderDocument =
                await orderContainer.Items.ReadItemAsync<IOrderDocument>(
                    orderId.ToString(),
                    id.ToString());

            return orderDocument.Resource;
        }

        public async Task<IEnumerable<IOrderDocument>> FetchListByOrderIdAsync(
            Guid orderId)
        {
            var orderContainer =
                _cosmosDatabase.Containers["orders"];

            var query =
                $"SELECT * FROM oli";

            var queryDefinition =
                new CosmosSqlQueryDefinition(query);

            var orders =
                orderContainer.Items.CreateItemQuery<IOrderDocument>(
                    queryDefinition, 
                    orderId.ToString());

            var orderList = new List<IOrderDocument>();

            while (orders.HasMoreResults)
            {
                orderList.AddRange(
                    await orders.FetchNextSetAsync());
            };

            return orderList;
        }

        public async Task<IOrderDocument> UpdateByIdAsync(
            Guid orderId,
            Guid id,
            IOrderDocument order)
        {
            var orderContainer =
               _cosmosDatabase.Containers["orders"];

            var orderDocument =
                await orderContainer.Items.ReplaceItemAsync<IOrderDocument>(
                    orderId.ToString(),
                    id.ToString(),
                    order);

            return orderDocument.Resource;
        }
    }
}
