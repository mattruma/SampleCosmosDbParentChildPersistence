using Newtonsoft.Json;
using System;

namespace Demo2ConsoleApp.Data
{
    public class OrderLineItemDocument : IOrderDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } // Every document needs to have an Id
   
        [JsonProperty("orderLineItemId")]
        public Guid OrderLineItemId { get; set; }

        [JsonProperty("orderId")]
        public Guid OrderId { get; set; }  // This property is used to support multiple document types in a single collection and storing the order line items in a separate collection

        [JsonProperty("itemId")]
        public Guid ItemId { get; set; }
      
        [JsonProperty("type")]
        public string Type => "OrderLineItem"; // This property is used to support multiple document types in a single collection
      
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [JsonProperty("unitCost")]
        public decimal UnitCost { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount => this.Quantity * this.UnitCost;

        public OrderLineItemDocument()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
