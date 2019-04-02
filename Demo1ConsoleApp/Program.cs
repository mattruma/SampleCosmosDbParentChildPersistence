using System;
using System.Threading.Tasks;
using Demo1ConsoleApp.Data;
using Demo1ConsoleApp.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Demo1ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleHelper.WriteLine("Demo 1", ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("Parent and children documents stored in a single collection", ConsoleColor.Yellow);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();

            // NOTE: Leveraging IOptions as this is available in Web Apps through DI, which is nice, but you could just use the CosmosDbDataStoreOptions without IOptions

            var cosmosDbDataStoreOptions =
                Options.Create(new CosmosDbDataStoreOptions(configuration));

            var cosmosClient =
                new CosmosClient(cosmosDbDataStoreOptions.Value.ConnectionString);
            var cosmosDatabase =
                cosmosClient.Databases[cosmosDbDataStoreOptions.Value.DatabaseId];

            OrderDocument orderDocument;

            ConsoleHelper.WriteLine("The following code is calling CosmosDB directly:", ConsoleColor.Blue);

            orderDocument = CreateOrderDocument();

            CreateOrderLineItemDocument(orderDocument);
            CreateOrderLineItemDocument(orderDocument);
            CreateOrderLineItemDocument(orderDocument);

            await WriteOrderDocumentAsync(
                cosmosDatabase,
                orderDocument);

            await ReadOrderDocumentAsync(
                cosmosDatabase,
                orderDocument.OrderId,
                orderDocument.Id);

            ConsoleHelper.WriteLine("The following code is following a repository pattern:", ConsoleColor.Blue);

            orderDocument = CreateOrderDocument();

            CreateOrderLineItemDocument(orderDocument);
            CreateOrderLineItemDocument(orderDocument);
            CreateOrderLineItemDocument(orderDocument);

            var orderCosmosDbDataStore =
                new OrderCosmosDbDataStore(cosmosDbDataStoreOptions);

            await orderCosmosDbDataStore.AddAsync(
                orderDocument);

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
            OrderDocument order)
        {
            ConsoleHelper.WriteLine("Creating order line item...");

            var orderLineItemDocument =
                new OrderLineItemDocument();

            orderLineItemDocument.Quantity = 5;
            orderLineItemDocument.ItemId = Guid.NewGuid();
            orderLineItemDocument.UnitCost = 1.99M;

            order.LineItems.Add(orderLineItemDocument);

            ConsoleHelper.WriteLine("Order line item created.");

            return orderLineItemDocument;
        }

        static async Task WriteOrderDocumentAsync(
            CosmosDatabase cosmosDatabase,
            OrderDocument orderDocument)
        {
            ConsoleHelper.WriteLine("Writing order to orders collection...");

            var cosmosContainer =
               cosmosDatabase.Containers["orders"];

            orderDocument = await cosmosContainer.Items.CreateItemAsync(
                 orderDocument.OrderId.ToString(),
                 orderDocument);

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderDocument, Formatting.Indented), 
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order written.");
        }

        static async Task<OrderDocument> ReadOrderDocumentAsync(
            CosmosDatabase cosmosDatabase,
            Guid orderId,
            Guid id)
        {
            ConsoleHelper.WriteLine("Reading order from orders collection...");

            var cosmosContainer =
               cosmosDatabase.Containers["orders"];

            var orderDocument =
                await cosmosContainer.Items.ReadItemAsync<OrderDocument>(
                    orderId.ToString(),
                    id.ToString());

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderDocument.Resource, Formatting.Indented), 
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order read.");

            return orderDocument;
        }
    }
}
