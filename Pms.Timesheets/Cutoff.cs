using Pms.Common.Enums;
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
            var cutoffDate = GetCutoffDate();
            CutoffDate = cutoffDate;
            CutoffId = GetCutoffId(cutoffDate);
            CutoffRange = GetCutoffRange(CutoffDate, Site);
        }

        public Cutoff(DateTime cutoffDate)
        {
            CutoffDate = cutoffDate;
            CutoffId = GetCutoffId(cutoffDate);
            CutoffRange = GetCutoffRange(CutoffDate, Site);
        }

        public Cutoff(string? cutoffId)
        {
            if (string.IsNullOrEmpty(cutoffId))
            {
                var cutoffDate = GetCutoffDate();
                CutoffDate = cutoffDate;
                CutoffId = GetCutoffId(cutoffDate);
            }
            else
            {
                CutoffId = cutoffId;
                CutoffDate = GetCutoffDate(cutoffId);
            }

            CutoffRange = GetCutoffRange(CutoffDate, Site);
        }

        public Cutoff(string? cutoffId, SiteChoices? site)
        {
            Site = site ?? SiteChoices.MANILA;

            if (string.IsNullOrEmpty(cutoffId))
            {
                var cutoffDate = GetCutoffDate();
                CutoffDate = cutoffDate;
                CutoffId = GetCutoffId(cutoffDate);
            }
            else
            {
                CutoffId = cutoffId;
                CutoffDate = GetCutoffDate(cutoffId);
            }

            CutoffRange = GetCutoffRange(CutoffDate, Site);
        }

        public DateTime CutoffDate { get; private set; }
        public string CutoffId { get; private set; }
        public DateTime[] CutoffRange { get; private set; }
        //public string Site { get; private set; } = string.Empty;
        public SiteChoices Site { get; private set; } = SiteChoices.MANILA;
        public string GetPreviousCutoff() => new Cutoff(CutoffDate.AddDays(-15)).CutoffId;

        public void SetSite(string? site)
        {
            if (Enum.TryParse(site, out SiteChoices result))
            {
                Site = result;
                CutoffRange = GetCutoffRange(CutoffDate, Site);
            }
        }

        public void SetSite(SiteChoices? site)
        {
            Site = site ?? SiteChoices.MANILA;
            CutoffRange = GetCutoffRange(CutoffDate, Site);
        }

        public override string ToString() => CutoffId;

        private DateTime GetCutoffDate()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            return DateTime.Now.Day < 20
                ? new DateTime(year, month, 15)
                : new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }

        private DateTime GetCutoffDate(string cutoffId)
        {
            int year = int.Parse(cutoffId.Substring(0, 2));
            int month = int.Parse(cutoffId.Substring(2, 2));
            int dayIdx = int.Parse(cutoffId[5..]);
            int day = 15;

            if (dayIdx == 2)
            {
                day = DateTime.DaysInMonth(year, month);
            }
            else if (dayIdx != 1)
            {
                day = dayIdx;
            }

            return new DateTime(year + 2000, month, day);
        }
        //private void GetCutoffDate()
        //{
        //    int year = int.Parse(CutoffId.Substring(0, 2));
        //    int month = int.Parse(CutoffId.Substring(2, 2));
        //    int dayIdx = int.Parse(CutoffId.Substring(5, 1));
        //    int day = 15;
        //    if (dayIdx == 2)
        //        day = DateTime.DaysInMonth(year, month);

        //    CutoffDate = new DateTime(year + 2000, month, day); // update year to 3000 at the end of the millennium
        //}

        //private void GetCutoffId()
        //{
        //    if (CutoffDate.Day <= 15) CutoffId = $"{CutoffDate:yyMM}-1";
        //    else CutoffId = $"{CutoffDate:yyMM}-2";
        //}

        private string GetCutoffId(DateTime cutoffDate)
        {
            switch (cutoffDate.Day)
            {
                case <= 15:
                    return $"{cutoffDate:yyMM}-1";
                default:
                    return $"{cutoffDate:yyMM}-2";
            }
        }

        //private void GetCutoffRange()
        //{
        //    if (Site == "LEYTE")
        //    {
        //        if (new[] { 28, 29, 30, 31 }.Contains(CutoffDate.Day))
        //            CutoffRange = new[] { new DateTime(CutoffDate.Year, CutoffDate.Month, 4), new DateTime(CutoffDate.Year, CutoffDate.Month, 18) };
        //        else if (15 == CutoffDate.Day)
        //        {
        //            var previousMonth = CutoffDate.AddMonths(-1);
        //            CutoffRange = new[] { new DateTime(previousMonth.Year, previousMonth.Month, 19), new DateTime(CutoffDate.Year, CutoffDate.Month, 3) };
        //        }
        //    }
        //    else
        //    {
        //        if (new[] { 28, 29, 30, 31 }.Contains(CutoffDate.Day))
        //            CutoffRange = new[] { new DateTime(CutoffDate.Year, CutoffDate.Month, 5), new DateTime(CutoffDate.Year, CutoffDate.Month, 19) };
        //        else if (15 == CutoffDate.Day)
        //        {
        //            var previousMonth = CutoffDate.AddMonths(-1);
        //            CutoffRange = new[] { new DateTime(previousMonth.Year, previousMonth.Month, 20), new DateTime(CutoffDate.Year, CutoffDate.Month, 4) };
        //        }
        //    }
        //}

        private DateTime[] GetCutoffRange(DateTime cutoffDate, SiteChoices site)
        {
            if (Enumerable.Range(28, 4).Contains(cutoffDate.Day))
            {
                return new[]
                {
                    new DateTime(cutoffDate.Year, cutoffDate.Month, site == SiteChoices.LEYTE ? 4 : 5),
                    new DateTime(cutoffDate.Year, cutoffDate.Month, site == SiteChoices.LEYTE ? 18 : 19)
                };
            }
            else if (cutoffDate.Day == 15)
            {
                var previousMonth = cutoffDate.AddMonths(-1);
                return new[]
                {
                    new DateTime(previousMonth.Year, previousMonth.Month, site == SiteChoices.LEYTE ? 19 : 20),
                    new DateTime(cutoffDate.Year, cutoffDate.Month, site == SiteChoices.LEYTE ? 3 : 4)
                };
            }
            else
            {
                return Array.Empty<DateTime>();
            }
        }
    }
}
