using MySqlX.XDevAPI.Common;
using Pms.Adjustments.Models;
using Pms.Adjustments.Module.Models;
using Pms.Common;
using Pms.Masterlists.Entities;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            DetailCommand = new DelegateCommand<object?>(Detail);

            BillingRecords = new();
        }

        public IAdjustmentMain? Main { get; set; }
        public AdjustmentTypes AdjustmentName { get => _adjustmentName; set => SetProperty(ref _adjustmentName, value); }
        public RangedObservableCollection<BillingRecord> BillingRecords { get; set; }

        #region commands
        public DelegateCommand<object?> DetailCommand { get; }
        public DelegateCommand ImportCommand { get; }
        #endregion

        #region Detail
        private void Detail(object? parameter)
        {
            var dialogParams = new DialogParameters
            {
                { PmsConstants.BillingRecord, parameter }
            };

            s_Dialog.Show(ViewNames.BillingRecordDetailView, dialogParams, (_) => { });
        }
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
                foreach (var fileName in fileNames)
                {
                    OnProgressStart();

                    OnMessageSent($"Importing from {fileName}");
                    var records = await m_BillingRecords.Import(fileName, cancellationToken);

                    OnProgressStart(records.Count);
                    foreach (var record in records)
                    {
                        await m_BillingRecords.SaveRecord(record, cancellationToken);
                        OnProgressIncrement();
                    }
                }

                OnTaskCompleted();
                s_Message.ShowDialog("Import done.", "");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Import", ex.ToString());
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
                OnProgressStart();

                OnMessageSent("Retrieving billing info...");
                var payrollCode = Main?.PayrollCode?.PayrollCodeId;
                var billingRecords = await m_BillingRecords.GetByPayrollCode(payrollCode, cancellationToken);
                BillingRecords.ReplaceRange(billingRecords);

                OnTaskCompleted();
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "List billings", ex.ToString());
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
        #endregion

        private void Main_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ListBillings();
        }
    }
}
