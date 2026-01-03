using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Features.UnitOfMeasurements.Commands;
using SolutionOrdersReact.Server.Features.UnitOfMeasurements.Queries;

namespace SolutionOrdersReact.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitOfMeasurementsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UnitOfMeasurementsController> _logger;

        public UnitOfMeasurementsController(IMediator mediator, ILogger<UnitOfMeasurementsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Pobiera wszystkie aktywne jednostki miary
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<UnitOfMeasurementDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/unitofmeasurements - Pobieranie wszystkich jednostek miary");

            var query = new GetAllUnitOfMeasurementsQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Pobiera jednostkę miary po ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UnitOfMeasurementDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GET /api/unitofmeasurements/{Id} - Pobieranie jednostki miary", id);

            var query = new GetUnitOfMeasurementByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Jednostka miary o ID {id} nie została znaleziona" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Tworzy nową jednostkę miary
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUnitOfMeasurementCommand command)
        {
            _logger.LogInformation("POST /api/unitofmeasurements - Tworzenie nowej jednostki miary");

            var unitId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = unitId },
                new { id = unitId, message = "Jednostka miary została utworzona" }
            );
        }

        /// <summary>
        /// Aktualizuje jednostkę miary
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUnitOfMeasurementCommand command)
        {
            _logger.LogInformation("PUT /api/unitofmeasurements/{Id} - Aktualizacja jednostki miary", id);

            if (id != command.IdUnitOfMeasurement)
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
        /// Usuwa jednostkę miary (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("DELETE /api/unitofmeasurements/{Id} - Usuwanie jednostki miary", id);

            var command = new DeleteUnitOfMeasurementCommand(id);

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
