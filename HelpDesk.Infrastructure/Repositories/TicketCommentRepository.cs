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
    public class TicketCommentRepository : ITicketCommentRepository
    {
        private readonly AppDbContext _dbContext;

        public TicketCommentRepository(AppDbContext dbContext) {
            _dbContext=dbContext;
        }
        public async Task AddAsync(TicketComment comment)
        {
            await _dbContext.TicketComments.AddAsync(comment);
        }
        public async Task<List<TicketComment>> GetByTicketIdAsync(int ticketId)
        {
            return await _dbContext.TicketComments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
