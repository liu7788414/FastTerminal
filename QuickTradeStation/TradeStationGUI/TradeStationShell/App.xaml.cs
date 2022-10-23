using System;
using System.Windows;

using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

using TradeStation.Infrastructure.CommonUtils;

namespace TradeStationShell
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public LogUtils StartupLogger { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StartupLogger = new LogUtils();
            //this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

#if (DEBUG)
            //RunInDebugMode();
            RunInReleaseMode();
#else
            RunInReleaseMode();
#endif
        }

        private void RunInReleaseMode()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            try
            {
                var bootstrapper = new TradeStationBootstrapper();
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private void HandleException(Exception ex)
        {
            if (ex == null)
                return;

            StartupLogger.Error("发生系统未处理异常: " + ex.Message, ex);

            ExceptionPolicy.HandleException(ex, "Default Policy");
            MessageBox.Show(TradeStationShell.Properties.Resources.UnhandledException);
            Environment.Exit(1);
        }

    }

}
