using Pms.Masterlists.Entities;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Pms.Masterlists.Module.ViewModels
{
    public class EmployeeDetailViewModel : BindableBase, IDialogAware
    {
        private readonly Employees m_Employees;
        private readonly PayrollCodes m_PayrollCodes;
        private readonly IDialogService s_Dialog;
        private Employee? l_Employee;
        public EmployeeDetailViewModel(IDialogService dialogService, Employees employees, PayrollCodes payrollCodes)
        {
            m_Employees = employees;
            m_PayrollCodes = payrollCodes;
            s_Dialog = dialogService;

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

        public IEnumerable<string> PayrollCodes { get; }

        #region init
        private void Init()
        {
        }
        #endregion

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
