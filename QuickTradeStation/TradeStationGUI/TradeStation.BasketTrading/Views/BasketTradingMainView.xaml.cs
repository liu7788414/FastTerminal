using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Practices.Prism.Commands;

using Infragistics.Windows.Controls;
using Infragistics.Windows.DockManager;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.ViewModels;

namespace TradeStation.BasketTrading.Views
{
    /// <summary>
    /// BasketTradingMainView.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class BasketTradingMainView : TabItemEx, ISequencableView
    {
        public BasketTradingMainView()
        {
            InitializeComponent();
        }

        public int Order
        {
            get { return CommonUtil.VIEW_ORDER_BASKET_TRADING; }
        }
    }
}
