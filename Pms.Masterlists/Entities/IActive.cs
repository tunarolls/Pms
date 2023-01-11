using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Entities
{
    public interface IActive
    {
        bool Active { get; set; }
        string EEId { get; set; }
    }
}
