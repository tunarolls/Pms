using Pms.Adjustments.Models;
using Pms.Adjustments.Module.Models;
using Pms.Common;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pms.Adjustments.Module.ViewModels
{
    public class BillingRecordDetailViewModel : CancellableBase, IDialogAware
    {
        private readonly BillingRecords m_BillingRecords;
        private readonly Employees m_Employees;
        private string _fullName = string.Empty;
        private BillingRecord? _billingRecord;
        private string _eeId = string.Empty;

        public BillingRecordDetailViewModel(BillingRecords billingRecords, Employees employees)
        {
            m_BillingRecords = billingRecords;
            m_Employees = employees;

            SaveCommand = new DelegateCommand(Save);

            PropertyChanged += BillingRecordDetailViewModel_PropertyChanged;
        }

        public IEnumerable<AdjustmentTypes> AdjustmentTypes { get; } = Enum.GetValues<AdjustmentTypes>();
        public IEnumerable<BillingRecordStatus> BillingRecordStatus { get; } = Enum.GetValues<BillingRecordStatus>();
        public IEnumerable<DeductionOptions> DeductionOptions { get; } = Enum.GetValues<DeductionOptions>();
        public BillingRecord? BillingRecord { get => _billingRecord; set => SetProperty(ref _billingRecord, value); }
        public string EEId { get => _eeId; set => SetProperty(ref _eeId, value); }

        //public string EEId
        //{
        //    get => eeId;
        //    set
        //    {
        //        SetProperty(ref eeId, value);

        //        _billingRecord.EE = Employees.Find(value);
        //        if (_billingRecord.EE is not null)
        //        {
        //            _billingRecord.EEId = eeId;
        //            Fullname = _billingRecord.EE.Fullname;
        //        }

        //    }
        //}

        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }

        #region save
        public DelegateCommand SaveCommand { get; }

        public void Save()
        {

        }

        private async Task Save(CancellationToken cancellationToken = default)
        {
            try
            {
                if (BillingRecord != null)
                {
                    var recordId = BillingRecord.GenerateId(BillingRecord);
                    await m_BillingRecords.SaveRecord(BillingRecord, cancellationToken);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception)
            {

            }
        }
        #endregion

        #region find employee
        private async Task FindEmployee(string eeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var ee = await m_Employees.Find(eeId, cancellationToken);
                if (ee != null)
                {

                }
            }
            catch
            {
            }
        }
        #endregion

        private void BillingRecordDetailViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BillingRecord))
            {
                EEId = BillingRecord?.EEId ?? string.Empty;
                FullName = BillingRecord?.EE?.Fullname ?? string.Empty;
            }
        }

        #region IDialogAware
        public string Title { get; set; } = string.Empty;

        public event Action<IDialogResult>? RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            BillingRecord = parameters.GetValue<BillingRecord?>(PmsConstants.BillingRecord);
        }
        #endregion
    }
}
