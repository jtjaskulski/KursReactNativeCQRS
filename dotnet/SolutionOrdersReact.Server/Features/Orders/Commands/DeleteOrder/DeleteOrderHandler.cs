using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteOrderHandler> _logger;

        public DeleteOrderHandler(
            ApplicationDbContext context,
            ILogger<DeleteOrderHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            DeleteOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Usuwanie zamówienia ID: {OrderId}, HardDelete: {HardDelete}",
                request.IdOrder, request.HardDelete);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.IdOrder == request.IdOrder, cancellationToken);

            if (order == null)
            {
                throw new KeyNotFoundException($"Zamówienie o ID {request.IdOrder} nie istnieje");
            }

            if (request.HardDelete)
            {
                // Hard delete - fizyczne usunięcie (CASCADE usunie też OrderItems)
                _context.Orders.Remove(order);
                _logger.LogWarning("Hard delete zamówienia {OrderId}", request.IdOrder);
            }
            else
            {
                // Soft delete - oznaczenie jako nieaktywne
                order.IsActive = false;
                foreach (var item in order.OrderItems)
                {
                    item.IsActive = false;
                }
                _logger.LogInformation("Soft delete zamówienia {OrderId}", request.IdOrder);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}