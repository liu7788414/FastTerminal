using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.TradeResultForBasketRegion)]
    public partial class TradeResultPanelForBasket
    {
        public TradeResultPanelForBasket()
        {
            InitializeComponent();
        }

        [Import]
        public TradeResultPanelViewModelForBasket Model
        {
            get
            {
                return DataContext as TradeResultPanelViewModelForBasket;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
