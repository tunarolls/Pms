using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Masterlists.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Pms.Masterlists.ServiceLayer.Files
{
    public class EmployeeBankInformationImporter
    {
        public IEnumerable<IBankInformation> StartImport(string fileName)
        {
            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);
            HSSFFormulaEvaluator formulator = new HSSFFormulaEvaluator(nWorkbook);
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            List<IBankInformation> employeeBankInformations = new();
            int i = 1;
            while (i <= nSheet.LastRowNum)
            {
                IRow row = nSheet.GetRow(i);
                if (ValidateRow(row) == false) break;

                IBankInformation bankInfo = new Employee();
                bankInfo.EEId = row.GetCell(1).GetValue(formulator);

                Regex trimmer = new("\\s{2,}");
                
                bankInfo.LastName = trimmer.Replace(row.GetCell(2).GetValue(formulator), " ").Trim();
                bankInfo.FirstName = trimmer.Replace(row.GetCell(3).GetValue(formulator), " ").Trim();
                bankInfo.MiddleName = trimmer.Replace(row.GetCell(4).GetValue(formulator), " ").Trim();

                bankInfo.NameExtension = row.GetCell(5).GetValue(formulator).Trim();
                bankInfo.AccountNumber = row.GetCell(6).GetValue(formulator);
                bankInfo.CardNumber = row.GetCell(7).GetValue(formulator);
                bankInfo.BankSetter = row.GetCell(8).GetValue(formulator);
                bankInfo.PayrollCode = row.GetCell(9).GetValue(formulator);
                
                employeeBankInformations.Add(bankInfo);
                i++;
            }

            return employeeBankInformations;
        }

        private bool ValidateRow(IRow row)
        {
            if (row is null) return false;

            ICell cell = row.GetCell(1);
            if (cell is null) return false;
            if (cell.GetValue() == string.Empty) return false;
            if (cell.GetValue().Trim().Length != 4) return false;

            return true;
        }
    }
}
