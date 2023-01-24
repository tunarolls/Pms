using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Common.ViewModels
{
    public class CancelDialogViewModel : BindableBase, IDialogAware
    {
        private INotifyTaskCompletion? _caller;
        private string _message = string.Empty;
        private string _title = string.Empty;
        private bool _canClose = false;
        private CancellationTokenSource? c_Cts;
        private bool _hasErrors = false;
        private int _errorCount = 0;
        private bool _isIndeterminate = true;
        private int _progressMax;
        private int _progressValue;

        public CancelDialogViewModel()
        {
            CancelCommand = new DelegateCommand(Cancel);
        }

        public DelegateCommand CancelCommand { get; }
        public string Message { get => _message; set => SetProperty(ref _message, value); }
        public bool HasErrors { get => _hasErrors; set => SetProperty(ref _hasErrors, value); }
        public int ErrorCount { get => _errorCount; set => SetProperty(ref _errorCount, value); }
        public bool IsIndeterminate { get => _isIndeterminate; set => SetProperty(ref _isIndeterminate, value); }
        public int ProgressMax { get => _progressMax; set => SetProperty(ref _progressMax, value); }
        public int ProgressValue { get => _progressValue; set => SetProperty(ref _progressValue, value); }
        public string ProgressStatus => $"{ProgressValue} of {ProgressMax}";

        private void Caller_TaskCompleted(object? sender, EventArgs e)
        {
            var result = new DialogResult(ButtonResult.None);
            _canClose = true;
            RequestClose?.Invoke(result);
        }

        private void Caller_TaskException(object? sender, EventArgs e)
        {
            var result = new DialogResult(ButtonResult.None);
            _canClose = true;
            RequestClose?.Invoke(result);
        }

        private void Caller_MessageSent(object? sender, MessageSentEventArgs e)
        {
            Message = e.Message;
        }

        private void Cancel()
        {
            c_Cts?.Cancel();
            //_caller?.RaiseTaskCancelled();
        }

        #region IDialogAware

        public event Action<IDialogResult>? RequestClose;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public bool CanCloseDialog()
        {
            return _canClose;
        }

        public void OnDialogClosed()
        {
            if (_caller != null)
            {
                _caller.Enable();
                _caller.TaskCompleted -= Caller_TaskCompleted;
                _caller.TaskException -= Caller_TaskException;
                _caller.MessageSent -= Caller_MessageSent;
                _caller.ErrorFound -= Caller_ErrorFound;
                _caller.ProgressStart -= Caller_ProgressStart;
                _caller.ProgressIncrement -= Caller_ProgressIncrement;
            }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string?>(DialogParameterNames.Message) ?? string.Empty;
            Title = parameters.GetValue<string?>(DialogParameterNames.Title) ?? string.Empty;
            _caller = parameters.GetValue<INotifyTaskCompletion?>(DialogParameterNames.TaskCompletion);
            c_Cts = parameters.GetValue<CancellationTokenSource?>(DialogParameterNames.CancellationTokenSource);

            if (_caller != null)
            {
                _caller.Disable();
                _caller.TaskCompleted += Caller_TaskCompleted;
                _caller.TaskException += Caller_TaskException;
                _caller.MessageSent += Caller_MessageSent;
                _caller.ErrorFound += Caller_ErrorFound;
                _caller.ProgressStart += Caller_ProgressStart;
                _caller.ProgressIncrement += Caller_ProgressIncrement;
            }
        }
        #endregion

        private void Caller_ErrorFound(object? sender, EventArgs e)
        {
            ErrorCount += 1;
            HasErrors = ErrorCount > 0;
        }

        private void Caller_ProgressIncrement(object? sender, ProgressIncrementEventArgs e)
        {
            ProgressValue += e.Increment < 1 ? 1 : e.Increment;
            RaisePropertyChanged(nameof(ProgressStatus));
        }

        private void Caller_ProgressStart(object? sender, ProgressStartEventArgs e)
        {
            ProgressMax = e.ProgressMax;
            ProgressValue = 0;
            IsIndeterminate = e.ProgressMax == 0;
            RaisePropertyChanged(nameof(ProgressStatus));
        }
    }
}
