﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Pms.Payrolls
{
    public class PayrollRegisterImportBase : IImportPayrollService
    {
        private ImportProcessChoices _process;

        public PayrollRegisterImportBase(ImportProcessChoices process)
        {
            _process = process;
        }

        public IEnumerable<Payroll> StartImport(string payRegisterFilePath)
        {
            if (_process == ImportProcessChoices.PD)
                return new PayrollRegisterPDImport().StartImport(payRegisterFilePath);
            else if (_process == ImportProcessChoices.KS)
                return new PayrollRegisterKSImport().StartImport(payRegisterFilePath);
            throw new Exception("Please Select a Process type.");
        }

        public void ValidatePayRegisterFile() { }
    }
}
