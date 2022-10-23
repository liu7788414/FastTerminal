using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.OptionTradeResultRegion)]
    public partial class TradeResultPanelOption
    {
        public TradeResultPanelOption()
        {
            InitializeComponent();
        }

        [Import]
        public TradeResultPanelViewModelOption Model
        {
            get
            {
                return DataContext as TradeResultPanelViewModelOption;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
