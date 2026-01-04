using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Klient składający zamówienia
    /// </summary>
    [Table("Clients")]
    public class Client
    {
        // ========== PRIMARY KEY ==========

        [Key]
        public int IdClient { get; set; }

        // ========== DANE OSOBOWE ==========

        /// <summary>
        /// Imię klienta
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Nazwisko klienta
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? LastName { get; set; }

        // ========== DANE KONTAKTOWE ==========

        /// <summary>
        /// Adres email
        /// </summary>
        [MaxLength(200)]
        public string? Email { get; set; }

        /// <summary>
        /// Numer telefonu
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        // ========== DANE ADRESOWE ==========

        /// <summary>
        /// Adres (ulica, numer)
        /// </summary>
        [MaxLength(500)]
        public string? Address { get; set; }

        /// <summary>
        /// Miasto
        /// </summary>
        [MaxLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// Kod pocztowy
        /// </summary>
        [MaxLength(20)]
        public string? PostalCode { get; set; }

        // ========== STATUS ==========

        /// <summary>
        /// Czy klient jest aktywny (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== NAVIGATION PROPERTIES ==========

        /// <summary>
        /// Kolekcja zamówień klienta (1:M)
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // ========== COMPUTED PROPERTIES ==========

        /// <summary>
        /// Pełne imię i nazwisko klienta
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}