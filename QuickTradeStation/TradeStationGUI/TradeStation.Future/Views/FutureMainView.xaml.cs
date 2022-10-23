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

namespace TradeStation.Future.Views
{
    /// <summary>
    /// FutureMainView.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class FutureMainView : TabItemEx, ISequencableView
    {
        public FutureMainView()
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
                case "btFutureTrade":
                    {
                        cp = cpFutureTrade;
                        break;
                    }
                case "btFutureEntrust":
                    {
                        cp = cpFutureEntrust;
                        break;
                    }
                case "btFutureTradeResult":
                    {
                        cp = cpFutureTradeResult;
                        break;
                    }
                case "btFuturePosition":
                    {
                        cp = cpFuturePosition;
                        break;
                    }
                case "btFutureMarginInfo":
                    {
                        cp = cpFutureMarginInfo;
                        break;
                    }
                case "btInstrumentInfo":
                    {
                        cp = cpInstrumentInfo;
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
            SaveAndLoadUILayoutHelper.LoadLayout(CommonUtil.LayoutFileFuture, dockManagerFuture);
        }

        private void Window_Closing()
        {
            SaveAndLoadUILayoutHelper.SaveLayout(CommonUtil.LayoutFileFuture, dockManagerFuture);
        }

        public int Order
        {
            get { return CommonUtil.VIEW_ORDER_FUTURE; }
        }
    }
}
