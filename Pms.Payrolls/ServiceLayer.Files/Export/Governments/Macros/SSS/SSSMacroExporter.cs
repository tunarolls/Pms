using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments.Macros
{
    public class SSSMacroExporter
    {
        private readonly string _exportDirectory;
        private readonly string _exportFilename;
        private readonly string _templateFilename;

        public SSSMacroExporter(Cutoff cutoff, string companyId)
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
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            var sheetWriter = new RegularSheetWriter(payrolls, new SSSRowWriter());
            sheetWriter.Write(nSheet);

            using var nReportFile = new FileStream(_exportFilename, FileMode.Open, FileAccess.Write);
            nWorkbook.Write(nReportFile, false);
        }
    }
}
