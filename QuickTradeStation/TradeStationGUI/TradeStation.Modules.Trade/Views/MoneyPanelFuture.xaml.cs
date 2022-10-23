using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;


namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// PositionPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.MoneyRegionFuture)]
    public partial class MoneyPanelFuture
    {
        public MoneyPanelFuture()
        {
            InitializeComponent();
        }

        [Import]
        public MoneyPanelViewModelFuture Model
        {
            get
            {
                return DataContext as MoneyPanelViewModelFuture;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
