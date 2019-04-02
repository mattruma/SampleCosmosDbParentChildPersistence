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
