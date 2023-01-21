using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Wordprocessing;
using Org.BouncyCastle.Crypto;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Payrolls.Exceptions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Pms.Payrolls.Module.ViewModels
{
    public class PayrollViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        #region properties
        private int _cbcCount;
        private double _cbcTotal;
        private int _chkCount;
        private double _chkTotal;
        private Company _company = new();
        private Cutoff _cutoff = new();
        private int _grandCount;
        private double _grandTotal;
        private int _lbpCount;
        private double _lbpTotal;
        private int _mpaloCount;
        private double _mpaloTotal;
        private int _mtacCount;
        private double _mtacTotal;
        private PayrollCode _payrollCode = new();
        private IEnumerable<Payroll> _payrolls = Enumerable.Empty<Payroll>();
        private int _unknownEECount;
        private double _unknownEETotal;

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
        public IEnumerable<Payroll> Payrolls { get => _payrolls; set => SetProperty(ref _payrolls, value); }
        public int UnknownEECount { get => _unknownEECount; set => SetProperty(ref _unknownEECount, value); }
        public double UnknownEETotal { get => _unknownEETotal; set => SetProperty(ref _unknownEETotal, value); }
        #endregion

        private IMain? _main;
        private readonly Payrolls _payroll;
        private readonly IFileDialogService _fileDialog;

        public PayrollViewModel(IFileDialogService fileDialog, Payrolls payroll)
        {
            _fileDialog = fileDialog;
            _payroll = payroll;

            Export13thMonthCommand = new DelegateCommand(Export13thMonth);
            ExportAlphalistCommand = new DelegateCommand(ExportAlphalist);
            ExportBankReportCommand = new DelegateCommand(ExportBankReport);
            ImportCommand = new DelegateCommand(Import);
            ExportMacroCommand = new DelegateCommand(ExportMacro);
        }

        #region Commands
        public DelegateCommand Export13thMonthCommand { get; }
        public DelegateCommand ExportAlphalistCommand { get; }
        public DelegateCommand ExportBankReportCommand { get; }
        public DelegateCommand ExportMacroCommand { get; }
        public DelegateCommand ImportCommand { get; }

        private void Export13thMonth()
        {
            _ = Export13thMonth(default);
        }

        private void ExportAlphalist()
        {
            _ = ExportAlphalist(default);
        }

        private void ExportBankReport()
        {
            _ = ExportBankReport(default);
        }

        private void Import()
        {
            StartImportPayroll();
        }

        private void ExportMacro()
        {
            _ = ExportMacro(default);
        }
        #endregion

        private async Task Export13thMonth(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(_cutoff.CutoffId);
                var company = _company;
                var employeePayrolls = await _payroll.GetYearlyPayrollsByEmployee(cutoff.YearCovered, _payrollCode.PayrollCodeId,
                    _company.CompanyId, cancellationToken);
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
                            if (p.RegHours > 96)
                                return p.RegularPay / p.RegHours * 96;
                            return p.RegularPay;
                        }) / 12,
                    };

                    payrolls.Add(v_13thMonthPayroll);
                }

                _payroll.ExportBankReport(payrolls, $"{cutoff.CutoffDate:yy}{12}-13", _payrollCode.PayrollCodeId);
            }
            catch
            {
                throw;
            }
        }

        private async Task ExportAlphalist(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(_cutoff.CutoffId);
                var employeePayrolls = await _payroll.GetYearlyPayrollsByEmployee(cutoff.YearCovered, _company.CompanyId, cancellationToken);
                var payrollsGroup = employeePayrolls.GroupBy(t => t.EEId);
                var alphalists = new List<AlphalistDetail>();

                foreach (var employeePayroll in payrollsGroup)
                {
                    var alphaDetailFactory = new AutomatedAlphalistDetail(employeePayroll, _company.MinimumRate, cutoff.YearCovered);
                    var alphalistDetail = alphaDetailFactory.CreateAlphalistDetail();

                    if (!string.IsNullOrEmpty(alphalistDetail.EEId))
                    {
                        alphalists.Add(alphalistDetail);
                    }
                }

                _payroll.ExportAlphalist(alphalists, cutoff.YearCovered, _company);
                _payroll.ExportAlphalistVerifier(payrollsGroup, cutoff.YearCovered, _company);
            }
            catch
            {
                throw;
            }
        }

        private async Task ExportBankReport(CancellationToken cancellationToken = default)
        {
            try
            {
                string cutoffId = _cutoff.CutoffId;
                string payrollCode = _payrollCode.PayrollCodeId;
                var payrolls = await _payroll.Get(cutoffId, payrollCode, cancellationToken);
                _payroll.ExportBankReport(payrolls, cutoffId, payrollCode);
            }
            catch
            {
                throw;
            }
        }

        private async Task ExportMacro(CancellationToken cancellationToken = default)
        {
            try
            {
                var cutoff = new Cutoff(_cutoff.CutoffId);
                var company = _company;
                var payrolls = await _payroll.GetMonthlyPayrolls(cutoff.CutoffDate.Month, company.CompanyId, cancellationToken);
                _payroll.ExportMacro(payrolls, cutoff, company.CompanyId);
                _payroll.ExportMacroB(payrolls, cutoff, company.CompanyId);
            }
            catch
            {
                throw;
            }
        }

        private void StartImportPayroll()
        {
            _fileDialog.ShowMultiFileDialog(ImportPayrollCallback);
        }

        private void ImportPayrollCallback(IFileDialogResult result)
        {
            _ = ImportPayroll(result.FileNames);
        }

        private async Task ImportPayroll(string[] fileNames, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var payRegister in fileNames)
                {
                    var extractedPayrolls = await _payroll.Import(payRegister, (ImportProcessChoices)_payrollCode.Process, cancellationToken);

                    foreach (var payroll in extractedPayrolls)
                    {
                        await _payroll.Save(payroll, _payrollCode.PayrollCodeId, _company.CompanyId, cancellationToken);
                    }
                }

                //var noEEPayrolls = _payroll.ListNoEEPayrolls();
                await Listing(cancellationToken);
            }
            catch (PayrollRegisterHeaderNotFoundException)
            {
                throw;
            }
            catch
            {
                throw;
            }
        }

        private async Task Listing(CancellationToken cancellationToken = default)
        {
            try
            {
                var payrolls = (await _payroll.Get(_cutoff.CutoffId, cancellationToken))
                    .SetCompanyId(_company.CompanyId)
                    .SetPayrollCode(_payrollCode.PayrollCodeId);

                Payrolls = payrolls;
                ChkCount = payrolls.Count(t => t.EE.Bank == BankChoices.CHK);
                LbpCount = payrolls.Count(t => t.EE.Bank == BankChoices.LBP);
                CbcCount = payrolls.Count(t => t.EE.Bank == BankChoices.CBC);
                MtacCount = payrolls.Count(p => p.EE.Bank == BankChoices.MTAC);
                MpaloCount = payrolls.Count(p => p.EE.Bank == BankChoices.MPALO);

                ChkTotal = payrolls.Where(p => p.EE.Bank == BankChoices.CHK).Sum(p => p.NetPay);
                LbpTotal = payrolls.Where(p => p.EE.Bank == BankChoices.LBP).Sum(p => p.NetPay);
                CbcTotal = payrolls.Where(p => p.EE.Bank == BankChoices.CBC).Sum(p => p.NetPay);
                MtacTotal = payrolls.Where(p => p.EE.Bank == BankChoices.MTAC).Sum(p => p.NetPay);
                MpaloTotal = payrolls.Where(p => p.EE.Bank == BankChoices.MPALO).Sum(p => p.NetPay);

                UnknownEECount = payrolls.Count(p => p.EE == null || p.EE.FirstName == string.Empty);
                UnknownEETotal = payrolls.Where(p => p.EE == null || p.EE.FirstName == string.Empty).Sum(p => p.NetPay);

                GrandCount = payrolls.Count();
                GrandTotal = payrolls.Sum(p => p.NetPay);
            }
            catch
            {
                throw;
            }
        }


        #region IRegionMemberLifetime
        public bool KeepAlive => true;
        #endregion

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
            _main = navigationContext.Parameters.GetValue<IMain?>(PmsConstants.Main);
            if (_main != null)
            {
                _main.PropertyChanged += Main_PropertyChanged;
            }

            _ = LoadValues();
        }
        #endregion

        private async Task LoadValues(CancellationToken cancellationToken = default)
        {
            if (_main != null)
            {
                _cutoff = !string.IsNullOrEmpty(_main.CutoffId) ? new Cutoff(_main.CutoffId) : new Cutoff();
                _payrollCode = _main.PayrollCode ?? new PayrollCode();
                _company = _main.Company ?? new();

                await Listing(cancellationToken);
            }
        }

        private void Main_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _ = LoadValues();
        }
    }
}
