# Introduction
This is an example of ~~three~~ four different ways to store and access a document consisting of a parent and child relationship in a CosmosDB database.

For more information on data modeling in CosmosDB see https://docs.microsoft.com/en-us/azure/cosmos-db/modeling-data.

In most of the demo I provided examples of storing and retrieving the document with direct reads and writes to CosmosDB as well as an example that follows a more repository like the pattern, the latter allowing for better testing.

For an explanation of the repository pattern please see https://www.codecompiled.com/csharp/repository-pattern-in-csharp.

## Azure CosmosDB Configuration

This code example assumes that you have an Azure account.

In your Azure account navigate to CosmosDB and create a single CosmosDB account, in my example I created a CosmosDB account called `mjr-003-cosdb`.

Create ~~three~~ four separate databases with the following collections:

```
- demo1
  - orders
- demo2
  - orders
- demo3
  - orders
  - orderLineItems
- demo3
  - orders
```
  
You should set the `partitionKey` for all collections to `orderId`.

## Application Settings Configuration

Add an `appsettings.Development.json` filed to each of the console applications and update your configuration to look like the below along with the CosmosDB Primary Connection String:

```
{
  "CosmosDbDataStoreOptions": {
    "DatabaseId": "",
    "ConnectionString": ""
  }
}
```
That's it as far as setup goes!

## Examples
 
### Demo1ConsoleApp

This example demonstrates a single document that includes both the **Order** and the **Order Line Items**. 

This is an example of an embedded data model, read more at https://docs.microsoft.com/en-us/azure/cosmos-db/modeling-data.

Executing this example inserts an **Order** document, which includes both the **Order** and related **Order Line Items**, into the `orders` collection.

```
{
    "id": "9677a262-f98c-4af4-9797-c4e46d5093cd",
    "orderId": "9677a262-f98c-4af4-9797-c4e46d5093cd",
    "date": "2019-04-02T12:28:00.5491859Z",
    "lineItems": [
        {
            "orderLineItemId": "0f47ca15-e1fe-4489-872a-2efaf4c40b1a",
            "itemId": "fe14015e-acf1-4cb9-9c5f-a44639981f8a",
            "quantity": 5,
            "unitCost": 1.99,
            "totalAmount": 9.95
        },
        {
            "orderLineItemId": "c32e84c3-d356-4ed2-8d35-df96ccb557a7",
            "itemId": "e0a62961-eadc-4e1b-8f71-9d92a8d4426c",
            "quantity": 5,
            "unitCost": 1.99,
            "totalAmount": 9.95
        },
        {
            "orderLineItemId": "5239b8ad-f70a-4dae-a751-43dbd326fe3c",
            "itemId": "c9603a37-0e3b-4041-876d-4d49b5e1b503",
            "quantity": 5,
            "unitCost": 1.99,
            "totalAmount": 9.95
        }
    ]
}
```
*Order Document*

### Demo2ConsoleApp

This example demonstrates inserting an **Order** document into an `orders` collection, and the related **Order Line Item** documents (see below) also inserted into the `orders` collection. 

This is an example of a referenced data model, you can read more at https://docs.microsoft.com/en-us/azure/cosmos-db/modeling-data.

```
{
    "id": "9677a262-f98c-4af4-9797-c4e46d5093cd",
    "orderId": "9677a262-f98c-4af4-9797-c4e46d5093cd",
    "type": "Order",
    "date": "2019-04-02T12:28:00.5491859Z"
}
```
*Order Document*

```
{
    "id": "f9a1cf64-3b46-42f2-828e-22c298539090",
    "orderLineItemId": "f9a1cf64-3b46-42f2-828e-22c298539090",
    "orderId": "7cc90f0a-19f0-4dd1-9cef-2915f4c30164",
    "itemId": "65fa00bc-ca4e-4bad-9cfd-b0923df5fe33",
    "type": "OrderLineItem",
    "quantity": 5,
    "unitCost": 1.99,
    "totalAmount": 9.95
}
```
*Order Line Item Document*

The `type` property allows you to determine what the document is, e.g. **Order** or **Order Line Item**.

The benefit of doing it this way is you can quickly retrieve all the documents, **Order** and related **Order Line Items**, with a single call using the partition key, which has been set to `orderId`.

### Demo3ConsoleApp

This example demonstrates inserting an **Order** document into an `orders` collection, and the related **Order Line Item** documents inserted into an `orderLineItems` collection. 

This is also an example of a referenced data model, except the different documents are stored in differenct collections, read more at https://docs.microsoft.com/en-us/azure/cosmos-db/modeling-data.

```
{
    "id": "9677a262-f98c-4af4-9797-c4e46d5093cd",
    "orderId": "9677a262-f98c-4af4-9797-c4e46d5093cd",
    "date": "2019-04-02T12:28:00.5491859Z"
}
```
*Order Document*

```
{
    "id": "f9a1cf64-3b46-42f2-828e-22c298539090",
    "orderLineItemId": "f9a1cf64-3b46-42f2-828e-22c298539090",
    "orderId": "7cc90f0a-19f0-4dd1-9cef-2915f4c30164",
    "itemId": "65fa00bc-ca4e-4bad-9cfd-b0923df5fe33",
    "quantity": 5,
    "unitCost": 1.99,
    "totalAmount": 9.95
}
```
*Order Line Item Document*

### Demo4ConsoleApp

This example is similar to the Demo3ConsoleApp example, except **Order** and **Order Line Item** documents, which both implement the `IOrderDocument` interface,  are fetched with a single call to CosmosDB and then converted to the correct document type using a custom JSON converter.

```
using Newtonsoft.Json;
using System;

namespace Demo4ConsoleApp.Data
{
    [JsonConverter(typeof(OrderDocumentConverter))]
    public interface IOrderDocument
    {
        [JsonProperty("id")]
        Guid Id { get; set; }

        [JsonProperty("orderId")]
        Guid OrderId { get; set; }

        [JsonProperty("type")]
        string Type { get; set; }
    }
}
```
*IOrderDocument interface implemented by both Order and Order Line Item*

```
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Demo4ConsoleApp.Data
{
    public class OrderDocumentConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IOrderDocument);
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            IOrderDocument order;

            var type = jsonObject["type"].Value<string>();

            switch (type)
            {
                case OrderDocumentType.Order:
                    order = new OrderDocument();
                    break;
                case OrderDocumentType.OrderLineItem:
                    order = new OrderLineItemDocument();
                    break;
                default:
                    throw new NotSupportedException($"The type '{type}' is not supported.");
            }

            serializer.Populate(jsonObject.CreateReader(), order);

            return order;
        }
    }
}
```
*Custom JSON Converter to serialize based on document type, e.g. Order and Order Line Item*

# Conclusion

These are the most common ways I have found to persist parent and child related data in CosmosDB, there are probably other ways, maybe even better ways, if you have one, please share it with me. 