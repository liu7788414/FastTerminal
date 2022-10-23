using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// TradePanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.FundTradeRegion)]
    public partial class TradePanelFund
    {
        public TradePanelFund()
        {
            InitializeComponent();
        }

        [Import]
        public TradePanelViewModelFund Model
        {
            get
            {
                return DataContext as TradePanelViewModelFund;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
