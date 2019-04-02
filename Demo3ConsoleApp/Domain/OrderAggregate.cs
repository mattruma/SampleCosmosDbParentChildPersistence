using Demo3ConsoleApp.Data;
using System.Collections.Generic;

namespace Demo3ConsoleApp.Domain
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
