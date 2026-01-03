using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Features.Items.Commands.CreateItem;
using SolutionOrdersReact.Server.Features.Items.Commands.DeleteItem;
using SolutionOrdersReact.Server.Features.Items.Commands.UpdateItem;
using SolutionOrdersReact.Server.Features.Items.Queries.GetAllItems;
using SolutionOrdersReact.Server.Features.Items.Queries.GetItemById;

namespace SolutionOrdersReact.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(IMediator mediator, ILogger<ItemsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Pobiera wszystkie aktywne produkty
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/items - Pobieranie wszystkich produktów");

            var query = new GetAllItemsQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Pobiera produkt po ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/items/{Id} - Pobieranie produktu", id);

            var query = new GetItemByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Produkt o ID {id} nie został znaleziony" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Tworzy nowy produkt
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateItemCommand command)
        {
            _logger.LogInformation("POST /api/items - Tworzenie nowego produktu");

            var itemId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = itemId },
                new { id = itemId, message = "Produkt został utworzony" }
            );
        }

        /// <summary>
        /// Aktualizuje produkt
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateItemCommand command)
        {
            _logger.LogInformation("PUT /api/items/{Id} - Aktualizacja produktu", id);

            if (id != command.IdItem)
            {
                return BadRequest(new { message = "ID w URL różni się od ID w body" });
            }

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Usuwa produkt (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/items/{Id} - Usuwanie produktu", id);

            var command = new DeleteItemCommand(id);

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}