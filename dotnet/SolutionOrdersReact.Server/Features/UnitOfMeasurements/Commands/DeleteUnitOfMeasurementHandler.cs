using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.UnitOfMeasurements.Commands
{
    public class DeleteUnitOfMeasurementHandler : IRequestHandler<DeleteUnitOfMeasurementCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteUnitOfMeasurementHandler> _logger;

        public DeleteUnitOfMeasurementHandler(ApplicationDbContext context, ILogger<DeleteUnitOfMeasurementHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            DeleteUnitOfMeasurementCommand request,
            CancellationToken cancellationToken)
        {
            var unit = await _context.UnitOfMeasurements
                .FirstOrDefaultAsync(u => u.IdUnitOfMeasurement == request.IdUnitOfMeasurement, cancellationToken);

            if (unit == null)
            {
                _logger.LogError("Jednostka miary o ID {Id} nie istnieje", request.IdUnitOfMeasurement);
                throw new KeyNotFoundException($"Jednostka miary o ID {request.IdUnitOfMeasurement} nie istnieje");
            }

            _logger.LogInformation("Usuwanie jednostki miary o ID: {Id}", request.IdUnitOfMeasurement);

            unit.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Usunięto jednostkę miary o ID: {Id}", request.IdUnitOfMeasurement);

            return Unit.Value;
        }
    }
}