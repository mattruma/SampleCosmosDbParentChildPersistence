using Newtonsoft.Json;
using System;

namespace Demo2ConsoleApp.Data
{
    public interface IOrderDocument
    {
        [JsonProperty("orderId")]
        Guid OrderId { get; set; }

        [JsonProperty("type")]
        string Type { get; }
    }
}
