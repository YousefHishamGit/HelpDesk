using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Domain.Entity
{
    public class RefreshToken :BaseEntity<int>
    {
       
        public string  Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
