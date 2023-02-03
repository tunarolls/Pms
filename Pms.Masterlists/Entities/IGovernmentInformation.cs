using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Entities
{
    public interface IGovernmentInformation
    {
        DateTime? BirthDate { get; set; }
        string BirthDateSetter { set; }
        string EEId { get; set; }
        string Pagibig { get; set; }
        string PhilHealth { get; set; }
        string SSS { get; set; }
        string TIN { get; set; }
    }
}
