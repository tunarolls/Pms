using NPOI.SS.UserModel;
using Pms.Payrolls.Domain;
using Pms.Payrolls.Domain.SupportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments
{
    public interface IRowWriter
    {
        void Write(IRow row, Payroll payroll, int sequence = 0);
        void WriteTotal(IRow row, PayrollRegister payrolls);
    }
}
