using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Pms.Timesheets
{
    public class Cutoff
    {
        public Cutoff()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            if (DateTime.Now.Day < 20)
                CutoffDate = new DateTime(year, month, 15);
            else
                CutoffDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            GetCutoffId();
            GetCutoffRange();
        }

        public Cutoff(DateTime cutoffDate)
        {
            CutoffDate = cutoffDate;

            GetCutoffId();
            GetCutoffRange();
        }

        public Cutoff(string cutoffId)
        {
            CutoffId = cutoffId;

            GetCutoffDate();
            GetCutoffRange();
        }

        public DateTime CutoffDate { get; private set; }
        public string CutoffId { get; private set; } = string.Empty;
        public DateTime[] CutoffRange { get; private set; } = new DateTime[] { };
        public string Site { get; private set; } = string.Empty;
        public string GetPreviousCutoff() => new Cutoff(CutoffDate.AddDays(-15)).CutoffId;

        public void SetSite(string site)
        {
            Site = site;
            GetCutoffRange();
        }

        public override string ToString() => CutoffId;

        private void GetCutoffDate()
        {
            int year = int.Parse(CutoffId.Substring(0, 2));
            int month = int.Parse(CutoffId.Substring(2, 2));
            int dayIdx = int.Parse(CutoffId.Substring(5, 1));
            int day = 15;
            if (dayIdx == 2)
                day = DateTime.DaysInMonth(year, month);

            CutoffDate = new DateTime(year + 2000, month, day); // update year to 3000 at the end of the millennium
        }

        private void GetCutoffId()
        {
            if (CutoffDate.Day <= 15) CutoffId = $"{CutoffDate:yyMM}-1";
            else CutoffId = $"{CutoffDate:yyMM}-2";
        }

        private void GetCutoffRange()
        {
            if (Site == "LEYTE")
            {
                if (new[] { 28, 29, 30, 31 }.Contains(CutoffDate.Day))
                    CutoffRange = new[] { new DateTime(CutoffDate.Year, CutoffDate.Month, 4), new DateTime(CutoffDate.Year, CutoffDate.Month, 18) };
                else if (15 == CutoffDate.Day)
                {
                    var previousMonth = CutoffDate.AddMonths(-1);
                    CutoffRange = new[] { new DateTime(previousMonth.Year, previousMonth.Month, 19), new DateTime(CutoffDate.Year, CutoffDate.Month, 3) };
                }
            }
            else
            {
                if (new[] { 28, 29, 30, 31 }.Contains(CutoffDate.Day))
                    CutoffRange = new[] { new DateTime(CutoffDate.Year, CutoffDate.Month, 5), new DateTime(CutoffDate.Year, CutoffDate.Month, 19) };
                else if (15 == CutoffDate.Day)
                {
                    var previousMonth = CutoffDate.AddMonths(-1);
                    CutoffRange = new[] { new DateTime(previousMonth.Year, previousMonth.Month, 20), new DateTime(CutoffDate.Year, CutoffDate.Month, 4) };
                }
            }
        }
    }
}
