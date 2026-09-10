using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.DAL.Entities
{
    public class RefreshTokenEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }//index

        public UserEntity User { get; set; }

        public string RefreshToken { get; set; }//index

        public DateTime ExpiresAt { get; set; } //index

        public DateTime CreatedAt { get; set; }
    }
}
