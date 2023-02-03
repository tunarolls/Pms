using FastExpressionCompiler.LightExpression;
using Google.Protobuf.WellKnownTypes;
using NPOI.HPSF;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Exceptions;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pms.Masterlists.Module.ViewModels
{
    public class DummyEmployeeListingViewMode : EmployeeListingViewModel
    {
        public DummyEmployeeListingViewMode(Employees employees, IMessageBoxService message, IDialogService dialog, IFileDialogService fileDialog)
            : base(employees, message, dialog, fileDialog)
        {
        }
    }

    public class EmployeeListingViewModel : CancellableBase, INavigationAware, IRegionMemberLifetime
    {
        private readonly Employees m_Employees;
        private readonly IDialogService s_Dialog;
        private readonly IFileDialogService s_FileDialog;
        private readonly IMessageBoxService s_Message;

        public EmployeeListingViewModel(Employees employees,
            IMessageBoxService message,
            IDialogService dialog,
            IFileDialogService fileDialog)
        {
            s_Message = message;
            m_Employees = employees;
            s_Dialog = dialog;
            s_FileDialog = fileDialog;

            SyncResignedCommand = new DelegateCommand(SyncResigned);
            SyncNewlyHiredCommand = new DelegateCommand(SyncNewlyHired);
            SyncAllCommand = new DelegateCommand(SyncAll);

            BankImportCommand = new DelegateCommand(BankImport);
            EEDataImportCommand = new DelegateCommand(EEDataImport);
            MasterFileImportCommand = new DelegateCommand(MasterFileImport);

            MasterlistExportCommand = new DelegateCommand(MasterlistExport);
            UnknownTinExportCommand = new DelegateCommand(UnknownTinExport);

            CheckDetailCommand = new DelegateCommand<object?>(CheckDetail);
            OpenPayrollCodeViewCommand = new DelegateCommand(OpenPayrollCodeView);

            Employees = new RangedObservableCollection<Employee>();
            var source = CollectionViewSource.GetDefaultView(Employees);
            source.Filter = t => FilterEmployee(t);
            source.CollectionChanged += Employees_CollectionChanged;
        }

        

        public bool KeepAlive => true;
        public IMasterlistsMain? Main { get; set; }

        #region properties
        private int _activeEECount;
        public int ActiveEECount { get => _activeEECount; set => SetProperty(ref _activeEECount, value); }

        public RangedObservableCollection<Employee> Employees { get; set; }

        private bool _hideArchived;
        public bool HideArchived
        {
            get => _hideArchived;
            set
            {
                if (SetProperty(ref _hideArchived, value))
                {
                    var source = CollectionViewSource.GetDefaultView(Employees);
                    source.Refresh();
                }
            }
        }

        private int _nonActiveEECount;
        public int NonActiveEECount { get => _nonActiveEECount; set => SetProperty(ref _nonActiveEECount, value); }

        private string _searchInput = string.Empty;
        public string SearchInput
        {
            get => _searchInput;
            set
            {
                if (SetProperty(ref _searchInput, value))
                {
                    var source = CollectionViewSource.GetDefaultView(Employees);
                    source.Refresh();
                }
            }
        }
        #endregion

        #region commands
        public DelegateCommand MasterlistExportCommand { get; }
        public DelegateCommand BankImportCommand { get; }
        public DelegateCommand<object?> CheckDetailCommand { get; }
        public DelegateCommand EEDataImportCommand { get; }
        public DelegateCommand MasterFileImportCommand { get; }
        public DelegateCommand UnknownTinExportCommand { get; }
        public DelegateCommand OpenPayrollCodeViewCommand { get; }
        public DelegateCommand SyncAllCommand { get; }
        public DelegateCommand SyncNewlyHiredCommand { get; }
        public DelegateCommand SyncResignedCommand { get; }
        #endregion

        public void SelectDate(Action<IDialogResult> selectDateCallback)
        {
            s_Dialog.ShowDialog(ViewNames.SelectDateView, selectDateCallback);
        }

        #region sync newly hired
        public void SyncNewlyHired()
        {
            SelectDate(SyncNewlyHiredCallback);
        }

        public void SyncNewlyHiredCallback(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                var selectedDate = result.Parameters.GetValue<DateTime?>(PmsConstants.SelectedDate) ?? DateTime.Now;
                var cts = GetCancellationTokenSource();
                var dialogParameters = CreateDialogParameters(this, cts);
                s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
                _ = SyncNewlyHired(selectedDate, cts.Token);
            }
        }

        private async Task SyncNewlyHired(DateTime selectedDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var site = Main?.Site ?? SiteChoices.MANILA;
                var exceptions = new List<Exception>();

                OnProgressStart();
                OnMessageSent("Retrieving HRMS data...");
                var employees = await m_Employees.SyncNewlyHired(selectedDate, site, cancellationToken);

                if (!employees.Any())
                {
                    OnTaskCompleted();
                    s_Message.ShowDialog("No employees synced.", "Sync");
                    return;
                }

                OnMessageSent("Syncing...");
                OnProgressStart(employees.Count);
                foreach (var employee in employees)
                {
                    try
                    {
                        var foundEmployee = await m_Employees.FindEmployee(employee.EEId, cancellationToken);
                        if (foundEmployee != null)
                        {
                            employee.Active = true;
                            await m_Employees.Save(employee, cancellationToken);
                        }
                    }
                    catch (TaskCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        exceptions.Add(new Exception($"Error found with employee id: {employee.EEId}", ex));
                        OnErrorFound();
                    }
                    finally { OnProgressIncrement(); }
                }

                await Task.Run(() =>
                {
                    m_Employees.ReportExceptions(exceptions, new PayrollCode(), $"{site}-NEWLYHIRED");
                }, cancellationToken);

                if (exceptions.Any())
                {
                    throw new AggregateException(exceptions);
                }
                else
                {
                    var payrollCode = Main?.PayrollCode?.PayrollCodeId;
                    await LoadValues(payrollCode, cancellationToken);
                    OnTaskCompleted();
                    s_Message.ShowDialog("Operation completed without errors.", "Sync");
                }
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (AggregateException ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Sync", ex.ToString());
            }
            catch (HttpRequestException ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ErrorMessages.HrmsHttpRequestError, "Sync", ex.ToString());
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Sync", ex.ToString());
            }
        }
        #endregion

        #region sync resigned
        public void SyncResigned()
        {
            SelectDate(SyncResignedCallback);
        }

        public void SyncResignedCallback(IDialogResult result)
        {
            if (result.Result == ButtonResult.OK)
            {
                var selectedDate = result.Parameters.GetValue<DateTime?>(PmsConstants.SelectedDate) ?? DateTime.Now;
                var cts = GetCancellationTokenSource();
                var dialogParameters = CreateDialogParameters(this, cts);
                s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
                _ = SyncResigned(selectedDate, cts.Token);
            }
        }

        private async Task SyncResigned(DateTime selectedDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var site = Main?.Site ?? SiteChoices.MANILA;
                var exceptions = new List<Exception>();

                OnProgressStart();
                OnMessageSent("Retrieving HRMS data...");
                var employees = await m_Employees.SyncResigned(selectedDate, site, cancellationToken);

                if (!employees.Any())
                {
                    OnTaskCompleted();
                    s_Message.ShowDialog("No employees synced.", "Sync");
                    return;
                }

                OnMessageSent("Syncing...");
                OnProgressStart(employees.Count);
                foreach (var employee in employees)
                {
                    try
                    {
                        if (employee.JobRemarks != JobRemarks.Transferred)
                        {
                            employee.Active = false;
                            await m_Employees.Save(employee, cancellationToken);
                        }
                    }
                    catch (TaskCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        exceptions.Add(new Exception($"Error found with employee id: {employee.EEId}", ex));
                        OnErrorFound();
                    }
                    finally { OnProgressIncrement(); }
                }

                await Task.Run(() =>
                {
                    m_Employees.ReportExceptions(exceptions, new PayrollCode(), $"{site}-RESIGNED");
                }, cancellationToken);

                if (exceptions.Any())
                {
                    throw new AggregateException(exceptions);
                }
                else
                {
                    var payrollCode = Main?.PayrollCode?.PayrollCodeId;
                    await LoadValues(payrollCode, cancellationToken);
                    OnTaskCompleted();
                    s_Message.ShowDialog("Operation completed without errors.", "Sync");
                }
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (AggregateException ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Sync", ex.ToString());
            }
            catch (HttpRequestException ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ErrorMessages.HrmsHttpRequestError, "Sync", ex.ToString());
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Sync", ex.ToString());
            }
        }
        #endregion

        #region sync all
        private readonly SemaphoreSlim _syncSs = new(5);

        private void SyncAll()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            var source = CollectionViewSource.GetDefaultView(Employees);
            var eeIds = source.OfType<Employee>().Select(t => t.EEId).ToArray();
            _ = SyncAll(eeIds, cts.Token);
        }

        private async Task SyncAll(string[] eeIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var site = Main?.Site ?? SiteChoices.MANILA;
                var exceptions = new List<Exception>();
                var tasks = new List<Task>();

                OnMessageSent("Syncing...");
                OnProgressStart(eeIds.Length);
                foreach (var eeId in eeIds)
                {
                    tasks.Add(SyncOne(eeId, site.ToString(), cancellationToken));
                }

                while (tasks.Any())
                {
                    var completedTask = await Task.WhenAny(tasks);

                    if (completedTask.Exception?.InnerException is Exception ex)
                    {
                        switch (ex)
                        {
                            case InvalidFieldValueException:
                            case InvalidFieldValuesException:
                            case DuplicateBankInformationException:
                            default:
                                exceptions.Add(ex);
                                break;
                        }
                    }

                    tasks.Remove(completedTask);
                }

                if (exceptions.Any())
                {
                    await Task.Run(() =>
                    {
                        m_Employees.ReportExceptions(exceptions, new PayrollCode(), $"{site}-REGULAR");
                    }, cancellationToken);

                    throw new AggregateException(exceptions);
                }
                else
                {
                    var payrollCode = Main?.PayrollCode?.PayrollCodeId;
                    await LoadValues(payrollCode, cancellationToken);
                    OnTaskCompleted();
                    s_Message.ShowDialog("Operation completed without issues.", "Sync");
                }
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (AggregateException ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Sync", ex.ToString());
            }
            catch (HttpRequestException ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ErrorMessages.HrmsHttpRequestError, "Sync", ex.ToString());
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Sync", ex.ToString());
            }
        }

        private async Task SyncOne(string eeId, string site, CancellationToken cancellationToken = default)
        {
            try
            {
                await _syncSs.WaitAsync(cancellationToken);

                var foundEmployee = await m_Employees.SyncOne(eeId, site.ToString(), cancellationToken);
                var foundEmployeeLocal = await m_Employees.FindEmployee(eeId, cancellationToken);

                if (foundEmployee == null && foundEmployeeLocal == null)
                {
                    var employee = new Employee()
                    {
                        EEId = eeId,
                        Active = false
                    };
                    await m_Employees.Save(employee, cancellationToken);
                }
                else if (foundEmployee == null && foundEmployeeLocal != null)
                {
                    foundEmployeeLocal.Active = false;
                    await m_Employees.Save(foundEmployeeLocal, cancellationToken);
                }
                else if (foundEmployee != null)
                {
                    foundEmployee.Active = true;
                    await m_Employees.Save(foundEmployee, cancellationToken);
                }
            }
            catch
            {
                OnErrorFound();
                throw;
            }
            finally
            {
                _syncSs.Release();
                OnProgressIncrement();
            }
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Main = navigationContext.Parameters.GetValue<IMasterlistsMain?>(PmsConstants.Main);
            if (Main != null)
            {
                Main.PropertyChanged += Main_PropertyChanged;
            }

            LoadValues();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (Main != null)
            {
                Main.PropertyChanged -= Main_PropertyChanged;
            }
        }
        #endregion

        private void Main_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            LoadValues();
        }

        private void LoadValues()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = StartLoadValues(cts.Token);
        }

        private async Task StartLoadValues(CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCode = Main?.PayrollCode?.PayrollCodeId;
                await LoadValues(payrollCode, cancellationToken);
                OnTaskCompleted();
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Load", ex.ToString());
            }
        }

        private async Task LoadValues(string? payrollCode, CancellationToken cancellationToken = default)
        {
            try
            {
                OnProgressStart();
                OnMessageSent("Loading employees...");
                var employees = await m_Employees.GetEmployees(payrollCode, cancellationToken);
                Employees.ReplaceRange(employees);
            }
            catch
            {
                throw;
            }
        }

        #region Bank import
        private void BankImport()
        {
            s_FileDialog.ShowMultiFileDialog(BankImportCallback);
        }

        private void BankImportCallback(IFileDialogResult result)
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = BankImport(result.FileNames, cts.Token);
        }

        private async Task BankImport(string[] fileNames, CancellationToken cancellationToken = default)
        {
            var errors = new Dictionary<string, List<Exception>>();

            try
            {
                foreach (var fileName in fileNames)
                {
                    errors.TryAdd(fileName, new List<Exception>());

                    try
                    {
                        OnMessageSent($"Saving extracted employee bank information from {Path.GetFileName(fileName)}...");
                        var extractedEmployee = await m_Employees.ImportBankInformation(fileName, cancellationToken);
                        OnProgressStart(extractedEmployee.Count);

                        foreach (var employee in extractedEmployee)
                        {
                            try
                            {
                                await m_Employees.Save(employee, cancellationToken);
                            }
                            catch (TaskCanceledException) { throw; }
                            catch (InvalidFieldValueException ex) { errors[fileName].Add(ex); OnErrorFound(); }
                            catch (DuplicateBankInformationException ex) { errors[fileName].Add(ex); OnErrorFound(); }
                            finally { OnProgressIncrement(); }
                        }
                    }
                    catch (TaskCanceledException) { throw; }
                    catch (Exception ex) { errors[fileName].Add(ex); OnErrorFound(); }
                }

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message, "Unexpected error");
            }
        }
        #endregion

        #region EE data import
        private void EEDataImport()
        {
            s_FileDialog.ShowMultiFileDialog(EEDataImportCallback);
        }

        private void EEDataImportCallback(IFileDialogResult result)
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Loading", "Importing files...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = EEDataImport(result.FileNames, cts.Token);
        }

        private async Task EEDataImport(string[] fileNames, CancellationToken cancellationToken = default)
        {
            var errors = new Dictionary<string, List<Exception>>();

            try
            {
                foreach (var fileName in fileNames)
                {
                    errors.TryAdd(fileName, new List<Exception>());

                    try
                    {
                        OnMessageSent($"Saving employee EE data information from {Path.GetFileName(fileName)}...");
                        var extractedEmployee = await m_Employees.ImportEEData(fileName, cancellationToken);

                        foreach (var employee in extractedEmployee)
                        {
                            try
                            {
                                await m_Employees.Save(employee, cancellationToken);
                            }
                            catch (TaskCanceledException) { throw; }
                            catch (Exception ex) { errors[fileName].Add(ex); }
                        }
                    }
                    catch (TaskCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        errors[fileName].Add(ex);
                    }
                }

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message, "Unexpected error");
            }
        }
        #endregion

        #region master file import
        private void MasterFileImport()
        {
            s_FileDialog.ShowFileDialog(MasterFileImportCallback);
        }

        private void MasterFileImportCallback(IFileDialogResult result)
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Loading", "Importing files...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = MasterFileImport(result.FileName, cts.Token);
        }

        private async Task MasterFileImport(string fileName, CancellationToken cancellationToken = default)
        {
            var errors = new Dictionary<string, List<Exception>>();

            try
            {
                errors.TryAdd(fileName, new List<Exception>());

                OnMessageSent($"Saving extracted employee job codes from {Path.GetFileName(fileName)}...");
                var extractedEmployee = await m_Employees.ImportMasterFile(fileName, cancellationToken);

                foreach (var employee in extractedEmployee)
                {
                    try
                    {
                        await m_Employees.Save(employee, cancellationToken);
                    }
                    catch (TaskCanceledException) { throw; }
                    catch (Exception ex) { errors[fileName].Add(ex); }
                }

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message, "Unexpected error");
            }
        }
        #endregion

        #region masterlist export
        private void MasterlistExport()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = MasterlistExport(cts.Token);
        }

        private async Task MasterlistExport(CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCode = Main?.PayrollCode;
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);
                var source = CollectionViewSource.GetDefaultView(Employees);
                var employees = source.OfType<Employee>().ToList();

                OnMessageSent("Exporting masterlist...");
                var path = await m_Employees.ExportMasterlist(employees, payrollCode, cancellationToken: cancellationToken);
                OnTaskCompleted();

                s_Message.ShowDialog("Masterfile exported.", "Success", $"File saved to {path}");
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Unexpected error", ex.ToString());
            }
        }
        #endregion

        #region unknown tin export
        private void UnknownTinExport()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Loading", "Exporting unknown TIN...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = UnknownTinExport(cts.Token);
        }

        private async Task UnknownTinExport(CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCode = Main?.PayrollCode;
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);
                var source = CollectionViewSource.GetDefaultView(Employees);
                var employees = source.OfType<Employee>().ToList();
                var noTin = employees.Where(t => string.IsNullOrEmpty(t.TIN));

                OnMessageSent("Exporting unknown TIN...");
                var path = await m_Employees.ExportMasterlist(noTin, payrollCode, cancellationToken: cancellationToken);
                OnTaskCompleted();

                s_Message.ShowDialog("Unknown TIN exported.", "Success", $"File saved to {path}");
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Unexpected error", ex.ToString());
            }
        }
        #endregion

        #region check detail
        private void CheckDetail(object? parameter)
        {
            if (parameter is not Employee employee)
            {
                employee = new();
            }

            var dialogParams = new DialogParameters
            {
                { PmsConstants.Employee, employee }
            };

            s_Dialog.Show(ViewNames.EmployeeDetailView, dialogParams, (_) => { });
        }
        #endregion

        #region open payroll code view
        private void OpenPayrollCodeView()
        {
            var dialogParams = new DialogParameters
            {
            };

            s_Dialog.Show(ViewNames.PayrollCodeDetailView, dialogParams, OpenPayrollCodeViewCallback);
        }

        private void OpenPayrollCodeViewCallback(IDialogResult result)
        {
        }
        #endregion

        private bool FilterEmployee(object? t)
        {
            if (t is Employee employee)
            {
                var isEmpty = string.IsNullOrEmpty(SearchInput);
                var hideArchived = employee.Active || !HideArchived;
                var idMatch = employee.EEId.Contains(SearchInput);
                var nameMatch = employee.Fullname.Contains(SearchInput);
                var cardMatch = employee.CardNumber.Contains(SearchInput);
                var accountMatch = employee.AccountNumber.Contains(SearchInput);

                return (isEmpty || idMatch || nameMatch || cardMatch || accountMatch) && hideArchived;
            }

            return false;
        }

        private void Employees_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ICollectionView source)
            {
                var employees = source.OfType<Employee>();
                ActiveEECount = employees.Count(t => t.Active);
                NonActiveEECount = employees.Count(t => !t.Active);
            }
        }
    }
}
