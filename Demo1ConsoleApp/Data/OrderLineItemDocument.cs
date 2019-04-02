using Newtonsoft.Json;
using System;

namespace Demo1ConsoleApp.Data
{
    public class OrderLineItemDocument
    {
        [JsonProperty("orderLineItemId")]
        public Guid OrderLineItemId { get; set; }

        [JsonProperty("itemId")]
        public Guid ItemId { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [JsonProperty("unitCost")]
        public decimal UnitCost { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount => this.Quantity * this.UnitCost;

        public OrderLineItemDocument()
        {
            this.OrderLineItemId = Guid.NewGuid();
        }
    }
}
