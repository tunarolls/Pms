using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls
{
    public class Cutoff
    {
        public string CutoffId { get; private set; }
        public DateTime CutoffDate { get; private set; }

        public int YearCovered
        {
            get
            {
                return CutoffDate.Month == 12
                    ? CutoffDate.Year + 1
                    : CutoffDate.Year;
            }
        }

        public Cutoff()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            var cutoffDate = DateTime.Now.Day < 20
                ? new DateTime(year, month, 15)
                : new DateTime(year, month, DateTime.DaysInMonth(year, month));

            CutoffDate = cutoffDate;
            CutoffId = GetCutoffId(cutoffDate);
        }

        public Cutoff(string cutoffId)
        {
            CutoffId = cutoffId;
            CutoffDate = GetCutoffDate(cutoffId);
        }

        public Cutoff(DateTime cutoffDate)
        {
            CutoffDate = cutoffDate;
            CutoffId = GetCutoffId(cutoffDate);
        }

        private void GetCutoffDate()
        {
            int year = int.Parse(CutoffId.Substring(0, 2));
            int month = int.Parse(CutoffId.Substring(2, 2));
            int dayIdx = int.Parse(CutoffId[5..]);
            int day = 15;


            if (dayIdx == 2)
                day = DateTime.DaysInMonth(year, month);
            else if (dayIdx != 1)
                day = dayIdx;

            CutoffDate = new DateTime(year + 2000, month, day); // update year to 3000 at the end of the millennium
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

        private void GetCutoffId()
        {
            if (CutoffDate.Day == 13)
                CutoffId = $"{CutoffDate:yyMM}-13";
            else if (CutoffDate.Day <= 15)
                CutoffId = $"{CutoffDate:yyMM}-1";
            else
                CutoffId = $"{CutoffDate:yyMM}-2";
        }

        private string GetCutoffId(DateTime cutoffDate)
        {
            switch (cutoffDate.Day)
            {
                case 13:
                    return $"{cutoffDate:yyMM}-13";
                case <= 15:
                    return $"{cutoffDate:yyMM}-1";
                default:
                    return $"{cutoffDate:yyMM}-2";
            }
        }

        public override string ToString() => CutoffId;
    }
}
