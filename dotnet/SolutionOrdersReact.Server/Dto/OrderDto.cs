namespace SolutionOrdersReact.Server.Dto
{
    /// <summary>
    /// DTO dla zamówienia (lista i szczegóły)
    /// </summary>
    public class OrderDto
    {
        public int IdOrder { get; set; }
        public DateTime? DataOrder { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }

        // Dane klienta (z relacji)
        public int? IdClient { get; set; }
        public string? ClientName { get; set; }
        public string? ClientPhone { get; set; }

        // Dane pracownika (z relacji)
        public int? IdWorker { get; set; }
        public string? WorkerName { get; set; }

        // Pozycje zamówienia (zagnieżdżona lista)
        public List<OrderItemDto> Items { get; set; } = new();

        // Obliczone wartości
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}