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
    [ViewExport(RegionName = RegionNames.EtfBaseInfoPanelRegion)]
    public partial class EtfBaseInfoPanel
    {
        public EtfBaseInfoPanel()
        {
            InitializeComponent();
        }

        [Import]
        public EtfBaseInfoPanelViewModel Model
        {
            get
            {
                return DataContext as EtfBaseInfoPanelViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
