using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.ServiceLayer.Files
{
    static class FileAppender
    {
        public static string AppendFile(this string filename, string filePath)
        {
            int duplicateCount = Directory.GetFiles(filePath, "**" + filename + "**").Length;
            if (duplicateCount > 0)
                filename = $@"{filePath}\{filename}({duplicateCount}).xls";
            else
                filename = $@"{filePath}\{filename}.xls";
            return filename;
        }
    }

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
                    return (formulator?.EvaluateInCell(cell)).GetValue();
                default:
                    return cell.StringCellValue;
            }
        }
    }
}
