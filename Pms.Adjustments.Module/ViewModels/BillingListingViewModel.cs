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
using Pms.Masterlists.Entities;
using System.Windows.Data;
using System.Collections.Specialized;

namespace Pms.Adjustments.Module.ViewModels
{
    static class PayrollFilterExtension
    {
        public static IEnumerable<Billing> FilterPayrollCode(this IEnumerable<Billing> payrolls, string payrollCode)
        {
            return !string.IsNullOrEmpty(payrollCode) ? payrolls.Where(t => t.EE != null && t.EE.PayrollCode == payrollCode) : payrolls;
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

        private AdjustmentTypes? _adjustmentName;

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

            Billings = new();
            var source = CollectionViewSource.GetDefaultView(Billings);
            source.Filter = t => FilterBillings(t);
            source.CollectionChanged += Billings_CollectionChanged;
        }

        public AdjustmentTypes? AdjustmentName
        {
            get => _adjustmentName;
            set
            {
                SetProperty(ref _adjustmentName, value);

                var source = CollectionViewSource.GetDefaultView(Billings);
                source.Refresh();
            }
        }
        public IEnumerable<AdjustmentTypes> AdjustmentNames { get; }
        public RangedObservableCollection<Billing> Billings { get; set; }
        public IAdjustmentMain? Main { get; set; }

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
                var cutoff = new Cutoff(Main?.CutoffId);
                var payrollCode = Main?.PayrollCode?.PayrollCodeId;
                var adjustment = AdjustmentName;
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);
                if (adjustment == null) throw new Exception(ErrorMessages.AdjustmentIsEmpty);

                var source = CollectionViewSource.GetDefaultView(Billings);
                var billings = source.OfType<Billing>();

                OnProgressStart();
                OnMessageSent("Exporting adjustments...");
                await Task.Run(() => m_Billings.Export(billings, cutoff.CutoffId, payrollCode, adjustment.Value), cancellationToken);

                OnTaskCompleted();
                s_Message.ShowDialog("Done.", "Export billings");
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export billings", ex.ToString());
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
            await GenerateBillings(cancellationToken);
            ListBillings();
        }

        private async Task GenerateBillings(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(Main?.CutoffId);
                var payrollCode = Main?.PayrollCode;
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);
                var billingItems = new List<Billing>();

                OnMessageSent("Retrieving billing records...");
                OnProgressStart();
                var employeesWithPcv = await m_Billings.GetEmployeesWithPcv(payrollCode.PayrollCodeId, cutoff.CutoffId, cancellationToken);
                var employeesWithBillingRecord = await m_Billings.GetEmployeesWithBillingRecord(payrollCode.PayrollCodeId, cancellationToken);
                var eeIds = employeesWithPcv.Union(employeesWithBillingRecord).Distinct();

                OnMessageSent("Generating billing...");
                OnProgressStart(eeIds.Count());
                foreach (var eeId in eeIds)
                {
                    if (eeId != null)
                    {
                        await m_Billings.ResetBillings(cutoff.CutoffId, eeId, cancellationToken);
                        var billingFromBillingRecord = await m_Billings.GenerateBillingFromBillingRecord(cutoff.CutoffId, eeId, cancellationToken);
                        var billingFromTimesheetView = await m_Billings.GenerateBillingFromTimesheetView(cutoff.CutoffId, eeId, cancellationToken);
                        billingItems.AddRange(billingFromBillingRecord);
                        billingItems.AddRange(billingFromTimesheetView);
                    }
                    
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
                s_Message.ShowDialog("Done.", "Generate billings");
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Generate billings", ex.ToString());
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
                var cutoff = new Cutoff(Main?.CutoffId);
                var payrollCode = Main?.PayrollCode?.PayrollCodeId;

                OnProgressStart();

                OnMessageSent("Retrieving billing info...");
                var billings = await m_Billings.GetBillings(cutoff.CutoffId, payrollCode, cancellationToken);
                Billings.ReplaceRange(billings);
                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "List billings", ex.ToString());
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
                var cutoff = new Cutoff(Main?.CutoffId);

                OnProgressStart();
                OnMessageSent("Retrieving timesheets...");
                var timesheets = await m_Timesheets.GetTimesheets(cutoff.CutoffId, cancellationToken);

                OnMessageSent("Exporting adjustments...");
                var source = CollectionViewSource.GetDefaultView(Billings);
                var billings = source.OfType<Billing>();
                OnProgressStart(billings.Count());

                foreach (var billing in billings)
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

                s_Message.ShowDialog("Done.", "Add to adjust");
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Add to adjust", ex.ToString());
            }
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Main = navigationContext.Parameters.GetValue<IAdjustmentMain?>(PmsConstants.Main);

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

        private bool FilterBillings(object t)
        {
            if (t is Billing billing)
            {
                var adjustmentMatch = AdjustmentName == null || billing.AdjustmentType == AdjustmentName;

                return adjustmentMatch;
            }

            return false;
        }

        private void Billings_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
}
