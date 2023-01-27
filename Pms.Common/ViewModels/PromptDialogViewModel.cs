using Pms.Common.Enums;
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
    public class PromptDialogViewModel : BindableBase, IDialogAware
    {
        private IDialogParameters? _dialogParameters;

        public PromptDialogViewModel()
        {
            PromptCommand = new DelegateCommand<object?>(Prompt);
        }

        public PromptDialogButton PromptDialogButton { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public DelegateCommand<object?> PromptCommand { get; }

        private void Prompt(object? parameter)
        {
            if (parameter is ButtonResult result)
            {
                RequestClose?.Invoke(new DialogResult(result, _dialogParameters));
            }
        }

        #region IDialogAware
        public string Title { get; private set; } = string.Empty;

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
            _dialogParameters = parameters;

            PromptDialogButton = parameters.GetValue<PromptDialogButton?>(DialogParameterNames.PromptDialogButton) ?? PromptDialogButton.Ok;
            Message = parameters.GetValue<string?>(DialogParameterNames.Message) ?? string.Empty;
            Title = parameters.GetValue<string?>(DialogParameterNames.Title) ?? string.Empty;
            RaisePropertyChanged(nameof(PromptDialogButton));
            RaisePropertyChanged(nameof(Message));
            RaisePropertyChanged(nameof(Title));
        }
        #endregion
    }
}
