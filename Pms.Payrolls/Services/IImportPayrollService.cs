using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.Services
{
    public interface IImportPayrollService
    {
        IEnumerable<Payroll> StartImport(string payRegisterFilePath);

        void ValidatePayRegisterFile();
    }
}
