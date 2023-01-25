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

namespace Pms.Adjustments.Module.Views
{

    public partial class BillingListingView : UserControl
    {
        public BillingListingView()
        {
            InitializeComponent();
        }

        //private void Page_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //if (Shared.DefaultPayRegister is not null)
        //    //    AdjustmentBillingViewSource.Source = BillingService.CollectBillingsByPayRegister(Shared.DefaultPayRegister, null);

        //    //AdjustmentNameViewSource.Source = BillingService.CollectAdjustmentNames();
        //}

        //private void btnGenerateBilling_Click(object sender, RoutedEventArgs e)
        //{
        //    //if (Shared.DefaultCutoff is not null)
        //    //    GenerationService.GenerateBillings(Shared.DefaultCutoff.PayrollDate);
        //}

        //private void btnExport_Click(object sender, RoutedEventArgs e)
        //{
        //    //GenerationService.ExportBillings(CbAdjustmentName.Text, Shared.DefaultPayRegister);
        //}

        //private void CbAdjustmentName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //if (Shared.DefaultPayRegister is not null && e.AddedItems is not null)
        //    //    AdjustmentBillingViewSource.Source =
        //    //        BillingService.CollectBillingsByPayRegister(Shared.DefaultPayRegister, e.AddedItems[0].ToString());
        //}

    }
}
