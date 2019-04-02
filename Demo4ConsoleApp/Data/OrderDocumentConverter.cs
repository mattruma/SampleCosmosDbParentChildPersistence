using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Demo4ConsoleApp.Data
{
    // https://skrift.io/articles/archive/bulletproof-interface-deserialization-in-jsonnet/
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
