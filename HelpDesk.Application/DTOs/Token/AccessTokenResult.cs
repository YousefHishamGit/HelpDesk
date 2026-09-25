using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.DTOs.Token
{
    public class AccessTokenResult
    {
        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }
    }
}
