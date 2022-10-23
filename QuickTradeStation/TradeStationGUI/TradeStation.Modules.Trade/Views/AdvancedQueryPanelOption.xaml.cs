using System.Windows;
using System.ComponentModel.Composition;
using TradeStation.Modules.Trade.ViewModels;

namespace TradeStation.Modules.Trade.Views
{
    [Export]
    public partial class AdvancedQueryPanelOption
    {
        public AdvancedQueryPanelOption()
        {
            InitializeComponent();
        }

        [Import]
        public AdvancedQueryViewModelOption Model
        {
            get
            {
                return DataContext as AdvancedQueryViewModelOption;
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
