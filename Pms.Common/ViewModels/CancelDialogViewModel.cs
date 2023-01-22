using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common.ViewModels
{
    public class CancelDialogViewModel : BindableBase, IDialogAware
    {
        private INotifyTaskCompletion? _caller;
        private string _message = string.Empty;
        private string _title = string.Empty;
        private bool _canClose = false;
        public CancelDialogViewModel()
        {
            CancelCommand = new DelegateCommand(Cancel);
        }

        public DelegateCommand CancelCommand { get; }
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        private void Caller_TaskCompleted(object? sender, EventArgs e)
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
            _caller?.RaiseTaskCancelled();
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
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string?>(DialogParameterNames.Message) ?? string.Empty;
            Title = parameters.GetValue<string?>(DialogParameterNames.Title) ?? string.Empty;
            _caller = parameters.GetValue<INotifyTaskCompletion?>(DialogParameterNames.TaskCompletion);

            if (_caller != null)
            {
                _caller.TaskCompleted += Caller_TaskCompleted;
                _caller.MessageSent += Caller_MessageSent;
            }
        }
        #endregion
    }
}
