using Demo2ConsoleApp.Data;
using Demo2ConsoleApp.Domain;
using Demo2ConsoleApp.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo2ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleHelper.WriteLine("Demo 2", ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("Parent documents and children documents stored in a single collection", ConsoleColor.Yellow);

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

            for (var index = 0; index < 3; index++)
            {
                var orderLineItemDocument =
                    CreateOrderLineItemDocument(orderDocument);

                await WriteOrderLineItemDocumentAsync(
                    cosmosDatabase,
                    orderLineItemDocument);
            }

            await ReadOrderDocumentsAsync(
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
            ConsoleHelper.WriteLine("Writing order line item to orders collection...");

            var cosmosContainer =
               cosmosDatabase.Containers["orders"];

            await cosmosContainer.Items.CreateItemAsync(
                orderLineItemDocument.OrderId.ToString(),
                orderLineItemDocument);

            ConsoleHelper.WriteLine("Order line item written.");
        }

        static async Task<OrderAggregate> ReadOrderDocumentsAsync(
            CosmosDatabase cosmosDatabase,
            Guid orderId)
        {
            ConsoleHelper.WriteLine("Reading orders and order line items from orders collection...");

            var cosmosContainer =
               cosmosDatabase.Containers["orders"];

            var query =
                $"SELECT * FROM o";

            var queryDefinition =
                new CosmosSqlQueryDefinition(query);

            // Reading all the documents based on the partition key which is order id, reading
            // as a dynamic so any document type can be read

            var orderDocuments =
                cosmosContainer.Items.CreateItemQuery<dynamic>(
                    queryDefinition,
                    orderId.ToString());

            var orderDocumentList =
                new List<dynamic>();

            while (orderDocuments.HasMoreResults)
            {
                orderDocumentList.AddRange(
                    await orderDocuments.FetchNextSetAsync());
            };

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderDocumentList, Formatting.Indented),
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Orders and order line items read.");

            var orderAggregate =
                new OrderAggregate();

            var orderDocument =
                orderDocumentList.First(
                    x => x.type == "Order");

            orderAggregate.Order =
                JsonConvert.DeserializeObject<OrderDocument>(
                    orderDocument.ToString());

            var orderLineItemDocuments =
                orderDocumentList
                    .Where(x => x.type == "OrderLineItem")
                    .ToList();

            foreach (var orderLineItemDocument in orderLineItemDocuments)
            {
                orderAggregate.OrderLineItems.Add(
                    JsonConvert.DeserializeObject<OrderLineItemDocument>(
                        orderLineItemDocument.ToString()));
            }

            ConsoleHelper.WriteLine(
                JsonConvert.SerializeObject(orderAggregate, Formatting.Indented),
                ConsoleColor.Green);

            ConsoleHelper.WriteLine("Order aggregate built.");

            return orderAggregate;
        }
    }
}
