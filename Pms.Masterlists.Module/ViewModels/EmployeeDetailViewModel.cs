using NPOI.OpenXmlFormats.Dml.Diagram;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Masterlists.Entities;
using Pms.Masterlists.ServiceLayer.Hrms.Exceptions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module.ViewModels
{
    public class EmployeeDetailViewModel : CancellableBase, IDialogAware
    {
        private readonly Employees m_Employees;
        private readonly PayrollCodes m_PayrollCodes;
        private readonly IDialogService s_Dialog;
        private readonly IMessageBoxService s_Message;
        private Employee? l_Employee;
        private IEnumerable<string>? l_PayrollCodes;

        public EmployeeDetailViewModel(IDialogService dialog, IMessageBoxService message, Employees employees, PayrollCodes payrollCodes)
        {
            m_Employees = employees;
            m_PayrollCodes = payrollCodes;
            s_Dialog = dialog;
            s_Message = message;

            Sites = Enum.GetValues<SiteChoices>().Select(t => t.ToString());
            BankTypes = Enum.GetValues<BankChoices>();

            SaveCommand = new DelegateCommand(Save);
            SyncCommand = new DelegateCommand(Sync);
        }

        public Employee? Employee { get => l_Employee; set => SetProperty(ref l_Employee, value); }
        public IEnumerable<string>? PayrollCodes { get => l_PayrollCodes; set => SetProperty(ref l_PayrollCodes, value); }
        public IEnumerable<string> Sites { get; }
        public IEnumerable<BankChoices> BankTypes { get; }

        public DelegateCommand SyncCommand { get; }
        public DelegateCommand SaveCommand { get; }

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
            LoadPayrollCodes();
            Employee = parameters.GetValue<Employee?>(PmsConstants.Timesheet);
        }
        #endregion

        #region init
        private void LoadPayrollCodes()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Loading", "Retrieving payroll codes...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = LoadPayrollCodes(cts.Token);
        }

        private async Task LoadPayrollCodes(CancellationToken cancellationToken = default)
        {
            try
            {
                PayrollCodes = (await m_PayrollCodes.ListPayrollCodes(cancellationToken)).Select(t => t.PayrollCodeId);
                OnTaskCompleted();
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
                RequestClose?.Invoke(new DialogResult());
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.ToString());
                RequestClose?.Invoke(new DialogResult(ButtonResult.None));
            }
        }
        #endregion

        private void Save()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Loading", "Saving data...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Save(cts.Token);
        }

        private async Task Save(CancellationToken cancellationToken = default)
        {
            try
            {
                if (l_Employee != null)
                {
                    await m_Employees.Save(l_Employee, cancellationToken);
                    OnTaskCompleted();
                    s_Message.Show("Save success");
                    RequestClose?.Invoke(new DialogResult());
                }
                else
                {
                    throw new NullReferenceException("Employee not found.");
                }
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (NullReferenceException nrex)
            {
                OnTaskException();
                s_Message.ShowError(nrex.ToString());
                RequestClose?.Invoke(new DialogResult());
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.ToString());
            }
        }

        private void Sync()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Loading", "Syncing data...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Sync(cts.Token);
        }

        private async Task Sync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (l_Employee != null)
                {
                    var employeeFoundOnServer = await m_Employees.SyncOne(l_Employee.EEId, l_Employee.Site, cancellationToken);
                    if (employeeFoundOnServer != null)
                    {
                        l_Employee.LastName = employeeFoundOnServer.LastName;
                        l_Employee.FirstName = employeeFoundOnServer.FirstName;
                        l_Employee.MiddleName = employeeFoundOnServer.MiddleName;
                        l_Employee.JobCode = employeeFoundOnServer.JobCode;
                        l_Employee.Location = employeeFoundOnServer.Location;

                        RaisePropertyChanged(nameof(Employee));

                        s_Message.Show("Sync success");
                    }
                    else
                    {
                        throw new EmployeeNotFoundException("Employee not found.");
                    }
                }
                else
                {
                    throw new NullReferenceException("Employee not found.");
                }

                OnTaskCompleted();
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (NullReferenceException nrex)
            {
                OnTaskException();
                s_Message.ShowError(nrex.ToString());
                RequestClose?.Invoke(new DialogResult());
            }
            catch (EmployeeNotFoundException enfex)
            {
                OnTaskException();
                s_Message.ShowError(enfex.ToString());
            }
            catch (HttpRequestException hrex)
            {
                OnTaskException();
                s_Message.ShowError(hrex.ToString());
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.ToString());
            }
        }
    }
}
