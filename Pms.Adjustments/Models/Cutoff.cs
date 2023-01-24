using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Models
{
    public class Cutoff
    {
        public string CutoffId { get; private set; } = string.Empty;
        public DateTime CutoffDate { get; private set; }
        public DeductionOptions DeductionOption => CutoffDate.Day == 15 ? DeductionOptions.ONLY15TH : DeductionOptions.ONLY30TH;

        public Cutoff()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            if (DateTime.Now.Day < 15)
                CutoffDate = new DateTime(year, month, 15);
            else CutoffDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            GetCutoffId();
        }

        public Cutoff(string cutoffId)
        {
            CutoffId = cutoffId;
            GetCutoffDate();
        }

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

        public override string ToString() => CutoffId;
    }
}
