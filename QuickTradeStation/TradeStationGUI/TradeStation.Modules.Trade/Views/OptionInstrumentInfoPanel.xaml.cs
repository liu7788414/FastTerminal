using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;


namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// OptionInstrumentInfoPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.OptionInstrumentInfoRegion)]
    public partial class OptionInstrumentInfoPanel
    {
        public OptionInstrumentInfoPanel()
        {
            InitializeComponent();
        }

        [Import]
        public OptionInstrumentPanelViewModel Model
        {
            get
            {
                return DataContext as OptionInstrumentPanelViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
