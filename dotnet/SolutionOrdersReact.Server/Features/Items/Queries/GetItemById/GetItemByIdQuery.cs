using MediatR;
using SolutionOrdersReact.Server.Dto;

namespace SolutionOrdersReact.Server.Features.Items.Queries.GetItemById
{
    /// <summary>
    /// Query z parametrem - pobiera produkt po ID
    /// </summary>
    public class GetItemByIdQuery : IRequest<ItemDto?>
    {
        public int Id { get; set; }

        public GetItemByIdQuery(int id)
        {
            Id = id;
        }
    }
}
