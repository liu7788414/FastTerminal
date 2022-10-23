using System.Windows;
using System.ComponentModel.Composition;

using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;


namespace TradeStation.Modules.Trade.Views
{
    /// <summary>
    /// PositionPanel.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.PositionRegion)]
    public partial class PositionPanel
    {
        public PositionPanel()
        {
            InitializeComponent();
            cbShowPosition0.IsChecked = false;
        }

        [Import]
        public PositionPanelViewModel Model
        {
            get
            {
                return DataContext as PositionPanelViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            CommonUtil.FilterPosition(xamGrid, cbShowPosition0);
        }
    }
}
