﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common
{
    public class PmsConstants
    {
        public const string ConfigFilename = "appsettings.json";
        public const string DevelopmentConnectionName = "Development";
        public const string ProductionConnectionName = "Production";
        public const string Timesheets = nameof(Timesheets);
        public const string Timesheet = nameof(Timesheet);
        public const string Site = nameof(Site);
        public const string PayrollCode = nameof(PayrollCode);
        public const string Cutoff = nameof(Cutoff);
        public const string Main = nameof(Main);
        public const string SelectedDate = nameof(SelectedDate);
    }
}
