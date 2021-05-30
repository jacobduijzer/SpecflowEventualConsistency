using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpecflowEventualConsistency.Application;

namespace SpecflowEventualConsistency.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class DataController : ControllerBase
    {
        private readonly AddNewOrdersCommand _addNewOrdersCommand;

        public DataController(AddNewOrdersCommand addNewOrdersCommand)
        {
            _addNewOrdersCommand = addNewOrdersCommand;
        }

        [HttpPost]
        public async Task<IActionResult> AddData(IEnumerable<OrderDto> orders)
        {
            try
            {
                await _addNewOrdersCommand.Handle(orders).ConfigureAwait(false);
                return new OkResult();
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(exception.Message);
            }
        }
    }
}