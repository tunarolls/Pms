using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Pms.Masterlists.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pms.Masterlists.ServiceLayer.Files
{
    public class EmployeeEEDataImporter
    {
        public IEnumerable<IEEDataInformation> StartImport(string fileName)
        {
            IWorkbook nWorkbook;
            using (var nTemplateFile = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                nWorkbook = new HSSFWorkbook(nTemplateFile);
            HSSFFormulaEvaluator formulator = new(nWorkbook);
            ISheet nSheet = nWorkbook.GetSheetAt(0);

            List<IEEDataInformation> employees = new();
            int i = 6;
            while (i <= nSheet.LastRowNum)
            {
                IRow row = nSheet.GetRow(i);
                if (ValidateRow(row, formulator) == true)
                {
                    Employee employee = new();
                    try
                    {
                        employee.EEId = row.GetCell(1).GetValue(formulator).Trim();
                        employee.LastName = row.GetCell(2).GetValue(formulator).Trim();
                        employee.FirstName = row.GetCell(3).GetValue(formulator).Trim();
                        employee.MiddleName = row.GetCell(4).GetValue(formulator).Trim();
                        employee.NameExtension = row.GetCell(5).GetValue(formulator).Trim();
                        employee.Gender = row.GetCell(6).GetValue(formulator).Trim();
                        employee.BirthDateSetter = row.GetCell(7).GetValue(formulator).Trim();
                        employee.SSS = row.GetCell(8).GetValue(formulator)
                            .Replace(" ", "")
                            .Replace("-", "")
                            .Replace("'", "");
                        employee.PhilHealth = row.GetCell(9).GetValue(formulator)
                            .Replace(" ", "")
                            .Replace("-", "")
                            .Replace("'", "");
                        employee.TIN = row.GetCell(10).GetValue(formulator)
                            .Replace(" ", "")
                            .Replace("-", "")
                            .Replace("'", "");
                        employee.Pagibig = row.GetCell(11).GetValue(formulator)
                            .Replace(" ", "")
                            .Replace("-", "")
                            .Replace("'", "");

                        employee.ValidateAll();

                        IEEDataInformation personal = employee;
                        employees.Add(personal);
                    }
                    catch (Exception ex)
                    {
                        row.GetCell(0).SetCellValue(ex.Message);
                        row.RowStyle = Styles.ErrorRow(nWorkbook);
                    }
                }
                i++;
            }


            using (var nReportFile = new FileStream(fileName, FileMode.Open, FileAccess.Write))
                nWorkbook.Write(nReportFile, false);


            return employees;
        }

        private bool ValidateRow(IRow row, HSSFFormulaEvaluator formulator)
        {
            if (row is null) return false;

            ICell cell = row.GetCell(1);
            if (cell is null) return false;
            if (cell.GetValue(formulator) == string.Empty) return false;
            if (cell.GetValue(formulator).Trim().Length != 4) return false;

            return true;
        }
    }
}
