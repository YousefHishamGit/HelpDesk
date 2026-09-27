using HelpDesk.Domain.Entity;
using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task AddAsync(Ticket ticket);
        Task<Ticket?> GetByIdAsync(int id);
        Task<List<Ticket>> GetMyTicketsAsync(int userId,UserRole role);
        Task<Ticket?> GetByIdForUpdateAsync(int ticketId);
        Task UpdateAsync(Ticket ticket);

        IQueryable<Ticket> GetAllTicketsAsync();
        Task SaveChangesAsync();
    }
}
