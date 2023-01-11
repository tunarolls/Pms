using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.Exceptions
{
    public class BankReportTemplateNotFoundException : Exception
    {
        public string TemplateFile { get; set; }
        public BankReportTemplateNotFoundException(string templateFile)
        {
            TemplateFile = templateFile;
        }
    }
}
