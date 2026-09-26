using HelpDesk.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        IQueryable<User> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}
