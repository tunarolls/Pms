using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ServiceLayer.EfCore;

namespace Pms.MasterlistsTests.ServiceLayer.EfCore
{
    [TestClass]
    public class EmployeeProviderTests
    {
        private IDbContextFactory<EmployeeDbContext> _factory;
        private EmployeeProvider _service;

        public EmployeeProviderTests()
        {
            _factory = new EmployeeDbContextFactoryFixture();
            _service = new EmployeeProvider(_factory);
        }

        [TestMethod]
        public void FindEmployeeTest()
        {
            string expectedEEId = "DYYJ";
            Employee? actualEmployee = _service.FindEmployee(expectedEEId);
            Assert.AreEqual(actualEmployee?.EEId, expectedEEId);
        }

        //[TestMethod]
        //public void ShouldReturnEmployeesUsingGetEmployees()
        //{
        //}
    }
}
