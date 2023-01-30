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
    public enum TaskResult
    {
        CancelledByUser,
        CancelledByException,
        Completed
    }

    public interface ITaskCompletionResult
    {
        public TaskResult TaskResult { get; }
    }

    public class TaskCompletionResult : ITaskCompletionResult
    {
        public TaskCompletionResult(TaskResult taskResult)
        {
            TaskResult = taskResult;
        }

        public TaskResult TaskResult { get; }
    }

    public class MessageSentEventArgs : EventArgs
    {
        public MessageSentEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class ProgressStartEventArgs : EventArgs
    {
        public ProgressStartEventArgs(int progressMax)
        {
            ProgressMax = progressMax;
        }

        public int ProgressMax { get; }
    }

    public class ProgressIncrementEventArgs : EventArgs
    {
        public ProgressIncrementEventArgs(int increment)
        {
            Increment = increment;
        }

        public int Increment { get; }
    }

    public interface INotifyTaskCompletion
    {
        event EventHandler<EventArgs> TaskCompleted;
        event EventHandler<EventArgs> TaskCancelled;
        event EventHandler<EventArgs> TaskException;
        event EventHandler<MessageSentEventArgs> MessageSent;
        event EventHandler<Exception> ErrorFound;
        event EventHandler<ProgressStartEventArgs> ProgressStart;
        event EventHandler<ProgressIncrementEventArgs> ProgressIncrement;

        void Enable();
        void Disable();
    }

    public abstract class CancellableBase : BindableBase, INotifyTaskCompletion
    {
        private bool l_IsEnabled = true;

        public event EventHandler<MessageSentEventArgs>? MessageSent;

        public event EventHandler<EventArgs>? TaskCancelled;

        public event EventHandler<EventArgs>? TaskCompleted;

        public event EventHandler<EventArgs>? TaskException;

        public event EventHandler<Exception>? ErrorFound;
        public event EventHandler<ProgressStartEventArgs>? ProgressStart;
        public event EventHandler<ProgressIncrementEventArgs>? ProgressIncrement;

        public bool IsEnabled { get => l_IsEnabled; set => SetProperty(ref l_IsEnabled, value); }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        protected IDialogParameters CreateDialogParameters(string title, string message,
            INotifyTaskCompletion taskCompletion, CancellationTokenSource cts)
        {
            return new DialogParameters()
            {
                { DialogParameterNames.Message, message },
                { DialogParameterNames.Title, title },
                { DialogParameterNames.TaskCompletion, taskCompletion },
                { DialogParameterNames.CancellationTokenSource, cts }
            };
        }

        protected IDialogParameters CreateDialogParameters(INotifyTaskCompletion taskCompletion, CancellationTokenSource cts)
        {
            return new DialogParameters()
            {
                { DialogParameterNames.Message, "Starting..." },
                { DialogParameterNames.Title, "Loading" },
                { DialogParameterNames.TaskCompletion, taskCompletion },
                { DialogParameterNames.CancellationTokenSource, cts }
            };
        }

        protected CancellationTokenSource GetCancellationTokenSource()
        {
            return new CancellationTokenSource();
        }

        protected void OnMessageSent(string message)
        {
            MessageSent?.Invoke(this, new MessageSentEventArgs(message));
        }

        protected void OnTaskCancelled()
        {
            TaskCancelled?.Invoke(this, EventArgs.Empty);
        }

        protected void OnTaskCompleted()
        {
            TaskCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected void OnTaskException()
        {
            TaskException?.Invoke(this, EventArgs.Empty);
        }

        protected void OnErrorFound()
        {
            ErrorFound?.Invoke(this, new Exception());
        }

        protected void OnErrorFound(Exception ex)
        {
            ErrorFound?.Invoke(this, ex);
        }

        protected void OnProgressIncrement(int increment = 1)
        {
            ProgressIncrement?.Invoke(this, new ProgressIncrementEventArgs(increment));
        }

        protected void OnProgressStart(int progressMax = 0)
        {
            ProgressStart?.Invoke(this, new ProgressStartEventArgs(progressMax));
        }
    }
}
