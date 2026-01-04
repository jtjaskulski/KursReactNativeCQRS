using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionOrdersReact.Server.Dto;
using SolutionOrdersReact.Server.Features.Orders.Commands.CreateOrder;
using SolutionOrdersReact.Server.Features.Orders.Commands.UpdateOrder;
using SolutionOrdersReact.Server.Features.Orders.Commands.DeleteOrder;
using SolutionOrdersReact.Server.Features.Orders.Queries.GetAllOrders;
using SolutionOrdersReact.Server.Features.Orders.Queries.GetOrderById;

namespace SolutionOrdersReact.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Pobiera wszystkie zamówienia
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? clientId = null,
            [FromQuery] int? workerId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] bool includeInactive = false)
        {
            var query = new GetAllOrdersQuery
            {
                ClientId = clientId,
                WorkerId = workerId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                IncludeInactive = includeInactive
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Pobiera zamówienie po ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Zamówienie o ID {id} nie zostało znalezione" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Tworzy nowe zamówienie z pozycjami
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            try
            {
                var orderId = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = orderId },
                    new { id = orderId, message = "Zamówienie zostało utworzone" }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Aktualizuje zamówienie
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderCommand command)
        {
            if (id != command.IdOrder)
            {
                return BadRequest(new { message = "ID w URL nie zgadza się z ID w body" });
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Usuwa zamówienie (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            try
            {
                var command = new DeleteOrderCommand(id, hardDelete);
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