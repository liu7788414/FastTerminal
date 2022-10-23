using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;


namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// PositionPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.MoneyRegion)]
    public partial class MoneyPanel
    {
        public MoneyPanel()
        {
            InitializeComponent();
        }

        [Import]
        public MoneyPanelViewModel Model
        {
            get
            {
                return DataContext as MoneyPanelViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
