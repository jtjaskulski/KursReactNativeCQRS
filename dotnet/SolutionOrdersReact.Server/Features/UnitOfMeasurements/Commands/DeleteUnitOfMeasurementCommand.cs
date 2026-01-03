using MediatR;

namespace SolutionOrdersReact.Server.Features.UnitOfMeasurements.Commands
{
    public class DeleteUnitOfMeasurementCommand : IRequest<Unit>
    {
        public int IdUnitOfMeasurement { get; set; }

        public DeleteUnitOfMeasurementCommand(int id)
        {
            IdUnitOfMeasurement = id;
        }
    }
}
