using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pms.Common.Enums;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Exceptions;

namespace Pms.MasterlistsTests.Entities
{
    [TestClass]
    public class An_eeId_is_valid
    {
        [TestMethod]
        public void if_it_does_not_accept_characted_lenght_less_than_three()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.EEId = "DY";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_characted_lenght_greater_than_four()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.EEId = "DYYYJ";
                employee.ValidatePersonalInformation();
            });
        }
    }

    [TestClass]
    public class A_firstname_is_valid
    {
        [TestMethod]
        public void if_it_does_not_accept_lower_case()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.FirstName = "Sean Ivan";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_length_less_than_two()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.FirstName = "I";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_length_greater_than_45()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.FirstName = "SEAN SEAN SEAN SEAN SEAN SEAN SEAN SEAN SEAN 1";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_numbers()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.FirstName = "IVAN 123";
                employee.ValidatePersonalInformation();
            });
        }
    }

    [TestClass]
    public class A_lastname_is_valid
    {
        [TestMethod]
        public void if_it_does_not_accept_lower_case()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.LastName = "Fernandez";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_length_less_than_two()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.LastName = "F";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_length_greater_than_45()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.LastName = "SEAN SEAN SEAN SEAN SEAN SEAN FERNANDEZ FERNANDEZ";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_numbers()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.LastName = "IVAN 123";
                employee.ValidatePersonalInformation();
            });
        }
    }

    [TestClass]
    public class A_middle_is_valid
    {
        [TestMethod]
        public void if_it_does_not_accept_lower_case()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.MiddleName = "sean IVAN";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_length_greater_than_45()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.MiddleName = "MARTINEZ MARTINEZ MARTINEZ MARTINEZ MARTINEZ F";
                employee.ValidatePersonalInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_accept_numbers()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.MiddleName = "MARTINEZ 123";
                employee.ValidatePersonalInformation();
            });
        }
    }

    [TestClass]
    public class A_tin_number_have_the_right_format
    {
        [DataRow("123456789")]
        [DataRow("1234567890")]
        [DataRow("12345678900")]
        [DataRow("123456789000")]
        [DataRow("1234567890000")]
        [DataRow("123-456-789")]
        [DataRow("123-45-6789")]
        [DataRow("123-456-789-000")]
        [DataTestMethod]
        public void if_it_matched_the_regular_expression(string tin)
        {
            Employee employee = new();
            employee.TIN = tin;
            employee.ValidateGovernmentInformation();
        }

        [TestMethod]
        public void if_it_does_not_have_letters()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Pagibig = "123A56789";
                employee.ValidateGovernmentInformation();
            });
        }
    }

    [TestClass]
    public class A_pagibig_number_have_the_right_format
    {
        [DataRow("123456789012")]
        [DataRow("1234-5678-9012")]
        [DataTestMethod]
        public void if_it_matched_the_regular_expression(string pagibig)
        {
            Employee employee = new();
            employee.Pagibig = pagibig;
            employee.ValidateGovernmentInformation();
        }

        [TestMethod]
        public void if_it_does_not_have_letters()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Pagibig = "123A56789012";
                employee.ValidateGovernmentInformation();
            });
        }
    }

    [TestClass]
    public class A_sss_number_have_the_right_format
    {
        [DataRow("1234567890")]
        [DataRow("12-3456789-0")]
        [DataTestMethod]
        public void if_it_matched_the_regular_expression(string sss)
        {
            Employee employee = new();
            employee.SSS = sss;
            employee.ValidateGovernmentInformation();
        }

        [TestMethod]
        public void if_it_does_not_have_letters()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.SSS = "123A56789O";
                employee.ValidateGovernmentInformation();
            });
        }
    }

    [TestClass]
    public class A_philhealth_number_have_the_right_format
    {
        [DataRow("123456789012")]
        [DataRow("12-345678901-2")]
        [DataTestMethod]
        public void if_it_matched_the_regular_expression(string philhealth)
        {
            Employee employee = new();
            employee.PhilHealth = philhealth;
            employee.ValidateGovernmentInformation();
        }

        [TestMethod]
        public void if_it_does_not_have_letters()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.PhilHealth = "123A56789OI2";
                employee.ValidateGovernmentInformation();
            });
        }
    }

    [TestClass]
    public class A_account_number_is_valid
    {
        [DataRow(BankChoices.LBP)]
        [DataRow(BankChoices.CBC)]
        [DataRow(BankChoices.CBC1)]
        [DataRow(BankChoices.MTAC)]
        [DataRow(BankChoices.MPALO)]
        [DataTestMethod]
        public void if_it_throws_exception_when_account_number_is_empty(BankChoices bank)
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = bank;
                employee.AccountNumber = string.Empty;
                employee.ValidateBankInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_have_letters()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.AccountNumber = "123A56789OI2";
                employee.ValidateBankInformation();
            });
        }

        [TestMethod]
        public void if_it_throws_exception_when_bank_is_LBP_and_length_is_not_19()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.LBP;
                employee.AccountNumber = "2206060193770081881";
                employee.AccountNumber = employee.AccountNumber.PadRight(20);
                employee.ValidateBankInformation();
            });
        }

        [TestMethod]
        public void if_it_throws_exception_when_bank_is_LBP_and_it_is_does_not_contain_19372()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.LBP;
                employee.AccountNumber = "2206060193770081881";
                employee.ValidateBankInformation();
            });
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(11)]
        [DataRow(13)]
        [DataRow(14)]
        [DataRow(15)]
        [DataRow(16)]
        [DataRow(17)]
        [DataRow(20)]
        [DataRow(21)]
        [DataTestMethod]
        public void if_it_throws_exception_when_bank_is_CBC_CBC1_and_length_is_not_10_12_18_19(int length)
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.CBC;
                employee.AccountNumber = employee.AccountNumber.PadRight(length);
                employee.ValidateBankInformation();
            });
        }

        [DataRow(10)]
        [DataRow(12)]
        [DataRow(18)]
        [DataRow(19)]
        [DataTestMethod]
        public void if_it_does_not_throws_exception_when_bank_is_CBC_CBC1_and_length_is_10_12_18_19(int length)
        {
            Employee employee = new();
            employee.Bank = BankChoices.CBC;
            employee.AccountNumber = employee.AccountNumber.PadRight(length);
            employee.ValidateBankInformation();
        }

        [TestMethod]
        public void if_it_throws_exception_when_bank_is_MTAC_and_length_is_not_13()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.MTAC;
                employee.AccountNumber = "5253525602126";
                employee.AccountNumber = employee.AccountNumber.PadRight(14);
                employee.ValidateBankInformation();
            });
        }

        [TestMethod]
        public void if_it_throws_exception_when_bank_is_MTAC_and_it_is_does_not_have_a_leading_525()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.MTAC;
                employee.AccountNumber = "5223525602126";
                employee.ValidateBankInformation();
            });
        }

        [TestMethod]
        public void if_it_throws_exception_when_bank_is_MPALO_and_length_is_not_13()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.MPALO;
                employee.AccountNumber = "7563899703376";
                employee.AccountNumber = employee.AccountNumber.PadRight(14);
                employee.ValidateBankInformation();
            });
        }

        [TestMethod]
        public void if_it_throws_exception_when_bank_is_MPALO_and_it_is_does_not_have_a_leading_756()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.MPALO;
                employee.AccountNumber = "7553899703376";
                employee.ValidateBankInformation();
            });
        }
    }

    [TestClass]
    public class A_card_number_is_valid
    {
        [DataRow(BankChoices.LBP)]
        [DataRow(BankChoices.CBC)]
        [DataRow(BankChoices.CBC1)]
        [DataRow(BankChoices.MTAC)]
        [DataRow(BankChoices.MPALO)]
        [DataTestMethod]
        public void if_it_throws_exception_when_bank_is_empty(BankChoices bank)
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = bank;
                employee.CardNumber = string.Empty;
                employee.ValidateBankInformation();
            });
        }

        [TestMethod]
        public void if_it_does_not_have_letters()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.LBP;
                employee.CardNumber = "123A56789OI2";
                employee.ValidateBankInformation();
            });
        }

        [TestMethod]
        public void if_it_throws_exception_when_bank_is_LBP_and_length_is_not_16()
        {
            Assert.ThrowsException<InvalidFieldValueException>(() =>
            {
                Employee employee = new();
                employee.Bank = BankChoices.LBP;
                employee.CardNumber = employee.CardNumber.PadRight(13);
                employee.ValidateBankInformation();
            });
        }
    }
}
