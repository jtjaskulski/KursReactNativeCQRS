using MediatR;
using Microsoft.EntityFrameworkCore;
using SolutionOrdersReact.Server.Data;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.UnitOfMeasurements.Queries
{
    public class GetUnitOfMeasurementByIdHandler : IRequestHandler<GetUnitOfMeasurementByIdQuery, UnitOfMeasurementDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetUnitOfMeasurementByIdHandler> _logger;

        public GetUnitOfMeasurementByIdHandler(ApplicationDbContext context, ILogger<GetUnitOfMeasurementByIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UnitOfMeasurementDto?> Handle(
            GetUnitOfMeasurementByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pobieranie jednostki miary o ID: {Id}", request.Id);

            var unit = await _context.UnitOfMeasurements
                .Where(u => u.IdUnitOfMeasurement == request.Id)
                .Select(u => new UnitOfMeasurementDto
                {
                    IdUnitOfMeasurement = u.IdUnitOfMeasurement,
                    Name = u.Name,
                    Description = u.Description,
                    IsActive = u.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (unit == null)
            {
                _logger.LogWarning("Jednostka miary o ID {Id} nie została znaleziona", request.Id);
            }

            return unit;
        }
    }
}
