using Pms.Adjustments.Models;
using Pms.Adjustments.ServiceLayer.EfCore;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.Module.Models
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

        public async Task<EmployeeView?> Find(string eeId, CancellationToken cancellationToken = default)
        {
            return await _provider.FindEmployee(eeId, cancellationToken);
        }
    }
}
