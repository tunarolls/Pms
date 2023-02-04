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
        public DummyListingViewModel(IDialogService dialog, IMessageBoxService message, Timesheets timesheets)
            : base(dialog, message, timesheets)
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

        public string SearchInput
        {
            get => _searchInput;
            set
            {
                if (SetProperty(ref _searchInput, value))
                {
                    var source = CollectionViewSource.GetDefaultView(Timesheets);
                    source.Refresh();
                }
            }
        }

        public RangedObservableCollection<Timesheet> Timesheets { get; set; }

        public int TotalTimesheets { get => _totalTimesheets; set => SetProperty(ref _totalTimesheets, value); }
        #endregion

        private readonly IDialogService s_Dialog;
        private readonly IMessageBoxService s_Message;
        private readonly Timesheets m_Timesheets;

        public TimesheetListingViewModel(IDialogService dialog, IMessageBoxService message, Timesheets timesheets)
        {
            s_Dialog = dialog;
            s_Message = message;
            m_Timesheets = timesheets;

            LoadTimesheetsCommand = new DelegateCommand(LoadTimesheets);
            LoadSummaryCommand = new DelegateCommand(LoadSummary);
            ExportCommand = new DelegateCommand(Export);
            DownloadCommand = new DelegateCommand<object?>(Download);
            DetailTimesheetCommand = new DelegateCommand<object?>(ShowDetail);

            Timesheets = new RangedObservableCollection<Timesheet>();
            var source = CollectionViewSource.GetDefaultView(Timesheets);
            source.Filter = t => FilterTimesheets(t);
            source.CollectionChanged += Timesheets_CollectionChanged;
        }

        private void Timesheets_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (sender is ICollectionView source)
            {
                var filtered = source.OfType<Timesheet>();
                Confirmed = filtered.Count(t => t.TotalHours > 0 && t.IsConfirmed);
                ConfirmedWithoutAttendance = filtered.Count(t => t.TotalHours == 0 && t.IsConfirmed);
                NotConfirmed = filtered.Count(t => !t.IsConfirmed);
                NotConfirmedWithAttendance = filtered.Count(t => t.TotalHours > 0 && !t.IsConfirmed);
                TotalTimesheets = filtered.Count();
            }
        }

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
            _ = StartDownload(parameter, cts.Token);
        }

        private async Task StartDownload(object? parameter, CancellationToken cancellationToken)
        {
            await Download(parameter, cancellationToken);
            LoadTimesheets();
        }

        private async Task Download(object? parameter, CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCode = Main?.PayrollCode;
                var cutoff = new Cutoff(Main?.CutoffId, Main?.Site);
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);

                OnMessageSent("Downloading timesheets...");
                if (parameter is int page)
                {
                    await Download(new int[] { page }, cutoff, payrollCode.Name, cancellationToken: cancellationToken);
                }
                else if (parameter is int[] pages)
                {
                    await Download(pages, cutoff, payrollCode.Name, cancellationToken: cancellationToken);
                }
                else
                {
                    var summary = await m_Timesheets.DownloadContentSummary(cutoff, payrollCode.Name, cancellationToken);
                    pages = summary != null
                        ? Enumerable.Range(0, int.Parse(summary.TotalPage) + 1).ToArray()
                        : Array.Empty<int>();

                    await Download(pages, cutoff, payrollCode.Name, cancellationToken: cancellationToken);
                }

                OnTaskCompleted();
                s_Message.ShowDialog("Download done.", "");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Download", ex.ToString());
            }
        }

        readonly SemaphoreSlim _tempDownloadTaskCap = new(5);

        private async Task Download(int[] pages, Cutoff cutoff, string payrollCode, int startIndex = 0, CancellationToken cancellationToken = default)
        {
            var downloadCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                OnProgressStart(pages.Length);
                var tasks = new List<Task>();

                for (int i = startIndex; i < pages.Length; i++)
                {
                    await _tempDownloadTaskCap.WaitAsync(cancellationToken);
                    tasks.Add(Download(i, cutoff, payrollCode, downloadCts.Token));
                }

                await Task.WhenAll(tasks);
            }
            catch
            {
                throw;
            }
        }

        // timesheets with non-existing EEs will still get downloaded
        private async Task Download(int page, Cutoff cutoff, string payrollCode, CancellationToken cancellationToken = default)
        {
            try
            {
                await m_Timesheets.DownloadContent(cutoff, payrollCode, page, cancellationToken);
                OnProgressIncrement();
            }
            catch
            {
                throw;
            }
            finally
            {
                _tempDownloadTaskCap.Release();
            }
        }
        #endregion

        #region export
        private void Export()
        {
            var cts = GetCancellationTokenSource();
            var dialogParameters = CreateDialogParameters(this, cts);
            s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
            _ = StartExport(cts.Token);
        }

        private async Task StartExport(CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCode = Main?.PayrollCode;
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);
                var cutoff = new Cutoff(Main?.CutoffId);
                cutoff.SetSite(payrollCode.Site);

                OnProgressStart();
                OnMessageSent("Retrieving timesheets...");
                var timesheets = await m_Timesheets.GetTimesheets(cutoff.CutoffId, payrollCode.PayrollCodeId, cancellationToken);
                
                if (timesheets.Any(t => !t.IsValid))
                {
                    OnTaskCompleted();
                    PromptInvalidTimesheets(timesheets, cutoff, payrollCode.PayrollCodeId);
                    return;
                }
                else
                {
                    await Export(timesheets, cutoff, payrollCode.PayrollCodeId, cancellationToken);
                }

                OnTaskCompleted();
                s_Message.ShowDialog("Export done.", "");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export", ex.ToString());
            }
        }

        private void PromptInvalidTimesheets(IEnumerable<Timesheet> timesheets, Cutoff cutoff, string payrollCode)
        {
            var dialogParameters = new DialogParameters()
            {
                { PmsConstants.Timesheets, timesheets },
                { PmsConstants.Cutoff, cutoff },
                { PmsConstants.PayrollCode, payrollCode }
            };

            s_Message.ShowDialog("Some timesheets are invalid. Proceed?", dialogParameters, PromptInvalidTimesheetsCallback, "Export", PromptDialogButton.YesNo);
        }

        private void PromptInvalidTimesheetsCallback(IDialogResult result)
        {
            if (result.Result == ButtonResult.Yes)
            {
                var timesheets = result.Parameters.GetValue<IEnumerable<Timesheet>>(PmsConstants.Timesheets);
                var cutoff = result.Parameters.GetValue<Cutoff>(PmsConstants.Cutoff);
                var payrollCode = result.Parameters.GetValue<string>(PmsConstants.PayrollCode);

                if (timesheets != null && cutoff != null && payrollCode != null)
                {
                    var cts = GetCancellationTokenSource();
                    var dialogParameters = CreateDialogParameters(this, cts);
                    s_Dialog.Show(DialogNames.CancelDialog, dialogParameters, (_) => { });
                    _ = StartExport(timesheets, cutoff, payrollCode, cts.Token);
                }
            }
        }

        private async Task StartExport(IEnumerable<Timesheet> timesheets, Cutoff cutoff, string payrollCode, CancellationToken cancellationToken = default)
        {
            try
            {
                await Export(timesheets, cutoff, payrollCode, cancellationToken);
                OnTaskCompleted();

                s_Message.ShowDialog("Export done.", "");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export", ex.ToString());
            }
        }

        private async Task Export(IEnumerable<Timesheet> timesheets, Cutoff cutoff, string payrollCode, CancellationToken cancellationToken = default)
        {
            try
            {
                OnMessageSent("Retrieving two period timesheets...");
                OnProgressStart();
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

                OnMessageSent("Exporting...");
                await Task.WhenAll(tasks);
            }
            catch
            {
                throw;
            }
        }

        public static string GetTimesheetExportDirectory(string cutoffId, string payrollCode)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EXPORT", cutoffId, payrollCode);
        }

        public static string GetTimesheetExportFilename(string cutoffId, string payrollCode, TimesheetBankChoices bank, string fileExtension = "")
        {
            return $"{payrollCode}_{bank}_{cutoffId}{(string.IsNullOrEmpty(fileExtension) ? "" : $".{fileExtension.Replace(".", "")}")}";
        }

        private void ExportDbf(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank, IEnumerable<Timesheet> exportable)
        {
            var dir = GetTimesheetExportDirectory(cutoff.CutoffId, payrollCode);
            var filename = GetTimesheetExportFilename(cutoff.CutoffId, payrollCode, bank, "dbf");
            var path = Path.Combine(dir, filename);
            Directory.CreateDirectory(dir);
            TimesheetDbfExporter.ExportDbf(path, cutoff.CutoffDate, exportable);
        }

        private void ExportEFile(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank, IEnumerable<Timesheet[]> exportable)
        {
            var exporter = new TimesheetEFileExporter(cutoff, payrollCode, bank, exportable);
            var dir = GetTimesheetExportDirectory(cutoff.CutoffId, payrollCode);
            var filename = GetTimesheetExportFilename(cutoff.CutoffId, payrollCode, bank, "dbf");
            var path = Path.Combine(dir, filename);
            Directory.CreateDirectory(dir);
            exporter.ExportEFile(path);
        }

        private void ExportFeedback(Cutoff cutoff, string payrollCode, TimesheetBankChoices bank,
            IEnumerable<Timesheet> exportable,
            IEnumerable<Timesheet> unconfirmedTimesheetWithAttendance,
            IEnumerable<Timesheet> unconfirmedTimesheetWithoutAttendance)
        {

            var service = new TimesheetFeedbackExporter(cutoff, payrollCode, bank,
                    exportable,
                    unconfirmedTimesheetWithAttendance,
                    unconfirmedTimesheetWithoutAttendance);

            var dir = GetTimesheetExportDirectory(cutoff.CutoffId, payrollCode);
            var filename = $"{payrollCode}_{bank}_{cutoff.CutoffId}-FEEDBACK.XLS";
            var path = Path.Combine(dir, filename);
            Directory.CreateDirectory(dir);
            service.StartExport(path);
        }
        #endregion

        #region show detail
        private void ShowDetail(object? parameter)
        {
            if (parameter is Timesheet timesheet)
            {
                var dialogParams = new DialogParameters
                {
                    { PmsConstants.Timesheet, timesheet }
                };
                s_Dialog.Show(ViewNames.TimesheetDetailView, dialogParams, (_) => { });
            }
        }
        #endregion

        //private async Task FillEmployeeDetail(CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        if (Cutoff != null)
        //        {
        //            var timesheets = (await m_Timesheets.GetTimesheets(Cutoff.CutoffId, cancellationToken))
        //                .Where(t => t.EE?.PayrollCode == PayrollCode.PayrollCodeId);

        //            foreach (var timesheet in timesheets)
        //            {
        //                // nothing happens here
        //                // _timesheet.SaveEmployeeData(timesheet);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

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
                var payrollCode = Main?.PayrollCode;
                var cutoff = new Cutoff(Main?.CutoffId, Main?.Site);
                if (payrollCode == null) throw new Exception(ErrorMessages.PayrollCodeIsEmpty);

                var summary = await m_Timesheets.DownloadContentSummary(cutoff, payrollCode.Name, cancellationToken);
                var timesheets = await m_Timesheets.MapEmployeeView(summary?.UnconfirmedTimesheet, cancellationToken);
                Timesheets.ReplaceRange(timesheets);

                OnTaskCompleted();
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Load summary", ex.ToString());
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

        // timesheets with non-existing EEs won't get loaded
        private async Task LoadTimesheets(CancellationToken cancellationToken = default)
        {
            try
            {
                var payrollCode = Main?.PayrollCode;
                var cutoff = new Cutoff(Main?.CutoffId);

                OnMessageSent("Retrieving timesheets...");
                var timesheets = await m_Timesheets.GetTimesheets(cutoff.CutoffId, payrollCode?.PayrollCodeId, cancellationToken);
                Timesheets.ReplaceRange(timesheets);

                OnTaskCompleted();
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Load timesheets", ex.ToString());
            }
        }
        #endregion



        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Main = navigationContext.Parameters.GetValue<ITimesheetsMain>(PmsConstants.Main);

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
                var isEmpty = string.IsNullOrEmpty(SearchInput);
                var idMatch = timesheet.EEId?.Contains(SearchInput) ?? false;
                var nameMatch = timesheet.EE?.FullName?.Contains(SearchInput) ?? false;

                return isEmpty || idMatch || nameMatch;
            }

            return false;
        }
    }
}

