using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Domain.Entity
{
    public class Ticket : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public TicketPriority Priority { get; set; } 
        public TicketStatus Status { get; set; }


        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }


        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int CreatedById { get; set; } 
        public User? CreatedBy { get; set; }

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }


        public ICollection<TicketComment> Comments { get; set; }= new List<TicketComment>();
    }
}
