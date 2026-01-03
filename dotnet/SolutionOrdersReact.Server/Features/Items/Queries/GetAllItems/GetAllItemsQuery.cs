using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetAllItems
{
    /// <summary>
    /// Query - pobiera wszystkie aktywne produkty
    /// </summary>
    public class GetAllItemsQuery : IRequest<List<ItemDto>>
    {
        // Query bez parametrów - po prostu "daj wszystkie"
    }
}