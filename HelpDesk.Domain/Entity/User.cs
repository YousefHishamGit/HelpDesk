using HelpDesk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Domain.Entity
{
    public class User : BaseEntity<int>
    {
        public string FullName { get; set; } 

        public string Email { get; set; } 

        public string PasswordHash { get; set; } 

        public string? Phone { get; set; }

        public UserRole Role { get; set; } = UserRole.Employee;

        public bool IsActive { get; set; } = true;

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
