using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.Common
{
    public class ApiErrorResponseDTO
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public Dictionary<string, string[]>? ValidationErrors { get; set; }
    }
}
