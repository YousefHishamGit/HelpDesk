using HelpDesk.Application.DTOs.Common;
using HelpDesk.Application.DTOs.Tickets;
using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<TicketResponseDto> CreateTicketAsync(int currentUserId,CreateTicketRequestDto dto);

        Task<TicketDetailsDto?> GetByIdAsync(int ticketId,int currentUserId,UserRole currentUserRole);
        Task<List<TicketDetailsDto>> GetMyTicketsAsync(int currentUserId,UserRole currentUserRole);
        Task<PagedResultDto<TicketDetailsDto>> GetAllTicketsAsync(TicketQueryParametersDto? parameters = null);
        Task<TicketDetailsDto> AssignTicketAsync(int ticketId, AssignTicketRequestDto dto);
        Task<TicketDetailsDto> UnassignTicketAsync(int ticketId);
        Task<TicketDetailsDto> StartTicketAsync(int ticketId,int currentUserId);
        Task<TicketDetailsDto> ResolveTicketAsync(int ticketId,int currentUserId);
        Task<TicketDetailsDto> CloseTicketAsync(int ticketId, int currentUserId, UserRole currentUserRole);
    }
}
