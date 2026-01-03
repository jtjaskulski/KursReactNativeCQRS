using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Categories.Queries
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllCategoriesHandler> _logger;

        public GetAllCategoriesHandler(ApplicationDbContext context, ILogger<GetAllCategoriesHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> Handle(
            GetAllCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie wszystkich kategorii");

            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    IdCategory = c.IdCategory,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Pobrano {Count} kategorii", categories.Count);

            return categories;
        }
    }
}