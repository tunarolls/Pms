using Pms.Common;
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
using System.Windows.Shapes;

namespace Pms.Masterlists.Module.Views
{
    public partial class EmployeeDetailView : UserControl
    {
        public EmployeeDetailView()
        {
            InitializeComponent();

            Loaded += EmployeeDetailView_Loaded;
        }

        private void EmployeeDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            IconHelper.RemoveIcon(Window.GetWindow(this));
        }
    }
}
