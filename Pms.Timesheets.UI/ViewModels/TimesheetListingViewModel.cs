using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xaml.Behaviors.Media;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Timesheets.ServiceLayer.EfCore;
using Pms.Timesheets.ServiceLayer.Files;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace Pms.Timesheets.Module.ViewModels
{
    public class DummyListingViewModel : TimesheetListingViewModel
    {
        public DummyListingViewModel(IDialogService dialog, Timesheets timesheets) : base(dialog, timesheets)
        {
            Timesheets.Add(new Timesheet()
            {
                TotalHours = 100.0,
                TotalOT = 100.0,
                TotalTardy = 100.0,
                TotalND = 100.0
            });
        }
    }

    public class TimesheetListingViewModel : BindableBase, INavigationAware, IActiveAware, IRegionMemberLifetime
    {
        #region Properties
        private int _confirmed;
        private int _confirmedWithoutAttendance;
        private int _notConfirmed;
        private int _notConfirmedWithAttendance;
        private DownloadOptions _options = DownloadOptions.All;
        private string _searchInput = string.Empty;
        private int _totalTimesheets;

        public int Confirmed { get => _confirmed; set => SetProperty(ref _confirmed, value); }
        public int ConfirmedWithoutAttendance { get => _confirmedWithoutAttendance; set => SetProperty(ref _confirmedWithoutAttendance, value); }
        public int NotConfirmed { get => _notConfirmed; set => SetProperty(ref _notConfirmed, value); }
        public int NotConfirmedWithAttendance { get => _notConfirmedWithAttendance; set => SetProperty(ref _notConfirmedWithAttendance, value); }
        public DownloadOptions Options { get => _options; set => SetProperty(ref _options, value); }

        public string SearchInput { get => _searchInput; set => SetProperty(ref _searchInput, value); }

        public ObservableCollection<Timesheet> Timesheets { get; set; } = new();

        public int TotalTimesheets { get => _totalTimesheets; set => SetProperty(ref _totalTimesheets, value); }
        #endregion

        private readonly IDialogService _dialog;
        private readonly Timesheets _timesheet;
        private IMain? _main;
        private Cutoff _cutoff = new();
        private SemaphoreSlim _busyLock = new(1);
        private PayrollCode _payrollCode = new PayrollCode();
        private SiteChoices _site = SiteChoices.MANILA;
        private const int _busyTimeout = 1000;

        #region IActiveAware
        public bool IsActive { get; set; }
        public event EventHandler? IsActiveChanged;
        #endregion

        public TimesheetListingViewModel(IDialogService dialog, Timesheets timesheets)
        {
            _dialog = dialog;
            _timesheet = timesheets;

            LoadTimesheetsCommand = new DelegateCommand(ExecuteLoadTimesheets);
            LoadSummaryCommand = new DelegateCommand(ExecuteLoadSummary);
            EvaluateCommand = new DelegateCommand<object?>(ExecuteEvaluate);
            ExportCommand = new DelegateCommand(ExecuteExport);
            DownloadCommand = new DelegateCommand<object?>(ExecuteDownload);
            DetailTimesheetCommand = new DelegateCommand<object?>(ExecuteDetailTimesheet);
        }

        public Cutoff Cutoff { get => _cutoff; set => SetProperty(ref _cutoff, value); }

        public PayrollCode PayrollCode { get => _payrollCode; set => SetProperty(ref _payrollCode, value); }

        public SiteChoices Site { get => _site; set => SetProperty(ref _site, value); }

        private async Task Download(object? parameter, CancellationToken cancellationToken = default)
        {
            if (await _busyLock.WaitAsync(_busyTimeout, cancellationToken))
            {
                try
                {
                    string site = Site.ToString();
                    string payrollCode = PayrollCode.Name;
                    Cutoff cutoff = Cutoff;
                    cutoff.SetSite(site);

                    if (parameter is int page)
                    {
                        await Download(page, cutoff, payrollCode, site, cancellationToken);
                    }
                    else if (parameter is int[] pages)
                    {
                        await Download(pages, cutoff, payrollCode, site, cancellationToken);
                    }
                    else
                    {
                        var summary = await _timesheet.DownloadContentSummary(cutoff, payrollCode, site);
                        pages = summary != null
                            ? Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToArray()
                            : Array.Empty<int>();
                        await Download(pages, cutoff, payrollCode, site, cancellationToken);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _busyLock.Release();
                }
            }
        }

        private async Task Download(int[] pages, Cutoff cutoff, string payrollCode, string site, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (int page in pages)
                {
                    await Download(page, cutoff, payrollCode, site, cancellationToken);
                }
            }
            catch
            {
                throw;
            }
        }

        private async Task Download(int page, Cutoff cutoff, string payrollCOde, string site, CancellationToken cancellationToken = default)
        {
            try
            {
                var timesheets = await _timesheet.DownloadContent(cutoff, payrollCOde, site, page);

                foreach (var timesheet in timesheets)
                {
                    var ee = _timesheet.FindEmployeeView(timesheet.EEId);
                    timesheet.EE = ee;
                    timesheet.CutoffId = cutoff.CutoffId;

                    Timesheets.Add(timesheet);
                }
            }
            catch
            {
                throw;
            }
        }

        private async Task Evaluate(object? parameter, CancellationToken cancellationToken = default)
        {
            if (await _busyLock.WaitAsync(_busyTimeout, cancellationToken))
            {
                try
                {
                    string cutoffId = Cutoff.CutoffId;
                    string payrollCode = PayrollCode.PayrollCodeId;
                    int[] missingPages = _timesheet.GetMissingPages(cutoffId, payrollCode);

                    if (missingPages.Length == 0)
                    {
                        var noEETimesheets = _timesheet.ListTimesheetNoEETimesheet(cutoffId);
                    }
                    else
                    {
                        await Download(missingPages, cancellationToken);
                    }

                    await LoadTimesheets(cancellationToken);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _busyLock.Release();
                }
            }
        }

        #region export
        private async Task Export(CancellationToken cancellationToken = default)
        {
            if (await _busyLock.WaitAsync(_busyTimeout, cancellationToken))
            {
                try
                {
                    var cutoff = Cutoff;
                    var cutoffId = cutoff.CutoffId;
                    var payrollCode = PayrollCode.PayrollCodeId;
                    cutoff.SetSite(PayrollCode.Site);
                    var timesheets = await _timesheet.GetTimesheets(cutoffId, cancellationToken);
                    var filtered = timesheets.FilterByPayrollCode(payrollCode);

                    // if (timesheets.Any(t => !t.IsValid)) // IsValid property doesn't exist
                    // {
                    //     // prompt to proceed
                    // }

                    IEnumerable<Timesheet> twoPeriodTimesheets = _timesheet.GetTwoPeriodTimesheets(cutoffId).FilterByPayrollCode(payrollCode);
                    List<TimesheetBankChoices> bankCategories = filtered.ExtractBanks();
                    var bankTasks = new List<Task>();

                    foreach (var bankCategory in bankCategories)
                    {
                        var bankTask = Task.Run(() =>
                        {
                            var timesheetsByBankCategory = filtered.FilterByBank(bankCategory);
                            var twoPeriodTimesheetsByBankCategory = twoPeriodTimesheets.FilterByBank(bankCategory);

                            if (timesheetsByBankCategory.Any())
                            {
                                var exportable = timesheetsByBankCategory.ByExportable().ToList();
                                var unconfirmedTimesheetsWithAttendance = timesheetsByBankCategory.ByUnconfirmedWithAttendance().ToList();
                                var unconfirmedTimesheetsWithoutAttendance = timesheetsByBankCategory.ByUnconfirmedWithoutAttendance().ToList();
                                var monthlyExportable = twoPeriodTimesheetsByBankCategory.ByExportable().GroupTimesheetsByEEId().ToList();

                                ExportDbf(cutoff, payrollCode, bankCategory, exportable);
                                ExportFeedback(cutoff, payrollCode, bankCategory, exportable,
                                    unconfirmedTimesheetsWithAttendance,
                                    unconfirmedTimesheetsWithoutAttendance);
                                ExportEFile(cutoff, payrollCode, bankCategory, monthlyExportable);
                            }
                        }, cancellationToken);

                        bankTasks.Add(bankTask);
                    }

                    await Task.WhenAll(bankTasks);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _busyLock.Release();
                }
            }
        }

        private void ExportDbf(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank, IEnumerable<Timesheet> exportable)
        {
            try
            {
                string dbfdir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\{cutoff.CutoffId}\{payrollCode}";
                string dbfpath = $@"{dbfdir}\{payrollCode}_{bank}_{cutoff.CutoffId}.DBF";
                Directory.CreateDirectory(dbfdir);
                TimesheetDbfExporter.ExportDbf(dbfpath, cutoff.CutoffDate, exportable);
            }
            catch
            {
                throw;
            }
        }

        private void ExportEFile(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank, IEnumerable<Timesheet[]> exportable)
        {
            try
            {
                var exporter = new TimesheetEFileExporter(cutoff, payrollCode, bank, exportable);
                string efiledir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\{cutoff.CutoffId}\{payrollCode}";
                string efilepath = $@"{efiledir}\{payrollCode}_{bank}_{cutoff.CutoffId}.XLS";
                Directory.CreateDirectory(efiledir);
                exporter.ExportEFile(efilepath);
            }
            catch
            {
                throw;
            }
        }

        private void ExportFeedback(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank,
            IEnumerable<Timesheet> exportable,
            IEnumerable<Timesheet> unconfirmedTimesheetWithAttendance,
            IEnumerable<Timesheet> unconfirmedTimesheetWithoutAttendance)
        {

            try
            {
                var service =  new TimesheetFeedbackExporter(cutoff, payrollCode, bank,
                    exportable,
                    unconfirmedTimesheetWithAttendance,
                    unconfirmedTimesheetWithoutAttendance);

                string efiledir = $@"{AppDomain.CurrentDomain.BaseDirectory}\EXPORT\{cutoff.CutoffId}\{payrollCode}";
                string efilepath = $@"{efiledir}\{payrollCode}_{bank}_{cutoff.CutoffId}-FEEDBACK.XLS";
                Directory.CreateDirectory(efiledir);
                service.StartExport(efilepath);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        private void ExecuteDetailTimesheet(object? parameter)
        {
            try
            {
                if (parameter is Timesheet timesheet)
                {
                    var dialogParams = new DialogParameters
                    {
                        { PmsConstants.Timesheets, _timesheet },
                        { PmsConstants.Timesheet, timesheet }
                    };
                    _dialog.ShowDialog(ViewNames.TimesheetDetailView, dialogParams, (_) => { });
                }
            }
            catch
            {
                throw;
            }
        }

        private void ExecuteDownload(object? parameter)
        {
            _ = Download(parameter);
        }

        private void ExecuteEvaluate(object? parameter)
        {
            _ = Evaluate(parameter);
        }

        private void ExecuteExport()
        {
            _ = Export();
        }

        private void ExecuteLoadSummary()
        {
            _ = LoadSummary();
        }

        private void ExecuteLoadTimesheets()
        {
            _ = LoadTimesheets();
        }

        private async Task FillEmployeeDetail(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Cutoff != null)
                {
                    var timesheets = (await _timesheet.GetTimesheets(Cutoff.CutoffId, cancellationToken))
                        .Where(t => t.EE?.PayrollCode == PayrollCode.PayrollCodeId);

                    foreach (var timesheet in timesheets)
                    {
                        // nothing happens here
                        // _timesheet.SaveEmployeeData(timesheet);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private async Task LoadSummary(CancellationToken cancellationToken = default)
        {
            if (await _busyLock.WaitAsync(_busyTimeout, cancellationToken))
            {
                try
                {
                    string site = Site.ToString();
                    string payrollCode = PayrollCode.Name;
                    Cutoff cutoff = Cutoff;
                    cutoff.SetSite(site);
                    var summary = await _timesheet.DownloadContentSummary(cutoff, payrollCode, site);
                    if (summary == null) throw new Exception("Summary is null.");
                    var timesheets = _timesheet.MapEmployeeView(summary.UnconfirmedTimesheet);

                    Timesheets.Clear();
                    Timesheets.AddRange(timesheets);
                    NotConfirmed = Timesheets.Count;
                    Confirmed = int.Parse(summary.TotalConfirmed);
                    TotalTimesheets = int.Parse(summary.TotalCount);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _busyLock.Release();
                }
            }
        }

        private async Task LoadTimesheets(CancellationToken cancellationToken = default)
        {
            if (await _busyLock.WaitAsync(_busyTimeout, cancellationToken))
            {
                try
                {
                    if (Cutoff != null && PayrollCode != null)
                    {
                        var timesheets = (await _timesheet.GetTimesheets(Cutoff.CutoffId, cancellationToken))
                            .FilterPayrollCode(PayrollCode.PayrollCodeId)
                            .FilterSearchInput(SearchInput);

                        Timesheets.Clear();
                        Timesheets.AddRange(timesheets);
                        Confirmed = Timesheets.Count(t => t.TotalHours > 0 && t.IsConfirmed);
                        ConfirmedWithoutAttendance = Timesheets.Count(t => t.TotalHours == 0 && t.IsConfirmed);
                        NotConfirmed = Timesheets.Count(t => !t.IsConfirmed);
                        NotConfirmedWithAttendance = Timesheets.Count(t => t.TotalHours > 0 && !t.IsConfirmed);
                        TotalTimesheets = Timesheets.Count;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _busyLock.Release();
                }
            }
        }

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _main = navigationContext.Parameters.GetValue<IMain?>(PmsConstants.Main);
            if (_main != null)
            {
                _main.PropertyChanged += Main_PropertyChanged;
            }

            _ = LoadValues();
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
            if (_main != null)
            {
                _cutoff = !string.IsNullOrEmpty(_main.CutoffId) ? new Cutoff(_main.CutoffId) : new Cutoff();
                _payrollCode = _main.PayrollCode ?? new PayrollCode();
                _site = _main.Site ?? SiteChoices.UNKNOWN;

                await LoadTimesheets(cancellationToken);
            }
        }

        #region commands
        public DelegateCommand<object?> DetailTimesheetCommand { get; }
        public DelegateCommand<object?> DownloadCommand { get; }
        public DelegateCommand<object?> EvaluateCommand { get; }
        public DelegateCommand ExportCommand { get; }
        public DelegateCommand LoadSummaryCommand { get; }
        public DelegateCommand LoadTimesheetsCommand { get; }
        #endregion

        #region IRegionMemberLifetime
        public bool KeepAlive => true;
        #endregion
    }
}

