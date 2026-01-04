using MediatR;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommand : IRequest<Unit>
    {
        public int IdOrder { get; set; }
        public int? IdClient { get; set; }
        public int? IdWorker { get; set; }
        public string? Notes { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public bool IsActive { get; set; }

        // Aktualizacja pozycji - pełna wymiana
        public List<UpdateOrderItemDto> Items { get; set; } = new();
    }

    public class UpdateOrderItemDto
    {
        public int? IdOrderItem { get; set; }  // null = nowa pozycja
        public int IdItem { get; set; }
        public decimal Quantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}