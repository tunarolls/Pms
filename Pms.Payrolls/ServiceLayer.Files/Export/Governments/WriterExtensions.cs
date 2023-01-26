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
            return row.GetCell(index) ?? row.CreateCell(index);
        }
    
        public static IRow GetOrCreateRow(this ISheet sheet, int index)
        {
            return sheet.GetRow(index) ?? sheet.CreateRow(index);
        }
    }
}
