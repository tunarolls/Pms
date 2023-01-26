using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Entities
{
    public class Company
    {
        public string Acronym { get; set; } = string.Empty;
        public int BranchCode { get; set; }
        public string CompanyId { get; set; } = string.Empty;
        public double MinimumRate { get; set; }
        public string Region { get; set; } = string.Empty;
        public string RegisteredName { get; set; } = string.Empty;
        public string Site { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;

        public static string GenerateId(Company company) => $"{company.Site[0]}{company.Acronym}{company.BranchCode:00}";

        public override string ToString() => CompanyId;
    }
}
