using MediatR;

namespace SolutionOrdersReact.Server.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommand : IRequest<Unit>
    {
        public int IdOrder { get; set; }
        public bool HardDelete { get; set; } = false;  // Domyślnie soft delete

        public DeleteOrderCommand(int idOrder, bool hardDelete = false)
        {
            IdOrder = idOrder;
            HardDelete = hardDelete;
        }
    }
}