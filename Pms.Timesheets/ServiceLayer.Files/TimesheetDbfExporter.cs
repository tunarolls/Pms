using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.ServiceLayer.Files
{
    public class TimesheetDbfExporter
    {
        public static void ExportDbf(string location, DateTime payrollDate, IEnumerable<Timesheet> timesheets)
        {
            if (timesheets.Any())
            {
                Stream dbfStream = File.Create(location);

                DotNetDBF.DBFWriter dbfWriter = new(dbfStream);
                dbfWriter.CharEncoding = Encoding.UTF8;
                dbfWriter.Fields = GetDBFFields().ToArray();

                int CODE = payrollDate.Day == 15 ? 1 : 2;
                for (int r = 0, loopTo = timesheets.Count() - 1; r <= loopTo; r++)
                {
                    dbfWriter.WriteRecord(ToDBFRecordFormat(timesheets.ElementAt(r), CODE, 0));
                }

                dbfWriter.Close();
                dbfStream.Close();
            }
        }

        private static List<DotNetDBF.DBFField> GetDBFFields()
        {
            List<DotNetDBF.DBFField> flds = new()
            {
                new DotNetDBF.DBFField("DATER", DotNetDBF.NativeDbType.Numeric, 10, 0),
                new DotNetDBF.DBFField("CODE", DotNetDBF.NativeDbType.Numeric, 10, 0),
                new DotNetDBF.DBFField("ID", DotNetDBF.NativeDbType.Char, 4, 0),
                new DotNetDBF.DBFField("REG_HRS", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("R_OT", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("RD_OT", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("RD_8", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("HOL_OT", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("HOL_OT8", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("ND", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("ABS_TAR", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("ADJUST1", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("GROSS_PAY", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("ADJUST2", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("TAX", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("SSS_EE", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("SSS_ER", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("PHIC", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("NET_PAY", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("REG_PAY", DotNetDBF.NativeDbType.Float, 10, 2),
                new DotNetDBF.DBFField("TAG", DotNetDBF.NativeDbType.Char, 1, 0)
            };
            return flds;
        }

        private static string[] ToDBFRecordFormat(Timesheet timesheet, int CODE, int DATER) =>
            new[] { DATER.ToString(),
                CODE.ToString(),
                timesheet.EEId ?? string.Empty,
                timesheet.TotalHours.ToString(),
                timesheet.TotalOT.ToString(),
                timesheet.TotalRDOT.ToString(),
                0.ToString(),
                timesheet.TotalHOT.ToString(),
                0.ToString(),
                timesheet.TotalND.ToString(),
                timesheet.TotalTardy.ToString(),
                0.ToString(),
                0.ToString(),
                0.ToString(),
                0.ToString(),
                0.ToString(), 0.ToString(), 0.ToString(),
                0.ToString(), 0.ToString(), 0.ToString()
            };
    }
}
