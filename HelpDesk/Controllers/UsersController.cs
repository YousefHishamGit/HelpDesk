using HelpDesk.Application.DTOs.Users;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userServ;

        public UsersController(IUserService userService) {
            _userServ=userService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParametersDto parameters) {
            var users = await _userServ.GetUsersAsync(parameters);
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("agents")]
        public async Task<IActionResult> CreateAgent(CreateAgentRequestDto dto)
        {
            var agent = await _userServ.CreateAgentAsync(dto);
            return Ok(agent);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateUserStatusDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _userServ.UpdateUserStatusAsync(currentUserId,id,dto);
            return NoContent();

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,UpdateUserRequestDto dto)
        {
            await _userServ.UpdateUserAsync(id, dto);

            return NoContent();
        }
    }
}
