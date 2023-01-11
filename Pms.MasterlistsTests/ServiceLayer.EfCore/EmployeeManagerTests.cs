using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.MasterlistsTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.MasterlistsTests.ServiceLayer.EfCore
{
    [TestClass()]
    public class EmployeeManagerTests
    {
        private IDbContextFactory<EmployeeDbContext> _factory;
        private EmployeeManager _service;

        private string _eeId;
        private Employee _expectedEmployee;

        public EmployeeManagerTests()
        {
            _factory = new EmployeeDbContextFactoryFixture();
            _service = new EmployeeManager(_factory);

            _eeId = "ABCD";
            _expectedEmployee = Seeder.AddSeedEmployee(_eeId, "M-P1A", "CHK", "SOFTWARE", "000000001", "00000000000001");
        }

        [TestMethod]
        public void Bank_Informations_should_not_Update_General_Information()
        {
            IBankInformation bankInfo = _expectedEmployee;

            _service.Save(bankInfo);

            using EmployeeDbContext context = _factory.CreateDbContext();
            Employee? actualEmployee = context.Employees.Where(ee => ee.EEId == _eeId).FirstOrDefault();
            Assert.IsNotNull(actualEmployee);

            context.Employees.Remove(actualEmployee);
            context.SaveChanges();

            Assert.IsFalse(actualEmployee.Location == _expectedEmployee.Location);
        }

        //[Fact]
        //public void General_Information_should_not_Update_Bank_Information()
        //{
        //    IPersonalInformation bankInfo = expectedEmployee;

        //    _service.Save(bankInfo);

        //    using EmployeeDbContext context = _factory.CreateDbContext();
        //    Employee actualEmployee = context.Employees.Where(ee => ee.EEId == eeId).FirstOrDefault();

        //    context.Employees.Remove(actualEmployee);
        //    context.SaveChanges();


        //    Assert.NotNull(actualEmployee);
        //    Assert.False(actualEmployee.AccountNumber == expectedEmployee.AccountNumber);
        //}

    }
}
