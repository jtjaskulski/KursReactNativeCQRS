using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Pozycja zamówienia - rekord Detail
    /// </summary>
    [Table("OrderItems")]
    public class OrderItem
    {
        // ========== PRIMARY KEY ==========
        [Key]
        public int IdOrderItem { get; set; }

        // ========== RELACJA Z ORDER (wymagana, Master) ==========

        /// <summary>
        /// FK do zamówienia (NOT NULL - pozycja musi należeć do zamówienia)
        /// </summary>
        public int IdOrder { get; set; }

        /// <summary>
        /// Navigation property do zamówienia (Master)
        /// </summary>
        public virtual Order Order { get; set; } = null!;

        // ========== RELACJA Z ITEM (wymagana) ==========

        /// <summary>
        /// FK do produktu
        /// </summary>
        public int IdItem { get; set; }

        /// <summary>
        /// Navigation property do produktu
        /// </summary>
        public virtual Item Item { get; set; } = null!;

        // ========== DANE POZYCJI ==========

        /// <summary>
        /// Ilość zamówiona
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Cena jednostkowa W MOMENCIE ZAMÓWIENIA
        /// (kopia z Item.Price - nie zmienia się gdy cena produktu się zmieni)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// Czy pozycja jest aktywna (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== COMPUTED PROPERTIES ==========

        /// <summary>
        /// Wartość pozycji (ilość × cena)
        /// </summary>
        [NotMapped]
        public decimal LineTotal => (Quantity ?? 0) * (UnitPrice ?? 0);
    }
}