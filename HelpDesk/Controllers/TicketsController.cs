using HelpDesk.Application.DTOs.Tickets;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("nameid")?.Value;

            if (!int.TryParse(userIdClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            try
            {
                var result = await _ticketService.CreateTicketAsync(currentUserId, dto);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!Enum.TryParse<UserRole>(role, true, out var currentUserRole))
            {
                return Forbid();
            }

            var result = await _ticketService.GetByIdAsync(
                id,
                currentUserId,
                currentUserRole);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (roleClaim == null)
                return Unauthorized();

            var role = Enum.Parse<UserRole>(roleClaim.Value);

            var tickets = await _ticketService.GetMyTicketsAsync(userId, role);

            return Ok(tickets);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets([FromQuery] TicketQueryParametersDto parameters)
        {
            var result = await _ticketService.GetAllTicketsAsync(parameters);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignTicket(int id,AssignTicketRequestDto dto)
        {
            var result = await _ticketService.AssignTicketAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/unassign")]
        public async Task<IActionResult> UnAssignTicket(int id)
        {
            var result = await _ticketService.UnassignTicketAsync(id);
            return Ok(result);
        }
    }
}
