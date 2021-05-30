using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.Specs
{
    public interface IOrderApi
    {
        [Get("/Order")]
        Task<IEnumerable<Order>> GetAllOrders();

        [Post("/Order")]
        Task AddOrders(IEnumerable<Order> orders);

        [Delete("/Order/{customerId}")]
        Task DeleteOrdersForCustomer(int customerId);
    }
}