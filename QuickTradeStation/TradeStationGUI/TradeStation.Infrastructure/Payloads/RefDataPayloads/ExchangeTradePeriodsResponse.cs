using System.Collections.Generic;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.Models;

namespace TradeStation.Infrastructure.Payloads.RefDataPayloads
{
    [DataContract]
    public class ExchangeTradePeriodsResponse : JsonResponse
    {
        [DataMember(Name = "value")]
        public IList<ExchangeTradePeriod> ExchangeTradePeriods { get; set; }
    }
}
