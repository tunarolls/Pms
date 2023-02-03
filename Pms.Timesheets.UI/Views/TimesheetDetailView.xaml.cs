using Pms.Common;
using System.Windows;
using System.Windows.Controls;

namespace Pms.Timesheets.Module.Views
{
    /// <summary>
    /// Interaction logic for TimesheetDetailView.xaml
    /// </summary>
    public partial class TimesheetDetailView : UserControl
    {
        public TimesheetDetailView()
        {
            InitializeComponent();

            Loaded += TimesheetDetailView_Loaded;
        }

        private void TimesheetDetailView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            IconHelper.RemoveIcon(Window.GetWindow(this));
        }
    }
}
