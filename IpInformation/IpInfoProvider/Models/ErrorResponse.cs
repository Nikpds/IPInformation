using System;
using System.Collections.Generic;
using System.Text;

namespace IpInfoProvider.Models
{
    class ErrorResponse
    {
        public bool Success { get; set; }
        public Error Error { get; set; }
    }
    class Error
    {
        public short Code { get; set; }
        public string Type { get; set; }
        public string Info { get; set; }
    }
}
