using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Categories.Commands
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteCategoryHandler> _logger;

        public DeleteCategoryHandler(ApplicationDbContext context, ILogger<DeleteCategoryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            DeleteCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.IdCategory == request.IdCategory, cancellationToken);

            if (category == null)
            {
                _logger.LogError("Kategoria o ID {IdCategory} nie istnieje", request.IdCategory);
                throw new KeyNotFoundException($"Kategoria o ID {request.IdCategory} nie istnieje");
            }

            _logger.LogInformation("Usuwanie kategorii o ID: {IdCategory}", request.IdCategory);

            category.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Usunięto kategorię o ID: {IdCategory}", request.IdCategory);

            return Unit.Value;
        }
    }
}