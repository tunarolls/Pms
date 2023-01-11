using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.Files
{
    static class Evaluator
    {
        public static string GetValue(this ICell? cell, HSSFFormulaEvaluator? formulator = null)
        {
            if (cell is null) return string.Empty;

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        DateTime date = cell.DateCellValue;
                        //ICellStyle style = cell.CellStyle;
                        //string format = Regex.Replace(style.GetDataFormatString(), @"[@;]", "").Replace('m', 'M');
                        //return date.ToString(format);
                        return date.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        double numericValue = cell.NumericCellValue;
                        ICellStyle style = cell.CellStyle;
                        string format = style.GetDataFormatString();
                        if (format != "General")
                            return numericValue.ToString(format);
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Formula:
                    return GetValue(formulator?.EvaluateInCell(cell));
                default:
                    return cell.StringCellValue;
            }
        }
    }
}
