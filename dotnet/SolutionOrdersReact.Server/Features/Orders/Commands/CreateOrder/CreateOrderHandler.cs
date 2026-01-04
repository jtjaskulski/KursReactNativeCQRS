using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateOrderHandler> _logger;

        public CreateOrderHandler(
            ApplicationDbContext context,
            ILogger<CreateOrderHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tworzenie zamówienia dla klienta {ClientId}", request.IdClient);

            // =========================================
            // WALIDACJA
            // =========================================

            // Sprawdź czy są pozycje
            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ArgumentException("Zamówienie musi zawierać przynajmniej jedną pozycję");
            }

            // Sprawdź czy klient istnieje (jeśli podany)
            if (request.IdClient.HasValue)
            {
                var clientExists = await _context.Clients
                    .AnyAsync(c => c.IdClient == request.IdClient && c.IsActive, cancellationToken);

                if (!clientExists)
                {
                    throw new ArgumentException($"Klient o ID {request.IdClient} nie istnieje");
                }
            }

            // Sprawdź czy pracownik istnieje (jeśli podany)
            if (request.IdWorker.HasValue)
            {
                var workerExists = await _context.Workers
                    .AnyAsync(w => w.IdWorker == request.IdWorker && w.IsActive, cancellationToken);

                if (!workerExists)
                {
                    throw new ArgumentException($"Pracownik o ID {request.IdWorker} nie istnieje");
                }
            }

            // Pobierz produkty dla walidacji i cen
            var itemIds = request.Items.Select(i => i.IdItem).ToList();
            var items = await _context.Items
                .Where(i => itemIds.Contains(i.IdItem))
                .ToDictionaryAsync(i => i.IdItem, cancellationToken);

            // Sprawdź czy wszystkie produkty istnieją
            foreach (var itemDto in request.Items)
            {
                if (!items.ContainsKey(itemDto.IdItem))
                {
                    throw new ArgumentException($"Produkt o ID {itemDto.IdItem} nie istnieje");
                }

                if (!items[itemDto.IdItem].IsActive)
                {
                    throw new ArgumentException($"Produkt '{items[itemDto.IdItem].Name}' jest nieaktywny");
                }

                if (itemDto.Quantity <= 0)
                {
                    throw new ArgumentException($"Ilość musi być większa od 0 dla produktu '{items[itemDto.IdItem].Name}'");
                }
            }

            // =========================================
            // TWORZENIE W TRANSAKCJI
            // =========================================

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Utwórz zamówienie (Master)
                var order = new Order
                {
                    DataOrder = DateTime.Now,
                    IdClient = request.IdClient,
                    IdWorker = request.IdWorker,
                    Notes = request.Notes,
                    DeliveryDate = request.DeliveryDate,
                    IsActive = true
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Utworzono zamówienie ID: {OrderId}", order.IdOrder);

                // Utwórz pozycje (Detail)
                foreach (var itemDto in request.Items)
                {
                    var item = items[itemDto.IdItem];

                    var orderItem = new OrderItem
                    {
                        IdOrder = order.IdOrder,
                        IdItem = itemDto.IdItem,
                        Quantity = itemDto.Quantity,
                        UnitPrice = item.Price ?? 0,  // Kopiujemy aktualną cenę!
                        IsActive = true
                    };

                    _context.OrderItems.Add(orderItem);

                    _logger.LogInformation(
                        "Dodano pozycję: {ItemName} x {Quantity} @ {Price}",
                        item.Name, itemDto.Quantity, item.Price);
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Commit transakcji
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation(
                    "Zamówienie {OrderId} utworzone z {Count} pozycjami",
                    order.IdOrder, request.Items.Count);

                return order.IdOrder;
            }
            catch (Exception ex)
            {
                // Rollback w przypadku błędu
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Błąd podczas tworzenia zamówienia");
                throw;
            }
        }
    }
}