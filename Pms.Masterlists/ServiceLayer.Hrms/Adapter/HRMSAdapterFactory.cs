using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.Hrms.Adapter
{
    public class HRMSAdapterFactory
    {
        public static HRMSAdapter CreateAdapter(IConfigurationRoot config)
        {
            var section = config.GetRequiredSection("HRMSAPI");

            Dictionary<string, string?> urls = new()
            {
                { "MANILA", section.GetValue<string>("Url") },
                { "LEYTE", section.GetValue<string>("Url_Leyte") }
            };

            Dictionary<string, string?> bodyArgs = new()
            {
                { "idno", "" },
                { "what", section.GetValue<string>("What") },
                { "field", section.GetValue<string>("Field") },
                { "search", section.GetValue<string>("Search") },
                { "apitoken", section.GetValue<string>("APIToken") }
            };

            HRMSParameter param = new(bodyArgs: bodyArgs, urls: urls);
            return new HRMSAdapter(param);
        }
    }
}
