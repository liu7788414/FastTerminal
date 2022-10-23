using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// TradePanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.TradeRegion)]
    public partial class TradePanel
    {
        public TradePanel()
        {
            InitializeComponent();
        }

        [Import]
        public TradePanelViewModel Model
        {
            get
            {
                return DataContext as TradePanelViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
