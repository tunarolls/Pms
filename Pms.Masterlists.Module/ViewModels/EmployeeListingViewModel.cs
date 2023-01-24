using FastExpressionCompiler.LightExpression;
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module.ViewModels
{
    public class DummyEmployeeListingViewMode : EmployeeListingViewModel
    {
        public DummyEmployeeListingViewMode(Employees employees, IMessageBoxService message, IDialogService dialog, IFileDialogService fileDialog)
            : base(employees, message, dialog, fileDialog)
        {
            Employees = new Employee[] { new Employee() };
        }
    }

    public class EmployeeListingViewModel : CancellableBase, INavigationAware, IRegionMemberLifetime
    {
        private readonly Employees m_Employees;
        private readonly IDialogService s_Dialog;
        private readonly IFileDialogService s_FileDialog;
        private readonly IMessageBoxService s_Message;
        private IMain? _main;

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
        }

        #region properties
        private IEnumerable<Employee> _employees = Enumerable.Empty<Employee>();
        private int _activeEECount;
        private string _companyId = string.Empty;
        private bool _hideArchived;
        private int _nonActiveEECount;
        private PayrollCode _payrollCode = new();
        private string _payrollCodeId = string.Empty;
        private string _searchInput = string.Empty;
        private SiteChoices _site = SiteChoices.MANILA;

        public int ActiveEECount { get => _activeEECount; set => SetProperty(ref _activeEECount, value); }

        public string CompanyId { get => _companyId; set => SetProperty(ref _companyId, value); }

        public IEnumerable<Employee> Employees { get => _employees; set => SetProperty(ref _employees, value); }

        public bool HideArchived { get => _hideArchived; set => SetProperty(ref _hideArchived, value); }

        public int NonActiveEECount { get => _nonActiveEECount; set => SetProperty(ref _nonActiveEECount, value); }

        public string SearchInput { get => _searchInput; set => SetProperty(ref _searchInput, value); }
        public SiteChoices Site { get => _site; set => SetProperty(ref _site, value); }
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

        public bool KeepAlive => true;
        #endregion

        public void SelectDate(Action<IDialogResult> selectDateCallback)
        {
            s_Dialog.Show(ViewNames.SelectDateView, selectDateCallback);
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
                var selectedDate = result.Parameters.GetValue<DateTime>(PmsConstants.SelectedDate);
                var cts = GetCancellationTokenSource();
                var dialogParameters = CreateDialogParameters("Loading", "Syncing data...", this, cts);
                s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
                _ = SyncNewlyHired(selectedDate, cts.Token);
            }
        }

        private async Task SyncNewlyHired(DateTime selectedDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var exceptions = new List<Exception>();
                var employees = await m_Employees.SyncNewlyHired(selectedDate, _site.ToString(), cancellationToken);
                if (!employees.Any()) return;
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
                    catch (InvalidFieldValuesException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (InvalidFieldValueException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (DuplicateBankInformationException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (Exception ex) { exceptions.Add(ex); OnErrorFound(); }
                    finally { OnProgressIncrement(); }
                }

                m_Employees.ReportExceptions(exceptions, new PayrollCode(), $"{_site}-NEWLYHIRED");
                OnTaskCompleted();
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (HttpRequestException)
            {
                OnTaskException();
                s_Message.ShowError(ErrorMessages.HrmsHttpRequestError);
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
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
                var selectedDate = result.Parameters.GetValue<DateTime>(PmsConstants.SelectedDate);
                var cts = GetCancellationTokenSource();
                var dialogParameters = CreateDialogParameters("Loading", "Syncing data...", this, cts);
                s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
                _ = SyncResigned(selectedDate, cts.Token);
            }
        }

        private async Task SyncResigned(DateTime selectedDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var exceptions = new List<Exception>();
                var employees = await m_Employees.SyncResigned(selectedDate, _site.ToString(), cancellationToken);
                if (!employees.Any()) return;
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
                    catch (InvalidFieldValuesException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (InvalidFieldValueException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (DuplicateBankInformationException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (Exception ex) { exceptions.Add(ex); OnErrorFound(); }
                    finally { OnProgressIncrement(); }
                }

                m_Employees.ReportExceptions(exceptions, new PayrollCode(), $"{_site}-RESIGNED");
                OnTaskCompleted();
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (HttpRequestException)
            {
                OnTaskException();
                s_Message.ShowError(ErrorMessages.HrmsHttpRequestError);
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
            }
        }
        #endregion

        #region sync all
        private void SyncAll()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Loading", "Syncing data...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = SyncAll(Employees.Select(t => t.EEId).ToArray(), cts.Token);
        }

        private async Task SyncAll(string[] eeIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var exceptions = new List<Exception>();
                OnProgressStart(eeIds.Length);

                foreach (var eeId in eeIds)
                {
                    try
                    {
                        var foundEmployee = await m_Employees.SyncOne(eeId, _site.ToString(), cancellationToken);
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
                    catch (InvalidFieldValuesException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (InvalidFieldValueException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (DuplicateBankInformationException ex) { exceptions.Add(ex); OnErrorFound(); }
                    catch (Exception ex) { exceptions.Add(ex); OnErrorFound(); }
                    finally { OnProgressIncrement(); }
                }

                m_Employees.ReportExceptions(exceptions, _payrollCode, $"{_site}-REGULAR");
                OnTaskCompleted();
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (HttpRequestException)
            {
                OnTaskException();
                s_Message.ShowError(ErrorMessages.HrmsHttpRequestError);
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
            }
        }
        #endregion

        #region cancel test
        public void OpenCancelDialog()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Running", "Testing CancelDialog...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            var operation = RunOperation(1000, cts.Token);
        }

        private async Task RunOperation(int lengthMilliseconds = 1000, CancellationToken cancellationToken = default)
        {
            try
            {
                OnMessageSent("Testing CancelDialog...");
                await Task.Delay(lengthMilliseconds, cancellationToken);

                OnMessageSent("Indeterminate operation");
                OnProgressStart();
                await Task.Delay(5000, cancellationToken);

                OnMessageSent("Determinate operation");
                int count = 10;
                OnProgressStart(count);
                for (int i = 0; i < count; i++)
                {
                    await Task.Delay(lengthMilliseconds, cancellationToken);
                    OnProgressIncrement();
                }

                OnErrorFound();

                OnMessageSent("New operation");
                count = 20;
                OnProgressStart(count);
                for (int i = 0; i < count; i++)
                {
                    await Task.Delay(lengthMilliseconds, cancellationToken);
                    OnProgressIncrement();
                }

                OnTaskCompleted();
            }
            catch
            {
                OnTaskException();
            }
        }
        #endregion


        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _main = navigationContext.Parameters.GetValue<IMain?>(PmsConstants.Main);
            if (_main != null)
            {
                _main.PropertyChanged += Main_PropertyChanged;
            }

            LoadValues();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (_main != null)
            {
                _main.PropertyChanged -= Main_PropertyChanged;
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
            var dialogParameters = CreateDialogParameters("Loading", "Retrieving data...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = LoadValues(cts.Token);
        }

        private async Task LoadValues(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_main != null)
                {
                    _payrollCode = _main.PayrollCode ?? new PayrollCode();
                    _payrollCodeId = _payrollCode.PayrollCodeId;
                    _site = _main.Site ?? SiteChoices.UNKNOWN;
                    _companyId = _main.Company?.CompanyId ?? new Company().CompanyId;

                    await LoadEmployees(cancellationToken);
                }

                OnTaskCompleted();
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
            }
        }

        private async Task LoadEmployees(CancellationToken cancellationToken = default)
        {
            try
            {
                var employees = (await m_Employees.GetEmployees(cancellationToken))
                    .HideArchived(HideArchived)
                    .FilterSearchInput(SearchInput)
                    .FilterPayrollCode(_payrollCodeId);

                Employees = employees;
                ActiveEECount = employees.Count(t => t.Active);
                NonActiveEECount = employees.Count(t => !t.Active);
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
            var dialogParameters = CreateDialogParameters("Loading", "Importing files...", this, cts);
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
            var dialogParameters = CreateDialogParameters("Loading", "Exporting masterlist...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = MasterlistExport(cts.Token);
        }

        private async Task MasterlistExport(CancellationToken cancellationToken = default)
        {
            try
            {
                var employees = Employees.ToList();
                await m_Employees.ExportMasterlist(employees, _payrollCode, cancellationToken: cancellationToken);
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
                var noTin = Employees.Where(t => string.IsNullOrEmpty(t.TIN)).ToList();
                await m_Employees.ExportMasterlist(noTin, _payrollCode, cancellationToken: cancellationToken);
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

            s_Dialog.Show(ViewNames.PayrollCodeDetailView, dialogParams, (_) => { });
        }
        #endregion
    }
}
