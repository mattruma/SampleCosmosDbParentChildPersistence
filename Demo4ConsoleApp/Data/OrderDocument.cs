using Newtonsoft.Json;
using System;

namespace Demo4ConsoleApp.Data
{
    public class OrderDocument : IOrderDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } // Every document needs to have an Id

        [JsonProperty("orderId")]
        public Guid OrderId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } // This property is used to support multiple document types in a single collection

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        public OrderDocument()
        {
            this.Id = Guid.NewGuid();
            this.Type = OrderDocumentType.Order;
        }
    }
}
