using Pms.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common
{
    public class PayrollCode
    {
        public PayrollCode() { }

        public PayrollCode(string name, string site)
        {
            Name = name;
            Site = site;
        }

        public string CompanyId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PayrollCodeId { get; set; } = string.Empty;
        //public CompanyView Company { get; set; }
        public PayrollRegisterTypes Process { get; set; }
        public string Site { get; set; } = string.Empty;

        public static string GenerateId(PayrollCode payrollCode) => $"{payrollCode.Site[0]}-{payrollCode.Name}";

        public override string ToString()
        {
            return PayrollCodeId;
        }
    }
}
