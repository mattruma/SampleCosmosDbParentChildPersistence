using Demo4ConsoleApp.Data;
using Demo4ConsoleApp.Domain;
using Demo4ConsoleApp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Demo4ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleHelper.WriteLine("Demo 4", ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("Parent documents and children documents stored in a single collection", ConsoleColor.Yellow);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();

            // NOTE: Leveraging IOptions as this is available in Web Apps through DI, which is nice, but you could just use the CosmosDbDataStoreOptions without IOptions

            var cosmosDbDataStoreOptions =
                Options.Create(new CosmosDbDataStoreOptions(configuration));

            OrderDocument orderDocument;

            orderDocument = CreateOrderDocument();

            var orderCosmosDbDataStore =
                new OrderCosmosDbDataStore(cosmosDbDataStoreOptions);

            await orderCosmosDbDataStore.AddAsync(
                orderDocument.OrderId,
                orderDocument);

            OrderLineItemDocument orderlineItemDocument;

            orderlineItemDocument =
                CreateOrderLineItemDocument(orderDocument);

            await orderCosmosDbDataStore.AddAsync(
                orderDocument.OrderId,
                orderlineItemDocument);

            orderlineItemDocument =
                CreateOrderLineItemDocument(orderDocument);

            await orderCosmosDbDataStore.AddAsync(
                orderDocument.OrderId,
                orderlineItemDocument);

            orderlineItemDocument =
                CreateOrderLineItemDocument(orderDocument);

            await orderCosmosDbDataStore.AddAsync(
                orderDocument.OrderId,
                orderlineItemDocument);

            var orderAggregateFetcher =
                new OrderAggregateFetcher(
                    orderCosmosDbDataStore);

            await orderAggregateFetcher.FetchByIdAsync(
                orderDocument.Id);

            ConsoleHelper.WriteLine("Press any key to exit...", true);
        }

        static OrderDocument CreateOrderDocument()
        {
            ConsoleHelper.WriteLine("Creating order...");

            var orderDocument =
                new OrderDocument();

            orderDocument.Id = Guid.NewGuid();
            orderDocument.OrderId = orderDocument.Id;
            orderDocument.Date = DateTime.UtcNow;

            ConsoleHelper.WriteLine("Order created.");

            return orderDocument;
        }

        static OrderLineItemDocument CreateOrderLineItemDocument(
            OrderDocument orderDocument)
        {
            ConsoleHelper.WriteLine("Creating order line item...");

            var orderLineItemDocument =
                new OrderLineItemDocument();

            orderLineItemDocument.Id = Guid.NewGuid();
            orderLineItemDocument.OrderLineItemId = orderLineItemDocument.Id;
            orderLineItemDocument.OrderId = orderDocument.OrderId;
            orderLineItemDocument.Quantity = 5;
            orderLineItemDocument.ItemId = Guid.NewGuid();
            orderLineItemDocument.UnitCost = 1.99M;

            ConsoleHelper.WriteLine("Order line item created.");

            return orderLineItemDocument;
        }
    }
}
