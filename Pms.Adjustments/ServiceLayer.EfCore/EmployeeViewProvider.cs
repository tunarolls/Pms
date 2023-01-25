using Microsoft.EntityFrameworkCore;
using Pms.Adjustments.Models;
using Pms.Adjustments.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.ServiceLayer.EfCore
{
    public class EmployeeViewProvider
    {
        protected IDbContextFactory<AdjustmentDbContext> _factory;

        public EmployeeViewProvider(IDbContextFactory<AdjustmentDbContext> factory)
        {
            _factory = factory;
        }

        public EmployeeView FindEmployee(string eeId)
        {
            using AdjustmentDbContext context = _factory.CreateDbContext();
            return context.Employees.Find(eeId);
        }

        public async Task<EmployeeView?> FindEmployee(string eeId, CancellationToken cancellationToken = default)
        {
            using var context = _factory.CreateDbContext();
            return await context.Employees.FindAsync(new object[] { eeId }, cancellationToken);
        }
    }
}
