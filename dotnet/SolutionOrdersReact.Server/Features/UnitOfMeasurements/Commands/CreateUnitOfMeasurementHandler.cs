using MediatR;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Models;

namespace SolutionOrdersReact.Server.Features.UnitOfMeasurements.Commands
{
    public class CreateUnitOfMeasurementHandler : IRequestHandler<CreateUnitOfMeasurementCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateUnitOfMeasurementHandler> _logger;

        public CreateUnitOfMeasurementHandler(ApplicationDbContext context, ILogger<CreateUnitOfMeasurementHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(
            CreateUnitOfMeasurementCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tworzenie nowej jednostki miary: {Name}", request.Name);

            var unit = new UnitOfMeasurement
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true
            };

            _context.UnitOfMeasurements.Add(unit);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Utworzono jednostkę miary o ID: {Id}", unit.IdUnitOfMeasurement);

            return unit.IdUnitOfMeasurement;
        }
    }
}
