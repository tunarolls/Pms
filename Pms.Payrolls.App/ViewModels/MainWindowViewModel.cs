using Pms.Common;
using Pms.Common.Enums;
using Prism.Commands;
using Prism.Common;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Payrolls.App.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMain
    {
        public string[] _cutoffIds = Array.Empty<string>();
        private IEnumerable<Company> _companies = Enumerable.Empty<Company>();
        private string _companyId = string.Empty;
        private string _cutoffId = string.Empty;
        private string _payrollCodeId = string.Empty;
        private IEnumerable<PayrollCode> _payrollCodes = Enumerable.Empty<PayrollCode>();
        private SiteChoices _site = SiteChoices.MANILA;
        private readonly IRegionManager _regionManager;
        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            PropertyChanged += MainWindowViewModel_PropertyChanged;
            AlphalistCommand = new DelegateCommand(ExecuteAlphalist);
            BillingCommand = new DelegateCommand(ExecuteBilling);
            BillingRecordCommand = new DelegateCommand(ExecuteBillingRecord);
            EmployeeCommand = new DelegateCommand(ExecuteEmployee);
            LoadFilterCommand = new DelegateCommand(ExecuteLoadFilter);
            PayrollCommand = new DelegateCommand(ExecutePayroll);
            TimesheetCommand= new DelegateCommand(ExecuteTimesheet);
        }

        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PayrollCode))
            {
                Company = Companies.SingleOrDefault(t => t.CompanyId == PayrollCode.CompanyId);
                Site = Sites.SingleOrDefault(t => t.ToString() == PayrollCode.Site);
            }
        }

        #region commands
        public DelegateCommand AlphalistCommand { get; }

        public DelegateCommand BillingCommand { get; }

        public DelegateCommand BillingRecordCommand { get; }

        public DelegateCommand EmployeeCommand { get; }

        public DelegateCommand LoadFilterCommand { get; }

        public DelegateCommand PayrollCommand { get; }

        public DelegateCommand TimesheetCommand { get; }

        private void ExecuteAlphalist()
        {
            throw new NotImplementedException();
        }

        private void ExecuteBilling()
        {
            throw new NotImplementedException();
        }

        private void ExecuteBillingRecord()
        {
            throw new NotImplementedException();
        }

        private void ExecuteEmployee()
        {
            throw new NotImplementedException();
        }

        private void ExecuteLoadFilter()
        {
            throw new NotImplementedException();
        }

        private void ExecutePayroll()
        {
            throw new NotImplementedException();
        }

        private void ExecuteTimesheet()
        {
            var navParams = new NavigationParameters()
            {
                { Constants.Placeholder, this }
            };

            _regionManager.RequestNavigate(RegionNames.PayrollsContentRegion, ViewNames.Timesheets, navParams);
        }
        #endregion



        public IEnumerable<Company> Companies { get => _companies; set => SetProperty(ref _companies, value); }

        public Company? Company { get; set; }

        public string CompanyId { get => _companyId; set => SetProperty(ref _companyId, value); } 

        //public Cutoff Cutoff { get; set; }

        public string CutoffId { get => _cutoffId; set => SetProperty(ref _cutoffId, value); }

        public string[] CutoffIds { get => _cutoffIds; set => SetProperty(ref _cutoffIds, value); }

        

        public PayrollCode PayrollCode { get; set; } = new() { PayrollCodeId = string.Empty };

        public string PayrollCodeId { get => _payrollCodeId; set => SetProperty(ref _payrollCodeId, value); }

        public IEnumerable<PayrollCode> PayrollCodes { get => _payrollCodes; set => SetProperty(ref _payrollCodes, value); }
        public SiteChoices Site { get => _site; set => SetProperty(ref _site, value); }
        public IEnumerable<SiteChoices> Sites { get; } = Enum.GetValues(typeof(SiteChoices)).Cast<SiteChoices>().ToList();
    }
}
