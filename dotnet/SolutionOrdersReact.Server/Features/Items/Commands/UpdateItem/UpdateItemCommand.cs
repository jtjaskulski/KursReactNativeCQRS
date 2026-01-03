using MediatR;

namespace SolutionOrdersReact.Server.Features.Items.Commands.UpdateItem
{
    /// <summary>
    /// Command - aktualizuje produkt
    /// </summary>
    public class UpdateItemCommand : IRequest<Unit>
    {
        public int IdItem { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int IdCategory { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public string? FotoUrl { get; set; }
        public int? IdUnitOfMeasurement { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
    }
}