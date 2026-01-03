using MediatR;

namespace SolutionOrdersReact.Server.Features.UnitOfMeasurements.Commands
{
    public class CreateUnitOfMeasurementCommand : IRequest<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
