using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pms.Common;
using Pms.Masterlists.Module.ViewModels;
using Pms.Masterlists.Module.Views;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ServiceLayer.Hrms.Adapter;
using Pms.Masterlists.ServiceLayer.Hrms.Services;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module
{
    public class MasterlistsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<Companies>();
            containerRegistry.Register<Employees>();
            containerRegistry.Register<PayrollCodes>();
            containerRegistry.Register<EmployeeManager>();
            containerRegistry.Register<EmployeeProvider>();
            containerRegistry.Register<HrmsEmployeeProvider>();
            containerRegistry.Register<CompanyManager>();
            containerRegistry.Register<PayrollCodeManager>();
            containerRegistry.RegisterDialog<SelectDateView>(ViewNames.SelectDateView);
            containerRegistry.RegisterDialog<EmployeeDetailView>(ViewNames.EmployeeDetailView);
            containerRegistry.RegisterDialog<PayrollCodeDetailView>(ViewNames.PayrollCodeDetailView);
            containerRegistry.RegisterForNavigation<EmployeeListingView>(ViewNames.EmployeeListingView);


#if DEBUG
            containerRegistry.Register<IMessageBoxService, DummyMessageBoxService>();
            containerRegistry.Register<EmployeeListingViewModel, DummyEmployeeListingViewMode>();
#else
            containerRegistry.Register<IMessageBoxService, MessageBoxService>();
#endif
        }
    }
}
