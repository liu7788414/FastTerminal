using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Infrastructure.Models
{
    [DataContract]
    public class ExchangeTradePeriod
    {
        [DataMember(Name = "exchId")]
        public string ExID { get; set; }

        [DataMember(Name = "varidtyType")]
        public string VarietyType { get; set; }

        [DataMember(Name = "tradeTimes")]
        public IList<MarketPeriodRange> PeriodRanges { get; set; }

        [DataMember(Name = "marketTag")]
        public int MarketTag { get; set; }
    }

    [DataContract]
    public class MarketPeriodRange
    {
        [DataMember(Name = "startTime")]
        public long StartTime { get; set; }

        [DataMember(Name = "endTime")]
        public long EndTime { get; set; }
    }
}
