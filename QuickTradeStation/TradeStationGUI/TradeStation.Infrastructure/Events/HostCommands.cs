using Microsoft.Practices.Prism.Commands;

namespace TradeStation.Infrastructure.Events
{
    public static class HostCommands
    {
        #region Startup Command

        private static readonly CompositeCommand Startup = new CompositeCommand();

        public static CompositeCommand StartupCommand
        {
            get { return Startup; }
        }

        #endregion

        #region Shutdown Command

        private static readonly CompositeCommand Shutdown = new CompositeCommand();

        public static CompositeCommand ShutdownCommand
        {
            get { return Shutdown; }
        }

        #endregion
    }
}
