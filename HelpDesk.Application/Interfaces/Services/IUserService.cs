using HelpDesk.Application.DTOs.Common;
using HelpDesk.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResultDto<UserListItemDto>> GetUsersAsync(UserQueryParametersDto? parameters = null);
        Task<AgentResponseDto> CreateAgentAsync(CreateAgentRequestDto dto);
        Task UpdateUserStatusAsync(int currentUserId,int userId, UpdateUserStatusDto dto);

        Task UpdateUserAsync(int userId,UpdateUserRequestDto dto);
    }
}
