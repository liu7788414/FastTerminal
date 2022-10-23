using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.FundTradeResultRegion)]
    public partial class TradeResultPanelFund
    {
        public TradeResultPanelFund()
        {
            InitializeComponent();
        }

        [Import]
        public TradeResultPanelViewModelFund Model
        {
            get
            {
                return DataContext as TradeResultPanelViewModelFund;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
