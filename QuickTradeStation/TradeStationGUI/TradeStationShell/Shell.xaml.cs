using System;
using System.ComponentModel.Composition;
using System.Windows;

using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Events;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Services;

namespace TradeStationShell
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class Shell : IPartImportsSatisfiedNotification
    {
        [Import]
        private DialogService DialogService { get; set; }

        [Import]
        private MarketDataService MarketDataService { get; set; }

        [Import]
        public ShellViewModel ViewModel
        {
            set
            {
                DataContext = value;
            }
            get
            {
                return DataContext as ShellViewModel;
            }
        }

        [Import]
        public LogUtils Logger { get; set; }

        [Import]
        public UserSettings Us { get; set; }

        public Shell()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = DialogService.ShowConfirmation("退出确认", "是否退出程序?");

            if (result)
            {
                // Executes the shutdown command to trigger each handlers in each module.
                if (HostCommands.ShutdownCommand.CanExecute(e))
                {
                    HostCommands.ShutdownCommand.Execute(e);
                }

                e.Cancel = false;

                // Saves the user settings.
                CommonUtil.SaveUserSettings(Us);
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Executes the startup command to trigger each handlers in each module.
            if (HostCommands.StartupCommand.CanExecute(e))
            {
                HostCommands.StartupCommand.Execute(e);
            }
        }

        // 摘要: 
        //     在满足部件的导入并可安全使用时调用。
        public void OnImportsSatisfied()
        {
            //CommonUtil.LoadUserSettings(Us);
        }
    }
}
