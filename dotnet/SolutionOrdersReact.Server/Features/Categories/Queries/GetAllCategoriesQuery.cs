using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
    {
    }
}