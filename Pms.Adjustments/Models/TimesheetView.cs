using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Models
{
    public class TimesheetView
    {
        public double Allowance { get; set; }
        public string CutoffId { get; set; } = string.Empty;
        public EmployeeView EE { get; set; }
        public string EEId { get; set; } = string.Empty;
        public string RawPCV { get; set; } = string.Empty;
        public string TimesheetId { get; set; } = string.Empty;
    }
}
