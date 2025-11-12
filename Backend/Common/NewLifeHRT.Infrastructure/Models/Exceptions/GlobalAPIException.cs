using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Models.Exceptions
{
    public class GlobalAPIException : Exception
    {
        public int StatusCode { get; }

        public GlobalAPIException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public GlobalAPIException(string message, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
