using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    [Table("Items")]
    public class Item
    {
        [Key]
        public int IdItem { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Foreign Key - Category (wymagana)
        public int IdCategory { get; set; }

        [ForeignKey("IdCategory")]
        public virtual Category Category { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal? Quantity { get; set; }

        [MaxLength(500)]
        public string? FotoUrl { get; set; }

        // Foreign Key - UnitOfMeasurement (opcjonalna)
        public int? IdUnitOfMeasurement { get; set; }

        [ForeignKey("IdUnitOfMeasurement")]
        public virtual UnitOfMeasurement? UnitOfMeasurement { get; set; }

        [MaxLength(50)]
        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;
    }
}