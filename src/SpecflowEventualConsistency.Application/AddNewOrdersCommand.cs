using System.Collections.Generic;
using System.Threading.Tasks;
using SpecflowEventualConsistency.Domain;
using SpecflowEventualConsistency.Infrastructure;

namespace SpecflowEventualConsistency.Application
{
    public class AddNewOrdersCommand
    {
        private readonly EventPublisher _eventPublisher;

        public AddNewOrdersCommand(EventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(IEnumerable<OrderDto> orderDtos)
        {
            foreach (var orderDto in orderDtos)
            {
                await _eventPublisher.PublishEvent(new NewOrderEvent(new Order
                {
                    CustomerId = orderDto.CustomerId,
                    ProductId = orderDto.ProductId,
                    Amount = orderDto.Amount
                })).ConfigureAwait(false);
            }
        }
    }
}