using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetItemById
{
    /// <summary>
    /// Handler - pobiera pojedynczy produkt po ID
    /// </summary>
    public class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, ItemDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetItemByIdHandler> _logger;

        public GetItemByIdHandler(ApplicationDbContext context, ILogger<GetItemByIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ItemDto?> Handle(
            GetItemByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie produktu o ID: {Id}", request.Id);

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.UnitOfMeasurement)
                .Where(i => i.IdItem == request.Id)
                .Select(i => new ItemDto
                {
                    IdItem = i.IdItem,
                    Name = i.Name,
                    Description = i.Description,
                    IdCategory = i.IdCategory,
                    CategoryName = i.Category.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    IdUnitOfMeasurement = i.IdUnitOfMeasurement,
                    UnitName = i.UnitOfMeasurement != null ? i.UnitOfMeasurement.Name : null,
                    Code = i.Code,
                    IsActive = i.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (item == null)
            {
                _logger.LogWarning("Produkt o ID {Id} nie został znaleziony", request.Id);
            }

            return item;
        }
    }
}
