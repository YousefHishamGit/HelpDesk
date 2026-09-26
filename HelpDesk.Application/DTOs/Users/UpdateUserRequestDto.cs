using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.DTOs.Users
{
    public class UpdateUserRequestDto
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
    }
}
