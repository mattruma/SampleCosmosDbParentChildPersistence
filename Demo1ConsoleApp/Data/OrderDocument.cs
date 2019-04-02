using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Demo1ConsoleApp.Data
{
    public class OrderDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } // Every document needs to have an Id

        [JsonProperty("orderId")]
        public Guid OrderId { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("lineItems")]
        public ICollection<OrderLineItemDocument> LineItems { get; set; }

        public OrderDocument()
        {
            this.Id = Guid.NewGuid();
            this.LineItems = new List<OrderLineItemDocument>();
        }
    }
}
