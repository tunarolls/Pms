using Pms.Common;
using Pms.Common.Enums;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pms.Masterlists.Module.ViewModels
{
    public class PayrollCodeDetailViewModel : CancellableBase, IDialogAware
    {
        private readonly Companies m_Companies;
        private readonly PayrollCodes m_PayrollCodes;
        private readonly IDialogService s_Dialog;
        private readonly IMessageBoxService s_Message;
        private IEnumerable<PayrollCode> l_PayrollCodes = Enumerable.Empty<PayrollCode>();
        private PayrollCode? l_SelectedPayrollCode;
        private IEnumerable<string> l_CompanyIds = Enumerable.Empty<string>();

        public PayrollCodeDetailViewModel(IDialogService dialog, IMessageBoxService message, Companies companies, PayrollCodes payrollCodes)
        {
            s_Dialog = dialog;
            s_Message = message;
            m_Companies = companies;
            m_PayrollCodes = payrollCodes;

            SaveCommand = new DelegateCommand(Save);

            Sites = Enum.GetValues<SiteChoices>().Select(t => t.ToString());
            ProcessTypes = Enum.GetValues<PayrollRegisterTypes>();
        }

        public IEnumerable<PayrollCode> PayrollCodes { get => l_PayrollCodes; set => SetProperty(ref l_PayrollCodes, value); }
        public IEnumerable<PayrollRegisterTypes> ProcessTypes { get; set; }
        public DelegateCommand SaveCommand { get; }
        public PayrollCode? SelectedPayrollCode { get => l_SelectedPayrollCode; set => SetProperty(ref l_SelectedPayrollCode, value); }
        public IEnumerable<string> Sites { get; set; }
        public IEnumerable<string> CompanyIds { get => l_CompanyIds; set => SetProperty(ref l_CompanyIds, value); }

        private void Listing()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters("Loading", "Retrieving data...", this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Listing(cts.Token);
        }

        private async Task Listing(CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCodes = await m_PayrollCodes.ListPayrollCodes(cancellationToken);
                var companies = await m_Companies.ListCompanies(cancellationToken);
                PayrollCodes = payrollCodes;
                CompanyIds = companies.Select(t => t.CompanyId);
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
                RequestClose?.Invoke(new DialogResult());
            }
        }

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
                if (SelectedPayrollCode != null)
                {
                    SelectedPayrollCode.PayrollCodeId = SelectedPayrollCode.GenerateId();
                    await m_PayrollCodes.Save(SelectedPayrollCode, cancellationToken);
                    OnTaskCompleted();
                    s_Message.Show("Save success");
                }
                else
                {
                    OnTaskCompleted();
                }
            }
            catch (TaskCanceledException)
            {
                OnTaskException();
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.ToString());
            }
        }

        #region IDialogAware
        public event Action<IDialogResult>? RequestClose;

        public string? Title { get; set; }
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Listing();
        }
        #endregion
    }
}
