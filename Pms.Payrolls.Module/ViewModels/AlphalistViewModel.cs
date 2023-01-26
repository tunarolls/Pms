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

namespace Pms.Payrolls.Module.ViewModels
{
    public class AlphalistViewModel : BindableBase, INavigationAware
    {
        private Company _company = new();
        private Cutoff _cutoff = new();
        private string _payrollCodeId = string.Empty;
        private IPayrollsMain? _main;
        private readonly IFileDialogService _fileDialog;
        public AlphalistViewModel(IFileDialogService fileDialog)
        {
            _fileDialog = fileDialog;
            ImportToBirCommand = new DelegateCommand(ImportToBir);
        }

        public string BirDbfDirectory { get; set; } = string.Empty;

        #region commands
        public DelegateCommand ImportToBirCommand { get; }
        #endregion

        private void ImportToBir()
        {
            StartImportAlphalist();
        }

        private void StartImportAlphalist()
        {
            _fileDialog.ShowMultiFileDialog(ImportAlphalistCallback);
        }

        private void ImportAlphalistCallback(IFileDialogResult result)
        {
            _ = ImportAlphalist(result.FileNames);
        }

        private async Task ImportAlphalist(string[] fileNames, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (string payRegister in fileNames)
                {
                    var cutoff = _cutoff;
                    var payrollCode = _payrollCodeId;
                    var company = _company;
                    var companyView = new CompanyView(company.RegisteredName, company.TIN, company.BranchCode, company.Region);
                    var importer = new AlphalistImport();

                    await Task.Run(() =>
                    {
                        importer.ImportToBIRProgram(payRegister, BirDbfDirectory, companyView, cutoff.YearCovered);
                    }, cancellationToken);
                }
            }
            catch
            {
                throw;
            }
        }

        #region INavigationAware
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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _main = navigationContext.Parameters.GetValue<IPayrollsMain?>(PmsConstants.Main);
            if (_main != null)
            {
                _main.PropertyChanged += Main_PropertyChanged;
            }

            LoadValues();
        }
        #endregion

        private void LoadValues()
        {
            if (_main != null)
            {
                _cutoff = !string.IsNullOrEmpty(_main.CutoffId) ? new Cutoff(_main.CutoffId) : new Cutoff();
                _payrollCodeId = _main.PayrollCode?.PayrollCodeId ?? string.Empty;
                _company = _main.Company ?? new();
            }
        }

        private void Main_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            LoadValues();
        }

    }
}
