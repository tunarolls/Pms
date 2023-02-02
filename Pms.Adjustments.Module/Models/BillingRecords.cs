using Pms.Adjustments.Models;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Adjustments.ServiceLayer.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Adjustments.Module.Models
{
    public class BillingRecords
    {
        private readonly BillingRecordManager _manager;
        private readonly BillingRecordProvider _provider;

        public BillingRecords(BillingRecordManager manager, BillingRecordProvider provider)
        {
            _manager = manager;
            _provider = provider;
        }

        public IEnumerable<BillingRecord> Get()
        {
            return _provider.GetBillingRecords();
        }

        public IEnumerable<BillingRecord> GetByPayrollCode(string payrollCode)
        {
            return _provider.GetBillingRecordsByPayrollCode(payrollCode);
        }

        public async Task<ICollection<BillingRecord>> GetByPayrollCode(string? payrollCode, CancellationToken cancellationToken = default)
        {
            return await _provider.GetBillingRecordsByPayrollCode(payrollCode, cancellationToken);
        }

        public void SaveRecord(BillingRecord record)
        {
            _manager.Save(record);
        }

        public async Task SaveRecord(BillingRecord record, CancellationToken cancellationToken = default)
        {
            await _manager.Save(record, cancellationToken);
        }

        public IEnumerable<BillingRecord> Import(string filePath)
        {
            return BillingRecordImporter.Import(filePath);
        }

        public async Task<ICollection<BillingRecord>> Import(string fileName, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return BillingRecordImporter.Import(fileName);
            }, cancellationToken);
        }
    }
}
