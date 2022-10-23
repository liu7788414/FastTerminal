using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Infragistics.Windows.DockManager;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.ViewModels;

namespace TradeStation.Fund.Views
{
    /// <summary>
    /// FundMainView.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class FundMainView : ISequencableView
    {
        public FundMainView()
        {
            InitializeComponent();

            HostCommands.StartupCommand.RegisterCommand(new DelegateCommand(Window_Loaded));
            HostCommands.ShutdownCommand.RegisterCommand(new DelegateCommand(Window_Closing));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var bt = (Button)sender;
            ContentPane cp = null;
            switch (bt.Name)
            {
                case "btFundPurchaseRedemption":
                {
                    cp = cpFundPurchaseRedemption;
                    break;
                }
                case "btFundEntrust":
                {
                    cp = cpFundEntrust;
                    break;
                }
                case "btFundTradeResult":
                {
                    cp = cpFundTradeResult;
                    break;
                }
                //case "btFundPosition":
                //{
                //    cp = cpFundPosition;
                //    break;
                //}
                case "btFundEtfBaseInfo":
                {
                    cp = cpFundEtfBaseInfo;
                    break;
                }
                case "btFundEtfStockList":
                {
                    cp = cpFundEtfStockList;
                    break;
                }
                case "btFundEtfEntrustDetail":
                {
                    cp = cpFundEtfEntrustDetail;
                    break;
                }
                case "btFundEtfTradeResultDetail":
                {
                    cp = cpFundEtfTradeResultDetail;
                    break;
                }
                case "btFundEtfPurchase":
                {
                    cp = cpFundEtfPurchase;
                    break;
                }
                // TODO: Add navigation button for each panel.
            }

            if (cp != null)
            {
                cp.Visibility = Visibility.Visible;
                cp.Activate();
            }
        }

        private void Window_Loaded()
        {
            SaveAndLoadUILayoutHelper.LoadLayout(CommonUtil.LayoutFileFund, dockManagerFund);
        }

        private void Window_Closing()
        {
            SaveAndLoadUILayoutHelper.SaveLayout(CommonUtil.LayoutFileFund, dockManagerFund);
        }

        public int Order
        {
            get { return CommonUtil.VIEW_ORDER_FUND; }
        }
    }
}
