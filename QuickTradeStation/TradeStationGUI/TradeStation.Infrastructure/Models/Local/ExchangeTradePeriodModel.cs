using System;
using System.Collections.Generic;

namespace TradeStation.Infrastructure.Models.Local
{
    public class ExchangeTradePeriodModel
    {
        public string ExID { get; set; }

        public string VarietyType { get; set; }

        public IList<MarketPeriodRangeModel> PeriodRanges { get; set; }

        public int MarketTag { get; set; }
    }

    public class MarketPeriodRangeModel
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan Interval
        {
            get
            {
                if (StartTime <= EndTime)
                {
                    return EndTime - StartTime;
                }
                else
                {
                    return new TimeSpan(0, 0, 0);
                }
            }
        }
    }

    public class KLinePeriod
    {
        // Index is built by the KLine minuts
        public long KLineIndex { get; set; }

        // The range of this K line item
        public MarketPeriodRangeModel Period { get; set; }
    }
}
