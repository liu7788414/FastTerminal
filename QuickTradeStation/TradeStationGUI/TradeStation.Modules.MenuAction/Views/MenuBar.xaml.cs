using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel.Composition;
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Modules.MenuAction.Views
{
    /// <summary>
    /// MenuBar.xaml 的交互逻辑
    /// </summary>
   [ViewExport(RegionName = RegionNames.MenuBarRegion)]
    public partial class MenuBar : UserControl
    {
        public MenuBar()
        {
            InitializeComponent();
        }
    }
}
