using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.FutureTradeResultForBasketRegion)]
    public partial class TradeResultPanelFutureForBasket
    {
        public TradeResultPanelFutureForBasket()
        {
            InitializeComponent();
        }

        [Import]
        public TradeResultPanelViewModelFutureForBasket Model
        {
            get
            {
                return DataContext as TradeResultPanelViewModelFutureForBasket;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
