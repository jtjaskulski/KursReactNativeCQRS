using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;

namespace SolutionOrdersReact.Server.Features.UnitOfMeasurements.Commands
{
    public class UpdateUnitOfMeasurementHandler : IRequestHandler<UpdateUnitOfMeasurementCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateUnitOfMeasurementHandler> _logger;

        public UpdateUnitOfMeasurementHandler(ApplicationDbContext context, ILogger<UpdateUnitOfMeasurementHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            UpdateUnitOfMeasurementCommand request,
            CancellationToken cancellationToken)
        {
            var unit = await _context.UnitOfMeasurements
                .FirstOrDefaultAsync(u => u.IdUnitOfMeasurement == request.IdUnitOfMeasurement, cancellationToken);

            if (unit == null)
            {
                _logger.LogError("Jednostka miary o ID {Id} nie istnieje", request.IdUnitOfMeasurement);
                throw new KeyNotFoundException($"Jednostka miary o ID {request.IdUnitOfMeasurement} nie istnieje");
            }

            _logger.LogInformation("Aktualizacja jednostki miary o ID: {Id}", request.IdUnitOfMeasurement);

            unit.Name = request.Name;
            unit.Description = request.Description;
            unit.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Zaktualizowano jednostkę miary o ID: {Id}", request.IdUnitOfMeasurement);

            return Unit.Value;
        }
    }
}
