using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.ServiceLayer.Files
{
    static class Evaluator
    {
        public static string GetValue(this ICell cell, HSSFFormulaEvaluator formulator = null)
        {
            if (cell is null) return string.Empty;

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        DateTime date = cell.DateCellValue;
                        ICellStyle style = cell.CellStyle;
                        string format = style.GetDataFormatString().Replace('m', 'M');
                        return date.ToString(format);
                    }
                    else
                    {
                        double numericValue = cell.NumericCellValue;
                        ICellStyle style = cell.CellStyle;
                        string format = style.GetDataFormatString();
                        if (format != "General")
                            return numericValue.ToString(format
                                .Replace("*", "")
                                .Replace(")", "")
                                .Replace("(", "")
                                .Replace("_", "")
                                .Split(";")[0]
                                );
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Formula:
                    return GetValue(formulator.EvaluateInCell(cell));
                default:
                    return cell.StringCellValue;
            }
        }
    }
}