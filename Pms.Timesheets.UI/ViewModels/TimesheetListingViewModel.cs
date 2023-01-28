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

        public string SearchInput
        {
            get => _searchInput;
            set
            {
                if (SetProperty(ref _searchInput, value))
                {
                    var source = CollectionViewSource.GetDefaultView(Timesheets);
                    source.Filter = t => FilterTimesheets(t);
                }
            }
        }

        public ObservableCollection<Timesheet>? Timesheets { get; set; }

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
            DetailTimesheetCommand = new DelegateCommand<object?>(ShowDetail);
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
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (System.Net.Http.HttpRequestException ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.GetBaseException().Message, "Download");
            }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.GetBaseException().ToString(), "Download");
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
                    tasks.Add(Download(i, cutoff, payrollCode, downloadCts.Token));
                }

                while (tasks.Any())
                {
                    var completedTask = await Task.WhenAny(tasks);

                    if (completedTask.Exception != null)
                    {
                        throw completedTask.Exception;
                    }

                    tasks.Remove(completedTask);
                }
            }
            catch (TaskCanceledException) { throw; }
            catch
            {
                downloadCts.Cancel();
                throw;
            }
        }

        // timesheets with non-existing EEs will still get downloaded
        private async Task Download(int page, Cutoff cutoff, string payrollCode, CancellationToken cancellationToken = default)
        {
            try
            {
                await _tempDownloadTaskCap.WaitAsync(cancellationToken);
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
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export");
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
                var timesheets = result.Parameters.GetValue<IEnumerable<Timesheet>?>(PmsConstants.Timesheets);
                var cutoff = result.Parameters.GetValue<Cutoff?>(PmsConstants.Cutoff);
                var payrollCode = result.Parameters.GetValue<string?>(PmsConstants.PayrollCode);

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
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Export");
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

        #region show detail
        private void ShowDetail(object? parameter)
        {
            if (parameter is Timesheet timesheet)
            {
                var dialogParams = new DialogParameters
                {
                    { PmsConstants.Timesheet, timesheet }
                };
                s_Dialog.ShowDialog(ViewNames.TimesheetDetailView, dialogParams, (_) => { });
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
                Timesheets = new ObservableCollection<Timesheet>(timesheets);

                RaisePropertyChanged(nameof(Timesheets));
                var source = CollectionViewSource.GetDefaultView(Timesheets);
                source.Filter = t => FilterTimesheets(t);

                NotConfirmed = Timesheets.Count;
                Confirmed = int.TryParse(summary?.TotalConfirmed, out int confirmed) ? confirmed : 0;
                TotalTimesheets = int.TryParse(summary?.TotalCount, out int total) ? total : 0;

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Load summary");
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
                if (payrollCode == null)
                {
                    var timesheets = await m_Timesheets.GetTimesheets(cutoff.CutoffId, cancellationToken);
                    Timesheets = new ObservableCollection<Timesheet>(timesheets);
                }
                else
                {
                    var timesheets = await m_Timesheets.GetTimesheets(cutoff.CutoffId, payrollCode.PayrollCodeId, cancellationToken);
                    Timesheets = new ObservableCollection<Timesheet>(timesheets);
                }

                RaisePropertyChanged(nameof(Timesheets));
                var source = CollectionViewSource.GetDefaultView(Timesheets);
                source.Filter = t => FilterTimesheets(t);

                Confirmed = Timesheets.Count(t => t.TotalHours > 0 && t.IsConfirmed);
                ConfirmedWithoutAttendance = Timesheets.Count(t => t.TotalHours == 0 && t.IsConfirmed);
                NotConfirmed = Timesheets.Count(t => !t.IsConfirmed);
                NotConfirmedWithAttendance = Timesheets.Count(t => t.TotalHours > 0 && !t.IsConfirmed);
                TotalTimesheets = Timesheets.Count;

                OnTaskCompleted();
            }
            catch (TaskCanceledException) { OnTaskException(); }
            catch (Exception ex)
            {
                OnTaskException();
                s_Message.ShowDialog(ex.Message, "Load timesheets");
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
                var isEmpty = string.IsNullOrEmpty(SearchInput);
                var idMatch = timesheet.EEId.Contains(SearchInput);
                var nameMatch = timesheet.EE.Fullname.Contains(SearchInput);

                return isEmpty || idMatch || nameMatch;
            }

            return false;
        }
    }
}

