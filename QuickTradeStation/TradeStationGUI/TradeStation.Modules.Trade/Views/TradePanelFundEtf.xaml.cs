using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// TradePanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.FundEtfTradeRegion)]
    public partial class TradePanelFundEtf
    {
        public TradePanelFundEtf()
        {
            InitializeComponent();
        }

        [Import]
        public TradePanelViewModelFundEtf Model
        {
            get
            {
                return DataContext as TradePanelViewModelFundEtf;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
