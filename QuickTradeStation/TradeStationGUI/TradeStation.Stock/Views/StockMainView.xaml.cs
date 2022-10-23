using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Practices.Prism.Commands;

using Infragistics.Windows.Controls;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DockManager.Events;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.ViewModels;

namespace TradeStation.Stock.Views
{
    /// <summary>
    /// StockMainView.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class StockMainView : TabItemEx, ISequencableView
    {
        public StockMainView()
        {
            InitializeComponent();

            HostCommands.StartupCommand.RegisterCommand(new DelegateCommand(Window_Loaded));
            HostCommands.ShutdownCommand.RegisterCommand(new DelegateCommand(Window_Closing));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ContentPane cp = null;
            switch (bt.Name)
            {
                case "btStockTrade":
                    {
                        cp = cpStockTrade;
                        break;
                    }
                case "btStockEntrust":
                    {
                        cp = cpStockEntrust;
                        break;
                    }
                case "btStockPosition":
                    {
                        cp = cpStockPosition;
                        break;
                    }
                case "btStockTradeResult":
                    {
                        cp = cpStockTradeResult;
                        break;
                    }
                case "btStockAccountMoney":
                    {
                        cp = cpStockAccountMoney;
                        break;
                    }
                case "btLog":
                    {
                        cp = cpLog;
                        break;
                    }
            }

            if (cp != null)
            {
                cp.Visibility = Visibility.Visible;
                cp.Activate();
            }
        }

        private void Window_Loaded()
        {
            SaveAndLoadUILayoutHelper.LoadLayout(CommonUtil.LayoutFileStock, dockManagerStock);
        }

        private void Window_Closing()
        {
            SaveAndLoadUILayoutHelper.SaveLayout(CommonUtil.LayoutFileStock, dockManagerStock);
        }

        public int Order
        {
            get { return CommonUtil.VIEW_ORDER_STOCK; }
        }

        private void dockManagerStock_ToolWindowLoaded(object sender, PaneToolWindowEventArgs e)
        {
        }
    }
}
