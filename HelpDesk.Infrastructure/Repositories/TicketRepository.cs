using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Domain.Entity;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {

        private readonly AppDbContext _dbContext;

        public TicketRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Ticket ticket)
        {
            await _dbContext.AddAsync(ticket);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        async Task<Ticket?> ITicketRepository.GetByIdAsync(int id)
        {
            return await _dbContext.Ticket
                                    .AsNoTracking()
                                    .Include(t => t.Category)
                                    .Include(t => t.CreatedBy)
                                    .Include(t => t.AssignedTo)
                                    .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Ticket>> GetMyTicketsAsync(int userId,UserRole role)
        {
            IQueryable<Ticket> query = _dbContext.Ticket
                                        .AsNoTracking()
                                        .Include(t => t.Category)
                                        .Include(t => t.CreatedBy)
                                        .Include(t => t.AssignedTo);

            if (role == UserRole.Employee)
            {
                query = query.Where(t => t.CreatedById == userId);
            }
            else if (role == UserRole.Agent)
            {
                query = query.Where(t => t.AssignedToId == userId);
            }
            else
            {
                return new List<Ticket>();
            }

            return await query.ToListAsync();
        }

       IQueryable<Ticket> ITicketRepository.GetAllTicketsAsync()
        {
            return _dbContext.Ticket
                             .AsNoTracking()
                             .Include(t => t.Category)
                             .Include(t => t.CreatedBy)
                             .Include(t => t.AssignedTo);
        }

        public async Task<Ticket?> GetByIdForUpdateAsync(int ticketId)
        {
            return await _dbContext.Ticket.FirstOrDefaultAsync(t => t.Id == ticketId);
        }

       public async Task UpdateAsync(Ticket ticket)
        {
             _dbContext.Ticket.Update(ticket);
        }
    }
}
