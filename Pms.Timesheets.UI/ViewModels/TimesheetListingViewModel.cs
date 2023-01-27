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
using System.Windows.Data;
using Microsoft.Xaml.Behaviors.Media;
using Pms.Common;
using Pms.Common.Enums;
using Pms.Common.Exceptions;
using Pms.Masterlists.Entities;
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
        public DummyListingViewModel(IDialogService dialog, IMessageBoxService message, Timesheets timesheets) : base(dialog, message, timesheets)
        {
        }
    }

    public class TimesheetListingViewModel : CancellableBase, INavigationAware
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

        public ObservableCollection<Timesheet> Timesheets { get; set; }

        public int TotalTimesheets { get => _totalTimesheets; set => SetProperty(ref _totalTimesheets, value); }
        #endregion

        private readonly IDialogService s_Dialog;
        private readonly IMessageBoxService s_Message;
        private readonly Timesheets m_Timesheets;
        private Cutoff _cutoff = new();
        private PayrollCode _payrollCode = new PayrollCode();
        private SiteChoices _site = SiteChoices.MANILA;

        public TimesheetListingViewModel(IDialogService dialog, IMessageBoxService message, Timesheets timesheets)
        {
            s_Dialog = dialog;
            s_Message = message;
            m_Timesheets = timesheets;

            LoadTimesheetsCommand = new DelegateCommand(LoadTimesheets);
            LoadSummaryCommand = new DelegateCommand(LoadSummary);
            ExportCommand = new DelegateCommand(Export);
            DownloadCommand = new DelegateCommand<object?>(Download);
            DetailTimesheetCommand = new DelegateCommand<object?>(ExecuteDetailTimesheet);

            Timesheets = new();
            var source = CollectionViewSource.GetDefaultView(Timesheets);
            source.Filter = t => FilterTimesheets(t);
        }

        public Cutoff Cutoff { get => _cutoff; set => SetProperty(ref _cutoff, value); }
        public PayrollCode PayrollCode { get => _payrollCode; set => SetProperty(ref _payrollCode, value); }
        public SiteChoices Site { get => _site; set => SetProperty(ref _site, value); }
        public ITimesheetsMain? Main { get; set; }

        #region commands
        public DelegateCommand<object?> DetailTimesheetCommand { get; }
        public DelegateCommand<object?> DownloadCommand { get; }
        public DelegateCommand ExportCommand { get; }
        public DelegateCommand LoadSummaryCommand { get; }
        public DelegateCommand LoadTimesheetsCommand { get; }
        #endregion

        #region download
        private void Download(object? parameter)
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Download(parameter, cts.Token);
        }

        private async Task Download(object? parameter, CancellationToken cancellationToken = default)
        {
            try
            {
                OnMessageSent("Downloading timesheets...");

                var site = Main?.Site ?? SiteChoices.MANILA;
                var payrollCode = Main?.PayrollCode?.Name;
                var cutoff = new Cutoff(Main?.CutoffId, site);

                if (string.IsNullOrEmpty(payrollCode)) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);

                if (parameter is int page)
                {
                    await Download(new int[] { page }, cutoff, payrollCode, cancellationToken: cancellationToken);
                }
                else if (parameter is int[] pages)
                {
                    await Download(pages, cutoff, payrollCode, cancellationToken: cancellationToken);
                }
                else
                {
                    var summary = await m_Timesheets.DownloadContentSummary(cutoff, payrollCode, cancellationToken);
                    pages = summary != null
                        ? Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToArray()
                        : Array.Empty<int>();

                    await Download(pages, cutoff, payrollCode, cancellationToken: cancellationToken);
                }

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
            }
        }

        private async Task Download(int[] pages, Cutoff cutoff, string payrollCode, int startIndex = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                OnProgressStart(pages.Length);

                for (int i = startIndex; i < pages.Length; i++)
                {
                    await m_Timesheets.DownloadContent(cutoff, payrollCode, i, cancellationToken);
                    OnProgressIncrement();
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region export
        private void Export()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = Export(cts.Token);
        }

        private async Task Export(IEnumerable<Timesheet> timesheets, Cutoff cutoff, string payrollCode, CancellationToken cancellationToken = default)
        {
            try
            {
                var twoPeriodTimesheets = await m_Timesheets.GetTwoPeriodTimesheets(cutoff.CutoffId, payrollCode, cancellationToken);
                var bankCategories = twoPeriodTimesheets.ExtractBanks();
                var tasks = new List<Task>();

                foreach (var bankCategory in bankCategories)
                {
                    var timesheetsByBank = timesheets.FilterByBank(bankCategory);
                    var twoPeriodTimesheetsByBank = twoPeriodTimesheets.FilterByBank(bankCategory);

                    if (timesheetsByBank.Any())
                    {
                        var exportable = timesheetsByBank.Exportable().ToList();
                        var unconfirmed = timesheetsByBank.UnconfirmedWithAttendance().ToList();
                        var unconfirmedWithoutAttendance = timesheetsByBank.UnconfirmedWithoutAttendance().ToList();
                        var monthlyExportable = twoPeriodTimesheetsByBank.Exportable().TimesheetsByEEId().ToList();

                        var dbfTask = Task.Run(() =>
                        {
                            ExportDbf(cutoff, payrollCode, bankCategory, exportable);
                        }, cancellationToken);

                        var feedbackTask = Task.Run(() =>
                        {
                            ExportFeedback(cutoff, payrollCode, bankCategory, exportable, unconfirmed, unconfirmedWithoutAttendance);
                        }, cancellationToken);

                        var eFileTask = Task.Run(() =>
                        {
                            ExportEFile(cutoff, payrollCode, bankCategory, monthlyExportable);
                        }, cancellationToken);

                        tasks.AddRange(new[] { dbfTask, feedbackTask, eFileTask });
                    }
                }

                await Task.WhenAll(tasks);
            }
            catch
            {
                throw;
            }
        }

        private async Task Export(CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCode = Main?.PayrollCode;
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);
                var cutoff = new Cutoff(Main?.CutoffId);
                cutoff.SetSite(payrollCode.Site);
                var timesheets = await m_Timesheets.GetTimesheets(cutoff.CutoffId, payrollCode.PayrollCodeId, cancellationToken);

                await Export(timesheets, cutoff, payrollCode.PayrollCodeId, cancellationToken);
                // if (timesheets.Any(t => !t.IsValid)) // IsValid property doesn't exist
                // {
                //     // prompt to proceed
                // }

                //IEnumerable<Timesheet> twoPeriodTimesheets = m_Timesheets.GetTwoPeriodTimesheets(cutoffId).FilterByPayrollCode(payrollCode);
                //List<TimesheetBankChoices> bankCategories = filtered.ExtractBanks();
                //var bankTasks = new List<Task>();

                //foreach (var bankCategory in bankCategories)
                //{
                //    var bankTask = Task.Run(() =>
                //    {
                //        var timesheetsByBankCategory = filtered.FilterByBank(bankCategory);
                //        var twoPeriodTimesheetsByBankCategory = twoPeriodTimesheets.FilterByBank(bankCategory);

                //        if (timesheetsByBankCategory.Any())
                //        {
                //            var exportable = timesheetsByBankCategory.ByExportable().ToList();
                //            var unconfirmedTimesheetsWithAttendance = timesheetsByBankCategory.ByUnconfirmedWithAttendance().ToList();
                //            var unconfirmedTimesheetsWithoutAttendance = timesheetsByBankCategory.ByUnconfirmedWithoutAttendance().ToList();
                //            var monthlyExportable = twoPeriodTimesheetsByBankCategory.ByExportable().GroupTimesheetsByEEId().ToList();

                //            ExportDbf(cutoff, payrollCode, bankCategory, exportable);
                //            ExportFeedback(cutoff, payrollCode, bankCategory, exportable,
                //                unconfirmedTimesheetsWithAttendance,
                //                unconfirmedTimesheetsWithoutAttendance);
                //            ExportEFile(cutoff, payrollCode, bankCategory, monthlyExportable);
                //        }
                //    }, cancellationToken);

                //    bankTasks.Add(bankTask);
                //}

                //await Task.WhenAll(bankTasks);

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
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
                        { PmsConstants.Timesheets, m_Timesheets },
                        { PmsConstants.Timesheet, timesheet }
                    };
                    s_Dialog.ShowDialog(ViewNames.TimesheetDetailView, dialogParams, (_) => { });
                }
            }
            catch
            {
                throw;
            }
        }

        private async Task FillEmployeeDetail(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Cutoff != null)
                {
                    var timesheets = (await m_Timesheets.GetTimesheets(Cutoff.CutoffId, cancellationToken))
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

        #region load summary
        private void LoadSummary()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = LoadSummary(cts.Token);
        }

        private async Task LoadSummary(CancellationToken cancellationToken = default)
        {
            try
            {
                //string site = Site.ToString();
                //string payrollCode = PayrollCode.Name;
                //Cutoff cutoff = Cutoff;
                //cutoff.SetSite(site);
                //var summary = await m_Timesheets.DownloadContentSummary(cutoff, payrollCode, site);
                //if (summary == null) throw new Exception("Summary is null.");
                //var timesheets = m_Timesheets.MapEmployeeView(summary.UnconfirmedTimesheet);

                //Timesheets.Clear();
                //Timesheets.AddRange(timesheets);
                //NotConfirmed = Timesheets.Count;
                //Confirmed = int.Parse(summary.TotalConfirmed);
                //TotalTimesheets = int.Parse(summary.TotalCount);
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
            }
        }

        #endregion

        #region load timesheets
        private void LoadTimesheets()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = LoadTimesheets(cts.Token);
        }

        private async Task LoadTimesheets(CancellationToken cancellationToken = default)
        {
            try
            {
                var timesheets = await m_Timesheets.GetTimesheets(Cutoff.CutoffId, cancellationToken);

                //Timesheets = timesheets.FilterByPayrollCode(PayrollCode.PayrollCodeId)
                //    .FilterSearchInput(SearchInput);

                //Timesheets.Clear();
                //Timesheets.AddRange(timesheets);
                //Confirmed = Timesheets.Count(t => t.TotalHours > 0 && t.IsConfirmed);
                //ConfirmedWithoutAttendance = Timesheets.Count(t => t.TotalHours == 0 && t.IsConfirmed);
                //NotConfirmed = Timesheets.Count(t => !t.IsConfirmed);
                //NotConfirmedWithAttendance = Timesheets.Count(t => t.TotalHours > 0 && !t.IsConfirmed);
                //TotalTimesheets = Timesheets.Count;

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowError(ex.Message);
            }
        }
        #endregion



        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Main = navigationContext.Parameters.GetValue<ITimesheetsMain?>(PmsConstants.Main);

            if (Main != null)
            {
                Main.PropertyChanged += Main_PropertyChanged;
            }

            LoadTimesheets();
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
            LoadTimesheets();
        }

        private bool FilterTimesheets(object t)
        {
            if (t is Timesheet timesheet)
            {

            }

            return true;
        }

        private bool FilterByPayrollCode(Timesheet t, string payrollCode)
        {
            return t.EE.PayrollCode == payrollCode;
        }

        private bool FilterByCutoffId(Timesheet t, string cutoffId)
        {
            return t.CutoffId == cutoffId;
        }
    }
}

