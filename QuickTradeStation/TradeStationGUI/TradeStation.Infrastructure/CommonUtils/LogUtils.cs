using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using log4net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace TradeStation.Infrastructure.CommonUtils
{
    [Export(typeof(LogUtils))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LogUtils : ILoggerFacade
    {
        #region ILoggerFacade Members

        private static readonly ILog LogError = LogManager.GetLogger("logError");
        private static readonly ILog LogDebug = LogManager.GetLogger("logDebug");
        private static readonly ILog LogInfo = LogManager.GetLogger("logInfo");

        public void Log(string message, Category category, Priority priority)
        {
            log4net.Config.XmlConfigurator.Configure();
            switch (category)
            {
                case Category.Debug:
                    LogDebug.Debug(message);
                    break;
                case Category.Warn:
                    LogDebug.Warn(message);
                    break;
                case Category.Exception:
                    LogDebug.Error(message);
                    LogError.Error(message);
                    break;
                case Category.Info:
                    LogDebug.Info(message);
                    LogInfo.Info(message);
                    break;
            }
        }

        #endregion

        public void Debug(string message) { LogDebug.Debug(message); }

        public void Warn(string message) { LogDebug.Warn(message); }

        public void Info(string message) { LogDebug.Info(message); LogInfo.Info(message); }

        public void Error(string message, Exception ex = null) { LogDebug.Error(message); LogError.Error(message, ex); }

        public void Fatal(string message) { LogDebug.Fatal(message); }
    }
}
