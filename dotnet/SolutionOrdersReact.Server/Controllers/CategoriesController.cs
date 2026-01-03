using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Features.Categories.Commands;
using SolutionOrdersReact.Server.Features.Categories.Queries;

namespace SolutionOrdersReact.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Pobiera wszystkie aktywne kategorie
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/categories - Pobieranie wszystkich kategorii");

            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Pobiera kategorię po ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/categories/{Id} - Pobieranie kategorii", id);

            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Kategoria o ID {id} nie została znaleziona" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Tworzy nową kategorię
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            _logger.LogInformation("POST /api/categories - Tworzenie nowej kategorii");

            var categoryId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = categoryId },
                new { id = categoryId, message = "Kategoria została utworzona" }
            );
        }

        /// <summary>
        /// Aktualizuje kategorię
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command)
        {
            _logger.LogInformation("PUT /api/categories/{Id} - Aktualizacja kategorii", id);

            if (id != command.IdCategory)
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
        /// Usuwa kategorię (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/categories/{Id} - Usuwanie kategorii", id);

            var command = new DeleteCategoryCommand(id);

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