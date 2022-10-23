using System.Windows;
using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.FundEntrustRegion)]
    public partial class EntrustPanelFund
    {
        public EntrustPanelFund()
        {
            InitializeComponent();
            Filter();
        }

        [Import]
        public EntrustPanelFundViewModel Model
        {
            get
            {
                return DataContext as EntrustPanelFundViewModel;
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
            CommonUtil.Filter(xamGrid, cbTraded, cbCancelled, cbObsolete);
        }
    }
}
