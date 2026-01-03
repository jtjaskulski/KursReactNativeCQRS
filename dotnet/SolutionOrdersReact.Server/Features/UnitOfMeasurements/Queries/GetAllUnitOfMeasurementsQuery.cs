using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.UnitOfMeasurements.Queries
{
    public class GetAllUnitOfMeasurementsQuery : IRequest<List<UnitOfMeasurementDto>>
    {
    }
}
