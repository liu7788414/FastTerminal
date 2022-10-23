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
using Infragistics.Windows.DockManager;

namespace TradeStation.Future.Views
{
    /// <summary>
    /// FutureQuotPanel.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class FutureQuotPanel : ContentPane
    {
        public FutureQuotPanel()
        {
            InitializeComponent();
        }
    }
}
