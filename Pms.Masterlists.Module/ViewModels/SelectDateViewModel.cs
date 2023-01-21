using MathNet.Numerics;
using Pms.Common;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module.ViewModels
{
    public class SelectDateViewModel : BindableBase, IDialogAware
    {
        private DateTime _selectedDate;

        public SelectDateViewModel()
        {
            SelectCommand = new DelegateCommand(Select);
        }

        public DateTime SelectedDate { get => _selectedDate; set => SetProperty(ref _selectedDate, value); }

        public DelegateCommand SelectCommand { get; }

        private void Select()
        {
            var dialogParams = new DialogParameters()
            {
                { PmsConstants.SelectedDate, SelectedDate }
            };

            var dialogResult = new DialogResult(ButtonResult.OK, dialogParams);
            RequestClose?.Invoke(dialogResult);
        }

        #region IDialogAware
        public event Action<IDialogResult>? RequestClose;

        public string Title => "Please select date";
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            SelectedDate = DateTime.Now.AddDays(-15);
        }
        #endregion
    }
}
