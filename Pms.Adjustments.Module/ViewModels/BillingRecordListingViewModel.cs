using Pms.Adjustments.Models;
using Pms.Adjustments.Module.Models;
using Pms.Common;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Adjustments.Module.ViewModels
{
    public class BillingRecordListingViewModel : CancellableBase, INavigationAware
    {

        private AdjustmentTypes _adjustmentName;
        private IEnumerable<BillingRecord> _billingRecords = Enumerable.Empty<BillingRecord>();
        private readonly IMessageBoxService s_Message;
        private readonly IDialogService s_Dialog;
        private readonly BillingRecords m_BillingRecords;
        private readonly Employees m_Employees;
        private readonly IFileDialogService s_FileDialog;

        public BillingRecordListingViewModel(BillingRecords billingRecords, Employees employees,
            IMessageBoxService message, IDialogService dialog, IFileDialogService fileDialog)
        {
            s_Dialog = dialog;
            s_FileDialog = fileDialog;
            s_Message = message;
            m_BillingRecords = billingRecords;
            m_Employees = employees;

            ImportCommand = new DelegateCommand(Import);

            //payrollCodeId = WeakReferenceMessenger.Default.Send<CurrentPayrollCodeRequestMessage>().Response.PayrollCodeId;
            //string cutoffId = WeakReferenceMessenger.Default.Send<CurrentCutoffIdRequestMessage>();
            //if (!string.IsNullOrEmpty(cutoffId))
            //    cutoff = new Cutoff(cutoffId);

            //IsActive = true;


            //Detail = new Detail(billingRecords, employees);
            //Import = new Import(this, billingRecords);

            //ListBillings = new Listing(this, billingRecords);
            //ListBillings.Execute(null);
        }

        public IMain? Main { get; set; }
        public AdjustmentTypes AdjustmentName { get => _adjustmentName; set => SetProperty(ref _adjustmentName, value); }
        public IEnumerable<BillingRecord> BillingRecords { get => _billingRecords; set => SetProperty(ref _billingRecords, value); }

        #region commands
        public DelegateCommand DetailCommand { get; }
        public DelegateCommand ImportCommand { get; }
        public DelegateCommand ListBillingsCommand { get; }
        #endregion

        #region Detail

        #endregion

        #region Import
        private void Import()
        {
            s_FileDialog.ShowMultiFileDialog(ImportCallback, FileFilters.BillingRecordImport);
        }

        private void ImportCallback(IFileDialogResult result)
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Import(result.FileNames, cts.Token);
        }

        private async Task Import(string[] fileNames, CancellationToken cancellationToken = default)
        {
            try
            {
                OnMessageSent("Importing...");
                OnProgressStart();

                foreach (var fileName in fileNames)
                {
                    OnMessageSent($"Importing from {fileName}");
                    OnProgressStart();

                    var records = await m_BillingRecords.Import(fileName, cancellationToken);
                    OnProgressStart(records.Count);

                    foreach (var record in records)
                    {
                        await m_BillingRecords.SaveRecord(record, cancellationToken);
                        OnProgressIncrement();
                    }
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

        }

        private async Task ListBillings(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Main == null) throw new Exception(ErrorMessages.MainIsNull);
                if (Main.PayrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsNull);

                var billingRecordItems = await m_BillingRecords.GetByPayrollCode(Main.PayrollCode.PayrollCodeId, cancellationToken);
                BillingRecords = billingRecordItems;

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
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
        #endregion

        //protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if ((new string[] { nameof(PayrollCodeId), nameof(Cutoff) }).Any(p => p == e.PropertyName))
        //        ListBillings.Execute(null);

        //    base.OnPropertyChanged(e);
        //}

        //protected override void OnActivated()
        //{
        //    Messenger.Register<BillingRecordListingViewModel, SelectedPayrollCodeChangedMessage>(this, (r, m) => r.PayrollCodeId = m.Value.PayrollCodeId);
        //    Messenger.Register<BillingRecordListingViewModel, SelectedCutoffIdChangedMessage>(this, (r, m) => r.Cutoff = new Cutoff(m.Value));
        //}
    }
}
