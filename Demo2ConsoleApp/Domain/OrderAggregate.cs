using Demo2ConsoleApp.Data;
using System.Collections.Generic;

namespace Demo2ConsoleApp.Domain
{
    public class OrderAggregate
    {
        public OrderDocument Order { get; set; }
        public IList<OrderLineItemDocument> OrderLineItems { get; set; }

        public OrderAggregate()
        {
            this.OrderLineItems = new List<OrderLineItemDocument>();
        }
    }
}
