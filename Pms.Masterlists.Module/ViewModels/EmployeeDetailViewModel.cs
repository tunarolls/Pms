namespace Pms.Masterlists.Module.ViewModels
{
    //public class EmployeeDetailViewModel : BindableBase
    //{
    //    private Employee _employee;
    //    public EmployeeDetailViewModel(Employee employee, Models.Employees employees)
    //    {
    //        Employee = employee;
    //        Save = new Save(this, employees);
    //        Sync = new SyncOne(this, employees);

    //        PayrollCodes = WeakReferenceMessenger.Default.Send<CurrentPayrollCodesRequestMessage>().Response;
    //    }

    //    public event EventHandler OnRequestClose;

    //    public ObservableCollection<BankChoices> BankTypes
    //    {
    //        get
    //        {
    //            return new ObservableCollection<BankChoices>(Enum.GetValues(typeof(BankChoices)).Cast<BankChoices>());
    //        }
    //    }

    //    public Employee Employee { get => _employee; set => SetProperty(ref _employee, value); }
    //    public string[] PayrollCodes { get; }

    //    public ICommand Save { get; set; }

    //    public ObservableCollection<string> Sites
    //    {
    //        get
    //        {
    //            List<string> sites = new();
    //            foreach (SiteChoices site in Enum.GetValues(typeof(SiteChoices)))
    //                sites.Add(site.ToString());

    //            return new ObservableCollection<string>(sites);
    //        }
    //    }
    //    public ICommand Sync { get; set; }
    //    public void Close() => OnRequestClose?.Invoke(this, new EventArgs());

    //    public void RefreshProperties()
    //    {
    //        OnPropertyChanged(nameof(Employee));
    //    }
    //}
}
