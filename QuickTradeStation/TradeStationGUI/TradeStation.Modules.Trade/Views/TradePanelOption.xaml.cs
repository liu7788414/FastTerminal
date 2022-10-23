using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// TradePanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.OptionTradeRegion)]
    public partial class TradePanelOption
    {
        public TradePanelOption()
        {
            InitializeComponent();
        }

        [Import]
        public TradePanelViewModelOption Model
        {
            get
            {
                return DataContext as TradePanelViewModelOption;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
