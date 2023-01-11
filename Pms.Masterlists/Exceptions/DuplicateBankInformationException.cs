using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Exceptions
{
    public class DuplicateBankInformationException : Exception
    {
        public DuplicateBankInformationException(string eeId, string eeIdFound, string field, string value)
            : base($"{eeId} has the same {field} of {value} as {eeIdFound}.") { }
    }
}
