using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// TradePanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.BasketTradeRegion)]
    public partial class TradePanelBasket
    {
        public TradePanelBasket()
        {
            InitializeComponent();
        }

        [Import]
        public TradePanelViewModelBasket Model
        {
            get
            {
                return DataContext as TradePanelViewModelBasket;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
