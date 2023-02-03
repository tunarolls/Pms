using Pms.Common;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Timesheets.Module.ViewModels
{
    public class TimesheetDetailViewModel : CancellableBase, IDialogAware
    {
        private Timesheet? _timesheet;
        private readonly Timesheets m_Timesheets;
        private readonly IDialogService s_Dialog;
        private readonly IMessageBoxService s_Message;

        public TimesheetDetailViewModel(IDialogService dialog, IMessageBoxService message, Timesheets timesheets)
        {
            m_Timesheets = timesheets;
            s_Dialog = dialog;
            s_Message = message;

            SaveCommand = new DelegateCommand(Save);
            CloseCommand = new DelegateCommand(Close);
        }

        public IEnumerable<TimesheetBankChoices> BankChoices { get; } = Enum.GetValues<TimesheetBankChoices>();
        public bool IsForEditing { get => !string.IsNullOrEmpty(_timesheet?.EEId); }
        public Timesheet? Timesheet { get => _timesheet; set => SetProperty(ref _timesheet, value); }

        #region dialog
        public event Action<IDialogResult>? RequestClose;

        public string Title { get; set; } = string.Empty;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Timesheet = parameters.GetValue<Timesheet?>(PmsConstants.Timesheet);
            RaisePropertyChanged(nameof(Timesheet));
            RaisePropertyChanged(nameof(IsForEditing));
        }
        #endregion

        #region commands
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CloseCommand { get; }
        #endregion

        #region save
        private void Save()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Save(cts.Token);
        }

        private async Task Save(CancellationToken cancellationToken = default)
        {
            try
            {
                OnMessageSent("Saving...");
                await m_Timesheets.SaveTimesheet(_timesheet, cancellationToken);
                OnTaskCompleted();
                s_Message.ShowDialog("Timesheet saved.", "Save");
                Close();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.GetBaseException().ToString(), "Save");
            }
        }
        #endregion

        private void Close()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.None));
        }
    }
}
