using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// EntrustPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.TradeResultRegion)]
    public partial class TradeResultPanel
    {
        public TradeResultPanel()
        {
            InitializeComponent();
        }

        [Import]
        public TradeResultPanelViewModel Model
        {
            get
            {
                return DataContext as TradeResultPanelViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
