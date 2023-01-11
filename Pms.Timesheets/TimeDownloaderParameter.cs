using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class TimeDownloaderParameter
    {
        public PostDataClass PostData { get; set; } = new();
        public Dictionary<string, string> Urls { get; set; } = new();

        public class PostDataClass
        {
            public string ApiToken { get; set; } = string.Empty;
            public string DateFrom { get; set; } = string.Empty;
            public string DateTo { get; set; } = string.Empty;
            public string Info { get; set; } = string.Empty;
            public string Page { get; set; } = string.Empty;
            public string PayrollCode { get; set; } = string.Empty;
        }
    }
}
