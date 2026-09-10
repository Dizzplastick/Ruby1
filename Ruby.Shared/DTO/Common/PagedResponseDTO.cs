using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.Common
{
    public class PagedResponseDTO<T>
    {
        public List<T> Items { get; set; } = new();
        
        public int TotalCount { get; set; }
        
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public bool HasNextPage => CurrentPage * PageSize < TotalCount;
        public bool HasPreviousPage => CurrentPage > 1;
    }
}
