using Pms.Common;
using Pms.Masterlists.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Adjustments.Module
{
    public interface IAdjustmentMain : IHasMasterlist, IHasCutoffId, INotifyPropertyChanged
    {
    }
}
