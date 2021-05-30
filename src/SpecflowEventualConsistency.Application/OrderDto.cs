using System;

namespace SpecflowEventualConsistency.Application
{
    public class OrderDto
    {
        public int ProductId { get; set; }
        
        public int CustomerId { get; set; }
        
        public int Amount { get; set; }
    }
}