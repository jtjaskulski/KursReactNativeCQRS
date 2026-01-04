using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolutionOrdersReact.Server.Models
{
    /// <summary>
    /// Pracownik obsługujący zamówienia
    /// </summary>
    [Table("Workers")]
    public class Worker
    {
        // ========== PRIMARY KEY ==========

        [Key]
        public int IdWorker { get; set; }

        // ========== DANE OSOBOWE ==========

        /// <summary>
        /// Imię pracownika
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Nazwisko pracownika
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? LastName { get; set; }

        // ========== DANE KONTAKTOWE ==========

        /// <summary>
        /// Adres email służbowy
        /// </summary>
        [MaxLength(200)]
        public string? Email { get; set; }

        /// <summary>
        /// Numer telefonu służbowy
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
        /// Czy pracownik jest aktywny (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== NAVIGATION PROPERTIES ==========

        /// <summary>
        /// Kolekcja zamówień obsługiwanych przez pracownika (1:M)
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // ========== COMPUTED PROPERTIES ==========

        /// <summary>
        /// Pełne imię i nazwisko pracownika
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}