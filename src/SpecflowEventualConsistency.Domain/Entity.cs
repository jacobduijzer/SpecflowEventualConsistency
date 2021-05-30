using System.ComponentModel.DataAnnotations;

namespace SpecflowEventualConsistency.Domain
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}