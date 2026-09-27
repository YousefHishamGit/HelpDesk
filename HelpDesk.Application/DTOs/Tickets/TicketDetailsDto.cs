using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.DTOs.Tickets
{
    public class TicketDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public TicketPriority Priority { get; set; }

        public TicketStatus Status { get; set; }

        public int CreatedById { get; set; }

        public string CreatedByName { get; set; } = null!;

        public int? AssignedToId { get; set; }

        public string? AssignedToName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DueAt { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}
