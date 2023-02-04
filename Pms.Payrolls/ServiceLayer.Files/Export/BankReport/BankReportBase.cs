using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pms.Common.Enums;
using Pms.Payrolls.Services;

namespace Pms.Payrolls.ServiceLayer.Files.Export.BankReport
{
    public class BankReportBase : IExportBankReportService
    {
        private Dictionary<BankChoices, IExportBankReportService> _exporters;

        public BankReportBase(string cutoffId, string payrollCode)
        {
            Cutoff cutoff = new(cutoffId);

            _exporters = new()
            {
                { BankChoices.CHK, new CHKExporter(cutoff, payrollCode) },
                { BankChoices.LBP, new LBPExporter(cutoff, payrollCode) },
                { BankChoices.CBC, new CBCExporter(cutoff, payrollCode, "CBC") },
                { BankChoices.CBC1, new CBCExporter(cutoff, payrollCode, "CBC1") },
                { BankChoices.MPALO, new MBExporter(cutoff, payrollCode, "MPALO") },
                { BankChoices.MTAC, new MBExporter(cutoff, payrollCode, "MTAC") }
            };
        }

        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            Dictionary<BankChoices, List<Payroll>> payrollsByBank = payrolls
                .GroupBy(p => p.EE?.Bank ?? BankChoices.UNKNOWN)
                .Select(pp => pp.ToList())
                .ToDictionary(pp => pp.First().EE?.Bank ?? BankChoices.UNKNOWN);
            foreach (BankChoices bank in payrollsByBank.Keys)
                _exporters[bank].StartExport(payrollsByBank[bank]);
        }
    }
}
