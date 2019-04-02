using Demo4ConsoleApp.Data;
using Demo4ConsoleApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo4ConsoleApp.Domain
{
    public class OrderAggregateFetcher
    {
        private readonly IOrderCosmosDbDataStore _orderCosmosDbDataStore;

        public OrderAggregateFetcher(
            IOrderCosmosDbDataStore orderCosmosDbDataStore)
        {
            _orderCosmosDbDataStore = orderCosmosDbDataStore;
        }

        public async Task<OrderAggregate> FetchByIdAsync(
            Guid id)
        {
            ConsoleHelper.WriteLine("Fetching order...");

            var orderAggregate =
                new OrderAggregate();

            var orders =
                await _orderCosmosDbDataStore.FetchListByOrderIdAsync(id);

            var order =
                orders.Single(x => x.Type == OrderDocumentType.Order);

            orderAggregate.Order = 
                order as OrderDocument;

            orderAggregate.OrderLineItems =
                new List<OrderLineItemDocument>();

            foreach(var orderLineItem in orders
                .Where(x => x.Type == OrderDocumentType.OrderLineItem))
            {
                orderAggregate.OrderLineItems.Add(
                    orderLineItem as OrderLineItemDocument);
            }

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderAggregate, Formatting.Indented),
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order fetched.");

            return orderAggregate;
        }
    }
}
