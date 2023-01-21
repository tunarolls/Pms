using Pms.Masterlists.Entities;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Pms.Masterlists.Module.ViewModels
{
    public class EmployeeDetailViewModel : BindableBase, IDialogAware
    {
        private Employee? l_Employee;
        private readonly Employees m_Employees;

        public EmployeeDetailViewModel(Employees employees)
        {
            m_Employees = employees;

            //Save = new Save(this, employees);
            //Sync = new SyncOne(this, employees);

            //PayrollCodes = WeakReferenceMessenger.Default.Send<CurrentPayrollCodesRequestMessage>().Response;
        }

        #region IDialogAware
        public event Action<IDialogResult>? RequestClose;
        public string Title => "Employee Detail";

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            l_Employee = parameters.GetValue<Employee?>(Common.PmsConstants.Timesheet);
            RaisePropertyChanged(nameof(Employee));
        }
        #endregion

        //public ObservableCollection<BankChoices> BankTypes
        //{
        //    get
        //    {
        //        return new ObservableCollection<BankChoices>(Enum.GetValues(typeof(BankChoices)).Cast<BankChoices>());
        //    }
        //}

        public Employee? Employee { get => l_Employee; set => SetProperty(ref l_Employee, value); }

        //public string[] PayrollCodes { get; }

        //public ICommand Save { get; set; }

        //public ObservableCollection<string> Sites
        //{
        //    get
        //    {
        //        List<string> sites = new();
        //        foreach (SiteChoices site in Enum.GetValues(typeof(SiteChoices)))
        //            sites.Add(site.ToString());

        //        return new ObservableCollection<string>(sites);
        //    }
        //}
        //public ICommand Sync { get; set; }
        //public void Close() => OnRequestClose?.Invoke(this, new EventArgs());

        //public void RefreshProperties()
        //{
        //    OnPropertyChanged(nameof(Employee));
        //}
    }
}
