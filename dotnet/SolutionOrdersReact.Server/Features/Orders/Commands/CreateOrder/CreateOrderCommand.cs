using MediatR;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.CreateOrder
{
    /// <summary>
    /// Command do tworzenia nowego zamówienia z pozycjami
    /// </summary>
    public class CreateOrderCommand : IRequest<int>
    {
        // Dane zamówienia
        public int? IdClient { get; set; }
        public int? IdWorker { get; set; }
        public string? Notes { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // Lista pozycji (Detail)
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO dla pojedynczej pozycji przy tworzeniu
    /// </summary>
    public class CreateOrderItemDto
    {
        public int IdItem { get; set; }
        public decimal Quantity { get; set; }
        // UnitPrice pobierzemy z Item przy zapisie
    }
}