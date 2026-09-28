using HelpDesk.Application.DTOs.Tickets;
using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface ITicketCommentService
    {
        Task<TicketCommentDto> AddCommentAsync(int ticketId,int currentUserId,UserRole currentUserRole,CreateTicketCommentDto dto);
        Task<List<TicketCommentDto>> GetCommentsAsync(int ticketId,int currentUserId, UserRole currentUserRole);
    }
}
