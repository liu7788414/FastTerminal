using System.ComponentModel.Composition;
using System.Windows;
using TradeStation.Infrastructure.Behaviors;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    [Export]
    public partial class QryEtfStockListWindow
    {
        public QryEtfStockListWindow()
        {
            InitializeComponent();
        }

        [Import]
        public QryEtfStockListViewModel Model
        {
            get
            {
                return DataContext as QryEtfStockListViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            ((QryEtfStockListViewModel)DataContext).AddStocks("1");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        } 
    }
}
