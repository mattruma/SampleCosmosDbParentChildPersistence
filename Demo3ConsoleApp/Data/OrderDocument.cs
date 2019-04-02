using Newtonsoft.Json;
using System;

namespace Demo3ConsoleApp.Data
{
    public class OrderDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } // Every document needs to have an Id

        [JsonProperty("orderId")]
        public Guid OrderId { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        public OrderDocument()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
