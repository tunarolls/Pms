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
        private readonly string ExportDirectory;
        private readonly string ExportFilename;
        private readonly string TemplateFilename;


        public BenefitsMacroExporter(Cutoff cutoff, string companyId)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            ExportDirectory = $@"{startupPath}\EXPORT\{cutoff.CutoffId}\GOVERNMENT\MACRO";
            ExportFilename = $@"{ExportDirectory}\{companyId}_{cutoff.CutoffDate:yyyyMMdd}-MACRO.xls";
            TemplateFilename = $@"{startupPath}\TEMPLATES\MACRO.xls";

            Directory.CreateDirectory(ExportDirectory);
        }


        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            File.Copy(TemplateFilename, ExportFilename, true);

            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(ExportFilename, FileMode.Open, FileAccess.ReadWrite))
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




            using (var nReportFile = new FileStream(ExportFilename, FileMode.Open, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }


    }
}
