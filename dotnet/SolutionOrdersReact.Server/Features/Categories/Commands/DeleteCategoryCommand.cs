using MediatR;

namespace SolutionOrdersReact.Server.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public int IdCategory { get; set; }

        public DeleteCategoryCommand(int idCategory)
        {
            IdCategory = idCategory;
        }
    }
}