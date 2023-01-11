using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Entities
{
    public interface IHRMSInformation
    {
        string EEId { get; set; }
        string FirstName { get; set; }
        string JobCode { get; set; }
        string LastName { get; set; }
        string Location { get; set; }
        string MiddleName { get; set; }
    }
}
