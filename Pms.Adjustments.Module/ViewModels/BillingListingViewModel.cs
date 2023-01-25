using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using Pms.Adjustments.Module.Models;
using Pms.Common;
using Pms.Adjustments.Models;
using Prism.Commands;
using System.Threading.Tasks;
using System.Threading;
using Prism.Services.Dialogs;
using Prism.Regions;

namespace Pms.Adjustments.Module.ViewModels
{
    static class PayrollFilterExtension
    {
        public static IEnumerable<Billing> FilterPayrollCode(this IEnumerable<Billing> payrolls, string payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode) ? payrolls.Where(t => t.EE.PayrollCode == payrollCode) : payrolls;
        }

        public static IEnumerable<Billing> FilterPayrollCode(this ICollection<Billing> payrolls, string payrollCode)
        {
            return FilterPayrollCode((IEnumerable<Billing>)payrolls, payrollCode);
        }

        public static IEnumerable<Billing> FilterAdjustmentName(this IEnumerable<Billing> payrolls, AdjustmentTypes adjustmentType)
        {
            return payrolls.Where(t => t.AdjustmentType == adjustmentType);
        }

        public static IEnumerable<Billing> FilterAdjustmentName(this ICollection<Billing> payrolls, AdjustmentTypes adjustmentType)
        {
            return FilterAdjustmentName((IEnumerable<Billing>)payrolls, adjustmentType);
        }
    }

    public class BillingListingViewModel : CancellableBase, INavigationAware
    {

        private AdjustmentTypes _adjustmentName;
        private IEnumerable<Billing> _billings = Enumerable.Empty<Billing>();

        private readonly Models.Timesheets m_Timesheets;
        private readonly Billings m_Billings;
        private readonly IMessageBoxService s_Message;
        private readonly IDialogService s_Dialog;

        public BillingListingViewModel(IDialogService dialog, IMessageBoxService message, Billings billings, Models.Timesheets timesheets)
        {
            s_Dialog = dialog;
            s_Message = message;
            m_Billings = billings;
            m_Timesheets = timesheets;

            AddToAdjustmentCommand = new DelegateCommand<object?>(AddToAdjustment);
            ExportBillingsCommand = new DelegateCommand(ExportBillings);
            GenerateBillingsCommand = new DelegateCommand(GenerateBillings);
            AdjustmentNames = Enum.GetValues<AdjustmentTypes>();
        }

        public AdjustmentTypes AdjustmentName { get => _adjustmentName; set => SetProperty(ref _adjustmentName, value); }
        public IEnumerable<AdjustmentTypes> AdjustmentNames { get; }
        public IEnumerable<Billing> Billings { get => _billings; set => SetProperty(ref _billings, value); }
        public IMain? Main { get; set; }

        #region commands
        public DelegateCommand<object?> AddToAdjustmentCommand { get; }
        public DelegateCommand ExportBillingsCommand { get; }
        public DelegateCommand GenerateBillingsCommand { get; }
        #endregion

        #region Export
        private void ExportBillings()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = ExportBillings(cts.Token);
        }

        private async Task ExportBillings(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Main == null) throw new Exception(ErrorMessages.MainIsNull);
                if (Main.PayrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsNull);

                OnMessageSent("Exporting adjustments...");
                OnProgressStart();
                await m_Billings.Export(Billings, Main.CutoffId, Main.PayrollCode.PayrollCodeId, AdjustmentName, cancellationToken);
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

        #region GenerateBillings
        private void GenerateBillings()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = StartGenerateBillings(cts.Token);
        }

        private async Task StartGenerateBillings(CancellationToken cancellationToken = default)
        {
            try
            {
                await GenerateBillings(cancellationToken);
                await ListBillings(cancellationToken);
            }
            catch (Exception ex)
            {
                s_Message.ShowError(ex.Message);
            }
        }

        private async Task GenerateBillings(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Main == null) throw new Exception("Main not initialized.");
                if (Main.PayrollCode == null) throw new Exception("Empty payroll code.");

                OnMessageSent("Retrieving billing records...");
                OnProgressStart();
                var billingItems = new List<Billing>();
                var employeesWithPcv = await m_Billings.GetEmployeesWithPcv(Main.PayrollCode.PayrollCodeId, Main.CutoffId, cancellationToken);
                var employeesWithBillingRecord = await m_Billings.GetEmployeesWithBillingRecord(Main.PayrollCode.PayrollCodeId, Main.CutoffId, cancellationToken);
                var eeIds = employeesWithPcv.Union(employeesWithBillingRecord);

                OnMessageSent("Generating billing...");
                OnProgressStart(eeIds.Count());
                foreach (var eeId in eeIds)
                {
                    await m_Billings.ResetBillings(Main.CutoffId, eeId, cancellationToken);
                    var billingFromBillingRecord = await m_Billings.GenerateBillingFromBillingRecord(Main.CutoffId, eeId, cancellationToken);
                    var billingFromTimesheetView = await m_Billings.GenerateBillingFromTimesheetView(Main.CutoffId, eeId, cancellationToken);

                    OnProgressIncrement();
                }

                OnMessageSent("Saving generated billing from PCVs and Allowances...");
                OnProgressStart(billingItems.Count);
                foreach (var billingItem in billingItems)
                {
                    await m_Billings.AddBilling(billingItem, cancellationToken);
                    OnProgressIncrement();
                }

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

        #region ListBillings
        private void ListBillings()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = ListBillings(cts.Token);
        }

        private async Task ListBillings(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Main == null) throw new Exception("Main not initialized.");
                if (Main.PayrollCode == null) throw new Exception("Empty payroll code.");

                OnMessageSent("Retrieving billing info...");
                OnProgressStart();
                var billingItems = await m_Billings.GetBillings(Main.CutoffId, cancellationToken);
                Billings = billingItems.FilterPayrollCode(Main.PayrollCode.PayrollCodeId).FilterAdjustmentName(AdjustmentName);
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

        #region AddToAdjustment
        private void AddToAdjustment(object? parameter)
        {
            if (parameter is AdjustmentOptions adjustOption)
            {
                var cts = GetCancellationTokenSource();
                var dialogParameters = CreateDialogParameters(this, cts);
                s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
                _ = AddToAdjustment(adjustOption, cts.Token);
            }
        }

        private async Task AddToAdjustment(AdjustmentOptions adjustOption, CancellationToken cancellationToken = default)
        {
            try
            {
                if (Main == null) throw new Exception("Main not initialized.");

                OnMessageSent("Retrieving timesheets...");
                OnProgressStart();
                var timesheets = await m_Timesheets.GetTimesheets(Main.CutoffId, cancellationToken);

                OnMessageSent("Exporting adjustments...");
                OnProgressStart(Billings.Count());
                foreach (var billing in Billings)
                {
                    var timesheet = timesheets.SingleOrDefault(t => t.CutoffId == billing.CutoffId && t.EEId == billing.EEId);

                    if (timesheet != null)
                    {
                        if (!billing.Applied)
                        {
                            switch (adjustOption)
                            {
                                case AdjustmentOptions.ADJUST1:
                                    timesheet.Adjust1 += billing.Amount;
                                    break;
                                case AdjustmentOptions.ADJUST2:
                                    timesheet.Adjust2 += billing.Amount;
                                    break;
                            }
                        }
                        else if (billing.Applied && billing.AdjustmentOption != adjustOption)
                        {
                            switch (adjustOption)
                            {
                                case AdjustmentOptions.ADJUST1:
                                    timesheet.Adjust2 -= billing.Amount;
                                    timesheet.Adjust1 += billing.Amount;
                                    break;
                                case AdjustmentOptions.ADJUST2:
                                    timesheet.Adjust1 -= billing.Amount;
                                    timesheet.Adjust2 += billing.Amount;
                                    break;
                            }
                        }

                        await m_Timesheets.SaveTimesheet(timesheet, cancellationToken);
                        billing.AdjustmentOption = adjustOption;
                        billing.Applied = true;
                        await m_Billings.AddBilling(billing, cancellationToken);
                    }

                    OnProgressIncrement();
                }

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

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Main = navigationContext.Parameters.GetValue<IMain?>(PmsConstants.Main);
            if (Main != null)
            {
                Main.PropertyChanged += Main_PropertyChanged;
            }

            ListBillings();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (Main != null)
            {
                Main.PropertyChanged -= Main_PropertyChanged;
            }
        }

        private void Main_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ListBillings();
        }
        #endregion
    }
}
