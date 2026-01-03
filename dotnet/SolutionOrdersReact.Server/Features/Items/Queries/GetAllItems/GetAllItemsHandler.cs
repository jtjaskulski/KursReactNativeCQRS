using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetAllItems
{
    /// <summary>
    /// Handler - logika biznesowa dla GetAllItemsQuery
    /// </summary>
    public class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, List<ItemDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllItemsHandler> _logger;

        public GetAllItemsHandler(ApplicationDbContext context, ILogger<GetAllItemsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ItemDto>> Handle(
            GetAllItemsQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie wszystkich produktów");

            var items = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.UnitOfMeasurement)
                .Where(i => i.IsActive)
                .OrderBy(i => i.Name)
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
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Pobrano {Count} produktów", items.Count);

            return items;
        }
    }
}