using HelpDesk.Application.DTOs.Auth;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher<User> _passHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUserRepository user, IPasswordHasher<User> passwordHasher, IJwtTokenService jwtTokenService) {
            _userRepo=user;
            _passHasher=passwordHasher;
            _jwtTokenService = jwtTokenService;
        }
        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var isEmailExist = await _userRepo.GetByEmailAsync(dto.Email);
            if (isEmailExist is not null)
                throw new InvalidOperationException("Email already Exists");

            var newUser = new User
            {
                Email    = dto.Email,
                Phone    = dto.Phone,
                FullName = dto.FullName,
            };

            
            newUser.PasswordHash = _passHasher.HashPassword(newUser, dto.Password);

            
            var accessToken  = _jwtTokenService.GenerateAccessToken(newUser);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            newUser.RefreshTokens = new List<RefreshToken> { refreshToken };

            await _userRepo.AddAsync(newUser);
            await _userRepo.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken  = accessToken.Token,
                RefreshToken = refreshToken.Token,
                ExpiresAt    = refreshToken.ExpiresAt
            };
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                return null;
            }
            if (!user.IsActive)
            {
                return null;
            }
            //check pass
            var verifyPass = _passHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verifyPass == PasswordVerificationResult.Failed)
            {
                return null;
            }
            //Generate JWT
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            user.RefreshTokens ??= new List<RefreshToken>();
            user.RefreshTokens.Add(refreshToken);
            await _userRepo.UpdateAsync(user);

            return new LoginResponseDto()
            {
                AccessToken = accessToken.Token,
                RefreshToken= refreshToken.Token,
                ExpiresAt= refreshToken.ExpiresAt

              
                

            };
        }

        async Task<LoginResponseDto?> IAuthService.RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetByRefreshTokenAsync(refreshToken);
            if (user == null) return null;

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken);
            if (userRefreshToken == null) return null;

            if (userRefreshToken.ExpiresAt < DateTime.UtcNow || userRefreshToken.RevokedAt.HasValue)
                return null;

            
            userRefreshToken.RevokedAt = DateTime.UtcNow;

            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);

            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();

           
            return new LoginResponseDto
            {
                AccessToken = newAccessToken.Token,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = newRefreshToken.ExpiresAt
            };
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userRepo.GetByRefreshTokenAsync(token);
            if (user == null) return false;

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.Token == token);
            if (userRefreshToken == null) return false;

            if (userRefreshToken.RevokedAt.HasValue || userRefreshToken.ExpiresAt < DateTime.UtcNow)
                return false;

            userRefreshToken.RevokedAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);
            return true;
        }
    }
}
