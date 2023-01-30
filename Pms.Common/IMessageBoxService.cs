using DryIoc;
using Pms.Common.Enums;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pms.Common
{
    public interface IMessageBoxService
    {
        void Show(string message, string caption = "");
        void ShowError(string message, string caption = "");
        void ShowPrompt(string message, string caption = "");
        void ShowDialog(string message, string title);
        void ShowDialog(string message, string title, string moreInfo);
        void ShowDialog(string message, IDialogParameters parameters, Action<IDialogResult> callback,
            string title = "",
            PromptDialogButton button = PromptDialogButton.Ok);
    }

    public class MessageBoxService : IMessageBoxService
    {
        private readonly IDialogService s_Dialog;

        public MessageBoxService(IDialogService dialog)
        {
            s_Dialog = dialog;
        }

        public void Show(string message, string caption = "")
        {
            MessageBox.Show(message, caption, button: MessageBoxButton.OK, icon: MessageBoxImage.None);
        }

        public void ShowDialog(string message, IDialogParameters parameters, Action<IDialogResult> callback, string title = "", PromptDialogButton button = PromptDialogButton.Ok)
        {
            parameters.Add(DialogParameterNames.Message, message);
            parameters.Add(DialogParameterNames.Title, title);
            parameters.Add(DialogParameterNames.PromptDialogButton, button);
            s_Dialog.ShowDialog(DialogNames.PromptDialog, parameters, callback);
        }

        public void ShowDialog(string message, string title)
        {
            var dialogParameters = new DialogParameters()
            {
                { DialogParameterNames.Message, message },
                { DialogParameterNames.Title, title },
                { DialogParameterNames.PromptDialogButton, PromptDialogButton.Ok }
            };
            s_Dialog.ShowDialog(DialogNames.PromptDialog, dialogParameters, (_) => { });
        }

        public void ShowDialog(string message, string title, string moreInfo)
        {
            var dialogParameters = new DialogParameters()
            {
                { DialogParameterNames.Message, message },
                { DialogParameterNames.Title, title },
                { DialogParameterNames.MoreInfo, moreInfo },
                { DialogParameterNames.PromptDialogButton, PromptDialogButton.Ok }
            };
            s_Dialog.ShowDialog(DialogNames.PromptDialog, dialogParameters, (_) => { });
        }

        public void ShowError(string message, string caption = "")
        {
            MessageBox.Show(message, caption, button: MessageBoxButton.OK, icon: MessageBoxImage.Error);
        }

        public void ShowPrompt(string message, string caption = "")
        {
            throw new NotImplementedException();
        }
    }

    public class DummyMessageBoxService : IMessageBoxService
    {
        public void Show(string message, string caption = "")
        {
            // do nothing
        }

        public void ShowDialog(string message, IDialogParameters parameters, Action<IDialogResult> callback, string caption = "", PromptDialogButton button = PromptDialogButton.Ok)
        {
            // do nothing
        }

        public void ShowDialog(string message, string title)
        {
            throw new NotImplementedException();
        }

        public void ShowDialog(string message, string title, string moreInfo)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string message, string caption = "")
        {
            // do nothing
        }

        public void ShowPrompt(string message, string caption = "")
        {
            // do nothing
        }
    }
}
