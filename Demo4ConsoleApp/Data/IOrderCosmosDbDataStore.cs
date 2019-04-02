using Demo4ConsoleApp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo4ConsoleApp.Data
{
    public interface IOrderCosmosDbDataStore : ICosmosDbDataStore
    {
        Task<IOrderDocument> AddAsync(
            Guid orderId,
            IOrderDocument order);

        Task<IOrderDocument> DeleteByIdAsync(
            Guid orderId,
            Guid id);

        Task<IOrderDocument> FetchByIdAsync(
            Guid orderId,
            Guid id);

        Task<IEnumerable<IOrderDocument>> FetchListByOrderIdAsync(
            Guid orderId);

        Task<IOrderDocument> UpdateByIdAsync(
             Guid orderId,
             Guid id,
             IOrderDocument order);
    }
}
