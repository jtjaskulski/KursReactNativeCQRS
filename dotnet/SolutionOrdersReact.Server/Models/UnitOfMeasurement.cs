using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Jednostka miary produktów
    /// </summary>
    [Table("UnitOfMeasurements")]
    public class UnitOfMeasurement
    {
        // ========== PRIMARY KEY ==========

        [Key]
        public int IdUnitOfMeasurement { get; set; }

        // ========== DANE PODSTAWOWE ==========

        /// <summary>
        /// Nazwa/symbol jednostki (np. "szt", "kg", "l")
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// Pełna nazwa jednostki (np. "Sztuki", "Kilogramy")
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// Czy jednostka jest aktywna (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== NAVIGATION PROPERTIES ==========

        /// <summary>
        /// Kolekcja produktów używających tej jednostki (1:M)
        /// </summary>
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}

