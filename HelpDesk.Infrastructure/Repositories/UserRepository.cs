using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entity;
using HelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext) {
            _dbContext = dbContext;
        }
        async Task IUserRepository.AddAsync(User user)
        {
            await _dbContext.AddAsync(user);
        }
        public async Task UpdateAsync(User user)
        {
            await _dbContext.SaveChangesAsync();
        }

        async Task<User?> IUserRepository.GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u=>u.Email == email);
        }
        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbContext.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
        }

         async Task IUserRepository.SaveChangesAsync()
        {
           await _dbContext.SaveChangesAsync();
        }

        public IQueryable<User> GetAllAsync()
        {
            return _dbContext.Users.AsNoTracking();
        }

        async Task<User?> IUserRepository.GetByIdAsync(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u=>u.Id == id);
        }
    }
}
