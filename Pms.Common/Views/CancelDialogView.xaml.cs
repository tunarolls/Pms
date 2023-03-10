using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pms.Common.Views
{
    /// <summary>
    /// Interaction logic for CancelDialogView.xaml
    /// </summary>
    public partial class CancelDialogView : UserControl
    {
        public CancelDialogView()
        {
            InitializeComponent();

            Loaded += CancelDialogView_Loaded;
        }

        private void CancelDialogView_Loaded(object sender, RoutedEventArgs e)
        {
            IconHelper.RemoveIcon(Window.GetWindow(this));
        }
    }
}
