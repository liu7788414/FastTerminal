using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.PubSubEvents;

using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Events
{
    public class SelectDisplayStockQuotationEvent : PubSubEvent<SecurityInfo>
    {

    }

    public class SelectDisplayFutureQuotationEvent : PubSubEvent<SecurityInfo>
    {

    }

    public class SelectDisplayOptionQuotationEvent : PubSubEvent<SecurityInfo>
    {

    }

    public class OptionInfoModelSelectedEvent : PubSubEvent<OptionInfoModel>
    {

    }

    public class OptionInfoReadyEvent : PubSubEvent<string>
    {

    }
}
