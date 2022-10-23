using System.Windows.Controls;
using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;


namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// PositionPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.InstrumentInfoPanelRegion)]
    public partial class InstrumentInfoPanel
    {
        public InstrumentInfoPanel()
        {
            InitializeComponent();
        }

        [Import]
        public InstrumentPanelViewModel Model
        {
            get
            {
                return DataContext as InstrumentPanelViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
