using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Items.Commands.DeleteItem
{
    /// <summary>
    /// Handler - logika usuwania produktu (soft delete)
    /// </summary>
    public class DeleteItemHandler : IRequestHandler<DeleteItemCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteItemHandler> _logger;

        public DeleteItemHandler(ApplicationDbContext context, ILogger<DeleteItemHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            DeleteItemCommand request,
            CancellationToken cancellationToken)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.IdItem == request.IdItem, cancellationToken);

            if (item == null)
            {
                _logger.LogError("Produkt o ID {IdItem} nie istnieje", request.IdItem);
                throw new KeyNotFoundException($"Produkt o ID {request.IdItem} nie istnieje");
            }

            _logger.LogInformation("Usuwanie produktu ID: {IdItem}", request.IdItem);

            // Soft delete - tylko ustawiamy IsActive = false
            item.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Usunięto produkt ID: {IdItem}", request.IdItem);

            return Unit.Value;
        }
    }
}
