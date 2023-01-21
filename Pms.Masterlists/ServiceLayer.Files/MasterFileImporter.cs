using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Masterlists.Entities;
using System.Collections.Generic;
using System.IO;

namespace Pms.Masterlists.ServiceLayer.Files
{
    public class MasterFileImporter
    {
        //public IEnumerable<IMasterFileInformation> StartImport(string fileName)
        //{
        //    IWorkbook nWorkbook;
        //    using (var nTemplateFile = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
        //        nWorkbook = new HSSFWorkbook(nTemplateFile);
        //    HSSFFormulaEvaluator formulator = new HSSFFormulaEvaluator(nWorkbook);
        //    ISheet nSheet = nWorkbook.GetSheetAt(0);

        //    List<IMasterFileInformation> employeeBankInformations = new();
        //    int i = 1;
        //    while (i <= nSheet.LastRowNum)
        //    {
        //        IRow row = nSheet.GetRow(i);
        //        if (ValidateRow(row) == false) break;

        //        IMasterFileInformation info = new Employee();
        //        info.EEId = row.GetCell(0).GetValue(formulator);
        //        info.JobCode = row.GetCell(3).GetValue(formulator);

        //        employeeBankInformations.Add(info);
        //        i++;
        //    }

        //    return employeeBankInformations;
        //}

        public static IEnumerable<IMasterFileInformation> StartImport(string fileName)
        {
            using var nTemplateFile = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
            var nWorkbook = new HSSFWorkbook(nTemplateFile);
            var formulator = new HSSFFormulaEvaluator(nWorkbook);
            var nSheet = nWorkbook.GetSheetAt(0);
            int i = 1;

            while (i <= nSheet.LastRowNum)
            {
                var row = nSheet.GetRow(i);
                if (!ValidateRow(row)) break;

                var info = new Employee
                {
                    EEId = row.GetCell(0).GetValue(formulator),
                    JobCode = row.GetCell(3).GetValue(formulator)
                };

                i++;

                yield return info;
            }
        }

        private static bool ValidateRow(IRow? row)
        {
            if (row == null) return false;
            var cell = row.GetCell(0);
            if (cell == null) return false;
            if (string.IsNullOrEmpty(cell.GetValue())) return false;
            if (cell.GetValue().Trim().Length != 4) return false;

            return true;
        }
    }
}
