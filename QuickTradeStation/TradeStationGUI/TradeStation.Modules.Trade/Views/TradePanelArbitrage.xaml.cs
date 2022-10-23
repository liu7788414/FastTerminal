using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// TradePanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.TradePanelArbitrageRegion)]
    public partial class TradePanelArbitrage
    {
        public TradePanelArbitrage()
        {
            InitializeComponent();
            Filter();
        }

        [Import]
        public TradePanelArbitrageViewModel Model
        {
            get
            {
                return DataContext as TradePanelArbitrageViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Filter();
        }

        private void cbNotTraded_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            if (cbTraded.IsChecked.Value)
            {
                CommonUtil.FilterArbitrageStatus(xamGridSupervisor, cbTraded);
            }
            else
            {
                CommonUtil.FilterArbitrageStatus(xamGridSupervisor, cbTraded);
            }
        }
    }
}
