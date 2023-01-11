using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.ServiceLayer.Hrms.Adapter
{
    public class HRMSParameter
    {
        public HRMSParameter() : this(new(), new()) { }

        public HRMSParameter(Dictionary<string, string?> bodyArgs, Dictionary<string, string?> urls)
        {
            BodyArgs = bodyArgs;
            Urls = urls;
        }

        public Dictionary<string, string?> BodyArgs { get; set; }
        public Dictionary<string, string?> Urls { get; set; }
    }
}
