using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.ServiceLayer.TimeSystem.Adapter
{
    public class TimeDownloaderParameter
    {
        public PostDataClass PostData { get; set; } = new();
        public Dictionary<string, string> Urls { get; set; } = new();

        public class PostDataClass
        {
            [JsonProperty("api_token")]
            public string ApiToken { get; set; } = string.Empty;
            [JsonProperty("date_from")]
            public string DateFrom { get; set; } = string.Empty;
            [JsonProperty("date_to")]
            public string DateTo { get; set; } = string.Empty;
            [JsonProperty("info")]
            public string Info { get; set; } = string.Empty;
            [JsonProperty("page")]
            public string Page { get; set; } = string.Empty;
            [JsonProperty("payroll_code")]
            public string PayrollCode { get; set; } = string.Empty;
        }
    }
}
