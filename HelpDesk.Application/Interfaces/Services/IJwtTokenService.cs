using HelpDesk.Application.DTOs.Token;
using HelpDesk.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {

        public AccessTokenResult GenerateAccessToken(User user);
        public RefreshToken GenerateRefreshToken();

    }
}
