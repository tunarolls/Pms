using MathNet.Numerics;
using Pms.Timesheets;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Pms.Timesheets.Module.ViewModels
{
    public class TimesheetDetailViewModel : BindableBase, IDialogAware
    {
        private List<TimesheetBankChoices>? _bankChoices;
        private Timesheet? _timesheet;
        private Timesheets? _timesheets;
        private readonly SemaphoreSlim _saveLock = new(1);
        private const int _saveTimeout = 1000;
        public TimesheetDetailViewModel()
        {
            SaveCommand = new DelegateCommand(Save, CanSave);
            CancelCommand = new DelegateCommand(Cancel);
        }

        #region dialog
        public event Action<IDialogResult>? RequestClose;

        public string Title => string.Empty;

        public bool CanCloseDialog()
        {
            return _canSave;
        }

        public void OnDialogClosed()
        {
            Debug.WriteLine("Dialog closed.");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _timesheet = parameters.GetValue<Timesheet?>(Common.Constants.Timesheet);
            _timesheets = parameters.GetValue<Timesheets?>(Common.Constants.Timesheets);
            RaisePropertyChanged(nameof(Timesheet));
            RaisePropertyChanged(nameof(IsForEditing));
        }
        #endregion


        public List<TimesheetBankChoices> BankChoices => _bankChoices ??= new(Enum.GetValues(typeof(TimesheetBankChoices)).Cast<TimesheetBankChoices>());
        
        public bool IsForEditing { get => !string.IsNullOrEmpty(_timesheet?.EEId); }

        public Timesheet? Timesheet { get => _timesheet; set => SetProperty(ref _timesheet, value); }

        //public void Close()
        //{
        //    var dialogResult = new DialogResult(ButtonResult.OK);
        //    RequestClose?.Invoke(dialogResult);
        //} 

        #region save
        public DelegateCommand SaveCommand { get; }

        private bool _canSave = true;
        private bool CanSave()
        {
            return _canSave;
        }

        private void Save()
        {
            if (!IsCurrentTaskRunning())
            {
                var cts = GetCts();
                _currentTask = Save(cts.Token);
                Debug.WriteLine("Saving");
            }
            else
            {
                Debug.WriteLine("Already saving");
            }
        }

        private async Task Save(CancellationToken cancellationToken = default)
        {
            if (await _saveLock.WaitAsync(_saveTimeout, cancellationToken))
            {
                try
                {
                    DisableSave();
                    if (_timesheets == null) return;
                    await _timesheets.SaveTimesheet(_timesheet, cancellationToken);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    EnableSave();
                    _saveLock.Release();
                }
            }
        }
        #endregion

        #region cancel
        public DelegateCommand CancelCommand { get; }

        private void Cancel()
        {
            if (IsCurrentTaskRunning())
            {
                _cts?.Cancel();
            }
        }
        #endregion

        // disable any other commands while saving
        // also option to cancel
        #region test
        private CancellationTokenSource? _cts;
        private Task? _currentTask;

        private bool IsCurrentTaskRunning()
        {
            return !(_currentTask == null || _currentTask.IsCompleted);
        }

        private CancellationTokenSource GetCts()
        {
            if (_cts == null)
            {
                _cts = new CancellationTokenSource();
                return _cts;
            }
            else
            {
                if (!IsCurrentTaskRunning())
                {
                    _cts.Dispose();
                    _cts = new CancellationTokenSource();
                    return _cts;
                }
                else
                {
                    return _cts;
                }
            }
        }

        private void DisableSave()
        {
            _canSave = false;
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void EnableSave()
        {
            _canSave = true;
            SaveCommand.RaiseCanExecuteChanged();
        }

        private async Task StartSave(CancellationToken cancellationToken = default)
        {
            if (await _saveLock.WaitAsync(_saveTimeout, cancellationToken))
            {
                try
                {
                    DisableSave();
                    await ReallyLongSave(cancellationToken);
                    Debug.WriteLine("Saved");
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _saveLock.Release();
                    EnableSave();
                }
            }
        }

        private async Task ReallyLongSave(CancellationToken cancellationToken = default)
        {
            await Task.Delay(10000, cancellationToken);
        }
        #endregion
    }
}
