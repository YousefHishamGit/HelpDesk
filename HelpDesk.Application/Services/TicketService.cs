using HelpDesk.Application.DTOs.Common;
using HelpDesk.Application.DTOs.Tickets;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entity;
using HelpDesk.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services
{
    public class TicketService :ITicketService
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUserRepository _userRepo;

        public TicketService(ITicketRepository ticketRepo,ICategoryRepository categoryRepo,IUserRepository userRepository)
        {
            _ticketRepo = ticketRepo;
            _categoryRepo = categoryRepo;
            _userRepo = userRepository;
        }

        public async Task<TicketResponseDto> CreateTicketAsync(int currentUserId, CreateTicketRequestDto dto)
        {
            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);

            if (category is null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            if (!category.IsActive)
            {
                throw new InvalidOperationException(
                    "Cannot create ticket with inactive category.");
            }

            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Priority = dto.Priority,
                CreatedById = currentUserId,
                Status = TicketStatus.Open,
                AssignedToId = null,
                
            };

            await _ticketRepo.AddAsync(ticket);
            await _ticketRepo.SaveChangesAsync();
            return new TicketResponseDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId,
                CategoryName = category.Name,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedById = ticket.CreatedById,
                CreatedAt = ticket.CreatedAt
            };
        }

       public async Task<TicketDetailsDto?> GetByIdAsync(int ticketId, int currentUserId, UserRole currentUserRole)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket is null) throw new KeyNotFoundException("ticket not Found");

            if (currentUserRole == UserRole.Employee && ticket.CreatedById != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not allowed to view this ticket.");
            }
            if (currentUserRole == UserRole.Agent && ticket.AssignedToId != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not allowed to view this ticket.");
            }
            return new TicketDetailsDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryName = ticket.Category.Name,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedById = ticket.CreatedById,
                CreatedByName = ticket.CreatedBy.FullName,
                AssignedToId = ticket.AssignedToId,
                AssignedToName = ticket.AssignedTo?.FullName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                DueAt = ticket.DueAt,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt
            };
        }

        async Task<List<TicketDetailsDto>> ITicketService.GetMyTicketsAsync(int currentUserId, UserRole currentUserRole)
        {
            var tickets = await _ticketRepo.GetMyTicketsAsync(currentUserId, currentUserRole);
            return tickets.Select(t => new TicketDetailsDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CategoryName = t.Category.Name,
                Priority = t.Priority,
                Status = t.Status,

                CreatedById = t.CreatedById,
                CreatedByName = t.CreatedBy?.FullName,

                AssignedToId = t.AssignedToId,
                AssignedToName = t.AssignedTo?.FullName,

                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueAt = t.DueAt,
                ResolvedAt = t.ResolvedAt,
                ClosedAt = t.ClosedAt
            }).ToList();
        }

        async Task<PagedResultDto<TicketDetailsDto>> ITicketService.GetAllTicketsAsync(TicketQueryParametersDto? parameters)
        {
            parameters ??= new TicketQueryParametersDto();

            var pageNumber = Math.Max(1, parameters.PageNumber);
            var pageSize   = Math.Clamp(parameters.PageSize, 1, 50);

            var query = _ticketRepo.GetAllTicketsAsync();

            
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim();
                query = query.Where(t => t.Title.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                if (Enum.TryParse<TicketStatus>(parameters.Status, true, out var status))
                    query = query.Where(t => t.Status == status);
                else
                    throw new ArgumentException("Invalid status value.");
            }

            if (!string.IsNullOrWhiteSpace(parameters.Priority))
            {
                if (Enum.TryParse<TicketPriority>(parameters.Priority, true, out var priority))
                    query = query.Where(t => t.Priority == priority);
                else
                    throw new ArgumentException("Invalid priority value.");
            }

            if (parameters.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == parameters.CategoryId.Value);

        
            query = parameters.SortBy?.ToLower() switch
            {
                "title" => parameters.SortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title),

                "status" => parameters.SortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),

                "priority" => parameters.SortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.Priority)
                    : query.OrderBy(t => t.Priority),

                "createdat" => parameters.SortDirection?.ToLower() == "desc"
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),

                _ => query.OrderByDescending(t => t.CreatedAt)
            };

         
            var totalCount = await query.CountAsync();

            var tickets = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TicketDetailsDto
                {
                    Id             = t.Id,
                    Title          = t.Title,
                    Description    = t.Description,
                    CategoryName   = t.Category.Name,
                    Priority       = t.Priority,
                    Status         = t.Status,
                    CreatedById    = t.CreatedById,
                    CreatedByName  = t.CreatedBy.FullName,
                    AssignedToId   = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? t.AssignedTo.FullName : null,
                    CreatedAt      = t.CreatedAt,
                    UpdatedAt      = t.UpdatedAt,
                    DueAt          = t.DueAt,
                    ResolvedAt     = t.ResolvedAt,
                    ClosedAt       = t.ClosedAt
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResultDto<TicketDetailsDto>
            {
                Items      = tickets,
                PageNumber = pageNumber,
                PageSize   = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<TicketDetailsDto> AssignTicketAsync(int ticketId,AssignTicketRequestDto dto)
        {
            var ticket = await _ticketRepo.GetByIdForUpdateAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            var agent = await _userRepo.GetAgentByIdAsync(dto.AgentId);

            if (agent == null)
                throw new KeyNotFoundException("Agent not found or inactive.");

            ticket.AssignedToId = agent.Id;
            ticket.Status = TicketStatus.Assigned;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);
            await _ticketRepo.SaveChangesAsync();

            return await GetByIdAsync(ticket.Id, 0, UserRole.Admin);
        }

        public async Task<TicketDetailsDto> UnassignTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetByIdForUpdateAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            if (ticket.AssignedToId == null)
                throw new InvalidOperationException(
                    "Ticket is not assigned to any agent.");

            ticket.AssignedToId = null;
            ticket.Status = TicketStatus.Open;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);
            await _ticketRepo.SaveChangesAsync();

            
            return await GetByIdAsync( ticket.Id, 0,  UserRole.Admin);
        }

        public async Task<TicketDetailsDto> StartTicketAsync(int ticketId,int currentUserId)
        {
            var ticket = await _ticketRepo.GetByIdForUpdateAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

           
            if (ticket.AssignedToId != currentUserId)
                throw new UnauthorizedAccessException(
                    "You are not assigned to this ticket.");

           
            if (ticket.Status != TicketStatus.Assigned)
                throw new InvalidOperationException(
                    "Only assigned tickets can be started.");

            ticket.Status = TicketStatus.InProgress;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);
            await _ticketRepo.SaveChangesAsync();

            return await GetByIdAsync( ticket.Id,currentUserId, UserRole.Agent);
        }


        public async Task<TicketDetailsDto> ResolveTicketAsync(int ticketId, int currentUserId)
        {
            var ticket = await _ticketRepo
                .GetByIdForUpdateAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            if (ticket.AssignedToId != currentUserId)
                throw new UnauthorizedAccessException(
                    "You are not assigned to this ticket.");

            if (ticket.Status != TicketStatus.InProgress)
                throw new InvalidOperationException(
                    "Only in-progress tickets can be resolved.");

            ticket.Status = TicketStatus.Resolved;
            ticket.ResolvedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);
            await _ticketRepo.SaveChangesAsync();

            return await GetByIdAsync( ticket.Id,currentUserId, UserRole.Agent);
        }

        public async Task<TicketDetailsDto> CloseTicketAsync(int ticketId, int currentUserId, UserRole currentUserRole)
        {
            var ticket = await _ticketRepo.GetByIdForUpdateAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            bool isOwner = ticket.CreatedById == currentUserId;
            bool isAdmin = currentUserRole == UserRole.Admin;
            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("Only the employee who created the ticket or an admin can close it.");
            }

            if (ticket.Status != TicketStatus.Resolved)
                throw new InvalidOperationException("Only resolved tickets can be closed.");

            ticket.Status = TicketStatus.Closed;
            ticket.ClosedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);
            await _ticketRepo.SaveChangesAsync();

            return await GetByIdAsync(ticket.Id, currentUserId, currentUserRole);
        }
    }
}
