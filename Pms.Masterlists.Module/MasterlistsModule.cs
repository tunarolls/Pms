using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pms.Common;
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
            var config = new ConfigurationBuilder().AddJsonFile(PmsConstants.ConfigFilename, optional: false, reloadOnChange: true).Build();
            var connectionString = config.GetConnectionString(PmsConstants.DevelopmentConnectionName) ?? string.Empty;
            containerRegistry.RegisterInstance<IDbContextFactory<EmployeeDbContext>>(new EmployeeDbContextFactory(connectionString));
            containerRegistry.RegisterInstance(HRMSAdapterFactory.CreateAdapter(config));
            containerRegistry.Register<Companies>();
            containerRegistry.Register<Employees>();
            containerRegistry.Register<PayrollCodes>();
            containerRegistry.Register<EmployeeManager>();
            containerRegistry.Register<EmployeeProvider>();
            containerRegistry.Register<HrmsEmployeeProvider>();
            containerRegistry.Register<CompanyManager>();
            containerRegistry.Register<PayrollCodeManager>();
            containerRegistry.RegisterDialog<SelectDateView>(ViewNames.SelectDateView);
            containerRegistry.RegisterForNavigation<EmployeeListingView>(ViewNames.EmployeeListingView);
        }
    }
}
