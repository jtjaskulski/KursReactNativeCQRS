using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Kategoria produktów
    /// </summary>
    [Table("Categories")]
    public class Category
    {
        // ========== PRIMARY KEY ==========

        [Key]
        public int IdCategory { get; set; }

        // ========== DANE PODSTAWOWE ==========

        /// <summary>
        /// Nazwa kategorii
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// Opis kategorii
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Czy kategoria jest aktywna (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== NAVIGATION PROPERTIES ==========

        /// <summary>
        /// Kolekcja produktów w tej kategorii (1:M)
        /// </summary>
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}

