using Pms.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common
{
    public interface IMain : INotifyPropertyChanged
    {
        public Company? Company { get; }
        public string CutoffId { get; }
        public PayrollCode? PayrollCode { get; }
        public SiteChoices? Site { get; }
        public IEnumerable<PayrollCode> PayrollCodes { get; }
    }
}
