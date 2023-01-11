using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class TimesheetDbfExporter
    {
        public void ExportDBF(string location, DateTime payrollDate, List<Timesheet> timesheets)
        {
            if (timesheets.Any())
            {
                Stream dbfStream = File.Create(location);

                DotNetDBF.DBFWriter dbfWriter = new(dbfStream);
                dbfWriter.CharEncoding = Encoding.UTF8;
                dbfWriter.Fields = GetDBFFields().ToArray();

                int CODE = payrollDate.Day == 15 ? 1 : 2;
                for (int r = 0, loopTo = timesheets.Count - 1; r <= loopTo; r++)
                {
                    dbfWriter.WriteRecord(ToDBFRecordFormat(timesheets[r], CODE, 0));
                }

                dbfWriter.Close();
                dbfStream.Close();
            }
        }

        private List<DotNetDBF.DBFField> GetDBFFields()
        {
            List<DotNetDBF.DBFField> flds = new();
            flds.Add(new DotNetDBF.DBFField("DATER", DotNetDBF.NativeDbType.Numeric, 10, 0));
            flds.Add(new DotNetDBF.DBFField("CODE", DotNetDBF.NativeDbType.Numeric, 10, 0));
            flds.Add(new DotNetDBF.DBFField("ID", DotNetDBF.NativeDbType.Char, 4, 0));
            flds.Add(new DotNetDBF.DBFField("REG_HRS", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("R_OT", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("RD_OT", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("RD_8", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("HOL_OT", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("HOL_OT8", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("ND", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("ABS_TAR", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("ADJUST1", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("GROSS_PAY", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("ADJUST2", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("TAX", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("SSS_EE", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("SSS_ER", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("PHIC", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("NET_PAY", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("REG_PAY", DotNetDBF.NativeDbType.Float, 10, 2));
            flds.Add(new DotNetDBF.DBFField("TAG", DotNetDBF.NativeDbType.Char, 1, 0));
            return flds;
        }

        private string[] ToDBFRecordFormat(Timesheet timesheet, int CODE, int DATER) =>
            new[] { DATER.ToString(),
                CODE.ToString(),
                timesheet.EEId,
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
