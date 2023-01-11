using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Masterlists.Entities;
using System.Collections.Generic;
using System.IO;

namespace Pms.Masterlists.ServiceLayer.Files
{
    public class MasterFileImporter
    {
        public IEnumerable<IMasterFileInformation> StartImport(string fileName)
        {
            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);
            HSSFFormulaEvaluator formulator = new HSSFFormulaEvaluator(nWorkbook);
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            List<IMasterFileInformation> employeeBankInformations = new();
            int i = 1;
            while (i <= nSheet.LastRowNum)
            {
                IRow row = nSheet.GetRow(i);
                if (ValidateRow(row) == false) break;

                IMasterFileInformation info = new Employee();
                info.EEId = row.GetCell(0).GetValue(formulator);
                info.JobCode = row.GetCell(3).GetValue(formulator);

                employeeBankInformations.Add(info);
                i++;
            }

            return employeeBankInformations;
        }

        private bool ValidateRow(IRow row)
        {
            if (row is null) return false;

            ICell cell = row.GetCell(0);
            if (cell is null) return false;
            if (cell.GetValue() == string.Empty) return false;
            if (cell.GetValue().Trim().Length != 4) return false;

            return true;
        }
    }
}
