using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SpecflowEventualConsistency.Domain;
using SpecflowEventualConsistency.Infrastructure;

namespace SpecflowEventualConsistency.Application
{
    public class NewOrdersCommand
    {
        public class Command : INotification
        {
            public readonly IEnumerable<Order> Orders;

            public Command(IEnumerable<Order> orders) =>
                Orders = orders;
        }
        
        public class Handler : INotificationHandler<Command>
        {
            private readonly EventPublisher _eventPublisher;

            public Handler(EventPublisher eventPublisher) => 
                _eventPublisher = eventPublisher;

            public async Task Handle(Command notification, CancellationToken cancellationToken)
            {
                foreach (var order in notification.Orders)
                    await _eventPublisher.PublishEvent(new NewOrderEvent(order)).ConfigureAwait(false);
            }
        }
    }
}