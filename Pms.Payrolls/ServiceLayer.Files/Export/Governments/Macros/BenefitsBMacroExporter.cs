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
        private readonly string _exportDirectory;
        private readonly string _exportFilename;
        private readonly string _templateFilename;


        public BenefitsBMacroExporter(Cutoff cutoff, string companyId)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            _exportDirectory = $@"{startupPath}\EXPORT\{cutoff.CutoffId}\GOVERNMENT\MACRO";
            _exportFilename = $@"{_exportDirectory}\{companyId}_{cutoff.CutoffDate:yyyyMMdd}-MACROB.xls";
            _templateFilename = $@"{startupPath}\TEMPLATES\MACROB.xls";

            Directory.CreateDirectory(_exportDirectory);
        }


        public void StartExport(IEnumerable<Payroll> payrolls)
        {
            File.Copy(_templateFilename, _exportFilename, true);

            using var nTemplateFile = new FileStream(_exportFilename, FileMode.Open, FileAccess.ReadWrite);
            var nWorkbook = new HSSFWorkbook(nTemplateFile);
            var regularWriter = new RegularSheetWriter(payrolls);

            regularWriter.RowWriter = new PagibigCBCRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(0),3);

            regularWriter.RowWriter = new PagibigMBRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(1), 3);

            regularWriter.RowWriter = new PhilHealthBRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(2), 4);

            regularWriter.RowWriter = new SSSBRowWriter();
            regularWriter.Write(nWorkbook.GetSheetAt(3), 3);

            using var nReportFile = new FileStream(_exportFilename, FileMode.Open, FileAccess.Write);
            nWorkbook.Write(nReportFile, false);
        }
    }
}
