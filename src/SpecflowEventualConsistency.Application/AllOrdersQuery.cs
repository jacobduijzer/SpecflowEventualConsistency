using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.Application
{
    public class AllOrdersQuery
    {
        public class Query : IRequest<IEnumerable<Order>>
        {
            
        }
        
        public class Handler : IRequestHandler<Query, IEnumerable<Order>>
        {
            private readonly IRepository<Order> _orderRepository;

            public Handler(IRepository<Order> orderRepository) => 
                _orderRepository = orderRepository;

            public async Task<IEnumerable<Order>> Handle(Query request, CancellationToken cancellationToken) =>
                await _orderRepository.Get(x => true).ConfigureAwait(false);
        }
    }
}
