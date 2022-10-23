using System.Windows;
using System.ComponentModel.Composition;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    [Export]
    public partial class QryPanel
    {
        public QryPanel()
        {
            InitializeComponent();
        }

        [Import]
        public QryHistoricalEntrustViewModel Model
        {
            get
            {
                return DataContext as QryHistoricalEntrustViewModel;
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
