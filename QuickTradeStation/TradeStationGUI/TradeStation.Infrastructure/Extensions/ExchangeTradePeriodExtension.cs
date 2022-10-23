using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeStation.Infrastructure.Models;
using TradeStation.Infrastructure.Models.Local;

namespace TradeStation.Infrastructure.Extensions
{
    public static class ExchangeTradePeriodExtension
    {
        public static ExchangeTradePeriodModel ToExchangeTradePeriodModel(this ExchangeTradePeriod model)
        {
            if (null == model)
            {
                return null;
            }

            return new ExchangeTradePeriodModel()
            {
                ExID = model.ExID,
                VarietyType = model.VarietyType,
                MarketTag = model.MarketTag,

                PeriodRanges = model.PeriodRanges.Select(x => new MarketPeriodRangeModel()
                {
                    StartTime = DateTimeHelper.ConvertToDateTime(x.StartTime * 1000),
                    EndTime = DateTimeHelper.ConvertToDateTime(x.EndTime * 1000),
                }).ToList(),
            };
        }
    }
}
