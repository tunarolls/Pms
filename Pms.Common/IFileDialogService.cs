using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Common
{
    public interface IFileDialogService
    {
        public void ShowMultiFileDialog(Action<IFileDialogResult> callback);
        public void ShowFileDialog(Action<IFileDialogResult> callback);
    }

    public interface IFileDialogResult
    {
        public string[] FileNames { get; }
        public string FileName { get; }
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
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() ?? false)
            {
                callback(new FileDialogResult(dialog.FileName));
            }
        }

        public void ShowMultiFileDialog(Action<IFileDialogResult> callback)
        {
            var dialog = new OpenFileDialog()
            {
                Multiselect = true
            };

            if (dialog.ShowDialog() ?? false)
            {
                callback(new FileDialogResult(dialog.FileNames));
            }
        }
    }
}
