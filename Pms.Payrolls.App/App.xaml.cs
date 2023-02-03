using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pms.Adjustments.Module;
using Pms.Adjustments.Persistence;
using Pms.Common;
using Pms.Common.Views;
using Pms.Masterlists.Module;
using Pms.Masterlists.Persistence;
using Pms.Masterlists.ServiceLayer.EfCore;
using Pms.Masterlists.ServiceLayer.Hrms.Adapter;
using Pms.Payrolls.App.ViewModels;
using Pms.Payrolls.App.Views;
using Pms.Payrolls.Module;
using Pms.Payrolls.Persistence;
using Pms.Payrolls.ServiceLayer.EfCore;
using Pms.Payrolls.Services;
using Pms.Timesheets;
using Pms.Timesheets.Module;
using Pms.Timesheets.Persistence;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.TimeSystem;
using Pms.Timesheets.ServiceLayer.TimeSystem.Adapter;
using Pms.Timesheets.ServiceLayer.TimeSystem.Services;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Payrolls.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var config = new ConfigurationBuilder().AddJsonFile(PmsConstants.ConfigFilename, optional: false, reloadOnChange: true).Build();
            var connectionString = config.GetConnectionString(PmsConstants.DevelopmentConnectionName) ?? string.Empty;
            containerRegistry.RegisterInstance(HRMSAdapterFactory.CreateAdapter(config));
            containerRegistry.RegisterInstance(TimeDownloaderFactory.CreateAdapter(config));
            containerRegistry.RegisterInstance<IDbContextFactory<TimesheetDbContext>>(new TimesheetDbContextFactory(connectionString));
            containerRegistry.RegisterInstance<IDbContextFactory<AdjustmentDbContext>>(new AdjustmentDbContextFactory(connectionString));
            containerRegistry.RegisterInstance<IDbContextFactory<PayrollDbContext>>(new PayrollDbContextFactory(connectionString));
            containerRegistry.RegisterInstance<IDbContextFactory<EmployeeDbContext>>(new EmployeeDbContextFactory(connectionString));

            containerRegistry.Register<IDownloadContentProvider, DownloadContentProvider>();
            containerRegistry.Register<IProvideTimesheetService, TimesheetProvider>();
            containerRegistry.Register<TimesheetManager>();
            containerRegistry.Register<Timesheets.Module.Timesheets>();

            containerRegistry.Register<IManagePayrollService, PayrollManager>();
            containerRegistry.Register<IProvidePayrollService, PayrollProvider>();
            containerRegistry.Register<Module.Payrolls>();

            containerRegistry.Register<CompanyManager>();
            containerRegistry.Register<PayrollManager>();
            containerRegistry.Register<PayrollCodes>();
            containerRegistry.Register<Companies>();

            containerRegistry.Register<IMessageBoxService, MessageBoxService>();
            containerRegistry.Register<IFileDialogService, FileDialogService>();
            containerRegistry.RegisterDialog<CancelDialogView>(DialogNames.CancelDialog);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<CommonModule>();
            moduleCatalog.AddModule<TimesheetsModule>();
            moduleCatalog.AddModule<PayrollsModule>();
            moduleCatalog.AddModule<MasterlistsModule>();
            moduleCatalog.AddModule<AdjustmentsModule>();
        }
    }
}
