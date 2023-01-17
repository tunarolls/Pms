using Pms.Common.Enums;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common.Events
{
    public sealed class SelectedSiteChanged : PubSubEvent<SiteChoices> { }
    public sealed class SelectedPayrollCodesChanged : PubSubEvent<string[]> { }
    public sealed class SelectedPayrollCodeChanged : PubSubEvent<string> { }
    public sealed class SelectedCutoffIdChanged : PubSubEvent<string> { }
    public sealed class SelectedCompanyChanged : PubSubEvent<Company> { }
}
