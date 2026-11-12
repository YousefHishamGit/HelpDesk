using HelpDesk.Application.DTOs.Tickets;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entity;
using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services
{
    public class TicketCommentService : ITicketCommentService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly ITicketCommentRepository _ticketCommRepo;

        public TicketCommentService(ITicketRepository ticketRepo, ITicketCommentRepository commentRepo)
        {
            _ticketRepo     = ticketRepo;
            _ticketCommRepo = commentRepo;
        }

        public async Task<TicketCommentDto> AddCommentAsync(int ticketId, int currentUserId, UserRole currentUserRole, CreateTicketCommentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException("Comment content is required.");

            
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            if (currentUserRole == UserRole.Employee && ticket.CreatedById != currentUserId)
                throw new UnauthorizedAccessException("You are not allowed to comment on this ticket.");

            if (currentUserRole == UserRole.Agent && ticket.AssignedToId != currentUserId)
                throw new UnauthorizedAccessException("You are not allowed to comment on this ticket.");

            var commenterName = currentUserRole switch
            {
                UserRole.Employee => ticket.CreatedBy?.FullName,
                UserRole.Agent    => ticket.AssignedTo?.FullName,
                _                 => null
            };

            var comment = new TicketComment
            {
                TicketId  = ticketId,
                UserId    = currentUserId,
                Content   = dto.Content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _ticketCommRepo.AddAsync(comment);
            await _ticketCommRepo.SaveChangesAsync();

            return new TicketCommentDto
            {
                Id        = comment.Id,
                UserId    = comment.UserId,
                UserName  = commenterName,
                Content   = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<List<TicketCommentDto>> GetCommentsAsync(int ticketId, int currentUserId, UserRole currentUserRole)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            if (currentUserRole == UserRole.Employee && ticket.CreatedById != currentUserId)
                throw new UnauthorizedAccessException("You are not allowed to view comments on this ticket.");

            if (currentUserRole == UserRole.Agent && ticket.AssignedToId != currentUserId)
                throw new UnauthorizedAccessException("You are not allowed to view comments on this ticket.");

            var comments = await _ticketCommRepo.GetByTicketIdAsync(ticketId);

            return comments.Select(c => new TicketCommentDto
            {
                Id        = c.Id,
                UserId    = c.UserId,
                UserName  = c.User.FullName,
                Content   = c.Content,
                CreatedAt = c.CreatedAt
            }).ToList();
        }
    }
}
