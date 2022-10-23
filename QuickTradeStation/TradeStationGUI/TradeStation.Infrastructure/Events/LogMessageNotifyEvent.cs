using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.PubSubEvents;

namespace TradeStation.Infrastructure.Events
{

    public enum LogMessageLevel
    {
        INFO = 0,
        WARN = 1,
        ERROR = 2
    }
    public class LogMessageEntity
    {
        public LogMessageLevel LogLevel { get; set; }
        public string Message { get; set; }
    }
    public class LogMessageNotifyEvent : PubSubEvent<LogMessageEntity>
    {
    }
}
