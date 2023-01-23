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
    }

    public class MessageBoxService : IMessageBoxService
    {
        public void Show(string message, string caption = "")
        {
            MessageBox.Show(message, caption, button: MessageBoxButton.OK, icon: MessageBoxImage.None);
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
