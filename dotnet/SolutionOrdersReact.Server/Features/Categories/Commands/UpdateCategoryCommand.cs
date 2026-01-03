using MediatR;

namespace SolutionOrdersReact.Server.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<Unit>
    {
        public int IdCategory { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
