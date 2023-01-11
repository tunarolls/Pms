using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class DownloadSummary<T>
    {
        public string Status { get; set; } = string.Empty;
        public string TotalConfirmed { get; set; } = string.Empty;
        public string TotalCount { get; set; } = string.Empty;
        public string TotalPage { get; set; } = string.Empty;
        public T[]? UnconfirmedTimesheet { get; set; }
    }
}
