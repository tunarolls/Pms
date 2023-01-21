using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class BenefitsBMacroExporter
    {
        private readonly string ExportDirectory;
        private readonly string ExportFilename;
        private readonly string TemplateFilename;


        public BenefitsBMacroExporter(Cutoff cutoff, string companyId)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            ExportDirectory = $@"{startupPath}\EXPORT\{cutoff.CutoffId}\GOVERNMENT\MACRO";
            ExportFilename = $@"{ExportDirectory}\{companyId}_{cutoff.CutoffDate:yyyyMMdd}-MACROB.xls";
            TemplateFilename = $@"{startupPath}\TEMPLATES\MACROB.xls";

            Directory.CreateDirectory(ExportDirectory);
        }


        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            File.Copy(TemplateFilename, ExportFilename, true);

            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(ExportFilename, FileMode.Open, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);


            RegularSheetWriter regularWriter = new(payrolls);
            regularWriter.RowWriter = new PagibigCBCRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(0),3);

            regularWriter.RowWriter = new PagibigMBRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(1), 3);

            regularWriter.RowWriter = new PhilHealthBRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(2), 4);

            regularWriter.RowWriter = new SSSBRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(3), 3);




            using (var nReportFile = new FileStream(ExportFilename, FileMode.Open, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }


    }
}
