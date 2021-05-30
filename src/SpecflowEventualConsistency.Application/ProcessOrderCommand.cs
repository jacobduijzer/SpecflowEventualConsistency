using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.Application
{
    public class ProcessOrderCommand
    {
        public class Command : INotification
        {
            public readonly Order Order;

            public Command(Order order) =>
                Order = order;
        }

        public class Handler : INotificationHandler<Command>
        {
            private readonly IRepository<Order> _orderRepository;

            public Handler(IRepository<Order> orderRepository) =>
                _orderRepository = orderRepository;

            public async Task Handle(Command notification, CancellationToken cancellationToken)
            {
                // Fake some long process
                await Task.Delay(5000).ConfigureAwait(false);

                await _orderRepository.Add(notification.Order).ConfigureAwait(false);
            }
        }
    }
}