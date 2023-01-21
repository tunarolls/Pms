using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments
{
    static class WriterExtensions
    {
        public static ICell GetOrCreateCell(this IRow row, int index)
        {
            if (row.GetCell(index) is not null)
                return row.GetCell(index);
            else
                return row.CreateCell(index);
        }
    
        public static IRow GetOrCreateRow(this ISheet sheet, int index)
        {
            if (sheet.GetRow(index) is not null)
                return sheet.GetRow(index);
            else
                return sheet.CreateRow(index);
        }
    }
}
