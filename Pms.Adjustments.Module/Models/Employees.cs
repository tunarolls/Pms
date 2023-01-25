using Pms.Adjustments.Models;
using Pms.Adjustments.ServiceLayer.EfCore;

namespace Pms.AdjustmentModule.FrontEnd.Models
{
    public class Employees
    {
        private readonly EmployeeViewProvider _provider;

        public Employees(EmployeeViewProvider provider)
        {
            _provider = provider;
        }

        public EmployeeView Find(string eeId)
        {
            return _provider.FindEmployee(eeId);
        }
    }
}
