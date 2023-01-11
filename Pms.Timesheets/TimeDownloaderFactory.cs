using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets
{
    public class TimeDownloaderFactory
    {
        public static TimeDownloaderAdapter CreateAdapter(IConfigurationRoot config)
        {
            var section = config.GetRequiredSection("TimeDownloaderAPI");

            Dictionary<string, string> Urls = new()
            {
                { "MANILA", section.GetValue<string>("Url") ?? string.Empty },
                { "LEYTE", section.GetValue<string>("Url_Leyte") ?? string.Empty }
            };

            TimeDownloaderParameter parameter = new()
            {
                PostData = new()
                {
                    Info = section.GetValue<string>("Info") ?? string.Empty,
                    ApiToken = section.GetValue<string>("APIToken") ?? string.Empty
                },
                Urls = Urls,
            };

            return new TimeDownloaderAdapter(parameter);
        }
    }
}
