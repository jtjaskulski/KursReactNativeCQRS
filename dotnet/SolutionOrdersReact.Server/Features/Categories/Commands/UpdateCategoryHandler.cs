using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Categories.Commands
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateCategoryHandler> _logger;

        public UpdateCategoryHandler(ApplicationDbContext context, ILogger<UpdateCategoryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.IdCategory == request.IdCategory, cancellationToken);

            if (category == null)
            {
                _logger.LogError("Kategoria o ID {IdCategory} nie istnieje", request.IdCategory);
                throw new KeyNotFoundException($"Kategoria o ID {request.IdCategory} nie istnieje");
            }

            _logger.LogInformation("Aktualizacja kategorii o ID: {IdCategory}", request.IdCategory);

            category.Name = request.Name;
            category.Description = request.Description;
            category.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Zaktualizowano kategorię o ID: {IdCategory}", request.IdCategory);

            return Unit.Value;
        }
    }
}