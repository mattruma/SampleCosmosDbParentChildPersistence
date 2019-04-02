using CosmosDbParentChildDemo.Demo2ConsoleApp.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CosmosDbParentChildDemo.Demo2ConsoleApp.Helpers
{
    // https://skrift.io/articles/archive/bulletproof-interface-deserialization-in-jsonnet/

    public class OrderAggregateConverter : JsonConverter
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
            IOrderDocument orderDocument;

            var type = jsonObject["type"].Value<string>();

            switch (type)
            {
                case "Order":
                    orderDocument = new OrderDocument();
                    break;
                case "OrderLineItem":
                    orderDocument = new OrderLineItemDocument();
                    break;
                default:
                    throw new NotSupportedException($"The type '{type}' is not supported.");
            }

            serializer.Populate(jsonObject.CreateReader(), orderDocument);

            return orderDocument;
        }
    }
}
