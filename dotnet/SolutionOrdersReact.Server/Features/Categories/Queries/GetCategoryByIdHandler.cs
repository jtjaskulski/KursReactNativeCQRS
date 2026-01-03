using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Categories.Queries
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetCategoryByIdHandler> _logger;

        public GetCategoryByIdHandler(ApplicationDbContext context, ILogger<GetCategoryByIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CategoryDto?> Handle(
            GetCategoryByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie kategorii o ID: {Id}", request.Id);

            var category = await _context.Categories
                .Where(c => c.IdCategory == request.Id)
                .Select(c => new CategoryDto
                {
                    IdCategory = c.IdCategory,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (category == null)
            {
                _logger.LogWarning("Kategoria o ID {Id} nie została znaleziona", request.Id);
            }

            return category;
        }
    }
}