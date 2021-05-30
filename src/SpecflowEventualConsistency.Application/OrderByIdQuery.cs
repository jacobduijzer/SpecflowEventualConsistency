using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.Application
{
    public class OrderByIdQuery
    {
        public class Query : IRequest<Order>
        {
            public readonly int OrderId;

            public Query(int orderId) =>
                OrderId = orderId;
        }

        public class Handler : IRequestHandler<Query, Order>
        {
            private readonly IRepository<Order> _orderRepository;

            public Handler(IRepository<Order> orderRepository) =>
                _orderRepository = orderRepository;

            public async Task<Order> Handle(Query request, CancellationToken cancellationToken)
            {
                var orders = await _orderRepository.Get(x => x.Id == request.OrderId).ConfigureAwait(false);
                return orders.First();
            }
        }
    }
}