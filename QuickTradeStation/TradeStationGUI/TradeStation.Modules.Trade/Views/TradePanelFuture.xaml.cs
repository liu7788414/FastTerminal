using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// TradePanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.FutureTradeRegion)]
    public partial class TradePanelFuture
    {
        public TradePanelFuture()
        {
            InitializeComponent();
        }

        [Import]
        public TradePanelViewModelFuture Model
        {
            get
            {
                return DataContext as TradePanelViewModelFuture;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
