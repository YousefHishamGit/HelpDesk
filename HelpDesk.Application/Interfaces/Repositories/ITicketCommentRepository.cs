using HelpDesk.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface ITicketCommentRepository
    {
        Task AddAsync(TicketComment comment);
        Task<List<TicketComment>> GetByTicketIdAsync(int ticketId);
        Task SaveChangesAsync();
    }
}
