using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pms.Adjustments.Module.Models;
using Pms.Adjustments.Module.Views;
using Pms.Adjustments.Persistence;
using Pms.Adjustments.ServiceLayer.EfCore;
using Pms.Adjustments.ServiceLayer.Files;
using Pms.Adjustments.Services;
using Pms.Common;
using Pms.Timesheets;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Module
{
    public class AdjustmentsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var config = new ConfigurationBuilder().AddJsonFile(PmsConstants.ConfigFilename, optional: false, reloadOnChange: true).Build();
            var connectionString = config.GetConnectionString(PmsConstants.DevelopmentConnectionName) ?? string.Empty;
            containerRegistry.RegisterInstance<IDbContextFactory<AdjustmentDbContext>>(new AdjustmentDbContextFactory(connectionString));
            containerRegistry.RegisterInstance<IDbContextFactory<TimesheetDbContext>>(new TimesheetDbContextFactory(connectionString));
            containerRegistry.Register<IMessageBoxService, DummyMessageBoxService>();
            containerRegistry.Register<Models.Timesheets>();
            containerRegistry.Register<Billings>();
            containerRegistry.Register<IManageBillingService, BillingManager>();
            containerRegistry.Register<IProvideBillingService, BillingProvider>();
            containerRegistry.Register<IGenerateBillingService, BillingGenerator>();
            containerRegistry.Register<IProvideTimesheetService, TimesheetProvider>();
            containerRegistry.Register<TimesheetManager>();
            containerRegistry.RegisterForNavigation<BillingListingView>(ViewNames.BillingListingView);
        }
    }
}
