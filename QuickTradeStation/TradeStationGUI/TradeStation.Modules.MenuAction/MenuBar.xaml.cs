using System.ComponentModel.Composition;
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.MenuAction.Views
{
    /// <summary>
    /// MenuBar.xaml 的交互逻辑
    /// </summary>
   [ViewExport(RegionName = RegionNames.MenuBarRegion)]
    public partial class MenuBar
    {
        public MenuBar()
        {
            InitializeComponent();
        }

        [Import]
        public MenubarViewModel Model
        {
            get
            {
                return DataContext as MenubarViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
