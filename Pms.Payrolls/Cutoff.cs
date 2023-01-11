using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls
{
    public class Cutoff
    {
        public Cutoff()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            if (DateTime.Now.Day < 20)
                CutoffDate = new DateTime(year, month, 15);
            else CutoffDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            GetCutoffId();
        }

        public Cutoff(string cutoffId)
        {
            CutoffId = cutoffId;
            GetCutoffDate();
        }

        public Cutoff(DateTime cutoffDate)
        {
            CutoffDate = cutoffDate;
            GetCutoffId();
        }

        public DateTime CutoffDate { get; private set; }
        public string CutoffId { get; private set; } = string.Empty;
        public int YearCovered
        {
            get
            {
                return CutoffDate.Month == 12 ? CutoffDate.Year + 1 : CutoffDate.Year;
            }
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
            if (CutoffDate.Day <= 15)
                CutoffId = $"{CutoffDate:yyMM}-1";
            else CutoffId = $"{CutoffDate:yyMM}-2";
        }
    }
}
