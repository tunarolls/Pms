using Pms.Common;
using Pms.Masterlists.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Timesheets.Module
{
    public interface ITimesheetsMain : IHasCutoffId, IHasMasterlist, IHasSiteChoices, INotifyPropertyChanged
    {
    }
}
