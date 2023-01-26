using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class BenefitsMacroExporter
    {
        private readonly string _exportDirectory;
        private readonly string _exportFilename;
        private readonly string _templateFilename;

        public BenefitsMacroExporter(Cutoff cutoff, string companyId)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            _exportDirectory = $@"{startupPath}\EXPORT\{cutoff.CutoffId}\GOVERNMENT\MACRO";
            _exportFilename = $@"{_exportDirectory}\{companyId}_{cutoff.CutoffDate:yyyyMMdd}-MACRO.xls";
            _templateFilename = $@"{startupPath}\TEMPLATES\MACRO.xls";

            Directory.CreateDirectory(_exportDirectory);
        }


        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            File.Copy(_templateFilename, _exportFilename, true);

            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(_exportFilename, FileMode.Open, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);



            SpecialSheetWriter specialWriter = new(payrolls);
            specialWriter.RowWriter = new PagibigRowWriter();
            specialWriter.Write(nWorkbook.GetSheetAt(0), 3);

            specialWriter.RowWriter = new BenefitsRowWriter();
            specialWriter.Write(nWorkbook.GetSheetAt(3), 3);

            IEnumerable<Payroll> wtaxPayrolls = payrolls.Where(p => p.WithholdingTax > 0.01);
            SpecialSheetWriter wtaxSpecialWriter = new(wtaxPayrolls, new WithholdingTaxRowWriter());
            wtaxSpecialWriter.Write(nWorkbook.GetSheetAt(4), 3);


            RegularSheetWriter regularWriter = new(payrolls);

            regularWriter.RowWriter = new PhilHealthRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(1), 3);

            regularWriter.RowWriter = new SSSRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(2), 3);




            using (var nReportFile = new FileStream(_exportFilename, FileMode.Open, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }


    }
}
