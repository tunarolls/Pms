using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class TimesheetFeedbackExporter
    {
        public TimesheetFeedbackExporter(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank, List<Timesheet> timesheets, List<Timesheet> unconfirmedTimesheetsWithAttendance, List<Timesheet> unconfirmedTimesheetsWithoutAttendance)
        {
            Cutoff = cutoff;
            PayrollCode = payrollCode;
            Bank = bank;
            Timesheets = timesheets;
            UnconfirmedTimesheetsWithAttendance = unconfirmedTimesheetsWithAttendance;
            UnconfirmedTimesheetsWithoutAttendance = unconfirmedTimesheetsWithoutAttendance;
        }

        private TimesheetBankChoices Bank { get; set; }
        private Cutoff Cutoff { get; set; }
        private string PayrollCode { get; set; }
        private List<Timesheet> Timesheets { get; set; }
        private List<Timesheet> UnconfirmedTimesheetsWithAttendance { get; set; }
        private List<Timesheet> UnconfirmedTimesheetsWithoutAttendance { get; set; }

        public void StartExport(string filePath)
        {
            if (Timesheets.Count == 0) return;

            IWorkbook nWorkbook = new HSSFWorkbook();
            ISheet nSheet = nWorkbook.CreateSheet(Cutoff.CutoffId);
            WritePayRegisterInfo(nSheet);
            WriteHeader(nSheet);

            int currentRowIndex = 3;
            currentRowIndex = WriteTimesheets(UnconfirmedTimesheetsWithAttendance, nSheet, currentRowIndex, "UNCONFIRMED TIMESHEETS(WITH ATTENDANCE)");
            currentRowIndex = WriteTimesheets(UnconfirmedTimesheetsWithoutAttendance, nSheet, currentRowIndex, "UNCONFIRMED TIMESHEETS(WITHOUT ATTENDANCE)");
            _ = WriteTimesheets(Timesheets, nSheet, currentRowIndex, "CONFIRMED TIMESHEETS");

            using var nEFile = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            nWorkbook.Write(nEFile, false);
        }

        private static void WriteEERow(IRow row, Timesheet timesheet)
        {
            row.CreateCell(1).SetCellValue(timesheet.EEId);
            if (timesheet.EE is not null)
            {
                row.CreateCell(2).SetCellValue(timesheet.EE.Fullname);
                row.CreateCell(3).SetCellValue(timesheet.EE.Location);
            }
            row.CreateCell(4).SetCellValue(timesheet.TotalHours);
            row.CreateCell(5).SetCellValue(timesheet.TotalOT);
            row.CreateCell(6).SetCellValue(timesheet.TotalRDOT);
            row.CreateCell(7).SetCellValue(timesheet.TotalHOT);
            row.CreateCell(8).SetCellValue(timesheet.TotalND);
            row.CreateCell(9).SetCellValue(timesheet.TotalTardy);
            row.CreateCell(10).SetCellValue(timesheet.Allowance);
        }

        private static void WriteHeader(ISheet nSheet)
        {
            IRow row = nSheet.CreateRow(3);
            row.CreateCell(0).SetCellValue("#");
            row.CreateCell(1).SetCellValue("# ID");
            row.CreateCell(2).SetCellValue("NAME");
            row.CreateCell(3).SetCellValue("DEPT");
            row.CreateCell(4).SetCellValue("REG HRS");
            row.CreateCell(5).SetCellValue("R OT");
            row.CreateCell(6).SetCellValue("RD OT");
            row.CreateCell(7).SetCellValue("HOL OT");
            row.CreateCell(8).SetCellValue("ND");
            row.CreateCell(9).SetCellValue("TARDY");
            row.CreateCell(10).SetCellValue("ALLOWANCE");
        }

        private void WritePayRegisterInfo(ISheet nSheet)
        {
            IRow nRow = nSheet.CreateRow(0);
            nRow.CreateCell(2).SetCellValue($"{PayrollCode} - {Bank}");

            nRow = nSheet.CreateRow(1);
            nRow.CreateCell(2).SetCellValue($"{Cutoff.CutoffRange?[0]:MMMM d} - {Cutoff.CutoffRange?[1]:MMMM dd, yyyy}");
        }

        private int WriteTimesheets(List<Timesheet> timesheets, ISheet sheet, int currentRowIndex, string header = "")
        {
            IRow row;
            currentRowIndex++;
            currentRowIndex++;
            sheet.CreateRow(currentRowIndex).CreateCell(0).SetCellValue(header);
            currentRowIndex++;

            for (int r = 0, loopTo = timesheets.Count - 1; r <= loopTo; r++)
            {
                row = sheet.CreateRow(currentRowIndex + r);
                row.CreateCell(0).SetCellValue(r + 1);
                WriteEERow(row, timesheets[r]);
            }

            return currentRowIndex += timesheets.Count;
        }
    }
}
