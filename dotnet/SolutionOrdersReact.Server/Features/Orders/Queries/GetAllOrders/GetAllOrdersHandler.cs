using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllOrdersHandler> _logger;

        public GetAllOrdersHandler(
            ApplicationDbContext context,
            ILogger<GetAllOrdersHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OrderDto>> Handle(
            GetAllOrdersQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie zamówień");

            var query = BuildOrderQuery();
            query = ApplyActivityFilter(query, request);
            query = ApplyClientFilter(query, request);
            query = ApplyWorkerFilter(query, request);
            query = ApplyDateFilters(query, request);

            var orders = await ProjectOrdersToDto(query, cancellationToken);

            _logger.LogInformation("Pobrano {Count} zamówień", orders.Count);

            return orders;
        }

        private IQueryable<Order> BuildOrderQuery()
        {
            return _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Worker)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                        .ThenInclude(i => i.UnitOfMeasurement)
                .AsQueryable();
        }

        private IQueryable<Order> ApplyActivityFilter(IQueryable<Order> query, GetAllOrdersQuery request)
        {
            if (!request.IncludeInactive)
            {
                query = query.Where(o => o.IsActive);
            }

            return query;
        }

        private IQueryable<Order> ApplyClientFilter(IQueryable<Order> query, GetAllOrdersQuery request)
        {
            if (request.ClientId.HasValue)
            {
                query = query.Where(o => o.IdClient == request.ClientId.Value);
            }

            return query;
        }

        private IQueryable<Order> ApplyWorkerFilter(IQueryable<Order> query, GetAllOrdersQuery request)
        {
            if (request.WorkerId.HasValue)
            {
                query = query.Where(o => o.IdWorker == request.WorkerId.Value);
            }

            return query;
        }

        private IQueryable<Order> ApplyDateFilters(IQueryable<Order> query, GetAllOrdersQuery request)
        {
            if (request.DateFrom.HasValue)
            {
                query = query.Where(o => o.DataOrder >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                query = query.Where(o => o.DataOrder <= request.DateTo.Value);
            }

            return query;
        }

        private async Task<List<OrderDto>> ProjectOrdersToDto(IQueryable<Order> query, CancellationToken cancellationToken)
        {
            return await query
                .OrderByDescending(o => o.DataOrder)
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
                        .Where(oi => oi.IsActive)
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
                .ToListAsync(cancellationToken);
        }
    }
}