using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MySqlX.XDevAPI.Common;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Wordprocessing;
using Org.BouncyCastle.Crypto;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Masterlists.Entities;
using Pms.Payrolls.Exceptions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Pms.Payrolls.Module.ViewModels
{
    public class PayrollViewModel : CancellableBase, INavigationAware
    {
        private int _cbcCount;
        private double _cbcTotal;
        private int _chkCount;
        private double _chkTotal;
        private int _grandCount;
        private double _grandTotal;
        private int _lbpCount;
        private double _lbpTotal;
        private int _mpaloCount;
        private double _mpaloTotal;
        private int _mtacCount;
        private double _mtacTotal;
        private IEnumerable<Payroll> _payrolls = Enumerable.Empty<Payroll>();
        private int _unknownEECount;
        private double _unknownEETotal;

        private readonly Payrolls m_Payrolls;
        private readonly IFileDialogService s_FileDialog;
        private readonly IDialogService s_Dialog;
        private readonly IMessageBoxService s_Message;

        public PayrollViewModel(IDialogService dialog, IFileDialogService fileDialog, IMessageBoxService message, Payrolls payrolls)
        {
            s_Dialog = dialog;
            s_FileDialog = fileDialog;
            s_Message = message;
            m_Payrolls = payrolls;

            Export13thMonthCommand = new DelegateCommand(Export13thMonth);
            ExportAlphalistCommand = new DelegateCommand(ExportAlphalist);
            ExportBankReportCommand = new DelegateCommand(ExportBankReport);
            ImportCommand = new DelegateCommand(Import);
            ExportMacroCommand = new DelegateCommand(ExportMacro);

            Payrolls = new RangedObservableCollection<Payroll>();
            var source = CollectionViewSource.GetDefaultView(Payrolls);
            source.Filter = t => FilterPayrolls(t);
            source.CollectionChanged += Payrolls_CollectionChanged;
        }

        public int CbcCount { get => _cbcCount; set => SetProperty(ref _cbcCount, value); }
        public double CbcTotal { get => _cbcTotal; set => SetProperty(ref _cbcTotal, value); }
        public int ChkCount { get => _chkCount; set => SetProperty(ref _chkCount, value); }
        public double ChkTotal { get => _chkTotal; set => SetProperty(ref _chkTotal, value); }
        public int GrandCount { get => _grandCount; set => SetProperty(ref _grandCount, value); }
        public double GrandTotal { get => _grandTotal; set => SetProperty(ref _grandTotal, value); }
        public int LbpCount { get => _lbpCount; set => SetProperty(ref _lbpCount, value); }
        public double LbpTotal { get => _lbpTotal; set => SetProperty(ref _lbpTotal, value); }
        public int MpaloCount { get => _mpaloCount; set => SetProperty(ref _mpaloCount, value); }
        public double MpaloTotal { get => _mpaloTotal; set => SetProperty(ref _mpaloTotal, value); }
        public int MtacCount { get => _mtacCount; set => SetProperty(ref _mtacCount, value); }
        public double MtacTotal { get => _mtacTotal; set => SetProperty(ref _mtacTotal, value); }
        public RangedObservableCollection<Payroll> Payrolls { get; set; }
        public int UnknownEECount { get => _unknownEECount; set => SetProperty(ref _unknownEECount, value); }
        public double UnknownEETotal { get => _unknownEETotal; set => SetProperty(ref _unknownEETotal, value); }

        public IPayrollsMain? Main { get; set; }

        #region Commands
        public DelegateCommand Export13thMonthCommand { get; }
        public DelegateCommand ExportAlphalistCommand { get; }
        public DelegateCommand ExportBankReportCommand { get; }
        public DelegateCommand ExportMacroCommand { get; }
        public DelegateCommand ImportCommand { get; }
        #endregion

        #region Export13thMonth
        private void Export13thMonth()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Export13thMonth(cts.Token);
        }

        private async Task Export13thMonth(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(Main?.CutoffId);
                var companyId = Main?.Company?.CompanyId;
                var payrollCode = Main?.PayrollCode?.PayrollCodeId;
                if (string.IsNullOrEmpty(payrollCode)) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);

                OnMessageSent("Retrieving payrolls...");
                var employeePayrolls = await m_Payrolls.GetYearlyPayrollsByEmployee(cutoff.YearCovered, payrollCode, companyId, cancellationToken);
                var payrollsGroup = employeePayrolls.GroupBy(t => t.EEId);
                var payrolls = new List<Payroll>();

                foreach (var employeePayroll in payrollsGroup)
                {
                    var payroll = employeePayroll.First();
                    var v_13thMonthPayroll = new Payroll()
                    {
                        EE = payroll.EE,
                        EEId = payroll.EEId,
                        CutoffId = $"{DateTime.Now:yy}{12}-13",
                        PayrollCode = payroll.PayrollCode,
                        YearCovered = payroll.YearCovered,
                        NetPay = employeePayroll.Sum(p =>
                        {
                            return p.RegHours > 96
                                ? p.RegularPay / p.RegHours * 96
                                : p.RegularPay;
                        }) / 12,
                    };

                    payrolls.Add(v_13thMonthPayroll);
                }

                // io bound, refactor later
                OnMessageSent("Exporting...");
                await Task.Run(() => m_Payrolls.ExportBankReport(payrolls, $"{cutoff.CutoffDate:yy}{12}-13", payrollCode), cancellationToken);
                OnTaskCompleted();

                s_Message.ShowDialog("Export done.", "");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export 13th month", ex.ToString());
            }
        }
        #endregion

        #region ExportAlphalist
        private void ExportAlphalist()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = ExportAlphalist(cts.Token);
        }

        private async Task ExportAlphalist(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(Main?.CutoffId);
                var company = Main?.Company;
                if (company == null) throw new Exception(ErrorMessages.CompanyIsEmpty);

                OnMessageSent("Retrieving payrolls...");
                var yearlyPayrolls = await m_Payrolls.GetYearlyPayrollsByEmployee(cutoff.YearCovered, company.CompanyId, cancellationToken);
                var payrollByEmployee = yearlyPayrolls.GroupBy(t => t.EEId);
                var alphalists = new List<AlphalistDetail>();

                foreach (var payrolls in payrollByEmployee)
                {
                    var alphaDetailFactory = new AutomatedAlphalistDetail(payrolls, company.MinimumRate, cutoff.YearCovered);
                    var alphalistDetail = alphaDetailFactory.CreateAlphalistDetail();

                    if (!string.IsNullOrEmpty(alphalistDetail.EEId))
                    {
                        alphalists.Add(alphalistDetail);
                    }
                }

                // io bound, refactor later
                var alphalistTask = Task.Run(() => m_Payrolls.ExportAlphalist(alphalists, cutoff.YearCovered, company), cancellationToken);
                var verifierTask = Task.Run(() => m_Payrolls.ExportAlphalistVerifier(payrollByEmployee, cutoff.YearCovered, company), cancellationToken);
                await Task.WhenAll(new[] {alphalistTask, verifierTask });
                OnTaskCompleted();

                s_Message.ShowDialog("Export done.", "");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export alphalist", ex.ToString());
            }
        }
        #endregion

        #region ExportBankReport
        private void ExportBankReport()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = ExportBankReport(cts.Token);
        }

        private async Task ExportBankReport(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(Main?.CutoffId);
                var payrollCode = Main?.PayrollCode?.PayrollCodeId;
                if (string.IsNullOrEmpty(payrollCode)) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);

                OnMessageSent("Retrieving payrolls...");
                var payrolls = await m_Payrolls.Get(cutoff.CutoffId, string.Empty, payrollCode, cancellationToken);

                // io bound, refactor later
                OnMessageSent("Exporting...");
                await Task.Run(() => m_Payrolls.ExportBankReport(payrolls, cutoff.CutoffId, payrollCode), cancellationToken);
                OnTaskCompleted();

                s_Message.ShowDialog("Export done.", "");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export bank report", ex.ToString());
            }
        }
        #endregion

        #region import payroll
        private void Import()
        {
            StartImportPayroll();
        }

        private void StartImportPayroll()
        {
            s_FileDialog.ShowMultiFileDialog(ImportPayrollCallback);
        }

        private void ImportPayrollCallback(IFileDialogResult result)
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = ImportPayroll(result.FileNames, cts.Token);
        }

        private async Task ImportPayroll(string[] fileNames, CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCode = Main?.PayrollCode;
                var company = Main?.Company;
                if (company == null) throw new Exception(ErrorMessages.CompanyIsEmpty);
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);

                foreach (var payRegister in fileNames)
                {
                    OnProgressStart();
                    OnMessageSent($"Importing payrolls from '{payRegister}'");
                    var extractedPayrolls = await m_Payrolls.Import(payRegister, (ImportProcessChoices)payrollCode.Process, cancellationToken);

                    OnProgressStart(extractedPayrolls.Count);
                    foreach (var payroll in extractedPayrolls)
                    {
                        await m_Payrolls.Save(payroll, payrollCode.PayrollCodeId, company.CompanyId, cancellationToken);
                        OnProgressIncrement();
                    }
                }

                OnTaskCompleted();

                s_Message.ShowDialog("Import done.", "");
                LoadValues();
            }
            catch (PayrollRegisterHeaderNotFoundException ex)
            {
                OnTaskException();
                s_Message.ShowDialog($"{ex.Header} not found in {ex.PayrollRegisterFilePath}.", "Import payroll", ex.ToString());
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Import payroll", ex.ToString());
            }
        }
        #endregion

        #region export macro
        private void ExportMacro()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = ExportMacro(cts.Token);
        }

        private async Task ExportMacro(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(Main?.CutoffId);
                var company = Main?.Company;
                if (company == null) throw new Exception(ErrorMessages.CompanyIsEmpty);

                OnMessageSent("Retrieving payrolls...");
                var payrolls = await m_Payrolls.GetMonthlyPayrolls(cutoff.CutoffDate.Month, company.CompanyId, cancellationToken);

                var exportMacroTask = Task.Run(() => m_Payrolls.ExportMacro(payrolls, cutoff, company.CompanyId), cancellationToken);
                var exportMacroBTask = Task.Run(() => m_Payrolls.ExportMacroB(payrolls, cutoff, company.CompanyId), cancellationToken);

                OnMessageSent("Exporting...");
                await Task.WhenAll(new[] { exportMacroTask, exportMacroBTask });
                OnTaskCompleted();

                s_Message.ShowDialog("Export done.", "");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export macro", ex.ToString());
            }
        }
        #endregion

        private void LoadValues()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Listing(cts.Token);
        }

        private async Task Listing(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(Main?.CutoffId);
                var companyId = Main?.Company?.CompanyId;
                var payrollCode = Main?.PayrollCode?.PayrollCodeId;

                OnMessageSent("Retrieving payrolls...");
                var payrolls = await m_Payrolls.Get(cutoff.CutoffId, companyId, payrollCode, cancellationToken);
                Payrolls.ReplaceRange(payrolls);
                OnTaskCompleted();
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Listing", ex.ToString());
            }
        }

        #region INavigationAware
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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Main = navigationContext.Parameters.GetValue<IPayrollsMain?>(PmsConstants.Main);

            if (Main != null)
            {
                Main.PropertyChanged += Main_PropertyChanged;
            }

            LoadValues();
        }
        #endregion

        private void Main_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            LoadValues();
        }

        private bool FilterPayrolls(object t)
        {
            return true;
        }

        private void Payrolls_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ICollectionView source)
            {
                var payrolls = source.OfType<Payroll>();

                ChkCount = payrolls.Count(t => t.EE?.Bank == BankChoices.CHK);
                LbpCount = payrolls.Count(t => t.EE?.Bank == BankChoices.LBP);
                CbcCount = payrolls.Count(t => t.EE?.Bank == BankChoices.CBC);
                MtacCount = payrolls.Count(p => p.EE?.Bank == BankChoices.MTAC);
                MpaloCount = payrolls.Count(p => p.EE?.Bank == BankChoices.MPALO);

                ChkTotal = payrolls.Where(p => p.EE?.Bank == BankChoices.CHK).Sum(p => p.NetPay);
                LbpTotal = payrolls.Where(p => p.EE?.Bank == BankChoices.LBP).Sum(p => p.NetPay);
                CbcTotal = payrolls.Where(p => p.EE?.Bank == BankChoices.CBC).Sum(p => p.NetPay);
                MtacTotal = payrolls.Where(p => p.EE?.Bank == BankChoices.MTAC).Sum(p => p.NetPay);
                MpaloTotal = payrolls.Where(p => p.EE?.Bank == BankChoices.MPALO).Sum(p => p.NetPay);

                UnknownEECount = payrolls.Count(p => string.IsNullOrEmpty(p.EE?.FirstName));
                UnknownEETotal = payrolls.Where(p => string.IsNullOrEmpty(p.EE?.FirstName)).Sum(p => p.NetPay);

                GrandCount = payrolls.Count();
                GrandTotal = payrolls.Sum(p => p.NetPay);
            }
        }
    }
}
