using Pms.Common;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using Prism.Commands;
using Pms.Payrolls.ServiceLayer.Files.Import.Alphalist;
using Pms.Masterlists.Entities;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;

namespace Pms.Payrolls.Module.ViewModels
{
    public class AlphalistViewModel : CancellableBase, INavigationAware
    {
        private readonly IDialogService s_Dialog;
        private readonly IFileDialogService s_FileDialog;
        private readonly IMessageBoxService s_Message;

        public AlphalistViewModel(IDialogService dialog, IFileDialogService fileDialog, IMessageBoxService message)
        {
            s_Dialog = dialog;
            s_FileDialog = fileDialog;
            s_Message = message;

            ImportToBirCommand = new DelegateCommand(ImportToBir);
        }

        public IPayrollsMain? Main { get; set; }
        public string BirDbfDirectory { get; set; } = string.Empty;

        #region commands
        public DelegateCommand ImportToBirCommand { get; }
        #endregion

        #region import alphalist
        private void ImportToBir()
        {
            StartImportAlphalist();
        }

        private void StartImportAlphalist()
        {
            s_FileDialog.ShowMultiFileDialog(ImportAlphalistCallback);
        }

        private void ImportAlphalistCallback(IFileDialogResult result)
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = ImportAlphalist(result.FileNames, cts.Token);
        }

        private async Task ImportAlphalist(string[] fileNames, CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(Main?.CutoffId);
                var payrollCodeId = Main?.PayrollCode?.PayrollCodeId;
                var company = Main?.Company;
                if (company == null) throw new Exception(ErrorMessages.CompanyIsEmpty);
                if (string.IsNullOrEmpty(payrollCodeId)) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);

                var importTasks = new List<Task>();

                foreach (string payRegister in fileNames)
                {
                    var companyView = new CompanyView(company.RegisteredName, company.TIN, company.BranchCode, company.Region);
                    var importer = new AlphalistImport();

                    var importTask = Task.Run(() =>
                    {
                        importer.ImportToBIRProgram(payRegister, BirDbfDirectory, companyView, cutoff.YearCovered);
                    }, cancellationToken);
                    importTasks.Add(importTask);
                }

                OnMessageSent("Importing...");
                await Task.WhenAll(importTasks);
                OnTaskCompleted();

                s_Message.ShowDialog("Import done.", "Import alphalist");
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Import alphalist", ex.ToString());
            }
        }
        #endregion

        #region INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Main = navigationContext.Parameters.GetValue<IPayrollsMain?>(PmsConstants.Main);
        }
        #endregion
    }
}
