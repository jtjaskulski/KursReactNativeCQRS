using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Items.Commands.UpdateItem
{
    /// <summary>
    /// Handler - logika aktualizacji produktu
    /// </summary>
    public class UpdateItemHandler : IRequestHandler<UpdateItemCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateItemHandler> _logger;

        public UpdateItemHandler(ApplicationDbContext context, ILogger<UpdateItemHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            UpdateItemCommand request,
            CancellationToken cancellationToken)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.IdItem == request.IdItem, cancellationToken);

            if (item == null)
            {
                _logger.LogError("Produkt o ID {IdItem} nie istnieje", request.IdItem);
                throw new KeyNotFoundException($"Produkt o ID {request.IdItem} nie istnieje");
            }

            _logger.LogInformation("Aktualizacja produktu ID: {IdItem}", request.IdItem);

            item.Name = request.Name;
            item.Description = request.Description;
            item.IdCategory = request.IdCategory;
            item.Price = request.Price;
            item.Quantity = request.Quantity;
            item.FotoUrl = request.FotoUrl;
            item.IdUnitOfMeasurement = request.IdUnitOfMeasurement;
            item.Code = request.Code;
            item.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Zaktualizowano produkt ID: {IdItem}", request.IdItem);

            return Unit.Value;
        }
    }
}
