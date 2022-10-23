using System.Collections;
using System.Windows;
using System.Windows.Media;

namespace TradeStation.Infrastructure
{
    /// <summary>
    /// winDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WinDialog
    {
        public WinDialog(IEnumerable itemsSource, ImageSource icon, MessageBoxButton button = MessageBoxButton.YesNo)
        {
            InitializeComponent();

            switch(button)
            {
                case MessageBoxButton.YesNo:
                    {
                        btOK.Visibility = Visibility.Collapsed;
                        break;
                    }
                case MessageBoxButton.OK:
                    {
                        btYes.Visibility = btNo.Visibility = Visibility.Collapsed;
                        break;
                    }
            }

            Icon = icon;
            xgGrid.ItemsSource = itemsSource;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (System.Exception)
            {
                Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
            }
            catch (System.Exception)
            {
                Close();
            }
        }
    }

}
