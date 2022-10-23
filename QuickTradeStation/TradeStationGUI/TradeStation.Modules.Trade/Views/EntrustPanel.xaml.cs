using System.Windows;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.EntrustRegion)]
    public partial class EntrustPanel
    {
        public EntrustPanel()
        {
            InitializeComponent();
            Filter();
        }

        [Import]
        public EntrustPanelViewModel Model
        {
            get
            {
                return DataContext as EntrustPanelViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void cbNotTraded_Unchecked(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            if (cbFilter.IsChecked.Value)
            {
                CommonUtil.Filter(xamGrid, cbTraded, cbCancelled, cbObsolete, intellibox);
            }
            else
            {
                CommonUtil.Filter(xamGrid, cbTraded, cbCancelled, cbObsolete, null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void intellibox_SearchBeginning(string arg1, int arg2, object arg3)
        {
            cbFilter.IsChecked = false;
        }

        private void xamGrid_Filtered(object sender, Infragistics.Controls.Grids.FilteredEventArgs e)
        {

        }

        private void xamGrid_Filtering(object sender, Infragistics.Controls.Grids.CancellableFilteringEventArgs e)
        {

        }

        private void ListView_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }
    }
}
