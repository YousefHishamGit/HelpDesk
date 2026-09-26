using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.DTOs.Users
{
    public class UserQueryParametersDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public string? Role { get; set; }

        public bool? IsActive { get; set; }

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; }
    }
}
