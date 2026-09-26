using HelpDesk.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string token);

    }
}
