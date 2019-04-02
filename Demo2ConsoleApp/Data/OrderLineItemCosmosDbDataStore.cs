using Demo2ConsoleApp.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo2ConsoleApp.Data
{
    public class OrderLineItemCosmosDbDataStore : CosmosDbDataStore, IOrderLineItemCosmosDbDataStore
    {
        public OrderLineItemCosmosDbDataStore(
            IOptions<CosmosDbDataStoreOptions> cosmosDbDataStoreOptions) : base(cosmosDbDataStoreOptions)
        {
        }

        public async Task<OrderLineItemDocument> AddAsync(
            Guid orderId,
            OrderLineItemDocument orderLineItem)
        {
            ConsoleHelper.WriteLine("Creating order line item...");

            var orderLineItemContainer =
               _cosmosDatabase.Containers["orders"];

            var orderLineItemDocument =
                await orderLineItemContainer.Items.CreateItemAsync<OrderLineItemDocument>(
                    orderId.ToString(),
                    orderLineItem);

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderLineItemDocument.Resource, Formatting.Indented),
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order line item created.");

            return orderLineItemDocument.Resource;
        }

        public async Task<OrderLineItemDocument> DeleteByIdAsync(
            Guid orderId,
            Guid id)
        {
            var orderLineItemContainer =
               _cosmosDatabase.Containers["orders"];

            var orderLineItemDocument =
                await orderLineItemContainer.Items.DeleteItemAsync<OrderLineItemDocument>(
                    orderId.ToString(),
                    id.ToString());

            return orderLineItemDocument.Resource;
        }

        public async Task<OrderLineItemDocument> FetchByIdAsync(
            Guid orderId,
            Guid id)
        {
            var orderLineItemContainer =
               _cosmosDatabase.Containers["orders"];

            var orderLineItemDocument =
                await orderLineItemContainer.Items.ReadItemAsync<OrderLineItemDocument>(
                    orderId.ToString(),
                    id.ToString());

            return orderLineItemDocument.Resource;
        }

        public async Task<IEnumerable<OrderLineItemDocument>> FetchListByOrderIdAsync(
            Guid orderId)
        {
            var orderContainer =
                _cosmosDatabase.Containers["orders"];

            var query =
                $"SELECT * FROM o";

            query += $" WHERE o.type = 'OrderLineItem'";

            var queryDefinition =
                new CosmosSqlQueryDefinition(query);

            var orderLineItems =
                orderContainer.Items.CreateItemQuery<OrderLineItemDocument>(
                    queryDefinition, 
                    orderId.ToString());

            var orderLineItemList = new List<OrderLineItemDocument>();

            while (orderLineItems.HasMoreResults)
            {
                orderLineItemList.AddRange(
                    await orderLineItems.FetchNextSetAsync());
            };

            return orderLineItemList;
        }

        public async Task<OrderLineItemDocument> UpdateByIdAsync(
            Guid orderId,
            Guid id,
            OrderLineItemDocument orderLineItem)
        {
            var orderLineItemContainer =
               _cosmosDatabase.Containers["orders"];

            var orderLineItemDocument =
                await orderLineItemContainer.Items.ReplaceItemAsync<OrderLineItemDocument>(
                    orderId.ToString(),
                    id.ToString(),
                    orderLineItem);

            return orderLineItemDocument.Resource;
        }
    }
}
