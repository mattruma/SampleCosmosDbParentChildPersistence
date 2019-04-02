using Demo3ConsoleApp.Data;
using Demo3ConsoleApp.Domain;
using Demo3ConsoleApp.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo3ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleHelper.WriteLine("Demo 3", ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("Parent documents and children documents stored in separate collections");

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

            await WriteOrderDocumentAsync(
                cosmosDatabase,
                orderDocument);

            await ReadOrderDocumentAsync(
                cosmosDatabase, 
                orderDocument.OrderId,
                orderDocument.Id);

            for (var index = 0; index < 3; index++)
            {
                var orderLineItemDocument =
                    CreateOrderLineItemDocument(orderDocument);

                await WriteOrderLineItemDocumentAsync(
                    cosmosDatabase,
                    orderLineItemDocument);
            }

            await ReadOrderLineItemDocumentsAsync(
                cosmosDatabase, 
                orderDocument.OrderId);

            ConsoleHelper.WriteLine("The following code is following a repository pattern:", ConsoleColor.Blue);

            orderDocument = CreateOrderDocument();

            var orderCosmosDbDataStore =
                new OrderCosmosDbDataStore(cosmosDbDataStoreOptions);

            await orderCosmosDbDataStore.AddAsync(
                orderDocument);

            OrderLineItemDocument orderlineItemDocument;

            var orderLineItemCosmosDbDataStore =
                new OrderLineItemCosmosDbDataStore(cosmosDbDataStoreOptions);

            orderlineItemDocument =
                CreateOrderLineItemDocument(orderDocument);

            await orderLineItemCosmosDbDataStore.AddAsync(
                orderDocument.Id,
                orderlineItemDocument);

            orderlineItemDocument =
                CreateOrderLineItemDocument(orderDocument);

            await orderLineItemCosmosDbDataStore.AddAsync(
                orderDocument.Id,
                orderlineItemDocument);

            orderlineItemDocument =
                CreateOrderLineItemDocument(orderDocument);

            await orderLineItemCosmosDbDataStore.AddAsync(
                orderDocument.Id,
                orderlineItemDocument);

            var orderAggregateFetcher =
                new OrderAggregateFetcher(
                    orderCosmosDbDataStore,
                    orderLineItemCosmosDbDataStore);

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

        static async Task WriteOrderDocumentAsync(
            CosmosDatabase cosmosDatabase,
            OrderDocument orderDocument)
        {
            ConsoleHelper.WriteLine("Writing order to orders collection...");

            var cosmosContainer =
               cosmosDatabase.Containers["orders"];

            await cosmosContainer.Items.CreateItemAsync(
                orderDocument.OrderId.ToString(),
                orderDocument);

            ConsoleHelper.WriteLine("Order written.");
        }

        static async Task WriteOrderLineItemDocumentAsync(
            CosmosDatabase cosmosDatabase,
            OrderLineItemDocument orderLineItemDocument)
        {
            ConsoleHelper.WriteLine("Writing order line item to orderLineItems collection...");

            var cosmosContainer =
               cosmosDatabase.Containers["orderLineItems"];

            await cosmosContainer.Items.CreateItemAsync(
                orderLineItemDocument.OrderId.ToString(),
                orderLineItemDocument);

            ConsoleHelper.WriteLine("Order line item written.");
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

        static async Task<IEnumerable<OrderLineItemDocument>> ReadOrderLineItemDocumentsAsync(
            CosmosDatabase cosmosDatabase,
            Guid orderId)
        {
            ConsoleHelper.WriteLine("Reading order line items from order line items collection...");

            var cosmosContainer =
               cosmosDatabase.Containers["orderLineItems"];

            var query =
                $"SELECT * FROM oli";

            var queryDefinition =
                new CosmosSqlQueryDefinition(query);

            var orderLineItemDocuments =
                cosmosContainer.Items.CreateItemQuery<OrderLineItemDocument>(
                    queryDefinition,
                    orderId.ToString());

            var orderLineItemDocumentList = 
                new List<OrderLineItemDocument>();

            while (orderLineItemDocuments.HasMoreResults)
            {
                orderLineItemDocumentList.AddRange(
                    await orderLineItemDocuments.FetchNextSetAsync());
            };

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderLineItemDocumentList, Formatting.Indented),
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order line items read.");

            return orderLineItemDocumentList;
        }
    }
}
