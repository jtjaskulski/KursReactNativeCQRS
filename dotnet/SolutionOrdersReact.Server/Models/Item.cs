using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Produkt/towar w systemie
    /// </summary>
    [Table("Items")]
    public class Item
    {
        // ========== PRIMARY KEY ==========

        [Key]
        public int IdItem { get; set; }

        // ========== DANE PODSTAWOWE ==========

        /// <summary>
        /// Nazwa produktu
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }

        /// <summary>
        /// Opis produktu
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Kod produktu (SKU)
        /// </summary>
        [MaxLength(50)]
        public string? Code { get; set; }

        // ========== CENA I ILOŚĆ ==========

        /// <summary>
        /// Cena jednostkowa produktu
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Ilość w magazynie
        /// </summary>
        [Column(TypeName = "decimal(18,3)")]
        public decimal? Quantity { get; set; }

        // ========== MULTIMEDIA ==========

        /// <summary>
        /// URL do zdjęcia produktu
        /// </summary>
        [MaxLength(500)]
        public string? FotoUrl { get; set; }

        // ========== STATUS ==========

        /// <summary>
        /// Czy produkt jest aktywny (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== RELACJA Z CATEGORY (wymagana, 1:M) ==========

        /// <summary>
        /// FK do kategorii (NOT NULL - produkt musi mieć kategorię)
        /// </summary>
        public int IdCategory { get; set; }

        /// <summary>
        /// Navigation property do kategorii
        /// </summary>
        [ForeignKey("IdCategory")]
        public virtual Category Category { get; set; } = null!;

        // ========== RELACJA Z UNIT OF MEASUREMENT (opcjonalna, 1:M) ==========

        /// <summary>
        /// FK do jednostki miary (nullable)
        /// </summary>
        public int? IdUnitOfMeasurement { get; set; }

        /// <summary>
        /// Navigation property do jednostki miary
        /// </summary>
        [ForeignKey("IdUnitOfMeasurement")]
        public virtual UnitOfMeasurement? UnitOfMeasurement { get; set; }

        // ========== NAVIGATION PROPERTIES ==========

        /// <summary>
        /// Kolekcja pozycji zamówień z tym produktem (1:M)
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

