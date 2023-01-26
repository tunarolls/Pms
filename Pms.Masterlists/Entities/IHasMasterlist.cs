using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Entities
{
    public interface IHasMasterlist
    {
        public Company? Company { get; }
        public PayrollCode? PayrollCode { get; }
        public IEnumerable<PayrollCode> PayrollCodes { get; }
    }
}
