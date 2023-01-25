using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pms.Common;
using Pms.Payrolls.Module.Views;
using Pms.Payrolls.Persistence;
using Pms.Payrolls.ServiceLayer.EfCore;
using Pms.Payrolls.Services;
using Prism.Ioc;
using Prism.Modularity;
using System;

namespace Pms.Payrolls.Module
{
    public class PayrollsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IManagePayrollService, PayrollManager>();
            containerRegistry.Register<IProvidePayrollService, PayrollProvider>();
            containerRegistry.Register<Payrolls>();
            containerRegistry.Register<IFileDialogService, FileDialogService>();
            containerRegistry.RegisterForNavigation<PayrollView>(ViewNames.PayrollsView);
            containerRegistry.RegisterForNavigation<AlphalistView>(ViewNames.ImportAlphalistView);
        }
    }
}
