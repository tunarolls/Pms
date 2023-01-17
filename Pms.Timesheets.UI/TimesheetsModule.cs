using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pms.Common;
using Pms.Timesheets.Module.ViewModels;
using Pms.Timesheets.Module.Views;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.TimeSystem;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.Module
{
    public class TimesheetsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var config = new ConfigurationBuilder().AddJsonFile(Constants.ConfigFilename, optional: false, reloadOnChange: true).Build();
            var connectionString = config.GetConnectionString(Constants.DevelopmentConnectionName) ?? string.Empty;
            containerRegistry.Register<IDownloadContentProvider, DownloadContentProvider>();
            containerRegistry.Register<IProvideTimesheetService, TimesheetProvider>();
            containerRegistry.Register<TimesheetManager>();
            containerRegistry.Register<Timesheets>();
            containerRegistry.RegisterDialog<TimesheetDetailView>(ViewNames.TimesheetDetailView);
            containerRegistry.RegisterForNavigation<TimesheetListingView>(ViewNames.Timesheets);
            containerRegistry.RegisterInstance(TimeDownloaderFactory.CreateAdapter(config));
            containerRegistry.RegisterInstance<IDbContextFactory<TimesheetDbContext>>(new TimesheetDbContextFactory(connectionString));

#if DEBUG
            containerRegistry.Register<TimesheetListingViewModel, DummyListingViewModel>();
#endif
        }
    }
}
