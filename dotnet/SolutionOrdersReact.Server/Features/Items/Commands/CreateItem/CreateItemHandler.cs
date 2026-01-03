using MediatR;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.Items.Commands.CreateItem
{
    /// <summary>
    /// Handler - logika tworzenia produktu
    /// </summary>
    public class CreateItemHandler : IRequestHandler<CreateItemCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateItemHandler> _logger;

        public CreateItemHandler(ApplicationDbContext context, ILogger<CreateItemHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(
            CreateItemCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tworzenie nowego produktu: {Name}", request.Name);

            var item = new Item
            {
                Name = request.Name,
                Description = request.Description,
                IdCategory = request.IdCategory,
                Price = request.Price,
                Quantity = request.Quantity,
                FotoUrl = request.FotoUrl,
                IdUnitOfMeasurement = request.IdUnitOfMeasurement,
                Code = request.Code,
                IsActive = true
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Utworzono produkt ID: {IdItem}", item.IdItem);

            return item.IdItem;
        }
    }
}