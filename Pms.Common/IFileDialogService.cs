using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pms.Common
{
    public interface IFileDialogService
    {
        public void ShowFolderDialog(Action<IFolderDialogResult> callback);
        public void ShowFolderDialog(string startDirectory, Action<IFolderDialogResult> callback);
        public void ShowMultiFileDialog(Action<IFileDialogResult> callback);
        public void ShowFileDialog(Action<IFileDialogResult> callback);
    }

    public interface IFolderDialogResult
    {
        public string DirectoryName { get; }
    }

    public interface IFileDialogResult
    {
        public string[] FileNames { get; }
        public string FileName { get; }
    }

    public class FolderDialogResult : IFolderDialogResult
    {
        public FolderDialogResult(string directoryName)
        {
            DirectoryName = directoryName;
        }

        public string DirectoryName { get; }
    }

    public class FileDialogResult : IFileDialogResult
    {
        public FileDialogResult(string[] paths)
        {
            FileName = string.Empty;
            FileNames = paths;
        }

        public FileDialogResult(string path)
        {
            FileName = path;
            FileNames = new string[] { path };
        }

        public string FileName { get; }
        public string[] FileNames { get; }
    }

    public class FileDialogService : IFileDialogService
    {
        public void ShowFileDialog(Action<IFileDialogResult> callback)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            if (dialog.ShowDialog() ?? false)
            {
                callback.Invoke(new FileDialogResult(dialog.FileName));
            }
        }

        public void ShowMultiFileDialog(Action<IFileDialogResult> callback)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Multiselect = true
            };

            if (dialog.ShowDialog() ?? false)
            {
                callback.Invoke(new FileDialogResult(dialog.FileNames));
            }
        }

        public void ShowFolderDialog(Action<IFolderDialogResult> callback)
        {
            var dialog = new FolderBrowserDialog()
            {

            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                callback.Invoke(new FolderDialogResult(dialog.SelectedPath));
            }
        }

        public void ShowFolderDialog(string startDirectory, Action<IFolderDialogResult> callback)
        {
            var dialog = new FolderBrowserDialog()
            {
                InitialDirectory = startDirectory
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                callback.Invoke(new FolderDialogResult(dialog.SelectedPath));
            }
        }
    }
}
