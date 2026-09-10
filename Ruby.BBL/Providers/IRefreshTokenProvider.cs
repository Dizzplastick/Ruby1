using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBL.Providers
{
    public interface IRefreshTokenProvider
    {
        
        public string GenerateRefreshToken();

    }
}
