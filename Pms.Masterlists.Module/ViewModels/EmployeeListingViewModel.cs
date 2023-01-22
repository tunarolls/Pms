using DryIoc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula.PTG;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Masterlists.Entities;
using Pms.Masterlists.Exceptions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Pms.Masterlists.Module.ViewModels
{
    public class DummyEmployeeListingViewMode : EmployeeListingViewModel
    {
        public DummyEmployeeListingViewMode(Employees employees, PayrollCodes payrollCodes, Companies companies, IDialogService dialog, IFileDialogService fileDialog)
            : base(employees, payrollCodes, companies, dialog, fileDialog)
        {
            Employees = new Employee[] { new Employee() };
        }
    }

    public class EmployeeListingViewModel : CancellableBase, INavigationAware, IRegionMemberLifetime
    {
        private readonly Employees m_Employees;
        private readonly PayrollCodes m_PayrollCodes;
        private readonly Companies m_Companies;
        private readonly IDialogService s_Dialog;
        private readonly IFileDialogService s_FileDialog;
        private IMain? _main;

        public EmployeeListingViewModel(Employees employees,
            PayrollCodes payrollCodes,
            Companies companies,
            IDialogService dialog,
            IFileDialogService fileDialog)
        {
            m_Employees = employees;
            m_PayrollCodes = payrollCodes;
            m_Companies = companies;
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

            //OpenPayrollCodeView = new Commands.Payroll_Codes.OpenView(this, payrollCodes, companies);
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
                var selectedDate = result.Parameters.GetValue<DateTime>(PmsConstants.SelectedDate);
                _ = SyncNewlyHired(selectedDate);
            }
        }

        private async Task SyncNewlyHired(DateTime selectedDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var exceptions = new List<Exception>();
                var employees = await m_Employees.SyncNewlyHired(selectedDate, _site.ToString(), cancellationToken);
                if (!employees.Any()) return;

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
                    catch (InvalidFieldValuesException ex) { exceptions.Add(ex); }
                    catch (InvalidFieldValueException ex) { exceptions.Add(ex); }
                    catch (DuplicateBankInformationException ex) { exceptions.Add(ex); }
                    catch (Exception ex) { exceptions.Add(ex); }
                }

                m_Employees.ReportExceptions(exceptions, new PayrollCode(), $"{_site}-NEWLYHIRED");
            }
            catch (HttpRequestException) { }
            catch (Exception) { }

            // Load employees
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
                _ = SyncResigned(selectedDate);
            }
        }

        private async Task SyncResigned(DateTime selectedDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var exceptions = new List<Exception>();
                var employees = await m_Employees.SyncResigned(selectedDate, _site.ToString(), cancellationToken);
                if (!employees.Any()) return;

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
                    catch (InvalidFieldValuesException ex) { exceptions.Add(ex); }
                    catch (InvalidFieldValueException ex) { exceptions.Add(ex); }
                    catch (DuplicateBankInformationException ex) { exceptions.Add(ex); }
                    catch (Exception ex) { exceptions.Add(ex); }
                }

                m_Employees.ReportExceptions(exceptions, new PayrollCode(), $"{_site}-RESIGNED");
            }
            catch (HttpRequestException)
            {
            }
            catch (Exception)
            {
            }

            // Load employees
        }
        #endregion

        #region sync all
        private void SyncAll()
        {
            _ = SyncAll(Employees.Select(t => t.EEId).ToArray());
        }

        private async Task SyncAll(string[] eeIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var exceptions = new List<Exception>();

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
                    catch (InvalidFieldValuesException ex) { exceptions.Add(ex); }
                    catch (InvalidFieldValueException ex) { exceptions.Add(ex); }
                    catch (DuplicateBankInformationException ex) { exceptions.Add(ex); }
                    catch (Exception ex) { exceptions.Add(ex); }
                }

                m_Employees.ReportExceptions(exceptions, _payrollCode, $"{_site}-REGULAR");
            }
            catch (HttpRequestException) { }
            catch (Exception) { }

            // Sync newly hired
        }
        #endregion

        #region cancel test
        public void OpenCancelDialog()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Hi", "Hi", this);
            var operation = RunOperation(1000, cts.Token);
            s_Dialog.ShowDialog(DialogNames.CancelDialog, dialogParameters, (_) => { });
        }

        private async Task RunOperation(int lengthMilliseconds = 1000, CancellationToken cancellationToken = default)
        {
            try
            {
                OnMessageSent(new("Hello"));
                await Task.Delay(lengthMilliseconds, cancellationToken);
                await Task.Delay(lengthMilliseconds, cancellationToken);
                await Task.Delay(lengthMilliseconds, cancellationToken);
                await Task.Delay(lengthMilliseconds, cancellationToken);
                await Task.Delay(lengthMilliseconds, cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                OnTaskCompleted();
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

            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Initialization", "Loading...", this);
            _ = LoadValues(cts.Token);
            s_Dialog.ShowDialog(DialogNames.CancelDialog, dialogParameters, (_) => { });
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
            _ = LoadValues();
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
            }
            catch
            {
                throw;
            }
            finally
            {
                OnTaskCompleted();
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
            _ = BankImport(result.FileNames);
        }

        private async Task BankImport(string[] fileNames, CancellationToken cancellationToken = default)
        {
            foreach (var fileName in fileNames)
            {
                try
                {
                    var extractedEmployee = await m_Employees.ImportBankInformation(fileName, cancellationToken);

                    foreach (var employee in extractedEmployee)
                    {
                        try
                        {
                            await m_Employees.Save(employee, cancellationToken);
                        }
                        catch (InvalidFieldValueException)
                        {
                            throw;
                        }
                        catch (DuplicateBankInformationException)
                        {
                            throw;
                        }
                    }
                }
                catch
                {
                    throw;
                }
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
            _ = EEDataImport(result.FileNames);
        }

        private async Task EEDataImport(string[] fileNames, CancellationToken cancellationToken = default)
        {
            foreach (var fileName in fileNames)
            {
                try
                {
                    var extractedEmployee = await m_Employees.ImportEEData(fileName, cancellationToken);

                    foreach (var employee in extractedEmployee)
                    {
                        try
                        {
                            await m_Employees.Save(employee, cancellationToken);
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
                catch
                {
                    throw;
                }
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
            _ = MasterFileImport(result.FileName);
        }

        private async Task MasterFileImport(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var extractedEmployee = await m_Employees.ImportMasterFile(fileName, cancellationToken);

                foreach (var employee in extractedEmployee)
                {
                    try
                    {
                        await m_Employees.Save(employee, cancellationToken);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region masterlist export
        private void MasterlistExport()
        {
            _ = MasterlistExport(default);
        }

        private async Task MasterlistExport(CancellationToken cancellationToken = default)
        {
            try
            {
                var employees = Employees.ToList();
                await m_Employees.ExportMasterlist(employees, _payrollCode, cancellationToken: cancellationToken);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region unknown tin export
        private void UnknownTinExport()
        {
            _ = UnknownTinExport(default);
        }

        private async Task UnknownTinExport(CancellationToken cancellationToken = default)
        {
            try
            {
                var noTin = Employees.Where(t => string.IsNullOrEmpty(t.TIN)).ToList();
                await m_Employees.ExportMasterlist(noTin, _payrollCode, cancellationToken: cancellationToken);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region check detail
        private void CheckDetail(object? parameter)
        {
            try
            {
                if (parameter is not Employee employee)
                {
                    employee = new();
                }

                var dialogParams = new DialogParameters
                {
                    { PmsConstants.Employee, employee }
                };

                s_Dialog.ShowDialog(ViewNames.EmployeeDetailView, dialogParams, (_) => { });
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
