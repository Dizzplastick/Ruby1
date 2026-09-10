using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ruby.DAL.Entities
{
    public class LikeEntity
    {
        public Guid UserId { get; set; }

        public UserEntity User { get; set; }

        public Guid TrackId { get; set; }

        public TrackEntity Track { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
