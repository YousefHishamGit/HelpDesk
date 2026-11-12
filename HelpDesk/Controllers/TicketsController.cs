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

        private int? GetCurrentUserId()
        {
            var val = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? User.FindFirst("nameid")?.Value
                   ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return int.TryParse(val, out var id) ? id : null;
        }

        private UserRole? GetCurrentUserRole()
        {
            var roleStr = User.FindFirst(ClaimTypes.Role)?.Value
                       ?? User.FindFirst("role")?.Value
                       ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            return Enum.TryParse<UserRole>(roleStr, true, out var r) ? r : null;
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequestDto dto)
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            var result = await _ticketService.CreateTicketAsync(currentUserId.Value, dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (!currentUserId.HasValue || !currentUserRole.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user credentials in token." });
            }

            var result = await _ticketService.GetByIdAsync(
                id,
                currentUserId.Value,
                currentUserRole.Value);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTickets()
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (!currentUserId.HasValue || !currentUserRole.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user credentials in token." });
            }

            var tickets = await _ticketService.GetMyTicketsAsync(currentUserId.Value, currentUserRole.Value);

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
        public async Task<IActionResult> AssignTicket(int id, AssignTicketRequestDto dto)
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
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            var result = await _ticketService.StartTicketAsync(id, currentUserId.Value);
            return Ok(result);
        }

        [Authorize(Roles = "Agent")]
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            var result = await _ticketService.ResolveTicketAsync(id, currentUserId.Value);
            return Ok(result);
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (!currentUserId.HasValue || !currentUserRole.HasValue)
            {
                return Unauthorized(new { message = "Invalid user token claims." });
            }

            var result = await _ticketService.CloseTicketAsync(id, currentUserId.Value, currentUserRole.Value);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{ticketId}/comments")]
        public async Task<IActionResult> AddComment(int ticketId, CreateTicketCommentDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (!currentUserId.HasValue || !currentUserRole.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user credentials in token." });
            }

            var result = await _ticketCommentService.AddCommentAsync(ticketId, currentUserId.Value, currentUserRole.Value, dto);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{ticketId}/comments")]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (!currentUserId.HasValue || !currentUserRole.HasValue)
            {
                return Unauthorized(new { message = "Invalid or missing user credentials in token." });
            }

            var comments = await _ticketCommentService.GetCommentsAsync(ticketId, currentUserId.Value, currentUserRole.Value);
            return Ok(comments);
        }
    }
}
