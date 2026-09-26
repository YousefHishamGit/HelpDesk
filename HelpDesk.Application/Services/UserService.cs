using HelpDesk.Application.DTOs.Common;
using HelpDesk.Application.DTOs.Users;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entity;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher<User> _passHash;

        public UserService(IUserRepository userRep, IPasswordHasher<User> passwordHasher) {
            _userRepo=userRep;
            _passHash=passwordHasher;
        }

        public async Task<PagedResultDto<UserListItemDto>> GetUsersAsync(UserQueryParametersDto? parameters = null)
        {
            parameters ??= new UserQueryParametersDto();

            
            var pageNumber = Math.Max(1, parameters.PageNumber);
            var pageSize = Math.Clamp(parameters.PageSize, 1, 50);

            
            var query = _userRepo.GetAllAsync();

           
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim();

                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search));
            }

        
            if (!string.IsNullOrWhiteSpace(parameters.Role))
            {
                if (Enum.TryParse<UserRole>(
                        parameters.Role,
                        true,
                        out var role))
                {
                    query = query.Where(u => u.Role == role);
                }
                else
                {
                    throw new ArgumentException("Invalid role.");
                }
            }

           
            if (parameters.IsActive.HasValue)
            {
                query = query.Where(u =>
                    u.IsActive == parameters.IsActive.Value);
            }

            query = parameters.SortBy?.ToLower() switch
            {
                "fullname" => parameters.SortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.FullName)
                    : query.OrderBy(u => u.FullName),

                "email" => parameters.SortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),

                
                "createdat" => parameters.SortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),

                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserListItemDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

           
            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

           
            return new PagedResultDto<UserListItemDto>
            {
                Items = users,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<AgentResponseDto> CreateAgentAsync(CreateAgentRequestDto dto)
        {
            //check email
            var EmailExist = await _userRepo.GetByEmailAsync(dto.Email);
            if (EmailExist is not null) {
                throw new InvalidOperationException("Email is already Exist");
            }
            //create
            var AgentUser = new User
            {
                Email = dto.Email,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Role = UserRole.Agent,
            };
            //hash pass
            AgentUser.PasswordHash = _passHash.HashPassword(AgentUser, $"{AgentUser.Email}123");
            //save
            await _userRepo.AddAsync(AgentUser);
            await _userRepo.SaveChangesAsync();
            //return
            return new AgentResponseDto
            {
                Id=AgentUser.Id,
                Email=AgentUser.Email,
                FullName=AgentUser.FullName,
                Phone=AgentUser.Phone,
                Role=AgentUser.Role.ToString(),
                IsActive = AgentUser.IsActive,
                CreatedAt=AgentUser.CreatedAt

            };
        }

        public async Task UpdateUserAsync(int userId, UpdateUserRequestDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user is null) {
                throw new InvalidOperationException("User Not Found");
            }
            user.FullName = dto.FullName;
            user.Phone= dto.Phone;

            // ✅ كانوا مش awaited — التغييرات مش كانت بتتحفظ
            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();

                
        }

        public async Task UpdateUserStatusAsync(int currentUserId, int userId, UpdateUserStatusDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user is null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (user.Id == currentUserId) {
                throw new InvalidOperationException("You cannot change your own status.");
            }

            user.IsActive = dto.IsActive;

            await _userRepo.SaveChangesAsync();
        }
    }
}
