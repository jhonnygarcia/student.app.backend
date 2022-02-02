using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Dto
{
    public class ErrorResponse
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }
}
