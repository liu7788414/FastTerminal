using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.FutureTradeResultRegion)]
    public partial class TradeResultPanelFuture
    {
        public TradeResultPanelFuture()
        {
            InitializeComponent();
        }

        [Import]
        public TradeResultPanelViewModelFuture Model
        {
            get
            {
                return DataContext as TradeResultPanelViewModelFuture;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
