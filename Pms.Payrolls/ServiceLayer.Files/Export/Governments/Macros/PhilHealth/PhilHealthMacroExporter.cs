using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class PhilHealthMacroExporter
    {
        private readonly string ExportDirectory;
        private readonly string ExportFilename;
        private readonly string TemplateFilename;

        private ISheetWriter SheetWriter;

        public PhilHealthMacroExporter(Cutoff cutoff, string companyId)
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
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            SheetWriter = new RegularSheetWriter(payrolls, new PhilHealthRowWriter());
            SheetWriter.Write(nSheet);

            using (var nReportFile = new FileStream(ExportFilename, FileMode.Open, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);
        }


    }
}
