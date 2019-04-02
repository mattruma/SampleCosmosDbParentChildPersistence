using Demo2ConsoleApp.Data;
using Demo2ConsoleApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Demo2ConsoleApp.Domain
{
    public class OrderAggregateFetcher
    {
        private readonly IOrderCosmosDbDataStore _orderCosmosDbDataStore;
        private readonly IOrderLineItemCosmosDbDataStore _orderLineItemCosmosDbDataStore;

        public OrderAggregateFetcher(
            IOrderCosmosDbDataStore orderCosmosDbDataStore,
            IOrderLineItemCosmosDbDataStore orderLineItemCosmosDbDataStore)
        {
            _orderCosmosDbDataStore = orderCosmosDbDataStore;
            _orderLineItemCosmosDbDataStore = orderLineItemCosmosDbDataStore;
        }

        public async Task<OrderAggregate> FetchByIdAsync(
            Guid id)
        {
            ConsoleHelper.WriteLine("Fetching order...");

            var orderAggregate =
                new OrderAggregate();

            var order =
                await _orderCosmosDbDataStore.FetchByIdAsync(id);

            orderAggregate.Order = order;

            var orderLineItems = 
                await _orderLineItemCosmosDbDataStore.FetchListByOrderIdAsync(id);

            orderAggregate.OrderLineItems = orderLineItems.ToList();

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderAggregate, Formatting.Indented),
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order fetched.");

            return orderAggregate;
        }
    }
}
