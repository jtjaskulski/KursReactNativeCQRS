using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetOrderByIdHandler> _logger;

        public GetOrderByIdHandler(
            ApplicationDbContext context,
            ILogger<GetOrderByIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OrderDto?> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie zamówienia ID: {Id}", request.Id);

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Worker)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                        .ThenInclude(i => i.UnitOfMeasurement)
                .Where(o => o.IdOrder == request.Id)
                .Select(o => new OrderDto
                {
                    IdOrder = o.IdOrder,
                    DataOrder = o.DataOrder,
                    DeliveryDate = o.DeliveryDate,
                    Notes = o.Notes,
                    IsActive = o.IsActive,
                    IdClient = o.IdClient,
                    ClientName = o.Client != null ? o.Client.FirstName : null,
                    ClientPhone = o.Client != null ? o.Client.Phone : null,
                    IdWorker = o.IdWorker,
                    WorkerName = o.Worker != null
                        ? o.Worker.FirstName + " " + o.Worker.LastName
                        : null,
                    Items = o.OrderItems
                        .Select(oi => new OrderItemDto
                        {
                            IdOrderItem = oi.IdOrderItem,
                            IdItem = oi.IdItem,
                            ItemName = oi.Item.Name,
                            ItemCode = oi.Item.Code,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            LineTotal = (oi.Quantity ?? 0) * (oi.UnitPrice ?? 0),
                            UnitName = oi.Item.UnitOfMeasurement != null
                                ? oi.Item.UnitOfMeasurement.Name
                                : "szt",
                            IsActive = oi.IsActive
                        })
                        .ToList(),
                    TotalAmount = o.OrderItems
                        .Where(oi => oi.IsActive)
                        .Sum(oi => (oi.Quantity ?? 0) * (oi.UnitPrice ?? 0)),
                    ItemCount = o.OrderItems.Count(oi => oi.IsActive)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Zamówienie ID {Id} nie znalezione", request.Id);
            }

            return order;
        }
    }
}