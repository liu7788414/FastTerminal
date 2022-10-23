using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Option.ViewModels;

namespace TradeStation.Option.Views
{
    /// <summary>
    /// CombinedFutureTradeView.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.CombinedFutureTradeRegion)]
    public partial class CombinedFutureTradeView : UserControl
    {
        public CombinedFutureTradeView()
        {
            InitializeComponent();
        }

        [Import]
        public CombinedFutureTradeViewModel Model
        {
            get
            {
                return DataContext as CombinedFutureTradeViewModel;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
