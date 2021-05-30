namespace SpecflowEventualConsistency.Domain
{
    public class Order : Entity
    {
        public int CustomerId { get; set; }
 
        public int ProductId { get; set; }
        
        public int Amount { get; set; }
    }
}