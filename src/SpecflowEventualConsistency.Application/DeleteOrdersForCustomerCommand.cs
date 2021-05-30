using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.Application
{
    public class DeleteOrdersForCustomerCommand
    {
        public class Command : INotification
        {
            public readonly int CustomerId;

            public Command(int customerId) =>
                CustomerId = customerId;
        }

        public class Handler : INotificationHandler<Command>
        {
            private readonly IRepository<Order> _orderRepository;

            public Handler(IRepository<Order> orderRepository) => 
                _orderRepository = orderRepository;

            public async Task Handle(Command notification, CancellationToken cancellationToken) =>
                await _orderRepository.Delete(x => x.CustomerId == notification.CustomerId).ConfigureAwait(false);
        }
    }
}