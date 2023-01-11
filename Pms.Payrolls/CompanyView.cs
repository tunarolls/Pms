using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls
{
    public class CompanyView
    {
        public CompanyView() { }

        public CompanyView(string registeredName, string tin, int branchCode, string region)
        {
            RegisteredName = registeredName;
            TIN = tin;
            BranchCode = branchCode;
            Region = region;
        }

        public string Acronym { get; set; } = string.Empty;
        public int BranchCode { get; set; }
        public string CompanyId { get; set; } = string.Empty;
        public double MinimumRate { get; set; }
        public string Region { get; set; } = string.Empty;
        public string RegisteredName { get; set; } = string.Empty;
        public string Site { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;

        public override string ToString() => CompanyId;
    }
}
