using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Zamówienie - rekord Master
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        // ========== PRIMARY KEY ==========
        [Key]
        public int IdOrder { get; set; }

        // ========== DANE PODSTAWOWE ==========

        /// <summary>
        /// Data utworzenia zamówienia
        /// </summary>
        public DateTime? DataOrder { get; set; }

        /// <summary>
        /// Notatki/uwagi do zamówienia
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Planowana data dostawy
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Czy zamówienie jest aktywne (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== RELACJA Z CLIENT (1:M, opcjonalna) ==========

        /// <summary>
        /// FK do klienta (nullable = zamówienie anonimowe)
        /// </summary>
        public int? IdClient { get; set; }

        /// <summary>
        /// Navigation property do klienta
        /// </summary>
        public virtual Client? Client { get; set; }

        // ========== RELACJA Z WORKER (1:M, opcjonalna) ==========

        /// <summary>
        /// FK do pracownika obsługującego
        /// </summary>
        public int? IdWorker { get; set; }

        /// <summary>
        /// Navigation property do pracownika
        /// </summary>
        public virtual Worker? Worker { get; set; }

        // ========== RELACJA Z ORDER_ITEMS (1:M, Master-Detail) ==========

        /// <summary>
        /// Kolekcja pozycji zamówienia (Detail)
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // ========== COMPUTED PROPERTIES (nie mapowane do bazy) ==========

        /// <summary>
        /// Suma wartości zamówienia
        /// </summary>
        [NotMapped]
        public decimal TotalAmount => OrderItems
            .Where(oi => oi.IsActive)
            .Sum(oi => (oi.UnitPrice ?? 0) * (oi.Quantity ?? 0));

        /// <summary>
        /// Liczba pozycji w zamówieniu
        /// </summary>
        [NotMapped]
        public int ItemCount => OrderItems.Count(oi => oi.IsActive);
    }
}