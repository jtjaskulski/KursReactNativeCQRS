using MediatR;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.Categories.Commands
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateCategoryHandler> _logger;

        public CreateCategoryHandler(ApplicationDbContext context, ILogger<CreateCategoryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tworzenie nowej kategorii: {Name}", request.Name);

            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Utworzono kategorię o ID: {IdCategory}", category.IdCategory);

            return category.IdCategory;
        }
    }
}