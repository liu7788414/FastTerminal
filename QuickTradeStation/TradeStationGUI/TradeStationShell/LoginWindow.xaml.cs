using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using TradeStation.Infrastructure.CommonUtils;

namespace TradeStationShell
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class LoginWindow
    {
        [Import]
        public LogUtils Logger { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
            Closing += OnLoginClose;
        }

        private void OnLoginClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsAuth) return;
            e.Cancel = false;
            Environment.Exit(0);
        }

        public bool IsAuth = false;

        [Import]
        public LoginViewModel Model
        {
            get
            {
                return DataContext as LoginViewModel;
            }
            set
            {
                DataContext = value;
                var vm = DataContext as LoginViewModel;
                if (vm != null)
                {
                    vm.TheWindow = this;
                }
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Height = 545;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Height = 295; 
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PbPassword.Clear();
        }

        private void cbLockContentPane_Unchecked(object sender, RoutedEventArgs e)
        {
            Model.UserSettings.IsQuotesOnly = false;
        }

        private void cbLockContentPane_Checked(object sender, RoutedEventArgs e)
        {
            Model.UserSettings.IsQuotesOnly = true;
        }
    }
}
