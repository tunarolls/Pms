using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.Hrms.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException() : base()
        {

        }

        public InvalidRequestException(string? response) : base(response)
        {
        }
    }
}
