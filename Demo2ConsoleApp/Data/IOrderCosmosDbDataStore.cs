using Demo2ConsoleApp.Helpers;
using System;
using System.Threading.Tasks;

namespace Demo2ConsoleApp.Data
{
    public interface IOrderCosmosDbDataStore : ICosmosDbDataStore
    {
        Task<OrderDocument> AddAsync(
            OrderDocument order);

        Task<OrderDocument> DeleteByIdAsync(
            Guid id);

        Task<OrderDocument> FetchByIdAsync(
            Guid id);

       Task<OrderDocument> UpdateByIdAsync(
            Guid id,
            OrderDocument order);
    }
}
