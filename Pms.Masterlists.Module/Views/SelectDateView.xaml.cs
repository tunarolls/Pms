using Pms.Common;
using System.Windows;
using System.Windows.Controls;

namespace Pms.Masterlists.Module.Views
{
    /// <summary>
    /// Interaction logic for SelectDateWidget.xaml
    /// </summary>
    public partial class SelectDateView : UserControl
    {
        public SelectDateView()
        {
            InitializeComponent();

            Loaded += SelectDateView_Loaded;
        }

        private void SelectDateView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            IconHelper.RemoveIcon(Window.GetWindow(this));
        }
    }
}
