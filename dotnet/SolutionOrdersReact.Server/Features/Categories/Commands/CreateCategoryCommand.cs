using MediatR;

namespace SolutionOrdersReact.Server.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}