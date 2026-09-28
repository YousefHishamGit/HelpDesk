using HelpDesk.Application.DTOs.Tickets;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Services;
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
        private readonly ITicketCommentService _ticketCommentService;
        public TicketsController(ITicketService ticketService, ITicketCommentService ticketCommentService)
        {
            _ticketService = ticketService;
            _ticketCommentService = ticketCommentService;
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

        [Authorize(Roles = "Agent")]
        [HttpPut("{id}/start")]
        public async Task<IActionResult> StartTicket(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var result = await _ticketService.StartTicketAsync(id, userId);

            return Ok(result);
        }

        [Authorize(Roles = "Agent")]
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var result = await _ticketService
                .ResolveTicketAsync(id, userId);

            return Ok(result);
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRole))
                return Unauthorized();

            if (!int.TryParse(userIdClaim, out int userId) || !Enum.TryParse<UserRole>(userRole, true, out var parsedRole))
                return BadRequest(new { message = "Invalid user token claims." });

            try
            {
                var result = await _ticketService.CloseTicketAsync(id, userId, parsedRole);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("{ticketId}/comments")]
        public async Task<IActionResult> AddComment(int ticketId,CreateTicketCommentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (roleClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var role = Enum.Parse<UserRole>(roleClaim.Value);

            var result = await _ticketCommentService.AddCommentAsync(ticketId, userId,role, dto);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{ticketId}/comments")]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (roleClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var role = Enum.Parse<UserRole>(roleClaim.Value);

            var comments = await _ticketCommentService.GetCommentsAsync(ticketId, userId, role);

            return Ok(comments);
        }
    }
}
