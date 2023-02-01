using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Models
{
    public class Cutoff
    {
        public string CutoffId { get; private set; }
        public DateTime CutoffDate { get; private set; }
        public DeductionOptions DeductionOption => CutoffDate.Day == 15 ? DeductionOptions.ONLY15TH : DeductionOptions.ONLY30TH;

        public Cutoff()
        {
            var cutoffDate = GetCutoffDate();
            CutoffDate = cutoffDate;
            CutoffId = GetCutoffId(cutoffDate);
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
        }

        private DateTime GetCutoffDate()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            return DateTime.Now.Day < 15
                ? new DateTime(year, month, 15)
                : new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }

        private DateTime GetCutoffDate(string cutoffId)
        {
            int year = int.Parse(cutoffId.Substring(0, 2));
            int month = int.Parse(cutoffId.Substring(2, 2));
            int dayIdx = int.Parse(cutoffId.Substring(5, 1));
            int day = 15;

            if (dayIdx == 2)
            {
                day = DateTime.DaysInMonth(year, month);
            }

            return new DateTime(year + 2000, month, day);
        }

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

        public override string ToString() => CutoffId;
    }
}
