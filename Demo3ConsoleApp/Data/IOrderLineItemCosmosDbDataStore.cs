using Demo3ConsoleApp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo3ConsoleApp.Data
{
    public interface IOrderLineItemCosmosDbDataStore : ICosmosDbDataStore
    {
        Task<OrderLineItemDocument> AddAsync(
           Guid orderId,
           OrderLineItemDocument orderLineItem);

        Task<OrderLineItemDocument> DeleteByIdAsync(
           Guid orderId,
           Guid id);

        Task<OrderLineItemDocument> FetchByIdAsync(
           Guid orderId,
           Guid id);

        Task<IEnumerable<OrderLineItemDocument>> FetchListByOrderIdAsync(
           Guid orderId);

        Task<OrderLineItemDocument> UpdateByIdAsync(
            Guid orderId,
            Guid id,
            OrderLineItemDocument orderLineItem);
    }
}
