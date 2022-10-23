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

namespace TradeStation.Option.Views
{
    /// <summary>
    /// OptionMainView.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class OptionMainView : TabItemEx, ISequencableView
    {
        public OptionMainView()
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
                case "btOptionTrade":
                    {
                        cp = cpOptionTrade;
                        break;
                    }
                case "btOptionEntrust":
                    {
                        cp = cpOptionEntrust;
                        break;
                    }
                case "btOptionTradeResult":
                    {
                        cp = cpOptionTradeResult;
                        break;
                    }
                case "btOptionPosition":
                    {
                        cp = cpOptionPosition;
                        break;
                    }
                case "btOptionMarginInfo":
                    {
                        cp = cpOptionMarginInfo;
                        break;
                    }
                case "btOptionInstrumentInfo":
                    {
                        cp = cpOptionInstrumentInfo;
                        break;
                    }
                case "btOptionMatrixPanel":
                    {
                        cp = cpOptionMatrixPanel;
                        break;
                    }
                case "btOptionCombinedFutureTrade":
                    {
                        cp = cpCombinedFutureTrade;
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
            SaveAndLoadUILayoutHelper.LoadLayout(CommonUtil.LayoutFileOption, dockManagerOption);
        }

        private void Window_Closing()
        {
            SaveAndLoadUILayoutHelper.SaveLayout(CommonUtil.LayoutFileOption, dockManagerOption);
        }

        public int Order
        {
            get { return CommonUtil.VIEW_ORDER_OPTION; }
        }
    }
}
