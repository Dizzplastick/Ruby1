using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ruby.DAL.Entities;


namespace BBL.Providers
{
    public interface IJwtTokenProvider
    {

        public string GenerateJwtToken(UserEntity User);
    }
}
