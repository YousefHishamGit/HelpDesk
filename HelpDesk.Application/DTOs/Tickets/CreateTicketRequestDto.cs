using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.DTOs.Tickets
{
    public class CreateTicketRequestDto
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int CategoryId { get; set; }

        public TicketPriority Priority { get; set; }
    }
}
