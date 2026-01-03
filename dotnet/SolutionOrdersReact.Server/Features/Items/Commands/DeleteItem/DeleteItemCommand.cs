using MediatR;

namespace SolutionOrdersReact.Server.Features.Items.Commands.DeleteItem
{
    /// <summary>
    /// Command - usuwa produkt (soft delete)
    /// </summary>
    public class DeleteItemCommand : IRequest<Unit>
    {
        public int IdItem { get; set; }

        public DeleteItemCommand(int idItem)
        {
            IdItem = idItem;
        }
    }
}
