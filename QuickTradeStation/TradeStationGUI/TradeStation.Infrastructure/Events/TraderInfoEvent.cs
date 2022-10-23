using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.PubSubEvents;


namespace TradeStation.Infrastructure.Events
{
    public class TraderLoginStatusEntity
    {
        public string TraderID;
        public bool LoginStatus;
        public string Message;
    }

    public class TraderLoginStatusNotifyEvent : PubSubEvent<TraderLoginStatusEntity>
    {

    }
}
