using Pms.Common.Views;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common
{
    public class CommonModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<CancelDialogView>(DialogNames.CancelDialog);
            containerRegistry.RegisterDialog<PromptDialogView>(DialogNames.PromptDialog);
        }
    }
}
