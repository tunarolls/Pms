using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Adjustments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Models.Tests
{
    [TestClass]
    public class BillingTests
    {
        [DataRow("DYYJ_PCV", "2208-1", 0, "DYYJ_PCV_2208-1_0")]
        [DataRow("TEST_ALLOWANCE", "2208-2", 1, "TEST_ALLOWANCE_2208-2_1")]
        [DataTestMethod]
        public void GenerateIdWithRecordIdTestShouldEqual(string recordId, string cutoffId, int iterator, string expected)
        {
            // GIVEN
            Billing _sut = new()
            {
                RecordId = recordId,
                CutoffId = cutoffId
            };

            //WHEN
            string actual = Billing.GenerateId(_sut, iterator);

            // THEN
            Assert.AreEqual(expected, actual);
        }


        [DataRow("DYYJ", "PCV", "2208-1", 0, "DYYJ_PCV_2208-1_0")]
        [DataRow("TEST", "ALLOWANCE", "2208-2", 1, "TEST_ALLOWANCE_2208-2_1")]
        [DataTestMethod]
        public void GenerateIdWithoutRecordIdTestShouldEqual(string eeId, string adjustmentName, string cutoffId, int iterator, string expected)
        {
            // GIVEN
            Billing _sut = new()
            {
                EEId = eeId,
                CutoffId = cutoffId
            };

            // WHEN
            string actual = Billing.GenerateId(_sut, iterator);

            // THEN
            Assert.AreEqual(expected, actual);
        }
    }
}