using Pms.Adjustments.Models;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Adjustments.ServiceLayer.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pms.Adjustments.Modue
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

        public void SaveRecord(BillingRecord record)
        {
            _manager.Save(record);
        }

        public IEnumerable<BillingRecord> Import(string filePath)
        {
            return new BillingRecordImporter().Import(filePath);
        }
    }
}
