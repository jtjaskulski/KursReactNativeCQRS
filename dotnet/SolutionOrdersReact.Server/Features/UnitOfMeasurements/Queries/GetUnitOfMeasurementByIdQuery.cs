using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.UnitOfMeasurements.Queries
{
    public class GetUnitOfMeasurementByIdQuery : IRequest<UnitOfMeasurementDto?>
    {
        public int Id { get; set; }

        public GetUnitOfMeasurementByIdQuery(int id)
        {
            Id = id;
        }
    }
}