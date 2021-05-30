using System.Threading.Tasks;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.Application
{
    public class ProcessOrderCommand
    {
        private readonly IRepository<Order> _orderRepository;

        public ProcessOrderCommand(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(Order order) 
        {
            // Fake some long process
            await Task.Delay(5000).ConfigureAwait(false);

            await _orderRepository.Add(order).ConfigureAwait(false);
        }
    }
}