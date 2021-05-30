using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpecflowEventualConsistency.Application;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var orders = await _mediator.Send(new AllOrdersQuery.Query()).ConfigureAwait(false);
                return new OkObjectResult(orders);
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception.Message);
            }
		}

        [HttpGet("/{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            try
            {
                var order = await _mediator.Send(new OrderByIdQuery.Query(orderId)).ConfigureAwait(false);
                return new OkObjectResult(order);
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception.Message);
            } 
        }

        [HttpPost]
        public async Task<IActionResult> AddData(IEnumerable<Order> orders)
        {
            try
            {
                await _mediator.Publish(new NewOrdersCommand.Command(orders)).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception.Message);
            }
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteOrdersForCustomer(int customerId)
        {
            try
            {
                await _mediator.Publish(new DeleteOrdersForCustomerCommand.Command(customerId)).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, exception);
                return new BadRequestObjectResult(exception.Message);
            }
        }
    }
}