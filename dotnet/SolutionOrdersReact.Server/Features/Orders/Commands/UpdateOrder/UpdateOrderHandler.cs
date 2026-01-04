using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateOrderHandler> _logger;

        public UpdateOrderHandler(
            ApplicationDbContext context,
            ILogger<UpdateOrderHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            UpdateOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Aktualizacja zamówienia ID: {OrderId}", request.IdOrder);

            // Znajdź zamówienie z pozycjami
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.IdOrder == request.IdOrder, cancellationToken);

            if (order == null)
            {
                throw new KeyNotFoundException($"Zamówienie o ID {request.IdOrder} nie istnieje");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Aktualizuj dane zamówienia
                order.IdClient = request.IdClient;
                order.IdWorker = request.IdWorker;
                order.Notes = request.Notes;
                order.DeliveryDate = request.DeliveryDate;
                order.IsActive = request.IsActive;

                // Pobierz produkty do walidacji cen
                var itemIds = request.Items.Select(i => i.IdItem).ToList();
                var items = await _context.Items
                    .Where(i => itemIds.Contains(i.IdItem))
                    .ToDictionaryAsync(i => i.IdItem, cancellationToken);

                // Usuń pozycje które nie są w nowej liście (hard delete lub soft delete)
                var incomingIds = request.Items
                    .Where(i => i.IdOrderItem.HasValue)
                    .Select(i => i.IdOrderItem!.Value)
                    .ToList();

                var toRemove = order.OrderItems
                    .Where(oi => !incomingIds.Contains(oi.IdOrderItem))
                    .ToList();

                foreach (var item in toRemove)
                {
                    item.IsActive = false;  // Soft delete
                }

                // Aktualizuj istniejące i dodaj nowe
                foreach (var itemDto in request.Items)
                {
                    if (itemDto.IdOrderItem.HasValue)
                    {
                        // Aktualizacja istniejącej pozycji
                        var existing = order.OrderItems
                            .FirstOrDefault(oi => oi.IdOrderItem == itemDto.IdOrderItem);

                        if (existing != null)
                        {
                            existing.IdItem = itemDto.IdItem;
                            existing.Quantity = itemDto.Quantity;
                            existing.IsActive = itemDto.IsActive;
                            // Cena pozostaje oryginalna (z momentu zamówienia)
                        }
                    }
                    else
                    {
                        // Nowa pozycja
                        var newItem = new OrderItem
                        {
                            IdOrder = order.IdOrder,
                            IdItem = itemDto.IdItem,
                            Quantity = itemDto.Quantity,
                            UnitPrice = items.ContainsKey(itemDto.IdItem)
                                ? items[itemDto.IdItem].Price ?? 0
                                : 0,
                            IsActive = true
                        };

                        _context.OrderItems.Add(newItem);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Zamówienie {OrderId} zaktualizowane", request.IdOrder);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Błąd podczas aktualizacji zamówienia {OrderId}", request.IdOrder);
                throw;
            }
        }
    }
}