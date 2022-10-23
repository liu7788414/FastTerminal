using System.Windows;
using System.ComponentModel.Composition;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    [Export]
    public partial class QryHistoricalTradeResultPanel
    {
        public QryHistoricalTradeResultPanel()
        {
            InitializeComponent();
        }

        [Import]
        public QryHistoricalTradeResultViewModel Model
        {
            get
            {
                return DataContext as QryHistoricalTradeResultViewModel; 
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
        }
    }
}
