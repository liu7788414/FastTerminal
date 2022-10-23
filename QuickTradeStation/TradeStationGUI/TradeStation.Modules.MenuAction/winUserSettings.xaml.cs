using System.Windows;
using TradeStation.Infrastructure.Models;

namespace TradeStation.Modules.MenuAction
{
    /// <summary>
    /// winUserSettings.xaml 的交互逻辑
    /// </summary>
    public partial class WinUserSettings
    {
        public WinUserSettings(UserSettings us)
        {
            InitializeComponent();

            pgUserSettings.SelectedObject = us;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
    }
}
