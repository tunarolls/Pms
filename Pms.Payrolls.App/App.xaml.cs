using Pms.Masterlists.Module;
using Pms.Payrolls.App.Views;
using Pms.Payrolls.Module;
using Pms.Timesheets.Module;
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
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<TimesheetsModule>();
            moduleCatalog.AddModule<PayrollsModule>();
            moduleCatalog.AddModule<MasterlistsModule>();
        }
    }
}
