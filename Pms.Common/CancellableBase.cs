using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Common
{
    public class MessageSentEventArgs : EventArgs
    {
        public MessageSentEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public interface INotifyTaskCompletion
    {
        event EventHandler<EventArgs> TaskCompleted;
        event EventHandler<EventArgs> TaskCancelled;
        event EventHandler<MessageSentEventArgs> MessageSent;

        void RaiseTaskCancelled();
    }

    public abstract class CancellableBase : BindableBase, INotifyTaskCompletion
    {
        private CancellationTokenSource? l_Cts;

        protected CancellableBase()
        {
            TaskCancelled += CancellableBase_TaskCancelled;
        }

        public event EventHandler<MessageSentEventArgs>? MessageSent;

        public event EventHandler<EventArgs>? TaskCancelled;

        public event EventHandler<EventArgs>? TaskCompleted;

        public void RaiseTaskCancelled()
        {
            TaskCancelled?.Invoke(this, EventArgs.Empty);
        }

        protected IDialogParameters CreateDialogParameters(string title, string message, INotifyTaskCompletion taskCompletion)
        {
            return new DialogParameters()
            {
                { DialogParameterNames.Message, message },
                { DialogParameterNames.Title, title },
                { DialogParameterNames.TaskCompletion, taskCompletion }
            };
        }

        protected CancellationTokenSource GetCancellationTokenSource()
        {
            l_Cts = new CancellationTokenSource();
            return l_Cts;
        }

        protected void OnMessageSent(MessageSentEventArgs e)
        {
            MessageSent?.Invoke(this, e);
        }

        protected void OnTaskCancelled(object? sender, EventArgs e)
        {
            l_Cts?.Cancel();
        }

        protected void OnTaskCompleted()
        {
            TaskCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void CancellableBase_TaskCancelled(object? sender, EventArgs e)
        {
            l_Cts?.Cancel();
        }
    }
}
