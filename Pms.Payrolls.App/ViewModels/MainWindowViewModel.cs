using Pms.Adjustments.Module;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Module;
using Pms.Payrolls.Module;
using Pms.Timesheets.Module;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Payrolls.App.ViewModels
{
    public interface IMain : ITimesheetsMain, IPayrollsMain, IMasterlistsMain, IAdjustmentMain
    {
    }

    public class MainWindowViewModel : CancellableBase, IMain
    {
        #region properties
        public string[] _cutoffIds = Array.Empty<string>();
        private IEnumerable<Company> _companies = Enumerable.Empty<Company>();
        private string _companyId = string.Empty;
        private string _cutoffId = string.Empty;
        private string _payrollCodeId = string.Empty;
        private IEnumerable<PayrollCode> _payrollCodes = Enumerable.Empty<PayrollCode>();
        private SiteChoices? _site;
        private Company? _company;
        private PayrollCode? _payrollCode;

        public IEnumerable<Company> Companies { get => _companies; set => SetProperty(ref _companies, value); }
        public Company? Company { get => _company; set => SetProperty(ref _company, value); }
        public string CompanyId { get => _companyId; set => SetProperty(ref _companyId, value); }
        public string CutoffId { get => _cutoffId; set => SetProperty(ref _cutoffId, value); }
        public string[] CutoffIds { get => _cutoffIds; set => SetProperty(ref _cutoffIds, value); }
        public PayrollCode? PayrollCode { get => _payrollCode; set => SetProperty(ref _payrollCode, value); }
        public string PayrollCodeId { get => _payrollCodeId; set => SetProperty(ref _payrollCodeId, value); }
        public IEnumerable<PayrollCode> PayrollCodes { get => _payrollCodes; set => SetProperty(ref _payrollCodes, value); }
        public SiteChoices? Site { get => _site; set => SetProperty(ref _site, value); }
        public IEnumerable<SiteChoices> Sites { get; } = Enum.GetValues<SiteChoices>();
        #endregion

        private readonly IRegionManager _regionManager;
        private readonly Timesheets.Module.Timesheets m_Timesheets;
        private readonly Payrolls.Module.Payrolls m_Payrolls;
        private readonly Masterlists.Module.Companies m_Companies;
        private readonly Masterlists.Module.PayrollCodes m_PayrollCodes;
        private readonly IDialogService s_Dialog;
        private readonly IMessageBoxService s_Message;

        public MainWindowViewModel(IRegionManager regionManager,
            IDialogService dialog,
            IMessageBoxService message,
            Pms.Timesheets.Module.Timesheets timesheets)
        {
            s_Dialog = dialog;
            s_Message = message;
            _regionManager = regionManager;
            m_Timesheets = timesheets;
            //m_PayrollCodes = payrollCodes;
            //m_Payrolls = payrolls;
            //m_Companies = companies;

            AlphalistCommand = new DelegateCommand(Alphalist);
            BillingCommand = new DelegateCommand(Billing);
            BillingRecordCommand = new DelegateCommand(BillingRecord);
            EmployeeCommand = new DelegateCommand(Employee);
            PayrollCommand = new DelegateCommand(Payroll);
            TimesheetCommand= new DelegateCommand(Timesheet);

            PropertyChanged += MainWindowViewModel_PropertyChanged;

            Listing();
        }

        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PayrollCode))
            {
                Company = Companies.SingleOrDefault(t => t.CompanyId == PayrollCode?.CompanyId);
                Site = Sites.SingleOrDefault(t => t.ToString() == PayrollCode?.Site);
            }
        }

        #region commands
        public DelegateCommand AlphalistCommand { get; }
        public DelegateCommand BillingCommand { get; }
        public DelegateCommand BillingRecordCommand { get; }
        public DelegateCommand EmployeeCommand { get; }
        public DelegateCommand PayrollCommand { get; }
        public DelegateCommand TimesheetCommand { get; }

        private void Alphalist()
        {
            var navParams = new NavigationParameters()
            {
                { PmsConstants.Main, this }
            };

            _regionManager.RequestNavigate(RegionNames.PayrollsContentRegion, ViewNames.ImportAlphalistView, navParams);
        }

        private void Billing()
        {
            var navParams = new NavigationParameters()
            {
                { PmsConstants.Main, this }
            };

            _regionManager.RequestNavigate(RegionNames.PayrollsContentRegion, ViewNames.BillingListingView, navParams);
        }

        private void BillingRecord()
        {
            throw new NotImplementedException();
        }

        private void Employee()
        {
            var navParams = new NavigationParameters()
            {
                { PmsConstants.Main, this }
            };

            _regionManager.RequestNavigate(RegionNames.PayrollsContentRegion, ViewNames.EmployeeListingView, navParams);
        }

        private void Payroll()
        {
            var navParams = new NavigationParameters()
            {
                { PmsConstants.Main, this }
            };

            _regionManager.RequestNavigate(RegionNames.PayrollsContentRegion, ViewNames.PayrollsView, navParams);
        }

        private void Timesheet()
        {
            var navParams = new NavigationParameters()
            {
                { PmsConstants.Main, this }
            };

            _regionManager.RequestNavigate(RegionNames.PayrollsContentRegion, ViewNames.Timesheets, navParams);
        }
        #endregion

        #region Load filter
        private void Listing()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Listing(cts.Token);
        }

        private async Task Listing(CancellationToken cancellationToken = default)
        {
            try
            {
                OnMessageSent("Initializing...");
                OnProgressStart();

                OnMessageSent("Retrieving cutoff ids...");
                var timesheetCutoffIds = await m_Timesheets.ListCutoffIds(cancellationToken);
                //var payrollCutoffIds = await m_Payrolls.ListCutoffIds(cancellationToken);
                //var cutoffIds = timesheetCutoffIds.Union(payrollCutoffIds).OrderByDescending(t => t).ToArray();

                OnMessageSent("Retrieving payroll codes...");
                //var payrollCodes = await m_PayrollCodes.ListPayrollCodes(cancellationToken);

                OnMessageSent("Retrieving companies...");
                //var companies = await m_Companies.ListCompanies(cancellationToken);

                //Companies = companies;
                //PayrollCodes = payrollCodes;
                //CutoffIds = cutoffIds;

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
            }
        }
        #endregion
    }
}
