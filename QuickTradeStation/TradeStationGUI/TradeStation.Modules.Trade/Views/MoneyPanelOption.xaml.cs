using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;


namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// PositionPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.MoneyRegionOption)]
    public partial class MoneyPanelOption
    {
        public MoneyPanelOption()
        {
            InitializeComponent();
        }

        [Import]
        public MoneyPanelViewModelOption Model
        {
            get
            {
                return DataContext as MoneyPanelViewModelOption;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
