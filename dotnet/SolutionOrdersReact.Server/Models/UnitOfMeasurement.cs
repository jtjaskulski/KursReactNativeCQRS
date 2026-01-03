using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    [Table("UnitOfMeasurements")]
    public class UnitOfMeasurement
    {
        [Key]
        public int IdUnitOfMeasurement { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property - jeden-do-wielu z Item
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
