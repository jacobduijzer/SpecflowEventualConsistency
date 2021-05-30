using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Polly;
using Refit;
using SpecflowEventualConsistency.Domain;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SpecflowEventualConsistency.Specs
{
    [Binding]
    public class ProcessNewOrdersSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ITestOutputHelper _output;
        private readonly IOrderApi _api;

        public ProcessNewOrdersSteps(ScenarioContext scenarioContext, ITestOutputHelper output)
        {
            _scenarioContext = scenarioContext;
            _output = output;
            _api = RestService.For<IOrderApi>("http://localhost");
        }
        
        [BeforeScenario]
        public async Task SetupTestUsers()
        {
            await _api.DeleteOrdersForCustomer(1);
        }

        [Given("the user has these unprocessed orders")]
        public void GivenTheUserHasTheseUnprocessedOrders(Table orders)
        {
           _scenarioContext.Add("ORDERS", orders.CreateSet<Order>()); 
        }

        [When("he sends this orders to the api")]
        public async Task WhenHeSendsThisOrdersToTheApi()
        {
            await _api.AddOrders(_scenarioContext.Get<IEnumerable<Order>>("ORDERS"));
        }

        [Then("the orders will be processed and added to the database")]
        public async Task ThenTheOrdersWillBeProcessedAndAddedToTheDatabase()
        {
            var newOrders = _scenarioContext.Get<IEnumerable<Order>>("ORDERS");
            var retry = Policy
                .Handle<XunitException>()
                .WaitAndRetryAsync( 3, retryAttempt => TimeSpan.FromSeconds(3), (exception, timeSpan, context) => {
                   _output.WriteLine($"RetryException: {exception.Message}"); 
                });
            await retry.ExecuteAsync(async () =>
            {
                var processedOrders = await _api.GetAllOrders();
                processedOrders.Count().Should().Be(newOrders.Count()); 
            });
        }
    }
}