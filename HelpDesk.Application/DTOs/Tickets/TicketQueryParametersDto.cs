using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.DTOs.Tickets
{
    public class TicketQueryParametersDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

       
        public string? Search { get; set; }

 
        public string? Status { get; set; }

    
        public string? Priority { get; set; }

       
        public int? CategoryId { get; set; }

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; }
    }
}
